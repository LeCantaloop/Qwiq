using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Client.Soap
{
    public class WorkItemTypeCollection : Qwiq.WorkItemTypeCollection
    {
        private readonly TeamFoundation.WorkItemTracking.Client.WorkItemTypeCollection _workItemTypeCollection;

        internal WorkItemTypeCollection(
            TeamFoundation.WorkItemTracking.Client.WorkItemTypeCollection workItemTypeCollection)
        {
            _workItemTypeCollection = workItemTypeCollection
                                      ?? throw new ArgumentNullException(nameof(workItemTypeCollection));
        }

        public override int Count => _workItemTypeCollection.Count;

        public override IWorkItemType this[string typeName] => ExceptionHandlingDynamicProxyFactory
            .Create<IWorkItemType>(new WorkItemType(_workItemTypeCollection[typeName]));

        public override IEnumerator<IWorkItemType> GetEnumerator()
        {
            return _workItemTypeCollection.Cast<TeamFoundation.WorkItemTracking.Client.WorkItemType>()
                                          .Select(
                                              item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItemType>(
                                                  new WorkItemType(item)))
                                          .GetEnumerator();
        }
    }
}