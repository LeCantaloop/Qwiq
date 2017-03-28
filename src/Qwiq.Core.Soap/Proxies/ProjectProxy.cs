using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    internal class ProjectProxy : Microsoft.Qwiq.Proxies.ProjectProxy
    {
        internal ProjectProxy(Tfs.Project project, IWorkItemStore store)
            : base(
                project.Id,
                project.Guid,
                project.Name,
                project.Uri,
                store,
                new Lazy<IEnumerable<IWorkItemType>>(
                    () => project.WorkItemTypes.Cast<Tfs.WorkItemType>()
                                 .Select(
                                     item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItemType>(
                                         new WorkItemTypeProxy(item)))),
                new Lazy<IEnumerable<INode>>(
                    () => project.AreaRootNodes.Cast<Tfs.Node>()
                                 .Select(
                                     item => ExceptionHandlingDynamicProxyFactory.Create<INode>(new NodeProxy(item)))),
                new Lazy<IEnumerable<INode>>(
                    () => project.IterationRootNodes.Cast<Tfs.Node>()
                                 .Select(
                                     item => ExceptionHandlingDynamicProxyFactory.Create<INode>(new NodeProxy(item)))))
        {
        }
    }
}