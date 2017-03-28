using System;

namespace Microsoft.Qwiq.Mocks
{
    public class MockFieldDefinition : FieldDefinition
    {
        public MockFieldDefinition(string name, string referenceName)
            : base(referenceName, name)
        {
        }
    }
}
