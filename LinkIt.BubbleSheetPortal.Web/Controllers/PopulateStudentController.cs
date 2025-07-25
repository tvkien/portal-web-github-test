using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class PopulateStudentController : BaseController
    {
        private readonly ClassService classService;
        private readonly ClassStudentService classStudentService;
        private readonly ClassUserService classUserService;
        private readonly UserService userService;
        private readonly TeacherDistrictTermService teacherDistrictTermService;
        private readonly ClassStudentCustomService classStudentCustomService;
        private readonly VulnerabilityService vulnerabilityService;
        private readonly UserSchoolService _userSchoolService;

        public PopulateStudentController(ClassService classService,
            ClassStudentService classStudentService,
            ClassUserService classUserService,
            UserService userService,
            TeacherDistrictTermService teacherDistrictTermService,
            ClassStudentCustomService classStudentCustomService,
            VulnerabilityService vulnerabilityService,
            UserSchoolService userSchoolService
            )
        {
            this.classService = classService;
            this.classStudentService = classStudentService;
            this.classUserService = classUserService;
            this.userService = userService;
            this.teacherDistrictTermService = teacherDistrictTermService;
            this.classStudentCustomService = classStudentCustomService;
            this.vulnerabilityService = vulnerabilityService;
            this._userSchoolService = userSchoolService;
        }

        [HttpGet]
        public ActionResult GetTerms(int? userId, int schoolId)
        {
            userId = userId ?? CurrentUser.Id;
            var data = GetDistrictTermsByUserId(userId, schoolId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTermsByUserIds(string userIds, int schoolId)
        {
            var data = new List<ListItem>();

            if (!string.IsNullOrEmpty(userIds))
            {
                var listUserId = userIds.Split(';').Select(x => Convert.ToInt32(x)).ToList();

                data = teacherDistrictTermService.GetTermsByUserIdsAndSchoolId(listUserId, schoolId)
                    .Select(x => new ListItem { Id = x.DistrictTermId, Name = x.DistrictName }).Distinct().OrderBy(x => x.Name).ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClasses(int? termId, int? userId, int schoolId = 0)
        {
            userId = userId ?? CurrentUser.Id;
            var data = GetClassesByDistrictTermIdAndUserId(termId.GetValueOrDefault(), userId.GetValueOrDefault(), schoolId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClassesBySchoolTermUser(int termId, int? userId, int schoolId)
        {
            userId = userId ?? CurrentUser.Id;
            var data = GetClassesBySchoolIdTermIdAndUserIdOnClassUser(termId, userId.GetValueOrDefault(), schoolId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassesBySchoolAndTermAndUser(int schoolId, int termId, int? userId)
        {
            userId = userId ?? CurrentUser.Id;
            var data = GetClassesBySchoolIdAndTermIdAndUserId(termId, userId.GetValueOrDefault(), schoolId);
            data = data.OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassesBySchoolAndTermsAndUsers(int schoolId, string termIds, string userIds)
        {
            var data = new List<ListItem>();

            if (!string.IsNullOrEmpty(userIds) && !string.IsNullOrEmpty(termIds))
            {
                var listUserId = userIds.Split(';').Select(x => Convert.ToInt32(x)).ToList();
                var listTermId = termIds.Split(';').Select(x => Convert.ToInt32(x)).ToList();

                data = classService.GetClassesBySchoolIdAndTermIdsAndUserIds(listTermId, listUserId, schoolId)
                   .Select(o => new ListItem()
                   {
                       Id = o.Id,
                       Name = o.Name
                   }).Distinct().OrderBy(x => x.Name).ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStudents(int classId)
        {
            var data = classStudentService.GetClassStudentsByClassId(classId).ToList().Select(x => new { x.StudentId, x.FullName }).OrderBy(x => x.FullName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStudentsActive(int classId)
        {
            if (!vulnerabilityService.CheckUserCanAccessClass(CurrentUser, classId))
            {
                return Json(new { error = "Has no right to access class" }, JsonRequestBehavior.AllowGet);
            }
            var data = classStudentCustomService.GetStudentActiveByClassId(classId).ToList()
                .Select(x => new { x.StudentId, x.FullName })
                .Distinct()
                .OrderBy(x => x.FullName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<ListItem> GetDistrictTermsByUserId(int? userId, int? schoolId)
        {
            var teacher = userService.GetUserById(userId.GetValueOrDefault());
            if (teacher.IsNull())
            {
                return new List<ListItem>();
            }
            return teacherDistrictTermService.GetTermsByUserIdAndSchoolId(userId.GetValueOrDefault(), schoolId.GetValueOrDefault())
                .Select(x => new ListItem { Id = x.DistrictTermId, Name = x.DistrictName }).OrderBy(x => x.Name);
        }

        public IEnumerable<ListItem> GetClassesByDistrictTermIdAndUserId(int termId, int userId, int schoolId = 0)
        {
            var teacher = userService.GetUserById(userId);
            if (teacher.IsNull())
            {
                return new List<ListItem>();
            }
            var schoolIdsAccess = _userSchoolService.GetSchoolsUserHasAccessTo(userId)
            .Where(x => x.SchoolId.HasValue == true && (schoolId == 0 || x.SchoolId == schoolId))
            .Select(x => x.SchoolId.Value).ToArray();
            var classUsers = classUserService.GetClassUsersByUserId(teacher.Id).ToList();
            var classItems = new List<ListItem>();
            foreach (var classUser in classUsers)
            {
                var singleClass = classService.GetClassesByDistrictTermIdAndClassId(termId, classUser.ClassId);
                if (singleClass.IsNull() || (singleClass.SchoolId.HasValue && !schoolIdsAccess.Contains(singleClass.SchoolId.Value)))
                {
                    continue;
                }
                classItems.Add(new ListItem { Id = singleClass.Id, Name = singleClass.Name });
            }
            
            var vResult = classItems
                .OrderBy(o => o.Name);
            return vResult;
        }

        public IEnumerable<ListItem> GetClassesBySchoolIdAndTermIdAndUserId(int termId, int userId, int schoolId)
        {
            var teacher = userService.GetUserById(userId);
            if (teacher.IsNull())
            {
                return new List<ListItem>();
            }
            var classItems = new List<ListItem>();
            return classService.GetClassesBySchoolIdAndTermIdAndUserId(termId, userId, schoolId)
                   .Select(o => new ListItem()
                   {
                       Id = o.Id,
                       Name = o.Name
                   });
        }

        /// <summary>
        /// Get All Class By SchoolID, DistrictTermID and UserID in ClassUser
        /// </summary>
        /// <param name="termId"></param>
        /// <param name="userId"></param>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public IEnumerable<ListItem> GetClassesBySchoolIdTermIdAndUserIdOnClassUser(int termId, int userId, int schoolId)
        {
            var teacher = userService.GetUserById(userId);
            if (teacher.IsNull())
            {
                return new List<ListItem>();
            }
            var classItems = new List<ListItem>();
            return classUserService.GetClassesBySchoolIdTermIdAndUserIdOnClassUser(termId, userId, schoolId)
                   .Select(o => new ListItem()
                   {
                       Id = o.ClassId,
                       Name = o.Name
                   });
        }

        [HttpGet]
        public ActionResult GetTermsBySchool(int schoolId)
        {
            var terms = teacherDistrictTermService.GetTermBySchool(schoolId, CurrentUser.Id, CurrentUser.RoleId);
            if (terms == null)
                return Json(new ListItem(), JsonRequestBehavior.AllowGet);
            var data = terms.Select(x => new ListItem { Id = x.DistrictTermId, Name = x.DistrictName }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassDistrictTermBySchool(int schoolId)
        {
            var classes = classService.GetClassDistrictTermBySchool(schoolId, CurrentUser.Id, CurrentUser.RoleId);
            if (classes == null)
                return Json(new ListItem(), JsonRequestBehavior.AllowGet);

            var result = new LargeJsonResult
            {
                Data = classes,
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
        }
    }
}
