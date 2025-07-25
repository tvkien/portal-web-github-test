using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class GetTestByAdvanceFilterRequest
    {
        public int DistrictID { get; set; }
        public string ModuleCode { get; set; }
        public GetTestFilter Filters { get; set; } = new GetTestFilter();
        public GetTestPaging Pagination { get; set; } = new GetTestPaging();
        public string GeneralSearch { get; set; }
    }

    public class GetTestFilter
    {
        public IEnumerable<string> SubjectNames { get; set; } = new List<string>();
        public IEnumerable<int> BankIds { get; set; } = new List<int>();
        public IEnumerable<int> CategoryIds { get; set; } = new List<int>();
        public IEnumerable<int> GradeIds { get; set; } = new List<int>();
        public IEnumerable<int> ExcludedVirtualTestIds { get; set; } = new List<int>();
    }

    public class GetTestPaging
    {
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
