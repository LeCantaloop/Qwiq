using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Mapper.Attributes;
using Microsoft.IE.Qwiq.Mapper.Tests.Mocks;
using Microsoft.IE.Qwiq.Mocks;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace Microsoft.IE.Qwiq.Mapper.Tests
{
    // POCO serialization pulling static typed objects
    // When running benchmarks, be sure to compile in Release and not attach a debugger
    public sealed class WorkItemGenerator<T> where T : IWorkItem
    {
        private readonly HashSet<string> _propertiesToSkip;

        public WorkItemGenerator(Func<T> create, IEnumerable<string> propertiesToSkip )
        {
            _propertiesToSkip = new HashSet<string>(propertiesToSkip, StringComparer.OrdinalIgnoreCase);
            Items = GenerateWorkItems(create);
        }

        public IList<IWorkItem> Items { get; private set; }

        private IList<IWorkItem> GenerateWorkItems(Func<T> create)
        {
            // Create objects to map
            const int ItemCount = 500;
            var items = new List<IWorkItem>(ItemCount);
            var generatedItems = new HashSet<int>();

            for (var i = 0; i < ItemCount; i++)
            {
                var instance = create();
                PopulatePropertiesOnInstance(instance);

                if (Randomizer.ShouldEnter())
                {
                    instance["Custom String 01"] = Math.Abs(Randomizer.NextDecimal()).ToString("F2");
                }

                if (!generatedItems.Contains(instance.Id))
                {
                    generatedItems.Add(instance.Id);
                    items.Add(instance);
                }


                foreach (var link in instance.Links.OfType<IRelatedLink>())
                {
                    if (!generatedItems.Contains(link.RelatedWorkItemId))
                    {
                        instance = create();
                        PopulatePropertiesOnInstance(instance);
                        instance["Id"] = link.RelatedWorkItemId;
                        items.Add(instance);
                        generatedItems.Add(instance.Id);
                    }
                }
            }

            return items;
        }

        private void PopulatePropertiesOnInstance(T instance)
        {
            foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty))
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

        private static object GetRandomValue(Type propertyType)
        {
            const string Chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
            var randomizer = Randomizer.Instance;

            object value;
            switch (propertyType.ToString())
            {
                case "System.Int32":
                    value = randomizer.Next();
                    break;
                case "System.String":
                    value = new string(Enumerable.Repeat(Chars, randomizer.Next(5, 250)).Select(s => s[randomizer.Next(s.Length)]).ToArray());
                    break;
                case "System.DateTime":
                    var range = DateTime.MaxValue - DateTime.MinValue;
                    var randTimeSpan = new TimeSpan((long)(randomizer.NextDouble() * range.Ticks));
                    value = DateTime.MinValue + randTimeSpan;
                    break;
                case "System.Collections.Generic.ICollection`1[Microsoft.IE.Qwiq.ILink]":
                    var retval = new List<ILink>();

                    if (Randomizer.ShouldEnter())
                    {
                        for (var i = 0; i < randomizer.Next(0, 10); i++)
                        {
                            retval.Add(
                                new MockWorkItemLink()
                                    {
                                        LinkTypeEnd =
                                            new MockWorkItemLinkTypeEnd(
                                            MockModel.ReverseLinkName,
                                            "Giver"),
                                        RelatedWorkItemId = randomizer.Next(1, 36)
                                    });
                        }
                    }

                    if (Randomizer.ShouldEnter())
                    {
                        for (var i = 0; i < randomizer.Next(0, 10); i++)
                        {
                            retval.Add(
                                new MockWorkItemLink()
                                    {
                                        LinkTypeEnd =
                                            new MockWorkItemLinkTypeEnd(
                                            MockModel.ForwardLinkName,
                                            "Taker"),
                                        RelatedWorkItemId = randomizer.Next(1, 36)
                                    });
                        }
                    }

                    value = retval;

                    break;
                case "System.Uri":
                    var c = "ABCDEFGHIJKLMNOPQRSTUVWXYZ/";
                    value = new Uri("http://tempuri.org/" + new string(Enumerable.Repeat(c, randomizer.Next(5, 250)).Select(s => s[randomizer.Next(s.Length)]).ToArray()));
                    break;
                case "Microsoft.IE.Qwiq.IWorkItemType":
                    value = new MockWorkItemType("Baz");
                    break;
                case "System.Object":
                    value = new object();
                    break;
                default:
                    Trace.TraceInformation(propertyType.ToString());
                    value = new object();
                    break;
            }
            return value;
        }

        private class Randomizer : Random
        {
            private static Randomizer random;

            public static Randomizer Instance => random ?? (random = new Randomizer());

            public static bool ShouldEnter()
            {
                return Instance.NextDouble() < 0.5;
            }


            public static int NextInt32()
            {
                unchecked
                {
                    var firstBits = Instance.Next(0, 1 << 4) << 28;
                    var lastBits = Instance.Next(0, 1 << 28);
                    return firstBits | lastBits;
                }
            }

            /// <summary>
            /// Taken from http://stackoverflow.com/questions/609501/generating-a-random-decimal-in-c-sharp Jon Skeet's answer
            /// </summary>
            public static decimal NextDecimal()
            {
                var scale = (byte)Instance.Next(29);
                var sign = Instance.Next(2) == 1;
                return new decimal(NextInt32(), NextInt32(), NextInt32(), sign, scale);
            }
        }
    }

    [WorkItemType("Baz")]
    public class MockModel : IIdentifiable
    {
        private string _milestone;

        private string _title;

        private string _assignedTo;

        private string _status;

        private string _resolution;

        private string _keywords;

        private string _openedBy;

        private string _treePath;

        private string _closedBy;

        private string _tags;

        private string _history;

        private string _issueType;

        private string _productFamily;

        private string _product;

        private string _release;

        private string _releaseType;

        public const string ReverseLinkName = "NS.SampleLink-Reverse";
        public const string ForwardLinkName = "NS.SampleLink-Forward";

        private IEnumerable<MockModel> _givers;
        private IEnumerable<MockModel> _takers;

        [WorkItemLink(typeof(SubMockModel), ReverseLinkName)]
        public IEnumerable<MockModel> Givers
        {
            get { return (_givers ?? Enumerable.Empty<MockModel>()); }
            internal set { _givers = value; }
        }

        [WorkItemLink(typeof(MockModel), ForwardLinkName)]
        public IEnumerable<MockModel> Takers
        {
            get { return (_takers ?? Enumerable.Empty<MockModel>()); }
            internal set { _takers = value; }
        }

        [JsonIgnore]
        public int Id
        {
            get
            {
                return ID;
            }
            set
            {
                ID = value;
            }
        }

        [FieldDefinition("Id")]
        public virtual int ID { get; set; }

        /// <summary>
        /// The title of the issue.
        /// </summary>
        [FieldDefinition("Title")]
        public virtual string Title
        {
            get
            {
                return _title ?? string.Empty;
            }
            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// The priority of the issue (e.g. 1, 2, etc.).
        /// </summary>
        [FieldDefinition("Priority")]
        public virtual int? Priority { get; set; }

        /// <summary>
        /// The alias of the user to which this issue is assigned.
        /// </summary>
        [FieldDefinition("Assigned To")]

        public virtual string AssignedTo
        {
            get
            {
                return _assignedTo ?? string.Empty;
            }
            set
            {
                _assignedTo = value;
            }
        }

        /// <summary>
        /// The string of this issue's status (e.g. 'Active', 'Closed - Completed', etc.).
        /// </summary>
        [FieldDefinition("State")]
        public virtual string Status
        {
            get
            {
                return _status ?? string.Empty;
            }
            set
            {
                _status = value;
            }
        }

        /// <summary>
        /// The string of this issue's status (e.g. 'By Design', 'Submitted', 'Won't Fix', etc.).
        /// </summary>
        [FieldDefinition("Resolved Reason")]
        public virtual string Resolution
        {
            get
            {
                return _resolution ?? string.Empty;
            }
            set
            {
                _resolution = value;
            }
        }

        /// <summary>
        /// The string of this issue's milestone (e.g. 'M1 Coding', 'M1', etc.).
        /// </summary>
        [FieldDefinition("Iteration Path")]
        public virtual string Milestone
        {
            get
            {
                return _milestone ?? string.Empty;
            }
            set
            {
                _milestone = value;
            }
        }

        /// <summary>
        /// The DateTime when this issue was opened in the local timezone.
        /// </summary>
        [FieldDefinition("Created Date")]
        public virtual DateTime OpenedDate { get; set; }

        /// <summary>
        /// The user that opened this issue.
        /// </summary>
        [FieldDefinition("Created By")]

        public virtual string OpenedBy
        {
            get
            {
                return _openedBy ?? string.Empty;
            }
            set
            {
                _openedBy = value;
            }
        }

        /// <summary>
        /// The issue's keywords text field. If there are no keywords it returns an empty string.
        /// </summary>
        [FieldDefinition("Keywords")]
        public virtual string KeyWords
        {
            get
            {
                return _keywords ?? string.Empty;
            }
            set
            {
                _keywords = value;
            }
        }

        /// <summary>
        /// Used for "As Of" queries. For each revision of an issue, the ChangedDate is when the issue was modified
        /// (or opened for the first revision), and that bug is considered current until <see cref="RevisedDate"/>.
        /// If the revision is the lastest revision, the revised date is 9999/01/01 to avoid NULL values.
        /// </summary>
        [FieldDefinition("Changed Date")]
        public virtual DateTime ChangedDate { get; set; }

        /// <summary>
        /// The DateTime when this issue was last modified.
        /// </summary>
        [FieldDefinition("Revised Date")]
        public virtual DateTime RevisedDate { get; set; }

        /// <summary>
        /// The 'tree path' or 'area path' of the issue (e.g. \IE-Internet Explorer\COMP-Composition and Rendering\).
        /// </summary>
        [FieldDefinition("Area Path")]
        public virtual string TreePath
        {
            get
            {
                return _treePath ?? string.Empty;
            }
            set
            {
                _treePath = value;
            }
        }

        /// <summary>
        /// Type of issue this object represents (e.g. "Code Bug", "Dev Task", "Spec Bug", "Buffer", etc.)
        /// </summary>
        [FieldDefinition("Issue Type")]
        public virtual string IssueType
        {
            get
            {
                return _issueType ?? string.Empty;
            }
            set
            {
                _issueType = value;
            }
        }

        [FieldDefinition("Work Item Type")]
        public virtual string WorkItemType { get; set; }

        /// <summary>
        /// The alias of the user that closed this issue.
        /// </summary>
        [FieldDefinition("Closed By")]
        public virtual string ClosedBy
        {
            get
            {
                return _closedBy ?? string.Empty;
            }
            set
            {
                _closedBy = value;
            }
        }

        /// <summary>
        /// The DateTime when this issue was closed in the local timezone. Null if the issue is still open.
        /// </summary>
        [FieldDefinition("Closed Date")]
        public virtual DateTime? ClosedDate { get; set; }

        [FieldDefinition("Product Family")]
        public virtual string ProductFamily
        {
            get
            {
                return _productFamily ?? string.Empty;
            }
            set
            {
                _productFamily = value;
            }
        }

        [FieldDefinition("Product")]
        public virtual string Product
        {
            get
            {
                return _product ?? string.Empty;
            }
            set
            {
                _product = value;
            }
        }

        [FieldDefinition("Release")]
        public virtual string Release
        {
            get
            {
                return _release ?? string.Empty;
            }
            set
            {
                _release = value;
            }
        }

        [FieldDefinition("Tags")]
        public virtual string Tags
        {
            get
            {
                return _tags ?? string.Empty;
            }
            set
            {
                _tags = value;
            }
        }

        [FieldDefinition("Release Type")]
        public virtual string ReleaseType
        {
            get
            {
                return _releaseType ?? string.Empty;
            }
            set
            {
                _releaseType = value;
            }
        }

        [FieldDefinition("History")]
        public virtual string History
        {
            get
            {
                return _history ?? string.Empty;
            }
            set
            {
                _history = value;
            }
        }

        [FieldDefinition("Custom String 01", true)]
        public double Effort { get; set; }
    }


    [WorkItemType("Baz")]
    public class SubMockModel : MockModel
    {
    }



    [TestClass]
    public class BenchmarkRunnerMappingContext : ContextSpecification
    {
        public override void When()
        {
            BenchmarkRunner.Run<Benchmark>(
                ManualConfig
                    .Create(DefaultConfig.Instance)
                    .With(Job.Clr)
                    .With(Job.Core)
                    .With(Job.AllJits)
                    .With(Job.ConcurrentServerGC)
            );
        }


        [TestMethod]
        [TestCategory("Performance")]
        public void Execute_Mapping_Performance_Benchmark()
        {
            // Intentionally left blank
        }

        public class Benchmark
        {
            private WorkItemMapper _mapper;
            private IEnumerable<IWorkItem> _items;

            [Setup]
            public void SetupData()
            {
                var propertyInspector = new PropertyInspector(new PropertyReflector());
                var typeParser = new TypeParser();
                var mappingStrategies = new IWorkItemMapperStrategy[]
                                            { new AttributeMapperStrategy(propertyInspector, typeParser) };
                _mapper = new WorkItemMapper(mappingStrategies);

                var b = new WorkItemGenerator<MockWorkItem>(() => new MockWorkItem("Baz"), new[] { "Revisions", "Item" });
                _items = b.Items;
            }

            [Benchmark]
            public IList Execute()
            {
                return _mapper.Create<MockModel>(_items).ToList();
            }
        }
    }

    [TestClass]
    public class MapperContext : BenchmarkRunnerMappingContext
    {
        private Benchmark _benchmark;

        public override void Given()
        {
            _benchmark = new Benchmark();
            _benchmark.SetupData();
        }

        public override void When()
        {
            _benchmark.Execute();
        }
    }

    [TestClass]
    public class BenchmarkRunnerLinksMappingContext : ContextSpecification
    {
        public override void When()
        {
            BenchmarkRunner.Run<Benchmark>(
                ManualConfig
                    .Create(DefaultConfig.Instance)
                    .With(Job.Clr)
                    .With(Job.Core)
                    .With(Job.AllJits)
                    .With(Job.ConcurrentServerGC)
            );
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void Execute_Links_Performance_Benchmark()
        {
            // Intentionally left blank
        }

        public class Benchmark
        {
            public WorkItemMapper Mapper { get; private set; }

            private IEnumerable<IWorkItem> _items;

            public IEnumerable<IWorkItem> MappingItems { get; private set; }

            public bool SimulateQueryTimes { get; set; }

            [Setup]
            public void SetupData()
            {
                var b = new WorkItemGenerator<MockWorkItem>(() => new MockWorkItem("Baz"), new[] { "Revisions", "Item" });
                _items = b.Items;
                var propertyInspector = new PropertyInspector(new PropertyReflector());
                var typeParser = new TypeParser();
                var mappingStrategies = new IWorkItemMapperStrategy[]
                                            {
                                                new AttributeMapperStrategy(propertyInspector, typeParser),
                                                new WorkItemLinksMapperStrategy(propertyInspector, new MockWorkItemStore(_items) {SimulateQueryTimes = SimulateQueryTimes}),
                                            };
                Mapper = new WorkItemMapper(mappingStrategies);
                MappingItems = _items.Take(500).ToList();
            }

            [Benchmark]
            public IList Execute()
            {
                return Mapper.Create<MockModel>(MappingItems).ToList();
            }
        }
    }

    [TestClass]
    public class LinkMapperContext : BenchmarkRunnerLinksMappingContext
    {
        private Benchmark _benchmark;

        public override void Given()
        {
            _benchmark = new Benchmark();
            _benchmark.SetupData();
        }

        public override void When()
        {
            _benchmark.Mapper.Create<MockModel>(_benchmark.MappingItems).ToList();
        }
    }
}
