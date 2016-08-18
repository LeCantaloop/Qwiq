using Microsoft.IE.Qwiq;
using Microsoft.IE.Qwiq.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Qwiq.Benchmark
{
    public class WorkItemGenerator<T>
        where T : IWorkItem
    {
        private readonly HashSet<string> _propertiesToSkip;

        private readonly Func<T> _create;

        public WorkItemGenerator(Func<T> create, IEnumerable<string> propertiesToSkip)
        {
            if (create == null) throw new ArgumentNullException(nameof(create));
            if (propertiesToSkip == null) throw new ArgumentNullException(nameof(propertiesToSkip));

            _propertiesToSkip = new HashSet<string>(propertiesToSkip, StringComparer.OrdinalIgnoreCase);
            _create = create;
        }

        public IList<IWorkItem> Generate(int quantity = 500)
        {
            Items = new List<IWorkItem>(quantity);
            var generatedItems = new HashSet<int>();

            for (var i = 0; i < quantity; i++)
            {
                var instance = _create();
                PopulatePropertiesOnInstance(instance);

                if (Randomizer.ShouldEnter())
                {
                    instance["Custom String 01"] = Math.Abs(Randomizer.NextDecimal()).ToString("F2");
                }

                if (!generatedItems.Contains(instance.Id))
                {
                    generatedItems.Add(instance.Id);
                    Items.Add(instance);
                }

                foreach (var link in instance.Links.OfType<IRelatedLink>())
                {
                    if (!generatedItems.Contains(link.RelatedWorkItemId))
                    {
                        instance = _create();
                        PopulatePropertiesOnInstance(instance);
                        instance["Id"] = link.RelatedWorkItemId;
                        Items.Add(instance);
                        generatedItems.Add(instance.Id);
                    }
                }
            }

            return Items;
        }

        public IList<IWorkItem> Items { get; private set; }

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

                var value = GetRandomValue(property.PropertyType);
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

        protected virtual object GetRandomValue(Type propertyType)
        {

            var randomizer = Randomizer.Instance;

            object value;
            switch (propertyType.ToString())
            {
                case "System.Int32":
                    value = Randomizer.NextInt32();
                    break;

                case "System.String":
                    value =
                        new string(
                            Enumerable.Repeat(Chars, randomizer.Next(5, 250))
                                      .Select(s => s[randomizer.Next(s.Length)])
                                      .ToArray());
                    break;

                case "System.DateTime":
                    var range = DateTime.MaxValue - DateTime.MinValue;
                    var randTimeSpan = new TimeSpan((long)(randomizer.NextDouble() * range.Ticks));
                    value = DateTime.MinValue + randTimeSpan;
                    break;

                case "System.Collections.Generic.ICollection`1[Microsoft.IE.Qwiq.ILink]":
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

                case "Microsoft.IE.Qwiq.IWorkItemType":
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