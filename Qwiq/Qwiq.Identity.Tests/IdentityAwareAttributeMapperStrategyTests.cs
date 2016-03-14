using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq;
using Microsoft.IE.Qwiq.Identity.Attributes;
using Microsoft.IE.Qwiq.Mapper;
using Microsoft.IE.Qwiq.Mapper.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Identity.Tests.Mocks;

namespace Qwiq.Identity.Tests
{
    public abstract class IdentityAwareAttributeMapperStrategyTests : ContextSpecification
    {
        private IWorkItemMapperStrategy _strategy;
        private IEnumerable<KeyValuePair<IWorkItem, object>> _workItemMappings;

        protected MockIdentityType Actual
        {
            get { return _workItemMappings.Select(kvp => kvp.Value).Cast<MockIdentityType>().Single(); }
        }

        protected string IdentityFieldBackingValue { get; set; }


        public override void Given()
        {
            var propertyInspector = new PropertyInspector(new PropertyReflector());
            var typeParser = new TypeParser();
            _strategy = new IdentityAwareAttributeMapperStrategy(propertyInspector, typeParser, new MockIdentityManagementService());
            var sourceWorkItems = new[]
            {
                new MockWorkItem
                {
                    Properties = new Dictionary<string, object>
                    {
                        { MockIdentityType.BackingField, IdentityFieldBackingValue }
                    }
                }
            };

            _workItemMappings = sourceWorkItems.Select(t => new KeyValuePair<IWorkItem, object>(t, new MockIdentityType())).ToList();
        }

        public override void When()
        {
            _strategy.Map(typeof (MockIdentityType), _workItemMappings, null);
        }
    }

    [TestClass]
    public class when_a_backing_source_is_the_empty_string_the_field_should_be_mapped_with_the_original_value : IdentityAwareAttributeMapperStrategyTests
    {
        public override void Given()
        {
            IdentityFieldBackingValue = string.Empty;
            base.Given();
        }

        [TestMethod]
        public void the_actual_identity_value_is_the_original_value()
        {
            Actual.AnIdentity.ShouldEqual(IdentityFieldBackingValue);
        }
    }
}
