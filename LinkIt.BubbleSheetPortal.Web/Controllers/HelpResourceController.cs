using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Models.HelpResource;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Models.HelpResource;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Models.Constants;
using DevExpress.XtraCharts;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class HelpResourceController : BaseController
    {
        private IHelpResourceService _service;

        public HelpResourceController(IHelpResourceService service)
        {
            _service = service;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.HelpResource)]
        public ActionResult Index()
        {
            var title = ContaintUtil.Help;
            if (Request.QueryString["codes"] != null)
            {
                List<int> listOrderCategory = new List<int>();
                string[] strListCodes = Request.QueryString["codes"].ToString().ToLower().Split(',');
                var data = _service.GetHelpResourceCategories(CurrentUser.RoleId);
                foreach (var item in data)
                {
                    if (strListCodes.Contains(item.Code.ToLower()))
                    {
                        listOrderCategory.Add(item.HelpResourceCategoryID);
                    }
                }
                ViewBag.ListCategorySeleted = string.Join(",", listOrderCategory);
                foreach (var code in strListCodes)
                {
                    switch (code)
                    {
                        case ResourceCodesConstants.SUPPORT:
                            title = ContaintUtil.Techsupport;
                            break;
                        case ResourceCodesConstants.REPORTING:
                            title = ContaintUtil.Reporting;
                            break;
                        case ResourceCodesConstants.INTERVENTIONMANAGER:
                            title = ContaintUtil.InterventionManager;
                            break;
                        case ResourceCodesConstants.ONLINETESTING:
                            title = ContaintUtil.Onlinetesting;
                            break;
                        case ResourceCodesConstants.BUBBLESHEETS:
                            title = ContaintUtil.Managebubblesheets;
                            break;
                        case ResourceCodesConstants.DATALOCKER:
                            title = ContaintUtil.ResultsEntryDataLocker;
                            break;
                        case ResourceCodesConstants.TESTDESIGN:
                            title = ContaintUtil.Testdesign;
                            break;
                        case ResourceCodesConstants.USERCLASSMANAGEMENT:
                            title = ContaintUtil.DataAdmin;
                            break;
                        case ResourceCodesConstants.LEARNINGLIBRARY:
                            title = ContaintUtil.Lessons;
                            break;
                    }
                }
                
            }
            else
            {
                ViewBag.ListCategorySeleted = string.Empty;
            }
            ViewBag.Title = HelperExtensions.FormatPageTitle(title, ResourceCodesConstants.HELPRESOURCES);
            return View();
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.HelpResource)]
        public ActionResult Admin()
        {
            if (!CurrentUser.IsPublisher)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetHelpResourceCategories()
        {
            var data = _service.GetHelpResourceCategories(CurrentUser.RoleId);
            if (data == null) return Json(new List<HelpResourceCategoryModel>(), JsonRequestBehavior.AllowGet);

            var result = data.Select(o => Transform(o)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetHelpResourceFileTypes()
        {
            var data = _service.GetHelpResourceFileTypes();
            if (data == null) return Json(new List<HelpResourceFileTypeModel>(), JsonRequestBehavior.AllowGet);

            var result = data.Select(o => Transform(o)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetHelpResourceTypes()
        {
            var data = _service.GetHelpResourceTypes();
            if (data == null) return Json(new List<HelpResourceTypeModel>(), JsonRequestBehavior.AllowGet);

            var result = data.Select(o => Transform(o)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetHelpResources(HelpResourceCriteria criteria)
        {
            var result = new DataTableResponse();
            result.sEcho = criteria.sEcho;
            result.sColumns = criteria.sColumns;

            if (criteria.PageLoad)
            {
                result.iTotalDisplayRecords = 0;
                result.iTotalRecords = 0;
                result.aaData = new List<HelpResourceRow>();
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var request = new GetHelpResourcesRequest();
            request.HelpResourceCaterogyIDs = criteria.SelectedCategories;
            request.PageSize = criteria.iDisplayLength;
            request.SearchText = criteria.SearchText;
            request.StartRow = criteria.iDisplayStart;
            request.RoleId = CurrentUser.RoleId;

            if (!string.IsNullOrWhiteSpace(criteria.sColumns) && criteria.iSortCol_0.HasValue)
            {
                var columns = criteria.sColumns.Split(',');
                request.SortColumn = columns[criteria.iSortCol_0.Value];
                request.SortDirection = string.Compare(criteria.sSortDir_0, "desc") == 0 ? "DESC" : "ASC";
            }

            var data = _service.GetHelpResources(request);

            result.iTotalDisplayRecords = data.TotalRecord;
            result.iTotalRecords = data.TotalRecord;
            result.aaData = data.Data.Select(o => Transform(o)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteHelpResource(int? helpResourceID)
        {
            if (helpResourceID.HasValue) _service.DeleteHelpResource(helpResourceID.Value);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        private HelpResourceRow Transform(HelpResourcesSearchItem data)
        {
            if (data == null) return null;

            var result = new HelpResourceRow
            {
                Category = data.CategoryText,
                Description = data.Description,
                FileType = data.HelpResourceFileTypeName,
                HelpResourceTypeID = data.HelpResourceTypeID.HasValue ? data.HelpResourceTypeID.Value : 0,
                ID = data.HelpResourceID,
                Topic = data.Topic,
                UpdatedDate = data.DateUpdated,
                HelpResourceFilePath = data.HelpResourceFilePath,
                HelpResourceLink = data.HelpResourceLink,
                HelpResourceFileTypeID = data.HelpResourceFileTypeID,
                HelpResourceTypeDisplayText = data.HelpResourceTypeDisplayText,
                HelpResourceID = data.HelpResourceID,
                HelpResourceTypeIcon = data.HelpResourceTypeIcon
            };

            return result;
        }

        private string BuildS3Link(string fileName)
        {
            var s3Setting = LinkitConfigurationManager.GetS3Settings();
            var s3Domain = s3Setting.S3Domain;
            var s3Bucket = s3Setting.HelpResourceBucket;
            var s3Folder = s3Setting.HelpReourceFolder;

            var result = UrlUtil.GenerateS3DownloadLink(s3Domain, s3Bucket, s3Folder, fileName);

            return result;
        }

        private HelpResourceCategoryModel Transform(HelpResourceCategoryItem data)
        {
            if (data == null) return null;

            var result = new HelpResourceCategoryModel();
            result.DisplayText = data.DisplayText;
            result.ID = data.HelpResourceCategoryID;
            result.Index = data.SortOrder;

            return result;
        }

        private HelpResourceFileTypeModel Transform(HelpResourceFileTypeItem data)
        {
            if (data == null) return null;

            var result = new HelpResourceFileTypeModel();
            result.DisplayText = data.DisplayText;
            result.ID = data.HelpResourceFileTypeID;

            return result;
        }

        private HelpResourceTypeModel Transform(HelpResourceTypeItem data)
        {
            if (data == null) return null;

            var result = new HelpResourceTypeModel();
            result.DisplayText = data.DisplayText;
            result.ID = data.HelpResourceTypeID;
            result.Description = data.Description;
            result.ImgPath = data.ImgPath;

            return result;
        }

        private List<HelpResourceCategoryModel> GenerateHelpResourceCategories()
        {
            var listData = new List<HelpResourceCategoryModel>();
            for (var i = 1; i <= 20; i++)
            {
                listData.Add(new HelpResourceCategoryModel
                {
                    ID = i,
                    DisplayText = "DisplayText" + i,
                    Index = i
                });
            }

            return listData;
        }

        private DataTableResponse CreateHelpResourceResponse(HelpResourceCriteria criteria)
        {
            var listData = new List<HelpResourceRow>();
            for (var i = 1; i <= 100; i++)
            {
                listData.Add(new HelpResourceRow
                {
                    Category = "Category" + i,
                    Description = "Description" + i,
                    FileType = "1",
                    HelpResourceTypeID = 1,
                    ID = i,
                    Topic = "Topic" + i,
                    UpdatedDate = DateTime.UtcNow,
                    HelpResourceFilePath = "BuildS3Link(data.HelpResourceFilePath)",
                    HelpResourceLink = "data.HelpResourceLink",
                    HelpResourceFileTypeID = 1,
                    HelpResourceTypeDisplayText = "HelpResourceTypeDisplayText"
                });
            }

            var result = new DataTableResponse();
            result.iTotalDisplayRecords = listData.Count();
            result.iTotalRecords = listData.Count();
            result.sEcho = criteria.sEcho;
            result.aaData = listData.OrderBy(o => o.ID).Skip(criteria.iDisplayStart).Take(criteria.iDisplayLength).ToList();
            result.sColumns = criteria.sColumns;

            return result;
        }
    }
}
