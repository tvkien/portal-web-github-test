using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class EditUserViewModelValidator : ManageUsersBaseViewModelValidator<EditUserViewModel>
    {
        public EditUserViewModelValidator(IRepository<User> repository) : base(repository)
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
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
                .Must(BeUniqueGlobally())
                .When(CreatingEditingPublisher, ApplyConditionTo.CurrentValidator)
                .When(UserNameIsChanging(), ApplyConditionTo.CurrentValidator)
                .WithMessage("That username already exists.")
                .Must(NotBePublisherName())
                .When(CreatingEditingNonPublisher, ApplyConditionTo.CurrentValidator)
                .When(UserNameIsChanging(), ApplyConditionTo.CurrentValidator)
                .WithMessage("That username already exists.")
                .Must(BeUniqueToDistrict())
                .When(CreatingEditingNonPublisher, ApplyConditionTo.CurrentValidator)
                .When(UserNameIsChanging(), ApplyConditionTo.CurrentValidator)
                .WithMessage("That username already exists in the selected district.");

            RuleFor(x => x.EmailAddress)
                .NotNull()
                .EmailAddress()
                .When(x => x.EmailAddress != string.Empty);
            //Remove check FirstName,LastName and LocalCode here because there are two messages not one

            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty()
                .WithMessage("First Name is required.");

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Last Name is required.");

            RuleFor(x => x.PhoneNumber)
                .NotNull();

            RuleFor(x => x.LocalCode)
                .NotNull()
                .NotEmpty()
                .WithMessage("Local Code is required.")
                .Must(BeUniqueLocalCodeToDistrict())
                .When(CreatingNonPublisher, ApplyConditionTo.CurrentValidator)
                .When(LocalCodeIsChanging(), ApplyConditionTo.CurrentValidator)
                .WithMessage("User code already exists in the selected district.")
                .Must(BeUniqueLocalCodePublisher())
                .When(CreatingPublisher, ApplyConditionTo.CurrentValidator)
                .When(LocalCodeIsChanging(), ApplyConditionTo.CurrentValidator)
                .WithMessage("User code already exists.");

            RuleFor(x => x.RoleId)
                .NotNull()
                .Must(CanAccessUserRole())
                .WithMessage("Invalid User Role.");

            RuleFor(x => x.StateCode)
                .NotNull();
            
        }

        private Func<EditUserViewModel, int, bool> CanAccessUserRole()
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

        public override Func<EditUserViewModel, string, bool> BeUniqueToDistrict()
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
