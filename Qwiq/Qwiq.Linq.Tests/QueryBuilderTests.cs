using System;
using System.Collections.Generic;
using System.Linq;
using Should;
using Microsoft.Qwiq.Linq.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Qwiq.Linq.Tests
{
    public abstract class GenericQueryBuilderTestsBase<T> : QueryTestsBase
    {
        protected Query<T> Query;
        protected string Expected;
        protected string Actual;
        protected IEnumerable<string> FieldNames;

        public override void Given()
        {
            PropertyReflector = new MockPropertyReflector();
            base.Given();
        }

        public override void When()
        {
            base.When();
            Query = new Query<T>(QueryProvider, Builder);
            FieldNames = FieldMapper.GetFieldNames(typeof(T));
        }
    }

    public abstract class QueryBuilderTestsBase : GenericQueryBuilderTestsBase<MockModel>
    {
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_where_clause_with_an_and_expression : QueryBuilderTestsBase
    {
        private DateTime _date;

        public override void Given()
        {
            _date = DateTime.SpecifyKind(new DateTime(2012, 11, 29, 17, 0, 0), DateTimeKind.Utc);
            base.Given();
        }

        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE ((([StringField] = 'asdf') AND ([DateTimeField] > '2012-11-29 17:00:00Z')) AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => item.StringField == "asdf" && item.DateTimeField > _date).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_a_where_with_an_and_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_an_asof_clause : QueryBuilderTestsBase
    {
        private DateTime _date;

        public override void Given()
        {
            // Use .Parse so we can specify the timezone we want (i.e. the build machine may be in a different time zone)
            _date = DateTime.Parse("2014-06-01T00:00:00-7:00");
            base.Given();
        }

        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([Work Item Type] = 'MockWorkItem')) ASOF '2014-06-01 07:00:00Z'";
            Actual = Query.AsOf(_date).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_an_asof_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_chained_where_clauses : QueryBuilderTestsBase
    {
        private DateTime _date;

        public override void Given()
        {
            _date = DateTime.SpecifyKind(new DateTime(2012, 11, 29, 17, 0, 0), DateTimeKind.Utc);
            base.Given();
        }

        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([Field with Spaces] = 'asdf') AND ([StringField] = 'String Value') AND ([DateTimeField] > '2012-11-29 17:00:00Z') AND ([Work Item Type] = 'MockWorkItem'))";
            Actual =
                Query.Where(item => item.FieldWithSpaces == "asdf")
                    .Where(item => item.StringField == "String Value")
                    .Where(item => item.DateTimeField > _date)
                    .ToString();
        }

        [TestMethod]
        public void it_is_translated_to_a_list_of_and_operators()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_where_clause_with_an_or_expression : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE ((([StringField] = 'person1') OR ([StringField] = 'person2')) AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => item.StringField == "person1" || item.StringField == "person2").ToString();
        }

        [TestMethod]
        public void it_is_translated_to_a_where_with_an_or_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_field_that_should_be_in_a_list_of_values : QueryBuilderTestsBase
    {
        private string[] _values;
        public override void Given()
        {
            _values = new[] { "person1", "person2" };
            base.Given();
        }

        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([StringField] IN ('person1', 'person2')) AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => _values.Contains(item.StringField)).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_an_in_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_is_on_a_field_that_is_nullable : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([NullableField] = 1) AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => item.NullableField.Value == 1).ToString();
        }

        [TestMethod]
        public void the_value_of_the_field_is_compared()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_field_compared_with_null : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([NullableField] <> '') AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => item.NullableField != null).ToString();
        }

        [TestMethod]
        public void the_null_is_converted_to_the_empty_string()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_field_comparison_of_greater_than : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([IntField] > 1) AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => item.IntField > 1).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_the_greater_than_symbol()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_uses_the_not_equals_operator : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([IntField] <> 1234) AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => item.IntField != 1234).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_less_than_greater_than()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_has_a_lazy_ienumerable_in_the_expression : QueryBuilderTestsBase
    {
        private readonly string[] _aliases = { "person1", "person2" };
        private IEnumerable<string> _filteredAliases;

        public override void Given()
        {
            _filteredAliases = _aliases.Where(alias => alias == "person1");
            base.Given();
        }

        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([StringField] IN ('person1')) AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => _filteredAliases.Contains(item.StringField)).ToString();
            base.When();
        }

        [TestMethod]
        public void the_ienumerable_is_enumerated_prior_to_translation()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_constant_has_a_special_wiql_character : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([StringField] = 'Robert O''Sullivan') AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => item.StringField == "Robert O'Sullivan").ToString();

        }

        [TestMethod]
        public void it_is_escaped()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_an_ienumerable_contains_constants_with_special_wiql_characters : QueryBuilderTestsBase
    {
        private readonly string[] _values = { "Robert O'Sullivan", "Robert O'Laney" };

        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([StringField] IN ('Robert O''Sullivan', 'Robert O''Laney')) AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => _values.Contains(item.StringField)).ToString();
        }

        [TestMethod]
        public void each_string_is_escaped()
        {
            Expected.ShouldEqual(Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_filters_on_a_field_with_no_field_definition_attribute : QueryBuilderTestsBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void an_argument_exception_is_thrown()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Query.Where(item => item.UnmappedProperty != null).ToString();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_contains_an_OrderByDescending_and_ThenBy_clauses : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([Work Item Type] = 'MockWorkItem')) ORDER BY [StringField] desc, [DateTimeField] asc";
            Actual = Query.OrderByDescending(item => item.StringField).ThenBy(bug => bug.DateTimeField).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_order_by_desc_with_other_sorts_asc()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_contains_a_ToUpper_call_on_a_string : QueryBuilderTestsBase
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void a_NotSupportedException_is_thrown_to_notify_the_developer_that_text_matches_are_case_insensitive()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Query.Where(item => item.StringField.ToUpper() == "TEST").ToString();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_uses_the_StartsWith_string_function : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([StringField] UNDER 'path1\\path2\\path3') AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => item.StringField.StartsWith(@"path1\path2\path3")).ToString();
        }

        [TestMethod]
        public void the_StartsWith_is_translated_to_an_under_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_uses_the_Contains_string_function : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([StringField] CONTAINS 'Obsolete') AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => item.StringField.Contains("Obsolete")).ToString();
        }

        [TestMethod]
        public void the_Contains_is_translated_to_an_under_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_select_clause_is_used : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Select(item => new { One = item.IntField, Two = item.IntField }).ToString();
        }

        [TestMethod]
        public void the_query_string_contains_all_fields_for_work_item()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_two_select_clauses_are_chained : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Select(item => new { One = item.IntField, Two = item.IntField }).Select(item2 => new { ABC = item2.Two }).ToString();
        }

        [TestMethod]
        public void the_query_string_contains_all_fields_for_work_item()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_is_chained_to_a_select_clause : QueryBuilderTestsBase
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([IntField] > 1) AND ([Work Item Type] = 'MockWorkItem'))";
            Actual = Query.Where(item => item.IntField > 1).Select(item => new { One = item.IntField, Two = item.IntField }).ToString();
        }

        [TestMethod]
        public void the_where_clause_is_preserved_in_the_query()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_is_against_a_type_with_no_workitemtype_attribute : GenericQueryBuilderTestsBase<MockModelNoType>
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE ((([ID] = 1) AND ([IntField] > 5)))";
            Actual = Query.Where(item => item.Id == 1 && item.IntField > 5).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_a_where_with_no_type_restriction()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_is_against_a_type_with_multiple_workitemtype_attributes : GenericQueryBuilderTestsBase<MockModelMultipleTypes>
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT " + string.Join(", ", FieldNames) + " FROM WorkItems WHERE (([IntField] > 1) AND ([Work Item Type] IN ('Baz', 'Buzz', 'Fizz')))";
            Actual = Query.Where(item => item.IntField > 1).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_a_where_with_multiple_type_restriction()
        {
            Actual.ShouldEqual(Expected);
        }
    }
}

