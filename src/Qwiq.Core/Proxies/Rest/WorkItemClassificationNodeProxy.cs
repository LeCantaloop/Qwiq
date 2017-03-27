using System;
using System.Linq;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies
{
    internal partial class NodeProxy
    {
        public NodeProxy(WorkItemClassificationNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            Id = node.Id;
            IsAreaNode = node.StructureType == TreeNodeStructureType.Area;
            IsIterationNode = !IsAreaNode;
            Name = node.Name;
            HasChildNodes = node.Children?.Any() ?? false;
            ChildNodes = HasChildNodes
                             ? node.Children.Select(s => new NodeProxy(s) { ParentNode = this }).ToList()
                             : Enumerable.Empty<INode>();

            Path = ((ParentNode?.Path ?? string.Empty) + "\\" + Name).Trim('\\');
        }
    }
}