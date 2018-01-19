using FastMember;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Microsoft.Qwiq.Mapper.Attributes
{
    public class WorkItemLinksMapperStrategy : WorkItemMapperStrategyBase
    {
        private static readonly ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, List<PropertyInfo>>
            PropertiesThatExistOnWorkItem =
                new ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, List<PropertyInfo>>();

        private static readonly ConcurrentDictionary<PropertyInfo, WorkItemLinkAttribute> PropertyInfoFields =
            new ConcurrentDictionary<PropertyInfo, WorkItemLinkAttribute>();

        private readonly IPropertyInspector _inspector;

        public WorkItemLinksMapperStrategy(IPropertyInspector inspector, IWorkItemStore store)
        {
            _inspector = inspector;
            Store = store;
        }

        protected IWorkItemStore Store { get; }

        public override void Map(Type targetWorkItemType, IDictionary<IWorkItem, IIdentifiable<int?>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            var linksLookup = BuildLinksRelationships(targetWorkItemType, workItemMappings);

            // REVIEW: We don't have any cycle detection, this avoids causing stack overflows in those cases
            workItemMapper = new WorkItemMapper(workItemMapper.MapperStrategies.Except(new[] { this }));

            // If there were no items added to the lookup, don't bother querying VSO
            if (!linksLookup.Any()) return;

            // Load all the items
            var workItems = Store.Query(linksLookup.SelectMany(p => p.Value).Distinct())
                                  .ToDictionary(k => k.Id, e => e);

            var idToMapTargetLookup = workItemMappings.ToDictionary(k => k.Key.Id, e => e);
            var accessor = TypeAccessor.Create(targetWorkItemType, true);

            // REVIEW: The recursion of links can cause mapping multiple times on the same values
            // For example, a common ancestor
            var previouslyMapped = new Dictionary<Tuple<int, RuntimeTypeHandle>, IIdentifiable<int?>>();

            // Enumerate through items requiring a VSO lookup and map the objects
            // There are n-passes to map, where n=number of link types
            foreach (var item in linksLookup)
            {
                var sourceWorkItem = idToMapTargetLookup[item.Key.Item1].Key;
                var targetWorkItem = idToMapTargetLookup[item.Key.Item1].Value;

#if DEBUG && TRACE
                Trace.TraceInformation("{0}: Mapping {1} on {2}", GetType().Name, item.Key.Item2, sourceWorkItem.Id);
#endif

                foreach (var property in
                    PropertiesOnWorkItemCache(
                        _inspector,
                        sourceWorkItem,
                        targetWorkItemType,
                        typeof(WorkItemLinkAttribute)))
                {
                    var def = PropertyInfoLinkTypeCache(_inspector, property);
                    if (def != null)
                    {
                        var propertyType = def.WorkItemType;
                        var linkType = def.LinkName;
                        var key = new Tuple<int, string>(sourceWorkItem.Id, linkType);

                        if (!linksLookup.TryGetValue(key, out List<int> linkIds))
                        {
                            // Could not find any IDs for the given ID/LinkType
                            continue;
                        }

                        var wi = linkIds
                            // Only get the new items that need to be mapped
                            .Where(p => !previouslyMapped.ContainsKey(new Tuple<int, RuntimeTypeHandle>(p, propertyType.TypeHandle)))
                            .Select(
                            s =>
                                {
                                    workItems.TryGetValue(s, out IWorkItem val);
                                    return val;
                                }).Where(p => p != null)
                                .ToList();

                        var createdItems = workItemMapper.Create(propertyType, wi).ToList();

                        // Add the newly created items to the cache
                        // This allows for lazy creation of common parents
                        foreach (var createdItem in createdItems)
                        {
                            if (!createdItem.Id.HasValue) continue;
                            var key2 = new Tuple<int, RuntimeTypeHandle>(createdItem.Id.Value, propertyType.TypeHandle);
                            previouslyMapped[key2] = createdItem;
                        }

                        var existing = linkIds
                            // Only get the new items that need to be mapped
                            .Where(
                                p =>
                                previouslyMapped.ContainsKey(
                                    new Tuple<int, RuntimeTypeHandle>(p, propertyType.TypeHandle)))
                            .Select(s => previouslyMapped[new Tuple<int, RuntimeTypeHandle>(s, propertyType.TypeHandle)]);

                        var allItems = createdItems.Union(existing).ToList();

                        // REVIEW: These steps are required as the type defined for the link may be different than targetWorkItemType
                        // ReSharper disable SuggestVarOrType_SimpleTypes
                        IList results = (IList)typeof(List<>)
                                                   // ReSharper restore SuggestVarOrType_SimpleTypes
                                                   .MakeGenericType(propertyType)
                                                   .GetConstructor(new[] { typeof(int) })
                                                   .Invoke(new object[] { allItems.Count });
                        foreach (var link in allItems)
                        {
                            results.Add(link);
                        }

                        accessor[targetWorkItem, property.Name] = results;
                    }
                }
            }
        }

        protected virtual IEnumerable<IWorkItem> Query(IEnumerable<int> ids)
        {
            return Store.Query(ids);
        }

        private static IEnumerable<PropertyInfo> PropertiesOnWorkItemCache(
            IPropertyInspector inspector,
            IWorkItem workItem,
            Type targetType,
            Type attributeType)
        {
            // Composite key: work item type and target type

            var workItemType = workItem.WorkItemType;
            var key = new Tuple<string, RuntimeTypeHandle>(workItemType, targetType.TypeHandle);

            return PropertiesThatExistOnWorkItem.GetOrAdd(
                key,
                tuple => inspector.GetAnnotatedProperties(targetType, attributeType).ToList());
        }

        private static WorkItemLinkAttribute PropertyInfoLinkTypeCache(
                                    IPropertyInspector inspector,
            PropertyInfo property)
        {
            return PropertyInfoFields.GetOrAdd(
                property,
                info => inspector.GetAttribute<WorkItemLinkAttribute>(property));
        }

        private Dictionary<Tuple<int, string>, List<int>> BuildLinksRelationships(
            Type targetWorkItemType,
            IEnumerable<KeyValuePair<IWorkItem, IIdentifiable<int?>>> workItemMappings)
        {
            var linksLookup = new Dictionary<Tuple<int, string>, List<int>>();

            // Aggregate up all the items that need to be loaded from VSO
            foreach (var workItemMapping in workItemMappings)
            {
                var sourceWorkItem = workItemMapping.Key;

                if (sourceWorkItem.Links == null) continue;

                foreach (var property in
                    PropertiesOnWorkItemCache(_inspector, sourceWorkItem, targetWorkItemType, typeof(WorkItemLinkAttribute)))
                {
                    var def = PropertyInfoLinkTypeCache(_inspector, property);
                    if (def != null)
                    {
                        var linkType = def.LinkName;

                        var ids =
                            sourceWorkItem.Links.OfType<IRelatedLink>()
                                          .Where(wil => wil.LinkTypeEnd.ImmutableName == linkType)
                                          .Select(wil => wil.RelatedWorkItemId)
                                          .ToList();

                        if (!ids.Any()) continue;
                        var key = new Tuple<int, string>(sourceWorkItem.Id, linkType);
                        if (linksLookup.ContainsKey(key))
                        {
                            var val = new HashSet<int>(linksLookup[key]);
                            foreach (var id in ids)
                            {
                                val.Add(id);
                            }
                            ids = val.ToList();
                        }
                        linksLookup[key] = ids;
                    }
                }
            }
            return linksLookup;
        }
    }
}