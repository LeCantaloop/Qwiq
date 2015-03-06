namespace Microsoft.IE.Qwiq
{
    public interface IWorkItemLinkType
    {
        IWorkItemLinkTypeEnd ForwardEnd { get; }
        bool IsActive { get; }
        string ReferenceName { get; }
        IWorkItemLinkTypeEnd ReverseEnd { get; }
    }
}