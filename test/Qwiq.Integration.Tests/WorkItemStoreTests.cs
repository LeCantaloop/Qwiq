//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Microsoft.Qwiq.Core.Tests;
//using Microsoft.Qwiq.Soap;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using Should;
//using Should.Core.Exceptions;

//namespace Microsoft.Qwiq.Integration.Tests
//{
//    [TestClass]
//    public class Given_a_WorkItemStore_from_each_implementation : WorkItemStoreComparisonContextSpecification
//    {
//        [TestMethod]
//        [TestCategory("localOnly")]
//        [TestCategory("REST")]
//        [TestCategory("SOAP")]
//        [ExpectedException(typeof(EqualException), "No EqualityComparer for TfsCredentials")]
//        public void AuthorizedCredentials_are_equal()
//        {
//            Rest.AuthorizedCredentials.ShouldEqual(Soap.AuthorizedCredentials);
//        }

//        [TestMethod]
//        [TestCategory("localOnly")]
//        [TestCategory("REST")]
//        [TestCategory("SOAP")]
//        [ExpectedException(typeof(NotImplementedException), "REST does not have an implementation")]
//        public void FieldDefinitions_are_equal()
//        {
//            Rest.FieldDefinitions.ShouldContainOnly(Soap.FieldDefinitions);
//        }

//        [TestMethod]
//        [TestCategory("localOnly")]
//        [TestCategory("REST")]
//        [TestCategory("SOAP")]
//        public void UserSid_are_equal()
//        {
//            Rest.UserSid.ShouldEqual(Soap.UserSid);
//        }
//    }

//    [TestClass]
//    public class Given_SOAP_and_REST_workitemstore_implementations : WorkItemStoreComparisonContextSpecification
//    {
//        public override void When()
//        {
//            SoapLinkTypes = TimedAction(() => Qwiq.Soap.WorkItemLinkTypes, "SOAP", "Work Item Link Types");
//            RestLinkTypes = TimedAction(() => Qwiq.Rest.WorkItemLinkTypes, "REST", "Work Item Link Types");
//        }

//        [TestMethod]
//        [TestCategory("localOnly")]
//        [TestCategory("SOAP")]
//        [TestCategory("REST")]
//        public void WorkItemLinkTypeCollections_are_equal()
//        {
//            RestLinkTypes.ShouldContainOnly(SoapLinkTypes);
//        }

//        public WorkItemLinkTypeCollection RestLinkTypes { get; set; }

//        public WorkItemLinkTypeCollection SoapLinkTypes { get; set; }
//    }

//    [TestClass]
//    public class given_an_IWorkItemStore_an_a_query_with_two_ids_and_an_asof_date : WorkItemStoreSoapTests
//    {
//        private IEnumerable<IWorkItem> _actual;

//        [TestMethod]
//        public void a_query_is_created()
//        {
//            ((MockQueryFactory)QueryFactory).CreateCallCount.ShouldEqual(1);
//        }

//        [TestMethod]
//        public void a_query_string_with_two_ids_is_generated()
//        {
//            ((MockQueryFactory)QueryFactory)
//                .QueryWiqls.Single()
//                .ShouldEqual("SELECT * FROM WorkItems ASOF '2012-01-12 12:23:34Z'");
//        }

//        public override void Given()
//        {
//            QueryFactory = new MockQueryFactory();
//            base.Given();
//        }

//        public override void When()
//        {
//            _actual = WorkItemStore.Query(new[] { 1, 2 }, new DateTime(2012, 1, 12, 12, 23, 34, DateTimeKind.Utc));
//        }
//    }

//    [TestClass]
//    public class given_an_IWorkItemStore_and_a_query_with_no_ids : WorkItemStoreSoapTests
//    {
//        private IEnumerable<IWorkItem> _actual;

//        [TestMethod]
//        public void a_query_is_never_created()
//        {
//            ((MockQueryFactory)QueryFactory).CreateCallCount.ShouldEqual(0);
//        }

//        [TestMethod]
//        public void an_empty_result_set_is_returned()
//        {
//            _actual.ShouldBeEmpty();
//        }

//        public override void Given()
//        {
//            QueryFactory = new MockQueryFactory();
//            base.Given();
//        }

//        public override void When()
//        {
//            _actual = WorkItemStore.Query(new int[] { });
//        }
//    }

//    [TestClass]
//    public class given_an_IWorkItemStore_and_a_query_with_one_id : WorkItemStoreSoapTests
//    {
//        private IEnumerable<IWorkItem> _actual;

//        [TestMethod]
//        public void a_query_is_created()
//        {
//            ((MockQueryFactory)QueryFactory).CreateCallCount.ShouldEqual(1);
//        }

//        [TestMethod]
//        public void a_query_string_with_one_id_is_generated()
//        {
//            ((MockQueryFactory)QueryFactory)
//                .QueryWiqls.Single()
//                .ShouldEqual("SELECT * FROM WorkItems");
//        }

//        public override void Given()
//        {
//            QueryFactory = new MockQueryFactory();
//            base.Given();
//        }

//        public override void When()
//        {
//            _actual = WorkItemStore.Query(new[] { 1 });
//        }
//    }

//    [TestClass]
//    public class given_an_IWorkItemStore_and_a_query_with_two_ids : WorkItemStoreSoapTests
//    {
//        private IEnumerable<IWorkItem> _actual;

//        [TestMethod]
//        public void a_query_is_created()
//        {
//            ((MockQueryFactory)QueryFactory).CreateCallCount.ShouldEqual(1);
//        }

//        [TestMethod]
//        public void a_query_string_with_two_ids_is_generated()
//        {
//            ((MockQueryFactory)QueryFactory)
//                .QueryWiqls.Single()
//                .ShouldEqual("SELECT * FROM WorkItems");
//        }

//        public override void Given()
//        {
//            QueryFactory = new MockQueryFactory();
//            base.Given();
//        }

//        public override void When()
//        {
//            _actual = WorkItemStore.Query(new[] { 1, 2 });
//        }
//    }

//    public abstract class WorkItemStoreSoapTests : WorkItemStoreTests<IWorkItemStore>
//    {
//        protected override IWorkItemStore Create()
//        {
//            return new WorkItemStore((TfsTeamProjectCollection)null, s => QueryFactory);
//        }
//    }

//    [TestClass]
//    public class LinkTypeTests : WorkItemStoreComparisonContextSpecification
//    {
//        private IEnumerable<IWorkItemLinkType> _restResult;

//        private IEnumerable<IWorkItemLinkType> _soapResult;

//        [TestMethod]
//        [TestCategory("localOnly")]
//        [TestCategory("SOAP")]
//        [TestCategory("REST")]
//        public void Equal()
//        {
//            _restResult.ShouldContainOnly(_soapResult);
//        }

//        public override void When()
//        {
//            _soapResult = Soap.WorkItemLinkTypes.ToList();
//            _restResult = Rest.WorkItemLinkTypes.ToList();
//        }
//    }

using Microsoft.Qwiq;
using Microsoft.Qwiq.Core.Tests;

public abstract class WorkItemStoreTests<T> : TimedContextSpecification
    where T : IWorkItemStore
{
    internal IQueryFactory QueryFactory;

    protected T WorkItemStore;

    public override void Cleanup()
    {
        WorkItemStore?.Dispose();
        base.Cleanup();
    }

    public override void Given()
    {
        WorkItemStore = Create();
    }

    protected abstract T Create();
}
//}
