using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class PrintTestRequest
    { 
        public string StartCountingOnCover { get; set; }
        public string TestTitle { get; set; }
        public string TeacherName { get; set; }
        public string IncludePageNumbers { get; set; }
        public string IncludeCoverPage { get; set; }
        public string Columns { get; set; }
        public string AnswerLabelFormat { get; set; }
        public string IncludeStandards { get; set; }
        public string ShowQuestionBorders { get; set; }
        public string TestInstructions { get; set; }
        public string ExtendedTextAreaShowLines { get; set; }
        public string IncludeTags { get; set; }
        public string VirtualTestID { get; set; }
        public string Instruction { get; set; }
        public string ClassName { get; set; }
        public string DrawReferenceBackground { get; set; }
        public string ExtendedTextAreaAnswerOnSeparateSheet { get; set; }
        public string ShowSectionHeadings { get; set; }
        public string PrintingType { get; set; }
        public string QuestionPrefix { get; set; }
        public string ExtendedTextAreaNumberOfLines { get; set; }
        public string Token { get; set; }
        public string PrintAnswerKeyURL { get; set; }
        public string PrintTestURL { get; set; }
        public bool IsLockbank { get; set; }
        public string RubricKey { get; set; }
        public string RubricLink { get; set; }
        public bool CanPrintQuestion { get; set; }
        public bool CanPrintAnswerKey { get; set; }
    }
}