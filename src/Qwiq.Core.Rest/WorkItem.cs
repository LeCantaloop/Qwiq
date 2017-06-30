using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class WorkItem : Qwiq.WorkItem
    {
        [NotNull]
        private readonly TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem _item;

        [NotNull]
        private readonly Func<string, IWorkItemLinkType> _linkFunc;

        [CanBeNull]
        private IFieldCollection _fields;

        [CanBeNull]
        private LinkCollection _links;

        private Uri _uri;

        public WorkItem(
            [NotNull] TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem item,
            [NotNull] IWorkItemType wit,
            [NotNull] Func<string, IWorkItemLinkType> linkFunc)
            : base(wit)
        {
            Contract.Requires(item != null);
            Contract.Requires(wit != null);
            Contract.Requires(linkFunc != null);
            _item = item ?? throw new ArgumentNullException(nameof(item));
            _linkFunc = linkFunc ?? throw new ArgumentNullException(nameof(linkFunc));
            Url = _item.Url;
            _uri = new Uri(_item.Url, UriKind.Absolute);
        }

        public WorkItem(
            TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem item,
            Lazy<IWorkItemType> wit,
            Func<string, IWorkItemLinkType> linkFunc)
            : base(wit)
        {
            Contract.Requires(item != null);
            Contract.Requires(wit != null);
            Contract.Requires(linkFunc != null);
            _item = item ?? throw new ArgumentNullException(nameof(item));
            _linkFunc = linkFunc ?? throw new ArgumentNullException(nameof(linkFunc));
            Url = _item.Url;
        }

        public override int AttachedFileCount
        {
            get
            {
                var fv = GetValue<int?>(CoreFieldRefNames.AttachedFileCount);
                if (!fv.HasValue)
                {
                    var cnt = Attachments.Count();
                    SetValue(CoreFieldRefNames.AttachedFileCount, cnt);
                    fv = cnt;
                }
                return fv.GetValueOrDefault();
            }
        }

        public override int ExternalLinkCount
        {
            get
            {
                var fv = GetValue<int?>(CoreFieldRefNames.ExternalLinkCount);
                if (!fv.HasValue)
                {
                    var cnt = Links.Count(p => p.BaseType == BaseLinkType.ExternalLink);
                    SetValue(CoreFieldRefNames.ExternalLinkCount, cnt);
                    fv = cnt;
                }
                return fv.GetValueOrDefault();
            }
        }

        public override IFieldCollection Fields => _fields
                                                   ?? (_fields = new FieldCollection(
                                                                                      this,
                                                                                      Type.FieldDefinitions,
                                                                                      (r, d) => new Field(r, d)));

        public override int HyperlinkCount
        {
            get
            {
                var fv = GetValue<int?>(CoreFieldRefNames.HyperlinkCount);
                if (!fv.HasValue)
                {
                    var cnt = Links.Count(p => p.BaseType == BaseLinkType.Hyperlink);
                    SetValue(CoreFieldRefNames.HyperlinkCount, cnt);
                    fv = cnt;
                }
                return fv.GetValueOrDefault();
            }
        }

        public override int Id => _item.Id.GetValueOrDefault(0);

        public override string Keywords
        {
            get => GetValue<string>(WorkItemFields.Keywords);
            set => SetValue(WorkItemFields.Keywords, value);
        }

        public override ICollection<ILink> Links => _links
                                                    ?? (_links = new LinkCollection((List<WorkItemRelation>)_item.Relations, _linkFunc));

        /// <inheritdoc />
        public override int RelatedLinkCount
        {
            get
            {
                var fv = GetValue<int?>(CoreFieldRefNames.ExternalLinkCount);
                if (!fv.HasValue)
                {
                    var cnt = Links.Count(p => p.BaseType == BaseLinkType.RelatedLink);
                    SetValue(CoreFieldRefNames.ExternalLinkCount, cnt);
                    fv = cnt;
                }
                return fv.GetValueOrDefault();
            }
        }

        public override int Rev => _item.Rev.GetValueOrDefault(0);

        public override Uri Uri => _uri ?? (_uri = new Uri(_item.Url, UriKind.Absolute));

        public override string Url { get; }

        protected override object GetValue(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            _item.Fields.TryGetValue(name, out object value);
            return value;
        }

        protected override void SetValue(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) return;

            _item.Fields[name] = value;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_item != null);
        }
    }
}