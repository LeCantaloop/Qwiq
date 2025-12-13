using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Linq;

namespace Qwiq.WireMock
{
    /// <summary>
    /// Tests for REST client WIQL query execution using WireMock.
    /// </summary>
    /// <remarks>
    /// These tests exercise the full REST client implementation including:
    /// - HTTP request/response handling
    /// - JSON serialization/deserialization
    /// - Query execution pipeline
    /// - Work item materialization
    ///
    /// Tests use real captured Azure DevOps API responses for offline testing.
    /// </remarks>
    [TestClass]
    [TestCategory("WireMock")]
    public class Given_WireMock_WorkItemStore_When_Querying_Single_Bug : WireMockRestContextSpecification
    {
        private IWorkItemCollection? _result;
        private const int TestWorkItemId = 1;
        private const string TestWiql = "SELECT [System.Id] FROM WorkItems WHERE [System.Id] = 1";

        public override void When()
        {
            _result = TimedAction(() => Store?.Query(TestWiql), "WireMock", "Query WIQL");
        }

        [TestMethod]
        public void Should_Return_One_WorkItem()
        {
            _result.ShouldNotBeNull();
#pragma warning disable CA1829, CA1826
            _result.Count().ShouldBe(1);
#pragma warning restore CA1829, CA1826
        }

        [TestMethod]
        public void Should_Have_Correct_WorkItem_Id()
        {
            _result.ShouldNotBeNull();
            var workItem = _result[0];
            workItem.Id.ShouldBe(TestWorkItemId);
        }

        [TestMethod]
        public void Should_Have_Correct_Title()
        {
            _result.ShouldNotBeNull();
            var workItem = _result[0];
            workItem.Title.ShouldBe("Integration Test");
        }

        [TestMethod]
        public void Should_Have_Correct_Work_Item_Type()
        {
            _result.ShouldNotBeNull();
            var workItem = _result[0];
            workItem.WorkItemType.ShouldBe("Bug");
        }
    }

    /// <summary>
    /// Tests for REST client querying multiple work items using real captured responses.
    /// </summary>
    [TestClass]
    [TestCategory("WireMock")]
    public class Given_WireMock_WorkItemStore_When_Querying_Multiple_Bugs : WireMockRestContextSpecification
    {
        private IWorkItemCollection? _result;
        private const string TestWiql = "SELECT [System.Id] FROM WorkItems WHERE [System.WorkItemType] = 'Bug'";

        public override void When()
        {
            _result = TimedAction(() => Store?.Query(TestWiql), "WireMock", "Query Multiple Bugs");
        }

        [TestMethod]
        public void Should_Return_WorkItems()
        {
            _result.ShouldNotBeNull();
#pragma warning disable CA1829, CA1826
            _result.Count().ShouldBeGreaterThan(0);
#pragma warning restore CA1829, CA1826
        }

        [TestMethod]
        public void Should_Contain_Work_Item_Id_1()
        {
            _result.ShouldNotBeNull();
            var ids = _result.Select(wi => wi.Id).ToArray();
            ids.ShouldContain(1);
        }

        [TestMethod]
        public void Should_Have_Valid_Work_Item_Properties()
        {
            _result.ShouldNotBeNull();
            var firstItem = _result[0];
            firstItem.Id.ShouldBeGreaterThan(0);
            firstItem.Title.ShouldNotBeNullOrEmpty();
            firstItem.State.ShouldNotBeNullOrEmpty();
        }
    }

    /// <summary>
    /// Tests for REST client handling queries for non-existent work items.
    /// </summary>
    [TestClass]
    [TestCategory("WireMock")]
    public class Given_WireMock_WorkItemStore_When_Query_Returns_Empty : WireMockRestContextSpecification
    {
        private IWorkItemCollection? _result;
        private const string TestWiql = "SELECT [System.Id] FROM WorkItems WHERE [System.Id] = -1";

        public override void When()
        {
            _result = TimedAction(() => Store?.Query(TestWiql), "WireMock", "Query Empty");
        }

        [TestMethod]
        public void Should_Execute_Without_Error()
        {
            _result.ShouldNotBeNull();
        }

        [TestMethod]
        public void Should_Return_Collection()
        {
            _result.ShouldNotBeNull();
#pragma warning disable CA1829, CA1826
            _result.Count().ShouldBeGreaterThanOrEqualTo(0);
#pragma warning restore CA1829, CA1826
        }
    }
}
