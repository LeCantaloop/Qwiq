using System;
using System.Linq;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Client.Soap
{
    internal class Node : Qwiq.Node
    {
        internal Node(Tfs.Node node)
            : base(
                   node.Id,
                   node.IsAreaNode ? NodeType.Area : node.IsIterationNode ? NodeType.Iteration : NodeType.None,
                   node.Name,
                   node.Uri,
                   () => node.ParentNode != null ? new Node(node.ParentNode) : null,
                   n => node.ChildNodes.Cast<Tfs.Node>().Select(item => new Node(item)).ToList())
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            Path = node.Path;
        }

        public override string Path { get; }
    }
}