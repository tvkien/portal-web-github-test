using FluentValidation;
using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class SourceColumnMappingValidator : AbstractValidator<SourceColumnMapping>
    {
        public SourceColumnMappingValidator()
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
        }
    }
}