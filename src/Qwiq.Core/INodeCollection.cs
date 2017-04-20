using System;

namespace Microsoft.Qwiq
{
    public interface INodeCollection : IReadOnlyCollectionWithId<INode, int>, IEquatable<INodeCollection>
    {
    }
}