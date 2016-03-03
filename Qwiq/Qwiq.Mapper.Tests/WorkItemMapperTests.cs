using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Mapper.Attributes;
using Microsoft.IE.Qwiq.Mapper.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.IE.Qwiq.Mapper.Tests
{
    public abstract class WorkItemMapperContext<T> : ContextSpecification where T : new()
    {
        protected readonly Dictionary<string, object> WorkItemBackingStore = new Dictionary<string, object>
            {
                { "DateTimeField" ,  new DateTime(2014, 1, 1) },
                { "Field with Spaces" ,  "7" },
                { "Id" ,  7 },
                { "IntField" ,  1 },
                { "Issue Type" ,  "Code Bug" },
                { "FieldWithDifferentName" ,  "forty-two" },
                { "NullableField" ,  null },
                { "StringField" ,  "sample" }
            };

        protected IWorkItemStore WorkItemStore;
        private IWorkItemMapper _workItemMapper;
        protected IEnumerable<IWorkItem> SourceWorkItems;
        protected T Actual;

        public override void Given()
        {
            var propertyInspector = new PropertyInspector(new PropertyReflector());
            var typeParser = new TypeParser();
            var mappingStrategies = new IWorkItemMapperStrategy[]
            {
                new AttributeMapperStrategy(propertyInspector, typeParser),
                new WorkItemLinksMapperStrategy(propertyInspector, WorkItemStore)
            };
            _workItemMapper = new WorkItemMapper(mappingStrategies);
        }

        public override void When()
        {
            Actual = _workItemMapper.Create<T>(SourceWorkItems).SingleOrDefault();
        }

        public override void Cleanup()
        {
            WorkItemStore.Dispose();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_the_issue_factory_parses_an_issue_with_links : WorkItemMapperContext<MockModelWithLinks>
    {
        public override void Given()
        {
            var giverWorkItemBackingStore = new Dictionary<string, object>(WorkItemBackingStore);
            giverWorkItemBackingStore["Id"] = 233;

            var takerWorkItemBackingStore = new Dictionary<string, object>(WorkItemBackingStore);
            takerWorkItemBackingStore["Id"] = 144;

            WorkItemStore = new MockWorkItemStore(new[]
            {
                new MockWorkItem
                {
                    Id = 233,
                    Properties = giverWorkItemBackingStore,
                    Type = new MockWorkItemType {Name = "Baz"},
                    WorkItemLinks = new MockLinkCollection()
                },
                new MockWorkItem
                {
                    Id = 144,
                    Properties = takerWorkItemBackingStore,
                    Type = new MockWorkItemType {Name = "Baz"},
                    WorkItemLinks = new MockLinkCollection()
                }
            });

            var links = new IWorkItemLink[]
            {
                new MockWorkItemLink
                {
                    LinkTypeEnd = new MockWorkItemLinkTypeEnd {ImmutableName = MockModelWithLinks.ForwardLinkName},
                    TargetId = 233
                },
                new MockWorkItemLink
                {
                    LinkTypeEnd = new MockWorkItemLinkTypeEnd {ImmutableName = MockModelWithLinks.ReverseLinkName},
                    TargetId = 144
                }
            };

            SourceWorkItems = new IWorkItem[]
            {
                new MockWorkItem
                {
                    Properties = WorkItemBackingStore,
                    Type = new MockWorkItemType { Name = "Baz" },
                    WorkItemLinks = new MockLinkCollection
                    {
                        Count = 2,
                        Links = links,
                    }
                }
            };

            base.Given();
        }

        [TestMethod]
        public void issue_has_givers()
        {
            Actual.Givers.Any().ShouldBeTrue();
        }

        [TestMethod]
        public void issue_has_one_giver()
        {
            Actual.Givers.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void issue_has_takers()
        {
            Actual.Takers.Any().ShouldBeTrue();
        }

        [TestMethod]
        public void issue_has_one_taker()
        {
            Actual.Takers.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void issue_giver_is_expected_id()
        {
            Actual.Givers.Single().Id.ShouldEqual(144);
        }

        [TestMethod]
        public void issue_taker_is_expected_id()
        {
            Actual.Takers.Single().Id.ShouldEqual(233);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_the_issue_factory_parses_an_issue_without_links : WorkItemMapperContext<MockModelSubclass>
    {
        private MockModelSubclass _expected;

        public override void Given()
        {
            WorkItemStore = new MockWorkItemStore(Enumerable.Empty<IWorkItem>());

            SourceWorkItems = new[]
            {
                new MockWorkItem
                {
                    Properties = WorkItemBackingStore,
                    Type = new MockWorkItemType {Name = "Baz"},
                    WorkItemLinks = new MockLinkCollection()
                }
            };

            _expected = new MockModelSubclass
            {
                DateTimeField = new DateTime(2014, 1, 1),
                FieldWithSpaces = "7",
                Id = 7,
                IntField = 1,
                NotTheSameName = "forty-two",
                NullableField = null,
            };
            base.Given();
        }

        [TestMethod]
        public void the_fields_are_translated_according_to_the_attribute()
        {
            PropertiesAreEqual(_expected, Actual);
        }

        private static void PropertiesAreEqual<T>(T expected, T actual)
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var expectedVal = property.GetValue(expected);
                var actualVal = property.GetValue(actual);

                var val = expectedVal as ICollection;
                if (val != null)
                {
                    CollectionAssert.AreEqual(val, (ICollection)actualVal);
                }
                else
                {
                    actualVal.ShouldEqual(expectedVal);
                }
            }
        }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_an_issue_is_mapped_without_a_workitemtype : WorkItemMapperContext<MockModelWithNoBacking>
    {
        private MockModelWithNoBacking _expected;

        public override void Given()
        {
            WorkItemStore = new MockWorkItemStore(Enumerable.Empty<IWorkItem>());

            SourceWorkItems = new[]
            {
                    new MockWorkItem
                    {
                        Properties = WorkItemBackingStore,
                        Type = new MockWorkItemType {Name = "Baz"}
                    }
                };

            _expected = new MockModelWithNoBacking
            {
                Id = int.Parse(WorkItemBackingStore["Id"].ToString())
            };
            base.Given();
        }

        [TestMethod]
        public void the_Id_field_is_populated_correctly()
        {
            _expected.Id.ShouldEqual(Actual.Id);
        }

        [TestMethod]
        public void the_FieldWithNoBackingStore_field_is_null()
        {
            _expected.FieldWithNoBackingStore.ShouldBeNull();
        }
    }
}