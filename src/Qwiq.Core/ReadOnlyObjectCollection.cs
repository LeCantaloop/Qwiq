using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Qwiq
{
    public abstract class ReadOnlyObjectCollection<T> : IReadOnlyObjectCollection<T>
    {
        private readonly object _lockObj = new object();
        private volatile bool _alreadyInit;
        private Func<IEnumerable<T>> _itemFactory;
        private Lazy<IEnumerable<T>> _lazyItems;

        protected ReadOnlyObjectCollection([NotNull] Func<IEnumerable<T>> itemFactory)
        {
            ItemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
        }

        protected ReadOnlyObjectCollection([CanBeNull] List<T> items)
            : this()
        {
            List = items ?? new List<T>(0);
            _alreadyInit = false;
        }

        protected ReadOnlyObjectCollection([CanBeNull] IEnumerable<T> items)
            : this(() => items)
        {
        }

        protected ReadOnlyObjectCollection()
        {
            Initialize();
        }

        protected internal List<T> List { get; set; }

        public virtual int Count
        {
            get
            {
                Ensure();
                return List.Count;
            }
        }

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

        [DebuggerStepThrough]
        public virtual bool Contains(T value)
        {
            return IndexOf(value) != -1;
        }

        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [DebuggerStepThrough]
        public virtual IEnumerator<T> GetEnumerator()
        {
            Ensure();
            return List.GetEnumerator();
        }

        T IReadOnlyObjectCollection<T>.GetItem(int index)
        {
            return GetItem(index);
        }

        public virtual int IndexOf(T value)
        {
            Ensure();
            for (var i = 0; i < Count; i++) if (GenericComparer<T>.Default.Equals(this[i], value)) return i;

            return -1;
        }

        protected int Add(T value)
        {
            var index = List.Count;

            Add(value, index);

            List.Add(value);
            return index;
        }

        protected virtual void Add(T value, int index)
        {
            // Available for hooking Add operations
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
                _alreadyInit = false;
            }
        }
    }
}