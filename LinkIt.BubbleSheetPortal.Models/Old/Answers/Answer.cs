using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class Answer
    {
        private string answerLetter = string.Empty;
        private string answerText = string.Empty;
        private string bubbleSheetErrorType = string.Empty;

        public int AnswerID { get; set; }
        public int PointsEarned { get; set; }
        public int PointsPossible { get; set; }
        public bool WasAnswered { get; set; }
        public int TestResultID { get; set; }
        public int VirtualQuestionID { get; set; }
        public bool Blocked { get; set; }
         

        public string AnswerLetter
        {
            get { return answerLetter; }
            set { answerLetter = value.ConvertNullToEmptyString(); }
        }
        public string AnswerText
        {
            get { return answerText; }
            set { answerText = value.ConvertNullToEmptyString(); }
        }
        public string BubbleSheetErrorType
        {
            get { return bubbleSheetErrorType; }
            set { bubbleSheetErrorType = value.ConvertNullToEmptyString(); }
        }
         
    }
}
