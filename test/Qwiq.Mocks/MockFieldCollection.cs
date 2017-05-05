using JetBrains.Annotations;

namespace Microsoft.Qwiq.Mocks
{
    public class MockFieldCollection : FieldCollection
    {
        public MockFieldCollection([NotNull] WorkItemCore w, [NotNull] IFieldDefinitionCollection definitions)
            : base(w, definitions, (r, d) => new MockField(r, d))
        {
        }

        public new void SetField([NotNull] IField field)
        {
            base.SetField(field);
        }

        public void SetFieldValue([NotNull] string name, [CanBeNull] object value)
        {
            TryGetByName(name, out IField f);
            f.Value = value;
            SetField(f);
        }
    }
}