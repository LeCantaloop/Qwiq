using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    /// <summary>
    /// A compatability class
    /// </summary>
    /// <seealso cref="Microsoft.Qwiq.WorkItemCommon" />
    /// <seealso cref="Microsoft.Qwiq.IWorkItem" />
    public abstract class WorkItem : WorkItemCommon, IWorkItem
    {
        private readonly IWorkItemType _type;

        protected WorkItem(IWorkItemType type)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }

        protected WorkItem()
        {
        }

        public virtual int Revision => Rev;

        public new virtual int AttachedFileCount => base.AttachedFileCount.GetValueOrDefault(0);

        public virtual IEnumerable<IAttachment> Attachments => throw new NotImplementedException();

        public new virtual DateTime ChangedDate => base.ChangedDate.GetValueOrDefault(DateTime.MinValue);

        public new virtual DateTime CreatedDate => base.CreatedDate.GetValueOrDefault(DateTime.MinValue);

        public new virtual int ExternalLinkCount => base.ExternalLinkCount.GetValueOrDefault(0);

        public virtual IFieldCollection Fields => throw new NotImplementedException();

        public new virtual int HyperLinkCount => base.HyperLinkCount.GetValueOrDefault(0);

        public new virtual int Id => base.Id.GetValueOrDefault(0);

        public virtual bool IsDirty => throw new NotImplementedException();

        public virtual string Keywords
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public virtual ICollection<ILink> Links => throw new NotImplementedException();

        public new virtual int RelatedLinkCount => base.RelatedLinkCount.GetValueOrDefault(0);

        public new virtual int Rev => base.Rev.GetValueOrDefault(0);

        public new virtual DateTime RevisedDate => base.RevisedDate.GetValueOrDefault(DateTime.MinValue);

        public virtual IEnumerable<IRevision> Revisions => throw new NotImplementedException();

        public virtual IWorkItemType Type => _type ?? throw new NotImplementedException();

        public abstract Uri Uri { get; }

        public virtual void ApplyRules(bool doNotUpdateChangedBy = false)
        {
            throw new NotImplementedException();
        }

        public virtual void Close()
        {
            throw new NotImplementedException();
        }

        public virtual IWorkItem Copy()
        {
            throw new NotImplementedException();
        }

        public virtual IHyperlink CreateHyperlink(string location)
        {
            throw new NotImplementedException();
        }

        public virtual IRelatedLink CreateRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem relatedWorkItem)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsValid()
        {
            throw new NotImplementedException();
        }

        public virtual void Open()
        {
            throw new NotImplementedException();
        }

        public virtual void PartialOpen()
        {
            throw new NotImplementedException();
        }

        public virtual void Reset()
        {
            throw new NotImplementedException();
        }

        public virtual void Save()
        {
            throw new NotImplementedException();
        }

        public virtual void Save(SaveFlags saveFlags)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<IField> Validate()
        {
            throw new NotImplementedException();
        }
    }
}