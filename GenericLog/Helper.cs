using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace GenericLog
{
    public static class Helper
    {
        public static Dictionary<string, object> ParseQueryStringToDictionary(HttpRequestBase request)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            //if (request.QueryString.Count > 0)
            //{
            //    foreach (String key in request.QueryString)
            //    {
            //        dic.Add(key, request.QueryString[key]);

            //    }
            //}
            //else if (request.Form.Count > 0)
            //{
            //    foreach (String key in request.Form)
            //    {
            //        dic.Add(key, request.Form[key]);
            //    }
            //}

            return dic;
        }

        public static string ParseResponseToJson(HttpResponseBase response)
        {
            return string.Empty;
        }

        public static string ParseDictionaryToJson(Dictionary<string, object> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": [{1}]", d.Key, string.Join(",", JsonConvert.SerializeObject(d.Value))));
            return "{" + string.Join(",", entries) + "}";
        }

        public static string ParseObjectToJson(object propValue)
        {
            var value = string.Empty;

            try
            {
                if (propValue != null)
                {
                    value = JsonConvert.SerializeObject(propValue);
                }
            }
            catch (Exception)
            {
            }

            return value;
        }
    }
}
