using System.Collections.Generic;
using DevExpress.Office.Utils;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class BankCustomListViewModel
    {
        public string SubjectName { get; set; }
        public string GradeName { get; set; }
        public string Name { get; set; }
        public int? BankDistrictAccessId { get; set; }
        public bool Hide { get; set; }
        public int? SubjectId { get; set; }
        public int? GradeId { get; set; }
        public int? DistrictId { get; set; }
        public int BankAccessId { get; set; }
        public int BankDistrictId { get; set; }
        public List<int> SubjectIds { get; set; }
    }
}