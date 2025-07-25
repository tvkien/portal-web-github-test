using FluentValidation;
using LinkIt.BubbleSheetPortal.Web.ViewModels.GenericSheets;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class AssignGenericSheetViewModelValidator : AbstractValidator<AssignGenericSheetViewModel>
    {
        public AssignGenericSheetViewModelValidator()
        {
            RuleFor(x => x.BubbleSheetFileId)
                .GreaterThan(0)
                .WithMessage("Invalid BubbleSheetFile.");

            RuleFor(x => x.SelectedRemainingStudentsId)
                .GreaterThan(0)
                .When(x => !x.IsAllStudentsChecked)
                .WithMessage("Please select a student.");

            RuleFor(x => x.SelectedAllStudentsId)
                .GreaterThan(0)
                .When(x => x.IsAllStudentsChecked)
                .WithMessage("Please select a student.");
        }
    }
}