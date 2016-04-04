using System.Linq;
using System.Reflection;
using Microsoft.IE.Qwiq.Identity.Attributes;
using Microsoft.IE.Qwiq.Mapper;
using Microsoft.IE.Qwiq.Mapper.Attributes;

namespace Microsoft.IE.Qwiq.Identity.Mapper
{
    public class IdentityAwareAttributeMapperStrategy : AttributeMapperStrategy
    {
        private readonly IPropertyInspector _inspector;
        private readonly IIdentityManagementService _identityService;

        public IdentityAwareAttributeMapperStrategy(IPropertyInspector inspector, ITypeParser typeParser,
            IIdentityManagementService identityService) : base(inspector, typeParser)
        {
            _inspector = inspector;
            _identityService = identityService;
        }

        protected override object ParseValue(PropertyInfo property, object value)
        {
            value = base.ParseValue(property, value);
            var identityField = _inspector.GetAttribute<IdentityFieldAttribute>(property);
            if (identityField != null && value != null)
            {
                value = TryGetAlias(value.ToString());
            }
            return value;
        }

        /// <summary>
        /// Try to resolve a display name as an alias. If the display name cannot be resolved, return the display name.
        /// </summary>
        /// <param name="displayName">The display name to try to resolve</param>
        /// <returns>
        /// The alias (without the domain) corresponding to the display name. If the display name cannot be resolved it is returned as-is
        /// </returns>
        private string TryGetAlias(string displayName)
        {
            // identity service throws an exception if you try to resolve null or empty-string
            return !string.IsNullOrWhiteSpace(displayName)
                ? _identityService.GetAliasesForDisplayName(displayName).FirstOrDefault()
                : null;
        }
    }
}
