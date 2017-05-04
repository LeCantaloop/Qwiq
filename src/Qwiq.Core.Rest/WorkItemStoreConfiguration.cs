using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Client.Rest
{
    public class WorkItemStoreConfiguration : Qwiq.WorkItemStoreConfiguration
    {
        private WorkItemExpand _workItemExpand;

        private IEnumerable<string> _defaultFields;

        private static readonly string[] DefaultFieldValue = {
                                            CoreFieldRefNames.AssignedTo,
                                            CoreFieldRefNames.ChangedBy,
                                            CoreFieldRefNames.ChangedDate,
                                            CoreFieldRefNames.Id,
                                            CoreFieldRefNames.State,
                                            CoreFieldRefNames.TeamProject,
                                            CoreFieldRefNames.Title,
                                            CoreFieldRefNames.WorkItemType
                                        };

        internal WorkItemStoreConfiguration()
        {
            WorkItemExpand = WorkItemExpand.All;
            WorkItemErrorPolicy = WorkItemErrorPolicy.Omit;
            DefaultFields = null;
        }

        /// <summary>
        ///     Gets or sets a value indicating which fields to load when querying for a work item. Has no effect if
        ///     <see cref="WorkItemExpand" /> is set to <see cref="M:WorkItemExpand.Full" /> or
        ///     <see cref="M:WorkItemExpand.Fields" />. Defaults to
        ///     <see cref="M:CoreFieldRefNames.Id" />, <see cref="M:CoreFieldRefNames.WorkItemType" />,
        ///     <see cref="M:CoreFieldRefNames.Title" />, <see cref="M:CoreFieldRefNames.AssignedTo" />,
        ///     <see cref="M:CoreFieldRefNames.State" />, <see cref="M:CoreFieldRefNames.Tags" />
        /// </summary>
        public IEnumerable<string> DefaultFields
        {
            get { return _defaultFields; }
            set
            {
                if (WorkItemExpand != WorkItemExpand.None)
                {
                    Trace.TraceWarning($"The {nameof(WorkItemExpand)} parameter can not be used with the {nameof(DefaultFields)} parameter.");
                    _workItemExpand = WorkItemExpand.None;
                }
                _defaultFields = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating the expected behavior when querying for a work item that does not exist. Default
        ///     value is <see cref="M:WorkItemErrorPolicy.Omit" />
        /// </summary>
        public WorkItemErrorPolicy WorkItemErrorPolicy { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating which sub-elements of
        ///     <see cref="TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem" /> to expand. Default value is
        ///     <see cref="M:WorkItemExpand.All" />.
        /// </summary>
        public WorkItemExpand WorkItemExpand
        {
            get { return _workItemExpand; }
            set
            {
                _workItemExpand = value;
                if (value != WorkItemExpand.None)
                {
                    Trace.TraceWarning(
                                       $"The {nameof(WorkItemExpand)} parameter can not be used with the {nameof(DefaultFields)} parameter.");
                    _defaultFields = null;
                }
                else
                {
                    _defaultFields = DefaultFieldValue;
                }
            }
        }
    }
}