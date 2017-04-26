using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Mapper.Mocks
{
    [WorkItemType("MockWorkItem")]
    public class MockModelSubclass : MockModel
    {
        public override string StringField => "42";
    }
}

