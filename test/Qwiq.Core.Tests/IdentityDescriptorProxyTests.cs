using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;



namespace Microsoft.Qwiq.Core.Tests
{
    public abstract class IdentityDescriptorProxyTests : ContextSpecification
    {
        protected Soap.IdentityDescriptor IdentityDescriptorProxy;
        protected TeamFoundation.Framework.Client.IdentityDescriptor IdentityDescriptor;

        public override void When()
        {
            IdentityDescriptorProxy = new Soap.IdentityDescriptor(IdentityDescriptor);
        }
    }

    [TestClass]
    public class given_an_IdentityDescriptor_when_a_proxy_is_created : IdentityDescriptorProxyTests
    {
        private const string Identifier = "identifier";
        private const string IdentityType = "identityType";

        public override void Given()
        {
            IdentityDescriptor = new TeamFoundation.Framework.Client.IdentityDescriptor(IdentityType, Identifier);
        }

        [TestMethod]
        public void the_identifier_can_be_retrieved()
        {
            IdentityDescriptorProxy.Identifier.ShouldEqual(Identifier);
        }

        [TestMethod]
        public void the_identity_type_can_be_retrieve()
        {
            IdentityDescriptorProxy.IdentityType.ShouldEqual(IdentityType);
        }
    }
}

