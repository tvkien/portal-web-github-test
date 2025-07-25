using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Models.ParentConnect;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using System.Threading;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    public class ComposeController : BaseController
    {
        private readonly ComposeControllerParameters parameters;

        public ComposeController(ComposeControllerParameters parameters)
        {
            this.parameters = parameters;
        }
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.MessageInbox)]
        public ActionResult Index()
        {
            var model = new ComposeMessageViewModel();
            model.RoleId = CurrentUser.RoleId;
            model.CurrentUserId = CurrentUser.Id;
            Role role = parameters.RoleService.GetRoleById(CurrentUser.RoleId);

            if (role != null)
            {
                model.RoleName = role.Display;
            }
            var user = parameters.UserService.GetUserById(CurrentUser.Id);


            if (model.IsPublisher || model.IsNetworkAdmin )
            {
                model.CurrentSelectedDistrictId = 0;//allow to select all district
            }
            else
            {
                model.CurrentSelectedDistrictId = CurrentUser.DistrictId.GetValueOrDefault();//allow to see current district
            }
            if (user != null)
            {
                model.From = string.Format("{0} {1}, ({2})", user.FirstName, user.LastName, model.RoleName);
            }
            else
            {
                model.From = string.Format("{0}, ({1})", CurrentUser.UserName, model.RoleName);
            }


            return View(model);
        }
       
        #region Group management
        public ActionResult LoadListStudentGroup()
        {
            return PartialView("_ListStudentGroup");
        }
        public ActionResult AddEditStudentGroup(int? reportGroupId, string reportGroupName)
        {
            var studentGroup = new AddEditStudentGroupViewModel();
            studentGroup.RoleId = CurrentUser.RoleId;
            studentGroup.CurrentUserId = CurrentUser.Id;
            studentGroup.CurrentUserName = CurrentUser.UserName;

            if (reportGroupId.HasValue) // Edit an existing group
            {
                studentGroup.GroupId = reportGroupId.GetValueOrDefault();
                studentGroup.Name = reportGroupName;
            }
            else //Add a new group
            {
                //studentGroup.CurrentSelectedDistrictId = CurrentUser.DistrictId.GetValueOrDefault();
                
                studentGroup.GroupId = 0;
            }
            studentGroup.CreatedUserId = CurrentUser.Id;
            
            return PartialView("_AddEditStudentGroup", studentGroup);


        }
        public ActionResult CheckUniqueGroupName(int groupId, string strGroupName)
        {
            if (parameters.ParentConnectService.CheckUniqueGroupName(groupId, CurrentUser.Id, strGroupName))
            {
                return Json(new { Message = "", Success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "Group name must be unique.", Success = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AjaxOnly]
        public ActionResult AddEditStudentGroupSave(AddEditStudentGroupViewModel model)
        {
            //TODO: check model valid            
            if (!string.IsNullOrEmpty(model.Name))
            {
                int groupId = SaveStudentGroup(model);
                SaveStudentInGroup(groupId, model.StudentIdList);
                model.GroupId = groupId;//remember the groupId has been created
                return Json(new { Success = true, Id = groupId, GroupName = model.Name });
            }
            return Json(false);
        }

        private int SaveStudentGroup(AddEditStudentGroupViewModel model)
        {
            ReportGroup obj = new ReportGroup();
            obj.ReportGroupId = model.GroupId;
            obj.Name = model.Name;
            obj.UserId = CurrentUser.Id;
            obj = parameters.ParentConnectService.SaveStudentGroup(obj);

            return obj == null ? 0 : obj.ReportGroupId;
        }
        private bool SaveStudentInGroup(int groupId, List<string> studentIdList)
        {
            if (groupId > 0)
            {
                parameters.ParentConnectService.SaveStudentGroupByGroupId(groupId, studentIdList);
                return true;
            }
            return false;
        }
        [HttpGet]
        public ActionResult GetStudentInGroupByGroupId(int studentGroupId)
        {
            var vstudents = parameters.ParentConnectService.GetStudentInGroupByGroupId(studentGroupId)
                .Select(x => new StudentGroupViewModel { Id = x.StudentId, Name = x.FullName, Detail = "" }
                );
            return Json(vstudents, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStudentGroup()
        {
            //Get all the student group that user has created
            var groups = parameters.ParentConnectService.GetReportGroup().Where(x => x.UserId == CurrentUser.Id)
                .Select(x => new ReportGroupViewModel()
                {
                    ReportGroupId = x.ReportGroupId,
                    Name = x.Name
                });
            var parser = new DataTableParser<ReportGroupViewModel>();
            return Json(parser.Parse(groups), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteReportGroup(int reportgroupId)
        {
            try
            {
                parameters.ParentConnectService.DeleteReportGroup(reportgroupId);
                return Json(new { Success = true, Message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "Failed to delete group. Selected group does not exist." }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region By Parent
        public ActionResult GetSchoolsByUser(int userId)
        {
            var userSchools = parameters.UserSchoolService.GetSchoolsUserHasAccessTo(userId);
            var data = userSchools.Select(x => new ListItem { Name = x.SchoolName, Id = x.SchoolId.Value }).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetSchools(int? districtId)
        {
            List<ListItem> data;
            if (districtId.HasValue)
            {
                var schools = parameters.SchoolService.GetSchoolsByDistrictId(districtId.Value);
                data = schools.Select(x => new ListItem { Name = x.Name, Id = x.Id }).OrderBy(x => x.Name).ToList();
            }
            else
            {
                data = GetSchoolsBasedOnPermissions().ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTeachersInSchool(int schoolId)
        {
            if (!CurrentUser.IsTeacher)
            {
                var data = parameters.UserSchoolService.GetSchoolsUserBySchoolId(schoolId).Select(x => new
                {
                    Name =
                x.UserName,
                    x.
                FirstName,
                    x.
                LastName,
                    x.
                DisplayName,
                    Id =
                x.UserId
                }).OrderBy(
                                                                                                               x =>
                                                                                                               x.
                                                                                                                   LastName)
                    .ThenBy(x => x.FirstName).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var data = parameters.UserSchoolService.GetSchoolsUserByUserlId(CurrentUser.Id).Select(x => new
                {
                    Name =
                x.UserName,
                    x.
                FirstName,
                    x.
                LastName,
                    x.
                DisplayName,
                    Id =
                x.UserId
                }).OrderBy(
                                                                                                                       x =>
                                                                                                                       x.
                                                                                                                           LastName)
                            .ThenBy(x => x.FirstName).ToList().Take(1);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
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
        [HttpGet]
        public ActionResult GetClasses(int userId, int? schoolId)
        {
            var data = new List<ListItem>();
            var classUsers = parameters.ClassUserService.GetClassUsersByUserId(userId).ToList();
            if (!schoolId.HasValue)
            {
                data.AddRange(classUsers.Select(classUser => parameters.ClassService.GetClassById(classUser.ClassId)).Select(singleClass => new ListItem { Id = singleClass.Id, Name = singleClass.Name }));
            }
            else
            {
                DistrictTerm dt;
                string classCustomeName = string.Empty;
                foreach (var classUser in classUsers)
                {
                    var singleClass = parameters.ClassService.GetClassById(classUser.ClassId);
                    if (singleClass.IsNotNull() && singleClass.SchoolId.Equals(schoolId.Value))
                    {
                        if (singleClass.DistrictTermId.HasValue)
                        {
                            //get district term name of this class
                            dt = parameters.DistrictTermService.GetDistrictTermById(singleClass.DistrictTermId.Value);
                            classCustomeName = string.Format("{0}, {1}", dt.Name, singleClass.Name);

                        }
                        else
                        {
                            classCustomeName = string.Format(".{0}", singleClass.Name);
                        }
                        data.Add(new ListItem { Id = singleClass.Id, Name = classCustomeName });
                    }
                }
            }
            data = data.OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetClassesByUserId(int userID, int? schoolID)
        {
            List<ListItem> data;

            var classesQuery = parameters.ClassListService.GetClassListByPrimaryTeacherID(userID);
            if (schoolID.HasValue)
            {
                classesQuery = classesQuery.Where(o => o.SchoolID == schoolID.Value);
            }

            data = classesQuery.ToList().Select(c => new ListItem()
            {
                Id = c.ClassId,
                Name = c.ClassName

            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadStudentInClass(int classId)
        {
            var model = parameters.ClassService.GetClassById(classId);
            return PartialView("_ListStudentInClass", model);
            //return PartialView("_Test", model);
        }
        [HttpGet]
        public ActionResult GetStudentsInClass(int classId)
        {

            var students = parameters.ParentConnectService.GetAllStudentInClass(classId).Select(x => new StudentInClassWithParentsViewModel
            {
                ID = x.ID,
                FirstName = x.FirstName,
                //MiddleName = x.MiddleName,
                LastName = x.LastName,
                Gender = x.Gender,
                LocalId = x.Code,
                ParentList = ParseParents(x.Parents),
                StudentID = x.StudentID,


            });

            //var result = students.ToList();

            var parser = new DataTableParser<StudentInClassWithParentsViewModel>();
            return Json(parser.Parse(students), JsonRequestBehavior.AllowGet);


        }
        [HttpGet]
        public ActionResult GetStudents(int classId)
        {

            var students = parameters.ParentConnectService.GetAllStudentInClass(classId).Select(x => new StudentInClassWithParentsViewModel
            {
                StudentID = x.StudentID,
            });
            List<int> studentIdList = students.ToList().Select(x => x.StudentID).ToList();
            var data = parameters.ClassStudentService.GetClassStudentsByClassId(classId).Where(x => studentIdList.Contains(x.StudentId)).ToList().Select(x => new { x.StudentId, x.FullName }).OrderBy(x => x.FullName);

            return Json(data, JsonRequestBehavior.AllowGet);


        }
        private int GetDistrictIdFromSubdomain()
        {
            var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            return parameters.DistrictService.GetLiCodeBySubDomain(subDomain);
        }
        #endregion



        #region Parse Parent from Xml to Object
        public List<StudentParentViewModel> ParseParents(string xmlParent)
        {
            if (string.IsNullOrWhiteSpace(xmlParent)) return new List<StudentParentViewModel>();
            var xdoc = XDocument.Parse(xmlParent);
            var result = new List<StudentParentViewModel>();

            foreach (var node in xdoc.Element("Parents").Elements("Parent"))
            {
                var p = new StudentParentViewModel();
                p.ParentUserId = GetIntValue(node.Element("ParentUserID"));
                p.Email = GetStringValue(node.Element("Email"));
                p.Phone = GetStringValue(node.Element("Phone"));
                p.FirstName = GetStringValue(node.Element("NameFirst"));
                p.LastName = GetStringValue(node.Element("NameLast"));
                result.Add(p);
            }

            return result;
        }
        private bool GetBoolValue(XElement element)
        {
            if (element == null) return false;
            return element.Value == "1" || element.Value == "true";
        }

        private string GetStringValue(XElement element)
        {
            if (element == null) return string.Empty;
            return element.Value;
        }

        private int GetIntValue(XElement element)
        {
            if (element == null) return 0;
            return Convert.ToInt32(element.Value);
        }

        private int? GetIntNullableValue(XElement element)
        {
            try
            {
                return Convert.ToInt32(element.Value);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}
