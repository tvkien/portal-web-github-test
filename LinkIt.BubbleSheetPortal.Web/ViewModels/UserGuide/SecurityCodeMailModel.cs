using LinkIt.BubbleSheetPortal.Models.UserGuide;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.UserGuide
{
    public class SecurityCodeMailModel
    {
        public UserSecurityCodeData SecurityCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}