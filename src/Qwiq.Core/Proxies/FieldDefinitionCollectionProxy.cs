using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Qwiq.Proxies
{
    internal abstract class FieldDefinitionCollectionProxy  : IFieldDefinitionCollection
    {
        public abstract IEnumerator<IFieldDefinition> GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract int Count { get; }

        public abstract IFieldDefinition this[string name] { get; }


        public abstract bool Contains(string fieldName);
    }
}
