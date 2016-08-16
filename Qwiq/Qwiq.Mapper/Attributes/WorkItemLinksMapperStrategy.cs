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
            var linkProperties = _inspector.GetAnnotatedProperties(targetWorkItemType, typeof(WorkItemLinkAttribute));
            foreach (var property in linkProperties)
            {
                var def = _inspector.GetAttribute<WorkItemLinkAttribute>(property);
                if (def != null)
                {
                    var linkType = def.GetLinkName();

                    var ids = sourceWorkItem.Links.OfType<IRelatedLink>()
                            .Where(wil => wil.LinkTypeEnd.ImmutableName == linkType)
                            .Select(wil => wil.RelatedWorkItemId).ToArray();

                    if (ids.Any())
                    {
                        var propertyType = def.GetWorkItemType();
                        var workItems = _store.Query(ids).ToList();
                        IList results = (IList)typeof(List<>).MakeGenericType(propertyType).GetConstructor(new[] { typeof(int) }).Invoke(new object[] { workItems.Count });

                        var createdItems = workItemMapper.Create(propertyType, workItems);
                        foreach (var item in createdItems)
                        {
                            results.Add(item);
                        }

                        property.SetValue(targetWorkItem, results);
                    }
                }
            }
        }

        public override void Map(Type targetWorkItemType, IEnumerable<KeyValuePair<IWorkItem, object>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            var accessor = TypeAccessor.Create(targetWorkItemType, true);
            var lookup = new Dictionary<Tuple<int, string>, List<int>>();
            var lookup2 = workItemMappings.ToDictionary(k => k.Key.Id, e => e);

            // Aggregate up all the items that need to be loaded from VSO
            foreach (var workItemMapping in lookup2.Values)
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
                        var linkType = def.GetLinkName();

                        var ids = sourceWorkItem
                                    .Links
                                    .OfType<IRelatedLink>()
                                    .Where(wil => wil.LinkTypeEnd.ImmutableName == linkType)
                                    .Select(wil => wil.RelatedWorkItemId)
                                    .ToList();

                        if (!ids.Any()) continue;
                        var key = new Tuple<int, string>(sourceWorkItem.Id, linkType);
                        lookup[key] = ids;
                    }
                }
            }

            // If there were no items added to the lookup, don't bother querying VSO
            if (!lookup.Any()) return;

            // Load all the items
            var workItems = _store
                                .Query(lookup.SelectMany(p => p.Value).Distinct())
                                .ToDictionary(k => k.Id, e => e);


            // Enumerate through items requiring a VSO lookup and map the objects
            foreach (var item in lookup)
            {
                var sourceWorkItem = lookup2[item.Key.Item1].Key;
                var targetWorkItem = lookup2[item.Key.Item1].Value;

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
                        var propertyType = def.GetWorkItemType();
                        var linkType = def.GetLinkName();
                        var key = new Tuple<int, string>(sourceWorkItem.Id, linkType);
                        List<int> ids;

                        if (!lookup.TryGetValue(key, out ids))
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
                             .Where(p=> p != null)
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