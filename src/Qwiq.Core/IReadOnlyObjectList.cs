using System.Collections.Generic;

namespace Microsoft.Qwiq
{
    /// <summary>
    ///     Represents a read-only collection of elements that can be accessed by index or name.
    /// </summary>
    /// <typeparam name="T">The type of elements in the read-only list. </typeparam>
    public interface IReadOnlyObjectWithNameCollection<T> : IReadOnlyList<T>
    {
        /// <summary>
        ///     Gets the element of the specified name in the read-only list.
        /// </summary>
        /// <param name="name">The name of the element to get.</param>
        /// <returns>The element of the specified name in the read-only list.</returns>
        T this[string name] { get; }

        /// <summary>
        ///     Determines whether the read-only collection contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the read-only list.</param>
        /// <returns><c>true</c> if the item is found; otherwise, <c>false</c>.</returns>
        bool Contains(T value);

        /// <summary>
        ///     Determins whether the read-only collection contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name of an element.</param>
        /// <returns><c>true</c> if the item is found; otherwise, <c>false</c>.</returns>
        bool Contains(string name);

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

        /// <summary>
        ///     Attempts to get the value associated with the specified name from the read-only list.
        /// </summary>
        /// <param name="name">The name of an element.</param>
        /// <param name="value">
        ///     When this method returns, contains the object from the read-only list that has the specified name,
        ///     or the default value of the type if the operation failed.
        /// </param>
        /// <returns><c>true</c> if the name was found in the read-only list; otherwise, <c>false</c>.</returns>
        bool TryGetByName(string name, out T value);
    }
}