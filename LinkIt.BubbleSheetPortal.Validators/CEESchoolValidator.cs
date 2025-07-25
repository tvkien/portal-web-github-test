using System;
using System.Linq;
using System.Text.RegularExpressions;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class CEESchoolValidator : AbstractValidator<School>
    {
        private readonly IRepository<School> repository;

        public CEESchoolValidator(IRepository<School> repository)
        {
            this.repository = repository;

            RuleFor(x => x.Code)
                .NotNull()
                .NotEmpty()
                .Length(0, 20)
                .Must(AlphaNumericOnly)
                .WithMessage("Code must be alphanumeric.");

            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .Length(0, 100)
                .Must(NameMustBeUniqueToDistrictAndLocationCode())
                .WithMessage("Name already exists.");

            RuleFor(x => x.Code)
                .NotNull()
                .NotEmpty()
                .Length(0, 20)
                .Must(CodeMustBeUniqueToDistrict())
                .WithMessage("Code already exists.");

            RuleFor(x => x.StateCode)
                .Length(0, 20)
                .Must(AlphaNumericOnly)
                .WithMessage("State Code must be alpha numeric.")
                .Must(StateCodeMustBeUniqueToDistrict())
                .WithMessage("State already exists.");
        }

        private Func<School, string, bool> NameMustBeUniqueToDistrictAndLocationCode()
        {
            return (model, schoolName) => repository.Select().FirstOrDefault(x =>
                model.Id != x.Id && x.DistrictId.Equals(model.DistrictId) && x.Name.Equals(schoolName) && x.LocationCode.Equals(model.LocationCode)).IsNull();
        }

        private Func<School, string, bool> CodeMustBeUniqueToDistrict()
        {
            return (model, code) => repository.Select().FirstOrDefault(x =>
                model.Id != x.Id && x.DistrictId.Equals(model.DistrictId) && x.Code.Trim() != string.Empty && x.Code.Equals(code)).IsNull();
        }

        private Func<School, string, bool> StateCodeMustBeUniqueToDistrict()
        {
            return (model, state) => repository.Select().FirstOrDefault(x =>
                model.Id != x.Id && x.DistrictId.Equals(model.DistrictId) && x.StateCode.Trim() != string.Empty && x.StateCode.Equals(state)).IsNull();
        }

        private bool AlphaNumericOnly(string value)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9]*$");
            bool isValid = regex.IsMatch(value ?? string.Empty);
            return isValid;
        }
    }
}