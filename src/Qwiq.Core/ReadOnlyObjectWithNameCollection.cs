using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    /// <summary>
    ///     Base class for common operations for Collections.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ReadOnlyObjectWithNameCollection<T> : IReadOnlyObjectWithNameCollection<T>
    {
        private readonly object _lockObj = new object();

        [CanBeNull]
        private readonly Func<T, string> _nameFunc;

        private bool _alreadyInit;

        private Func<IEnumerable<T>> _itemFactory;

        private Lazy<IEnumerable<T>> _lazyItems;

        private IDictionary<string, int> _mapByName;

        protected ReadOnlyObjectWithNameCollection([NotNull] Func<IEnumerable<T>> itemFactory, [CanBeNull] Func<T, string> nameFunc)
        {
            Contract.Requires(itemFactory != null);
            Contract.Requires(nameFunc != null);

            ItemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
            _nameFunc = nameFunc;
        }

        protected ReadOnlyObjectWithNameCollection([CanBeNull] List<T> items, [CanBeNull] Func<T, string> nameFunc)
        {
            List = items ?? new List<T>(0);
            _mapByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _nameFunc = nameFunc;
            _alreadyInit = false;
        }

        protected ReadOnlyObjectWithNameCollection([CanBeNull] IEnumerable<T> items)
            : this(()=> items, null)
        {
        }

        protected ReadOnlyObjectWithNameCollection([CanBeNull] List<T> items)
            : this(items, null)
        {
        }

        protected ReadOnlyObjectWithNameCollection()
        {
            Initialize();
        }

        public virtual int Count
        {
            get
            {
                Ensure();
                return List.Count;
            }
        }

        protected internal List<T> List { get; private set; }

        protected Func<IEnumerable<T>> ItemFactory
        {
            get => _itemFactory;
            set
            {
                _itemFactory = value;
                lock (_lockObj)
                {
                    _lazyItems = new Lazy<IEnumerable<T>>(_itemFactory);
                    Initialize();
                }
            }
        }

        public virtual T this[int index]
        {
            get
            {
                Ensure();
                if (index < 0 || index >= List.Count) throw new ArgumentOutOfRangeException(nameof(index));
                return List[index];
            }
        }

        public virtual T this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                Ensure();
                if (_mapByName.TryGetValue(name, out int num)) return List[num];

                throw new DeniedOrNotExistException();
            }
        }

        [DebuggerStepThrough]
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

        [DebuggerStepThrough]
        public virtual IEnumerator<T> GetEnumerator()
        {
            Ensure();
            return List.GetEnumerator();
        }

        public virtual int IndexOf(T value)
        {
            Ensure();
            for (var i = 0; i < Count; i++) if (GenericComparer<T>.Default.Equals(this[i], value)) return i;

            return -1;
        }

        public virtual bool TryGetByName(string name, out T value)
        {
            if (string.IsNullOrEmpty(name))
            {
                value = default(T);
                return false;
            }

            Ensure();
            if (_mapByName.TryGetValue(name, out int num))
            {
                value = List[num];
                return true;
            }
            value = default(T);
            return false;
        }

        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        T IReadOnlyObjectWithNameCollection<T>.GetItem(int index)
        {
            return GetItem(index);
        }

        protected virtual void Add(T value, int index)
        {
            if (_nameFunc != null)
            {
                var name = _nameFunc(value);
                AddByName(name, index);
            }
        }

        protected int Add(T value)
        {
            var index = List.Count;

            Add(value, index);

            List.Add(value);
            return index;
        }

        protected void AddByName(string name, int index)
        {

            var exists = _mapByName.ContainsKey(name);

            Debug.Assert(!exists, $"An item with the name {name} already exists.");

            if (!exists)
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

        
        }

        protected void Ensure()
        {
            if (!_alreadyInit)
                lock (_lockObj)
                {
                    if (!_alreadyInit)
                    {
                        if (_lazyItems != null) foreach (var item in _lazyItems.Value) Add(item);
                        else for (var i = 0; i < List.Count; i++) Add(List[i], i);

                        _alreadyInit = true;
                        _lazyItems = null;
                    }
                }
        }

        protected virtual T GetItem(int index)
        {
            return this[index];
        }

        private void Initialize()
        {
            lock (_lockObj)
            {
                List = new List<T>();
                _mapByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                _alreadyInit = false;
            }
        }
    }
}