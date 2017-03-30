using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Exceptions;

using Tfs = Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Microsoft.Qwiq.Soap
{
    internal class FieldCollection : IFieldCollection
    {
        private readonly Tfs.FieldCollection _innerCollection;

        public FieldCollection(Tfs.FieldCollection innerCollection)
        {
            _innerCollection = innerCollection;
        }

        public IField this[string name] => ExceptionHandlingDynamicProxyFactory.Create<IField>(new Field(_innerCollection[name]));

        public int Count => _innerCollection.Count;

        public bool Contains(string name)
        {
            return _innerCollection.Contains(name);
        }

        public IField TryGetById(int id)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IField>(new Field(_innerCollection.TryGetById(id)));
        }

        public IField GetById(int id)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IField>(new Field(_innerCollection.GetById(id)));
        }

        public IEnumerator<IField> GetEnumerator()
        {
            return _innerCollection.Cast<Tfs.Field>().Select(field => ExceptionHandlingDynamicProxyFactory.Create<IField>(new Field(field))).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

