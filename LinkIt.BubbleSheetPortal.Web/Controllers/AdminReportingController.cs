using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    public class AdminReportingController : BaseController
    {
        private DistrictDecodeService _districtDecodeService;

        public AdminReportingController(DistrictDecodeService districtDecodeService)
        {
            this._districtDecodeService = districtDecodeService;
        }

        public JsonResult GetWalkmeSnippetByDistrict(int districtId)
        {
            var walkmeSnippetURL = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, DistrictDecodeLabelConstant.WalkmeSnippetURL);
            return new JsonResult { Data = walkmeSnippetURL != null ? walkmeSnippetURL.Value : string.Empty, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
