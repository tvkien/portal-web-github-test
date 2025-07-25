using System.Collections.Generic;
using System.Globalization;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class PreviouslyAnsweredQuestion
    {
        public int AnswerID { get; set; }
        public int VirtualQuestionId { get; set; }
        public string AnswerLetter { get; set; }
        public bool WasAnswered { get; set; }
        public int? BubbleSheetId { get; set; }
        public int StudentId { get; set; }
        public int QuestionOrder { get; set; }
    }
}