namespace Microsoft.IE.Qwiq
{
    public class WorkItemLinkInfoProxy : IWorkItemLinkInfo
    {
        private readonly TeamFoundation.WorkItemTracking.Client.WorkItemLinkInfo _item;

        public WorkItemLinkInfoProxy(TeamFoundation.WorkItemTracking.Client.WorkItemLinkInfo item)
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