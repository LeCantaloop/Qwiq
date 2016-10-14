using Microsoft.Qwiq.Mapper;
using Microsoft.Qwiq.Mapper.Attributes;

namespace Microsoft.Qwiq.Relatives.Tests.Mocks
{
    [WorkItemType("SimpleMockWorkItem")]
    public class SimpleMockModel : IIdentifiable
    {
        [FieldDefinition("ID")]
        public int Id { get; internal set; }

        [FieldDefinition("Priority")]
        public int Priority { get; internal set; }
    }
}

