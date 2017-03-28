using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap.Proxies
{
    internal class FieldCollectionProxy : IFieldCollection
    {
        private readonly Tfs.FieldCollection _innerCollection;

        public FieldCollectionProxy(Tfs.FieldCollection innerCollection)
        {
            _innerCollection = innerCollection;
        }

        public IField this[string name]
        {
            get { return ExceptionHandlingDynamicProxyFactory.Create<IField>(new FieldProxy(_innerCollection[name])); }
        }

        public int Count
        {
            get { return _innerCollection.Count; }
        }
        public bool Contains(string fieldName)
        {
            return _innerCollection.Contains(fieldName);
        }

        public IEnumerator<IField> GetEnumerator()
        {
            return _innerCollection.Cast<Tfs.Field>().Select(field => ExceptionHandlingDynamicProxyFactory.Create<IField>(new FieldProxy(field))).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

