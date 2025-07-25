using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Enums;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class TestLibraryExportController : BaseController
    {
        private readonly QTIITemService _qtiItemService;
        private readonly VirtualTestService _virtualTestService;

        public TestLibraryExportController(QTIITemService qtiItemService, VirtualTestService virtualTestService)
        {
            _qtiItemService = qtiItemService;
            _virtualTestService = virtualTestService;

        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtTestLibraryExport)]
        public ActionResult Index()
        {
            var model = new TestLibraryExportViewModel()
            {
                CurrentDistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };
            if (CurrentUser.IsNetworkAdmin)
            {
                ViewBag.ListDictrictIDs = ConvertListIdToStringId(CurrentUser.GetMemberListDistrictId());
            }
            return View(model);
        }

        private string ConvertListIdToStringId(List<int> districtIDs)
        {
            if (!districtIDs.Any())
            {
                return string.Empty;
            }
            return string.Join(",", districtIDs);
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtTestLibraryExport)]
        [HttpGet]
        public ActionResult ExportToCSV(int type, int districtID)
        {
            try
            {
                var vDistrictID = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin ? districtID : CurrentUser.DistrictId.GetValueOrDefault();
                var result = "";
                switch (type)
                {
                    case (int)LibraryExportEnum.ItemLibrary:
                        result = _qtiItemService.ProcessExportItemLibraryByDistrictID(vDistrictID); ;
                        break;
                    case (int)LibraryExportEnum.TestLibrary:
                        result = _virtualTestService.ProcessExportTestLibraryByDistrictID(vDistrictID);
                        break;
                    case (int)LibraryExportEnum.PassageLibrary:
                        var model = new MediaModel();
                        result = _qtiItemService.ProcessExportPassageLibraryByDistrictID(vDistrictID, CurrentUser.Id, CurrentUser.RoleId, model.UpLoadBucketName, model.AUVirtualTestROFolder);
                        break;
                    default:
                        break;
                }

                return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
            }
            catch (System.Exception ex)
            {
                return new JsonResult() { Data = "ExportToCSV-Exception:" + ex.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet};
            }
        }
    }
}
