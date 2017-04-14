using System;

namespace Microsoft.Qwiq
{
    public interface IWorkItemCommon : IWorkItemCore
    {
        int? AreaId { get; set; }

        /// <summary>
        ///     Gets or sets the string value of the AreaPath field for this work item.
        /// </summary>
        string AreaPath { get; set; }

        string AssignedTo { get; set; }

        /// <summary>
        ///     Gets the number of attached files for this work item.
        /// </summary>
        int? AttachedFileCount { get; }

        /// <summary>
        ///     Gets the string value of the ChangedBy field for this work item.
        /// </summary>
        string ChangedBy { get; }

        /// <summary>
        ///     Gets the System.DateTime object that represents the date and time that this
        ///     work item was last changed.
        /// </summary>
        DateTime? ChangedDate { get; }

        /// <summary>
        ///     Gets the string value of the CreatedBy field for this work item.
        /// </summary>
        string CreatedBy { get; }

        /// <summary>
        ///     Gets the System.DateTime object that represents the date and time that this
        ///     work item was created.
        /// </summary>
        DateTime? CreatedDate { get; }

        /// <summary>
        ///     Gets or sets a string that describes this work item.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        ///     Gets the number of external links in this work item.
        /// </summary>
        int? ExternalLinkCount { get; }

        string History { get; set; }

        /// <summary>
        ///     Gets the number of hyperlinks in this work item.
        /// </summary>
        int? HyperLinkCount { get; }

        int? IterationId { get; set; }

        /// <summary>
        ///     Gets or sets the string value of the IterationPath field of this work item.
        /// </summary>
        string IterationPath { get; set; }

        /// <summary>
        ///     Gets the number of related links of this work item.
        /// </summary>
        int? RelatedLinkCount { get; }

        /// <summary>
        ///     Gets a System.DateTime object that represents the revision date and time
        ///     of this work item.
        /// </summary>
        DateTime? RevisedDate { get; }

        /// <summary>
        ///     Gets or sets a string that describes the state of this work item.
        /// </summary>
        string State { get; set; }

        /// <summary>
        ///     Gets or sets a string of all the tags on this work item.
        /// </summary>
        string Tags { get; set; }

        /// <summary>
        ///     Gets or sets a string that describes the title of this work item.
        /// </summary>
        string Title { get; set; }

        int? Watermark { get; }

        string WorkItemType { get; }

        string TeamProject { get; }
    }
}