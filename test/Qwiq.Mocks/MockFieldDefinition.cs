namespace Microsoft.Qwiq.Mocks
{
    public class MockFieldDefinition : FieldDefinition
    {
        public MockFieldDefinition(int id, string name, string referenceName)
            : base(id, referenceName, name)
        {
        }

        public MockFieldDefinition(string name, string referenceName)
            : base(referenceName, name)
        {
        }

        /// <summary>
        /// Given a reference name or friendly name, attempt to look up from existing fields. If the field is not a core field, a new field definition is created.
        /// </summary>
        /// <param name="name">The reference name or friendly name of a field.</param>
        /// <returns></returns>
        public static IFieldDefinition Create(string name)
        {
            IFieldDefinition field;
            if (CoreFieldDefinitions.NameLookup.TryGetValue(name, out field))
            {
                return field;
            }

            if (CoreFieldDefinitions.ReferenceNameLookup.TryGetValue(name, out field))
            {
                return field;
            }

            field = new MockFieldDefinition(name, $"Microsoft.Qwiq.Mocks.{name.Replace(" ", string.Empty)}");

            return field;
        }
    }
}
