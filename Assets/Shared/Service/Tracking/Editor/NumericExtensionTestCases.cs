using System;
using System.Collections;
using System.Globalization;
using NUnit.Framework;
using Shared.Utils;
using UnityEngine.TestTools;

namespace Shared.Tracking.Editor
{
    public class NumericExtensionTestCases
    {
        [UnityTest]
        public IEnumerator Test_DecimalToString()
        {
            decimal number = 12345.6789m;
            string result = number.ToLocalizePriceString();

            Assert.AreEqual("12345.6789", result,
                "InvariantCulture should format the decimal with '.' as the decimal separator.");
            yield return null;
        }

        // String to Number Tests
        [UnityTest]
        public IEnumerator Test_StringToDecimal_InvariantCulture()
        {
            string numberStr = "12345.6789";
            decimal result = numberStr.ToLocalizePriceDecimal();

            Assert.AreEqual(12345.6789m, result,
                "InvariantCulture should parse the string with '.' as the decimal separator.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_StringToDecimal_USCulture()
        {
            string numberStr = "12345.6789";
            decimal result = numberStr.ToLocalizePriceDecimal(new CultureInfo("en-US"));

            Assert.AreEqual(12345.6789m, result,
                "US culture should parse the string with '.' as the decimal separator.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_StringToDecimal_FrenchCulture()
        {
            string numberStr = "12345,6789";
            decimal result = numberStr.ToLocalizePriceDecimal(new CultureInfo("fr-FR"));

            Assert.AreEqual(12345.6789m, result,
                "French culture should parse the string with ',' as the decimal separator.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_StringToDecimal_GermanCulture()
        {
            string numberStr = "12345,6789";
            decimal result = numberStr.ToLocalizePriceDecimal(new CultureInfo("de-DE"));

            Assert.AreEqual(12345.6789m, result,
                "German culture should parse the string with ',' as the decimal separator.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_StringToDecimal_JapaneseCulture()
        {
            string numberStr = "12345.6789";
            decimal result = numberStr.ToLocalizePriceDecimal(new CultureInfo("ja-JP"));

            Assert.AreEqual(12345.6789m, result,
                "Japanese culture should parse the string with '.' as the decimal separator.");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_StringToDecimal_CustomCulture()
        {
            CultureInfo customCulture = new CultureInfo("en-US")
            {
                NumberFormat = { NumberDecimalSeparator = "|" }
            };
            string numberStr = "12345|6789";
            decimal result = numberStr.ToLocalizePriceDecimal(customCulture);

            Assert.AreEqual(12345.6789m, result,
                "Custom culture should parse the string with '|' as the decimal separator.");
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Test_StringToDecimal_AmbiguousDecimalSign_Case_1()
        {
            string ambiguousNumberStr = "12,345.6789"; // Ambiguous format
            decimal result = ambiguousNumberStr.ToLocalizePriceDecimal();
            Assert.AreEqual(12345.6789m, result,
                "It should parse the string with both ',' and '.' correctly");
            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_StringToDecimal_AmbiguousDecimalSign_Case_2()
        {
            string ambiguousNumberStr = "12.345,6789"; // Ambiguous format
            decimal result = ambiguousNumberStr.ToLocalizePriceDecimal();
            Assert.AreEqual(12345.6789m, result,
                "It should parse the string with both ',' and '.' correctly");
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Test_StringToDecimal_AmbiguousDecimalSign_Case_3()
        {
            string ambiguousNumberStr = "12.7777,999"; // Ambiguous format
            decimal result = ambiguousNumberStr.ToLocalizePriceDecimal();
            Assert.AreEqual(127777.999m, result,
                "It should parse the string with both ',' and '.' correctly");
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator Test_StringToDecimal_Exception_InvalidNumber()
        {
            string invalidNumberStr = "12.345|6789"; // Invalid string
            Assert.Throws<FormatException>(() => { invalidNumberStr.ToLocalizePriceDecimal(); },
                "Parsing an invalid format should throw a FormatException.");
            yield return null;
        }
    }
}