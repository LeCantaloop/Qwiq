using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Identity
{
    public abstract class IdentityFieldValueContextSpecification : ContextSpecification
    {
        protected string DisplayName;

        protected string FullName;
        protected IdentityFieldValue Result;
    }

    [TestClass]
    public class Given_an_identity_with_only_a_display_name : IdentityFieldValueContextSpecification
    {
        /// <inheritdoc />
        public override void Given()
        {
            DisplayName = "Chris Johnson";
        }

        /// <inheritdoc />
        public override void When()
        {
            Result = new IdentityFieldValue(DisplayName);
        }

        [TestMethod]
        public void TeamFoundationId_is_null()
        {
            Result.TeamFoundationId.ShouldBeNull();
        }

        [TestMethod]
        public void Alias_is_null()
        {
            Result.LogonName.ShouldBeNull();
        }

        [TestMethod]
        public void AccountName_is_null()
        {
            Result.AccountName.ShouldBeNull();
        }

        [TestMethod]
        public void DisplayName_is_FirstLast()
        {
            Result.DisplayName.ShouldEqual(DisplayName);
        }

        [TestMethod]
        public void Domain_is_null()
        {
            Result.Domain.ShouldBeNull();
        }

        [TestMethod]
        public void Email_is_null()
        {
            Result.Email.ShouldBeNull();
        }

        [TestMethod]
        public void FullName_is_null()
        {
            Result.Identifier.ShouldBeNull();
        }

        [TestMethod]
        public void IdentityName_is_empty()
        {
            Result.IdentityName.ShouldBeNull();
        }
    }

    [TestClass]
    public class Given_an_identity_with_a_combo_string : IdentityFieldValueContextSpecification
    {
        /// <inheritdoc />
        public override void Given()
        {
            DisplayName = "Chris Johnson <chrisjohns@contoso.com>";
        }

        /// <inheritdoc />
        public override void When()
        {
            Result = new IdentityFieldValue(DisplayName);
        }

        [TestMethod]
        public void TeamFoundationId_is_null()
        {
            Result.TeamFoundationId.ShouldBeNull();
        }

        [TestMethod]
        public void Alias_is_null()
        {
            Result.LogonName.ShouldEqual("chrisjohns");
        }

        [TestMethod]
        public void AccountName_is_null()
        {
            Result.AccountName.ShouldEqual("chrisjohns@contoso.com");
        }

        [TestMethod]
        public void DisplayName_is_FirstLast()
        {
            Result.DisplayName.ShouldEqual("Chris Johnson");
        }

        [TestMethod]
        public void Domain_is_null()
        {
            Result.Domain.ShouldBeNull();
        }

        [TestMethod]
        public void Email_is_null()
        {
            Result.Email.ShouldEqual("chrisjohns@contoso.com");
        }

        [TestMethod]
        public void FullName_is_null()
        {
            Result.Identifier.ShouldBeNull();
        }

        [TestMethod]
        public void IdentityName_is_empty()
        {
            Result.IdentityName.ShouldEqual("chrisjohns@contoso.com");
        }
    }

    [TestClass]
    public class Given_an_identity_with_a_combo_string_and_fullname : IdentityFieldValueContextSpecification
    {
        /// <inheritdoc />
        public override void Given()
        {
            DisplayName = "Chris Johnson <chrisjohns@contoso.com>";
            FullName = "CD4C5751-F4E6-41D5-A4C9-EFFD66BC8E9C\\chrisjohns@contoso.com";
        }

        /// <inheritdoc />
        public override void When()
        {
            Result = new IdentityFieldValue(DisplayName, FullName, null);
        }

        [TestMethod]
        public void TeamFoundationId_is_null()
        {
            Result.TeamFoundationId.ShouldBeNull();
        }

        [TestMethod]
        public void Alias_is_null()
        {
            Result.LogonName.ShouldEqual("chrisjohns");
        }

        [TestMethod]
        public void AccountName_is_null()
        {
            Result.AccountName.ShouldEqual("chrisjohns@contoso.com");
        }

        [TestMethod]
        public void DisplayName_is_FirstLast()
        {
            Result.DisplayName.ShouldEqual("Chris Johnson");
        }

        [TestMethod]
        public void Domain_is_null()
        {
            Result.Domain.ShouldEqual("CD4C5751-F4E6-41D5-A4C9-EFFD66BC8E9C");
        }

        [TestMethod]
        public void Email_is_null()
        {
            Result.Email.ShouldEqual("chrisjohns@contoso.com");
        }

        [TestMethod]
        public void FullName_is_null()
        {
            Result.Identifier.ShouldEqual("CD4C5751-F4E6-41D5-A4C9-EFFD66BC8E9C\\chrisjohns@contoso.com");
        }

        [TestMethod]
        public void IdentityName_is_empty()
        {
            Result.IdentityName.ShouldEqual("chrisjohns@contoso.com");
        }
    }

    [TestClass]
    public class Given_an_identity_with_a_display_name_and_fullname : Given_an_identity_with_a_combo_string_and_fullname
    {
        /// <inheritdoc />
        public override void Given()
        {
            base.Given();
            DisplayName = "Chris Johnson";
        }
    }
}
