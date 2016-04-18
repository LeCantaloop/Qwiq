using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.Qwiq.Proxies;

namespace Microsoft.IE.Qwiq.Mapper.Tests.Mocks
{
    public class MockWorkItem : IWorkItem
    {
        public Dictionary<string, object> Properties { get; set; }

        public string AssignedTo
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string AreaPath
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int AttachedFileCount
        {
            get { throw new NotImplementedException(); }
        }

        public string ChangedBy
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime ChangedDate
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public IWorkItem Copy()
        {
            throw new NotImplementedException();
        }

        public string CreatedBy
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime CreatedDate
        {
            get { throw new NotImplementedException(); }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int ExternalLinkCount
        {
            get { throw new NotImplementedException(); }
        }

        public IFieldCollection Fields
        {
            get { return new MockFieldCollection(Properties.Select(p => new MockField {Name = p.Key, Value = p.Value}).Cast<IField>().ToList()); }
        }

        public string History
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int HyperLinkCount
        {
            get { throw new NotImplementedException(); }
        }

        public int Id { get; set; }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public bool IsDirty
        {
            get { throw new NotImplementedException(); }
        }

        public string IterationPath
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void PartialOpen()
        {
            throw new NotImplementedException();
        }

        public int RelatedLinkCount
        {
            get { throw new NotImplementedException(); }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public DateTime RevisedDate
        {
            get { throw new NotImplementedException(); }
        }

        public int Revision
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IRevision> Revisions
        {
            get { throw new NotImplementedException(); }
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Save(SaveFlags saveFlags)
        {
            throw new NotImplementedException();
        }

        public string State
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Tags
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Keywords
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public object this[string name]
        {
            get { return Properties[name]; }
            set { Properties[name] = value; }
        }

        public string Title
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Uri Uri
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IField> Validate()
        {
            throw new NotImplementedException();
        }

        public ICollection<ILink> Links { get; set; }

        public IEnumerable<IAttachment> Attachments
        {
            get { throw new NotImplementedException(); }
        }

        public IWorkItemType Type { get; set; }

        public int Rev
        {
            get { throw new NotImplementedException(); }
        }

        public IRelatedLink CreateRelatedLink(IWorkItemLinkTypeEnd end, IWorkItem relatedWorkItem)
        {
            throw new NotImplementedException();
        }

        public IHyperlink CreateHyperlink(string location)
        {
            throw new NotImplementedException();
        }
    }
}
