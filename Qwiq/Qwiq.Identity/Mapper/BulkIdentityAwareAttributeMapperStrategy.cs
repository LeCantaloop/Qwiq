using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Qwiq.Identity.Attributes;
using Microsoft.Qwiq.Mapper;
using Microsoft.Qwiq.Mapper.Attributes;

using FastMember;

namespace Microsoft.Qwiq.Identity.Mapper
{
    public class BulkIdentityAwareAttributeMapperStrategy : WorkItemMapperStrategyBase
    {
        private readonly IPropertyInspector _inspector;
        private readonly IIdentityManagementService _identityManagementService;

        public BulkIdentityAwareAttributeMapperStrategy(IPropertyInspector inspector, IIdentityManagementService identityManagementService)
        {
            _inspector = inspector;
            _identityManagementService = identityManagementService;
        }

        public override void Map(Type targeWorkItemType, IEnumerable<KeyValuePair<IWorkItem, IIdentifiable>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            var workingSet = workItemMappings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, new WorkItemKeyComparer());

            if (!workingSet.Any())
            {
                return;
            }

            var validIdentityProperties = GetWorkItemIdentityFieldNameToIdentityPropertyMap(targeWorkItemType, _inspector);

            if (!validIdentityProperties.Any())
            {
                return;
            }
            var accessor = TypeAccessor.Create(targeWorkItemType, true);
            var validIdentityFieldsWithWorkItems = GetWorkItemsWithIdentityFieldValues(workingSet.Keys.ToList(), validIdentityProperties.Keys.ToList());
            var identitySearchTerms = GetIdentitySearchTerms(validIdentityFieldsWithWorkItems).ToList();
            var identitySearchResults = GetIdentityMap(_identityManagementService, identitySearchTerms);

            foreach (var workItem in validIdentityFieldsWithWorkItems)
            {
                var targetObject = workingSet[workItem.WorkItem];
                foreach (var sourceField in workItem.ValidFields)
                {
                    var targetProperty = validIdentityProperties[sourceField.Name];
                    var mappedValue = identitySearchResults[sourceField.Value];
                    if (!string.IsNullOrEmpty(mappedValue))
                    {
                        accessor[targetObject, targetProperty.Name] = mappedValue;
                    }
                }
            }
        }

        internal static IDictionary<string, string> GetIdentityMap(IIdentityManagementService ims, ICollection<string> searchTerms)
        {
            return ims.GetAliasesForDisplayNames(searchTerms.ToArray()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.FirstOrDefault());
        }

        internal static ICollection<WorkItemWithFields> GetWorkItemsWithIdentityFieldValues(IReadOnlyCollection<IWorkItem> workItems, IReadOnlyCollection<string> witFieldNames)
        {
            return
                workItems.Select(
                    wi =>
                        new WorkItemWithFields
                        {
                            WorkItem = wi,
                            ValidFields =
                                witFieldNames
                                    .Where(fn => wi.Fields.Contains(fn))
                                    .Select(fn =>
                                                new WorkItemField
                                                {
                                                    Name = fn,
                                                    Value = wi[fn] as string
                                                })
                                    .Where(f => !string.IsNullOrEmpty(f.Value))
                        }).ToList();
        }

        internal static IEnumerable<string> GetIdentitySearchTerms(ICollection<WorkItemWithFields> workItemsWithIdentityFields)
        {
            return workItemsWithIdentityFields.SelectMany(wiwf => wiwf.ValidFields.Select(f => f.Value)).Distinct();
        }

        internal static IDictionary<string, PropertyInfo> GetWorkItemIdentityFieldNameToIdentityPropertyMap(Type targetWorkItemType, IPropertyInspector propertyInspector)
        {
            var identityProperties = propertyInspector.GetAnnotatedProperties(targetWorkItemType, typeof(IdentityFieldAttribute));
            return
                identityProperties
                .Select(
                    p =>
                        new
                        {
                            IdentityProperty = p,
                            WitFieldName = propertyInspector.GetAttribute<FieldDefinitionAttribute>(p)?.FieldName
                        })
                .Where(p => !string.IsNullOrEmpty(p.WitFieldName) && p.IdentityProperty.CanWrite)
                .ToDictionary(x => x.WitFieldName, x => x.IdentityProperty);
        }
    }
}

