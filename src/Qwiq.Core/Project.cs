using System;
using System.Globalization;

namespace Qwiq
{
    public class Project : IProject, IEquatable<IProject>
    {
        private readonly Lazy<IWorkItemClassificationNodeCollection<int>> _area;

        private readonly Lazy<IWorkItemClassificationNodeCollection<int>> _iteration;

        private readonly Lazy<IWorkItemTypeCollection> _wits;

        private readonly Lazy<IQueryFolderCollection> _queryHierarchy;

        internal Project(
            Guid guid,
            string name,
            Uri uri,
            Lazy<IWorkItemTypeCollection> wits,
            Lazy<IWorkItemClassificationNodeCollection<int>> area,
            Lazy<IWorkItemClassificationNodeCollection<int>> iteration,
            Lazy<IQueryFolderCollection> queryHierarchy)
        {
            Guid = guid;
            Name = name != null ? string.Intern(name) : throw new ArgumentNullException(nameof(name));
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            _wits = wits ?? throw new ArgumentNullException(nameof(wits));
            _area = area ?? throw new ArgumentNullException(nameof(area));
            _iteration = iteration ?? throw new ArgumentNullException(nameof(iteration));
            _queryHierarchy = queryHierarchy ?? throw new ArgumentNullException(nameof(queryHierarchy));
        }

        private Project()
        {
        }

        public bool Equals(IProject other)
        {
            return ProjectComparer.Default.Equals(this, other);
        }

        public IWorkItemClassificationNodeCollection<int> AreaRootNodes => _area.Value;

        public Guid Guid { get; }

        public IWorkItemClassificationNodeCollection<int> IterationRootNodes => _iteration.Value;

        public string Name { get; }

        public Uri Uri { get; }

        public IWorkItemTypeCollection WorkItemTypes => _wits.Value;

        public IQueryFolderCollection QueryHierarchy => _queryHierarchy.Value;

        public override bool Equals(object obj)
        {
            return ProjectComparer.Default.Equals(this, obj as IProject);
        }

        public override int GetHashCode()
        {
            return ProjectComparer.Default.GetHashCode(this);
        }

        public override string ToString()
        {
            FormattableString s = $"{Guid.ToString()} ({Name})";
            return s.ToString(CultureInfo.InvariantCulture);
        }

        public Guid Id => Guid;
    }
}
