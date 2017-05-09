namespace Microsoft.Qwiq
{
    public interface IWorkItemLinkTypeEnd
    {
        string ImmutableName { get; }
        bool IsForwardLink { get; }
        IWorkItemLinkType LinkType { get; }
        string Name { get; }
        IWorkItemLinkTypeEnd OppositeEnd { get; }
    }
}
