using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Qwiq.Client.Rest
{
    internal class WorkItem : Qwiq.WorkItem
    {
        private readonly Func<string, IWorkItemLinkType> _linkFunc;

        private readonly TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem _item;



        [CanBeNull]
        private LinkCollection _links2;




        [CanBeNull]
        private IFieldCollection _fields2;

        public WorkItem(
            [NotNull] TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem item,
            [NotNull] IWorkItemType wit,
            Func<string, IWorkItemLinkType> linkFunc
            )
            : base(wit)
        {
            Contract.Requires(item != null);
            Contract.Requires(wit != null);
            _item = item ?? throw new ArgumentNullException(nameof(item));
            _linkFunc = linkFunc;





        }



        public override IFieldCollection Fields => _fields2 ?? (_fields2 = new FieldCollection(this, Type.FieldDefinitions, (r, d) => new Field(r, d)));

        public override int Id => _item.Id.GetValueOrDefault(0);

        public override string Keywords
        {
            get => GetValue<string>(WorkItemFields.Keywords);
            set => SetValue(WorkItemFields.Keywords, value);
        }

        public override ICollection<ILink> Links => _links2
                                                    ?? (_links2 = new LinkCollection((List<WorkItemRelation>)_item.Relations, _linkFunc));

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

        public override Uri Uri { get; }

        public override string Url { get; }

        protected override object GetValue(string name)
        {
            //return _item.Fields[name];
            _item.Fields.TryGetValue(name, out object value);
            return value;
        }

        protected override void SetValue(string name, object value)
        {
            _item.Fields[name] = value;
        }
    }
}