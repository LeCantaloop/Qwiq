using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public class WorkItemLinkTypeProxy : IWorkItemLinkType
    {
        private readonly Tfs.WorkItemLinkType _linkType;

        internal WorkItemLinkTypeProxy(Tfs.WorkItemLinkType linkType)
        {
            _linkType = linkType;
        }

        public IWorkItemLinkTypeEnd ForwardEnd
        {
            get { return new WorkItemLinkTypeEndProxy(_linkType.ForwardEnd); }
        }

        public bool IsActive
        {
            get { return _linkType.IsActive; }
        }

        public string ReferenceName
        {
            get { return _linkType.ReferenceName; }
        }

        public IWorkItemLinkTypeEnd ReverseEnd
        {
            get { return new WorkItemLinkTypeEndProxy(_linkType.ReverseEnd); }
        }
    }
}