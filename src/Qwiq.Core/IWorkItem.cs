using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    /// <summary>
    /// Wrapper around the TFS WorkItem. This exists so that every agent doesn't need to reference
    /// all the TFS libraries.
    /// </summary>
    public interface IWorkItem
    {
        /// <summary>
        /// Gets or sets the string value of the AreaPath field for this work item.
        /// </summary>
        string AreaPath { get; set; }

        string AssignedTo { get; set; }

        /// <summary>
        /// Gets the number of attached files for this work item.
        /// </summary>
        int AttachedFileCount { get; }

        IEnumerable<IAttachment> Attachments { get; }

        /// <summary>
        /// Gets the string value of the ChangedBy field for this work item.
        /// </summary>
        string ChangedBy { get; }

        /// <summary>
        /// Gets the System.DateTime object that represents the date and time that this
        /// work item was last changed.
        /// </summary>
        DateTime ChangedDate { get; }

        /// <summary>
        /// Gets the string value of the CreatedBy field for this work item.
        /// </summary>
        string CreatedBy { get; }

        /// <summary>
        /// Gets the System.DateTime object that represents the date and time that this
        /// work item was created.
        /// </summary>
        DateTime CreatedDate { get; }

        /// <summary>
        /// Gets or sets a string that describes this work item.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets the number of external links in this work item.
        /// </summary>
        int ExternalLinkCount { get; }

        IFieldCollection Fields { get; }

        /// <summary>
        /// Gets or sets the string value of the History field for this work item.
        /// </summary>
        string History { get; set; }

        /// <summary>
        /// Gets the number of hyperlinks in this work item.
        /// </summary>
        int HyperLinkCount { get; }

        /// <summary>
        /// Gets the ID of this work item.
        /// </summary>
        int Id { get; }

        bool IsDirty { get; }

        /// <summary>
        /// Gets or sets the string value of the IterationPath field of this work item.
        /// </summary>
        string IterationPath { get; set; }

        string Keywords { get; set; }

        /// <summary>
        /// Gets the links of the work item in this revision.
        /// </summary>
        ICollection<ILink> Links { get; }

        /// <summary>
        /// Gets the number of related links of this work item.
        /// </summary>
        int RelatedLinkCount { get; }

        int Rev { get; }

        /// <summary>
        /// Gets a System.DateTime object that represents the revision date and time
        /// of this work item.
        /// </summary>
        DateTime RevisedDate { get; }

        /// <summary>
        /// Gets the integer that represents the revision number of this work item.
        /// </summary>
        int Revision { get; }

        /// <summary>
        /// Gets an object that represents a collection of valid revision numbers for this work
        /// item.
        /// </summary>
        IEnumerable<IRevision> Revisions { get; }

        /// <summary>
        /// Gets or sets a string that describes the state of this work item.
        /// </summary>
        string State { get; set; }

        /// <summary>
        /// Gets or sets a string of all the tags on this work item.
        /// </summary>
        string Tags { get; set; }

        /// <summary>
        /// Gets or sets a string that describes the title of this work item.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets a Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType object
        /// that represents the type of this work item.
        /// </summary>
        /// <exception cref="Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemTypeDeniedOrNotExistException">
        /// The Type property is null.
        /// </exception>
        IWorkItemType Type { get; }

        /// <summary>
        /// Gets the uniform resource identifier (System.Uri) of this work item.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets or sets the value of a field in this work item that is specified by
        /// the field name.
        /// </summary>
        /// <param name="name">
        /// The string that is passed in name could be either the field name or a reference name.
        /// </param>
        /// <returns>The object that is contained in this field.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// The name parameter is null.
        /// </exception>
        object this[string name] { get; set; }

        /// <summary>
        /// Closes this WorkItem instance and frees memory that is associated with it.
        /// </summary>
        void Close();

        /// <summary>
        /// Creates a copy of this WorkItem instance.
        /// </summary>
        /// <returns>A new WorkItem instance that is a copy of this WorkItem instance.</returns>
        IWorkItem Copy();
        IHyperlink CreateHyperlink(string location);

        IRelatedLink CreateRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem relatedWorkItem);

        /// <summary>
        /// Validates the fields of this work item.
        /// </summary>
        /// <returns>
        /// True if all fields are valid. False if at least one field is not valid.
        /// </returns>
        bool IsValid();

        /// <summary>
        /// Opens this work item for modification.
        /// </summary>
        void Open();

        /// <summary>
        /// Opens this work item for modification when transmitting minimal amounts of data over the network.
        /// </summary>
        /// This WorkItem instance does not belong to a Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemCollection.
        /// This WorkItem instance could not be opened for edit correctly.
        void PartialOpen();

        /// <summary>
        /// Reverts all changes that were made since the last save.
        /// </summary>
        void Reset();

        /// <summary>
        /// Saves any pending changes on this work item.
        /// </summary>
        void Save();

        /// <summary>
        /// Saves any pending changes on this work item.
        /// </summary>
        /// <param name="saveFlags">
        /// If set to <see cref="SaveFlags.MergeLinks"/>, does not return errors if the link that
        /// is being added already exists or the link that is being removed was already removed.
        /// </param>
        void Save(SaveFlags saveFlags);

        /// <summary>
        /// Gets an ArrayList of fields in this work item that are not valid.
        /// </summary>
        /// <returns>
        /// An ArrayList of the fields in this work item that are not valid.
        /// </returns>
        IEnumerable<IField> Validate();
    }
}
