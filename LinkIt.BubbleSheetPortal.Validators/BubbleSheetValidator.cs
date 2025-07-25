using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class BubbleSheetValidator : AbstractValidator<BubbleSheet>
    {
        public BubbleSheetValidator()
        {
            RuleFor(x => x.ClassId)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Invalid class.");
        }
    }
}