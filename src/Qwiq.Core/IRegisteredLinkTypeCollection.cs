using System;

namespace Microsoft.Qwiq
{
    public interface IRegisteredLinkTypeCollection : IReadOnlyCollection<IRegisteredLinkType>, IEquatable<IRegisteredLinkTypeCollection>
    {
    }
}