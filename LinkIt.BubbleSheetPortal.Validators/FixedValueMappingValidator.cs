using FluentValidation;
using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class FixedValueMappingValidator : AbstractValidator<FixedValueMapping>
    {
        public FixedValueMappingValidator()
        {
            RuleFor(x => x.Destination)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.DestinationColumnID)
                .GreaterThan(0);

            RuleFor(x => x.Value)
                .NotNull()
                .NotEmpty();
        }
    }
}