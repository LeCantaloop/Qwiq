using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwiq.Mocks
{
    public class WorkItemGenerator<T>
        where T : IWorkItem
    {
        private readonly Func<T> _create;

        private readonly PropertyValueGenerator<T> _propertyGenerator;

        public WorkItemGenerator(Func<T> createFunc)
            : this(createFunc, null)
        {
        }

        public WorkItemGenerator(Func<T> createFunc, IEnumerable<string> propertiesToSkip = null)
        {
            _create = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _propertyGenerator = new PropertyValueGenerator<T>(propertiesToSkip);
        }

        public System.Collections.Generic.IReadOnlyCollection<T> Generate(int quantity = 50)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            // After generating the parent/child links, this can grow an order of magnitude
            var items = new List<T>(quantity * 10);
            var generatedItems = new HashSet<int>();

            int GenerateUnusedWorkItemId()
            {
                // ID needs to be populated prior to other properties (as they may depend on that value)
                var id = Randomizer.Instance.NextSystemId(quantity);
                while (generatedItems.Contains(id))
                {
                    id = Randomizer.Instance.NextSystemId(quantity * 10);
                }

                return id;
            }

            for (var i = 0; i < quantity; i++)
            {
                GenerateItem(_create, GenerateUnusedWorkItemId, generatedItems, items);
            }

            Items = items.AsReadOnly();
            return Items;
        }

        // Generates an item and link references
        private T GenerateItem(Func<T> createFunc, Func<int> idFunc, ISet<int> generatedItems, ICollection<T> items)
        {
            var instance = GenerateItem(createFunc, idFunc);

            if (generatedItems.Contains(instance.Id))
            {
                // Item has already been generated
                var id = instance.Id;
                return items.Single(p => p.Id == id);
            }


            items.Add(instance);
            generatedItems.Add(instance.Id);

            foreach (var link in instance.Links.OfType<IRelatedLink>().ToArray())
            {
                var linked = default(T);

                if (!generatedItems.Contains(link.RelatedWorkItemId))
                {
                    linked = GenerateItem(createFunc, () => link.RelatedWorkItemId, generatedItems, items);
                }

                // Determine if we need to create a recipricol link
                if (!(link.LinkTypeEnd?.LinkType.IsDirectional ?? false)) continue;

                // Look up the item if it was not previously generated
                if (linked == null)
                {
                    linked = items.Single(p => p.Id == link.RelatedWorkItemId);
                }

                // Add the recipricol link
                linked.Links.Add(linked.CreateRelatedLink(instance.Id, link.LinkTypeEnd.OppositeEnd));
            }

            return instance;
        }

        /// Generates a single item
        private T GenerateItem(Func<T> createFunc, Func<int> idFunc)
        {
            var instance = createFunc();
            var id = idFunc();
            instance[CoreFieldRefNames.Id] = id;
            return _propertyGenerator.PopulateInstance(instance);
        }

        public System.Collections.Generic.IReadOnlyCollection<T> Items { get; private set; }

        protected const string Chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";

        protected virtual object GetRandomValue(T instance, string propertyName, Type propertyType)
        {
            return _propertyGenerator.GetRandomValue(instance, propertyName, propertyType);

        }
    }
}
