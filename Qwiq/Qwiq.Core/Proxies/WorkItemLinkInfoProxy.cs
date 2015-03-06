using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq
{
    public class WorkItemLinkInfoProxy : IWorkItemLinkInfo
    {
        private readonly Tfs.WorkItemLinkInfo _item;

        internal WorkItemLinkInfoProxy(Tfs.WorkItemLinkInfo item)
        {
            _item = item;
        }

        public bool IsLocked
        {
            get { return _item.IsLocked; }
        }

        public int LinkTypeId
        {
            get { return _item.LinkTypeId; }
        }

        public int SourceId
        {
            get { return _item.SourceId; }
        }

        public int TargetId
        {
            get { return _item.TargetId; }
        }
    }
}