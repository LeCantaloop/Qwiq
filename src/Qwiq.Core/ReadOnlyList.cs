using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    /// <summary>
    ///     Base class for common operations for Collections.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IList<T> _list;

        private readonly object _lockObj = new object();

        private readonly IDictionary<string, int> _mapByName;

        private readonly Func<T, string> _nameFunc;

        private bool _alreadyInit;

        private IEnumerable<T> _items;

        protected ReadOnlyList(IEnumerable<T> items, Func<T, string> nameFunc)
        {
            _items = items ?? Enumerable.Empty<T>();
            _nameFunc = nameFunc;
            _list = new List<T>();
            _mapByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public virtual int Count
        {
            get
            {
                Ensure();
                return _list.Count;
            }
        }

        public virtual T this[int index]
        {
            get
            {
                Ensure();
                if (index < 0 || index >= _list.Count) throw new ArgumentOutOfRangeException(nameof(index));
                return _list[index];
            }
        }

        public virtual T this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                Ensure();
                int num;
                if (_mapByName.TryGetValue(name, out num)) return _list[num];

                throw new DeniedOrNotExistException();
            }
        }

        public virtual bool Contains(T value)
        {
            return IndexOf(value) != -1;
        }

        public virtual bool Contains(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Ensure();
            return _mapByName.ContainsKey(name);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            Ensure();
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        T IReadOnlyList<T>.GetItem(int index)
        {
            return GetItem(index);
        }

        public virtual int IndexOf(T value)
        {
            Ensure();
            for (var i = 0; i < Count; i++) if (GenericComparer<T>.Default.Equals(this[i], value)) return i;

            return -1;
        }

        public virtual bool TryGetByName(string name, out T value)
        {
            Ensure();
            int num;
            if (_mapByName.TryGetValue(name, out num))
            {
                value = _list[num];
                return true;
            }
            value = default(T);
            return false;
        }

        protected virtual void Add(T value, int index)
        {
            if (_nameFunc != null)
            {
                var name = _nameFunc(value);
                AddByName(name, index);
            }
        }

        protected void AddByName(string name, int index)
        {
            try
            {
                _mapByName.Add(name, index);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException($"An item with the name {name} already exists.", e);
            }
        }

        protected void Ensure()
        {
            if (!_alreadyInit)
                lock (_lockObj)
                {
                    if (!_alreadyInit)
                    {
                        foreach (var item in _items) Add(item);

                        _alreadyInit = true;
                        _items = null;
                    }
                }
        }

        protected virtual T GetItem(int index)
        {
            return this[index];
        }

        private void Add(T value)
        {
            var index = _list.Count;

            Add(value, index);

            _list.Add(value);
        }
    }
}