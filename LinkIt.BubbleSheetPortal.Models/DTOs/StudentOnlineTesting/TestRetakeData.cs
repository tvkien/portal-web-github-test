using LinkIt.BubbleSheetPortal.Models.ManagePreference;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting
{
    public class TestRetakeData
    {
        public List<TestRetakeDataStudentInfo> StudentList { get; set; }
        public int OriginalTestId { get; set; }
        public int TestId { get; set; }        
        public int DistrictId { get; set; }        
        public TestPreferenceModel TestPreferenceModel { get; set; }

        public string TestName { get; set; }
        public string SubjectName { get; set; }
        public string GradeName { get; set; }
        public int TestRetakeNumber { get; set; }
        public string RetakeType { get; set; }
        public string GUID { get; set; }
    }
    public class TestRetakeDataStudentInfo
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
    }
}
