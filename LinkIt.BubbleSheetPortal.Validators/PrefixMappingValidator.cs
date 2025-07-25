using FluentValidation;
using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class PrefixMappingValidator : AbstractValidator<PrefixMapping>
    {
        public PrefixMappingValidator()
        {
            RuleFor(x => x.Source)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.SourcePosition)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Destination)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.DestinationColumnID)
                .GreaterThan(0);

            RuleFor(x => x.Prefix)
                .NotNull()
                .NotEmpty();
        }
    }
}