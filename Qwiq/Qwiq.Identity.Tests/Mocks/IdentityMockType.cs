using Microsoft.IE.Qwiq.Identity.Attributes;
using Microsoft.IE.Qwiq.Mapper.Attributes;

namespace Qwiq.Identity.Tests.Mocks
{
    public class MockIdentityType
    {
        public const string BackingField = "Identity Field";

        [FieldDefinition(BackingField)]
        [IdentityField]
        public string AnIdentity { get; set; }
    }
}
