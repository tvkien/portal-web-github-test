using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BubbleSheetStudentDetailsViewModel
    {
        public int? BubbleSheetFileId { get; set; }
        public int BubbleSheetId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int RosterPosition { get; set; }
        public string Ticket { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string SchoolName { get; set; }
        public string TeacherName { get; set; }
        public DateTime? UploadedDate { get; set; }
        public string UploadedBy { get; set; }
        public string FileName { get; set; }
        public string ImageUrl { get; set; }
        public bool HasBubbleSheetFile { get; set; }
        public bool IsManualEntry { get; set; }
        public string VirtualTestFileKey { get; set; }
        public string VirtualTestFileName { get; set; }
        public string ArtifactFileName { get; set; }

        public bool OnlyOnePage { get; set; }
        public List<PreviousBubbleSheetDetailsViewModel> PreviousBubbleSheets { get; set; }
        public List<UnansweredQuestion> UnansweredQuestions { get; set; }
        public List<AlreadyAnsweredQuestion> AnsweredQuestions { get; set; }

        private string unansweredQuestionsLable = "Unanswered Questions";

        public string UnansweredQuestionLable
        {
            get { return unansweredQuestionsLable; }
            set { unansweredQuestionsLable = value; }
        }

        private string answeredQuestionsLable = "Answered Questions";

        public string AnsweredQuestionLable
        {
            get { return answeredQuestionsLable; }
            set { answeredQuestionsLable = value; }
        }

        private string questionLable = "Question";

        public string QuestionLable
        {
            get { return questionLable; }
            set { questionLable = value; }
        }

        private string answerChoicesLable = "Answer Choices";

        public string AnswerChoicesLable
        {
            get { return answerChoicesLable; }
            set { answerChoicesLable = value; }
        }

        public BubbleSheetStudentDetailsViewModel()
        {
            PreviousBubbleSheets = new List<PreviousBubbleSheetDetailsViewModel>();
            UnansweredQuestions = new List<UnansweredQuestion>();
            AnsweredQuestions = new List<AlreadyAnsweredQuestion>();
        }

        public bool IsMultipleChoice
        {
            get
            {
                if (UnansweredQuestions.Any(o => o.QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple)
                    || AnsweredQuestions.Any(o => o.QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple))
                    return true;
                return false;
            }
        }

        public bool IsApplyFullCreditAnswer { get; set; }
        public bool IsApplyZeroCreditAnswer { get; set; }
    }
}
