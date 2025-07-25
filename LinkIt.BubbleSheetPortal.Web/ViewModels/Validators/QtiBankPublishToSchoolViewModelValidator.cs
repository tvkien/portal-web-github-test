using Envoc.Core.Shared.Data;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;


namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class QtiBankPublishToSchoolViewModelValidator : AbstractValidator<QtiBankPublishToSchoolViewModel>
    {
        public QtiBankPublishToSchoolViewModelValidator() 
        {
            RuleFor(x => x.SchoolId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("School is required.");
        }
    }
}