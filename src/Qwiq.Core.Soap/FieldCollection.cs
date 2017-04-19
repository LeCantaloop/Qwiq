using System;
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

        internal FieldCollection(Tfs.FieldCollection innerCollection)
        {
            _innerCollection = innerCollection ?? throw new ArgumentNullException(nameof(innerCollection));
        }

        public int Count => _innerCollection.Count;

        public IField this[string name] => ExceptionHandlingDynamicProxyFactory.Create<IField>(
            new Field(_innerCollection[name]));

        public bool Contains(IField value)
        {
            return IndexOf(value) != -1;
        }

        public bool Contains(string name)
        {
            return _innerCollection.Contains(name);
        }

        public IField GetItem(int index)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IField>(new Field(_innerCollection[index]));
        }

        public int IndexOf(IField value)
        {
            if (value is Field) return _innerCollection.IndexOf(((Field)value).NativeField);

            for (var i = 0; i < Count; i++) if (GenericComparer<IField>.Default.Equals(this[i], value)) return i;

            return -1;
        }

        public bool TryGetByName(string name, out IField value)
        {
            if (name == null)
            {
                value = null;
                return false;
            }
            if (!Contains(name))
            {
                value = null;
                return false;
            }

            try
            {
                value = this[name];
                return false;
            }
            catch (Exception)
            {
                value = null;
                return false;
            }
        }

        public IField GetById(int id)
        {
            return ExceptionHandlingDynamicProxyFactory.Create<IField>(new Field(_innerCollection.GetById(id)));
        }

        public bool Contains(int id)
        {
            return _innerCollection.Contains(id);
        }

        public IEnumerator<IField> GetEnumerator()
        {
            return _innerCollection.Cast<Tfs.Field>()
                                   .Select(
                                       field => ExceptionHandlingDynamicProxyFactory.Create<IField>(new Field(field)))
                                   .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetById(int id, out IField value)
        {
            try
            {
                var nativeField = _innerCollection.TryGetById(id);
                if (nativeField != null)
                {
                    value = ExceptionHandlingDynamicProxyFactory.Create<IField>(new Field(nativeField));
                    return true;
                }
            }
            catch (Exception)
            {
            }

            value = null;
            return false;
        }

        public IField this[int index]
        {
            get
            {
                if (index < 0 || index >= _innerCollection.Count) throw new ArgumentOutOfRangeException(nameof(index));
                return GetItem(index);
            }
        }

        public bool Equals(IReadOnlyCollectionWithId<IField, int> other)
        {
            return Comparer.FieldCollection.Equals(this, other);
        }
    }
}