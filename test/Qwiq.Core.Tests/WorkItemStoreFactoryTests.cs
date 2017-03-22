using System;

using Microsoft.Qwiq.Credentials;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Core.Tests
{
    public abstract class WorkItemStoreFactoryContextSpecification : ContextSpecification
    {
        protected IWorkItemStoreFactory Instance { get; private set; }
        protected IWorkItemStore Store { get; set; }

        public override void Given()
        {
            base.Given();
            Instance = WorkItemStoreFactory.GetInstance();
        }

        public override void Cleanup()
        {
            base.Cleanup();
            Store?.Dispose();
        }
    }

    [TestClass]
    public class Given_a_Uri_and_Credential : WorkItemStoreFactoryContextSpecification
    {
        public override void When()
        {
            var uri = new Uri("https://microsoft.visualstudio.com/DefaultCollection");
            var cred = new VssClientCredentials(
                new WindowsCredential(true),
                CredentialPromptType.PromptIfNeeded)
                           {
                               Storage = new VssClientCredentialStorage()
                           };

#pragma warning disable CS0618 // Type or member is obsolete
            Store = Instance.Create(
                uri,
                new TfsCredentials(cred));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Store_is_Created()
        {
            Store.ShouldNotBeNull();
        }
    }

    [TestClass]
    public class Given_a_Uri_and_Credentials_from_the_CredentialsFactory : WorkItemStoreFactoryContextSpecification
    {
        public override void When()
        {
            var uri = new Uri("https://microsoft.visualstudio.com/DefaultCollection");


#pragma warning disable CS0618 // Type or member is obsolete
            Store = Instance.Create(
                uri,
                CredentialsFactory.CreateCredentials((string)null));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Store_is_Created()
        {
            Store.ShouldNotBeNull();
        }
    }
}
