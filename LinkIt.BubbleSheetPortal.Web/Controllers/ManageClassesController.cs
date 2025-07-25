using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting.Native;
using Envoc.Core.Shared.Extensions;
using FluentValidation.Results;
using iTextSharp.text.pdf.qrcode;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.Classes;
using LinkIt.BubbleSheetPortal.Models.DTOs.DistrictDecode;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageStudent;
using LinkIt.BubbleSheetPortal.Models.DTOs.UserDefinedTypesDto;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ManageClassEnhancement;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using Twilio;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class ManageClassesController : BaseController
    {
        private readonly ManageClassesControllerParameters parameters;

        public ManageClassesController(ManageClassesControllerParameters parameters)
        {
            this.parameters = parameters;
        }

        private enum SchoolClassCurrentSelection
        {
            NotSelect = 0,
            District,
            School,
            Teacher,
            Class
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageSchools)]
        public ActionResult ManageSchoolAndClass()
        {
            var model = new SchoolAndClassViewModel
            {
                RoleId = CurrentUser.RoleId,
                IsPublisher = CurrentUser.IsPublisher,
                IsDistrictAdmin = CurrentUser.IsDistrictAdmin,
                DefaultDistrictId = CurrentUser.DistrictId.GetValueOrDefault(0),
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                ListDistricIds = CurrentUser.IsNetworkAdmin ? CurrentUser.GetMemberListDistrictId() : null
            };
            var district = parameters.DistrictService.GetDistrictById(model.DefaultDistrictId);
            model.DefaultDistrictName = district.IsNull() ? string.Empty : district.Name;
            model.UserId = CurrentUser.Id;
            model.UserLastName = parameters.UserService.GetUserById(CurrentUser.Id)?.LastName;
            System.Web.HttpContext.Current.Session["EditStudentSource"] = EditStudentSource.FromManageSchools;
            SetModelSelectedState(model);

            ViewBag.RoleID = CurrentUser.RoleId;
            ViewBag.CurrentDistrictID = CurrentUser.DistrictId.GetValueOrDefault();
            return View(model);
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageSchools)]
        public ActionResult ManageSchoolClass()
        {
            return View();
        }

        private void SetModelSelectedState(SchoolAndClassViewModel model)
        {
            switch (SchoolClassCurrentButtonSelection)
            {
                case SchoolClassCurrentSelection.District:
                    model.CurrentSelectedDistrictId = SelectedDistrictID;
                    break;
                case SchoolClassCurrentSelection.School:
                    model.CurrentSelectedDistrictId = SelectedDistrictID;
                    model.CurrentSelectedSchoolId = SelectedSchoolID;
                    break;
                case SchoolClassCurrentSelection.Teacher:
                    model.CurrentSelectedDistrictId = SelectedDistrictID;
                    model.CurrentSelectedSchoolId = SelectedSchoolID;
                    model.CurrentSelectedTeacherId = SelectedTeacherID;
                    break;
                case SchoolClassCurrentSelection.Class:
                    model.CurrentSelectedDistrictId = SelectedDistrictID;
                    model.CurrentSelectedSchoolId = SelectedSchoolID;
                    model.CurrentSelectedTeacherId = SelectedTeacherID;
                    model.CurrentSelectedClassId = SelectedClassID;
                    break;
                case SchoolClassCurrentSelection.NotSelect:
                    model.CurrentSelectedDistrictId = 0;
                    model.CurrentSelectedSchoolId = 0;
                    model.CurrentSelectedTeacherId = 0;
                    model.CurrentSelectedClassId = 0;
                    break;
                default:
                    model.CurrentSelectedDistrictId = 0;
                    model.CurrentSelectedSchoolId = 0;
                    model.CurrentSelectedTeacherId = 0;
                    model.CurrentSelectedClassId = 0;
                    break;
            }
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageClasses)]
        public ActionResult ManageClass(bool isNewSession = false)
        {
            var model = new SchoolAndClassViewModel
            {
                RoleId = CurrentUser.RoleId,
                DefaultDistrictId = CurrentUser.DistrictId.GetValueOrDefault(0),
                CurrentSelectedDistrictId = 0,
                CurrentSelectedSchoolId = 0,
                CurrentSelectedTeacherId = (CurrentUser.RoleId == 2) ? CurrentUser.Id : 0,
                CurrentSelectedClassId = 0,
                UserId = CurrentUser.Id,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                IsPublisher = CurrentUser.IsPublisher
            };

            var schools = parameters.UserSchoolService
                            .GetSchoolsUserHasAccessTo(CurrentUser.Id).ToArray()
                            //.Where(x => x.DistrictId == CurrentUser.DistrictId.GetValueOrDefault(0)) //todo: Investigate why this doesn't ger returned with a district id
                            .ToList();

            if (schools.Count == 1)
            {
                model.CurrentSelectedSchoolId = schools[0].SchoolId.GetValueOrDefault(0);
            }

            var district = parameters.DistrictService.GetDistrictById(model.DefaultDistrictId);
            model.DefaultDistrictName = district.IsNull() ? string.Empty : district.Name;
            model.UserLastName = parameters.UserService.GetUserById(CurrentUser.Id)?.LastName;
            System.Web.HttpContext.Current.Session["EditStudentSource"] = EditStudentSource.FromManageClasses;
            ViewBag.IsNewSession = isNewSession;
            return View(model);
        }

        private int SelectedDistrictID
        {
            get { return GetSessionValue("SelectedDistrictID"); }
            set { System.Web.HttpContext.Current.Session["SelectedDistrictID"] = value; }
        }

        private int SelectedSchoolID
        {
            get { return GetSessionValue("SelectedSchoolID"); }
            set { System.Web.HttpContext.Current.Session["SelectedSchoolID"] = value; }
        }

        private int SelectedTeacherID
        {
            get { return GetSessionValue("SelectedTeacherID"); }
            set { System.Web.HttpContext.Current.Session["SelectedTeacherID"] = value; }
        }

        private int SelectedClassID
        {
            get { return GetSessionValue("SelectedClassID"); }
            set { System.Web.HttpContext.Current.Session["SelectedClassID"] = value; }
        }

        private int AddNewStudentToClassId
        {
            get { return GetSessionValue("AddNewStudentToClassId"); }
            set { System.Web.HttpContext.Current.Session["AddNewStudentToClassId"] = value; }
        }

        private int GetSessionValue(string valueName)
        {
            if (System.Web.HttpContext.Current.Session[valueName].IsNull())
            {
                return 0;
            }

            int value;
            if (int.TryParse(System.Web.HttpContext.Current.Session[valueName].ToString(), out value))
            {
                return value;
            }
            return 0;
        }

        private SchoolClassCurrentSelection SchoolClassCurrentButtonSelection
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["SchoolClassCurrentButtonSelection"].IsNull())
                {
                    return SchoolClassCurrentSelection.NotSelect;
                }
                var schoolClassCurrentButtonSelection = System.Web.HttpContext.Current.Session["SchoolClassCurrentButtonSelection"].ToString();
                return (SchoolClassCurrentSelection)Enum.Parse(typeof(SchoolClassCurrentSelection), schoolClassCurrentButtonSelection);
            }
            set
            {
                System.Web.HttpContext.Current.Session["SchoolClassCurrentButtonSelection"] = value.ToString();
            }
        }

        [HttpGet]
        public ActionResult GetTeacherBySchool(int schoolId, int districtId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var districtIdInput = districtId == 0 ? CurrentUser.DistrictId.GetValueOrDefault() : districtId;

            var data = parameters.UserSchoolService.GetTeacherBySchooolIdProc(schoolId, CurrentUser.Id, CurrentUser.RoleId, districtIdInput)
                        .Select(o => new ListItem { Id = o.UserId, Name = o.DisplayName }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTermDistrict(int districtId)
        {
            var data = parameters.DistrictTermService.GetDistrictTermByDistrictID(districtId).Select(x => new ListItem
            {
                Id = x.DistrictTermID,
                Name = x.Name
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassType()
        {
            var data = parameters.ClassTypeService.GetClassTypes().Select(x => new ListItem
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTeachersInSchool(int schoolId)
        {

            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            SelectedSchoolID = schoolId;
            var validUserSchoolRoleId = new[] { 2, 3, 5, 8, 27 };

            var data =
                    parameters.UserSchoolService.GetTeachersBySchoolId(schoolId)
                        .Where(
                            x =>
                                validUserSchoolRoleId.Contains((int)(x.Role)) &&
                                x.UserStatusId == (int)UserStatus.Active)
                        .Select(x => new
                        {
                            Name = x.UserName,
                            x.FirstName,
                            x.LastName,
                            x.DisplayName,
                            Id = x.UserId
                        }).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

            if (CurrentUser.IsDistrictAdmin)
            {
                var districtAdminUsers = parameters.UserService.Select().Where(x => x.RoleId == 3 && ((x.Active.HasValue && x.Active.Value) || !x.Active.HasValue)
                    && x.DistrictId.HasValue && CurrentUser.DistrictId.HasValue && x.DistrictId.Value == CurrentUser.DistrictId.Value).Select(x => new
                    {
                        Name = x.UserName,
                        x.FirstName,
                        x.LastName,
                        DisplayName = string.Format("{0}, {1} ({2})", x.LastName, x.FirstName, x.UserName),
                        x.Id
                    }).ToList();
                data.AddRange(districtAdminUsers);
            }

            data = data.DistinctBy(x => x.Id).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult DoesSchoolHaveScheduledRosterUpdates(int schoolId)
        {
            var school = parameters.SchoolService.GetSchoolById(schoolId);
            if (school == null)
            {
                return HttpNotFound();
            }

            var result = parameters.DistrictRosterUploadService.DistrictHasAutoUpdatingRosters(school.DistrictId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClasses(int userId, int? schoolId)
        {
            var data = new List<ListItem>();
            var classUsers = parameters.ClassUserService.GetClassUsersByUserId(userId);
            if (!schoolId.HasValue)
            {
                data.AddRange(classUsers.Select(classUser => parameters.ClassService.GetClassById(classUser.ClassId)).Select(singleClass => new ListItem { Id = singleClass.Id, Name = singleClass.Name }));
            }
            else
            {
                if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId.Value))
                {
                    return Json(new { success = false, message = "Has no right", },
                        JsonRequestBehavior.AllowGet);
                }

                SelectedSchoolID = schoolId.Value;
                foreach (var classUser in classUsers)
                {
                    var singleClass = parameters.ClassService.GetClassById(classUser.ClassId);
                    if (singleClass.IsNotNull() && singleClass.SchoolId.Equals(schoolId.Value))
                    {
                        data.Add(new ListItem { Id = singleClass.Id, Name = singleClass.Name });
                    }
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClasseBySchoolUserAndTerm(int userId, int? schoolId, int districtTermId)
        {
            var vSchoolId = schoolId.HasValue ? schoolId.Value : 0;

            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, vSchoolId))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var data = parameters.ClassService.GetClassesBySchoolIdAndTermIdAndUserId(districtTermId, userId, vSchoolId)
                .Select(o => new ListItem { Id = o.Id, Name = o.Name }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [AdminOnly(Order = 3)]
        public ActionResult LoadStudentInClass(int classId, int teacherId, int schoolId, int districtId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            SelectedClassID = classId;
            SelectedDistrictID = districtId;
            SelectedSchoolID = schoolId;
            SelectedTeacherID = teacherId;
            SchoolClassCurrentButtonSelection = SchoolClassCurrentSelection.Class;
            System.Web.HttpContext.Current.Session["EditStudentSource"] = EditStudentSource.FromManageSchools;

            var model = parameters.ClassService.GetClassById(classId);
            return PartialView("_ListStudentInClass", model);
        }

        public ActionResult ManageStudents(int id)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, id))
            {
                return RedirectToAction("ManageClass");
            }

            SelectedClassID = id;
            SchoolClassCurrentButtonSelection = SchoolClassCurrentSelection.Class;
            var model = parameters.ClassService.GetClassById(id);
            SelectedSchoolID = model.SchoolId.Value;
            return View("ManageStudents", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewStudent(AddEditStudentViewModel model)
        {
            model.SetValidator(parameters.AddStudentViewModelValidator);
            if (!IsValid(model))
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }
            // check code start with zero
            var student = parameters.StudentService.CheckExistCodeStartWithZero(model.DistrictId, model.StudentLocalId, model.StudentId);
            if (student != null)
            {
                string strLink = string.Format("<a href='/ManageClasses/EditStudent/{0}'>{1} {2}</a>", student.Id, student.FirstName, student.LastName);
                var validationFailures = model.ValidationErrors.ToList();
                string errorMessage = string.Format("That Local ID is already in use in this {0} by student {1}.", LabelHelper.DistrictLabel, strLink);
                validationFailures.Add(new ValidationFailure("error", errorMessage));
                return Json(new { Success = false, ErrorList = validationFailures }, JsonRequestBehavior.AllowGet);
            }

            bool canAccessSchool = parameters.SchoolService.CheckUserCanAccessSchool(CurrentUser.Id, CurrentUser.RoleId, model.AdminSchoolId.Value);
            if (!canAccessSchool)
            {
                var validationFailures = model.ValidationErrors.ToList();
                validationFailures.Add(new ValidationFailure("error", "Can not access school."));
                return Json(new { Success = false, ErrorList = validationFailures }, JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrEmpty(model.Email) && !IsValidEmail(model.Email))
            {
                var validationFailures = model.ValidationErrors.ToList();
                validationFailures.Add(new ValidationFailure("error", "The email is incorrect format."));
                return Json(new { Success = false, ErrorList = validationFailures }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                model.Password = model.ConfirmPassword = model.StudentLocalId;
            }

            string messageInvalid = ValidateStudentData(model);
            if (!string.IsNullOrEmpty(messageInvalid))
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>() { new ValidationFailure("error", messageInvalid) } });

            HandleUnKnownRace(model);

            if (!IsValid(model))
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (model.ParentUserId > 0)
                {
                    if (!parameters.ManageParentService.IsParentUser(model.ParentUserId))
                    {
                        return Json(new { Success = false, ErrorList = new ValidationFailure[] { new ValidationFailure("error", "Invalid parent user") } }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (model.FromManageStudent || model.ParentUserId > 0)
                {
                    //check if user has selected any class to assign this student or not
                    var classIdList = model.ClassIdString.ParseIdsFromString();
                    if (classIdList.Count == 0)
                    {
                        var validationFailures = model.ValidationErrors.ToList();
                        validationFailures.Add(new ValidationFailure("error", "Please assign one or more classes for student before saving."));
                        return Json(new { Success = false, ErrorList = validationFailures }, JsonRequestBehavior.AllowGet);
                    }

                    if (classIdList.Any(id => !parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, id)))
                    {
                        var validationFailures = model.ValidationErrors.ToList();
                        validationFailures.Add(new ValidationFailure("error", "Can not access class."));
                        return Json(new { Success = false, ErrorList = validationFailures }, JsonRequestBehavior.AllowGet);
                    }

                    var newStudent = GetStudentFromViewModel(model);
                    if (model.FilterDistrictId > 0)
                    {
                        newStudent.DistrictId = model.FilterDistrictId;
                    }
                    else
                    {
                        newStudent.DistrictId = model.DistrictId;
                    }
                    //check if user has right to access the district
                    if (!Util.HasRightOnDistrict(CurrentUser, newStudent.DistrictId))
                    {
                        return Json(new { error = "Has no right to access district" }, JsonRequestBehavior.AllowGet);
                    }
                    parameters.StudentService.Save(newStudent);

                    //save StudentGradeHistory
                    if (newStudent.Id > 0 && newStudent.CurrentGradeId > 0)
                    {
                        parameters.StudentGradeHistoryService.Save(newStudent);
                    }

                    //Store StudentMetaData
                    AddOrUpdateStudentMetaData(model.StudentMetaDatas, newStudent.Id);
                    foreach (var classId in classIdList)
                    {
                        try
                        {
                            var cls = parameters.ClassService.GetClassById(classId);
                            if (cls != null)
                            {
                                var classStudent = new ClassStudentData
                                {
                                    ClassID = classId,
                                    StudentID = newStudent.Id,
                                    Active = true
                                };
                                parameters.ClassStudentDataService.SaveClassStudent(classStudent);
                                SaveSchoolStudent(cls.SchoolId.Value, newStudent.Id);
                            }

                        }
                        catch (Exception ex)
                        {
                            PortalAuditManager.LogException(ex);
                            // catch excetion
                        }
                    }
                    if (model.ParentUserId > 0)
                    {
                        parameters.ManageParentService.AddStudentsToParent(newStudent.Id.ToString(), model.ParentUserId);
                    }
                    return Json(new { Success = true, studentId = newStudent.Id }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var currentClass = parameters.ClassService.GetClassById(AddNewStudentToClassId);
                    var currentSchool = parameters.SchoolService.GetSchoolById(currentClass.SchoolId.Value);
                    SelectedSchoolID = SelectedSchoolID == 0 ? currentSchool.Id : SelectedSchoolID;
                    var newStudent = GetStudentFromViewModel(model);
                    newStudent.DistrictId = currentSchool.DistrictId;

                    parameters.StudentService.Save(newStudent);
                    AddOrUpdateStudentMetaData(model.StudentMetaDatas, newStudent.Id);

                    if (AddNewStudentToClassId > 0)
                    {
                        var classStudent = new ClassStudentData
                        {
                            ClassID = AddNewStudentToClassId,
                            StudentID = newStudent.Id,
                            Active = true
                        };
                        parameters.ClassStudentDataService.SaveClassStudent(classStudent);
                        SaveSchoolStudent(currentSchool.Id, newStudent.Id);
                        AddNewStudentToClassId = 0;
                        return Json(new { Success = true, studentId = newStudent.Id }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                var validationFailures = model.ValidationErrors.ToList();

                if (ex != null)
                    validationFailures.Add(new ValidationFailure("error", ex.Message));
                else
                    validationFailures.Add(new ValidationFailure("error", "An error has occurred, please try again."));
                return Json(new { Success = false, ErrorList = validationFailures }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.StudentLookup)]
        public ActionResult EditStudent(int id, int? editStudentSource, bool? fromManageStudent, string returnUrl = "")
        {
            var model = new AddEditStudentViewModel();
            if (!CanAccessStudentByAdminSchool(id))
            {
                model.CanAccess = false;
                return View(model);
            }

            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, id.ToString()))
            {
                return RedirectToAction("Index", "StudentLookup");
            }

            var student = parameters.StudentService.GetStudentById(id);
            if (student.IsNull())
            {
                return HttpNotFound();
            }

            model = GetAddOrEditStudentViewModel(student, editStudentSource);
            model.HasPortalAccount = parameters.StudentMetaService.GetUserIdByStudentId(student.Id) > 0;
            if (fromManageStudent.HasValue)
            {
                model.FromManageStudent = fromManageStudent.Value;
            }
            model.ReturnUrl = returnUrl;
            model.HasGenerateLogin = parameters.DistrictDecodeService.GetDistrictDecodeOrConfigurationByLabel(student.DistrictId, Constanst.ALLOW_STUDENT_USER_GENERATION);

            return View(model);
        }

        [HttpGet]
        public ActionResult GetAvailableProgramsView(int studentId)
        {
            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentId.ToString()))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var student = parameters.StudentService.GetStudentById(studentId);
            if (student.IsNull())
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AvailablePrograms", student);
        }

        [HttpGet]
        public ActionResult GetStudentPrograms(int studentId)
        {
            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentId.ToString()))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var student = parameters.StudentService.GetStudentById(studentId);
            if (student.IsNull())
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AssignedPrograms", student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudent(AddEditStudentViewModel model)
        {
            model.SetValidator(parameters.AddEditStudentViewModelValidator);
            if (!IsValid(model))
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors }, JsonRequestBehavior.AllowGet);
            }

            string messageInvalid = ValidateStudentData(model);
            if (!string.IsNullOrEmpty(messageInvalid))
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>() { new ValidationFailure("error", messageInvalid) } });

            // check code start with rezo
            var s = parameters.StudentService.CheckExistCodeStartWithZero(model.DistrictId, model.StudentLocalId, model.StudentId);
            if (s != null)
            {
                string strLink = string.Format("<a href='/ManageClasses/EditStudent/{0}'>{1} {2}</a>", s.Id, s.FirstName, s.LastName);
                var validationFailures = model.ValidationErrors.ToList();
                validationFailures.Add(new ValidationFailure("error", "That Local ID is already in use in this " + LabelHelper.DistrictLabel + " by student " + strLink + "."));
                return Json(new { Success = false, ErrorList = validationFailures }, JsonRequestBehavior.AllowGet);
            }

            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, model.StudentId.ToString()))
            {
                return ShowJsonResultException(model, "You do not have permission to access this student.");
            }

            if (!parameters.SchoolService.CheckUserCanAccessSchool(CurrentUser.Id, CurrentUser.RoleId,
                model.AdminSchoolId.Value))
            {
                var validationFailures = model.ValidationErrors.ToList();
                validationFailures.Add(new ValidationFailure("error", "Can not access school."));
                return Json(new { Success = false, ErrorList = validationFailures }, JsonRequestBehavior.AllowGet);
            }

            if (!string.IsNullOrEmpty(model.Email) && !IsValidEmail(model.Email))
            {
                var validationFailures = model.ValidationErrors.ToList();
                validationFailures.Add(new ValidationFailure("error", "The email is incorrect format."));
                return Json(new { Success = false, ErrorList = validationFailures }, JsonRequestBehavior.AllowGet);
            }

            HandleUnKnownRace(model);

            if (!CanAccessStudentByAdminSchool(model.StudentId))
            {
                return ShowJsonResultException(model, "You do not have permission to access this student.");
            }

            try
            {
                var student = parameters.StudentService.GetStudentById(model.StudentId);
                if (!student.SISID.HasValue)
                {
                    student.FirstName = model.FirstName;
                    student.LastName = model.LastName;
                    student.MiddleName = model.MiddleName;
                    student.Email = model.Email;
                    student.RaceId = model.RaceId;
                    student.Code = model.StudentLocalId;
                    student.AltCode = model.StudentStateId;
                    student.GenderId = model.GenderId;
                    student.CurrentGradeId = model.GradeId;
                    student.AdminSchoolId = model.AdminSchoolId;
                    student.Status = 1;
                    student.DistrictId = model.DistrictId;
                    student.CreatedDate = DateTime.UtcNow;
                    student.ModifiedDate = DateTime.UtcNow;
                    student.ModifiedUser = CurrentUser.Id;
                    student.ModifiedBy = "Portal";
                }

                // Allow to change username && password even when this student is SISID student
                if (!string.IsNullOrEmpty(model.Password))
                {
                    model.Password = Md5Hash.GetMd5Hash(model.Password);
                    student.Password = model.Password;
                }

                //check if user has right to access the district
                if (!Util.HasRightOnDistrict(CurrentUser, student.DistrictId))
                {
                    return Json(new { error = "Has no right to access district" }, JsonRequestBehavior.AllowGet);
                }

                string errorMessage = string.Empty;
                parameters.StudentService.UpdateAndSaveLog(student, CurrentUser.Id, model.UserName ?? string.Empty, model.Password ?? string.Empty, out errorMessage);

                //save StudentGradeHistory
                if (student.Id > 0 && student.CurrentGradeId > 0)
                {
                    parameters.StudentGradeHistoryService.Save(student);
                }

                AddOrUpdateStudentMetaData(model.StudentMetaDatas, model.StudentId);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    var validationFailures = model.ValidationErrors.ToList();
                    validationFailures.Add(new ValidationFailure("error", errorMessage));

                    return Json(new { Success = false, ErrorList = validationFailures }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                if (ex != null)
                    return ShowJsonResultException(model, ex.Message);
                else
                    return ShowJsonResultException(model, "An error has occurred, please try again.");
            }
        }

        private string ValidateStudentData(AddEditStudentViewModel model)
        {
            if (CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin)
            {
                model.DistrictId = CurrentUser.DistrictId ?? 0;
            }
            return parameters.DistrictDecodeService.GetDistrictDecodeValidations(model.DistrictId, GetValidateFields(model));
        }

        public Dictionary<string, string> GetValidateFields(AddEditStudentViewModel student)
        {
            var dataFields = new Dictionary<string, string>();
            dataFields.Add(Constanst.STUDENT_CODE_VALIDATION, student.StudentLocalId);
            if (!string.IsNullOrEmpty(student.StudentStateId))
                dataFields.Add(Constanst.STUDENT_ALT_CODE_VALIDATION, student.StudentStateId);

            return dataFields;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool CanAccessStudentByAdminSchool(int studentId)
        {
            var oldStudent = parameters.StudentService.GetStudentById(studentId);
            if (oldStudent != null
                && oldStudent.AdminSchoolId.HasValue
                && !CurrentUser.IsLinkItAdminOrPublisher()
                && !CurrentUser.IsNetworkAdmin
                && (!CurrentUser.IsDistrictAdminOrPublisher || (CurrentUser.IsDistrictAdminOrPublisher && CurrentUser.DistrictId != oldStudent.DistrictId))
                )
            {
                var userSchools = parameters.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id);
                if (!userSchools.Any(en => en.SchoolId == oldStudent.AdminSchoolId))
                {
                    return false;
                }
            }

            return true;
        }

        [AdminOnly(Order = 3)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageSchools)]
        public ActionResult AddTerm(int? id)
        {
            return View(id);
        }

        [AdminOnly(Order = 3)]
        public ActionResult ShowAddOrEditTermForm(int districtId)
        {
            var model = new AddEditTermViewModel { DistrictID = districtId };
            return PartialView("_AddOrEditTermForm", model);
        }

        [AdminOnly(Order = 3)]
        public ActionResult AddNewTerm(AddEditTermViewModel model)
        {
            if (model.DistrictID == 0)
            {
                model.DistrictID = SelectedDistrictID;
            }

            model.SetValidator(parameters.AddOrEditTermViewModelValidator);
            if (!IsValid(model) || model.DistrictID == 0)
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var term = GetTermByModel(model);
                if (term.IsNotNull())
                {
                    term.UpdatedByUserID = term.ModifiedUser = term.CreatedByUserID = CurrentUser.Id; //ModifiedByUserID
                    term.DateCreated = term.DateUpdated = DateTime.Now;
                    term.ModifiedBy = "Portal";

                    parameters.DistrictTermService.Save(term);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                return ShowJsonResultException(model, "An error has occurred, please try again.");
            }
            catch (Exception)
            {
                return ShowJsonResultException(model, "An error has occurred, please try again.");
            }
        }

        [AdminOnly(Order = 3)]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageSchools)]
        public ActionResult EditTerm(int id)
        {
            var districtTerm = parameters.DistrictTermService.GetDistrictTermById(id);
            var model = new AddEditTermViewModel
            {
                TermID = districtTerm.DistrictTermID,
                DistrictID = districtTerm.DistrictID,
                Name = districtTerm.Name,
                DateStart = districtTerm.DateStart,
                DateEnd = districtTerm.DateEnd
            };
            return View(model);
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageSchools)]
        public ActionResult EditSchool(int id)
        {
            if (!parameters.SchoolService.CheckUserCanAccessSchool(CurrentUser.Id, CurrentUser.RoleId, id))
            {
                return RedirectToAction("ManageSchoolAndClass");
            }

            School school = parameters.SchoolService.GetSchoolById(id);
            if (school.IsNotNull())
            {
                if (CurrentUser.RoleId == (int)Permissions.Publisher ||
                    CurrentUser.RoleId == (int)Permissions.NetworkAdmin ||
                    (CurrentUser.RoleId == (int)Permissions.DistrictAdmin &&
                     school.DistrictId.Equals(CurrentUser.DistrictId.GetValueOrDefault())))
                {
                    return View(school);
                }
            }

            return RedirectToAction("ManageSchoolAndClass");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSchool(School editedSchool)
        {
            editedSchool.SetValidator(parameters.SchoolValidator);
            if (editedSchool.IsValid)
            {
                if (!parameters.SchoolService.CheckUserCanAccessSchool(CurrentUser.Id, CurrentUser.RoleId, editedSchool.Id))
                {
                    return Json(new { Success = false, ErrorList = new List<ValidationFailure> { new ValidationFailure("", "Access denied") } }, JsonRequestBehavior.AllowGet);
                }

                string messageInvalid = ValidateSchoolData(editedSchool);
                if (!string.IsNullOrEmpty(messageInvalid))
                    return Json(new { Success = false, ErrorList = new List<ValidationFailure>() { new ValidationFailure("error", messageInvalid) } });

                School school = parameters.SchoolService.GetSchoolById(editedSchool.Id);
                if (school.IsNotNull() &&
                    (CurrentUser.RoleId == (int)Permissions.Publisher ||
                     CurrentUser.RoleId == (int)Permissions.NetworkAdmin ||
                     (CurrentUser.RoleId == (int)Permissions.DistrictAdmin &&
                      school.DistrictId.Equals(CurrentUser.DistrictId.GetValueOrDefault()))))
                {
                    school.Name = editedSchool.Name;
                    school.Code = editedSchool.Code;
                    school.StateCode = editedSchool.StateCode;

                    school.ModifiedDate = DateTime.Now;
                    school.ModifiedUser = CurrentUser.Id;
                    school.ModifiedBy = "Portal";

                    //Store SchoolMetaData
                    AddOrUpdateSchoolMetaData(editedSchool.SchoolMetaDatas, editedSchool.Id);

                    parameters.SchoolService.Save(school);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = false, ErrorList = new List<ValidationFailure> { new ValidationFailure("", "Access denied") } }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = false, ErrorList = editedSchool.ValidationErrors }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageSchools)]
        public ActionResult AddNewSchool()
        {
            var model = new School
            {
                DistrictId = SelectedDistrictID > 0 ? SelectedDistrictID : CurrentUser.DistrictId.GetValueOrDefault()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNewSchool(School addedSchool)
        {
            var district = SelectedDistrictID > 0 ? SelectedDistrictID : CurrentUser.DistrictId.GetValueOrDefault();
            addedSchool.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            int districtid;
            if (int.TryParse(district.ToString(CultureInfo.InvariantCulture), out districtid))
            {
                addedSchool.DistrictId = districtid;
            }
            addedSchool.SetValidator(parameters.SchoolValidator);
            if (addedSchool.IsValid)
            {
                string messageInvalid = ValidateSchoolData(addedSchool);
                if (!string.IsNullOrEmpty(messageInvalid))
                    return Json(new { Success = false, ErrorList = new List<ValidationFailure>() { new ValidationFailure("error", messageInvalid) } });

                addedSchool.ModifiedDate = addedSchool.CreatedDate = DateTime.Now;
                addedSchool.ModifiedUser = CurrentUser.Id;
                addedSchool.Status = 1;
                addedSchool.ModifiedBy = "Portal";

                parameters.SchoolService.Save(addedSchool);

                //Store StudentMetaData
                AddOrUpdateSchoolMetaData(addedSchool.SchoolMetaDatas, addedSchool.Id);

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, ErrorList = addedSchool.ValidationErrors }, JsonRequestBehavior.AllowGet);
        }

        private string ValidateSchoolData(School model)
        {
            return parameters.DistrictDecodeService.GetDistrictDecodeValidations(model.DistrictId, GetValidateFields(model));
        }

        public Dictionary<string, string> GetValidateFields(School school)
        {
            var dataFields = new Dictionary<string, string>();
            dataFields.Add(Constanst.SCHOOL_CODE_VALIDATION, school.Code);
            if (!string.IsNullOrEmpty(school.StateCode))
                dataFields.Add(Constanst.SCHOOL_STATE_CODE_VALIDATION, school.StateCode);

            return dataFields;
        }

        [HttpGet]
        public ActionResult GetStudentClasses(int studentId)
        {
            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentId.ToString()))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var student = parameters.StudentService.GetStudentById(studentId);
            if (student.IsNull())
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AssignedClasses", student);
        }

        [HttpGet]
        public ActionResult GetStudentsInClass(int classId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(new { success = false, message = "Has no right to acess class.", },
                    JsonRequestBehavior.AllowGet);
            }

            var students = parameters.StudentsInClassService.GetAllStudentInClass(classId).Select(x => new StudentInClassViewModel
            {
                ID = x.ID,
                StudentID = x.StudentID,
                LastName = x.LastName,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                Code = x.Code,
                Grade = x.GradeName,
                Gender = x.Gender,
                CanAccess = CanAccessStudentByAdminSchool(x.StudentID)
            });
            var parser = new DataTableParser<StudentInClassViewModel>();
            return Json(parser.Parse(students), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveStudentFromClass()
        {
            int classId;
            if (!int.TryParse(Request["classId"], out classId))
            {
                return Json(new { message = "Class is invalid.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
            }
            var movedStudentIds = Request["removedStudentIds"];
            var studentIdList = GetStudentIdsByString(movedStudentIds);
            if (studentIdList.IsNull())
            {
                return Json(new { message = "Removed student(s) is invalid.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
            }

            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(new { message = "Class is invalid", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
            }

            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, movedStudentIds))
            {
                return Json(new { message = "Removed student(s) is invalid.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
            }
            parameters.ClassStudentDataService.RemoveStudentsFromClass(studentIdList, classId);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult MoveStudents()
        {
            var listError = new List<ValidationFailure>();

            var oldClassId = ValidateClassId(Request["oldClassId"], listError, "Old class is invalid.");
            var newClassId = ValidateClassId(Request["newClassId"], listError, "New class is invalid.");
            var studentIdList = ValidateMoveStudents(newClassId, Request["movedStudentIds"], listError);

            if (listError != null && listError.Any())
            {
                return Json(new { Success = false, ErrorList = listError, Type = "error" }, JsonRequestBehavior.AllowGet);
            }

            parameters.ClassStudentDataService.MoveStudents(studentIdList, oldClassId, newClassId);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        private void HandleUnKnownRace(AddEditStudentViewModel model)
        {
            var districtId = GetDistrictOfLoginUser();
            var races = parameters.RaceService.GetRacesByDistrictID(districtId).ToList();
            if (races.Count == 0)
            {
                var unknowRace = CreateUnknownRace(districtId);
                model.RaceId = unknowRace.Id;
            }
            else if (model.RaceId == 0)
            {
                var unknownRace = races.FirstOrDefault(o => o.Name == "Unknown" && o.Code == "U");
                if (unknownRace == null)
                    unknownRace = races.FirstOrDefault(o => o.Name == "Unknown");
                if (unknownRace != null)
                {
                    model.RaceId = unknownRace.Id;
                }
                else
                {
                    var unknowRace = CreateUnknownRace(districtId);
                    model.RaceId = unknowRace.Id;
                }
            }
        }

        private Race CreateUnknownRace(int districtID)
        {
            var unknowRace = new Race
            {
                Code = "U",
                DistrictID = districtID,
                Name = "Unknown"
            };
            parameters.RaceService.AddRace(unknowRace);

            return unknowRace;
        }

        private int ValidateClassId(string oldClassId, List<ValidationFailure> listError, string errorMessage)
        {
            listError = listError ?? new List<ValidationFailure>();
            int classID;
            if (!int.TryParse(oldClassId, out classID))
            {
                listError.Add(new ValidationFailure(string.Empty, errorMessage));
            }

            return classID;
        }

        private List<int> ValidateMoveStudents(int newClassID, string movedStudentIds, List<ValidationFailure> listError)
        {
            var studentIdList = GetStudentIdsByString(movedStudentIds);
            if (studentIdList.IsNull())
            {
                listError.Add(new ValidationFailure(string.Empty, "Moved student(s) is invalid."));
                return new List<int>();
            }

            var students = parameters.StudentsInClassService.GetAllStudentInClass(newClassID).Where(o => studentIdList.Contains(o.StudentID));
            if (students.Any())
            {
                var existedStudent = string.Join(", ", students.Select(o => o.FirstName + " " + o.LastName).Distinct().ToArray());
                listError.Add(new ValidationFailure(string.Empty, string.Format("Student(s) aldready exist in destination class: {0}.", existedStudent)));
                return new List<int>();
            }
            return studentIdList;
        }

        private List<int> GetStudentIdsByString(string studentIdString)
        {
            var studentIds = studentIdString.Split(',');
            var isStudentIdValid = true;
            var studentIdList = new List<int>();
            foreach (string id in studentIds)
            {
                int studentId;
                if (int.TryParse(id, out studentId))
                {
                    studentIdList.Add(studentId);
                }
                else
                {
                    isStudentIdValid = false;
                    break;
                }
            }
            return !isStudentIdValid ? null : studentIdList;
        }

        public ActionResult GetClassesByStudentId(int studentId)
        {
            var students = parameters.UserStudentService.GetStudentsByStudent(studentId).ToList();

            if (!CurrentUser.IsPublisher
                && !CurrentUser.IsDistrictAdmin
                && !CurrentUser.IsNetworkAdmin)
            {
                var adminSchoolIds = parameters.UserSchoolService.GetListSchoolIdByUserId(CurrentUser.Id);
                students = students.Where(x => adminSchoolIds.Contains(x.SchoolID)).ToList();
            }
            var teacherIds = parameters.ClassUserService.GetPrimaryTeacherByClassIds(students.GroupBy(x => x.ClassID).Select(x => x.Key).ToList());
            var classesStudent = students.Where(x => x.TeacherID.HasValue && teacherIds.Contains(x.TeacherID.Value))
                                         .GroupBy(x => new { x.ClassID, x.ClassName, x.SchoolName })
                                         .Select(g => new StudentClassViewModel
                                         {
                                             ClassId = g.Key.ClassID,
                                             ClassName = g.Key.ClassName,
                                             SchoolName = g.Key.SchoolName,
                                             TeacherName = g.Max(x => x.TeacherName),
                                             TermName = g.Max(x => x.TermName),
                                             ModifiedBy = g.Max(x => x.ModifiedBy)
                                         }).OrderBy(x => x.SchoolName).ThenBy(x => x.ClassName).AsQueryable();
            var parser = new DataTableParser<StudentClassViewModel>();
            return Json(parser.Parse(classesStudent), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAvailableClassesView(int studentId)
        {
            var student = parameters.StudentService.GetStudentById(studentId);
            ViewBag.CurrentUserRoleId = CurrentUser.RoleId;
            ViewBag.CurrentUserId = CurrentUser.Id;
            ViewBag.SelectedSchoolId = SelectedSchoolID;
            ViewBag.IsTeacher = CurrentUser.IsTeacher;

            if (student.IsNull())
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_AvailableClasses", student);
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.StudentLookup)]
        public ActionResult AddNewStudent(bool? fromManageStudent, int? filterDistrictId, int? parentUserId)
        {
            //check access to button "Add new student"
            var isAccess = CheckAccessToAddNewStudent(fromManageStudent);
            if (!isAccess)
                return RedirectToAction("ManageClass");

            var districtId = GetDistrictOfLoginUser();
            if (fromManageStudent.HasValue && fromManageStudent.Value)
            {
                if (filterDistrictId.HasValue && filterDistrictId.Value > 0)
                {
                    districtId = filterDistrictId.Value;
                }
                else
                {
                    districtId = CurrentUser.DistrictId.Value;
                }
            }


            var model = new AddEditStudentViewModel
            {
                Genders = parameters.GenderService.GetAllGenders().Select(x => new SelectListItem { Text = x.Name, Value = x.GenderID.ToString(CultureInfo.InvariantCulture) }).ToList(),
                Races = parameters.RaceService.GetRacesByDistrictID(districtId).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(CultureInfo.InvariantCulture) }).ToList(),
                Grades = parameters.GradeService.GetGradesByDistrictId(districtId).Select(x => new SelectListItem { Text = x.Name, Value = x.GradeID.ToString() }).ToList(),
                DistrictId = districtId,
                CurrentUserId = CurrentUser.Id,
                CurrentUserRoleId = CurrentUser.RoleId,
                FromManageStudent = fromManageStudent.HasValue && fromManageStudent.Value,
                ParentUserId = parentUserId ?? 0,
                FilterDistrictId = filterDistrictId.HasValue ? filterDistrictId.Value : 0
            };

            if (model.Races.Count == 0) model.Races.Add(new SelectListItem { Text = "Unknown", Value = "1" });

            return View(model);
        }

        private bool CheckAccessToAddNewStudent(bool? fromManageStudent)
        {
            if (!CurrentUser.IsAdmin()) return false;

            //get setup hide or show "Add New Student" in configuration table
            var isShowAddNewStudent =
                parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue(Util.IsShow_AddNewStudentButton, "true");
            if (isShowAddNewStudent.ToLower() == "false")
            {
                if (fromManageStudent.HasValue && fromManageStudent.Value)
                {
                    var menuAccess = HelperExtensions.GetMenuForDistrict(CurrentUser);
                    if (!menuAccess.IsDisplayStudentLookup)
                        return false;

                    return true;
                }
                return false;
            }
            return true;
        }

        private int GetDistrictOfLoginUser()
        {
            var district = System.Web.HttpContext.Current.Session["SelectedDistrictID"];
            var districtId = CurrentUser.DistrictId.GetValueOrDefault();
            if (district.IsNotNull() && !int.TryParse(district.ToString(), out districtId))
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            return districtId;
        }

        [HttpGet]
        [AjaxOnly]
        public ActionResult GetAvailableClassesByStudentId(GetAvailableClassesRequest request)
        {
            if (request.SchoolId.HasValue && !parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, request.SchoolId.Value))
            {
                return Json(new { success = false, message = "Has no right", }, JsonRequestBehavior.AllowGet);
            }

            var selectedSchoolId = 0;
            if (SelectedSchoolID > 0 && !request.SchoolId.HasValue)
                selectedSchoolId = SelectedSchoolID;
            else if (request.SchoolId.HasValue)
                selectedSchoolId = request.SchoolId.Value;

            string sortBy = "ClassId";
            string sortDirection = "ASC";
            string searchString = null;
            if (!string.IsNullOrWhiteSpace(request.sColumns) && request.iSortCol_0.HasValue)
            {
                var columns = request.sColumns.Split(',');
                sortBy = columns[request.iSortCol_0.Value];
                sortDirection = request.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
            }

            if (!string.IsNullOrEmpty(request.sSearch))
            {
                searchString = request.sSearch.Trim();
            }
            int? userId = null;
            if ((int)Permissions.Teacher == CurrentUser.RoleId)
                userId = CurrentUser.Id;
            var data = parameters.UserStudentService.GetAvailableClassesBySchoolAndStudentId(selectedSchoolId, request.StudentId, userId, request.iDisplayStart, request.iDisplayLength, searchString, sortBy, sortDirection);

            var result = new GenericDataTableResponse<StudentClassViewModel>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = data.Select(x => new StudentClassViewModel
                {
                    ClassId = x.ClassID,
                    ClassName = x.ClassName,
                    SchoolName = x.SchoolName,
                    TeacherName = x.TeacherName,
                    TermName = x.TermName
                }).ToList(),
                iTotalDisplayRecords = data.Count > 0 ? data[0].TotalCount : 0,
                iTotalRecords = data.Count > 0 ? data[0].TotalCount : 0
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveClassStudent(int classId, int studentId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(new { success = false, message = $"You do not have permission", JsonRequestBehavior.AllowGet });
            }

            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentId.ToString()))
            {
                return Json(new { success = false, message = $"You do not have permission", JsonRequestBehavior.AllowGet });
            }

            var classStudent = parameters.ClassStudentDataService.GetClassStudent(studentId, classId);
            if (classStudent.IsNotNull())
            {
                classStudent.Active = false;
                parameters.ClassStudentDataService.Delete(classStudent);
                return Json(new { success = true, message = $"Remove class successfully", JsonRequestBehavior.AllowGet });
            }

            return Json(new { success = true, message = $"Remove class successfully", JsonRequestBehavior.AllowGet });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignClassForStudent(int classId, int studentId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var classStudent = parameters.ClassStudentDataService.GetClassStudent(studentId, classId);
            if (classStudent.IsNotNull())
            {
                classStudent.Active = true;
                parameters.ClassStudentDataService.Save(classStudent);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            classStudent = new ClassStudentData { ClassID = classId, StudentID = studentId, Active = true };
            parameters.ClassStudentDataService.Save(classStudent);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, AdminOnly(Order = 3)]
        [AjaxOnly]
        public ActionResult EditTerm(AddEditTermViewModel model)
        {
            model.SetValidator(parameters.AddOrEditTermViewModelValidator);
            if (!IsValid(model))
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var term = GetTermByModel(model);
                if (term.IsNotNull())
                {
                    term.UpdatedByUserID = term.ModifiedUser = CurrentUser.Id; //ModifiedByUserID
                    term.DateUpdated = DateTime.Now;
                    term.ModifiedBy = "Portal";

                    parameters.DistrictTermService.Save(term);
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                return ShowJsonResultException(model, "An error has occurred, please try again.");
            }
            catch (Exception)
            {
                return ShowJsonResultException(model, "An error has occurred, please try again.");
            }
        }

        [AdminOnly(Order = 3)]
        public ActionResult LoadDistrictDetail(int districtId, string schoolName)
        {
            if (districtId > 0 && (CurrentUser.RoleId == (int)Permissions.Publisher
                                    || CurrentUser.RoleId == (int)Permissions.NetworkAdmin
                                    || (CurrentUser.RoleId == (int)Permissions.DistrictAdmin && CurrentUser.DistrictId.GetValueOrDefault() == districtId)))
            {
                var district = parameters.DistrictService.GetDistrictById(districtId);
                if (district.IsNotNull())
                {
                    SelectedDistrictID = district.Id;
                    SchoolClassCurrentButtonSelection = SchoolClassCurrentSelection.District;
                    ViewBag.SchoolName = schoolName;
                    return PartialView("_DistrictDetail", district);
                }
            }
            return PartialView("_DistrictDetail", new District());
        }

        public ActionResult LoadDistrictDetailV2(int districtId, int schoolID)
        {
            ViewBag.SchoolID = schoolID;
            if (districtId > 0 && (CurrentUser.RoleId == (int)Permissions.Publisher
                                    || CurrentUser.RoleId == (int)Permissions.NetworkAdmin
                                    || (CurrentUser.RoleId == (int)Permissions.DistrictAdmin && CurrentUser.DistrictId.GetValueOrDefault() == districtId)))
            {
                var district = parameters.DistrictService.GetDistrictById(districtId);
                if (district.IsNotNull())
                {
                    SelectedDistrictID = district.Id;
                    SchoolClassCurrentButtonSelection = SchoolClassCurrentSelection.District;
                    return PartialView("_DistrictDetail", district);
                }
            }
            return PartialView("_DistrictDetail", new District());
        }

        private DistrictTerm GetTermByModel(AddEditTermViewModel model)
        {
            DistrictTerm term = null;
            if (model.IsNotNull())
            {
                term = new DistrictTerm
                {
                    DistrictTermID = model.TermID,
                    Name = model.Name,
                    DistrictID = model.DistrictID,
                    DateStart = model.DateStart,
                    DateEnd = model.DateEnd
                };
            }
            return term;
        }

        private void SaveSchoolStudent(int schoolId, int studentId)
        {
            if (schoolId <= 0 || studentId <= 0) return;
            var objSchoolStudent = new SchoolStudentData { SchoolID = schoolId, StudentID = studentId, Active = true };
            parameters.SchoolStudentDataService.Save(objSchoolStudent);
        }

        private Student GetStudentFromViewModel(AddEditStudentViewModel model)
        {
            return new Student
            {
                Id = model.StudentId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                Email = model.Email,
                RaceId = model.RaceId,
                Code = model.StudentLocalId,
                AltCode = model.StudentStateId,
                GenderId = model.GenderId,
                Password = Md5Hash.GetMd5Hash(model.Password),
                CurrentGradeId = model.GradeId,
                AdminSchoolId = model.AdminSchoolId,
                Status = 1,
                DistrictId = model.DistrictId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                ModifiedUser = CurrentUser.Id,
                ModifiedBy = "Portal"
            };
        }

        private AddEditStudentViewModel GetAddOrEditStudentViewModel(Student student, int? editStudentSource)
        {
            var model = new AddEditStudentViewModel { CanAccess = CanAccessStudent(student) };
            if (model.CanAccess)
            {
                model.IsSISsystem = student.SISID.HasValue;
                model.StudentId = student.Id;
                model.FirstName = student.FirstName;
                model.MiddleName = student.MiddleName;
                model.LastName = student.LastName;
                model.GenderId = student.GenderId;
                model.Genders = parameters.GenderService.GetAllGenders().Select(x => new SelectListItem { Text = x.Name, Value = x.GenderID.ToString() }).ToList();
                model.RaceId = student.RaceId;
                model.Races = parameters.RaceService.GetRacesByDistrictID(student.DistrictId).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                model.GradeId = student.CurrentGradeId.GetValueOrDefault();
                model.Grades = parameters.GradeService.GetGradesByDistrictId(student.DistrictId).Select(x => new SelectListItem { Text = x.Name, Value = x.GradeID.ToString() }).ToList();
                model.Password = student.Password;
                model.ConfirmPassword = student.Password;
                model.StudentLocalId = student.Code;
                model.StudentStateId = student.AltCode;
                model.DistrictId = GetDistrictOfLoginUser();
                //model.DistrictId = SelectedDistrictID;
                model.AdminSchoolId = student.AdminSchoolId;
                model.CurrentUserId = CurrentUser.Id;
                model.CurrentUserRoleId = CurrentUser.RoleId;
                model.Email = student.Email;
                model.SharedSecret = student.SharedSecret;

                int userId = parameters.StudentMetaService.GetUserIdViaStudentUser(student.Id);
                if (userId > 0)
                {
                    model.UserId = userId;
                    var userStudent = parameters.UserService.GetUserById(model.UserId.Value);
                    if (userStudent != null)
                    {
                        model.UserName = userStudent.UserName;
                    }
                }

                if (model.Races.Count == 0) model.Races.Add(new SelectListItem { Text = "Unknown", Value = "1" });
            }
            return model;
        }

        private bool CanAccessStudent(Student student)
        {
            var canAccess = false;
            switch ((Permissions)CurrentUser.RoleId)
            {
                case Permissions.Publisher:
                    canAccess = true;
                    break;
                case Permissions.DistrictAdmin:
                    canAccess = student.DistrictId.Equals(CurrentUser.DistrictId) && CanAccessStudentByAdminSchool(student.Id);
                    break;
                case Permissions.SchoolAdmin:
                    canAccess = CanAccessStudentByAdminSchool(student.Id);
                    break;
                case Permissions.Teacher:
                    canAccess = parameters.UserStudentService.HasAccessStudentByUserAsTeacher(CurrentUser.Id, student.Id) && CanAccessStudentByAdminSchool(student.Id);
                    break;
                case Permissions.NetworkAdmin:
                    canAccess = CurrentUser.GetMemberListDistrictId().Contains(student.DistrictId) && CanAccessStudentByAdminSchool(student.Id);
                    break;
            }
            return canAccess;
        }


        [AdminOnly(Order = 3)]
        public ActionResult LoadSchoolDetail(int schoolId, int districtId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var school = parameters.SchoolService.GetSchoolById(schoolId);
            if (school.IsNotNull())
            {
                SelectedSchoolID = schoolId;
                SelectedDistrictID = districtId;
                SchoolClassCurrentButtonSelection = SchoolClassCurrentSelection.School;
                return PartialView("_SchoolDetail", school);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [AdminOnly(Order = 3)]
        public ActionResult LoadTeacherDetail(int teacherId, int schoolId, int districtId, string fromManageSchools, string className)
        {
            var teacher = parameters.UserService.GetUserById(teacherId);
            if (teacher.IsNotNull())
            {
                SelectedSchoolID = schoolId;
                SelectedTeacherID = teacherId;
                SchoolClassCurrentButtonSelection = SchoolClassCurrentSelection.Teacher;
            }

            SelectedDistrictID = districtId;
            ViewBag.SchoolID = schoolId;
            ViewBag.FromManageSchools = false;
            ViewBag.ClassName = className;
            ViewBag.TeacherId = teacherId;
            ViewBag.TeacherName = string.Empty;

            if (!string.IsNullOrEmpty(fromManageSchools))
            {
                ViewBag.FromManageSchools = true;
                return PartialView("_ListClassesByTeacherAndSchool", teacher);
            }

            var SchoolAndClassModel = new SchoolAndClassViewModel
            {
                RoleId = CurrentUser.RoleId,
                DefaultDistrictId = CurrentUser.DistrictId.GetValueOrDefault(0),
                CurrentSelectedDistrictId = 0,
                CurrentSelectedSchoolId = 0,
                CurrentSelectedTeacherId = (CurrentUser.RoleId == 2) ? CurrentUser.Id : 0,
                CurrentSelectedClassId = 0,
                UserId = CurrentUser.Id,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                IsPublisher = CurrentUser.IsPublisher
            };

            return PartialView("_ListClassesByTeacherAndSchool", SchoolAndClassModel);
        }

        public ActionResult ListClassesByTeacher(int teacherId, int districtId = 0, int schoolId = 0, string className = "")
        {
            if (schoolId > 0)
                return LoadTeacherDetail(teacherId, schoolId, districtId, "", className);

            var teacher = parameters.UserService.GetUserById(teacherId);
            if (teacher.IsNotNull())
            {
                SelectedSchoolID = 0;
                SelectedTeacherID = teacherId;
                SelectedDistrictID = districtId;
                SchoolClassCurrentButtonSelection = SchoolClassCurrentSelection.Teacher;
                ViewBag.TeacherId = teacherId;
                var SchoolAndClassModel = new SchoolAndClassViewModel
                {
                    RoleId = CurrentUser.RoleId,
                    DefaultDistrictId = CurrentUser.DistrictId.GetValueOrDefault(0),
                    CurrentSelectedDistrictId = 0,
                    CurrentSelectedSchoolId = 0,
                    CurrentSelectedTeacherId = (CurrentUser.RoleId == 2) ? CurrentUser.Id : 0,
                    CurrentSelectedClassId = 0,
                    UserId = CurrentUser.Id,
                    IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                    IsPublisher = CurrentUser.IsPublisher
                };

                var schools = parameters.UserSchoolService
                                .GetSchoolsUserHasAccessTo(CurrentUser.Id)
                                .ToList();
                if (schools.Count == 1)
                {
                    SchoolAndClassModel.CurrentSelectedSchoolId = schools[0].SchoolId.GetValueOrDefault(0);
                }

                return PartialView("_ListClassesByTeacherAndSchool", SchoolAndClassModel);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassInSchool(int schoolId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var classes = parameters.ClassDistrictTermService.GetClassDistrictTermBySchoolId(schoolId).OrderBy(x => x.ClassName).Select(x => new ListItemsViewModel
            {
                Id = x.ClassId,
                Name = x.ClassName
            });
            return Json(classes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTeacherBySchoolId(int schoolId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var schoolTeacherList = parameters.SchoolTeacherListService.GetSchoolTeacherListBySchoolId(schoolId)
                .Select(c => new SchoolTeacherListViewModel
                {
                    UserID = c.UserID,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    UserName = c.UserName,
                    ClassName = c.ClassName,
                    SchoolID = c.SchoolID,
                    ClassID = c.ClassID,
                    Active = c.Active,
                    Action = CurrentUser.RoleId != (int)Permissions.SchoolAdmin ? "move" : ""
                });

            var parser = new DataTableParser<SchoolTeacherListViewModel>();
            return Json(parser.Parse(schoolTeacherList), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageClasses)]
        public ActionResult AddClass(int teacherID, string fromManageSchools)
        {
            ViewBag.FromManageSchools = false;

            if (!string.IsNullOrEmpty(fromManageSchools))
            {
                ViewBag.FromManageSchools = bool.Parse(fromManageSchools);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddClass(AddClassViewModel model)
        {
            if (!parameters.VulnerabilityService.HasRightToAcessUser(CurrentUser, model.TeacherId.Value, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure> { new ValidationFailure("", "Access denied") } }, JsonRequestBehavior.AllowGet);
            }

            if (CurrentUser.IsTeacher && CurrentUser.Id != model.TeacherId.GetValueOrDefault())
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure> { new ValidationFailure("", "Access denied") } }, JsonRequestBehavior.AllowGet);
            }
            //check district term
            var districtTerms = parameters.DistrictTermService.GetDistrictTermByDistrictID(model.DistrictId.GetValueOrDefault());
            if (!districtTerms.Any(x => x.DistrictTermID == model.DistrictTermId))
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure> { new ValidationFailure("", "" + LabelHelper.DistrictLabel + " term is invalid") } }, JsonRequestBehavior.AllowGet);
            }
            //check
            model.SetValidator(parameters.AddClassViewModelValidator);
            if (!model.IsValid)
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors }, JsonRequestBehavior.AllowGet);
            }

            var newClass = MapViewModelToClass(model);
            int[] arr = { 2, 3, 8 };
            var vTeacher = parameters.UserService.GetUserById(model.TeacherId.GetValueOrDefault());
            if (arr.Contains(CurrentUser.RoleId))
            {
                newClass.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            else
            {
                if (vTeacher != null)
                {
                    newClass.DistrictId = vTeacher.DistrictId.GetValueOrDefault();
                }
            }
            parameters.ClassService.SaveClass(newClass);
            if (newClass.TeacherId.HasValue)
            {
                InsertClassUser(newClass.TeacherId.Value, newClass);

                if (vTeacher != null)
                {
                    vTeacher.DateConfirmedActive = DateTime.UtcNow;
                    parameters.UserService.SaveUser(vTeacher);
                }
            }
            return Json(new { Success = true, RedirectUrl = Url.Action("EditClass", new { id = newClass.Id }) }, JsonRequestBehavior.AllowGet);
        }

        private Class MapViewModelToClass(AddClassViewModel model)
        {
            var clazz = new Class
            {
                ClassType = model.ClassTypeId,
                DistrictTermId = model.DistrictTermId,
                Name = model.Course,
                Course = model.Course,
                Section = model.Section,
                CourseNumber = model.CourseNumber,
                SchoolId = model.SchoolId,
                TeacherId = model.TeacherId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                ModifiedUser = CurrentUser.Id,
                ModifiedBy = "Portal"
            };
            if (!string.IsNullOrWhiteSpace(model.Section)) clazz.Name += " " + model.Section;
            return clazz;
        }

        private void InsertClassUser(int teacherId, Class newClass)
        {
            var userSchool = new ClassUser
            {
                UserId = teacherId,
                ClassId = newClass.Id,
                ClassUserLOEId = 1
            };
            parameters.ClassUserService.InsertClassUser(userSchool);
        }

        [HttpGet]
        public ActionResult GetAvailableStudents(int classId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            Class aClass = parameters.ClassService.GetClassById(classId);
            if (aClass.IsNotNull())
            {
                //get setup hide or show "Add New Student" in configuration table
                var isShowAddNewStudent = parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue(Util.IsShow_AddNewStudentButton, "true");
                ViewBag.IsShowAddNewStudent = CurrentUser.IsAdmin() && isShowAddNewStudent.ToLower() == "true";

                return PartialView("_AvailableStudents", aClass);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAvailableStudentsEnhancement(int classId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            Class aClass = parameters.ClassService.GetClassById(classId);
            if (aClass.IsNotNull())
            {
                if (CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin)
                {
                    if (aClass.DistrictId.HasValue && aClass.DistrictId.Value > 0)
                    {
                        SelectedDistrictID = aClass.DistrictId.GetValueOrDefault();
                    }
                    else
                    {
                        var objDistrictTerm = parameters.DistrictTermService.GetDistrictTermById(aClass.DistrictTermId.GetValueOrDefault());
                        if (objDistrictTerm != null)
                        {
                            SelectedDistrictID = objDistrictTerm.DistrictID;
                        }
                    }
                }
                else
                {
                    SelectedDistrictID = CurrentUser.DistrictId.GetValueOrDefault();
                }

                //get setup hide or show "Add New Student" in configuration table
                var isShowAddNewStudent = parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue(Util.IsShow_AddNewStudentButton, "true");
                var isShowAddNewStudentBool = bool.Parse(isShowAddNewStudent) && CurrentUser.IsAdmin();

                List<int> studentIdList = parameters.StudentsInClassService.GetAllStudentInClass(classId).Select(x => x.StudentID).ToList();
                var programs = parameters.StudentService.GetProgramsStudent(SelectedDistrictID, CurrentUser.Id, CurrentUser.RoleId).ToList();
                var programItems = programs.Where(x => !studentIdList.Contains(x.StudentID)).Select(x => new ListItem() { Id = x.ProgramID, Name = x.ProgramName }).DistinctBy(x => x.Id).OrderBy(x => x.Name).ToList();
                var grades = parameters.StudentService.GetGradesStudent(SelectedDistrictID, CurrentUser.Id, CurrentUser.RoleId).ToList();
                var gradeItems = grades.Where(x => !studentIdList.Contains(x.StudentID)).DistinctBy(x => x.GradeID).OrderBy(x => x.Order).Select(x => new ListItem() { Id = x.GradeID, Name = x.GradeName }).ToList();

                var model = new ManageClassEnhancement() { Class = aClass, Programs = programItems, Grades = gradeItems, IsShowAddNewStudent = isShowAddNewStudentBool };
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStudentsListView(int classId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            Class aClass = parameters.ClassService.GetClassById(classId);
            if (aClass.IsNotNull())
            {
                return PartialView("_ListStudentByClass", aClass);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTeachersListView(int classId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            ViewBag.IsAlowEditTeacher = true;
            ViewBag.CurrentUserId = CurrentUser.Id;
            ViewBag.IsTeacher = CurrentUser.IsTeacher;
            if (CurrentUser.IsTeacher)
                ViewBag.IsAlowEditTeacher = parameters.ClassUserService.CheckUserIsPrimaryTeacher(classId, CurrentUser.Id);

            var model = parameters.ClassService.GetClassById(classId);
            if (model.IsNotNull())
            {
                return PartialView("_ListTeachersByClass", model);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStudentByClassId(int classId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var classStudent = parameters.ClassStudentService.GetClassStudentsByClassId(classId).Where(o => o.Active != null && o.Active.Value).ToList();

            var model = classStudent.Select(s => new ClassStudentViewModel
            {
                StudentId = s.StudentId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Code = s.Code,
                ClassId = s.ClassId
            });

            var parser = new DataTableParser<ClassStudentViewModel>();
            return Json(parser.Parse(model.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStudentsInDistrict(int classId, bool showInactive = false)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var students = GetStudentsAvailableToAssignClass(classId);

            if (!showInactive)
            {
                students = students.Where(x => x.Status.Equals(1));
            }

            var studentList = students.Select(x => new ClassSchoolStudentViewModel
            {
                StudentId = x.StudentId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Code = x.Code,
                Gender = x.Gender,
                Grade = x.Grade
            });

            var parser = new DataTableParser<ClassSchoolStudentViewModel>();
            return Json(parser.Parse(studentList), JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetStudentsInDistrictByFilter(int classId, string programIdList, string gradeIdList, bool showInactive = false)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var students = GetAvailableStudents(classId, programIdList, gradeIdList, showInactive);

            var studentList = students.Select(x => new ClassSchoolStudentViewModel
            {
                StudentId = x.StudentId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Code = x.Code,
                Gender = x.Gender,
                Grade = x.Grade
            });

            var parser = new DataTableParser<ClassSchoolStudentViewModel>();
            return Json(parser.Parse(studentList.AsQueryable()), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetStudentsInDistrictByFilterCustom(GetStudentsInDistrictByFilterRequest request)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, request.ClassId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var students = GetAvailableStudents(request.ClassId, string.IsNullOrEmpty(request.ProgramIdList) ? "" : request.ProgramIdList, string.IsNullOrEmpty(request.GradeIdList) ? "" : request.GradeIdList, false);

            var studentList = students.Select(x => new ClassSchoolStudentViewModel
            {
                StudentId = x.StudentId,
                FirstName = x.FirstName ?? string.Empty,
                LastName = x.LastName ?? string.Empty,
                Code = x.Code ?? string.Empty,
                Gender = x.Gender ?? string.Empty,
                Grade = x.Grade ?? string.Empty
            }).ToList();

            if (!string.IsNullOrEmpty(request.sSearch))
            {
                studentList = studentList.Where(x => x.FirstName.ToLower().Contains(request.sSearch.Trim().ToLower())
                                        || x.LastName.ToLower().Contains(request.sSearch.Trim().ToLower())
                                        || x.Code.ToLower().Contains(request.sSearch.Trim().ToLower())
                                        || x.Gender.ToLower().Contains(request.sSearch.Trim().ToLower())
                                        || x.Grade.ToLower().Contains(request.sSearch.Trim().ToLower())).ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.sColumns) && request.iSortCol_0.HasValue)
            {
                var columns = request.sColumns.Split(',');
                var sortColumn = columns[request.iSortCol_0.Value];
                var sortDirection = request.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
                switch (sortColumn)
                {
                    case "FirstName":
                        studentList = sortDirection.Equals("ASC")
                            ? studentList.OrderBy(x => x.FirstName).ToList()
                            : studentList.OrderByDescending(x => x.FirstName).ToList();
                        break;

                    case "LastName":
                        studentList = sortDirection.Equals("ASC")
                            ? studentList.OrderBy(x => x.LastName).ToList()
                            : studentList.OrderByDescending(x => x.LastName).ToList();
                        break;

                    case "Code":
                        studentList = sortDirection.Equals("ASC")
                            ? studentList.OrderBy(x => x.Code).ToList()
                            : studentList.OrderByDescending(x => x.Code).ToList();
                        break;

                    case "Gender":
                        studentList = sortDirection.Equals("ASC")
                            ? studentList.OrderBy(x => x.Gender).ToList()
                            : studentList.OrderByDescending(x => x.Gender).ToList();
                        break;

                    case "Grade":
                        studentList = sortDirection.Equals("ASC")
                            ? studentList.OrderBy(x => x.Grade).ToList()
                            : studentList.OrderByDescending(x => x.Grade).ToList();
                        break;
                }
            }

            var result = new GenericDataTableResponse<ClassSchoolStudentViewModel>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = studentList.Skip(request.iDisplayStart).Take(request.iDisplayLength).ToList(),
                iTotalDisplayRecords = studentList.Count,
                iTotalRecords = studentList.Count
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSelectedStudentIncaseSelectAll(int classId, string programIdList, string gradeIdList, string searchText, string removeStudents, bool showInactive = false)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            var students = GetAvailableStudents(classId, programIdList, gradeIdList, showInactive);
            if (!string.IsNullOrEmpty(removeStudents))
            {
                students = students.Where(x => !removeStudents.Contains(x.StudentId.ToString())).ToList();
            }
            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim().ToLower();
                students =
                    students.Where(x => (!string.IsNullOrEmpty(x.FirstName) && x.FirstName.ToLower().StartsWith(searchText))
                                        || (!string.IsNullOrEmpty(x.LastName) && x.LastName.ToLower().StartsWith(searchText))
                                        || (!string.IsNullOrEmpty(x.Code) && x.Code.ToLower().StartsWith(searchText))
                                        || (!string.IsNullOrEmpty(x.Gender) && x.Gender.ToLower().StartsWith(searchText))
                                        || (!string.IsNullOrEmpty(x.Grade) && x.Grade.ToLower().StartsWith(searchText))).ToList();
            }
            var studentList = students.Select(x => new ClassSchoolStudentViewModel
            {
                StudentId = x.StudentId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Code = x.Code,
                Gender = x.Gender,
                Grade = x.Grade
            });
            return Json(studentList, JsonRequestBehavior.AllowGet);
        }

        private List<StudentGenderGrade> GetAvailableStudents(int classId, string programIdList, string gradeIdList, bool showInactive)
        {
            List<int> studentIdList =
                parameters.StudentsInClassService.GetAllStudentInClass(classId).Select(x => x.StudentID).ToList();

            if (!string.IsNullOrEmpty(programIdList))
                programIdList = string.Format(",{0},", programIdList);
            if (!string.IsNullOrEmpty(gradeIdList))
                gradeIdList = string.Format(",{0},", gradeIdList);

            var districtId = SelectedDistrictID;
            if (districtId <= 0)
            {
                var classInfo = parameters.ClassService.GetClassById(classId);
                var school = parameters.SchoolService.GetSchoolById(classInfo.SchoolId ?? 0);
                districtId = school.DistrictId;
            }
            var students = parameters.StudentService.GetStudentsAvailableByFilter(districtId, programIdList, gradeIdList,
                showInactive, CurrentUser.Id, CurrentUser.RoleId);
            if (studentIdList.Any())
            {
                students = students.Where(x => !studentIdList.Contains(x.StudentId)).ToList();
            }
            return students;
        }

        private IQueryable<StudentGenderGrade> GetStudentsAvailableToAssignClass(int classId)
        {
            List<int> studentIdList = parameters.StudentsInClassService.GetAllStudentInClass(classId).Select(x => x.StudentID).ToList();

            IQueryable<StudentGenderGrade> students;
            var districtId = SelectedDistrictID;
            if (districtId <= 0)
            {
                var classInfo = parameters.ClassService.GetClassById(classId);
                var school = parameters.SchoolService.GetSchoolById(classInfo.SchoolId ?? 0);
                districtId = school.DistrictId;
            }
            if (CurrentUser.RoleId == (int)Permissions.SchoolAdmin || CurrentUser.RoleId == (int)Permissions.Teacher)
            {
                List<int?> schoolIdList = parameters.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(
                                        x => x.SchoolId).ToList();
                students = parameters.StudentService.GetAvailableStudentsForTeacherSchoolAdmin(districtId, schoolIdList, studentIdList);
            }
            else
            {
                //TODO: Fix bug NetworkAdmin
                if (CurrentUser.IsPublisher() || CurrentUser.IsNetworkAdmin)
                {
                    var objClass = parameters.ClassService.GetClassById(classId);
                    if (objClass != null)
                    {
                        if (objClass.DistrictId.HasValue && objClass.DistrictId.Value > 0)
                        {
                            SelectedDistrictID = objClass.DistrictId.GetValueOrDefault();
                        }
                        else
                        {
                            var objDistrictTerm = parameters.DistrictTermService.GetDistrictTermById(objClass.DistrictTermId.GetValueOrDefault());
                            if (objDistrictTerm != null)
                            {
                                SelectedDistrictID = objClass.DistrictId.GetValueOrDefault();
                            }
                        }
                    }
                }
                students = parameters.StudentService.GetAllStudentsInDistrict(districtId, studentIdList);
            }
            return students;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddStudentToClass(int studentId, int classId)
        {

            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            //if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentId.ToString())) // using this check here is wrong because at this time student still not assigned to class
            //{
            //    return Json(false);
            //}
            var isValidStudentIdForAssignToClass =
                GetStudentsAvailableToAssignClass(classId)
                    .Where(x => x.Status.Equals(1))
                    .Any(x => x.StudentId == studentId);
            if (!isValidStudentIdForAssignToClass)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            List<int> studentIdList = parameters.StudentsInClassService.GetAllStudentInClass(classId).Select(x => x.StudentID).ToList();

            IQueryable<StudentGenderGrade> students;

            if (CurrentUser.RoleId == (int)Permissions.SchoolAdmin || CurrentUser.RoleId == (int)Permissions.Teacher)
            {
                List<int?> schoolIdList = parameters.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(
                                        x => x.SchoolId).ToList();
                students = parameters.StudentService.GetAvailableStudentsForTeacherSchoolAdmin(SelectedDistrictID, schoolIdList, studentIdList);
            }
            else
            {
                students = parameters.StudentService.GetAllStudentsInDistrict(SelectedDistrictID, studentIdList);
            }

            if (students.Any(s => s.StudentId == studentId))
            {
                var classStudent = new ClassStudentData
                {
                    ClassID = classId,
                    StudentID = studentId,
                    Active = true
                };

                parameters.ClassStudentDataService.SaveClassStudent(classStudent);
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddManyStudentToClass(StudentAdding model)
        {

            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, model.ClassId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (!model.IsCheckAll && string.IsNullOrEmpty(model.StudentIdsStr))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            string studentIdsValid = string.Empty;

            if (model.IsCheckAll)
            {
                var students = GetAvailableStudents(model.ClassId, model.ProgramIdList ?? string.Empty, model.GradeIdList ?? string.Empty, false);
                if (!string.IsNullOrEmpty(model.RemoveStudents))
                {
                    students = students.Where(x => !model.RemoveStudents.Contains(x.StudentId.ToString())).ToList();
                }
                if (!string.IsNullOrWhiteSpace(model.SearchBox))
                {
                    var parser = new DataTableParser<StudentGenderGrade>();
                    var studentFilter = parser.ParseNotPaging(students.AsQueryable(), false, model.SearchBox, model.ColumnSearchs.Split(',')?.Select(Int32.Parse)?.ToList());
                    if(studentFilter != null && studentFilter.aaData != null)
                    {
                        var studentsFilter = studentFilter.aaData.ToArray();
                        var properties = typeof(StudentGenderGrade).GetProperties().ToList();
                        var studentIdIndex = properties.FindIndex(x => x.Name == nameof(StudentGenderGrade.StudentId));
                        var studentIds = studentsFilter.Select(m => m.ElementAt(studentIdIndex)).Distinct().ToList();
                        studentIdsValid = string.Join(",", studentIds);
                    }
                }
                else
                {
                    studentIdsValid = string.Join(",", students.Select(x => x.StudentId).Distinct());
                } 
                
            }
            if (!model.IsCheckAll && !string.IsNullOrEmpty(model.StudentIdsStr))
            {
                var studentIds = model.StudentIdsStr
                            .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => Convert.ToInt32(x))
                            .ToList();

                var studenstInClass = parameters.StudentsInClassService.GetAllStudentInClass(model.ClassId).Select(x => x.StudentID).ToList();

                studentIdsValid = string.Join(",", studentIds.Except(studenstInClass).ToList());
            }

            parameters.StudentService.AddManyStudentsToClass(model.ClassId, studentIdsValid);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ActivateTeacher(int? userId)
        {
            if (userId.HasValue && !parameters.VulnerabilityService.HasRightToAcessUser(CurrentUser, userId.Value, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            User user = parameters.UserService.GetUserById(userId.GetValueOrDefault());
            if (user.IsNotNull())
            {
                user.Active = true;
                parameters.UserService.SaveUser(user);
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeactivateTeacher(int userId)
        {
            if (!parameters.VulnerabilityService.HasRightToAcessUser(CurrentUser, userId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var listError = new List<ValidationFailure>();
            var user = parameters.UserService.GetUserById(userId);
            if (user.IsNull())
            {
                listError.Add(new ValidationFailure("", "Cannot deactivate because the teacher's info did not exist."));
                return Json(new { Success = false, ErrorList = listError }, JsonRequestBehavior.AllowGet);
            }

            var checkTeacherHasClass = parameters.ClassListService.GetClassListByPrimaryTeacherID(userId).Any();
            if (checkTeacherHasClass)
            {
                listError.Add(new ValidationFailure("", "Cannot deactivate this teacher because he has active classes."));
                return Json(new { Success = false, ErrorList = listError }, JsonRequestBehavior.AllowGet);
            }

            user.Active = false;
            parameters.UserService.SaveUser(user);

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddNewStudentToClass(int classId)
        {
            AddNewStudentToClassId = classId;
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetAllDistricts()
        {
            var districts = parameters.DistrictService.GetDistricts();
            if (CurrentUser.IsNetworkAdmin)
            {
                districts = districts.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id));
            }
            var data = districts.Select(x => new ListItem { Name = x.Name, Id = x.Id }).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSchoolsByUser(int userId)
        {
            if (userId > 0 && CurrentUser.Id != userId)
            {
                if (!parameters.VulnerabilityService.HasRightToAcessUser(CurrentUser, userId,
                    CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { error = "Has no right on the user" }, JsonRequestBehavior.AllowGet);
                }
            }
            var userSchools = parameters.UserSchoolService.GetSchoolsUserHasAccessTo(userId);
            var data = userSchools.Select(x => new ListItem { Name = x.SchoolName, Id = x.SchoolId.Value }).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchoolsByDistrict(int districtId, string schoolName)
        {
            if (districtId > 0 && CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            var schools = parameters.SchoolService.GetSchoolsByDistrictId(districtId).Select(x => new SchoolListViewModel
            {
                SchoolID = x.Id,
                SchoolName = x.Name,
                Code = x.Code,
                StateCode = x.StateCode
            });

            if (!string.IsNullOrEmpty(schoolName))
            {
                schoolName = schoolName.ToLower();
                schools = schools.Where(m => m.SchoolName.Contains(schoolName));
            }

            var parser = new DataTableParser<SchoolListViewModel>();
            return Json(parser.Parse2018(schools), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchoolsByDistrictV2(int districtId, int schoolId)
        {
            if (districtId > 0 && CurrentUser.RoleId != (int)Permissions.Publisher && CurrentUser.RoleId != (int)Permissions.NetworkAdmin)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            var parser = new DataTableParserProc<SchoolListViewModel>();
            var generalSearch = Request["sSearch"] ?? string.Empty;
            int? totalRecords = 0;

            var input = new GetSchoolRequestModel()
            {
                DistrictId = districtId,
                SchoolId = schoolId,
                StartIndex = parser.StartIndex,
                PageSize = parser.PageSize,
                SortColumns = parser.SortableColumns,
                GeneralSearch = generalSearch?.Trim()
            };

            var schools = parameters.SchoolService.GetSchoolsByDistrictV2(input, ref totalRecords)
                .Select(x => new SchoolListViewModel()
                {
                    SchoolID = x.Id,
                    SchoolName = x.Name,
                    Code = x.Code,
                    StateCode = x.StateCode
                }).AsQueryable();

            return Json(parser.Parse(schools, totalRecords ?? 0), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchoolsByDistrictRaw(int districtId)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right to access district" }, JsonRequestBehavior.AllowGet);
            }

            var schools = GetSchoolsByDistrictId(districtId);
            return Json(schools, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTermsByDistrict(int districtId)
        {
            if (districtId > 0 && CurrentUser.IsDistrictAdmin)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            var terms = parameters.DistrictTermService.GetAllTermsByDistrictID(districtId).Select(x => new TermViewModel
            {
                TermID = x.DistrictTermID,
                Name = x.Name,
                DateStart = x.DateStart,
                DateEnd = x.DateEnd
            });
            var parser = new DataTableParser<TermViewModel>();
            return Json(parser.Parse2018(terms), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransferTermsByDistrict(int districtId)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right to access district" }, JsonRequestBehavior.AllowGet);
            }
            var terms = parameters.DistrictTermService.GetDistrictTermByDistrictID(districtId).Select(x => new ListItem
            {
                Id = x.DistrictTermID,
                Name = x.Name
            });
            return Json(terms, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserNotMatchSchool(int schoolId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var school = parameters.SchoolService.GetSchoolById(schoolId);
            if (school.IsNull())
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_ListUserNotMatchSchool", school);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetUserNotMatchSchool(int schoolId, int districtId)
        {
            var validUserSchoolRoleId = new[] { 2, 3, 5, 8, 27 };

            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            IEnumerable<int> lstUser = parameters.UserSchoolService.ListUserIdBySchoolId(schoolId);
            var userNotMatch = parameters.UserService.GetUserNotAssociatedWithSchool(districtId, lstUser).Where(x => validUserSchoolRoleId.Contains(x.RoleId));//Only Publisher, " + LabelHelper.DistrictLabel + " Admin, Shool Admin, Teacher
            userNotMatch = userNotMatch.Where(x => x.UserStatusId == (int)UserStatus.Active);
            var users = userNotMatch.Select(o => new UserNotMatchSchoolViewModel
            {
                UserId = o.Id,
                UserName = o.UserName,
                RoleId = o.RoleId,
                SchoolId = schoolId
            }).ToList();
            //var teachersHasTerm = parameters.TeacherDistrictTermService.GetTeachersHasTermsInDistrict(districtId).Select(x => x.UserId).ToList();
            //users = users.Where(x => teachersHasTerm.Contains(x.UserId)).ToList();

            var parser = new DataTableParser<UserNotMatchSchoolViewModel>();
            return Json(parser.Parse(users.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetUserNotMatchSchool_V2(int schoolId, int districtId)
        {
            var validUserSchoolRoleId = new[] { 2, 3, 5, 8, 27 };

            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            IEnumerable<int> lstUser = parameters.UserSchoolService.ListUserIdBySchoolId(schoolId);
            var userNotMatch = parameters.UserService.GetUserNotAssociatedWithSchool(districtId, lstUser).Where(x => validUserSchoolRoleId.Contains(x.RoleId));//Only Publisher, " + LabelHelper.DistrictLabel + " Admin, Shool Admin, Teacher
            userNotMatch = userNotMatch.Where(x => x.UserStatusId == (int)UserStatus.Active);

            var users = userNotMatch.Select(o => new UserNotMatchSchoolDataViewModel
            {
                UserId = o.Id,
                UserName = o.UserName,
                RoleId = o.RoleId,
                SchoolId = schoolId
            }).ToList();

            var roles = parameters.UserService.GetRoles().ToList();
            foreach (var user in users)
            {
                user.RoleName = roles.FirstOrDefault(p => p.RoleId == user.RoleId)?.Name;
            }

            var parser = new DataTableParser<UserNotMatchSchoolDataViewModel>();
            return Json(parser.Parse(users.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTeachersForSchool(int schoolId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var school = parameters.SchoolService.GetSchoolById(schoolId);
            if (school.IsNull())
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return PartialView("_ListUserBySchool", school);
        }

        [HttpPost, AjaxOnly]
        public ActionResult RemoveUserSchool(int userSchoolId)
        {
            var userSchool = parameters.UserSchoolService.GetUserSchoolById(userSchoolId);
            if (userSchool.IsNull())
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (CurrentUser.Id == userSchool.UserId)
            {
                return Json(new { message = "Error. You cannot remove your account from this school.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
            }
            if (parameters.DistrictTermClassService.SchoolHaveActiveClass(userSchool.UserId, userSchool.SchoolId.GetValueOrDefault(), userSchool.DistrictId.GetValueOrDefault()))
            {
                return Json(new { message = "Error. You cannot remove this user from this school. Teacherâ€™s class is still active.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
            }
            if (parameters.DistrictTermClassService.SchoolHaveClassNotStartYet(userSchool.UserId, userSchool.SchoolId.GetValueOrDefault(), userSchool.DistrictId.GetValueOrDefault()))
            {
                return Json(new { message = "Error. You cannot remove this user from this school. Teacherâ€™s class has not started yet.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
            }
            parameters.UserSchoolService.RemoveUserSchool(userSchool);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetUserBySchoolId(int schoolId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var userschool = parameters.UserSchoolService.GetSchoolsUserBySchoolId(schoolId);
            var users = userschool.Select(o => new UserSchoolViewModel
            {
                UserSchoolId = o.UserSchoolId ?? 0,
                UserName = o.UserName,
                RoleId = (int)o.Role
            });
            var parser = new DataTableParser<UserSchoolViewModel>();
            return Json(parser.Parse(users), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetUserBySchoolId_V2(int schoolId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var userschool = parameters.UserSchoolService.GetSchoolsUserBySchoolId(schoolId);
            var users = userschool.Select(o => new UserSchoolDataViewModel
            {
                UserSchoolId = o.UserSchoolId ?? 0,
                UserName = o.UserName,
                RoleName = o.RoleName
            });
            var parser = new DataTableParser<UserSchoolDataViewModel>();
            return Json(parser.Parse(users), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassesBySchoolId(int schoolId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var classes = parameters.ClassListService.GetClassListBySchoolID(schoolId).Select(c => new ClassListViewModel
            {
                ID = c.ClassId,
                UserID = c.UserId ?? 0,
                Name = c.ClassName,
                Term = c.TermName,
                PrimaryTeacher = c.PrimaryTeacher,
                Locked = c.IsLocked,
                SchoolID = c.SchoolID
            });
            var parser = new DataTableParser<ClassListViewModel>();
            return Json(parser.Parse(classes), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult MoveTeacher(int oldSchoolId, int newSchoolId, int teacherId)
        {
            if (!parameters.VulnerabilityService.HasRightToAcessUser(CurrentUser, teacherId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, oldSchoolId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (!parameters.VulnerabilityService.CheckUserCanAccessSchool(CurrentUser, newSchoolId))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var teacherInfo = parameters.SchoolTeacherListService.GetSchoolTeacherListBySchoolId(oldSchoolId).FirstOrDefault(s => s.UserID.Equals(teacherId));
            if (teacherInfo.IsNull())
            {
                var listError = new List<ValidationFailure> { new ValidationFailure(string.Empty, "Invalid parameter") };
                return Json(new { Success = false, ErrorList = listError }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(teacherInfo.ClassID) == false)
            {
                var listError = new List<ValidationFailure> { new ValidationFailure(string.Empty, "This teacher have some classes.") };
                return Json(new { Success = false, ErrorList = listError }, JsonRequestBehavior.AllowGet);
            }
            var userSchool = parameters.UserSchoolService.GetUserSchoolByUserIdSchoolId(teacherId, oldSchoolId);
            parameters.UserSchoolService.RemoveUserSchool(userSchool);
            userSchool = new UserSchool
            {
                SchoolId = newSchoolId,
                UserId = teacherId,
                DateActive = DateTime.Now,
                InActive = false
            };
            parameters.UserSchoolService.InsertUserSchool(userSchool);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageClasses)]
        public ActionResult EditClass(int id, string fromManageSchools)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, id))
            {
                return RedirectToAction("ManageSchoolAndClass");
            }

            var aClass = parameters.ClassService.GetClassById(id);
            if (aClass.IsNotNull())
            {
                var districtTerm = parameters.DistrictTermService.GetDistrictTermById(aClass.DistrictTermId.HasValue ? aClass.DistrictTermId.Value : 0);
                if (districtTerm.IsNotNull())
                {
                    ViewBag.ListDistrictTerm = parameters.DistrictTermService.GetDistrictTermByDistrictID(districtTerm.DistrictID).Select(x => new ListItemsViewModel { Id = x.DistrictTermID, Name = x.Name }).ToList();
                }
                ViewBag.ClassTypes = parameters.ClassTypeService.GetClassTypes().Select(x => new SelectListItem
                {
                    Selected = aClass.ClassType.HasValue && aClass.ClassType.Value.Equals(x.Id),
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                ViewBag.FromManageSchools = false;
                if (!string.IsNullOrEmpty(fromManageSchools))
                {
                    ViewBag.FromManageSchools = bool.Parse(fromManageSchools);
                }
                ViewBag.IsRosteredClasses = false;

                if ((aClass.SISID.HasValue && aClass.SISID > 0) || aClass.ModifiedBy.Equals(Constanst.MODIFIEDBY_ROSTERLOADER))
                {
                    ViewBag.IsRosteredClasses = true;
                }
                var metaClassConfig = parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(aClass.DistrictId.Value, Constanst.CLASS_META_DATA).FirstOrDefault();
                if (metaClassConfig != null && !string.IsNullOrEmpty(metaClassConfig.Value))
                {
                    aClass.HasConfigClassMeta = true;
                    var aggregateSubjectMapping = parameters.DistrictDecodeService.GetAggregateSubjectMappingByDistrict(aClass.DistrictId.Value);
                    if (aggregateSubjectMapping != null && metaClassConfig != null)
                    {
                        aClass.SubjectMappingOptionJson = JsonConvert.SerializeObject(aggregateSubjectMapping);
                        var classIds = new List<int>() { aClass.Id };
                        aClass.ClassMetas = parameters.ClassService.GetMetaClassByClassId(classIds, metaClassConfig.Value);
                    }
                }

                return View(aClass);
            }
            return RedirectToAction("ManageSchoolAndClass");
        }

        [HttpGet]
        public ActionResult SchoolDetail(int id)
        {
            if (id > 0 && (CurrentUser.IsDistrictAdminOrPublisher || parameters.UserSchoolService.CanAccessSchoolByAdminSchool(id, CurrentUser)))
            {
                School school = parameters.SchoolService.GetSchoolById(id);
                if (school.IsNotNull() && school.DistrictId.Equals(CurrentUser.DistrictId.GetValueOrDefault()))
                {
                    return View(school);
                }
            }
            return RedirectToAction("ManageSchoolAndClass");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClass(Class editedClass)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, editedClass.Id))
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure> { new ValidationFailure("", "Access denied") } }, JsonRequestBehavior.AllowGet);
            }
            var classInDB = parameters.ClassService.GetClassById(editedClass.Id);
            if (classInDB.IsNotNull())
            {
                if (parameters.ClassUserService.ClassDoesNotHavePrimaryTeacher(editedClass.Id))
                {
                    var errorList = classInDB.ValidationErrors.ToList();
                    errorList.Add(new ValidationFailure(string.Empty, "Class must have a primary teacher."));
                    return Json(new { Success = false, ErrorList = errorList }, JsonRequestBehavior.AllowGet);
                }
                if (!string.IsNullOrEmpty(editedClass.ClassMetaStr))
                {
                    var classMeta = JsonConvert.DeserializeObject<List<ClassMetaDto>>(editedClass.ClassMetaStr);
                    var classMetaSave = new List<CreateClassMetas>()
                    {
                        new CreateClassMetas()
                        {
                            ClassId= editedClass.Id,
                            ClassMetas = classMeta
                        }
                    };

                    parameters.ClassService.SaveClassMeta(classMetaSave);
                }

                //Never edit RosterClass. Still allow primary/co-teachers to be added, changed and removed.
                if ((classInDB.SISID.HasValue && classInDB.SISID > 0) || classInDB.ModifiedBy.Equals(Constanst.MODIFIEDBY_ROSTERLOADER) || classInDB.ModifiedBy.Equals(Constanst.MODIFIEDBY_FOCUS_GROUP_AUTOMATION))
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                classInDB.Name = editedClass.Course;
                if (!string.IsNullOrWhiteSpace(editedClass.Section)) classInDB.Name += " " + editedClass.Section;
                classInDB.Course = editedClass.Course;
                classInDB.Section = editedClass.Section;
                classInDB.CourseNumber = editedClass.CourseNumber;
                classInDB.DistrictTermId = editedClass.DistrictTermId;
                classInDB.ClassType = editedClass.ClassType;

                classInDB.SetValidator(parameters.ClassValidator);
                if (!classInDB.IsValid)
                {
                    return Json(new { Success = false, ErrorList = (List<ValidationFailure>)classInDB.ValidationErrors }, JsonRequestBehavior.AllowGet);
                }
                classInDB.ModifiedDate = DateTime.UtcNow;
                classInDB.ModifiedUser = CurrentUser.Id;
                classInDB.ModifiedBy = "Portal";
                parameters.ClassService.SaveClass(classInDB);
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = false, ErrorList = new List<ValidationFailure> { new ValidationFailure("", "Class not exists") } }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteClass(int classId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return DeleteClassException("Have no right to access");
            }

            var classTestResult = parameters.ClassTestResultListService.GetByClassId(classId).ToList();
            if (classTestResult.Any())
            {
                var listError = BuildErrorMessageForRemovingClass(classTestResult);
                return Json(new { Success = false, ErrorList = listError }, JsonRequestBehavior.AllowGet);
            }

            var bubbleSheets = parameters.BubbleSheetService.GetBubbleSheetsByClass(classId);
            if (bubbleSheets.Any())
            {
                return DeleteClassException("This class cannot be deleted because it has bubble sheets associated with it.");
            }

            var classStudents = parameters.ClassStudentService.GetClassStudentDataByClassId(classId);
            if (classStudents.Any())
            {
                return DeleteClassException("This class cannot be deleted because it has students associated with it.");
            }

            if (parameters.ClassStudentService.HasClassLinkedToSGO(classId))
            {
                return DeleteClassException("This class cannot be deleted because it has SGOs associated with it.");
            }

            parameters.ClassService.DeleteClass(classId);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        private ActionResult DeleteClassException(string exception)
        {
            var errorList = new List<ValidationFailure>
                {
                    new ValidationFailure("", exception)
                };
            return Json(new { Success = false, ErrorList = errorList }, JsonRequestBehavior.AllowGet);
        }

        private List<ValidationFailure> BuildErrorMessageForRemovingClass(List<ClassTestResultList> classTestResult)
        {
            var listError = new List<ValidationFailure>();
            var sb = new StringBuilder();
            sb.Append("The class cannot be removed until the test results listed below have been removed.").Append("<br />");
            var testCount = classTestResult.Count > 10 ? 10 : classTestResult.Count;
            for (var i = 0; i < testCount; i++)
            {
                sb.AppendFormat("{0}: {1}, ", classTestResult[i].TestName, classTestResult[i].TestCount);
            }
            if (classTestResult.Count > 10)
            {
                sb.Append("continue...");
            }
            listError.Add(new ValidationFailure(string.Empty, sb.ToString()));
            return listError;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteBatchClasses(string listIds)
        {
            var arrClassIds = listIds.Split(new[] { ',' });
            var listIdToDelete = new List<int>();
            foreach (var strId in arrClassIds)
            {
                int temp;
                if (int.TryParse(strId, out temp))
                {
                    listIdToDelete.Add(temp);
                }
            }

            var classTestResult = parameters.ClassTestResultListService.GetByClassIdList(listIdToDelete).ToList();
            if (classTestResult.Any())
            {
                var listError = BuildErrorMessageForRemovingClass(classTestResult);
                return Json(new { Success = false, ErrorList = listError }, JsonRequestBehavior.AllowGet);
            }

            var bubbleSheets = parameters.BubbleSheetService.GetBubbleSheetsByClassIdList(listIdToDelete);
            if (bubbleSheets.Any())
            {
                return DeleteClassException("These classes cannot be deleted because it has bubble sheets associated with them.");
            }

            var classStudents = parameters.ClassStudentService.GetClassStudentsByClassIdList(listIdToDelete);
            if (classStudents.Any())
            {
                return DeleteClassException("These classes cannot be deleted because it has students associated with them.");
            }

            foreach (var classId in listIdToDelete)
            {
                parameters.ClassService.DeleteClass(classId);
            }
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTeachers(int classId, int currentTeacherID)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var aClass = parameters.ClassService.GetClassById(classId);
            if (aClass.IsNotNull() && aClass.SchoolId.HasValue)
            {
                var schoolId = parameters.SchoolService.GetSchoolById(aClass.SchoolId.Value).Id;
                var data = parameters.SchoolTeacherService
                                .GetTeachersBySchoolId(schoolId)
                                .Where(x => !x.UserId.Equals(currentTeacherID))
                                .OrderBy(x => x.TeacherName)
                                .Select(x => new ListItem
                                {
                                    Id = x.UserId,
                                    Name = x.TeacherName
                                });
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClassesByUserId(int? userID, int? schoolID, string assignedClassIdString, string subject, string className, int? districtId, bool isFirstLoad = false)
        {
            var parser = new DataTableParser<ClassListViewModel>();
            if (isFirstLoad || !schoolID.HasValue)
            {
                return Json(parser.Parse2018(new List<ClassListViewModel>().AsQueryable()), JsonRequestBehavior.AllowGet);
            }
            var classesQuery = userID.HasValue && userID > 0 ?
                parameters.ClassListService.GetClassListByTeacherID(userID.Value) : parameters.ClassListService.Select();

            if (schoolID.HasValue && schoolID.Value > 0)
            {
                classesQuery = classesQuery.Where(o => o.SchoolID == schoolID.Value);
            }

            var classIdList = assignedClassIdString.ParseIdsFromString();
            if (classIdList.Count > 0)
            {
                classesQuery = classesQuery.Where(x => !classIdList.Contains(x.ClassId));
            }
            if (!string.IsNullOrWhiteSpace(subject) && subject != "select" && subject != "No Results Found")
            {
                classesQuery = classesQuery.Where(x => x.Subjects.Contains(subject));
            }

            if (!string.IsNullOrWhiteSpace(className))
            {
                classesQuery = classesQuery.Where(x => x.ClassName.Contains(className));
            }
            var _districtID = districtId.HasValue ? districtId.Value : CurrentUser.IsPublisherOrNetworkAdmin == false ? CurrentUser.DistrictId : 0;
            var hasConfigClassMeta = false;
            if (_districtID.HasValue && _districtID.Value > 0)
            {
                var metaClassConfig = parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(_districtID.Value, Constanst.CLASS_META_DATA).FirstOrDefault();
                if (metaClassConfig != null && !string.IsNullOrEmpty(metaClassConfig.Value))
                {
                    hasConfigClassMeta = true;
                }
            }

            var classes = classesQuery.ToList().Select(c => new ClassListViewModel
            {
                ID = c.ClassId,
                Locked = c.IsLocked,
                Name = c.ClassName,
                Term = c.TermName,
                TermStartDate = c.TermStartDate,
                TermEndDate = c.TermEndDate,
                UserID = c.UserId ?? 0,
                SchoolID = c.SchoolID,
                PrimaryTeacher = c.PrimaryTeacher,
                SchoolName = c.SchoolName,
                Students = c.Students,
                Teachers = c.Teachers,
                ModifiedBy = c.ModifiedBy,
                ClassType = c.ClassType,
                Subjects = c.Subjects,
                HasConfigClassMeta = hasConfigClassMeta
            }).DistinctBy(x => x.ID);

            return Json(parser.Parse2018(classes.AsQueryable()), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMetaDataForClass(int? districtId, List<int> classIds)
        {
            try
            {
                var _districtID = CurrentUser.DistrictId ?? 0;
                var model = new MetaDataDto();
                if (districtId.HasValue && districtId.Value > 0)
                {
                    _districtID = districtId.Value;
                }
                var metaClassConfig = parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(_districtID, Constanst.CLASS_META_DATA).FirstOrDefault();
                if (metaClassConfig != null && !string.IsNullOrEmpty(metaClassConfig.Value))
                {
                    model.HasConfigClassMeta = true;
                    var aggregateSubjectMapping = parameters.DistrictDecodeService.GetAggregateSubjectMappingByDistrict(_districtID);
                    if (aggregateSubjectMapping != null && metaClassConfig != null)
                    {
                        model.SubjectMappingOptionJson = JsonConvert.SerializeObject(aggregateSubjectMapping);
                        model.ClassMetas = parameters.ClassService.GetMetaClassByClassId(classIds, metaClassConfig.Value);
                    }
                }
                return Json(new { data = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { message = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetMetaDataForDropdown(int? districtId)
        {
            var _districtID = CurrentUser.DistrictId ?? 0;
            if (districtId.HasValue && districtId.Value > 0)
            {
                _districtID = districtId.Value;
            }
            var metaClassConfig = parameters.DistrictDecodeService.GetAggregateSubjectMappingByDistrict(_districtID);
            var data = new List<ListItem>();
            if (metaClassConfig != null)
            {
                data = metaClassConfig.Select(s => new ListItem { Name = s.AggregateSubjectName, Id = s.AggregateSubjectMappingID }).OrderBy(x => x.Name).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveMetaDataForClass(List<CreateClassMetas> models)
        {
            try
            {
                parameters.ClassService.SaveClassMeta(models);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetClassesForManageStudent(GetAvailableAddNewStudentAssignClassRequest request)
        {
            if (request.UserId > 0 && CurrentUser.Id != request.UserId)
            {
                if (!parameters.VulnerabilityService.HasRightToAcessUser(CurrentUser, request.UserId.Value,
                    CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { error = "Has no right on the user" }, JsonRequestBehavior.AllowGet);
                }
            }

            var classesQuery = parameters.ClassListService.GetClassListByPrimaryTeacherID(request.UserId.Value);

            if (request.SchoolID.HasValue)
            {
                classesQuery = classesQuery.Where(o => o.SchoolID == request.SchoolID.Value);
            }

            var classIdList = request.AssignedClassIdString.ParseIdsFromString();
            if (classIdList.Count > 0)
            {
                classesQuery = classesQuery.Where(x => !classIdList.Contains(x.ClassId));
            }

            var classes = classesQuery.ToList().Select(c => new ClassListViewModel
            {
                ID = c.ClassId,
                Locked = c.IsLocked,
                Name = c.ClassName,
                Term = c.TermName,
                TermStartDate = c.TermStartDate,
                TermEndDate = c.TermEndDate,
                UserID = c.UserId ?? 0,
                SchoolID = c.SchoolID,
                PrimaryTeacher = c.PrimaryTeacher,
                SchoolName = c.SchoolName
            }).DistinctBy(x => x.ID);

            var result = new GenericDataTableResponse<ClassListViewModel>()
            {
                sEcho = request.sEcho,
                sColumns = request.sColumns,
                aaData = classes.ToList()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassesDropdown(int schoolID)
        {
            var items = parameters.ClassListService.Select().Where(x => x.SchoolID == schoolID);

            if (CurrentUser.IsTeacher)
                items = items.Where(x => x.UserId == CurrentUser.Id);

            var classes = items.Select(x => new ListItem
            {
                Id = x.ClassId,
                Name = x.ClassName
            }).ToList().DistinctBy(x => x.Id);

            return new LargeJsonResult
            {
                Data = classes.OrderBy(x => x.Name),
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpGet]
        public ActionResult GetAvailableProgramsByStudentId(int studentId)
        {
            //Check right to access studentId
            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentId.ToString()))
            {
                return Json(new { error = "Has no right to access student." }, JsonRequestBehavior.AllowGet);
            }
            var student = parameters.StudentService.GetStudentById(studentId);
            if (student.IsNull())
            {
                return Json(new { message = "Student is invalid.", success = false, type = "error" });
            }
            var notMatchPrograms = GetAuthorizedProgramsByStudentId(studentId);
            var data = notMatchPrograms.Select(x => new ProgramViewModel { Id = x.Id, Name = x.Name });
            var parser = new DataTableParser<ProgramViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddProgramToStudent(int programId, int studentId)
        {
            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentId.ToString()))
            {
                return Json(new { error = "Has no right to access student." }, JsonRequestBehavior.AllowGet);
            }

            var student = parameters.StudentService.GetStudentById(studentId);
            var program = parameters.ProgramService.GetProgramById(programId);

            //if (!parameters.VulnerabilityService.HasRigtToEditProgram(CurrentUser, programId, CurrentUser.DistrictId.GetValueOrDefault()))
            //{
            //    return Json(false);
            //}
            var validProgram = GetAuthorizedProgramsByStudentId(studentId);
            if (!validProgram.Any(x => x.Id != programId))
            {
                return Json(new { error = "Has no right to access program." }, JsonRequestBehavior.AllowGet);
            }

            if (student.IsNull() || program.IsNull())
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if ((CurrentUser.IsDistrictAdmin || CurrentUser.IsNetworkAdmin) &&
                (program.AccessLevelID == (int)AccessLevelEnum.LinkItOnly ||
                 program.AccessLevelID == (int)AccessLevelEnum.StateUsers))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (CurrentUser.IsSchoolAdmin &&
                program.AccessLevelID != (int)AccessLevelEnum.DistrictAndSchoolAdmins &&
                program.AccessLevelID != (int)AccessLevelEnum.AllUsers)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (CurrentUser.IsTeacher && program.AccessLevelID != (int)AccessLevelEnum.AllUsers)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            parameters.StudentProgramService.AddStudentProgram(studentId, programId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProgramsByStudentId(int studentId)
        {
            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentId.ToString()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            var studentPrograms = parameters.StudentProgramService.GetStudentsProgramsByStudentId(studentId);

            if (CurrentUser.IsDistrictAdmin || CurrentUser.IsNetworkAdmin)
            {
                studentPrograms =
                    studentPrograms.Where(
                        x =>
                            x.AccessLevelId != (int)AccessLevelEnum.LinkItOnly &&
                            x.AccessLevelId != (int)AccessLevelEnum.StateUsers);
            }
            if (CurrentUser.IsSchoolAdmin)
            {
                studentPrograms =
                    studentPrograms.Where(
                        x =>
                            x.AccessLevelId == (int)AccessLevelEnum.DistrictAndSchoolAdmins ||
                            x.AccessLevelId == (int)AccessLevelEnum.AllUsers);
            }
            if (CurrentUser.IsTeacher)
            {
                studentPrograms =
                    studentPrograms.Where(
                        x => x.AccessLevelId == (int)AccessLevelEnum.AllUsers);

            }
            var data = studentPrograms.Select(x => new StudentProgramViewModel
            {
                StudentProgramId = x.StudentProgramID,
                ProgramName = x.ProgramName
            });
            var parser = new DataTableParser<StudentProgramViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveProgramFromStudent(int studentProgramId)
        {
            var studentProgram = parameters.StudentProgramService.GetStudentProgramById(studentProgramId);
            if (studentProgram.IsNull())
            {
                return Json(new { error = "Do not exists." }, JsonRequestBehavior.AllowGet);
            }
            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentProgram.StudentID.ToString()))
            {
                return Json(new { error = "Has no right to access student." }, JsonRequestBehavior.AllowGet);
            }


            parameters.StudentProgramService.DeleteStudentProgram(studentProgram);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTeachersForClass(int classId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var classUsers = parameters.ClassUserService.GetClassUsersByClassId(classId);

            var model = classUsers.Select(x => new ClassUserLOEViewModel
            {
                ClassUserId = x.Id,
                TeacherFirstName = x.User.FirstName,
                TeacherLastName = x.User.LastName,
                LOEName = x.ClassUserLOE.Name,
                ClassId = x.ClassId,
                UserId = x.UserId
            });

            var parser = new DataTableParser<ClassUserLOEViewModel>();
            return Json(parser.Parse(model.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddTeacherToClass(int classId, bool isAutoFocusGroup = false)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return RedirectToAction("ManageClass");
            }

            var listLOETypes = AddLOETypes();
            if (isAutoFocusGroup)
            {
                listLOETypes = listLOETypes.Where(x => x.Value != "1").ToList();
            }

            var model = new AddTeacherToClassViewModel
            {
                ClassId = classId,
                LOETypes = listLOETypes,
                Teachers = AddTeachersByClassId(classId),
            };

            return PartialView("_AddTeacherToSchool", model);
        }

        private IEnumerable<SelectListItem> AddLOETypes()
        {
            return parameters.ClassUserService.GetLOETypes().ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(CultureInfo.InvariantCulture)
            });
        }

        private IEnumerable<SelectListItem> AddTeachersByClassId(int classId)
        {
            var districtTerm = parameters.DistrictTermClassService.GetAll().FirstOrDefault(x => x.ClassId == classId);

            var teachers = parameters.ClassUserService.GetTeachersForSchoolByClassId(classId).Where(x => x.UserStatusId == (int)UserStatus.Active).ToList();

            int currentDistrict = CurrentUser.DistrictId.GetValueOrDefault();

            if (CurrentUser.IsDistrictAdmin)
            {
                var districtAdmins = parameters.UserService.GetUsersByDistrictId(currentDistrict).Where(x => x.RoleId == (int)Permissions.DistrictAdmin && x.UserStatusId == (int)UserStatus.Active)
                    .Select(x => new TeacherSchoolClass
                    {
                        TeacherFirstName = x.FirstName,
                        TeacherLastName = x.LastName,
                        UserName = x.UserName,
                        ClassId = classId,
                        UserId = x.Id
                    });

                teachers.AddRange(districtAdmins);
            }

            //should remove current teacher in class
            var classUsers = parameters.ClassUserService.GetClassUsersByClassId(classId).Where(x => x.ClassUserLOEId == 1);
            var userIdList = classUsers.Select(x => x.UserId).ToList();
            teachers = teachers.Where(x => !userIdList.Contains(x.UserId)).ToList();
            return teachers.Select(x => new SelectListItem
            {
                Text = string.IsNullOrEmpty(x.TeacherFirstName) ? string.Format("{0} ({1})", x.TeacherLastName, x.UserName) : string.Format("{0}, {1} ({2})", x.TeacherLastName, x.TeacherFirstName, x.UserName),
                Value = x.UserId.ToString(CultureInfo.InvariantCulture)
            }).OrderBy(x => x.Text);
        }

        [HttpPost, AjaxOnly]
        public ActionResult AddTeacherToClass(AddTeacherToClassViewModel model)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, model.ClassId)
                || !parameters.VulnerabilityService.HasRightToAcessUser(CurrentUser, model.SelectedTeacher, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = false, ErrorList = new List<ValidationFailure> { new ValidationFailure("", "Access denied") } }, JsonRequestBehavior.AllowGet);
            }

            var classUser = new ClassUser
            {
                ClassId = model.ClassId,
                UserId = model.SelectedTeacher,
                ClassUserLOEId = model.SelectedLOE
            };

            classUser.SetValidator(parameters.ClassUserValidator);
            if (!classUser.IsValid)
            {
                return Json(new { Success = false, ErrorList = classUser.ValidationErrors }, JsonRequestBehavior.AllowGet);
            }

            parameters.ClassUserService.InsertClassUser(classUser);
            parameters.UserService.UpdateDateConfirmActive(classUser.UserId);
            if (classUser.ClassUserLOEId == 1)
            {
                parameters.ClassService.UpdateUserId(classUser.ClassId, classUser.UserId, CurrentUser.Id);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, AjaxOnly]
        public ActionResult RemoveTeacherFromClass(int classUserId, int classId, int userId)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            if (!parameters.VulnerabilityService.HasRightToAcessUser(CurrentUser, userId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, message = "Has no right", },
                    JsonRequestBehavior.AllowGet);
            }

            var classUser = parameters.ClassUserService.GetClassUserById(classUserId);

            if (classUser.IsNull())
            {
                return HttpNotFound();
            }

            if (classUser.ClassId != classId || classUser.UserId != userId)
            {
                return HttpNotFound();
            }

            parameters.ClassUserService.DeleteClassUser(classUser);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, AjaxOnly]
        public ActionResult TransferStudents(int oldClassId, int newClassId, string movedStudentIds, bool transferTests)
        {
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, oldClassId))
            {
                return Json(new { error = "Has no right to access class" }, JsonRequestBehavior.AllowGet);
            }
            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, newClassId))
            {
                return Json(new { error = "Has no right to access class" }, JsonRequestBehavior.AllowGet);
            }
            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, movedStudentIds))
            {
                return Json(new { error = "Has no right to access student" }, JsonRequestBehavior.AllowGet);
            }
            var errorMessages = new List<string>();
            var hasErrors = false;

            foreach (var movedStudentId in movedStudentIds.Split(',').Select(int.Parse))
            {
                var student = new Student { Id = movedStudentId };
                var oldClass = parameters.ClassService.GetClassById(oldClassId);
                var newClass = parameters.ClassService.GetClassById(newClassId);

                try
                {
                    parameters.StudentTransferService.TransferStudent(student, oldClass, newClass);
                    if (transferTests)
                    {
                        parameters.StudentTransferService.TransferTests(student, oldClass, newClass);
                    }
                }
                catch (Exception e)
                {
                    hasErrors = true;
                    errorMessages.Add("Error transferring student: " + student.Id);
                }
            }

            return Json(new { success = !hasErrors, errors = errorMessages }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult ReplacePrimaryTeacher(int classId)
        {
            var model = new ReplacePrimaryTeacherViewModel
            {
                ClassId = classId,
                Teachers = AddTeachersByClassId(classId),
            };

            return PartialView("_ReplacePrimaryTeacher", model);
        }

        [HttpPost, AjaxOnly]
        public ActionResult ReplacePrimaryTeacher(ReplacePrimaryTeacherViewModel model)
        {
            var classUser = new ClassUser
            {
                ClassId = model.ClassId,
                UserId = model.SelectedTeacher,
                ClassUserLOEId = 1
            };

            if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classUser.ClassId))
            {
                return Json("Have no right to access", JsonRequestBehavior.AllowGet);
            }

            if (!parameters.VulnerabilityService.HasRightToAcessUser(CurrentUser, classUser.UserId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json("Have no right to access", JsonRequestBehavior.AllowGet);
            }

            parameters.ClassUserService.ReplacePrimaryTeacherClassUser(classUser);

            //Update UserID on Class table
            parameters.ClassService.UpdateUserId(classUser.ClassId, classUser.UserId, CurrentUser.Id);

            var teacher = parameters.UserService.GetUserById(classUser.UserId);
            if (teacher != null)
            {
                teacher.DateConfirmedActive = DateTime.UtcNow;
                parameters.UserService.SaveUser(teacher);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ResetPassword(int userId)
        {
            var model = new ResetPassword { UserId = userId };
            return View("_ResetPassword", model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ResetPassword(ResetPassword model)
        {
            var student = parameters.StudentService.GetStudentById(model.UserId);
            if (student == null)
            {
                ModelState.AddModelError("error", "Student does not exist.  Please try again.");
                return View("_ResetPasswordForm", model);
            }

            if (!string.IsNullOrEmpty(model.NewPassword) && !Regex.IsMatch(model.NewPassword, ConfigurationManager.AppSettings["PasswordRegex"]))
            {
                ModelState.AddModelError("error", ConfigurationManager.AppSettings["PasswordRequirements"]);
            }

            if (!ModelState.IsValid)
            {
                return View("_ResetPasswordForm", model);
            }
            //check right
            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, model.UserId.ToString()))
            {
                return Json(new { error = "Has no right to access student." }, JsonRequestBehavior.AllowGet);
            }

            // LNKT-28753 Allow Different Passwords to be Used between Student Portal and Test Taker
            //student.Password = Md5Hash.GetMd5Hash(model.NewPassword);
            //parameters.StudentService.Save(student);

            var userId = parameters.StudentMetaService.GetUserIdByStudentId(student.Id);
            if (userId > 0)
            {
                var user = parameters.UserService.GetUserById(userId);
                if (user != null)
                {
                    parameters.UserService.ResetUsersPassword(user, model.NewPassword, true);
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ResetStudentPassword(ResetPassword model)
        {
            var student = parameters.StudentService.GetStudentById(model.UserId);
            if (student == null)
            {
                ModelState.AddModelError("error", "Student does not exist. Please try again.");
                return View("_ResetPasswordForm", model);
            }

            if (!string.IsNullOrEmpty(model.NewPassword) && !Regex.IsMatch(model.NewPassword, ConfigurationManager.AppSettings["PasswordRegex"]))
            {
                ModelState.AddModelError("error", ConfigurationManager.AppSettings["PasswordRequirements"]);
            }

            if (!ModelState.IsValid)
            {
                return View("_ResetPasswordForm", model);
            }

            //check right
            if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, model.UserId.ToString()))
            {
                return Json(new { error = "Has no right to access student." }, JsonRequestBehavior.AllowGet);
            }

            student.Password = Md5Hash.GetMd5Hash(model.NewPassword);
            parameters.StudentService.Save(student);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ResetStudentPassword(int userId)
        {
            var model = new ResetPassword { UserId = userId };
            return View("_ResetStudentPassword", model);
        }

        public ActionResult GetClassesByClassIds(string classIdString)
        {
            //classIdString looks like -1-,-2-,-3-, with 1,2,3 are classid
            var classIdList = classIdString.ParseIdsFromString();
            var teacherIds = parameters.ClassUserService.GetPrimaryTeacherByClassIds(classIdList);
            var classes = parameters.ClassListService.Select().Where(x => x.UserId.HasValue && teacherIds.Contains(x.UserId.Value) && classIdList.Contains(x.ClassId)).Select(c => new AddNewStudentAssignClassViewModel
            {
                ID = c.ClassId,
                Name = c.ClassName,
                Term = c.TermName,
                PrimaryTeacher = c.PrimaryTeacher,
                SchoolName = c.SchoolName
            });

            var parser = new DataTableParser<AddNewStudentAssignClassViewModel>();
            return Json(parser.Parse(classes), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadAddNewStudentAssignClassPopup(int? districtId, bool? layoutV2 = false)
        {
            if (!districtId.HasValue || districtId.Value <= 0)
            {
                districtId = CurrentUser.DistrictId;
            }
            var model = new AddNewStudentAssignClassFilter();
            model.UserId = CurrentUser.Id;
            model.DistrictId = districtId.Value;
            if (districtId.Value != CurrentUser.DistrictId)
            {
                model.StateId = parameters.DistrictService.GetDistrictById(districtId.Value).StateId;
            }
            else
            {
                model.StateId = CurrentUser.StateId.GetValueOrDefault();
            }
            model.IsPublisher = CurrentUser.IsPublisher;
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            model.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            model.IsSchoolAdmin = CurrentUser.IsSchoolAdmin;
            model.IsTeacher = CurrentUser.IsTeacher;

            if (layoutV2 == true)
            {
                return PartialView("v2/_AddNewStudentAssignClassFilter", model);
            }

            return PartialView("_AddNewStudentAssignClassFilter", model);
        }
        private IQueryable<Program> GetAuthorizedProgramsByStudentId(int studentId)
        {
            var student = parameters.StudentService.GetStudentById(studentId);

            var studentPrograms = parameters.StudentProgramService.GetStudentsProgramsByStudentId(studentId).Select(x => x.ProgramID).ToList();
            var notMatchPrograms = parameters.ProgramService.GetProgramsNotMatchWithStudent(student.DistrictId, studentPrograms);
            if (CurrentUser.IsDistrictAdmin || CurrentUser.IsNetworkAdmin)
            {
                notMatchPrograms =
                    notMatchPrograms.Where(
                        x =>
                            x.AccessLevelID != (int)AccessLevelEnum.LinkItOnly &&
                            x.AccessLevelID != (int)AccessLevelEnum.StateUsers);
            }
            if (CurrentUser.IsSchoolAdmin)
            {
                notMatchPrograms =
                    notMatchPrograms.Where(
                        x =>
                            x.AccessLevelID == (int)AccessLevelEnum.DistrictAndSchoolAdmins ||
                            x.AccessLevelID == (int)AccessLevelEnum.AllUsers);
            }
            if (CurrentUser.IsTeacher)
            {
                notMatchPrograms =
                    notMatchPrograms.Where(
                        x => x.AccessLevelID == (int)AccessLevelEnum.AllUsers);

            }
            return notMatchPrograms;
        }

        private IQueryable<School> GetSchoolsByDistrictId(int districtId)
        {
            if (CurrentUser.RoleId == (int)Permissions.SchoolAdmin || CurrentUser.RoleId == (int)Permissions.Teacher)
            {
                var vSchool = parameters.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(o => new School
                {
                    Id = o.SchoolId ?? 0,
                    Name = o.SchoolName
                });
                return vSchool;
            }
            var schools = parameters.SchoolService.GetSchoolsByDistrictId(districtId);
            return schools;
        }
        [NonAction]
        private IEnumerable<ListItem> GetSchoolsBasedOnPermissions()
        {
            if (CurrentUser.IsDistrictAdminOrPublisher)
            {
                return
                    parameters.SchoolService.GetSchoolsByDistrictId(CurrentUser.DistrictId.GetValueOrDefault()).Select(
                        x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);
            }

            return
                parameters.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(
                    x => new ListItem { Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName }).OrderBy(x => x.Name);
        }

        private void AddOrUpdateStudentMetaData(string studentMetaData, int studentId)
        {
            try
            {
                if (!string.IsNullOrEmpty(studentMetaData))
                {
                    var studentMetaDatas = JsonConvert.DeserializeObject<List<StudentMetaDataDto>>(studentMetaData).Where(m => string.IsNullOrEmpty(m.ViewColumn));
                    foreach (var studentMeta in studentMetaDatas)
                    {
                        if (studentMeta.Type.Equals(TextConstants.META_TYPE_DATE) && !string.IsNullOrEmpty(studentMeta.Value))
                        {
                            DateTime result;
                            if (DateTime.TryParse(studentMeta.Value, out result))
                                studentMeta.Value = result.ToLongDateString();
                            else
                                throw new Exception(string.Format("The '{0}' is not a correct date format", studentMeta.Name));

                        }
                        parameters.StudentMetaService.AddOrUpdateStudentMeta(studentId, studentMeta.Name, studentMeta.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetListStudentMetaData(int studentId, int districtId)
        {
            if (studentId > 0)
            {
                var student = parameters.StudentService.GetStudentById(studentId);
                if (student != null && student.DistrictId > 0)
                {
                    districtId = student.DistrictId;
                }
            }
            try
            {
                var studentMetaDataLables = parameters.DistrictDecodeService.GetStudentMetaLabel(districtId);

                if (studentMetaDataLables == null)
                    return Json(new { data = new List<StudentMetaDataDto>() }, JsonRequestBehavior.AllowGet);

                var studentMetaValues = parameters.StudentMetaService.GetStudentsMetaByStudentId(studentId).ToList();

                var calculatedMetaDataColumns = studentMetaDataLables.Where(m => !string.IsNullOrEmpty(m.ViewColumn))
                                                    .Select(m => new StringValueListDto { Name = m.Name, Value = m.ViewColumn });

                var calculatedMetaDatas = parameters.StudentMetaService.GetCalculatedMetaData(calculatedMetaDataColumns, new int[] { studentId });

                // Convert to dictionary
                var caculatedDictionary = calculatedMetaDatas
                 .GroupBy(g => $"{g.StudentId}|{g.MetaData}")
                 .ToDictionary(k => k.Key, k => k.First().Value);
                //>

                foreach (var studentMetaDataLable in studentMetaDataLables)
                {
                    string key = $"{studentId}|{studentMetaDataLable.Name}";
                    if (!string.IsNullOrEmpty(studentMetaDataLable.ViewColumn))
                        studentMetaDataLable.Value = caculatedDictionary.ContainsKey(key) ? caculatedDictionary[key] : string.Empty;
                    else
                        studentMetaDataLable.Value = studentMetaValues.FirstOrDefault(x => x.Name.Equals(studentMetaDataLable.Name, StringComparison.OrdinalIgnoreCase))?.Data;
                }
                return Json(new { data = studentMetaDataLables.AsQueryable() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { message = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetListSchoolMetaData(int schoolId, int districtId)
        {
            if (schoolId > 0)
            {
                var school = parameters.SchoolService.GetSchoolById(schoolId);
                if (school != null && school.DistrictId > 0)
                {
                    districtId = school.DistrictId;
                }
            }
            try
            {
                var schoolMetaDataLabels = parameters.DistrictDecodeService.GetSchoolMetaLabel(districtId);

                if (schoolMetaDataLabels == null)
                    return Json(new { data = new List<StudentMetaDataDto>() }, JsonRequestBehavior.AllowGet);

                var schoolMetaValues = parameters.SchoolMetaService.GetSchoolMetaBySchoolId(schoolId).ToList();

                foreach (var schoolMetaDataLabel in schoolMetaDataLabels)
                {
                    schoolMetaDataLabel.Value = schoolMetaValues.FirstOrDefault(x => x.Name.Equals(schoolMetaDataLabel.Name, StringComparison.OrdinalIgnoreCase))?.Data;
                }
                return Json(new { data = schoolMetaDataLabels.AsQueryable() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        private void AddOrUpdateSchoolMetaData(string schoolMetaData, int schoolId)
        {
            try
            {
                if (!string.IsNullOrEmpty(schoolMetaData))
                {
                    var schoolMetaDatas = JsonConvert.DeserializeObject<List<SchoolMetaDataDto>>(schoolMetaData);
                    foreach (var schoolMeta in schoolMetaDatas)
                    {
                        parameters.SchoolMetaService.AddOrUpdateSchoolMeta(schoolId, schoolMeta.Name, schoolMeta.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}
