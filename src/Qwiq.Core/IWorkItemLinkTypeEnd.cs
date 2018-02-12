namespace Qwiq
{
    public interface IWorkItemLinkTypeEnd : INamed
    {
        string ImmutableName { get; }
        bool IsForwardLink { get; }
        IWorkItemLinkType LinkType { get; }
        IWorkItemLinkTypeEnd OppositeEnd { get; }
    }
}
