using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SATBubbleSheetStudentDetailsViewModel
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
        public string ArtifactFileName { get; set; }
        public string ImageUrl { get; set; }
        public bool HasBubbleSheetFile { get; set; }
        public bool IsManualEntry { get; set; }
        public string VirtualTestFileKey { get; set; }
        public string VirtualTestFileName { get; set; }

        public bool OnlyOnePage { get; set; }
        public List<PreviousBubbleSheetDetailsViewModel> PreviousBubbleSheets { get; set; }
        public List<ACTUnansweredQuestion> UnansweredQuestions { get; set; }
        public List<ACTAlreadyAnsweredQuestion> AnsweredQuestions { get; set; }

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

        public SATBubbleSheetStudentDetailsViewModel()
        {
            PreviousBubbleSheets = new List<PreviousBubbleSheetDetailsViewModel>();
            UnansweredQuestions = new List<ACTUnansweredQuestion>();
            AnsweredQuestions = new List<ACTAlreadyAnsweredQuestion>();
            ListSection = new List<int>();
            ListSectionQuestions = new List<VirtualSectionQuestion>();
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

        //\
        public List<ACTUnansweredQuestion> LstUnansweredQuestionSection1
        {
            get
            {
                return UnansweredQuestions.Where(o => o.OrderSectionIndex == 1)
                    .OrderBy(o => o.OrderSectionQuestionIndex)
                    .ToList();
            }
        }

        public List<ACTUnansweredQuestion> LstUnansweredQuestionSection2
        {
            get
            {
                return UnansweredQuestions.Where(o => o.OrderSectionIndex == 2)
                    .OrderBy(o => o.OrderSectionQuestionIndex)
                    .ToList();
            }
        }

        public List<ACTUnansweredQuestion> LstUnansweredQuestionSection3
        {
            get
            {
                return UnansweredQuestions.Where(o => o.OrderSectionIndex == 3)
                    .OrderBy(o => o.OrderSectionQuestionIndex)
                    .ToList();
            }
        }

        public List<ACTUnansweredQuestion> LstUnansweredQuestionSection4
        {
            get
            {
                return UnansweredQuestions.Where(o => o.OrderSectionIndex == 4)
                    .OrderBy(o => o.OrderSectionQuestionIndex)
                    .ToList();
            }
        }

        public List<ACTUnansweredQuestion> LstUnansweredQuestionSection5
        {
            get
            {
                return UnansweredQuestions.Where(o => o.OrderSectionIndex == 5)
                    .OrderBy(o => o.OrderSectionQuestionIndex)
                    .ToList();
            }
        }

        public List<ACTAlreadyAnsweredQuestion> LstAnsweredQuestionsSection1
        {
            get
            {
                return AnsweredQuestions.Where(o => o.OrderSectionIndex == 1)
                    .OrderBy(o => o.OrderSectionQuestionIndex)
                    .ToList();
            }
        }

        public List<ACTAlreadyAnsweredQuestion> LstAnsweredQuestionsSection2
        {
            get
            {
                return AnsweredQuestions.Where(o => o.OrderSectionIndex == 2)
                    .OrderBy(o => o.OrderSectionQuestionIndex)
                    .ToList();
            }
        }

        public List<ACTAlreadyAnsweredQuestion> LstAnsweredQuestionsSection3
        {
            get
            {
                return AnsweredQuestions.Where(o => o.OrderSectionIndex == 3)
                    .OrderBy(o => o.OrderSectionQuestionIndex)
                    .ToList();
            }
        }

        public List<ACTAlreadyAnsweredQuestion> LstAnsweredQuestionsSection4
        {
            get
            {
                return AnsweredQuestions.Where(o => o.OrderSectionIndex == 4)
                    .OrderBy(o => o.OrderSectionQuestionIndex)
                    .ToList();
            }
        }

        public List<ACTAlreadyAnsweredQuestion> LstAnsweredQuestionsSection5
        {
            get
            {
                return AnsweredQuestions.Where(o => o.OrderSectionIndex == 5)
                    .OrderBy(o => o.OrderSectionQuestionIndex)
                    .ToList();
            }
        }

        public List<int> ListSection { get; set; }

        public List<VirtualSectionQuestion> ListSectionQuestions { get; set; }

        public DateTime? ResultDate { get; set; }

        public List<ACTResultDateModel> ResultDateChoices
        {
            get
            {
                var choices = new List<ACTResultDateModel>();
                //var resultDate = ResultDate.HasValue ? ResultDate.Value.DisplayDateWithFormat() : "xx/xx/xxxx";
                var resultDate = ResultDate.HasValue ? ResultDate.Value.ToString("MM/dd/yyyy") : "xx/xx/xxxx";
                var splitDate = resultDate.Split(new[] { '/' });
                for (int i = 0; i < splitDate.Length; i++)
                {
                    for (int j = 0; j < splitDate[i].Length; j++)
                    {
                        if (i == 0)
                        {
                            choices.Add(new ACTResultDateModel
                            {
                                Choice = splitDate[i][j].ToString(),
                                Property = string.Format("M{0}", j),
                                Label = "Month"
                            });
                        }
                        else if (i == 1)
                        {
                            choices.Add(new ACTResultDateModel
                            {
                                Choice = splitDate[i][j].ToString(),
                                Property = string.Format("D{0}", j),
                                Label = "Day"
                            });
                        }
                        else
                        {
                            choices.Add(new ACTResultDateModel
                            {
                                Choice = splitDate[i][j].ToString(),
                                Property = string.Format("Y{0}", j),
                                Label = "Year"
                            });
                        }
                    }
                }
                return choices;
            }
        }
    }
}
