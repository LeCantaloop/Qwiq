using System.Collections.Generic;

namespace Qwiq
{
    public interface IReadOnlyObjectCollection<T> : IReadOnlyList<T>
    {
        /// <summary>
        ///     Determines whether the read-only collection contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the read-only list.</param>
        /// <returns><c>true</c> if the item is found; otherwise, <c>false</c>.</returns>
        bool Contains(T value);
        /// <summary>
        ///     Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the read-only list.</returns>
        T GetItem(int index);
        /// <summary>
        ///     Searches for the specified object and returns the index of its first occurrence in the read-only list.
        /// </summary>
        /// <param name="value">The object to locate in the read-only list.</param>
        /// <returns>A zero-based index of the first occurrence of <paramref name="value" /> in the read-only list; otherwise, -1.</returns>
        int IndexOf(T value);
    }
}