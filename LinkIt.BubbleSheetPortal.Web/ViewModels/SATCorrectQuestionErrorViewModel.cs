using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SATCorrectQuestionErrorViewModel
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

        public List<ACTUnansweredQuestion> UnansweredQuestions { get; set; }
        public List<int> ListSection { get; set; }

        public SATCorrectQuestionErrorViewModel()
        {
            UnansweredQuestions = new List<ACTUnansweredQuestion>();
            ListSection = new List<int>();
        }

        public bool IsMultipleChoice
        {
            get
            {
                if (UnansweredQuestions.Any(o => o.QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple))
                    return true;
                return false;
            }
        }
    }
}
