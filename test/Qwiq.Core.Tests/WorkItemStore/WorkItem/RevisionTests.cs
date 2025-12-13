using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.WorkItemStore.WorkItem
{
    /// <summary>
    /// Tests for <see cref="Revision"/> class covering both constructors:
    /// 1. Constructor with IWorkItem - used for normal revision access
    /// 2. Constructor with IFieldDefinitionCollection - used for constructing from raw field data
    /// </summary>
    [TestClass]
    public class RevisionTests
    {
    }

    [TestClass]
    public class Given_a_Revision_with_WorkItem : ContextSpecification
    {
        private IRevision _revision = null!;
        private IWorkItem _workItem = null!;

        public override void Given()
        {
            // Create a MockWorkItem with revisions
            var mockWorkItem = new MockWorkItem(
                new MockWorkItemType("Task"),
                new Dictionary<string, object?>
                {
                    { "System.Id", 1 },
                    { "System.Rev", 1 },
                    { "System.Title", "Test Work Item" }
                });

            // Create and add a revision to the work item
            var revisionData = new Dictionary<string, object?>
            {
                { "System.Id", 1 },
                { "System.Rev", 1 },
                { "Index", 1 },
                { "System.Title", "Test Work Item" }
            };
            mockWorkItem.Revisions = new[] { new MockRevision(revisionData) };
            _workItem = mockWorkItem;
        }

        public override void When()
        {
            _revision = _workItem.Revisions.First();
        }

        [TestMethod]
        public void Index_property_is_set()
        {
            _revision.Index.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public void Fields_can_be_accessed()
        {
            _revision.Fields.ShouldNotBeNull();
        }
    }

    [TestClass]
    public class Given_a_Revision_without_WorkItem : ContextSpecification
    {
        private IRevision _revision = null!;
        private Dictionary<string, object?> _fieldValues = null!;

        public override void Given()
        {
            _fieldValues = new Dictionary<string, object?>
            {
                { "System.Id", 42 },
                { "System.Rev", 3 },
                { "Index", 3 },
                { "System.Title", "Revision from field data" }
            };
        }

        public override void When()
        {
            _revision = new MockRevision(_fieldValues);
        }

        [TestMethod]
        [Description("Verifies that Revision can be constructed without a WorkItem reference")]
        public void WorkItem_property_is_null()
        {
            _revision.WorkItem.ShouldBeNull();
        }

        [TestMethod]
        [Description("Verifies Index is set correctly when constructed from field definitions")]
        public void Index_property_is_set_from_field_data()
        {
            _revision.Index.ShouldBe(3);
        }

        [TestMethod]
        [Description("Verifies fields can be accessed when constructed without WorkItem")]
        public void Fields_can_be_accessed()
        {
            _revision.Fields.ShouldNotBeNull();
        }

        [TestMethod]
        [Description("Verifies field values can be retrieved when constructed without WorkItem")]
        public void Field_values_can_be_retrieved()
        {
            _revision["System.Title"].ShouldBe("Revision from field data");
        }

        [TestMethod]
        [Description("Verifies Id returns null when constructed without WorkItem")]
        public void Id_property_returns_null()
        {
            // Cast to IWorkItemCore to access Id property
            var core = (IWorkItemCore)_revision;
            core.Id.HasValue.ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_a_Revision_constructed_with_explicit_index : ContextSpecification
    {
        private IRevision _revision = null!;
        private Dictionary<string, object?> _fieldValues = null!;
        private const int ExpectedIndex = 5;

        public override void Given()
        {
            _fieldValues = new Dictionary<string, object?>
            {
                { "System.Id", 100 },
                { "System.Rev", 5 },
                { "System.Title", "Explicit index revision" }
            };
        }

        public override void When()
        {
            _revision = new MockRevision(_fieldValues, ExpectedIndex);
        }

        [TestMethod]
        public void Index_matches_explicit_value()
        {
            _revision.Index.ShouldBe(ExpectedIndex);
        }

        [TestMethod]
        public void WorkItem_is_null()
        {
            _revision.WorkItem.ShouldBeNull();
        }
    }

    [TestClass]
    public class When_accessing_Revision_indexer_with_null_name : ContextSpecification
    {
        private IRevision _revision = null!;
        private Exception _exception = null!;

        public override void Given()
        {
            var fieldValues = new Dictionary<string, object?>
            {
                { "System.Id", 1 },
                { "Index", 1 }
            };
            _revision = new MockRevision(fieldValues);
        }

        public override void When()
        {
            try
            {
                var _ = _revision[null!];
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void ArgumentNullException_is_thrown()
        {
            _exception.ShouldBeOfType<ArgumentNullException>();
        }
    }

    [TestClass]
    public class Given_a_Revision_constructed_from_FieldDefinitions : ContextSpecification
    {
        private Revision _revision = null!;
        private IFieldDefinitionCollection _definitions = null!;
        private const int RevisionNumber = 7;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _definitions = workItemType.FieldDefinitions;
        }

        public override void When()
        {
            _revision = new Revision(_definitions, RevisionNumber);
        }

        [TestMethod]
        public void Rev_property_is_set()
        {
            _revision.Rev.ShouldBe(RevisionNumber);
        }

        [TestMethod]
        public void Index_equals_Rev()
        {
            _revision.Index.ShouldBe(RevisionNumber);
        }

        [TestMethod]
        public void WorkItem_is_null()
        {
            _revision.WorkItem.ShouldBeNull();
        }

        [TestMethod]
        public void Id_is_null()
        {
            _revision.Id.ShouldBeNull();
        }

        [TestMethod]
        public void Url_is_null()
        {
            _revision.Url.ShouldBeNull();
        }

        [TestMethod]
        public void Fields_collection_is_created()
        {
            _revision.Fields.ShouldNotBeNull();
        }
    }

    [TestClass]
    public class Given_a_Revision_constructed_from_WorkItem : ContextSpecification
    {
        private Revision _revision = null!;
        private IWorkItem _workItem = null!;
        private const int RevisionNumber = 3;
        private const int WorkItemId = 42;

        public override void Given()
        {
            var mockWorkItem = new MockWorkItem(
                new MockWorkItemType("Task"),
                new Dictionary<string, object?>
                {
                    { "System.Id", WorkItemId },
                    { "System.Rev", RevisionNumber },
                    { "System.Title", "Test Work Item" }
                });
            _workItem = mockWorkItem;
        }

        public override void When()
        {
            _revision = new Revision(_workItem, RevisionNumber);
        }

        [TestMethod]
        public void Rev_property_is_set()
        {
            _revision.Rev.ShouldBe(RevisionNumber);
        }

        [TestMethod]
        public void WorkItem_is_set()
        {
            _revision.WorkItem.ShouldBe(_workItem);
        }

        [TestMethod]
        public void Id_returns_WorkItem_Id()
        {
            _revision.Id.ShouldBe(WorkItemId);
        }

        [TestMethod]
        public void Url_returns_WorkItem_Url()
        {
            _revision.Url.ShouldBe(_workItem.Url);
        }
    }

    [TestClass]
    public class Given_a_Revision_with_null_WorkItem_throws : ContextSpecification
    {
        private Exception _exception = null!;

        public override void When()
        {
            try
            {
                _ = new Revision((IWorkItem)null!, 1);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void ArgumentNullException_is_thrown()
        {
            _exception.ShouldBeOfType<ArgumentNullException>();
        }
    }

    [TestClass]
    public class Given_a_Revision_calling_Attachments : ContextSpecification
    {
        private Revision _revision = null!;
        private Exception _exception = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _revision = new Revision(workItemType.FieldDefinitions, 1);
        }

        public override void When()
        {
            try
            {
                var _ = _revision.Attachments;
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void NotSupportedException_is_thrown()
        {
            _exception.ShouldBeOfType<NotSupportedException>();
        }
    }

    [TestClass]
    public class Given_a_Revision_calling_Links : ContextSpecification
    {
        private Revision _revision = null!;
        private Exception _exception = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _revision = new Revision(workItemType.FieldDefinitions, 1);
        }

        public override void When()
        {
            try
            {
                var _ = _revision.Links;
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void NotSupportedException_is_thrown()
        {
            _exception.ShouldBeOfType<NotSupportedException>();
        }
    }

    [TestClass]
    public class Given_a_Revision_calling_GetTagLine : ContextSpecification
    {
        private Revision _revision = null!;
        private Exception _exception = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _revision = new Revision(workItemType.FieldDefinitions, 1);
        }

        public override void When()
        {
            try
            {
                _ = _revision.GetTagLine();
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void NotSupportedException_is_thrown()
        {
            _exception.ShouldBeOfType<NotSupportedException>();
        }
    }

    [TestClass]
    public class Given_a_Revision_calling_SetFieldValue_via_IRevisionInternal : ContextSpecification
    {
        private Revision _revision = null!;
        private Exception _exception = null!;
        private IFieldDefinition _fieldDefinition = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _revision = new Revision(workItemType.FieldDefinitions, 1);
            _fieldDefinition = MockFieldDefinition.Create("System.Title");
        }

        public override void When()
        {
            try
            {
                ((IRevisionInternal)_revision).SetFieldValue(_fieldDefinition, "new value");
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void InvalidOperationException_is_thrown()
        {
            _exception.ShouldBeOfType<InvalidOperationException>();
        }
    }

    [TestClass]
    public class Given_a_Revision_calling_indexer_setter_via_IWorkItemCore : ContextSpecification
    {
        private Revision _revision = null!;
        private Exception _exception = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _revision = new Revision(workItemType.FieldDefinitions, 1);
        }

        public override void When()
        {
            try
            {
                ((IWorkItemCore)_revision)["System.Title"] = "new value";
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void NotSupportedException_is_thrown()
        {
            _exception.ShouldBeOfType<NotSupportedException>();
        }
    }

    [TestClass]
    public class Given_a_Revision_using_internal_SetFieldValue : ContextSpecification
    {
        private Revision _revision = null!;
        private const int FieldId = 100;
        private const string FieldValue = "test value";

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _revision = new Revision(workItemType.FieldDefinitions, 1);
        }

        public override void When()
        {
            _revision.SetFieldValue(FieldId, FieldValue);
        }

        [TestMethod]
        public void HasValue_returns_true()
        {
            _revision.HasValue(FieldId).ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_a_Revision_checking_HasValue_for_missing_field : ContextSpecification
    {
        private Revision _revision = null!;
        private bool _result;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _revision = new Revision(workItemType.FieldDefinitions, 1);
        }

        public override void When()
        {
            _result = _revision.HasValue(999);
        }

        [TestMethod]
        public void HasValue_returns_false()
        {
            _result.ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_a_Revision_GetCurrentFieldValue_with_FieldDefinitions : ContextSpecification
    {
        private Revision _revision = null!;
        private IFieldDefinition _fieldDefinition = null!;
        private object? _result;
        private const string ExpectedValue = "field value";

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Task");
            _revision = new Revision(workItemType.FieldDefinitions, 1);
            _fieldDefinition = MockFieldDefinition.Create("System.Title");
            _revision.SetFieldValue(_fieldDefinition.Id, ExpectedValue);
        }

        public override void When()
        {
            _result = _revision.GetCurrentFieldValue(_fieldDefinition);
        }

        [TestMethod]
        public void Returns_stored_value()
        {
            _result.ShouldBe(ExpectedValue);
        }
    }
}
