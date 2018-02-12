using System;
using System.Xml;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

namespace Qwiq
{
    [TestClass]
    public class when_parsing_a_value_type_to_nullable_value_type : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = (int?)1;
            Actual = Parser.Parse<int?>(1L);
        }

        [TestMethod]
        public void value_is_converted()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_enum_value_Generic : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = Formatting.Indented;
            Actual = Parser.Parse("Indented", Formatting.None);
        }

        [TestMethod]
        public void enum_value_is_returned()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_enum_value_null_with_defaultValue : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = (Formatting)0;
            Actual = Parser.Parse(typeof(Formatting), null, Formatting.None);
        }

        [TestMethod]
        public void enum_value_is_returned()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_enum_value_null_defaultValue_null : TypeParserTestsContext
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void enum_value_is_returned()
        {
            Parser.Parse(typeof(Formatting), null, null);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_enum_value_null : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = (Formatting)0;
            Actual = Parser.Parse(typeof(Formatting), null);
        }

        [TestMethod]
        public void enum_value_is_returned()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_to_nonnullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 11d;
            Actual = Parser.Parse<double>(null, 11);
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_invalid_string_to_nullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = null;
            Actual = Parser.Parse(typeof(double?), (object)"blarg");
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_invalid_string_to_nonnullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 0d;
            Actual = Parser.Parse(typeof(double), (object)"blarg");
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_int64_string_to_int32 : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 0;
            Actual = Parser.Parse(typeof(int), (object)"0x7FFFFFFFFFFFFFFF");
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_null_to_nonnullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 0d;
            Actual = Parser.Parse(typeof(double), null);
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_DateTime_to_nonnullable_DateTime : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = new DateTime(2014, 1, 1);
            Actual = Parser.Parse(typeof(DateTime), null, Expected);
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_bool : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = false;
            Actual = Parser.Parse<bool>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_SByte : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = (sbyte)0;
            Actual = Parser.Parse<sbyte>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_byte : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = (byte)0;
            Actual = Parser.Parse<byte>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_short : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = (short)0;
            Actual = Parser.Parse<short>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_ushort : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = (ushort)0;
            Actual = Parser.Parse<ushort>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_double : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = 0d;
            Actual = Parser.Parse<double>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_double_and_null_defaultvalue : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = 0d;
            Actual = Parser.Parse(typeof(double), "", null);
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_uint : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = (uint)0;
            Actual = Parser.Parse<uint>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_int : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = 0;
            Actual = Parser.Parse<int>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_string : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = string.Empty;
            Actual = Parser.Parse<string>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_long : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = 0L;
            Actual = Parser.Parse<long>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_ulong : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = (ulong)0;
            Actual = Parser.Parse<ulong>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_float : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = (float)0;
            Actual = Parser.Parse<float>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_decimal : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = (decimal)0;
            Actual = Parser.Parse<decimal>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_DateTime : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = DateTime.MinValue;
            Actual = (DateTime)Parser.Parse(typeof(DateTime), (object)"");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_DateTime_Generic : TypeParserFirstChanceExceptionContext
    {
        public override void When()
        {
            Expected = DateTime.MinValue;
            Actual = Parser.Parse<DateTime>("");
        }

        [TestMethod]
        public void default_value_is_returned_without_exception()
        {
            Actual.ShouldEqual(Expected);
            FirstChanceException.ShouldBeNull();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nonnullable_double_with_nuill_for_value_and_defaultvalue : TypeParserTestsContext
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void value_is_parsed_as_double()
        {
            Parser.Parse(typeof(double), null, null);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nonnullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 7d;
            Actual = Parser.Parse<double>("7");
        }

        [TestMethod]
        public void value_is_parsed_as_double()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_nullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = null;
            Actual = Parser.Parse<double?>(null);
        }

        [TestMethod]
        public void value_is_null()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_byte_to_decimal : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = (decimal)255;
            Actual = Parser.Parse(typeof(decimal), (object)((byte)255));
        }

        [TestMethod]
        public void decimal_value_is_expected()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_nullable_double_with_defaultValue_of_decimal : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 1.0M;
            Actual = Parser.Parse(typeof(double?), null, 1.0M);
        }

        [TestMethod]
        public void value_is_defaultValue()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 7d;
            Actual = Parser.Parse<double?>("7");
        }

        [TestMethod]
        public void value_is_parsed_as_double()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_generic_nullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 7d;
            Actual = Parser.Parse<double?>("7");
        }

        [TestMethod]
        public void value_is_parsed_as_double()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_nullable_int : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = null;
            Actual = Parser.Parse<int?>(null);
        }

        [TestMethod]
        public void value_is_null()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nullable_int : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 7;
            Actual = Parser.Parse<int?>("7");
        }

        [TestMethod]
        public void value_is_parsed_as_int()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_nullable_date : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = null;
            Actual = Parser.Parse<DateTime?>(null);
        }

        [TestMethod]
        public void value_is_null()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nullable_date : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = DateTime.Today;
            Actual = Parser.Parse<DateTime?>(Expected.ToString());
        }

        [TestMethod]
        public void value_is_parsed_as_date()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_nonnullable_string : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = "";
            Actual = Parser.Parse<string>(null, "");
        }

        [TestMethod]
        public void value_is_empty_string()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nonnullable_string : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = "test string";
            Actual = Parser.Parse<string>("test string");
        }

        [TestMethod]
        public void value_is_parsed_as_string()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_nonnullable_int : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 14;
            Actual = Parser.Parse<int>(null, 14);
        }

        [TestMethod]
        public void value_is_default()
        {
            Actual.ShouldEqual(Expected);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nonnullable_int : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 7;
            Actual = Parser.Parse<int>(7);
        }

        [TestMethod]
        public void value_is_parsed_as_int()
        {
            Actual.ShouldEqual(Expected);
        }
    }
}

