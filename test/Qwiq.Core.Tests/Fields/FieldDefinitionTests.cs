using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Fields
{
    /// <summary>
    /// Tests for FieldDefinition construction and behavior.
    /// Tests the Qwiq.Core.FieldDefinition class directly using internal constructors.
    /// </summary>
    [TestClass]
    public class Given_valid_FieldDefinition_parameters : ContextSpecification
    {
        private FieldDefinition _field = null!;
        private string _name = null!;
        private string _referenceName = null!;

        public override void Given()
        {
            _name = "Title";
            _referenceName = "Custom.Title";
        }

        public override void When()
        {
            _field = new FieldDefinition(_referenceName, _name);
        }

        [TestMethod]
        public void Then_Name_is_set()
        {
            _field.Name.ShouldBe(_name);
        }

        [TestMethod]
        public void Then_ReferenceName_is_set()
        {
            _field.ReferenceName.ShouldBe(_referenceName);
        }

        [TestMethod]
        public void Then_Id_is_computed()
        {
            _field.Id.ShouldNotBe(0);
        }
    }

    [TestClass]
    public class Given_FieldDefinition_with_core_field_referenceName : ContextSpecification
    {
        private FieldDefinition _field = null!;

        public override void When()
        {
            // System.Id is a core field with a known ID
            _field = new FieldDefinition(CoreFieldRefNames.Id, "ID");
        }

        [TestMethod]
        public void Then_Id_is_set_to_core_field_id()
        {
            // Core fields have predefined IDs
            CoreFieldRefNames.CoreFieldIdLookup.TryGetValue(CoreFieldRefNames.Id, out var expectedId);
            _field.Id.ShouldBe(expectedId);
        }
    }

    [TestClass]
    public class Given_FieldDefinition_with_explicit_id : ContextSpecification
    {
        private FieldDefinition _field = null!;
        private int _id;

        public override void Given()
        {
            _id = 12345;
        }

        public override void When()
        {
            _field = new FieldDefinition(_id, "Custom.Field", "Custom Field");
        }

        [TestMethod]
        public void Then_Id_is_set_to_explicit_value()
        {
            _field.Id.ShouldBe(_id);
        }
    }

    [TestClass]
    public class Given_FieldDefinition_with_zero_id_and_non_core_field : ContextSpecification
    {
        private FieldDefinition _field = null!;

        public override void When()
        {
            // Explicit zero ID with non-core field should compute ID from hash
            _field = new FieldDefinition(0, "Custom.MyField", "My Field");
        }

        [TestMethod]
        public void Then_Id_is_computed_from_hash()
        {
            _field.Id.ShouldNotBe(0);
        }
    }

    [TestClass]
    public class FieldDefinitionValidationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FieldDefinition_with_null_name_throws_ArgumentException()
        {
            _ = new FieldDefinition("System.Title", null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FieldDefinition_with_empty_name_throws_ArgumentException()
        {
            _ = new FieldDefinition("System.Title", string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FieldDefinition_with_whitespace_name_throws_ArgumentException()
        {
            _ = new FieldDefinition("System.Title", "   ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FieldDefinition_with_null_referenceName_throws_ArgumentException()
        {
            _ = new FieldDefinition(null!, "Title");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FieldDefinition_with_empty_referenceName_throws_ArgumentException()
        {
            _ = new FieldDefinition(string.Empty, "Title");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FieldDefinition_with_whitespace_referenceName_throws_ArgumentException()
        {
            _ = new FieldDefinition("   ", "Title");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FieldDefinition_with_id_null_name_throws_ArgumentException()
        {
            _ = new FieldDefinition(1, "System.Title", null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FieldDefinition_with_id_null_referenceName_throws_ArgumentException()
        {
            _ = new FieldDefinition(1, null!, "Title");
        }
    }

    [TestClass]
    public class Given_FieldDefinition_ToString : ContextSpecification
    {
        private FieldDefinition _field = null!;
        private string _result = null!;

        public override void Given()
        {
            _field = new FieldDefinition("System.Title", "Title");
        }

        public override void When()
        {
            _result = _field.ToString();
        }

        [TestMethod]
        public void Then_returns_ReferenceName()
        {
            _result.ShouldBe("System.Title");
        }
    }

    [TestClass]
    public class Given_two_FieldDefinitions_with_same_values : ContextSpecification
    {
        private FieldDefinition _field1 = null!;
        private FieldDefinition _field2 = null!;

        public override void Given()
        {
            _field1 = new FieldDefinition("System.Title", "Title");
            _field2 = new FieldDefinition("System.Title", "Title");
        }

        [TestMethod]
        public void Then_Equals_returns_true()
        {
            _field1.Equals(_field2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_is_same()
        {
            _field1.GetHashCode().ShouldBe(_field2.GetHashCode());
        }
    }

    [TestClass]
    public class Given_two_FieldDefinitions_with_different_name : ContextSpecification
    {
        private FieldDefinition _field1 = null!;
        private FieldDefinition _field2 = null!;

        public override void Given()
        {
            _field1 = new FieldDefinition("System.Title", "Title");
            _field2 = new FieldDefinition("System.Title", "Description");
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _field1.Equals(_field2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_two_FieldDefinitions_with_different_referenceName : ContextSpecification
    {
        private FieldDefinition _field1 = null!;
        private FieldDefinition _field2 = null!;

        public override void Given()
        {
            _field1 = new FieldDefinition("System.Title", "Title");
            _field2 = new FieldDefinition("System.Description", "Title");
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _field1.Equals(_field2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_FieldDefinition_Equals_with_null : ContextSpecification
    {
        private FieldDefinition _field = null!;

        public override void Given()
        {
            _field = new FieldDefinition("System.Title", "Title");
        }

        [TestMethod]
        public void Then_Equals_IFieldDefinition_null_returns_false()
        {
            _field.Equals((IFieldDefinition?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_null_returns_false()
        {
            _field.Equals((object?)null).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_FieldDefinition_with_case_insensitive_comparison : ContextSpecification
    {
        private FieldDefinition _field1 = null!;
        private FieldDefinition _field2 = null!;

        public override void Given()
        {
            _field1 = new FieldDefinition("SYSTEM.TITLE", "TITLE");
            _field2 = new FieldDefinition("system.title", "title");
        }

        [TestMethod]
        public void Then_Equals_returns_true()
        {
            _field1.Equals(_field2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_is_same()
        {
            _field1.GetHashCode().ShouldBe(_field2.GetHashCode());
        }
    }

    /// <summary>
    /// Tests for FieldDefinitionComparer.
    /// </summary>
    [TestClass]
    public class Given_FieldDefinitionComparer_with_null : ContextSpecification
    {
        private FieldDefinition _field = null!;

        public override void Given()
        {
            _field = new FieldDefinition("System.Title", "Title");
        }

        [TestMethod]
        public void Then_null_null_returns_true()
        {
            FieldDefinitionComparer.Default.Equals(null, null).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_field_null_returns_false()
        {
            FieldDefinitionComparer.Default.Equals(_field, null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_null_field_returns_false()
        {
            FieldDefinitionComparer.Default.Equals(null, _field).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_GetHashCode_null_returns_zero()
        {
            FieldDefinitionComparer.Default.GetHashCode(null!).ShouldBe(0);
        }

        [TestMethod]
        public void Then_GetHashCode_returns_non_zero()
        {
            FieldDefinitionComparer.Default.GetHashCode(_field).ShouldNotBe(0);
        }
    }

    [TestClass]
    public class Given_FieldDefinitionComparer_with_same_reference : ContextSpecification
    {
        private bool _result;
        private FieldDefinition _field = null!;

        public override void Given()
        {
            _field = new FieldDefinition("System.Title", "Title");
        }

        public override void When()
        {
            _result = FieldDefinitionComparer.Default.Equals(_field, _field);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }
}
