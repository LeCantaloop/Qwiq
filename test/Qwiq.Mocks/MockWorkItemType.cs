using System;

namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemType : IWorkItemType
    {
        private readonly IWorkItemStore _store;

        public MockWorkItemType()
            : this("Mock")
        {
        }

        public MockWorkItemType(string name)
            :this(new MockWorkItemStore(), name)
        {
        }

        public MockWorkItemType(IWorkItemStore store, string name, string description = null)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            Name = name ?? throw new ArgumentNullException(nameof(name));

            Description = description;
        }

        public string Description { get; set; }

        public string Name { get; set; }

        public IFieldDefinitionCollection FieldDefinitions => new MockFieldDefinitionCollection(_store, this);

        public IWorkItem NewWorkItem()
        {
            return new MockWorkItem(this);
        }
    }
}
