namespace Microsoft.IE.Qwiq
{
    public interface IRelatedLink : ILink
    {
        int RelatedWorkItemId { get; }
        WorkItemLinkDirection LinkDirection { get; }
        string LinkSubType { get; }
    }
}