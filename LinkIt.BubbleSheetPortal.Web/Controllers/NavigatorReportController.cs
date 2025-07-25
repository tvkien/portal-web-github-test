using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.ManageAccess;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.App_Start;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using S3Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [JsonNetFilter]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class NavigatorReportController : BaseController
    {
        private readonly IS3Service _s3Service;
        private readonly INavigatorReportService _navigatorReportService;
        private readonly ConfigurationService configurationService;
        private readonly NotificationMessageService _notificationMessageService;
        public NavigatorReportController(INavigatorReportService navigatorReportService, ConfigurationService configurationService,
            NotificationMessageService notificationMessageService, IS3Service s3Service)
        {
            _navigatorReportService = navigatorReportService;
            this.configurationService = configurationService;
            _notificationMessageService = notificationMessageService;
            ConfigS3ToService();
            _s3Service = s3Service;
        }

        private void ConfigS3ToService()
        {
            string bucket = LinkitConfigurationManager.GetS3Settings().NavigatorBucket;
            string folder = LinkitConfigurationManager.GetS3Settings().NavigatorFolder;
            this._navigatorReportService.Bucket = bucket;
            this._navigatorReportService.Folder = folder;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.NavigatorReport)]
        public ActionResult Index()
        {
            ViewBag.RoleId = CurrentUser.RoleId;
            ViewBag.UserId = CurrentUser.Id;
            return View();
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.NavigatorReportUpload)]
        public ActionResult Upload()
        {
            return View();
        }

        public ActionResult ListFileUpload()
        {
            return PartialView("_ListFileUpload");
        }

        [HttpPost]
        public JsonResult RecordsExist(IEnumerable<NavigatorReportUploadFileFormDataDTO> forms)
        {
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(forms
                  , new ValidationContext(forms)
                  , validationResults, true);

            if (validationResults?.Count > 0)
            {
                return Json(Util.ErrorFormat(string.Join(Environment.NewLine, validationResults.Select(c => c.ErrorMessage)), null), JsonRequestBehavior.AllowGet);
            }
            var recordsExist = _navigatorReportService.GetRecordsExist(forms);
            return Json(recordsExist);
        }

        [HttpPost]
        public ActionResult Submit()
        {
            var formData = Request.Form;
            HttpFileCollectionBase files = Request.Files;
            NavigatorReportUploadFileFormDataDTO navReportUploadFileFormData = NavigatorReportUploadFileFormDataDTO.InitFromNameValueCollection(formData);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(navReportUploadFileFormData
                  , new ValidationContext(navReportUploadFileFormData)
                  , validationResults, true);

            if (validationResults?.Count > 0)
            {
                return Json(Util.ErrorFormat(string.Join(Environment.NewLine, validationResults.Select(c => c.ErrorMessage)), null), JsonRequestBehavior.AllowGet);
            }
            var _postedFiles = Enumerable.Range(0, files.Count)
               .Select(c =>
                 files[c]
                   ).Select(c =>
                   {
                       using (MemoryStream ms = new MemoryStream())
                       {
                           if (c.InputStream == null)
                               return null;
                           c.InputStream.CopyTo(ms);
                           if (ms.Length <= 0)
                               return null;
                           NavigatorReportFileDTO resx = new NavigatorReportFileDTO()
                           {
                               ContentType = c.ContentType,
                               FileName = c.FileName,
                               FileBinary = ms.ToArray()
                           };
                           return resx;
                       }
                   }
                   ).ToList();
            if (_postedFiles.Count == 0 || _postedFiles.Exists(c => c == null || c.FileBinary == null || c.FileBinary.Length <= 0))
                return Json(BaseResponseModel<List<int>>.InstanceError("Please attach file.", "error", new List<int> { }), JsonRequestBehavior.AllowGet);

            var distributeSetting = new DistributeSetting()
            {
                TestCodePrefix = LinkitConfigurationManager.GetLinkitSettings().DatabaseID,
                DistributedBy = CurrentUser.Name
            };
            var uploadResponse = _navigatorReportService.ProcessUploadFiles(navReportUploadFileFormData, _postedFiles, this.CurrentUser.Id, distributeSetting);
            return Json(uploadResponse, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetSchoolYear(int districtId)
        {

            var schoolYears = _navigatorReportService.GetSchoolYears(districtId);
            return Json(Util.SuccessFormat(schoolYears), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FillTable(IEnumerable<NavigatorReportFillTableDto> forms)
        {
            var results = _navigatorReportService.OnFillTable(forms);

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetNavigatorCategory()
        {
            var categories = _navigatorReportService.GetNavigatorCategory();
            return Json(Util.SuccessFormat(categories), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetReportTypes(int? navigatorCategoryID)
        {
            var reportTypes = _navigatorReportService.GetReportTypes(navigatorCategoryID);
            return Json(Util.SuccessFormat(reportTypes), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetNavigatorCheckboxesDataByStateIdAndDistrictId(int? stateId, int? districtId)
        {
            var reportTypes = _navigatorReportService.GetNavigatorCheckboxesDataByStateIdAndDistrictId(CurrentUser.Id, CurrentUser.RoleId, stateId, districtId);
            return Json(Util.SuccessFormat(reportTypes), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetConfigurationById(int? navigatorConfigurationId)
        {
            var configuration = _navigatorReportService.GetConfigurationById(navigatorConfigurationId);
            return Json(Util.SuccessFormat(configuration), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetReportingPeriod()
        {
            var reportPeriods = _navigatorReportService.GetReportingPeriod();
            return Json(Util.SuccessFormat(reportPeriods), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetKeywords()
        {
            var keywords = _navigatorReportService.GetKeywords();
            return Json(Util.SuccessFormat(keywords), JsonRequestBehavior.AllowGet);
        }

        private JsonResult GetConfiguration(string key, char separateChar)
        {
            try
            {
                var data = configurationService.GetConfigurationByKey(key);
                if (data == null)
                {
                    return Json(Util.ErrorFormat($"Please config {key}", null), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var _list = data.Value.Split(new Char[] { separateChar }, StringSplitOptions.RemoveEmptyEntries).Select((v, i) => new { Id = v, Name = v }).ToList();
                    return Json(Util.SuccessFormat(_list), JsonRequestBehavior.AllowGet);
                }
            }
            catch (System.Exception ex)
            {
                return Json(Util.ErrorFormat(ex.Message, null), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAssociateUser(string nodePath, bool isPublished, bool canPublishStudent, bool canPublishTeacher, bool canPublishSchoolAdmin, bool canPublishDistrictAdmin, string programIds, string gradeIds, int districtId, bool exludeCurrentUser = false)
        {
            districtId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(districtId);
            var nodePaths = nodePath.Split(new string[] { "-_-" }, StringSplitOptions.RemoveEmptyEntries);
            var reportId = string.Join(",", _navigatorReportService.GetNavigatorReportIdsByNodePaths(nodePaths, CurrentUser.Id, CurrentUser.RoleId
                , districtId).Select(n => n.ToString()).ToArray());
            var data = _navigatorReportService.GetAssociateUser(reportId, CurrentUser.Id, isPublished, canPublishStudent, canPublishTeacher, canPublishSchoolAdmin, canPublishDistrictAdmin, programIds, gradeIds, districtId, CurrentUser.RoleId);
            var parser = new DataTableParser<NavigatorUserDto>();

            var dataQuery = data.StrongData.AsQueryable();
            if (exludeCurrentUser)
            {
                dataQuery = dataQuery.Where(c => c.UserID != CurrentUser.Id);
            }
            return Json(parser.Parse(dataQuery), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetNavigatorReportDetail(string nodePath, int districtId = 0)
        {
            districtId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(districtId);
            var reportDetail = _navigatorReportService.NavigatorGetReportDetail(nodePath, CurrentUser.Id, CurrentUser.RoleId, districtId);

            return Json(reportDetail, JsonRequestBehavior.AllowGet);
        }

        private int GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(int districtId)
        {
            if (!CurrentUser.IsPublisherOrNetworkAdmin)
                districtId = CurrentUser.DistrictId ?? 0;
            return districtId;
        }

        [HttpGet]
        public ActionResult GetListNavigatorReportByLevel(string nodePath = "", int districtId = 0)
        {
            IEnumerable<NavigatorReportDto> navigatorReports = null;
            if (!CurrentUser.IsPublisherOrNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId ?? 0;
            }
            if (districtId <= 0)
            {
                navigatorReports = new NavigatorReportDto[0];
            }
            else
            {
                navigatorReports = _navigatorReportService.GetNavigatorReports(nodePath, CurrentUser.Id, CurrentUser.RoleId, districtId);
            }

            var parser = new DataTableParser<NavigatorReportDto>();
            var listToBeParse = navigatorReports.OrderBy(x => 1);
            return Json(parser.Parse2020(listToBeParse, false), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUploadedReportsInfo(string reportIds)
        {
            var navigatorReportIds = !string.IsNullOrEmpty(reportIds) ? reportIds.Split(',').Select(Int32.Parse).ToArray() : new int[] { };
            var parser = new DataTableParser<NavigatorReportUploadFileResponseDto>();
            if (CurrentUser.RoleId != (int)Permissions.Publisher)
            {
                return Json(parser.Parse2018(new List<NavigatorReportUploadFileResponseDto>().AsQueryable()), JsonRequestBehavior.AllowGet);
            }
            IQueryable<NavigatorReportUploadFileResponseDto> data = _navigatorReportService.GetUploadedReportsInfo(CurrentUser.Id, navigatorReportIds);
            return Json(parser.Parse2018(data), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Publishing(int districtId)
        {
            if (_navigatorReportService.DontHaveRightToPublish(CurrentUser.RoleId))
            {
                return RedirectToAction("Index");
            }
            var model = new NavigatorConfigurationDTO();
            ViewBag.CurrentRoleId = CurrentUser.RoleId;
            ViewBag.DistrictId = districtId;
            return View(model);
        }

        [HttpGet]
        public ActionResult GetRolesToPublishByNodePaths(string nodePath, int districtId = 0)
        {
            districtId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(districtId);
            var roleDefinitions = _navigatorReportService.GetRolesToPublishByNodePaths(nodePath, CurrentUser.Id, CurrentUser.RoleId, districtId);
            return Json(roleDefinitions, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetNavigatorConfigurationPublishing(string nodePath, int districtId = 0)
        {
            districtId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(districtId);
            var model = _navigatorReportService.GetManagePublishingConfiguration(nodePath, CurrentUser.Id, CurrentUser.RoleId, districtId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Publish(NavigatorReportPublishRequestDto model)
        {
            if (_navigatorReportService.DontHaveRightToPublish(CurrentUser.RoleId))
            {
                return RedirectToAction("Index");
            }
            model.DistrictId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(model.DistrictId);
            model.GeneralUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), Constanst.NAVIGATOR_DISTRICT_CODE_SUB_DOMAIN, null, true);
            var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.NavigatorUseEmailCredentialKey);

            var res = _navigatorReportService.Publish(model, CurrentUser.Id, CurrentUser.RoleId, model.DistrictId, emailCredentialSetting);
            if (res.IsSuccess)
            {
                return Json(Util.SuccessFormat(true), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Util.ErrorFormat(res.Message, false), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UnPublish(NavigatorReportUnpublishRequestDto model)
        {
            if (_navigatorReportService.DontHaveRightToPublish(CurrentUser.RoleId))
            {
                return RedirectToAction("Index");
            }
            model.DistrictId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(model.DistrictId);
            var res = _navigatorReportService.UnPublish(model, CurrentUser.Id, CurrentUser.RoleId, model.DistrictId);
            if (res.IsSuccess)
            {
                var notification = _notificationMessageService.GetByNotificationType(CurrentUser.Id, TextConstants.NOTIFICATION_TYPE_NAVIGATOR_REPORT);
                if (notification != null)
                {
                    notification.Status = TextConstants.NOTIFICATION_UNPUBLISHED_STATUS;
                    _notificationMessageService.SaveNotification(notification);
                }
                return Json(Util.SuccessFormat(true), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Util.ErrorFormat(res.Message, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult PublishByRole(NavigatorPublishByRoleDTO navigatorPublishByRoleDTO)
        {
            navigatorPublishByRoleDTO.UserId = CurrentUser.Id;
            navigatorPublishByRoleDTO.RoleId = CurrentUser.RoleId;
            navigatorPublishByRoleDTO.DistrictId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(navigatorPublishByRoleDTO.DistrictId ?? 0);
            navigatorPublishByRoleDTO.GeneralUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), Constanst.NAVIGATOR_DISTRICT_CODE_SUB_DOMAIN, null, true);
            var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.NavigatorUseEmailCredentialKey);

            BaseResponseModel<NavigatorReportPublishByRoleResultDTO> publishByRoleResult = _navigatorReportService.PublishByRoleAndNodePaths(navigatorPublishByRoleDTO, emailCredentialSetting);
            return Json(publishByRoleResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult TryGetFile(string nodePath, int classId, int districtId = 0)
        {
            var resFile = GetFileByIdentifier(nodePath, classId, districtId);
            if (resFile.IsSuccess && resFile.StrongData?.FileData != null)
                return Json(new
                {
                    resFile.Message,
                    resFile.IsSuccess,
                    resFile.Status
                }, JsonRequestBehavior.AllowGet);
            else
            {
                return Json(Util.ErrorFormat("File not found", false), JsonRequestBehavior.AllowGet);
            }
        }
        private BaseResponseModel<NavigatorReportFileInfoResponseDto> GetFileByIdentifier(string nodePath, int classId, int districtId = 0)
        {
            districtId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(districtId);
            BaseResponseModel<NavigatorReportFileInfoResponseDto> resFile = _navigatorReportService.GetFile(nodePath, CurrentUser.Id, districtId, CurrentUser.RoleId, classId);
            return resFile;
        }

        [HttpGet]
        public ActionResult GetFile(string nodePath, int classId, int districtId = 0)
        {

            var resFile = GetFileByIdentifier(nodePath, classId, districtId);
            if (resFile.IsSuccess && resFile.StrongData?.FileData != null)
                return File(resFile.StrongData.FileData, resFile.StrongData.ContentType, resFile.StrongData.FileName);
            else
            {
                return Json(Util.ErrorFormat("File not found", false), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFilterDownload(string nodePath, int districtId = 0)
        {
            districtId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(districtId);
            var donwloadFilters = _navigatorReportService.GetFilterDownload(nodePath, CurrentUser.Id, CurrentUser.RoleId, districtId);
            return Json(donwloadFilters, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetNavigatorConfiguration(string nodePath, int districtId = 0)
        {
            districtId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(districtId);
            var model = _navigatorReportService.GetNavigatorConfiguration(nodePath, CurrentUser.Id, CurrentUser.RoleId, districtId);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAssociateEmails(string nodePath, string selectedRoleIds, int districtId = 0)
        {
            districtId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(districtId);
            var associateEmails = _navigatorReportService.GetAssociateEmailsWhichNotPublished(nodePath, selectedRoleIds, CurrentUser.Id, CurrentUser.RoleId, districtId);
            var res = BaseResponseModel<IEnumerable<NavigatorUserEmailDto>>.InstanceSuccess(associateEmails);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetManageAccessPublishDetail(string nodePath, string checkedUserIds, int districtId = 0)
        {
            districtId = GetCurrentDistrictIdIfIsntPublisherOrNetworkAdmin(districtId);
            var manageAccessPublishDetail = _navigatorReportService.GetManageAccessPublishDetail(nodePath, checkedUserIds, CurrentUser.Id, CurrentUser.RoleId, districtId);
            var res = BaseResponseModel<IEnumerable<ManageAccessPublishDetailDto>>.InstanceSuccess(manageAccessPublishDetail);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

    }
}
