using System;
using System.Linq;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Rest.Proxies
{
    internal class Node : Qwiq.Node
    {
        internal Node(WorkItemClassificationNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            Id = node.Id;
            IsAreaNode = node.StructureType == TreeNodeStructureType.Area;
            IsIterationNode = !IsAreaNode;
            Name = node.Name;
            HasChildNodes = node.Children?.Any() ?? false;
            ChildNodes = HasChildNodes
                             ? node.Children.Select(s => new Node(s) { ParentNode = this }).ToList()
                             : Enumerable.Empty<INode>();

            Path = ((ParentNode?.Path ?? string.Empty) + "\\" + Name).Trim('\\');
        }
    }
}