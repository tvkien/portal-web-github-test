using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.Algorithmic
{
    public interface IAlgorithmicConditionParser
    {
        List<AlgorithmicCorrectAnswer> ConvertToAlgorithmicCorrectAnswers(List<AlgorithmicQuestionExpression> algorithmQuestions);
    }
}