using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class MappingInformationValidator : AbstractValidator<MappingInformation>
    {
        public MappingInformationValidator()
        {
            RuleFor(x => x.MapID)
                .NotNull();

            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}