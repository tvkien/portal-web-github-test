using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class StringExtensions
    {
        public static string RationalizeFileName(this string rawFileName, char replaceBy)
        {
            if (string.IsNullOrEmpty(rawFileName))
                throw new Exception("Input fileName cannot be null");
            var charsCheckValid = rawFileName
                .ToArray()
                .AsParallel()
                .Select((val, index) => new
                {
                    val,
                    index,
                    isValid = Regex.IsMatch(new string(val, 1), "([a-z]|[A-Z]|[0-9])")
                }).ToArray();

            var replacedChars = charsCheckValid
            .Where(c => c.isValid || c.index == 0 || charsCheckValid[c.index - 1].isValid)
            .Select(c => c.isValid ? c.val : replaceBy)
            .ToArray();
            var result = new string(replacedChars);
            return result;
        }

        public static int[] ToIntArray(this string source, string separator = ",")
        {
            return source?.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c =>
                {
                    if (int.TryParse(c, out int _value))
                        return _value;
                    return default(int?);
                }).Where(c => c.HasValue)
                .Select(c => c.Value)
                .ToArray() ?? new int[0];
        }

        public static List<int> ToIntList(this string source, string separator = ",")
        {
            return source?.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c =>
                {
                    if (int.TryParse(c, out int _value))
                        return _value;
                    return default(int?);
                }).Where(c => c.HasValue)
                .Select(c => c.Value)
                .ToList() ?? new List<int>();
        }

        public static string ConvertToString(this IEnumerable<int> source, string separator = ",")
        {
            return source == null || source.Count() == 0
                ? string.Empty
                : string.Join(separator, source);
        }

        public static int?[] ToNullableIntArray(this string source, string separator = ",")
        {
            return source?.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c =>
                {
                    if (int.TryParse(c, out int _value))
                        return _value;
                    return default(int?);
                }).Where(c => c.HasValue)
                .Select(c => (int?)c.Value)
                .ToArray() ?? new int?[0];
        }

        public static string GetExtension(this string filePath)
        {
            return $".{filePath.Split('.')[1]}";
        }

        public static string RemoveLineBreak(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = input.Replace("\n ", "\n").Replace("\n", " ").Trim();
            }

            return input;
        }

        public static string ReplaceMultiple(this string content, Dictionary<string, string> replacers)
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }
            return replacers?.Aggregate(content, (theContent, replacer) =>
            {
                return theContent.Replace(replacer.Key, replacer.Value);
            });
        }

        public static string DescriptionAttr<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }

        private static Object GetPropValue(this Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static T GetPropValue<T>(this Object obj, String name)
        {
            Object retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            return (T)retval;
        }

        public static string ToIntCommaSeparatedString(this string source, string separator = ",")
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            var validNumbers = source
                .Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .Where(x => x > 0)
                .Distinct()
                .OrderBy(x => x);

            return validNumbers.Any() ? string.Join(separator, validNumbers) : string.Empty;
        }

        public static string ToIntCommaSeparatedString(this IEnumerable<int> source, string separator = ",")
        {
            if (source == null || source.Count() == 0)
            {
                return string.Empty;
            }

            var validNumbers = source
                .Where(x => x > 0)
                .Distinct()
                .OrderBy(x => x);

            return validNumbers.Any() ? string.Join(separator, validNumbers) : string.Empty;
        }

        public static IEnumerable<int> ToEnumerable(this string source, string separator = ",")
        {
            if (string.IsNullOrEmpty(source))
            {
                return Enumerable.Empty<int>();
            }

            return source
                .Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .Where(x => x > 0)
                .Distinct()
                .OrderBy(x => x);

        }

        public static bool EqualSupportNull(this string source, string destination, bool isIgnoreCase = true)
        {
            if (isIgnoreCase)
            {
                return string.Compare(source, destination, StringComparison.OrdinalIgnoreCase) == 0;
            }
            return string.Compare(source, destination) == 0;
        }
        public static string UrlDecode(this string input)
        {
            return HttpUtility.UrlDecode(input);
        }
        public static string UrlEncode(this string input)
        {
            return Uri.EscapeDataString(input);
        }

        public static string HtmlDecode(this string input)
        {
            return HttpUtility.HtmlDecode(input);
        }

        public static string HtmlEncode(this string input)
        {
            return HttpUtility.HtmlEncode(input);
        }
        public static T DeserializeObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        private static readonly List<string> WordsHaveAnArticle = new List<string> { "a", "e", "i", "o", "u" };


        public static bool IsRelativeUrl(this string linkTo)
        {
            return !Uri.TryCreate(linkTo, UriKind.Absolute, out Uri _);
        }

        public static string RemoveLineBreaks(this string lines)
        {
            return lines.Replace("\r", "").Replace("\n", "");
        }

        public static string ReplaceLineBreaks(this string lines, string replacement)
        {
            return lines.Replace("\r\n", replacement)
                        .Replace("\r", replacement)
                        .Replace("\n", replacement);
        }

        public static string ExtractAbbreviation(this string text)
        {
            var sb = new StringBuilder();
            foreach (char c in text.Where(char.IsUpper))
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static string ExtractFirstCharacterForEachWord(this string text)
        {
            var sb = new StringBuilder();
            var listWords = text.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in listWords)
            {
                var firstChar = word[0];
                if (char.IsUpper(firstChar))
                    sb.Append(firstChar.ToString().ToUpper());
            }
            return sb.ToString();
        }

        public static string ParseSgoComparisonTypeToHtmlCharacter(string comparisonType)
        {
            switch (comparisonType)
            {
                case "GreaterOrEqual":
                    return WebUtility.HtmlDecode("&ge;");
                case "Less":
                    return WebUtility.HtmlDecode("<");
                default:
                    return "";
            }
        }

        public static string StringWithArticle(this string text)
        {
            var prefix = text.Substring(0, 1);
            if (WordsHaveAnArticle.Any(x => x.Equals(prefix, StringComparison.OrdinalIgnoreCase)))
                return $"an {text}";
            else
                return $"a {text}";
        }

        public static List<string> ToStringList(this string source, string separator = ",")
        {
            return source?.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList() ?? new List<string>();
        }

        public static JArray ParseToJArray(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new JArray();
            }
            try
            {
                var value = JArray.Parse(json);
                return value;
            }
            catch (Exception)
            {
                return new JArray();
            }
        }
    }
}
