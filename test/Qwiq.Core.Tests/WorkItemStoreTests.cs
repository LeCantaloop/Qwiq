using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Qwiq.Core.Tests.Mocks;
using Microsoft.Qwiq.Proxies;
using TfsSoap = Microsoft.Qwiq.Proxies.Soap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Microsoft.Qwiq.Core.Tests
{
    public abstract class WorkItemStoreTests<T> : ContextSpecification
        where T : IWorkItemStore
    {
        protected IWorkItemStore WorkItemStore;
        internal MockQueryFactory QueryFactory;

        protected abstract T Create();

        public override void Given()
        {
            WorkItemStore = Create();
        }

        public override void Cleanup()
        {
            WorkItemStore.Dispose();
        }
    }

    public abstract class WorkItemStoreSoapTests : WorkItemStoreTests<TfsSoap.WorkItemStoreProxy>
    {
        protected override TfsSoap.WorkItemStoreProxy Create()
        {
            return new TfsSoap.WorkItemStoreProxy(null, null, QueryFactory);
        }
    }

    [TestClass]
    public class given_an_IWorkItemStore_and_a_query_with_no_ids : WorkItemStoreSoapTests
    {
        private IEnumerable<IWorkItem> _actual;

        public override void Given()
        {
            QueryFactory = new MockQueryFactory();
            base.Given();
        }

        public override void When()
        {
            _actual = WorkItemStore.Query(new int[] {});
        }

        [TestMethod]
        public void a_query_is_never_created()
        {
            QueryFactory.CreateCallCount.ShouldEqual(0);
        }

        [TestMethod]
        public void an_empty_result_set_is_returned()
        {
            _actual.ShouldBeEmpty();
        }
    }

    [TestClass]
    public class given_an_IWorkItemStore_and_a_query_with_one_id : WorkItemStoreSoapTests
    {
        public override void Given()
        {
            QueryFactory = new MockQueryFactory();
            base.Given();
        }

        public override void When()
        {
            WorkItemStore.Query(new[] { 1 });
        }

        [TestMethod]
        public void a_query_is_created()
        {
            QueryFactory.CreateCallCount.ShouldEqual(1);
        }

        [TestMethod]
        public void a_query_string_with_one_id_is_generated()
        {
            QueryFactory.QueryWiqls.Single().ShouldEqual("SELECT * FROM WorkItems WHERE [System.Id] IN (1)");
        }
    }

    [TestClass]
    public class given_an_IWorkItemStore_and_a_query_with_two_ids : WorkItemStoreSoapTests
    {
        public override void Given()
        {
            QueryFactory = new MockQueryFactory();
            base.Given();
        }

        public override void When()
        {
            WorkItemStore.Query(new[] { 1, 2 });
        }

        [TestMethod]
        public void a_query_is_created()
        {
            QueryFactory.CreateCallCount.ShouldEqual(1);
        }

        [TestMethod]
        public void a_query_string_with_two_ids_is_generated()
        {
            QueryFactory.QueryWiqls.Single().ShouldEqual("SELECT * FROM WorkItems WHERE [System.Id] IN (1, 2)");
        }
    }

    [TestClass]
    public class given_an_IWorkItemStore_an_a_query_with_two_ids_and_an_asof_date : WorkItemStoreSoapTests
    {
        public override void Given()
        {
            QueryFactory = new MockQueryFactory();
            base.Given();
        }

        public override void When()
        {
            WorkItemStore.Query(new[] { 1, 2 }, new DateTime(2012, 1, 12, 12, 23, 34, DateTimeKind.Utc));
        }

        [TestMethod]
        public void a_query_is_created()
        {
            QueryFactory.CreateCallCount.ShouldEqual(1);
        }

        [TestMethod]
        public void a_query_string_with_two_ids_and_an_asof_clause_is_generated()
        {
            QueryFactory.QueryWiqls.Single().ShouldEqual("SELECT * FROM WorkItems WHERE [System.Id] IN (1, 2) ASOF '2012-01-12 12:23:34Z'");
        }
    }
}

