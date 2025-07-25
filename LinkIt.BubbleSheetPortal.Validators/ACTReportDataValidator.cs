using System.Threading;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class ACTReportDataValidator : AbstractValidator<ACTReportData>
    {
        public ACTReportDataValidator()
        {
            RuleFor(x => x.TestId)
                .GreaterThan(0)
                .WithMessage("Please select a test.");

            RuleFor(x => x.StudentIdList.Count)
                .GreaterThan(0)
                .WithMessage("Please select at least one student.");

            RuleFor(x => x.TeacherId)
                .GreaterThan(0)
                .WithMessage("Please select a teacher.");

            RuleFor(x => x.ClassId)
                .GreaterThan(0)
                .WithMessage("Please select a class.");
        }
    }
}