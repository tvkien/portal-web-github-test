using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ACTCorrectQuestionErrorViewModel
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

        public ACTCorrectQuestionErrorViewModel()
        {
            UnansweredQuestions = new List<ACTUnansweredQuestion>();
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
    }
}
