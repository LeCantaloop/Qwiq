using System;
using System.Collections.Generic;
using System.Linq;

using Qwiq.Exceptions;

namespace Qwiq.Client.Soap
{
    public class WorkItemTypeCollection : Qwiq.WorkItemTypeCollection
    {
        private readonly Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemTypeCollection _workItemTypeCollection;

        internal WorkItemTypeCollection(Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemTypeCollection workItemTypeCollection)
            :base((List<IWorkItemType>)null)
        {
            _workItemTypeCollection = workItemTypeCollection
                                      ?? throw new ArgumentNullException(nameof(workItemTypeCollection));
        }

        public override int Count => _workItemTypeCollection.Count;

        public override IWorkItemType this[string name] => ExceptionHandlingDynamicProxyFactory
            .Create<IWorkItemType>(new WorkItemType(_workItemTypeCollection[name]));

        public override IEnumerator<IWorkItemType> GetEnumerator()
        {
            return _workItemTypeCollection.Cast<Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType>()
                                          .Select(
                                              item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItemType>(
                                                  new WorkItemType(item)))
                                          .GetEnumerator();
        }
    }
}