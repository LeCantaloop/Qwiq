using System;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public interface IWorkItemLinkInfo : IEquatable<IWorkItemLinkInfo>
    {
        [CanBeNull]
        IWorkItemLinkTypeEnd LinkType { get; }

        int SourceId { get; }

        int TargetId { get; }
    }
}