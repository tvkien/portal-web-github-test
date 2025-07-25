namespace LinkIt.BubbleSheetPortal.Web.ViewModels.AuthorGroup
{
    public class AddAuthorGroupUserSchoolDistrictViewModel
    {
        public bool IsDistrictAdmin { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsTeacher { get; set; }
        public int DistrictId { get; set; }
        public int StateId { get; set; }
        public int AuthorGroupId { get; set; }
        public int SchoolId { get; set; }
        public string GroupName { get; set; }
        public bool CanEditGroup { get; set; }
        public bool IsNetworkAdmin { get; set; }
    }
}