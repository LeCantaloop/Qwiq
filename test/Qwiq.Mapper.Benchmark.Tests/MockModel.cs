using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Mapper.Attributes;

using Newtonsoft.Json;

namespace Microsoft.Qwiq.Mapper.Benchmark.Tests
{
    [WorkItemType("Task")]
    public class MockModel : IIdentifiable<int?>
    {
        public const string ForwardLinkName = "NS.SampleLink-Forward";

        public const string ReverseLinkName = "NS.SampleLink-Reverse";

        private string _assignedTo;

        private string _closedBy;

        private IEnumerable<MockModel> _givers;

        private string _history;

        private string _issueType;

        private string _keywords;

        private string _milestone;

        private string _openedBy;

        private string _product;

        private string _productFamily;

        private string _release;

        private string _releaseType;

        private string _resolution;

        private string _status;

        private string _tags;

        private IEnumerable<MockModel> _takers;

        private string _title;

        private string _treePath;

        /// <summary>
        ///     The alias of the user to which this issue is assigned.
        /// </summary>
        [FieldDefinition("Assigned To")]
        public virtual string AssignedTo
        {
            get => _assignedTo ?? string.Empty;
            set => _assignedTo = value;
        }

        /// <summary>
        ///     Used for "As Of" queries. For each revision of an issue, the ChangedDate is when the issue was modified
        ///     (or opened for the first revision), and that bug is considered current until <see cref="RevisedDate" />.
        ///     If the revision is the lastest revision, the revised date is 9999/01/01 to avoid NULL values.
        /// </summary>
        [FieldDefinition("Changed Date")]
        public virtual DateTime ChangedDate { get; set; }

        /// <summary>
        ///     The alias of the user that closed this issue.
        /// </summary>
        [FieldDefinition("Closed By")]
        public virtual string ClosedBy
        {
            get => _closedBy ?? string.Empty;
            set => _closedBy = value;
        }

        /// <summary>
        ///     The DateTime when this issue was closed in the local timezone. Null if the issue is still open.
        /// </summary>
        [FieldDefinition("Closed Date")]
        public virtual DateTime? ClosedDate { get; set; }

        [FieldDefinition("Custom String 01", true)]
        public double Effort { get; set; }

        [WorkItemLink(typeof(SubMockModel), ReverseLinkName)]
        public IEnumerable<MockModel> Givers
        {
            get => _givers ?? Enumerable.Empty<MockModel>();
            internal set => _givers = value;
        }

        [FieldDefinition("History")]
        public virtual string History
        {
            get => _history ?? string.Empty;
            set => _history = value;
        }

        [JsonIgnore]
        public int? Id
        {
            get => ID;
            set => ID = value.GetValueOrDefault();
        }

        [FieldDefinition("Id")]
        public virtual int ID { get; set; }

        /// <summary>
        ///     Type of issue this object represents (e.g. "Code Bug", "Dev Task", "Spec Bug", "Buffer", etc.)
        /// </summary>
        [FieldDefinition("Issue Type")]
        public virtual string IssueType
        {
            get => _issueType ?? string.Empty;
            set => _issueType = value;
        }

        /// <summary>
        ///     The issue's keywords text field. If there are no keywords it returns an empty string.
        /// </summary>
        [FieldDefinition("Keywords")]
        public virtual string KeyWords
        {
            get => _keywords ?? string.Empty;
            set => _keywords = value;
        }

        /// <summary>
        ///     The string of this issue's milestone (e.g. 'M1 Coding', 'M1', etc.).
        /// </summary>
        [FieldDefinition("Iteration Path")]
        public virtual string Milestone
        {
            get => _milestone ?? string.Empty;
            set => _milestone = value;
        }

        /// <summary>
        ///     The user that opened this issue.
        /// </summary>
        [FieldDefinition("Created By")]
        public virtual string OpenedBy
        {
            get => _openedBy ?? string.Empty;
            set => _openedBy = value;
        }

        /// <summary>
        ///     The DateTime when this issue was opened in the local timezone.
        /// </summary>
        [FieldDefinition("Created Date")]
        public virtual DateTime OpenedDate { get; set; }

        /// <summary>
        ///     The priority of the issue (e.g. 1, 2, etc.).
        /// </summary>
        [FieldDefinition("Priority")]
        public virtual int? Priority { get; set; }

        [FieldDefinition("Product")]
        public virtual string Product
        {
            get => _product ?? string.Empty;
            set => _product = value;
        }

        [FieldDefinition("Product Family")]
        public virtual string ProductFamily
        {
            get => _productFamily ?? string.Empty;
            set => _productFamily = value;
        }

        [FieldDefinition("Release")]
        public virtual string Release
        {
            get => _release ?? string.Empty;
            set => _release = value;
        }

        [FieldDefinition("Release Type")]
        public virtual string ReleaseType
        {
            get => _releaseType ?? string.Empty;
            set => _releaseType = value;
        }

        /// <summary>
        ///     The string of this issue's status (e.g. 'By Design', 'Submitted', 'Won't Fix', etc.).
        /// </summary>
        [FieldDefinition("Resolved Reason")]
        public virtual string Resolution
        {
            get => _resolution ?? string.Empty;
            set => _resolution = value;
        }

        /// <summary>
        ///     The DateTime when this issue was last modified.
        /// </summary>
        [FieldDefinition("Revised Date")]
        public virtual DateTime RevisedDate { get; set; }

        /// <summary>
        ///     The string of this issue's status (e.g. 'Active', 'Closed - Completed', etc.).
        /// </summary>
        [FieldDefinition("State")]
        public virtual string Status
        {
            get => _status ?? string.Empty;
            set => _status = value;
        }

        [FieldDefinition("Tags")]
        public virtual string Tags
        {
            get => _tags ?? string.Empty;
            set => _tags = value;
        }

        [WorkItemLink(typeof(MockModel), ForwardLinkName)]
        public IEnumerable<MockModel> Takers
        {
            get => _takers ?? Enumerable.Empty<MockModel>();
            internal set => _takers = value;
        }

        /// <summary>
        ///     The title of the issue.
        /// </summary>
        [FieldDefinition("Title")]
        public virtual string Title
        {
            get => _title ?? string.Empty;
            set => _title = value;
        }

        /// <summary>
        ///     The 'tree path' or 'area path' of the issue (e.g. \IE-Internet Explorer\COMP-Composition and Rendering\).
        /// </summary>
        [FieldDefinition("Area Path")]
        public virtual string TreePath
        {
            get => _treePath ?? string.Empty;
            set => _treePath = value;
        }

        [FieldDefinition("Work Item Type")]
        public virtual string WorkItemType { get; set; }
    }
}