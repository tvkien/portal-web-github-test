using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    public class ReportServiceController : BaseController
    {
        [HttpGet, AdminOnly]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadDistrictUsage)]
        public ActionResult DistrictUsage()
        {
            return View();
        }

        //[HttpGet, PublisherOnly]
        //public ActionResult TestExtract()
        //{
        //    return View();
        //}
    }
}
