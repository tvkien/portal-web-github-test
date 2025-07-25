using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class RegistrationControllerParameters
    {
        public UserService UserServices { get; set; }
        public APIAccountService APIAccountServices { get; set; }

        public APIPermissionService APIPermissionServices { get; set; }

        public APIFunctionService APIFunctionServices { get; set; }

        public DistrictService DistrictServices { get; set; }

        public DistrictDecodeService DistrictDecodeServices { get; set; }
        public IValidator<RegistrationViewModel> RegistrationViewModelValidator{ get; set; }
        public IFormsAuthenticationService FormsAuthenticationService { get; set; }

        public ConfigurationService ConfigurationServices { get; set; }
        public EmailService EmailService { get; set; }
    }
}