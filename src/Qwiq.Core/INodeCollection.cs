using System;

namespace Microsoft.Qwiq
{
    public interface INodeCollection : IReadOnlyObjectWithIdCollection<INode, int>, IEquatable<INodeCollection>
    {
    }
}