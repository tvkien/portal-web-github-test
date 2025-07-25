using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
	[SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    public class ChytenReportController : BaseController
    {
        private readonly ChytenReportControllerParameters parameters;
        private readonly IS3Service s3Service;

        public ChytenReportController(ChytenReportControllerParameters parameters, IS3Service s3Service)
        {
            this.parameters = parameters;
            this.s3Service = s3Service;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemChytenReport)]
        public ActionResult Index()
        {            
            return View();
        }
        public ActionResult LoadTestFilter()
        {
            TestFilterViewModel model = new TestFilterViewModel();
            model.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            model.IsPublisher = CurrentUser.IsPublisher();
            model.IsTeacher = CurrentUser.IsTeacher();
            model.UserID = CurrentUser.Id;

            return PartialView("_TestFilter", model);
        }
        public PartialViewResult LoadInstruction()
        {
            return PartialView("_Instructions");
        }

        [HttpGet]
        public ActionResult GetAllDistrict()
        {
            var data =
                parameters.DistrictStateServices.GetDistricts().Where(o => o.DistrictId == CurrentUser.DistrictId)
                    .Select(o => new ListItem { Id = o.DistrictId, Name = o.DistrictNameCustom });
            if (CurrentUser.IsPublisher())
            {
                data = parameters.DistrictStateServices.GetDistricts().Select(o => new ListItem { Id = o.DistrictId, Name = o.DistrictNameCustom });
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetTestBankByDistrictId(int districtId)
        {
            var schoolIdString = string.Empty;
            if (CurrentUser.IsSchoolAdmin)
            {
                var schoolIds = parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(x => x.SchoolId).ToList();
                if (schoolIds.Any())
                    schoolIdString = string.Join(";", schoolIds);
            }

            var data = parameters.ChytenReportServices.GetBankByDistrictId(districtId, schoolIdString, CurrentUser.Id, CurrentUser.RoleId);
            var districtDecode =
                parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.CHYTEN_BankStudentTest);
            if (districtDecode.Any())
            {
                var bankIdString = districtDecode.First().Value;
                if (!string.IsNullOrWhiteSpace(bankIdString))
                {
                    var bankIds = bankIdString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                    data = data.Where(x => bankIds.Contains(x.Id.ToString())).OrderBy(x => x.Name).ToList();
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchoolTestResultDistrict(int districtId, int bankId)
        {
            var schoolIdString = string.Empty;
            if (CurrentUser.IsSchoolAdmin)
            {
                var schoolIds =
                    parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(CurrentUser.Id)
                        .Select(x => x.SchoolId)
                        .ToList();
                if (schoolIds.Any())
                    schoolIdString = string.Join(";", schoolIds);
            }

            var data = parameters.ChytenReportServices.GetSchoolByDistrictIdAndBankId(districtId, bankId, schoolIdString, CurrentUser.Id, CurrentUser.RoleId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTeacherTestResultDistrict(int districtId, int bankId, int schoolId)
        {
            var schoolIdString = string.Empty;
            if (CurrentUser.IsSchoolAdmin)
            {
                var schoolIds =
                    parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(CurrentUser.Id)
                        .Select(x => x.SchoolId)
                        .ToList();
                if (schoolIds.Any())
                    schoolIdString = string.Join(";", schoolIds);
            }

            var data = parameters.ChytenReportServices.GetTeacherByDistrictIdAndBankIdAndSchoolId(districtId, bankId, schoolId, schoolIdString, CurrentUser.Id, CurrentUser.RoleId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTermTestResultDistrict(int districtId, int bankId, int schoolId, int teacherId)
        {
            var schoolIdString = string.Empty;
            if (CurrentUser.IsSchoolAdmin)
            {
                var schoolIds =
                    parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(CurrentUser.Id)
                        .Select(x => x.SchoolId)
                        .ToList();
                if (schoolIds.Any())
                    schoolIdString = string.Join(";", schoolIds);
            }

            var data = parameters.ChytenReportServices.GetTermsHaveTestResult(districtId, bankId, schoolId, teacherId, schoolIdString, CurrentUser.Id, CurrentUser.RoleId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetClassTestResultDistrict(int districtId, int bankId, int schoolId, int teacherId, int termId)
        {
            var schoolIdString = string.Empty;
            if (CurrentUser.IsSchoolAdmin)
            {
                var schoolIds =
                    parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(CurrentUser.Id)
                        .Select(x => x.SchoolId)
                        .ToList();
                if (schoolIds.Any())
                    schoolIdString = string.Join(";", schoolIds);
            }

            var data = parameters.ChytenReportServices.GetClassedHaveTestResult(districtId, bankId, schoolId, teacherId, termId, schoolIdString, CurrentUser.Id, CurrentUser.RoleId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadTestResultByFilter(int districtId, int bankId, int classId,
                                               int schoolId, int teacherId, int termId)
        {
            SpecializedTestResultFilterViewModel model = new SpecializedTestResultFilterViewModel()
            {
                DistrictId = districtId,
                BankId = bankId,
                ClassId = classId,
                SchoolId = schoolId,
                TeacherId = teacherId,
                TermrId = termId
            };
            return PartialView("_TestResultByFilter", model);
        }
        public ActionResult GetTestResultToView(int districtId, int bankId, int classId, int schoolId, int teacherId, int termId)
        {
            var schoolIdString = string.Empty;

            if (CurrentUser.IsSchoolAdmin)
            {
                var schoolIds = parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(x => x.SchoolId).ToList();
                if (schoolIds.Any())
                    schoolIdString = string.Join(";", schoolIds);
            }
            
            var testResults = parameters.ChytenReportServices.GetTestResultFilter(districtId, bankId, schoolId, teacherId, classId, termId, schoolIdString, CurrentUser.Id, CurrentUser.RoleId)
                .Select(x => new SpecializedResultFilterViewModel()
                {
                    ID = x.StudentId,
                    TestNameCustom = x.BankName,
                    SchoolName = x.SchoolName,
                    TeacherCustom = x.TeacherCustom,
                    ClassNameCustom = x.ClassNameCustom,
                    StudentCustom = x.StudentCustom,
                    BankId = x.BankId
                }).AsQueryable();
            var parser = new DataTableParser<SpecializedResultFilterViewModel>();
            return Json(parser.Parse(testResults), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllStudentIds(int districtId, int bankId, int classId, int schoolId, int teacherId, int termId)
        {
            var schoolIdString = string.Empty;

            if (CurrentUser.IsSchoolAdmin)
            {
                var schoolIds = parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(x => x.SchoolId).ToList();
                if (schoolIds.Any())
                    schoolIdString = string.Join(";", schoolIds);
            }

            var data =
                parameters.ChytenReportServices.GetTestResultFilter(districtId, bankId, schoolId, teacherId, classId,
                    termId, schoolIdString, CurrentUser.Id, CurrentUser.RoleId)
                    .Select(x => new {studentId = x.StudentId, bankId = x.BankId}).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult Generate(string studentBankIdString, string reportFileName)
        {
            var generateReportUrl = ConfigurationManager.AppSettings["ChytenReportGenerateUrl"];

            var request = (HttpWebRequest)WebRequest.Create(generateReportUrl);

            var postData = "studentBankIdString=" + studentBankIdString;
            postData += "&reportFileName=" + reportFileName;
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();    

            return Json(new { IsSuccess = true });
        }

	    [HttpPost]
        [AjaxOnly]
        public ActionResult CheckS3FileExisted(string fileName)
        {
            var folder = LinkitConfigurationManager.GetS3Settings().ChytenReportFolder;
            var bucketName = LinkitConfigurationManager.GetS3Settings().ChytenReportBucket;
            var result = s3Service.DownloadFile(bucketName, folder + "/" + fileName);

            if (result.IsSuccess)
            {
                var s3Url = string.Format("https://s3.amazonaws.com/{0}/{1}/{2}", bucketName, folder, fileName);
                return Json(new { Result = true, Url = s3Url });
            }
            else
            {
                return Json(new { Result = false });
            }
        }
    }     
}
