using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.IE.Qwiq.Relatives.Tests.Mocks
{
    public class MockFieldCollection : IFieldCollection
    {
        private readonly ICollection<IField> _fields;

        public MockFieldCollection(ICollection<IField> fields)
        {
            _fields = fields;
        }

        public bool Contains(string fieldName)
        {
            return _fields.Any(f => f.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        }

        public IField this[string name]
        {
            get { return _fields.Single(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)); }
        }

        public int Count => _fields.Count;
        public IEnumerator<IField> GetEnumerator()
        {
            return _fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}