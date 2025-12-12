using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qwiq.Mocks;
using Shouldly;
using System.Linq;

namespace Qwiq.Soap
{
    /// <summary>
    /// Tests for SOAP client WIQL query execution using Moq.
    /// </summary>
    /// <remarks>
    /// These tests exercise the SOAP client implementation including:
    /// - Query factory delegation
    /// - Query execution pipeline
    /// - Work item materialization
    ///
    /// Unlike integration tests, these run without TFS connectivity
    /// and provide fast, deterministic results using mocked dependencies.
    /// </remarks>
    [TestClass]
    [TestCategory("SoapUnit")]
    public class Given_SOAP_WorkItemStore_When_Querying_Single_Bug : SoapContextSpecification
    {
        private IWorkItemCollection? _result;
        private const int TestWorkItemId = 1;
        private const string TestWiql = "SELECT [System.Id] FROM WorkItems WHERE [System.Id] = 1";

        public override void Given()
        {
            base.Given();

            // Create test work item
            var workItemType = new MockWorkItemType("Bug");
            var testWorkItem = new MockWorkItem(
                workItemType,
                TestWorkItemId,
                new System.Collections.Generic.KeyValuePair<string, object?>(CoreFieldRefNames.Title, "Test Bug"),
                new System.Collections.Generic.KeyValuePair<string, object?>(CoreFieldRefNames.State, "Active"));

            // Create mock query that returns our test work item
            var mockQuery = new Mock<IQuery>(MockBehavior.Strict);
            mockQuery
                .Setup(x => x.RunQuery())
                .Returns(new WorkItemCollection(new[] { testWorkItem }));

            // Setup query factory to return our mock query
            // Note: IWorkItemStore.Query defaults dayPrecision to false (interface default)
            MockQueryFactory?
                .Setup(x => x.Create(TestWiql, false))
                .Returns(mockQuery.Object);
        }

        public override void When()
        {
            _result = Store?.Query(TestWiql);
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
            workItem.Title.ShouldBe("Test Bug");
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
    /// Tests for SOAP client querying multiple work items using mocked dependencies.
    /// </summary>
    [TestClass]
    [TestCategory("SoapUnit")]
    public class Given_SOAP_WorkItemStore_When_Querying_Multiple_Bugs : SoapContextSpecification
    {
        private IWorkItemCollection? _result;
        private const string TestWiql = "SELECT [System.Id] FROM WorkItems WHERE [System.WorkItemType] = 'Bug'";

        public override void Given()
        {
            base.Given();

            // Create multiple test work items
            var workItemType = new MockWorkItemType("Bug");
            var workItem1 = new MockWorkItem(
                workItemType,
                1,
                new System.Collections.Generic.KeyValuePair<string, object?>(CoreFieldRefNames.Title, "First Bug"),
                new System.Collections.Generic.KeyValuePair<string, object?>(CoreFieldRefNames.State, "Active"));

            var workItem2 = new MockWorkItem(
                workItemType,
                2,
                new System.Collections.Generic.KeyValuePair<string, object?>(CoreFieldRefNames.Title, "Second Bug"),
                new System.Collections.Generic.KeyValuePair<string, object?>(CoreFieldRefNames.State, "Active"));

            var workItem3 = new MockWorkItem(
                workItemType,
                3,
                new System.Collections.Generic.KeyValuePair<string, object?>(CoreFieldRefNames.Title, "Third Bug"),
                new System.Collections.Generic.KeyValuePair<string, object?>(CoreFieldRefNames.State, "Closed"));

            // Create mock query that returns our test work items
            var mockQuery = new Mock<IQuery>(MockBehavior.Strict);
            mockQuery
                .Setup(x => x.RunQuery())
                .Returns(new WorkItemCollection(new[] { workItem1, workItem2, workItem3 }));

            // Setup query factory to return our mock query
            // Note: IWorkItemStore.Query defaults dayPrecision to false (interface default)
            MockQueryFactory?
                .Setup(x => x.Create(TestWiql, false))
                .Returns(mockQuery.Object);
        }

        public override void When()
        {
            _result = Store?.Query(TestWiql);
        }

        [TestMethod]
        public void Should_Return_Three_WorkItems()
        {
            _result.ShouldNotBeNull();
#pragma warning disable CA1829, CA1826
            _result.Count().ShouldBe(3);
#pragma warning restore CA1829, CA1826
        }

        [TestMethod]
        public void Should_Contain_All_Expected_Ids()
        {
            _result.ShouldNotBeNull();
            var ids = _result.Select(wi => wi.Id).OrderBy(id => id).ToArray();
            int[] expectedIds = { 1, 2, 3 };
            ids.ShouldBe(expectedIds);
        }

        [TestMethod]
        public void Should_All_Be_Bug_Type()
        {
            _result.ShouldNotBeNull();
            _result.All(wi => wi.WorkItemType == "Bug").ShouldBeTrue();
        }
    }

    /// <summary>
    /// Tests for SOAP client querying by IDs using mocked dependencies.
    /// </summary>
    [TestClass]
    [TestCategory("SoapUnit")]
    public class Given_SOAP_WorkItemStore_When_Querying_By_Ids : SoapContextSpecification
    {
        private IWorkItemCollection? _result;
        private static readonly int[] TestIds = { 1, 2, 3 };

        public override void Given()
        {
            base.Given();

            // Create test work items
            var workItemType = new MockWorkItemType("Task");
            var workItem1 = new MockWorkItem(workItemType, 1);
            var workItem2 = new MockWorkItem(workItemType, 2);
            var workItem3 = new MockWorkItem(workItemType, 3);

            // Create mock query that returns our test work items
            var mockQuery = new Mock<IQuery>(MockBehavior.Strict);
            mockQuery
                .Setup(x => x.RunQuery())
                .Returns(new WorkItemCollection(new[] { workItem1, workItem2, workItem3 }));

            // Setup query factory to return our mock query
            // Cast to IEnumerable<int> to avoid ambiguity with Create(IEnumerable<int>, string) overload
            MockQueryFactory?
                .Setup(x => x.Create((System.Collections.Generic.IEnumerable<int>)TestIds, (System.DateTime?)null))
                .Returns(mockQuery.Object);
        }

        public override void When()
        {
            _result = Store?.Query(TestIds);
        }

        [TestMethod]
        public void Should_Return_Correct_Count()
        {
            _result.ShouldNotBeNull();
#pragma warning disable CA1829, CA1826
            _result.Count().ShouldBe(3);
#pragma warning restore CA1829, CA1826
        }

        [TestMethod]
        public void Should_Return_Correct_Ids()
        {
            _result.ShouldNotBeNull();
            var ids = _result.Select(wi => wi.Id).OrderBy(id => id).ToArray();
            ids.ShouldBe(TestIds);
        }
    }

    /// <summary>
    /// Tests for SOAP client empty query results.
    /// </summary>
    [TestClass]
    [TestCategory("SoapUnit")]
    public class Given_SOAP_WorkItemStore_When_Query_Returns_No_Results : SoapContextSpecification
    {
        private IWorkItemCollection? _result;
        private const string TestWiql = "SELECT [System.Id] FROM WorkItems WHERE [System.Id] = -1";

        public override void Given()
        {
            base.Given();

            // Create mock query that returns no work items
            var mockQuery = new Mock<IQuery>(MockBehavior.Strict);
            mockQuery
                .Setup(x => x.RunQuery())
                .Returns(new WorkItemCollection(System.Array.Empty<IWorkItem>()));

            // Setup query factory to return our mock query
            // Note: IWorkItemStore.Query defaults dayPrecision to false (interface default)
            MockQueryFactory?
                .Setup(x => x.Create(TestWiql, false))
                .Returns(mockQuery.Object);
        }

        public override void When()
        {
            _result = Store?.Query(TestWiql);
        }

        [TestMethod]
        public void Should_Return_Empty_Collection()
        {
            _result.ShouldNotBeNull();
#pragma warning disable CA1829, CA1826
            _result.Count().ShouldBe(0);
#pragma warning restore CA1829, CA1826
        }

        [TestMethod]
        public void Should_Not_Be_Null()
        {
            _result.ShouldNotBeNull();
        }
    }
}
