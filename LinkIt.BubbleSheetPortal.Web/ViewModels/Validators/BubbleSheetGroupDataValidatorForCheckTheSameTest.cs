using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class BubbleSheetGroupDataValidatorForCheckTheSameTest: AbstractValidator<BubbleSheetGroupData>
    {
        public BubbleSheetGroupDataValidatorForCheckTheSameTest()
        {
            RuleFor(x => x.TestId)
                .GreaterThan(0)
                .WithMessage("Please select a Test.");

            RuleFor(x => x.GroupId)
                .GreaterThan(0)
                .When(x => !x.IsGenericBubbleSheet)
                .WithMessage("Please select a Group.");
        }
    }
}
