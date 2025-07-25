using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models.AccountDto
{
    public class SetUserInformationResultDto
    {
        public bool Success { get; set; }
        public string RedirectUrl { get; set; }
        public IEnumerable<ValidationFailure> ErrorList { get; internal set; }
        public string ErrorMessage { get; internal set; }
    }
}
