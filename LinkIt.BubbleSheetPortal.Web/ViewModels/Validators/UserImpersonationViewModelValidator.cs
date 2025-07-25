using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class UserImpersonationViewModelValidator : AbstractValidator<UserImpersonationViewModel>
    {
        public UserImpersonationViewModelValidator()
        {
            RuleFor(x => x.RoleId)
                .GreaterThan(0)
                .WithMessage("Please select a Role.");

            RuleFor(x => x.StateId)
                .GreaterThan(0)
                .WithMessage("Please select a State.");

            RuleFor(x => x.DistrictId)
                .GreaterThan(0)
                .WithMessage("Please select a " + LabelHelper.DistrictLabel + ".");

            //RuleFor(x => x.UserName)
            //    .NotEmpty()
            //    .WithMessage("Please select a UserName.")
            //    .Length(1, 255).
            //    WithMessage("User Name cannot exceed 255 characters.");

            RuleFor(x => x.MemberDistrictId)
                .GreaterThan(0)
                .WithMessage("Please select a " + LabelHelper.DistrictLabel + " member.")
                .When(x =>x.RoleId.Equals((int)Permissions.NetworkAdmin));
        }
    }
}
