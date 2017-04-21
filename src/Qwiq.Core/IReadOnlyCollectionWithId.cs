using System;

namespace Microsoft.Qwiq
{
    public interface IReadOnlyCollectionWithId<T, TId> : IReadOnlyCollection<T>, IEquatable<IReadOnlyCollectionWithId<T, TId>>
        where T : IIdentifiable<TId>
    {
        T GetById(TId id);
        bool Contains(TId id);
        bool TryGetById(TId id, out T value);
    }
}