using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

namespace Microsoft.Qwiq.Proxies.Soap
{
    public class FieldDefinitionCollectionProxy : IFieldDefinitionCollection
    {
        private readonly TeamFoundation.WorkItemTracking.Client.FieldDefinitionCollection _innerCollection;

        public FieldDefinitionCollectionProxy(TeamFoundation.WorkItemTracking.Client.FieldDefinitionCollection innerCollection)
        {
            _innerCollection = innerCollection;
        }

        public IEnumerator<IFieldDefinition> GetEnumerator()
        {
            return _innerCollection
                .Cast<TeamFoundation.WorkItemTracking.Client.FieldDefinition>()
                .Select(field => ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinition>(new FieldDefinitionProxy(field)))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _innerCollection.Count;

        public IFieldDefinition this[string name] => ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinition>(
            new FieldDefinitionProxy(_innerCollection[name]));

        public bool Contains(string fieldName)
        {
            return _innerCollection.Contains(fieldName);
        }
    }
}