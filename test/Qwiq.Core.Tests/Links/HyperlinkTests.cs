using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.Links
{
    /// <summary>
    /// Tests for Hyperlink class creation.
    /// </summary>
    [TestClass]
    public class Given_Hyperlink_with_location : ContextSpecification
    {
        private Hyperlink _hyperlink = null!;
        private const string TestLocation = "https://example.com/page";

        public override void When()
        {
            _hyperlink = new Hyperlink(TestLocation);
        }

        [TestMethod]
        public void Then_location_is_set()
        {
            _hyperlink.Location.ShouldBe(TestLocation);
        }

        [TestMethod]
        public void Then_base_type_is_hyperlink()
        {
            _hyperlink.BaseType.ShouldBe(BaseLinkType.Hyperlink);
        }

        [TestMethod]
        public void Then_comment_is_null()
        {
            _hyperlink.Comment.ShouldBeNull();
        }
    }

    [TestClass]
    public class Given_Hyperlink_with_location_and_comment : ContextSpecification
    {
        private Hyperlink _hyperlink = null!;
        private const string TestLocation = "https://example.com/page";
        private const string TestComment = "Documentation link";

        public override void When()
        {
            _hyperlink = new Hyperlink(TestLocation, TestComment);
        }

        [TestMethod]
        public void Then_location_is_set()
        {
            _hyperlink.Location.ShouldBe(TestLocation);
        }

        [TestMethod]
        public void Then_comment_is_set()
        {
            _hyperlink.Comment.ShouldBe(TestComment);
        }
    }

    [TestClass]
    public class Given_Hyperlink_with_null_location : ContextSpecification
    {
        private Exception? _exception;

        public override void When()
        {
            try
            {
                _ = new Hyperlink(null!);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void Then_throws_ArgumentException()
        {
            _exception.ShouldBeOfType<ArgumentException>();
        }
    }

    [TestClass]
    public class Given_Hyperlink_with_empty_location : ContextSpecification
    {
        private Exception? _exception;

        public override void When()
        {
            try
            {
                _ = new Hyperlink(string.Empty);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [TestMethod]
        public void Then_throws_ArgumentException()
        {
            _exception.ShouldBeOfType<ArgumentException>();
        }
    }

    /// <summary>
    /// Tests for Hyperlink equality.
    /// </summary>
    [TestClass]
    public class Given_two_Hyperlinks_with_same_location : ContextSpecification
    {
        private Hyperlink _hyperlink1 = null!;
        private Hyperlink _hyperlink2 = null!;
        private const string TestLocation = "https://example.com/page";

        public override void When()
        {
            _hyperlink1 = new Hyperlink(TestLocation);
            _hyperlink2 = new Hyperlink(TestLocation);
        }

        [TestMethod]
        public void Then_they_are_equal()
        {
            _hyperlink1.Equals(_hyperlink2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_Equals_object_returns_true()
        {
            _hyperlink1.Equals((object)_hyperlink2).ShouldBeTrue();
        }

        [TestMethod]
        public void Then_hash_codes_are_equal()
        {
            _hyperlink1.GetHashCode().ShouldBe(_hyperlink2.GetHashCode());
        }
    }

    [TestClass]
    public class Given_two_Hyperlinks_with_different_locations : ContextSpecification
    {
        private Hyperlink _hyperlink1 = null!;
        private Hyperlink _hyperlink2 = null!;

        public override void When()
        {
            _hyperlink1 = new Hyperlink("https://example.com/page1");
            _hyperlink2 = new Hyperlink("https://example.com/page2");
        }

        [TestMethod]
        public void Then_they_are_not_equal()
        {
            _hyperlink1.Equals(_hyperlink2).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_returns_false()
        {
            _hyperlink1.Equals((object)_hyperlink2).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_Hyperlink_compared_to_null : ContextSpecification
    {
        private Hyperlink _hyperlink = null!;

        public override void When()
        {
            _hyperlink = new Hyperlink("https://example.com/page");
        }

        [TestMethod]
        public void Then_Equals_IHyperlink_null_returns_false()
        {
            _hyperlink.Equals((IHyperlink?)null).ShouldBeFalse();
        }

        [TestMethod]
        public void Then_Equals_object_null_returns_false()
        {
            _hyperlink.Equals((object?)null).ShouldBeFalse();
        }
    }

    [TestClass]
    public class Given_Hyperlink_compared_to_itself : ContextSpecification
    {
        private Hyperlink _hyperlink = null!;

        public override void When()
        {
            _hyperlink = new Hyperlink("https://example.com/page");
        }

        [TestMethod]
        public void Then_Equals_returns_true()
        {
            _hyperlink.Equals(_hyperlink).ShouldBeTrue();
        }
    }

    [TestClass]
    public class Given_two_Hyperlinks_with_same_location_different_case : ContextSpecification
    {
        private Hyperlink _hyperlink1 = null!;
        private Hyperlink _hyperlink2 = null!;

        public override void When()
        {
            _hyperlink1 = new Hyperlink("https://example.com/Page");
            _hyperlink2 = new Hyperlink("https://EXAMPLE.COM/PAGE");
        }

        [TestMethod]
        public void Then_they_are_equal_case_insensitive()
        {
            _hyperlink1.Equals(_hyperlink2).ShouldBeTrue();
        }
    }
}
