using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using FastMember;

namespace Microsoft.IE.Qwiq.Mapper.Attributes
{
    public class WorkItemLinksMapperStrategy : IndividualWorkItemMapperBase
    {
        private readonly IPropertyInspector _inspector;
        private readonly IWorkItemStore _store;
        private static readonly ConcurrentDictionary<PropertyInfo, WorkItemLinkAttribute> PropertyInfoFields = new ConcurrentDictionary<PropertyInfo, WorkItemLinkAttribute>();
        private static readonly ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, List<PropertyInfo>> PropertiesThatExistOnWorkItem = new ConcurrentDictionary<Tuple<string, RuntimeTypeHandle>, List<PropertyInfo>>();


        public WorkItemLinksMapperStrategy(IPropertyInspector inspector, IWorkItemStore store)
        {
            _inspector = inspector;
            _store = store;
        }

        private static WorkItemLinkAttribute PropertyInfoLinkTypeCache(IPropertyInspector inspector, PropertyInfo property)
        {
            return PropertyInfoFields.GetOrAdd(
                property,
                info => inspector.GetAttribute<WorkItemLinkAttribute>(property));
        }

        private static IEnumerable<PropertyInfo> PropertiesOnWorkItemCache(IPropertyInspector inspector, IWorkItem workItem, Type targetType, Type attributeType)
        {
            // Composite key: work item type and target type

            var workItemType = workItem.Type.Name;
            var key = new Tuple<string, RuntimeTypeHandle>(workItemType, targetType.TypeHandle);

            return PropertiesThatExistOnWorkItem.GetOrAdd(
                key,
                tuple => inspector.GetAnnotatedProperties(targetType, attributeType).ToList());
        }

        protected override void Map(Type targetWorkItemType, IWorkItem sourceWorkItem, object targetWorkItem, IWorkItemMapper workItemMapper)
        {
            Map(
                targetWorkItemType,
                new[] { new KeyValuePair<IWorkItem, object>(sourceWorkItem, targetWorkItem) },
                workItemMapper);
        }

        private class TreeNode
        {

        }

        public override void Map(Type targetWorkItemType, IEnumerable<KeyValuePair<IWorkItem, object>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            var accessor = TypeAccessor.Create(targetWorkItemType, true);
            var linksLookup = new Dictionary<Tuple<int, string>, List<int>>();
            var idToMapTargetLookup = workItemMappings.ToDictionary(k => k.Key.Id, e => e);

            // Aggregate up all the items that need to be loaded from VSO
            foreach (var workItemMapping in idToMapTargetLookup.Values)
            {
                var sourceWorkItem = workItemMapping.Key;

                if (sourceWorkItem.Links == null) continue;

                foreach (
                    var property in
                        PropertiesOnWorkItemCache(
                            _inspector,
                            sourceWorkItem,
                            targetWorkItemType,
                            typeof(WorkItemLinkAttribute)))
                {
                    var def = PropertyInfoLinkTypeCache(_inspector, property);
                    if (def != null)
                    {
                        var linkType = def.LinkName;

                        var ids = sourceWorkItem
                                    .Links
                                    .OfType<IRelatedLink>()
                                    .Where(wil => wil.LinkTypeEnd.ImmutableName == linkType)
                                    .Select(wil => wil.RelatedWorkItemId)
                                    .ToList();

                        if (!ids.Any()) continue;
                        var key = new Tuple<int, string>(sourceWorkItem.Id, linkType);
                        linksLookup[key] = ids;
                    }
                }
            }

            // If there were no items added to the lookup, don't bother querying VSO
            if (!linksLookup.Any()) return;

            // Load all the items
            var workItems = _store
                                .Query(linksLookup.SelectMany(p => p.Value).Distinct())
                                .ToDictionary(k => k.Id, e => e);

#if DEBUG
            var instance = Guid.NewGuid().ToString("N");
#endif

            // Enumerate through items requiring a VSO lookup and map the objects
            foreach (var item in linksLookup)
            {
                var sourceWorkItem = idToMapTargetLookup[item.Key.Item1].Key;
                var targetWorkItem = idToMapTargetLookup[item.Key.Item1].Value;

#if DEBUG && TRACE
                Trace.TraceInformation("{0} ({1}): Mapping {2}", GetType().Name, instance, sourceWorkItem.Id);
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
                        List<int> ids;

                        if (!linksLookup.TryGetValue(key, out ids))
                        {
                            // Could not find any IDs for the given ID/LinkType
                            continue;
                        }

                        var wi = ids
                            .Select(
                            s =>
                                {
                                    IWorkItem val;
                                    workItems.TryGetValue(s, out val);
                                    return val;
                                })
                                .Where(p => p != null)
                                .ToList();

                        // ReSharper disable SuggestVarOrType_SimpleTypes
                        IList results = (IList)typeof(List<>)
                                                   // ReSharper restore SuggestVarOrType_SimpleTypes
                                                   .MakeGenericType(propertyType)
                                                   .GetConstructor(new[] { typeof(int) })
                                                   .Invoke(new object[] { wi.Count });

                        var createdItems = workItemMapper.Create(propertyType, wi);
                        foreach (var createdItem in createdItems)
                        {
                            results.Add(createdItem);
                        }
                        accessor[targetWorkItem, property.Name] = results;
                    }
                }
            }
        }
    }
}