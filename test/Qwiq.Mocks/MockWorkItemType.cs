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
            : this(name, CoreFieldRefNames.All.Select(s => new MockFieldDefinition(s, CoreFieldRefNames.NameLookup[s])), description)
        {
        }

        public MockWorkItemType(string name, IEnumerable<IFieldDefinition> fieldDefinitions, string description = null)
            : this(name, new MockFieldDefinitionCollection(fieldDefinitions), description)
        {
            if (fieldDefinitions == null) throw new ArgumentNullException(nameof(fieldDefinitions));
        }

        public MockWorkItemType(string name, IFieldDefinitionCollection fieldDefinitions, string description = null)
            : base(name, description, new Lazy<IFieldDefinitionCollection>(() => fieldDefinitions))
        {
            WorkItemFactory = () => new MockWorkItem(this);
        }
    }
}