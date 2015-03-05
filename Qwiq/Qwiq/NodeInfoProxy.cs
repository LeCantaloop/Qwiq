namespace Microsoft.IE.Qwiq
{
    public class NodeInfoProxy : INodeInfo
    {
        private readonly TeamFoundation.Server.NodeInfo _nodeInfo;

        public NodeInfoProxy(TeamFoundation.Server.NodeInfo nodeInfo)
        {
            _nodeInfo = nodeInfo;
        }

        public string Uri
        {
            get { return _nodeInfo.Uri; }
            set { _nodeInfo.Uri = value; }
        }
    }
}
