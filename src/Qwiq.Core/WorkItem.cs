using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    /// <summary>
    ///     A compatability class
    /// </summary>
    /// <seealso cref="WorkItemCommon" />
    /// ///
    /// <seealso cref="IWorkItem" />
    public abstract class WorkItem : WorkItemCommon, IWorkItem, IRevisionInternal, IEquatable<IWorkItem>
    {
        [CanBeNull]
        private readonly Lazy<IWorkItemType> _lazyType;

        [CanBeNull]
        private readonly IWorkItemType _type;

        [CanBeNull]
        private Func<IFieldCollection> _fieldFactory;

        private IFieldCollection _fields;

        private bool _useFields = true;

        protected internal WorkItem([NotNull] IWorkItemType workItemType, [CanBeNull] Dictionary<string, object> fields)
            : base(fields)
        {
            Contract.Requires(workItemType != null);

            _type = workItemType ?? throw new ArgumentNullException(nameof(workItemType));
        }

        protected internal WorkItem([NotNull] IWorkItemType workItemType)
        {
            Contract.Requires(workItemType != null);

            _type = workItemType ?? throw new ArgumentNullException(nameof(workItemType));
        }

        protected internal WorkItem([NotNull] Lazy<IWorkItemType> type)
        {
            Contract.Requires(type != null);
            _lazyType = type;
        }

        protected internal WorkItem([NotNull] IWorkItemType workItemType, [NotNull] Func<IFieldCollection> fieldCollectionFactory)
        {
            Contract.Requires(workItemType != null);
            Contract.Requires(fieldCollectionFactory != null);
            _type = workItemType ?? throw new ArgumentNullException(nameof(workItemType));
            _fieldFactory = fieldCollectionFactory ?? throw new ArgumentNullException(nameof(fieldCollectionFactory));
        }

        public new virtual int AttachedFileCount => base.AttachedFileCount.GetValueOrDefault(0);

        public virtual IEnumerable<IAttachment> Attachments => throw new NotSupportedException();

        public new virtual DateTime ChangedDate => base.ChangedDate.GetValueOrDefault(DateTime.MinValue);

        public new virtual DateTime CreatedDate => base.CreatedDate.GetValueOrDefault(DateTime.MinValue);

        public new virtual int ExternalLinkCount => base.ExternalLinkCount.GetValueOrDefault(0);

        public virtual IFieldCollection Fields
        {
            get
            {
                if (_fields != null) return _fields;

                if (_fieldFactory != null)
                {
                    _fields = _fieldFactory();
                    _fieldFactory = null;
                }
                else
                {
                    _fields = new FieldCollection(this, Type.FieldDefinitions, (revision, definition) => new Field(revision, definition));
                }

                return _fields;
            }
        }

        public new virtual int HyperlinkCount => base.HyperlinkCount.GetValueOrDefault(0);

        public new virtual int Id => base.Id.GetValueOrDefault(0);

        public virtual bool IsDirty => throw new NotSupportedException();

        public virtual ICollection<ILink> Links => throw new NotSupportedException();

        public new virtual int RelatedLinkCount => base.RelatedLinkCount.GetValueOrDefault(0);

        public new virtual int Rev => base.Rev.GetValueOrDefault(0);

        public new virtual DateTime RevisedDate => base.RevisedDate.GetValueOrDefault(DateTime.MinValue);

        public virtual int Revision => Rev;

        public virtual IEnumerable<IRevision> Revisions => throw new NotSupportedException();

        public virtual IWorkItemType Type => _type ?? _lazyType?.Value ?? throw new InvalidOperationException($"No value specified for {nameof(Type)}.");

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
                    catch (InvalidOperationException ioex) when (ioex.Source == "Microsoft.Qwiq.Client.Rest")
                    {
                        _useFields = false;
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

        public virtual IRelatedLink CreateRelatedLink(int relatedWorkItemId, IWorkItemLinkTypeEnd linkTypeEnd = null)
        {
            throw new NotSupportedException();
        }

        public bool Equals(IWorkItem other)
        {
            return WorkItemComparer.Default.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return WorkItemComparer.Default.Equals(this, obj as IWorkItem);
        }

        public override int GetHashCode()
        {
            return WorkItemComparer.Default.GetHashCode(this);
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

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{WorkItemType} {Id} {Title}";
        }

        public virtual IEnumerable<IField> Validate()
        {
            throw new NotSupportedException();
        }
    }
}