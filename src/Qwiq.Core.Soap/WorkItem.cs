using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    /// <summary>
    ///     Wrapper around the TFS WorkItem. This exists so that every agent doesn't need to reference
    ///     all the TFS libraries.
    /// </summary>
    public class WorkItem : Qwiq.WorkItem, IWorkItem
    {
        private readonly Tfs.WorkItem _item;

        internal WorkItem(Tfs.WorkItem item)
            : base(ExceptionHandlingDynamicProxyFactory.Create<IWorkItemType>(new WorkItemType(item.Type)))
        {
            _item = item;
            Url = item.Uri.ToString();
        }

        /// <summary>
        ///     Gets or sets the string value of the AreaPath field for this work item.
        /// </summary>
        public override string AreaPath
        {
            get => _item.AreaPath;
            set => _item.AreaPath = value;
        }

        /// <summary>
        ///     Gets the number of attached files for this work item.
        /// </summary>
        public override int AttachedFileCount => _item.AttachedFileCount;

        /// <summary>
        ///     Gets the Microsoft.TeamFoundation.WorkItemTracking.Client.AttachmentCollection
        ///     object that represents the attachments that belong to this work item.
        /// </summary>
        public override IEnumerable<IAttachment> Attachments
        {
            get
            {
                return _item.Attachments.Cast<Tfs.Attachment>()
                            .Select(
                                item => ExceptionHandlingDynamicProxyFactory
                                    .Create<IAttachment>(new Attachment(item)));
            }
        }



        /// <summary>
        ///     Gets the string value of the ChangedBy field for this work item.
        /// </summary>
        public override string ChangedBy => _item.ChangedBy;

        /// <summary>
        ///     Gets the System.DateTime object that represents the date and time that this
        ///     work item was last changed.
        /// </summary>
        public override DateTime ChangedDate => _item.ChangedDate;

        /// <summary>
        ///     Gets the string value of the CreatedBy field for this work item.
        /// </summary>
        public override string CreatedBy => _item.CreatedBy;

        /// <summary>
        ///     Gets the System.DateTime object that represents the date and time that this
        ///     work item was created.
        /// </summary>
        public override DateTime CreatedDate => _item.CreatedDate;

        /// <summary>
        ///     Gets or sets a string that describes this work item.
        /// </summary>
        public override string Description
        {
            get => _item.Description;
            set => _item.Description = value;
        }

        /// <summary>
        ///     Gets the number of external links in this work item.
        /// </summary>
        public override int ExternalLinkCount => _item.ExternalLinkCount;

        public override IFieldCollection Fields => ExceptionHandlingDynamicProxyFactory.Create<IFieldCollection>(
            new FieldCollection(_item.Fields));

        /// <summary>
        ///     Gets or sets the string value of the History field for this work item.
        /// </summary>
        public override string History
        {
            get => _item.History;
            set => _item.History = value;
        }

        /// <summary>
        ///     Gets the number of hyperlinks in this work item.
        /// </summary>
        public new int HyperLinkCount => _item.HyperLinkCount;

        /// <summary>
        ///     Gets the ID of this work item.
        /// </summary>
        public override int Id => _item.Id;

        public override bool IsDirty => _item.IsDirty;

        /// <summary>
        ///     Gets or sets the string value of the IterationPath field of this work item.
        /// </summary>
        public override string IterationPath
        {
            get => _item.IterationPath;
            set => _item.IterationPath = value;
        }

        public override string Keywords
        {
            get => (string)_item[WorkItemFields.Keywords];
            set => _item[WorkItemFields.Keywords] = value;
        }

        /// <summary>
        ///     Gets the collection of the links in this work item.
        /// </summary>
        /// <summary>
        ///     Gets the links of the work item in this revision.
        /// </summary>
        public override ICollection<ILink> Links => ExceptionHandlingDynamicProxyFactory.Create<ICollection<ILink>>(
            new LinkCollection(_item));

        /// <summary>
        ///     Gets the number of related links of this work item.
        /// </summary>
        public override int RelatedLinkCount => _item.RelatedLinkCount;

        public override int Rev => _item.Rev;

        /// <summary>
        ///     Gets a System.DateTime object that represents the revision date and time
        ///     of this work item.
        /// </summary>
        public override DateTime RevisedDate => _item.RevisedDate;

        /// <summary>
        ///     Gets the integer that represents the revision number of this work item.
        /// </summary>
        public override int Revision => _item.Revision;

        /// <summary>
        ///     Gets an object that represents a collection of valid revision numbers for this work
        ///     item.
        /// </summary>
        public override IEnumerable<IRevision> Revisions
        {
            get
            {
                return _item.Revisions.Cast<Tfs.Revision>()
                            .Select(r => ExceptionHandlingDynamicProxyFactory.Create<IRevision>(new Revision(r)));
            }
        }

        /// <summary>
        ///     Gets or sets a string that describes the state of this work item.
        /// </summary>
        public override string State
        {
            get => _item.State;
            set => _item.State = value;
        }

        

        public override string Tags
        {
            get => _item.Tags;
            set => _item.Tags = value;
        }

        /// <summary>
        ///     Gets or sets a string that describes the title of this work item.
        /// </summary>
        public override string Title
        {
            get => _item.Title;
            set => _item.Title = value;
        }

        /// <summary>
        ///     Gets the uniform resource identifier (System.Uri) of this work item.
        /// </summary>
        public override Uri Uri => _item.Uri;

        public override string Url { get; }

        public override void ApplyRules(bool doNotUpdateChangedBy = false)
        {
            _item.ApplyRules(doNotUpdateChangedBy);
        }

        /// <summary>
        ///     Closes this WorkItem instance and frees memory that is associated with it.
        /// </summary>
        public override void Close()
        {
            _item.Close();
        }

        /// <summary>
        ///     Creates a copy of this WorkItem instance.
        /// </summary>
        /// <returns>A new WorkItem instance that is a copy of this WorkItem instance.</returns>
        public override IWorkItem Copy()
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItem(_item.Copy()));
        }

        public override IHyperlink CreateHyperlink(string location)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IHyperlink>(new Hyperlink(new Tfs.Hyperlink(location)));
        }

        public override IRelatedLink CreateRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem relatedWorkItem)
        {
            var rawLinkTypeEnd = LinkTypeEndMapper.Map(_item.Store, linkTypeEnd);
            return ExceptionHandlingDynamicProxyFactory.Create<IRelatedLink>(
                new RelatedLink(new Tfs.RelatedLink(rawLinkTypeEnd, relatedWorkItem.Id)));
        }

        /// <summary>
        ///     Validates the fields of this work item.
        /// </summary>
        /// <returns>
        ///     True if all fields are valid. False if at least one field is not valid.
        /// </returns>
        public override bool IsValid()
        {
            return _item.IsValid();
        }

        /// <summary>
        ///     Opens this work item for modification.
        /// </summary>
        public override void Open()
        {
            _item.Open();
        }

        /// <summary>
        ///     Opens this work item for modification when transmitting minimal amounts of data over the network.
        /// </summary>
        /// <exception cref="Microsoft.TeamFoundation.WorkItemTracking.Client.ValidationException">
        ///     This WorkItem instance does not belong to a Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemCollection.
        /// </exception>
        /// <exception cref="Microsoft.TeamFoundation.WorkItemTracking.Client.DeniedOrNotExistException">
        ///     This WorkItem instance could not be opened for edit correctly.
        /// </exception>
        public override void PartialOpen()
        {
            _item.PartialOpen();
        }

        /// <summary>
        ///     Reverts all changes that were made since the last save.
        /// </summary>
        public override void Reset()
        {
            _item.Reset();
        }

        /// <summary>
        ///     Saves any pending changes on this work item.
        /// </summary>
        public override void Save()
        {
            _item.Save();
        }

        /// <summary>
        ///     Saves any pending changes on this work item.
        /// </summary>
        /// <param name="saveFlags">
        ///     If set to Microsoft.TeamFoundation.WorkItemTracking.Client.SaveFlags.MergeLinks,
        ///     does not return errors if the link that is being added already exists or
        ///     the link that is being removed was already removed.
        /// </param>
        public override void Save(SaveFlags saveFlags)
        {
            try
            {
                _item.Save((Tfs.SaveFlags)saveFlags);
            }
            catch (Tfs.ItemAlreadyUpdatedOnServerException ex)
            {
                throw new ItemAlreadyUpdatedOnServerException(ex);
            }
            catch (Tfs.ServerRejectedChangesException ex)
            {
                throw new ServerRejectedChangesException(ex);
            }
        }

        /// <summary>
        ///     Gets an ArrayList of fields in this work item that are not valid.
        /// </summary>
        /// <returns>
        ///     An ArrayList of the fields in this work item that are not valid.
        /// </returns>
        public override IEnumerable<IField> Validate()
        {
            return _item.Validate()
                        .Cast<Tfs.Field>()
                        .Select(field => ExceptionHandlingDynamicProxyFactory.Create<IField>(new Field(field)));
        }

        /// <summary>
        ///     Creates a copy of this WorkItem instance that is of the specified
        ///     Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType.
        /// </summary>
        /// <param name="targetType">The type of the target work item.</param>
        /// <returns>
        ///     A new WorkItem instance of the specified Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType
        ///     that is a copy of this WorkItem instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when targetType is null.
        /// </exception>
        public IWorkItem Copy(IWorkItemType targetType)
        {
            var type = GetWorkItemType(targetType);
            return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(new WorkItem(_item.Copy(type)));
        }

        /// <summary>
        ///     Creates a copy of this WorkItem instance that is of the specified
        ///     Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType.
        /// </summary>
        /// <param name="targetType">The type of the target work item.</param>
        /// <param name="flags">Flags that specify items to copy in addition to fields.</param>
        /// <returns>
        ///     A new WorkItem instance of the specified Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItemType
        ///     that is a copy of this WorkItem instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when targetType is null.
        /// </exception>
        public IWorkItem Copy(IWorkItemType targetType, WorkItemCopyFlags flags)
        {
            var type = GetWorkItemType(targetType);

            return ExceptionHandlingDynamicProxyFactory.Create<IWorkItem>(
                new WorkItem(_item.Copy(type, (Tfs.WorkItemCopyFlags)flags)));
        }

        protected override object GetValue(string name)
        {
            return _item[name];
        }

        protected override void SetValue(string name, object value)
        {
            _item[name] = value;
        }

        private Tfs.WorkItemType GetWorkItemType(IWorkItemType type)
        {
            return _item.Project.WorkItemTypes[type.Name];
        }
    }
}