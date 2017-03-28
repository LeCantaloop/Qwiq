using System;

using Microsoft.Qwiq.Tests.Common;

namespace Microsoft.Qwiq.Core.Tests
{
    public abstract class WorkItemStoreComparisonContextSpecification : ContextSpecification
    {
        protected IWorkItemStore Rest { get; private set; }

        protected IWorkItemStore Soap { get; private set; }

        public override void Given()
        {
            var credentials = Credentials.CredentialsFactory.CreateCredentials((string)null);
            var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

            Soap = Microsoft.Qwiq.Soap.WorkItemStoreFactory.Instance.Create(uri, credentials, ClientType.Soap);
            Rest = Microsoft.Qwiq.Rest.WorkItemStoreFactory.Instance.Create(uri, credentials, ClientType.Rest);
        }
    }
}