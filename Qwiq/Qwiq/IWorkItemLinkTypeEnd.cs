namespace Microsoft.IE.Qwiq
{
    public interface IWorkItemLinkTypeEnd
    {
        int Id { get; }
        string ImmutableName { get; }
        bool IsForwardLink { get; }
        IWorkItemLinkType LinkType { get; }
        string Name { get; }
        IWorkItemLinkTypeEnd OppositeEnd { get; }
    }
}