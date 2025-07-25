using LinkIt.BubbleSheetPortal.Models.UserGuide;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.UserGuide
{
    public class UserGuideModel
    {
        public UserSecurityCodeData SecurityCode { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; }
    }
}