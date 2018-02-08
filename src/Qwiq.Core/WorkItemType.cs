using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Qwiq
{
    public class WorkItemType : IWorkItemType, IEquatable<IWorkItemType>
    {
        private IFieldDefinitionCollection _fdc;

        [CanBeNull]
        private readonly Lazy<IFieldDefinitionCollection> _lazyFieldDefinitions;

        private Func<IFieldDefinitionCollection> _fieldDefinitionFactory;

        internal WorkItemType(
            [NotNull] string name,
            [CanBeNull] string description,
            [CanBeNull] Lazy<IFieldDefinitionCollection> fieldDefinitions,
            Func<IWorkItem> workItemFactory = null)
        {
            Contract.Requires(name != null);
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(fieldDefinitions != null);

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            _lazyFieldDefinitions = fieldDefinitions;
            WorkItemFactory = workItemFactory;
            Name = string.Intern(name);
            Description = description == null ? string.Empty : string.Intern(description);
        }

        protected internal Func<IFieldDefinitionCollection> FieldDefinitionFactory
        {
            get => _fieldDefinitionFactory;
            internal set
            {
                Debug.Assert(_lazyFieldDefinitions == null, "_lazyFieldDefinitions == null");
                _fieldDefinitionFactory = value;
            }
        }

        protected internal Func<IWorkItem> WorkItemFactory { get; internal set; }

        public bool Equals([CanBeNull] IWorkItemType other)
        {
            return WorkItemTypeComparer.Default.Equals(this, other);
        }

        public string Description { get; }

        public virtual IFieldDefinitionCollection FieldDefinitions => _fdc ?? (_fdc = FieldDefinitionFactory == null ? _lazyFieldDefinitions.Value : FieldDefinitionFactory());

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