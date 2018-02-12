using System;

using Tfs = Microsoft.TeamFoundation.Server;

namespace Qwiq.Client.Soap
{
    internal class NodeInfo : INodeInfo
    {
        private readonly Tfs.NodeInfo _nodeInfo;

        internal NodeInfo(Tfs.NodeInfo nodeInfo)
        {
            _nodeInfo = nodeInfo ?? throw new ArgumentNullException(nameof(nodeInfo));
        }

        public string Uri
        {
            get => _nodeInfo.Uri;
            set => _nodeInfo.Uri = value;
        }
    }
}

