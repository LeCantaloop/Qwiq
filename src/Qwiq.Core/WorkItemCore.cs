using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public abstract class WorkItemCore : IWorkItemCore, IEquatable<IWorkItemCore>, IRevisionInternal
    {
        private readonly Dictionary<string, object> _fields;

        protected internal WorkItemCore()
            :this(null)
        {
        }

        protected internal WorkItemCore([CanBeNull] Dictionary<string, object> fields)
        {
            _fields = fields ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public virtual int? Id => GetValue<int?>(CoreFieldRefNames.Id);

        public virtual int? Rev => GetValue<int?>(CoreFieldRefNames.Rev);

        public abstract string Url { get; }

        /// <summary>
        /// Gets or sets the <see cref="object"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="object"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// name is null
        /// </exception>
        public virtual object this[string name]
        {
            get

            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                return GetValue(name);
            }
            set
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                SetValue(name, value);
            }
        }

        protected virtual T GetValue<T>(string name)
        {
            var value = GetValue(name);

            if (value == null)
            {
                if (typeof(T) == typeof(string))
                {
                    return (T)(object)string.Empty;
                }
            }

            return TypeParser.Default.Parse(value, default(T));
        }

        [CanBeNull]
        [JetBrains.Annotations.Pure]
        protected virtual object GetValue([NotNull] string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            
            return _fields.TryGetValue(name, out object val) ? val : null;
        }

        protected virtual void SetValue(string name, object value)
        {
            _fields[name] = value;
        }

        public bool Equals(IWorkItemCore other)
        {
            return NullableIdentifiableComparer.Default.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return NullableIdentifiableComparer.Default.Equals(this, obj as IWorkItemCore);
        }

        public override int GetHashCode()
        {
            return NullableIdentifiableComparer.Default.GetHashCode(this);
        }

        [CanBeNull]
        public object GetCurrentFieldValue(IFieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null) throw new ArgumentNullException(nameof(fieldDefinition));
            return GetValue(fieldDefinition.ReferenceName);
        }

        public void SetFieldValue(IFieldDefinition fieldDefinition, object value)
        {
            if (fieldDefinition == null) throw new ArgumentNullException(nameof(fieldDefinition));
            SetValue(fieldDefinition.ReferenceName, value);
        }
    }
}