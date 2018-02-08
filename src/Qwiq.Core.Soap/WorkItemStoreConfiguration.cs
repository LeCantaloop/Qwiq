using System.Collections.Generic;

namespace Qwiq.Client.Soap
{
    public class WorkItemStoreConfiguration : Qwiq.WorkItemStoreConfiguration
    {
        internal WorkItemStoreConfiguration()
        {
            WorkItemExpand = WorkItemExpand.All;
            DefaultFields = CoreFieldRefNames.All;
        }

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