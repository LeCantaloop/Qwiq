using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemType : IWorkItemType
    {
        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See ctor(IWorkItemStore, String, String).")]
        public MockWorkItemType()
            : this("Mock")
        {
        }

        [Obsolete(
            "This method has been deprecated and will be removed in a future release. See ctor(String, IEnumerable<IFieldDefinition>, String).")]
        public MockWorkItemType(string name)
            : this(name, CoreFieldRefNames.All.Select(s => new MockFieldDefinition(s)))
        {
        }

        public MockWorkItemType(string name, IEnumerable<IFieldDefinition> fieldDefinitions, string description = null)
            : this(name, new MockFieldDefinitionCollection(fieldDefinitions), description)
        {
            if (fieldDefinitions == null) throw new ArgumentNullException(nameof(fieldDefinitions));
        }

        public MockWorkItemType(string name, IFieldDefinitionCollection fieldDefinitions, string description = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            FieldDefinitions = fieldDefinitions ?? throw new ArgumentNullException(nameof(fieldDefinitions));
            Description = description;
        }

        public string Description { get; }

        public IFieldDefinitionCollection FieldDefinitions { get; }

        public string Name { get; }

        public IWorkItem NewWorkItem()
        {
            return new MockWorkItem(this);
        }
    }
}