using System.Collections;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport
{
    public class AnswerSectionViewModel
    {
        public int PointsEarned { get; set; }
        public int PointsPossible { get; set; }
        public bool WasAnswered { get; set; }
        public string AnswerLetter { get; set; }
        public string CorrectAnswer { get; set; }
        public int AnswerID { get; set; }
        public int QuestionOrder { get; set; }
        public int QTISchemaID { get; set; }
        public string TagNames { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public int VirtualQuestionID { get; set; }
        public List<string> PassageNames { get; set; }
        public List<int?> PassageIds { get; set; }
        public string PassageName { get; set; }
        public bool IsCorrected
        {
            get { return PointsEarned == PointsPossible && WasAnswered; }
        }

        public bool IsBlank
        {
            get { return !WasAnswered; }
        }

        public class Comparer : IEqualityComparer<AnswerSectionViewModel>
        {
            public bool Equals(AnswerSectionViewModel x, AnswerSectionViewModel y)
            {
                //return x.AnswerID == y.AnswerID;
                return x.VirtualQuestionID == y.VirtualQuestionID;
            }

            public int GetHashCode(AnswerSectionViewModel obj)
            {
                return obj.VirtualQuestionID;
            }
        }

        public class ACTComparer : IEqualityComparer<AnswerSectionViewModel>
        {
            public bool Equals(AnswerSectionViewModel x, AnswerSectionViewModel y)
            {
                return x.AnswerID == y.AnswerID;
            }

            public int GetHashCode(AnswerSectionViewModel obj)
            {
                return obj.AnswerID;
            }
        }
    }
}