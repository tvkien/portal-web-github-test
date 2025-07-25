using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Common;
using System.Configuration;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Models.DataLockerForStudent;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using System.Linq;
using LinkIt.BubbleSheetPortal.Services;
using System;
using Lokad.Cloud.Storage;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class DataLockerForStudentController : BaseController
    {
        private readonly DataLockerForStudentControllerParameters _parameters;
        private IS3Service _s3Service;
        private readonly IDocumentManagement _documentManagement;
        public DataLockerForStudentController(DataLockerForStudentControllerParameters parameters, IS3Service s3Service, IDocumentManagement documentManagement)
        {
            _parameters = parameters;
            _s3Service = s3Service;
            _documentManagement = documentManagement;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.AttachmentForStudent)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.AttachmentForStudent)]
        public ActionResult AttachmentForStudents()
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            var assessmentArtifactFileTypeGroups = Mapper.Map<List<EntryResultArtifactFileTypeGroupViewModel>>(_parameters.DistrictDecodeService.GetAssessmentArtifactFileTypeGroups(CurrentUser.DistrictId ?? 0));
            ViewBag.AssessmentArtifactFileTypeGroups = jsonSerializer.Serialize(assessmentArtifactFileTypeGroups);
            ViewBag.CurrentUserId = CurrentUser.Id;
            return View();
        }
        [HttpGet]
        public JsonResult GetAttachmentForStudent(AttachementForStudentRequest request)
        {
            var paggingRequest = MappingGetAttachmentForStudentRequest(request);
            paggingRequest.UserId = CurrentUser.Id;
            var timeZoneId = _parameters.StateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
            TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            paggingRequest.CurrentDateDistrict = TimeZoneInfo.ConvertTime(DateTime.UtcNow, targetTimeZone);
            var attachmentForStudents = _parameters.DataLockerForStudentService.GetListAttachmentForStudents(paggingRequest);
            GetAttachmentInfoForStudent(attachmentForStudents.Data);

            var result = new GenericDataTableResponse<AttachmentForStudentModel>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = attachmentForStudents?.Data.ToList(),
                iTotalDisplayRecords = attachmentForStudents.TotalRecord,
                iTotalRecords = attachmentForStudents.TotalRecord
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PublishDataLockerPreference(PublishFormToStudentModels model)
        {
            try
            {
                var result = _parameters.DataLockerForStudentService.PublishDataLockerPreference(model, CurrentUser.Id, CurrentUser.DistrictId.Value, GenerateClassTestCode());
                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UnPublishDataLockerPreference(int virtualTestID, int classID = 0)
        {
            var result = _parameters.DataLockerForStudentService.UnPublishDataLockerPreference(virtualTestID, classID);
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveStudentAttachments(SaveStudentAttachmentsParameters model)
        {
            var results = _parameters.DataLockerForStudentService.SaveStudentArtifacts(model, CurrentUser.Id);
            if (results != null)
            {
                var documentIds = results.Where(p => p.DocumentGuid != null && p.DocumentGuid != Guid.Empty).Select(p => p.DocumentGuid);
                var documentInfos = _documentManagement.GetDocumentInfoList(documentIds).ToDictionary(x => (Guid?)x.DocumentGuid, y => y.FileName);
                if (documentInfos != null && documentInfos.Any())
                {
                    foreach (var result in results)
                    {
                        if (documentInfos.TryGetValue(result.DocumentGuid.GetValueOrDefault(), out string fileName))
                        {
                            result.Name = fileName;
                        }
                    }
                }
            }

            return Json(results);
        }
        public ActionResult GetAttachmentUrl(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return Json(string.Empty);
            var fileNamePath = string.Format("{0}/{1}", _parameters.DTLFolder.RemoveEndSlash(), fileName);
            var s3Url = _s3Service.GetPublicUrl(_parameters.DTLBucket, fileNamePath);
            return Json(new { data = s3Url }, JsonRequestBehavior.AllowGet);
        }
        private string GenerateClassTestCode()
        {
            var objCon = _parameters.ConfigurationService.GetConfigurationByKey(Constanst.ClassTestCodeLength);
            int iTestCodeLength = 5;
            if (objCon != null)
            {
                iTestCodeLength = CommonUtils.ConverStringToInt(objCon.Value, 5);
            }

            string strTestCodePrefix = ConfigurationManager.AppSettings["SurveyTestCodePrefix"] ?? "001";
            var testCode = _parameters.TestCodeGenerator.GenerateTestCode(iTestCodeLength, strTestCodePrefix);
            return testCode;
        }
        private GetDatalockerForStudentPaginationRequest MappingGetAttachmentForStudentRequest(AttachementForStudentRequest request)
        {
            var response = new GetDatalockerForStudentPaginationRequest()
            {
                VirtualTestName = request.VirtualTestName,
                TeacherName = request.TeacherName,
                ClassName= request.ClassName,
                PageSize = request.iDisplayLength,
                StartRow = request.iDisplayStart                
            };
            if (!string.IsNullOrWhiteSpace(request.sColumns) && request.iSortCol_0.HasValue)
            {
                var columns = request.sColumns.Split(',');
                response.SortColumn = columns[request.iSortCol_0.Value];
                response.SortDirection = request.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
            }
            if (!string.IsNullOrWhiteSpace(request.sSearch))
            {
                response.SearchString = request.sSearch.Trim();
            }
            return response;
        }

        private void GetAttachmentInfoForStudent(IEnumerable<AttachmentForStudentModel> attachmentForStudents)
        {
            var documentIds = attachmentForStudents.SelectMany(p => p.Artifacts.Where(w => w.DocumentGuid != null && w.DocumentGuid != Guid.Empty).Select(s => s.DocumentGuid));
            if (!documentIds.Any()) return;

            var documentInfos = _documentManagement.GetDocumentInfoList(documentIds).ToDictionary(x => (Guid?)x.DocumentGuid, y => y.FileName);
            if (documentInfos != null && documentInfos.Any())
            {
                foreach (var attachmentForStudent in attachmentForStudents)
                {
                    foreach (var artifact in attachmentForStudent.Artifacts.Where(p => !p.IsLink))
                    {
                        if (documentInfos.TryGetValue(artifact.DocumentGuid.GetValueOrDefault(), out string fileName))
                        {
                            artifact.Name = fileName;
                        }
                    }
                }
            }
        }
    }
}
