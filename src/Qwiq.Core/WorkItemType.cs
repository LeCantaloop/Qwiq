using System;

namespace Microsoft.Qwiq
{
    public class WorkItemType : IWorkItemType, IEquatable<IWorkItemType>, IComparable<IWorkItemType>
    {
        internal WorkItemType(
            string name,
            string description,
            Lazy<IFieldDefinitionCollection> fieldDefinitions,
            Func<IWorkItem> workItemFactory = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            FieldDefinitionFactory = () => fieldDefinitions.Value;
            WorkItemFactory = workItemFactory;
            Name = name;
            Description = description;
        }

        protected internal Func<IFieldDefinitionCollection> FieldDefinitionFactory { get; internal set; }

        protected internal Func<IWorkItem> WorkItemFactory { get; internal set; }

        public int CompareTo(IWorkItemType other)
        {
            return WorkItemTypeComparer.Instance.Compare(this, other);
        }

        public bool Equals(IWorkItemType other)
        {
            return WorkItemTypeComparer.Instance.Equals(this, other);
        }

        public string Description { get; }

        public IFieldDefinitionCollection FieldDefinitions => FieldDefinitionFactory();

        public string Name { get; }

        public IWorkItem NewWorkItem()
        {
            return WorkItemFactory();
        }

        public override bool Equals(object obj)
        {
            return WorkItemTypeComparer.Instance.Equals(this, obj as IWorkItemType);
        }

        public override int GetHashCode()
        {
            return WorkItemTypeComparer.Instance.GetHashCode(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}