using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class ManageUsersBaseViewModelValidator<T> : AbstractValidator<T> where T : class, IUserViewModel
    {
        protected readonly IRepository<User> repository; 

        public ManageUsersBaseViewModelValidator(IRepository<User> repository)
        {
            this.repository = repository;
        }

        public virtual Func<T, string, bool> BeUniqueToDistrict()
        {
            return (model, username) => repository.Select().FirstOrDefault(x => x.DistrictId.Equals(model.DistrictId) && x.UserName.Equals(username)).IsNull();
        }
        public Func<T, string, bool> BeUniqueLocalCodeToDistrict()
        {
            List<int> lstStudentParent = new List<int>() {26,28 };
            return (model, localcode) => repository.Select().FirstOrDefault(x => x.DistrictId.Equals(model.DistrictId)
            && x.LocalCode.Equals(localcode)
            && !lstStudentParent.Contains(x.RoleId)).IsNull();
            // Allow the same userCode with RoleIds = 26 (Parent) & 28 (studentUser)
        }

        public Func<T, string, bool> BeUniqueLocalCodePublisher()
        {
            return (model, localcode) => repository.Select().FirstOrDefault(x => x.LocalCode.Equals(localcode) && (x.RoleId.Equals((int)Permissions.Publisher))).IsNull();
        }
        public Func<string, bool> BeUniqueGlobally()
        {
            return username => repository.Select().FirstOrDefault(x => x.UserName.Equals(username)).IsNull();
        }

        public Func<string, bool> NotBePublisherName()
        {
            return username => repository.Select().FirstOrDefault(x => x.UserName.Equals(username) && (x.RoleId.Equals((int)Permissions.Publisher))).IsNull();
        }

        public Func<EditUserViewModel, bool> UserNameIsChanging()
        {
            return model =>
                {
                    var user = repository.Select().FirstOrDefault(x => x.Id.Equals(model.UserId));
                    return user != null && model.UserName.ToLower() != user.UserName.ToLower();
                };
        }
        public Func<EditUserViewModel, bool> LocalCodeIsChanging()
        {
            return model =>
                {
                    var user = repository.Select().FirstOrDefault(x => x.Id.Equals(model.UserId));
                    return user != null && model.LocalCode.ToLower() != user.LocalCode.ToLower();
                };
        }
        public Func<EditParentViewModel, bool> ParentUserNameIsChanging()
        {
            return model =>
            {
                var user = repository.Select().FirstOrDefault(x => x.Id.Equals(model.UserId));
                return user != null && model.UserName.ToLower() != user.UserName.ToLower();
            };
        }

        public bool CreatingNonPublisher(T model)
        {
            return model.RoleId.Equals((int) Permissions.SchoolAdmin) || model.RoleId.Equals((int) Permissions.Teacher) || model.RoleId.Equals((int) Permissions.DistrictAdmin);
        }

        public bool CreatingPublisher(T model)
        {
            return !CreatingNonPublisher(model);
        }

        public bool RequiresState(T model)
        {
            return (model.CurrentUserRoleId == (int)Permissions.LinkItAdmin || model.CurrentUserRoleId == (int)Permissions.Publisher)
                && (model.RoleId != (int)Permissions.LinkItAdmin && model.RoleId != (int)Permissions.Publisher)
                && (model.RoleId != 0);
        }

        public bool RequiresDistrict(T model)
        {
            return model.CurrentUserRoleId != (int)Permissions.SchoolAdmin && (model.RoleId == (int)Permissions.DistrictAdmin || RequiresSchool(model));
        }

        public bool RequiresSchool(T model)
        {
            return model.RoleId == (int)Permissions.SchoolAdmin || model.RoleId == (int)Permissions.Teacher;
        }

        public bool CreatingEditingNonPublisher(T model)
        {
            return model.RoleId.Equals((int)Permissions.SchoolAdmin)
                || model.RoleId.Equals((int)Permissions.Teacher)
                || model.RoleId.Equals((int)Permissions.DistrictAdmin)
                || model.RoleId.Equals((int)Permissions.NetworkAdmin);
        }

        public bool CreatingEditingPublisher(T model)
        {
            return model.RoleId.Equals((int)Permissions.Publisher);
        }
    }
}
