using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class ClassValidator : AbstractValidator<Class>
    {
        public ClassValidator()
        {
            RuleFor(x => x.Course)
                .NotNull()
                .NotEmpty()
                .Length(0, 49);
        }
    }
}