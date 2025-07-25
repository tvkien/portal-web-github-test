using Envoc.Core.Shared.Data;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;


namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class NewACTReportDataValidator : AbstractValidator<NewACTReportData>
    {
        public NewACTReportDataValidator()
        {
            //RuleFor(x => x.TestId)
            //    .GreaterThan(0)
            //    .WithMessage("Please select a test.");

            //RuleFor(x => x.StudentIdList.Count)
            //    .GreaterThan(0)
            //    .WithMessage("Please select at least one student.");

            //RuleFor(x => x.TeacherId)
            //    .GreaterThan(0)
            //    .WithMessage("Please select a teacher.");

            //RuleFor(x => x.ClassId)
            //    .GreaterThan(0)
            //    .WithMessage("Please select a class.");
        }
    }
}