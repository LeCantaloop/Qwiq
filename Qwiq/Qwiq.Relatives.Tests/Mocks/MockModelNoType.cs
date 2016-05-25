using Microsoft.IE.Qwiq.Mapper.Attributes;

namespace Microsoft.IE.Qwiq.Relatives.Tests.Mocks
{
    public class MockModelNoType
    {
        [FieldDefinition("ID")]
        public int Id { get; internal set; }

        [FieldDefinition("Priority")]
        public int Priority { get; internal set; }
    }
}
