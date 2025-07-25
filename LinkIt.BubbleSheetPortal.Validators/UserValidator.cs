using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.HashedPassword)
                .NotEmpty();
        }
    }
}