using System;
using System.Collections.Generic;

using Microsoft.TeamFoundation.WorkItemTracking.Common.Constants;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemProxy : IWorkItem
    {
        private readonly WorkItem _item;

        private readonly Lazy<IWorkItemType> _wit;

        internal WorkItemProxy(WorkItem item, Lazy<IWorkItemType> wit)
        {
            _item = item;
            _wit = wit;
            Uri = new Uri(item.Url);
            
        }

        public string AreaPath
        {
            get => GetValue<string>(CoreFieldRefNames.AreaPath);
            set => SetValue(CoreFieldRefNames.AreaPath, value);
        }

        public string AssignedTo
        {
            get => GetValue<string>(CoreFieldRefNames.AssignedTo);
            set => SetValue(CoreFieldRefNames.AssignedTo, value);
        }

        public int AttachedFileCount => GetValue<int>(CoreFieldRefNames.AttachedFileCount);

        public IEnumerable<IAttachment> Attachments => throw new NotImplementedException();

        public string ChangedBy
        {
            get => GetValue<string>(CoreFieldRefNames.ChangedBy);
            set => SetValue(CoreFieldRefNames.ChangedBy, value);
        }

        public DateTime ChangedDate
        {
            get => (DateTime)GetValue(CoreFieldRefNames.ChangedDate);
            set => SetValue(CoreFieldRefNames.ChangedDate, value);
        }

        public string CreatedBy
        {
            get => GetValue<string>(CoreFieldRefNames.CreatedBy);
            set => SetValue(CoreFieldRefNames.CreatedBy, value);
        }

        public DateTime CreatedDate
        {
            get => (DateTime)GetValue(CoreFieldRefNames.CreatedDate);
            set => SetValue(CoreFieldRefNames.CreatedDate, value);
        }

        public string Description
        {
            get => GetValue<string>(CoreFieldRefNames.Description);
            set => SetValue(CoreFieldRefNames.Description, value);
        }

        public int ExternalLinkCount => GetValue<int>(CoreFieldRefNames.ExternalLinkCount);

        public IFieldCollection Fields => throw new NotImplementedException();

        public string History
        {
            get => GetValue(CoreFieldRefNames.History) as string ?? string.Empty;
            set => SetValue(CoreFieldRefNames.History, value);
        }

        public int HyperLinkCount => GetValue<int>(CoreFieldRefNames.HyperLinkCount);

        public int Id => _item.Id.GetValueOrDefault(0);

        public bool IsDirty => throw new NotImplementedException();

        public string IterationPath
        {
            get => GetValue<string>(CoreFieldRefNames.IterationPath);
            set => SetValue(CoreFieldRefNames.IterationPath, value);
        }

        public string Keywords
        {
            get => GetValue<string>(WorkItemFields.Keywords);
            set => SetValue(WorkItemFields.Keywords, value);
        }

        public ICollection<ILink> Links => throw new NotImplementedException();

        public int RelatedLinkCount => GetValue<int>(CoreFieldRefNames.RelatedLinkCount);

        public long Rev => GetValue<long>(CoreFieldRefNames.Rev);

        public DateTime RevisedDate => GetValue<DateTime>(CoreFieldRefNames.RevisedDate);

        public long Revision => Rev;

        public IEnumerable<IRevision> Revisions => throw new NotImplementedException();

        public string State
        {
            get => GetValue<string>(CoreFieldRefNames.State);
            set => SetValue(CoreFieldRefNames.State, value);
        }

        public string Tags
        {
            get => GetValue<string>(CoreFieldRefNames.Tags);
            set => SetValue(CoreFieldRefNames.Tags, value);
        }

        public string Title
        {
            get => GetValue<string>(CoreFieldRefNames.Title);
            set => SetValue(CoreFieldRefNames.Title, value);
        }

        public IWorkItemType Type => _wit.Value;

        public Uri Uri { get; }

        public object this[string name]
        {
            get => GetValue(name);
            set => SetValue(name, value);
        }

        public void ApplyRules(bool doNotUpdateChangedBy = false)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
        }

        public IWorkItem Copy()
        {
            throw new NotImplementedException();
        }

        public IHyperlink CreateHyperlink(string location)
        {
            throw new NotImplementedException();
        }

        public IRelatedLink CreateRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem relatedWorkItem)
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
        }

        public void PartialOpen()
        {
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Save(SaveFlags saveFlags)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IField> Validate()
        {
            throw new NotImplementedException();
        }

        private T GetValue<T>(string field)
        {
            return (T)GetValue(field);
        }

        private object GetValue(string field)
        {
            //if (!Type.FieldDefinitions.Contains(field))
            //{
            //    // To preserve OM compatability
            //    throw new FieldDefinitionNotExistException(
            //        $"TF26026: A field definition ID {field} in the work item type definition file does not exist. Add a definition for this field ID, or remove the reference to the field ID and try again.");
            //}

            return !_item.Fields.TryGetValue(field, out object val) ? null : val;
        }

        private void SetValue(string field, object value)
        {
            _item.Fields[field] = value;
        }
    }
}