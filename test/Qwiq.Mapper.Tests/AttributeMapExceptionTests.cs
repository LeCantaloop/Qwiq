using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Mapper
{
    /// <summary>
    /// Tests for AttributeMapException and its message formatting behavior.
    /// </summary>
    [TestClass]
    public class Given_AttributeMapException_with_full_context : ContextSpecification
    {
        private AttributeMapException _exception = null!;
        private IWorkItem _sourceWorkItem = null!;
        private PropertyInfo _property = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            _sourceWorkItem = new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 1 } });
            _property = typeof(TestModel).GetProperty(nameof(TestModel.Title))!;
        }

        public override void When()
        {
            var typePair = new TypePair(_sourceWorkItem, typeof(TestModel));
            var propertyMap = new PropertyMap(_property, "System.Title");

            _exception = new AttributeMapException(
                "Field mapping failed",
                new InvalidOperationException("Inner error"),
                typePair,
                propertyMap);
        }

        [TestMethod]
        public void Then_Message_contains_base_message()
        {
            _exception.Message.ShouldContain("Field mapping failed");
        }

        [TestMethod]
        public void Then_Message_contains_type_mapping_info()
        {
            _exception.Message.ShouldContain("Mapping types:");
            _exception.Message.ShouldContain("Bug");
            _exception.Message.ShouldContain(typeof(TestModel).FullName!);
        }

        [TestMethod]
        public void Then_Message_contains_property_mapping_info()
        {
            _exception.Message.ShouldContain("Property:");
            _exception.Message.ShouldContain("System.Title");
            _exception.Message.ShouldContain("Title");
        }

        [TestMethod]
        public void Then_InnerException_is_set()
        {
            _exception.InnerException.ShouldNotBeNull();
            _exception.InnerException.ShouldBeOfType<InvalidOperationException>();
        }

        [TestMethod]
        public void Then_Types_property_is_set()
        {
            _exception.Types.ShouldNotBeNull();
            _exception.Types!.Value.Source.ShouldBe(_sourceWorkItem);
            _exception.Types!.Value.Destination.ShouldBe(typeof(TestModel));
        }

        [TestMethod]
        public void Then_PropertyMap_is_set()
        {
            _exception.PropertyMap.ShouldNotBeNull();
            _exception.PropertyMap!.Value.SourceField.ShouldBe("System.Title");
            _exception.PropertyMap!.Value.DestinationProperty.ShouldBe(_property);
        }

        private class TestModel
        {
            public string? Title { get; set; }
        }
    }

    [TestClass]
    public class Given_AttributeMapException_with_message_only : ContextSpecification
    {
        private AttributeMapException _exception = null!;

        public override void When()
        {
            _exception = new AttributeMapException("Simple error message");
        }

        [TestMethod]
        public void Then_Message_equals_provided_message()
        {
            _exception.Message.ShouldBe("Simple error message");
        }

        [TestMethod]
        public void Then_Types_is_null()
        {
            _exception.Types.ShouldBeNull();
        }

        [TestMethod]
        public void Then_PropertyMap_is_null()
        {
            _exception.PropertyMap.ShouldBeNull();
        }
    }

    [TestClass]
    public class Given_AttributeMapException_with_inner_exception : ContextSpecification
    {
        private AttributeMapException _exception = null!;
        private InvalidOperationException _innerException = null!;

        public override void Given()
        {
            _innerException = new InvalidOperationException("Inner failure");
        }

        public override void When()
        {
            _exception = new AttributeMapException("Outer error", _innerException);
        }

        [TestMethod]
        public void Then_Message_contains_message()
        {
            _exception.Message.ShouldBe("Outer error");
        }

        [TestMethod]
        public void Then_InnerException_is_preserved()
        {
            _exception.InnerException.ShouldBe(_innerException);
        }
    }

    [TestClass]
    public class Given_AttributeMapException_default_constructor : ContextSpecification
    {
        private AttributeMapException _exception = null!;

        public override void When()
        {
            _exception = new AttributeMapException();
        }

        [TestMethod]
        public void Then_Message_is_empty()
        {
            _exception.Message.ShouldBeEmpty();
        }
    }

    /// <summary>
    /// Tests for PropertyMap struct.
    /// </summary>
    [TestClass]
    public class Given_PropertyMap_with_valid_values : ContextSpecification
    {
        private PropertyMap _propertyMap;
        private PropertyInfo _property = null!;

        public override void Given()
        {
            _property = typeof(TestModel).GetProperty(nameof(TestModel.Id))!;
        }

        public override void When()
        {
            _propertyMap = new PropertyMap(_property, "System.Id");
        }

        [TestMethod]
        public void Then_DestinationProperty_is_set()
        {
            _propertyMap.DestinationProperty.ShouldBe(_property);
        }

        [TestMethod]
        public void Then_SourceField_is_set()
        {
            _propertyMap.SourceField.ShouldBe("System.Id");
        }

        private class TestModel
        {
            public int? Id { get; set; }
        }
    }

    /// <summary>
    /// Tests for TypePair struct.
    /// </summary>
    [TestClass]
    public class Given_TypePair_with_valid_values : ContextSpecification
    {
        private TypePair _typePair;
        private IWorkItem _workItem = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Task");
            _workItem = new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 42 } });
        }

        public override void When()
        {
            _typePair = new TypePair(_workItem, typeof(DestinationModel));
        }

        [TestMethod]
        public void Then_Source_is_set()
        {
            _typePair.Source.ShouldBe(_workItem);
        }

        [TestMethod]
        public void Then_Destination_is_set()
        {
            _typePair.Destination.ShouldBe(typeof(DestinationModel));
        }

        private class DestinationModel
        {
            public int? Id { get; set; }
        }
    }
}
