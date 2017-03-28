using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Core.Tests
{
    [TestClass]
    public class LinkTypeTests : WorkItemStoreComparisonContextSpecification
    {
        private IEnumerable<IWorkItemLinkType> _restResult;

        private IEnumerable<IWorkItemLinkType> _soapResult;

        [TestMethod]
        [TestCategory("localOnly")]
        [TestCategory("SOAP")]
        [TestCategory("REST")]
        public void Equal()
        {
            _restResult.ShouldContainOnly(_soapResult);
        }

        public override void When()
        {
            _soapResult = Soap.WorkItemLinkTypes.ToList();
            _restResult = Rest.WorkItemLinkTypes.ToList();
        }
    }
}