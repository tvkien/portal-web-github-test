using System;
using FluentValidation;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class AddOrEditTermViewModelValidator : AbstractValidator<AddEditTermViewModel>
    {
        public AddOrEditTermViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage("Name is required.")
                .Length(0, 100)
                .WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.DateStart)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("Start Date is required.")
                .Must(StartDateGreaterThanEndDate)
                .WithMessage("Start Date must before then End Date.");

            RuleFor(x => x.DateEnd)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage("End Date is required.");
            //.Must(EndDateLessThanStartDate)
            //.WithMessage("EndDate must after then StartDate.");
        }

        private bool StartDateGreaterThanEndDate(AddEditTermViewModel viewModel, DateTime? startDate)
        { 
            return !(startDate.HasValue && viewModel.DateEnd.HasValue) || startDate.Value <= viewModel.DateEnd.Value;
        }

        private bool EndDateLessThanStartDate(AddEditTermViewModel viewModel, DateTime? endDate)
        {
            return !(endDate.HasValue && viewModel.DateStart.HasValue) || endDate.Value >= viewModel.DateStart.Value;
        }
    }
}