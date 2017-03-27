using System;

namespace Microsoft.Qwiq.Proxies
{
    public partial class WorkItemTypeProxy : IWorkItemType, IEquatable<IWorkItemType>
    {
        private readonly Lazy<IFieldDefinitionCollection> _fieldDefinitions;

        private readonly Func<IWorkItem> _workItemFactory;

        internal WorkItemTypeProxy(
            string name,
            string description,
            Lazy<IFieldDefinitionCollection> fieldDefinitions,
            Func<IWorkItem> workItemFactory)
        {
            _fieldDefinitions = fieldDefinitions;
            _workItemFactory = workItemFactory;
            Name = name;
            Description = description;
        }

        public bool Equals(IWorkItemType other)
        {
            return WorkItemTypeComparer.Instance.Equals(this, other);
        }

        public string Description { get; }

        public IFieldDefinitionCollection FieldDefinitions => _fieldDefinitions.Value;

        public string Name { get; }

        public IWorkItem NewWorkItem()
        {
            return _workItemFactory();
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