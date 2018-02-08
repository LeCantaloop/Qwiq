using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Qwiq
{
    /// <summary>
    ///     Base class for common operations for Collections.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ReadOnlyObjectWithNameCollection<T> : ReadOnlyObjectCollection<T>, IReadOnlyObjectWithNameCollection<T>
    //TODO: Restrict T to INamed
    {
        
        private readonly object _lockObj = new object();

        [CanBeNull]
        private readonly Func<T, string> _nameFunc;
        private IDictionary<string, int> _mapByName;

        protected ReadOnlyObjectWithNameCollection(
            [NotNull] Func<IEnumerable<T>> itemFactory,
            [CanBeNull] Func<T, string> nameFunc)
        : this()
        {
            Contract.Requires(itemFactory != null);
            Contract.Requires(nameFunc != null);

            ItemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
            _nameFunc = nameFunc;
        }

        protected ReadOnlyObjectWithNameCollection([CanBeNull] List<T> items, [CanBeNull] Func<T, string> nameFunc)
            : base(items)
        {
            _nameFunc = nameFunc;
            Initialize();
        }

        protected ReadOnlyObjectWithNameCollection([CanBeNull] IEnumerable<T> items)
            : this(() => items, null)
        {
        }

        protected ReadOnlyObjectWithNameCollection([CanBeNull] List<T> items)
            : this(items, null)
        {
        }

        protected ReadOnlyObjectWithNameCollection()
            : base()
        {
            Initialize();
        }

        private void Initialize()
        {
            lock (_lockObj)
            {
                _mapByName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
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

        public virtual bool Contains(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Ensure();
            return _mapByName.ContainsKey(name);
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

        protected override void Add(T value, int index)
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
    }
}