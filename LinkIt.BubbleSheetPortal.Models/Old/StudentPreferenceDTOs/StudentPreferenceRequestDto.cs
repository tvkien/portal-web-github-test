namespace LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs
{
    public class StudentPreferenceRequestDto : PaggingInfo
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int DistrictID { get; set; }
        public int SchoolID { get; set; }
        public string Level { get; set; }
        public string VirtualTestTypeIds { get; set; }
        public string GradeIDs { get; set; }
        public string SubjectIDs { get; set; }
        public int Visibilities { get; set; }
        public string GeneralSearch { get; set; }
        public string ClassIds { get; set; }
        public string ExcludeTestTypes { get; set; }
    }
}
