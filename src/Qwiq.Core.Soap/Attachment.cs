using System;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    public class Attachment : IAttachment
    {
        private readonly Tfs.Attachment _attachment;

        internal Attachment(Tfs.Attachment attachment)
        {
            _attachment = attachment;
        }

        public DateTime AttachedTime => DateTime.SpecifyKind(_attachment.AttachedTimeUtc, DateTimeKind.Utc);

        public string Comment
        {
            get => _attachment.Comment;
            set => _attachment.Comment = value;
        }

        public DateTime CreationTime => DateTime.SpecifyKind(_attachment.CreationTimeUtc, DateTimeKind.Utc);

        public string Extension => _attachment.Extension;

        public bool IsSaved => _attachment.IsSaved;

        public DateTime LastWriteTime => DateTime.SpecifyKind(_attachment.LastWriteTimeUtc, DateTimeKind.Utc);

        public long Length => _attachment.Length;

        public string Name => _attachment.Name;

        public Uri Uri => _attachment.Uri;
    }
}
