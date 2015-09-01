using System;
using System.Collections.Generic;
using System.Linq;
using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Proxies
{
    /// <summary>
    /// Wrapper around the TFS WorkItem. This exists so that every agent doesn't need to reference
    /// all the TFS libraries.
    /// </summary>
    public class WorkItemProxy : IWorkItem
    {
        private readonly Tfs.WorkItem _item;

        internal WorkItemProxy(Tfs.WorkItem item)
        {
            _item = item;
        }

        public string AssignedTo
        {
            get { return _item["Assigned To"].ToString(); }
            set { _item["Assigned To"] = value; }
        }

        /// <summary>
        /// Gets or sets the string value of the AreaPath field for this work item.
        /// </summary>
        public string AreaPath
        {
            get { return _item.AreaPath; }
            set { _item.AreaPath = value; }
        }

        /// <summary>
        /// Gets the number of attached files for this work item.
        /// </summary>
        public int AttachedFileCount
        {
            get { return _item.AttachedFileCount; }
        }

        /// <summary>
        /// Gets the Microsoft.TeamFoundation.WorkItemTracking.Client.AttachmentCollection
        /// object that represents the attachments that belong to this work item.
        /// </summary>
        public IEnumerable<IAttachment> Attachments
        {
            get { return _item.Attachments.Cast<Tfs.Attachment>().Select(item => new AttachmentProxy(item)); }
        }

        /// <summary>
        /// Gets the string value of the ChangedBy field for this work item.
        /// </summary>
        public string ChangedBy
        {
            get { return _item.ChangedBy; }
        }

        /// <summary>
        /// Gets the System.DateTime object that represents the date and time that this
        /// work item was last changed.
        /// </summary>
        public DateTime ChangedDate
        {
            get { return _item.ChangedDate; }
        }

        /// <summary>
        /// Gets the string value of the CreatedBy field for this work item.
        /// </summary>
        public string CreatedBy
        {
            get { return _item.CreatedBy; }
        }

        /// <summary>
        /// Gets the System.DateTime object that represents the date and time that this
        /// work item was created.
        /// </summary>
        public DateTime CreatedDate
        {
            get { return _item.CreatedDate; }
        }

        /// <summary>
        /// Closes this WorkItem instance and frees memory that is associated with it.
        /// </summary>
        public void Close()
        {
            _item.Close();
        }

        /// <summary>
        /// Gets or sets a string that describes this work item.
        /// </summary>
        public string Description
        {
            get { return _item.Description; }
            set { _item.Description = value; }
        }

        /// <summary>
        /// Gets the number of external links in this work item.
        /// </summary>
        public int ExternalLinkCount
        {
            get { return _item.ExternalLinkCount; }
        }

        public IEnumerable<IField> Fields
        {
            get { return _item.Fields.Cast<Tfs.Field>().Select(field => new FieldProxy(field)); }
        }

        /// <summary>
        /// Gets or sets the string value of the History field for this work item.
        /// </summary>
        public string History
        {
            get { return _item.History; }
            set { _item.History = value; }
        }

        /// <summary>
        /// Gets the number of hyperlinks in this work item.
        /// </summary>
        public int HyperLinkCount
        {
            get { return _item.HyperLinkCount; }
        }

        /// <summary>
        /// Gets the ID of this work item.
        /// </summary>
        public int Id
        {
            get { return _item.Id; }
        }

        /// <summary>
        /// Gets or sets the string value of the IterationPath field of this work item.
        /// </summary>
        public string IterationPath
        {
            get { return _item.IterationPath; }
            set { _item.IterationPath = value; }
        }

        /// <summary>
        /// Gets the collection of the links in this work item.
        /// </summary>
        /// <summary>
        /// Gets the links of the work item in this revision.
        /// </summary>
        public ICollection<ILink> Links
        {
            get { return new LinkCollectionProxy(_item); }
        }

        /// <summary>
        /// Gets the number of related links of this work item.
        /// </summary>
        public int RelatedLinkCount
        {
            get { return _item.RelatedLinkCount; }
        }

        /// <summary>
        /// Gets a System.DateTime object that represents the revision date and time
        /// of this work item.
        /// </summary>
        public DateTime RevisedDate
        {
            get { return _item.RevisedDate; }
        }

        /// <summary>
        /// Gets the integer that represents the revision number of this work item.
        /// </summary>
        public int Revision
        {
            get { return _item.Revision; }
        }

        /// <summary>
        /// Gets an object that represents a collection of valid revision numbers for this work
        /// item.
        /// </summary>
        public IEnumerable<IRevision> Revisions
        {
            get { return _item.Revisions.Cast<Tfs.Revision>().Select(r => new RevisionProxy(r)); }
        }

        /// <summary>
        /// Gets or sets a string that describes the state of this work item.
        /// </summary>
        public string State
        {
            get { return _item.State; }
            set { _item.State = value; }
        }

        public string Tags
        {
            get { return _item.Tags; }
            set { _item["Tags"] = value; }
        }

        public string Keywords
        {
            get { return (string)_item["Keywords"]; }
            set { _item["Keywords"] = value; }
        }

        /// <summary>
        /// Gets or sets a string that describes the title of this work item.
        /// </summary>
        public string Title
        {
            get { return _item.Title; }
            set { _item.Title = value; }
        }

        /// <summary>
        /// Gets a Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType object
        /// that represents the type of this work item.
        /// </summary>
        /// <exception cref="Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemTypeDeniedOrNotExistException">
        /// The Type property is null.
        /// </exception>
        public IWorkItemType Type
        {
            get { return new WorkItemTypeProxy(_item.Type); }
        }

        /// <summary>
        /// Gets the uniform resource identifier (System.Uri) of this work item.
        /// </summary>
        public Uri Uri
        {
            get { return _item.Uri; }
        }

        /// <summary>
        /// Gets a Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemLinkCollection
        /// object that represents a collection of the Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemLinks
        /// that link to this work item now or linked to it in the past.
        /// </summary>
        public IEnumerable<IWorkItemLink> WorkItemLinkHistory
        {
            get { return _item.WorkItemLinkHistory.Cast<Tfs.WorkItemLink>().Select(item => new WorkItemLinkProxy(item)); }
        }

        /// <summary>
        /// Gets a Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemLinkCollection
        /// object that represents a collection of the Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemLinks
        /// that currently link to this work item.
        /// </summary>
        public IEnumerable<IWorkItemLink> WorkItemLinks
        {
            get { return _item.WorkItemLinks.Cast<Tfs.WorkItemLink>().Select(item => new WorkItemLinkProxy(item)); }
        }

        public int Rev
        {
            get { return _item.Rev; }
        }

        public IRelatedLink CreateRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem relatedWorkItem)
        {
            var rawLinkTypeEnd = LinkTypeEndMapper.Map(_item.Store, linkTypeEnd);
            return new RelatedLinkProxy(new Tfs.RelatedLink(rawLinkTypeEnd, relatedWorkItem.Id));
        }

        public IHyperlink CreateHyperlink(string location)
        {
            return new HyperlinkProxy(new Tfs.Hyperlink(location));
        }

        public IWorkItemLink CreateWorkItemLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem targetWorkItem)
        {
            var rawLinkTypeEnd = LinkTypeEndMapper.Map(_item.Store, linkTypeEnd);
            return new WorkItemLinkProxy(new Tfs.WorkItemLink(rawLinkTypeEnd, _item.Id, targetWorkItem.Id));
        }

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
        public object this[string name]
        {
            get { return _item[name]; }
            set { _item[name] = value; }
        }

        /// <summary>
        /// Creates a copy of this WorkItem instance.
        /// </summary>
        /// <returns>A new WorkItem instance that is a copy of this WorkItem instance.</returns>
        public IWorkItem Copy()
        {
            return new WorkItemProxy(_item.Copy());
        }

        /// <summary>
        /// Creates a copy of this WorkItem instance that is of the specified Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType.
        /// </summary>
        /// <param name="targetType">The type of the target work item.</param>
        /// <returns>
        /// A new WorkItem instance of the specified Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType
        /// that is a copy of this WorkItem instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when targetType is null.
        /// </exception>
        public WorkItemProxy Copy(IWorkItemType targetType)
        {
            var type = GetWorkItemType(targetType);
            return new WorkItemProxy(_item.Copy(type));
        }

        private Tfs.WorkItemType GetWorkItemType(IWorkItemType type)
        {
            var workItemTypes = _item.Project.WorkItemTypes.Cast<Tfs.WorkItemType>();
            return workItemTypes.Single(item => item.Name == type.Name);
        }

        /// <summary>
        /// Creates a copy of this WorkItem instance that is of the specified Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType.
        /// </summary>
        /// <param name="targetType">The type of the target work item.</param>
        /// <param name="flags">Flags that specify items to copy in addition to fields.</param>
        /// <returns>
        /// A new WorkItem instance of the specified Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType
        /// that is a copy of this WorkItem instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when targetType is null.
        /// </exception>
        public WorkItemProxy Copy(IWorkItemType targetType, WorkItemCopyFlags flags)
        {
            var type = GetWorkItemType(targetType);

            return new WorkItemProxy(_item.Copy(type, (Tfs.WorkItemCopyFlags)flags));
        }

        /// <summary>
        /// Validates the fields of this work item.
        /// </summary>
        /// <returns>
        /// True if all fields are valid. False if at least one field is not valid.
        /// </returns>
        public bool IsValid()
        {
            return _item.IsValid();
        }

        public bool IsDirty
        {
            get { return _item.IsDirty; }
        }

        /// <summary>
        /// Opens this work item for modification.
        /// </summary>
        public void Open()
        {
            _item.Open();
        }

        /// <summary>
        /// Opens this work item for modification when transmitting minimal amounts of data over the network.
        /// </summary>
        /// <exception cref="Microsoft.TeamFoundation.WorkItemTracking.Client.ValidationException">
        /// This WorkItem instance does not belong to a Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemCollection.
        /// </exception>
        /// <exception cref="Microsoft.TeamFoundation.WorkItemTracking.Client.DeniedOrNotExistException">
        /// This WorkItem instance could not be opened for edit correctly.
        /// </exception>
        public void PartialOpen()
        {
            _item.PartialOpen();
        }

        /// <summary>
        /// Reverts all changes that were made since the last save.
        /// </summary>
        public void Reset()
        {
            _item.Reset();
        }

        /// <summary>
        /// Gets an ArrayList of fields in this work item that are not valid.
        /// </summary>
        /// <returns>
        /// An ArrayList of the fields in this work item that are not valid.
        /// </returns>
        public IEnumerable<IField> Validate()
        {
            return _item.Validate().Cast<Tfs.Field>().Select(field => new FieldProxy(field));
        }

        /// <summary>
        /// Saves any pending changes on this work item.
        /// </summary>
        public void Save()
        {
            _item.Save();
        }

        /// <summary>
        /// Saves any pending changes on this work item.
        /// </summary>
        /// <param name="saveFlags">
        /// If set to Microsoft.TeamFoundation.WorkItemTracking.Client.SaveFlags.MergeLinks,
        /// does not return errors if the link that is being added already exists or
        /// the link that is being removed was already removed.
        /// </param>
        public void Save(SaveFlags saveFlags)
        {
            try
            {
                _item.Save((Tfs.SaveFlags) saveFlags);
            }
            catch (Tfs.ItemAlreadyUpdatedOnServerException ex)
            {
                throw new ItemAlreadyUpdatedOnServerException(ex);
            }
            catch (Tfs.ServerRejectedChangesException)
            {
                throw new ServerRejectedChangesException();
            }
        }
    }
}
