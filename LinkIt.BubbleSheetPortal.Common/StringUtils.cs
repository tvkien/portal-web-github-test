using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class StringUtils
    {
        public static string ReplaceWeirdCharacters1(string str)
        {
            // Just convert to Utf8 when this string is a Ansi string
            //if (!IsANSIString(str)) //no use this function, from now, data is alway encoded to window1252 when saving so data is convert from window1252 to unicode when displaying
            //    return str;

            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            str = str.ReplaceSpecialCharacters();
            var mc = Regex.Matches(str, @"&#\s*;");
            foreach (Match m in mc)
            {
                str = str.Replace(m.ToString(), System.Web.HttpUtility.HtmlDecode(m.ToString()));
            }

            var result = str.ConvertFromWindow1252ToUnicode();

            result = result.Replace("“", "\"");//“ must not be replaced in ReplaceSpecialCharacters because there might be some charaters contain “ in ConvertFromWindow1252ToUnicode , such as ("Ã”" -> "Ó")
            result = result.Replace("”", "\"");
            result = result.Replace("�", "");

            return result;
        }

        public static string ConvertFromWindow1252ToUnicode(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            //There'are some character, that were encoded by window1252 but when they are decoded backto window1252 ,they are wrong
            //Must to manually replace these characters
            foreach (var specialCharacter in Window1252EncodedSpecialChars)
            {
                var temp = specialCharacter.Value.ConvertFromUnicodeToWindow1252();
                //It's impossible to replace directly so that it must encode the original before replacing
                str = str.Replace(specialCharacter.Key, temp);
                // replace directly str.Replace(specialCharacter.Key, specialCharacter.Value) will cause wrong decoded result
            }
            var result = Encoding.UTF8.GetString(Encoding.GetEncoding("Windows-1252").GetBytes(str));
            return result;
        }

        public static string ConvertFromUnicodeToWindow1252(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            var result = Encoding.GetEncoding("Windows-1252").GetString(Encoding.UTF8.GetBytes(str));
            return result;
        }

        public static List<KeyValuePair<string, string>> Window1252EncodedSpecialChars = new List<KeyValuePair<string, string>> {
            //new KeyValuePair<string, string>("âˆ ", "∠"),
            //new KeyValuePair<string, string>("âˆ\"", "∓"),
            //new KeyValuePair<string, string>("â„\"", "ℓ"),
            ////new KeyValuePair<string, string>("Ã\"", "Ó"),
            //new KeyValuePair<string, string>("â€\"", "—"),
            //new KeyValuePair<string, string>("â€“", "–"),
            //new KeyValuePair<string, string>("â‰ ", "≠"),
            ////new KeyValuePair<string, string>("Ã”", "Ó"),
            ////new KeyValuePair<string, string>("Ã“", "Ô"),
            //new KeyValuePair<string, string>("Ã'", "Ò"),
            //new KeyValuePair<string, string>("å‡", "几")
        };

        public static string ReplaceSpecialCharacters(this string str)
        {
            var specialCharacters = GetSpecialCharacters();
            foreach (var specialCharacter in specialCharacters)
            {
                str = str.Replace(specialCharacter.Key, specialCharacter.Value);
            }

            return str;
        }

        private static List<KeyValuePair<string, string>> GetSpecialCharacters()
        {
            var results = new List<KeyValuePair<string, string>>();
            results.Add(new KeyValuePair<string, string>("Ã ", "&#224;"));

            //results.Add(new KeyValuePair<string, string>("â€“", "-"));
            //results.Add(new KeyValuePair<string, string>("â€™", "'"));
            //results.Add(new KeyValuePair<string, string>("â€œ", "\""));

            //results.Add(new KeyValuePair<string, string>("â€", "\""));
            //results.Add(new KeyValuePair<string, string>("'â€˜", "'"));
            //results.Add(new KeyValuePair<string, string>("â€¢", "-"));
            //results.Add(new KeyValuePair<string, string>(" ", " "));
            //results.Add(new KeyValuePair<string, string>(" ", " "));
            //results.Add(new KeyValuePair<string, string>("“", "\""));
            //results.Add(new KeyValuePair<string, string>("”", "\""));
            //results.Add(new KeyValuePair<string, string>("’", "'"));
            results.Add(new KeyValuePair<string, string>("Â ", "&nbsp;"));
            //results.Add(new KeyValuePair<string, string>("Î\"", "Î”"));
            //results.Add(new KeyValuePair<string, string>("âˆ'", "âˆ’"));

            return results;
        }

        public static string ReplaceWeirdCharacters(this string str)
        {
            // Just convert to Utf8 when this string is a Ansi string
            //if (!IsANSIString(str)) //no use this function, from now, data is alway encoded to window1252 when saving so data is convert from window1252 to unicode when displaying
            //    return str;

            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            str = str.ReplaceSpecialCharacters();
            str = str.DecodeHtmlCharacter();

            //var htmlEntityReplace = string.Format("<![CDATA[{0}]]>", Guid.NewGuid());

            //str = str.Replace("&#", htmlEntityReplace);
            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            str = str.ReplaceXmlSpecialChars(xmlSpecialCharToken);

            var result = str.ConvertFromWindow1252ToUnicode();
            //result = result.Replace(htmlEntityReplace, "&#");
            result = result.RecoverXmlSpecialChars(xmlSpecialCharToken);

            result = result.Replace("“", "\"");//“ must not be replaced in ReplaceSpecialCharacters because there might be some charaters contain “ in ConvertFromWindow1252ToUnicode , such as ("Ã”" -> "Ó")
            result = result.Replace("”", "\"");
            result = result.Replace("�", "");

            return result;
        }

        public static string ReplaceWeirdCharactersXmlContent(this string result)
        {
            if (string.IsNullOrWhiteSpace(result)) return string.Empty;
            result = result.Replace("'", "&prime;"); // ′ not '
            result = result.Replace("â€²", "&prime;");
            result = result.Replace("&apos;", "&prime;");
            result = result.Replace("&#x00027;", "&prime;");
            result = result.Replace("&#39;", "&prime;");

            return result;
        }

        public static string DecodeHtmlCharacter(this string str)
        {
            //MatchCollection mc = Regex.Matches(str, @"&#\w*;");//try to find string start with &#
            MatchCollection mc = Regex.Matches(str, @"&#\s*;");//try to find string start with &#
            foreach (Match m in mc)
            {
                str = str.Replace(m.ToString(), System.Web.HttpUtility.HtmlDecode(m.ToString()));
            }
            return str;
        }

        public static byte[] ConvertStreamToByteArray(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        public static string GetValidFileName(string filename)
        {
            string validPath = "";
            StringBuilder strBuilder = new StringBuilder(filename);
            char[] invalidChars = Path.GetInvalidPathChars();
            foreach (var c in invalidChars)
            {
                if (filename.Contains(c.ToString()))
                {
                    if (c.ToString() != @"\")
                    {
                        strBuilder.Replace(c.ToString(), "");
                    }
                }
            }
            validPath = strBuilder.ToString();
            return validPath;
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string CorrectFileNameOnURL(this string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return string.Empty;
            return Uri.EscapeDataString(filename);
        }
    }
}
