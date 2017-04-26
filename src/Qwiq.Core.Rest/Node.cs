using System;
using System.Linq;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class Node : Qwiq.Node
    {
        internal Node(WorkItemClassificationNode node, INode parentNode = null)
            : base(
                   node.Id,
                   node.StructureType == TreeNodeStructureType.Area,
                   node.StructureType == TreeNodeStructureType.Iteration,
                   node.Name,
                   new Uri(node.Url),
                   () => parentNode,
                   n => node.Children?.Any() ?? false ? node.Children.Select(s => new Node(s, n)).ToList() : Enumerable.Empty<INode>())
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
        }
    }
}