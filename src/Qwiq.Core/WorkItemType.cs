﻿using System;

namespace Microsoft.Qwiq
{
    public class WorkItemType : IWorkItemType, IEquatable<IWorkItemType>, IComparable<IWorkItemType>
    {
        private IFieldDefinitionCollection _fdc;

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

        public string Description { get; }
        public IFieldDefinitionCollection FieldDefinitions => _fdc ?? (_fdc = FieldDefinitionFactory());
        public string Name { get; }
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