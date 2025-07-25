namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentGenderGrade
    {
        public int StudentId { get; set; }
        public int? Status { get; set; }
        public int DistrictId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public string Gender { get; set; }
        public string Grade { get; set; }
        public int? AdminSchoolID { get; set; }
    }
}