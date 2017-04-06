using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public interface IRevision : IWorkItemCore
    {
        /// <summary>
        /// Gets the fields of the work item in this revision.
        /// </summary>
        IFieldCollection Fields { get; }

        /// <summary>
        /// Gets the value of the specified field in the work item of this revision.
        /// </summary>
        /// <param name="name">The field of interest in the work item of this revision.</param>
        /// <returns>The value of the specified field.</returns>
        new object this[string name] { get; }

      
    }

    internal class Revision : IRevision
    {
        private readonly Lazy<IFieldCollection> _fields;

        private Dictionary<int, object> _values;

        internal Revision(
            IFieldDefinitionCollection definitions,
            int revision,
            Func<IRevision, IFieldDefinitionCollection, IFieldCollection> fieldFactory)
        {
            Rev = revision;
            _values = new Dictionary<int, object>();
            _fields = new Lazy<IFieldCollection>(() => fieldFactory(this, definitions));
        }

        internal Revision(
            WorkItem workItem,
            int revision)
        {
            WorkItem = workItem ?? throw new ArgumentNullException(nameof(workItem));
            Rev = revision;
            _fields = new Lazy<IFieldCollection>(() => WorkItem.Fields);
        }

        public virtual object this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                return Fields[name].Value;
            }
        }

        public int? Rev { get; }

        object IWorkItemCore.this[string name]
        {
            get => this[name];
            set => throw new NotSupportedException();
        }

        private WorkItem WorkItem { get; }

        public IFieldCollection Fields => _fields.Value;

       
        public int? Id => WorkItem?.Id;

        public string Url => WorkItem?.Url;
    }
}


