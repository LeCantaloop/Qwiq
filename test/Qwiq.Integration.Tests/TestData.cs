namespace Qwiq
{
    /// <summary>
    /// Centralized test data constants for integration tests against qwiq-sandbox.visualstudio.com/WIT
    /// </summary>
    /// <remarks>
    /// This class contains all work item IDs, identity references, and project configuration
    /// used by integration tests. Centralizing these values makes it easy to update when
    /// the sandbox environment changes.
    ///
    /// Sandbox Environment:
    /// - Organization: https://qwiq-sandbox.visualstudio.com
    /// - Project: WIT (ID: 0a4c0240-1a67-45de-93db-fc1de9f54ffb)
    /// - Process Template: WIT_TEST
    /// </remarks>
    public static class TestData
    {
        #region Work Item IDs

        /// <summary>
        /// Basic Bug work item (ID: 1) - "Integration Test"
        /// Use for: Basic work item retrieval tests, single ID tests, flat queries
        /// Replaces legacy ID: 10726528
        /// </summary>
        public const int BasicWorkItemId = 1;

        /// <summary>
        /// Bug with Related links to IDs 1 and 4 (ID: 5) - "Work Item with Links for Integration Tests"
        /// Use for: Link traversal tests, work item with links tests
        /// Replaces legacy ID: 6413554
        /// </summary>
        /// <remarks>
        /// Current links:
        /// - Related: ID 1, ID 4
        /// Note: Legacy ID 6413554 had 147 related, 14 external, 8 hyperlinks, 8 attachments.
        /// Additional links can be added to enhance test coverage.
        /// </remarks>
        public const int WorkItemWithLinksId = 5;

        /// <summary>
        /// User Story parent with 2 Task children (ID: 3) - "Parent Story for Integration Tests"
        /// Use for: Hierarchy queries, parent-child relationship tests
        /// Replaces legacy ID: 10726623
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// - Parent: ID 3 (User Story)
        ///   - Child: ID 2 (Task)
        ///   - Child: ID 6 (Task)
        /// </remarks>
        public const int HierarchyParentId = 3;

        /// <summary>
        /// Child Task under HierarchyParentId (ID: 2) - "Child Task for Integration Tests"
        /// Use for: Child work item tests
        /// </summary>
        public const int HierarchyChildId = 2;

        /// <summary>
        /// Bug for mapper tests (ID: 4) - "Bug for Mapper Integration Tests"
        /// Use for: Attribute mapper tests, WIQL mapper tests
        /// Replaces legacy ID: 8663955
        /// </summary>
        public const int MapperBugId = 4;

        #endregion

        #region Test User Identity

        /// <summary>
        /// Test user email/UPN for identity tests
        /// </summary>
        public const string TestUserUpn = "rjmurillo@msn.com";

        /// <summary>
        /// Test user alias (username portion of email)
        /// </summary>
        public const string TestUserAlias = "rjmurillo";

        /// <summary>
        /// Test user display name
        /// </summary>
        public const string TestUserDisplayName = "Richard Murillo";

        #endregion

        #region Project Configuration

        /// <summary>
        /// Project name in Azure DevOps
        /// </summary>
        public const string ProjectName = "WIT";

        /// <summary>
        /// Default team name
        /// </summary>
        public const string TeamName = "WIT Team";

        #endregion
    }
}