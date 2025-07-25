using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using RestSharp;
using RestSharp.Authenticators;
using S3Library;
using System.Web;
using LinkIt.BubbleSheetPortal.Web.Helpers;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class NotificationController : Controller
    {
        private const string AccessDeniedMessage = "You don't have a permission to view this file";
        private const string PortalNotificationRole = "Portal-Notification-Role";

        private DistrictService districtService;
        private VirtualTestFileService virtualTestFileService;
        private S3PermissionService S3PermissionServices;
        private IS3Service s3Service;
        private UserService userServices;

        private string s3PortalLinkContext = LinkitConfigurationManager.GetDatabaseSettings().S3PortalLinkContext;

        public NotificationController(DistrictService districtService, VirtualTestFileService virtualTestFileService, S3PermissionService S3PermissionServices, UserService userServices, IS3Service s3Service)
        {
            this.districtService = districtService;
            this.virtualTestFileService = virtualTestFileService;
            this.S3PermissionServices = S3PermissionServices;
            this.userServices = userServices;
            this.s3Service = s3Service;
        }

        protected UserPrincipal CurrentUser
        {
            get { return ((UserPrincipal)User); }
        }

        [HttpGet]
        public ActionResult DownloadFile(string key)
        {
            ViewData["key"] = key;
            return View();
        }

        [HttpPost]
        public ActionResult DownloadFile(string key, string temp)
        {
            var link = s3Service.GetS3PortalLinkByKey(key, LinkitConfigurationManager.GetDatabaseSettings().S3PortalLinkContext);
            var hasDownloadFilePermission = HasDownLoadFilePermission(link);
            if (!hasDownloadFilePermission)
            {
                ViewData["message"] = AccessDeniedMessage;
                return View("Error");
            }

            var result = s3Service.DownloadFile(link.BucketName, link.FilePath);
            if (result.IsSuccess)
            {
                var fileName = Path.GetFileName(link.FilePath);
                return File(result.ReturnStream, "text/plain", fileName);
            }

            ViewData["message"] = "File not found";

            return View("Error");
        }

        private bool HasDownLoadFilePermission(S3PortalLinkDTO link)
        {
            if (link == null) return false;
            if (link.DistrictID.HasValue)
            {
                var district = districtService.GetDistrictById(link.DistrictID.Value);
                if (district == null) return false;
            }

            var isUpdateStatusService = link.ServiceType == (int)ServiceTypeEnum.StudentStatusUpdate ||
                                        link.ServiceType == (int)ServiceTypeEnum.UserStatusUpdate;
            if (isUpdateStatusService)
            {
                if (!CurrentUser.IsDistrictAdminOrPublisher)
                    return false;
                return true;
            }


            var notificationRole = s3Service.GetConfiguration(PortalNotificationRole, 3, s3PortalLinkContext);
            if (notificationRole == null || string.IsNullOrWhiteSpace(notificationRole.Value)) return false;

            var notificationRoles =
                notificationRole.Value.Split(',');
            if (notificationRoles.Count(o => o != null && o.Trim() == CurrentUser.RoleId.ToString(CultureInfo.InvariantCulture)) == 0)
            {
                return false;
            }

            if (CurrentUser.IsDistrictAdminOrPublisher)
            {
                return true;
            }

            if (CurrentUser.DistrictId.HasValue && link.DistrictID.HasValue && CurrentUser.DistrictId.Value == link.DistrictID.Value)
            {
                return true;
            }

            return false;
        }
        
        public ActionResult DownloadRubricFile(string key)
        {
            var virtualTestFile = virtualTestFileService.GetByFileKey(key);

            if (virtualTestFile != null)
            {
                var bucketName = LinkitConfigurationManager.GetS3Settings().RubricBucketName;

                var result = s3Service.DownloadFile(bucketName, virtualTestFile.FileUrl);
                if (result.IsSuccess)
                {
                    var fileName = Path.GetFileName(virtualTestFile.FileUrl);
                    var contentType = MimeMapping.GetMimeMapping(virtualTestFile.FileUrl);
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = virtualTestFile.FileName,
                        Inline = true,
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());
                    return File(result.ReturnStream, contentType);
                }
            }

            ViewData["message"] = "Invalid download link";

            return View("Error");
        }
        public ActionResult DownloadDataExtraxtFile(string key)
        {
            var dto = s3Service.GetS3PortalLinkByKey(key, s3PortalLinkContext);
            if (dto != null)
            {
                var s3Permission = S3PermissionServices.GetS3PermissionById(dto.S3PortalLinkID);
                if (s3Permission != null)
                {                    
                    var hasPermission = false;
                    if (CurrentUser.IsPublisher)
                        hasPermission = true;
                    else
                    {
                        if (s3Permission.UserId == CurrentUser.Id)
                            hasPermission = true;
                        else
                        {
                            var userextract = userServices.GetUserById(s3Permission.UserId);
                            if (userextract.DistrictId == CurrentUser.DistrictId)                             
                            {
                                if (userextract.RoleId == (int) Permissions.SchoolAdmin && CurrentUser.IsDistrictAdmin)
                                    hasPermission = true;
                                else if (userextract.RoleId == (int) Permissions.Teacher &&
                                         (CurrentUser.IsDistrictAdmin || CurrentUser.IsSchoolAdmin))
                                    hasPermission = true;
                            }
                        }
                    }
                    if (hasPermission)
                    {
                        var result = s3Service.DownloadFile(dto.BucketName, dto.FilePath);
                        if (result.IsSuccess)
                        {
                            var fileName = Path.GetFileName(dto.FilePath);
                            return File(result.ReturnStream, "text/plain", fileName);
                        }
                    }
                    else
                    {
                        ViewData["message"] = AccessDeniedMessage;
                        return View("Error");
                    }
                }
                else
                {
                    ViewData["message"] = AccessDeniedMessage;
                    return View("Error");
                }
            }

            ViewData["message"] = "Invalid download link";

            return View("Error");
        }
        public ActionResult DownloadTestResultFile(string key)
        {
            var dto = s3Service.GetS3PortalLinkByKey(key, s3PortalLinkContext);
            if (dto != null)
            {
                var s3Permission = S3PermissionServices.GetS3PermissionById(dto.S3PortalLinkID, CurrentUser.Id);
                if (s3Permission != null)
                {
                    var result = s3Service.DownloadFile(dto.BucketName, dto.FilePath);
                    if (result.IsSuccess)
                    {
                        var fileName = Path.GetFileName(dto.FilePath);
                        return File(result.ReturnStream, "text/plain", fileName);
                    }
                }
                else
                {
                    ViewData["message"] = AccessDeniedMessage;
                    return View("Error");
                }
            }

            ViewData["message"] = "Invalid download link";

            return View("Error");
        }

        public ActionResult DownloadTestExtractResultFile(string key)
        {
            var dto = s3Service.GetS3PortalLinkByKey(key, s3PortalLinkContext);
            if (dto != null)
            {
               var result = s3Service.DownloadFile(dto.BucketName, dto.FilePath);
                    if (result.IsSuccess)
                    {
                        var fileName = Path.GetFileName(dto.FilePath);
                        return File(result.ReturnStream, "text/plain", fileName);
                    }                
            }

            ViewData["message"] = "Invalid download link";

            return View("Error");
        }
        public ActionResult DownloadLesson(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var bucketName = LinkitConfigurationManager.GetS3Settings().LessonBucketName;
                var S3LessonFolder= ConfigurationManager.AppSettings[ContaintUtil.AppSettingLessonFolderName];
                var fileName = Path.GetFileName(filePath);
                //var fileUrl = S3LessonFolder + "/" + fileName;
                var fileUrl = S3LessonFolder + "/" + filePath;
                var result = s3Service.DownloadFile(bucketName, fileUrl);
                if (result.IsSuccess)
                {
                    var mimeType = GetMimeType(fileName);
                    return File(result.ReturnStream, mimeType, fileName);
                }
            }

            ViewData["message"] = "Invalid download link";

            return View("Error");
        }

        private string GetMimeType(string fileName)
        {
            return string.IsNullOrWhiteSpace(fileName) ? "application/octet-stream" : MimeMapping.GetMimeMapping(fileName);
        }

        public ActionResult DownloadGuide(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var bucketName = LinkitConfigurationManager.GetS3Settings().GuideBucketName;
                var S3GuideFolder = ConfigurationManager.AppSettings[ContaintUtil.AppSettingGuideFolderName];
                var fileName = Path.GetFileName(filePath);
                //var fileUrl = S3GuideFolder + "/" + fileName;
                var fileUrl = S3GuideFolder + "/" + filePath;
                var result = s3Service.DownloadFile(bucketName, fileUrl);
                if (result.IsSuccess)
                {
                    return File(result.ReturnStream, "text/plain", fileName);
                }
            }

            ViewData["message"] = "Invalid download link";

            return View("Error");
        }
        public ActionResult DownloadAnswerKeySampleFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var bucketName = LinkitConfigurationManager.GetS3Settings().AnswerKeySampleFileBucketName;
                var S3GuideFolder = ConfigurationManager.AppSettings[ContaintUtil.AppSettingAnswerKeySampleFileFolderName];
                var fileName = Path.GetFileName(filePath);
                //var fileUrl = S3GuideFolder + "/" + fileName;
                var fileUrl = S3GuideFolder + "/" + filePath;
                var result = s3Service.DownloadFile(bucketName, fileUrl);
                if (result.IsSuccess)
                {
                    return File(result.ReturnStream, "text/plain", fileName);
                }
            }

            ViewData["message"] = "Invalid download link";

            return View("Error");
        }

        public ActionResult DownloadLessonActivateInstruction(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var client = new RestClient("https://app.activateinstruction.org");

                var consumerKey = ConfigurationManager.AppSettings["ActivateInstruction_ConsumerKey"];
                var consumerSecret = ConfigurationManager.AppSettings["ActivateInstruction_ConsumerSecret"];
                var accessToken = ConfigurationManager.AppSettings["ActivateInstruction_AccessToken"];
                var accessTokenSecret = ConfigurationManager.AppSettings["ActivateInstruction_AccessTokenSecret"];
                client.Authenticator = OAuth1Authenticator.ForProtectedResource(consumerKey, consumerSecret, accessToken, accessTokenSecret);

                filePath = filePath.Replace("https://app.activateinstruction.org", "");
                var request = new RestRequest(filePath, Method.GET);

                var response = client.Execute(request);
                
                if (response != null && response.Headers.Count > 2)
                {
                    var fileName = response.Headers[2].Value.ToString().Replace("attachment; filename=","").Replace("\"","");
                    return File(response.RawBytes, "text/plain", fileName);
                }
            }

            ViewData["message"] = "Invalid download link";

            return View("Error");
        }
    }
}
