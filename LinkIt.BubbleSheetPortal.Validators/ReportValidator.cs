using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class ReportValidator : AbstractValidator<Report>
    {
        public ReportValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Report name is required");

            RuleFor(x => x.URL)
                .NotEmpty()
                .WithMessage("Report URL is required");
        }
    }
}