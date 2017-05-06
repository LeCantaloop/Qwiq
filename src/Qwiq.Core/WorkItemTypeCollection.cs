using System;
using System.Collections.Generic;
using System.Diagnostics;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public class WorkItemTypeCollection : ReadOnlyObjectWithNameCollection<IWorkItemType>, IWorkItemTypeCollection
    {
        [DebuggerStepThrough]
        internal WorkItemTypeCollection([NotNull] Func<IEnumerable<IWorkItemType>> workItemTypesFactory)
            : base(workItemTypesFactory, type => type.Name)
        {
        }

        [DebuggerStepThrough]
        internal WorkItemTypeCollection([CanBeNull] List<IWorkItemType> workItemTypes)
            : base(workItemTypes, type => type.Name)
        {
        }

        [DebuggerStepThrough]
        public bool Equals(IWorkItemTypeCollection other)
        {
            return WorkItemTypeCollectionComparer.Default.Equals(this, other);
        }

        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            return WorkItemTypeCollectionComparer.Default.Equals(this, obj as IWorkItemTypeCollection);
        }

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return WorkItemTypeCollectionComparer.Default.GetHashCode(this);
        }
    }
}