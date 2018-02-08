using Qwiq.Mapper.Attributes;

namespace Qwiq.Mapper.Mocks
{
    [WorkItemType("Task")]
    public class SimpleMockModel : IIdentifiable<int?>
    {
        [FieldDefinition(CoreFieldRefNames.Id)]
        public int? Id { get; internal set; }

        [FieldDefinition(CoreFieldRefNames.IterationId)]
        public int IntField { get; internal set; }
    }
}

