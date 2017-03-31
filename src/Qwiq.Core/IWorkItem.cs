using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    /// <summary>
    ///     Wrapper around the TFS WorkItem. This exists so that every agent doesn't need to reference
    ///     all the TFS libraries.
    /// </summary>
    public interface IWorkItem : IWorkItemCommon
    {
        new int AttachedFileCount { get; }

        IEnumerable<IAttachment> Attachments { get; }

        new DateTime ChangedDate { get; }

        new DateTime CreatedDate { get; }

        new int ExternalLinkCount { get; }

        IFieldCollection Fields { get; }

        new int HyperLinkCount { get; }

        new int RelatedLinkCount { get; }

        new DateTime RevisedDate { get; }

        /// <summary>
        ///     Gets the ID of this work item.
        /// </summary>
        new int Id { get; }

        bool IsDirty { get; }

        string Keywords { get; set; }

        /// <summary>
        ///     Gets the links of the work item in this revision.
        /// </summary>
        ICollection<ILink> Links { get; }

        new int Rev { get; }

        /// <summary>
        ///     Gets the integer that represents the revision number of this work item.
        /// </summary>
        /// int Revision { get; }
        /// <summary>
        ///     Gets an object that represents a collection of valid revision numbers for this work
        ///     item.
        /// </summary>
        IEnumerable<IRevision> Revisions { get; }

        /// <summary>
        ///     Gets a Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType object
        ///     that represents the type of this work item.
        /// </summary>
        /// <exception cref="Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemTypeDeniedOrNotExistException">
        ///     The Type property is null.
        /// </exception>
        IWorkItemType Type { get; }

        /// <summary>
        ///     Gets the uniform resource identifier (System.Uri) of this work item.
        /// </summary>
        [Obsolete(
            "This property is deprecated and will be removed in a future release. See IWorkItemReference.Url instead.")]
        Uri Uri { get; }

        /// <summary>
        ///     Applies the server rules for validation and fix up to the work item.
        /// </summary>
        /// <param name="doNotUpdateChangedBy">
        ///     If true, will set ChangedBy to the user context of the <see cref="IWorkItemStore" />.
        ///     If false, ChangedBy will not be modified.
        /// </param>
        /// <remarks>
        ///     Use ApplyRules(true) in the case where you want "transparent fix ups".
        /// </remarks>
        void ApplyRules(bool doNotUpdateChangedBy = false);

        /// <summary>
        ///     Closes this WorkItem instance and frees memory that is associated with it.
        /// </summary>
        void Close();

        /// <summary>
        ///     Creates a copy of this WorkItem instance.
        /// </summary>
        /// <returns>A new WorkItem instance that is a copy of this WorkItem instance.</returns>
        IWorkItem Copy();

        IHyperlink CreateHyperlink(string location);

        IRelatedLink CreateRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem relatedWorkItem);

        /// <summary>
        ///     Validates the fields of this work item.
        /// </summary>
        /// <returns>
        ///     True if all fields are valid. False if at least one field is not valid.
        /// </returns>
        bool IsValid();

        /// <summary>
        ///     Opens this work item for modification.
        /// </summary>
        void Open();

        /// <summary>
        ///     Opens this work item for modification when transmitting minimal amounts of data over the network.
        /// </summary>
        /// This WorkItem instance does not belong to a Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemCollection.
        /// This WorkItem instance could not be opened for edit correctly.
        void PartialOpen();

        /// <summary>
        ///     Reverts all changes that were made since the last save.
        /// </summary>
        void Reset();

        /// <summary>
        ///     Saves any pending changes on this work item.
        /// </summary>
        void Save();

        /// <summary>
        ///     Saves any pending changes on this work item.
        /// </summary>
        /// <param name="saveFlags">
        ///     If set to <see cref="SaveFlags.MergeLinks" />, does not return errors if the link that
        ///     is being added already exists or the link that is being removed was already removed.
        /// </param>
        void Save(SaveFlags saveFlags);

        /// <summary>
        ///     Gets an ArrayList of fields in this work item that are not valid.
        /// </summary>
        /// <returns>
        ///     An ArrayList of the fields in this work item that are not valid.
        /// </returns>
        IEnumerable<IField> Validate();
    }
}