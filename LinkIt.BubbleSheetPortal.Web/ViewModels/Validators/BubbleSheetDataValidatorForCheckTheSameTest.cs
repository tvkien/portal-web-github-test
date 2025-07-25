using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class BubbleSheetDataValidatorForCheckTheSameTest: AbstractValidator<BubbleSheetData>
    {
        public BubbleSheetDataValidatorForCheckTheSameTest()
        {
            RuleFor(x => x.TestId)
                .GreaterThan(0)
                .WithMessage("Please select a Test.");

            RuleFor(x => x.ClassId)
                .GreaterThan(0)
                .When(x => !x.IsGenericBubbleSheet)
                .WithMessage("Please select a Class");

            RuleFor(x => x.StudentIdList)
                .NotEmpty()
                .When(x => !x.IsGenericBubbleSheet)
                .WithMessage("Please select at least 1 Student");
        }
    }
}
