using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Mapper.Tests.Mocks
{
    [WorkItemType("MockWorkItem")]
    public class MockModelSubclass : MockModel
    {
        public override string StringField => "42";
    }
}

