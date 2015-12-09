using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.Qwiq.Exceptions;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Proxies
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
        public IEnumerable<IAttachment> Attachments
        {
            get { return _rev.Attachments.Cast<Tfs.Attachment>().Select(item => ExceptionHandlingDynamicProxyFactory.Create<IAttachment>(new AttachmentProxy(item))); }
        }

        /// <summary>
        /// Gets the fields of the work item in this revision.
        /// </summary>
        public IDictionary<string, IField> Fields
        {
            get
            {
                return _rev.Fields.Cast<Tfs.Field>().ToDictionary(field => field.Name, field => ExceptionHandlingDynamicProxyFactory.Create<IField>(new FieldProxy(field)) as IField);
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
        public IEnumerable<ILink> Links
        {
            get { return _rev.Links.Cast<Tfs.Link>().Select(item => ExceptionHandlingDynamicProxyFactory.Create<ILink>(new LinkProxy(item))); }
        }

        /// <summary>
        /// Gets the work item that is stored in this revision.
        /// </summary>
        public IWorkItem WorkItem
        {
            get { return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItemProxy(_rev.WorkItem)); }
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
