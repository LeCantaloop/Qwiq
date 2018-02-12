using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Qwiq
{
    public abstract class WorkItemStoreConfiguration
    {
        private int _pageSize;

        protected WorkItemStoreConfiguration()
        {
            LazyLoadingEnabled = true;
            ProxyCreationEnabled = true;
            PageSize = 50;
        }

        public abstract IEnumerable<string> DefaultFields { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether lazy loading of certain properties is enabled. Lazy loading is enabled by default
        /// </summary>
        public bool LazyLoadingEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the number of work items to be returned in a page. Value is 50 by default.
        /// </summary>
        public int PageSize

        {
            get => _pageSize;
            set
            {
                const int MinimumBatchSize = 50;
                const int MaximumBatchSize = 200;

                Contract.Requires(value >= MinimumBatchSize);
                Contract.Requires(value <= MaximumBatchSize);

                if (value < MinimumBatchSize || value > MaximumBatchSize) throw new PageSizeRangeException();

                _pageSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the framework will create instances of dynamically generated proxy classes whenever it creates an instance of a type.
        /// </summary>
        /// <remarks>
        /// Note that even if proxy creation is enabled with this flag, proxy instances will only be created for entity types that meet the requirements for being proxied. Proxy
        /// creation is enabled by default.
        /// </remarks>
        public bool ProxyCreationEnabled { get; set; }

        public abstract WorkItemErrorPolicy WorkItemErrorPolicy { get; set; }

        public abstract WorkItemExpand WorkItemExpand { get; set; }
    }
}