using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq;
using Qwiq.Tests.Common;
using Shouldly;
using System;
using System.Globalization;
using System.Linq;

namespace Qwiq.Client.Rest
{
    /// <summary>
    /// Integration tests for REST client query execution against qwiq-sandbox.
    /// </summary>
    /// <remarks>
    /// These tests validate the REST client's ability to execute WIQL queries against
    /// a real Azure DevOps instance. They use Windows authentication with interactive
    /// login prompts when needed.
    ///
    /// Test Environment:
    /// - Organization: https://qwiq-sandbox.visualstudio.com
    /// - Project: WIT
    /// - See TestData.cs for work item IDs and test user information.
    /// </remarks>
    [TestClass]
    [TestCategory("REST")]
    public class Given_WorkItemStore_When_Querying_With_WIQL : TimedContextSpecification
    {
        private IWorkItemStore? _store;
        private IWorkItemCollection? _result;

        // Query for the basic test Bug work item
        private static readonly string TestWiql = ((FormattableString)$"SELECT [System.Id], [System.WorkItemType], [System.Title] FROM WorkItems WHERE [System.Id] = {TestData.BasicWorkItemId}").ToString(CultureInfo.InvariantCulture);

        public override void Given()
        {
            // Create REST store using integration settings (Windows auth with interactive prompt)
            _store = TimedAction(() => IntegrationSettings.CreateRestStore(), "REST", "Create WorkItemStore");
        }

        public override void When()
        {
            // Execute query against Azure DevOps
            _result = TimedAction(() => _store?.Query(TestWiql), "REST", "Query WIQL");
        }

        [TestMethod]
        public void Should_Return_One_WorkItem()
        {
            _result.ShouldNotBeNull();
#pragma warning disable CA1829, CA1826 // Use Count property when available - IWorkItemCollection doesn't have Count
            _result.Count().ShouldBe(1);
#pragma warning restore CA1829, CA1826
        }

        [TestMethod]
        public void Should_Have_Correct_WorkItem_Id()
        {
            _result.ShouldNotBeNull();
            var workItem = _result.First();
            workItem.Id.ShouldBe(TestData.BasicWorkItemId);
        }

        [TestMethod]
        public void Should_Have_Bug_WorkItem_Type()
        {
            _result.ShouldNotBeNull();
            var workItem = _result.First();
            workItem.Type?.Name.ShouldBe("Bug");
        }

        public override void Cleanup()
        {
            (_store as IDisposable)?.Dispose();
            base.Cleanup();
        }
    }

    /// <summary>
    /// Integration tests for REST client querying multiple work items.
    /// </summary>
    [TestClass]
    [TestCategory("REST")]
    public class Given_WorkItemStore_When_Querying_Multiple_Bugs : TimedContextSpecification
    {
        private IWorkItemStore? _store;
        private IWorkItemCollection? _result;

        // Query for all Bug work items in the project
        private const string TestWiql = "SELECT [System.Id], [System.WorkItemType], [System.Title] FROM WorkItems WHERE [System.WorkItemType] = 'Bug' AND [System.TeamProject] = 'WIT'";

        public override void Given()
        {
            _store = TimedAction(() => IntegrationSettings.CreateRestStore(), "REST", "Create WorkItemStore");
        }

        public override void When()
        {
            _result = TimedAction(() => _store?.Query(TestWiql), "REST", "Query Multiple Bugs");
        }

        [TestMethod]
        public void Should_Return_At_Least_One_WorkItem()
        {
            _result.ShouldNotBeNull();
            _result.Any().ShouldBeTrue("Expected at least one Bug work item in the WIT project");
        }

        [TestMethod]
        public void Should_All_Be_Bug_Type()
        {
            _result.ShouldNotBeNull();
            _result.All(wi => wi.Type?.Name == "Bug").ShouldBeTrue("All returned work items should be Bugs");
        }

        [TestMethod]
        public void Should_Include_Known_Test_Bug()
        {
            _result.ShouldNotBeNull();
            var ids = _result.Select(wi => wi.Id).ToArray();
            ids.ShouldContain(TestData.BasicWorkItemId, $"Expected to find Bug ID {TestData.BasicWorkItemId}");
        }

        public override void Cleanup()
        {
            (_store as IDisposable)?.Dispose();
            base.Cleanup();
        }
    }
}
