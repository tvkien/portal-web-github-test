using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Common.JsonExtension
{
    public static class JsonExtension
    {
        public static string JsonModify(this string jsonString, string jsonPath, object value)
        {
            JToken token = JToken.Parse(jsonString);
            var jValue = value is null ? new JValue((object)null) : JToken.FromObject(value);

            var property = token.SelectToken(jsonPath);
            if (property == null)
                return string.Empty;

            property.Replace(jValue);
            return token.ToString(Formatting.None);
        }
    }
}
