using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Microsoft.Qwiq.Mocks;

namespace Microsoft.Qwiq.Benchmark
{
    public class WorkItemGenerator<T>
        where T : IWorkItem
    {
        private readonly HashSet<string> _propertiesToSkip;

        private readonly Func<T> _create;

        private readonly string[] _assignees
            ;

        public WorkItemGenerator(Func<T> createFunc, IEnumerable<string> propertiesToSkip = null)
        {
            _create = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _propertiesToSkip = propertiesToSkip == null
                                    ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                                    : new HashSet<string>(propertiesToSkip, StringComparer.OrdinalIgnoreCase);

            // Add properties that have special cases
            _propertiesToSkip.Add("Id");

            // Calculated fields
            _propertiesToSkip.Add("HyperLinkCount");
            _propertiesToSkip.Add("RelatedLinkCount");
            _propertiesToSkip.Add("ExternalLinkCount");
            _propertiesToSkip.Add("AttachedFileCount");

            _propertiesToSkip.Add("Type");
            _propertiesToSkip.Add("WorkItemType");

            _propertiesToSkip.Add("TeamProject");

            //
            _propertiesToSkip.Add("IsDirty");
            _propertiesToSkip.Add("Rev");
            _propertiesToSkip.Add("Revision");
            _propertiesToSkip.Add("RevisedDate");
            _propertiesToSkip.Add("History");
            _propertiesToSkip.Add("Watermark");

            //// Identity fields
            //_propertiesToSkip.Add("AssignedTo");
            //_propertiesToSkip.Add("ChangedBy");
            //_propertiesToSkip.Add("CreatedBy");

            _assignees = new[]
                                {
                                    MockIdentityManagementService.Danj.DisplayName,
                                    MockIdentityManagementService.Adamb.DisplayName,
                                    MockIdentityManagementService.Chrisj.DisplayName,
                                    MockIdentityManagementService.Chrisjoh.DisplayName,
                                    MockIdentityManagementService.Chrisjohn.DisplayName,
                                    MockIdentityManagementService.Chrisjohns.DisplayName
                                };
        }

        public IReadOnlyCollection<T> Generate(int quantity = 50)
        {
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
            PopulatePropertiesOnInstance(instance);
            return instance;
        }

        public IReadOnlyCollection<T> Items { get; private set; }

        private void PopulatePropertiesOnInstance(T instance)
        {
            foreach (
                var property in
                    typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty))
            {
                // If we can't set the property, don't bother
                if (property.GetSetMethod() == null) continue;

                // If we should skip the source property
                if (_propertiesToSkip.Contains(property.Name)) continue;

                var value = GetRandomValue(instance, property.Name, property.PropertyType);
                try
                {
                    property.SetValue(instance, value);
                }
                catch (TargetParameterCountException)
                {
                    // Best effort
                    // May fail with index properties
                }
                catch (ArgumentException)
                {
                    // Best effort
                    // May fail because the setter is not available
                }
            }
        }

        protected const string Chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";

        protected virtual object GetRandomValue(T instance, string propertyName, Type propertyType)
        {

            var randomizer = Randomizer.Instance;

            object value;
            switch (propertyType.ToString())
            {
                case "System.Int32":
                    value = Randomizer.Instance.NextSystemId();
                    break;
                case "System.Nullable`1[System.Int32]":
                    if (randomizer.ShouldEnter())
                    {
                        value = randomizer.NextSystemId();
                    }
                    else
                    {
                        value = null;
                    }
                    break;
                case "System.String":
                    if (StringComparer.OrdinalIgnoreCase.Equals("AssignedTo", propertyName)
                        || StringComparer.OrdinalIgnoreCase.Equals("ChangedBy", propertyName)
                        || StringComparer.OrdinalIgnoreCase.Equals("CreatedBy", propertyName))
                    {
                        var i = Randomizer.Instance.Next(0, _assignees.Length - 1);
                        value = _assignees[i];
                    }
                    else
                    {
                        value = new string(
                                           Enumerable.Repeat(Chars, randomizer.Next(5, 250))
                                                     .Select(s => s[randomizer.Next(s.Length)])
                                                     .ToArray());
                    }
                    break;
                case "System.Nullable`1[System.DateTime]":
                    if (!Randomizer.Instance.ShouldEnter())
                    {
                        value = null;
                    }
                    else
                    {
                        var range2 = DateTime.MaxValue - DateTime.MinValue;
                        var randTimeSpan2 = new TimeSpan((long)(randomizer.NextDouble() * range2.Ticks));
                        value = DateTime.MinValue + randTimeSpan2;
                    }

                    break;
                case "System.DateTime":
                    var range = DateTime.MaxValue - DateTime.MinValue;
                    var randTimeSpan = new TimeSpan((long)(randomizer.NextDouble() * range.Ticks));
                    value = DateTime.MinValue + randTimeSpan;
                    break;

                case "System.Collections.Generic.ICollection`1[Microsoft.Qwiq.ILink]":
                    var retval = new List<ILink>();
                    value = retval;
                    break;

                case "System.Uri":
                    var c = "ABCDEFGHIJKLMNOPQRSTUVWXYZ/";
                    value =
                        new Uri(
                            "http://tempuri.org/"
                            + new string(
                                  Enumerable.Repeat(c, randomizer.Next(5, 250))
                                            .Select(s => s[randomizer.Next(s.Length)])
                                            .ToArray()));
                    break;

                case "Microsoft.Qwiq.IWorkItemType":
                    value = new MockWorkItemType("Baz");
                    break;

                case "System.Object":
                    value = new object();
                    break;

                default:
                    Trace.TraceInformation($"{propertyType.ToString()} is an unrecognized type. Create new instance of System.Object");
                    value = new object();
                    break;
            }
            return value;
        }
    }
}
