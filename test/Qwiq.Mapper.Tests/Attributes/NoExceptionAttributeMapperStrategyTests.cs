using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mapper.Attributes;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Mapper.Attributes
{
    /// <summary>
    /// Tests that NoExceptionAttributeMapperStrategy suppresses exceptions during mapping.
    /// This is the key behavior difference from AttributeMapperStrategy.
    /// </summary>
    [TestClass]
    public class Given_NoExceptionStrategy_mapping_field_that_does_not_exist : ContextSpecification
    {
        private MockWorkItem _sourceWorkItem = null!;
        private MissingFieldModel _target = null!;
        private IWorkItemMapper _mapper = null!;

        public override void Given()
        {
            // Work item that does NOT have the "MissingField" field
            var backingStore = new Dictionary<string, object?>
            {
                { "Id", 123 },
                { "Title", "Test" }
            };

            var wit = new MockWorkItemType("Bug", backingStore.Keys.Select(MockFieldDefinition.Create));
            _sourceWorkItem = new MockWorkItem(wit, backingStore);

            var inspector = new PropertyInspector(new PropertyReflector());
            _mapper = new WorkItemMapper(new IWorkItemMapperStrategy[]
            {
                new NoExceptionAttributeMapperStrategy(inspector)
            });
        }

        public override void When()
        {
            // This triggers the exception handling path in GetFieldValue
            _target = _mapper.Create<MissingFieldModel>(new[] { _sourceWorkItem }).Single();
        }

        [TestMethod]
        public void Then_mapping_succeeds_without_exception()
        {
            _target.ShouldNotBeNull();
        }

        [TestMethod]
        public void Then_Id_is_mapped()
        {
            _target.Id.ShouldBe(123);
        }

        [TestMethod]
        public void Then_missing_field_has_default_value()
        {
            _target.MissingField.ShouldBeNull();
        }

        [WorkItemType("Bug")]
        public class MissingFieldModel : IIdentifiable<int?>
        {
            [FieldDefinition("Id")]
            public int? Id { get; set; }

            [FieldDefinition("MissingField")]
            public string? MissingField { get; set; }
        }
    }

    /// <summary>
    /// Tests that mapping null to a non-nullable value type doesn't throw.
    /// </summary>
    [TestClass]
    public class Given_NoExceptionStrategy_mapping_null_to_non_nullable_int : ContextSpecification
    {
        private MockWorkItem _sourceWorkItem = null!;
        private NonNullableModel _target = null!;
        private IWorkItemMapper _mapper = null!;

        public override void Given()
        {
            var backingStore = new Dictionary<string, object?>
            {
                { "Id", 123 },
                { "NullableField", null }
            };

            var wit = new MockWorkItemType("Bug", backingStore.Keys.Select(MockFieldDefinition.Create));
            _sourceWorkItem = new MockWorkItem(wit, backingStore);

            var inspector = new PropertyInspector(new PropertyReflector());
            _mapper = new WorkItemMapper(new IWorkItemMapperStrategy[]
            {
                new NoExceptionAttributeMapperStrategy(inspector)
            });
        }

        public override void When()
        {
            _target = _mapper.Create<NonNullableModel>(new[] { _sourceWorkItem }).Single();
        }

        [TestMethod]
        public void Then_mapping_succeeds_without_exception()
        {
            _target.ShouldNotBeNull();
        }

        [TestMethod]
        public void Then_non_nullable_field_has_default_value()
        {
            _target.Value.ShouldBe(0);
        }

        [WorkItemType("Bug")]
        public class NonNullableModel : IIdentifiable<int?>
        {
            [FieldDefinition("Id")]
            public int? Id { get; set; }

            [FieldDefinition("NullableField")]
            public int Value { get; set; }
        }
    }

    /// <summary>
    /// Tests complete field mapping behavior with multiple field types.
    /// </summary>
    [TestClass]
    public class Given_NoExceptionStrategy_mapping_multiple_fields : ContextSpecification
    {
        private MockWorkItem _sourceWorkItem = null!;
        private CompleteModel _target = null!;
        private IWorkItemMapper _mapper = null!;

        public override void Given()
        {
            var backingStore = new Dictionary<string, object?>
            {
                { "Id", 456 },
                { "Title", "Test Work Item" },
                { "Priority", 2 },
                { "OptionalField", null },
                { "MissingFieldName", "should not map" }  // Field exists but maps to different property name
            };

            var wit = new MockWorkItemType("Bug", backingStore.Keys.Select(MockFieldDefinition.Create));
            _sourceWorkItem = new MockWorkItem(wit, backingStore);

            var inspector = new PropertyInspector(new PropertyReflector());
            _mapper = new WorkItemMapper(new IWorkItemMapperStrategy[]
            {
                new NoExceptionAttributeMapperStrategy(inspector)
            });
        }

        public override void When()
        {
            _target = _mapper.Create<CompleteModel>(new[] { _sourceWorkItem }).Single();
        }

        [TestMethod]
        public void Then_Id_is_mapped()
        {
            _target.Id.ShouldBe(456);
        }

        [TestMethod]
        public void Then_Title_is_mapped()
        {
            _target.Title.ShouldBe("Test Work Item");
        }

        [TestMethod]
        public void Then_Priority_is_mapped()
        {
            _target.Priority.ShouldBe(2);
        }

        [TestMethod]
        public void Then_Optional_field_is_null()
        {
            _target.OptionalField.ShouldBeNull();
        }

        [WorkItemType("Bug")]
        public class CompleteModel : IIdentifiable<int?>
        {
            [FieldDefinition("Id")]
            public int? Id { get; set; }

            [FieldDefinition("Title")]
            public string? Title { get; set; }

            [FieldDefinition("Priority")]
            public int Priority { get; set; }

            [FieldDefinition("OptionalField")]
            public string? OptionalField { get; set; }
        }
    }
}
