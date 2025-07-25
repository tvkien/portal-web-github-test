using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.TestMaker;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class XmlContentSerializer
    {
        public AssessmentItem Deserialize(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent)) return null;

            var xdoc = new XmlDocument();
            xdoc.LoadXml(xmlContent);
            foreach (XmlNode assessmentItemNode in xdoc.ChildNodes)
            {
                if (!string.Equals(assessmentItemNode.Name, "assessmentItem", StringComparison.CurrentCultureIgnoreCase))
                    continue;
                var result = new AssessmentItem
                {
                    TimeDependent = XmlUtils.GetNodeAttributeCaseInSensitive(assessmentItemNode, "timeDependent"),
                    QtiSchemeID = XmlUtils.GetNodeAttributeCaseInSensitive(assessmentItemNode, "qtiSchemeID"),
                    ResponseDeclarations = new List<ResponseDeclaration>(),
                    ResponseIdentifiers = GetResponseIdentifiers(xdoc)
                };

                foreach (XmlNode node in assessmentItemNode.ChildNodes)
                {
                    if (string.Equals(node.Name, "responseDeclaration", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var responseDeclaration = DeserializeResponseDeclaration(node);
                        responseDeclaration.ResponseDeclarationXml = XmlUtils.RemoveAllNamespaces(node).OuterXml;
                        result.ResponseDeclarations.Add(responseDeclaration);
                    }

                    if (result.QtiSchemeID == ((int)QTISchemaEnum.ChoiceMultipleVariable).ToString()
                        && string.Equals(node.Name, "itemBody", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.ResponseDeclarations[0].CorrectResponses[0].AnswerTexts = new List<string>();
                        List<XmlNode> simpleChoices = new List<XmlNode>();
                        XmlUtils.RecurseGetChildNodeListByName(node, "simpleChoice", false, simpleChoices);
                        foreach(var simpleChoice in simpleChoices)
                        {
                            var answerText = XmlUtils.GetSingleChildNodeByName(simpleChoice, "div").InnerText;
                            result.ResponseDeclarations[0].CorrectResponses[0].AnswerTexts.Add(answerText);
                        }
                    }
                }
                
                return result;
            }

            return null;
        }

        public ResponseDeclaration DeserializeResponseDeclaration(XmlNode node)
        {
            var result = new ResponseDeclaration
            {
                AbsoluteGrading = XmlUtils.GetNodeAttributeCaseInSensitive(node, "absoluteGrading"),
                AbsoluteGradingPoints = XmlUtils.GetNodeAttributeCaseInSensitive(node, "absoluteGradingPoints"),
                BaseType = XmlUtils.GetNodeAttributeCaseInSensitive(node, "baseType"),
                Cardinality = XmlUtils.GetNodeAttributeCaseInSensitive(node, "cardinality"),
                CaseSensitive = XmlUtils.GetNodeAttributeCaseInSensitive(node, "caseSensitive"),
                Depending = XmlUtils.GetNodeAttributeCaseInSensitive(node, "depending"),
                Identifier = XmlUtils.GetNodeAttributeCaseInSensitive(node, "identifier"),
                Major = XmlUtils.GetNodeAttributeCaseInSensitive(node, "major"),
                Method = XmlUtils.GetNodeAttributeCaseInSensitive(node, "method"),
                PartialGradingThreshold = XmlUtils.GetNodeAttributeCaseInSensitive(node, "partialGradingThreshold"),
                PointsValue = XmlUtils.GetNodeAttributeCaseInSensitive(node, "pointsValue"),
                Range = XmlUtils.GetNodeAttributeCaseInSensitive(node, "range"),
                RelativeGrading = XmlUtils.GetNodeAttributeCaseInSensitive(node, "relativeGrading"),
                RelativeGradingPoints = XmlUtils.GetNodeAttributeCaseInSensitive(node, "relativeGradingPoints"),
                Spelling = XmlUtils.GetNodeAttributeCaseInSensitive(node, "spelling"),
                SpellingDeduction = XmlUtils.GetNodeAttributeCaseInSensitive(node, "spellingDeduction"),
                Type = XmlUtils.GetNodeAttributeCaseInSensitive(node, "type"),
                AlgorithmicGrading = XmlUtils.GetNodeAttributeCaseInSensitive(node, "algorithmicGrading"),
                AllOrNothingGrading = XmlUtils.GetNodeAttributeCaseInSensitive(node, "allOrNothingGrading"),

                CorrectResponses = new List<CorrectResponse>()
            };

            foreach (XmlNode correctResponseNode in node.ChildNodes)
            {
                if (!string.Equals(correctResponseNode.Name, "correctResponse",
                        StringComparison.CurrentCultureIgnoreCase)) continue;

                var correctResponse = DeserializeCorrectResponse(correctResponseNode);
                result.CorrectResponses.Add(correctResponse);
            }

            return result;
        }

        public CorrectResponse DeserializeCorrectResponse(XmlNode node)
        {
            var result = new CorrectResponse
            {
                DestIdentifier = XmlUtils.GetNodeAttributeCaseInSensitive(node, "destIdentifier"),
                Identifier = XmlUtils.GetNodeAttributeCaseInSensitive(node, "identifier"),
                SrcIdentifier = XmlUtils.GetNodeAttributeCaseInSensitive(node, "srcIdentifier"),
				Values = new List<string>(),
                ValuesXML = new List<string>()
            };
            if (XmlUtils.GetNodeAttribute(node, "pointValue") != null)
            {
                result.PointValue = Int32.Parse(XmlUtils.GetNodeAttribute(node, "pointValue"));
            }
			
			foreach (XmlNode valueNode in node.ChildNodes)
            {
                if (!string.Equals(valueNode.Name, "value", StringComparison.CurrentCultureIgnoreCase))
                    continue;
                var xmlns = string.Format(" xmlns=\"{0}\"", valueNode.NamespaceURI);
                var value = valueNode.InnerXml.Replace(xmlns, "");
                var valueXML = valueNode.OuterXml.Replace(xmlns, "");
                result.Values.Add(value);
                result.ValuesXML.Add(valueXML);
            }

            return result;
        }

        public List<ResponseDeclaration> GetResponseDeclarations(XmlDocument xdoc)
        {
            var result = new List<ResponseDeclaration>();
            if (xdoc == null) return result;
            var nodes = xdoc.GetElementsByTagName("responseDeclaration");
            foreach (XmlNode node in nodes)
            {
                var responseDeclaration = DeserializeResponseDeclaration(node);
                result.Add(responseDeclaration);
            }

            return result;
        }

        public List<ResponseIdentifier> GetResponseIdentifiers(XmlDocument xdoc)
        {
            var result = new List<ResponseIdentifier>();
            if (xdoc == null)
            {
                return result;
            }

            var tagNames = EnumUtils.GetDescriptions(typeof(QTISchemaEnum)).ToList();

            // Use distinct here to because choiceInteraction tag exists twice (multiple choice single select and multi select)
            foreach (var tagName in tagNames.Distinct())
            {
                var nodes = xdoc.GetElementsByTagName(tagName);
                foreach (XmlNode node in nodes)
                {
                    var maxChoices = XmlUtils.GetNodeAttributeCaseInSensitive(node, "maxChoices");
                    var identifier = new ResponseIdentifier
                    {
                        Identifier = XmlUtils.GetNodeAttributeCaseInSensitive(node, "responseIdentifier"),
                        maxChoices = !string.IsNullOrWhiteSpace(maxChoices)
                            ? Convert.ToInt32(maxChoices)
                            : 0,
                        TagName = tagName,
                        variablePoints = XmlUtils.GetNodeAttributeCaseInSensitive(node, "variablePoints")
                    };

                    result.Add(identifier);
                }
            }

            return result;
        }

        public List<ResponseIdentifier> GetResponseIdentifiers(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent)) return new List<ResponseIdentifier>();

            var xml = new XmlDocument();
            xml.LoadXml(xmlContent);

            var result = GetResponseIdentifiers(xml);

            return result;
        }

        public Dictionary<string, int> GetImageIndexInQuestion(string xmlContent)
        {
            var result = new Dictionary<string, int>();
            var xdoc = new XmlDocument();
            xdoc.LoadXml(xmlContent);
            var images = xdoc.GetElementsByTagName("img");
            if(images != null && images.Count > 0)
            {
                for (int i = 0; i < images.Count; i++)
                {
                    var drawable = XmlUtils.GetNodeAttributeCaseInSensitive(images[i], "drawable");
                    if(drawable == "true" && images[i].ParentNode != null && images[i].ParentNode.Name == "extendedTextInteraction")
                    {
                        var responseIdentifier = XmlUtils.GetNodeAttributeCaseInSensitive(images[i].ParentNode, "responseIdentifier");
                        result.Add(responseIdentifier, i);
                    }
                }
            }
            return result;
        }
    }
}
