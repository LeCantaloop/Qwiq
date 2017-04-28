using System;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public class WorkItemType : IWorkItemType, IEquatable<IWorkItemType>
    {
        private IFieldDefinitionCollection _fdc;

        internal WorkItemType(
            [NotNull] string name,
            [CanBeNull] string description,
            [NotNull] Lazy<IFieldDefinitionCollection> fieldDefinitions,
            Func<IWorkItem> workItemFactory = null)
        {
            Contract.Requires(name != null);
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(fieldDefinitions != null);

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            FieldDefinitionFactory = () => fieldDefinitions.Value;
            WorkItemFactory = workItemFactory;
            Name = name;
            Description = description;
        }

        protected internal Func<IFieldDefinitionCollection> FieldDefinitionFactory { get; internal set; }

        protected internal Func<IWorkItem> WorkItemFactory { get; internal set; }

        public bool Equals([CanBeNull] IWorkItemType other)
        {
            return WorkItemTypeComparer.Default.Equals(this, other);
        }

        public string Description { get; }

        public IFieldDefinitionCollection FieldDefinitions => _fdc ?? (_fdc = FieldDefinitionFactory());

        public string Name { get; }

        public IWorkItem NewWorkItem()
        {
            return WorkItemFactory();
        }

        public override bool Equals(object obj)
        {
            return WorkItemTypeComparer.Default.Equals(this, obj as IWorkItemType);
        }

        public override int GetHashCode()
        {
            return WorkItemTypeComparer.Default.GetHashCode(this);
        }

        [NotNull]
        public override string ToString()
        {
            return Name;
        }
    }
}