using System;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies
{
    public class AttachmentProxy : IAttachment
    {
        private readonly Tfs.Attachment _attachment;

        internal AttachmentProxy(Tfs.Attachment attachment)
        {
            _attachment = attachment;
        }

        public DateTime AttachedTime
        {
            get { return DateTime.SpecifyKind(_attachment.AttachedTimeUtc, DateTimeKind.Utc); }
        }

        public string Comment
        {
            get { return _attachment.Comment; }
            set { _attachment.Comment = value; }
        }

        public DateTime CreationTime
        {
            get { return DateTime.SpecifyKind(_attachment.CreationTimeUtc, DateTimeKind.Utc); }
        }

        public string Extension
        {
            get { return _attachment.Extension; }
        }

        public bool IsSaved
        {
            get { return _attachment.IsSaved; }
        }

        public DateTime LastWriteTime
        {
            get { return DateTime.SpecifyKind(_attachment.LastWriteTimeUtc, DateTimeKind.Utc); }
        }

        public long Length
        {
            get { return _attachment.Length; }
        }

        public string Name
        {
            get { return _attachment.Name; }
        }

        public Uri Uri
        {
            get { return _attachment.Uri; }
        }
    }
}
