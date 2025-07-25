using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class AuthorGroupValidator : AbstractValidator<AuthorGroup>
    {
        public AuthorGroupValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(0, 64);
        }
    }
}