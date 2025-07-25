using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class RubricCustomListViewModel
    {
        public string SubjectName { get; set; }
        public string GradeName { get; set; }
        public string TestBankName { get; set; }
        public string Author { get; set; }
        public string TestName { get; set; }
        public string FileName { get; set; }

        public int? DistrictId { get; set; }
        public int? GradeId { get; set; }
        public int? SubjectId { get; set; }
        public int? RubricId { get; set; }
        public int TestId { get; set; }
        public string RubricKey { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortColumns { get; set; }
    }
}
