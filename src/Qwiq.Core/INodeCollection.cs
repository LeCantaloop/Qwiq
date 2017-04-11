using System;

namespace Microsoft.Qwiq
{
    public interface INodeCollection : IReadOnlyListWithId<INode, int>, IEquatable<INodeCollection>
    {
    }
}