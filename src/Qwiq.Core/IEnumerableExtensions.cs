// ReSharper disable CheckNamespace
namespace System.Collections.Generic
// ReSharper restore CheckNamespace
{
    // ReSharper disable InconsistentNaming
    internal static class IEnumerableExtensions
    // ReSharper restore InconsistentNaming
    {
        public static IEnumerable<T[]> Partition<T>(this HashSet<T> source, int size)
        {
            var count = 0;
            T[] group = null; // use arrays as buffer
            foreach (var item in source)
            {
                if (group == null) group = new T[size];
                group[count++] = item;
                if (count != size) continue;
                yield return group;
                group = null;
                count = 0;
            }
            if (count <= 0) yield break;
            Array.Resize(ref group, count);
            yield return group;
        }
    }
}