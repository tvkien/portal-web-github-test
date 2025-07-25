using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Data.SqlTypes;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class ExtensionMethods
    {
        public static string JoinToString(this IEnumerable<string> strings, string separater)
        {
            return string.Join(separater, strings ?? new string[0]);
        }
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static string ReplaceInsensitive(this string s, string oldValue, string newValue)
        {
            if (oldValue == null || newValue == null) return s;
            var regEx = new Regex(oldValue,
               RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Replace(s, newValue);
        }
        public static string RemoveZeroWidthSpaceCharacterFromUnicodeString(this string inString)
        {
            if (inString == null) return null;
            var outString = inString.Replace("\u200B", ""); ;

            return outString;
        }
        public static List<int?> ParseIdsFromStringAsNullableInt(this string s)
        {
            return ParseIdsFromString(s).Select(c => (int?)c).ToList();
        }

        public static List<int> ParseIdsFromString(this string s)
        {
            s = s ?? string.Empty;
            s = s.Replace("-", "");
            var tmp = s.Split(new char[] { ',' });
            var result = new List<int>();
            if (tmp != null)
            {
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (!string.IsNullOrEmpty(tmp[i]))
                    {
                        result.Add(Int32.Parse(tmp[i]));
                    }

                }
            }
            return result;
        }
        public static string ReplaceXmlSpecialChars(this string s, XmlSpecialCharToken token)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                foreach (var item in token.SpecialCharReplacementDic)
                {
                    s = s.Replace(item.Key, item.Value);
                }
            }
            return s;
        }
        public static string RecoverXmlSpecialChars(this string s, XmlSpecialCharToken token)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                s = s.DecodeHtmlSpecialChars();
                foreach (var item in token.SpecialCharReplacementDic)
                {
                    s = s.Replace(item.Value, item.Key);
                }
            }
            return s;
        }
        public static string AdjustFileNameForPDFPrinting(this string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return fileName;
            }
            string validChars = " 0123456789~!@#$%^&()-_=+[{]};',.abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ";

            string result = string.Empty;
            int i = 0;
            for (i = 0; i < fileName.Length; i++)
            {
                if (validChars.Contains(fileName[i].ToString()))
                {
                    result += fileName[i];
                }
            }

            return result;
        }

        public static string DecodeHtmlSpecialChars(this string s)
        {
            Dictionary<string, string> SpecialCharDictionary = new Dictionary<string, string>()
            {
                //some html characters that cause error when loading into xml
                //see full list here: http://www.degraeve.com/reference/specialcharacters.php
                {"&lsquo;","‘"},
                {"&times;", "Ã—" },
                //{"&rsquo;","’"},// has been processed in Web\Helper\Util.GetSpecialCharacters
                {"&sbquo;","‚"},
                //{"&ldquo;","“"},// has been processed in Web\Helper\Util.GetSpecialCharacters
                //{"&rdquo;","”"},// has been processed in Web\Helper\Util.GetSpecialCharacters
                //{"&bdquo;","„"},
                //{"&dagger;","†"},
                //{"&Dagger;","‡"},
                //{"&permil;","‰"},
                //{"&lsaquo;","‹"},
                //{"&rsaquo;","›"},
                //{"&spades;","♠"},
                //{"&clubs;","♣"},
                //{"&hearts;","♥"},
                //{"&diams;","♦"},
                //{"&oline;", "‾"},
                //{"&larr;", "←"},
                //{"&uarr;", "↑"},
                //{"&rarr;", "→"},
                //{"&darr;", "↓"},
                //{"&trade;", "™"},

                //{"&hellip;", "…"},
                {"&ndash;", "–"},
                {"&mdash;", "—"},
                //{"&iexcl;", "¡"},
                //{"&cent;", "¢"},
                //{"&pound;", "£"},
                //{"&curren;", "¤"},
                //{"&yen;", "¥"},
                //{"&brvbar;", "¦"},
                //{"&brkbar;", "¦"},
                //{"&sect;", "§"},
                //{"&uml;", "¨"},
                //{"&die;", "¨"},
                //{"&copy;", "©"},
                //{"&ordf;", "ª"},
                //{"&laquo;", "«"},
                //{"&not;", "¬"},
                //{"&shy;", ""},
                //{"&reg;", "®"},
                //{"&macr;", "¯"},
                //{"&hibar;", "¯"},
                //{"&deg;", "°"},
                //{"&plusmn;", "±"},
                //{"&sup2;", "²"},
                //{"&sup3;", "³"},
                //{"&acute;", "´"},
                //{"&micro;", "µ"},
                //{"&para;", "¶"},
                //{"&middot;", "·"},
                //{"&cedil;", "¸"},
                //{"&sup1;", "¹"},
                //{"&ordm;", "º"},
                //{"&raquo;", "»"},

                //{"&frac14;", "¼"},
                //{"&frac12;", "½"},
                //{"&frac34;", "¾"},
                //{"&iquest;", "¿"},

                //{"&Agrave;","À"},//in pallet,not process
                //{"&Aacute;","Á"},//in pallet,not process
                //{"&Acirc;","Â"},//in pallet,not process
                //{"&Atilde;","Ã"},//in pallet,not process
                //{"&Auml;","Ä"},//in pallet,not process
                //{"&Aring;","Å"},
                //{"&AElig;","Æ"},

                //{"&Ccedil;","Ç"},//in pallet,not process

                //{"&Egrave;","È"},//in pallet,not process
                //{"&Eacute;","É"},//in pallet,not process
                //{"&Ecirc;","Ê"},//in pallet,not process
                //{"&Euml;","Ë"},//in pallet,not process

                //{"&Igrave;","Ì"},//in pallet,not process
                //{"&Iacute;","Í"},//in pallet,not process
                //{"&Icirc;","Î"},//in pallet,not process

                //{"&Iuml;","Ï"},//in pallet,not process
                //{"&ETH;","Ð"},
                //{"&Ntilde;","Ñ"},//in pallet,not process

                //{"&Ograve;","Ò"},//in pallet,not process
                //{"&Oacute;","Ó"},//in pallet,not process
                //{"&Ocirc;","Ô"},//in pallet,not process
                //{"&Otilde;","Õ"},
                //{"&Ouml;","Ö"},//in pallet,not process

                //{"&times;","×"}, //in pallet,not process
                //{"&Oslash;","Ø"},

                //{"&Ugrave;","Ù"},//in pallet,not process
                //{"&Uacute;","Ú"},
                //{"&Ucirc;","Û"},//in pallet,not process
                //{"&Uuml;","Ü"},//in pallet,not process

                //{"&Yacute;","Ý"},
                //{"&THORN;","Þ"},
                //{"&szlig;","ß"},

                //{"&agrave;","à"},//in pallet,not process
                //{"&aacute;","á"},//in pallet,not process
                //{"&acirc;","â"},//in pallet,not process
                //{"&atilde;","ã"},//in pallet,not process
                //{"&auml;","ä"},//in pallet,not process
                //{"&aring;","å"},
                //{"&aelig;","æ"},

                //{"&ccedil;","ç"},//in pallet,not process

                //{"&egrave;","è"},//in pallet,not process
                //{"&eacute;","é"},//in pallet,not process
                //{"&ecirc;","ê"},//in pallet,not process
                //{"&euml;","ë"},//in pallet,not process

                //{"&igrave;","ì"},//in pallet,not process
                //{"&iacute;","í"},//in pallet,not process
                //{"&icirc;","î"},//in pallet,not process
                //{"&iuml;","ï"},//in pallet,not process

                //{"&eth;","ð"},
                //{"&ntilde;","ñ"},//in pallet,not process
                //{"&ograve;","ò"},//in pallet,not process
                //{"&oacute;","ó"},//in pallet,not process
                //{"&ocirc;","ô"},//in pallet,not process
                //{"&otilde;","õ"},//in pallet,not process
                //{"&ouml;","ö"},//in pallet,not process
                //{"&divide;","÷"},//in pallet,not process
                //{"&oslash;","ø"},

                //{"&ugrave;","ù"},//in pallet,not process
                //{"&uacute;","ú"},//in pallet,not process
                //{"&ucirc;","û"},//in pallet,not process
                //{"&uuml;","ü"},//in pallet,not process

                //{"&yacute;","ý"},
                //{"&thorn;","þ"},
                //{"&yuml;","ÿ"},
                //{"&Alpha;","Α"},
                //{"&alpha;","α"},//in pallet,not process
                //{"&Beta;","Β"},//in pallet,not process
                //{"&beta;","β"},//in pallet,not process
                //{"&Gamma;","Γ"},
                //{"&gamma;","γ"},//in pallet,not process
                //{"&Delta;","Δ"},//in pallet,not process
                //{"&delta;","δ"},//in pallet,not process
                //{"&Epsilon;","Ε"},
                //{"&epsilon;","ε"},
                //{"&Zeta;","Ζ"},
                //{"&zeta;","ζ"},
                //{"&Eta;","Η"},
                //{"&eta;","η"},//in pallet,not process
                //{"&Theta;","Θ"},//in pallet,not process
                //{"&theta;","θ"},//in pallet,not process
                //{"&Iota;","Ι"},
                //{"&iota;","ι"},
                //{"&Kappa;","Κ"},//in pallet,not process
                //{"&kappa;","κ"},//in pallet,not process
                //{"&Lambda;","Λ"},//in pallet,not process
                //{"&lambda;","λ"},//in pallet,not process
                //{"&Mu;","Μ"},
                //{"&mu;","μ"},
                //{"&Nu;","Ν"},
                //{"&nu;","Ξ"},
                //{"&Xi;",""},
                //{"&xi;","ξ"},
                //{"&Omicron;","Ο"},
                //{"&omicron;","ο"},
                //{"&Pi;","Π"},//in pallet,not process
                //{"&pi;","π"},//in pallet,not process
                //{"&Rho;","Ρ"},//in pallet,not process
                //{"&rho;","ρ"},
                //{"&sigma;","σ"},//in pallet,not process
                //{"&Tau;","Τ"},//in pallet,not process
                //{"&tau;","τ"},
                //{"&Upsilon;","Υ"},
                //{"&upsilon;","υ"},
                //{"&Phi;","Φ"},
                //{"&phi;","φ"},
                //{"&Chi;","Χ"},
                //{"&chi;","χ"},
                //{"&Psi;","Ψ"},//in pallet,not process
                //{"&psi;","ψ"},//in pallet,not process
                //{"&Omega;","Ω"},//in pallet,not process
                //{"&omega;","ω"},//in pallet,not process
                {"&bull;","•"}
            };
            foreach (var item in SpecialCharDictionary)
            {
                if (!string.IsNullOrWhiteSpace(item.Value))
                {
                    s = s.Replace(item.Value, item.Key);
                }
            }
            return s;
        }

        // http://stackoverflow.com/questions/20762/how-do-you-remove-invalid-hexadecimal-characters-from-an-xml-based-data-source-p/14912930#14912930
        public static string RemoveTroublesomeCharacters(this string inString)
        {
            if (inString == null) return null;
            char[] validXmlChars = inString.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
            return new string(validXmlChars);
        }

        public static string GetFullExceptionMessage(this Exception ex)
        {
            string message = string.Empty;
            message = ex.GetType().ToString();
            message += ex.Message;
            if (ex.InnerException != null)
            {
                if (!string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    message += ex.InnerException.Message;
                }
            }
            return message;
        }

        /// <summary>
        /// Return a file name with timestamp added
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string AddTimestampToFileName(this string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                return String.Empty;
            }
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);

            if (!String.IsNullOrWhiteSpace(fileNameWithoutExt))
            {
                fileNameWithoutExt = String.Format("{0}-{1}", fileNameWithoutExt,
                                                   DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
            }
            if (!String.IsNullOrWhiteSpace(fileNameWithoutExt))
            {
                fileName = fileNameWithoutExt;
            }
            if (!String.IsNullOrWhiteSpace(ext) && !String.IsNullOrWhiteSpace(fileName))
            {
                fileName = String.Format("{0}{1}", fileName, ext);
            }
            return fileName;

        }
        /// <summary>
        /// Return a new file path with the file name added with timestamp
        /// </summary>
        /// <param name="filePath">File path with file name need to add timestamp</param>
        /// <returns></returns>
        public static string AddTimestampToFilePath(this string filePath)
        {
            var newFileNameWithTimeStamp = filePath.AddTimestampToFileName();
            if (string.IsNullOrEmpty(newFileNameWithTimeStamp))
            {
                return filePath;
            }
            else
            {
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
                int idx = filePath.LastIndexOf(fileNameWithoutExt);
                if (idx >= 0)
                {
                    string newFilePath = filePath.Substring(0, idx);
                    newFilePath += newFileNameWithTimeStamp;
                    return newFilePath;
                }
                return filePath;
            }
        }
        /// <summary>
        /// Check if an url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsExisting(this string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //get passage json file ( like RO_XXXX.json ) then read the passage content
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    var result = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch
            {
                return false;
            }
            return false;
        }

        public static string RemoveStartSlash(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            while (s.StartsWith("/"))
            {
                s = s.Remove(0, 1);
            }
            return s;
        }
        public static string RemoveEndSlash(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            while (s.EndsWith("/"))
            {
                s = s.Remove(s.Length - 1, 1);
            }
            return s;
        }
        /// <summary>
        /// Process some special characters may cause query error on database
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ProcessWildCharacters(this string s)
        {
            if (s == null)
            {
                return null;
            }
            return s.Replace("'", "''");
        }
        /// <summary>
        /// Decode parameters with was encoded with encodeParameter(s)  in custom.js on client 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DecodeParameters(this string s)
        {
            if (s == null)
            {
                return null;
            }
            s = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(s));
            return s.ProcessWildCharacters();
        }

        public static string DecodeHtml(this string s)
        {
            if (s == null)
            {
                return null;
            }
            s = HttpUtility.HtmlDecode(s);
            return s;
        }
        public static bool IsUnicode(this string input)
        {
            var asciiBytesCount = Encoding.ASCII.GetByteCount(input);
            var unicodBytesCount = Encoding.UTF8.GetByteCount(input);
            return asciiBytesCount != unicodBytesCount;
        }
        public static string CleanUpXmlContentInput(this string xmlContent)
        {
            //Remove unhandle control characters
            xmlContent = xmlContent.RemoveTroublesomeCharacters();
            xmlContent = xmlContent.RemoveZeroWidthSpaceCharacterFromUnicodeString();
            xmlContent = xmlContent.DecodeHtmlSpecialChars();
            return xmlContent;
        }

        public static string DisplayDateWithFormat(this DateTime? date, bool showTime = false)
        {
            if (!date.HasValue)
            {
                return string.Empty;
            }
            return DisplayDateWithFormat(date.Value, showTime);
        }
        public static string DisplayDateWithFormat(this DateTime date, bool showTime = false)
        {
            var ckDefaultDateFormat = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
            var ckDefaultTimeFormat = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultTimeFormat];

            var dateFormat = ckDefaultDateFormat == null ? Constanst.DefaultDateFormatValue : ckDefaultDateFormat.Value;
            var timeFormat = ckDefaultTimeFormat == null ? Constanst.DefaultTimeFormatValue : ckDefaultTimeFormat.Value;

            return DisplayDateWithFormat(date, dateFormat, timeFormat, showTime);
        }
        public static string DisplayDateWithFormat(this DateTime date, string dateFormat, string timeFormat, bool showTime = false)
        {
            if (string.IsNullOrWhiteSpace(dateFormat))
            {
                return string.Empty;
            }
            if (showTime) //somewhere needs to display time but there's no time config, set the default format of showing time is h:mm tt
            {
                if (string.IsNullOrWhiteSpace(timeFormat))
                {
                    if (dateFormat.IndexOf("m") < 0 && dateFormat.IndexOf("h") < 0)
                    {
                        dateFormat = dateFormat + " h:mm tt";
                    }
                }
                else
                {
                    dateFormat = dateFormat + " " + timeFormat;
                }

            }
            return date.ToString(dateFormat);
        }
        public static bool TryParseDateWithFormat(this string dateInString, out DateTime result)
        {
            var dateFormatCookie = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];

            string dateFormat = dateFormatCookie == null ? Constanst.DefaultDateFormatValue : dateFormatCookie.Value;
            CultureInfo provider = CultureInfo.InvariantCulture;
            if (DateTime.TryParseExact(dateInString, dateFormat, provider, DateTimeStyles.None, out result))
            {
                return true;
            }
            else
            {
                result = Convert.ToDateTime(dateInString);
                return false;
            }
        }
        public static string SerializeToJson(this object obj)
        {
            var json = new JavaScriptSerializer().Serialize(obj);
            return json;
        }

        public static string FormatDecimalPoint(this decimal d, int numberOfDecimal)
        {
            var s = string.Empty;
            string format = "#.";
            for (var i = 0; i < numberOfDecimal; i++)
            {
                format += "0";
            }
            s = d.ToString(format);
            if (d == 0)
            {
                s = "0" + s;
            }
            return s;
        }

        public static string TryParseDateWithFormats(this string dateInString, List<string> formats)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime result;
            if (DateTime.TryParseExact(dateInString, formats.ToArray(), provider, DateTimeStyles.None, out result))
            {
                return result.ToLongDateString();
            }
            return string.Empty;
        }

        public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        public static DateTime ConvertTimeFromUtc(this DateTime dateTime, string timeZoneId)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZoneInfo);
        }

        public static T DeserializeObject<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
