using System;

namespace Microsoft.Qwiq
{
    public abstract class WorkItemCore : IWorkItemCore
    {
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
        public object this[string name]
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

        protected abstract object GetValue(string name);

        protected abstract void SetValue(string name, object value);
    }
}