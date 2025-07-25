using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class RequestParameterValidator : AbstractValidator<RequestParameter>
    {
        public RequestParameterValidator()
        {
            RuleFor(x => x.RequestId)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Value)
                .NotNull()
                .NotEmpty();
        }
    }
}