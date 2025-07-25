using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Linq;


namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class EditParentViewModelValidator : ManageUsersBaseViewModelValidator<EditParentViewModel>
    {
        public EditParentViewModelValidator(IRepository<User> repository, string passwordRegex, string passwordMessage)
            : base(repository)
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("User is required.");

            RuleFor(x => x.DistrictId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("" + LabelHelper.DistrictLabel + " is required.")
                .WithName(LabelHelper.DistrictLabel);

            RuleFor(x => x.UserName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Username is required.")
                .Must(BeUniqueToDistrict())
                .When(ParentUserNameIsChanging(), ApplyConditionTo.CurrentValidator)
                .WithMessage("That username already exists in the selected district.");

            RuleFor(x => x.EmailAddress)
                .NotNull()
                .EmailAddress()
                .When(x => x.EmailAddress != string.Empty);

            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.PhoneNumber)
                .NotNull();

            RuleFor(x => x.MessageNumber)
                .NotNull();

            RuleFor(x => x.Password)
                .NotNull();                

            RuleFor(x => x.ConfirmPassword)
                .Must((x, confirmPassword) => confirmPassword == x.Password)
                .When(x => x.Password != string.Empty && x.ConfirmPassword != string.Empty)
                .WithMessage("Passwords must match");
        }

        public override Func<EditParentViewModel, string, bool> BeUniqueToDistrict()
        {
            return (model, username) => repository
            .Select()
            .FirstOrDefault(x => x.DistrictId.Equals(model.DistrictId) && x.UserName.Equals(username) &&
                x.RoleId == (int)Permissions.Parent)
            .IsNull();
        }
    }
}
