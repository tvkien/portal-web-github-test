using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Models
{
    public class PrintVirtualTestModel
    {
        public int? VirtualTestID { get; set; }
        public string TestTitle { get; set; }

        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string IncludeCoverPage { get; set; }
        public string TestInstructions { get; set; }
        public string Columns { get; set; }
        public string AnswerLabelFormat { get; set; }
        public string ShowQuestionBorders { get; set; }
        public string ShowSectionHeadings { get; set; }
        public string DrawReferenceBackground { get; set; }
        public string QuestionPrefix { get; set; }
        public string IncludePageNumbers { get; set; }
        public string StartCountingOnCover { get; set; }
        public int? ExtendedTextAreaNumberOfLines { get; set; }
        public string ExtendedTextAreaAnswerOnSeparateSheet { get; set; }
        public string ExtendedTextAreaShowLines { get; set; }
        public string IncludeStandards { get; set; }
        public string IncludeTags { get; set; }
        public bool? IncludeRationale { get; set; }
        public bool? IncludeGuidance { get; set; }
    }
}