using FluentValidation;
using LinkIt.BubbleSheetPortal.Web.ViewModels.AuthorGroup;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class AddAuthorGroupViewModelValidator : AbstractValidator<AddAuthorGroupViewModel>
    {
        public AddAuthorGroupViewModelValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            //RuleFor(x => x.StateId)
            //    .NotNull()
            //   .WithMessage("State is required.");

            //RuleFor(x => x.StateId)
            //    .GreaterThan(0)
            //    .WithMessage("State is required.");

            //RuleFor(x => x.DistrictId)
            //    .Must(DistrictIdIsNotNull)
            //    .WithMessage("" + LabelHelper.DistrictLabel + " is required");

            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .Length(0, 64);

            RuleFor(x => x.SchoolId)
                .Must(SchoolIdIsNotNull)
                .WithMessage("School is required.");
        }

        private bool DistrictIdIsNotNull(AddAuthorGroupViewModel viewModel, int? districtId)
        {
            if (viewModel.IsPublisher)
            {
                return true;
            }
            return districtId.HasValue;
        }

        private bool SchoolIdIsNotNull(AddAuthorGroupViewModel viewModel, int? schoolId)
        {
            if (viewModel.IsSchoolAdmin || viewModel.IsTeacher)
            {
                return viewModel.SchoolId.HasValue && viewModel.SchoolId.Value > 0;
            }
            return true;
        }
    }
}