using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;
using Should.Core.Exceptions;

namespace Microsoft.Qwiq.Linq
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_where_clause_with_an_and_expression : WiqlQueryBuilderContextSpecification
    {
        private DateTime _date;

        public override void Given()
        {
            base.Given();
            _date = DateTime.SpecifyKind(new DateTime(2012, 11, 29, 17, 0, 0), DateTimeKind.Utc);
        }

        public override void When()
        {
            base.When();
            Expected =
                "SELECT * FROM WorkItems WHERE ((([Title] = 'asdf') AND ([Changed Date] > '2012-11-29 17:00:00Z')))";
            Actual = Query.Where(item => item.Title == "asdf" && item.ChangedDate > _date).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_a_where_with_an_and_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_an_asof_clause : WiqlQueryBuilderContextSpecification
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
            Expected = "SELECT * FROM WorkItems ASOF '2014-06-01 07:00:00Z'";
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
    public class when_a_query_has_chained_where_clauses : WiqlQueryBuilderContextSpecification
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
            Expected =
                "SELECT * FROM WorkItems WHERE (([Title] = 'asdf') AND ([Tags] = 'String Value') AND ([Created Date] > '2012-11-29 17:00:00Z'))";
            Actual =
                Query.Where(item => item.Title == "asdf")
                    .Where(item => item.Tags == "String Value")
                    .Where(item => item.CreatedDate > _date)
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
    public class when_a_query_has_a_where_clause_with_an_or_expression : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE ((([Tags] = 'person1') OR ([Tags] = 'person2')))";
            Actual = Query.Where(item => item.Tags == "person1" || item.Tags == "person2").ToString();
        }

        [TestMethod]
        public void it_is_translated_to_a_where_with_an_or_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_field_that_should_be_in_a_list_of_string_array_values : WiqlQueryBuilderContextSpecification
    {
        private string[] _values;

        public override void Given()
        {
            _values = new[] {"person1", "person2"};
            base.Given();
        }

        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Tags] IN ('person1', 'person2')))";
            Actual = Query.Where(item => _values.Contains(item.Tags)).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_an_in_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_field_that_should_be_in_a_list_of_IEnumerable_string_values : WiqlQueryBuilderContextSpecification
    {
        private IEnumerable<string> _values;

        public override void Given()
        {
            _values = new[] { "person1", "person2" }.AsEnumerable();
            base.Given();
        }

        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Tags] IN ('person1', 'person2')))";
            Actual = Query.Where(item => _values.Contains(item.Tags)).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_an_in_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_field_that_should_be_in_a_list_of_Collection_string_values : WiqlQueryBuilderContextSpecification
    {
        private Collection<string> _values;

        public override void Given()
        {
            _values = new Collection<string> { "person1", "person2" };
            base.Given();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void it_is_not_supported()
        {
            Actual = Query.Where(item => _values.Contains(item.Tags)).ToString();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_field_that_should_be_in_a_list_of_HashSet_string_values : WiqlQueryBuilderContextSpecification
    {
        private HashSet<string> _values;

        public override void Given()
        {
            _values = new HashSet<string>
            {
                "person1",
                "person2"
            };
            base.Given();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void it_is_not_supported()
        {
            Actual = Query.Where(item => _values.Contains(item.Tags)).ToString();
        }
    }


    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_field_compared_with_null : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Title] <> ''))";
            Actual = Query.Where(item => item.Title != null).ToString();
        }

        [TestMethod]
        public void the_null_is_converted_to_the_empty_string()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_field_comparison_of_greater_than : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Id] > 1))";
            Actual = Query.Where(item => item.Id > 1).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_the_greater_than_symbol()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_uses_the_not_equals_operator : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Id] <> 1234))";
            Actual = Query.Where(item => item.Id != 1234).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_less_than_greater_than()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_has_a_lazy_ienumerable_in_the_expression : WiqlQueryBuilderContextSpecification
    {
        private readonly string[] _aliases = {"person1", "person2"};
        private IEnumerable<string> _filteredAliases;

        public override void Given()
        {
            _filteredAliases = _aliases.Where(alias => alias == "person1");
            base.Given();
        }

        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Assigned To] IN ('person1')))";
            Actual = Query.Where(item => _filteredAliases.Contains(item.AssignedTo)).ToString();
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
    public class when_a_constant_has_a_special_wiql_character : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Assigned To] = 'Robert O''Sullivan'))";
            Actual = Query.Where(item => item.AssignedTo == "Robert O'Sullivan").ToString();

        }

        [TestMethod]
        public void it_is_escaped()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_an_ienumerable_contains_constants_with_special_wiql_characters : WiqlQueryBuilderContextSpecification
    {
        private readonly string[] _values = {"Robert O'Sullivan", "Robert O'Laney"};

        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Assigned To] IN ('Robert O''Sullivan', 'Robert O''Laney')))";
            Actual = Query.Where(item => _values.Contains(item.AssignedTo)).ToString();
        }

        [TestMethod]
        public void each_string_is_escaped()
        {
            Expected.ShouldEqual(Actual);
        }
    }


    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_contains_an_OrderByDescending_and_ThenBy_clauses : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems ORDER BY [Title] desc, [Revised Date] asc";
            Actual = Query.OrderByDescending(item => item.Title).ThenBy(bug => bug.RevisedDate).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_order_by_desc_with_other_sorts_asc()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_contains_a_ToUpper_call_on_a_string : WiqlQueryBuilderContextSpecification
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void a_NotSupportedException_is_thrown_to_notify_the_developer_that_text_matches_are_case_insensitive()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Actual = Query.Where(item => item.Title.ToUpper() == "TEST").ToString();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_uses_the_StartsWith_string_function : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Area Path] UNDER 'path1\\path2\\path3'))";
            Actual = Query.Where(item => item.AreaPath.StartsWith(@"path1\path2\path3")).ToString();
        }

        [TestMethod]
        public void the_StartsWith_is_translated_to_an_under_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_uses_the_ever_function : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Assigned To] EVER 'alias'))";
            Actual = Query.Where(item => item.AssignedTo.WasEver("alias")).ToString();
        }

        [TestMethod]
        public void the_WasEver_is_translated_to_an_ever_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_uses_the_Contains_string_function : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Tags] CONTAINS 'Obsolete'))";
            Actual = Query.Where(item => item.Tags.Contains("Obsolete")).ToString();
        }

        [TestMethod]
        public void the_Contains_is_translated_to_an_under_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }


    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_is_chained_to_a_select_clause : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Id] > 1))";
            Actual = Query.Where(item => item.Id > 1).Select(item => new {One = item.Id, Two = item.Title}).ToString();
        }

        [TestMethod]
        public void the_where_clause_is_preserved_in_the_query()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_where_clause_with_a_ToString_in_it : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Id] = '42'))";
            Actual = Query.Where(item => item.Id.ToString() == "42").ToString();
        }

        [TestMethod]
        public void the_ToString_call_is_ignored()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_where_clause_with_a_type_that_uses_an_indexer : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Some Property] = 'Some Value'))";
            Actual = Query.Where(item => item["Some Property"].ToString() == "Some Value").ToString();
        }

        [TestMethod]
        public void the_index_name_is_used_as_a_field_name()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    public class Given_a_query_with_a_where_clause_with_an_enum : WiqlQueryBuilderContextSpecification
    {
        enum Sample { One, Two, Three}

        interface IWorkItem2 : IWorkItem
        {
            Sample EnumProperty { get; }
        }

        private new IOrderedQueryable<IWorkItem2> Query;

        /// <inheritdoc />
        public override void Given()
        {
            base.Given();
            Query = new Query<IWorkItem2>(QueryProvider, WiqlQueryBuilder);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void the_enum_is_translated()
        {
            Actual = Query.Where(item => item.EnumProperty == Sample.Three).ToString();
        }
    }

    [TestClass]
    public class Given_a_query_with_a_single_variable : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Id] = 1))";
            int id = 1;
            Actual = Query.Where(item => item.Id == id).ToString();
        }

        [TestMethod]
        public void the_variable_value_is_written_to_the_wiql()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class Given_a_query_with_a_where_clause_on_a_known_identity_property_with_a_combo_value : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Assigned To] = 'Dan Jump <danj@contoso.com>'))";
            Actual = Query.Where(item => item.AssignedTo == "Dan Jump <danj@contoso.com>").ToString();
        }

        [TestMethod]
        public void the_value_is_written_to_WIQL()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class Given_a_query_with_a_where_clause_on_a_known_identity_property_with_an_alias : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Assigned To] = 'danj'))";
            Actual = Query.Where(item => item.AssignedTo == "danj").ToString();
        }

        [TestMethod]
        public void the_value_is_written_to_WIQL()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class Given_a_query_with_a_where_clause_on_a_identity_via_indexer : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Assigned To] = 'danj'))";
            Actual = Query.Where(item => item["Assigned To"].ToString() == "danj").ToString();
        }

        [TestMethod]
        public void the_value_is_written_to_WIQL()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class Given_a_query_with_a_where_clause_on_a_identity_via_fields_indexer : WiqlQueryBuilderContextSpecification
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE (([Assigned To] = 'danj'))";
            Actual = Query.Where(item => item.Fields["Assigned To"].ToString() == "danj").ToString();
        }

        [TestMethod]
        public void the_value_is_written_to_WIQL()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    public class Given_a_query_with_a_projection : WiqlQueryBuilderContextSpecification
    {
        /// <inheritdoc />
        public override void When()
        {
            Expected = "SELECT Id FROM WorkItems WHERE (([Id] = 1))";
            Actual = Query.Where(item => item.Id == 1).Select(s => s.Id).ToString();
        }

        [TestMethod]
        [ExpectedException(typeof(EqualException))]
        public void the_column_written_to_WIQL_in_SELECT_is_the_projected_property()
        {
            Actual.ShouldEqual(Expected);
        }
    }
}
