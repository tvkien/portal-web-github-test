using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class TLDSReportControllerParameters
    {
        public GenderService GenderService { get; set; }
        public SchoolService SchoolService { get; set; }
        public StateService StateService { get; set; }
        public DistrictService DistrictService { get; set; }
        public TLDSService TLDSService { get; set; }
        public UserSchoolService UserSchoolService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public UserService UserService { get; set; }
    }
}