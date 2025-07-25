using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestQuestionsViewModel
    {
        public int BubbleSheetFileId { get; set; }
        public int BubbleSheetId { get; set; }
        public int BubbleSheetErrorId { get; set; }
        public int RosterPosition { get; set; }
        public int ErrorCode { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int QuestionCount { get; set; }
        public string Barcode { get; set; }
        public string InputFileName { get; set; }
        public string InputFilePath { get; set; }
        public string RelatedImage { get; set; }

        public IEnumerable<UnansweredQuestionAnswer> Answers { get; set; }
        public List<UnansweredQuestion> UnansweredQuestions { get; set; }
        public List<UnansweredQuestionAnswer> UnansweredQuestionAnswers { get; set; }

        public TestQuestionsViewModel()
        {
            Answers = new List<UnansweredQuestionAnswer>();
            UnansweredQuestionAnswers = new List<UnansweredQuestionAnswer>();
        }
        public int DistrictID { get; set; }
    }
}