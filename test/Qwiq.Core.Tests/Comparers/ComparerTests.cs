using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Comparers
{
    /// <summary>
    /// Tests for NullableIdentifiableComparer.
    /// </summary>
    [TestClass]
    public class Given_NullableIdentifiableComparer_with_two_items_same_id : ContextSpecification
    {
        private bool _result;
        private MockWorkItem _item1 = null!;
        private MockWorkItem _item2 = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Bug");
            _item1 = new MockWorkItem(workItemType, 123);
            _item2 = new MockWorkItem(workItemType, 123);
        }

        public override void When()
        {
            _result = NullableIdentifiableComparer.Default.Equals(_item1, _item2);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_NullableIdentifiableComparer_with_two_items_different_id : ContextSpecification
    {
        private bool _result;
        private MockWorkItem _item1 = null!;
        private MockWorkItem _item2 = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Bug");
            _item1 = new MockWorkItem(workItemType, 123);
            _item2 = new MockWorkItem(workItemType, 456);
        }

        public override void When()
        {
            _result = NullableIdentifiableComparer.Default.Equals(_item1, _item2);
        }

        [TestMethod]
        public void Then_returns_false()
        {
            _result.ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_NullableIdentifiableComparer_with_null_items : ContextSpecification
    {
        private MockWorkItem _item = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Bug");
            _item = new MockWorkItem(workItemType, 123);
        }

        [TestMethod]
        public void Then_null_null_returns_true()
        {
            NullableIdentifiableComparer.Default.Equals(null, null).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_item_null_returns_false()
        {
            NullableIdentifiableComparer.Default.Equals(_item, null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_null_item_returns_false()
        {
            NullableIdentifiableComparer.Default.Equals(null, _item).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_NullableIdentifiableComparer_GetHashCode : ContextSpecification
    {
        private MockWorkItem _item = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Bug");
            _item = new MockWorkItem(workItemType, 123);
        }

        [TestMethod]
        public void Then_null_returns_zero()
        {
            NullableIdentifiableComparer.Default.GetHashCode(null!).ShouldBe(0);
        }

        [TestMethod]
        public void Then_item_returns_non_zero_hashcode()
        {
            NullableIdentifiableComparer.Default.GetHashCode(_item).ShouldNotBe(0);
        }
    }

    /// <summary>
    /// Tests for IdentifiableComparer.
    /// </summary>
    [TestClass]
    public class Given_IdentifiableComparer_with_two_items_same_id : ContextSpecification
    {
        private bool _result;
        private MockWorkItem _item1 = null!;
        private MockWorkItem _item2 = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Bug");
            _item1 = new MockWorkItem(workItemType, 456);
            _item2 = new MockWorkItem(workItemType, 456);
        }

        public override void When()
        {
            _result = IdentifiableComparer.Default.Equals(_item1, _item2);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    /// <summary>
    /// Tests for WorkItemComparer.
    /// </summary>
    [TestClass]
    public class Given_WorkItemComparer_with_same_work_items : ContextSpecification
    {
        private bool _result;
        private MockWorkItem _wi1 = null!;
        private MockWorkItem _wi2 = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Bug");
            _wi1 = new MockWorkItem(workItemType, 100);
            _wi2 = new MockWorkItem(workItemType, 100);
        }

        public override void When()
        {
            _result = WorkItemComparer.Default.Equals(_wi1, _wi2);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_WorkItemComparer_with_different_ids : ContextSpecification
    {
        private bool _result;
        private MockWorkItem _wi1 = null!;
        private MockWorkItem _wi2 = null!;

        public override void Given()
        {
            var workItemType = new MockWorkItemType("Bug");
            _wi1 = new MockWorkItem(workItemType, 100);
            _wi2 = new MockWorkItem(workItemType, 200);
        }

        public override void When()
        {
            _result = WorkItemComparer.Default.Equals(_wi1, _wi2);
        }

        [TestMethod]
        public void Then_returns_false()
        {
            _result.ShouldBeFalse();
        }
    }

    /// <summary>
    /// Tests for WorkItemTypeComparer.
    /// </summary>
    [TestClass]
    public class Given_WorkItemTypeComparer_with_same_types : ContextSpecification
    {
        private bool _result;

        public override void When()
        {
            var type1 = new MockWorkItemType("Bug", "Test Project");
            var type2 = new MockWorkItemType("Bug", "Test Project");
            _result = WorkItemTypeComparer.Default.Equals(type1, type2);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_WorkItemTypeComparer_with_different_names : ContextSpecification
    {
        private bool _result;

        public override void When()
        {
            var type1 = new MockWorkItemType("Bug", "Test Project");
            var type2 = new MockWorkItemType("Task", "Test Project");
            _result = WorkItemTypeComparer.Default.Equals(type1, type2);
        }

        [TestMethod]
        public void Then_returns_false()
        {
            _result.ShouldBeFalse();
        }
    }

    /// <summary>
    /// Tests for GenericComparer.
    /// </summary>
    [TestClass]
    public class Given_GenericComparer_with_string_items : ContextSpecification
    {
        private bool _result;

        public override void When()
        {
            _result = GenericComparer<string>.Default.Equals("test", "test");
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_GenericComparer_with_different_strings : ContextSpecification
    {
        private bool _result;

        public override void When()
        {
            _result = GenericComparer<string>.Default.Equals("test1", "test2");
        }

        [TestMethod]
        public void Then_returns_false()
        {
            _result.ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_GenericComparer_with_null : ContextSpecification
    {
        [TestMethod]
        public void Then_null_null_returns_true()
        {
            GenericComparer<string>.Default.Equals(null, null).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_value_null_returns_false()
        {
            GenericComparer<string>.Default.Equals("test", null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_GetHashCode_null_returns_zero()
        {
            GenericComparer<string>.Default.GetHashCode(null!).ShouldBe(0);
        }

        [TestMethod]
        public void Then_null_value_returns_false()
        {
            GenericComparer<string>.Default.Equals(null, "test").ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_GenericComparer_with_same_reference : ContextSpecification
    {
        private string _item = "test";

        [TestMethod]
        public void Then_same_reference_returns_true()
        {
            GenericComparer<string>.Default.Equals(_item, _item).ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_GenericComparer_with_int_values : ContextSpecification
    {
        [TestMethod]
        public void Then_equal_ints_return_true()
        {
            GenericComparer<int>.Default.Equals(42, 42).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_different_ints_return_false()
        {
            GenericComparer<int>.Default.Equals(42, 43).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Compare_returns_zero_for_equal()
        {
            GenericComparer<int>.Default.Compare(42, 42).ShouldBe(0);
        }

        [TestMethod]
        public void Then_Compare_returns_negative_for_less()
        {
            GenericComparer<int>.Default.Compare(42, 43).ShouldBeLessThan(0);
        }

        [TestMethod]
        public void Then_Compare_returns_positive_for_greater()
        {
            GenericComparer<int>.Default.Compare(43, 42).ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public void Then_GetHashCode_returns_same_for_equal_values()
        {
            GenericComparer<int>.Default.GetHashCode(42).ShouldBe(GenericComparer<int>.Default.GetHashCode(42));
        }
    }

    [TestClass]
    public class Given_GenericComparer_with_nullable_int_values : ContextSpecification
    {
        [TestMethod]
        public void Then_null_null_returns_zero()
        {
            GenericComparer<int?>.Default.Compare(null, null).ShouldBe(0);
        }

        [TestMethod]
        public void Then_value_null_returns_negative()
        {
            GenericComparer<int?>.Default.Compare(42, null).ShouldBeLessThan(0);
        }

        [TestMethod]
        public void Then_null_value_returns_negative()
        {
            GenericComparer<int?>.Default.Compare(null, 42).ShouldBeLessThan(0);
        }

        [TestMethod]
        public void Then_equal_values_return_zero()
        {
            GenericComparer<int?>.Default.Compare(42, 42).ShouldBe(0);
        }
    }

    [TestClass]
    public class Given_GenericComparer_with_array_values : ContextSpecification
    {
        [TestMethod]
        public void Then_equal_arrays_return_true()
        {
            var arr1 = new[] { 1, 2, 3 };
            var arr2 = new[] { 1, 2, 3 };
            GenericComparer<int[]>.Default.Equals(arr1, arr2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_different_arrays_return_false()
        {
            var arr1 = new[] { 1, 2, 3 };
            var arr2 = new[] { 1, 2, 4 };
            GenericComparer<int[]>.Default.Equals(arr1, arr2).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_different_length_arrays_return_false()
        {
            var arr1 = new[] { 1, 2, 3 };
            var arr2 = new[] { 1, 2 };
            GenericComparer<int[]>.Default.Equals(arr1, arr2).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_empty_arrays_return_true()
        {
            var arr1 = Array.Empty<int>();
            var arr2 = Array.Empty<int>();
            GenericComparer<int[]>.Default.Equals(arr1, arr2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_returns_same_for_equal_arrays()
        {
            var arr1 = new[] { 1, 2, 3 };
            var arr2 = new[] { 1, 2, 3 };
            GenericComparer<int[]>.Default.GetHashCode(arr1).ShouldBe(GenericComparer<int[]>.Default.GetHashCode(arr2));
        }

        [TestMethod]
        public void Then_GetHashCode_returns_different_for_different_arrays()
        {
            var arr1 = new[] { 1, 2, 3 };
            var arr2 = new[] { 4, 5, 6 };
            GenericComparer<int[]>.Default.GetHashCode(arr1).ShouldNotBe(GenericComparer<int[]>.Default.GetHashCode(arr2));
        }

        [TestMethod]
        public void Then_array_with_null_null_return_false()
        {
            var arr1 = new[] { 1, 2, 3 };
            GenericComparer<int[]>.Default.Equals(arr1, null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_null_array_return_false()
        {
            var arr2 = new[] { 1, 2, 3 };
            GenericComparer<int[]>.Default.Equals(null, arr2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_GenericComparer_with_list_values : ContextSpecification
    {
        [TestMethod]
        public void Then_equal_lists_return_true()
        {
            var list1 = new List<string> { "a", "b", "c" };
            var list2 = new List<string> { "a", "b", "c" };
            GenericComparer<List<string>>.Default.Equals(list1, list2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_different_lists_return_false()
        {
            var list1 = new List<string> { "a", "b", "c" };
            var list2 = new List<string> { "a", "b", "d" };
            GenericComparer<List<string>>.Default.Equals(list1, list2).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_list_with_nulls_handled()
        {
            var list1 = new List<string?> { "a", null, "c" };
            var list2 = new List<string?> { "a", null, "c" };
            GenericComparer<List<string?>>.Default.Equals(list1, list2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_handles_null_items()
        {
            var list = new List<string?> { "a", null, "c" };
            // Should not throw
            var hash = GenericComparer<List<string?>>.Default.GetHashCode(list);
            hash.ShouldNotBe(0);
        }
    }

    [TestClass]
    public class Given_GenericComparer_with_DateTime_values : ContextSpecification
    {
        [TestMethod]
        public void Then_equal_dates_return_true()
        {
            var date1 = new DateTime(2025, 1, 1);
            var date2 = new DateTime(2025, 1, 1);
            GenericComparer<DateTime>.Default.Equals(date1, date2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_Compare_returns_negative_for_earlier_date()
        {
            var earlier = new DateTime(2025, 1, 1);
            var later = new DateTime(2025, 12, 31);
            GenericComparer<DateTime>.Default.Compare(earlier, later).ShouldBeLessThan(0);
        }

        [TestMethod]
        public void Then_Compare_returns_positive_for_later_date()
        {
            var earlier = new DateTime(2025, 1, 1);
            var later = new DateTime(2025, 12, 31);
            GenericComparer<DateTime>.Default.Compare(later, earlier).ShouldBeGreaterThan(0);
        }
    }

    [TestClass]
    public class Given_GenericComparer_with_object_values : ContextSpecification
    {
        private class SimpleObject
        {
            public int Value { get; set; }

            public override bool Equals(object? obj) => obj is SimpleObject other && Value == other.Value;
            public override int GetHashCode() => Value.GetHashCode();
        }

        [TestMethod]
        public void Then_equal_objects_return_true()
        {
            var obj1 = new SimpleObject { Value = 42 };
            var obj2 = new SimpleObject { Value = 42 };
            GenericComparer<SimpleObject>.Default.Equals(obj1, obj2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_different_objects_return_false()
        {
            var obj1 = new SimpleObject { Value = 42 };
            var obj2 = new SimpleObject { Value = 43 };
            GenericComparer<SimpleObject>.Default.Equals(obj1, obj2).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_GetHashCode_returns_object_hash()
        {
            var obj = new SimpleObject { Value = 42 };
            GenericComparer<SimpleObject>.Default.GetHashCode(obj).ShouldBe(obj.GetHashCode());
        }
    }

    [TestClass]
    public class Given_GenericComparer_Compare_with_IEquatable : ContextSpecification
    {
        private class EquatableOnly : IEquatable<EquatableOnly>
        {
            public int Value { get; set; }
            public bool Equals(EquatableOnly? other) => other != null && Value == other.Value;
            public override bool Equals(object? obj) => obj is EquatableOnly other && Equals(other);
            public override int GetHashCode() => Value.GetHashCode();
        }

        [TestMethod]
        public void Then_equal_equatables_return_zero()
        {
            var eq1 = new EquatableOnly { Value = 42 };
            var eq2 = new EquatableOnly { Value = 42 };
            GenericComparer<EquatableOnly>.Default.Compare(eq1, eq2).ShouldBe(0);
        }

        [TestMethod]
        public void Then_different_equatables_return_negative()
        {
            var eq1 = new EquatableOnly { Value = 42 };
            var eq2 = new EquatableOnly { Value = 43 };
            GenericComparer<EquatableOnly>.Default.Compare(eq1, eq2).ShouldBeLessThan(0);
        }
    }

    [TestClass]
    public class Given_GenericComparer_Compare_enumerable_branch_coverage : ContextSpecification
    {
        [TestMethod]
        public void Then_shorter_array_first_returns_negative()
        {
            var arr1 = new[] { 1, 2 };
            var arr2 = new[] { 1, 2, 3 };
            GenericComparer<int[]>.Default.Compare(arr1, arr2).ShouldBeLessThan(0);
        }

        [TestMethod]
        public void Then_longer_array_first_returns_negative()
        {
            var arr1 = new[] { 1, 2, 3 };
            var arr2 = new[] { 1, 2 };
            GenericComparer<int[]>.Default.Compare(arr1, arr2).ShouldBeLessThan(0);
        }

        [TestMethod]
        public void Then_single_element_arrays_equal()
        {
            var arr1 = new[] { 42 };
            var arr2 = new[] { 42 };
            GenericComparer<int[]>.Default.Compare(arr1, arr2).ShouldBe(0);
        }
    }

    /// <summary>
    /// Tests for QueryDefinitionComparer.
    /// </summary>
    [TestClass]
    public class Given_QueryDefinitionComparer_with_same_queries : ContextSpecification
    {
        private bool _result;
        private MockQueryDefinition _query1 = null!;
        private MockQueryDefinition _query2 = null!;
        private Guid _id;

        public override void Given()
        {
            _id = Guid.NewGuid();
            _query1 = new MockQueryDefinition(_id, "Query", "SELECT * FROM WorkItems", "/path1");
            _query2 = new MockQueryDefinition(_id, "Query", "SELECT * FROM WorkItems WHERE [Id] = 1", "/path2");
        }

        public override void When()
        {
            _result = QueryDefinitionComparer.Default.Equals(_query1, _query2);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_QueryDefinitionComparer_with_different_ids : ContextSpecification
    {
        private bool _result;
        private MockQueryDefinition _query1 = null!;
        private MockQueryDefinition _query2 = null!;

        public override void Given()
        {
            _query1 = new MockQueryDefinition(Guid.NewGuid(), "Query", "SELECT * FROM WorkItems", "/path");
            _query2 = new MockQueryDefinition(Guid.NewGuid(), "Query", "SELECT * FROM WorkItems", "/path");
        }

        public override void When()
        {
            _result = QueryDefinitionComparer.Default.Equals(_query1, _query2);
        }

        [TestMethod]
        public void Then_returns_false()
        {
            _result.ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_QueryDefinitionComparer_with_different_names : ContextSpecification
    {
        private bool _result;
        private MockQueryDefinition _query1 = null!;
        private MockQueryDefinition _query2 = null!;
        private Guid _id;

        public override void Given()
        {
            _id = Guid.NewGuid();
            _query1 = new MockQueryDefinition(_id, "Query1", "SELECT * FROM WorkItems", "/path");
            _query2 = new MockQueryDefinition(_id, "Query2", "SELECT * FROM WorkItems", "/path");
        }

        public override void When()
        {
            _result = QueryDefinitionComparer.Default.Equals(_query1, _query2);
        }

        [TestMethod]
        public void Then_returns_false()
        {
            _result.ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_QueryDefinitionComparer_with_null : ContextSpecification
    {
        private MockQueryDefinition _query = null!;

        public override void Given()
        {
            _query = new MockQueryDefinition(Guid.NewGuid(), "Query", "SELECT * FROM WorkItems", "/path");
        }

        [TestMethod]
        public void Then_null_null_returns_true()
        {
            QueryDefinitionComparer.Default.Equals(null, null).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_query_null_returns_false()
        {
            QueryDefinitionComparer.Default.Equals(_query, null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_null_query_returns_false()
        {
            QueryDefinitionComparer.Default.Equals(null, _query).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_GetHashCode_null_returns_zero()
        {
            QueryDefinitionComparer.Default.GetHashCode(null!).ShouldBe(0);
        }

        [TestMethod]
        public void Then_GetHashCode_returns_non_zero()
        {
            QueryDefinitionComparer.Default.GetHashCode(_query).ShouldNotBe(0);
        }
    }

    [TestClass]
    public class Given_QueryDefinitionComparer_with_same_reference : ContextSpecification
    {
        private bool _result;
        private MockQueryDefinition _query = null!;

        public override void Given()
        {
            _query = new MockQueryDefinition(Guid.NewGuid(), "Query", "SELECT * FROM WorkItems", "/path");
        }

        public override void When()
        {
            _result = QueryDefinitionComparer.Default.Equals(_query, _query);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_QueryDefinitionComparer_with_case_insensitive_names : ContextSpecification
    {
        private bool _result;
        private MockQueryDefinition _query1 = null!;
        private MockQueryDefinition _query2 = null!;
        private Guid _id;

        public override void Given()
        {
            _id = Guid.NewGuid();
            _query1 = new MockQueryDefinition(_id, "Query", "SELECT * FROM WorkItems", "/path");
            _query2 = new MockQueryDefinition(_id, "QUERY", "SELECT * FROM WorkItems", "/path");
        }

        public override void When()
        {
            _result = QueryDefinitionComparer.Default.Equals(_query1, _query2);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }

        [TestMethod]
        public void Then_hash_codes_are_same()
        {
            QueryDefinitionComparer.Default.GetHashCode(_query1)
                .ShouldBe(QueryDefinitionComparer.Default.GetHashCode(_query2));
        }
    }

    /// <summary>
    /// Tests for QueryFolderComparer.
    /// </summary>
    [TestClass]
    public class Given_QueryFolderComparer_with_null : ContextSpecification
    {
        private MockQueryFolder _folder = null!;

        public override void Given()
        {
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _folder = new MockQueryFolder(Guid.NewGuid(), "Folder", "/path", subFolders, queries);
        }

        [TestMethod]
        public void Then_null_null_returns_true()
        {
            QueryFolderComparer.Default.Equals(null, null).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_folder_null_returns_false()
        {
            QueryFolderComparer.Default.Equals(_folder, null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_null_folder_returns_false()
        {
            QueryFolderComparer.Default.Equals(null, _folder).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_GetHashCode_null_returns_zero()
        {
            QueryFolderComparer.Default.GetHashCode(null!).ShouldBe(0);
        }

        [TestMethod]
        public void Then_GetHashCode_returns_non_zero()
        {
            QueryFolderComparer.Default.GetHashCode(_folder).ShouldNotBe(0);
        }
    }

    [TestClass]
    public class Given_QueryFolderComparer_with_same_reference : ContextSpecification
    {
        private bool _result;
        private MockQueryFolder _folder = null!;

        public override void Given()
        {
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _folder = new MockQueryFolder(Guid.NewGuid(), "Folder", "/path", subFolders, queries);
        }

        public override void When()
        {
            _result = QueryFolderComparer.Default.Equals(_folder, _folder);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_QueryFolderComparer_with_different_ids : ContextSpecification
    {
        private bool _result;
        private MockQueryFolder _folder1 = null!;
        private MockQueryFolder _folder2 = null!;

        public override void Given()
        {
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _folder1 = new MockQueryFolder(Guid.NewGuid(), "Folder", "/path", subFolders, queries);
            _folder2 = new MockQueryFolder(Guid.NewGuid(), "Folder", "/path", subFolders, queries);
        }

        public override void When()
        {
            _result = QueryFolderComparer.Default.Equals(_folder1, _folder2);
        }

        [TestMethod]
        public void Then_returns_false()
        {
            _result.ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_QueryFolderComparer_with_different_names : ContextSpecification
    {
        private bool _result;
        private MockQueryFolder _folder1 = null!;
        private MockQueryFolder _folder2 = null!;
        private Guid _id;

        public override void Given()
        {
            _id = Guid.NewGuid();
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _folder1 = new MockQueryFolder(_id, "Folder1", "/path", subFolders, queries);
            _folder2 = new MockQueryFolder(_id, "Folder2", "/path", subFolders, queries);
        }

        public override void When()
        {
            _result = QueryFolderComparer.Default.Equals(_folder1, _folder2);
        }

        [TestMethod]
        public void Then_returns_false()
        {
            _result.ShouldBeFalse();
        }
    }
}
