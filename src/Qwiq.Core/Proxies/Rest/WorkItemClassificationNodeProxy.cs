using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemClassificationNodeProxy : INode
    {
        private readonly Lazy<string> _path;

        public WorkItemClassificationNodeProxy(WorkItemClassificationNode node)
        {
            Id = node.Id;
            IsAreaNode = node.StructureType == TreeNodeStructureType.Area;
            IsIterationNode = !IsAreaNode;
            Name = node.Name;
            HasChildNodes = node.Children?.Any() ?? false;
            ChildNodes = InitializeChildNodes(this, node.Children);

            _path = new Lazy<string>(()=> ((ParentNode?.Path ?? string.Empty) + "\\" + Name).Trim('\\'));
        }

        private IEnumerable<INode> InitializeChildNodes(WorkItemClassificationNodeProxy t, IEnumerable<WorkItemClassificationNode> children)
        {
            var c = new List<INode>();
            if (children != null)
            {
                c.AddRange(children.Select(child => new WorkItemClassificationNodeProxy(child) { ParentNode = this }));
            }
            return c;
        }

        public IEnumerable<INode> ChildNodes { get; }

        public bool HasChildNodes { get; }

        public int Id { get; }

        public bool IsAreaNode { get; }

        public bool IsIterationNode { get; }

        public string Name { get; }

        public INode ParentNode { get; private set; }

        public string Path => _path.Value;

        public Uri Uri { get; }

        public override string ToString()
        {
            return Path;
        }
    }
}