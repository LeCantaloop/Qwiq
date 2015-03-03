using System.Collections.Generic;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public interface IRevision
    {
        /// <summary>
        /// Gets the attachments of the work item in this revision.
        /// </summary>
        Tfs.AttachmentCollection Attachments { get; }

        /// <summary>
        /// Gets the fields of the work item in this revision.
        /// </summary>
        IDictionary<string, object> Fields { get; }

        /// <summary>
        /// Gets the index of this revision.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the links of the work item in this revision.
        /// </summary>
        IEnumerable<ILink> Links { get; }

        /// <summary>
        /// Gets the work item that is stored in this revision.
        /// </summary>
        WorkItemProxy WorkItemProxy { get; }

        /// <summary>
        /// Gets the value of the specified field in the work item of this revision.
        /// </summary>
        /// <param name="name">The field of interest in the work item of this revision.</param>
        /// <returns>The value of the specified field.</returns>
        object this[string name] { get; }

        /// <summary>
        /// Gets the tagline for this revision.
        /// </summary>
        /// <returns>Returns System.String.</returns>
        string GetTagLine();
    }
}
