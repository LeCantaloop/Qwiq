using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Core.Tests
{
    [TestClass]
    public class LinkTypeTests : WorkItemStoreComparisonContextSpecification
    {
        

        private IEnumerable<IWorkItemLinkType> _soapResult;
        
        private IEnumerable<IWorkItemLinkType> _restResult;

        

        public override void When()
        {
            _soapResult = Soap.WorkItemLinkTypes.ToList();
            _restResult = Rest.WorkItemLinkTypes.ToList();
        }

        [TestMethod]
        [TestCategory("localOnly")]
        public void Equal()
        {
            _restResult.ShouldContainOnly(_soapResult);
        }
    }
}