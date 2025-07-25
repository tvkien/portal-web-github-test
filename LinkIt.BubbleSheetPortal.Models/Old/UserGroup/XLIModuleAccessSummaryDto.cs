namespace LinkIt.BubbleSheetPortal.Models.Old.UserGroup
{
    public class XLIModuleAccessSummaryDto
    {
        public int ModuleID { get; set; }

        public string ModuleCode { get; set; }

        public string ModuleName { get; set; }

        public int AreaID { get; set; }

        public string AreaCode { get; set; }

        public string AreaName { get; set; }

        public string DistrictAccess { get; set; }

        public string SchoolAccess { get; set; }

        public string UserGroupWithAccess { get; set; }

        public string UserGroupWithoutAccess { get; set; }
    }
}
