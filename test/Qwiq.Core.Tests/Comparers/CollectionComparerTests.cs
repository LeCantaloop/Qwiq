using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Comparers
{
    /// <summary>
    /// Tests for WorkItemCollectionComparer accessed via Comparer.WorkItemCollection.
    /// </summary>
    [TestClass]
    public class Given_WorkItemCollectionComparer_with_null_values : ContextSpecification
    {
        [TestMethod]
        public void Then_null_null_are_equal()
        {
            Comparer.WorkItemCollection.Equals(null!, null!).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_null_collection_are_not_equal()
        {
            var collection = CreateMockWorkItemCollection(1);
            Comparer.WorkItemCollection.Equals(null!, collection).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_collection_null_are_not_equal()
        {
            var collection = CreateMockWorkItemCollection(1);
            Comparer.WorkItemCollection.Equals(collection, null!).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_GetHashCode_of_null_returns_zero()
        {
            Comparer.WorkItemCollection.GetHashCode(null!).ShouldBe(0);
        }

        private static IWorkItemCollection CreateMockWorkItemCollection(params int[] ids)
        {
            var wit = new MockWorkItemType("Bug");
            var items = new List<IWorkItem>();
            foreach (var id in ids)
            {
                items.Add(new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", id } }));
            }
            return new WorkItemCollection(items);
        }
    }

    [TestClass]
    public class Given_WorkItemCollectionComparer_with_same_reference : ContextSpecification
    {
        private IWorkItemCollection _collection = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            var items = new List<IWorkItem>
            {
                new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 1 } })
            };
            _collection = new WorkItemCollection(items);
        }

        [TestMethod]
        public void Then_same_reference_are_equal()
        {
            Comparer.WorkItemCollection.Equals(_collection, _collection).ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_WorkItemCollectionComparer_with_equal_collections : ContextSpecification
    {
        private IWorkItemCollection _collection1 = null!;
        private IWorkItemCollection _collection2 = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            var items1 = new List<IWorkItem>
            {
                new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 1 }, { "Title", "Test" } }),
                new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 2 }, { "Title", "Test2" } })
            };
            var items2 = new List<IWorkItem>
            {
                new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 1 }, { "Title", "Test" } }),
                new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 2 }, { "Title", "Test2" } })
            };
            _collection1 = new WorkItemCollection(items1);
            _collection2 = new WorkItemCollection(items2);
        }

        [TestMethod]
        public void Then_equal_collections_are_equal()
        {
            Comparer.WorkItemCollection.Equals(_collection1, _collection2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_equal_collections_have_same_hash_code()
        {
            Comparer.WorkItemCollection.GetHashCode(_collection1)
                .ShouldBe(Comparer.WorkItemCollection.GetHashCode(_collection2));
        }
    }

    [TestClass]
    public class Given_WorkItemCollectionComparer_with_different_count : ContextSpecification
    {
        private IWorkItemCollection _collection1 = null!;
        private IWorkItemCollection _collection2 = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            var items1 = new List<IWorkItem>
            {
                new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 1 } })
            };
            var items2 = new List<IWorkItem>
            {
                new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 1 } }),
                new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 2 } })
            };
            _collection1 = new WorkItemCollection(items1);
            _collection2 = new WorkItemCollection(items2);
        }

        [TestMethod]
        public void Then_different_count_are_not_equal()
        {
            Comparer.WorkItemCollection.Equals(_collection1, _collection2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_WorkItemCollectionComparer_with_different_items : ContextSpecification
    {
        private IWorkItemCollection _collection1 = null!;
        private IWorkItemCollection _collection2 = null!;

        public override void Given()
        {
            var wit = new MockWorkItemType("Bug");
            var items1 = new List<IWorkItem>
            {
                new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 1 } })
            };
            var items2 = new List<IWorkItem>
            {
                new MockWorkItem(wit, new Dictionary<string, object?> { { "Id", 999 } })
            };
            _collection1 = new WorkItemCollection(items1);
            _collection2 = new WorkItemCollection(items2);
        }

        [TestMethod]
        public void Then_different_items_are_not_equal()
        {
            Comparer.WorkItemCollection.Equals(_collection1, _collection2).ShouldBeFalse();
        }
    }

    /// <summary>
    /// Tests for WorkItemTypeCollectionComparer accessed via Comparer.WorkItemTypeCollection.
    /// </summary>
    [TestClass]
    public class Given_WorkItemTypeCollectionComparer_with_null_values : ContextSpecification
    {
        [TestMethod]
        public void Then_null_null_are_equal()
        {
            Comparer.WorkItemTypeCollection.Equals(null!, null!).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_null_collection_are_not_equal()
        {
            var collection = CreateWorkItemTypeCollection(new MockWorkItemType("Bug"));
            Comparer.WorkItemTypeCollection.Equals(null!, collection).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_collection_null_are_not_equal()
        {
            var collection = CreateWorkItemTypeCollection(new MockWorkItemType("Bug"));
            Comparer.WorkItemTypeCollection.Equals(collection, null!).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_GetHashCode_of_null_returns_zero()
        {
            Comparer.WorkItemTypeCollection.GetHashCode(null!).ShouldBe(0);
        }

        private static IWorkItemTypeCollection CreateWorkItemTypeCollection(params IWorkItemType[] types)
        {
            return new WorkItemTypeCollection(new List<IWorkItemType>(types));
        }
    }

    [TestClass]
    public class Given_WorkItemTypeCollectionComparer_with_same_reference : ContextSpecification
    {
        private IWorkItemTypeCollection _collection = null!;

        public override void Given()
        {
            _collection = new WorkItemTypeCollection(new List<IWorkItemType> { new MockWorkItemType("Bug") });
        }

        [TestMethod]
        public void Then_same_reference_are_equal()
        {
            Comparer.WorkItemTypeCollection.Equals(_collection, _collection).ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_WorkItemTypeCollectionComparer_with_equal_collections : ContextSpecification
    {
        private IWorkItemTypeCollection _collection1 = null!;
        private IWorkItemTypeCollection _collection2 = null!;

        public override void Given()
        {
            _collection1 = new WorkItemTypeCollection(new List<IWorkItemType>
            {
                new MockWorkItemType("Bug"),
                new MockWorkItemType("Task")
            });
            _collection2 = new WorkItemTypeCollection(new List<IWorkItemType>
            {
                new MockWorkItemType("Bug"),
                new MockWorkItemType("Task")
            });
        }

        [TestMethod]
        public void Then_equal_collections_are_equal()
        {
            Comparer.WorkItemTypeCollection.Equals(_collection1, _collection2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_equal_collections_have_same_hash_code()
        {
            Comparer.WorkItemTypeCollection.GetHashCode(_collection1)
                .ShouldBe(Comparer.WorkItemTypeCollection.GetHashCode(_collection2));
        }
    }

    [TestClass]
    public class Given_WorkItemTypeCollectionComparer_with_different_types : ContextSpecification
    {
        private IWorkItemTypeCollection _collection1 = null!;
        private IWorkItemTypeCollection _collection2 = null!;

        public override void Given()
        {
            _collection1 = new WorkItemTypeCollection(new List<IWorkItemType> { new MockWorkItemType("Bug") });
            _collection2 = new WorkItemTypeCollection(new List<IWorkItemType> { new MockWorkItemType("Task") });
        }

        [TestMethod]
        public void Then_different_types_are_not_equal()
        {
            Comparer.WorkItemTypeCollection.Equals(_collection1, _collection2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_WorkItemTypeCollectionComparer_with_asymmetric_collections : ContextSpecification
    {
        private IWorkItemTypeCollection _collection1 = null!;
        private IWorkItemTypeCollection _collection2 = null!;

        public override void Given()
        {
            _collection1 = new WorkItemTypeCollection(new List<IWorkItemType>
            {
                new MockWorkItemType("Bug"),
                new MockWorkItemType("Task")
            });
            _collection2 = new WorkItemTypeCollection(new List<IWorkItemType>
            {
                new MockWorkItemType("Bug")
            });
        }

        [TestMethod]
        public void Then_asymmetric_collections_are_not_equal()
        {
            Comparer.WorkItemTypeCollection.Equals(_collection1, _collection2).ShouldBeFalse();
        }
    }

    /// <summary>
    /// Tests for ProjectComparer accessed via Comparer.Project.
    /// </summary>
    [TestClass]
    public class Given_ProjectComparer_with_null_values : ContextSpecification
    {
        [TestMethod]
        public void Then_null_null_are_equal()
        {
            Comparer.Project.Equals(null!, null!).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_of_null_returns_zero()
        {
            Comparer.Project.GetHashCode(null!).ShouldBe(0);
        }
    }

    [TestClass]
    public class Given_ProjectComparer_with_MockWorkItemStore : ContextSpecification
    {
        private IProject _project = null!;

        public override void Given()
        {
            var store = new MockWorkItemStore();
            _project = new MockProject(store);
        }

        [TestMethod]
        public void Then_same_reference_are_equal()
        {
            Comparer.Project.Equals(_project, _project).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_returns_consistent_value()
        {
            var hash1 = Comparer.Project.GetHashCode(_project);
            var hash2 = Comparer.Project.GetHashCode(_project);
            hash1.ShouldBe(hash2);
        }
    }

    [TestClass]
    public class Given_ProjectComparer_with_different_projects : ContextSpecification
    {
        private IProject _project1 = null!;
        private IProject _project2 = null!;

        public override void Given()
        {
            // Each MockProject with different store gets different Guid
            var store1 = new MockWorkItemStore();
            var store2 = new MockWorkItemStore();
            _project1 = new MockProject(store1);
            _project2 = new MockProject(store2);
        }

        [TestMethod]
        public void Then_different_projects_are_not_equal()
        {
            // Different Guids should make them not equal
            Comparer.Project.Equals(_project1, _project2).ShouldBeFalse();
        }
    }
}
