using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq
{
    internal class ProjectCollection : IProjectCollection
    {
        private readonly Dictionary<Guid, int> _guidMap;

        private readonly Dictionary<string, int> _nameMap;

        private readonly IList<IProject> _projects;

        internal ProjectCollection(params IProject[] projects)
            : this(projects as IEnumerable<IProject>)
        {
            if (projects == null) throw new ArgumentNullException(nameof(projects));
        }

        internal ProjectCollection(IEnumerable<IProject> projects)
        {
            _projects = projects?.ToList() ?? throw new ArgumentNullException(nameof(projects));

            _nameMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _guidMap = new Dictionary<Guid, int>();

            for (var i = 0; i < _projects.Count; i++)
            {
                _nameMap.Add(_projects[i].Name, i);
                _guidMap.Add(_projects[i].Guid, i);
            }
        }

        public IProject this[string projectName]
        {
            get
            {
                if (projectName == null) throw new ArgumentNullException(nameof(projectName));
                if (_nameMap.TryGetValue(projectName, out int idx)) return _projects[idx];

                throw new DeniedOrNotExistException(projectName);
            }
        }

        public IProject this[Guid projectGuid]
        {
            get
            {
                if (projectGuid == Guid.Empty) throw new ArgumentException();

                if (_guidMap.TryGetValue(projectGuid, out int idx)) return _projects[idx];

                throw new DeniedOrNotExistException(projectGuid);
            }
        }

        public IEnumerator<IProject> GetEnumerator()
        {
            return _projects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator ProjectCollection(Project project)
        {
            return new ProjectCollection(project);
        }
    }
}