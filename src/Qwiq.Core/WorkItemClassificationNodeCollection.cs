using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public class WorkItemClassificationNodeCollection<TId> : ReadOnlyObjectWithIdCollection<IWorkItemClassificationNode<TId>, TId>, IEquatable<IWorkItemClassificationNodeCollection<TId>>, IWorkItemClassificationNodeCollection<TId>
    {
        public WorkItemClassificationNodeCollection([CanBeNull] IEnumerable<IWorkItemClassificationNode<TId>> items) : base(items)
        {
        }

        public bool Equals(IWorkItemClassificationNodeCollection<TId> other)
        {
            return ReadOnlyCollectionWithIdComparer<IWorkItemClassificationNode<TId>, TId>.Default.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return ReadOnlyCollectionWithIdComparer<IWorkItemClassificationNode<TId>, TId>.Default.Equals(this, obj as IWorkItemClassificationNodeCollection<TId>);
        }

        public override int GetHashCode()
        {
            return ReadOnlyCollectionWithIdComparer<IWorkItemClassificationNode<TId>, TId>.Default.GetHashCode(this);
        }
    }
}