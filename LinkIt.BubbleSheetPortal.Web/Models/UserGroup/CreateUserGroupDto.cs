namespace LinkIt.BubbleSheetPortal.Web.Models.UserGroup
{
    public class CreateUserGroupDto
    {
        public string Name { get; set; }

        public int? DistrictID { get; set; }

        public bool InheritRoleFunctionality { get; set; }
    }
}
