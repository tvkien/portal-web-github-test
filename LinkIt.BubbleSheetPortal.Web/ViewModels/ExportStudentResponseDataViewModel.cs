using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class ExportStudentResponseDataViewModel
    {
        public int TestResultID { get; set; }

        public string ClassName { get; set; }

        public int? UserID { get; set; }

        public string UserName { get; set; }

        public string Term { get; set; }

        public int? SchoolID { get; set; }

        public string SchoolName { get; set; }

        public int StudentID { get; set; }

        public string TestName { get; set; }

        public List<string> AnswerLetters { get; set; }
    }
}