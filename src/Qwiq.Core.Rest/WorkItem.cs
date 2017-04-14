using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq.Rest
{
    internal class WorkItem : Qwiq.WorkItem
    {
        private readonly Lazy<IFieldCollection> _fields;

        private readonly TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem _item;

        private readonly Lazy<ICollection<ILink>> _links;

        private readonly Lazy<IWorkItemType> _wit;

        internal WorkItem(TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem item, Lazy<IWorkItemType> wit, Func<string, IWorkItemLinkType> linkFunc)
            : base(item.Fields)
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

        public override int Rev => _item.Rev.GetValueOrDefault(0);

        public override IWorkItemType Type => _wit.Value;

        public override Uri Uri { get; }

        public override string Url { get; }
    }
}