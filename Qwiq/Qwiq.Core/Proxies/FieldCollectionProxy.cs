using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.Qwiq.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.IE.Qwiq.Proxies
{
    internal class FieldCollectionProxy : IFieldCollection
    {
        private readonly FieldCollection _innerCollection;

        public FieldCollectionProxy(FieldCollection innerCollection)
        {
            _innerCollection = innerCollection;
        }

        IEnumerator<IField> IEnumerable<IField>.GetEnumerator()
        {
            return _innerCollection.Cast<IField>().GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
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
    }
}
