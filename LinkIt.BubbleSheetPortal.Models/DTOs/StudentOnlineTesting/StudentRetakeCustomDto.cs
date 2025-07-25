namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting
{
    public class StudentRetakeCustomDto
    {
        public int ClassID { get; set; }
        public int StudentID { get; set; }
        public string FullName { get; set; }
        public int VirtualTestID { get; set; }
        public string VirtualTestName { get; set; }
        public string VirtualTestDisplayName { get; set; }
        public int TestStatus { get; set; }
        public string RetakeType { get; set; }
    }
}
