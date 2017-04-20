using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    public class GenericComparer<T> : IComparer<T>, IEqualityComparer<T>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly GenericComparer<T> Default = new GenericComparer<T>();

        public virtual int Compare(T x, T y)
        {
            // Enumerable?
            var enumerableX = x as IEnumerable;
            var enumerableY = y as IEnumerable;

            if (enumerableX != null && enumerableY != null)
            {
                var enumeratorX = enumerableX.GetEnumerator();
                var enumeratorY = enumerableY.GetEnumerator();

                while (true)
                {
                    var hasNextX = enumeratorX.MoveNext();
                    var hasNextY = enumeratorY.MoveNext();

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

            var type = typeof(T);

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

            if (x is IComparable<T> comparable1)
            {
                return comparable1.CompareTo(y);
            }

            // Implements IComparable?

            if (x is IComparable comparable2)
            {
                return comparable2.CompareTo(y);
            }

            // Implements IEquatable<T>?

            if (x is IEquatable<T> equatable)
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