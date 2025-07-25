using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using DevExpress.Charts.Native;
using iTextSharp.text;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using ListItem = LinkIt.BubbleSheetPortal.Models.ListItem;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    //[SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    public class PopulateReportingController : BaseController
    {
        private readonly PopulateReportingControllerParameters _parameters;

        public PopulateReportingController(PopulateReportingControllerParameters parameters)
        {
            _parameters = parameters;
        }

        [HttpGet]
        public ActionResult GetGrades(int districtId, string virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            var listIds = ConvertListIdFromString(virtualTestSubTypeId);
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            var tmp = new List<Grade>();
            foreach (var id in listIds)
            {
                var grades = _parameters.PopulateReportingService.ReportingGetGrades(CurrentUser.Id, currentDistrictId,
                    CurrentUser.RoleId, id, resultDateFrom, resultDateTo, isGetAllClass);
                if (grades.Any())
                {
                    tmp = tmp.Union(grades).ToList();
                }
            }
            //var tmp = _parameters.PopulateReportingService.ReportingGetGrades(CurrentUser.Id, currentDistrictId,
            //    CurrentUser.RoleId, virtualTestSubTypeId);
            if (tmp.Any())
            {
                var data = tmp.Select(x => new ListItem() {Id = x.Id, Name = x.Name}).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubjects(int districtId, int gradeId, string virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            var listIds = ConvertListIdFromString(virtualTestSubTypeId);
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            var tmp = new List<Subject>();
            foreach (var id in listIds)
            {
                var subjects = _parameters.PopulateReportingService.ReportingGetSubjects(gradeId, currentDistrictId,
                    CurrentUser.Id, CurrentUser.RoleId, id, resultDateFrom, resultDateTo, isGetAllClass);
                if (subjects.Any())
                {
                    tmp = tmp.Union(subjects).ToList();
                }
            }
            //var tmp = _parameters.PopulateReportingService.ReportingGetSubjects(gradeId, currentDistrictId,
            //    CurrentUser.Id, CurrentUser.RoleId, virtualTestSubTypeId);
            if (tmp != null)
            {
                var item =
                    tmp.GroupBy(s => s.Name)
                        .Select(x => new ListSubjectItem() {Id = string.Join(",", x.Select(s => s.Id)), Name = x.Key})
                        .ToList();
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBanks(int districtId, string subjectIds, string virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            var listIds = ConvertListIdFromString(virtualTestSubTypeId);
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var data = new List<ListItem>();
            var subjectIdList = subjectIds.Split(',');
            foreach (var id in subjectIdList)
            {
                int subjectId;
                var result = int.TryParse(id, out subjectId);
                if (result)
                {
                    var temp = new List<ListItem>();
                    foreach (var subTypeID in listIds)
                    {
                        var banks = _parameters.PopulateReportingService.ReportingGetBanks(subjectId, currentDistrictId,
                            CurrentUser.Id, CurrentUser.RoleId, subTypeID, resultDateFrom, resultDateTo, isGetAllClass).ToList();
                        if (banks.Any())
                        {
                            temp = temp.Union(banks).ToList();
                        }
                    }
                    //var temp =
                    //    _parameters.PopulateReportingService.ReportingGetBanks(subjectId, currentDistrictId,
                    //        CurrentUser.Id, CurrentUser.RoleId, virtualTestSubTypeId).ToList();
                    data.AddRange(temp);
                }
            }
            data = data.OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTests(int districtId, int bankId, string virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            var listIds = ConvertListIdFromString(virtualTestSubTypeId);
            int currentDistrictId = districtId;
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin())
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            var data = new List<ListItem>();
            foreach (var subTypeID in listIds)
            {
                var tests = _parameters.PopulateReportingService.ReportingGetTests(null, null, bankId, currentDistrictId,
                    CurrentUser.Id, CurrentUser.RoleId, subTypeID, resultDateFrom, resultDateTo, isGetAllClass).ToList();
                if (tests.Any())
                {
                    data = data.Union(tests).ToList();
                }
            }
            //var data =
            //    _parameters.PopulateReportingService.ReportingGetTests(bankId, currentDistrictId, CurrentUser.Id,
            //        CurrentUser.RoleId, virtualTestSubTypeId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public ActionResult GetSchools(int? districtId, int? virtualtestId, string virtualTestSubTypeId)
        {
            if (!districtId.HasValue)
            {
                districtId = CurrentUser.DistrictId;
            }
            var listIds = ConvertListIdFromString(virtualTestSubTypeId);

            var temp = new List<TeacherTestDistrictTerm>();
            foreach (var subTypeId in listIds)
            {
                var tempData = _parameters.TeacherDistrictTermService.GetTeacherTestDistrictTerm(districtId,
                    null, null, null, virtualtestId, subTypeId).ToList();
                if (tempData.Any())
                {
                    temp = temp.Union(tempData).ToList();
                }
            }

            var data = temp
                .Select(x => new {x.SchoolId, x.SchoolName})
                .Distinct()
                .OrderBy(x => x.SchoolName)
                .Select(x => new ListItem {Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName})
                .ToList();

            if (CurrentUser.IsTeacher || CurrentUser.IsSchoolAdmin)
            {
                // Return access schools only
                var accessSchools = _parameters.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id);
                data = data.Where(x => accessSchools.Any(s => s.SchoolId == x.Id)).ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTeachers(int? districtId, int? schoolId, int? virtualtestId, string virtualTestSubTypeId)
        {
            if (!districtId.HasValue)
            {
                districtId = CurrentUser.DistrictId;
            }
            var listIds = ConvertListIdFromString(virtualTestSubTypeId);

            var temp = new List<TeacherTestDistrictTerm>();
            
            int? teacherId = null;
            if (CurrentUser.IsTeacher)
                teacherId = CurrentUser.Id;

            foreach (var subTypeId in listIds)
            {
                var tempData = _parameters.TeacherDistrictTermService.GetTeacherTestDistrictTerm(districtId,
                schoolId, teacherId, null, virtualtestId, subTypeId).ToList();
                if (tempData.Any())
                {
                    temp = temp.Union(tempData).ToList();
                }
            }

            var data = temp
                .Select(x => new {x.UserId, x.UserName, x.NameLast, x.NameFirst})
                .Distinct()
                .OrderBy(x => x.NameLast).ThenBy(x => x.NameFirst)
                .Select(x => new
                {
                    Id = x.UserId,
                    Name = x.UserName,
                    FirstName = x.NameFirst,
                    LastName = x.NameLast
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public ActionResult GetTerms(int? districtId, int? schoolId, int? userId, int? virtualTestId,
            string virtualTestSubTypeId)
        {
            if (!districtId.HasValue)
            {
                districtId = CurrentUser.DistrictId;
            }
            var listIds = ConvertListIdFromString(virtualTestSubTypeId);

            var temp = new List<TeacherTestDistrictTerm>();
            foreach (var subTypeId in listIds)
            {
                var tempData = _parameters.TeacherDistrictTermService.GetTeacherTestDistrictTerm(districtId,
                schoolId, userId, null, virtualTestId, subTypeId).ToList();
                if (tempData.Any())
                {
                    temp = temp.Union(tempData).ToList();
                }
            }

            var data = temp
                .Select(x => new {x.DistrictTermId, x.DistrictTermName, x.DateStart})
                .Distinct()
                .OrderByDescending(x => x.DateStart)
                .Select(x => new ListItem {Id = x.DistrictTermId, Name = x.DistrictTermName})
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClasses(int? districtId, int? schoolId, int? userId, int? termId, int? virtualTestId,
            string virtualTestSubTypeId)
        {
            if (!userId.HasValue && CurrentUser.IsTeacher)
                userId = CurrentUser.Id;

            var listIds = ConvertListIdFromString(virtualTestSubTypeId);

            var temp = new List<TeacherTestDistrictTerm>();
            foreach (var subTypeId in listIds)
            {
                var tempData = _parameters.TeacherDistrictTermService.GetTeacherTestDistrictTerm(
                districtId, schoolId, userId, termId, virtualTestId, subTypeId).ToList();
                if (tempData.Any())
                {
                    temp = temp.Union(tempData).ToList();
                }
            }

            var data = temp
                .Select(x => new {x.ClassId, x.ClassName})
                .Distinct()
                .OrderBy(x => x.ClassName)
                .Select(x => new ListItem {Id = x.ClassId, Name = x.ClassName});

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MultipleTestGetTests(int districtId, int? gradeId, int? subjectId, int? bankId
            , string virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            var listIds = ConvertListIdFromString(virtualTestSubTypeId);
            int currentDistrictId = CurrentDistrict(districtId);
            var data = new List<ListItem>();
            
            foreach (var subTypeId in listIds)
            {
                var tests = _parameters.PopulateReportingService.ReportingGetTests(gradeId, subjectId, bankId, currentDistrictId,
                    CurrentUser.Id, CurrentUser.RoleId, subTypeId, resultDateFrom, resultDateTo, isGetAllClass).ToList();
                if (tests.Any())
                {
                    data = data.Union(tests).ToList();
                }
            }
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult MultipleTestGetSchools(int? districtId, string virtualTestIdString,
            string virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            if (!districtId.HasValue)
            {
                districtId = CurrentUser.DistrictId;
            }

            var teacherTestDistrictTerms = _parameters.PopulateReportingService.ReportingGetSchools(CurrentUser.Id,
                CurrentUser.RoleId,
                districtId.Value, virtualTestIdString, virtualTestSubTypeId, resultDateFrom, resultDateTo,
                isGetAllClass ?? false);

            var data = teacherTestDistrictTerms
                .Select(x => new {x.SchoolId, x.SchoolName})
                .Distinct()
                .OrderBy(x => x.SchoolName)
                .Select(x => new ListItem {Id = x.SchoolId.GetValueOrDefault(), Name = x.SchoolName})
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MultipleTestGetTeachers(int? districtId, int schoolId, string virtualtestIdString, string virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            if (!districtId.HasValue)
            {
                districtId = CurrentUser.DistrictId;
            }

            var teacherTestDistrictTerms = _parameters.PopulateReportingService.ReportingGetTeachers(CurrentUser.Id,
                CurrentUser.RoleId,
                districtId.Value, schoolId, virtualtestIdString, virtualTestSubTypeId, resultDateFrom, resultDateTo,
                isGetAllClass ?? false);

            var data = teacherTestDistrictTerms
                .Select(x => new { x.UserId, x.UserName, x.NameLast, x.NameFirst })
                .Distinct()
                .OrderBy(x => x.NameLast).ThenBy(x => x.NameFirst)
                .Select(x => new
                {
                    Id = x.UserId,
                    Name = x.UserName,
                    FirstName = x.NameFirst,
                    LastName = x.NameLast
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MultipleTestGetTerms(int? districtId, int? schoolId, int userId, string virtualTestIdString,
            string virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            if (!districtId.HasValue)
            {
                districtId = CurrentUser.DistrictId;
            }

            if (!schoolId.HasValue)
            {
                schoolId = 0;
            }

            var teacherTestDistrictTerms = _parameters.PopulateReportingService.ReportingGetTerms(CurrentUser.Id,
                CurrentUser.RoleId,
                districtId.Value, schoolId.Value, userId, virtualTestIdString, virtualTestSubTypeId, resultDateFrom, resultDateTo, isGetAllClass ?? false);

            var data = teacherTestDistrictTerms
                .Select(x => new { x.DistrictTermId, x.DistrictTermName, x.DateStart })
                .Distinct()
                .OrderByDescending(x => x.DateStart)
                .Select(x => new ListItem { Id = x.DistrictTermId, Name = x.DistrictTermName })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MultipleTestGetClasses(int? districtId, int? schoolId, int? userId, int termId, string virtualTestIdString,
            string virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            if (!districtId.HasValue)
            {
                districtId = CurrentUser.DistrictId;
            }

            if (!schoolId.HasValue)
            {
                schoolId = 0;
            }

            if (!userId.HasValue && CurrentUser.IsTeacher)
                userId = CurrentUser.Id;

            var teacherTestDistrictTerms = _parameters.PopulateReportingService.ReportingGetClasses(CurrentUser.Id,
                CurrentUser.RoleId,
                districtId.Value, schoolId.Value, userId.Value, termId, virtualTestIdString, virtualTestSubTypeId, resultDateFrom, resultDateTo, isGetAllClass ?? false);

            var data = teacherTestDistrictTerms
                .Select(x => new { x.ClassId, x.ClassName })
                .Distinct()
                .OrderBy(x => x.ClassName)
                .Select(x => new ListItem { Id = x.ClassId, Name = x.ClassName });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MultipleTestGetAllClasses(int? districtId, string virtualTestIdString,
            string virtualTestSubTypeId, DateTime? resultDateFrom, DateTime? resultDateTo, bool? isGetAllClass)
        {
            if (!districtId.HasValue)
            {
                districtId = CurrentUser.DistrictId;
            }

            var teacherTestDistrictTerms = _parameters.PopulateReportingService.ReportingGetAllClasses(CurrentUser.Id,
                CurrentUser.RoleId,
                districtId.Value, virtualTestIdString, virtualTestSubTypeId, resultDateFrom, resultDateTo);

            var data = teacherTestDistrictTerms
                .Select(x => new { x.ClassId, x.ClassName })
                .Distinct()
                .OrderBy(x => x.ClassName)
                .Select(x => new ListItem { Id = x.ClassId, Name = x.ClassName });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<int> ConvertListIdFromString(string virtualTestSubTypeId)
        {
            string[] subTypeIdStrings = virtualTestSubTypeId.Split(new char[] { ';' },
                StringSplitOptions.RemoveEmptyEntries);
            var listIds = new List<int>();
            foreach (var subTypeIdString in subTypeIdStrings)
            {
                int temp;
                if (int.TryParse(subTypeIdString, out temp))
                {
                    listIds.Add(temp);
                }
            }
            return listIds;
        }

        public ActionResult LoadSpecializedReportDownload()
        {
            return PartialView("_SpecializedReportDownload");
        }

        public ActionResult LoadSpecializedReportDownloadV2()
        {
            return PartialView("v2/_SpecializedReportDownload");
        }

        public ActionResult GetSpecializedReportDownload(bool? getAllJob)
        {
            var specializedReportJobs = _parameters.PopulateReportingService.GetSpecializedReportJobs(CurrentUser.Id)
                .Select(x => new SpecializedReportJobViewModel
                {
                    CreatedDate = x.CreatedDate,
                    CreatedDateString = x.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    DownloadUrl = x.DownloadUrl,
                    SpecializedReportJobId = x.SpecializedReportJobId,
                    Status = x.Status,
                    PercentCompleted =
                        x.GeneratedItem.HasValue
                            ? (x.GeneratedItem.Value*100)/
                              ((x.TotalItem.HasValue && x.TotalItem.Value > 0) ? x.TotalItem.Value : 1)
                            : 0,
                    TotalItem = x.TotalItem.GetValueOrDefault()
                });

            if (!getAllJob.HasValue || !getAllJob.Value)
            {
                var specializedReportJobIds = GetSpecializedReportJobSession();
                specializedReportJobs =
                    specializedReportJobs.Where(
                        x => specializedReportJobIds.Contains(x.SpecializedReportJobId));
            }

            var parser = new DataTableParser<SpecializedReportJobViewModel>();
            return Json(parser.Parse(specializedReportJobs.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        
        private List<int> GetSpecializedReportJobSession()
        {
            var specializedReportJobIds = new List<int>();
            if (Session["SpecializedReportJobIds"] != null)
            {
                specializedReportJobIds = (List<int>)Session["SpecializedReportJobIds"];
            }

            return specializedReportJobIds;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportingDownloadReport)]
        public ActionResult DownloadSpecializedReport()
        {
            var model = new DownloadSpecializedReportViewModel
            {
                ResultDateFrom = DateTime.Now.AddMonths(-1),
                ResultDateTo = DateTime.Now,
                IsPublisher = CurrentUser.RoleId.Equals((int) Permissions.Publisher),
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };
            return View(model);
        }

        public ActionResult SearchSpecializedReportDownload(DownloadSpecializedReportViewModel model)
        {
            var specializedReportJobs = new List<SpecializedReportJobViewModel>();
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
                model.DistrictId = CurrentUser.DistrictId;

            if (model.DistrictId.HasValue)
            {
                specializedReportJobs = _parameters.PopulateReportingService.GetSpecializedReportJobs(CurrentUser.Id)
                .Where(x => x.DistrictId == model.DistrictId.Value
                    && x.CreatedDate >= model.ResultDateFrom
                    && x.CreatedDate < model.ResultDateTo.AddDays(1))
                .Select(x => new SpecializedReportJobViewModel
                {
                    CreatedDate = x.CreatedDate,
                    CreatedDateString = x.CreatedDate.ToString("yyyy/MM/dd HH:mm:ss"),
                    DownloadUrl = x.DownloadUrl,
                    SpecializedReportJobId = x.SpecializedReportJobId,
                    Status = x.Status,
                    PercentCompleted =
                        x.GeneratedItem.HasValue
                            ? (x.GeneratedItem.Value * 100) /
                              ((x.TotalItem.HasValue && x.TotalItem.Value > 0) ? x.TotalItem.Value : 1)
                            : 0,
                    TotalItem = x.TotalItem.GetValueOrDefault()
                }).ToList();
            }

            var parser = new DataTableParser<SpecializedReportJobViewModel>();
            return Json(parser.Parse(specializedReportJobs.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

    }
}
