using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Qwiq.Core.Tests
{
    [TestClass]
    public class LinkTypeTests : ContextSpecification
    {
        private IWorkItemStore _soap;

        private IEnumerable<IWorkItemLinkType> _soapResult;
        private IWorkItemStore _rest;
        private IEnumerable<IWorkItemLinkType> _restResult;

        public override void Given()
        {
            var credentials = Credentials.CredentialsFactory.CreateCredentials((string)null);
            var fac = WorkItemStoreFactory.GetInstance();
            var uri = new Uri("https://microsoft.visualstudio.com/defaultcollection");

            _soap = fac.Create(uri, credentials, ClientType.Soap);
            _rest = fac.Create(uri, credentials, ClientType.Rest);
        }

        public override void When()
        {
            _soapResult = _soap.WorkItemLinkTypes.ToList();
            _restResult = _rest.WorkItemLinkTypes.ToList();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Equal()
        {
            _restResult.ShouldContainOnly(_soapResult);
        }
    }
}