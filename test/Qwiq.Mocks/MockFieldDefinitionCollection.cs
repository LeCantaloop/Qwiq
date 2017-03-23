using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Mocks
{
    public class MockFieldDefinitionCollection : IFieldDefinitionCollection
    {
        private readonly Dictionary<string, IFieldDefinition> _fieldUsagesByName;

        private readonly Dictionary<string, IFieldDefinition> _fieldUsagesByReferenceName;

        public MockFieldDefinitionCollection(IEnumerable<IFieldDefinition> fieldDefintions)
        {
            if (fieldDefintions == null) throw new ArgumentNullException(nameof(fieldDefintions));

            _fieldUsagesByName = fieldDefintions.ToDictionary(k => k.Name, e => e, StringComparer.OrdinalIgnoreCase);
            _fieldUsagesByReferenceName = fieldDefintions.ToDictionary(k => k.ReferenceName, e => e, StringComparer.OrdinalIgnoreCase);
        }

        public MockFieldDefinitionCollection(IWorkItemStore store, IWorkItemType type = null)
            :this(store.Projects.SelectMany(s => s.WorkItemTypes.Where(p=> (type == null) ? true : (p.Equals(type)))).SelectMany(s => s.FieldDefinitions))
        {
        }

        public IEnumerator<IFieldDefinition> GetEnumerator()
        {
            return _fieldUsagesByName.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _fieldUsagesByName.Count;

        public IFieldDefinition this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));

                if (!_fieldUsagesByName.TryGetValue(name, out IFieldDefinition def))
                {
                    if (!_fieldUsagesByReferenceName.TryGetValue(name, out def))
                    {
                        throw new FieldDefinitionNotExistException(name);
                    }
                }

                return def;
            }
        }

        public bool Contains(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentException("Value cannot be null or empty.", nameof(fieldName));
            return _fieldUsagesByName.ContainsKey(fieldName) || _fieldUsagesByReferenceName.ContainsKey(fieldName);
        }
    }
}