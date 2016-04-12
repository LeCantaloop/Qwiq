using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.Qwiq;

namespace Qwiq.Identity.Tests.Mocks
{
    public class MockFieldCollection : IFieldCollection
    {
        private readonly ICollection<IField> _mockFields;

        public MockFieldCollection(IEnumerable<IField> mockFields)
        {
            _mockFields = mockFields.ToList();
        }

        public IEnumerator GetEnumerator()
        {
            return _mockFields.GetEnumerator();
        }

        public bool Contains(string fieldName)
        {
            return _mockFields.Any(f => f.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        }

        public IField this[string name]
        {
            get { return _mockFields.First(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)); }
        }

        public int Count
        {
            get { return _mockFields.Count; }
        }
    }
}