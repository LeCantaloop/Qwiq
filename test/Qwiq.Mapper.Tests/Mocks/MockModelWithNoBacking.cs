using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Mapper.Tests.Mocks
{
    [WorkItemType("Baz")]
    public class MockModelWithNoBacking : IIdentifiable<int?>
    {
        [FieldDefinition("Id")]
        public virtual int? Id { get; internal set; }

        [FieldDefinition("FieldWithNoBackingStore")]
        public string FieldWithNoBackingStore { get; internal set; }
    }
}

