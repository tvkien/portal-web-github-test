using System.Web.Mvc;
using System.Web.SessionState;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    public class DemoSessionStateController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TestSecssionState()
        {
            System.Threading.Thread.Sleep(10000);
            return new EmptyResult();
        }
    }
}