using System;
using Microsoft.IE.Qwiq.Mapper.Attributes;

namespace Microsoft.IE.Qwiq.Mapper.Tests.Mocks
{
    public class MockModelWithNoType
    {
        [FieldDefinition("Id")]
        public virtual int Id { get; internal set; }

        [FieldDefinition("IntField")]
        public int IntField { get; internal set; }
}
}
