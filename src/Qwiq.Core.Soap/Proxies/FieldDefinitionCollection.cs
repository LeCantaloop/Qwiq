using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    internal class FieldDefinitionCollection : Qwiq.FieldDefinitionCollection
    {
        private readonly TeamFoundation.WorkItemTracking.Client.FieldDefinitionCollection _innerCollection;

        internal FieldDefinitionCollection(TeamFoundation.WorkItemTracking.Client.FieldDefinitionCollection innerCollection)
        {
            _innerCollection = innerCollection;
        }

        public override int Count => _innerCollection.Count;

        public override IFieldDefinition this[string name] => ExceptionHandlingDynamicProxyFactory
            .Create<IFieldDefinition>(new FieldDefinition(_innerCollection[name]));

        public override bool Contains(string fieldName)
        {
            return _innerCollection.Contains(fieldName);
        }

        public override IEnumerator<IFieldDefinition> GetEnumerator()
        {
            return _innerCollection.Cast<TeamFoundation.WorkItemTracking.Client.FieldDefinition>()
                                   .Select(
                                       field => ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinition>(
                                           new FieldDefinition(field)))
                                   .GetEnumerator();
        }
    }
}