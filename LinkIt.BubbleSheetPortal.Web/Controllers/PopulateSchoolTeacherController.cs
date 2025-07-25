using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Utils.OAuth;
using DevExpress.Xpo.DB.Helpers;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.Users;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using Lokad.Cloud.Storage;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class PopulateSchoolTeacherController : BaseController
    {
        private readonly SchoolService schoolService;
        private readonly SchoolTeacherService schoolTeacherService;
        private readonly UserSchoolService userSchoolService;
        private readonly TeacherDistrictTermService teacherDistrictTermService;
        private readonly ClassService classService;
        private readonly DistrictTermService districtTermService;
        private readonly UserService userService;
        private readonly ClassUserService classUserService;
        private readonly VulnerabilityService vulnerabilityService;

        public PopulateSchoolTeacherController(SchoolService schoolService, SchoolTeacherService schoolTeacherService, UserSchoolService userSchoolService, TeacherDistrictTermService teacherDistrictTermService,
            ClassService classService,
            DistrictTermService districtTermService,
            UserService userService,
            ClassUserService classUserService,
            VulnerabilityService vulnerabilityService)
        {
            this.schoolService = schoolService;
            this.schoolTeacherService = schoolTeacherService;
            this.userSchoolService = userSchoolService;
            this.teacherDistrictTermService = teacherDistrictTermService;
            this.classService = classService;
            this.districtTermService = districtTermService;
            this.userService = userService;
            this.classUserService = classUserService;
            this.vulnerabilityService = vulnerabilityService;
        }

        //[HttpGet]
        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult GetSchools(int? districtId)
        {
            List<ListItem> data;
            if (districtId.HasValue)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, districtId.Value))
                {
                    return Json(new { error = "Has no right" }, JsonRequestBehavior.AllowGet);
                }
                data = schoolService.GetSchoolsByDistrictId(districtId.GetValueOrDefault()).Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();
            }
            else
            {
                data = GetSchools(CurrentUser).ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult GetTeachers(int schoolId, bool hasTermOnly = false, bool sortedByLastNameFirstName = false, bool isIncludeDistrictAdmin = false)
        {
            var validUserSchoolRoleId = "2,3,5,8,27";
            if (!CurrentUser.IsPublisher && !vulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { error = "Has no right to the selected school." }, JsonRequestBehavior.AllowGet);
            }

            var data = userSchoolService.GetTeacherSchoolByTermProc(schoolId, hasTermOnly, CurrentUser.Id, CurrentUser.RoleId, validUserSchoolRoleId, isIncludeDistrictAdmin).ToList();

            var vResult = data.Select(x => new
            {
                Id = x.UserId,
                Name = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).OrderBy(x => x.LastName);
            if (sortedByLastNameFirstName)
            {
                var sort = data.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
                var soretedData = sort.Select(x => new ListItem
                {
                    Id = x.UserId,
                    Name = string.Format("{0}, {1}", x.LastName, x.FirstName)
                }).ToList();
                return Json(soretedData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(vResult, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpGet]
        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult GetTeachersForSGO(int schoolId, bool hasTermOnly = false, bool sortedByLastNameFirstName = false)
        {
            var validUserSchoolRoleId = "2,3,5,8,27";
            if (!CurrentUser.IsPublisher && !vulnerabilityService.CheckUserCanAccessSchool(CurrentUser, schoolId))
            {
                return Json(new { error = "Has no right to the selected school." }, JsonRequestBehavior.AllowGet);
            }
            if (CurrentUser.RoleId == (int)RoleEnum.SchoolAdmin)
            {
                schoolId = 0; //Support case SchoolAdmin have multiple School And loadd all teacher for all school user belong. (LNKT-52862)
            }
            var data = userSchoolService.GetTeacherSchoolByTermProc(schoolId, hasTermOnly, CurrentUser.Id, CurrentUser.RoleId, validUserSchoolRoleId).ToList();

            var vResult = data.Select(x => new
            {
                Id = x.UserId,
                Name = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).OrderBy(x => x.LastName);
            if (sortedByLastNameFirstName)
            {
                var sort = data.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
                var sortedData = sort.Select(x => new ListItem
                {
                    Id = x.UserId,
                    Name = string.Format("{0}, {1}", x.LastName, x.FirstName)
                }).ToList();
                return Json(sortedData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(vResult, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpGet]
        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult GetTerms(int schoolId)
        {
            var districtTermIds =
                classService.GetClassesBySchoolId(schoolId).Select(x => x.DistrictTermId).Distinct().ToList();
            var districtTerms = districtTermService.Select().Where(x => districtTermIds.Contains(x.DistrictTermID));

            var data = districtTerms.OrderBy(x => x.Name).Select(x => new ListItem
            {
                Id = x.DistrictTermID,
                Name = x.Name
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult GetTeachersByTerm(int schoolId, int districtTermId)
        {
            var soretedData = schoolTeacherService.GetAllListTeacherBySchoolIdAndDistrictTermId(schoolId, districtTermId, CurrentUser.Id, CurrentUser.RoleId);
            return Json(soretedData, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<ListItem> GetSchools(User currentUser)
        {
            if (currentUser.IsDistrictAdminOrPublisher)
            {
                return schoolService.GetSchoolsByDistrictId(currentUser.DistrictId.GetValueOrDefault()).Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);
            }

            return userSchoolService.GetSchoolsUserHasAccessTo(currentUser.Id).Select(x => new ListItem { Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName }).OrderBy(x => x.Name);
        }
    }
}
