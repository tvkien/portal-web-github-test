using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.Helpers.SGO;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class SGOAuditTrailController : BaseController
    {
        private readonly SGOAuditTrailParameters _parameters;

        public SGOAuditTrailController(SGOAuditTrailParameters parameters)
        {
            _parameters = parameters;
        }

        public ActionResult Index(int? id)
        {
            ViewBag.SGOID = id;

            var model = new ViewSGOAuditTrailsDTO();
            if(!id.HasValue)return View(model);

            var sgo = _parameters.SGOService.GetSGOByID(id.Value);
            if (sgo == null) return View(model);
            var vPermissionAccess = _parameters.SGOService.GetPermissionAccessAuditTrail(CurrentUser.Id, sgo);
            if (vPermissionAccess.Status != (int)SGOPermissionEnum.ReadOnly && vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate)
            {
                //return Json(new { Success = false, Id = 0, ErrorMessage = "Has no permission" }, JsonRequestBehavior.AllowGet);
                return RedirectToAction("Index", "SGOManage");
            }

            model.SGOID = id.Value;
            model.SGOName = sgo.Name;

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GetAuditTrails(int? id)
        {
            var parser = new DataTableParser<SGOAuditTrailSearchItem>();

            if (!id.HasValue)
            {
                return Json(parser.Parse((new List<SGOAuditTrailSearchItem>()).AsQueryable(), true),
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Check access permission
                var sgoObject = _parameters.SGOService.GetSGOByID(id.Value);
                var vPermissionAccess = _parameters.SGOService.GetPermissionAccessAuditTrail(CurrentUser.Id, sgoObject);
                if (vPermissionAccess.Status != (int) SGOPermissionEnum.ReadOnly &&
                    vPermissionAccess.Status != (int) SGOPermissionEnum.FullUpdate)
                {
                    return Json(parser.Parse((new List<SGOAuditTrailSearchItem>()).AsQueryable(), true),
                        JsonRequestBehavior.AllowGet);
                }
            }

            var data =
                _parameters.SGOAuditTrailService.GetAuditTrailBySGOID(id.Value);
            if (data != null && data.SGOAuditTrailSearchItems != null)
                return Json(parser.Parse(data.SGOAuditTrailSearchItems.AsQueryable(), true),
                    JsonRequestBehavior.AllowGet);

            return Json(parser.Parse((new List<SGOAuditTrailSearchItem>()).AsQueryable(), true),
                    JsonRequestBehavior.AllowGet);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult AddAuditTrail(AddSGOAuditTrailsDTO model)
        {
            if (model == null || !model.SGOID.HasValue || !model.SGOActionTypeID.HasValue)
            {
                return Json(new { status = "error", message = "Invalid input data" });
            }

            var sgo = _parameters.SGOService.GetSGOByID(model.SGOID.Value);
            if (sgo != null && sgo.SGOStatusID == (int)SGOStatusType.Draft) return Json(new { status = "success" });

            model.ChangedByUserID = CurrentUser.Id;

            _parameters.SGOAuditTrailService.AddSGOAuditTrail(model);

            return Json(new { status = "success" });
        }
    }
}
