using System;

namespace Qwiq
{
    public static class IWorkItemLinkTypeEndExtensions
    {
        /// <summary>
        /// Gets the Id from the specified <paramref name="item"/>.
        /// </summary>
        /// <param name="item">An instance of <see cref="IWorkItemLinkTypeEnd"/>.</param>
        /// <returns>
        /// 0 if <paramref name="item"/> is null or the <see cref="M:IWorkItemLinkTypeEnd.LinkType"/> is null; otherwise the link type id.
        /// </returns>
        /// <remarks>
        /// A true Id is only returned for SOAP instances of <paramref name="item"/>
        /// </remarks>
        public static int LinkTypeId(this IWorkItemLinkTypeEnd item)
        {
            // No link type. In SOAP this is equivilent to SELF and has a constant id of 0
            if (item == null)
            {
                return 0;
            }

            // In SOAP, the IWorkItemLinkTypeEnd is IIdentifiable<int>. Try to cast and return the Id
            if (item is IIdentifiable<int> i)
            {
                return i.Id;
            }

            // Same as initial case--no link type.
            if (item.LinkType == null)
            {
                return 0;
            }

            // Hack for REST: If there is an immutable name, get a case-insensitive hash
            if (!string.IsNullOrEmpty(item.ImmutableName))
            {
                var hash = Math.Abs(StringComparer.OrdinalIgnoreCase.GetHashCode(item.ImmutableName));
                // Forward links are ALWAYS a positive value
                if (item.IsForwardLink)
                {
                    return hash;
                }

                // Reverse links are ALWAYS a negative value
                return hash * -1;
            }

            return 0;
        }
    }
}
