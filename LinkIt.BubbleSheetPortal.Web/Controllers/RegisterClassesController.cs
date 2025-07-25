using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using FluentValidation.Results;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using Microsoft.Ajax.Utilities;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    public class RegisterClassesController : BaseController
    {
        private readonly ManageClassesControllerParameters parameters;

        public RegisterClassesController(ManageClassesControllerParameters parameters)
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

        public ActionResult ModifyClassRoster(int classId)
        {
            var dbClass =
            parameters.ClassService.GetClassById(classId);

            var payByClass = 35;
            var
            districtDecode =
            parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId.Value,
                                                                                         "PayByClass").SingleOrDefault();
            try
            {
                if (districtDecode != null)
                    payByClass = Convert.ToInt32(districtDecode.Value);
            }
            catch (Exception)
            {
            }


            ViewBag.ClassId = classId;
            ViewBag.ClassName = dbClass.Name;
            ViewBag.PayByClass = payByClass;

            return View();
        }

        public ActionResult GetStudentByClassId(int classId)
        {
            var students = parameters.StudentsInClassService.GetAllStudentInClassRegardlessStatus(classId).Select(x => new StudentInClassViewModel
            {
                ID = x.ID,
                StudentID = x.StudentID,
                LastName = x.LastName,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                Code = GetStudentRawCode(x.Code),
                Grade = x.GradeName,
                Gender = x.Gender,
            }).ToList().OrderBy(en => en.LastName).ThenBy(en => en.FirstName).ThenBy(en => en.Code);
            return Json(students, JsonRequestBehavior.AllowGet);
        }

        // Remove teacher email from student code if existed
        private string GetStudentRawCode(string studentCode)
        {
            if (studentCode.Contains("|"))
            {
                return studentCode.Substring(studentCode.IndexOf("|") + 1);
            }

            return studentCode;
        }

       public ActionResult UpdateStudentRoster(int classId, string studentsData)
        {
            var js = new JavaScriptSerializer();
            var students = js.Deserialize<List<StudentInClassViewModel>>(studentsData);

            var validateResult = UpdateStudentValidationRequiredField(classId, studentsData);
            if (validateResult != null)
                return validateResult;

            var teacher = parameters.UserService.GetUserById(CurrentUser.Id);
            var currentstudents = parameters.StudentsInClassService.GetAllStudentInClassRegardlessStatus(classId).ToList();

            var duplicateStudents = parameters.StudentService
                .GetDuplicateStudentRoster(CurrentUser.DistrictId.Value,
                                           classId,
                                           students.Select(en => new Student
                                                                     {
                                                                         Id = en.StudentID,
                                                                         Code = teacher.Id + "|" + en.Code.ToLower()
                                                                     }).
                                               ToList());

            // Remove old students
            var deleteStudentIdList = new List<int>();
            foreach (var student in currentstudents)
            {
                if (students.All(en => en.StudentID != student.StudentID))
                {
                    deleteStudentIdList.Add(student.StudentID);
                }
            }

            foreach (var studentId in deleteStudentIdList)
            {
                RemoveStudent(classId, studentId);
            }


            // Add new students
            foreach (var student in students)
            {
                if (currentstudents.All(en => en.StudentID != student.StudentID))
                {
                    // If this student is already existed ==> Update info and assign to this class
                    if (duplicateStudents.Any(en => en.Code.ToLower() == teacher.Id + "|" + student.Code.ToLower()))
                    {
                        var newStudent =
                            duplicateStudents.FirstOrDefault(en => en.Code.ToLower() == teacher.Id + "|" + student.Code.ToLower());
                        newStudent.Password = Md5Hash.GetMd5Hash(student.Code);
                        newStudent.FirstName = student.FirstName;
                        newStudent.LastName = student.LastName;
                        newStudent.ModifiedDate = DateTime.UtcNow;
                        parameters.StudentService.Save(newStudent);

                        var classStudent = new ClassStudentData
                        {
                            ClassID = classId,
                            StudentID = newStudent.Id,
                            Active = true
                        };
                        parameters.ClassStudentDataService.SaveClassStudent(classStudent);
                    }
                    else // else add new student and assign to this class
                    {
                        var currentClass = parameters.ClassService.GetClassById(classId);
                        var currentSchool = parameters.SchoolService.GetSchoolById(currentClass.SchoolId.Value);

                        var newStudent = new Student
                        {
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            MiddleName = student.MiddleName,
                            Code = teacher.Id + "|" + student.Code.ToLower(),
                            GenderId = 26,
                            Password = Md5Hash.GetMd5Hash(student.Code),
                            AdminSchoolId = currentSchool.Id,
                            Status = 1,
                            DistrictId = currentSchool.DistrictId,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow
                        };

                        parameters.StudentService.Save(newStudent);

                        var classStudent = new ClassStudentData
                        {
                            ClassID = classId,
                            StudentID = newStudent.Id,
                            Active = true
                        };
                        parameters.ClassStudentDataService.SaveClassStudent(classStudent);

                        var objSchoolStudent = new SchoolStudentData { SchoolID = currentSchool.Id, StudentID = newStudent.Id, Active = true };
                        parameters.SchoolStudentDataService.Save(objSchoolStudent);
                    }
                }
            }

            // Update old students
            foreach (var student in students)
            {
                if (currentstudents.Any(en => en.StudentID == student.StudentID
                    && (en.FirstName.ToLower() != student.FirstName.ToLower() || en.LastName.ToLower() != student.LastName.ToLower())))
                {
                    var dbStudent = parameters.StudentService.GetStudentById(student.StudentID);
                    //dbStudent.Code = teacher.Id + "|" + student.Code;
                    dbStudent.FirstName = student.FirstName;
                    dbStudent.LastName = student.LastName;
                    dbStudent.ModifiedDate = DateTime.UtcNow;
                    parameters.StudentService.Save(dbStudent);
                }
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateStudentValidation(int classId, string studentsData)
        {
            var js = new JavaScriptSerializer();
            var students = js.Deserialize<List<StudentInClassViewModel>>(studentsData);

            var validateResult = UpdateStudentValidationRequiredField(classId, studentsData);
            if (validateResult != null)
                return validateResult;

            var teacher = parameters.UserService.GetUserById(CurrentUser.Id);

            // Check Exists Code but difference LastName, FirstName
            var lst = parameters.ClassStudentCustomService.GetStudentsExistCodeDiffName(CurrentUser.DistrictId.Value,
                classId,
                students.Select(en => new Student
                {
                    Id = en.StudentID,
                    Code = teacher.Id + "|" + en.Code.ToLower(),
                    AltCode = en.Code.ToLower(),
                    LastName = en.LastName,
                    FirstName = en.FirstName
                }).ToList());
            if (lst.Count > 0)
            {
                var codeDuplicateDiffname = ";" + string.Join(";", lst) + ";";
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>(), HasDuplicateStudent = 1, DuplicateCodesDiffName = codeDuplicateDiffname });
            }

            return Json(new { Success = true, ErrorList = new List<ValidationFailure>() });
        }

        private ActionResult UpdateStudentValidationRequiredField(int classId, string studentsData)
        {
            var js = new JavaScriptSerializer();
            var students = js.Deserialize<List<StudentInClassViewModel>>(studentsData);

            if (students.Any(en => string.IsNullOrEmpty(en.Code) || string.IsNullOrEmpty(en.FirstName) || string.IsNullOrEmpty(en.LastName)))
            {
                var validationFailures = new List<ValidationFailure>();

                if (students.Any(en => string.IsNullOrEmpty(en.Code)))
                {
                    validationFailures.Add(new ValidationFailure("error", "Student Local Code is required"));
                }

                if (students.Any(en => string.IsNullOrEmpty(en.FirstName)))
                {
                    validationFailures.Add(new ValidationFailure("error", "First Name is required"));
                }

                if (students.Any(en => string.IsNullOrEmpty(en.LastName)))
                {
                    validationFailures.Add(new ValidationFailure("error", "Last Name is required"));
                }

                return Json(new { Success = false, ErrorList = validationFailures });
            }


            var duplicateStudentCodes = new List<string>();
            // Check duplicate students from input data
            if (students.GroupBy(en => en.Code.ToLower()).Any(g => g.Count() > 1))
            {
                duplicateStudentCodes.AddRange(students.GroupBy(en => en.Code.ToLower()).Where(g => g.Count() > 1).Select(en => en.Key).ToList());
            }

            if (duplicateStudentCodes.Any())
            {
                var validationFailures = new List<ValidationFailure>();
                validationFailures.Add(new ValidationFailure("error", "Duplicate student code"));

                return Json(new
                {
                    Success = false,
                    ErrorList = validationFailures,
                    DuplicateStudentCodes = ";" + string.Join(";", duplicateStudentCodes) + ";"
                });
            }

            var currentstudents = parameters.StudentsInClassService.GetAllStudentInClassRegardlessStatus(classId).ToList();

            // Remove old students
            var deleteStudentIdList =
                (from student in currentstudents
                 where students.All(en => en.StudentID != student.StudentID)
                 select student.StudentID).ToList();

            if (parameters.TestResultService.GetTestResultByStudentIdList(classId, deleteStudentIdList).Any())
            {
                var validationFailures = new List<ValidationFailure>();
                validationFailures.Add(
                    new ValidationFailure("error",
                                          "Students cannot be deleted because there are some test results associated with them."));

                return Json(new { Success = false, ErrorList = validationFailures });
            }

            return null;
        }
        private void RemoveStudent(int classId, int studentId)
        {
            var listStudent = new List<int> { studentId };
            parameters.ClassStudentDataService.RemoveStudentsFromClass(listStudent, classId);
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
                return Json(new { Success = false, ErrorList = listError });
            }

            var bubbleSheets = parameters.BubbleSheetService.GetBubbleSheetsByClassIdList(listIdToDelete);
            if (bubbleSheets.Any())
            {
                return ClassException("These classes cannot be deleted because it has bubble sheets associated with them.");
            }

            var testAssignments = parameters.TestAssignmentService.GetTestAssignmentByClassIdList(listIdToDelete);
            if (testAssignments.Any())
            {
                return ClassException("These classes cannot be deleted because it has test assignments associated with them.");
            }

            foreach (var classId in listIdToDelete)
            {
                parameters.ClassStudentService.DeleteClassStudentsOfClass(classId);
                parameters.ClassService.DeleteClass(classId);
            }
            return Json(new { Success = true });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteClass(int classId)
        {
            var classTestResult = parameters.ClassTestResultListService.GetByClassId(classId).ToList();
            if (classTestResult.Any())
            {
                var listError = BuildErrorMessageForRemovingClass(classTestResult);
                return Json(new { Success = false, ErrorList = listError });
            }

            var bubbleSheets = parameters.BubbleSheetService.GetBubbleSheetsByClass(classId);
            if (bubbleSheets.Any())
            {
                return ClassException("This class cannot be deleted because it has bubble sheets associated with it.");
            }

            var testAssignments = parameters.QTITestClassAssignmentService.GetTestAssignmentByClassId(classId);
            if (testAssignments.Any())
            {
                return ClassException("This class cannot be deleted because it has test assignments associated with it.");
            }

            parameters.ClassStudentService.DeleteClassStudentsOfClass(classId);
            parameters.ClassService.DeleteClass(classId);

            return Json(new { Success = true });
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
        //[AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.DadManageRegisterClasses)]
        public ActionResult Index()
        {
            var model = new SchoolAndClassViewModel
                {
                    RoleId = CurrentUser.RoleId,
                    DefaultDistrictId = CurrentUser.DistrictId.GetValueOrDefault(0),
                    CurrentSelectedDistrictId = 0,
                    CurrentSelectedSchoolId = 0,
                    CurrentSelectedTeacherId = (CurrentUser.RoleId == 2) ? CurrentUser.Id : 0,
                    CurrentSelectedClassId = 0,
                    UserId = CurrentUser.Id
                };

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
        public ActionResult GetTeachersInSchool(int schoolId)
        {
            SelectedSchoolID = schoolId;
            var validUserSchoolRoleId = new[] { 2, 3, 5, 8, 27 };

            var data = parameters.UserSchoolService.GetSchoolsUserBySchoolId(schoolId).Where(x => validUserSchoolRoleId.Contains((int)(x.Role))).Select(x => new //Only Publisher, District Admin, Shool Admin, Teacher
                {
                    Name = x.UserName,
                    x.FirstName,
                    x.LastName,
                    x.DisplayName,
                    Id = x.UserId
                }).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
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
            var data = parameters.ClassService.GetClassesBySchoolIdAndTermIdAndUserId(districtTermId, userId, vSchoolId)
                .Select(o => new ListItem { Id = o.Id, Name = o.Name }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddNewStudent(AddEditStudentViewModel model)
        {
            model.SetValidator(parameters.AddStudentViewModelValidator);
            if (!IsValid(model))
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                model.Password = model.ConfirmPassword = model.StudentLocalId;
            }

            HandleUnKnownRace(model);
            
            if (!IsValid(model))
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }
            try
            {
                var currentClass = parameters.ClassService.GetClassById(AddNewStudentToClassId);
                var currentSchool = parameters.SchoolService.GetSchoolById(currentClass.SchoolId.Value);
                SelectedSchoolID = SelectedSchoolID == 0 ? currentSchool.Id : SelectedSchoolID;
                var newStudent = GetStudentFromViewModel(model);
                newStudent.DistrictId = currentSchool.DistrictId;

                parameters.StudentService.Save(newStudent);
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
                    return Json(new { Success = true, studentId = newStudent.Id });
                }
                return Json(new { Success = true });
            }
            catch (Exception)
            {
                var validationFailures = model.ValidationErrors.ToList();
                validationFailures.Add(new ValidationFailure("error", "An error has occurred, please try again."));
                return Json(new { Success = false, ErrorList = validationFailures });
            }
        }


        [HttpPost]
        [AjaxOnly]
        public ActionResult EditStudent(AddEditStudentViewModel model)
        {
            model.SetValidator(parameters.AddEditStudentViewModelValidator);

            if (!IsValid(model))
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }
            HandleUnKnownRace(model);

            if (!CanAccessStudentByAdminSchool(model.StudentId))
            {
                return ShowJsonResultException(model, "You do not have permission to access this student.");
            }

            try
            {
                Student newStudent = GetStudentFromViewModel(model);
                parameters.StudentService.UpdateAndSaveLog(newStudent, CurrentUser.Id);
                return Json(new { Success = true });
            }
            catch (Exception)
            {
                return ShowJsonResultException(model, "An error has occurred, please try again.");
            }
        }

        private bool CanAccessStudentByAdminSchool(int studentId)
        {
            var oldStudent = parameters.StudentService.GetStudentById(studentId);
            if (oldStudent != null
                && oldStudent.AdminSchoolId.HasValue
                && !CurrentUser.IsLinkItAdminOrPublisher()
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

        [HttpPost]
        [AjaxOnly]
        public ActionResult EditSchool(School editedSchool)
        {
            editedSchool.SetValidator(parameters.SchoolValidator);
            if (editedSchool.IsValid)
            {
                parameters.SchoolService.Save(editedSchool);
                return Json(new { Success = true });
            }
            return Json(new { Success = false, ErrorList = editedSchool.ValidationErrors });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddNewSchool(string zipCode, string schoolName)
        {
            var addedSchool = new School
                                  {
                                      DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                                      Name = schoolName,
                                      LocationCode = zipCode,
                                      Status = 1,
                                      Code = (Guid.NewGuid()).ToString().Substring(0, 18).Replace("-", "")
                                  };

            addedSchool.SetValidator(parameters.SchoolValidator);
            if (addedSchool.IsValid)
            {
                parameters.SchoolService.Save(addedSchool);
                addedSchool.Code = addedSchool.Id.ToString();
                parameters.SchoolService.Save(addedSchool);
                return Json(new { Success = true, NewSchoolId = addedSchool.Id });
            }
            return Json(new { Success = false, ErrorList = addedSchool.ValidationErrors });
        }

        [HttpGet]
        public ActionResult GetStudentsInClass(int classId)
        {
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
                return Json(new { message = "Class is invalid.", success = false, type = "error" });
            }
            var movedStudentIds = Request["removedStudentIds"];
            var studentIdList = GetStudentIdsByString(movedStudentIds);
            if (studentIdList.IsNull())
            {
                return Json(new { message = "Removed student(s) is invalid.", success = false, type = "error" });
            }
            parameters.ClassStudentDataService.RemoveStudentsFromClass(studentIdList, classId);
            return Json(new { success = true });
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
                return Json(new { Success = false, ErrorList = listError, Type = "error" });
            }

            parameters.ClassStudentDataService.MoveStudents(studentIdList, oldClassId, newClassId);

            return Json(new { success = true });
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
                if (unknownRace == null) unknownRace = races.FirstOrDefault(o => o.Name == "Unknown");
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
            var classesStudent = parameters.UserStudentService.GetStudentsByStudentAndSchool(studentId, SelectedSchoolID).Select(x => new StudentClassViewModel
            {
                ClassId = x.ClassID,
                ClassName = x.ClassName,
                SchoolName = string.Empty,
                TeacherName = string.Empty,
                TermName = string.Empty
            }).Distinct();

            var parser = new DataTableParser<StudentClassViewModel>();
            return Json(parser.Parse(classesStudent), JsonRequestBehavior.AllowGet);
        }

        private int GetDistrictOfLoginUser()
        {
            var district = System.Web.HttpContext.Current.Session["SelectedDistrictID"];
            var districtId = CurrentUser.DistrictId.GetValueOrDefault();
            if (district.IsNotNull())
            {
                if (!int.TryParse(district.ToString(), out districtId))
                {
                    districtId = CurrentUser.DistrictId.GetValueOrDefault();
                }
            }
            return districtId;
        }

        [HttpGet]
        public ActionResult GetAvailableClassesByStudentId(int studentId)
        {
            var studentInfos = parameters.UserStudentService.GetUserStudentsBySchool(SelectedSchoolID);
            var existClass = studentInfos.Where(s => s.StudentID.Equals(studentId)).Select(x => x.ClassID).Distinct();

            var query = studentInfos.Where(s => !existClass.Contains(s.ClassID));
            if ((int)Permissions.Teacher == CurrentUser.RoleId)
            {
                query = query.Where(o => o.TeacherID == CurrentUser.Id);
            }

            var classesStudent = query.Select(x => new StudentClassViewModel
            {
                ClassId = x.ClassID,
                ClassName = x.ClassName,
                SchoolName = x.SchoolName,
                TermName = x.TermName,
                TeacherName = x.TeacherName
            }).Distinct();

            var parser = new DataTableParser<StudentClassViewModel>();

            return Json(parser.Parse(classesStudent), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveClassStudent(int classId, int studentId)
        {
            var classStudent = parameters.ClassStudentDataService.GetClassStudent(studentId, classId);
            if (classStudent.IsNotNull())
            {
                classStudent.Active = false;
                parameters.ClassStudentDataService.Delete(classStudent);
                return Json(true);
            }
            return Json(true);
        }

        public ActionResult AssignClassForStudent(int classId, int studentId)
        {
            var classStudent = parameters.ClassStudentDataService.GetClassStudent(studentId, classId);
            if (classStudent.IsNotNull())
            {
                classStudent.Active = true;
                parameters.ClassStudentDataService.Save(classStudent);
                return Json(true);
            }
            classStudent = new ClassStudentData { ClassID = classId, StudentID = studentId, Active = true };
            parameters.ClassStudentDataService.Save(classStudent);
            return Json(true);
        }

        [HttpPost, AdminOnly(Order = 3)]
        [AjaxOnly]
        public ActionResult EditTerm(AddEditTermViewModel model)
        {
            model.SetValidator(parameters.AddOrEditTermViewModelValidator);
            if (!IsValid(model))
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }
            try
            {
                var term = GetTermByModel(model);
                if (term.IsNotNull())
                {
                    parameters.DistrictTermService.Save(term);
                    return Json(new { Success = true });
                }
                return ShowJsonResultException(model, "An error has occurred, please try again.");
            }
            catch (Exception)
            {
                return ShowJsonResultException(model, "An error has occurred, please try again.");
            }
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
                    RaceId = model.RaceId,
                    Code = model.StudentLocalId,
                    AltCode = model.StudentStateId,
                    GenderId = model.GenderId,
                    Password = model.Password,
                    CurrentGradeId = model.GradeId,
                    AdminSchoolId = model.AdminSchoolId,
                    Status = 1,
                    DistrictId = model.DistrictId,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                };
        }

        private AddEditStudentViewModel GetAddOrEditStudentViewModel(Student student, int? editStudentSource)
        {
            var model = new AddEditStudentViewModel { CanAccess = CanAccessStudent(student) };
            if (model.CanAccess)
            {
                model.IsSISsystem = true;
                if (!student.SISID.HasValue)
                {
                    model.IsSISsystem = false;
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
                    model.DistrictId = SelectedDistrictID;
                    model.AdminSchoolId = student.AdminSchoolId;
                    model.CurrentUserId = CurrentUser.Id;
                    model.CurrentUserRoleId = CurrentUser.RoleId;
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
                    canAccess = parameters.UserStudentService.HasAccessStudentByLoginUser(CurrentUser.Id, student.Id) && CanAccessStudentByAdminSchool(student.Id);
                    break;
                case Permissions.Teacher:
                    canAccess = parameters.UserStudentService.HasAccessStudentByUserAsTeacher(CurrentUser.Id, student.Id) && CanAccessStudentByAdminSchool(student.Id);
                    break;
            }
            return canAccess;
        }

        public ActionResult ListClassesByTeacher(int teacherId, int districtId)
        {
            var teacher = parameters.UserService.GetUserById(teacherId);
            if (teacher.IsNotNull())
            {
                SelectedTeacherID = teacherId;
                SelectedDistrictID = districtId;
                SchoolClassCurrentButtonSelection = SchoolClassCurrentSelection.Teacher;
                return PartialView("_ListClassesByTeacher", teacher);
            }
            return Json(false);
        }

        [HttpGet]
        public ActionResult GetClassInSchool(int schoolId)
        {
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
        public ActionResult AddClass(int teacherID)
        {
            if (SelectedDistrictID == 0)
            {
                return RedirectToAction("Index");
            }
            
            if (CurrentUser.IsTeacher() && CurrentUser.Id != teacherID)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new AddClassViewModel
                {
                    DistrictTerms = parameters.DistrictTermService.GetDistrictTermByDistrictID(SelectedDistrictID).ToList().Select(x => new SelectListItem
                        {
                            Value = x.DistrictTermID.ToString(),
                            Text = x.Name
                        }),
                    ClassTypes = parameters.ClassTypeService.GetClassTypes().Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.Name
                        }),
                    TeacherId = teacherID,
                    IsUserTeacher = CurrentUser.IsTeacher()
                };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddClass(AddClassViewModel model)
        {
            if (CurrentUser.IsTeacher() && CurrentUser.Id != model.TeacherId)
            {
                return ClassException("User can not access this teacher");
            }
            
            model.DistrictTermId = GetUserDefaultDistrictTerm();
            model.ClassTypeId = 1;

            model.SetValidator(parameters.AddClassRosterViewModelValidator);
            if (!model.IsValid)
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }

            if (model.SchoolId <= 0)
            {
                if (!string.IsNullOrEmpty(model.SchoolName))
                {
                    var isDuplicateSchool = parameters.CEESchoolService.Select().Any(x => x.Name == model.SchoolName && x.LocationCode == model.ZipCode);

                    if (isDuplicateSchool)
                    {
                        return ClassException("SCHOOL NAME ALREADY EIXSTS FOR PROVIDED ZIP CODE " + model.ZipCode);
                    }

                    // Add new School
                    var jsonResult = AddNewSchool(model);
                    if (jsonResult != null)
                        return jsonResult;
                    else
                    {
                        // Add new CEESchool
                        var ceeSchool = new CEESchool
                                        {
                                            Name = model.SchoolName,
                                            LocationCode = model.ZipCode,
                                            SchoolId = model.SchoolId
                                        };
                        parameters.CEESchoolService.Save(ceeSchool);
                    }
                }
                else
                {
                    return
                        Json(
                            new
                            {
                                Success = false,
                                ErrorList =
                                    new List<ValidationFailure>() { new ValidationFailure("", "School is required.") }
                            });
                }
            }
            else
            {
                var ceeSchool = parameters.CEESchoolService.Select().FirstOrDefault(x => x.CEESchoolId == model.SchoolId);

                if (ceeSchool != null)
                {
                    if (ceeSchool.SchoolId.HasValue)
                    {
                        // Do nothing except checking duplicated class
                        model.SchoolId = ceeSchool.SchoolId.Value;

                        var isDuplicateClass = parameters.ClassService.GetClassesBySchoolIdAndTermIdAndUserId(
                            model.DistrictTermId, CurrentUser.Id,
                            model.SchoolId).Any(en => en.Section == model.Section && en.Course == model.Course);

                        if (isDuplicateClass)
                        {
                            return ClassException("Class is already existed.");
                        }
                    }
                    else
                    {
                        model.SchoolName = ceeSchool.Name;
                        model.StateCode = ceeSchool.StateCode;

                        // Add new school and update CeeSchool.SchoolID field
                        var jsonResult = AddNewSchool(model);
                        if (jsonResult != null)
                            return jsonResult;
                        else
                        {
                            ceeSchool.SchoolId = model.SchoolId;
                            parameters.CEESchoolService.Save(ceeSchool);
                        }
                    }
                }
                else
                {
                    return ClassException("School is not existed.");
                }
            }

            // Add new UserSchool record if not existed
            if (!parameters.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id).Any(en => en.SchoolId == model.SchoolId))
            {
                parameters.UserSchoolService
                        .InsertUserSchool(new UserSchool
                        {
                            SchoolId = model.SchoolId,
                            UserId = CurrentUser.Id,
                            DateActive = DateTime.UtcNow,
                            InActive = false
                        });
            }

            var newClass = MapViewModelToClass(model);
            int[] arr = { 2, 3, 8 };
            if (arr.Contains(CurrentUser.RoleId))
            {
                newClass.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            else
            {
                var vTeacher = parameters.UserService.GetUserById(model.TeacherId.GetValueOrDefault());
                if (vTeacher != null)
                {
                    newClass.DistrictId = vTeacher.DistrictId.GetValueOrDefault();
                }
            }
            parameters.ClassService.SaveClass(newClass);
            if (newClass.TeacherId.HasValue)
            {
                InsertClassUser(newClass.TeacherId.Value, newClass);
            }
            return Json(new { Success = true, RedirectUrl = Url.Action("Index") });
        }

        private JsonResult AddNewSchool(AddClassViewModel model)
        {
            // Add new school under CEE " + LabelHelper.DistrictLabel + "
            var district = parameters.DistrictService.GetDistricts().FirstOrDefault(x => x.LICode == Util.CEE_LICode);
            if (district == null)
                return Json(new { Success = false, ErrorList = new List<ValidationFailure>() { new ValidationFailure("", "CEE " + LabelHelper.DistrictLabel + " is not existed.") } });

            // If this school is already existed in School table ==> Just update it into CEESchool.SchoolID field; Else: Add new school
            var school =
                parameters.SchoolService.GetAll()
                    .FirstOrDefault(x => x.Name == model.SchoolName && x.LocationCode == model.ZipCode);

            if (school != null)
                model.SchoolId = school.Id;
            else
            {
                var addedSchool = new School
                {
                    DistrictId = district.Id,
                    Name = model.SchoolName,
                    LocationCode = model.ZipCode,
                    StateCode = model.StateCode,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    StateId = district.StateId,
                    Status = 1,
                    Code = (Guid.NewGuid()).ToString().Substring(0, 18).Replace("-", ""),
                    ModifiedBy = "Portal"
                };

                addedSchool.SetValidator(parameters.CEESchoolValidator);
                if (addedSchool.IsValid)
                {
                    parameters.SchoolService.Save(addedSchool);
                    addedSchool.Code = addedSchool.Id.ToString();
                    parameters.SchoolService.Save(addedSchool);
                    model.SchoolId = addedSchool.Id;
                }
                else
                {
                    return Json(new { Success = false, ErrorList = addedSchool.ValidationErrors });
                }
            }

            return null;
        }

        private int GetUserDefaultDistrictTerm()
        {
            var currentUser = parameters.UserService.GetUserById(CurrentUser.Id);
            if (currentUser.CreatedDate == null)
                currentUser.CreatedDate = DateTime.UtcNow;

            var now = DateTime.UtcNow;

            var termStartDate = new DateTime(now.Year, now.Month, 1);
            // Get Last day of month next year
            var termEndDate =
                new DateTime(now.Year, now.Month, 1)
                .AddYears(1).AddMonths(1).AddSeconds(-1);
            var districtTermName = termStartDate.ToString("MMM yyyy") + " - " + termEndDate.ToString("MMM yyyy");

            var districtTerm = parameters.DistrictTermService.GetDistrictTermByNameAndDate(
                currentUser.DistrictId.Value, termStartDate, termEndDate, districtTermName);

            if (districtTerm == null)
            {
                districtTerm = new DistrictTerm
                                   {
                                       CreatedByUserID = CurrentUser.Id,
                                       DateCreated = DateTime.UtcNow,
                                       DateEnd = termEndDate,
                                       DateStart = termStartDate,
                                       DistrictID = CurrentUser.DistrictId.Value,
                                       Name = districtTermName
                                   };
                parameters.DistrictTermService.Save(districtTerm);
            }

            return districtTerm.DistrictTermID;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PopulateDataByZipCode(string zipCode)
        {
            var data = parameters.CEESchoolService.GetSchoolsByLocationCode(zipCode).Select(x => new ListItem { Id = x.CEESchoolId, Name = x.Name }).OrderBy(x => x.Name).ToList();

            var stateCode = "";

            if (zipCode.All(Char.IsDigit) && zipCode.Length >= 3)
            {
                var stateZipCode = Convert.ToInt32(zipCode.Substring(0, 3));
                var state =
                    parameters.StateService.GetStateByZipCode(stateZipCode);
                if (state != null)
                {
                    stateCode = state.Code;
                }
            }

            return Json(new { schools = data, stateCode = stateCode }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetSchoolsByZipCode(string zipCode)
        {
            //var data = parameters.SchoolService.GetSchoolsByDistrictIdAndLocationCode(CurrentUser.DistrictId ?? 0, zipCode).Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();
            var data = parameters.CEESchoolService.GetSchoolsByLocationCode(zipCode).Select(x => new ListItem { Id = x.CEESchoolId, Name = x.Name }).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetStates()
        {
            var data = parameters.StateService.GetStates().Select(x => new ListItem { Id = x.Id, Name = x.Code }).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetStudentsInDistrict(int classId, bool showInactive = false)
        {
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


        [HttpPost]
        [AjaxOnly]
        public ActionResult AddStudentToClass(int studentId, int classId)
        {
            var classStudent = new ClassStudentData
            {
                ClassID = classId,
                StudentID = studentId,
                Active = true
            };

            parameters.ClassStudentDataService.SaveClassStudent(classStudent);
            return Json(true);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ActivateTeacher(int? userId)
        {
            User user = parameters.UserService.GetUserById(userId.GetValueOrDefault());
            if (user.IsNotNull())
            {
                user.Active = true;
                parameters.UserService.SaveUser(user);
            }
            return Json(new { Success = true });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeactivateTeacher(int userId)
        {
            var listError = new List<ValidationFailure>();
            var user = parameters.UserService.GetUserById(userId);
            if (user.IsNull())
            {
                listError.Add(new ValidationFailure("", "Cannot deactivate because the teacher's info did not exist."));
                return Json(new { Success = false, ErrorList = listError });
            }

            var checkTeacherHasClass = parameters.ClassListService.GetClassListByPrimaryTeacherID(userId).Any();
            if (checkTeacherHasClass)
            {
                listError.Add(new ValidationFailure("", "Cannot deactivate this teacher because he has active classes."));
                return Json(new { Success = false, ErrorList = listError });
            }

            user.Active = false;
            parameters.UserService.SaveUser(user);

            return Json(new { Success = true });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddNewStudentToClass(int classId)
        {
            AddNewStudentToClassId = classId;
            return Json(new { Success = true });
        }

        [HttpGet, AdminOnly(Order = 3)]
        public ActionResult GetAllDistricts()
        {
            var districts = parameters.DistrictService.GetDistricts();
            var data = districts.Select(x => new ListItem { Name = x.Name, Id = x.Id }).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSchoolsByUser(int userId)
        {
            var userSchools = parameters.UserSchoolService.GetSchoolsUserHasAccessTo(userId);
            var data = userSchools.Select(x => new ListItem { Name = x.SchoolName, Id = x.SchoolId.Value }).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchoolsByDistrict(int districtId)
        {
            var schools = parameters.SchoolService.GetSchoolsByDistrictId(districtId).Select(x => new SchoolListViewModel
            {
                SchoolID = x.Id,
                SchoolName = x.Name,
                Code = x.Code,
                StateCode = x.StateCode
            });
            var parser = new DataTableParser<SchoolListViewModel>();
            return Json(parser.Parse(schools), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchoolsByDistrictRaw(int districtId)
        {
            if (CurrentUser.RoleId == (int)Permissions.SchoolAdmin || CurrentUser.RoleId == (int)Permissions.Teacher)
            {
                var vSchool = parameters.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(o => new School
                {
                    Id = o.SchoolId ?? 0,
                    Name = o.SchoolName
                });
                return Json(vSchool, JsonRequestBehavior.AllowGet);
            }
            var schools = parameters.SchoolService.GetSchoolsByDistrictId(districtId);
            return Json(schools, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTermsByDistrict(int districtId)
        {
            var terms = parameters.DistrictTermService.GetAllTermsByDistrictID(districtId).Select(x => new TermViewModel
            {
                TermID = x.DistrictTermID,
                Name = x.Name,
                DateStart = x.DateStart,
                DateEnd = x.DateEnd
            });
            var parser = new DataTableParser<TermViewModel>();
            return Json(parser.Parse(terms), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransferTermsByDistrict(int districtId)
        {
            var terms = parameters.DistrictTermService.GetDistrictTermByDistrictID(districtId).Select(x => new ListItem
                {
                    Id = x.DistrictTermID,
                    Name = x.Name
                });
            return Json(terms, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetUserNotMatchSchool(int schoolId, int districtId)
        {
            var validUserSchoolRoleId = new[] { 2, 3, 5, 8, 27 };


            IEnumerable<int> lstUser = parameters.UserSchoolService.ListUserIdBySchoolId(schoolId);
            var userNotMatch = parameters.UserService.GetUserNotAssociatedWithSchool(districtId, lstUser).Where(x => validUserSchoolRoleId.Contains(x.RoleId));//Only Publisher, " + LabelHelper.DistrictLabel + " Admin, Shool Admin, Teacher
            var users = userNotMatch.Select(o => new UserNotMatchSchoolViewModel
            {
                UserId = o.Id,
                UserName = o.UserName,
                RoleId = o.RoleId,
                SchoolId = schoolId
            });
            var parser = new DataTableParser<UserNotMatchSchoolViewModel>();
            return Json(parser.Parse(users), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, AjaxOnly]
        public ActionResult RemoveUserSchool(int userSchoolId)
        {
            var userSchool = parameters.UserSchoolService.GetUserSchoolById(userSchoolId);
            if (userSchool.IsNull())
            {
                return Json(false);
            }
            if (CurrentUser.Id == userSchool.UserId)
            {
                return Json(new { message = "Error. You cannot remove your account from this school.", success = false, type = "error" });
            }
            if (parameters.DistrictTermClassService.SchoolHaveActiveClass(userSchool.UserId, userSchool.SchoolId.GetValueOrDefault(), userSchool.DistrictId.GetValueOrDefault()))
            {
                return Json(new { message = "Error. You cannot remove this user from this school. Teacher’s class is still active.", success = false, type = "error" });
            }
            if (parameters.DistrictTermClassService.SchoolHaveClassNotStartYet(userSchool.UserId, userSchool.SchoolId.GetValueOrDefault(), userSchool.DistrictId.GetValueOrDefault()))
            {
                return Json(new { message = "Error. You cannot remove this user from this school. Teacher’s class has not started yet.", success = false, type = "error" });
            }
            parameters.UserSchoolService.RemoveUserSchool(userSchool);
            return Json(true);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetUserBySchoolId(int schoolId)
        {
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

        [HttpGet]
        public ActionResult GetClassesBySchoolId(int schoolId)
        {
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
            var teacherInfo = parameters.SchoolTeacherListService.GetSchoolTeacherListBySchoolId(oldSchoolId).FirstOrDefault(s => s.UserID.Equals(teacherId));
            if (teacherInfo.IsNull())
            {
                var listError = new List<ValidationFailure> { new ValidationFailure(string.Empty, "Invalid parameter") };
                return Json(new { Success = false, ErrorList = listError });
            }
            if (string.IsNullOrEmpty(teacherInfo.ClassID) == false)
            {
                var listError = new List<ValidationFailure> { new ValidationFailure(string.Empty, "This teacher have some classes.") };
                return Json(new { Success = false, ErrorList = listError });
            }
            var userSchool = parameters.UserSchoolService.GetUserSchoolByUserIdSchoolId(teacherId, oldSchoolId);
            parameters.UserSchoolService.RemoveUserSchool(userSchool);
            userSchool = new UserSchool
                             {
                                 SchoolId = newSchoolId,
                                 UserId = teacherId,
                                 DateActive = DateTime.UtcNow,
                                 InActive = false
                             };
            parameters.UserSchoolService.InsertUserSchool(userSchool);
            return Json(new { Success = true });
        }

        [HttpGet]
        public ActionResult EditClass(int id)
        {
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
                        Selected = aClass.ClassType.Equals(x.Id),
                        Text = x.Name,
                        Value = x.Id.ToString()
                    });

                return View(aClass);
            }
            return RedirectToAction("Index", "RegisterClasses");
        }


        [HttpPost]
        [AjaxOnly]
        public ActionResult EditClass(Class editedClass)
        {
            var classInDB = parameters.ClassService.GetClassById(editedClass.Id);

            if (classInDB.Course.ToLower() != editedClass.Course.ToLower() ||
                classInDB.Section.ToLower() != editedClass.Section.ToLower())
            {
                var isDuplicateClass = parameters.ClassService.GetClassesBySchoolIdAndTermIdAndUserId(
                    classInDB.DistrictTermId.Value, CurrentUser.Id,
                    classInDB.SchoolId.Value).Any(
                        en => en.Section == editedClass.Section && en.Course == editedClass.Course);

                if (isDuplicateClass)
                {
                    return ClassException("Class is already existed.");
                }
            }

            classInDB.Name = editedClass.Course;
            if (!string.IsNullOrWhiteSpace(editedClass.Section)) classInDB.Name += " " + editedClass.Section;
            classInDB.Course = editedClass.Course;
            classInDB.Section = editedClass.Section;

            classInDB.SetValidator(parameters.ClassValidator);
            if (!classInDB.IsValid)
            {
                return Json(new { Success = false, ErrorList = (List<ValidationFailure>)classInDB.ValidationErrors });
            }

            if (parameters.ClassUserService.ClassDoesNotHavePrimaryTeacher(editedClass.Id))
            {
                var errorList = classInDB.ValidationErrors.ToList();
                errorList.Add(new ValidationFailure(string.Empty, "Class must have a primary teacher."));
                return Json(new { Success = false, ErrorList = errorList });
            }
            classInDB.ModifiedDate = DateTime.UtcNow;
            classInDB.ModifiedUser = CurrentUser.Id;
            classInDB.ModifiedBy = "Portal";
            parameters.ClassService.SaveClass(classInDB);
            return Json(new { Success = true });
        }



        private ActionResult ClassException(string exception)
        {
            var errorList = new List<ValidationFailure>
                {
                    new ValidationFailure("", exception)
                };
            return Json(new { Success = false, ErrorList = errorList });
        }





        [HttpGet]
        public ActionResult GetTeachers(int classId, int currentTeacherID)
        {
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

        public ActionResult GetClassesByUserId(int userID)
        {
            var classesQuery = parameters.ClassListService.GetClassListByPrimaryTeacherID(userID);

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
            });

            var parser = new DataTableParser<ClassListViewModel>();
            return Json(parser.Parse(classes.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAvailableProgramsByStudentId(int studentId)
        {
            var student = parameters.StudentService.GetStudentById(studentId);
            if (student.IsNull())
            {
                return Json(new { message = "Student is invalid.", success = false, type = "error" });
            }
            var studentPrograms = parameters.StudentProgramService.GetStudentsProgramsByStudentId(studentId).Select(x => x.ProgramID).ToList();
            var notMatchPrograms = parameters.ProgramService.GetProgramsNotMatchWithStudent(student.DistrictId, studentPrograms).Select(x => new ProgramViewModel { Id = x.Id, Name = x.Name });
            var parser = new DataTableParser<ProgramViewModel>();
            return Json(parser.Parse(notMatchPrograms), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddProgramToStudent(int programId, int studentId)
        {
            var student = parameters.StudentService.GetStudentById(studentId);
            var program = parameters.ProgramService.GetProgramById(programId);
            if (student.IsNull() || program.IsNull())
            {
                return Json(false);
            }
            parameters.StudentProgramService.AddStudentProgram(studentId, programId);
            return Json(true);
        }

        public ActionResult GetProgramsByStudentId(int studentId)
        {
            var studentPrograms = parameters.StudentProgramService.GetStudentsProgramsByStudentId(studentId).Select(x => new StudentProgramViewModel
                {
                    StudentProgramId = x.StudentProgramID,
                    ProgramName = x.ProgramName
                });
            var parser = new DataTableParser<StudentProgramViewModel>();
            return Json(parser.Parse(studentPrograms), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveProgramFromStudent(int studentProgramId)
        {
            var studentProgram = parameters.StudentProgramService.GetStudentProgramById(studentProgramId);
            if (studentProgram.IsNull())
            {
                return Json(false);
            }
            parameters.StudentProgramService.DeleteStudentProgram(studentProgram);
            return Json(true);
        }

        [HttpGet]
        public ActionResult GetTeachersForClass(int classId)
        {
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
            return parameters.ClassUserService.GetTeachersForSchoolByClassId(classId).ToList().Select(x => new SelectListItem
                {
                    Text = string.IsNullOrEmpty(x.TeacherFirstName) ? string.Format("{0} ({1})", x.TeacherLastName, x.UserName) : string.Format("{0}, {1} ({2})", x.TeacherLastName, x.TeacherFirstName, x.UserName),
                    Value = x.UserId.ToString(CultureInfo.InvariantCulture)
                }).OrderBy(x => x.Text);
        }

        [HttpPost, AjaxOnly]
        public ActionResult AddTeacherToClass(AddTeacherToClassViewModel model)
        {
            var classUser = new ClassUser
                {
                    ClassId = model.ClassId,
                    UserId = model.SelectedTeacher,
                    ClassUserLOEId = model.SelectedLOE
                };

            classUser.SetValidator(parameters.ClassUserValidator);
            if (!classUser.IsValid)
            {
                return Json(new { Success = false, ErrorList = classUser.ValidationErrors });
            }

            parameters.ClassUserService.InsertClassUser(classUser);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, AjaxOnly]
        public ActionResult RemoveTeacherFromClass(int classUserId, int classId, int userId)
        {
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
            return Json(true);
        }

        [HttpPost, AjaxOnly]
        public ActionResult TransferStudents(int oldClassId, int newClassId, string movedStudentIds, bool transferTests)
        {
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
                    errorMessages.Add("Error tansferring student: " + student.Id);
                }
            }

            return Json(new { success = !hasErrors, errors = errorMessages });
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

            parameters.ClassUserService.ReplacePrimaryTeacherClassUser(classUser);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
