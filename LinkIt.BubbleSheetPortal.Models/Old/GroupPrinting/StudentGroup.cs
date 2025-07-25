namespace LinkIt.BubbleSheetPortal.Models.GroupPrinting
{
    public class StudentGroup
    {
        public int GroupId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int? TeacherId { get; set; }
        public string StudentName { get; set; }
        public string Code { get; set; }
        public string TeacherName { get; set; }
        public string ClassName { get; set; }
        public string SchoolName { get; set; }
        public string DistrictName { get; set; }
        public int SchoolId { get; set; }
        public int? DistrictTermId { get; set; }
        public int DistrictId { get; set; }
    }
}