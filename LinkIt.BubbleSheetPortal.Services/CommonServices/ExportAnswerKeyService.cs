using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ResponseProcessingDTO;
using LinkIt.BubbleSheetPortal.Services.TestMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ExportAnswerKeyService
    {
        public readonly QTIItemConvert _qTIItemConvert;
        public ExportAnswerKeyService(QTIItemConvert qTIItemConvert)
        {
            _qTIItemConvert = qTIItemConvert;
        }

        Dictionary<string, string> replacements = new Dictionary<string, string>
            {
                {"&amp;","&"},
                {"&lt;","<"},
                {"&gt;",">"}
            };

        public string GetAnswerKey(AnswerKeyData item)
        {
            var scoringType = QtiItemScoringMethodHelper.GetQtiItemScoringMethod(item.XmlContent, item.QTISchemaID, item.IsRubricBasedQuestion.HasValue ? item.IsRubricBasedQuestion.Value : false);
            switch (item.QTISchemaID)
            {
                case (int)QtiSchemaEnum.MultipleChoice:
                case (int)QtiSchemaEnum.MultiSelect:
                    if (scoringType == "Auto-Scored" || String.IsNullOrEmpty(scoringType))
                    {
                        return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    if (scoringType == "Ungraded")
                    {
                        return "0 Point";
                    }
                    break;
                case (int)QtiSchemaEnum.ChoiceMultipleVariable:
                    if (scoringType == "Auto-Scored" || scoringType == "Ungraded")
                    {
                        var answerScores = item.ChoiceVariableAnswerScores.Where(s => s.VirtualQuestionId == (item.VirtualQuestionID.HasValue ? item.VirtualQuestionID.Value : 0)).OrderBy(s => s.VirtualQuestionAnswerScoreId).ToList();
                        return string.Join(" | ", answerScores.Select(s => $"{s.Answer.ReplaceMultiple(replacements).ConvertFromWindow1252ToUnicode()} - {s.Score} {(s.Score > 1 ? "Points" : "Point")}"));
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.InlineChoice:
                    if (scoringType == "Auto-Scored")
                    {
                        return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.TextEntry:
                    if (scoringType == "Auto-Scored" || scoringType == "Manual")
                    {
                        if (item.CorrectAnswer.StartsWith("<rangeValue>"))
                        {
                            return ParseCorrectAnswer(item.CorrectAnswer);
                        }
                        else
                        {
                            var answerScores = item.ChoiceVariableAnswerScores.Where(s => s.VirtualQuestionId == (item.VirtualQuestionID.HasValue ? item.VirtualQuestionID.Value : 0)).OrderBy(s => s.VirtualQuestionAnswerScoreId).ToList();
                            return string.Join(" | ", answerScores.Select(s => $"{s.Answer.ReplaceMultiple(replacements).ConvertFromWindow1252ToUnicode()} - {s.Score} {(s.Score > 1 ? "Points" : "Point")}"));
                        }
                    }
                    if (scoringType == "Ungraded")
                    {
                        return "0 Point";
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.ExtendedText:
                    if (scoringType == "Auto-Scored" || scoringType == "Manual" || scoringType == "Ungraded")
                    {
                        return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                    }
                    if (scoringType == "Rubric-Based")
                    {
                        var finalResult = "";
                        var rubricQuestionCategories = item.RubricAnswerScores.Where(s => s.VirtualQuestionID == (item.VirtualQuestionID.HasValue ? item.VirtualQuestionID.Value : 0)).ToList();
                        if (rubricQuestionCategories.Any())
                        {
                            for (int i = 0; i < rubricQuestionCategories.Count; i++)
                            {
                                var categoryTier = string.Join(" | ", rubricQuestionCategories[i].RubricCategoryTiers.Select(s =>
                                String.IsNullOrEmpty(s.Label) ? $"{s.Point} {(s.Point > 1 ? "Points" : "Point")}" : $"{s.Label} - {s.Point} {(s.Point > 1 ? "Points" : "Point")}"));
                                finalResult += $"{rubricQuestionCategories[i].CategoryName} ({categoryTier})";
                                if (i < rubricQuestionCategories.Count() - 1)
                                {
                                    finalResult += " | ";
                                }
                            }
                        }
                        return finalResult;
                    }
                    break;
                case (int)QtiSchemaEnum.Complex:
                    var complexVirtualQuestions = item.ComplexAnswerScores.Where(s => s.VirtualQuestionId == (item.VirtualQuestionID.HasValue ? item.VirtualQuestionID.Value : 0)).Select(
                        x => new { x.ResponseIdentifier, x.SubPointsPossible, x.QiSubCorrectAnswer, x.QiSubPointsPossible, x.QTISchemaID, x.ResponseProcessingTypeId })
                        .Distinct().OrderBy(x => x.ResponseIdentifier).ToList();
                    var complexResult = "";
                    var numResponse = 1;
                    foreach (var question in complexVirtualQuestions)
                    {
                        var testScore = string.IsNullOrEmpty(question.SubPointsPossible) ? 0 : int.Parse(question.SubPointsPossible);
                        var complexAnswerKey = "";
                        if (numResponse != 1)
                        {
                            complexResult += Environment.NewLine;
                        }
                        switch (question.QTISchemaID)
                        {
                            case (int)QtiSchemaEnum.MultipleChoice:
                            case (int)QtiSchemaEnum.MultiSelect:
                            case (int)QtiSchemaEnum.InlineChoice:
                            case (int)QtiSchemaEnum.ExtendedText:
                                complexAnswerKey = $"{question.QiSubCorrectAnswer.ConvertFromWindow1252ToUnicode()} - {question.QiSubPointsPossible} {(testScore > 1 ? "Points" : "Point")}";
                                complexResult += $"Response {numResponse.ToString()}: {complexAnswerKey}";
                                numResponse++;
                                break;
                            case (int)QtiSchemaEnum.TextEntry:
                                if (question.ResponseProcessingTypeId == (int)ResponseProcessingTypeIdEnum.Ungraded)
                                {
                                    complexAnswerKey = "0 Point";
                                }
                                else
                                {
                                    var complexCorrectAnswer = question.QiSubCorrectAnswer.ConvertFromWindow1252ToUnicode();
                                    if (complexCorrectAnswer.StartsWith("<rangeValue>"))
                                    {
                                        complexAnswerKey = ParseCorrectAnswer(complexCorrectAnswer);
                                    }
                                    else
                                    {
                                        var answerScores = item.ChoiceVariableAnswerScores.Where(s => s.VirtualQuestionId == (item.VirtualQuestionID.HasValue ? item.VirtualQuestionID.Value : 0) && s.ResponseIdentifier == question.ResponseIdentifier).OrderBy(s => s.VirtualQuestionAnswerScoreId).ToList();
                                        complexAnswerKey = string.Join(" | ", answerScores.Select(s => $"{s.Answer.ReplaceMultiple(replacements).ConvertFromWindow1252ToUnicode()} - {s.Score} {(s.Score > 1 ? "Points" : "Point")}"));
                                    }
                                }
                                complexResult += $"Response {numResponse.ToString()}: {complexAnswerKey}";
                                numResponse++;
                                break;
                            default:
                                break;
                        }
                    }
                    return complexResult;
                case (int)QtiSchemaEnum.DragAndDrop:
                    if (scoringType == "Auto-Scored")
                    {
                        return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.TextHotSpot:
                case (int)QtiSchemaEnum.ImageHotSpot:
                case (int)QtiSchemaEnum.TableHotSpot:
                case (int)QtiSchemaEnum.NumberLineHotSpot:
                    if (scoringType == "Auto-Scored")
                    {
                        var answerScores = item.ChoiceVariableAnswerScores.Where(s => s.VirtualQuestionId == (item.VirtualQuestionID.HasValue ? item.VirtualQuestionID.Value : 0)).OrderBy(s => s.VirtualQuestionAnswerScoreId).ToList();
                        if (answerScores.All(s => s.Score == 0))
                        {
                            return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                        }
                        else
                        {
                            return string.Join(" | ", answerScores.Select(s => $"{s.Answer.ReplaceMultiple(replacements).ConvertFromWindow1252ToUnicode()} - {s.Score} {(s.Score > 1 ? "Points" : "Point")}"));
                        }
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.DragAndDropNumerical:
                    if (scoringType == "Auto-Scored")
                    {
                        var responseProcessingParser = _qTIItemConvert.GetResponseProcessingParserFactory()
                            .GetResponseProcessingParser(item.QTISchemaID);
                        var responseProcessingDTO = responseProcessingParser.Parse(item.ResponseProcessing) as DragDropNumericalRPDTO;
                        if (responseProcessingDTO == null)
                        {
                            return string.Empty;
                        }
                        else
                        {
                            return $"{responseProcessingDTO.ExpressionPattern} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                        }
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.DragAndDropSequence:
                    if (scoringType == "Auto-Scored")
                    {
                        return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                default:
                    break;
            }
            return string.Empty;
        }

        public string GetQTIItemAnswerKey(AnswerKeyData item)
        {
            var scoringType = QtiItemScoringMethodHelper.GetQtiItemScoringMethod(item.XmlContent, item.QTISchemaID, false);
            switch (item.QTISchemaID)
            {
                case (int)QtiSchemaEnum.MultipleChoice:
                case (int)QtiSchemaEnum.MultiSelect:
                    if (scoringType == "Auto-Scored")
                    {
                        return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    if (scoringType == "Ungraded")
                    {
                        return "0 Point";
                    }
                    break;
                case (int)QtiSchemaEnum.ChoiceMultipleVariable:
                    if (scoringType == "Auto-Scored" || scoringType == "Ungraded")
                    {
                        var answerScores = item.QTIItemAnswerScores.Where(s => s.QTIItemID == item.QTIItemID).ToList();
                        return string.Join(" | ", answerScores.Select(s => $"{s.Answer.ReplaceMultiple(replacements).ConvertFromWindow1252ToUnicode()} - {s.ScoreValue} {(s.ScoreValue > 1 ? "Points" : "Point")}"));
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.InlineChoice:
                    if (scoringType == "Auto-Scored")
                    {
                        return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.TextEntry:
                    if (scoringType == "Auto-Scored" || scoringType == "Manual")
                    {
                        if (item.CorrectAnswer.StartsWith("<rangeValue>"))
                        {
                            return ParseCorrectAnswer(item.CorrectAnswer);
                        }
                        else
                        {
                            var answerScores = item.QTIItemAnswerScores.Where(s => s.QTIItemID == item.QTIItemID).ToList();
                            return string.Join(" | ", answerScores.Select(s => $"{s.Answer.ReplaceMultiple(replacements).ConvertFromWindow1252ToUnicode()} - {s.ScoreValue} {(s.ScoreValue > 1 ? "Points" : "Point")}"));
                        }
                    }
                    if (scoringType == "Ungraded")
                    {
                        return "0 Point";
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.ExtendedText:
                    if (scoringType == "Auto-Scored" || scoringType == "Manual" || scoringType == "Ungraded" || scoringType == "Rubric-Based")
                    {
                        return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                    }
                    break;
                case (int)QtiSchemaEnum.Complex:
                    var qtiSubs = item.QTIItemSubs
                        .Where(x => x.QTIItemID == item.QTIItemID)
                        .Select(x => new { x.ResponseIdentifier, x.PointsPossible, x.CorrectAnswer, x.QTISchemaID, x.ResponseProcessingTypeID, x.QTIItemID })
                        .Distinct()
                        .OrderBy(x => x.ResponseIdentifier)
                        .ToList();

                    var complexResult = "";
                    var numResponse = 1;
                    foreach (var qtiSub in qtiSubs)
                    {
                        var complexAnswerKey = "";
                        if (numResponse != 1)
                        {
                            complexResult += Environment.NewLine;
                        }
                        switch (qtiSub.QTISchemaID)
                        {
                            case (int)QtiSchemaEnum.MultipleChoice:
                            case (int)QtiSchemaEnum.MultiSelect:
                            case (int)QtiSchemaEnum.InlineChoice:
                            case (int)QtiSchemaEnum.ExtendedText:
                                complexAnswerKey = $"{qtiSub.CorrectAnswer.ConvertFromWindow1252ToUnicode()} - {qtiSub.PointsPossible} {(qtiSub.PointsPossible > 1 ? "Points" : "Point")}";
                                complexResult += $"Response {numResponse.ToString()}: {complexAnswerKey}";
                                numResponse++;
                                break;
                            case (int)QtiSchemaEnum.TextEntry:
                                if (qtiSub.ResponseProcessingTypeID == (int)ResponseProcessingTypeIdEnum.Ungraded)
                                {
                                    complexAnswerKey = "0 Point";
                                }
                                else
                                {
                                    var complexCorrectAnswer = qtiSub.CorrectAnswer.ConvertFromWindow1252ToUnicode();
                                    if (complexCorrectAnswer.StartsWith("<rangeValue>"))
                                    {
                                        complexAnswerKey = ParseCorrectAnswer(complexCorrectAnswer);
                                    }
                                    else
                                    {
                                        var answerScores = item.QTIItemAnswerScores.Where(x => x.QTIItemID == qtiSub.QTIItemID && x.ResponseIdentifier == qtiSub.ResponseIdentifier).ToList();
                                        complexAnswerKey = string.Join(" | ", answerScores.Select(s => $"{s.Answer.ReplaceMultiple(replacements).ConvertFromWindow1252ToUnicode()} - {s.ScoreValue} {(s.ScoreValue > 1 ? "Points" : "Point")}"));
                                    }
                                }
                                complexResult += $"Response {numResponse.ToString()}: {complexAnswerKey}";
                                numResponse++;
                                break;
                            default:
                                break;
                        }
                    }
                    return complexResult;
                case (int)QtiSchemaEnum.DragAndDrop:
                    if (scoringType == "Auto-Scored")
                    {
                        return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.TextHotSpot:
                case (int)QtiSchemaEnum.ImageHotSpot:
                case (int)QtiSchemaEnum.TableHotSpot:
                case (int)QtiSchemaEnum.NumberLineHotSpot:
                    if (scoringType == "Auto-Scored")
                    {
                        var answerScores = item.QTIItemAnswerScores.Where(s => s.QTIItemID == item.QTIItemID).ToList();
                        if (answerScores.All(s => s.ScoreValue == 0))
                        {
                            return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                        }
                        else
                        {
                            return string.Join(" | ", answerScores.Select(s => $"{s.Answer.ReplaceMultiple(replacements).ConvertFromWindow1252ToUnicode()} - {s.ScoreValue} {(s.ScoreValue > 1 ? "Points" : "Point")}"));
                        }
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.DragAndDropNumerical:
                    if (scoringType == "Auto-Scored")
                    {
                        var responseProcessingParser = _qTIItemConvert.GetResponseProcessingParserFactory()
                            .GetResponseProcessingParser(item.QTISchemaID);
                        var responseProcessingDTO = responseProcessingParser.Parse(item.ResponseProcessing) as DragDropNumericalRPDTO;
                        if (responseProcessingDTO == null)
                        {
                            return string.Empty;
                        }
                        else
                        {
                            return $"{responseProcessingDTO.ExpressionPattern} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                        }
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
                case (int)QtiSchemaEnum.DragAndDropSequence:
                    if (scoringType == "Auto-Scored")
                    {
                        return $"{item.CorrectAnswer} - {item.PointsPossible} {(item.PointsPossible > 1 ? "Points" : "Point")}";
                    }
                    if (scoringType == "Algorithmic")
                    {
                        return item.AlgorithmicExpression;
                    }
                    break;
            }
            return string.Empty;
        }

        private string ParseCorrectAnswer(string correctAnswer)
        {
            //add root elemnt for xml
            var xmlData = string.Format("<root>{0}</root>", correctAnswer);
            var textEntryRangeObject = new TextEntryRangeObject();
            var doc = XDocument.Parse(xmlData);
            if (doc.Root != null)
            {
                foreach (var rangeValueElement in doc.Root.Elements())
                {
                    var elements = rangeValueElement.Elements().ToList();
                    if (GetValueFromElement(SATConstants.RangeValueXmlNameTag, elements) == SATConstants.RangeValueXmlNameStartValue)
                    {
                        textEntryRangeObject.StartValue = GetValueFromElement(SATConstants.RangeValueXmlValueTag, elements);
                        textEntryRangeObject.IsStartExclusived =
                            GetValueFromElement(SATConstants.RangeValueXmlExclusivityTag, elements) == SATConstants.RangeValueXmlExclusitivityValue;
                    }

                    if (GetValueFromElement(SATConstants.RangeValueXmlNameTag, elements) == SATConstants.RangeValueXmlNameEndValue)
                    {
                        textEntryRangeObject.EndValue = GetValueFromElement(SATConstants.RangeValueXmlValueTag, elements);
                        textEntryRangeObject.IsEndExclusived =
                            GetValueFromElement(SATConstants.RangeValueXmlExclusivityTag, elements) == SATConstants.RangeValueXmlExclusitivityValue;
                    }
                }
            }
            var outputStringBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(textEntryRangeObject.StartValue) == false)
            {
                outputStringBuilder.Append(textEntryRangeObject.IsStartExclusived ? "> " : "≥ ");
                outputStringBuilder.AppendFormat("{0}", textEntryRangeObject.StartValue);

            }
            if (string.IsNullOrEmpty(textEntryRangeObject.EndValue) == false)
            {
                if (string.IsNullOrEmpty(textEntryRangeObject.StartValue) == false) outputStringBuilder.Append(" and ");
                outputStringBuilder.Append(textEntryRangeObject.IsEndExclusived ? "< " : "≤ ");
                outputStringBuilder.AppendFormat("{0}", textEntryRangeObject.EndValue);
            }

            return outputStringBuilder.ToString();
        }

        private string GetValueFromElement(string tagValue, List<XElement> listElements)
        {
            if (listElements.Any(x => x.Name.LocalName.Equals(tagValue)))
            {
                return listElements.First(x => x.Name.LocalName.Equals(tagValue)).Value;
            }
            return string.Empty;
        }
    }
}
