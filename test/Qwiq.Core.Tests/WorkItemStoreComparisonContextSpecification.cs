using System;
using System.Diagnostics;

using Microsoft.Qwiq.Credentials;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;
using Should.Core.Exceptions;

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


            Soap = TimedAction(
                ()=> Microsoft.Qwiq.Soap.WorkItemStoreFactory.Instance.Create(uri, credentials, ClientType.Soap),
                "SOAP",
                "Create WIS");



            Rest = TimedAction(
                () => Microsoft.Qwiq.Rest.WorkItemStoreFactory.Instance.Create(uri, credentials, ClientType.Rest),
                "REST",
                "Create WIS");

        }

        protected static T TimedAction<T>(Func<T> action, string category, string userMessage)
        {
            var start = Clock.GetTimestamp();
            try
            {
                return action();
            }
            finally
            {
                var stop = Clock.GetTimestamp();
                Debug.Print("{0}: {1} {2}", category,  Clock.GetTimeSpan(start, stop), userMessage);
            }
        }
    }

    [TestClass]
    public class Given_a_WorkItemStore_from_each_implementation : WorkItemStoreComparisonContextSpecification
    {
        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        [ExpectedException(typeof(EqualException), "No EqualityComparer for TfsCredentials")]
        public void AuthorizedCredentials_are_equal()
        {
            Rest.AuthorizedCredentials.ShouldEqual(Soap.AuthorizedCredentials);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        [ExpectedException(typeof(NotImplementedException), "REST does not have an implementation")]
        public void FieldDefinitions_are_equal()
        {
            Rest.FieldDefinitions.ShouldContainOnly(Soap.FieldDefinitions);
        }

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("REST")]
        [TestCategory("SOAP")]
        public void UserSid_are_equal()
        {
            Rest.UserSid.ShouldEqual(Soap.UserSid);
        }
    }
}