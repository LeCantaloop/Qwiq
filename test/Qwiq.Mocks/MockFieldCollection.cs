
namespace Qwiq.Mocks
{
    public class MockFieldCollection : FieldCollection
    {
        public MockFieldCollection(WorkItemCore w, IFieldDefinitionCollection definitions)
            : base(w, definitions, (r, d) => new MockField(r, d))
        {
        }

        public new void SetField(IField field)
        {
            base.SetField(field);
        }

        public void SetFieldValue(string name, object? value)
        {
            TryGetByName(name, out IField? f);
            if (f != null)
            {
                f.Value = value;
                SetField(f);
            }
        }
    }
}