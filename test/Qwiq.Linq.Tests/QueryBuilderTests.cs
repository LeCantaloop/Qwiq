using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Qwiq.Linq.Visitors;
using Microsoft.Qwiq.Mocks;
using Microsoft.Qwiq.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Microsoft.Qwiq.Linq.Tests
{
    public abstract class QueryBuilderTests : ContextSpecification
    {
        protected string Expected;
        protected string Actual;
        protected IOrderedQueryable<IWorkItem> Query;

        public override void Given()
        {
            base.Given();
            var builder = new WiqlQueryBuilder(new WiqlTranslator(), new PartialEvaluator(), new QueryRewriter());
            var queryProvider = new TeamFoundationServerWorkItemQueryProvider(new MockWorkItemStore(), builder);
            Query = new Query<IWorkItem>(queryProvider, builder);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_where_clause_with_an_and_expression : QueryBuilderTests
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
    public class when_a_query_has_an_asof_clause : QueryBuilderTests
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
    public class when_a_query_has_chained_where_clauses : QueryBuilderTests
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
                "SELECT * FROM WorkItems WHERE (([Title] = 'asdf') AND ([Keywords] = 'String Value') AND ([Created Date] > '2012-11-29 17:00:00Z'))";
            Actual =
                Query.Where(item => item.Title == "asdf")
                    .Where(item => item.Keywords == "String Value")
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
    public class when_a_query_has_a_where_clause_with_an_or_expression : QueryBuilderTests
    {
        public override void When()
        {
            base.When();
            Expected = "SELECT * FROM WorkItems WHERE ((([Keywords] = 'person1') OR ([Keywords] = 'person2')))";
            Actual = Query.Where(item => item.Keywords == "person1" || item.Keywords == "person2").ToString();
        }

        [TestMethod]
        public void it_is_translated_to_a_where_with_an_or_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_field_that_should_be_in_a_list_of_values : QueryBuilderTests
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
            Expected = "SELECT * FROM WorkItems WHERE (([Keywords] IN ('person1', 'person2')))";
            Actual = Query.Where(item => _values.Contains(item.Keywords)).ToString();
        }

        [TestMethod]
        public void it_is_translated_to_an_in_operator()
        {
            Actual.ShouldEqual(Expected);
        }
    }


    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_query_has_a_field_compared_with_null : QueryBuilderTests
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
    public class when_a_query_has_a_field_comparison_of_greater_than : QueryBuilderTests
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
    public class when_a_query_uses_the_not_equals_operator : QueryBuilderTests
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
    public class when_a_where_clause_has_a_lazy_ienumerable_in_the_expression : QueryBuilderTests
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
    public class when_a_constant_has_a_special_wiql_character : QueryBuilderTests
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
    public class when_an_ienumerable_contains_constants_with_special_wiql_characters : QueryBuilderTests
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
    public class when_a_query_contains_an_OrderByDescending_and_ThenBy_clauses : QueryBuilderTests
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
    public class when_a_where_clause_contains_a_ToUpper_call_on_a_string : QueryBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void a_NotSupportedException_is_thrown_to_notify_the_developer_that_text_matches_are_case_insensitive()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Query.Where(item => item.Title.ToUpper() == "TEST").ToString();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_a_where_clause_uses_the_StartsWith_string_function : QueryBuilderTests
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
    public class when_a_where_clause_uses_the_ever_function : QueryBuilderTests
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
    public class when_a_where_clause_uses_the_Contains_string_function : QueryBuilderTests
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
    public class when_a_where_clause_is_chained_to_a_select_clause : QueryBuilderTests
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
    public class when_a_query_has_a_where_clause_with_a_ToString_in_it : QueryBuilderTests
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
    public class when_a_query_has_a_where_clause_with_a_type_that_uses_an_indexer : QueryBuilderTests
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
}
