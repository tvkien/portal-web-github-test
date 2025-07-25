using System;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class BubbleSheetGroupDataValidator : AbstractValidator<BubbleSheetGroupData>
    {
        private readonly IVirtualTestRepository _virtualTestRepository;
        public BubbleSheetGroupDataValidator(IVirtualTestRepository virtualTestRepository, LabelService labelService)
        {
            _virtualTestRepository = virtualTestRepository;

            RuleFor(x => x.GradeId)
                .GreaterThan(0)
                .WithMessage("Please select a " + labelService.GradeLabel + ".");

            RuleFor(x => x.SubjectId)
                .GreaterThan(0)
                .WithMessage("Please select a subject.");

            RuleFor(x => x.BankId)
                .GreaterThan(0)
                .WithMessage("Please select a bank.");

            RuleFor(x => x.TestId)
                .GreaterThan(0)
                .WithMessage("Please select a test.");

            RuleFor(x => x.SheetStyleId)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Please select a sheet style.")
                .NotEqual(1)
                .WithMessage("Cannot create roster sheet for group printing job.");

            RuleFor(x => x.BubbleSizeId)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Please select a bubble size.");

            RuleFor(x => x.NumberOfGenericSheet)
               .GreaterThan(0)
               .When(IsGenericSatActTest())
               .WithMessage("Invalid Number of Generic Sheets");
        }

        private Func<BubbleSheetGroupData, bool> IsGenericSatActTest()
        {
            return x =>
            {
                if (x.TestId > 0)
                {
                    var virtualTest = _virtualTestRepository.GetVirtualTestByID(x.TestId);
                    return virtualTest != null &&
                           virtualTest.VirtualTestSubTypeID.HasValue &&
                           (virtualTest.VirtualTestSubTypeID.Value == (int)VirtualTestSubType.ACT ||
                            virtualTest.VirtualTestSubTypeID.Value == (int)VirtualTestSubType.SAT)
                            && x.IsGenericBubbleSheet;
                }
                return false;
            };
        }
    }
}