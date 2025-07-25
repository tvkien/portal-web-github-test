using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class PrintingGroupValidator : AbstractValidator<PrintingGroup>
    {
        public PrintingGroupValidator()
        {
            RuleFor(x => x.Id)
                .NotNull();                

            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}