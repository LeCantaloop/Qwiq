using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    public class FieldCollection : IFieldCollection
    {
        private readonly IDictionary<int, IField> _cache;

        private readonly IFieldDefinitionCollection _definitions;

        private readonly Func<IRevisionInternal, IFieldDefinition, IField> _fieldFactory;

        private readonly IRevisionInternal _revision;

        internal FieldCollection(
            IRevisionInternal revision,
            IFieldDefinitionCollection definitions,
            Func<IRevisionInternal, IFieldDefinition, IField> fieldFactory)
        {
            _revision = revision;
            _definitions = definitions;
            _fieldFactory = fieldFactory;
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

        public IField this[int index]
        {
            get
            {
                if (index < 0 || index >= _definitions.Count) throw new ArgumentOutOfRangeException(nameof(index));
                return GetItem(index);
            }
        }

        public bool Contains(int id)
        {
            return _definitions.Contains(id);
        }

        public bool Contains(IField value)
        {
            return IndexOf(value) != -1;
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

        public virtual IField GetById(int id)
        {
            if (!TryGetById(id, out IField byId)) throw new DeniedOrNotExistException();
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

        public IField GetItem(int index)
        {
            var def = _definitions[index];
            return GetById(def.Id);
        }

        public virtual int IndexOf(IField value)
        {
            for (var i = 0; i < Count; i++) if (GenericComparer<IField>.Default.Equals(this[i], value)) return i;

            return -1;
        }

        public bool TryGetById(int id, out IField value)
        {
            if (_cache.TryGetValue(id, out value)) return true;
            try
            {
                if (_definitions.TryGetById(id, out IFieldDefinition def))
                {
                    value = _fieldFactory(_revision, def);
                    _cache[id] = value;
                    return true;
                }
            }
            // REVIEW: Catch a more specific exception
            catch (Exception)
            {
            }
            return false;
        }

        public bool TryGetByName(string name, out IField value)
        {
            if (name == null)
            {
                value = null;
                return false;
            }
            if (!_definitions.TryGetByName(name, out IFieldDefinition def))
            {
                value = null;
                return false;
            }
            return TryGetById(def.Id, out value);
        }

        public bool Equals(IReadOnlyListWithId<IField, int> other)
        {
            return Comparer.FieldCollectionComparer.Equals(this, other);
        }
    }
}