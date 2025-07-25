using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class SearchBankAdvancedFilter
    {
        public IEnumerable<int> GradeIds { get; set; } = new List<int>();
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int UserRole { get; set; }
        public IEnumerable<string> SubjectNames { get; set; } = new string[0];
        public string ModuleCode { get; set; }
        public int SchoolId { get; set; }
        public string Level { get; set; }
        public string ClassIds { get; set; }
    }
}
