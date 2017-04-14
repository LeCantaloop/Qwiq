using System;

namespace Microsoft.Qwiq
{
    public interface IRegisteredLinkTypeCollection : IReadOnlyList<IRegisteredLinkType>, IEquatable<IRegisteredLinkTypeCollection>
    {
    }
}