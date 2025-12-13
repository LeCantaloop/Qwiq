using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Fields
{
    /// <summary>
    /// Tests for Field class.
    /// </summary>
    [TestClass]
    public class Given_valid_Field_parameters : ContextSpecification
    {
        private Field _field = null!;
        private Revision _revision = null!;
        private IFieldDefinition _fieldDefinition = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _revision = new Revision(workItemType.FieldDefinitions, 1);
            _fieldDefinition = MockFieldDefinition.Create("System.Title");
        }

        public override void When()
        {
            _field = new Field(_revision, _fieldDefinition);
        }

        [TestMethod]
        public void Then_FieldDefinition_is_set()
        {
            _field.FieldDefinition.ShouldBe(_fieldDefinition);
        }

        [TestMethod]
        public void Then_Id_equals_FieldDefinition_Id()
        {
            _field.Id.ShouldBe(_fieldDefinition.Id);
        }

        [TestMethod]
        public void Then_Name_equals_FieldDefinition_Name()
        {
            _field.Name.ShouldBe(_fieldDefinition.Name);
        }

        [TestMethod]
        public void Then_ReferenceName_equals_FieldDefinition_ReferenceName()
        {
            _field.ReferenceName.ShouldBe(_fieldDefinition.ReferenceName);
        }
    }

    [TestClass]
    public class Given_Field_with_Value : ContextSpecification
    {
        private Field _field = null!;
        private Revision _revision = null!;
        private IFieldDefinition _fieldDefinition = null!;
        private const string ExpectedValue = "Test Title";

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _revision = new Revision(workItemType.FieldDefinitions, 1);
            _fieldDefinition = MockFieldDefinition.Create("System.Title");
            _revision.SetFieldValue(_fieldDefinition.Id, ExpectedValue);
        }

        public override void When()
        {
            _field = new Field(_revision, _fieldDefinition);
        }

        [TestMethod]
        public void Then_Value_returns_stored_value()
        {
            _field.Value.ShouldBe(ExpectedValue);
        }
    }

    [TestClass]
    public class Given_Field_setting_Value : ContextSpecification
    {
        private Field _field = null!;
        private Revision _revision = null!;
        private IFieldDefinition _fieldDefinition = null!;
        private Exception _exception = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _revision = new Revision(workItemType.FieldDefinitions, 1);
            _fieldDefinition = MockFieldDefinition.Create("System.Title");
            _field = new Field(_revision, _fieldDefinition);
        }

        public override void When()
        {
            try
            {
                _field.Value = "New Value";
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void Then_InvalidOperationException_is_thrown()
        {
            // Setting value through Revision's IRevisionInternal throws InvalidOperationException
            _exception.ShouldBeOfType<InvalidOperationException>();
        }
    }

    [TestClass]
    public class FieldValidationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Field_with_null_revision_throws_ArgumentNullException()
        {
            var fieldDefinition = MockFieldDefinition.Create("System.Title");
            _ = new Field(null!, fieldDefinition);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Field_with_null_fieldDefinition_throws_ArgumentNullException()
        {
            var workItemType = new MockWorkItemType("Task");
            var revision = new Revision(workItemType.FieldDefinitions, 1);
            _ = new Field(revision, null!);
        }
    }

    [TestClass]
    public class Given_Field_IsValid_with_ValidationState : ContextSpecification
    {
        private Field _field = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            var revision = new Revision(workItemType.FieldDefinitions, 1);
            var fieldDefinition = MockFieldDefinition.Create("System.Title");
            _field = new Field(revision, fieldDefinition);
        }

        [TestMethod]
        public void Then_ValidationState_throws_NotImplementedException()
        {
            try
            {
                var _ = _field.ValidationState;
                Assert.Fail("Expected NotImplementedException");
            }
            catch (NotImplementedException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void Then_IsValid_throws_NotImplementedException()
        {
            // IsValid depends on ValidationState which throws NotImplementedException
            try
            {
                var _ = _field.IsValid;
                Assert.Fail("Expected NotImplementedException");
            }
            catch (NotImplementedException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void Then_IsChangedByUser_throws_NotImplementedException()
        {
            try
            {
                var _ = _field.IsChangedByUser;
                Assert.Fail("Expected NotImplementedException");
            }
            catch (NotImplementedException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void Then_IsDirty_throws_NotImplementedException()
        {
            try
            {
                var _ = _field.IsDirty;
                Assert.Fail("Expected NotImplementedException");
            }
            catch (NotImplementedException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void Then_IsEditable_throws_NotImplementedException()
        {
            try
            {
                var _ = _field.IsEditable;
                Assert.Fail("Expected NotImplementedException");
            }
            catch (NotImplementedException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void Then_IsRequired_throws_NotImplementedException()
        {
            try
            {
                var _ = _field.IsRequired;
                Assert.Fail("Expected NotImplementedException");
            }
            catch (NotImplementedException)
            {
                // Expected
            }
        }

        [TestMethod]
        public void Then_OriginalValue_throws_NotImplementedException()
        {
            try
            {
                var _ = _field.OriginalValue;
                Assert.Fail("Expected NotImplementedException");
            }
            catch (NotImplementedException)
            {
                // Expected
            }
        }
    }
}
