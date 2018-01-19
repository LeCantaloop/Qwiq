namespace Microsoft.Qwiq
{
    public interface IWorkItemLinkType : INamed
    {
        /// <summary>
        ///     Link type of the link at the source work item. The <see cref="IWorkItemLinkTypeEnd.Name" /> appears when you add
        ///     links to the source work item.
        /// </summary>
        IWorkItemLinkTypeEnd ForwardEnd { get; }

        bool IsActive { get; }

        bool IsDirectional { get; }

        /// <summary>
        ///     Name of the link type. This name is used internally when you create a link between two work items.
        /// </summary>
        string ReferenceName { get; }

        /// <summary>
        ///     Link type of the link at the target work item.  The <see cref="IWorkItemLinkTypeEnd.Name" /> appears when a listing
        ///     of the links at the target work item appears.
        /// </summary>
        IWorkItemLinkTypeEnd ReverseEnd { get; }
    }
}