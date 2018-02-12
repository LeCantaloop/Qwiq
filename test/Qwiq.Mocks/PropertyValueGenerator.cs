using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Qwiq.Mocks
{
    public class PropertyValueGenerator<T>
        where T : IWorkItem
    {
        private readonly string[] _assignees;
        private readonly HashSet<string> _propertiesToSkip;
        protected const string Chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";

        public PropertyValueGenerator()
            :this(null)
        {
        }

        public PropertyValueGenerator(IEnumerable<string> propertiesToSkip)
        {
            _assignees = new[]
                             {
                                 Identities.Danj.DisplayName,
                                 Identities.Adamb.DisplayName,
                                 Identities.Chrisj.DisplayName,
                                 Identities.Chrisjoh.DisplayName,
                                 Identities.Chrisjohn.DisplayName,
                                 Identities.Chrisjohns.DisplayName
                             };

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
        }

        public virtual T PopulateInstance(T instance)
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

            return instance;
        }

        public virtual object GetRandomValue(T instance, string propertyName, Type propertyType)
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
