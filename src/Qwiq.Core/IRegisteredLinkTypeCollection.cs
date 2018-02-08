using System;

namespace Qwiq
{
    public interface IRegisteredLinkTypeCollection : IReadOnlyObjectWithNameCollection<IRegisteredLinkType>, IEquatable<IRegisteredLinkTypeCollection>
    {
    }
}