using Microsoft.IE.Qwiq;

namespace Qwiq.Identity.Tests.Mocks
{
    public class MockIdentityDescriptor : IIdentityDescriptor
    {
        public MockIdentityDescriptor()
        {
        }

        public MockIdentityDescriptor(string alias) : this(alias, "doesn'tmatter")
        {
        }

        public MockIdentityDescriptor(string alias, string identityType)
        {
            Identifier = $"tenant\\{alias}@domain.com";
            IdentityType = identityType;
        }

        public string Identifier { get; set; }
        public string IdentityType { get; set; }
    }
}