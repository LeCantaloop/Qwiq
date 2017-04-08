using System;
using System.Linq;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class Node : Qwiq.Node
    {
        internal Node(Tfs.Node node)
            : base(
                node.Id,
                node.IsAreaNode,
                node.IsIterationNode,
                node.Name,
                () => node.ParentNode != null ? new Node(node.ParentNode) : null,
                n => node.ChildNodes.Cast<Tfs.Node>().Select(item => new Node(item)).ToList())
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            Path = node.Path;
            Uri = node.Uri;
        }

        public override string Path { get; }

        public override Uri Uri { get; }
    }
}