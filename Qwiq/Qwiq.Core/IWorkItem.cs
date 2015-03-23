using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.IE.Qwiq
{
    /// <summary>
    /// Wrapper around the TFS WorkItem. This exists so that every agent doesn't need to reference
    /// all the TFS libraries.
    /// </summary>
    public interface IWorkItem
    {
        string AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the string value of the AreaPath field for this work item.
        /// </summary>
        string AreaPath { get; set; }

        /// <summary>
        /// Gets the number of attached files for this work item.
        /// </summary>
        int AttachedFileCount { get; }

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
        /// Closes this WorkItem instance and frees memory that is associated with it.
        /// </summary>
        void Close();

        /// <summary>
        /// Creates a copy of this WorkItem instance.
        /// </summary>
        /// <returns>A new WorkItem instance that is a copy of this WorkItem instance.</returns>
        IWorkItem Copy();

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

        /// <summary>
        /// Validates the fields of this work item.
        /// </summary>
        /// <returns>
        /// True if all fields are valid. False if at least one field is not valid.
        /// </returns>
        bool IsValid();

        bool IsDirty { get; }

        /// <summary>
        /// Gets or sets the string value of the IterationPath field of this work item.
        /// </summary>
        string IterationPath { get; set; }

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
        /// Gets the number of related links of this work item.
        /// </summary>
        int RelatedLinkCount { get; }

        /// <summary>
        /// Reverts all changes that were made since the last save.
        /// </summary>
        void Reset();

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
        /// Saves any pending changes on this work item.
        /// </summary>
        void Save();

        /// <summary>
        /// Gets or sets a string that describes the state of this work item.
        /// </summary>
        string State { get; set; }

        /// <summary>
        /// Gets or sets a string of all the tags on this work item.
        /// </summary>
        string Tags { get; set; }

        string Keywords { get; set; }

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
        /// Gets or sets a string that describes the title of this work item.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets the uniform resource identifier (System.Uri) of this work item.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets an ArrayList of fields in this work item that are not valid.
        /// </summary>
        /// <returns>
        /// An ArrayList of the fields in this work item that are not valid.
        /// </returns>
        IEnumerable<IField> Validate();

        /// <summary>
        /// Gets the links of the work item in this revision.
        /// </summary>
        ICollection<ILink> Links { get; }

        IEnumerable<IAttachment> Attachments { get; }

        /// <summary>
        /// Gets a Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType object
        /// that represents the type of this work item.
        /// </summary>
        /// <exception cref="Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemTypeDeniedOrNotExistException">
        /// The Type property is null.
        /// </exception>
         IWorkItemType Type { get; }

         IEnumerable<IWorkItemLink> WorkItemLinks { get; }

         int Rev { get; }

        IRelatedLink CreateRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem relatedWorkItem);

        IHyperlink CreateHyperlink(string location);

        IWorkItemLink CreateWorkItemLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem targetWorkItem);
    }
}
