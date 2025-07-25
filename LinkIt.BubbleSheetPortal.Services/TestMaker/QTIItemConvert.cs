using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ResponseProcessingDTO;
using LinkIt.BubbleSheetPortal.Models.TestMaker;
using LinkIt.BubbleSheetPortal.Services.Grader;
using LinkIt.BubbleSheetPortal.Services.ResponseProcessingParser;

namespace LinkIt.BubbleSheetPortal.Services.TestMaker
{
    public class QTIItemConvert //: IQTIItemConvert
    {
        public QTIItemTestMaker ConvertFromXmlContent(string xmlContent)
        {
            var xmlContentSerializer = new XmlContentSerializer();
            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            xmlContent = xmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
            var assessmentItem = xmlContentSerializer.Deserialize(xmlContent);
            if (assessmentItem == null) return null;

            var qtiItem = new QTIItemTestMaker
            {
                QTISchemaID = ConvertValue.ToInt(assessmentItem.QtiSchemeID),
                XmlContent = xmlContent,
                AssessmentItem = assessmentItem,

                AnswerIdentifiers = "",
                CorrectAnswer = "",
                ResponseIdentifier = "",
                ResponseProcessing = "",
                Title = ""
            };

            Convert(qtiItem);           
            qtiItem.XmlContent = xmlContent.RecoverXmlSpecialChars(xmlSpecialCharToken);

            return qtiItem;
        }

        protected void Convert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem == null) return;
            switch (qtiItem.QTISchemaID)
            {
                case (int)QTISchemaEnum.TextEntry:
                    {
                        TextEntryConvert(qtiItem);
                        return;
                    }
                case (int)QTISchemaEnum.UploadComposite:
                    {
                        ComplexItemConvert(qtiItem);
                        return;
                    }
                case (int)QTISchemaEnum.InlineChoice:
                case (int)QTISchemaEnum.Choice:
                    {
                        ChoiceConvert(qtiItem);
                        return;
                    }
                case (int)QTISchemaEnum.ChoiceMultiple:
                    {
                        MultipleChoiceConvert(qtiItem);
                        return;
                    }
                case (int)QTISchemaEnum.ChoiceMultipleVariable:
                    {
                        MultipleChoiceVariableConvert(qtiItem);
                        return;
                    }
                case (int)QTISchemaEnum.ExtendedText:
                    {
                        ExtendedTextConvert(qtiItem);
                        return;
                    }
                case (int)QTISchemaEnum.DragAndDropPartialCredit:
                    {
                        DragAndDropPartialCreditConvert(qtiItem);
                        return;
                    }
                case (int)QTISchemaEnum.TextHotSpot:
                    {
                        TextHotSpotConvert(qtiItem);
                        return;
                    }

                case (int)QTISchemaEnum.ImageHotSpot:
                    {
                        ImageHotSpotConvert(qtiItem);
                        return;
                    }
                case (int)QTISchemaEnum.TableHotSpot:
                    {
                        TableHotSpotConvert(qtiItem);
                        return;
                    }
                case (int)QTISchemaEnum.NumberLineHotSpot:
                    {
                        NumberLineHotSpotConvert(qtiItem);
                        return;
                    }
                case (int)QTISchemaEnum.DragDropNumerical:
                    {
                        DragAndDropNumericalConvert(qtiItem);
                        return;
                    }
                case (int)QTISchemaEnum.DragDropSequence:
                    {
                        DragDropSequenceConvert(qtiItem);
                        return;
                    }
            }
        }

        //protected AssessmentItem ConvertToObject(string xmlContent)
        //{
        //    if (string.IsNullOrWhiteSpace(xmlContent)) return null;
        //    try
        //    {
        //        var deSerializer = new XmlSerializer(typeof(AssessmentItem), "http://www.imsglobal.org/xsd/imsqti_v2p0");
        //        AssessmentItem result;

        //        using (TextReader reader = new StringReader(xmlContent))
        //        {
        //            result = deSerializer.Deserialize(reader) as AssessmentItem;
        //        }

        //        if (result != null)
        //        {
        //            result.ResponseIdentifiers = GetResponseIdentifiers(xmlContent);
        //            return result;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }

        //    return null;
        //}

        //protected List<ResponseIdentifier> GetResponseIdentifiers(string xmlContent)
        //{
        //    var responseIdentifiers = new List<ResponseIdentifier>();
        //    if (string.IsNullOrWhiteSpace(xmlContent))
        //    {
        //        return responseIdentifiers;
        //    }

        //    var xml = new XmlDocument();
        //    xml.LoadXml(xmlContent);
        //    var tagNames = EnumUtils.GetDescriptions(typeof(QTISchemaEnum)).ToList();

        //    // Use distinct here to because choiceInteraction tag exists twice (multiple choice single select and multi select)
        //    foreach (var tagName in tagNames.Distinct())
        //    {
        //        var nodes = xml.GetElementsByTagName(tagName);
        //        foreach (XmlNode node in nodes)
        //        {
        //            var identifier = new ResponseIdentifier
        //                                 {
        //                                     Identifier = XmlUtils.GetNodeAttribute(node, "responseIdentifier"),
        //                                     maxChoices =
        //                                         XmlUtils.IsNodeAttributeExisted(node, "maxChoices")
        //                                             ? Int32.Parse(XmlUtils.GetNodeAttribute(node, "maxChoices"))
        //                                             : 0,
        //                                     TagName = tagName
        //                                 };

        //            responseIdentifiers.Add(identifier);
        //        }
        //    }

        //    return responseIdentifiers;
        //}

        protected string GetResponseProcessingXml(ResponseDeclaration responseDeclaration, int qtiSchemaId)
        {
            try
            {
                if (qtiSchemaId == (int)QTISchemaEnum.ChoiceMultiple)
                {
                    if (responseDeclaration == null) return string.Empty;
                    var result = responseDeclaration.ResponseDeclarationXml.ReplaceInsensitive("ResponseDeclaration", "process");
                    if (!result.Contains("allCorrect=")) result = result.Replace("<process", "<process allCorrect=\"true\" ");
                    return result;
                }
                else if (qtiSchemaId == (int)QTISchemaEnum.ChoiceMultipleVariable)
                {
                    if (responseDeclaration == null) return string.Empty;
                    var result = XmlUtils.Serialize(responseDeclaration.CorrectResponses[0]);
                    result = "<process method=\"special\" allCorrect=\"true\">" + result + "</process>";
                    return result;
                }
                else if (qtiSchemaId == (int)QTISchemaEnum.TextEntry)
                {
                    if (responseDeclaration == null) return string.Empty;
                    var result = responseDeclaration.ResponseDeclarationXml;
                    result = result.ReplaceInsensitive("ResponseDeclaration", "process");
                    return result;
                }
                else if (qtiSchemaId == (int)QTISchemaEnum.DragAndDropPartialCredit || qtiSchemaId == (int)QTISchemaEnum.DragDropNumerical)
                {
                    if (responseDeclaration == null) return string.Empty;
                    var result = responseDeclaration.ResponseDeclarationXml;
                    result = result.ReplaceInsensitive("ResponseDeclaration", "process");
                    return result;
                }
                else if (qtiSchemaId == (int)QTISchemaEnum.TextHotSpot)
                {
                    if (responseDeclaration == null) return string.Empty;
                    var result = responseDeclaration.ResponseDeclarationXml;
                    result = result.ReplaceInsensitive("ResponseDeclaration", "process");
                    return result;
                }
                else if (qtiSchemaId == (int)QTISchemaEnum.ImageHotSpot)
                {
                    if (responseDeclaration == null) return string.Empty;
                    var result = responseDeclaration.ResponseDeclarationXml;
                    result = result.ReplaceInsensitive("ResponseDeclaration", "process");
                    return result;
                }
                else if (qtiSchemaId == (int)QTISchemaEnum.NumberLineHotSpot)
                {
                    if (responseDeclaration == null) return string.Empty;
                    var result = responseDeclaration.ResponseDeclarationXml;
                    result = result.ReplaceInsensitive("ResponseDeclaration", "process");
                    return result;
                }
                else if (qtiSchemaId == (int)QTISchemaEnum.TableHotSpot)
                {
                    if (responseDeclaration == null) return string.Empty;
                    var result = responseDeclaration.ResponseDeclarationXml;
                    result = result.ReplaceInsensitive("ResponseDeclaration", "process");
                    return result;
                }
                else if (qtiSchemaId == (int)QTISchemaEnum.DragDropSequence)
                {
                    if (responseDeclaration == null) return string.Empty;
                    var result = responseDeclaration.ResponseDeclarationXml;
                    result = result.ReplaceInsensitive("ResponseDeclaration", "process");
                    return result;
                }
                else
                {
                    if (responseDeclaration == null) return string.Empty;
                    var result = responseDeclaration.ResponseDeclarationXml;
                    return result;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        protected ResponseProcessing TransformFrom(ResponseDeclaration responseDeclaration)
        {
            if (responseDeclaration == null) return null;
            var result = new ResponseProcessing
            {
                Method = responseDeclaration.Method,
                CaseSensitive = responseDeclaration.CaseSensitive,
                Spelling = responseDeclaration.Spelling,
                SpellingDeduction = responseDeclaration.SpellingDeduction,
                Type = responseDeclaration.Type
            };

            if (responseDeclaration.CorrectResponses == null || responseDeclaration.CorrectResponses.Count == 0 || responseDeclaration.CorrectResponses[0].Values == null) return result;
            result.CorrectResponse = new CorrectResponse();
            result.CorrectResponse.Values = new List<string>();
            foreach (var value in responseDeclaration.CorrectResponses[0].Values)
            {
                result.CorrectResponse.Values.Add(value);
            }

            return result;
        }

        protected int GetQtiSchemaID(string tagName)
        {
            var result = (int)EnumUtils.FromDescription<QTISchemaEnum>(tagName);
            return result;
        }

        protected List<Identifier> GetIdentifiers(string xmlContent, int qtiSchemaId)
        {
            var identifiers = new List<Identifier>();
            if (string.IsNullOrWhiteSpace(xmlContent))
            {
                return identifiers;
            }

            var xml = new XmlDocument();
            xml.LoadXml(xmlContent);
            var tagName = GetTagNameBySchema(qtiSchemaId);

            if (tagName != null)
            {
                var nodes = xml.GetElementsByTagName(tagName);

                foreach (XmlNode node in nodes)
                {
                    var identifier = new Identifier
                    {
                        Identifier = XmlUtils.GetNodeAttribute(node, "identifier"),
                        TagName = tagName
                    };

                    identifiers.Add(identifier);
                }
            }

            return identifiers;
        }

        private string GetTagNameBySchema(int qtiSchemaId)
        {
            switch (qtiSchemaId)
            {
                case 1:
                case 3:
                case 37:
                    return "simpleChoice";

                case 8:
                    return "inlineChoice";

                default:
                    return null;
            }
        }

        protected void TextEntryConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem == null || qtiItem.AssessmentItem == null) return;
            var responseIdentifiers = qtiItem.AssessmentItem.ResponseIdentifiers;
            var textEntryInteraction = EnumUtils.GetDescription(QTISchemaEnum.TextEntry);
            if (responseIdentifiers == null) return;

            var textEntryIdentifier = responseIdentifiers.FirstOrDefault(o => o.TagName == textEntryInteraction);
            if (textEntryIdentifier == null) return;

            var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
            if (responseDeclarations == null || responseDeclarations.Count == 0) return;
            var responseDeclaration = responseDeclarations.FirstOrDefault(o => o.Identifier == textEntryIdentifier.Identifier);

            string responseDeclarationMethod = null;
            if (responseDeclaration != null) responseDeclarationMethod = responseDeclaration.Method;
            qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, responseDeclarationMethod);
            qtiItem.ResponseIdentifier = responseDeclaration.Identifier;

            if (responseDeclaration == null || responseDeclaration.CorrectResponses == null || responseDeclaration.CorrectResponses.Count == 0) return;

            var values = responseDeclaration.CorrectResponses[0].Values;
            if (values == null || values.Count == 0) return;
            qtiItem.CorrectAnswer = values[0];
            qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);
            qtiItem.ResponseProcessing = GetResponseProcessingXml(responseDeclaration, qtiItem.QTISchemaID);

            foreach (var value in values)
            {
                var qtiItemAnswerScore = new QTIItemAnswerScoreTestMaker
                {
                    Answer = value,
                    Score = ConvertValue.ToInt(responseDeclaration.PointsValue),
                    ResponseIdentifier = responseDeclaration.Identifier
                };

                qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
            }
        }

        protected void ExtendedTextConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem == null || qtiItem.AssessmentItem == null) return;
            var responseIdentifiers = qtiItem.AssessmentItem.ResponseIdentifiers;
            var textEntryInteraction = EnumUtils.GetDescription(QTISchemaEnum.ExtendedText);
            if (responseIdentifiers == null) return;

            var textEntryIdentifier = responseIdentifiers.FirstOrDefault(o => o.TagName == textEntryInteraction);
            if (textEntryIdentifier == null) return;

            var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
            if (responseDeclarations == null || responseDeclarations.Count == 0) return;
            var responseDeclaration =
                responseDeclarations.FirstOrDefault(o => o.Identifier == textEntryIdentifier.Identifier);
            if (responseDeclaration == null) return;

            qtiItem.CorrectAnswer = "O";
            qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);
            qtiItem.ResponseIdentifier = responseDeclaration.Identifier;
            qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, responseDeclaration.Method);
            qtiItem.ResponseProcessing = "<process  method=\"default\" />";
            var qtiItemAnswerScore = new QTIItemAnswerScoreTestMaker
            {
                Answer = "O",
                Score = ConvertValue.ToInt(responseDeclaration.PointsValue),
                ResponseIdentifier = responseDeclaration.Identifier
            };

            qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
        }

        protected void ChoiceConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem == null || qtiItem.AssessmentItem == null) return;
            var responseIdentifiers = qtiItem.AssessmentItem.ResponseIdentifiers;
            var choiceInteraction = EnumUtils.GetDescription(QTISchemaEnum.Choice);
            var inlineChoiceInteraction = EnumUtils.GetDescription(QTISchemaEnum.InlineChoice);
            if (responseIdentifiers == null) return;

            var identifier = responseIdentifiers.FirstOrDefault(o => o.TagName == choiceInteraction || o.TagName == inlineChoiceInteraction);
            if (identifier == null) return;

            var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
            if (responseDeclarations == null || responseDeclarations.Count == 0) return;
            var responseDeclaration = responseDeclarations.FirstOrDefault(o => o.Identifier == identifier.Identifier);

            string responseDeclarationMethod = null;
            if (responseDeclaration != null) responseDeclarationMethod = responseDeclaration.Method;
            qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, responseDeclarationMethod);
            qtiItem.ResponseIdentifier = responseDeclaration.Identifier;
            qtiItem.ResponseProcessing = "<process  method=\"default\" />";

            if (responseDeclaration == null || responseDeclaration.CorrectResponses == null || responseDeclaration.CorrectResponses.Count == 0) return;

            var values = responseDeclaration.CorrectResponses[0].Values;
            if (values == null || values.Count == 0) return;
            qtiItem.CorrectAnswer = string.Join(";", values.Select(item => item));
            qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);
            var identifiers = GetIdentifiers(qtiItem.XmlContent, qtiItem.QTISchemaID);
            if (identifiers.Any()) qtiItem.AnswerIdentifiers = string.Join(";", identifiers.Select(o => o.Identifier));
            foreach (var value in values)
            {
                var qtiItemAnswerScore =
                    new QTIItemAnswerScoreTestMaker
                    {
                        Answer = value,
                        Score = ConvertValue.ToInt(responseDeclaration.PointsValue),
                        ResponseIdentifier = responseDeclaration.Identifier
                    };

                qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
            }
        }

        protected void MultipleChoiceConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem == null || qtiItem.AssessmentItem == null) return;
            var responseIdentifiers = qtiItem.AssessmentItem.ResponseIdentifiers;
            var choiceInteraction = EnumUtils.GetDescription(QTISchemaEnum.Choice);
            var inlineChoiceInteraction = EnumUtils.GetDescription(QTISchemaEnum.InlineChoice);
            if (responseIdentifiers == null) return;

            var identifier =
                responseIdentifiers.FirstOrDefault(
                    o => o.TagName == choiceInteraction || o.TagName == inlineChoiceInteraction);
            if (identifier == null) return;

            var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
            if (responseDeclarations == null || responseDeclarations.Count == 0) return;
            var responseDeclaration = responseDeclarations.FirstOrDefault(o => o.Identifier == identifier.Identifier);

            string responseDeclarationMethod = null;
            if (responseDeclaration != null) responseDeclarationMethod = responseDeclaration.Method;
            qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, responseDeclarationMethod);
            qtiItem.ResponseIdentifier = responseDeclaration.Identifier;

            if (responseDeclaration == null || responseDeclaration.CorrectResponses == null || responseDeclaration.CorrectResponses.Count == 0) return;

            var values = responseDeclaration.CorrectResponses[0].Values;
            if (values == null || values.Count == 0) return;
            qtiItem.CorrectAnswer = string.Join(";", values.Select(item => item));
            qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);

            qtiItem.ResponseProcessing = GetResponseProcessingXml(responseDeclaration, qtiItem.QTISchemaID);
            var identifiers = GetIdentifiers(qtiItem.XmlContent, qtiItem.QTISchemaID);
            if (identifiers.Any()) qtiItem.AnswerIdentifiers = string.Join(";", identifiers.Select(o => o.Identifier));
            foreach (var value in values)
            {
                var qtiItemAnswerScore =
                    new QTIItemAnswerScoreTestMaker
                    {
                        Answer = value,
                        Score = 0,
                        ResponseIdentifier = responseDeclaration.Identifier
                    };

                qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
            }
        }

        protected void MultipleChoiceVariableConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem == null || qtiItem.AssessmentItem == null) return;
            var responseIdentifiers = qtiItem.AssessmentItem.ResponseIdentifiers;
            var choiceInteraction = EnumUtils.GetDescription(QTISchemaEnum.Choice);
            var inlineChoiceInteraction = EnumUtils.GetDescription(QTISchemaEnum.InlineChoice);
            if (responseIdentifiers == null) return;

            var identifier =
                responseIdentifiers.FirstOrDefault(
                    o => o.TagName == choiceInteraction || o.TagName == inlineChoiceInteraction);
            if (identifier == null) return;

            var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
            if (responseDeclarations == null || responseDeclarations.Count == 0) return;
            var responseDeclaration = responseDeclarations.FirstOrDefault(o => o.Identifier == identifier.Identifier);

            string responseDeclarationMethod = null;
            if (responseDeclaration != null) responseDeclarationMethod = responseDeclaration.Method;
            qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, responseDeclarationMethod);
            qtiItem.ResponseIdentifier = responseDeclaration.Identifier;

            if (responseDeclaration == null || responseDeclaration.CorrectResponses == null || responseDeclaration.CorrectResponses.Count == 0) return;

            var values = responseDeclaration.CorrectResponses[0].ValuesXML;
            if (values == null || values.Count == 0) return;
            var answerTexts = responseDeclaration.CorrectResponses[0].AnswerTexts;
            qtiItem.CorrectAnswer = string.Empty; //string.Join(";", values.Select(item => item)); no use CorrectAnswer
            qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);

            qtiItem.ResponseProcessing = GetResponseProcessingXml(responseDeclaration, qtiItem.QTISchemaID);
            var identifiers = GetIdentifiers(qtiItem.XmlContent, qtiItem.QTISchemaID);
            if (identifiers.Any()) qtiItem.AnswerIdentifiers = string.Join(";", identifiers.Select(o => o.Identifier));
            var index = 0;
            foreach (var value in values)
            {
                var valDoc = new XmlDocument();
                valDoc.LoadXml(string.Format("<root>{0}</root>", value));
                var valueNodes = valDoc.GetElementsByTagName("value");
                if (valueNodes.Count == 0) continue;
                var valueNode = valueNodes[0];
                if (valueNode == null || valueNode.Attributes == null) continue;
                var answer = valueNode.Attributes["identifier"];
                if (answer == null) continue;
                var qtiItemAnswerScore =
                    new QTIItemAnswerScoreTestMaker
                    {
                        Answer = answer.Value,
                        Score = int.Parse(valDoc.InnerText),
                        ResponseIdentifier = responseDeclaration.Identifier,
                        AnswerText = answerTexts?[index]
                    };

                qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
                index++;
            }
        }

        protected void ComplexItemConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem == null || qtiItem.AssessmentItem == null) return;
            var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
            if (responseDeclarations == null || responseDeclarations.Count == 0) return;
            var responseIdentifiers = qtiItem.AssessmentItem.ResponseIdentifiers;
            if (responseIdentifiers == null || responseIdentifiers.Count == 0) return;
            qtiItem.CorrectAnswer = string.Empty;
            qtiItem.ResponseIdentifier = "multi";
            qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, null);
            qtiItem.ResponseProcessing = "<process  method=\"default\" />";
            qtiItem.PointsPossible = 0;

            foreach (var responseIdentifier in responseIdentifiers)
            {
                var responseDeclaration =
                responseDeclarations.FirstOrDefault(o => o.Identifier == responseIdentifier.Identifier);
                if (responseDeclaration == null
                    //|| responseDeclaration.CorrectResponse == null
                    ) continue;

                var qtiItemSubTestMaker = new QTIItemSubTestMaker();
                if (responseIdentifier.TagName == "choiceInteraction")
                {
                    if (!string.IsNullOrEmpty(responseIdentifier.variablePoints) &&
                        responseIdentifier.variablePoints.ToLower().Equals("true"))
                    {
                        qtiItemSubTestMaker.QTISchemaID = (int)QTISchemaEnum.ChoiceMultipleVariable;
                    }
                    else
                    {
                        // Detect Multiple choice Single select question or Multiple select question based on maxChoices attribute
                        qtiItemSubTestMaker.QTISchemaID = responseIdentifier.maxChoices == 1 ? 1 : 3;
                    }
                }
                else
                {
                    qtiItemSubTestMaker.QTISchemaID = GetQtiSchemaID(responseIdentifier.TagName);
                }

                qtiItemSubTestMaker.ResponseIdentifier = responseIdentifier.Identifier;
                qtiItemSubTestMaker.ResponseProcessing = GetResponseProcessingXml(responseDeclaration,
                                                                                  qtiItemSubTestMaker.QTISchemaID);
                qtiItemSubTestMaker.ResponseProcessingTypeID =
                    GetResponseProcessingTypeID(qtiItemSubTestMaker.QTISchemaID, responseDeclaration.Method);
                qtiItemSubTestMaker.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);

                qtiItemSubTestMaker.CorrectAnswer = "";
                qtiItemSubTestMaker.Depending = responseDeclaration.Depending;
                if (!string.IsNullOrEmpty(responseDeclaration.Major))
                {
                    qtiItemSubTestMaker.Major = bool.Parse(responseDeclaration.Major);
                }
                // Add O Correct Answer for Extended text question
                if (qtiItemSubTestMaker.QTISchemaID == 10)
                {
                    responseDeclaration.CorrectResponses = new List<CorrectResponse>(new CorrectResponse[]{ new CorrectResponse{
                                                                                                                                    Values = new List<string> {"O"}
                                                                                                                                }});
                }

                if (qtiItemSubTestMaker.QTISchemaID != (int)QTISchemaEnum.DragAndDropPartialCredit && responseDeclaration.CorrectResponses != null && responseDeclaration.CorrectResponses.Count > 0 && responseDeclaration.CorrectResponses[0].Values != null && responseDeclaration.CorrectResponses[0].Values.Count > 0)
                {
                    if (qtiItemSubTestMaker.QTISchemaID == 9)
                    {
                        // If Extended text ==> get 1st correct answer
                        qtiItemSubTestMaker.CorrectAnswer = responseDeclaration.CorrectResponses[0].Values.FirstOrDefault();
                    }
                    else
                    {
                        if (qtiItemSubTestMaker.QTISchemaID != (int)QTISchemaEnum.ChoiceMultipleVariable)//ChoiceMultipleVariable does not use CorrectAnswer
                        {
                            qtiItemSubTestMaker.CorrectAnswer = string.Join(";",
                                responseDeclaration.CorrectResponses[0].Values.Select(item => item));
                        }
                    }

                    foreach (var value in responseDeclaration.CorrectResponses[0].Values)
                    {
                        if (qtiItemSubTestMaker.QTISchemaID == (int)QTISchemaEnum.ChoiceMultipleVariable)
                        {
                            var vals = value.Split(':');
                            var qtiItemAnswerScore =
                                new QTIItemAnswerScoreTestMaker
                                {
                                    Answer = vals[0],
                                    Score = int.Parse(vals[1]),
                                    ResponseIdentifier = responseDeclaration.Identifier
                                };
                            qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
                        }
                        else
                        {
                            var qtiItemAnswerScore =
                           new QTIItemAnswerScoreTestMaker
                           {
                               Answer = value,
                               Score =
                                   qtiItemSubTestMaker.QTISchemaID == 3
                                       ? 0
                                       : ConvertValue.ToInt(responseDeclaration.PointsValue),
                               ResponseIdentifier = responseDeclaration.Identifier
                           };

                            qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
                        }
                    }
                }
                //New QTISchemaID = 30 ( Drag and Drop Partial Credit )
                if (qtiItemSubTestMaker.QTISchemaID == (int)QTISchemaEnum.DragAndDropPartialCredit)
                {
                    string correctAnswer = string.Empty;
                    //Just save as DEST_1-SRC_1,DEST_2-SRC_2,DEST_3-SRC_3,DEST_4-SRC_4 to column QtiItem.CorrectAnswer
                    if (responseDeclaration.CorrectResponses != null)
                    {
                        foreach (var correctResponse in responseDeclaration.CorrectResponses)
                        {
                            correctAnswer += string.Format("{0}-{1},", correctResponse.DestIdentifier,
                                                           correctResponse.SrcIdentifier);
                        }
                        if (correctAnswer.Length > 0)
                        {
                            //remove the last ';'
                            correctAnswer = correctAnswer.Remove(correctAnswer.Length - 1, 1);
                        }
                    }
                    qtiItemSubTestMaker.CorrectAnswer = correctAnswer;

                    var qtiItemAnswerScore =
                          new QTIItemAnswerScoreTestMaker
                          {
                              Answer = correctAnswer,
                              Score =
                                  qtiItemSubTestMaker.QTISchemaID == 3
                                      ? 0
                                      : ConvertValue.ToInt(responseDeclaration.PointsValue),
                              ResponseIdentifier = responseDeclaration.Identifier
                          };

                    qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
                }

                qtiItem.QTIItemSubTestMakers.Add(qtiItemSubTestMaker);

                // Calculate PointsPossible of QtiItem
                qtiItem.PointsPossible += qtiItemSubTestMaker.PointsPossible;               

            }
            var allNoThingGading = responseDeclarations.FirstOrDefault(x => x.AllOrNothingGrading != null)?.AllOrNothingGrading;
            if (!string.IsNullOrEmpty(allNoThingGading))
            {
                qtiItem.PointsPossible = ConvertValue.ToInt(allNoThingGading);
            }
        }

        // responseProcessingMethod: Auto grade: 1; manual grade: 2
        private int GetResponseProcessingTypeID(int qtiSchemaId, string responseProcessingMethod)
        {
            //TODO: [LNKT-30407]
            if (!string.IsNullOrEmpty(responseProcessingMethod)
                && responseProcessingMethod.Equals("algorithmic", StringComparison.OrdinalIgnoreCase))
            {
                return 7;
                //(algorithmicPoints)
                //Points calculated based on the result of an algorithmic expression.
            }
            switch (qtiSchemaId)
            {
                case (int)QTISchemaEnum.Choice:
                case (int)QTISchemaEnum.InlineChoice:
                case (int)QTISchemaEnum.UploadComposite:
                case (int)QTISchemaEnum.DragAndDropPartialCredit:
                case (int)QTISchemaEnum.TextHotSpot:
                case (int)QTISchemaEnum.ImageHotSpot:
                case (int)QTISchemaEnum.TableHotSpot:
                case (int)QTISchemaEnum.NumberLineHotSpot:

                case (int)QTISchemaEnum.ExtendedText:
                    return responseProcessingMethod == "ungraded" ? 3 : 1;

                case (int)QTISchemaEnum.ChoiceMultiple:
                    return 3;

                case (int)QTISchemaEnum.ChoiceMultipleVariable:
                    return 3;

                case (int)QTISchemaEnum.TextEntry:
                    switch (responseProcessingMethod)
                    {
                        case "manual":
                            return 2;

                        case "ungraded":
                            return 3;

                        default:
                            return 1;
                    }
                default:
                    return 1;
            }
        }

        protected void DragAndDropPartialCreditConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem == null || qtiItem.AssessmentItem == null) return;

            //There's only one responseDeclaration under assessmentItem
            var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
            if (responseDeclarations == null || responseDeclarations.Count == 0) return;
            var responseDeclaration = responseDeclarations[0];
            string correctAnswer = string.Empty;
            //Just save as DEST_1-SRC_1,DEST_2-SRC_2,DEST_3-SRC_3,DEST_4-SRC_4 to column QtiItem.CorrectAnswer
            foreach (var correctResponse in responseDeclaration.CorrectResponses)
            {
                correctAnswer += string.Format("{0}-{1},", correctResponse.DestIdentifier, correctResponse.SrcIdentifier);
            }
            if (correctAnswer.Length > 0)
            {
                //remove the last ';'
                correctAnswer = correctAnswer.Remove(correctAnswer.Length - 1, 1);
            }
            qtiItem.CorrectAnswer = correctAnswer;
            qtiItem.AnswerIdentifiers = string.Empty;
            qtiItem.ResponseIdentifier = responseDeclaration.Identifier;

            qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);
            qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, null);
            if (responseDeclaration.AlgorithmicGrading == "1")
            {
                qtiItem.ResponseProcessingTypeID = 7;// algorithmicGrading
            }
            //ProcessingTypeID must be 1
            qtiItem.ResponseProcessing = GetResponseProcessingXml(responseDeclaration, qtiItem.QTISchemaID);
        }

        protected void TableHotSpotConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem.QTISchemaID == (int)QTISchemaEnum.TableHotSpot)
            {
                if (qtiItem == null || qtiItem.AssessmentItem == null) return;

                //There's only one responseDeclaration under assessmentItem
                var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
                if (responseDeclarations == null || responseDeclarations.Count == 0) return;
                var responseDeclaration = responseDeclarations[0];
                string correctAnswer = string.Empty;
                //Just save as THS_1,THS_4 to column QtiItem.CorrectAnswer
                foreach (var correctResponse in responseDeclaration.CorrectResponses)
                {
                    correctAnswer += string.Format("{0},", correctResponse.Identifier);
                    var qtiItemAnswerScore = new QTIItemAnswerScoreTestMaker
                    {
                        Answer = correctResponse.Identifier,
                        Score = correctResponse.PointValue,
                        ResponseIdentifier = responseDeclaration.Identifier
                    };

                    qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
                }
                if (correctAnswer.Length > 0)
                {
                    //remove the last ';'
                    correctAnswer = correctAnswer.Remove(correctAnswer.Length - 1, 1);
                }
                qtiItem.CorrectAnswer = correctAnswer;
                qtiItem.AnswerIdentifiers = string.Empty;
                qtiItem.ResponseIdentifier = responseDeclaration.Identifier;

                qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);
                qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, null);//ProcessingTypeID must be 1
                if (responseDeclaration.AlgorithmicGrading == "1")
                {
                    qtiItem.ResponseProcessingTypeID = 7;// algorithmicGrading
                }
                qtiItem.ResponseProcessing = GetResponseProcessingXml(responseDeclaration, qtiItem.QTISchemaID);
            }
        }

        protected void NumberLineHotSpotConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem.QTISchemaID == (int)QTISchemaEnum.NumberLineHotSpot)
            {
                if (qtiItem == null || qtiItem.AssessmentItem == null) return;

                //There's only one responseDeclaration under assessmentItem
                var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
                if (responseDeclarations == null || responseDeclarations.Count == 0) return;
                var responseDeclaration = responseDeclarations[0];
                string correctAnswer = string.Empty;
                //Just save as THS_1,THS_4 to column QtiItem.CorrectAnswer
                foreach (var correctResponse in responseDeclaration.CorrectResponses)
                {
                    correctAnswer += string.Format("{0},", correctResponse.Identifier);
                    var qtiItemAnswerScore = new QTIItemAnswerScoreTestMaker
                    {
                        Answer = correctResponse.Identifier,
                        Score = correctResponse.PointValue,
                        ResponseIdentifier = responseDeclaration.Identifier
                    };

                    qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
                }
                if (correctAnswer.Length > 0)
                {
                    //remove the last ';'
                    correctAnswer = correctAnswer.Remove(correctAnswer.Length - 1, 1);
                }
                qtiItem.CorrectAnswer = correctAnswer;
                qtiItem.AnswerIdentifiers = string.Empty;
                qtiItem.ResponseIdentifier = responseDeclaration.Identifier;

                qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);
                qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, null);//ProcessingTypeID must be 1
                if (responseDeclaration.AlgorithmicGrading == "1")
                {
                    qtiItem.ResponseProcessingTypeID = 7;// algorithmicGrading
                }
                qtiItem.ResponseProcessing = GetResponseProcessingXml(responseDeclaration, qtiItem.QTISchemaID);
            }
        }

        protected void ImageHotSpotConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem.QTISchemaID == (int)QTISchemaEnum.ImageHotSpot)
            {
                if (qtiItem == null || qtiItem.AssessmentItem == null) return;

                //There's only one responseDeclaration under assessmentItem
                var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
                if (responseDeclarations == null || responseDeclarations.Count == 0) return;
                var responseDeclaration = responseDeclarations[0];
                string correctAnswer = string.Empty;
                //Just save as DEST_1-SRC_1,DEST_2-SRC_2,DEST_3-SRC_3,DEST_4-SRC_4 to column QtiItem.CorrectAnswer
                foreach (var correctResponse in responseDeclaration.CorrectResponses)
                {
                    correctAnswer += string.Format("{0},", correctResponse.Identifier);
                    var qtiItemAnswerScore = new QTIItemAnswerScoreTestMaker
                    {
                        Answer = correctResponse.Identifier,
                        Score = correctResponse.PointValue,
                        ResponseIdentifier = responseDeclaration.Identifier
                    };

                    qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
                }
                if (correctAnswer.Length > 0)
                {
                    //remove the last ';'
                    correctAnswer = correctAnswer.Remove(correctAnswer.Length - 1, 1);
                }
                qtiItem.CorrectAnswer = correctAnswer;
                qtiItem.AnswerIdentifiers = string.Empty;
                qtiItem.ResponseIdentifier = responseDeclaration.Identifier;

                qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);
                qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, null);//ProcessingTypeID must be 1
                if (responseDeclaration.AlgorithmicGrading == "1")
                {
                    qtiItem.ResponseProcessingTypeID = 7;// algorithmicGrading
                }
                qtiItem.ResponseProcessing = GetResponseProcessingXml(responseDeclaration, qtiItem.QTISchemaID);
            }
        }

        protected void TextHotSpotConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem.QTISchemaID == (int)QTISchemaEnum.TextHotSpot)
            {
                if (qtiItem == null || qtiItem.AssessmentItem == null) return;

                //There's only one responseDeclaration under assessmentItem
                var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
                if (responseDeclarations == null || responseDeclarations.Count == 0) return;
                var responseDeclaration = responseDeclarations[0];
                string correctAnswer = string.Empty;
                //Just save as DEST_1-SRC_1,DEST_2-SRC_2,DEST_3-SRC_3,DEST_4-SRC_4 to column QtiItem.CorrectAnswer
                foreach (var correctResponse in responseDeclaration.CorrectResponses)
                {
                    correctAnswer += string.Format("{0},", correctResponse.Identifier);
                    var qtiItemAnswerScore = new QTIItemAnswerScoreTestMaker
                    {
                        Answer = correctResponse.Identifier,
                        Score = correctResponse.PointValue,
                        ResponseIdentifier = responseDeclaration.Identifier
                    };

                    qtiItem.QTIITemAnswerScoreTestMakers.Add(qtiItemAnswerScore);
                }
                if (correctAnswer.Length > 0)
                {
                    //remove the last ';'
                    correctAnswer = correctAnswer.Remove(correctAnswer.Length - 1, 1);
                }
                qtiItem.CorrectAnswer = correctAnswer;
                qtiItem.AnswerIdentifiers = string.Empty;
                qtiItem.ResponseIdentifier = responseDeclaration.Identifier;

                qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);
                qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, null);//ProcessingTypeID must be 1
                if (responseDeclaration.AlgorithmicGrading == "1")
                {
                    qtiItem.ResponseProcessingTypeID = 7;// algorithmicGrading
                }
                qtiItem.ResponseProcessing = GetResponseProcessingXml(responseDeclaration, qtiItem.QTISchemaID);
            }
        }

        protected void DragDropSequenceConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem.QTISchemaID == (int)QTISchemaEnum.DragDropSequence)
            {
                if (qtiItem == null || qtiItem.AssessmentItem == null) return;

                //There's only one responseDeclaration under assessmentItem
                var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
                if (responseDeclarations == null || responseDeclarations.Count == 0) return;
                var responseDeclaration = responseDeclarations[0];
                string correctAnswer = string.Empty;
                //Just save as SRC_2,SRC_3,SRC_1,SRC_4 to column QtiItem.CorrectAnswer
                if (responseDeclaration.CorrectResponses != null && responseDeclaration.CorrectResponses.Count == 1)
                {
                    if (responseDeclaration.CorrectResponses[0].Values.Count == 1
                        && responseDeclaration.CorrectResponses[0].Values.Count == 1)
                        correctAnswer = responseDeclaration.CorrectResponses[0].Values[0];
                }

                qtiItem.CorrectAnswer = correctAnswer;
                qtiItem.AnswerIdentifiers = string.Empty;
                qtiItem.ResponseIdentifier = responseDeclaration.Identifier;

                qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);
                qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, null);//ProcessingTypeID must be 1
                if (responseDeclaration.AlgorithmicGrading == "1")
                {
                    qtiItem.ResponseProcessingTypeID = 7;// algorithmicGrading
                }
                qtiItem.ResponseProcessing = GetResponseProcessingXml(responseDeclaration, qtiItem.QTISchemaID);
            }
        }

        protected void DragAndDropNumericalConvert(QTIItemTestMaker qtiItem)
        {
            if (qtiItem == null || qtiItem.AssessmentItem == null) return;

            //There's only one responseDeclaration under assessmentItem
            var responseDeclarations = qtiItem.AssessmentItem.ResponseDeclarations;
            if (responseDeclarations == null || responseDeclarations.Count == 0) return;
            var responseDeclaration = responseDeclarations[0];

            qtiItem.AnswerIdentifiers = string.Empty;
            qtiItem.ResponseIdentifier = responseDeclaration.Identifier;

            qtiItem.PointsPossible = ConvertValue.ToInt(responseDeclaration.PointsValue);
            qtiItem.ResponseProcessingTypeID = GetResponseProcessingTypeID(qtiItem.QTISchemaID, null);
            if (responseDeclaration.AlgorithmicGrading == "1")
            {
                qtiItem.ResponseProcessingTypeID = 7;// algorithmicGrading
            }
            //ProcessingTypeID must be 1
            qtiItem.ResponseProcessing = GetResponseProcessingXml(responseDeclaration, qtiItem.QTISchemaID);
            qtiItem.CorrectAnswer = GetCorrectAnswerOfDragDropNumerical(qtiItem.ResponseProcessing, qtiItem.QTISchemaID);
        }

        private string GetCorrectAnswerOfDragDropNumerical(string responseProcessing, int qtiSchemaID)
        {
            if (string.IsNullOrWhiteSpace(responseProcessing)) return string.Empty;

            var responseProcessingParser = GetResponseProcessingParserFactory()
                        .GetResponseProcessingParser(qtiSchemaID);
            if (responseProcessingParser == null) return string.Empty;
            var responseProcessingDTO = responseProcessingParser.Parse(responseProcessing) as DragDropNumericalRPDTO;
            if (responseProcessingDTO == null) return string.Empty;
            var grader = new DragDropNumericalGrader();
            var correctMappings = grader.GetCorrectMappings(responseProcessingDTO);
            if (correctMappings == null) return string.Empty;
            var result = string.Join("<br>", correctMappings);

            return result;
        }

        public IResponseProcessingParserFactory GetResponseProcessingParserFactory()
        {
            IResponseProcessingParser dragDropNumericalRPParser = new DragDropNumericalRPParser();
            var parserFactory = new ResponseProcessingParserFactory(dragDropNumericalRPParser);
            return parserFactory;
        }

        public bool IsTrueFalseQuestion(string xmlContent, string responseIdentifier)
        {
            if (string.IsNullOrWhiteSpace(xmlContent)) return false;

            var xdoc = new XmlDocument();
            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            xmlContent = xmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
            xdoc.LoadXml(xmlContent);

            var nodes = xdoc.GetElementsByTagName("choiceInteraction");
            foreach (XmlNode node in nodes)
            {
                var identifier = XmlUtils.GetNodeAttributeCaseInSensitive(node, "responseIdentifier");
                if (!string.IsNullOrEmpty(identifier) && identifier == responseIdentifier)
                {
                    var subType = XmlUtils.GetNodeAttributeCaseInSensitive(node, "subtype");
                    if (!string.IsNullOrEmpty(subType) && subType.ToLower() == "truefalse")
                        return true;
                }
            }

            return false;
        }
    }
}
