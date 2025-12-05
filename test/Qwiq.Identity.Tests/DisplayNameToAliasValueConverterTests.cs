using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Qwiq.Mocks;
using Qwiq.Tests.Common;

using Should;

namespace Qwiq.Identity
{
    [TestClass]
    public abstract class DisplayNameToAliasValueConverterContextSpecification : ContextSpecification
    {
        protected DisplayNameToAliasValueConverter Instance { get; set; } = null!;
        protected IReadOnlyDictionary<string, object> Result { get; set; } = null!;
        protected string[] DisplayNames { get; set; } = Array.Empty<string>();
    }

    [TestClass]
    public class when_mapping_a_display_name_with_multiple_matching_identities : DisplayNameToAliasValueConverterContextSpecification
    {
        private Exception? _thrownException;

        public override void Given()
        {
            var identity1 = new MockTeamFoundationIdentity(
                MockIdentityDescriptor.Create("user1", "domain", "tenant"),
                "John Smith",
                Guid.NewGuid());

            var identity2 = new MockTeamFoundationIdentity(
                MockIdentityDescriptor.Create("user2", "domain", "tenant"),
                "John Smith",
                Guid.NewGuid());

            var identityMappings = new Dictionary<string, IEnumerable<ITeamFoundationIdentity>>(StringComparer.OrdinalIgnoreCase)
            {
                { "John Smith", new[] { identity1, identity2 } }
            };

            var mockIdentityService = new MockIdentityManagementService(identityMappings);

            Instance = new DisplayNameToAliasValueConverter(mockIdentityService);
            DisplayNames = new[] { "John Smith" };
        }

        public override void When()
        {
            try
            {
                Result = Instance.Map(DisplayNames);
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        [TestMethod]
        public void a_MultipleIdentitiesFoundException_is_thrown()
        {
            _thrownException.ShouldNotBeNull();
            _thrownException.ShouldBeType<MultipleIdentitiesFoundException>();
        }

        [TestMethod]
        public void the_exception_message_contains_the_display_name()
        {
            _thrownException.ShouldNotBeNull();
            _thrownException!.Message.ShouldContain("John Smith");
        }
    }

    [TestClass]
    public class when_mapping_a_display_name_with_single_matching_identity : DisplayNameToAliasValueConverterContextSpecification
    {
        public override void Given()
        {
            var identity = new MockTeamFoundationIdentity(
                MockIdentityDescriptor.Create("jsmith", "domain", "tenant"),
                "John Smith",
                Guid.NewGuid());

            var identityMappings = new Dictionary<string, IEnumerable<ITeamFoundationIdentity>>(StringComparer.OrdinalIgnoreCase)
            {
                { "John Smith", new[] { identity } }
            };

            var mockIdentityService = new MockIdentityManagementService(identityMappings);

            Instance = new DisplayNameToAliasValueConverter(mockIdentityService);
            DisplayNames = new[] { "John Smith" };
        }

        public override void When()
        {
            Result = Instance.Map(DisplayNames);
        }

        [TestMethod]
        public void the_result_contains_the_alias()
        {
            Result.ShouldNotBeNull();
            Result.ContainsKey("John Smith").ShouldBeTrue();
            Result["John Smith"].ShouldNotBeNull();
        }
    }

    [TestClass]
    public class when_mapping_a_display_name_with_no_matching_identity : DisplayNameToAliasValueConverterContextSpecification
    {
        public override void Given()
        {
            var identityMappings = new Dictionary<string, IEnumerable<ITeamFoundationIdentity>>(StringComparer.OrdinalIgnoreCase);

            var mockIdentityService = new MockIdentityManagementService(identityMappings);

            Instance = new DisplayNameToAliasValueConverter(mockIdentityService);
            DisplayNames = new[] { "Unknown User" };
        }

        public override void When()
        {
            Result = Instance.Map(DisplayNames);
        }

        [TestMethod]
        public void the_result_does_not_contain_a_mapping_for_unknown_user()
        {
            Result.ShouldNotBeNull();
            Result.ContainsKey("Unknown User").ShouldBeFalse();
        }
    }

    [TestClass]
    public class when_mapping_empty_display_names_array : DisplayNameToAliasValueConverterContextSpecification
    {
        public override void Given()
        {
            var mockIdentityService = new MockIdentityManagementService();
            Instance = new DisplayNameToAliasValueConverter(mockIdentityService);
            DisplayNames = Array.Empty<string>();
        }

        public override void When()
        {
            Result = Instance.Map(DisplayNames);
        }

        [TestMethod]
        public void an_empty_result_is_returned()
        {
            Result.ShouldNotBeNull();
            Result.Count.ShouldEqual(0);
        }
    }

    [TestClass]
    public class when_creating_DisplayNameToAliasValueConverter_with_null_service
    {
        private Exception? _thrownException;

        [TestMethod]
        public void an_ArgumentNullException_is_thrown()
        {
            try
            {
                _ = new DisplayNameToAliasValueConverter(null!);
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }

            _thrownException.ShouldNotBeNull();
            _thrownException.ShouldBeType<ArgumentNullException>();
        }
    }
}