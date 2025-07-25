using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class ItemBankValidator : AbstractValidator<ItemBank>
    {
        public ItemBankValidator()
        {           
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .Length(1, 256);

            RuleFor(x => x.UserID)
                .GreaterThan(0);
            
            RuleFor(x => x.SchoolID)
                .GreaterThan(0);

            RuleFor(x => x.DistrictID)
                .GreaterThan(0);

            RuleFor(x => x.StateID)
                .GreaterThan(0);
        }
    }
}