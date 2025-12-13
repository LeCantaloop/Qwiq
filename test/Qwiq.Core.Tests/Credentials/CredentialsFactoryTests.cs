using System.Linq;

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Qwiq.Tests.Common;

using Should;

namespace Qwiq.Credentials
{
    // GetBasicCredentials Tests

    [TestClass]
    public class Given_null_username_when_calling_GetBasicCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetBasicCredentials(null, "password").ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_empty_username_when_calling_GetBasicCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetBasicCredentials(string.Empty, "password").ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_null_password_when_calling_GetBasicCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetBasicCredentials("username", null).ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_empty_password_when_calling_GetBasicCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetBasicCredentials("username", string.Empty).ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_valid_credentials_when_calling_GetBasicCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetBasicCredentials("username", "password").ToArray();
        }

        [TestMethod]
        public void Then_one_credential_is_returned()
        {
            _result.Length.ShouldEqual(1);
        }

        [TestMethod]
        public void Then_prompt_type_is_DoNotPrompt()
        {
            _result[0].PromptType.ShouldEqual(CredentialPromptType.DoNotPrompt);
        }

        [TestMethod]
        public void Then_federated_credential_is_VssBasicCredential()
        {
            _result[0].Federated.ShouldBeType<VssBasicCredential>();
        }
    }

    // GetOAuthCredentials Tests

    [TestClass]
    public class Given_null_accessToken_when_calling_GetOAuthCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetOAuthCredentials(null).ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_empty_accessToken_when_calling_GetOAuthCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetOAuthCredentials(string.Empty).ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_valid_accessToken_when_calling_GetOAuthCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetOAuthCredentials("validAccessToken").ToArray();
        }

        [TestMethod]
        public void Then_one_credential_is_returned()
        {
            _result.Length.ShouldEqual(1);
        }

        [TestMethod]
        public void Then_prompt_type_is_DoNotPrompt()
        {
            _result[0].PromptType.ShouldEqual(CredentialPromptType.DoNotPrompt);
        }
    }

    // GetServiceIdentityCredentials Tests

    [TestClass]
    public class Given_null_username_when_calling_GetServiceIdentityCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetServiceIdentityCredentials(null, "password").ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_empty_username_when_calling_GetServiceIdentityCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetServiceIdentityCredentials(string.Empty, "password").ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_null_password_when_calling_GetServiceIdentityCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetServiceIdentityCredentials("username", null).ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_empty_password_when_calling_GetServiceIdentityCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetServiceIdentityCredentials("username", string.Empty).ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_valid_credentials_when_calling_GetServiceIdentityCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetServiceIdentityCredentials("username", "password").ToArray();
        }

        [TestMethod]
        public void Then_two_credentials_are_returned()
        {
            _result.Length.ShouldEqual(2);
        }

        [TestMethod]
        public void Then_first_credential_has_federated_credential()
        {
            _result[0].Federated.ShouldNotBeNull();
            _result[0].Federated!.GetType().Name.ShouldEqual("VssAadCredential");
        }

        [TestMethod]
        public void Then_first_credential_prompt_type_is_DoNotPrompt()
        {
            _result[0].PromptType.ShouldEqual(CredentialPromptType.DoNotPrompt);
        }

        [TestMethod]
        public void Then_second_credential_has_WindowsCredential()
        {
            _result[1].Windows.ShouldNotBeNull();
        }

        [TestMethod]
        public void Then_second_credential_prompt_type_is_DoNotPrompt()
        {
            _result[1].PromptType.ShouldEqual(CredentialPromptType.DoNotPrompt);
        }
    }

    // GetServiceIdentityPatCredentials Tests

    [TestClass]
    public class Given_null_password_when_calling_GetServiceIdentityPatCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetServiceIdentityPatCredentials(null).ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_empty_password_when_calling_GetServiceIdentityPatCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetServiceIdentityPatCredentials(string.Empty).ToArray();
        }

        [TestMethod]
        public void Then_no_credentials_are_returned()
        {
            _result.Length.ShouldEqual(0);
        }
    }

    [TestClass]
    public class Given_valid_password_when_calling_GetServiceIdentityPatCredentials : ContextSpecification
    {
        private VssCredentials[] _result = null!;

        public override void When()
        {
            _result = CredentialsFactory.GetServiceIdentityPatCredentials("personalAccessToken").ToArray();
        }

        [TestMethod]
        public void Then_one_credential_is_returned()
        {
            _result.Length.ShouldEqual(1);
        }

        [TestMethod]
        public void Then_prompt_type_is_DoNotPrompt()
        {
            _result[0].PromptType.ShouldEqual(CredentialPromptType.DoNotPrompt);
        }

        [TestMethod]
        public void Then_federated_credential_is_VssBasicCredential()
        {
            _result[0].Federated.ShouldBeType<VssBasicCredential>();
        }
    }
}
