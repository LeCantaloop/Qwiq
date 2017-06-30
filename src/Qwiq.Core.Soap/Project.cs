using System;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class Project : Qwiq.Project, IIdentifiable<int>
    {
        internal Project(Tfs.Project project)
            : base(
                project.Guid,
                project.Name,
                project.Uri,
                new Lazy<IWorkItemTypeCollection>(() => new WorkItemTypeCollection(project.WorkItemTypes)),
                new Lazy<INodeCollection>(
                    () => new NodeCollection(
                        project.AreaRootNodes.Cast<Tfs.Node>()
                               .Select(item => ExceptionHandlingDynamicProxyFactory.Create<INode>(new Node(item))))),
                new Lazy<INodeCollection>(
                    () => new NodeCollection(
                        project.IterationRootNodes.Cast<Tfs.Node>()
                               .Select(item => ExceptionHandlingDynamicProxyFactory.Create<INode>(new Node(item))))))
        {
            Id = project.Id;
        }

        public new int Id { get; }
    }
}