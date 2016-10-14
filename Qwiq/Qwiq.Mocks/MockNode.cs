using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    public class MockNode : INode
    {
        public MockNode(string name, bool isArea, bool isIteration)
        {
            Name = name;
            IsAreaNode = isArea;
            IsIterationNode = isIteration;
            ChildNodes = Enumerable.Empty<INode>();
            ParentNode = null;
        }

        public IEnumerable<INode> ChildNodes { get; set; }

        public bool HasChildNodes => ChildNodes.Any();

        public int Id { get; }

        public bool IsAreaNode { get; }

        public bool IsIterationNode { get; }

        public string Name { get; }

        public INode ParentNode { get; set; }

        public string Path => ParentNode?.Path + "\\" + Name;

        public Uri Uri { get; }
    }
}
