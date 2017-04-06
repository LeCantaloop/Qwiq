using Microsoft.Qwiq.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    /// <summary>
    ///     Wrapper around the TFS RevisionProxy. This exists so that every agent doesn't need to reference
    ///     all the TFS libraries.
    /// </summary>
    internal class Revision : IRevision
    {
        private readonly Tfs.Revision _rev;

        internal Revision(Tfs.Revision revision)
        {
            _rev = revision ?? throw new ArgumentNullException(nameof(revision));
        }

        /// <summary>
        ///     Gets the attachments of the work item in this revision.
        /// </summary>
        public IEnumerable<IAttachment> Attachments
        {
            get
            {
                return _rev.Attachments.Cast<Tfs.Attachment>()
                           .Select(
                               item => ExceptionHandlingDynamicProxyFactory.Create<IAttachment>(new Attachment(item)));
            }
        }

        /// <summary>
        ///     Gets the fields of the work item in this revision.
        /// </summary>
        public IFieldCollection Fields => ExceptionHandlingDynamicProxyFactory.Create<IFieldCollection>(new FieldCollection(_rev.Fields));

        /// <summary>
        ///     Gets the index of this revision.
        /// </summary>
        public int Index => _rev.Index;

        /// <summary>
        ///     Gets the links of the work item in this revision.
        /// </summary>
        public IEnumerable<ILink> Links
        {
            get
            {
                return _rev.Links.Cast<Tfs.Link>()
                           .Select(item => ExceptionHandlingDynamicProxyFactory.Create<ILink>(new Link(item)));
            }
        }

        /// <summary>
        ///     Gets the work item that is stored in this revision.
        /// </summary>
        public IWorkItem WorkItem => ExceptionHandlingDynamicProxyFactory
            .Create<IWorkItem>(new WorkItem(_rev.WorkItem));

        /// <summary>
        ///     Gets the value of the specified field in the work item of this revision.
        /// </summary>
        /// <param name="name">The field of interest in the work item of this revision.</param>
        /// <returns>The value of the specified field.</returns>
        public object this[string name] => _rev[name];

        /// <summary>
        ///     Gets the tagline for this revision.
        /// </summary>
        /// <returns>Returns System.String.</returns>
        public string GetTagLine()
        {
            return _rev.GetTagLine();
        }
    }
}