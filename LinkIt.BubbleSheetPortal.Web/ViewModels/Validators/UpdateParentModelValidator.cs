using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using iTextSharp.text.pdf.qrcode;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using LinkIt.BubbleSheetPortal.Models.Enum;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class UpdateParentModelValidator : AbstractValidator<UpdateParentRequestModel>
    {
        private readonly IRepository<User> repository;

        public UpdateParentModelValidator(IRepository<User> repository)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty();
            RuleFor(x => x.UserName)
               .NotNull()
               .NotEmpty()
               .WithMessage("Username/Email is required.")
                .Must(BeUniqueLocalCodeToDistrict())
               .WithMessage("Username/Email already exists.")
               .Must(BeUniqueToDistrict())
               .WithMessage("Username/Email already exists in the selected district.");

            RuleFor(x => x.UserName)
                .EmailAddress()
                .When(x => x.UserName != string.Empty)
                .WithMessage("Username/Email format is incorrect");
            this.repository = repository;
        }
        public Func<UpdateParentRequestModel, string, bool> BeUniqueToDistrict()
        {
            return (model, username) =>
            {
                var districtId = repository.Select()
                .Where(c => c.Id == model.UserId)
                .Select(c => c.DistrictId)
                .FirstOrDefault();

                return repository.Select()
                .FirstOrDefault(x =>
                    x.Id != model.UserId
                    && x.DistrictId.Equals(districtId)
                    && x.UserName.Equals(username)
                    && x.RoleId == (int)RoleEnum.Parent)
                .IsNull();
            };
        }
        public Func<UpdateParentRequestModel, string, bool> BeUniqueLocalCodeToDistrict()
        {

            return (model, localcode) =>
            {
                var districtId = repository.Select()
                .Where(c => c.Id == model.UserId)
                .Select(c => c.DistrictId)
                .FirstOrDefault();

                return repository.Select()
                .FirstOrDefault(x =>
                 x.Id != model.UserId
                && x.DistrictId.Equals(districtId)
                && x.LocalCode.Equals(localcode)
                ).IsNull();
            };
        }
    }
}
