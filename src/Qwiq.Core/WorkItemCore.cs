using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public abstract class WorkItemCore : IWorkItemCore, IEquatable<IWorkItemCore>, IRevisionInternal
    {
        private readonly IDictionary<string, object> _fields;

        protected internal WorkItemCore()
            :this(null)
        {
        }

        protected internal WorkItemCore(IDictionary<string, object> fields)
        {
            _fields = fields ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public virtual int? Id => GetValue<int?>(CoreFieldRefNames.Id);

        public virtual int? Rev => GetValue<int?>(CoreFieldRefNames.Rev);

        public abstract string Url { get; }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
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
            return TypeParser.Default.Parse<T>(GetValue(name));
        }

        protected virtual object GetValue(string name)
        {
            return _fields != null && _fields.TryGetValue(name, out object val) ? val : null;
        }

        protected virtual void SetValue(string name, object value)
        {
            if (_fields != null)
            {
                _fields[name] = value;
            }
        }

        public bool Equals(IWorkItemCore other)
        {
            return NullableIdentifiableComparer.Instance.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return NullableIdentifiableComparer.Instance.Equals(this, obj as IWorkItemCore);
        }

        public override int GetHashCode()
        {
            return NullableIdentifiableComparer.Instance.GetHashCode(this);
        }

        public object GetCurrentFieldValue(IFieldDefinition fieldDefinition)
        {
            return GetValue(fieldDefinition.ReferenceName);
        }

        public void SetFieldValue(IFieldDefinition fieldDefinition, object value)
        {
            SetValue(fieldDefinition.ReferenceName, value);
        }
    }
}