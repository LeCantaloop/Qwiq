using System;

namespace Qwiq.Mocks
{
    public class MockQueryDefinition : QueryDefinition
    {
        public MockQueryDefinition(Guid id, string name, string wiql, string path)
            : base(id, name, wiql, path)
        {
        }
    }
}
