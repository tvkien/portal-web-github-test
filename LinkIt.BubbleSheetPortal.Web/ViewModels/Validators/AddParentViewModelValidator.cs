using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class AddParentViewModelValidator : ManageUsersBaseViewModelValidator<CreateParentViewModel>
    {
        public AddParentViewModelValidator(IRepository<User> repository, string passwordRegex, string passwordMessage)
            : base(repository)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.DistrictId)
                .NotEmpty()
                .WithMessage("Please select a " + LabelHelper.DistrictLabel + ".");

            RuleFor(x => x.UserName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Username is required.")
                .Must(NotBePublisherName())
                .WithMessage("Username already exists.")
                .Must(BeUniqueToDistrict())
                .WithMessage("Username already exists in the selected district.");

            //RuleFor(x => x.Password)
            //    .NotNull()
            //    .NotEmpty()
            //    .Matches(passwordRegex)
            //    .WithMessage(passwordMessage);

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.PhoneNumber)
                .NotNull();

            RuleFor(x => x.EmailAddress)
                .NotNull()
                .EmailAddress()
                .When(x => x.EmailAddress != string.Empty);

            RuleFor(x => x.MessageNumber)
                .NotNull();
        }

        public override Func<CreateParentViewModel, string, bool> BeUniqueToDistrict()
        {
            return (model, username) => repository
            .Select()
            .FirstOrDefault(x => x.DistrictId.Equals(model.DistrictId) && x.UserName.Equals(username) &&
                x.RoleId == (int)Permissions.Parent)
            .IsNull();
        }
    }
}
