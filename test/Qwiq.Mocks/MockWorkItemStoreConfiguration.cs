using System.Collections.Generic;

namespace Qwiq.Mocks
{
    public class MockWorkItemStoreConfiguration : WorkItemStoreConfiguration
    {
        /// <inheritdoc/>
        ///
        public override IEnumerable<string> DefaultFields { get; set; }

        /// <inheritdoc/>
        ///
        public override WorkItemErrorPolicy WorkItemErrorPolicy { get; set; }

        /// <inheritdoc/>
        ///
        public override WorkItemExpand WorkItemExpand { get; set; }
    }
}