using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Mocks
{
    [Serializable]
    public class MockWorkItem : IWorkItem
    {
        internal bool PartialOpenWasCalled = false;
        private readonly ICollection<ILink> _links;


        private IWorkItemType _type;

        private IEnumerable<IRevision> _revisions;

        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See a constructor that takes IWorkItemType and fields.")]
        public MockWorkItem()
            : this("Mock")
        {
        }

        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See a constructor that takes IWorkItemType and fields.")]
        public MockWorkItem(string workItemType = null)
            : this(workItemType, null)
        {
        }

        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See a constructor that takes IWorkItemType and fields.")]
        public MockWorkItem(IDictionary<string, object> fields)
            : this((string)null, fields)
        {
        }

        /// <summary>
        /// Creates a new work item type of the specified name and new field definitions for that type based on the supplied fields.
        /// </summary>
        /// <param name="workItemType"></param>
        /// <param name="fields"></param>
        public MockWorkItem(string workItemType = null, IDictionary<string, object> fields = null)
            : this(
                  new MockWorkItemType(
                      workItemType ?? "Mock",
                      CoreFieldDefinitions.All.Union(fields?.Select(p => MockFieldDefinition.Create(p.Key)) ?? Enumerable.Empty<IFieldDefinition>())),
                  fields)
        {
        }

        public MockWorkItem(IWorkItemType type, IDictionary<string, object> fields = null)
            : this(type, fields?.Select(p => new MockField(type.FieldDefinitions[p.Key], p.Value, p.Value)) ?? Enumerable.Empty<IField>())
        {
        }

        public MockWorkItem(IWorkItemType type, IEnumerable<IField> fields)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            Type = type ?? throw new ArgumentNullException(nameof(type));

            foreach (var field in fields)
            {
                var f = Fields[field.ReferenceName ?? field.Name];
                f.Value = field.Value;
                f.OriginalValue = field.OriginalValue;
            }

            _links = new MockLinkCollection();
            Revisions = Enumerable.Empty<IRevision>();
            ApplyRules(false);
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

        public int AttachedFileCount
        {
            get => (int)GetValue(CoreFieldRefNames.AttachedFileCount);
            set => SetValue(CoreFieldRefNames.AttachedFileCount, value);
        }

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

        public int ExternalLinkCount
        {
            get => (int)GetValue(CoreFieldRefNames.ExternalLinkCount);
            set => SetValue(CoreFieldRefNames.ExternalLinkCount, value);
        }

        private IFieldCollection _fields;

        public IFieldCollection Fields => _fields ?? (_fields = new MockFieldCollection(Type.FieldDefinitions));

        public string History
        {
            get => GetValue(CoreFieldRefNames.History) as string ?? string.Empty;
            set => SetValue(CoreFieldRefNames.History, value);
        }

        public int HyperLinkCount
        {
            get => GetValue<int>(CoreFieldRefNames.HyperLinkCount);
            set => SetValue(CoreFieldRefNames.HyperLinkCount, value);
        }

        public int Id
        {
            get => ((int?)GetValue(CoreFieldRefNames.Id)).GetValueOrDefault(0);
            set => SetValue(CoreFieldRefNames.Id, value);
        }

        public bool IsDirty
        {
            get { return Fields.Any(p => p.IsDirty); }
        }

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

        public ICollection<ILink> Links { get; set; }

        public int RelatedLinkCount => Links.OfType<IRelatedLink>().Count();

        public string ReproSteps
        {
            get => GetValue<string>("Repro Steps");
            set
            {
                SetValue("Repro Steps", value);
                SetValue("Microsoft.VSTS.TCM.ReproSteps", value);
            }
        }

        public long Rev
        {
            get => GetValue<long>(CoreFieldRefNames.Rev);
            set => SetValue(CoreFieldRefNames.Rev, value);
        }

        public DateTime RevisedDate
        {
            get => GetValue<DateTime>(CoreFieldRefNames.RevisedDate);
            set => SetValue(CoreFieldRefNames.RevisedDate, value);
        }

        public long Revision
        {
            get => (long)GetValue("Revision");
            set => SetValue("Revision", value);
        }

        public IEnumerable<IRevision> Revisions
        {
            get => _revisions;
            set => _revisions = value;
        }

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

        public IWorkItemType Type
        {
            get => _type;
            set
            {
                _type = value;
                SetValue(CoreFieldRefNames.WorkItemType, value.Name);
            }
        }

        public Uri Uri { get; set; }

        public object this[string name]
        {
            get => GetValue(name);
            set => SetValue(name, value);
        }

        public void Close()
        {
        }

        public IWorkItem Copy()
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0;

                var newItem = (MockWorkItem)formatter.Deserialize(stream);
                newItem.Id = 0;

                var link = newItem.CreateRelatedLink(this);
                newItem.Links.Add(link);

                return newItem;
            }
        }

        public IHyperlink CreateHyperlink(string location)
        {
            throw new NotImplementedException();
        }

        public IRelatedLink CreateRelatedLink(IWorkItem target)
        {
            return CreateRelatedLink(new MockWorkItemStore().WorkItemLinkTypes.Single(s => s.ReferenceName == CoreLinkTypeReferenceNames.Related).ForwardEnd, target);
        }

        public IRelatedLink CreateRelatedLink(IWorkItemLinkTypeEnd linkTypeEnd, IWorkItem relatedWorkItem)
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            return Validate() == null;
        }

        public void Open()
        {
        }

        public void PartialOpen()
        {
            PartialOpenWasCalled = true;
        }

        public void Reset()
        {
        }

        public void Save()
        {
        }

        public void Save(SaveFlags flags)
        {
        }

        public IEnumerable<IField> Validate()
        {
            var invalidFields = Fields.Where(p => !p.IsValid).Select(p => p).ToArray();
            return invalidFields.Any()
                ? invalidFields
                : null;
        }

        private T GetValue<T>(string field)
        {
            return (T)GetValue(field);
        }

        private object GetValue(string field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            return Fields[field].Value;
        }

        private void SetValue(string field, object value)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            Fields[field].Value = value;
        }

        public void ApplyRules(bool doNotUpdateChangedBy = false)
        {
        }
    }
}
