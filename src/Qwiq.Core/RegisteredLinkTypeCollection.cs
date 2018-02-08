using System;
using System.Collections.Generic;

namespace Qwiq
{
    public class RegisteredLinkTypeCollection : ReadOnlyObjectWithNameCollection<IRegisteredLinkType>, IRegisteredLinkTypeCollection
    {
        internal RegisteredLinkTypeCollection(List<IRegisteredLinkType> linkTypes)
            : base(linkTypes, type => type.Name)
        {
        }

        public bool Equals(IRegisteredLinkTypeCollection other)
        {
            // TODO: Implement Equality Comparer
            throw new NotImplementedException();
        }
    }
}