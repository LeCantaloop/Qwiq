using System;

namespace Microsoft.Qwiq
{
    public interface IRegisteredLinkTypeCollection : IReadOnlyObjectWithNameCollection<IRegisteredLinkType>, IEquatable<IRegisteredLinkTypeCollection>
    {
    }
}