using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class AddStudentViewModelValidator : AbstractValidator<AddEditStudentViewModel>
    {
        private readonly IRepository<Student> repository;

        public AddStudentViewModelValidator(IRepository<Student> repository)
        {
            this.repository = repository;

            RuleFor(x => x.AdminSchoolId)
                .NotNull()                
                .WithMessage("Please select an admin school.");

            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty()
                .Length(0, 100);

            RuleFor(x => x.MiddleName)
                .Length(0, 100);

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty()
                .Length(0, 100);

            RuleFor(x => x.StudentLocalId)
                .NotNull()
                .NotEmpty()
                .Length(0, 50)
                .Must(LocalIdNotPreviouslyUsedInDistrict())
                .WithMessage("That Local ID is already in use in this " + LabelHelper.DistrictLabel + " by student {0}.", x => x.StudentNameWithSameLocalId);

            RuleFor(x => x.StudentStateId)
                .NotNull()
                .Length(0, 50)
                .Must(StateIdNotPreviouslyUsedInDistrict())
                .When(x => !string.IsNullOrEmpty(x.StudentStateId))
                .WithMessage("That State ID is already in use in this " + LabelHelper.DistrictLabel + " by student {0}", x => x.StudentNameWithSameStateId);

            RuleFor(x => x.GenderId)
                .GreaterThan(0)
                .WithMessage("Please select a gender.");

            //RuleFor(x => x.RaceId)
            //    .GreaterThan(0)
            //    .WithMessage("Please select a race.");

            RuleFor(x => x.Password)
                .NotNull()
                .Length(0, 100);

            RuleFor(x => x.ConfirmPassword)
                .NotNull()
                .Length(0, 100)
                .Must(MatchPassword())
                .WithMessage("Password and Confirm Password must match.");
        }

        private Func<AddEditStudentViewModel, string, bool> LocalIdNotPreviouslyUsedInDistrict()
        {
            return (model, studentLocalId) =>
            {
                var sameStudent = repository.Select().FirstOrDefault(x => x.Code.Equals(studentLocalId) && x.DistrictId.Equals(model.DistrictId) && x.Id != model.StudentId);

                if (sameStudent != null)
                {
                    model.StudentNameWithSameLocalId = CreateStudentNameLink(sameStudent);
                }

                return sameStudent == null;
            };
        }

        private Func<AddEditStudentViewModel, string, bool> StateIdNotPreviouslyUsedInDistrict()
        {
            return (model, studentStateId) =>
            {
                var sameStudent = repository.Select().FirstOrDefault(x => x.AltCode.Equals(studentStateId) && x.DistrictId.Equals(model.DistrictId) && x.Id != model.StudentId);

                if (sameStudent != null)
                {
                    model.StudentNameWithSameStateId = CreateStudentNameLink(sameStudent);
                }

                return sameStudent == null;
            };
        }

        private string CreateStudentNameLink(Student sameStudent)
        {
            return string.Format("<a href='/ManageClasses/EditStudent/{0}'>{1} {2}</a>", sameStudent.Id, sameStudent.FirstName, sameStudent.LastName);
        }

        private Func<AddEditStudentViewModel, string, bool> MatchPassword()
        {
            return (model, confirmPassword) => confirmPassword.Equals(model.Password);
        }
    }
}