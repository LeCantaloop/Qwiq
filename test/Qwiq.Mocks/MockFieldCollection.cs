using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Mocks
{
    public class MockFieldCollection : IFieldCollection
    {
        private readonly IDictionary<int, IField> _cache;

        private readonly IFieldDefinitionCollection _definitions;

        public MockFieldCollection(IFieldDefinitionCollection definitions)
        {
            _definitions = definitions;
            _cache = new Dictionary<int, IField>();
        }

        public int Count => _definitions.Count;

        public IField this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                return GetById(_definitions[name].Id);
            }
        }

        public bool Contains(string name)
        {
            try
            {
                return _definitions.Contains(name);
            }
            // REVIEW: Catch a more specific exception
            catch (Exception)
            {
                return false;
            }
        }

        public IField GetById(int id)
        {
            if (!TryGetById(id, out IField byId))
            {
                throw new ArgumentException($"Field {id} does not exist.", nameof(id));
            }
            return byId;
        }

        public IEnumerator<IField> GetEnumerator()
        {
            return _cache.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetById(int id, out IField field)
        {
            if (_cache.TryGetValue(id, out field)) return true;
            try
            {
                if (_definitions.TryGetById(id, out IFieldDefinition def))
                {
                    field = new MockField(def);
                    _cache[id] = field;
                    return true;
                }
            }
            // REVIEW: Catch a more specific exception
            catch (Exception)
            {
            }
            return false;
        }
    }
}