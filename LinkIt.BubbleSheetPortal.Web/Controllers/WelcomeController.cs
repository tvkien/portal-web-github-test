using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class WelcomeController : BaseController
    {
        [HttpGet]
        public ActionResult ReportingDashboard()
        {
            return View();
        }

        [HttpGet]
        public ActionResult TestDesign()
        {
            return View();
        }

        [HttpGet]
        public ActionResult OnlineTesting()
        {
            return View();
        }

        [HttpGet]
        public ActionResult BubbleSheets()
        {
            return View();
        }
    }
}