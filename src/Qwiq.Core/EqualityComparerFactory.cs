using System;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public static class EqualityComparerFactory<T>
    {
        private class MyComparer : GenericComparer<T>
        {
            private readonly Func<T, int> _getHashCodeFunc;
            private readonly Func<T, T, bool> _equalsFunc;

            public MyComparer(Func<T, int> getHashCodeFunc, Func<T, T, bool> equalsFunc = null)
            {
                _getHashCodeFunc = getHashCodeFunc ?? throw new ArgumentNullException(nameof(getHashCodeFunc));
                _equalsFunc = equalsFunc;
            }

            public override bool Equals(T x, T y) => _equalsFunc == null ? _equalsFunc(x, y) : base.Equals(x, y);

            public override int GetHashCode(T obj) => _getHashCodeFunc(obj);
        }

        public static IEqualityComparer<T> CreateComparer(Func<T, int> getHashCodeFunc, Func<T, T, bool> equalsFunc = null)
        {


            return new MyComparer(getHashCodeFunc, equalsFunc);
        }
    }
}
