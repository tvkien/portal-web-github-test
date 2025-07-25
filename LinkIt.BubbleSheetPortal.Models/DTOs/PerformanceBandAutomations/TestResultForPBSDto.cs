using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.PerformanceBandAutomations
{
    public class TestResultForPBSDto
    {
        public IEnumerable<int> SchoolIds { get; set; }
        public IEnumerable<int?> TeacherIds { get; set; }
        public IEnumerable<int> ClassIds { get; set; }
    }
}
