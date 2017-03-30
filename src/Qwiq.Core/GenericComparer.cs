using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public class GenericComparer<T> : IComparer<T>, IEqualityComparer<T>
    {
        public static readonly GenericComparer<T> Default = new GenericComparer<T>();

        public virtual int Compare(T x, T y)
        {
            Type type = typeof(T);

            // Enumerable?
            var enumerableX = x as IEnumerable;
            var enumerableY = y as IEnumerable;

            if (enumerableX != null && enumerableY != null)
            {
                IEnumerator enumeratorX = enumerableX.GetEnumerator();
                IEnumerator enumeratorY = enumerableY.GetEnumerator();

                while (true)
                {
                    bool hasNextX = enumeratorX.MoveNext();
                    bool hasNextY = enumeratorY.MoveNext();

                    if (!hasNextX || !hasNextY)
                    {
                        return hasNextX == hasNextY ? 0 : -1;
                    }

                    if (!Equals(enumeratorX.Current, enumeratorY.Current))
                    {
                        return -1;
                    }
                }
            }

            // Null?
            if (!type.IsValueType
                || (type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))))
            {
                if (object.Equals(x, default(T)))
                {
                    if (object.Equals(y, default(T)))
                    {
                        return 0;
                    }

                    return -1;
                }

                if (object.Equals(y, default(T)))
                {
                    return -1;
                }
            }

            // Implements IComparable<T>?
            var comparable1 = x as IComparable<T>;

            if (comparable1 != null)
            {
                return comparable1.CompareTo(y);
            }

            // Implements IComparable?
            var comparable2 = x as IComparable;

            if (comparable2 != null)
            {
                return comparable2.CompareTo(y);
            }

            // Implements IEquatable<T>?
            var equatable = x as IEquatable<T>;

            if (equatable != null)
            {
                return equatable.Equals(y) ? 0 : -1;
            }

            // Last case, rely on Object.Equals
            return object.Equals(x, y) ? 0 : -1;
        }

        public virtual bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return Compare(x, y) == 0;
        }

        public virtual int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}