using System;
using System.Diagnostics;
using System.Globalization;


namespace Qwiq
{
    public class WorkItemLinkInfo : IWorkItemLinkInfo
    {
        private Lazy<IWorkItemLinkTypeEnd?>? _lazyLinkTypeEnd;
        private IWorkItemLinkTypeEnd? _linkTypeEnd;

        internal WorkItemLinkInfo(int sourceId, int targetId, IWorkItemLinkTypeEnd? linkTypeEnd)
        {
            SourceId = sourceId;
            TargetId = targetId;
            _linkTypeEnd = linkTypeEnd;
        }

        internal WorkItemLinkInfo(int sourceId, int targetId, Lazy<IWorkItemLinkTypeEnd?>? linkTypeEnd)
        {
            SourceId = sourceId;
            TargetId = targetId;
            _lazyLinkTypeEnd = linkTypeEnd ?? throw new ArgumentNullException(nameof(linkTypeEnd));
        }

        public IWorkItemLinkTypeEnd? LinkType
        {
            get
            {
                if (_linkTypeEnd != null) return _linkTypeEnd;
                if (_lazyLinkTypeEnd != null)
                {
                    _linkTypeEnd = _lazyLinkTypeEnd.Value;
                    _lazyLinkTypeEnd = null;
                    return _linkTypeEnd;
                }

                return null;
            }
        }

        public int SourceId { get; }

        public int TargetId { get; }

        [DebuggerStepThrough]
        public bool Equals(IWorkItemLinkInfo? other)
        {
            return WorkItemLinkInfoComparer.Default.Equals(this, other);
        }

        [DebuggerStepThrough]
        public override bool Equals(object? obj)
        {
            return WorkItemLinkInfoComparer.Default.Equals(this, obj as IWorkItemLinkInfo);
        }

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return WorkItemLinkInfoComparer.Default.GetHashCode(this);
        }

        public override string ToString()
        {
            return $"S:{SourceId} T:{TargetId} Type:{LinkType?.ImmutableName ?? "UNKNOWN"}".ToString(CultureInfo.InvariantCulture);
        }
    }
}