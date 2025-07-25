using FluentValidation;
using LinkIt.BubbleSheetPortal.Web.ViewModels.GenericSheets;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class AssignGenericSheetActSatViewModelValidator : AbstractValidator<AssignGenericSheetActSatViewModel>
    {
        public AssignGenericSheetActSatViewModelValidator()
        {
            RuleFor(x => x.BubbleSheetFileId)
                .GreaterThan(0)
                .WithMessage("Invalid BubbleSheetFile.");

            RuleFor(x => x.SelectStudent)
                .GreaterThan(0)
                .WithMessage("Please select a student.");

            RuleFor(x => x.SelectClass)
                .GreaterThan(0)
                .WithMessage("Please select a class.");
        }
    }
}