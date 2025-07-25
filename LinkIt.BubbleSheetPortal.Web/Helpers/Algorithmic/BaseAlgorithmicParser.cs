using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.Algorithmic
{
    public abstract class BaseAlgorithmicParser
    {
        public const string SingleExpressionRegEx = @"{.*?}";
        public const string AtleastBlockRegEx = @"(ATLEAST_\d)\s*(\[.*?\])";
        public virtual List<AlgorithmicCorrectAnswer> ConvertToAlgorithmicCorrectAnswers(List<AlgorithmicQuestionExpression> algorithmQuestions)
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

        public virtual List<AlgorithmicCorrectAnswer> ParseToAlgorithmicCorrectAnswers(string expression)
        {
            var result = new List<AlgorithmicCorrectAnswer>();

            // parse ATLEAST_N
            var listAtleastExpression = SplitExpressionToBlocks(expression, AtleastBlockRegEx);
            foreach (var atleastExpression in listAtleastExpression)
            {
                result.Add(ParseAtleastBlock(atleastExpression));
            }

            if (listAtleastExpression.Count > 0)
                return result;

            var blockExpressions = SplitExpressionToBlocks(expression, SingleExpressionRegEx);
            foreach (var singleBlock in blockExpressions)
            {
                var correctAnswer = ParseSingleBlockToCorrectAnswer(singleBlock);
                if (correctAnswer != null)
                {
                    result.Add(correctAnswer);
                }
            }

            return result;
        }

        public virtual List<string> SplitExpressionToBlocks(string expression, string pattern)
        {
            var result = new List<string>();
            var regEx = new Regex(pattern);
            var matches = regEx.Matches(expression);

            for (int i = 0; i < matches.Count; i++)
            {
                var math = matches[i];
                result.Add(math.Value);
            }

            return result;
        }

        public virtual AlgorithmicCorrectAnswer ParseSingleBlockToCorrectAnswer(string singleBlock)
        {
            if (string.IsNullOrEmpty(singleBlock))
            {
                return null;
            }

            var data = singleBlock.Replace("{", "").Replace("}", "").Split(';');
            if (data.Length != 3) return null;

            var correctAnswer = new AlgorithmicCorrectAnswer()
            {
                ResponseIdentifier = data[0],
                ConditionType = int.Parse(data[1]),
                OriginalExpression = singleBlock
            };
            correctAnswer.ConditionValue.Add(data[2]);
            return correctAnswer;
        }

        private AlgorithmicCorrectAnswer ParseAtleastBlock(string atleastExpression)
        {
            var regEx = new Regex(AtleastBlockRegEx);
            var match = regEx.Match(atleastExpression);

            if (match.Success && match.Groups.Count == 3)
            {
                var atleastPart = match.Groups[1].Captures[0].Value; //e.g. ATLEAST_123
                var conditionBlockPart = match.Groups[2].Captures[0].Value; //e.g. [{RESPONSE_1;2;A}, {RESPONSE_2;2;A}]

                var amount = GetAtleastAmount(atleastPart);
                if (amount <= 0) return null;

                var singleConditionBlockList = SplitExpressionToBlocks(conditionBlockPart, SingleExpressionRegEx);
                var correctAnswer = new AlgorithmicCorrectAnswer()
                {
                    OriginalExpression = atleastExpression
                };
                var conditionValueAtleast = new List<string>();
                var responseIdentifier = string.Empty;
                var conditionType = 0;
                foreach (var conditionExpression in singleConditionBlockList)
                {
                    var conditionBlock = ParseSingleBlockToCorrectAnswer(conditionExpression);
                    if (conditionBlock != null)
                    {
                        responseIdentifier = conditionBlock.ResponseIdentifier;
                        conditionType = conditionBlock.ConditionType;
                        conditionValueAtleast.AddRange(conditionBlock.ConditionValue);
                    }
                }
                if (conditionValueAtleast.Any())
                {
                    correctAnswer.ConditionValue = conditionValueAtleast;
                    correctAnswer.ConditionType = conditionType;
                    correctAnswer.OriginalExpression = atleastExpression;
                    correctAnswer.ResponseIdentifier = responseIdentifier;
                    correctAnswer.Amount = amount;
                }

                return correctAnswer;
            }
            return null;
        }

        /// <summary>
        /// Retrieve number from ATLEAST_X string
        /// </summary>
        /// <param name="atleastText">ATLEAST_12</param>
        /// <returns>Return -1 if cannot extract number, otherwise return number</returns>
        private int GetAtleastAmount(string atleastText)
        {
            string number = atleastText.ToLower().Replace("atleast_", "");
            int amount;
            if (int.TryParse(number, out amount))
            {
                return amount;
            }
            return -1;
        }
    }
}