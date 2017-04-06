using System;
using System.Linq;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class Node : Qwiq.Node
    {
        internal Node(Tfs.Node node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            Id = node.Id;
            HasChildNodes = node.HasChildNodes;
            IsAreaNode = node.IsAreaNode;
            IsIterationNode = node.IsIterationNode;
            Name = node.Name;
            Path = node.Path;
            Uri = node.Uri;
            ChildNodes = node.ChildNodes.Cast<Tfs.Node>().Select(item => new Node(item));
            ParentNode = node.ParentNode != null ? new Node(node.ParentNode) : null;
        }
    }
}