using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Qwiq
{
    public abstract class WorkItemCore : IWorkItemCore, IEquatable<IWorkItemCore>, IRevisionInternal
    {
        private readonly Dictionary<string, object?> _fields = null!;

        protected internal WorkItemCore()
        {
        }

        protected internal WorkItemCore(Dictionary<string, object?> fields)
        {
            _fields = fields ?? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        }

        public virtual int? Id => GetValue<int?>(CoreFieldRefNames.Id);
        public virtual int? Rev => GetValue<int?>(CoreFieldRefNames.Rev);
        public abstract string Url { get; }
        /// <summary>
        /// Gets or sets the <see cref="object"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="object"/>, or null if the field is not set.
        /// </value>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>The field value, or null if not set.</returns>
        /// <exception cref="ArgumentNullException">
        /// name is null
        /// </exception>
        public virtual object? this[string name]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(name);
                return GetValue(name);
            }
            set
            {
                ArgumentNullException.ThrowIfNull(name);
                SetValue(name, value);
            }
        }

        public bool Equals(IWorkItemCore? other)
        {
            return NullableIdentifiableComparer.Default.Equals(this, other);
        }

        public override bool Equals(object? obj)
        {
            return NullableIdentifiableComparer.Default.Equals(this, obj as IWorkItemCore);
        }

        public object? GetCurrentFieldValue(IFieldDefinition fieldDefinition)
        {
            ArgumentNullException.ThrowIfNull(fieldDefinition);
            return GetValue(fieldDefinition.ReferenceName);
        }

        public override int GetHashCode()
        {
            return NullableIdentifiableComparer.Default.GetHashCode(this);
        }

        public void SetFieldValue(IFieldDefinition fieldDefinition, object? value)
        {
            ArgumentNullException.ThrowIfNull(fieldDefinition);
            SetValue(fieldDefinition.ReferenceName, value);
        }

        protected virtual T GetValue<T>(string name)
        {
            var value = GetValue(name);

            if (value == null) if (typeof(T) == typeof(string)) return (T)(object)string.Empty;

            return TypeParser.Default.Parse(value, default(T)!);
        }
        protected virtual object? GetValue(string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            if (_fields == null) throw new InvalidOperationException("Type must be initialized with fields.");
            _fields.TryGetValue(name, out object? val);

#if DEBUG
            Trace.WriteLine($"Get \'{name}\': {val.ToUsefulString()}");
#endif

            return val;
        }

        protected virtual void SetValue(string name, object? value)
        {
            if (_fields == null) throw new InvalidOperationException("Type must be initialized with fields.");
            _fields[name] = value;

#if DEBUG
            Trace.WriteLine($"Set \'{name}\' to {value.ToUsefulString()}");
#endif
        }
    }
}