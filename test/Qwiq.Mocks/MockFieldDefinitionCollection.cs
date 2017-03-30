using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Mocks
{
    public class MockFieldDefinitionCollection : FieldDefinitionCollection
    {
        public MockFieldDefinitionCollection(IWorkItemStore store)
            : base(store?.Projects.SelectMany(s => s.WorkItemTypes).SelectMany(s => s.FieldDefinitions).Select(s => s))
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
        }

        public MockFieldDefinitionCollection(IEnumerable<IFieldDefinition> fieldDefinitions)
            : base(fieldDefinitions)
        {
        }
    }
}