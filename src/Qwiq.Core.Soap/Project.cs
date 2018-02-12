using System;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Qwiq.Client.Soap
{
    internal class Project : Qwiq.Project, IIdentifiable<int>
    {
        internal Project(Tfs.Project project)
            : base(
                project.Guid,
                project.Name,
                project.Uri,
                new Lazy<IWorkItemTypeCollection>(() => new WorkItemTypeCollection(project.WorkItemTypes)),
                new Lazy<IWorkItemClassificationNodeCollection<int>>(()=> WorkItemClassificationNodeCollectionBuilder.Build(project.AreaRootNodes)),
                new Lazy<IWorkItemClassificationNodeCollection<int>>(() => WorkItemClassificationNodeCollectionBuilder.Build(project.IterationRootNodes))
                )
        {
            Id = project.Id;
        }

        public new int Id { get; }
    }
}