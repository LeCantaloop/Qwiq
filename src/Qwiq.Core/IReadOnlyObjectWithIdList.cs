using System;

using JetBrains.Annotations;

namespace Microsoft.Qwiq
{
    /// <summary>
    ///     Represents a read-only collection of elements that can be accessed by index, name, or id.
    /// </summary>
    /// <typeparam name="T">The type of elements in the read-only list.</typeparam>
    /// <typeparam name="TId">The type of the identifier of elements in the read-only list.</typeparam>
    public interface IReadOnlyObjectWithIdCollection<T, TId> : IReadOnlyObjectWithNameCollection<T>, IEquatable<IReadOnlyObjectWithIdCollection<T, TId>>
        where T : IIdentifiable<TId>
    {
        /// <summary>
        ///     Determins whether the read-only collection contains a specific value.
        /// </summary>
        /// <param name="id">The identity of an element.</param>
        /// <returns><c>true</c> if the item is found; otherwise, <c>false</c>.</returns>
        bool Contains([NotNull] TId id);

        /// <summary>
        ///     Gets the element with the specified id from the read-only collection.
        /// </summary>
        /// <param name="id">The identity of an element.</param>
        /// <returns>The element with the specified <paramref name="id" /> in the read-only list.</returns>
        [CanBeNull]
        T GetById([NotNull] TId id);

        /// <summary>
        ///     Attempts to get the value associated with the specified name from the read-only list.
        /// </summary>
        /// <param name="id">The identity of an element.</param>
        /// <param name="value">
        ///     When this method returns, contains the object from the read-only list that has the specified name,
        ///     or the default value of the type if the operation failed.
        /// </param>
        /// <returns><c>true</c> if the name was found in the read-only list; otherwise, <c>false</c>.</returns>
        bool TryGetById([NotNull] TId id, [CanBeNull] out T value);
    }
}