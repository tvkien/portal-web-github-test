using FluentValidation;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class AccountInformationViewModelValidator : AbstractValidator<AccountInformationViewModel>
    {
        public AccountInformationViewModelValidator(string passwordRegex, string passwordMessage)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .When(x => x.HasEmailAddress.Equals(false));

            RuleFor(x => x.Answer)
                .NotEmpty()
                .NotNull()
                .When(x => x.HasSecurityQuestion.Equals(false));

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .Matches(passwordRegex)
                .When(x => x.HasTemporaryPassword.Equals(true))
                .WithMessage(passwordMessage);
                
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .When(x => x.HasTemporaryPassword.Equals(true))
                .WithMessage("Passwords must match");
        }
    }
}