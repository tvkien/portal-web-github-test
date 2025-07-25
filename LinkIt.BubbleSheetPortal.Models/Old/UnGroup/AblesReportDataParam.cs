using System.Collections.Generic;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AblesReportDataParam : ValidatableEntity<AblesReportDataParam>
    {
        public int? DistrictId { get; set; }
        public int? ReportType { get; set; }
        public int? SchoolId { get; set; }
        public int? TermId { get; set; }
        public string AblesTestName { get; set; }

        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public int? TeacherId { get; set; }
        public string FileName { get; set; }
        public string SelectedStudent { get; set; }
    }
}