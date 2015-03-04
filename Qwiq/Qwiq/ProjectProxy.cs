using System;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public class ProjectProxy : IProject
    {
        private readonly Tfs.Project _project;

        internal ProjectProxy(Tfs.Project project)
        {
            _project = project;
        }

        public Tfs.NodeCollection AreaRootNodes
        {
            get { return _project.AreaRootNodes; }
        }

        public int Id
        {
            get { return _project.Id; }
        }

        public Tfs.NodeCollection IterationRootNodes
        {
            get { return _project.IterationRootNodes;  }
        }

        public string Name
        {
            get { return _project.Name; }
        }

        public Uri Uri
        {
            get { return _project.Uri; }
        }

        public Tfs.WorkItemTypeCollection WorkItemTypes
        {
            get { return _project.WorkItemTypes; }
        }
    }
}