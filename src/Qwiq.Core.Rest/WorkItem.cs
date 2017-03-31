using System;

namespace Microsoft.Qwiq.Rest
{
    public class WorkItem : Qwiq.WorkItem
    {
        private readonly TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem _item;

        private readonly Lazy<IWorkItemType> _wit;

        internal WorkItem(TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem item, Lazy<IWorkItemType> wit)
        {
            _item = item;
            _wit = wit;
            Uri = new Uri(item.Url);
            Url = item.Url;
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

        protected override object GetValue(string field)
        {
            //if (!Type.FieldDefinitions.Contains(field))
            //{
            //    // To preserve OM compatability
            //    throw new FieldDefinitionNotExistException(
            //        $"TF26026: A field definition ID {field} in the work item type definition file does not exist. Add a definition for this field ID, or remove the reference to the field ID and try again.");
            //}

            return !_item.Fields.TryGetValue(field, out object val) ? null : val;
        }

        protected override void SetValue(string field, object value)
        {
            _item.Fields[field] = value;
        }
    }
}