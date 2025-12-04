using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;


namespace Qwiq.Mocks
{
    public class MockRevision : Revision
    {
        public MockRevision(Dictionary<string, object> dictionary, int index)
            : base(new MockFieldDefinitionCollection(dictionary.Keys.Select(MockFieldDefinition.Create)), index)
        {
            Contract.Requires(dictionary != null);
            Contract.Requires(index > 0);

            if (dictionary == null) throw new System.ArgumentNullException(nameof(dictionary));
            var fieldDefs = base.FieldDefinitions!;
            foreach (var kvp in dictionary)
            {
                var fd = fieldDefs[kvp.Key];
                SetFieldValue(fd.Id, kvp.Value);
            }
        }

        public MockRevision(Dictionary<string, object> dictionary)
            : this(dictionary, (int)dictionary["Index"])
        {

        }
    }
}
