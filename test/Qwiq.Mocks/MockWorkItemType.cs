using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemType : WorkItemType
    {
        [DebuggerStepThrough]
        [Obsolete("This method has been deprecated and will be removed in a future release. See ctor(IWorkItemStore, String, String).")]
        public MockWorkItemType()
            : this("Mock")
        {
        }

        [DebuggerStepThrough]
        public MockWorkItemType(string name, string description = null, IWorkItemStore store = null)
            : this(name, new MockFieldDefinitionCollection(CoreFieldDefinitions.All), description, store)
        {
        }

        [DebuggerStepThrough]
        public MockWorkItemType(string name, params string[] fields)
            : this(name, fields as IEnumerable<string>)
        {
        }

        public MockWorkItemType(string name, IEnumerable<string> fields, string description = null, IWorkItemStore store = null)
            : this(name, fields.Select(MockFieldDefinition.Create), description, store)
        {
        }

        public MockWorkItemType(
            string name,
            IEnumerable<IFieldDefinition> fieldDefinitions,
            string description = null,
            IWorkItemStore store = null)
            : base(name, description, null, null)
        {
            if (fieldDefinitions == null) throw new ArgumentNullException(nameof(fieldDefinitions));
            WorkItemFactory = () => new MockWorkItem(this);
            FieldDefinitionFactory =
                    () => new MockFieldDefinitionCollection(
                                                            fieldDefinitions
                                                                    .Union(
                                                                           new[]
                                                                               {
                                                                                   CoreFieldDefinitions.Id,
                                                                                   CoreFieldDefinitions.WorkItemType
                                                                               })
                                                                    .Distinct());
            Store = store;
        }

        public MockWorkItemType(
            string name,
            IFieldDefinitionCollection fieldDefinitions,
            string description = null,
            IWorkItemStore store = null)
            : base(name, description, new Lazy<IFieldDefinitionCollection>(() => fieldDefinitions))
        {
            WorkItemFactory = () => new MockWorkItem(this);
            Store = store;
        }

        public IWorkItemStore Store { get; set; }
    }
}