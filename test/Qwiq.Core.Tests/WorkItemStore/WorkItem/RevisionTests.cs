using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Qwiq.Mocks;
using Qwiq.Tests.Common;

using Should;

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

    #region Revision with WorkItem Tests

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

    #endregion

    #region Revision without WorkItem Tests

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
            _revision.Index.ShouldEqual(3);
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
            _revision["System.Title"].ShouldEqual("Revision from field data");
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
            _revision.Index.ShouldEqual(ExpectedIndex);
        }

        [TestMethod]
        public void WorkItem_is_null()
        {
            _revision.WorkItem.ShouldBeNull();
        }
    }

    #endregion

    #region Revision Indexer Tests

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
            _exception.ShouldBeType<ArgumentNullException>();
        }
    }

    #endregion
}
