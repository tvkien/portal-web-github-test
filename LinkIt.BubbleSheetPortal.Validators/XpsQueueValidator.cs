using FluentValidation;
using LinkIt.BubbleSheetPortal.Models.Old.XpsQueue;
using LinkIt.BubbleSheetPortal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class XpsQueueValidator : AbstractValidator<XpsQueue>
    {
        public XpsQueueValidator(LabelService labelService)
        {
            RuleFor(x => x.XpsDistrictUploadID)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("xpsDistrictUpload ID is required.");
        }
    }
}
