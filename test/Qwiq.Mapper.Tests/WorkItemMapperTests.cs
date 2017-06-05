using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.Qwiq.Mapper.Attributes;
using Microsoft.Qwiq.Mapper.Mocks;
using Microsoft.Qwiq.Mocks;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Microsoft.Qwiq.Mapper
{
    public abstract class WorkItemMapperContext<T> : ContextSpecification
        where T : IIdentifiable<int?>, new()
    {
        protected readonly Dictionary<string, object> WorkItemBackingStore = new Dictionary<string, object>
        {
            {
                "DateTimeField",
                new DateTime(2014, 1, 1)
            },
            {
                "Field with Spaces",
                "7"
            },
            {
                "Id",
                7
            },
            {
                "IntField",
                1
            },
            {
                "Issue Type",
                "Code Bug"
            },
            {
                "FieldWithDifferentName",
                "forty-two"
            },
            {
                "NullableField",
                null
            },
            {
                "StringField",
                "sample"
            },
            {
                "EmptyStringField",
                string.Empty
            }
        };

        protected IWorkItemStore WorkItemStore;

        private IWorkItemMapper _workItemMapper;

        protected IEnumerable<IWorkItem> SourceWorkItems;

        protected T Actual;

        public override void Given()
        {
            var propertyInspector = new PropertyInspector(new PropertyReflector());
            var mappingStrategies = new IWorkItemMapperStrategy[]
                                        {
                                            new AttributeMapperStrategy(propertyInspector),
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
            WorkItemStore?.Dispose();
        }
    }

    [TestClass]
    public class Given_a_model_with_a_field_of_type_Double_mapped_from_StringEmpty_Requiring_Conversion : WorkItemMapperContext<Given_a_model_with_a_field_of_type_Double_mapped_from_StringEmpty_Requiring_Conversion.EmptyStringModel>
    {
        public override void Given()
        {
            SourceWorkItems = new[] { new MockWorkItem(new MockWorkItemType("EmptyStringField", WorkItemBackingStore.Keys.Select(MockFieldDefinition.Create)), WorkItemBackingStore) };
            WorkItemStore = new MockWorkItemStore().Add(SourceWorkItems);
            base.Given();
        }

        [TestMethod]
        public void the_mapped_property_is_the_substituted_value()
        {
            Actual.DoubleField.ShouldEqual(0.0d);
        }

        [WorkItemType("EmptyStringField")]
        public class EmptyStringModel : IIdentifiable<int?>
        {
            [FieldDefinition("Id")]
            public int? Id { get;set;}

            [FieldDefinition("EmptyStringField", true)]
            public double DoubleField { get;set;}
        }
    }

    [TestClass]
    public class Given_a_model_with_a_field_of_type_Double_mapped_from_StringEmpty_with_NullSub : WorkItemMapperContext<Given_a_model_with_a_field_of_type_Double_mapped_from_StringEmpty_Requiring_Conversion.EmptyStringModel>
    {
        public override void Given()
        {
            SourceWorkItems = new[] { new MockWorkItem(new MockWorkItemType("EmptyStringField", WorkItemBackingStore.Keys.Select(MockFieldDefinition.Create)), WorkItemBackingStore) };
            WorkItemStore = new MockWorkItemStore().Add(SourceWorkItems);
            base.Given();
        }

        [TestMethod]
        public void the_mapped_property_is_the_substituted_value()
        {
            Actual.DoubleField.ShouldEqual(0.0d);
        }

        [WorkItemType("EmptyStringField")]
        public class EmptyStringModel : IIdentifiable<int?>
        {
            [FieldDefinition("Id")]
            public int? Id { get; set; }

            [FieldDefinition("EmptyStringField", 0.0d)]
            public double DoubleField { get; set; }
        }
    }

    [TestClass]
    public class Given_a_model_with_a_field_of_type_Double_mapped_from_StringEmpty : WorkItemMapperContext<Given_a_model_with_a_field_of_type_Double_mapped_from_StringEmpty.EmptyStringModel>
    {
        public override void Given()
        {
            SourceWorkItems = new[] { new MockWorkItem(new MockWorkItemType("EmptyStringField", WorkItemBackingStore.Keys.Select(MockFieldDefinition.Create)), WorkItemBackingStore) };
            WorkItemStore = new MockWorkItemStore().Add(SourceWorkItems);
            base.Given();
        }

        [TestMethod]
        public void the_mapped_property_is_the_substituted_value()
        {
            Actual.DoubleField.ShouldEqual(0.0d);
        }

        [WorkItemType("EmptyStringField")]
        public class EmptyStringModel : IIdentifiable<int?>
        {
            [FieldDefinition("Id")]
            public int? Id { get; set; }

            [FieldDefinition("EmptyStringField")]
            public double DoubleField { get; set; }
        }
    }

    [TestClass]
    public class Given_a_model_with_a_field_of_type_Double_mapped_from_StringEmpty_Requiring_Conversion_with_Default : WorkItemMapperContext<Given_a_model_with_a_field_of_type_Double_mapped_from_StringEmpty_Requiring_Conversion_with_Default.EmptyStringModel>
    {
        public override void Given()
        {
            SourceWorkItems = new[] { new MockWorkItem(new MockWorkItemType("EmptyStringField", WorkItemBackingStore.Keys.Select(MockFieldDefinition.Create)), WorkItemBackingStore) };
            WorkItemStore = new MockWorkItemStore().Add(SourceWorkItems);
            base.Given();
        }

        [TestMethod]
        public void the_mapped_property_is_the_substituted_value()
        {
            Actual.DoubleField.ShouldEqual(0.0d);
        }

        [WorkItemType("EmptyStringField")]
        public class EmptyStringModel : IIdentifiable<int?>
        {
            [FieldDefinition("Id")]
            public int? Id { get; set; }

            [FieldDefinition("EmptyStringField", true, 0.0d)]
            public double DoubleField { get; set; }
        }
    }

    [TestClass]
    public class when_an_issue_is_mapped_with_a_field_containing_null_that_does_not_accept_null_and_has_a_null_substitute : WorkItemMapperContext<when_an_issue_is_mapped_with_a_field_containing_null_that_does_not_accept_null_and_has_a_null_substitute.MockModelWithMissingField>
    {
        public override void Given()
        {
            SourceWorkItems = new[] { new MockWorkItem(new MockWorkItemType("MissingFields", WorkItemBackingStore.Keys.Select(MockFieldDefinition.Create)), WorkItemBackingStore) };
            WorkItemStore = new MockWorkItemStore().Add(SourceWorkItems);
            base.Given();
        }

        [TestMethod]
        public void the_mapped_property_is_the_substituted_value()
        {
            Actual.DoesNotExist.ShouldEqual(-1);
        }

        [WorkItemType("MissingFields")]
        public class MockModelWithMissingField : IIdentifiable<int?>
        {
            [FieldDefinition("Id")]
            public virtual int? Id { get; internal set; }

            [FieldDefinition("NullableField", -1)]
            public virtual int DoesNotExist { get; internal set; }
        }
    }

    [TestClass]
    public class when_an_issue_is_mapped_with_a_field_containing_null_that_does_not_accept_null_and_convert_and_has_a_null_substitute : WorkItemMapperContext<when_an_issue_is_mapped_with_a_field_containing_null_that_does_not_accept_null_and_convert_and_has_a_null_substitute.MockModelWithMissingField>
    {
        public override void Given()
        {
            SourceWorkItems = new[] { new MockWorkItem(new MockWorkItemType("MissingFields", WorkItemBackingStore.Keys.Select(MockFieldDefinition.Create)), WorkItemBackingStore) };
            WorkItemStore = new MockWorkItemStore().Add(SourceWorkItems);
            base.Given();
        }

        [TestMethod]
        public void the_mapped_property_is_the_substituted_value()
        {
            Actual.DoesNotExist.ShouldEqual(-1);
        }

        [WorkItemType("Default")]
        public class MockModelWithMissingField : IIdentifiable<int?>
        {
            [FieldDefinition("Id")]
            public int? Id { get; internal set; }

            [FieldDefinition("NullableField", true, -1)]
            public int DoesNotExist { get; internal set; }
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_the_issue_factory_parses_an_issue_with_links : WorkItemMapperContext<MockModelWithLinks>
    {
        public override void Given()
        {
            var wit = new MockWorkItemType("Baz", WorkItemBackingStore.Keys);

            var related = new MockWorkItemLinkType("NS.SampleLink", true, "Taker", "Giver");

            WorkItemStore = new MockWorkItemStore().Add(
                new[]
                    {
                        new MockWorkItem(wit) {Id = 233 },
                        new MockWorkItem(wit) {Id = 144 }
                    }
                ).WithLinkType(related);






            var links = new ILink[]
            {
                new MockRelatedLink(related.ForwardEnd, 0, 233),
                new MockRelatedLink(related.ReverseEnd, 0, 144)
            };

            SourceWorkItems = new IWorkItem[]
            {
                new MockWorkItem(wit, WorkItemBackingStore)
                {
                    Links = new Collection<ILink>(links)
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
            WorkItemStore = new MockWorkItemStore();

            SourceWorkItems = new[] { new MockWorkItem(new MockWorkItemType("Baz", WorkItemBackingStore.Keys.Select(MockFieldDefinition.Create)), WorkItemBackingStore) };

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

                if (expectedVal is ICollection val)
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
        public class when_an_issue_is_mapped_without_a_workitemtype : WorkItemMapperContext<MockModelWithNoType>
        {
            private MockModelWithNoType _expected;

            public override void Given()
            {
                WorkItemStore = new MockWorkItemStore();

                SourceWorkItems = new[] { new MockWorkItem(new MockWorkItemType("Baz", WorkItemBackingStore.Keys.Select(MockFieldDefinition.Create)), WorkItemBackingStore) };

                _expected = new MockModelWithNoType
                {
                    Id = int.Parse(WorkItemBackingStore["Id"].ToString()),
                    IntField = int.Parse(WorkItemBackingStore["IntField"].ToString())
                };
                base.Given();
            }

            [TestMethod]
            public void the_Id_field_is_populated_correctly()
            {
                _expected.Id.ShouldEqual(Actual.Id);
            }

            [TestMethod]
            public void the_IntField_field_is_populated_correctly()
            {
                _expected.IntField.ShouldEqual(Actual.IntField);
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
            WorkItemStore = new MockWorkItemStore();

            SourceWorkItems = new[] { new MockWorkItem(new MockWorkItemType("Baz", WorkItemBackingStore.Keys.Select(MockFieldDefinition.Create)), WorkItemBackingStore) };

            _expected = new MockModelWithNoBacking { Id = int.Parse(WorkItemBackingStore["Id"].ToString()) };
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
