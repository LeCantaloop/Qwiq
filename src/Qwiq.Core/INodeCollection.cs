using System;

namespace Microsoft.Qwiq
{
    public interface INodeCollection : IReadOnlyObjectWithIdList<INode, int>, IEquatable<INodeCollection>
    {
    }
}