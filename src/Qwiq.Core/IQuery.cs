using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    /// <summary>
    ///     Represents a query to the <see cref="IWorkItemStore" />.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        ///     Gets a collection of <see cref="IWorkItemLinkTypeEnd" /> objects associated with this query.
        /// </summary>
        /// <returns>IEnumerable&lt;IWorkItemLinkTypeEnd&gt;.</returns>
        IWorkItemLinkTypeEndCollection GetLinkTypes();

        /// <summary>
        ///     Executes a query that gets a collection of <see cref="IWorkItemLinkInfo" /> objects that satisfy the WIQL.
        /// </summary>
        /// <returns>IEnumerable&lt;IWorkItemLinkInfo&gt;.</returns>
        IEnumerable<IWorkItemLinkInfo> RunLinkQuery();

        /// <summary>
        ///     Executes a query that gets a collection of <see cref="IWorkItem" /> objects that satisfy the WIQL.
        /// </summary>
        /// <returns>IEnumerable&lt;IWorkItem&gt;.</returns>
        IWorkItemCollection RunQuery();
    }
}