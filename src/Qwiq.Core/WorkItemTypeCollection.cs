using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    public class WorkItemTypeCollection : IWorkItemTypeCollection, IEquatable<IWorkItemTypeCollection>
    {
        private readonly IDictionary<string, int> _nameMap;

        private readonly IList<IWorkItemType> _workItemTypes;

        internal WorkItemTypeCollection()
            : this(null)
        {
        }

        internal WorkItemTypeCollection(params IWorkItemType[] workItemTypes)
            : this(workItemTypes as IEnumerable<IWorkItemType>)
        {
        }

        internal WorkItemTypeCollection(IEnumerable<IWorkItemType> workItemTypes)
        {
            _workItemTypes = workItemTypes?.ToList() ?? new List<IWorkItemType>();
            _nameMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < _workItemTypes.Count; i++) _nameMap.Add(_workItemTypes[i].Name, i);
        }

        public bool Equals(IWorkItemTypeCollection other)
        {
            return WorkItemTypeCollectionComparer.Instance.Equals(this, other);
        }

        public virtual int Count => _workItemTypes.Count;

        public virtual IWorkItemType this[string typeName]
        {
            get
            {
                if (typeName == null) throw new ArgumentNullException(nameof(typeName));
                if (_nameMap.TryGetValue(typeName, out int idx)) return _workItemTypes[idx];

                throw new Exception();
            }
        }

        public virtual IEnumerator<IWorkItemType> GetEnumerator()
        {
            return _workItemTypes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return WorkItemTypeCollectionComparer.Instance.Equals(this, obj as IWorkItemTypeCollection);
        }

        public override int GetHashCode()
        {
            return WorkItemTypeCollectionComparer.Instance.GetHashCode(this);
        }
    }
}