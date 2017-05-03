using System;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class Node : Qwiq.Node
    {
        internal Node([NotNull] WorkItemClassificationNode node)
            : this(node, null)
        {
        }

        internal Node([NotNull] WorkItemClassificationNode node, [CanBeNull] INode parentNode)
            : base(
                   node.Id,
                   node.StructureType == TreeNodeStructureType.Area,
                   node.StructureType == TreeNodeStructureType.Iteration,
                   node.Name,
                   new Uri(node.Url),
                   () => parentNode,
                   n => node.Children?.Any() ?? false ? node.Children.Select(s => new Node(s, n)).ToList() : Enumerable.Empty<INode>())
        {
            Contract.Requires(node != null);
            
            if (node == null) throw new ArgumentNullException(nameof(node));
        }
    }
}