using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public class WorkItemLinkInfo : IWorkItemLinkInfo
    {
        [CanBeNull]
        private readonly IWorkItemLinkTypeEnd _id;

        [CanBeNull]
        private readonly Lazy<IWorkItemLinkTypeEnd> _lazyId;

        internal WorkItemLinkInfo(int sourceId, int targetId, [NotNull] IWorkItemLinkTypeEnd id)
            : this(sourceId, targetId, (Lazy<IWorkItemLinkTypeEnd>) null)
        {
            Contract.Requires(id != null);
            
            _id = id ?? throw new ArgumentNullException(nameof(id));
        }

        internal WorkItemLinkInfo(int sourceId, int targetId, [CanBeNull] Lazy<IWorkItemLinkTypeEnd> lazyId)
        {
            SourceId = sourceId;
            TargetId = targetId;
            _lazyId = lazyId;
        }

        public IWorkItemLinkTypeEnd LinkType => _id ?? _lazyId?.Value ?? throw new InvalidOperationException();

        public int SourceId { get; }

        public int TargetId { get; }

        [DebuggerStepThrough]
        public bool Equals(IWorkItemLinkInfo other)
        {
            return WorkItemLinkInfoComparer.Default.Equals(this, other);
        }

        [DebuggerStepThrough]
        public static bool operator !=(WorkItemLinkInfo x, WorkItemLinkInfo y)
        {
            return !WorkItemLinkInfoComparer.Default.Equals(x, y);
        }

        [DebuggerStepThrough]
        public static bool operator ==(WorkItemLinkInfo x, WorkItemLinkInfo y)
        {
            return WorkItemLinkInfoComparer.Default.Equals(x, y);
        }

        [DebuggerStepThrough]
        public override bool Equals(object obj)
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
            return $"S:{SourceId} T:{TargetId} Type:{LinkType}".ToString(CultureInfo.InvariantCulture);
        }
    }
}