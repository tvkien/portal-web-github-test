using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.TestMaker;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class NotificationMessageControllerParameters
    {
        public NotificationMessageService NotificationMessageService { get; set; }
        public UserMetaService UserMetaService { get; set; }        

        public IFormsAuthenticationService FormsAuthenticationService { get; set; }
    }
}