namespace Microsoft.Qwiq.Mocks
{
    public class MockWorkItemType : IWorkItemType
    {
        public MockWorkItemType()
            : this("Mock")
        {
        }

        public MockWorkItemType(string name)
        {
            Name = name;
        }

        public string Description { get; set; }

        public string Name { get; set; }

        public IWorkItem NewWorkItem()
        {
            return new MockWorkItem(Name);
        }
    }
}
