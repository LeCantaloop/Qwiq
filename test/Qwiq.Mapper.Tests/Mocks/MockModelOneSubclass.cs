using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Mapper.Tests.Mocks
{
    [WorkItemType("Baz")]
    public class MockModelOneSubclass : MockModelOne
    {
        public override string StringField { get { return "42"; } }
    }
}

