﻿using Tfs = Microsoft.TeamFoundation.Server;

namespace Microsoft.IE.Qwiq
{
    public class NodeInfoProxy : INodeInfo
    {
        private readonly Tfs.NodeInfo _nodeInfo;

        internal NodeInfoProxy(Tfs.NodeInfo nodeInfo)
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