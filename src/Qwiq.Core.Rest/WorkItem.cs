using System;

namespace Microsoft.Qwiq.Rest
{
    internal class WorkItem : Qwiq.WorkItem
    {
        private readonly TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem _item;

        private readonly Lazy<IWorkItemType> _wit;

        private readonly Lazy<IFieldCollection> _fields;

        internal WorkItem(TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem item, Lazy<IWorkItemType> wit)
            :base(item.Fields)
        {
            _item = item;
            _wit = wit;
            Uri = new Uri(item.Url);
            Url = item.Url;
            _fields = new Lazy<IFieldCollection>(() => new FieldCollection(this, Type.FieldDefinitions, (r,d) => new Field(r,d)));
        }

        public override int Id => _item.Id.GetValueOrDefault(0);

        public override string Keywords
        {
            get => GetValue<string>(WorkItemFields.Keywords);
            set => SetValue(WorkItemFields.Keywords, value);
        }

        public override int Rev => _item.Rev.GetValueOrDefault(0);

        public override IWorkItemType Type => _wit.Value;

        public override Uri Uri { get; }

        public override string Url { get; }

        public override IFieldCollection Fields => _fields.Value;
    }
}