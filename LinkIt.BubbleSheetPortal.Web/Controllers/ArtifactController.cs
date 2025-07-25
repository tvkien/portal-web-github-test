using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [VersionFilter]
    public class ArtifactController : BaseController
    {
        private readonly IAnswerAttachmentService _answerAttachmentService;
        private readonly IDocumentManagement _documentManagement;
        public ArtifactController(IAnswerAttachmentService answerAttachmentService, IDocumentManagement documentManagement)
        {
            _answerAttachmentService = answerAttachmentService;
            _documentManagement = documentManagement;
        }

        public ActionResult View(Guid documentGuid)
        {
            string viewOnDsServerSrc = string.Format(ConfigurationManager.AppSettings[Constanst.AdminReporting.Configuration.REPORTING_URL]
                                                     + Constanst.AdminReporting.Endpoints.VIEW_ARTIFACT, documentGuid);

            var licode = HttpContext.GetLICodeFromRequest();
            viewOnDsServerSrc = viewOnDsServerSrc.Replace("[LICode]", licode);
            ViewBag.ArtifactViewSrc = viewOnDsServerSrc;
            return View();
        }

        [HttpGet]
        public ActionResult DownloadFile(string filePath)
        {
            var s3Settings = LinkitConfigurationManager.GetS3Settings();
            var fileContent = _answerAttachmentService.DownloadFile(s3Settings, filePath);

            if (fileContent == null)
            {
                return Json(new { success = false, messageError = "An error has occurred, please contact your administrator." }, JsonRequestBehavior.AllowGet);
            }

            var fileName  = Path.GetFileName(filePath);
            return Json(new { success = true, filename = fileName, fileContent = fileContent }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CreatePresignedLink(UploadRequestDto dto)
        {
            var userId = CurrentUser.Id;
            var folderDistrictId = GetFolderDistrictId();
            var presignedLink = _documentManagement.CreatePresignedLinkAsync(dto, userId, folderDistrictId);
            return Json(presignedLink);
        }

        [HttpGet]
        public ActionResult GetPresignedLink(Guid documentGuid)
        {
            var presignedLink = _documentManagement.GetPresignedLinkAsync(documentGuid);
            return Json(presignedLink, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AliveConfirm(AliveConfirmDto dto)
        {
            _documentManagement.AliveConfirm(dto);

            return Json(dto);
        }
        [HttpPost]
        public ActionResult CancelUploadMultiPart(CancelUploadDto dto)
        {
            var result = _documentManagement.CancelUploadMultiPart(dto);
            return Json(result);
        }
        [HttpPost]
        public ActionResult UpdatePathEtags(UpdatePathEtagsDto dto)
        {
            var result = _documentManagement.UpdatePathEtags(dto);
            return Json(result);
        }
        [HttpPost]
        public ActionResult CreateInfo(CreateDocumentDto dto)
        {
            dto.Author = CurrentUser.Id;
            dto.DistrictId = GetFolderDistrictId();
            var result = _documentManagement.CreateDocument(dto);
            return Json(result);
        }
        public string GetFolderDistrictId()
        {
            if (CurrentUser.DistrictId == Constanst.ENTERPRISE_DISTRICT && CurrentUser.IsPublisher)
            {
                return Constanst.ENTERPRISE_FOLDER;
            }
            return CurrentUser.DistrictId.ToString();
        }
    }
}
