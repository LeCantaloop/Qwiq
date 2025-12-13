using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Links
{
    /// <summary>
    /// Tests for ExternalLink construction and behavior.
    /// </summary>
    [TestClass]
    public class Given_valid_ExternalLink_parameters : ContextSpecification
    {
        private ExternalLink _link = null!;
        private string _uri = null!;
        private string _name = null!;
        private string _comment = null!;

        public override void Given()
        {
            _uri = "https://example.com/artifact/123";
            _name = "Build";
            _comment = "Test comment";
        }

        public override void When()
        {
            _link = new ExternalLink(_uri, _name, _comment);
        }

        [TestMethod]
        public void Then_LinkedArtifactUri_is_set()
        {
            _link.LinkedArtifactUri.ShouldBe(_uri);
        }

        [TestMethod]
        public void Then_ArtifactLinkTypeName_is_set()
        {
            _link.ArtifactLinkTypeName.ShouldBe(_name);
        }

        [TestMethod]
        public void Then_Comment_is_set()
        {
            _link.Comment.ShouldBe(_comment);
        }

        [TestMethod]
        public void Then_BaseType_is_ExternalLink()
        {
            _link.BaseType.ShouldBe(BaseLinkType.ExternalLink);
        }
    }

    [TestClass]
    public class ExternalLinkValidationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExternalLink_with_null_uri_throws_ArgumentNullException()
        {
            _ = new ExternalLink(null!, "Build");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExternalLink_with_empty_uri_throws_ArgumentNullException()
        {
            _ = new ExternalLink("   ", "Build");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExternalLink_with_null_name_throws_ArgumentNullException()
        {
            _ = new ExternalLink("https://example.com", null!);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExternalLink_with_empty_name_throws_ArgumentNullException()
        {
            _ = new ExternalLink("https://example.com", "   ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExternalLink_with_uri_too_long_throws_ArgumentException()
        {
            var longUri = "https://example.com/" + new string('a', 2100);
            _ = new ExternalLink(longUri, "Build");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExternalLink_with_Related_Workitem_name_throws_ArgumentException()
        {
            _ = new ExternalLink("https://example.com", "Related Workitem");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExternalLink_with_Workitem_Hyperlink_name_throws_ArgumentException()
        {
            _ = new ExternalLink("https://example.com", "Workitem Hyperlink");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExternalLink_with_Fixed_in_Changeset_name_throws_ArgumentException()
        {
            _ = new ExternalLink("https://example.com", "Fixed in Changeset");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExternalLink_with_Source_Code_File_name_throws_ArgumentException()
        {
            _ = new ExternalLink("https://example.com", "Source Code File");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExternalLink_with_Test_Result_name_throws_ArgumentException()
        {
            _ = new ExternalLink("https://example.com", "Test Result");
        }
    }

    [TestClass]
    public class Given_two_ExternalLinks_with_same_values : ContextSpecification
    {
        private ExternalLink _link1 = null!;
        private ExternalLink _link2 = null!;

        public override void Given()
        {
            _link1 = new ExternalLink("https://example.com", "Build");
            _link2 = new ExternalLink("https://example.com", "Build");
        }

        [TestMethod]
        public void Then_Equals_returns_true()
        {
            _link1.Equals(_link2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_is_same()
        {
            _link1.GetHashCode().ShouldBe(_link2.GetHashCode());
        }
    }

    [TestClass]
    public class Given_two_ExternalLinks_with_different_uri : ContextSpecification
    {
        private ExternalLink _link1 = null!;
        private ExternalLink _link2 = null!;

        public override void Given()
        {
            _link1 = new ExternalLink("https://example1.com", "Build");
            _link2 = new ExternalLink("https://example2.com", "Build");
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _link1.Equals(_link2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_two_ExternalLinks_with_different_name : ContextSpecification
    {
        private ExternalLink _link1 = null!;
        private ExternalLink _link2 = null!;

        public override void Given()
        {
            _link1 = new ExternalLink("https://example.com", "Build");
            _link2 = new ExternalLink("https://example.com", "Commit");
        }

        [TestMethod]
        public void Then_Equals_returns_false()
        {
            _link1.Equals(_link2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_ExternalLink_Equals_with_null : ContextSpecification
    {
        private ExternalLink _link = null!;

        public override void Given()
        {
            _link = new ExternalLink("https://example.com", "Build");
        }

        [TestMethod]
        public void Then_Equals_IExternalLink_null_returns_false()
        {
            _link.Equals((IExternalLink?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_null_returns_false()
        {
            _link.Equals((object?)null).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_ExternalLink_Equals_with_same_reference : ContextSpecification
    {
        private ExternalLink _link = null!;
        private bool _result;

        public override void Given()
        {
            _link = new ExternalLink("https://example.com", "Build");
        }

        public override void When()
        {
            _result = _link.Equals(_link);
        }

        [TestMethod]
        public void Then_returns_true()
        {
            _result.ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_ExternalLink_with_case_insensitive_comparison : ContextSpecification
    {
        private ExternalLink _link1 = null!;
        private ExternalLink _link2 = null!;

        public override void Given()
        {
            _link1 = new ExternalLink("https://EXAMPLE.com", "BUILD");
            _link2 = new ExternalLink("https://example.com", "build");
        }

        [TestMethod]
        public void Then_Equals_returns_true()
        {
            _link1.Equals(_link2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_GetHashCode_is_same()
        {
            _link1.GetHashCode().ShouldBe(_link2.GetHashCode());
        }
    }
}
