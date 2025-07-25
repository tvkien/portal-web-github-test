using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using LinkIt.BubbleSheetPortal.Models.Enum;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class CreateParentModelValidator : AbstractValidator<CreateParentRequestModel>
    {
        private readonly IRepository<User> repository;

        public CreateParentModelValidator(IRepository<User> repository)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.StateId)
                .NotEmpty()
                .WithMessage("Please select a state.");

            RuleFor(x => x.DistrictId)
                .NotEmpty()
                .WithMessage("Please select a " + LabelHelper.DistrictLabel + ".");



            RuleFor(x => x.UserName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Username/Email is required.")
                 .Must(BeUniqueLocalCodeToDistrict())
                .WithMessage("Username/Email already exists.")
                .Must(BeUniqueToDistrict())
                .WithMessage("Username/Email already exists in the selected district.");

            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.UserName)
                .EmailAddress()
                .When(x => x.UserName != string.Empty)
                .WithMessage("Username/Email format is incorrect");
            this.repository = repository;
        }


        public Func<CreateParentRequestModel, string, bool> BeUniqueToDistrict()
        {
            return (model, username) => repository.Select()
                .FirstOrDefault(x =>
                    x.DistrictId.Equals(model.DistrictId)
                    && x.UserName.Equals(username)
                    && x.RoleId == (int)RoleEnum.Parent)
                .IsNull();
        }
        public Func<CreateParentRequestModel, string, bool> BeUniqueLocalCodeToDistrict()
        {
            return (model, localcode) => repository.Select().FirstOrDefault(x => x.DistrictId.Equals(model.DistrictId) && x.LocalCode.Equals(localcode)).IsNull();
        }
    }
}
