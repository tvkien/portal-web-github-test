using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class RubricCustomList
    {
        public int? GradeId { get; set; }
        public int? DistrictId { get; set; }
        public int? UserId { get; set; }
        public int? UserRole { get; set; }
        public int? SubjectId { get; set; }    
        public string SubjectName { get; set; }
        public string BankName { get; set; }
        public string Author { get; set; }
        public string TestName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
    }
}
