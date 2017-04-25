using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FastMember;

using Microsoft.Qwiq.Identity;
using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Mapper
{
    /// <summary>
    /// Class BulkIdentityAwareAttributeMapperStrategy.
    /// </summary>
    /// <seealso cref="WorkItemMapperStrategyBase" />
    public class BulkIdentityAwareAttributeMapperStrategy : WorkItemMapperStrategyBase
    {
        private readonly IPropertyInspector _inspector;
        private readonly IIdentityManagementService _identityManagementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkIdentityAwareAttributeMapperStrategy"/> class.
        /// </summary>
        /// <param name="inspector">The inspector.</param>
        /// <param name="identityManagementService">The identity management service.</param>
        public BulkIdentityAwareAttributeMapperStrategy(IPropertyInspector inspector, IIdentityManagementService identityManagementService)
        {
            _inspector = inspector;
            _identityManagementService = identityManagementService;
        }

        /// <summary>
        /// Maps the specified targe work item type.
        /// </summary>
        /// <param name="targeWorkItemType">Type of the targe work item.</param>
        /// <param name="workItemMappings">The work item mappings.</param>
        /// <param name="workItemMapper">The work item mapper.</param>
        public override void Map(Type targeWorkItemType, IEnumerable<KeyValuePair<IWorkItem, IIdentifiable<int?>>> workItemMappings, IWorkItemMapper workItemMapper)
        {
            var workingSet = workItemMappings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, WorkItemComparer.Default);
            if (!workingSet.Any()) return;

            var validIdentityProperties = GetWorkItemIdentityFieldNameToIdentityPropertyMap(targeWorkItemType, _inspector);
            if (!validIdentityProperties.Any())return;

            var accessor = TypeAccessor.Create(targeWorkItemType, true);
            var validIdentityFieldsWithWorkItems = GetWorkItemsWithIdentityFieldValues(workingSet.Keys, validIdentityProperties.Keys.ToList());
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

        private static ICollection<WorkItemWithFields> GetWorkItemsWithIdentityFieldValues(
            IEnumerable<IWorkItem> workItems,
            System.Collections.Generic.IReadOnlyCollection<string> witFieldNames)
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

