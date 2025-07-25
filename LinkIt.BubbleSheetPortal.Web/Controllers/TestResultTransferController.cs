using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using RestSharp.Extensions;
using Twilio;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class TestResultTransferController : BaseController
    {
        private readonly TestResultTransferControllerParameters parameters;

        public TestResultTransferController(TestResultTransferControllerParameters param)
        {
            parameters = param;
        }

        //
        // GET: /TestResultTransfer/
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtTestResultTransfer)]
        public ActionResult Index()
        {
            var model = new TestResultTransferViewModel()
            {
                IsAdmin = parameters.UserServices.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull(),
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher,
                IsSchoolAdmin = CurrentUser.RoleId.Equals(8),
                IsTeacher = CurrentUser.RoleId.Equals(2),
                IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher),
                DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };
            return View(model);
        }

        public ActionResult OpenPopupTransferTestResult(TestResultTransferModel model)
        {
            if (model.TestResultIDs.HasValue())
            {
                string [] arr = model.TestResultIDs.Split(',');
                model.TotalResultSelected = arr.Length;
            }
            model.UserId = CurrentUser.Id;
            if (model.DistrictId < 0 && CurrentUser.DistrictId != null) 
                model.DistrictId = CurrentUser.DistrictId.Value;
            return PartialView("_TransferTestResultForm", model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult TransferTestResultSelected(int newClassId, string lstTestResultIds)
        {
            //Check user permission to access those test result before transfer            

            if (parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, newClassId)
                == false)
            {
                return Json(new { Success = false, Error = "Do not have permission" });
            }

            var testResultIds = new List<int>();
            var studentIds = new List<int>();           
            string[] arr = lstTestResultIds.Split(',');
            if (arr.Length > 0)
            {               
                foreach (var s in arr)
                {
                    string[] testStudentIds = s.Split('#');                   
                    int.TryParse(testStudentIds[0], out int item);
                    int.TryParse(testStudentIds[1], out int idstudent);
                    if (item > 0) testResultIds.Add(item);
                    if (idstudent > 0) studentIds.Add(idstudent);   
                }
                if (parameters.VulnerabilityService.CheckUserCanAccessTestResult(CurrentUser,
                string.Join(",",testResultIds)) == false)
                {
                    return Json(new { Success = false, Error = "Do not have permission" });
                }
                
                var vClass = parameters.ClassServices.GetClassById(newClassId);
                if (testResultIds.Count > 0 && vClass !=null)
                {
                    var classStudents = parameters.ClassServices.GetClassByStudentIds(newClassId, studentIds);
                    var studentsExitsClass = classStudents.Select(c => c.StudentID.ToString()).ToList();
                    var studentIdsNotTranfer = new List<string>();
                    var newtestResultIds = new List<int>();
                    arr.ToList().ForEach(x =>
                    {
                        var parts = x.Split('#');
                        if(parts.Length > 1) {
                            var studentIdPart = parts[1];
                            var testId = int.Parse(parts[0]);
                            if (studentsExitsClass.Contains(studentIdPart))
                                newtestResultIds.Add(testId);
                            else
                                studentIdsNotTranfer.Add(studentIdPart);
                        }
                       
                    });                   
                   
                    var studentNotTranfers = string.Empty;
                    if (studentIdsNotTranfer.Any())
                    {                       
                        studentNotTranfers = parameters.ClassServices.GetStudentsName(studentIdsNotTranfer);
                    }                    
                    if (newtestResultIds.Any())
                    {                       
                        parameters.TestResultServices.UpdateTestResultByClass(vClass, newtestResultIds);
                    }
                    return Json(new { Success = true, Data= newtestResultIds.Count, StudentNotTranfer = studentNotTranfers }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false, Error= "No testresult selected" }, JsonRequestBehavior.AllowGet);
        }
    }
}
