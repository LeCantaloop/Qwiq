using System.Linq;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class NodeProxy
    {
        internal NodeProxy(Tfs.Node node)
        {
            Id = node.Id;
            HasChildNodes = node.HasChildNodes;
            IsAreaNode = node.IsAreaNode;
            IsIterationNode = node.IsIterationNode;
            Name = node.Name;
            Path = node.Path;
            Uri = node.Uri;
            ChildNodes = node.ChildNodes.Cast<Tfs.Node>().Select(item => new NodeProxy(item));
            ParentNode = node.ParentNode != null ? new NodeProxy(node.ParentNode) : null;
        }
    }
}