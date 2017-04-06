using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class FieldDefinitionCollection : Qwiq.FieldDefinitionCollection
    {
        private readonly Tfs.FieldDefinitionCollection _innerCollection;

        internal FieldDefinitionCollection(Tfs.FieldDefinitionCollection innerCollection)
        {
            _innerCollection = innerCollection ?? throw new ArgumentNullException(nameof(innerCollection));
        }

        public override int Count => _innerCollection.Count;

        public override IFieldDefinition this[string name] => ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinition>(new FieldDefinition(_innerCollection[name]));

        public override bool Contains(string fieldName)
        {
            return _innerCollection.Contains(fieldName);
        }

        public override bool TryGetById(int id, out IFieldDefinition fieldDefinition)
        {
            try
            {
                var nativeFieldDefinition = _innerCollection.TryGetById(id);
                if (nativeFieldDefinition != null)
                {
                    fieldDefinition = ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinition>(new FieldDefinition(nativeFieldDefinition));
                    return true;
                }
            }
            catch (Exception)
            {
            }

            fieldDefinition = null;
            return false;
        }

        public override IEnumerator<IFieldDefinition> GetEnumerator()
        {
            return _innerCollection
                        .Cast<Tfs.FieldDefinition>()
                        .Select(field => ExceptionHandlingDynamicProxyFactory.Create<IFieldDefinition>(new FieldDefinition(field)))
                        .GetEnumerator();
        }
    }
}