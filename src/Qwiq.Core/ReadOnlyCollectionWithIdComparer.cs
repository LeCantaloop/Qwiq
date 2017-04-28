using System.Linq;

namespace Microsoft.Qwiq
{
    public class ReadOnlyCollectionWithIdComparer<T, TId> : GenericComparer<IReadOnlyObjectWithIdCollection<T, TId>>
        where T : IIdentifiable<TId>
    {
        public new static readonly ReadOnlyCollectionWithIdComparer<T, TId> Default = new ReadOnlyCollectionWithIdComparer<T, TId>();

        public override bool Equals(IReadOnlyObjectWithIdCollection<T, TId> x, IReadOnlyObjectWithIdCollection<T, TId> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            if (x.Count != y.Count) return false;
            var source = y.ToList();
            foreach (var item in x)
            {
                if (!y.Contains(item.Id)) return false;
                var item2 = y.GetById(item.Id);
                if (!GenericComparer<T>.Default.Equals(item, item2)) return false;

                // Removes the first occurrence, so if there are duplicates we'll still get a valid mismatch
                source.Remove(item);
            }

            // If there are any items left then fail
            if (source.Any()) return false;

            return true;
        }

        public override int GetHashCode(IReadOnlyObjectWithIdCollection<T, TId> obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            // IMPORTANT: The collection must be in the same order to produce the same hash
            return obj.OrderBy(p => p.Id)
                      .Aggregate(27, (current, node) => (13 * current) ^ GenericComparer<T>.Default.GetHashCode(node));
        }
    }
}