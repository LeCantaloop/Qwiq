using System;

namespace Microsoft.Qwiq.Proxies
{
    public class WorkItemTypeProxy : IWorkItemType, IEquatable<IWorkItemType>, IComparable<IWorkItemType>
    {
        private readonly Lazy<IFieldDefinitionCollection> _fieldDefinitions;

        internal WorkItemTypeProxy(string name, string description, Lazy<IFieldDefinitionCollection> fieldDefinitions)
            : this(name, description, fieldDefinitions, null)
        {
        }

        internal WorkItemTypeProxy(
            string name,
            string description,
            Lazy<IFieldDefinitionCollection> fieldDefinitions,
            Func<IWorkItem> workItemFactory)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            _fieldDefinitions = fieldDefinitions ?? throw new ArgumentNullException(nameof(fieldDefinitions));
            WorkItemFactory = workItemFactory;
            Name = name;
            Description = description;
        }

        public string Description { get; }
        public IFieldDefinitionCollection FieldDefinitions => _fieldDefinitions.Value;
        public string Name { get; }
        protected internal Func<IWorkItem> WorkItemFactory { get; internal set; }

        public int CompareTo(IWorkItemType other)
        {
            return WorkItemTypeComparer.Instance.Compare(this, other);
        }

        public bool Equals(IWorkItemType other)
        {
            return WorkItemTypeComparer.Instance.Equals(this, other);
        }
        public override bool Equals(object obj)
        {
            return WorkItemTypeComparer.Instance.Equals(this, obj as IWorkItemType);
        }

        public override int GetHashCode()
        {
            return WorkItemTypeComparer.Instance.GetHashCode(this);
        }

        public IWorkItem NewWorkItem()
        {
            return WorkItemFactory();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}