namespace Microsoft.Qwiq
{
    // ReSharper disable InconsistentNaming
    public static class IWorkItemLinkInfoExtensions
    // ReSharper restore InconsistentNaming
    {
        /// <summary>
        /// Gets an Id for a <see cref="IWorkItemLinkTypeEnd"/>.
        /// </summary>
        /// <param name="item">A <see cref="IWorkItemLinkInfo"/> with a <see cref="IWorkItemLinkTypeEnd"/></param>
        /// <returns>0 if no link or link type; otherwise, the link type id.</returns>
        /// <remarks>
        /// A true Id is only returned for SOAP instances of <paramref name="item"/>.
        /// </remarks>
        /// <seealso cref="IWorkItemLinkTypeEndExtensions"/>
        public static int LinkTypeId(this IWorkItemLinkInfo item)
        {
            if (item == null) return 0;

            // In SOAP, WorkItemLinkInfo is IIdentifiable<int>, where the Id is the LinkTypeId
            if (item is IIdentifiable<int> i)
            {
                return i.Id;
            }

            return item.LinkType.LinkTypeId();
        }
    }
}
