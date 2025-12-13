using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qwiq.Mocks;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Query
{
    /// <summary>
    /// Tests for QueryFolder construction and behavior.
    /// </summary>
    [TestClass]
    public class Given_valid_QueryFolder_parameters : ContextSpecification
    {
        private MockQueryFolder _queryFolder = null!;
        private Guid _id;
        private string _name = null!;
        private string _path = null!;
        private IQueryFolderCollection _subFolders = null!;
        private IQueryDefinitionCollection _queries = null!;

        public override void Given()
        {
            _id = Guid.NewGuid();
            _name = "My Folder";
            _path = "Shared Queries/My Folder";
            _subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            _queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
        }

        public override void When()
        {
            _queryFolder = new MockQueryFolder(_id, _name, _path, _subFolders, _queries);
        }

        [TestMethod]
        public void Then_Id_is_set()
        {
            _queryFolder.Id.ShouldBe(_id);
        }

        [TestMethod]
        public void Then_Name_is_set()
        {
            _queryFolder.Name.ShouldBe(_name);
        }

        [TestMethod]
        public void Then_Path_is_set()
        {
            _queryFolder.Path.ShouldBe(_path);
        }

        [TestMethod]
        public void Then_SubFolders_is_set()
        {
            _queryFolder.SubFolders.ShouldBe(_subFolders);
        }

        [TestMethod]
        public void Then_SavedQueries_is_set()
        {
            _queryFolder.SavedQueries.ShouldBe(_queries);
        }
    }

    [TestClass]
    public class QueryFolderValidationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void QueryFolder_with_empty_guid_throws_ArgumentOutOfRangeException()
        {
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _ = new MockQueryFolder(Guid.Empty, "Folder", "/path", subFolders, queries);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryFolder_with_null_name_throws_ArgumentException()
        {
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _ = new MockQueryFolder(Guid.NewGuid(), null!, "/path", subFolders, queries);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryFolder_with_empty_name_throws_ArgumentException()
        {
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _ = new MockQueryFolder(Guid.NewGuid(), string.Empty, "/path", subFolders, queries);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryFolder_with_null_path_throws_ArgumentException()
        {
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _ = new MockQueryFolder(Guid.NewGuid(), "Folder", null!, subFolders, queries);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryFolder_with_empty_path_throws_ArgumentException()
        {
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _ = new MockQueryFolder(Guid.NewGuid(), "Folder", string.Empty, subFolders, queries);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueryFolder_with_null_subFolders_throws_ArgumentNullException()
        {
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _ = new MockQueryFolder(Guid.NewGuid(), "Folder", "/path", null!, queries);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueryFolder_with_null_queries_throws_ArgumentNullException()
        {
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            _ = new MockQueryFolder(Guid.NewGuid(), "Folder", "/path", subFolders, null!);
        }
    }

    [TestClass]
    public class Given_QueryFolder_ToString : ContextSpecification
    {
        private MockQueryFolder _queryFolder = null!;
        private Guid _id;
        private string _result = null!;

        public override void Given()
        {
            _id = Guid.NewGuid();
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _queryFolder = new MockQueryFolder(_id, "My Folder", "/path", subFolders, queries);
        }

        public override void When()
        {
            _result = _queryFolder.ToString();
        }

        [TestMethod]
        public void Then_returns_formatted_string_with_id_and_name()
        {
            _result.ShouldBe($"{_id} (My Folder)");
        }
    }

    [TestClass]
    public class Given_QueryFolder_Equals_with_null : ContextSpecification
    {
        private MockQueryFolder _folder = null!;

        public override void Given()
        {
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _folder = new MockQueryFolder(Guid.NewGuid(), "Folder", "/path", subFolders, queries);
        }

        [TestMethod]
        public void Then_Equals_IQueryFolder_null_returns_false()
        {
            _folder.Equals((IQueryFolder?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_null_returns_false()
        {
            _folder.Equals((object?)null).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_QueryFolder_Path_can_be_modified : ContextSpecification
    {
        private MockQueryFolder _folder = null!;
        private string _newPath = null!;

        public override void Given()
        {
            var subFolders = new Mock<IQueryFolderCollection>(MockBehavior.Loose).Object;
            var queries = new Mock<IQueryDefinitionCollection>(MockBehavior.Loose).Object;
            _folder = new MockQueryFolder(Guid.NewGuid(), "Folder", "/original", subFolders, queries);
            _newPath = "/modified";
        }

        public override void When()
        {
            _folder.Path = _newPath;
        }

        [TestMethod]
        public void Then_Path_is_updated()
        {
            _folder.Path.ShouldBe(_newPath);
        }
    }
}
