using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Qwiq.Mocks
{
    public class MockFieldDefinition : IFieldDefinition
    {
        public MockFieldDefinition(string name, string referenceName)
        {
            Name = name;
            ReferenceName = referenceName;
        }

        public string Name { get; }

        public string ReferenceName { get; }
    }
}
