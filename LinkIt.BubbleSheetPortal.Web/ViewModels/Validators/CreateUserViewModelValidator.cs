using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class CreateUserViewModelValidator : ManageUsersBaseViewModelValidator<CreateUserViewModel>
    {
        public CreateUserViewModelValidator(IRepository<User> repository, string passwordRegex, string passwordMessage) : base(repository)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("Role is required.");

            RuleFor(x => x.StateId)
                .NotEmpty()
                .When(RequiresState)
                .WithMessage("Please select a state.");

            RuleFor(x => x.DistrictId)
                .NotEmpty()
                .When(RequiresDistrict)
                .WithMessage("Please select a " + LabelHelper.DistrictLabel + ".");

            RuleFor(x => x.SchoolId)
                .NotEmpty()
                .When(RequiresSchool)
                .WithMessage("Please select a school.");

            RuleFor(x => x.UserName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Username is required.")
                .Must(BeUniqueGlobally())
                .When(CreatingEditingPublisher, ApplyConditionTo.CurrentValidator)
                .WithMessage("Username already exists.")
                .Must(NotBePublisherName())
                .When(CreatingEditingNonPublisher, ApplyConditionTo.CurrentValidator)
                .WithMessage("Username already exists.")
                .Must(BeUniqueToDistrict())
                .When(CreatingEditingNonPublisher, ApplyConditionTo.CurrentValidator)
                .WithMessage("Username already exists in the selected district.");

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .Matches(passwordRegex)
                .WithMessage(passwordMessage);

            RuleFor(x => x.ConfirmPassword)
                .NotNull()
                .NotEmpty()
                .Equal(x => x.Password)
                .WithMessage("Passwords must match.");

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

            RuleFor(x => x.LocalCode)
                .NotNull()
                .NotEmpty()
                .WithMessage("Local Code is required.")
                .Must(BeUniqueLocalCodeToDistrict())
                .When(CreatingNonPublisher, ApplyConditionTo.CurrentValidator)
                .WithMessage("User code already exists in the selected district.")
                .Must(BeUniqueLocalCodePublisher())
                .When(CreatingPublisher, ApplyConditionTo.CurrentValidator)
                .WithMessage("User code already exists.");

            RuleFor(x => x.StateCode)
                .NotNull();

            RuleFor(x => x.RoleId)
                .NotNull()
                .Must(CanAccessUserRole())
                .WithMessage("Invalid User Role.");
        }

        private Func<CreateUserViewModel, int, bool> CanAccessUserRole()
        {
            return (model, roleId) =>
            {
                var lstRoles = new List<int>() { 2, 8 };
                switch (model.CurrentUserRoleId)
                {
                    case (int)Permissions.DistrictAdmin:
                        lstRoles.Add(3);
                        break;
                    case (int)Permissions.Publisher:
                        lstRoles.Add(3);
                        lstRoles.Add(27);
                        lstRoles.Add(5);
                        break;
                    case (int)Permissions.LinkItAdmin:
                        lstRoles.Add(3);
                        lstRoles.Add(27);
                        lstRoles.Add(15);
                        break;
                    case (int)Permissions.NetworkAdmin:
                        lstRoles.Add(3);
                        lstRoles.Add(27);
                        break;
                }
                return lstRoles.Contains(roleId);
            };

        }

        public override Func<CreateUserViewModel, string, bool> BeUniqueToDistrict()
        {
            var listOfRoleIds = new List<int> {
                (int) Permissions.Publisher,
                (int) Permissions.NetworkAdmin,
                (int) Permissions.DistrictAdmin,
                (int) Permissions.SchoolAdmin,
                (int) Permissions.Teacher,
            };
            return (model, username) => repository
            .Select()
            .FirstOrDefault(x => x.DistrictId.Equals(model.DistrictId) && x.UserName.Equals(username) &&
                listOfRoleIds.Contains(x.RoleId))
            .IsNull();
        }
    }
}
