using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    public class Revision : IRevision, IRevisionInternal
    {
        internal IFieldDefinitionCollection FieldDefinitions { get; }

        private readonly Dictionary<int, object> _values;

        private IFieldCollection _fields;

        internal Revision(
            [NotNull] IFieldDefinitionCollection definitions,
            int revision)
        {
            Rev = revision;
            _values = new Dictionary<int, object>();
            FieldDefinitions = definitions;
        }

        internal Revision([NotNull] IWorkItem workItem, int revision)
        {
            WorkItem = workItem ?? throw new ArgumentNullException(nameof(workItem));
            Rev = revision;
            FieldDefinitions = workItem.Type.FieldDefinitions;
        }

        /// <inheritdoc />
        public virtual IEnumerable<IAttachment> Attachments => throw new NotSupportedException();

        public IFieldCollection Fields => _fields ?? (_fields = new FieldCollection(this, FieldDefinitions, (r, d) => new Field(r, d)));

        /// <inheritdoc />
        public int Index => Rev.GetValueOrDefault(0);

        /// <inheritdoc />
        public virtual IEnumerable<ILink> Links => throw new NotSupportedException();

        public int? Id => WorkItem?.Id;

        public int? Rev { get; }

        public string Url => WorkItem?.Url;

        public IWorkItem WorkItem { get; }

        public virtual object this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                return Fields[name].Value;
            }
        }

        /// <inheritdoc />
        public virtual string GetTagLine()
        {
            throw new NotSupportedException();
        }

        object IWorkItemCore.this[string name]
        {
            get => this[name];
            set => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public object GetCurrentFieldValue(IFieldDefinition fieldDefinition)
        {
            if (WorkItem != null) return WorkItem.Fields[fieldDefinition.ReferenceName];

            return _values[fieldDefinition.Id];
        }

        /// <inheritdoc />
        void IRevisionInternal.SetFieldValue(IFieldDefinition fieldDefinition, object value)
        {
            throw new InvalidOperationException();
        }

        internal bool HasValue(int fieldId) => _values.ContainsKey(fieldId);

        internal void SetFieldValue(int fieldId, object value) => _values[fieldId] = value;
    }
}