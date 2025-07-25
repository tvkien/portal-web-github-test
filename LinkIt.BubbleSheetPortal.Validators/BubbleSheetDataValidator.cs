using System;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class BubbleSheetDataValidator : AbstractValidator<BubbleSheetData>
    {
        private readonly IVirtualTestRepository _virtualTestRepository; 
        public BubbleSheetDataValidator(IVirtualTestRepository virtualTestRepository, LabelService labelService)
        {
            _virtualTestRepository = virtualTestRepository;
            RuleFor(x => x.StateId)
                .GreaterThan(0)
                .When(IsPublisher())
                .WithMessage("Please select a state.");

            RuleFor(x => x.DistrictId)
                .GreaterThan(0)
                .When(IsPublisher())
                .WithMessage("Please select a " + labelService.DistrictLabel + ".");

            RuleFor(x => x.SchoolId)
                .GreaterThan(0)
                .When(IsAdmin())
                .WithMessage("Please select a school.");

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

            RuleFor(x => x.DistrictTermId)
                .GreaterThan(0)
                .WithMessage("Please select a term.");

            RuleFor(x => x.ClassId)
                .GreaterThan(0)
                .WithMessage("Please select a class.");

            RuleFor(x => x.StudentIdList.Count)
                .GreaterThan(0)
                .When(BubbleSheetIsNotGeneric())
                .WithMessage("Please select at least one student.");

            RuleFor(x => x.SheetStyleId)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Please select a sheet style.");

            RuleFor(x => x.BubbleSizeId)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Please select a bubble size.");

            RuleFor(x => x.NumberOfGenericSheet)
                .GreaterThan(0)
                .When(IsGenericSatActTest())
                .WithMessage("Invalid Number of Generic Sheets");


        }

        private Func<BubbleSheetData, bool> IsGenericSatActTest()
        {
            return x => 
            {
                if (x.TestId > 0)
                {
                    var virtualTest = _virtualTestRepository.GetVirtualTestByID(x.TestId);
                    return virtualTest != null &&
                           virtualTest.VirtualTestSubTypeID.HasValue &&
                           (virtualTest.VirtualTestSubTypeID.Value == (int) VirtualTestSubType.ACT ||
                            virtualTest.VirtualTestSubTypeID.Value == (int) VirtualTestSubType.SAT)
                            && x.IsGenericBubbleSheet;
                }
                return false;
            };
        }

        private Func<BubbleSheetData, bool> BubbleSheetIsNotGeneric()
        {
            return x => !x.IsGenericBubbleSheet;
        }

        private Func<BubbleSheetData, bool> IsPublisher()
        {
            return x => x.IsPublisher;
        }

        private Func<BubbleSheetData, bool> IsAdmin()
        {
            return x => x.IsAdmin;
        }
    }
}