using System;

namespace Microsoft.Qwiq
{
    public interface IReadOnlyListWithId<T, TId> : IReadOnlyList<T>, IEquatable<IReadOnlyListWithId<T, TId>>
        where T : IIdentifiable<TId>
    {
        T GetById(TId id);
        bool Contains(TId id);
        bool TryGetById(TId id, out T value);
    }
}