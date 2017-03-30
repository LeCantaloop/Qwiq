using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemType : WorkItemType
    {
        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See ctor(IWorkItemStore, String, String).")]
        public MockWorkItemType()
            : this("Mock")
        {
        }

        public MockWorkItemType(string name, string description = null)
            : this(name, CoreFieldDefinitions.All, description)
        {
        }

        public MockWorkItemType(string name, IEnumerable<IFieldDefinition> fieldDefinitions, string description = null)
            : base(name, description, null, null)
        {
            if (fieldDefinitions == null) throw new ArgumentNullException(nameof(fieldDefinitions));
            WorkItemFactory = () => new MockWorkItem(this);
            FieldDefinitionFactory = () => new MockFieldDefinitionCollection(fieldDefinitions);
        }

        public MockWorkItemType(string name, IFieldDefinitionCollection fieldDefinitions, string description = null)
            : base(name, description, new Lazy<IFieldDefinitionCollection>(() => fieldDefinitions))
        {
            WorkItemFactory = () => new MockWorkItem(this);
        }
    }
}