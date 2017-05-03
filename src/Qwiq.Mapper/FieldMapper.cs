using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.Qwiq.Linq;
using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Mapper
{
    public class FieldMapper : IFieldMapper
    {
        public IEnumerable<string> GetWorkItemType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var customAttributes = type.GetCustomAttributes(typeof(WorkItemTypeAttribute), true).Cast<WorkItemTypeAttribute>().ToList();
            return customAttributes.Select(ca => ca.GetTypeName()).OrderBy(name => name);
            // Order alphabetically so string comparisons work and we don't needlessly permute our queries
        }

        public IEnumerable<string> GetFieldNames(Type type)
        {
            var customAttributes =
                type.GetProperties()
                    .SelectMany(property => property.GetCustomAttributes(typeof(FieldDefinitionAttribute), true))
                    .Cast<FieldDefinitionAttribute>();
            var fieldNames = customAttributes.Select(attrib => attrib.FieldName);
            return fieldNames.OrderBy(name => name);
            // Order alphabetically so string comparisons work and we don't needlessly permute our queries
        }

        public string GetFieldName(Type type, string propertyName)
        {
            var customAttribute = GetFieldAttribute<FieldDefinitionAttribute>(type, propertyName);
            if (customAttribute == null)
            {
                throw new ArgumentException(
                    string.Format(
                        "No field definition found for property '{0}'. Querying on non-mapped fields is not allowed."
                        + " Either map the '{0}' property or remove it from the query.", propertyName), nameof(propertyName));
            }

            var fieldName = customAttribute.FieldName;
            return fieldName;
        }

        [CanBeNull]
        private static T GetFieldAttribute<T>([NotNull] Type type, [NotNull] string propertyName)
        {
            Contract.Requires(type != null);
            Contract.Requires(!string.IsNullOrEmpty(propertyName));

            var property = type.GetProperty(propertyName);

            var customAttributes = Enumerable.Empty<T>();
            if (property != null)
            {
                customAttributes =
                    property.GetCustomAttributes(typeof(T), true)
                        .Cast<T>()
                        .ToList();
            }
            return customAttributes.SingleOrDefault();
        }
    }
}

