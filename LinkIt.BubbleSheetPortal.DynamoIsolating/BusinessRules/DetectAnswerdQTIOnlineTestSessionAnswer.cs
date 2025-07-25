using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.BusinessRules
{
    public static class DetectAnswerdQTIOnlineTestSessionAnswer
    {
        public static bool Answered(QTIOnlineTestSessionAnswer answer)
        {
            if (answer == null) return false; 
            if (!answer.Answered.HasValue || !answer.Answered.Value) return false;
            if ("multi".Equals(answer.ResponseIdentifier)) return answer.Status;
            if (!string.IsNullOrWhiteSpace(answer.AnswerImage)) return true;
            if (!string.IsNullOrWhiteSpace(answer.AnswerText)) return true;
            if (answer.QTISchemaID.In((int)QtiSchemaEnum.InlineChoice,(int)QtiSchemaEnum.MultipleChoice,
                (int)QtiSchemaEnum.ChoiceMultiple,(int)QtiSchemaEnum.ChoiceMultipleVariable) && !"U".Equals(answer.AnswerChoice)) return true;
            if (!"O".Equals(answer.AnswerChoice) && !"U".Equals(answer.AnswerChoice)) return true;                
            return false;
        }
    }
}
