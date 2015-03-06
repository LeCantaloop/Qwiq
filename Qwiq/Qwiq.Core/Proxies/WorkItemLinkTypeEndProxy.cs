using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Proxies
{
    public class WorkItemLinkTypeEndProxy : IWorkItemLinkTypeEnd
    {
        internal readonly Tfs.WorkItemLinkTypeEnd End;

        internal WorkItemLinkTypeEndProxy(Tfs.WorkItemLinkTypeEnd end)
        {
            End = end;
        }

        public int Id
        {
            get { return End.Id; }
        }

        public string ImmutableName
        {
            get { return End.ImmutableName; }
        }

        public bool IsForwardLink
        {
            get { return End.IsForwardLink; }
        }

        public IWorkItemLinkType LinkType
        {
            get { return new WorkItemLinkTypeProxy(End.LinkType); }
        }

        public string Name
        {
            get { return End.Name; }
        }

        public IWorkItemLinkTypeEnd OppositeEnd
        {
            get { return new WorkItemLinkTypeEndProxy(End.OppositeEnd); }
        }
    }
}