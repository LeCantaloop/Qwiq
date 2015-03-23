namespace Microsoft.IE.Qwiq
{
    public interface IRelatedLink : ILink
    {
        int RelatedWorkItemId { get; }
        IWorkItemLinkTypeEnd LinkTypeEnd { get; }
        string LinkSubType { get; }
    }
}
