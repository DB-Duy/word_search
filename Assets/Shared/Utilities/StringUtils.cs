using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.Utils;
using UnityEngine;

public static class StringUtils
{
    public static string ToString(string[] values)
    {
        string str = "";
        for (int i = 0; i < values.Length; i++)
        {
            str += values[i];
        }
        return str;
    }

    public static string ToString(int[] values)
    {
        string str = "";
        for (int i = 0; i < values.Length; i++)
        {
            str += string.Format("{0}, ", values[i]);
        }
        return str;
    }
    
    /// <summary>
    /// string input = "DictionaryResourceRepository";
    /// string output = ToSnakeCase(input);
    /// Console.WriteLine(output); // Output: "resource_json_repository"
    /// </summary>
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // Insert underscores before each uppercase letter (except the first one)
        var result = Regex.Replace(input, "([a-z0-9])([A-Z])", "$1_$2");

        // Convert the result to lowercase
        return result.ToLower();
    }
    
    public static bool IsValidJson(this string strInput)
    {
        if (string.IsNullOrEmpty(strInput)) { return false;}
        strInput = strInput.Trim();
	    
        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
            (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException jex)
            {
                //Exception in parsing json
                Console.WriteLine(jex.Message);
                return false;
            }
            catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    
    public static string ToJsonString(params object[] jsonParams)
    {
        Dictionary<string, object> p = new();
        var maxLength = jsonParams.Length % 2 == 0 ? jsonParams.Length : jsonParams.Length - 1;
        for (var i = 0; i < maxLength; i += 2)
        {
            if (jsonParams[i] == null) continue;
            var v = jsonParams[i + 1];
            if (v is MonoBehaviour b) v = $"Behaviour {b.name}";
            p.Upsert(jsonParams[i].ToString(), v);
        }

        return JsonConvert.SerializeObject(p);
    }
    
    public static string ConvertToJsonString(this List<string> l) => JsonConvert.SerializeObject(l);

    public static string ToJsonString(this List<string> strings) => JsonConvert.SerializeObject(strings);
}
