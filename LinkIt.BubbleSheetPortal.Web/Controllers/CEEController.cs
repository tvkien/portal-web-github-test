using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class CEEController : BaseController
    {

        private readonly CEEControllerParameters _parameters;

        public CEEController(CEEControllerParameters parameters)
        {
            this._parameters = parameters;
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}