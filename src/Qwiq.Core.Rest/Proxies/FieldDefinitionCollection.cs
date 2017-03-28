using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest.Proxies
{
    internal class FieldDefinitionCollection : Qwiq.FieldDefinitionCollection
    {
        private readonly Dictionary<string, IFieldDefinition> _fieldUsagesByName;

        private readonly Dictionary<string, IFieldDefinition> _fieldUsagesByReferenceName;

        internal FieldDefinitionCollection(IEnumerable<WorkItemTypeFieldInstance> typeFields)
            : this(typeFields.Where(p => p != null).Select(s => s.Field))
        {
        }

        internal FieldDefinitionCollection(IEnumerable<WorkItemFieldReference> typeFields)
            : this(typeFields.Where(p=>p != null).Select(s => new FieldDefinition(s)))
        {
        }

        private FieldDefinitionCollection(IEnumerable<IFieldDefinition> fieldDefinitions)
        {
            if (fieldDefinitions == null) throw new ArgumentNullException(nameof(fieldDefinitions));

            _fieldUsagesByName = fieldDefinitions.ToDictionary(k => k.Name, e => e, StringComparer.OrdinalIgnoreCase);
            _fieldUsagesByReferenceName = fieldDefinitions.ToDictionary(
                k => k.ReferenceName,
                e => e,
                StringComparer.OrdinalIgnoreCase);
        }

        public override int Count => _fieldUsagesByName.Count;

        public override IFieldDefinition this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentException("Value cannot be null or empty.", nameof(name));

                if (!_fieldUsagesByName.TryGetValue(name, out IFieldDefinition def))
                    if (!_fieldUsagesByReferenceName.TryGetValue(name, out def))
                        throw new FieldDefinitionNotExistException(name);

                return def;
            }
        }

        public override bool Contains(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(fieldName));
            return _fieldUsagesByName.ContainsKey(fieldName) || _fieldUsagesByReferenceName.ContainsKey(fieldName);
        }

        public override IEnumerator<IFieldDefinition> GetEnumerator()
        {
            return _fieldUsagesByName.Values.GetEnumerator();
        }
    }
}