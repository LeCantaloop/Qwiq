using System;

namespace Microsoft.Qwiq
{
    public interface IRegisteredLinkTypeCollection : IReadOnlyObjectList<IRegisteredLinkType>, IEquatable<IRegisteredLinkTypeCollection>
    {
    }
}