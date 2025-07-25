using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class ClassUserValidator : AbstractValidator<ClassUser>
    {
        private readonly IRepository<ClassUser> classUserRepository;

        public ClassUserValidator(IRepository<ClassUser> classUserRepository)
        {
            this.classUserRepository = classUserRepository;

            RuleFor(x => x.ClassId)
                .GreaterThan(0)
                .WithMessage("Class is required.");

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("Teacher is required.")
                .Must(NotAlreadyBeAssigned())
                .WithMessage("That teacher has already been assigned a role in this class.");

            RuleFor(x => x.ClassUserLOEId)
                .GreaterThan(0)
                .WithMessage("Level of Engagement is required.")
                .Must(NotAlreadyHaveAPrimaryTeacher())
                .WithMessage("There is already a primary teacher for this class.");
        }

        private Func<ClassUser, int, bool> NotAlreadyBeAssigned()
        {
            return (classUser, y) => !classUserRepository.Select().Any(x => x.ClassId.Equals(classUser.ClassId) && x.UserId.Equals(classUser.UserId));
        }

        private Func<ClassUser, int?, bool> NotAlreadyHaveAPrimaryTeacher()
        {
            return (classUser, selectedLOEId) => !(classUserRepository.Select().Any(x => x.ClassId == classUser.ClassId && x.ClassUserLOEId == 1) && selectedLOEId == 1);
        }
    }
}
