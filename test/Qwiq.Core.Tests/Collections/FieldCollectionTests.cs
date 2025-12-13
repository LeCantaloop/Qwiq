using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Collections
{
    /// <summary>
    /// Tests for FieldCollection methods using MockFieldCollection.
    /// </summary>
    [TestClass]
    public class Given_FieldCollection_with_fields : ContextSpecification
    {
        private MockFieldCollection _fieldCollection = null!;
        private MockWorkItem _workItem = null!;
        private IFieldDefinitionCollection _definitions = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            _workItem = new MockWorkItem(wit, new Dictionary<string, object?>
            {
                { "Id", 1 },
                { "Title", "Test Bug" },
                { "State", "New" }
            });
            _definitions = wit.FieldDefinitions;
            _fieldCollection = new MockFieldCollection(_workItem, _definitions);
        }

        [TestMethod]
        public void Then_Count_returns_definition_count()
        {
            _fieldCollection.Count.ShouldBe(_definitions.Count);
        }

        [TestMethod]
        public void Then_indexer_by_name_returns_field()
        {
            var field = _fieldCollection["Title"];
            field.ShouldNotBeNull();
            field.Name.ShouldBe("Title");
        }

        [TestMethod]
        public void Then_Contains_by_name_returns_true_for_existing_field()
        {
            _fieldCollection.Contains("Title").ShouldBeTrue();
        }

        [TestMethod]
        public void Then_Contains_by_name_returns_false_for_non_existing_field()
        {
            _fieldCollection.Contains("NonExistent").ShouldBeFalse();
        }

        [TestMethod]
        public void Then_TryGetByName_returns_true_for_existing_field()
        {
            _fieldCollection.TryGetByName("Title", out IField? field).ShouldBeTrue();
            field.ShouldNotBeNull();
        }

        [TestMethod]
        public void Then_TryGetByName_returns_false_for_non_existing_field()
        {
            _fieldCollection.TryGetByName("NonExistent", out IField? field).ShouldBeFalse();
            field.ShouldBeNull();
        }

        [TestMethod]
        public void Then_TryGetByName_with_null_returns_false()
        {
            _fieldCollection.TryGetByName(null!, out IField? field).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Contains_by_id_returns_true_for_existing_field()
        {
            var idField = _definitions["System.Id"];
            _fieldCollection.Contains(idField.Id).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_Contains_by_id_returns_false_for_non_existing_id()
        {
            _fieldCollection.Contains(-999).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_FieldCollection_indexer_by_index : ContextSpecification
    {
        private MockFieldCollection _fieldCollection = null!;
        private MockWorkItem _workItem = null!;
        private IFieldDefinitionCollection _definitions = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            _workItem = new MockWorkItem(wit, new Dictionary<string, object?>
            {
                { "Id", 1 },
                { "Title", "Test Bug" }
            });
            _definitions = wit.FieldDefinitions;
            _fieldCollection = new MockFieldCollection(_workItem, _definitions);
        }

        [TestMethod]
        public void Then_indexer_by_index_returns_field()
        {
            var field = _fieldCollection[0];
            field.ShouldNotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Then_indexer_with_negative_index_throws()
        {
            var _ = _fieldCollection[-1];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Then_indexer_with_index_beyond_count_throws()
        {
            var _ = _fieldCollection[_definitions.Count + 1];
        }
    }

    [TestClass]
    public class Given_FieldCollection_indexer_by_name_with_null : ContextSpecification
    {
        private MockFieldCollection _fieldCollection = null!;
        private MockWorkItem _workItem = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            _workItem = new MockWorkItem(wit, new Dictionary<string, object?>
            {
                { "Id", 1 }
            });
            _fieldCollection = new MockFieldCollection(_workItem, wit.FieldDefinitions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_indexer_by_name_with_null_throws()
        {
            var _ = _fieldCollection[null!];
        }
    }

    [TestClass]
    public class Given_FieldCollection_GetById : ContextSpecification
    {
        private MockFieldCollection _fieldCollection = null!;
        private MockWorkItem _workItem = null!;
        private int _validId;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            _workItem = new MockWorkItem(wit, new Dictionary<string, object?>
            {
                { "Id", 1 }
            });
            _fieldCollection = new MockFieldCollection(_workItem, wit.FieldDefinitions);
            _validId = wit.FieldDefinitions["System.Id"].Id;
        }

        [TestMethod]
        public void Then_GetById_returns_field_for_valid_id()
        {
            var field = _fieldCollection.GetById(_validId);
            field.ShouldNotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(DeniedOrNotExistException))]
        public void Then_GetById_throws_for_invalid_id()
        {
            _fieldCollection.GetById(-999);
        }
    }

    [TestClass]
    public class Given_FieldCollection_TryGetById : ContextSpecification
    {
        private MockFieldCollection _fieldCollection = null!;
        private MockWorkItem _workItem = null!;
        private int _validId;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            _workItem = new MockWorkItem(wit, new Dictionary<string, object?>
            {
                { "Id", 1 }
            });
            _fieldCollection = new MockFieldCollection(_workItem, wit.FieldDefinitions);
            _validId = wit.FieldDefinitions["System.Id"].Id;
        }

        [TestMethod]
        public void Then_TryGetById_returns_true_for_valid_id()
        {
            _fieldCollection.TryGetById(_validId, out IField? field).ShouldBeTrue();
            field.ShouldNotBeNull();
        }

        [TestMethod]
        public void Then_TryGetById_returns_false_for_invalid_id()
        {
            _fieldCollection.TryGetById(-999, out IField? field).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_FieldCollection_SetField : ContextSpecification
    {
        private MockFieldCollection _fieldCollection = null!;
        private MockWorkItem _workItem = null!;
        private IFieldDefinition _fieldDef = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            _workItem = new MockWorkItem(wit, new Dictionary<string, object?>
            {
                { "Id", 1 },
                { "Title", "Test" }
            });
            _fieldCollection = new MockFieldCollection(_workItem, wit.FieldDefinitions);
            _fieldDef = wit.FieldDefinitions["Title"];
        }

        [TestMethod]
        public void Then_SetField_adds_field_to_cache()
        {
            var field = new MockField(_fieldDef, "New Title");
            _fieldCollection.SetField(field);
            _fieldCollection.GetById(_fieldDef.Id).ShouldNotBeNull();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_SetField_with_null_throws()
        {
            _fieldCollection.SetField(null!);
        }
    }

    [TestClass]
    public class Given_FieldCollection_Contains_IField : ContextSpecification
    {
        private MockFieldCollection _fieldCollection = null!;
        private MockWorkItem _workItem = null!;
        private IFieldDefinition _fieldDef = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            _workItem = new MockWorkItem(wit, new Dictionary<string, object?>
            {
                { "Id", 1 },
                { "Title", "Test" }
            });
            _fieldCollection = new MockFieldCollection(_workItem, wit.FieldDefinitions);
            _fieldDef = wit.FieldDefinitions["Title"];
        }

        [TestMethod]
        public void Then_Contains_returns_true_for_field_in_collection()
        {
            // First access the field to add it to the cache
            var field = _fieldCollection["Title"];
            _fieldCollection.Contains(field).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_Contains_returns_false_for_field_not_in_collection()
        {
            // Create a field with a different definition using static factory
            var otherDef = MockFieldDefinition.Create("Other.Field");
            var otherField = new MockField(otherDef, "value");
            _fieldCollection.Contains(otherField).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_FieldCollection_Equals : ContextSpecification
    {
        private MockFieldCollection _fieldCollection1 = null!;
        private MockFieldCollection _fieldCollection2 = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            var workItem1 = new MockWorkItem(wit, new Dictionary<string, object?>
            {
                { "Id", 1 },
                { "Title", "Test" }
            });
            var workItem2 = new MockWorkItem(wit, new Dictionary<string, object?>
            {
                { "Id", 1 },
                { "Title", "Test" }
            });
            _fieldCollection1 = new MockFieldCollection(workItem1, wit.FieldDefinitions);
            _fieldCollection2 = new MockFieldCollection(workItem2, wit.FieldDefinitions);
        }

        [TestMethod]
        public void Then_Equals_with_null_returns_false()
        {
            _fieldCollection1.Equals(null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_same_reference_returns_true()
        {
            _fieldCollection1.Equals(_fieldCollection1).ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_FieldCollection_GetEnumerator : ContextSpecification
    {
        private MockFieldCollection _fieldCollection = null!;
        private MockWorkItem _workItem = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            _workItem = new MockWorkItem(wit, new Dictionary<string, object?>
            {
                { "Id", 1 },
                { "Title", "Test" }
            });
            _fieldCollection = new MockFieldCollection(_workItem, wit.FieldDefinitions);
            // Access a field to populate the cache
            var _ = _fieldCollection["Title"];
        }

        [TestMethod]
        public void Then_GetEnumerator_returns_cached_fields()
        {
            var count = 0;
            foreach (var field in _fieldCollection)
            {
                field.ShouldNotBeNull();
                count++;
            }
            count.ShouldBeGreaterThan(0);
        }
    }
}
