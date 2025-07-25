using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.SqlServer.Server;

namespace LinkIt.BubbleSheetPortal.Common
{
    public class ExamviewXmlContentProcessing
    {
        private readonly string _lineFeedReplace = Guid.NewGuid().ToString();
        private readonly string _htmlEntityReplace = Guid.NewGuid().ToString();
        private readonly XmlSpecialCharToken _xmlSpecialCharToken = new XmlSpecialCharToken(false);
        private XmlDocument _xDoc;
        private readonly string _xmlContent;

        public ExamviewXmlContentProcessing(string xmlContent)
        {
            xmlContent = GetExamviewXml(xmlContent);
            xmlContent = xmlContent.Replace("&nbsp;", "&#160;");
            //xmlContent = xmlContent.Replace("&#", _htmlEntityReplace);
            xmlContent = xmlContent.ReplaceXmlSpecialChars(_xmlSpecialCharToken);

            xmlContent = xmlContent.Replace("\r\n", "\n");
            xmlContent = xmlContent.Replace("\r", "\n");
            xmlContent = xmlContent.Replace("\n", _lineFeedReplace);

            _xmlContent = xmlContent;

            _xDoc = new XmlDocument();
            _xDoc.PreserveWhitespace = true;
            try
            {
                _xDoc.LoadXml(xmlContent);
            }
            catch (Exception)
            {
                _xDoc = null;
            }
            
        }
        private static string GetExamviewXml(string xmlContent)
        {
            if (xmlContent == null) return null;
            var result = xmlContent.Trim();

            const string openPool = "<PooL>";
            const string closePool = "</PooL>";

            var indexOfStart = result.IndexOf(openPool, StringComparison.OrdinalIgnoreCase);
            if (indexOfStart > 0)
            {
                var indexEnd = result.IndexOf(closePool, StringComparison.OrdinalIgnoreCase) + closePool.Length;
                result = result.Substring(indexOfStart, indexEnd - indexOfStart);
            }

            return result;
        }
        public string GetXmlContent()
        {
            if (_xDoc == null) return string.Empty;
            var result = _xDoc.OuterXml;
           
            result = GetOrginalContent(result);

            return result;
        }

        public string GetOrginalContent(string processedContent)
        {
            //var result  = processedContent.Replace(_htmlEntityReplace, "&#");
            var result = processedContent.RecoverXmlSpecialChars(_xmlSpecialCharToken);
            result = result.Replace(_lineFeedReplace, "\n");
            return result;
        }
      
        public XmlNodeList GetElementsByTagName(string tagName)
        {
            return _xDoc.GetElementsByTagName(tagName);
        }
    }
}
