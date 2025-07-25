using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Common
{
    public class HtmlUtils
    {
        public static string ConvertTags(string data, List<string> tagNames, string destTageName)
        {
            return ConvertTags(data, tagNames, destTageName, string.Empty, string.Empty);
        }

        public static string ConvertTags(string data, List<string> tagNames, string destTageName, string clazz, string attr)
        {
            foreach (var token in tagNames)
            {
                var open = new KeyValuePair<string, string>(string.Format("<{0}", token),
                    string.Format("<{0} class=\"{1}{2}\"{3}", destTageName, token, clazz, attr));
                var close = new KeyValuePair<string, string>(string.Format("</{0}>", token), string.Format("</{0}>", destTageName));
                data = data.Replace(open.Key, open.Value);
                data = data.Replace(close.Key, close.Value);
            }

            return data;
        }

        public static string UpdateImageUrls(string imgPath, string content)
        {
            content = content.Trim();
            if (content.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>"))//For DataUploadFile Progressive
            {
                content = content.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            }
            var xmlContentProcessing = new XmlContentProcessing(content);
            xmlContentProcessing.UpdateImageUrls(imgPath);
            var result = xmlContentProcessing.GetXmlContent();

            return result;
        }
    }
}
