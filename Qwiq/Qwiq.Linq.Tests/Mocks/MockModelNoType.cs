using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Linq.Tests.Mocks
{
    public class MockModelNoType
    {
        [FieldDefinition("ID")]
        public int Id { get; internal set; }

        [FieldDefinition("IntField")]
        public int IntField { get; internal set; }
    }
}

