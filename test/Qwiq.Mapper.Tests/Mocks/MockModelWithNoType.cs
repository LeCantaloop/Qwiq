using Qwiq.Mapper.Attributes;

namespace Qwiq.Mapper.Mocks
{
    public class MockModelWithNoType : IIdentifiable<int?>
    {
        [FieldDefinition("Id")]
        public virtual int? Id { get; internal set; }

        [FieldDefinition("IntField")]
        public int IntField { get; internal set; }
}
}

