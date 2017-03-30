using Tfs = Microsoft.TeamFoundation.Server;

namespace Microsoft.Qwiq.Soap
{
    internal class NodeInfo : INodeInfo
    {
        private readonly Tfs.NodeInfo _nodeInfo;

        internal NodeInfo(Tfs.NodeInfo nodeInfo)
        {
            _nodeInfo = nodeInfo;
        }

        public string Uri
        {
            get => _nodeInfo.Uri;
            set => _nodeInfo.Uri = value;
        }
    }
}

