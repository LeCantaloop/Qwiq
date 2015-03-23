using System;

namespace Microsoft.IE.Qwiq
{
    public interface IWorkItemLink : ILink
    {
        string AddedBy { get; }
        DateTime AddedDate { get; }
        DateTime? ChangedDate { get; set; }
        IWorkItemLinkTypeEnd LinkTypeEnd { get; }
        int SourceId { get; set; }
        int TargetId { get; set; }
    }
}
