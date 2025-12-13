using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Query
{
    /// <summary>
    /// Tests for QueryDefinition construction and behavior.
    /// </summary>
    [TestClass]
    public class Given_valid_QueryDefinition_parameters : ContextSpecification
    {
        private MockQueryDefinition _queryDefinition = null!;
        private Guid _id;
        private string _name = null!;
        private string _wiql = null!;
        private string _path = null!;

        public override void Given()
        {
            _id = Guid.NewGuid();
            _name = "My Query";
            _wiql = "SELECT [System.Id] FROM WorkItems";
            _path = "Shared Queries/My Query";
        }

        public override void When()
        {
            _queryDefinition = new MockQueryDefinition(_id, _name, _wiql, _path);
        }

        [TestMethod]
        public void Then_Id_is_set()
        {
            _queryDefinition.Id.ShouldBe(_id);
        }

        [TestMethod]
        public void Then_Name_is_set()
        {
            _queryDefinition.Name.ShouldBe(_name);
        }

        [TestMethod]
        public void Then_Wiql_is_set()
        {
            _queryDefinition.Wiql.ShouldBe(_wiql);
        }

        [TestMethod]
        public void Then_Path_is_set()
        {
            _queryDefinition.Path.ShouldBe(_path);
        }
    }

    [TestClass]
    public class QueryDefinitionValidationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QueryDefinition_with_empty_guid_throws_ArgumentOutOfRangeException()
        {
            _ = new MockQueryDefinition(Guid.Empty, "Query", "SELECT * FROM WorkItems", "/path");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryDefinition_with_null_name_throws_ArgumentException()
        {
            _ = new MockQueryDefinition(Guid.NewGuid(), null!, "SELECT * FROM WorkItems", "/path");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryDefinition_with_empty_name_throws_ArgumentException()
        {
            _ = new MockQueryDefinition(Guid.NewGuid(), string.Empty, "SELECT * FROM WorkItems", "/path");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryDefinition_with_null_wiql_throws_ArgumentException()
        {
            _ = new MockQueryDefinition(Guid.NewGuid(), "Query", null!, "/path");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryDefinition_with_empty_wiql_throws_ArgumentException()
        {
            _ = new MockQueryDefinition(Guid.NewGuid(), "Query", string.Empty, "/path");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryDefinition_with_null_path_throws_ArgumentException()
        {
            _ = new MockQueryDefinition(Guid.NewGuid(), "Query", "SELECT * FROM WorkItems", null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryDefinition_with_empty_path_throws_ArgumentException()
        {
            _ = new MockQueryDefinition(Guid.NewGuid(), "Query", "SELECT * FROM WorkItems", string.Empty);
        }
    }

    [TestClass]
    public class Given_QueryDefinition_ToString : ContextSpecification
    {
        private MockQueryDefinition _queryDefinition = null!;
        private Guid _id;
        private string _result = null!;

        public override void Given()
        {
            _id = Guid.NewGuid();
            _queryDefinition = new MockQueryDefinition(_id, "My Query", "SELECT * FROM WorkItems", "/path");
        }

        public override void When()
        {
            _result = _queryDefinition.ToString();
        }

        [TestMethod]
        public void Then_returns_formatted_string_with_id_and_name()
        {
            _result.ShouldBe($"{_id} (My Query)");
        }
    }

    [TestClass]
    public class Given_two_QueryDefinitions_with_same_id_and_name : ContextSpecification
    {
        private MockQueryDefinition _query1 = null!;
        private MockQueryDefinition _query2 = null!;
        private Guid _id;

        public override void Given()
        {
            _id = Guid.NewGuid();
            _query1 = new MockQueryDefinition(_id, "Query", "SELECT * FROM WorkItems", "/path1");
            _query2 = new MockQueryDefinition(_id, "Query", "SELECT * FROM WorkItems WHERE [Id] = 1", "/path2");
        }

        [TestMethod]
        public void Then_Equals_returns_true()
        {
            _query1.Equals(_query2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_is_same()
        {
            _query1.GetHashCode().ShouldBe(_query2.GetHashCode());
        }
    }

    [TestClass]
    public class Given_two_QueryDefinitions_with_different_id : ContextSpecification
    {
        private MockQueryDefinition _query1 = null!;
        private MockQueryDefinition _query2 = null!;

        public override void Given()
        {
            _query1 = new MockQueryDefinition(Guid.NewGuid(), "Query", "SELECT * FROM WorkItems", "/path");
            _query2 = new MockQueryDefinition(Guid.NewGuid(), "Query", "SELECT * FROM WorkItems", "/path");
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _query1.Equals(_query2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_two_QueryDefinitions_with_different_name : ContextSpecification
    {
        private MockQueryDefinition _query1 = null!;
        private MockQueryDefinition _query2 = null!;
        private Guid _id;

        public override void Given()
        {
            _id = Guid.NewGuid();
            _query1 = new MockQueryDefinition(_id, "Query1", "SELECT * FROM WorkItems", "/path");
            _query2 = new MockQueryDefinition(_id, "Query2", "SELECT * FROM WorkItems", "/path");
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _query1.Equals(_query2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_QueryDefinition_Equals_with_null : ContextSpecification
    {
        private MockQueryDefinition _query = null!;

        public override void Given()
        {
            _query = new MockQueryDefinition(Guid.NewGuid(), "Query", "SELECT * FROM WorkItems", "/path");
        }

        [TestMethod]
        public void Then_Equals_IQueryDefinition_null_returns_false()
        {
            _query.Equals((IQueryDefinition?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_null_returns_false()
        {
            _query.Equals((object?)null).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_two_QueryDefinitions_with_same_name_different_case : ContextSpecification
    {
        private MockQueryDefinition _query1 = null!;
        private MockQueryDefinition _query2 = null!;
        private Guid _id;

        public override void Given()
        {
            _id = Guid.NewGuid();
            _query1 = new MockQueryDefinition(_id, "Query", "SELECT * FROM WorkItems", "/path");
            _query2 = new MockQueryDefinition(_id, "QUERY", "SELECT * FROM WorkItems", "/path");
        }

        [TestMethod]
        public void Then_Equals_returns_true_case_insensitive()
        {
            _query1.Equals(_query2).ShouldBeTrue();
        }
    }
}
