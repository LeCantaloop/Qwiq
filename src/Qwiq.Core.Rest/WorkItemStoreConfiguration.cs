using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qwiq.Client.Rest
{
    public sealed class WorkItemStoreConfiguration : Qwiq.WorkItemStoreConfiguration
    {
        private static readonly string[] DefaultFieldValue =
            {
                CoreFieldRefNames.AssignedTo,
                CoreFieldRefNames.ChangedBy,
                CoreFieldRefNames.ChangedDate,
                CoreFieldRefNames.Id,
                CoreFieldRefNames.State,
                CoreFieldRefNames.TeamProject,
                CoreFieldRefNames.Title,
                CoreFieldRefNames.WorkItemType
            };

        private static readonly string[] MandatoryFields =
        {
            CoreFieldRefNames.TeamProject,
            CoreFieldRefNames.WorkItemType,
        };

        private HashSet<string> _defaultFields;

        private WorkItemExpand _workItemExpand;

        internal WorkItemStoreConfiguration()
        {
            WorkItemExpand = WorkItemExpand.All;
            WorkItemErrorPolicy = WorkItemErrorPolicy.Omit;
        }

        /// <summary>
        /// Gets or sets a value indicating which fields to load when querying for a work item. Has no effect if <see cref="WorkItemExpand"/> is set to
        /// <see cref="M:WorkItemExpand.Full"/> or <see cref="M:WorkItemExpand.Fields"/>. Defaults to <see cref="M:CoreFieldRefNames.AssignedTo"/>, <see cref="M:CoreFieldRefNames.ChangedBy"/>,
        /// <see cref="M:CoreFieldRefNames.ChangedDate"/>, <see cref="M:CoreFieldRefNames.Id"/>, <see cref="M:CoreFieldRefNames.State"/>, <see cref="M:CoreFieldRefNames.TeamProject"/>,
        /// <see cref="M:CoreFieldRefNames.Title"/>, <see cref="M:CoreFieldRefNames.WorkItemType"/>.
        /// </summary>
        public sealed override IEnumerable<string> DefaultFields
        {
            get => _defaultFields;
            set
            {
                if (Equals(_defaultFields, value)) return;

                if (WorkItemExpand != WorkItemExpand.None)
                {
                    Trace.TraceWarning(
                                       $"The {nameof(DefaultFields)} parameter can not be used with the {nameof(WorkItemExpand)} parameter. Setting {nameof(WorkItemExpand)} to {WorkItemExpand.None}.");
                    _workItemExpand = WorkItemExpand.None;
                }
                if (value == null || !value.Any()) _defaultFields.Clear();
                else
                {
                    var fs = new HashSet<string>(value, Comparer.OrdinalIgnoreCase);
                    foreach (var f in MandatoryFields)
                    {
                        fs.Add(f);
                    }
                    _defaultFields = fs;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the expected behavior when querying for a work item that does not exist. Default value is <see cref="M:WorkItemErrorPolicy.Omit"/>
        /// </summary>
        public override WorkItemErrorPolicy WorkItemErrorPolicy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating which sub-elements of <see cref="TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem"/> to expand. Default value is <see cref="M:WorkItemExpand.All"/>.
        /// </summary>
        /// <remarks>
        /// When value is set to <see cref="M:WorkItemExpand.All"/>, the value of <see cref="M:WorkItemStoreConfiguration.DefaultFields"/> is set to null.
        /// </remarks>
        public override WorkItemExpand WorkItemExpand
        {
            get => _workItemExpand;
            set
            {
                _workItemExpand = value;
                if (value != WorkItemExpand.None)
                {
                    Trace.TraceWarning(
                                       $"The {nameof(WorkItemExpand)} parameter can not be used with the {nameof(DefaultFields)} parameter. Setting {nameof(DefaultFields)} to NULL.");
                    _defaultFields = null;
                }
                else
                {
                    _defaultFields = new HashSet<string>(DefaultFieldValue, Comparer.OrdinalIgnoreCase);
                }
            }
        }
    }
}