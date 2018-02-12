namespace Qwiq
{
    /// <summary>
    ///     Represents a read-only collection of elements that can be accessed by index or name.
    /// </summary>
    /// <typeparam name="T">The type of elements in the read-only list. </typeparam>
    public interface IReadOnlyObjectWithNameCollection<T> : IReadOnlyObjectCollection<T>
    {
        /// <summary>
        ///     Gets the element of the specified name in the read-only list.
        /// </summary>
        /// <param name="name">The name of the element to get.</param>
        /// <returns>The element of the specified name in the read-only list.</returns>
        T this[string name] { get; }

        /// <summary>
        ///     Determins whether the read-only collection contains an element with the specified name.
        /// </summary>
        /// <param name="name">The name of an element.</param>
        /// <returns><c>true</c> if the item is found; otherwise, <c>false</c>.</returns>
        bool Contains(string name);

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