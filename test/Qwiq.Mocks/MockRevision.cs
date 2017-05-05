using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

using JetBrains.Annotations;

namespace Microsoft.Qwiq.Mocks
{
    [PublicAPI]
    public class MockRevision : Revision
    {
        public MockRevision([NotNull] Dictionary<string, object> dictionary, int index)
            :base(new MockFieldDefinitionCollection(dictionary.Keys.Select(MockFieldDefinition.Create)), index)
        {
            Contract.Requires(dictionary != null);
            Contract.Requires(index > 0);

            foreach (var kvp in dictionary)
            {
                var fd = FieldDefinitions[kvp.Key];
                SetFieldValue(fd.Id, kvp.Value);
            }
        }

        public MockRevision([NotNull] Dictionary<string, object> dictionary)
            : this(dictionary, (int)dictionary["Index"])
        {
           
        }
    }
}
