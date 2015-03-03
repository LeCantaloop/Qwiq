using System.Collections.Generic;
using System.Linq;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    /// <summary>
    /// Wrapper around the TFS RevisionProxy. This exists so that every agent doesn't need to reference
    /// all the TFS libraries.
    /// </summary>
    public class RevisionProxy : IRevision
    {
        private readonly Tfs.Revision _rev;
        internal RevisionProxy(Tfs.Revision revision)
        {
            _rev = revision;
        }

        /// <summary>
        /// Gets the attachments of the work item in this revision.
        /// </summary>
        public Tfs.AttachmentCollection Attachments
        {
            get { return _rev.Attachments; }
        }

        /// <summary>
        /// Gets the fields of the work item in this revision.
        /// </summary>
        public IDictionary<string, object> Fields
        {
            get
            {
                return _rev.Fields.Cast<Tfs.Field>().ToDictionary(field => field.Name, field => field.Value);
            }
        }

        /// <summary>
        /// Gets the index of this revision.
        /// </summary>
        public int Index
        {
            get { return _rev.Index; }
        }

        /// <summary>
        /// Gets the links of the work item in this revision.
        /// </summary>
        public Tfs.LinkCollection Links
        {
            get { return _rev.Links; }
        }

        /// <summary>
        /// Gets the work item that is stored in this revision.
        /// </summary>
        public WorkItemProxy WorkItemProxy
        {
            get { return new WorkItemProxy(_rev.WorkItem); }
        }

        /// <summary>
        /// Gets the value of the specified field in the work item of this revision.
        /// </summary>
        /// <param name="name">The field of interest in the work item of this revision.</param>
        /// <returns>The value of the specified field.</returns>
        public object this[string name]
        {
            get { return _rev[name]; }
        }

        /// <summary>
        /// Gets the tagline for this revision.
        /// </summary>
        /// <returns>Returns System.String.</returns>
        public string GetTagLine()
        {
            return _rev.GetTagLine();
        }
    }
}
