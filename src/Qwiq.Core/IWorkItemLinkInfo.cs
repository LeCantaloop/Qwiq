using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemLinkInfo : IEquatable<IWorkItemLinkInfo>
    {
        IWorkItemLinkTypeEnd LinkType { get; }

        int SourceId { get; }

        int TargetId { get; }
    }
}