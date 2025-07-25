using FluentValidation;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class AddClassViewModelValidator : AbstractValidator<AddClassViewModel>
    {
        public AddClassViewModelValidator()
        {
            RuleFor(x => x.Course)
                .NotNull()
                .NotEmpty()
                .Length(0, 49);

            RuleFor(x => x.DistrictTermId)
                .GreaterThan(0)
                .WithMessage("Please select a Term");

            RuleFor(x => x.ClassTypeId)
                .GreaterThan(0)
                .WithMessage("Please select a Class Type.");

            RuleFor(x => x.SchoolId)
                .GreaterThan(0)
                .WithMessage("School is required.");

            RuleFor(x => x.TeacherId)
                .GreaterThan(0)
                .WithMessage("Teacher is required.");
        }
    }
}
