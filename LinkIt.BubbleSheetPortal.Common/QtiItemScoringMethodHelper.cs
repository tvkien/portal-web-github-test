using LinkIt.BubbleSheetPortal.Common.Enum;
using System.Collections.Generic;
using System.Xml;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class QtiItemScoringMethodHelper
    {
        public static Dictionary<string, string> QtiItemScoringMethods()
        {
            var dics = new Dictionary<string, string>();
            dics.Add("default", "Manual");
            dics.Add("manual", "Manual");
            dics.Add("ungraded", "Ungraded");
            dics.Add("informational-only", "Ungraded");
            dics.Add("rubricBasedGrading", "Rubric-Based");
            dics.Add("algorithmic", "Algorithmic");
            dics.Add("auto-scored", "Auto-Scored");
            return dics;
        }

        private static string GetScoringMethodBasic(XmlNode responseDeclaration, int qtiSchemaIdType)
        {
            var methodValue = responseDeclaration.Attributes["method"].Value;
            var isAutoGrading = qtiSchemaIdType == (int)QtiSchemaEnum.ChoiceSingle || qtiSchemaIdType == (int)QtiSchemaEnum.ChoiceMultiple
                        || qtiSchemaIdType == (int)QtiSchemaEnum.ChoiceMultipleVariable || qtiSchemaIdType == (int)QtiSchemaEnum.TextEntry ||
                        qtiSchemaIdType == (int)QtiSchemaEnum.InlineChoice;
            if (isAutoGrading && methodValue == "default")
            {
                return QtiItemScoringMethods()["auto-scored"];
            }
            return QtiItemScoringMethods()[methodValue];
        }

        private static string GetScoringMethodHotSpot(XmlNode responseDeclaration)
        {
            if (responseDeclaration != null && responseDeclaration.Attributes["absoluteGrading"] != null && responseDeclaration.Attributes["absoluteGrading"].Value == "1"
                       || responseDeclaration != null && responseDeclaration.Attributes["partialGrading"] != null && responseDeclaration.Attributes["partialGrading"].Value == "1")
            {
                return QtiItemScoringMethods()["auto-scored"];
            }
            else if (responseDeclaration != null && responseDeclaration.Attributes["algorithmicGrading"] != null && responseDeclaration.Attributes["algorithmicGrading"].Value == "1")
            {
                return QtiItemScoringMethods()["algorithmic"];
            }
            return QtiItemScoringMethods()["auto-scored"];
        }

        private static string GetScoringMethodDropDown(XmlNode responseDeclaration)
        {
            if (responseDeclaration != null && responseDeclaration.Attributes["absoluteGrading"] != null && responseDeclaration.Attributes["absoluteGrading"].Value == "1"
                       || (responseDeclaration != null && responseDeclaration.Attributes["relativeGrading"] != null && responseDeclaration.Attributes["relativeGrading"].Value == "1"))
            {
                return QtiItemScoringMethods()["auto-scored"];
            }
            else if (responseDeclaration != null && responseDeclaration.Attributes["algorithmicGrading"] != null && responseDeclaration.Attributes["algorithmicGrading"].Value == "1")
            {
                return QtiItemScoringMethods()["algorithmic"];
            }
            return QtiItemScoringMethods()["auto-scored"];
        }

        public static string GetQtiItemScoringMethod(string xmlContent, int qtiSchemaIdType, bool isRubricBasedQuestion)
        {
            try
            {
                if (isRubricBasedQuestion == true)
                {
                    return QtiItemScoringMethods()["rubricBasedGrading"];
                }
                if (qtiSchemaIdType == (int)QtiSchemaEnum.MultiPart)
                {
                    if (xmlContent.Contains("all-or-nothing-grading"))
                    {
                        return "Multi-part All or Nothing";
                    } else return "Varied";
                }

                XmlDocument _xDoc = new XmlDocument();
                _xDoc.PreserveWhitespace = true;
                _xDoc.LoadXml(xmlContent);
                var responseDeclaration = _xDoc.GetElementsByTagName("responseDeclaration")[0];

                if (responseDeclaration != null && responseDeclaration.Attributes["method"] != null)
                {
                    return GetScoringMethodBasic(responseDeclaration, qtiSchemaIdType);
                }
                else
                {
                    if (qtiSchemaIdType == (int)QtiSchemaEnum.TextHotSpot ||
                        qtiSchemaIdType == (int)QtiSchemaEnum.TableHotSpotSelection ||
                        qtiSchemaIdType == (int)QtiSchemaEnum.ImageHotSpotSelection ||
                        qtiSchemaIdType == (int)QtiSchemaEnum.NumberLine)
                    {
                        return GetScoringMethodHotSpot(responseDeclaration);
                    }
                    else if (qtiSchemaIdType == (int)QtiSchemaEnum.DragDrop ||
                        qtiSchemaIdType == (int)QtiSchemaEnum.DragDropNumerical ||
                        qtiSchemaIdType == (int)QtiSchemaEnum.SequenceOrder)
                    {
                        return GetScoringMethodDropDown(responseDeclaration);
                    }
                }

                return QtiItemScoringMethods()["auto-scored"];
            }
            catch
            {
                return QtiItemScoringMethods()["auto-scored"];
            }
        }
    }
}
