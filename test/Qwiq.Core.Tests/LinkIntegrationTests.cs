//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using Should;

//namespace Microsoft.Qwiq.Core.Tests
//{
//    public abstract class LinkIntegrationTests : WorkItemStoreComparisonContextSpecification
//    {
//        protected IWorkItem RestResult { get; private set; }
//        protected IWorkItem SoapResult { get; private set; }

//        protected int Id { get; set; }

//        public override void When()
//        {
//            RestResult = Rest.Query(Id);
//            SoapResult = Soap.Query(Id);
//        }
//    }

//    [TestClass]
//    public class Given_a_workitem_with_links : LinkIntegrationTests
//    {
//        public override void Given()
//        {
//            base.Given();
//            Id = 156027;
//        }

//        [TestMethod]
//        [TestCategory("localOnly")]
//        [TestCategory("SOAP")]
//        [TestCategory("REST")]
//        [ExpectedException(typeof(NotImplementedException), "This is not yet implemented in the REST client.")]
//        public void The_Links_are_equal()
//        {
//            RestResult.Links.ShouldContainOnly(SoapResult.Links);
//        }
//    }
//}
