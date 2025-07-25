using System;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class UserSchoolValidator : AbstractValidator<UserSchool>
    {
        public UserSchoolValidator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.SchoolId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.DateActive)
                .NotNull()
                .NotEmpty()
                .GreaterThan(DateTime.MinValue);
        }
    }
}