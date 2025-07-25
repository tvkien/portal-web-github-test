using LinkIt.BubbleSheetPortal.Common.Enum;
using System;

namespace LinkIt.BubbleSheetPortal.Models.ACTReport
{
    public class ACTSectionTagData
    {
        public int VirtualTestID { get; set; }
        public string SectionName { get; set; }
        public int SectionID { get; set; }
        public string TagName { get; set; }
        public string TagNameForOrder { get; set; }
        public int TagID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public int CategoryID { get; set; }
        public int TotalAnswer { get; set; }
        public int CorrectAnswer { get; set; }
        public int IncorrectAnswer { get; set; }
        public int BlankAnswer { get; set; }
        public int Percentage { get; set; }
        public int HistoricalAvg { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int TestResultID { get; set; }

        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int MinQuestionOrder { get; set; }
        public int? PresentationType { get; set; }
        public int? ItemTagCategoryOrder { get; set; }
        public int? ItemTagOrder { get; set; }
        public int? SectionOrder { get; set; }

        public override bool Equals(object obj)
        {
            ACTSectionTagData data = obj as ACTSectionTagData;
            return data != null && VirtualTestID.Equals(data.VirtualTestID) && SectionID.Equals(data.SectionID)
                   && TagID.Equals(data.TagID) && CategoryID.Equals(data.CategoryID) &&
                   TestResultID.Equals(data.TestResultID)
                   && SubjectID.Equals(data.SubjectID);
        }

        public override int GetHashCode()
        {
            return VirtualTestID*21 + SectionID*32 + TagID*43 + CategoryID*54 + TestResultID*65 + SubjectID*76;
        }
    }
}