using System;
using System.Xml;

namespace LinkIt.BubbleSheetPortal.Common
{
    public class XmlContentDocument : XmlDocument
    {
        private readonly string _lineFeedReplace = string.Format("<![CDATA[{0}]]>", Guid.NewGuid());
        private readonly string _spaceReplace = string.Format("<![CDATA[{0}]]>", Guid.NewGuid());
        private readonly XmlSpecialCharToken _xmlSpecialCharToken = new XmlSpecialCharToken();
        public override void LoadXml(string xml)
        {
            xml = xml.Replace("&nbsp;", "&#160;");
            //xml = xml.Replace("&#", _spaceReplace);
            xml = xml.ReplaceXmlSpecialChars(_xmlSpecialCharToken);

            xml = xml.Replace("\r\n", "\n");
            xml = xml.Replace("\r", "\n");
            xml = xml.Replace("\n", _lineFeedReplace);

            base.LoadXml(xml);
        }

        public override string OuterXml
        {
            get
            {
                var result = base.OuterXml;
                //result = result.Replace(_spaceReplace, "&#");
                result = result.RecoverXmlSpecialChars(_xmlSpecialCharToken);
                result = result.Replace(_lineFeedReplace, "\n");

                return result;
            }
        }
    }
}
