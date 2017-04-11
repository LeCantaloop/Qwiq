namespace Microsoft.Qwiq.Mocks
{
    public class MockFieldCollection : FieldCollection
    {
        public MockFieldCollection(WorkItemCore w, IFieldDefinitionCollection definitions)
            : base(w, definitions, (r, d) => new MockField(r, d))
        {
        }
    }
}