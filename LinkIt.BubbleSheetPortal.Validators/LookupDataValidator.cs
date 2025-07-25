using FluentValidation;
using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class LookupDataValidator : AbstractValidator<LookupData>
    {
        public LookupDataValidator()
        {
            RuleFor(x => x.New)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Existing)
                .NotNull()
                .NotEmpty();
        }
    }
}