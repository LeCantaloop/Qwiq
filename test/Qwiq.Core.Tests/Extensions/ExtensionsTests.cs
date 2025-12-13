using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qwiq.Tests.Common;
using Shouldly;

namespace Qwiq.ExtensionsTests
{
    /// <summary>
    /// Tests for Extensions.ToUsefulString method.
    /// </summary>
    [TestClass]
    public class Given_null_object_calling_ToUsefulString : ContextSpecification
    {
        private string _result = null!;

        public override void When()
        {
            _result = ((object?)null).ToUsefulString();
        }

        [TestMethod]
        public void Then_returns_null_representation()
        {
            _result.ShouldBe("[null]");
        }
    }

    [TestClass]
    public class Given_string_calling_ToUsefulString : ContextSpecification
    {
        private string _result = null!;
        private const string TestString = "Hello World";

        public override void When()
        {
            _result = ((object)TestString).ToUsefulString();
        }

        [TestMethod]
        public void Then_returns_quoted_string()
        {
            _result.ShouldBe("\"Hello World\"");
        }
    }

    [TestClass]
    public class Given_string_with_newlines_calling_ToUsefulString : ContextSpecification
    {
        private string _result = null!;

        public override void When()
        {
            _result = ((object)"Hello\nWorld\r").ToUsefulString();
        }

        [TestMethod]
        public void Then_returns_escaped_newlines()
        {
            _result.ShouldContain("\\n");
            _result.ShouldContain("\\r");
        }
    }

    [TestClass]
    public class Given_int_calling_ToUsefulString : ContextSpecification
    {
        private string _result = null!;

        public override void When()
        {
            _result = ((object)42).ToUsefulString();
        }

        [TestMethod]
        public void Then_returns_bracketed_value()
        {
            _result.ShouldBe("[42]");
        }
    }

    [TestClass]
    public class Given_bool_calling_ToUsefulString : ContextSpecification
    {
        private string _result = null!;

        public override void When()
        {
            _result = ((object)true).ToUsefulString();
        }

        [TestMethod]
        public void Then_returns_bracketed_value()
        {
            _result.ShouldBe("[True]");
        }
    }

    [TestClass]
    public class Given_DateTime_calling_ToUsefulString : ContextSpecification
    {
        private string _result = null!;
        private DateTime _date;

        public override void Given()
        {
            _date = new DateTime(2025, 12, 13, 10, 30, 0, DateTimeKind.Utc);
        }

        public override void When()
        {
            _result = ((object)_date).ToUsefulString();
        }

        [TestMethod]
        public void Then_returns_bracketed_date()
        {
            _result.ShouldStartWith("[");
            _result.ShouldEndWith("]");
        }
    }

    [TestClass]
    public class Given_List_calling_ToUsefulString : ContextSpecification
    {
        private string _result = null!;

        public override void When()
        {
            var list = new List<int> { 1, 2, 3 };
            _result = ((object)list).ToUsefulString();
        }

        [TestMethod]
        public void Then_returns_list_representation()
        {
            _result.ShouldContain("List");
        }

        [TestMethod]
        public void Then_contains_elements()
        {
            _result.ShouldContain("[1]");
            _result.ShouldContain("[2]");
            _result.ShouldContain("[3]");
        }
    }

    [TestClass]
    public class Given_empty_string_object_calling_ToUsefulString : ContextSpecification
    {
        private string _result = null!;
        private TestClassWithEmptyToString _testObject = null!;

        public override void Given()
        {
            _testObject = new TestClassWithEmptyToString();
        }

        public override void When()
        {
            _result = ((object)_testObject).ToUsefulString();
        }

        [TestMethod]
        public void Then_returns_type_with_empty_brackets()
        {
            _result.ShouldContain("TestClassWithEmptyToString");
            _result.ShouldContain("[]");
        }

        private class TestClassWithEmptyToString
        {
            public override string ToString() => string.Empty;
        }
    }

    [TestClass]
    public class Given_object_with_multiline_ToString : ContextSpecification
    {
        private string _result = null!;
        private TestClassWithMultilineToString _testObject = null!;

        public override void Given()
        {
            _testObject = new TestClassWithMultilineToString();
        }

        public override void When()
        {
            _result = ((object)_testObject).ToUsefulString();
        }

        [TestMethod]
        public void Then_returns_formatted_output()
        {
            _result.ShouldContain("Line1");
            _result.ShouldContain("Line2");
        }

        private class TestClassWithMultilineToString
        {
            public override string ToString() => "Line1\nLine2";
        }
    }

    /// <summary>
    /// Tests for Extensions.EachToUsefulString method.
    /// </summary>
    [TestClass]
    public class Given_small_list_calling_EachToUsefulString : ContextSpecification
    {
        private string _result = null!;

        public override void When()
        {
            var list = new List<int> { 1, 2, 3 };
            _result = list.EachToUsefulString();
        }

        [TestMethod]
        public void Then_contains_opening_brace()
        {
            _result.ShouldContain("{");
        }

        [TestMethod]
        public void Then_contains_closing_brace()
        {
            _result.ShouldContain("}");
        }

        [TestMethod]
        public void Then_contains_all_elements()
        {
            _result.ShouldContain("[1]");
            _result.ShouldContain("[2]");
            _result.ShouldContain("[3]");
        }
    }

    [TestClass]
    public class Given_large_list_calling_EachToUsefulString_with_limit : ContextSpecification
    {
        private string _result = null!;

        public override void When()
        {
            var list = new List<int>();
            for (int i = 1; i <= 15; i++)
            {
                list.Add(i);
            }
            _result = list.EachToUsefulString(5);
        }

        [TestMethod]
        public void Then_contains_more_elements_message()
        {
            _result.ShouldContain("more elements");
        }
    }

    [TestClass]
    public class Given_list_with_exactly_one_more_than_limit : ContextSpecification
    {
        private string _result = null!;

        public override void When()
        {
            var list = new List<int> { 1, 2, 3, 4, 5, 6 };
            _result = list.EachToUsefulString(5);
        }

        [TestMethod]
        public void Then_shows_last_element()
        {
            // When there's exactly one more element, it shows that element instead of "...more"
            _result.ShouldContain("[6]");
        }
    }
}
