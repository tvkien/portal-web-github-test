using System;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Requests;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator(LabelService labelService)
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("User Id is required.");

            RuleFor(x => x.DistrictId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Please select a " + labelService.DistrictLabel + ".");

            RuleFor(x => x.ImportedFileName)
                .NotNull()
                .NotEmpty()
                .WithMessage("File Name is required.");

            RuleFor(x => x.RequestTime)
                .NotNull()
                .GreaterThan(DateTime.MinValue)
                .WithMessage("Request Time is required.");

            RuleFor(x => x.DataRequestType)
                .NotNull()
                .Must(BeDefined())
                .WithMessage("Data Request Type is required.");
        }

        private Func<DataRequestType, bool> BeDefined()
        {
            return x => Enum.IsDefined(typeof (DataRequestType), x);
        }
    }
}