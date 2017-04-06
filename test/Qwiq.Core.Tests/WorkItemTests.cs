using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Rest;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Core.Tests
{
    public abstract class RestWorkItemContextSpecification : WorkItemContextSpecification<IWorkItemStore>
    {
        

        protected override IWorkItemStore Create()
        {
            var options = AuthenticationOptions;
            options.ClientType = ClientType.Rest;

            return TimedAction(() => Rest.WorkItemStoreFactory.Instance.Create(options), "REST", "WIS Create");
        }
    }

    public abstract class SoapWorkItemContextSpecification : WorkItemContextSpecification<IWorkItemStore>
    {


        protected override IWorkItemStore Create()
        {
            var options = AuthenticationOptions;
            options.ClientType = ClientType.Soap;

            return TimedAction(() => Soap.WorkItemStoreFactory.Instance.Create(options), "SOAP", "WIS Create");
        }
    }

    public abstract class WorkItemContextSpecification<T> : WorkItemStoreTests<T>
        where T : IWorkItemStore
    {
        private const int Id = 10726528;

        protected AuthenticationOptions AuthenticationOptions
        {
            get
            {
                var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

                var options = new AuthenticationOptions(uri, AuthenticationType.Windows) { CreateCredentials = CreateCredentials };
                return options;
            }
        }

        private static IEnumerable<TfsCredentials> CreateCredentials(AuthenticationType t)
        {
            // User did not specify a username or a password, so use the process identity
            yield return new VssClientCredentials(new WindowsCredential(false)) { Storage = new VssClientCredentialStorage(), PromptType = CredentialPromptType.DoNotPrompt };

            // Use the Windows identity of the logged on user
            yield return new VssClientCredentials(true) { Storage = new VssClientCredentialStorage(), PromptType = CredentialPromptType.PromptIfNeeded };
        }

        protected IWorkItem Result { get; private set; }

        public override void When()
        {
            Result = WorkItemStore.Query(Id);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Reading_Id_from_this_operator_with_ReferenceName_equals_the_property_value()
        {
            Result[CoreFieldRefNames.Id]?.ToString().ShouldEqual(Result.Id.ToString());
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Reading_Id_from_Fields_property_with_ReferenceName_equals_the_property_value()
        {
            Result.Fields[CoreFieldRefNames.Id]?.Value?.ToString().ShouldEqual(Result.Id.ToString());
        }
    }

    [TestClass]
    public class Given_a_WorkItem_from_REST : RestWorkItemContextSpecification
    {
       
    }

    [TestClass]
    public class Given_a_WorkItem_from_SOAP : RestWorkItemContextSpecification
    {

    }
}
