using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Shared.Utils
{
    public static class NumericExtensions
    {
        public static string ToLocalizePriceString(this decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static decimal ToLocalizePriceDecimal(this string str, IFormatProvider formatProvider = null)
        {
            formatProvider ??= CultureInfo.InvariantCulture;
            if (decimal.TryParse(str, NumberStyles.Any, provider: formatProvider, result: out var result))
            {
                return result;
            }
            else
            {
                foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                {
                    if (decimal.TryParse(str, NumberStyles.Any, provider: culture, result: out result))
                    {
                        return result;
                    }
                }
            }
            
            // If no valid culture was found, throw an exception
            throw new FormatException($"Unable to parse the string '{str}' to a decimal with any culture.");
        }
    }
}