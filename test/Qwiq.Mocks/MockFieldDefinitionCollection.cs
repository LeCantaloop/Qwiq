using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [DebuggerStepThrough]
        public MockFieldDefinitionCollection(IEnumerable<IFieldDefinition> fieldDefinitions)
            : base(fieldDefinitions)
        {
        }

        public override IFieldDefinition this[string name]
        {
            get
            {
                try
                {
                    return base[name];
                }
                catch (DeniedOrNotExistException)
                {
                    // For Mocks, add the definition lazily
                    var def = MockFieldDefinition.Create(name);
                    Add(def);

                    Trace.TraceWarning($"Added missing field {def.ReferenceName} ({def.Id})");

                    return def;
                }
            }
        }
    }
}