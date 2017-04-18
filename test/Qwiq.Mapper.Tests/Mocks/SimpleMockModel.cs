using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Mapper.Tests.Mocks
{
    [WorkItemType("SimpleMockWorkItem")]
    public class SimpleMockModel : IIdentifiable
    {
        [FieldDefinition(CoreFieldRefNames.Id)]
        public int? Id { get; internal set; }

        [FieldDefinition(CoreFieldRefNames.IterationId)]
        public int IntField { get; internal set; }
    }
}

