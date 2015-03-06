using System;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public class WorkItemLinkProxy : IWorkItemLink
    {
        private readonly Tfs.WorkItemLink _link;

        internal WorkItemLinkProxy(Tfs.WorkItemLink link)
        {
            _link = link;
        }

        public string Comment
        {
            get { return _link.Comment; }
        }

        public string AddedBy
        {
            get { return _link.AddedBy; }
        }

        public DateTime AddedDate
        {
            get { return DateTime.SpecifyKind(_link.AddedDateUtc, DateTimeKind.Utc); }
        }

        public DateTime? ChangedDate
        {
            get { return _link.ChangedDate; }
            set { _link.ChangedDate = value; }
        }

        public WorkItemLinkDirection LinkDirection
        {
            get { return _link.LinkTypeEnd.IsForwardLink ? WorkItemLinkDirection.Forward : WorkItemLinkDirection.Reverse; }
        }

        public int SourceId
        {
            get { return _link.SourceId; }
            set { _link.SourceId = value; }
        }

        public int TargetId
        {
            get { return _link.TargetId; }
            set { _link.TargetId = value; }
        }
    }
}