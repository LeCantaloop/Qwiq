namespace Microsoft.IE.Qwiq
{
    public interface IWorkItemLinkInfo
    {
        bool IsLocked { get; }
        int LinkTypeId { get; }
        int SourceId { get; }
        int TargetId { get; }
    }
}