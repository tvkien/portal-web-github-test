using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.ResponseProcessingDTO;

namespace LinkIt.BubbleSheetPortal.Services.ResponseProcessingParser
{
    public class DragDropNumericalRPParser : IResponseProcessingParser
    {
        public BaseResponseProcessingDTO Parse(string rpXml)
        {
            var result = new DragDropNumericalRPDTO();
            if (string.IsNullOrWhiteSpace(rpXml)) return result;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(rpXml);

                var elemList = doc.GetElementsByTagName("process");
                if (elemList.Count == 0) return result;

                var xmlAttributeCollection = elemList[0].Attributes;
                if (xmlAttributeCollection == null) return result;
                result.PointsValue = XmlUtils.GetIntNullable(xmlAttributeCollection, "pointsValue");
                result.Identifier = xmlAttributeCollection["identifier"].Value;
                result.ExpressionPattern = xmlAttributeCollection["expressionPattern"].Value;

                var sources = doc.GetElementsByTagName("source");

                result.Sources = new List<SrcOfDragDropNumericalRPDTO>();
                foreach (XmlNode source in sources)
                {
                    var sourceDTO = new SrcOfDragDropNumericalRPDTO();
                    foreach (XmlNode childNode in source.ChildNodes)
                    {
                        if (childNode.Name == "srcIdentifier") sourceDTO.SrcIdentifier = childNode.InnerText;
                        if (childNode.Name == "value") sourceDTO.Value = childNode.InnerText;
                    }

                    result.Sources.Add(sourceDTO);
                }

                if (!string.IsNullOrWhiteSpace(result.ExpressionPattern))
                {
                    var reg = @"{(.*?)}";
                    var m = Regex.Matches(result.ExpressionPattern, reg);
                    result.Destinations = new List<string>();
                    foreach (Match g in m)
                    {
                        var value = g.Groups[1].Value;
                        result.Destinations.Add(value);
                    }
                }

                return result;
            }
            catch
            {
                return result;
            }
        }
    }
}
