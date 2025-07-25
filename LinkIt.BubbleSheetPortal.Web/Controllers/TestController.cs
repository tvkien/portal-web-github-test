using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class TestController : BaseController
    {
        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignPassages)]
        public ActionResult Passages()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignTests)]
        public ActionResult Tests()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignAssement)]
        public ActionResult AssessmentItems()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignAssementOld)]
        public ActionResult AssessmentItemsOld()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignPassagesOld)]
        public ActionResult PassagesOld()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignTestsOld)]
        public ActionResult TestsOld()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlinetestingItem)]
        public ActionResult AssignTests(string id)
        {
            id = string.IsNullOrEmpty(id) ? string.Empty : id;
            return View(model: id);
        } 

        [HttpGet]
        public ActionResult PreviewTests()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Preferences()
        {
            return View();
        }
    }
}
