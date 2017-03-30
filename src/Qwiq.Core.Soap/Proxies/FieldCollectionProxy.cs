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

        public IField this[string name] => ExceptionHandlingDynamicProxyFactory.Create<IField>(new FieldProxy(_innerCollection[name]));

        public int Count => _innerCollection.Count;

        public bool Contains(string name)
        {
            return _innerCollection.Contains(name);
        }

        public IField TryGetById(int id)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IField>(new FieldProxy(_innerCollection.TryGetById(id)));
        }

        public IField GetById(int id)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IField>(new FieldProxy(_innerCollection.GetById(id)));
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

