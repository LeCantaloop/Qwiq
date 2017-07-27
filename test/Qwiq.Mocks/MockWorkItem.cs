using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

using JetBrains.Annotations;

namespace Microsoft.Qwiq.Mocks
{
    [Serializable]
    public class MockWorkItem : WorkItem, IWorkItem
    {
        private static int tempId = 0;

        private IFieldCollection _fields;

        internal bool PartialOpenWasCalled;

        private IEnumerable<IRevision> _revisions;

        private int _tempId;

        public MockWorkItem([CanBeNull] string workItemType, [CanBeNull] params IField[] fields)
            : this(new MockWorkItemType(workItemType ?? "Mock", CoreFieldDefinitions.All.Union(fields.Select(f=>f.FieldDefinition))))
        {
            if (fields == null) return;

            if (Fields is MockFieldCollection c)
            {
                foreach (var field in fields)
                {
                    c.SetField(field);
                    if (field is MockField f)
                    {
                        var val = field.Value;
                        f.Revision = this;
                        SetFieldValue(field.FieldDefinition, val);
                    }
                }
            }
        }

        public MockWorkItem([NotNull] IWorkItemType workItemType, int id)
            : this(workItemType, new KeyValuePair<string, object>(CoreFieldRefNames.Id, id))
        {
            Contract.Requires(id > 0);
        }

        public MockWorkItem([NotNull] IWorkItemType workItemType, int id, [CanBeNull] params KeyValuePair<string, object>[] fieldValues)
            : this(
                   workItemType,
                   fieldValues?.Union(new[] { new KeyValuePair<string, object>(CoreFieldRefNames.Id, id) })
                              .ToDictionary(k => k.Key, e => e.Value, StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { CoreFieldRefNames.Id, id } })
        {
            Contract.Requires(id > 0);
        }

        public MockWorkItem([NotNull] IWorkItemType workItemType, [CanBeNull] params KeyValuePair<string, object>[] fieldValues)
            : this(workItemType, fieldValues?.ToDictionary(k => k.Key, e => e.Value, StringComparer.OrdinalIgnoreCase))
        {
        }

        public MockWorkItem([NotNull] IWorkItemType workItemType, [CanBeNull] Dictionary<string, object> fields = null)
            : base(workItemType, NormalizeFields(workItemType, fields))
        {
            SetFieldValue(workItemType.FieldDefinitions[CoreFieldRefNames.WorkItemType], workItemType.Name);
            SetFieldValue(workItemType.FieldDefinitions[CoreFieldRefNames.RevisedDate], new DateTime(9999, 1, 1, 0, 0, 0));

            if (IsNew)
            {
                _tempId = Interlocked.Decrement(ref tempId);
            }

            Links = new HashSet<ILink>();
            Revisions = new HashSet<IRevision>();
            ApplyRules();
        }

        private static Dictionary<string, object> NormalizeFields(IWorkItemType type, Dictionary<string, object> fields)
        {
            if (fields == null) return null;
            var retval = new Dictionary<string, object>(fields.Comparer);

            foreach (var field in fields)
            {
                var fieldDef = type.FieldDefinitions[field.Key];
                retval.Add(fieldDef.ReferenceName, field.Value);

            }

            return retval;
        }

        public override IRelatedLink CreateRelatedLink(int id, IWorkItemLinkTypeEnd linkTypeEnd = null)
        {
            if (IsNew) throw new InvalidOperationException("Save first");
            if (id != 0
                && linkTypeEnd == null)
            {
                throw new ArgumentException($"Value cannot be zero when no {nameof(IWorkItemLinkTypeEnd)} specified.", nameof(id));
            }

            if (id == 0 && linkTypeEnd == null) return new MockRelatedLink(null, Id);

            return new MockRelatedLink(linkTypeEnd, Id, id);
        }

        public string ReproSteps
        {
            get => GetValue<string>("Repro Steps");
            set
            {
                SetValue("Repro Steps", value);
                SetValue("Microsoft.VSTS.TCM.ReproSteps", value);
            }
        }

        public new DateTime? ChangedDate
        {
            get => base.ChangedDate;
            set => this[CoreFieldRefNames.ChangedDate] = value;
        }

        public sealed override IFieldCollection Fields => _fields
                                                          ?? (_fields = new MockFieldCollection(this, Type.FieldDefinitions));

        public new int Id
        {
            get => GetValue<int>(CoreFieldRefNames.Id);
            set => SetValue(CoreFieldRefNames.Id, value);
        }

        public override bool IsDirty
        {
            get
            {
                return Fields.Any(p => p.IsDirty);
            }
        }

        public override string Keywords
        {
            get => GetValue<string>(WorkItemFields.Keywords);
            set => SetValue(WorkItemFields.Keywords, value);
        }

        public new ICollection<ILink> Links { get; set; }

        public new int RelatedLinkCount => Links.OfType<IRelatedLink>().Count();

        IEnumerable<IRevision> IWorkItem.Revisions => Revisions;

        public new IEnumerable<IRevision> Revisions
        {
            get => _revisions;
            set
            {
                _revisions = value;
                if (_revisions != null)
                {
                    SetFieldValue(Type.FieldDefinitions[CoreFieldRefNames.Rev], _revisions.Count() + 1);
                }
            }
        }

        public override string Url => $"vstfs:///WorkItemTracking/WorkItem/{Id}";

        public override void ApplyRules(bool doNotUpdateChangedBy = false)
        {
        }

        public override void Close()
        {
        }

        public override IWorkItem Copy()
        {
            // Copy links
            var target = new MockWorkItem(Type);
            foreach (var definition in Type.FieldDefinitions)
            {
                // Verify field is clonable
                if (definition.IsCloneable())
                {
                    Fields.TryGetById(definition.Id, out IField field);
                    if (field != null && field.Value != null && !Equals(field.Value, string.Empty))
                    {
                        var obj2 = field.Value;
                        target.SetFieldValue(definition, obj2);
                    }
                }
            }
            target.History = $"Copied from Work Item {Id}";

            // Copy links
            IEnumerator enumerator = Links.GetEnumerator();

            while (enumerator.MoveNext())
            {
                //TODO: Clone link
            }

            if (!IsNew)
            {
                var s = Type.Store();
                IWorkItemLinkType e;
                if (s != null)
                {
                    e = s.WorkItemLinkTypes[CoreLinkTypeReferenceNames.Related];
                }
                else
                {
                    using (var wis = new MockWorkItemStore())
                    {
                        e = wis.WorkItemLinkTypes[CoreLinkTypeReferenceNames.Related];
                    }
                }
                // Our limitation: need an ID to link so save before we do anything else
                target.Id = target._tempId;
                target.Links.Add(target.CreateRelatedLink(Id, e.ForwardEnd));

                // Recipricol links are typically handled in Save operation
                Links.Add(CreateRelatedLink(target.Id, e.ReverseEnd));

                target.Id = 0;
            }


            target.ApplyRules();
            return target;

        }

        public override bool IsValid()
        {
            return Validate() == null;
        }

        public override void Open()
        {
        }

        public override void PartialOpen()
        {
            PartialOpenWasCalled = true;
            ApplyRules(false);
        }

        public override void Reset()
        {
        }

        public override void Save()
        {
            Save(SaveFlags.None);
        }

        public bool IsNew => Id == 0;

        public override void Save(SaveFlags flags)
        {
            if (IsDirty || IsNew)
            {
                if (!IsValid())
                {
                    throw new Exception("Work item is not ready to save.");
                }

                if (!(Type is MockWorkItemType))
                {
                    throw new NotSupportedException();
                }

                var t = (MockWorkItemType)Type;
                if (!(t.Store is MockWorkItemStore))
                {
                    throw new NotSupportedException();
                }

                var s = (MockWorkItemStore)t.Store;

                s.BatchSave(new[] { this });
            }
        }

        public override IEnumerable<IField> Validate()
        {
            var invalidFields = Fields.Where(p => !p.IsValid).Select(p => p).ToArray();
            return invalidFields.Any() ? invalidFields : null;
        }
    }


}