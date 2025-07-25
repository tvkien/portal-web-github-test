using System.Text.RegularExpressions;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class XmlTagExtensions
    {
        public static string LowerCaseTags(string xml)
        {
            return Regex.Replace(
                xml,
                @"<[^<>]+>",
                m => { return m.Value.ToLower(); },
                RegexOptions.Multiline | RegexOptions.Singleline);
        }
    }
}