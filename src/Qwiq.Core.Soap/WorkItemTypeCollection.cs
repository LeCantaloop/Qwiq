using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Soap
{
    internal class WorkItemTypeCollection : IWorkItemTypeCollection
    {
        private readonly TeamFoundation.WorkItemTracking.Client.WorkItemTypeCollection _workItemTypeCollection;

        internal WorkItemTypeCollection(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemTypeCollection workItemTypeCollection)
        {
            _workItemTypeCollection = workItemTypeCollection;
        }

        public IEnumerator<IWorkItemType> GetEnumerator()
        {
            return _workItemTypeCollection.Cast<TeamFoundation.WorkItemTracking.Client.WorkItemType>()
                                          .Select(item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItemType>(new WorkItemType(item)))
                                          .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IWorkItemType this[string typeName] => ExceptionHandlingDynamicProxyFactory.Create<IWorkItemType>(new WorkItemType(_workItemTypeCollection[typeName]));
    }
}