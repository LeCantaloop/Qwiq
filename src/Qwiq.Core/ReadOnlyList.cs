using System;
using System.Collections;

namespace Microsoft.Qwiq
{
    public abstract class ReadOnlyList : IList, ICollection, IEnumerable
    {
        public abstract int Count { get; }

        public bool IsFixedSize => true;

        public bool IsReadOnly => true;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public object this[int index]
        {
            get => GetItem(index);
            set => throw new NotSupportedException();
        }

        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(object value)
        {
            return IndexOf(value) != -1;
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (array.Rank != 1) throw new ArgumentException();
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (array.Length - index < Count) throw new ArgumentException();
            foreach (var obj in this) array.SetValue(obj, index++);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator<object>(this);
        }

        public int IndexOf(object value)
        {
            for (var i = 0; i < Count; i++) if (Equals(this[i], value)) return i;

            return -1;
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        protected abstract object GetItem(int index);

        private sealed class Enumerator<T> : IEnumerator
        {
            private readonly IList _list;

            private int _index;

            private int _version;

            internal Enumerator(IList list)
            {
                _index = -1;
                _list = list;
                _version = GetVersionTag();
            }

            public T Current => (T)_list[_index];

            object IEnumerator.Current => _list[_index];

            public bool MoveNext()
            {
                if (GetVersionTag() != _version) throw new InvalidOperationException();
                var num = _index + 1;
                _index = num;
                return num < _list.Count;
            }

            public void Reset()
            {
                _index = -1;
                _version = GetVersionTag();
            }

            private int GetVersionTag()
            {
                return _list.Count;
            }
        }
    }
}