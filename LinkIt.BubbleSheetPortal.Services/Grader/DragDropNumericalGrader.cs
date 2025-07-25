using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LinkIt.BubbleSheetPortal.Models.ResponseProcessingDTO;
using NCalc;

namespace LinkIt.BubbleSheetPortal.Services.Grader
{
    public class DragDropNumericalGrader
    {
        private int _lastDestinationIndex = 0;

        public List<string> GetCorrectMappings(DragDropNumericalRPDTO responseProcessingDTO)
        {
            var result = new List<string>();
            if (responseProcessingDTO.Destinations == null || responseProcessingDTO.Destinations.Count == 0 ||
                responseProcessingDTO.Sources == null || responseProcessingDTO.Sources.Count == 0) return result;
            var destinations = responseProcessingDTO.Destinations;
            var sources = responseProcessingDTO.Sources.Select(o => o.SrcIdentifier).ToList();
            
            _lastDestinationIndex = destinations.Count - 1;
            Recursive(0, destinations, sources, string.Empty, result, true, responseProcessingDTO);
            return result;
        }

        private void Recursive(int destinationIndex, List<string> destinations, List<string> sources, string currentMapping, List<string> result, bool onlyCorrectMapping, DragDropNumericalRPDTO responseProcessingDTO)
        {
            var currentDestination = destinations[destinationIndex];
            var isLastDestination = destinationIndex == _lastDestinationIndex;
            foreach (var source in sources)
            {
                var newMapping = string.Empty;
                if (destinationIndex > 0) newMapping = currentMapping + ",";
                newMapping = newMapping + string.Format("{0}-{1}", currentDestination, source);
                if (isLastDestination)
                {
                    if (!onlyCorrectMapping)
                    {
                        result.Add(newMapping);
                    }
                    else
                    {
                        //var correctMathExpression = GetCorrectMathExpression(responseProcessingDTO, newMapping);
                        var correctMathExpression = GetCorrectMathExpressionWithCalcEngine(responseProcessingDTO, newMapping);
                        if (!string.IsNullOrWhiteSpace(correctMathExpression) && result.All(o => o != correctMathExpression)) result.Add(correctMathExpression);
                    }
                    continue;
                }

                Recursive(destinationIndex + 1, destinations, sources, newMapping, result, onlyCorrectMapping, responseProcessingDTO);
            }
        }

        public string GetCorrectMathExpression(DragDropNumericalRPDTO responseProcessingDTO, string answerText)
        {
            if (responseProcessingDTO == null) return string.Empty;

            var mathExpression = BuildMathExpression(responseProcessingDTO.ExpressionPattern,
                responseProcessingDTO.Sources, answerText);
            if (string.IsNullOrWhiteSpace(mathExpression)) return string.Empty;
            var evaluateExpression = mathExpression.Replace("[", "(").Replace("]", ")").Replace(",", "");
            var e = new Expression(evaluateExpression);
            if (!e.HasErrors())
            {
                try
                {
                    var result = e.Evaluate();
                    var str = result.ToString();
                    if ("True".Equals(str)) return mathExpression;
                }
                catch { }
            }

            return string.Empty;
        }

        public string BuildMathExpression(string expressionPattern, List<SrcOfDragDropNumericalRPDTO> sources,
            string answerText)
        {
            if (string.IsNullOrWhiteSpace(expressionPattern) || sources == null || string.IsNullOrWhiteSpace(answerText))
                return null;

            var answerMappings = GetAnswerMappings(answerText, sources);

            var reg = @"{(.*?)}";
            var m = Regex.Matches(expressionPattern, reg);
            foreach (Match g in m)
            {
                var value = g.Groups[1].Value;
                var valueOfDestinationInExpression =
                    answerMappings.Where(
                        o => string.Compare(o.Key, value, StringComparison.OrdinalIgnoreCase) == 0)
                        .Select(o => o.Value)
                        .FirstOrDefault();
                valueOfDestinationInExpression = valueOfDestinationInExpression ?? string.Empty;
                expressionPattern = expressionPattern.Replace("{" + value + "}", valueOfDestinationInExpression);
            }

            expressionPattern = ReplaceMathOperators(GetMathOperatorMappings(), expressionPattern);

            return expressionPattern;
        }

        public string ReplaceMathOperators(List<KeyValuePair<string, string>> mappings, string mathExpression)
        {
            if (mappings == null || string.IsNullOrWhiteSpace(mathExpression)) return string.Empty;
            foreach (var mapping in mappings)
            {
                mathExpression = mathExpression.Replace(mapping.Key, mapping.Value);
            }

            return mathExpression;
        }

        public List<KeyValuePair<string, string>> GetMathOperatorMappings()
        {
            var result = new List<KeyValuePair<string, string>>();
            result.Add(new KeyValuePair<string, string>("×", "*"));
            result.Add(new KeyValuePair<string, string>("÷", "/"));
            result.Add(new KeyValuePair<string, string>("&gt;=", ">="));
            result.Add(new KeyValuePair<string, string>("&lt;=", "<="));
            return result;
        }

        private List<KeyValuePair<string, string>> GetAnswerMappings(string answerText, List<SrcOfDragDropNumericalRPDTO> sources)
        {
            var result = new List<KeyValuePair<string, string>>();
            if (string.IsNullOrWhiteSpace(answerText)) return result;
            var mappings = answerText.Split(',');
            foreach (var mapping in mappings)
            {
                var items = mapping.Split('-');
                var destinationIdentifier = items[0] == null ? null : items[0].Trim();
                string sourceIdentifier = null;
                if (items.Count() == 2) sourceIdentifier = items[1] == null ? null : items[1].Trim();
                var sourceValue =
                    sources.FirstOrDefault(
                        o => string.Compare(o.SrcIdentifier, sourceIdentifier, StringComparison.OrdinalIgnoreCase) == 0);
                var value = sourceValue == null ? string.Empty : sourceValue.Value;
                result.Add(new KeyValuePair<string, string>(destinationIdentifier, value));
            }

            return result;
        }

        public string GetCorrectMathExpressionWithCalcEngine(DragDropNumericalRPDTO responseProcessingDTO,
            string answerText)
        {
            if (responseProcessingDTO == null) return string.Empty;

            var mathExpression = BuildMathExpression(responseProcessingDTO.ExpressionPattern,
                responseProcessingDTO.Sources, answerText);
            if (string.IsNullOrWhiteSpace(mathExpression)) return string.Empty;
            var evaluateExpression = mathExpression.Replace("[", "(").Replace("]", ")").Replace(",", "");

            try
            {
                var ce = new Zirpl.CalcEngine.CalculationEngine();
                var x = ce.Parse(evaluateExpression);
                var value = x.Evaluate();
                var str = value.ToString();
                if ("True".Equals(str))
                {
                    return mathExpression;
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }
    }
}
