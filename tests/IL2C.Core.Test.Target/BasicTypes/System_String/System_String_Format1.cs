using System;
using System.Runtime.CompilerServices;

namespace IL2C.BasicTypes
{
    public sealed class Format1_CustomProducer
    {
        public override string ToString() => "XYZ";
    }

    [TestId("System_String")]
    [TestCase("ABC123DEF", "Format1", "ABC{0}DEF", (byte)123)]
    [TestCase("ABC-123DEF", "Format1", "ABC{0}DEF", (sbyte)-123)]
    [TestCase("ABC-12345DEF", "Format1", "ABC{0}DEF", (short)-12345)]
    [TestCase("ABC12345DEF", "Format1", "ABC{0}DEF", (ushort)12345)]
    [TestCase("ABC1234567DEF", "Format1", "ABC{0}DEF", 1234567)]
    [TestCase("ABC-1234567DEF", "Format1", "ABC{0}DEF", -1234567)]
    [TestCase("ABC1234567890123DEF", "Format1", "ABC{0}DEF", 1234567890123)]
    [TestCase("ABC-1234567890123DEF", "Format1", "ABC{0}DEF", -1234567890123)]
    [TestCase("ABC123.456DEF", "Format1", "ABC{0}DEF", 123.456f)]
    [TestCase("ABC123.456DEF", "Format1", "ABC{0}DEF", 123.456)]
    [TestCase("ABCTrueDEF", "Format1", "ABC{0}DEF", true)]
    [TestCase("ABCFalseDEF", "Format1", "ABC{0}DEF", false)]
    [TestCase("ABCXDEF", "Format1", "ABC{0}DEF", 'X')]
    [TestCase("ABCXYZDEF", "Format1", "ABC{0}DEF", "XYZ")]
    [TestCase("ABCXYZDEF", "Format1_Custom", "ABC{0}DEF", IncludeTypes = new[] { typeof(Format1_CustomProducer) })]
    [TestCase(true, "Format1_Exception", "ABC{}DEF", 123)]
    [TestCase(true, "Format1_Exception", "ABC{12345678901234}DEF", 123)]
    [TestCase(true, "Format1_Exception", "ABC{1}DEF", 123)]
    public sealed class System_String_Format1
    {
        public static string Format1(string format, object value0)
        {
            return string.Format(format, value0);
        }

        public static string Format1_Custom(string format)
        {
            var cp = new Format1_CustomProducer();
            return string.Format(format, cp);
        }

        public static bool Format1_Exception(string format, object value0)
        {
            try
            {
                var s = string.Format(format, value0);
            }
            catch (FormatException)
            {
                return true;
            }
            return false;
        }
    }
}
