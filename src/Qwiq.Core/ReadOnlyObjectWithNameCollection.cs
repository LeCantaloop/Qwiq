using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private readonly Func<T, string>? _nameFunc;
        // Dictionary is lazily initialized in Initialize() to avoid allocations during construction
        // The null! is intentional - the field is guaranteed to be initialized before use via Ensure()
        private Dictionary<string, int> _mapByName = null!;

        protected ReadOnlyObjectWithNameCollection(
            Func<IEnumerable<T>> itemFactory,
            Func<T, string>? nameFunc)
        : this()
        {
            Contract.Requires(itemFactory != null);

            ItemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
            _nameFunc = nameFunc;
        }

        protected ReadOnlyObjectWithNameCollection(IList<T>? items, Func<T, string>? nameFunc)
            : base(items)
        {
            _nameFunc = nameFunc;
            Initialize();
        }

        protected ReadOnlyObjectWithNameCollection(IEnumerable<T> items)
            : this(() => items, null)
        {
        }

        protected ReadOnlyObjectWithNameCollection(IList<T>? items)
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
                ArgumentNullException.ThrowIfNull(name);
                Ensure();
                if (_mapByName.TryGetValue(name, out int num)) return List[num];

                throw new DeniedOrNotExistException();
            }
        }

        public virtual bool Contains(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            Ensure();
            return _mapByName.ContainsKey(name);
        }

        public virtual bool TryGetByName(string name, [MaybeNullWhen(false)] out T value)
        {
            if (string.IsNullOrEmpty(name))
            {
                value = default;
                return false;
            }

            Ensure();
            if (_mapByName.TryGetValue(name, out int num))
            {
                value = List[num];
                return true;
            }
            value = default;
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