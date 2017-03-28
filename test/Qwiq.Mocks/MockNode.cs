using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    public class MockNode : Microsoft.Qwiq.Proxies.NodeProxy
    {
        public MockNode(string name, bool isArea, bool isIteration)
        {
            Name = name;
            IsAreaNode = isArea;
            IsIterationNode = isIteration;
            ChildNodes = Enumerable.Empty<INode>();
            ParentNode = null;
            Path = ((ParentNode?.Path ?? string.Empty) + "\\" + Name).Trim('\\');
        }
    }
}
