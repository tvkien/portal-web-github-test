using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Data;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.ViewModels.GenericSheets;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class GenericSheetNewStudentViewModelValidator: AbstractValidator<GenericSheetNewStudentViewModel>
    {
        private readonly IRepository<Student> repository;
        public GenericSheetNewStudentViewModelValidator(IRepository<Student> repository)
        {
            this.repository = repository;

            RuleFor(x => x.SchoolId)
                .NotNull()
                .WithMessage("Invalid school.");

            RuleFor(x => x.ClassId)
                .NotNull()
                .WithMessage("Invalid Class.");

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
                .WithMessage("That Local ID is already in use in this " + LabelHelper.DistrictLabel + " by other student ");

            RuleFor(x => x.StudentStateId)
                .NotNull()
                .Length(0, 50)
                .Must(StateIdNotPreviouslyUsedInDistrict())
                .When(x => !string.IsNullOrEmpty(x.StudentStateId))
                .WithMessage("That State ID is already in use in this " + LabelHelper.DistrictLabel + " by other student ");

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

        private Func<GenericSheetNewStudentViewModel, string, bool> LocalIdNotPreviouslyUsedInDistrict()
        {
            return (model, studentLocalId) =>
            {
                var sameStudent = repository.Select().FirstOrDefault(x => x.Code.Equals(studentLocalId) && x.DistrictId.Equals(model.DistrictId) && x.Id != model.StudentId);
                return sameStudent == null;
            };
        }

        private Func<GenericSheetNewStudentViewModel, string, bool> StateIdNotPreviouslyUsedInDistrict()
        {
            return (model, studentStateId) =>
            {
                var sameStudent = repository.Select().FirstOrDefault(x => x.AltCode.Equals(studentStateId) && x.DistrictId.Equals(model.DistrictId) && x.Id != model.StudentId);
                return sameStudent == null;
            };
        }

        private Func<GenericSheetNewStudentViewModel, string, bool> MatchPassword()
        {
            return (model, confirmPassword) => confirmPassword.Equals(model.Password);
        }
    }
}