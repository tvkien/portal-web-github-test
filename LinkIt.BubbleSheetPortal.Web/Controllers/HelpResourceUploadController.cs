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
using System.IO;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.Models.HelpResourceUpload;
using System.Net;
using System.Web;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Helpers;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class HelpResourceUploadController : BaseController
    {
        private readonly IS3Service _s3Service;

        private IHelpResourceService _service;

        public HelpResourceUploadController(IHelpResourceService service, IS3Service s3Service)
        {
            _service = service;
            _s3Service = s3Service;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.HelpResource)]
        public ActionResult Index(int? helpResourceID)
        {
            if (!CurrentUser.IsPublisher)
            {
                return RedirectToAction("Index", "HelpResource");
            }
            ViewBag.HelpResourceID = helpResourceID;
            return View();
        }

        [HttpPost]
        public ActionResult GetHelpResourceByID(int? helpResourceID)
        {
            if (!helpResourceID.HasValue) return Json(new GetHelpResourceByIDResponse(), JsonRequestBehavior.AllowGet);

            var data = _service.GetHelpResourceByID(helpResourceID.Value);
            if (data == null) data = new GetHelpResourceByIDResponse();

            if (data.HelpResourceFileTypeID.HasValue && data.HelpResourceFileTypeID.Value != (int)HelpResourceFileTypeEnum.Link)
            {
                data.HelpResourceFilePath = data.HelpResourceFilePath;
            }
            else if (data.HelpResourceFileTypeID.HasValue && data.HelpResourceFileTypeID.Value == (int)HelpResourceFileTypeEnum.Link)
            {
                data.HelpResourceLinkValid = ValidateUrl(data.HelpResourceLink);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadHelpResource(UploadHelpResourceRequest request)
        {
            var saveRequest = Transform(request);
            if (request.HelpResourceLinkOrFile == "link")
            {
                saveRequest.HelpResourceFileTypeID = (int)HelpResourceFileTypeEnum.Link;
                _service.SaveHelpResource(saveRequest);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            if (request.PostedFile == null)
            {
                _service.SaveHelpResource(saveRequest);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return UploadFileToS3(saveRequest, request.PostedFile);
        }

        private JsonResult UploadFileToS3(SaveHelpResourceRequest saveRequest, HttpPostedFileBase postedFile)
        {
            var s3Setting = LinkitConfigurationManager.GetS3Settings();
            var timeout = System.Configuration.ConfigurationManager.AppSettings["ResourceUploadTimeoutMinutes"];

            var fileTypeID = ConvertFileExtToHelpResourceFileType(Path.GetExtension(postedFile.FileName));
            if (!fileTypeID.HasValue) return Json(new { success = false, errorMessage = "Forbidden file type." }, JsonRequestBehavior.AllowGet);

            try
            {
                var filePath = string.Format("{0}/{1}", s3Setting.HelpReourceFolder, postedFile.FileName);
                var s3Result = _s3Service.UploadResourceFile(s3Setting.HelpResourceBucket, filePath, postedFile.InputStream, int.Parse(timeout));

                if (s3Result.IsSuccess)
                {
                    saveRequest.HelpResourceFilePath = postedFile.FileName;
                    saveRequest.HelpResourceFileTypeID = fileTypeID;

                    _service.SaveHelpResource(saveRequest);

                    return Json(new { success = true, fileName = postedFile.FileName }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, errorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ValidateLink(string link)
        {
            var urlIsValid = ValidateUrl(link);
            return Json(new { success = urlIsValid }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download(string filePath)
        {
            ViewData["filePath"] = filePath;
            return View("DownloadFile");
        }

        public ActionResult DownloadHelpResource(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                ViewData["message"] = "Invalid download link";
                return View("Error");
            }

            var result = DownloadS3File(filePath);
            if (result.IsSuccess)
            {
                var fileName = Path.GetFileName(filePath);
                var mimeType = MimeMapping.GetMimeMapping(fileName);

                Response.AppendHeader("Content-Disposition", string.Format("inline; filename={0}", fileName));

                var fileExt = Path.GetExtension(filePath).ToUpper();
                if (MediaHelper.GetVideoExtensions().Contains(fileExt))
                {
                    mimeType = "video/mp4";
                    Response.AppendHeader("Accept-Ranges", "bytes");
                    Response.AppendHeader("Content-Type", "video/mp4");
                    var content = Request.Headers.Get("Range");
                    if (string.IsNullOrWhiteSpace(content) || content.Equals("bytes=0-"))
                    {
                        var fileLength = result.ReturnStream.Length;
                        var contentRangeValue = string.Format("bytes 0-{0}/{1}", fileLength - 1, fileLength);
                        Response.AppendHeader("Content-Range", contentRangeValue);
                    }
                }

                return File(result.ReturnStream, mimeType);
            }

            ViewData["message"] = "Invalid download link";

            return View("Error");
        }

        private S3DownloadResult DownloadS3File(string filePath)
        {
            var s3Setting = LinkitConfigurationManager.GetS3Settings();
            var s3Domain = s3Setting.S3Domain;
            var s3Bucket = s3Setting.HelpResourceBucket;
            var s3Folder = s3Setting.HelpReourceFolder;

            if (string.IsNullOrEmpty(filePath)) return null;

            var fileUrl = s3Folder + "/" + filePath;
            var result = _s3Service.DownloadFile(s3Bucket, fileUrl);

            return result;
        }

        private bool ValidateUrl(string url)
        {
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                if (request == null) return false;

                request.Method = "HEAD";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK) return true;
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
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

        private int? ConvertFileExtToHelpResourceFileType(string fileExt)
        {
            if (string.IsNullOrWhiteSpace(fileExt)) return null;

            var fileTypes = _service.GetHelpResourceFileTypes();
            if (fileTypes == null) return null;

            var fileType = fileTypes.FirstOrDefault(o => o.Extensions != null && o.Extensions.Contains(fileExt));
            if (fileType == null) return null;

            return fileType.HelpResourceFileTypeID;
        }

        private SaveHelpResourceRequest Transform(UploadHelpResourceRequest request)
        {
            if (request == null) return null;

            var result = new SaveHelpResourceRequest
            {
                HelpResourceID = request.HelpResourceID,
                HelpResourceTypeID = request.HelpResourceTypeID,
                HelpResourceCategoryID = request.HelpResourceCategoryID,
                Topic = request.Topic,
                KeyWords = request.KeyWords,
                Description = request.Description,
                HelpResourceLink = request.HelpResourceLink,
            };

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
                    FileType = "FileType" + i,
                    ID = i,
                    Topic = "Topic" + i,
                    UpdatedDate = DateTime.UtcNow
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
