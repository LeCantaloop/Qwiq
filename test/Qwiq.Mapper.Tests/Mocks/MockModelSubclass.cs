using Qwiq.Mapper.Attributes;

namespace Qwiq.Mapper.Mocks
{
    [WorkItemType("MockWorkItem")]
    public class MockModelSubclass : MockModel
    {
        public override string StringField => "42";
    }
}

