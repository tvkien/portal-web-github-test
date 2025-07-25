using FluentValidation;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class AddClassRosterViewModelValidator : AbstractValidator<AddClassViewModel>
    {
        public AddClassRosterViewModelValidator()
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
        }
    }
}