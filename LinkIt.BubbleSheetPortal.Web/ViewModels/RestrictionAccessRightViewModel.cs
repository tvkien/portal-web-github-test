namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class RestrictionAccessRightViewModel
    {
        public bool IsDistrictAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public int DistrictId { get; set; }
        public int StateId { get; set; }
        public int CurrentUserId { get; set; }
        public bool IsNetworkAdmin { get; set; }
        public int UserRoleId { get; set; }
    }
}
