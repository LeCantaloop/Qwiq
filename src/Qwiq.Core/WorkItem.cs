using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    /// <summary>
    ///     A compatability class
    /// </summary>
    /// <seealso cref="WorkItemCommon" />
    /// <seealso cref="IWorkItem" />
    public abstract class WorkItem : WorkItemCommon, IWorkItem, IEquatable<IWorkItem>
    {
        private readonly IWorkItemType _type;

        private bool _useFields = true;

        private readonly Lazy<IFieldCollection> _fields;

        protected internal WorkItem(IDictionary<string, object> fields)
            : base(fields)
        {
        }

        protected internal WorkItem(IWorkItemType type)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _fields = new Lazy<IFieldCollection>(()=> new FieldCollection(this, Type.FieldDefinitions, (revision, definition) => new Field(revision, definition)));
        }

        protected internal WorkItem(IWorkItemType type, Func<IFieldCollection> fieldCollectionFactory)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _fields = new Lazy<IFieldCollection>(fieldCollectionFactory);
        }

        public virtual int Revision => Rev;

        public bool Equals(IWorkItem other)
        {
            return WorkItemComparer.Instance.Equals(this, other);
        }

        public new virtual int AttachedFileCount => base.AttachedFileCount.GetValueOrDefault(0);

        public virtual IEnumerable<IAttachment> Attachments => throw new NotSupportedException();

        public new virtual DateTime ChangedDate => base.ChangedDate.GetValueOrDefault(DateTime.MinValue);

        public new virtual DateTime CreatedDate => base.CreatedDate.GetValueOrDefault(DateTime.MinValue);

        public new virtual int ExternalLinkCount => base.ExternalLinkCount.GetValueOrDefault(0);

        public virtual IFieldCollection Fields => _fields == null ? throw new NotSupportedException() : _fields.Value;

        public int Index => -2;


        public new virtual int HyperLinkCount => base.HyperLinkCount.GetValueOrDefault(0);

        public new virtual int Id => base.Id.GetValueOrDefault(0);

        public virtual bool IsDirty => throw new NotSupportedException();

        public virtual string Keywords
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public virtual ICollection<ILink> Links => throw new NotSupportedException();

        public new virtual int RelatedLinkCount => base.RelatedLinkCount.GetValueOrDefault(0);

        public new virtual int Rev => base.Rev.GetValueOrDefault(0);

        public new virtual DateTime RevisedDate => base.RevisedDate.GetValueOrDefault(DateTime.MinValue);

        public virtual IEnumerable<IRevision> Revisions => throw new NotSupportedException();

        public virtual IWorkItemType Type => _type ?? throw new NotSupportedException();

        public abstract Uri Uri { get; }

        public override object this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (_useFields)
                    try
                    {
                        return Fields[name].Value;
                    }
                    catch (NotSupportedException)
                    {
                        _useFields = false;
                    }

                return GetValue(name);
            }
            set
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (_useFields)
                    try
                    {
                        Fields[name].Value = value;
                    }
                    catch (NotSupportedException)
                    {
                        _useFields = false;
                    }
                SetValue(name, value);
            }
        }

        public string GetTagLine()
        {
            throw new NotSupportedException();
        }

        public virtual void ApplyRules(bool doNotUpdateChangedBy = false)
        {
            throw new NotSupportedException();
        }

        public virtual void Close()
        {
            throw new NotSupportedException();
        }

        public virtual IWorkItem Copy()
        {
            throw new NotSupportedException();
        }

        public virtual IHyperlink CreateHyperlink(string location)
        {
            throw new NotSupportedException();
        }

        public virtual IRelatedLink CreateRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem relatedWorkItem)
        {
            return CreateRelatedLink(relatedWorkItem.Id, linkTypeEnd);
        }

        public virtual IRelatedLink CreateRelatedLink(int relatedWorkItemId, IWorkItemLinkTypeEnd linkTypeEnd = null)
        {
            throw new NotSupportedException();
        }

        public virtual bool IsValid()
        {
            throw new NotSupportedException();
        }

        public virtual void Open()
        {
            throw new NotSupportedException();
        }

        public virtual void PartialOpen()
        {
            throw new NotSupportedException();
        }

        public virtual void Reset()
        {
            throw new NotSupportedException();
        }

        public virtual void Save()
        {
            throw new NotSupportedException();
        }

        public virtual void Save(SaveFlags saveFlags)
        {
            throw new NotSupportedException();
        }

        public virtual IEnumerable<IField> Validate()
        {
            throw new NotSupportedException();
        }

        public override bool Equals(object obj)
        {
            return WorkItemComparer.Instance.Equals(this, obj as IWorkItem);
        }

        public override int GetHashCode()
        {
            return WorkItemComparer.Instance.GetHashCode(this);
        }
    }
}