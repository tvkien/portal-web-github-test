using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc.Internal;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class ManageProgramController : BaseController
    {
        private readonly StudentProgramService _studentProgramService;
        private readonly ProgramService _programService;
        private readonly VulnerabilityService _vulnerabilityService;
        public ManageProgramController(StudentProgramService studentProgramService, ProgramService programService, VulnerabilityService vulnerabilityService)
        {
            this._studentProgramService = studentProgramService;
            this._programService = programService;
            this._vulnerabilityService = vulnerabilityService;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageProgram)]
        public ActionResult Index()
        {
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            return View();
        }

        public ActionResult StudentProgram(int? programId)
        {
            if (!programId.HasValue)
            {
                programId = 0;
            }
           
            ViewBag.ProgramId = programId;
            ViewBag.ProgramName = string.Empty;
            // get program
            var program = _programService.GetProgramById(programId.Value);
            if (program != null)
            {
                ViewBag.ProgramName = program.Name;
                if (!Util.HasRightOnDistrict(CurrentUser, program.DistrictID))
                {
                    return new EmptyResult();
                }
                if (!_vulnerabilityService.HasRigtToEditProgram(CurrentUser, program.Id, CurrentUser.DistrictId.GetValueOrDefault()))
                {
                    return new EmptyResult();
                }
            }
            else
            {
                ViewBag.ProgramId = 0;
            }
            return PartialView("StudentProgram");
        }
        public ActionResult LoadPrograms(int? districtId)
        {
            if (!CurrentUser.IsPublisher)
                districtId = CurrentUser.DistrictId.GetValueOrDefault();

            var data = _programService.LoadPrograms(CurrentUser, districtId.GetValueOrDefault(), DateTime.UtcNow.ToString()).ToList().
                Select(x => new ProgramListViewModel()
                {
                    ProgramId = x.ProgramId,
                    Name = x.Name,
                    AccessLevelId = x.AccessLevelId,
                    Code = x.Code,
                    StudentNumber = x.StudentNumber
                });
            var parser = new DataTableParser<ProgramListViewModel>();
            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadCreateProgram(int? districtId)
        {
            if (!CurrentUser.IsPublisher)
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            ViewBag.DistrictId = districtId.GetValueOrDefault();
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            ViewBag.IsSchoolAdmin = CurrentUser.IsSchoolAdmin;
            return PartialView("_Create");
        }
        [ValidateInput(false)]
        public ActionResult CreateProgram(int districtId, string name, int? accessLevelId, string code)
        {
            if(string.IsNullOrWhiteSpace(name))
                return Json(new { Success = false, ErrorMessage = "Name is required." }, JsonRequestBehavior.AllowGet);
            if (!accessLevelId.HasValue || accessLevelId.Value < 0)
            {
                return Json(new { Success = false, ErrorMessage = "Access level is required." }, JsonRequestBehavior.AllowGet);
            }
            if (CurrentUser.IsSchoolAdmin && accessLevelId == 2)
            {
                return Json(new { Success = false, ErrorMessage = "Invalid request." }, JsonRequestBehavior.AllowGet);
            }

            //avoid modify ajax parameters
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "." }, JsonRequestBehavior.AllowGet);
            }
            name = HttpUtility.UrlDecode(name);
            code = HttpUtility.UrlDecode(code);
            if (name != null)
            {
                name = name.Trim();
            }
            if (code != null)
            {
                code = code.Trim();
            }
            //check if this program is existing or not
            var existing = _programService.GetPrograms().Any(x => x.DistrictID == districtId && x.Name.ToLower().Equals(name.ToLower()));
            if (existing)
            {
                return Json(new { Success = false, ErrorMessage = "A program with that name already exists" }, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(code))
            {
                existing = _programService.GetPrograms().Any(x => x.DistrictID==districtId && x.Code.ToLower().Equals(code.ToLower()));
                if (existing)
                {
                    return Json(new { Success = false, ErrorMessage = "A program with that code already exists" }, JsonRequestBehavior.AllowGet);
                }
                
            }
            var program = new Program()
                          {
                              DistrictID = districtId,
                              Name = name,
                              AccessLevelID = accessLevelId,
                              Code = code
                          };
            _programService.Save(program);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadEditProgram(int programId)
        {
            if (!_vulnerabilityService.HasRigtToEditProgram(CurrentUser, programId, CurrentUser.DistrictId.GetValueOrDefault()))
            {
                return RedirectToAction("Index", "ManageProgram");
            }
            
            var model = _programService.GetProgramById(programId);
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            ViewBag.IsSchoolAdmin = CurrentUser.IsSchoolAdmin;
            return PartialView("_Edit", model);
        }
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditProgram(Program model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return Json(new { Success = false, ErrorMessage = "Name is required." }, JsonRequestBehavior.AllowGet);
            if (!model.AccessLevelID.HasValue || model.AccessLevelID.Value < 0)
            {
                return Json(new { Success = false, ErrorMessage = "Access level is required." }, JsonRequestBehavior.AllowGet);
            }
            if (model.Name != null)
            {
                model.Name = model.Name.Trim();
            }
            if (model.Code != null)
            {
                model.Code = model.Code.Trim();
            }
            //avoid modify ajax parameters
            var program = _programService.GetProgramById(model.Id);
            if (program == null)
            {
                return Json(new { Success = false, ErrorMessage = "Program does not exist." });
            }
            if (!Util.HasRightOnDistrict(CurrentUser, program.DistrictID))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "." }, JsonRequestBehavior.AllowGet);
            }
            if (!_vulnerabilityService.HasRigtToEditProgram(CurrentUser, model.Id, CurrentUser.DistrictId.GetValueOrDefault()))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this program." }, JsonRequestBehavior.AllowGet);
            }
            if (CurrentUser.IsSchoolAdmin && model.AccessLevelID == 2)
            {
                return Json(new { Success = false, ErrorMessage = "Invalid request." }, JsonRequestBehavior.AllowGet);
            }

            var existing = _programService.GetPrograms().Any(x => x.Id != model.Id && x.DistrictID == model.DistrictID && x.Name.ToLower().Equals(model.Name.ToLower()));
            if (existing)
            {
                return Json(new { Success = false, ErrorMessage = "A program with that name already exists" }, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(model.Code))
            {
                existing = _programService.GetPrograms().Any(x => x.Id != model.Id && x.DistrictID == model.DistrictID && x.Code.ToLower().Equals(model.Code.ToLower()));
                if (existing)
                {
                    return Json(new { Success = false, ErrorMessage = "A program with that code already exists" }, JsonRequestBehavior.AllowGet);
                }

            }
            try
            {
                _programService.Save(model);
            }
            catch
            {
                return Json(new { Success = false, ErrorMessage = "There was some error happend. Please contact admin." }, JsonRequestBehavior.AllowGet);    
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteProgram(int programId)
        {
            //avoid modify ajax parameters
            var program = _programService.GetProgramById(programId);
            if (program == null)
            {
                return Json(new { Success = false, ErrorMessage = "Program does not exist." }, JsonRequestBehavior.AllowGet);
            }
            if (!Util.HasRightOnDistrict(CurrentUser, program.DistrictID))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "." }, JsonRequestBehavior.AllowGet);
            }
            if (!_vulnerabilityService.HasRigtToEditProgram(CurrentUser, programId, CurrentUser.DistrictId.GetValueOrDefault()))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this program." }, JsonRequestBehavior.AllowGet);
            }
            if (_programService.DeleteProgramAndAssociationItems(programId))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public ActionResult GetStudentsOfProgram(int programId, string studentCode, string firstName, string lastName,bool? firstLoad)
        {
            if (!_vulnerabilityService.HasRigtToEditProgram(CurrentUser, programId, CurrentUser.DistrictId.GetValueOrDefault()))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this program." }, JsonRequestBehavior.AllowGet);
            }
            
            //if (firstLoad.HasValue && firstLoad.Value)
            //{
            //    var emptyDate = new List<StudentProgramManageItemViewModel>();
            //    return Json(new DataTableParser<StudentProgramManageItemViewModel>().Parse(emptyDate.AsQueryable(), true), JsonRequestBehavior.AllowGet);
            //}
            var parser = new DataTableParser<StudentProgramManageItemViewModel>();
            var data = _studentProgramService.GetStudentProgramView().Where(x => x.ProgramID == programId);

            if (!string.IsNullOrWhiteSpace(studentCode))
            {
                studentCode = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(studentCode));
                studentCode = Util.ProcessWildCharacters(studentCode);
                data = data.Where(x => x.StudentCode.ToLower().Contains(studentCode.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                firstName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(firstName));
                firstName = Util.ProcessWildCharacters(firstName);
                data = data.Where(x => x.FirstName.ToLower().Contains(firstName.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                lastName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(lastName));
                lastName = Util.ProcessWildCharacters(lastName);
                data = data.Where(x => x.LastName.ToLower().Contains(lastName.ToLower()));
            }
            var activeStudentIdList = _studentProgramService.GetActiveStudentsOfProgram(programId,DateTime.UtcNow.ToString());
            data = data.Where(x => activeStudentIdList.Contains(x.StudentID));
            var data1 = data.Select(x => new StudentProgramManageItemViewModel()
            {
                StudentProgramId = x.StudentProgramID,
                StudentCode = x.StudentCode,
                FirstName = x.FirstName,
                LastName = x.LastName
            });
            return Json(parser.Parse(data1, true), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShowAssignStudentDialog(int programId)
        {
            if (!_vulnerabilityService.HasRigtToEditProgram(CurrentUser, programId, CurrentUser.DistrictId.GetValueOrDefault()))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this program." }, JsonRequestBehavior.AllowGet);
            }
            
            ViewBag.ProgramId = programId;
            //check if the program has been deleted or not
            var program = _programService.GetProgramById(programId);
            if (program == null)
            {
                ViewBag.ProgramId = 0;
                ViewBag.Message = "This program has been deleted by someone already.";
                return PartialView("_Notifycation");
            }
            else
            {
                return PartialView("_AssignStudent");
            }
        }

        public ActionResult CheckProgramExisting(int programId)
        {
            var program = _programService.GetProgramById(programId);
            return Json(new { IsExisting = program!=null }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public ActionResult GetUnassignedStudentsOfProgram(int programId, string studentCode, string firstName, string lastName, bool? firstLoad)
        {
            if (!_vulnerabilityService.HasRigtToEditProgram(CurrentUser, programId, CurrentUser.DistrictId.GetValueOrDefault()))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this program." }, JsonRequestBehavior.AllowGet);
            }
            
         
            var parser = new DataTableParser<StudentViewModel>();
            //get the program
            var program = _programService.GetProgramById(programId);
            int districtId = CurrentUser.DistrictId.GetValueOrDefault();
            if (program != null)
            {
                districtId = program.DistrictID;
            }
            studentCode = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(studentCode));
            studentCode = Util.ProcessWildCharacters(studentCode);

            firstName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(firstName));
            firstName = Util.ProcessWildCharacters(firstName);

            lastName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(lastName));
            lastName = Util.ProcessWildCharacters(lastName);
            var data = _studentProgramService.GetUnassignedStudents(programId, studentCode, firstName, lastName,
                districtId).Select(x => new StudentViewModel()
                                               {
                                                   StudentID = x.Id,
                                                   FirstName = x.FirstName,
                                                   LastName = x.LastName,
                                                   Code = x.Code
                                               }).ToList();
            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignStudents(int programId,string selectedStudentIds)
        {
            //avoid modify ajax parameters
            var program = _programService.GetProgramById(programId);
            if (program == null)
            {
                return Json(new { Success = false, ErrorMessage = "Program does not exist." }, JsonRequestBehavior.AllowGet);
            }
            if (!Util.HasRightOnDistrict(CurrentUser, program.DistrictID))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "." }, JsonRequestBehavior.AllowGet);
            }
            if (!_vulnerabilityService.HasRigtToEditProgram(CurrentUser, programId, CurrentUser.DistrictId.GetValueOrDefault()))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this program." }, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(selectedStudentIds))
            {
                string[] studentIds = selectedStudentIds.Split(',');
                foreach (var studentId in studentIds)
                {
                    try
                    {
                        _studentProgramService.AddStudentProgram(Int32.Parse(studentId), programId);
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new { success = "false", errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeassignStudents(string selectedStudentProgramIds)
        {
            if (!string.IsNullOrEmpty(selectedStudentProgramIds))
            {
                var studentProgramIdList = selectedStudentProgramIds.ParseIdsFromString();
                var programIdList =
                    _studentProgramService.GetAll()
                        .Where(x => studentProgramIdList.Contains(x.StudentProgramID))
                        .Select(x => x.ProgramID).Distinct()
                        .ToList();
                if (!_vulnerabilityService.HasRigtToEditPrograms(CurrentUser, programIdList))
                {
                    return Json(new { success = "false", errorMessage = "Has no right on one or more program" }, JsonRequestBehavior.AllowGet);
                }
                foreach (var studentProgramId in studentProgramIdList)
                {
                    try
                    {
                        _studentProgramService.DeleteStudentProgram(studentProgramId);
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new { success = "false", errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}
