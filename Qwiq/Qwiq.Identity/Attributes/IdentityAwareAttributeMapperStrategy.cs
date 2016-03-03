using System.Linq;
using System.Reflection;
using Microsoft.IE.Qwiq.Mapper;
using Microsoft.IE.Qwiq.Mapper.Attributes;

namespace Microsoft.IE.Qwiq.Identity.Attributes
{
    public class IdentityAwareAttributeMapperStrategy : AttributeMapperStrategy
    {
        private readonly IPropertyInspector _inspector;
        private readonly IIdentityManagementService _identityMapper;

        public IdentityAwareAttributeMapperStrategy(IPropertyInspector inspector, ITypeParser typeParser,
            IIdentityManagementService identityMapper) : base(inspector, typeParser)
        {
            _inspector = inspector;
            _identityMapper = identityMapper;
        }

        protected override object ParseValue(PropertyInfo property, object value)
        {
            value = base.ParseValue(property, value);
            var identityField = _inspector.GetAttribute<IdentityFieldAttribute>(property);
            if (identityField != null && value != null)
            {
                var displayName = value.ToString();
                var alias = _identityMapper
                                .GetAliasesForDisplayName(displayName)
                                .FirstOrDefault();

                if (!string.IsNullOrEmpty(alias))
                {
                    value = alias;
                }
            }
            return value;
        }
    }
}