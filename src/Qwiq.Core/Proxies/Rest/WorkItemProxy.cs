using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Constants;

using WorkItem = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem;

namespace Microsoft.Qwiq.Proxies.Rest
{
    public class WorkItemProxy : IWorkItem
    {
        private readonly WorkItem _item;

        internal WorkItemProxy(WorkItem item)
        {
            _item = item;
            Type = new WorkItemTypeProxy(GetValue<string>(CoreFieldRefNames.WorkItemType));
            Uri = new Uri(item.Url);
        }

        public string AreaPath
        {
            get
            {
                return GetValue<string>(CoreFieldRefNames.AreaPath);
            }
            set
            {
                SetValue(CoreFieldRefNames.AreaPath, value);
            }
        }

        public string AssignedTo
        {
            get
            {
                return GetValue<string>(CoreFieldRefNames.AssignedTo);
            }
            set
            {
                SetValue(CoreFieldRefNames.AssignedTo, value);
            }
        }

        public int AttachedFileCount => GetValue<int>(CoreFieldRefNames.AttachedFileCount);

        public IEnumerable<IAttachment> Attachments { get { throw new NotImplementedException(); } }

        public string ChangedBy
        {
            get { return GetValue<string>(CoreFieldRefNames.ChangedBy); }
            set
            {
                SetValue(CoreFieldRefNames.ChangedBy, value);
            }
        }

        public DateTime ChangedDate
        {
            get { return (DateTime)GetValue(CoreFieldRefNames.ChangedDate); }
            set
            {
                SetValue(CoreFieldRefNames.ChangedDate, value);
            }
        }

        public string CreatedBy
        {
            get { return GetValue<string>(CoreFieldRefNames.CreatedBy); }
            set
            {
                SetValue(CoreFieldRefNames.CreatedBy, value);
            }
        }

        public DateTime CreatedDate
        {
            get { return (DateTime)GetValue(CoreFieldRefNames.CreatedDate); }
            set
            {
                SetValue(CoreFieldRefNames.CreatedDate, value);
            }
        }

        public string Description
        {
            get
            {
                return GetValue<string>(CoreFieldRefNames.Description);
            }
            set
            {
                SetValue(CoreFieldRefNames.Description, value);
            }
        }

        public int ExternalLinkCount => GetValue<int>(CoreFieldRefNames.ExternalLinkCount);

        public IFieldCollection Fields { get { throw new NotImplementedException(); } }

        public string History
        {
            get { return GetValue(CoreFieldRefNames.History) as string ?? string.Empty; }
            set
            {
                SetValue(CoreFieldRefNames.History, value);
            }
        }

        public int HyperLinkCount => GetValue<int>(CoreFieldRefNames.HyperLinkCount);

        public int Id => _item.Id.GetValueOrDefault(0);

        public bool IsDirty { get { throw new NotImplementedException(); } }

        public string IterationPath
        {
            get
            {
                return GetValue<string>(CoreFieldRefNames.IterationPath);
            }
            set
            {
                SetValue(CoreFieldRefNames.IterationPath, value);
            }
        }

        public string Keywords
        {
            get { return GetValue<string>(WorkItemFields.Keywords); }
            set { SetValue(WorkItemFields.Keywords, value); }
        }

        public ICollection<ILink> Links { get { throw new NotImplementedException(); } }

        public int RelatedLinkCount => GetValue<int>(CoreFieldRefNames.RelatedLinkCount);

        public long Rev => GetValue<long>(CoreFieldRefNames.Rev);

        public DateTime RevisedDate => GetValue<DateTime>(CoreFieldRefNames.RevisedDate);

        public long Revision => Rev;

        public IEnumerable<IRevision> Revisions { get { throw new NotImplementedException(); } }

        public string State
        {
            get { return GetValue<string>(CoreFieldRefNames.State); }
            set { SetValue(CoreFieldRefNames.State, value); }
        }

        public string Tags
        {
            get { return GetValue<string>(CoreFieldRefNames.Tags); }
            set { SetValue(CoreFieldRefNames.Tags, value); }
        }

        public string Title
        {
            get { return GetValue<string>(CoreFieldRefNames.Title); }
            set { SetValue(CoreFieldRefNames.Title, value); }
        }

        public IWorkItemType Type { get; }

        public Uri Uri { get; }

        public object this[string name]
        {
            get
            {
                return GetValue(name);
            }
            set
            {
                SetValue(name, value);
            }
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
            if (!_item.Fields.TryGetValue(field, out object val))
            {
                // To preserve OM compatability
                throw new FieldDefinitionNotExistException(
                    $"TF26026: A field definition ID {field} in the work item type definition file does not exist. Add a definition for this field ID, or remove the reference to the field ID and try again.");
            }
            return val;
        }

        private void SetValue(string field, object value)
        {
            _item.Fields[field] = value;
        }

        public void ApplyRules(bool doNotUpdateChangedBy = false)
        {
            throw new NotImplementedException();
        }
    }
}