using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class TestAssignmentDataValidator : AbstractValidator<TestAssignmentData>
    {
        public TestAssignmentDataValidator(LabelService labelService)
        {
            RuleFor(x => x.StateId)
                .GreaterThan(0)
                .When(IsPublisher())
                .WithMessage("Please select a state.");

            RuleFor(x => x.DistrictId)
                .GreaterThan(0)
                .When(IsPublisher())
                .WithMessage("Please select a " + labelService.DistrictLabel + ".");
            
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

            RuleFor(x => x.SchoolId)
                .GreaterThan(0)
                .When(IsAdmin())
                .WithMessage("Please select a school.");

            RuleFor(x => x.DistrictTermId)
                .GreaterThan(0)
                .When(NotAssignMultiClass())
                .WithMessage("Please select a term.");

            RuleFor(x => x.ClassId)
                .GreaterThan(0)
                .When(NotAssignMultiClass())
                .WithMessage("Please select a class.");

            RuleFor(x => x.StudentIdList.Count)
                .GreaterThan(0)
                .When(AssignStudent())
                .WithMessage("Please select at least one student.");

            RuleFor(x => x.GroupId)
                .GreaterThan(0)
                .When(AssignMultiClass())
                .WithMessage("Please select a Group.");
        }

        private Func<TestAssignmentData, bool> AssignStudent()
        {
            return x => x.AssignmentType <= 2  && x.IsUseRoster == false;
        }
        
        private Func<TestAssignmentData, bool> AssignMultiClass()
        {
            return x => x.AssignmentType >= 3 && x.AssignmentType < 5;
        }

        private Func<TestAssignmentData, bool> NotAssignMultiClass()
        {
            return x => x.AssignmentType < 3;
        }

        private Func<TestAssignmentData, bool> IsPublisher()
        {
            return x => x.IsPublisher;
        }

        private Func<TestAssignmentData, bool> IsAdmin()
        {
            return x => x.IsAdmin && x.AssignmentType < 3;
        }
    }
}
