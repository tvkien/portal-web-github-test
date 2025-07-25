namespace LinkIt.BubbleSheetPortal.Models
{
    public class AddModulePermissionRequest
    {
        public int XLIAreaId { get; set; }
        public int XLIGroupId { get; set; }
        public int XLIModuleId { get; set; }
        public string DisplayHeader { get; set; }
        public bool IsNotSupportSchoolAdminAndTeacher { get; set; }
        public bool AllRoles { get; set; }
        public bool NetworkAdmin { get; set; }
        public bool DistrictAdmin { get; set; }
        public bool SchoolAdmin { get; set; }
        public bool Teacher { get; set; }
    }
}
