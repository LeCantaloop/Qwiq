using Microsoft.IE.Qwiq.Mapper.Attributes;

namespace Microsoft.IE.Qwiq.Mapper.Tests.Mocks
{
    [WorkItemType("Baz")]
    public class MockModelWithNoBacking
    {
        [FieldDefinition("Id")]
        public virtual int Id { get; internal set; }

        [FieldDefinition("FieldWithNoBackingStore")]
        public string FieldWithNoBackingStore { get; internal set; }
    }
}
