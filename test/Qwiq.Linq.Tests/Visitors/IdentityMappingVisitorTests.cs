using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Identity;
using Qwiq.Linq.Visitors;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Linq.Tests.Visitors
{
    /// <summary>
    /// Test entity with identity fields for testing IdentityMappingVisitor.
    /// </summary>
    public class TestWorkItemWithIdentity
    {
        public string? AssignedTo { get; set; }

        public string? Title { get; set; }

        public int Id { get; set; }
    }

    /// <summary>
    /// Mock identity value converter for testing IdentityMappingVisitor.
    /// </summary>
    public class MockIdentityValueConverterForLinq : IIdentityValueConverter<string, object>
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
    /// Tests for IdentityMappingVisitor constructor.
    /// </summary>
    [TestClass]
    public class Given_IdentityMappingVisitor_constructor_with_null_converter : ContextSpecification
    {
        private ArgumentNullException? _exception;

        public override void When()
        {
            try
            {
                _ = new IdentityMappingVisitor(null!);
            }
            catch (ArgumentNullException ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void Then_throws_ArgumentNullException()
        {
            _exception.ShouldNotBeNull();
            _exception!.ParamName.ShouldBe("valueConverter");
        }
    }

    [TestClass]
    public class Given_IdentityMappingVisitor_constructor_with_valid_converter : ContextSpecification
    {
        private IdentityMappingVisitor _visitor = null!;
        private MockIdentityValueConverterForLinq _converter = null!;

        public override void Given()
        {
            _converter = new MockIdentityValueConverterForLinq();
        }

        public override void When()
        {
            _visitor = new IdentityMappingVisitor(_converter);
        }

        [TestMethod]
        public void Then_visitor_is_created()
        {
            _visitor.ShouldNotBeNull();
        }
    }

    /// <summary>
    /// Tests for IdentityMappingVisitor visiting binary expressions with identity fields.
    /// </summary>
    [TestClass]
    public class Given_IdentityMappingVisitor_visiting_binary_expression_with_AssignedTo : ContextSpecification
    {
        private IdentityMappingVisitor _visitor = null!;
        private MockIdentityValueConverterForLinq _converter = null!;
        private Expression _originalExpression = null!;
        private Expression _visitedExpression = null!;

        public override void Given()
        {
            _converter = new MockIdentityValueConverterForLinq { MappedResult = "domain\\jsmith" };
            _visitor = new IdentityMappingVisitor(_converter);

            // Create expression: entity => entity.AssignedTo == "jsmith"
            var param = Expression.Parameter(typeof(TestWorkItemWithIdentity), "entity");
            var property = Expression.Property(param, nameof(TestWorkItemWithIdentity.AssignedTo));
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
            _converter.MapCallCount.ShouldBe(1);
        }

        [TestMethod]
        public void Then_converter_receives_original_value()
        {
            _converter.LastMappedValue.ShouldBe("jsmith");
        }

        [TestMethod]
        public void Then_expression_is_modified()
        {
            _visitedExpression.ShouldNotBeNull();
            // The expression should contain the mapped value
            _visitedExpression.ToString().ShouldContain("domain\\jsmith");
        }
    }

    /// <summary>
    /// Tests for IdentityMappingVisitor visiting binary expressions without identity fields.
    /// </summary>
    [TestClass]
    public class Given_IdentityMappingVisitor_visiting_binary_expression_with_Title : ContextSpecification
    {
        private IdentityMappingVisitor _visitor = null!;
        private MockIdentityValueConverterForLinq _converter = null!;
        private Expression _originalExpression = null!;
        private Expression _visitedExpression = null!;

        public override void Given()
        {
            _converter = new MockIdentityValueConverterForLinq();
            _visitor = new IdentityMappingVisitor(_converter);

            // Create expression: entity => entity.Title == "Test"
            var param = Expression.Parameter(typeof(TestWorkItemWithIdentity), "entity");
            var property = Expression.Property(param, nameof(TestWorkItemWithIdentity.Title));
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
            _converter.MapCallCount.ShouldBe(0);
        }

        [TestMethod]
        public void Then_expression_is_unchanged()
        {
            _visitedExpression.ToString().ShouldBe(_originalExpression.ToString());
        }
    }

    /// <summary>
    /// Tests for IdentityMappingVisitor visiting binary expressions with non-string values.
    /// </summary>
    [TestClass]
    public class Given_IdentityMappingVisitor_visiting_binary_expression_with_int_value : ContextSpecification
    {
        private IdentityMappingVisitor _visitor = null!;
        private MockIdentityValueConverterForLinq _converter = null!;
        private Expression _originalExpression = null!;
        private Expression _visitedExpression = null!;

        public override void Given()
        {
            _converter = new MockIdentityValueConverterForLinq();
            _visitor = new IdentityMappingVisitor(_converter);

            // Create expression: entity => entity.Id == 123
            var param = Expression.Parameter(typeof(TestWorkItemWithIdentity), "entity");
            var property = Expression.Property(param, nameof(TestWorkItemWithIdentity.Id));
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
            _converter.MapCallCount.ShouldBe(0);
        }
    }

    /// <summary>
    /// Tests for IdentityMappingVisitor visiting simple constant expressions.
    /// </summary>
    [TestClass]
    public class Given_IdentityMappingVisitor_visiting_simple_constant : ContextSpecification
    {
        private IdentityMappingVisitor _visitor = null!;
        private MockIdentityValueConverterForLinq _converter = null!;
        private Expression _originalExpression = null!;
        private Expression _visitedExpression = null!;

        public override void Given()
        {
            _converter = new MockIdentityValueConverterForLinq();
            _visitor = new IdentityMappingVisitor(_converter);

            // Create a simple constant expression without binary context
            _originalExpression = Expression.Constant("test");
        }

        public override void When()
        {
            _visitedExpression = _visitor.Visit(_originalExpression);
        }

        [TestMethod]
        public void Then_converter_Map_is_not_called()
        {
            // Without NeedsIdentityMapping being set to true, the constant is not mapped
            _converter.MapCallCount.ShouldBe(0);
        }

        [TestMethod]
        public void Then_expression_is_unchanged()
        {
            _visitedExpression.ShouldBe(_originalExpression);
        }
    }

    /// <summary>
    /// Tests for IdentityMappingVisitor visiting constant with null value in identity context.
    /// </summary>
    [TestClass]
    public class Given_IdentityMappingVisitor_visiting_binary_with_null_constant : ContextSpecification
    {
        private IdentityMappingVisitor _visitor = null!;
        private MockIdentityValueConverterForLinq _converter = null!;
        private Expression _originalExpression = null!;
        private Expression _visitedExpression = null!;

        public override void Given()
        {
            _converter = new MockIdentityValueConverterForLinq();
            _visitor = new IdentityMappingVisitor(_converter);

            // Create expression: entity => entity.AssignedTo == null
            var param = Expression.Parameter(typeof(TestWorkItemWithIdentity), "entity");
            var property = Expression.Property(param, nameof(TestWorkItemWithIdentity.AssignedTo));
            var constant = Expression.Constant(null, typeof(string));
            _originalExpression = Expression.Equal(property, constant);
        }

        public override void When()
        {
            _visitedExpression = _visitor.Visit(_originalExpression);
        }

        [TestMethod]
        public void Then_converter_Map_is_not_called()
        {
            // Null values should not be mapped
            _converter.MapCallCount.ShouldBe(0);
        }
    }
}
