using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class WorkItem : Qwiq.WorkItem
    {
        private readonly Lazy<IFieldCollection> _fields;

        private readonly TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem _item;

        private readonly Lazy<ICollection<ILink>> _links;

        private readonly Lazy<IWorkItemType> _wit;

        internal WorkItem(
            TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem item,
            Lazy<IWorkItemType> wit,
            Func<string, IWorkItemLinkType> linkFunc)
            : base((Dictionary<string, object>)item.Fields)
        {
            _item = item;
            _wit = wit;
            Uri = new Uri(item.Url);
            Url = item.Url;
            _fields = new Lazy<IFieldCollection>(() => new FieldCollection(this, Type.FieldDefinitions, (r, d) => new Field(r, d)));
            _links = new Lazy<ICollection<ILink>>(() => new LinkCollection(item.Relations, linkFunc));
        }

        public override IFieldCollection Fields => _fields.Value;

        public override int Id => _item.Id.GetValueOrDefault(0);

        public override string Keywords
        {
            get => GetValue<string>(WorkItemFields.Keywords);
            set => SetValue(WorkItemFields.Keywords, value);
        }

        public override ICollection<ILink> Links => _links.Value;

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

        public override int Rev => _item.Rev.GetValueOrDefault(0);

        public override IWorkItemType Type => _wit.Value;

        public override Uri Uri { get; }

        public override string Url { get; }
    }
}