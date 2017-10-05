using System;
using JetBrains.Annotations;

namespace Qwiq.Mocks
{
    public class MockQueryDefinition: QueryDefinition
    {
        public MockQueryDefinition(Guid id, [NotNull] string name, [NotNull] string wiql)
            : base(id, name, wiql)
        {
        }
    }
}
