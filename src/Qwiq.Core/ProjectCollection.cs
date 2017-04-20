using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Qwiq
{
    internal class ProjectCollection : ReadOnlyCollectionWithId<IProject, Guid>, IProjectCollection
    {
        internal ProjectCollection(params IProject[] projects)
            : this(projects as IEnumerable<IProject>)
        {
            if (projects == null) throw new ArgumentNullException(nameof(projects));
        }

        internal ProjectCollection(IEnumerable<IProject> projects)
            : base(projects, project => project.Name)
        {
        }

        public IProject this[Guid id] => GetById(id);

        [DebuggerStepThrough]
        public static implicit operator ProjectCollection(Project project)
        {
            return new ProjectCollection(project);
        }
    }
}