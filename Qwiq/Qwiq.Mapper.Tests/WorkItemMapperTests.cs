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
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_the_issue_factory_parses_an_issue_with_links : ContextSpecification
    {
        private MockModelWithLinks _dependency;
        private IEnumerable<IWorkItem> _source;
        private IWorkItemStore _workItemStore;
        private IWorkItemMapper _mapper;

        public override void Cleanup()
        {
            _workItemStore.Dispose();
        }

        public override void Given()
        {
            var fakeWorkItemBackingStore = new Dictionary<string, object>
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

            var giverWorkItemBackingStore = new Dictionary<string, object>(fakeWorkItemBackingStore);
            giverWorkItemBackingStore["Id"] = 233;

            var takerWorkItemBackingStore = new Dictionary<string, object>(fakeWorkItemBackingStore);
            takerWorkItemBackingStore["Id"] = 144;

            _workItemStore = new MockWorkItemStore(new[]
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

            _source = new IWorkItem[]
            {
                new MockWorkItem
                {
                    Properties = fakeWorkItemBackingStore,
                    Type = new MockWorkItemType { Name = "Baz" },
                    WorkItemLinks = new MockLinkCollection
                    {
                        Count = 2,
                        Links = links,
                    }
                }
            };

            var propertyInspector = new PropertyInspector(new PropertyReflector());
            var typeParser = new TypeParser();
            var mappingStrategies = new IWorkItemMapperStrategy[]
            {
                new AttributeMapperStrategy(propertyInspector, typeParser),
                new WorkItemLinksMapperStrategy(propertyInspector, _workItemStore)
            };
            _mapper = new WorkItemMapper(new CachingFieldMapper(new FieldMapper()), mappingStrategies);
        }
        public override void When()
        {
            _dependency = _mapper.Create<MockModelWithLinks>(_source).SingleOrDefault();
        }

        [TestMethod]
        public void issue_has_givers()
        {
            _dependency.Givers.Any().ShouldBeTrue();
        }

        [TestMethod]
        public void issue_has_one_giver()
        {
            _dependency.Givers.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void issue_has_takers()
        {
            _dependency.Takers.Any().ShouldBeTrue();
        }

        [TestMethod]
        public void issue_has_one_taker()
        {
            _dependency.Takers.Count().ShouldEqual(1);
        }

        [TestMethod]
        public void issue_giver_is_expected_id()
        {
            _dependency.Givers.Single().Id.ShouldEqual(144);
        }

        [TestMethod]
        public void issue_taker_is_expected_id()
        {
            _dependency.Takers.Single().Id.ShouldEqual(233);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_the_issue_factory_parses_an_issue_without_links : ContextSpecification
    {
        private Dictionary<string, object> _fakeWorkItemBackingStore;
        private MockModelSubclass _expected;
        private MockModelSubclass _actual;
        private IWorkItemMapper _mapper;

        private IWorkItem _workItem;

        public override void Given()
        {
            _fakeWorkItemBackingStore = new Dictionary<string, object>
            {
                { "DateTimeField" ,  new DateTime(2014, 1, 1) },
                { "Field with Spaces" ,  "7" },
                { "Id" ,  7 },
                { "IntField" ,  1 },
                { "FieldWithDifferentName" ,  "forty-two" },
                { "NullableField" ,  null },
                { "StringField" ,  "sample" }
            };

            _workItem = new MockWorkItem
            {
                Properties = _fakeWorkItemBackingStore,
                Type = new MockWorkItemType { Name = "Baz" },
                WorkItemLinks = new MockLinkCollection()
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
            var propertyInspector = new PropertyInspector(new PropertyReflector());
            var typeParser = new TypeParser();
            var mappingStrategies = new IWorkItemMapperStrategy[]
            {
                new AttributeMapperStrategy(propertyInspector, typeParser),
                new WorkItemLinksMapperStrategy(propertyInspector, null)
            };
            _mapper = new WorkItemMapper(new CachingFieldMapper(new FieldMapper()), mappingStrategies);
        }

        public override void When()
        {
            _actual = _mapper.Create<MockModelSubclass>(new[] { _workItem }).SingleOrDefault();
        }

        [TestMethod]
        public void the_fields_are_translated_according_to_the_attribute()
        {
            PropertiesAreEqual(_expected, _actual);
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
    }
}
