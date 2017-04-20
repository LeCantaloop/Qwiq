using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public class RegisteredLinkTypeCollection : ReadOnlyCollection<IRegisteredLinkType>, IRegisteredLinkTypeCollection
    {
        public RegisteredLinkTypeCollection(IEnumerable<IRegisteredLinkType> linkTypes)
            : base(linkTypes, type => type.Name)
        {
        }

        public bool Equals(IRegisteredLinkTypeCollection other)
        {
            throw new NotImplementedException();
        }
    }
}