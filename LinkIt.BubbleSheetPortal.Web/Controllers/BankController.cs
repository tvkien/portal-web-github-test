using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    public class BankController : BaseController
    {
        private readonly BankService _bankService;

        public BankController(
            BankService bankService)
        {
            this._bankService = bankService;
        }

        [HttpGet]
        public ActionResult CheckIfBankLocked(int bankId, int? districtId)
        {
            var locked = _bankService.CheckIfBankLocked(CurrentDistrict(districtId), bankId);
            return Json(new { Locked = locked }, JsonRequestBehavior.AllowGet);
        }
    }
}
