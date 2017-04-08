using System;

namespace Microsoft.Qwiq
{
    public class Project : IProject, IEquatable<IProject>
    {
        private readonly Lazy<INodeCollection> _area;

        private readonly Lazy<INodeCollection> _iteration;

        private readonly Lazy<IWorkItemTypeCollection> _wits;

        internal Project(
            Guid guid,
            string name,
            Uri uri,
            Lazy<IWorkItemTypeCollection> wits,
            Lazy<INodeCollection> area,
            Lazy<INodeCollection> iteration)
        {
            Guid = guid;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            _wits = wits ?? throw new ArgumentNullException(nameof(wits));
            _area = area ?? throw new ArgumentNullException(nameof(area));
            _iteration = iteration ?? throw new ArgumentNullException(nameof(iteration));
        }

        private Project()
        {
        }

        public bool Equals(IProject other)
        {
            return ProjectComparer.Instance.Equals(this, other);
        }

        public INodeCollection AreaRootNodes => _area.Value;

        public Guid Guid { get; }

        public INodeCollection IterationRootNodes => _iteration.Value;

        public string Name { get; }

        public Uri Uri { get; }

        public IWorkItemTypeCollection WorkItemTypes => _wits.Value;

        public override bool Equals(object obj)
        {
            return ProjectComparer.Instance.Equals(this, obj as IProject);
        }

        public override int GetHashCode()
        {
            return ProjectComparer.Instance.GetHashCode(this);
        }

        public override string ToString()
        {
            return $"{Guid} ({Name})";
        }
    }
}