using System.Linq;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class LookupMappingValidator : AbstractValidator<LookupMapping>
    {
        public LookupMappingValidator()
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

            RuleFor(x => x.LookupValue)
                .NotNull();

            RuleFor(x => x.LookupValue.Count)
                .GreaterThan(0);

            RuleFor(x => x.LookupValue
                .Any(d => d.IsValid == false))
                .Equal(false);
        }
    }
}