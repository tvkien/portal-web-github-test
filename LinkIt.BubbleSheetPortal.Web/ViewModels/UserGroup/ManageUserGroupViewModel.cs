
namespace LinkIt.BubbleSheetPortal.Web.ViewModels.UserGroup
{
    public class ManageUserGroupViewModel
    {
        public int CurrentUserId { get; set; }

        public bool IsPublisher { get; set; }

        public bool IsNetworkAdmin { get; set; }

        public int XLIGroupID;

        public string Name;

        public int? DistrictID;

        public bool? InheritRoleFunctionality;
    }
}
