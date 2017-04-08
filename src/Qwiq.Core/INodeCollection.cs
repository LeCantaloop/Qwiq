using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface INodeCollection : IReadOnlyCollection<INode>
    {
        INode this[string name] { get; }

        bool Contains(string name);
    }
}