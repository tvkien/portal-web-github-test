namespace LinkIt.BubbleSheetPortal.Web.Models.UserGroup
{
    public class EditUserGroupDto
    {
        public int XLIGroupID { get; set; }

        public string Name { get; set; }

        public int? DistrictID { get; set; }

        public bool InheritRoleFunctionality { get; set; }
    }
}
