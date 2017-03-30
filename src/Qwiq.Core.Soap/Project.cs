using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class Project : Qwiq.Project
    {
        internal Project(Tfs.Project project)
            : base(
                project.Id,
                project.Guid,
                project.Name,
                project.Uri,
                new Lazy<IEnumerable<IWorkItemType>>(
                    () => project.WorkItemTypes.Cast<Tfs.WorkItemType>()
                                 .Select(
                                     item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItemType>(
                                         new WorkItemType(item)))),
                new Lazy<IEnumerable<INode>>(
                    () => project.AreaRootNodes.Cast<Tfs.Node>()
                                 .Select(
                                     item => ExceptionHandlingDynamicProxyFactory.Create<INode>(new Node(item)))),
                new Lazy<IEnumerable<INode>>(
                    () => project.IterationRootNodes.Cast<Tfs.Node>()
                                 .Select(
                                     item => ExceptionHandlingDynamicProxyFactory.Create<INode>(new Node(item)))))
        {
        }
    }
}