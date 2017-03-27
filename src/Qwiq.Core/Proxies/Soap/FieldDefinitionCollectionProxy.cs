using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Proxies.Soap
{
    internal class FieldDefinitionCollectionProxy : Proxies.FieldDefinitionCollectionProxy
    {
        private readonly FieldDefinitionCollection _innerCollection;

        internal FieldDefinitionCollectionProxy(FieldDefinitionCollection innerCollection)
        {
            _innerCollection = innerCollection;
        }

        public override int Count => _innerCollection.Count;

        public override IFieldDefinition this[string name] => ExceptionHandlingDynamicProxyFactory
            .Create<IFieldDefinition>(new FieldDefinitionProxy(_innerCollection[name]));

        public override bool Contains(string fieldName)
        {
            return _innerCollection.Contains(fieldName);
        }

        public override IEnumerator<IFieldDefinition> GetEnumerator()
        {
            return _innerCollection.Cast<FieldDefinition>()
                                   .Select(
                                       field => ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinition>(
                                           new FieldDefinitionProxy(field)))
                                   .GetEnumerator();
        }
    }
}