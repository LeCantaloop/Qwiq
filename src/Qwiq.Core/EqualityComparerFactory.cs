using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Qwiq
{
    public static class EqualityComparerFactory<T>
    {
        private class MyComparer : IEqualityComparer<T>
        {
            private readonly Func<T, int> _getHashCodeFunc;
            private readonly Func<T, T, bool> _equalsFunc;

            public MyComparer(Func<T, int> getHashCodeFunc, Func<T, T, bool> equalsFunc)
            {
                _getHashCodeFunc = getHashCodeFunc;
                _equalsFunc = equalsFunc;
            }

            public bool Equals(T x, T y) => _equalsFunc(x, y);

            public int GetHashCode(T obj) => _getHashCodeFunc(obj);
        }

        public static IEqualityComparer<T> CreateComparer(Func<T, int> getHashCodeFunc, Func<T, T, bool> equalsFunc)
        {
            if (getHashCodeFunc == null)
                throw new ArgumentNullException(nameof(getHashCodeFunc));
            if (equalsFunc == null)
                throw new ArgumentNullException(nameof(equalsFunc));

            return new MyComparer(getHashCodeFunc, equalsFunc);
        }
    }
}
