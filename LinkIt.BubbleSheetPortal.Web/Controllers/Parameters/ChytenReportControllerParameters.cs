using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.Requests;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.DistrictReferenceData;
using LinkIt.BubbleSheetPortal.Web.Helpers.DistrictReferenceData;
using LinkIt.BubbleSheetPortal.Web.Helpers.ETL;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Services.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class ChytenReportControllerParameters
    {      
        public DistrictStateService DistrictStateServices { get; set; }
        public ChytenReportService ChytenReportServices { get; set; }
        public UserSchoolService UserSchoolServices { get; set; }
        public DistrictDecodeService DistrictDecodeServices { get; set; }  
    }
}