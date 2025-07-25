using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.Models.DataTable
{
    public class GetGroupUserDataTableCriteria : GenericDataTableRequest
    {
        public int? DistrictID { get; set; }
        public int? UserID { get; set; }
        public int? RoleID { get; set; }
        public bool IsShowInactiveUser { get; set; }
        public int? XLIGroupID { get; set; }
    }

    public class GetUserAddToGroupDataTableCriteria : GenericDataTableRequest
    {
        public int? DistrictID { get; set; }
        public int GroupID { get; set; }
        public int? SchoolID { get; set; }
        public int? RoleID { get; set; }
        public bool IsShowInactiveUser { get; set; }
    }

    public class GetUserGroupManagementRequest : GenericDataTableRequest
    {
        public int? DistrictID { get; set; }
    }

    public class GetStudentsInDistrictByFilterRequest : GenericDataTableRequest
    {
        public int ClassId { get; set; }

        public string ProgramIdList { get; set; }

        public string GradeIdList { get; set; }
    }
}
