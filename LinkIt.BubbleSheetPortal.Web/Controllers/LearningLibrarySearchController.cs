using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class LearningLibrarySearchController : BaseController
    {

        private readonly LearningLibraryControllerParameters parameters;
        private readonly S3Library.IS3Service s3Service;
        public LearningLibrarySearchController(LearningLibraryControllerParameters parameters, S3Library.IS3Service s3Service)
        {
            this.parameters = parameters;
            this.s3Service = s3Service;
        }
        #region Search
        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.LearningLibrarySearch)]
        public ActionResult Index()
        {
            LearningLibrarySearchViewModel model = new LearningLibrarySearchViewModel();
            string grade1 = string.Empty;
            string grade2 = string.Empty;
            string subject = string.Empty;
            foreach (string key in Request.QueryString.Keys)//error happen in FireFox,Chrome when return URL does not decode url and contain parameter like &amp;Grade1=5&amp;GUID ...
            {
                if (!string.IsNullOrEmpty(key))
                {
                    switch (key.Replace("amp;", "").ToLower())
                    {
                        case "keywords":
                            model.KeywordsParameter = string.IsNullOrEmpty(Request.QueryString[key]) ? string.Empty : Request.QueryString[key];
                            break;
                        case "grade1":
                            grade1 = string.IsNullOrEmpty(Request.QueryString[key]) ? string.Empty : Request.QueryString[key];
                            break;
                        case "grade2":
                            grade2 = string.IsNullOrEmpty(Request.QueryString[key]) ? string.Empty : Request.QueryString[key];
                            break;
                        case "subject":
                            subject = string.IsNullOrEmpty(Request.QueryString[key]) ? string.Empty : Request.QueryString[key];
                            break;
                        case "searchtype":
                            model.SearchType = string.IsNullOrEmpty(Request.QueryString[key]) ? string.Empty : Request.QueryString[key];
                            break;
                        case "guid":
                            model.GUID = string.IsNullOrEmpty(Request.QueryString[key]) ? string.Empty : Request.QueryString[key];
                            break;

                    }
                }
            }


            //Get grade string from Grade1 to Grade2 by Grade Order
            Grade g1 = parameters.GradeService.GetGradesByName(grade1);
            Grade g2 = parameters.GradeService.GetGradesByName(grade2);
            string s1 = string.Empty;
            string s2 = string.Empty;

            if (g1 != null)
            {
                if (g1.Name.Contains("-"))
                {
                    s1 = string.Format("\"{0}\"", g1.Name);
                }
                else
                {
                    s1 = g1.Name;
                }
            }

            if (g2 != null)
            {
                if (g2.Name.Contains("-"))
                {
                    s2 = string.Format("\"{0}\"", g2.Name);
                }
                else
                {
                    s2 = g2.Name;
                }
            }

            if (!string.IsNullOrEmpty(subject))
            {
                LessonSubject sbj = parameters.LessonSubjectService.GetLessonSubjects().FirstOrDefault(x => x.Name.Equals(subject));
                if (sbj != null)
                {
                    model.SubjectIdParameter = sbj.SubjectId;
                }
            }
            if (model.SubjectIdParameter > 0 || model.GradeParameter.Length > 0 || model.KeywordsParameter.Length > 0 || model.SearchType.Length > 0)
            {
                model.HasParameter = true;
            }
            if (model.HasParameter)
            {
                if (g1 != null || g2 != null)
                {
                    model.GradeParameter = string.Format("{0}-{1}", s1, s2);
                    if (s1.Length > 0 && s1.ToLower().Equals(s2.ToLower()))
                    {
                        model.GradeParameter = s1;
                    }
                }
            }

            if (CurrentUser.DistrictId.HasValue)
            {
                model.CurrentUserDistrictId = CurrentUser.DistrictId.Value;
            }
            else
            {
                model.CurrentUserDistrictId = 0;
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult GetLessonContentTypes()
        {
            var data =
                parameters.LessonContentTypeService.GetLessonContentType().Select(x => new { x.Id, x.Name }).OrderBy(x => x.Name).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetSharedProviders()
        {
            var sharedProviders = new List<District>();
            var data = new List<ListItem>();
            if (CurrentUser.IsDistrictAdmin || CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher)
            {
                sharedProviders = parameters.LessonService.GetSharedProviders(CurrentUser.DistrictId ?? 0).ToList();
            }
            if (CurrentUser.IsPublisher)
            {
                //Get all districts for publisher
                sharedProviders = parameters.LessonService.GetSharedProviders(null).ToList();
            }
            data = sharedProviders.Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();
            if (CurrentUser.IsNetworkAdmin)
            {
                //Get all shared providers for each ditrict
                foreach (var districtId in CurrentUser.GetMemberListDistrictId())
                {
                    sharedProviders.AddRange(parameters.LessonService.GetSharedProviders(districtId).ToList());
                }
                //distinct 
                foreach (var provider in sharedProviders)
                {
                    ListItem item = new ListItem { Id = provider.Id, Name = provider.Name };
                    //if(!data.Contains(item))//do not work
                    if (!data.Exists(x => x.Id == item.Id))
                    {
                        data.Add(item);
                    }
                }
                data = data.OrderBy(x => x.Name).ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetLessons(string grade, int? subjectId, int? contentProviderId, int? typeId,
            string optionalSearch, bool? resourceAdminSearch)
        {
            var parser = new DataTableParser<LessonSearchViewModel>();
            var data = this.parameters.LessonService.GetLessons();
            if (subjectId.HasValue)
            {
                if (subjectId > 0)
                {
                    data = data.Where(x => x.SubjectId == subjectId.Value);
                }
            }

            if (contentProviderId.HasValue)
            {
                //Check Security
                int contentProviderIdValid = 0;
                if (parameters.VulnerabilityService.IsValidContenProvider(CurrentUser, contentProviderId.Value))
                {
                    contentProviderIdValid = contentProviderId.GetValueOrDefault();
                }
                data = data.Where(x => x.LessonProviderId == contentProviderIdValid);
            }
            else
            {
                if (!CurrentUser.IsPublisher)
                {
                    //Only Publisher can see all district
                    List<int> districtIdList = new List<int>();
                    if (CurrentUser.IsNetworkAdmin)
                    {
                        if (resourceAdminSearch.HasValue && resourceAdminSearch.Value == true)
                        {
                            data =
                                data.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.LessonProviderId.Value));
                        }
                        else
                        {
                            foreach (var districtId in CurrentUser.GetMemberListDistrictId())
                            {
                                districtIdList.AddRange(
                                    parameters.LessonService.GetSharedProviders(districtId).Select(x => x.Id).ToList());
                            }
                            districtIdList = districtIdList.Distinct().ToList();
                            data = data.Where(x => districtIdList.Contains(x.LessonProviderId.Value));
                        }
                    }
                    else
                    {
                        List<District> districts =
                            parameters.LessonService.GetSharedProviders(CurrentUser.DistrictId.HasValue
                                ? CurrentUser.DistrictId.Value
                                : 0);
                        districtIdList = districts.Select(x => x.Id).ToList();
                        data = data.Where(x => districtIdList.Contains(x.LessonProviderId.Value));
                    }
                }
            }
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin && !CurrentUser.IsDistrictAdmin)
            {
                if (resourceAdminSearch.HasValue && resourceAdminSearch.Value == true)
                {
                    //only restrict resources for admin search
                    if (CurrentUser.IsSchoolAdmin)
                    {
                        var authorizedUserIdList =
                            parameters.UserSchoolService.GetManageUserByRole(CurrentUser.Id,
                                CurrentUser.DistrictId.GetValueOrDefault(), CurrentUser.RoleId, 0, string.Empty, false)
                                .Select(x => x.UserId)
                                .ToList();
                        data = data.Where(x => authorizedUserIdList.Contains(x.tUserId.GetValueOrDefault()));
                    }
                    else
                    {
                        data = data.Where(x => x.tUserId == CurrentUser.Id);
                    }
                }

            }


            if (typeId.HasValue)
            {
                if (typeId > 0)
                {
                    data = data.Where(x => x.LessonContentTypeId == typeId);
                }
            }

            List<string> grades = ParseGrade(grade);
            //set the keywords as search text in textbox

            if (grades != null)
            {
                if (grades.Count >= 1)
                {
                    var coreSelect = data;
                    data = data.Where(x => x.Grade.Contains(grades[0]));
                    for (int i = 1; i < grades.Count; i++)
                    {
                        var temp = grades[i];
                        data = data.Union(coreSelect.Where(x => x.Grade.Contains(temp.Trim())));
                    }
                }
            }

            var activateInstructionLessonProviderId =
                Convert.ToInt32(ConfigurationManager.AppSettings["ActivateInstruction_LessonProviderId"]);
            Dictionary<string, string> listCachedImage = new Dictionary<string, string>();
            var data1 = data.Select(x => new LessonSearchViewModel
            {
                LessonId = x.LessonId,
                SubjectName = x.SubjectName,
                Grade = x.Grade,
                LessonName = x.LessonName,
                LessonType = x.LessonType,
                Provider = x.Provider,
                ProviderThumbnail = BuildProviderThumbnailUrl(x.ProviderThumbnail, listCachedImage),
                LessonPath = x.LessonPath,
                GuidePath = x.GuidePath,
                Keywords = x.Keywords,
                StandardGUIDString = x.StandardGUIDString,
                StandardDescriptionString = x.StandardDescriptionString,
                StandardSubjectString = x.StandardSubjectString,
                StandardNumberString = x.StandardNumberString,
                GradeOrderString = x.GradeOrderString,
                ActivateInstructionContentType =
                    (x.LessonProviderId != activateInstructionLessonProviderId
                        ? 0
                        : (x.LessonPath.Contains("dl/resource/file") ? 1 : 2))
            });
            return Json(parser.Parse(data1, true), JsonRequestBehavior.AllowGet);
        }

        private string BuildProviderThumbnailUrl(string providerThumnail, Dictionary<string, string> listCachedImage)
        {
            if (!string.IsNullOrEmpty(providerThumnail))
            {
                string keyName = string.Format("{0}/{1}",
                    ConfigurationManager.AppSettings["ProviderLogoFolder"].RemoveEndSlash(),
                    providerThumnail.Trim());
                if (listCachedImage.ContainsKey(keyName))
                {
                    return listCachedImage[keyName];
                }
                else
                {
                    var url = s3Service.GetPublicUrl(LinkitConfigurationManager.GetS3Settings().LessonBucketName, keyName);
                    listCachedImage.Add(keyName, url);
                    return url;
                }
            }
            return string.Empty;
        }

        [HttpGet]
        public ActionResult GetLessonSubjects()
        {
            var data = parameters.LessonSubjectService.GetLessonSubjects().Select(x => new ListItem { Id = x.SubjectId, Name = x.Name }).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetLessonSubjectsByProviderId(int providerId)
        {
            if (providerId == -1)
                return GetLessonSubjects();

            var query = parameters.LessonService.GetLessons()
                .Where(x => x.LessonProviderId == providerId)
                .GroupBy(s => s.SubjectId).Select(x => x).ToList();

            var lessons = query.SelectMany(x => x).ToList();

            List<int> subjectIds = new List<int>();

            foreach (var item in lessons)
            {
                subjectIds.Add(item.SubjectId);
            }

            subjectIds = subjectIds.Distinct().ToList();

            var subjects = parameters.LessonSubjectService.GetLessonSubjects().Where(s => subjectIds.Contains(s.SubjectId)).Select(x => new ListItem { Id = x.SubjectId, Name = x.Name }).OrderBy(x => x.Name).ToList();

            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetContentTypesByProviderId(int providerId)
        {
            if (providerId == -1)
                return GetLessonContentTypes();

            var query = parameters.LessonService.GetLessons()
                .Where(x => x.LessonProviderId == providerId)
                .GroupBy(s => s.LessonContentTypeId).Select(x => x).ToList();

            var lessons = query.SelectMany(x => x).ToList();

            List<int> contentTypeIds = new List<int>();

            foreach (var item in lessons)
            {
                contentTypeIds.Add(item.LessonContentTypeId.Value);
            }

            contentTypeIds = contentTypeIds.Distinct().ToList();

            var contentTypes = parameters.LessonContentTypeService.GetLessonContentType().Where(s => contentTypeIds.Contains(s.Id)).Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name).ToList();

            return Json(contentTypes, JsonRequestBehavior.AllowGet);
        }

        private List<string> ParseGrade(string grade)
        {
            /* To indicate a range of grades, use the format 8-11 to represent grades 8 through 11.  
             * To represent individual grade(s), enter the grades delimited with commas 5,8 
             * To indicate specific grades and a range of grades, enter 5,8,10-12
             * To indicate specific grades that has character '-' in its name, enter "9-12" */

            // An example 3,4,5,6,7,8-12,"9-12"

            List<string> result = new List<string>();//always return list of grade
            string temp = string.Empty;

            if (!string.IsNullOrEmpty(grade))
            {
                //decode grade
                grade = HttpUtility.UrlDecode(grade);
                string[] split = grade.Split(',');
                if (split != null)
                {
                    foreach (var s in split)
                    {
                        temp = s.Trim();
                        //first, if user input more than one " (example "",""",..) then reduce it to only one "
                        while (temp.Contains("\"\""))
                        {
                            temp = temp.Replace("\"\"", "\"");
                        }


                        if (temp.Contains("-"))
                        {
                            int pos = temp.IndexOf('-');
                            if (pos == 0 || pos == temp.Length - 1)
                            {
                                //wrong input from user
                                result.Add(temp);
                            }
                            else
                            {
                                var gradeList = ParseGradeFromTo(temp);
                                if (gradeList != null)
                                {
                                    result.AddRange(gradeList);
                                }
                            }
                        }
                        else
                        {
                            //just a single grade like 5 
                            result.Add(temp.Trim().Replace("\"", ""));
                        }
                    }
                }
            }

            return result;
        }

        private List<string> ParseGradeFromTo(string grade)
        {
            List<string> result = new List<string>();
            string s1 = string.Empty;
            string s2 = string.Empty;
            int pos1 = 0;
            int pos2 = 0;
            int count = grade.Count(x => x == '-');
            string expression = string.Empty;

            if (count == 1)
            {
                //there are two cases
                //case 1: single grade that grade name includes '-', such as "9-12", "8-12" 
                //case 2: grade from g1 to g2 : g1-g2
                // expression = "\"*-*\"";
                expression = "\"(.*)-(.*)\"";
                if (Regex.IsMatch(grade, expression))
                {
                    //case 1
                    //grade looke like "9-12" ,then it will be considered as a single grade "9-12"
                    result.Add(grade.Replace("\"", ""));
                }
                else
                {
                    //case 2: g1-g2 like 5-8 => get all grade from g1 to g2 order by grade order
                    expression = "(.*)-(.*)";
                    if (Regex.IsMatch(grade, expression))
                    {
                        pos1 = grade.IndexOf('-');
                        s1 = grade.Substring(0, pos1).Replace("\"", "");
                        s2 = grade.Substring(pos1 + 1, grade.Length - pos1 - 1).Replace("\"", "");
                    }
                    else
                    {
                        //wrong input
                        result.Add(grade);
                    }
                }
            }
            else if (count == 2)
            {
                //there are 
                //case 1:(right input) "9-12"-14
                //case 2:(right input) 5-"9-12"

                if (grade.Length < 3)
                {
                    //wrong input
                    result.Add(grade);
                }
                else
                {
                    //case 1
                    expression = "\"(.*)-(.*)\"-(.*)";
                    if (Regex.IsMatch(grade, expression))
                    {
                        pos2 = grade.LastIndexOf('-');
                        string temp = grade.Substring(pos2 + 1, grade.Length - pos2 - 1);
                        s1 = grade.Substring(0, pos2).Replace("\"", "");
                        s2 = temp.Replace("\"", "");//ignore the characer '-'
                    }
                    else
                    {
                        //case 2
                        expression = "(.*)-\"(.*)-(.*)\"";
                        if (Regex.IsMatch(grade, expression))
                        {
                            pos1 = grade.IndexOf('-');
                            s1 = grade.Substring(0, pos1).Replace("\"", "");
                            s2 = grade.Substring(pos1 + 1, grade.Length - pos1 - 1).Replace("\"", "");
                        }
                        else
                        {
                            //wrong input
                            result.Add(grade);
                        }
                    }
                }
            }
            else if (count == 3)
            {
                //there is only one case right input: "8-12"-"9-12"
                //expression "*-*"-"*-*"
                expression = "\"(.*)-(.*)\"-\"(.*)-(.*)\"";
                if (Regex.IsMatch(grade, expression))
                {
                    pos1 = grade.IndexOf('-');
                    pos2 = grade.Substring(pos1 + 1, grade.Length - pos1 - 1).IndexOf('-');
                    s1 = grade.Substring(0, pos1 + pos2).Replace("\"", "");
                    s2 = grade.Substring(pos1 + pos2 + 2, grade.Length - pos1 - pos2 - 2).Replace("\"", "");
                }
                else
                {
                    //wrong input
                    result.Add(grade);
                }
            }
            else
            {
                //wrong input from user
                result.Add(grade);
            }

            Grade g1 = null;
            Grade g2 = null;
            if (!string.IsNullOrEmpty(s1))
            {
                g1 = parameters.GradeService.GetGradesByName(s1.Trim());
            }

            if (!string.IsNullOrEmpty(s2))
            {
                g2 = parameters.GradeService.GetGradesByName(s2.Trim());
            }
            if (g1 != null && g2 != null)
            {
                var grades =
                    parameters.GradeService.GetGrades().Where(
                        x => x.Order >= g1.Order && x.Order <= g2.Order).ToList();
                if (grades.Count > 0)
                {
                    foreach (var g in grades)
                    {
                        result.Add(g.Name);
                    }
                }
                else
                {
                    //found no valid grade
                    result.Add(grade);
                }
            }
            if (!string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2))
            {
                if (g1 == null || g2 == null)
                {
                    result.Add(grade);//found no valid grade
                }
            }
            return result;
        }
        #endregion Search
    }
}

