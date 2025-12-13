using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Identity;
using Qwiq.Linq.Visitors;
using Qwiq.Mapper.Attributes;
using Qwiq.Tests.Common;
using Should;

namespace Qwiq.Identity
{
    /// <summary>
    /// Test class with IdentityFieldAttribute for testing IdentityFieldAttributeVisitor.
    /// </summary>
    public class TestEntityWithIdentity
    {
        [IdentityField]
        public string? AssignedTo { get; set; }

        public string? Title { get; set; }

        public int Id { get; set; }
    }

    /// <summary>
    /// Mock identity value converter for testing.
    /// </summary>
    public class MockIdentityValueConverter : IIdentityValueConverter<string, object>
    {
        public int MapCallCount { get; private set; }
        public string? LastMappedValue { get; private set; }
        public string MappedResult { get; set; } = "mapped_identity";

        public object Map(string value)
        {
            MapCallCount++;
            LastMappedValue = value;
            return MappedResult;
        }

        public IReadOnlyDictionary<string, object> Map(IEnumerable<string> values)
        {
            var result = new Dictionary<string, object>();
            foreach (var value in values)
            {
                result[value] = Map(value);
            }
            return result;
        }
    }

    /// <summary>
    /// Tests for IdentityFieldAttributeVisitor constructor.
    /// </summary>
    [TestClass]
    public class Given_IdentityFieldAttributeVisitor_constructor_with_null : ContextSpecification
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            new IdentityFieldAttributeVisitor(null!);
        }
    }

    [TestClass]
    public class Given_IdentityFieldAttributeVisitor_constructor_with_valid_converter : ContextSpecification
    {
        private IdentityFieldAttributeVisitor _visitor = null!;
        private MockIdentityValueConverter _converter = null!;

        public override void Given()
        {
            _converter = new MockIdentityValueConverter();
        }

        public override void When()
        {
            _visitor = new IdentityFieldAttributeVisitor(_converter);
        }

        [TestMethod]
        public void Then_visitor_is_created()
        {
            _visitor.ShouldNotBeNull();
        }
    }

    /// <summary>
    /// Tests for IdentityFieldAttributeVisitor with binary expressions.
    /// </summary>
    [TestClass]
    public class Given_IdentityFieldAttributeVisitor_visiting_binary_expression_with_identity_field : ContextSpecification
    {
        private IdentityFieldAttributeVisitor _visitor = null!;
        private MockIdentityValueConverter _converter = null!;
        private Expression _originalExpression = null!;
        private Expression _visitedExpression = null!;

        public override void Given()
        {
            _converter = new MockIdentityValueConverter { MappedResult = "domain\\jsmith" };
            _visitor = new IdentityFieldAttributeVisitor(_converter);

            // Create expression: entity => entity.AssignedTo == "jsmith"
            var param = Expression.Parameter(typeof(TestEntityWithIdentity), "entity");
            var property = Expression.Property(param, nameof(TestEntityWithIdentity.AssignedTo));
            var constant = Expression.Constant("jsmith");
            _originalExpression = Expression.Equal(property, constant);
        }

        public override void When()
        {
            _visitedExpression = _visitor.Visit(_originalExpression);
        }

        [TestMethod]
        public void Then_converter_Map_is_called()
        {
            _converter.MapCallCount.ShouldEqual(1);
        }

        [TestMethod]
        public void Then_converter_receives_original_value()
        {
            _converter.LastMappedValue.ShouldEqual("jsmith");
        }

        [TestMethod]
        public void Then_expression_is_modified()
        {
            _visitedExpression.ShouldNotBeNull();
        }
    }

    [TestClass]
    public class Given_IdentityFieldAttributeVisitor_visiting_binary_expression_without_identity_field : ContextSpecification
    {
        private IdentityFieldAttributeVisitor _visitor = null!;
        private MockIdentityValueConverter _converter = null!;
        private Expression _originalExpression = null!;
        private Expression _visitedExpression = null!;

        public override void Given()
        {
            _converter = new MockIdentityValueConverter();
            _visitor = new IdentityFieldAttributeVisitor(_converter);

            // Create expression: entity => entity.Title == "Test"
            var param = Expression.Parameter(typeof(TestEntityWithIdentity), "entity");
            var property = Expression.Property(param, nameof(TestEntityWithIdentity.Title));
            var constant = Expression.Constant("Test");
            _originalExpression = Expression.Equal(property, constant);
        }

        public override void When()
        {
            _visitedExpression = _visitor.Visit(_originalExpression);
        }

        [TestMethod]
        public void Then_converter_Map_is_not_called()
        {
            _converter.MapCallCount.ShouldEqual(0);
        }

        [TestMethod]
        public void Then_expression_is_unchanged()
        {
            _visitedExpression.ShouldEqual(_originalExpression);
        }
    }

    [TestClass]
    public class Given_IdentityFieldAttributeVisitor_visiting_binary_expression_with_int_field : ContextSpecification
    {
        private IdentityFieldAttributeVisitor _visitor = null!;
        private MockIdentityValueConverter _converter = null!;
        private Expression _originalExpression = null!;
        private Expression _visitedExpression = null!;

        public override void Given()
        {
            _converter = new MockIdentityValueConverter();
            _visitor = new IdentityFieldAttributeVisitor(_converter);

            // Create expression: entity => entity.Id == 123
            var param = Expression.Parameter(typeof(TestEntityWithIdentity), "entity");
            var property = Expression.Property(param, nameof(TestEntityWithIdentity.Id));
            var constant = Expression.Constant(123);
            _originalExpression = Expression.Equal(property, constant);
        }

        public override void When()
        {
            _visitedExpression = _visitor.Visit(_originalExpression);
        }

        [TestMethod]
        public void Then_converter_Map_is_not_called()
        {
            // Non-string constants should not be mapped
            _converter.MapCallCount.ShouldEqual(0);
        }
    }

    /// <summary>
    /// Tests for IdentityFieldAttributeVisitor with constant expressions.
    /// </summary>
    [TestClass]
    public class Given_IdentityFieldAttributeVisitor_visiting_simple_constant : ContextSpecification
    {
        private IdentityFieldAttributeVisitor _visitor = null!;
        private MockIdentityValueConverter _converter = null!;
        private Expression _originalExpression = null!;
        private Expression _visitedExpression = null!;

        public override void Given()
        {
            _converter = new MockIdentityValueConverter();
            _visitor = new IdentityFieldAttributeVisitor(_converter);

            // Create a simple constant expression
            _originalExpression = Expression.Constant("test");
        }

        public override void When()
        {
            _visitedExpression = _visitor.Visit(_originalExpression);
        }

        [TestMethod]
        public void Then_converter_Map_is_not_called()
        {
            // No identity context, so should not be mapped
            _converter.MapCallCount.ShouldEqual(0);
        }
    }
}
