namespace Qwiq
{
    // ReSharper disable InconsistentNaming
    public static class IWorkItemLinkTypeExtensions
    // ReSharper restore InconsistentNaming
    {
        ///<summary>
        /// Gets the Id for the link type's forward end.
        /// </summary>
        /// <param name="item">An instance of <see cref="IWorkItemLinkType"/>.</param>
        /// <returns>0 if no link or link type; otherwise, the link type id.</returns>
        /// <seealso cref="IWorkItemLinkTypeEndExtensions"/>
        public static int ForwardEndLinkTypeId(this IWorkItemLinkType item)
        {
            return item?.ForwardEnd.LinkTypeId() ?? 0;
        }

        ///<summary>
        /// Gets the Id for the link type's reverse end.
        /// </summary>
        /// <param name="item">An instance of <see cref="IWorkItemLinkType"/>.</param>
        /// <returns>0 if no link or link type; otherwise, the link type id.</returns>
        /// <seealso cref="IWorkItemLinkTypeEndExtensions"/>
        public static int ReverseEndLinkTypeId(this IWorkItemLinkType item)
        {
            return item?.ReverseEnd.LinkTypeId() ?? 0;
        }
    }
}
