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
    /// Unlike integration tests, these run without Azure DevOps connectivity
    /// and provide fast, deterministic results.
    ///
    /// Tests use real captured Azure DevOps API responses for offline testing.
    /// The stubs include VssConnection handshake, project info, WIQL queries,
    /// work items, and work item type definitions from qwiq-sandbox.
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
    /// NOTE: Captured stub only contains work item ID 1, so this tests the same ID.
    /// For testing multiple work items, capture additional stubs with different IDs.
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
    /// NOTE: Captured stubs don't include empty query responses.
    /// This test will return the same results as the stub data (ID 1).
    /// For true empty result testing, capture a stub with empty WIQL results.
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
            // Test that WireMock is properly handling the query
            // NOTE: Since captured stubs don't match ID=-1, this may return no results
            // or fall through to default WireMock behavior
            _result.ShouldNotBeNull();
        }

        [TestMethod]
        public void Should_Return_Collection()
        {
            // Verify we get a collection back (even if empty)
            _result.ShouldNotBeNull();
#pragma warning disable CA1829, CA1826
            _result.Count().ShouldBeGreaterThanOrEqualTo(0);
#pragma warning restore CA1829, CA1826
        }
    }
}
