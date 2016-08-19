using Microsoft.IE.Qwiq.Mapper;
using Microsoft.IE.Qwiq.Mapper.Attributes;

namespace Microsoft.IE.Qwiq.Linq.Tests.Mocks
{
    [WorkItemType("SimpleMockWorkItem")]
    public class SimpleMockModel : IIdentifiable
    {
        [FieldDefinition("ID")]
        public int Id { get; internal set; }

        [FieldDefinition("IntField")]
        public int IntField { get; internal set; }
    }
}
