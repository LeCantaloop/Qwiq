using System;
using System.Linq;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest
{
    internal class Node : Qwiq.Node
    {
        private readonly Lazy<Uri> _uri;

        internal Node(WorkItemClassificationNode node, INode parentNode = null)
            : base(
                node.Id,
                node.StructureType == TreeNodeStructureType.Area,
                node.StructureType == TreeNodeStructureType.Iteration,
                node.Name,
                () => parentNode,
                n => node.Children?.Any() ?? false
                         ? node.Children.Select(s => new Node(s, n)).ToList()
                         : Enumerable.Empty<INode>())
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            _uri = new Lazy<Uri>(() => new Uri(node.Url));
        }

        public override Uri Uri => _uri.Value;
    }
}