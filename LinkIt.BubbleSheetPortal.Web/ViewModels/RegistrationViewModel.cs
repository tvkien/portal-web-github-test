using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class RegistrationViewModel : ValidatableEntity<RegistrationViewModel>
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public bool TermsofUse { get; set; }

        public string TermsofUseContent { get; set; }
        public string MessageError { get; set; }

        //\
        public string AccessKey { get; set; }

        public string LinkitURL { get; set; }

        public string GoBackURL { get; set; }
    }
}