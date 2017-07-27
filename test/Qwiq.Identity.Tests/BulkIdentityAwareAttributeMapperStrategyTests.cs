using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Qwiq.Identity.Mocks;
using Microsoft.Qwiq.Mapper;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mocks;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;
using MockIdentityDescriptor = Microsoft.Qwiq.Mocks.MockIdentityDescriptor;

namespace Microsoft.Qwiq.Identity
{
    public abstract class BulkIdentityAwareAttributeMapperStrategyTests : ContextSpecification
    {
        private IWorkItemMapperStrategy _strategy;
        private Dictionary<IWorkItem, IIdentifiable<int?>> _workItemMappings;
        protected IDictionary<string, IEnumerable<ITeamFoundationIdentity>> Identities { get; set; }

        protected MockIdentityType Actual
        {
            get { return _workItemMappings.Select(kvp => kvp.Value).Cast<MockIdentityType>().Single(); }
        }

        protected string IdentityFieldBackingValue { get; set; }

        public override void Given()
        {
            var propertyInspector = new PropertyInspector(new PropertyReflector());
            _strategy = new BulkIdentityAwareAttributeMapperStrategy(
                            propertyInspector,
                            Identities == null
                                ? new MockIdentityManagementService()
                                : new MockIdentityManagementService(Identities));
            var sourceWorkItems = new[]
            {
                new MockWorkItem(
                    new MockWorkItemType("Baz", MockIdentityType.BackingField),
                    new Dictionary<string, object>
                    {
                        { MockIdentityType.BackingField, IdentityFieldBackingValue }
                    })
            };

            _workItemMappings = sourceWorkItems.ToDictionary(k => (IWorkItem)k, e => (IIdentifiable<int?>)new MockIdentityType());
        }

        public override void When()
        {
            _strategy.Map(typeof(MockIdentityType), _workItemMappings, null);
        }
    }

    [TestClass]
    public class when_a_backing_source_is_the_empty_string_the_field_should_not_be_mapped : BulkIdentityAwareAttributeMapperStrategyTests
    {
        public override void Given()
        {
            IdentityFieldBackingValue = string.Empty;
            base.Given();
        }

        [TestMethod]
        public void the_actual_identity_value_should_be_null()
        {
            Actual.AnIdentity.ShouldBeNull();
        }
    }

    [TestClass]
    public class when_the_backing_source_does_result_in_a_resolved_identity : BulkIdentityAwareAttributeMapperStrategyTests
    {
        private const string IdentityAlias = "jsmit";
        private const string IdentityDisplay = "Joe Smit";
        public override void Given()
        {
            IdentityFieldBackingValue = IdentityDisplay;
            Identities = new Dictionary<string, IEnumerable<ITeamFoundationIdentity>>
            {
                {IdentityFieldBackingValue, new []{new MockTeamFoundationIdentity(MockIdentityDescriptor.Create(IdentityAlias), IdentityDisplay, Guid.Empty) }}
            };
            base.Given();
        }

        [TestMethod]
        public void the_actual_identity_value_should_be_the_identity_alias()
        {
            Actual.AnIdentity.ShouldEqual(IdentityAlias);
        }

        [TestMethod]
        public void set_on_an_identity_property_should_be_called_once()
        {
            Actual.AnIdentitySetCount.ShouldEqual(1);
        }

        [TestMethod]
        public void the_IdentityFieldValue_contains_expected_value()
        {
            Actual.AnIdentityValue.ShouldNotBeNull();
            Actual.AnIdentityValue.DisplayName.ShouldEqual(IdentityDisplay);
            Actual.AnIdentityValue.IdentityName.ShouldEqual(IdentityAlias);
        }
    }

    [TestClass]
    public class given_a_work_item_with_defined_fields_when_the_field_names_to_properties_are_retrieved : ContextSpecification
    {
        private readonly Type _identityType = typeof(MockIdentityType);
        private Dictionary<string, List<PropertyInfo>> Expected { get; set; }
        private Dictionary<string, List<PropertyInfo>> Actual { get; set; }

        public override void Given()
        {
            Expected = new Dictionary<string, List<PropertyInfo>>
            {
                [MockIdentityType.BackingField] = new List<PropertyInfo> { _identityType.GetProperty(nameof(MockIdentityType.AnIdentity)), _identityType.GetProperty(nameof(MockIdentityType.AnIdentityValue)) },
                [MockIdentityType.NonExistantField] = new List<PropertyInfo> { _identityType.GetProperty(nameof(MockIdentityType.NonExistant)) },
                [MockIdentityType.UriIdentityField] = new List<PropertyInfo> { _identityType.GetProperty(nameof(MockIdentityType.UriIdentity)) }
            };
        }

        public override void When()
        {
            Actual =
                BulkIdentityAwareAttributeMapperStrategy.GetWorkItemIdentityFieldNameToIdentityPropertyMap(
                    _identityType, new PropertyInspector(new PropertyReflector()));
            base.When();
        }

        [TestMethod]
        public void only_valid_fields_and_properties_are_retrieved()
        {
            Actual.Count.ShouldEqual(Expected.Count);
            Actual[MockIdentityType.BackingField].ShouldContainOnly(Expected[MockIdentityType.BackingField]);
            Actual[MockIdentityType.NonExistantField].ShouldContainOnly(Expected[MockIdentityType.NonExistantField]);
            Actual[MockIdentityType.UriIdentityField].ShouldContainOnly(Expected[MockIdentityType.UriIdentityField]);
        }
    }


}

