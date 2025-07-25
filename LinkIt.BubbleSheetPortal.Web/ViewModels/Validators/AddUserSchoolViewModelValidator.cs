using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.Validators
{
    public class AddUserSchoolViewModelValidator : ManageUsersBaseViewModelValidator<AddUserSchoolViewModel>
    {
        private readonly IUserSchoolRepository<UserSchool> userSchoolRepository;

        public AddUserSchoolViewModelValidator(IRepository<User> repository, IUserSchoolRepository<UserSchool> userSchoolRepository) : base(repository)
        {
            this.userSchoolRepository = userSchoolRepository;

            RuleFor(x => x.SchoolId)
                .GreaterThan(0)
                .When(RequiresSchool)
                .WithMessage("Please select a school.")
                .Must(NotAlreadyExist())
                .WithMessage("That school is already assigned to this user.");
                
            RuleFor(x => x.UserId) 
                .NotNull()
                .GreaterThan(0)
                .WithMessage("User Id is required.");
        }

        private Func<AddUserSchoolViewModel, int, bool> NotAlreadyExist()
        {
            return (model, schoolId) => userSchoolRepository.Select().FirstOrDefault(x => x.UserId.Equals(model.UserId) && x.SchoolId.Equals(schoolId)).IsNull();
        }
    }
}