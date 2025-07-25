using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AnswerSubData
    {
        private string answerText = string.Empty;
        public int AnswerSubID { get; set; }
        public int PointsEarned { get; set; }
        public int VirtulaQuestionSubID { get; set; }
        public string AnswerText
        {
            get { return answerText; }
            set { answerText = value.ConvertNullToEmptyString(); }
        }
    }
}
