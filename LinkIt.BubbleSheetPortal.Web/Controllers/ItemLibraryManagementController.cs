using DevExpress.Utils.OAuth;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using S3Library;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class ItemLibraryManagementController : BaseController
    {
        #region Fields

        private readonly ItemBankControllerParameters _parameters;
        private readonly IS3Service _s3Service;

        #endregion

        #region Ctor

        public ItemLibraryManagementController(ItemBankControllerParameters parameters, IS3Service s3Service)
        {
            _parameters = parameters;
            _s3Service = s3Service;
        }

        #endregion

        #region Methods

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ItemLibraryManagement)]
        public ActionResult Index()
        {
            var xliAccess = _parameters.AuthItemLibService.GetXliFunctionAccess(CurrentUser.Id, CurrentUser.RoleId, CurrentUser.DistrictId ?? 0);
            ViewBag.XLIFunctionAccess = xliAccess;
            ViewBag.IsPublisher = CurrentUser.IsPublisher();
            ViewBag.UserId = CurrentUser.Id;
            return View();
        }

        #endregion
    }
}
