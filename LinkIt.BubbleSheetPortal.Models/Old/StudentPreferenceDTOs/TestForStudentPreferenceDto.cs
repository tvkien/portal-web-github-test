using System;
namespace LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs
{
    public class TestForStudentPreferenceDto
    {
        public int VirtualTestID { get; set; }
        public string TestName { get; set; }
        public int DataSetCategoryID { get; set; }
        public string DataSetCategory { get; set; }
        public string Subject { get; set; }
        public string Grade { get; set; }
        public int ResultCount { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
