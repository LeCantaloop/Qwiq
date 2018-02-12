using System;
using System.Collections.Generic;

namespace Qwiq
{
    internal class ProjectCollection : ReadOnlyObjectWithIdCollection<IProject, Guid>, IProjectCollection
    {
        internal ProjectCollection(List<IProject> projects)
            : base(projects, project => project.Name)
        {
        }

        public IProject this[Guid id] => GetById(id);
    }
}