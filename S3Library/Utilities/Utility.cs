using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S3Library.Utilities
{
    public class Utility
    {
        public static string BuildPortalKey()
        {
            return EncodeGuid(Guid.NewGuid());
        }

        private static string EncodeGuid(Guid guid)
        {
            string enc = Convert.ToBase64String(guid.ToByteArray());
            enc = enc.Replace("/", "_");
            enc = enc.Replace("+", "-");
            return enc.Substring(0, 22);
        }
        public static string RemoveStartSlash(string s)
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
        public static string RemoveEndSlash(string s)
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

        public static string CleanUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;

            var result = url.Replace("\\", "/");
            var arr = result.Split(new String[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            result = String.Join("/", arr);

            return result;
        }
    }
}
