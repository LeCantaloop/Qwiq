using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    internal class WorkItemTypeCollection : IWorkItemTypeCollection
    {
        private readonly IDictionary<string, int> _nameMap;

        private readonly IList<IWorkItemType> _workItemTypes;

        public WorkItemTypeCollection(params IWorkItemType[] workItemTypes)
            : this(workItemTypes as IEnumerable<IWorkItemType>)
        {
        }

        public WorkItemTypeCollection(IEnumerable<IWorkItemType> workItemTypes)
        {
            _workItemTypes = workItemTypes?.ToList() ?? throw new ArgumentNullException(nameof(workItemTypes));
            _nameMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < _workItemTypes.Count; i++) _nameMap.Add(_workItemTypes[i].Name, i);
        }

        public IWorkItemType this[string typeName]
        {
            get
            {
                if (typeName == null) throw new ArgumentNullException(nameof(typeName));
                if (_nameMap.TryGetValue(typeName, out int idx)) return _workItemTypes[idx];

                throw new Exception();
            }
        }

        public IEnumerator<IWorkItemType> GetEnumerator()
        {
            return _workItemTypes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}