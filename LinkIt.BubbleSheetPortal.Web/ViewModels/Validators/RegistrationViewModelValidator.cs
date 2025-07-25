using FluentValidation;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class RegistrationViewModelValidator : AbstractValidator<RegistrationViewModel>
    {
        public RegistrationViewModelValidator(string passwordRegex, string passwordMessage)
        {
            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .Matches(passwordRegex)
                .WithMessage(passwordMessage);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Passwords must match");

            RuleFor(x => x.TermsofUse)
               .Equal(true)
               .WithMessage("You have not agreed to the terms of use");
        }
    }
}