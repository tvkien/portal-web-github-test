using System.Collections.Generic;
using System.Web.Mvc;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class AccountInformationViewModel : ValidatableEntity<AccountInformationViewModel>
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string RedirectUrl { get; set; }
        public string Email { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool HasEmailAddress { get; set; }
        public bool HasSecurityQuestion { get; set; }
        public bool HasTemporaryPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public List<SelectListItem> Questions { get; set; }

        public bool ShowDisclaimerContent { get; set; }
        public string DisclaimerContent { get; set; }
        public string DisclaimerCheckboxLabel { get; set; }
        public bool TermOfUse { get; set; }
        public string ChangePasswordToken { get; set; }
    }
}
