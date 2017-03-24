using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Qwiq.Core.Tests;
using Should;
using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using Qwiq.Benchmark;

namespace Microsoft.Qwiq.Mapper.Tests
{
    // POCO serialization pulling static typed objects
    // When running benchmarks, be sure to compile in Release and not attach a debugger


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
            BenchmarkRunner.Run<Benchmark>();
        }


        [TestMethod]
        [TestCategory("Performance")]
        [TestCategory("localOnly")]
        public void Execute_Mapping_Performance_Benchmark()
        {
            // Intentionally left blank
        }

        [Config(typeof(BenchmarkConfig))]
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

                var generator = new WorkItemGenerator<MockWorkItem>(() => new MockWorkItem("Baz"), new[] { "Revisions", "Item" });
                _items = generator.Generate();
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
            BenchmarkRunner.Run<Benchmark>();
        }

        [TestMethod]
        [TestCategory("Performance")]
        [TestCategory("localOnly")]
        [TestCategory("Benchmark")]
        public void Execute_Links_Performance_Benchmark()
        {
            // Intentionally left blank
        }

        [Config(typeof(BenchmarkConfig))]
        public class Benchmark
        {
            public WorkItemMapper Mapper { get; private set; }

            private IEnumerable<IWorkItem> _items;

            public IEnumerable<IWorkItem> MappingItems { get; private set; }

            [Setup]
            public void SetupData()
            {
                var generator = new WorkItemLinkGenerator(() => new MockWorkItem("Baz"), new[] { "Revisions", "Item" });
                _items = generator.Generate();
                var propertyInspector = new PropertyInspector(new PropertyReflector());
                var typeParser = new TypeParser();
                var mappingStrategies = new IWorkItemMapperStrategy[]
                                            {
                                                new AttributeMapperStrategy(propertyInspector, typeParser),
                                                new WorkItemLinksMapperStrategy(propertyInspector, new MockWorkItemStore(_items)),
                                            };
                Mapper = new WorkItemMapper(mappingStrategies);
                MappingItems = _items.Take(500).ToList();
            }

            [Benchmark]
            public IList Execute()
            {
                return Mapper.Create<MockModel>(MappingItems).ToList();
            }

            private class WorkItemLinkGenerator : WorkItemGenerator<MockWorkItem>
            {
                public WorkItemLinkGenerator(Func<MockWorkItem> create, IEnumerable<string> propertiesToSkip)
                    : base(create, propertiesToSkip)
                {
                }

                protected override object GetRandomValue(Type propertyType)
                {
                    switch (propertyType.ToString())
                    {
                        case "System.Collections.Generic.ICollection`1[Microsoft.Qwiq.ILink]":
                            var retval = new List<ILink>();

                            if (Randomizer.ShouldEnter())
                            {
                                for (var i = 0; i < Randomizer.Instance.Next(0, 10); i++)
                                {
                                    retval.Add(
                                        new MockWorkItemLink {
                                            LinkTypeEnd =
                                                    new MockWorkItemLinkTypeEnd("Giver"),
                                            RelatedWorkItemId = Randomizer.Instance.Next(1, 36)
                                        });
                                }
                            }

                            if (Randomizer.ShouldEnter())
                            {
                                for (var i = 0; i < Randomizer.Instance.Next(0, 10); i++)
                                {
                                    retval.Add(
                                        new MockWorkItemLink {
                                            LinkTypeEnd =
                                                    new MockWorkItemLinkTypeEnd("Taker"),
                                            RelatedWorkItemId = Randomizer.Instance.Next(1, 36)
                                        });
                                }
                            }

                            return retval;


                        default:
                            return base.GetRandomValue(propertyType);
                    }

                }
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

