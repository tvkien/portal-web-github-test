using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models.ETL;

namespace LinkIt.BubbleSheetPortal.Validators
{
    public class MappingRuleValidator : AbstractValidator<MappingRule>
    {
        public MappingRuleValidator()
        {
            RuleFor(x => x.CommonField.MappingList)
                .Must(AllMappingsMustBeValid());

            RuleFor(x => x.TestList)
                .Must(AllTestMappingsMustBeValid());
        }

        private Func<List<BaseMapping>, bool> AllMappingsMustBeValid()
        {
            return model => model.Any(m => !m.IsValid) == false;
        }

        private Func<List<TestMapping>, bool> AllTestMappingsMustBeValid()
        {
            return model => model.Any(t => t.MappingList.Any(m => !m.IsValid)) == false;
        }
    }
}