using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Security;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class HomeSessionStateReadOnlyController : BaseController
    {
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly ConfigurationService _configurationService;

        public HomeSessionStateReadOnlyController(
            DistrictDecodeService districtDecodeService,
            ConfigurationService configurationService)
        {
            _districtDecodeService = districtDecodeService;
            _configurationService = configurationService;
        }

        public ActionResult KeepAlive()
        {
            return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetConfigTimeOutWarning()
        {
            var result = new SessionTimeOutDTO();
            result.ShowTimeOutWarning = _districtDecodeService.GetTimeOutDistrictDecodeByLabel(CurrentUser.DistrictId.GetValueOrDefault(), DistrictDecodeLabelConstant.ShowWarningTimeout);
            result.WarningTimeoutMinues = _configurationService.GetConfigurationByKeyWithDefaultValue(Constanst.PortalWarningTimeOutMinute, 5);
            result.DefaultCookieTimeOutMinutes = _configurationService.GetConfigurationByKeyWithDefaultValue(Constanst.DefaultCookieTimeOut, 30);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
