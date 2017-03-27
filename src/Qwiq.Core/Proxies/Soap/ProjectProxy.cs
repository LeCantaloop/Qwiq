using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class ProjectProxy : IProject
    {
        private readonly Tfs.Project _project;

        private readonly Lazy<IEnumerable<IWorkItemType>> _wits;

        internal ProjectProxy(Tfs.Project project, IWorkItemStore store)
        {
            Store = store;
            _project = project;
            Guid = project.Guid;
            _wits = new Lazy<IEnumerable<IWorkItemType>>(() => _project.WorkItemTypes.Cast<Tfs.WorkItemType>().Select(item => ExceptionHandlingDynamicProxyFactory.Create<IWorkItemType>(new WorkItemTypeProxy(item))));
        }

        public IEnumerable<INode> AreaRootNodes
        {
            get { return _project.AreaRootNodes.Cast<Tfs.Node>().Select(item => ExceptionHandlingDynamicProxyFactory.Create<INode>(new NodeProxy(item))); }
        }

        public Guid Guid { get; }

        public int Id => _project.Id;

        public IEnumerable<INode> IterationRootNodes
        {
            get { return _project.IterationRootNodes.Cast<Tfs.Node>().Select(item => ExceptionHandlingDynamicProxyFactory.Create<INode>(new NodeProxy(item))); }
        }

        public string Name => _project.Name;

        public Uri Uri => _project.Uri;

        public IEnumerable<IWorkItemType> WorkItemTypes => _wits.Value;

        public IWorkItemStore Store { get; }
    }
}
