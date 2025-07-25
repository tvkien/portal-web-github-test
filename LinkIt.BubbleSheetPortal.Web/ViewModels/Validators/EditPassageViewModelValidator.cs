using Envoc.Core.Shared.Data;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class EditPassageViewModelValidator : AbstractValidator<EditPassageViewModel>
    {
        public EditPassageViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage("Name is required.");
        }
    }
}