using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class UrlUtil
    {
        public static bool CheckUrlStatus(string Website)
        {
            try
            {
                var request = WebRequest.Create(Website) as HttpWebRequest;
                request.Method = "HEAD";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }
        public static string GenerateS3Subdomain(string s3Domain, string bucket)
        {
            //With s3domain looks like https://s3.amazonaws.com/
            //Bucket name look like testitemmedia
            //Need to return https://testitemmedia.s3.amazonaws.com/

            var result = string.Empty;
            if (string.IsNullOrEmpty(s3Domain) || string.IsNullOrEmpty(bucket))
            {
                return result;
            }
            if (s3Domain.ToLower().StartsWith("http://"))
            {
                result = string.Format("http://{0}.{1}", bucket.RemoveEndSlash().RemoveStartSlash(), s3Domain.ToLower().Remove(0, 7));
            }
            else if (s3Domain.ToLower().StartsWith("https://"))
            {
                result = string.Format("https://{0}.{1}", bucket.RemoveEndSlash().RemoveStartSlash(), s3Domain.ToLower().Remove(0, 8));
            }

            return result;
        }

        public static string GenerateS3DownloadLink(string s3Domain, string bucket, string folder, string fileName)
        {
            if (string.IsNullOrWhiteSpace(s3Domain) || string.IsNullOrWhiteSpace(bucket) || string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            var s3SubDomain = GenerateS3Subdomain(s3Domain, bucket);
            if (string.IsNullOrWhiteSpace(s3SubDomain)) return string.Empty;

            var result = string.Format("{0}/{1}/{2}", RemoveStartEndSlash(s3SubDomain), RemoveStartEndSlash(folder), RemoveStartEndSlash(fileName));

            return result;
        }

        public static string RemoveStartEndSlash(string str)
        {
            return str.RemoveEndSlash().RemoveStartSlash();
        }

        public static string CleanUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;

            var result = url.Replace("\\", "/");
            var arr = result.Split(new String['/'], StringSplitOptions.RemoveEmptyEntries);
            result = String.Join("/", arr);

            return result;
        }
    }
}