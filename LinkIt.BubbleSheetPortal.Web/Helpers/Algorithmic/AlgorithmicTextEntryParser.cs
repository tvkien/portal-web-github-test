using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.Algorithmic
{
    /// <summary>
    /// {RESPONSE_1;3;>=3} AND {RESPONSE_1;3;<=5} => {RESPONSE_1;3;>=3 and <=5}
    /// {RESPONSE_1;3;>6} AND {RESPONSE_1;3;<9}
    /// {RESPONSE_1;3;=Washinton} OR {RESPONSE_1;3;=WD}
    /// {RESPONSE_1;3;=Wasington City}
    /// </summary>
    public class AlgorithmicTextEntryParser : BaseAlgorithmicParser, IAlgorithmicConditionParser
    {
        public override List<AlgorithmicCorrectAnswer> ConvertToAlgorithmicCorrectAnswers(List<AlgorithmicQuestionExpression> algorithmQuestions)
        {
            var result = new List<AlgorithmicCorrectAnswer>();
            foreach (var item in algorithmQuestions)
            {
                var correctAnswers = ParseToAlgorithmicCorrectAnswers(item.AlgorithmicExpression);
                if (correctAnswers != null)
                {
                    for (int i = 0; i < correctAnswers.Count; i++)
                    {
                        correctAnswers[i].PointsEarned = item.PointsEarned;
                    }
                    result.AddRange(correctAnswers);
                }
            }

            return result;
        }

        public override List<AlgorithmicCorrectAnswer> ParseToAlgorithmicCorrectAnswers(string expression)
        {
            var result = new List<AlgorithmicCorrectAnswer>();

            var blockExpressions = SplitExpressionToBlocks(expression, SingleExpressionRegEx);

            // range value
            if (expression.Contains("AND") || expression.Contains("and"))
            {
                if (blockExpressions.Count == 2)
                {
                    var temp = new List<AlgorithmicCorrectAnswer>();
                    foreach (var singleBlock in blockExpressions)
                    {
                        var correctAnswer = ParseSingleBlockToCorrectAnswer(singleBlock);
                        if (correctAnswer != null)
                        {
                            temp.Add(correctAnswer);
                        }
                    }

                    if (temp.Count == 2)
                    {
                        var combineAnswer = new AlgorithmicCorrectAnswer();
                        combineAnswer.OriginalExpression = expression;
                        combineAnswer.ConditionType = temp[0].ConditionType;
                        combineAnswer.ResponseIdentifier = temp[0].ResponseIdentifier;
                        if (temp[0].ConditionValue.Any() && temp[1].ConditionValue.Any())
                        {
                            combineAnswer.ConditionValue.Add(string.Format("{0} and {1}", temp[0].ConditionValue[0], temp[1].ConditionValue[0]));
                        }

                        result.Add(combineAnswer);
                    }

                }
            }
            else
            {
                // text value
                foreach (var singleBlock in blockExpressions)
                {
                    var correctAnswer = ParseSingleBlockToCorrectAnswer(singleBlock);
                    if (correctAnswer != null)
                    {
                        result.Add(correctAnswer);
                    }
                }
            }

            return result;
        }
    }
}