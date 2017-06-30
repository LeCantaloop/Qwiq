using System;

namespace Microsoft.Qwiq.Mapper.Attributes
{
    /// <summary>
    /// Represents a class designating a property as an identity value.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IdentityFieldAttribute : Attribute
    {
    }
}
