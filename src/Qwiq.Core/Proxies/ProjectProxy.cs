using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Proxies
{
    internal class ProjectProxy : IProject, IComparer<IProject>, IEquatable<IProject>
    {
        private readonly Lazy<IEnumerable<INode>> _area;

        private readonly Lazy<IEnumerable<INode>> _iteration;

        private readonly Lazy<IEnumerable<IWorkItemType>> _wits;

        internal ProjectProxy(
            int id,
            Guid guid,
            string name,
            Uri uri,
            IWorkItemStore store,
            Lazy<IEnumerable<IWorkItemType>> wits,
            Lazy<IEnumerable<INode>> area,
            Lazy<IEnumerable<INode>> iteration)
        {
            Id = id;
            Guid = guid;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            Store = store ?? throw new ArgumentNullException(nameof(store));
            _wits = wits ?? throw new ArgumentNullException(nameof(wits));
            _area = area ?? throw new ArgumentNullException(nameof(area));
            _iteration = iteration ?? throw new ArgumentNullException(nameof(iteration));
        }

        private ProjectProxy()
        {
        }

        public int Compare(IProject x, IProject y)
        {
            return ProjectComparer.Instance.Compare(x, y);
        }

        public bool Equals(IProject other)
        {
            return ProjectComparer.Instance.Equals(this, other);
        }

        public IEnumerable<INode> AreaRootNodes => _area.Value;

        public Guid Guid { get; }

        public int Id { get; }

        public IEnumerable<INode> IterationRootNodes => _iteration.Value;

        public string Name { get; }

        public IWorkItemStore Store { get; }

        public Uri Uri { get; }

        public IEnumerable<IWorkItemType> WorkItemTypes => _wits.Value;

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