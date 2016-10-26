using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    public class MockFieldCollection : IFieldCollection
    {
        private readonly IDictionary<string, IField> _values;

        public MockFieldCollection(IDictionary<string, IField> values)
        {
            _values = values;
        }

        public MockFieldCollection(ICollection<IField> fields)
            : this(fields.ToDictionary(k => k.Name, e => e, StringComparer.OrdinalIgnoreCase))
        {

        }

        public IEnumerator<IField> GetEnumerator()
        {
            return _values.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(string fieldName)
        {
            return _values.ContainsKey(fieldName);
        }

        public IField this[string name] => _values[name];

        public int Count => _values.Count;
    }
}
