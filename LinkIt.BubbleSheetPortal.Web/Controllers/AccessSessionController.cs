using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class AccessSessionController : Controller
    {
        public ActionResult StoreSpecializedReportJobSession(int specializedReportJobId)
        {
            var specializedReportJobIds = new List<int>();
            if (Session["SpecializedReportJobIds"] != null)
            {
                specializedReportJobIds = (List<int>)Session["SpecializedReportJobIds"];
            }

            specializedReportJobIds.Add(specializedReportJobId);
            Session["SpecializedReportJobIds"] = specializedReportJobIds;

            return Json(new {result = true});
        }
    }
}
