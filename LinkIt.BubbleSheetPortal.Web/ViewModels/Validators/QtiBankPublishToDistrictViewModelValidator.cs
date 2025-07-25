using Envoc.Core.Shared.Data;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;


namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class QtiBankPublishToDistrictViewModelValidator : AbstractValidator<QtiBankPublishToDistrictViewModel>
    {
        public QtiBankPublishToDistrictViewModelValidator() 
        {
            RuleFor(x => x.StateId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("State is required.");

            RuleFor(x => x.DistrictId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("" + LabelHelper.DistrictLabel + " is required.")
                .WithName(LabelHelper.DistrictLabel);
        }
    }
}