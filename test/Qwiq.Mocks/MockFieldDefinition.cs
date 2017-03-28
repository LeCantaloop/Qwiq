using System;

namespace Microsoft.Qwiq.Mocks
{
    public class MockFieldDefinition : Microsoft.Qwiq.Proxies.FieldDefinitionProxy
    {
        public MockFieldDefinition(string referenceName)
        {
            if (string.IsNullOrWhiteSpace(referenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(referenceName));

            ReferenceName = referenceName;
        }

        public MockFieldDefinition(string name, string referenceName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(referenceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(referenceName));
            Name = name;
            ReferenceName = referenceName;
        }
    }
}
