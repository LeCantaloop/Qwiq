// CA1001: Test classes own disposable fields (_store) but disposal is handled
// by the ContextSpecification.Cleanup() pattern, which is called via [TestCleanup]
#pragma warning disable CA1001

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.WorkItemExtensionsTests
{
    // Static readonly arrays for CA1861 compliance
    internal static class TestArrays
    {
        internal static readonly int[] DefaultTargetIds = { 1, 2, 3 };
    }

    // AddRelatedLink(IWorkItem, IWorkItemStore, int) tests

    [TestClass]
    public class Given_null_workItem_calling_AddRelatedLink : ContextSpecification
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            using var store = new MockWorkItemStore();
            ((IWorkItem)null!).AddRelatedLink(store, 123);
        }
    }

    [TestClass]
    public class Given_null_store_calling_AddRelatedLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;

        public override void Given()
        {
            using var store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: store), 1);
            store.BatchSave(new[] { _workItem });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            _workItem.AddRelatedLink(null!, 123);
        }
    }

    [TestClass]
    public class Given_zero_targetId_calling_AddRelatedLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: _store), 1);
            _store.BatchSave(new[] { _workItem });
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Then_throws_ArgumentOutOfRangeException()
        {
            _workItem.AddRelatedLink(_store, 0);
        }
    }

    [TestClass]
    public class Given_valid_args_calling_AddRelatedLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;
        private const int TargetId = 123;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: _store), 1);
            _store.BatchSave(new[] { _workItem });
        }

        public override void When()
        {
            _workItem.AddRelatedLink(_store, TargetId);
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        public void Then_link_is_added()
        {
            _workItem.Links.Count.ShouldBe(1);
        }

        [TestMethod]
        public void Then_link_is_related_link()
        {
            _workItem.Links.First().ShouldBeOfType<MockRelatedLink>();
        }

        [TestMethod]
        public void Then_link_target_matches()
        {
            var link = (IRelatedLink)_workItem.Links.First();
            link.RelatedWorkItemId.ShouldBe(TargetId);
        }
    }

    // AddParentLink(IWorkItem, IWorkItemStore, int) tests

    [TestClass]
    public class Given_null_workItem_calling_AddParentLink : ContextSpecification
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            using var store = new MockWorkItemStore();
            ((IWorkItem)null!).AddParentLink(store, 123);
        }
    }

    [TestClass]
    public class Given_null_store_calling_AddParentLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;

        public override void Given()
        {
            using var store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: store), 1);
            store.BatchSave(new[] { _workItem });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            _workItem.AddParentLink(null!, 123);
        }
    }

    [TestClass]
    public class Given_zero_parentId_calling_AddParentLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: _store), 1);
            _store.BatchSave(new[] { _workItem });
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Then_throws_ArgumentOutOfRangeException()
        {
            _workItem.AddParentLink(_store, 0);
        }
    }

    [TestClass]
    public class Given_valid_args_calling_AddParentLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;
        private const int ParentId = 456;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: _store), 1);
            _store.BatchSave(new[] { _workItem });
        }

        public override void When()
        {
            _workItem.AddParentLink(_store, ParentId);
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        public void Then_link_is_added()
        {
            _workItem.Links.Count.ShouldBe(1);
        }

        [TestMethod]
        public void Then_link_target_matches()
        {
            var link = (IRelatedLink)_workItem.Links.First();
            link.RelatedWorkItemId.ShouldBe(ParentId);
        }
    }

    // AddChildLink(IWorkItem, IWorkItemStore, int) tests

    [TestClass]
    public class Given_null_workItem_calling_AddChildLink : ContextSpecification
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            using var store = new MockWorkItemStore();
            ((IWorkItem)null!).AddChildLink(store, 123);
        }
    }

    [TestClass]
    public class Given_null_store_calling_AddChildLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;

        public override void Given()
        {
            using var store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: store), 1);
            store.BatchSave(new[] { _workItem });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            _workItem.AddChildLink(null!, 123);
        }
    }

    [TestClass]
    public class Given_zero_childId_calling_AddChildLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: _store), 1);
            _store.BatchSave(new[] { _workItem });
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Then_throws_ArgumentOutOfRangeException()
        {
            _workItem.AddChildLink(_store, 0);
        }
    }

    [TestClass]
    public class Given_valid_args_calling_AddChildLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;
        private const int ChildId = 789;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: _store), 1);
            _store.BatchSave(new[] { _workItem });
        }

        public override void When()
        {
            _workItem.AddChildLink(_store, ChildId);
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        public void Then_link_is_added()
        {
            _workItem.Links.Count.ShouldBe(1);
        }

        [TestMethod]
        public void Then_link_target_matches()
        {
            var link = (IRelatedLink)_workItem.Links.First();
            link.RelatedWorkItemId.ShouldBe(ChildId);
        }
    }

    // AddChildrenLink(IWorkItem, IWorkItemStore, params int[]) tests

    [TestClass]
    public class Given_null_workItem_calling_AddChildrenLink : ContextSpecification
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            using var store = new MockWorkItemStore();
            ((IWorkItem)null!).AddChildrenLink(store, 1, 2, 3);
        }
    }

    [TestClass]
    public class Given_null_store_calling_AddChildrenLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;

        public override void Given()
        {
            using var store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: store), 1);
            store.BatchSave(new[] { _workItem });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            _workItem.AddChildrenLink(null!, 1, 2, 3);
        }
    }

    [TestClass]
    public class Given_null_childrenIds_calling_AddChildrenLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: _store), 1);
            _store.BatchSave(new[] { _workItem });
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            _workItem.AddChildrenLink(_store, null!);
        }
    }

    [TestClass]
    public class Given_empty_childrenIds_calling_AddChildrenLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: _store), 1);
            _store.BatchSave(new[] { _workItem });
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Then_throws_ArgumentException()
        {
            _workItem.AddChildrenLink(_store, Array.Empty<int>());
        }
    }

    [TestClass]
    public class Given_multiple_childrenIds_calling_AddChildrenLink : ContextSpecification
    {
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;
        private readonly int[] _childIds = { 10, 20, 30 };

        public override void Given()
        {
            _store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: _store), 1);
            _store.BatchSave(new[] { _workItem });
        }

        public override void When()
        {
            _workItem.AddChildrenLink(_store, _childIds);
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        public void Then_all_links_are_added()
        {
            _workItem.Links.Count.ShouldBe(3);
        }

        [TestMethod]
        public void Then_all_targets_match()
        {
            var targetIds = _workItem.Links.OfType<IRelatedLink>()
                .Select(l => l.RelatedWorkItemId)
                .OrderBy(id => id)
                .ToArray();
            targetIds.ShouldBe(_childIds);
        }
    }

    // AddRelatedLink(IWorkItem, IWorkItemStore, int[]) tests

    [TestClass]
    public class Given_null_workItem_calling_AddRelatedLink_array : ContextSpecification
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            using var store = new MockWorkItemStore();
            ((IWorkItem)null!).AddRelatedLink(store, TestArrays.DefaultTargetIds);
        }
    }

    [TestClass]
    public class Given_null_store_calling_AddRelatedLink_array : ContextSpecification
    {
        private MockWorkItem _workItem = null!;

        public override void Given()
        {
            using var store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: store), 1);
            store.BatchSave(new[] { _workItem });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            _workItem.AddRelatedLink(null!, TestArrays.DefaultTargetIds);
        }
    }

    [TestClass]
    public class Given_empty_targets_calling_AddRelatedLink_array : ContextSpecification
    {
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: _store), 1);
            _store.BatchSave(new[] { _workItem });
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Then_throws_ArgumentException()
        {
            _workItem.AddRelatedLink(_store, Array.Empty<int>());
        }
    }

    [TestClass]
    public class Given_multiple_targets_calling_AddRelatedLink_array : ContextSpecification
    {
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;
        private readonly int[] _targetIds = { 100, 200, 300 };

        public override void Given()
        {
            _store = new MockWorkItemStore();
            _workItem = new MockWorkItem(new MockWorkItemType("Bug", store: _store), 1);
            _store.BatchSave(new[] { _workItem });
        }

        public override void When()
        {
            _workItem.AddRelatedLink(_store, _targetIds);
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        public void Then_all_links_are_added()
        {
            _workItem.Links.Count.ShouldBe(3);
        }

        [TestMethod]
        public void Then_all_targets_match()
        {
            var actualIds = _workItem.Links.OfType<IRelatedLink>()
                .Select(l => l.RelatedWorkItemId)
                .OrderBy(id => id)
                .ToArray();
            actualIds.ShouldBe(_targetIds);
        }
    }

    // ToWorkItemCollection tests

    [TestClass]
    public class Given_null_items_calling_ToWIC : ContextSpecification
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Then_throws_ArgumentNullException()
        {
            ((IEnumerable<IWorkItem>)null!).ToWorkItemCollection();
        }
    }

    [TestClass]
    public class Given_IWorkItemColl_calling_ToWIC : ContextSpecification
    {
        private IWorkItemCollection _original = null!;
        private IWorkItemCollection _result = null!;
        private MockWorkItemStore _store = null!;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            var type = new MockWorkItemType("Bug", store: _store);
            var items = new List<IWorkItem>
            {
                new MockWorkItem(type, 1),
                new MockWorkItem(type, 2)
            };
            _store.BatchSave(items);
            _original = new WorkItemCollection(items);
        }

        public override void When()
        {
            _result = _original.ToWorkItemCollection();
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        public void Then_returns_same_instance()
        {
            _result.ShouldBeSameAs(_original);
        }
    }

    [TestClass]
    public class Given_List_calling_ToWIC : ContextSpecification
    {
        private List<IWorkItem> _items = null!;
        private IWorkItemCollection _result = null!;
        private MockWorkItemStore _store = null!;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            var type = new MockWorkItemType("Bug", store: _store);
            _items = new List<IWorkItem>
            {
                new MockWorkItem(type, 1),
                new MockWorkItem(type, 2)
            };
            _store.BatchSave(_items);
        }

        public override void When()
        {
            _result = _items.ToWorkItemCollection();
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        public void Then_returns_WorkItemCollection()
        {
            _result.ShouldBeOfType<WorkItemCollection>();
        }

        [TestMethod]
        public void Then_contains_all_items()
        {
            _result.Count.ShouldBe(2);
        }
    }

    [TestClass]
    public class Given_List_with_duplicates_calling_ToWIC : ContextSpecification
    {
        private List<IWorkItem> _items = null!;
        private IWorkItemCollection _result = null!;
        private MockWorkItem _workItem = null!;
        private MockWorkItemStore _store = null!;

        public override void Given()
        {
            _store = new MockWorkItemStore();
            var type = new MockWorkItemType("Bug", store: _store);
            _workItem = new MockWorkItem(type, 1);
            var otherItem = new MockWorkItem(type, 2);
            _store.BatchSave(new IWorkItem[] { _workItem, otherItem });

            _items = new List<IWorkItem>
            {
                _workItem,
                _workItem, // duplicate reference
                otherItem
            };
        }

        public override void When()
        {
            _result = _items.ToWorkItemCollection();
        }

        public override void Cleanup()
        {
            _store?.Dispose();
        }

        [TestMethod]
        public void Then_removes_duplicates()
        {
            _result.Count.ShouldBe(2);
        }
    }

    [TestClass]
    public class Given_empty_List_calling_ToWIC : ContextSpecification
    {
        private IWorkItemCollection _result = null!;

        public override void When()
        {
            _result = new List<IWorkItem>().ToWorkItemCollection();
        }

        [TestMethod]
        public void Then_returns_empty_collection()
        {
            _result.Count.ShouldBe(0);
        }
    }
}
