using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using FluentValidation.Results;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using RestSharp.Extensions;
using S3Library;
using System.Net.Http;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
	[AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class LearningLibraryAdminController : BaseController
    {

        private readonly LearningLibraryAdminControllerParameters parameters;
        public LearningLibraryAdminController(LearningLibraryAdminControllerParameters parameters, IS3Service s3Service)
        {
            this.parameters = parameters;
            _s3Service = s3Service;
        }
        private readonly IS3Service _s3Service;

        bool ValidLessonProviderRequest(int id)
        {
            if (id != 0)
            {
                var providerList = GetLessonProviderList();
                if (providerList.Count(x => x.Id == id) <= 0)
                    return false;
            }

            return true;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.LearningLibraryResourceAdmin)]
        public ActionResult Index(string gradeSearch, int? subjectIdSearch, int? contentProviderIdSearch, int? resourceTypeIdSearch, string filterSearch)
        {
            LearningLibrarySearchViewModel model = new LearningLibrarySearchViewModel();

            foreach (string key in Request.QueryString.Keys)//error happen in FireFox,Chrome when return URL does not decode url and contain parameter like &amp;Grade1=5&amp;GUID ...
            {
                string value = string.Empty;

                if (!string.IsNullOrEmpty(key))
                {
                    value = string.IsNullOrEmpty(Request.QueryString[key]) ? string.Empty : Request.QueryString[key];
                    switch (key.Replace("amp;", "").ToLower())
                    {
                        case "gradesearch":
                            model.GradeParameter = HttpUtility.UrlDecode(value);
                            break;
                        case "contentprovideridsearch":
                            model.ContentProviderIdParameter = value.Length == 0 ? 0 : int.Parse(value);
                            // check security content provider param
                            if (!ValidLessonProviderRequest(model.ContentProviderIdParameter))
                                return RedirectToAction("Index");
                            break;
                        case "subjectidsearch":
                            model.SubjectIdParameter = value.Length == 0 ? 0 : int.Parse(value);
                            break;
                        case "resourcetypeidsearch":
                            model.ResourceTypeIdParameter = value.Length == 0 ? 0 : int.Parse(value);
                            break;
                        case "filtersearch":
                            model.KeywordsParameter = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(value));
                            break;

                    }
                }
            }
            
            if (model.GradeParameter.Length > 0 || model.SubjectIdParameter > 0 || model.ContentProviderIdParameter > 0 || model.ResourceTypeIdParameter > 0 || model.KeywordsParameter.Length > 0)
            {
                model.HasParameter = true;
            }
            if (CurrentUser.DistrictId.HasValue)
            {
                model.CurrentUserDistrictId = CurrentUser.DistrictId.Value;
            }
            else
            {
                model.CurrentUserDistrictId = 0;
            }
            model.RoleId = CurrentUser.RoleId;
            return View(model);
        }


        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.LearningLibraryResourceAdmin)]
        [HttpGet]
        public ActionResult Resource(int id, string gradeSearch, int? subjectIdSearch, int? contentProviderIdSearch, int? resourceTypeIdSearch,string filterSearch )
        {
            //avoid modify url parameters
            if (id > 0)
            {
                if (!parameters.VulnerabilityService.HasRightToEditLesson(CurrentUser, id, CurrentUser.GetMemberListDistrictId()))
                {
                    return RedirectToAction("Index", "LearningLibrarySearch");
                }
            }
            var model = new ResourceViewModel();
            model.LessonId = id;
            foreach (string key in Request.QueryString.Keys)//error happen in FireFox,Chrome when return URL does not decode url and contain parameter like &amp;Grade1=5&amp;GUID ...
            {
                string value = string.Empty;

                if (!string.IsNullOrEmpty(key))
                {
                    value = string.IsNullOrEmpty(Request.QueryString[key]) ? string.Empty : Request.QueryString[key];
                    switch (key.Replace("amp;", "").ToLower())
                    {
                        case "gradesearch":
                            //model.GradeSearch = HttpUtility.UrlDecode(value.Replace("'", "%27"));
                            model.GradeSearch = HttpUtility.UrlDecode(value);
                            break;
                        case "contentprovideridsearch":
                            model.ContentProviderIdSearch = value.Length == 0 ? 0 : int.Parse(value);

                            // check security content provider param
                            if (!ValidLessonProviderRequest(model.ContentProviderIdSearch))
                            {
                                ViewBag.IsInValidLessonProvider = true;
                                model.ErrorMessage = "Provider is not valid.";
                                return View(model);
                            }
                            break;
                        case "subjectidsearch":
                            model.SubjectIdSearch = value.Length == 0 ? 0 : int.Parse(value);
                            break;
                        case "resourcetypeidsearch":
                            model.ResourceTypeIdSearch = value.Length == 0 ? 0 : int.Parse(value);
                            break;
                        case "filtersearch":
                            //model.FilterSearch = HttpUtility.UrlDecode(value.Replace("'", "%27"));
                            model.FilterSearch = HttpUtility.UrlDecode(value);
                            break;

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
            model.RoleId = CurrentUser.RoleId;
            string maxFileSize = string.Empty;
            try
            {
                maxFileSize = System.Configuration.ConfigurationManager.AppSettings["ResourceUploadMaxFileSizeMB"];
            }
            catch
            {
                maxFileSize = "30"; //use default value is 30 MB
            }
            string timeout = string.Empty;
            try
            {
                timeout = System.Configuration.ConfigurationManager.AppSettings["ResourceUploadTimeoutMinutes"];
            }
            catch
            {
                timeout = "10"; //use default value is 10 minutes
            }
            model.FileMaxSizeByte = int.Parse(maxFileSize) * 1024 * 1024;
            model.TimeoutSecond = int.Parse(timeout) * 1000 * 60;
            model.FileMaxSizeMBString = maxFileSize + "MB";

            if (model.LessonId>0)
            {   
                //Edit
                Lesson lesson = parameters.LessonService.GetLessons().Where(x => x.LessonId == model.LessonId).FirstOrDefault();
                //Check if lesson is existing or not
                if (lesson == null)
                {
                    model.ErrorMessage = "Lesson is not existing.";
                    return View(model);
                }

                //check if current user has permission to edit this lesson
                model.LessonProviderId = lesson.LessonProviderId;
                model.SelectedLessonProviderId = lesson.LessonProviderId.HasValue?lesson.LessonProviderId.Value:0;

                model.LessonName = lesson.LessonName;
                model.SubjectId = lesson.SubjectId;
                model.Description = lesson.Description;
                model.Description = lesson.Description.Replace("\n", "&#10");

                model.Keywords = lesson.Keywords;
                model.Keywords = lesson.Keywords.Replace("\n", "&#10");//newline character

                model.LessonContentTypeId = lesson.LessonContentTypeId;
                model.LessonFileTypeId = lesson.LessonFileTypeId;


                if (lesson.LessonFileTypeId == Util.LessonFileTypeUrlId)//link
                {
                    model.LessonPath = lesson.LessonPath;
                    model.LessonSelection = "link";
                }
                else
                {
                    model.FileNameLesson = lesson.LessonPath;//internal file
                    model.LessonSelection = "file";
                }

                if (string.IsNullOrEmpty(lesson.GuidePath))
                {
                    lesson.GuidePath = string.Empty;
                }
                if (lesson.GuidePath.Length == 0 || lesson.GuidePath.ToLower().StartsWith("http"))
                {
                    //use default selection is link
                    model.GuideSelection = "link";
                    model.GuidePath = lesson.GuidePath;
                }
                else
                {
                    model.GuideSelection = "file";
                    model.FileNameGuide = lesson.GuidePath;
                }
                   
                
                //Get assigned grade(s) of lesson
                List<int> gradeIdList = parameters.LessonService.GetAssignedGradeIdList(model.LessonId);
                if(gradeIdList.IsNotNull())
                {
                    model.AssignedGradeIdString = string.Empty;
                    //build model.AssignedGradeIdString
                    foreach (var gradeId in gradeIdList)
                    {
                        model.AssignedGradeIdString += string.Format(",-{0}-", gradeId);
                    }
                    
                }
                //Get assigned standard(s) of lesson
                List<int> standardIdList = parameters.LessonService.GetAssignedMasterStandardIdList(model.LessonId);
                if (standardIdList.IsNotNull())
                {
                    model.AssignedStandardIdString = string.Empty;
                    //build model.AssignedGradeIdString
                    foreach (var standardId in standardIdList)
                    {
                        model.AssignedStandardIdString += string.Format(",-{0}-", standardId);
                    }

                }

            }
            else
            {
                model.LessonProviderId = CurrentUser.DistrictId;
                model.SelectedLessonProviderId = CurrentUser.DistrictId.HasValue?CurrentUser.DistrictId.Value:0;
            }
            var resourceMimeType = parameters.ConfigurationService.GetConfigurationByKey(Util.Config_Resource_MimeType);
            var fileTypeList = Util.ParseFileTypesFromXmlConfig(resourceMimeType==null?string.Empty:resourceMimeType.Value);
            if (fileTypeList.Count > 0)
            {
                var mimeTypeString = string.Join(",", fileTypeList.Select(x =>string.Format("\"{0}\"", x.MimeType)).ToList());
                var extensionString = string.Join(",", fileTypeList.Select(x => x.Extension).ToList());
                model.ResourceMimeTypeString = mimeTypeString;
                model.ResourceExtensionString = extensionString;
            }
            
            //get these allow extensions
            return View(model);
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.LearningLibraryResourceAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Resource(ResourceViewModel model)
        {
            //avoid modify ajax parameters
            if (model.LessonId > 0)
            {
                if (!parameters.VulnerabilityService.HasRightToEditLesson(CurrentUser, model.LessonId, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = false, Error = "Has no right to edit this lesson." });
                }
            }
            //check right on content provider
            if (!Util.HasRightOnDistrict(CurrentUser, model.SelectedLessonProviderId)
                || !ValidLessonProviderRequest(model.SelectedLessonProviderId))
            {
                return Json(new { Success = false, Error = "Has no right on this Lesson Provider." });
            }

            model.SetValidator(parameters.ResourceViewModelValidator);
            if (!model.IsValid)
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }

            try
            {
                if (model.LessonId == 0)
                {
                    SaveNewResource(model);
                }
                else
                {
                    SaveEditResource(model);
                    //delete all existing assigned grades, standards
                    parameters.LessonService.DeleteLessonGrade(model.LessonId);
                    parameters.LessonService.DeleteLessonStateStandard(model.LessonId);
                }

                //grade assigned to this lesson 
                SaveAssignedGrades(model.LessonId, model.AssignedGradeIdString);
                
                //standard assigned to this lesson 
                SaveAssignedMasterStandard(model.LessonId, model.AssignedStandardIdString);

                //return Json(new { Success = true, CreatedLessonId = model.LessonId, RedirectUrl = string.Format("{0}/{1}","EditResource",model.LessonId)});
                return Json(new { Success = true, CreatedLessonId = model.LessonId });
            }
            catch (Exception e)
            {
                return ShowJsonResultException(model, e.Message);
            }
        }

        private void SaveNewResource(ResourceViewModel model)
        {
            Lesson l = new Lesson();
            l.LessonId = model.LessonId;
            //l.LessonProviderId = model.LessonProviderId;
            l.LessonProviderId = model.SelectedLessonProviderId;
            l.LessonContentTypeId = model.LessonContentTypeId;
            l.LessonName = HttpUtility.UrlDecode(model.LessonName);
            l.Description = HttpUtility.UrlDecode(model.Description);
            l.SubjectId = model.SubjectId;
            l.Keywords = HttpUtility.UrlDecode(model.Keywords);

            //Lesson file/path
            if (model.LessonSelection.Equals("link"))
            {
                l.LessonFileTypeId = Util.LessonFileTypeUrlId;//url
                l.LessonPath = model.LessonPath;
            }
            else
            {
                l.LessonFileTypeId = 26;//Unknown by default, upload function will update these fileds later
                l.LessonPath = string.Empty;//Upload function will update lesson path
            }

            //Guide file/path
            if (model.GuideSelection.Equals("link"))
            {
                l.GuidePath = string.IsNullOrEmpty(model.GuidePath) ? string.Empty : model.GuidePath;
            }
            else
            {
                //l.GuidePath = string.IsNullOrEmpty(model.FileNameLesson)?string.Empty:model.FileNameLesson;
                l.GuidePath = string.Empty;//Upload function will update guide path

            }

            l.tUserId = CurrentUser.Id;

            parameters.LessonService.Save(l);
            model.LessonId = l.LessonId;
        }

        private void SaveEditResource(ResourceViewModel model)
        {
            //get the current lesson from database
            Lesson lesson =  parameters.LessonService.GetLessons().Where(x => x.LessonId == model.LessonId).FirstOrDefault();
            if(lesson == null)
            {
                return;
            }
                
            Lesson updatedLesson = new Lesson();
            updatedLesson.LessonId = model.LessonId;
            updatedLesson.LessonProviderId = model.SelectedLessonProviderId;
            updatedLesson.LessonContentTypeId = model.LessonContentTypeId;
            updatedLesson.LessonName = HttpUtility.UrlDecode(model.LessonName);
            updatedLesson.Description = HttpUtility.UrlDecode(model.Description);
            updatedLesson.SubjectId = model.SubjectId;
            updatedLesson.Keywords = HttpUtility.UrlDecode(model.Keywords);

            //Resource file/path
            
            if (model.LessonSelection.Equals("link"))
            {
                updatedLesson.LessonFileTypeId = Util.LessonFileTypeUrlId;//url
                updatedLesson.LessonPath = model.LessonPath;
                //if the current lesson use a file, delete that file
                if (lesson.LessonFileTypeId != Util.LessonFileTypeUrlId)
                {
                    if(lesson.LessonPath.Length > 0)
                    {
                        string bucket = LinkitConfigurationManager.GetS3Settings().LessonBucketName;
                        string foler =  LinkitConfigurationManager.GetS3Settings().LessonFolder;
                        _s3Service.DeleteFile(bucket, foler, lesson.LessonPath);
                    }
                }
            }
            else
            {
                
                updatedLesson.LessonFileTypeId = lesson.LessonFileTypeId;//use current value as default, if user select another file, it'll be updated in UploadFileLesson
                updatedLesson.LessonPath = lesson.LessonPath;
            }
            

            //Reference file/path
            if (model.GuideSelection.Equals("link"))
            {
                updatedLesson.GuidePath = string.IsNullOrEmpty(model.GuidePath) ? string.Empty : model.GuidePath;
                //if the current lesson use a file, delete that file

                if(!string.IsNullOrEmpty(lesson.GuidePath))
                {

                    if (!lesson.GuidePath.ToLower().StartsWith("http"))
                    {
                        string bucket = LinkitConfigurationManager.GetS3Settings().GuideBucketName;
                        string foler = LinkitConfigurationManager.GetS3Settings().GuideFolder;
                        _s3Service.DeleteFile(bucket, foler, lesson.GuidePath);
                    }
                }
            }
            else
            {
                updatedLesson.GuidePath = lesson.GuidePath;//UploadFileGuide function will update guide path if user selecte another file

            }

            updatedLesson.tUserId = CurrentUser.Id;

            parameters.LessonService.Save(updatedLesson);
            model.LessonId = updatedLesson.LessonId;
        }
        
        private void SaveAssignedGrades(int lessonId, string gradeIdString)
        {
            List<int> assignedGradeIdList = ParseIdFromString(gradeIdString);
            foreach (var gradeId in assignedGradeIdList)
            {
                parameters.LessonService.AssignGradeToLesson(lessonId, gradeId);
            }
        }
        private void SaveAssignedMasterStandard(int lessonId, string standardIdString)
        {
            List<int> assignedStandardIdList = ParseIdFromString(standardIdString);
            foreach (var standardId in assignedStandardIdList)
            {
                parameters.LessonService.AssignMasterStandardToLesson(lessonId, standardId);
            }
        }
        public ActionResult ValidateLink(string link)
        {
            try
            {
                if (string.IsNullOrEmpty(link))
                {
                    return Json(new { Success = "Fail" }, JsonRequestBehavior.AllowGet);
                }
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = client.GetAsync(link).Result;
                        return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new { Success = "Fail" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch
            {
                return Json(new { Success = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetProviders()
        {
            object data = GetLessonProviderList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        List<ListItem> GetLessonProviderList()
        {
            List<ListItem> data = null;
            if (CurrentUser.IsPublisher)
                data = parameters.LessonProviderService.GetLessonProviders().Select(x => new ListItem { Name = x.Name, Id = x.Id }).OrderBy(x => x.Name).ToList(); 
            else if (CurrentUser.IsNetworkAdmin)
                data = parameters.LessonProviderService.GetLessonProviders().Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id)).Select(x => new ListItem { Name = x.Name, Id = x.Id }).OrderBy(x => x.Name).ToList(); 
            else
                data = parameters.LessonProviderService.GetLessonProviders().Where(x => x.Id == CurrentUser.DistrictId).Select(x => new ListItem { Name = x.Name, Id = x.Id }).OrderBy(x => x.Name).ToList(); 
            return data;
        }

        [HttpGet]
        public ActionResult GetStates()
        {
            List<ListItem> result = new List<ListItem>();
            //get the CC first
            var data = parameters.StateService.GetStates().Where(x => x.Code == "CC").Select(x => new ListItem { Name = x.Name, Id = x.Id });
            result.AddRange(data.ToList());
            if (CurrentUser.IsPublisher)
            {
                data = parameters.StateService.GetStates().Where(x => x.Code != "CC").OrderBy(x => x.Name).Select(x => new ListItem { Name = x.Name, Id = x.Id });
                result.AddRange(data.ToList());
                
            }
            else if(CurrentUser.IsNetworkAdmin)
            {
                var stateIds = parameters.DistrictService.GetStateIdByDictricIds(CurrentUser.GetMemberListDistrictId());//Get list id stat
                data = parameters.StateService.GetStates().Where(x =>stateIds.Contains(x.Id)).Select(x => new ListItem { Name = x.Name, Id = x.Id }).OrderBy(x => x.Name);
                result.AddRange(data.ToList());
            }
            else
            {
                data = parameters.StateService.GetStates().Where(x => x.Id == CurrentUser.StateId).Select(x => new ListItem { Name = x.Name, Id = x.Id }).OrderBy(x => x.Name);
                result.AddRange(data.ToList());
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetSubjectMasterStandardState(int stateId)
        {
            var data =
                parameters.MasterStandardService.GetMasterStandards().Where(x => x.StateId == stateId && x.Archived==false).Select(x=> new { Subject = x.Subject}).Distinct().ToList();
            List<ListItem> tmpData = new List<ListItem>(data.Count+1);
            tmpData.Add(new ListItem { Id = 0, Name = "Select Subject" });
            for (int i = 0; i < data.Count;i++ )
            {
                tmpData.Add(new ListItem {Id=i+1,Name=data[i].Subject});
            }
            return Json(tmpData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGrades()
        {
            var data = this.parameters.GradeService.GetGrades().OrderBy(x => x.Order).Select(x => new ListItem { Name = x.Name, Id = x.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStateSubjectGradeByStateAndSubject(int stateId, string subject)
        {
            var state = this.parameters.StateService.GetStateById(stateId);
            string stateCd = string.Empty;
            if(state!=null)
            {
                stateCd = state.Code;
            }
            var data = this.parameters.GradeService.GetStateSubjectGradeByStateAndSubject(stateCd,subject).OrderBy(x => x.Order).Select(x => new ListItem { Name = x.Name, Id = x.Id }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteResource(int lessonId)
        {
            
            try
            {
                Lesson lesson = parameters.LessonService.GetLessons().Where(x => x.LessonId == lessonId).FirstOrDefault();
                if (lesson != null)
                {
                    //avoid modify ajax parameters
                    if (!parameters.VulnerabilityService.HasRightToEditLesson(CurrentUser, lesson.LessonId, CurrentUser.GetMemberListDistrictId()))
                    {
                        return Json(new { Success = false, errorMessage = "Has no right to delete this lesson." });
                    }
                    //Delete LessonGrade
                    parameters.LessonService.DeleteLessonGrade(lesson.LessonId);
                    //Delete LessonStateStandard
                    parameters.LessonService.DeleteLessonStateStandard(lesson.LessonId);
                    //Delete file in S3 if lesson has file
                    if(lesson!=null)
                    {
                        string buketName = LinkitConfigurationManager.GetS3Settings().LessonBucketName;
                        string folder =LinkitConfigurationManager.GetS3Settings().LessonFolder;
                        if (lesson.LessonFileTypeId != Util.LessonFileTypeUrlId)
                        {
                            lesson.LessonPath = string.IsNullOrEmpty(lesson.LessonPath)
                                                    ? string.Empty
                                                    : lesson.LessonPath;
                            if(lesson.LessonPath.Length > 0 && !lesson.LessonPath.ToLower().StartsWith("http"))
                            {
                                _s3Service.DeleteFile(buketName, folder, lesson.LessonPath);
                            }
                        }

                        buketName = LinkitConfigurationManager.GetS3Settings().GuideBucketName;
                        folder = LinkitConfigurationManager.GetS3Settings().GuideFolder;
                        lesson.GuidePath = string.IsNullOrEmpty(lesson.GuidePath) ? string.Empty : lesson.GuidePath;
                        if (lesson.GuidePath.Length > 0 && !lesson.GuidePath.ToLower().StartsWith("http"))
                        {
                            _s3Service.DeleteFile(buketName, folder, lesson.GuidePath);
                        }
                    }
                    
                    //Delete lesson
                    parameters.LessonService.DeleteLesson(lesson);
                }
                return Json(new { Success = "success" });
            }
            catch (Exception e)
            {
                //return ShowJsonResultException(model, e.Message);
                return Json(new { Success = "fail", errorMessage = string.Format("Can not delete lesson right now, error detail: {0}", e.Message) });

            }
        }
        #region Upload file
        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadFileLesson(HttpPostedFileBase postedFile, string fileExt, int lessonId)
        {
            if(postedFile==null)
            {
                return Json(new { success = false, errorMessage = "There's no file to upload." }, JsonRequestBehavior.AllowGet); ;
            }
            Lesson lesson = parameters.LessonService.GetLessons().Where(x => x.LessonId == lessonId).FirstOrDefault();
            if(lesson==null)
            {
                return Json(new { success = false, errorMessage = "Can not find the resource." }, JsonRequestBehavior.AllowGet); ;
            }
            string buketName = string.Empty;
            string folder = string.Empty; 
            string timeout = string.Empty;
            try
            {
                buketName = LinkitConfigurationManager.GetS3Settings().LessonBucketName;
            }
            catch
            {
                return Json(new { success = false, errorMessage = "Can not find Configuration LessonBucketName." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                folder = LinkitConfigurationManager.GetS3Settings().LessonFolder;
            }
            catch
            {
                return Json(new { success = false, errorMessage = "Can not find Configuration LessonFolder." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                timeout = ConfigurationManager.AppSettings["ResourceUploadTimeoutMinutes"];
            }
            catch
            {
                timeout = "10";//use default value is 10 minutes
            }

            fileExt = Path.GetExtension(postedFile.FileName);//fileExt is now gotten from file name
            //validate fileExt
            if (!IsAllowedFileType(fileExt))
            {
                return Json(new { success = false, errorMessage = "Forbidden file type." }, JsonRequestBehavior.AllowGet);
            }
            var fileName = GenerateFileName(postedFile.FileName, lessonId);
            if (CheckFileExisting(lessonId, fileName,lesson.LessonPath,lesson.GuidePath))
            {
                return Json(new { success = false, errorMessage = string.Format("File \"{0}\" has already existed for this lesson as resource file or reference file. Please delete it first or select another file.", postedFile.FileName) }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                string filePath = string.Format("{0}/{1}", folder, fileName);
                var s3Result = _s3Service.UploadResourceFile(buketName, filePath, postedFile.InputStream,int.Parse(timeout));
                
                if (s3Result.IsSuccess)
                {
                    
                    //delete old file
                    
                    if (!string.IsNullOrEmpty(lesson.LessonPath))
                    {
                        if (lesson.LessonPath.Length > 0)
                        {
                            string bucket = LinkitConfigurationManager.GetS3Settings().LessonBucketName;
                            string foler = LinkitConfigurationManager.GetS3Settings().LessonFolder;
                            _s3Service.DeleteFile(bucket, foler, lesson.LessonPath);
                        }
                    }

                    //update lesson
                    int fileType = FindFileType(fileExt);
                    parameters.LessonService.UpdateLessonPath(lessonId, fileType, fileName);
                    return Json(new { success = true,fileName=fileName }, JsonRequestBehavior.AllowGet);
                }
                ViewBag.UploadResourceFileErrorMessage = s3Result.ErrorMessage;
                return Json(new { success = false, errorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                ViewBag.UploadResourceFileErrorMessage = ex.Message;
                return Json(new { success = false, errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadFileGuide(HttpPostedFileBase postedFile, int lessonId)
        {
            if (postedFile == null)
            {
                return Json(new { success = false, errorMessage = "There's no file to upload." }, JsonRequestBehavior.AllowGet); ;
            }
            Lesson lesson = parameters.LessonService.GetLessons().Where(x => x.LessonId == lessonId).FirstOrDefault();
            if(lesson==null)
            {
                return Json(new { success = false, errorMessage = "Can not find resource." }, JsonRequestBehavior.AllowGet); ;
            }
            string buketName = string.Empty;
            string folder = string.Empty;
            string timeout = string.Empty;

            var fileName = GenerateFileName(postedFile.FileName, lessonId);

            
            try
            {
                buketName = LinkitConfigurationManager.GetS3Settings().GuideBucketName;
            }
            catch
            {
                return Json(new { success = false, errorMessage = "Can not find Configuration GuideBucketName" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                folder = LinkitConfigurationManager.GetS3Settings().GuideFolder;
            }
            catch
            {
                return Json(new { success = false, errorMessage = "Can not find Configuration GuideFolder" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                timeout = System.Configuration.ConfigurationManager.AppSettings["ResourceUploadTimeoutMinutes"];
            }
            catch
            {
                timeout = "10";//use default value is 10 minutes
            }

            if (CheckFileExisting(lessonId, fileName,lesson.LessonPath,lesson.GuidePath))
            {
                return Json(new { success = false, errorMessage = string.Format("File \"{0}\" has already existed for this lesson as resource file or reference file. Please delete it first or select another file.", postedFile.FileName) }, JsonRequestBehavior.AllowGet);
            }
            var fileExt = Path.GetExtension(postedFile.FileName);//fileExt is now gotten from file name
            //validate fileExt
            if (!IsAllowedFileType(fileExt))
            {
                return Json(new { success = false, errorMessage = "Forbidden file type." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                string filePath = string.Format("{0}/{1}", folder, fileName);
                var s3Result = _s3Service.UploadResourceFile(buketName, filePath, postedFile.InputStream,int.Parse(timeout));

                if (s3Result.IsSuccess)
                {
                    //delete old file
                    
                    if (!string.IsNullOrEmpty(lesson.GuidePath))
                    {
                        if (lesson.GuidePath.Length > 0)
                        {
                            string bucket = LinkitConfigurationManager.GetS3Settings().GuideBucketName;
                            string foler = LinkitConfigurationManager.GetS3Settings().GuideFolder;
                            _s3Service.DeleteFile(bucket, foler, lesson.GuidePath);
                        }
                    }

                    //Update guide path only
                    parameters.LessonService.UpdateGuidePath(lessonId, fileName);
                    return Json(new { success = true, fileName = fileName }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, errorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteFileLesson(int lessonId, string fileName)
        {
            try
            {
                //Delete file logo in S3

                string buketName = LinkitConfigurationManager.GetS3Settings().LessonBucketName;
                string folder = LinkitConfigurationManager.GetS3Settings().LessonFolder;
                
                S3Result result = _s3Service.DeleteFile(buketName, folder, fileName);
                if (result.IsSuccess)
                {
                    parameters.LessonService.UpdateLessonPath(lessonId, 26, string.Empty); //clear lesson path
                    return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = "Fail", errorMessage = result.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteFileGuide(int lessonId, string fileName)
        {
            try
            {
                //Delete file logo in S3

                string buketName = LinkitConfigurationManager.GetS3Settings().GuideBucketName;
                string folder = LinkitConfigurationManager.GetS3Settings().GuideFolder;

                S3Result result = _s3Service.DeleteFile(buketName, folder, fileName);
                if (result.IsSuccess)
                {
                    parameters.LessonService.UpdateGuidePath(lessonId, string.Empty);//clear lesson path
                    return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = "Fail", errorMessage = result.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }

        private int FindFileType(string fileExt)
        {
            if (string.IsNullOrEmpty(fileExt))
            {
                return 26;//Unknown
            }
            var tmp = fileExt.Remove(".").ToLower();
            if (tmp.Equals("docx"))
            {
                return 24;//Microsoft Word Document
            }
            if (tmp.Equals("pptx"))
            {
                return 4;//Microsoft Powerpoint
            }
            if (tmp.Equals("xlsx"))
            {
                return 8;//Microsoft Excel
            }
            var fileType = parameters.LessonFileTypeService.GetLessonFileType().Where(
                x => x.Name.ToLower().Contains(tmp)).FirstOrDefault();
            if (fileType == null)
            {
                return 26;//Unknown
            }
            else
            {
                return fileType.LessonFileTypeId;
            }
        }
       
        private string GenerateFileName(string fileName, int lessonId)
        {
            string newFileName = string.Empty;
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = string.Empty;
            }
            //generate a new file name with prefix lessonid
            newFileName = string.Format("{0}_{1}", lessonId.ToString(), fileName);
            return newFileName;

        }
        private bool CheckFileExisting(int lessonId, string fileName,string lessonPath,string guidePath)
        {
            //check if the file is existing or not
            if (string.IsNullOrEmpty(lessonPath))
            {
                lessonPath = string.Empty;
            }
            if (string.IsNullOrEmpty(guidePath))
            {
                guidePath = string.Empty;
            }
            if (lessonPath.ToLower().Equals(fileName.ToLower()))
            {
                return true;
            }
            if (guidePath.ToLower().Equals(fileName.ToLower()))
            {
                return true;
            }
            return false;
        }

        private bool IsAllowedFileType(string fileExt)
        {
            var mimeTypeConfig = parameters.ConfigurationService.GetConfigurationByKey(Util.Config_Resource_MimeType);
            var fileTypeList = Util.ParseFileTypesFromXmlConfig(mimeTypeConfig == null ? string.Empty : mimeTypeConfig.Value).Select(x => x.Extension.ToLower()).ToList();
            fileExt = fileExt.Replace(".", "").ToLower();
            return fileTypeList.Contains(fileExt);
        }
        #endregion Upload file
        #region Assign Grade
        [HttpGet]
        public ActionResult ShowAssignedGradeList(string gradeIdString)
        {
            if (gradeIdString == null)
            {
                gradeIdString = string.Empty;
            }
            ViewBag.LessonId = 0;
            ViewBag.GradeIdString = gradeIdString;
            return PartialView("_AssignedGradeList");
        }
        public ActionResult GetAssignedGrades(string gradeIdString)
        {
            List<int> assignedGradeIdList = ParseIdFromString(gradeIdString);
            var parser = new DataTableParser<Grade>();
            var data = this.parameters.GradeService.GetGrades().Where(x => assignedGradeIdList.Contains(x.Id));

            return Json(parser.Parse(data, true), JsonRequestBehavior.AllowGet);
        }
        private  List<int> ParseIdFromString(string idString)
        {
            //idString looks like ,-124-,-1234-,-245-
            if (idString == null)
            {
                idString = string.Empty;
            }
            string[] idList = idString.Split(',');
            List<int> result  = new List<int>();

            if (idList.IsNotNull())
            {
                foreach (var gradeId in idList)
                {
                    try
                    {
                        result.Add(int.Parse(gradeId.Replace("-","")));
                    }
                    catch
                    {

                    }
                }

            }
            return result;
        }
        public List<int> GetAssignedGradeIdList(int lessonId)
        {
            var parser = new DataTableParser<Grade>();
            List<int> assignedGradeIdList = parameters.LessonService.GetAssignedGradeIdList(lessonId);
            if (assignedGradeIdList == null)
            {
                assignedGradeIdList = new List<int>();
            }
            return assignedGradeIdList;
        }
       
        public ActionResult ShowAvailableGrades(int lessonId)
        {
            ViewBag.LessonId = lessonId;
            var lesson = parameters.LessonService.GetLessons().FirstOrDefault(x => x.LessonId == lessonId);
            ViewBag.LessonName = lesson==null?string.Empty:lesson.LessonName;
            return PartialView("_AvailableGrades");
        }
        public ActionResult GetAvailabelGrades()
        {
            var parser = new DataTableParser<GradeResourceViewModel>();
            var data = this.parameters.GradeService.GetGrades().OrderBy(x => x.Order).Select(x => new GradeResourceViewModel { Name = x.Name, Id = x.Id, Order = x.Order });
            return Json(parser.Parse(data, true), JsonRequestBehavior.AllowGet);
        }
        
        
      
        #endregion Assign Grade

        #region Assign Master Standard
        public ActionResult ShowAssignedMasterStandardList(string standardIdString)
        {
            if (string.IsNullOrEmpty(standardIdString))
            {
                standardIdString = string.Empty;
            }
            ViewBag.StandardIdString = standardIdString;
            return PartialView("_AssignedMasterStandardList");
        }
        public ActionResult GetAssignedMasterStandards(string standardIdString)
        {
            List<int> assignedMasterStandardIdList = ParseIdFromString(standardIdString);
            var data = this.parameters.MasterStandardService.GetMasterStandards().Where(x => assignedMasterStandardIdList.Contains(x.MasterStandardID))
                .Select(x=> new MasterStandardResourceListViewModel
                                {
                                    MasterStandardId = x.MasterStandardID,
                                    State = x.State,
                                    Subject = x.Subject,
                                    Year = x.Year,
                                    Grade = x.Grade,
                                    Level = x.Level,
                                    Label = x.Label,
                                    Number = x.Number,
                                    Description = x.Description
                                });
            var parser = new DataTableParser<MasterStandardResourceListViewModel>();
            return Json(parser.Parse(data, true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowAvailableMasterStandardEdit(int lessonId)
        {
            ViewBag.LessonId = lessonId;
            var lesson = parameters.LessonService.GetLessons().FirstOrDefault(x => x.LessonId == lessonId);
            ViewBag.LessonName = lesson == null ? string.Empty : lesson.LessonName;
            ViewBag.StateId = CurrentUser.StateId??0;
            return PartialView("_AvailableMasterStandard");
        }
        public ActionResult GetMasterStandard(int lessonId,string state, string subject, string grade,string childParentView,string parentGUID)
        {
            //List<int> assignedMasterStandardIdList = parameters.LessonService.GetAssignedMasterStandardIdList(lessonId);
            //if (assignedMasterStandardIdList == null)
            //{
            //    assignedMasterStandardIdList = new List<int>();
            //}
            int stateId = 0;
            try
            {
                stateId = int.Parse(state);//state can be 'select'
            }
            catch 
            {
                stateId = 0;
            }
            int gradeId = 0;
            try
            {
                gradeId = int.Parse(grade);//grade can be 'select'
            }
            catch
            {
                gradeId = 0;
            }
            var parser = new DataTableParser<MasterStandardResourceViewModel>();
            var data = this.parameters.MasterStandardService.GetMasterStandards().Where(x => x.StateId == stateId && x.Subject.Equals(subject) && x.Archived == false);
            //only parent standard,parent standard's ParaneGUID column is not an valid GUID,length of a valid GUID is 36 or it has children
            if (childParentView.Equals("viewchild"))
            {
                if (string.IsNullOrEmpty(parentGUID))//only get the top parent
                {
                    data = data.Where(x => x.ParentGUID.Length < 36);
                    //if (stateId == 78)//Common Core State Standards,GetStateStandardsByStateAndSubjectAndGradesTopLevelCC filter Archived=0
                    //{
                    //    data = data.Where(x => x.Archived == false);
                    //}
                }
                else//non-top
                {
                    data = data.Where(x => x.ParentGUID.Equals(parentGUID));
                }
            }
            else//viewparent
            {
                var grandStandard =this.parameters.MasterStandardService.GetMasterStandards().Where(x => x.GUID == parentGUID).FirstOrDefault();
                if(grandStandard!=null)
                {
                    if (string.IsNullOrEmpty(grandStandard.ParentGUID))//only get the top parent
                    {
                        data = data.Where(x => x.ParentGUID.Length < 36);
                        //if (stateId == 78)//Common Core State Standards,GetStateStandardsByStateAndSubjectAndGradesTopLevelCC filter Archived=0
                        //{
                        //    data = data.Where(x => x.Archived == false);
                        //}
                    }
                    else
                    {
                        data = data.Where(x => x.ParentGUID.Equals(grandStandard.ParentGUID));
                    }
                    
                }

            }
            

            //check grade
            List<MasterStandardResourceViewModel> result = new List<MasterStandardResourceViewModel>();
            List<Grade> gradeList = this.parameters.GradeService.GetGrades().ToList();
            if( gradeList.IsNull())
            {
                gradeList = new List<Grade>();
            }
            int searchGradeOrder = 0;
            var g = gradeList.Find(x => x.Id == gradeId);
            if(g!=null)
            {
                searchGradeOrder = g.Order;
            }
            if (searchGradeOrder > 0)
            {
                foreach (var masterStandard in data)
                {
                    if (CheckValidStandardGrade(masterStandard, gradeList, searchGradeOrder))
                    {
                        result.Add(new MasterStandardResourceViewModel
                                       {
                                           GUID = masterStandard.GUID,
                                           MasterStandardId = masterStandard.MasterStandardID,
                                           Number = masterStandard.Number,
                                           Description = masterStandard.Description,
                                           CountChildren = masterStandard.CountChildren,
                                           ParentGUID = masterStandard.ParentGUID
                                       });
                    }
                }
            }
            //var data = this.parameters.MasterStandardService.GetMasterStandards().Select(x => new MasterStandardViewModel { MasterStandardId = x.MasterStandardId, Number = x.Number, Description = x.Description, State = x.State });

            return Json(parser.Parse(result.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }
        private bool CheckValidStandardGrade(MasterStandardResource masterStandard,List<Grade> gradeList,int searchGradeOrder )
        {
            int lowGrade = 0;
            int hightGrade = 0;
            if(masterStandard.LowGradeID > 0)
            {
                lowGrade = masterStandard.LowGradeID.Value;
            }
            else
            {
                //Need to get low grade base on LoGrade, but checking database and see that LoGrade is sometime GradeId, sometime is GradeName
                Grade g= gradeList.Find(x => x.Name.Equals(masterStandard.LoGrade));
                if(g!=null)
                {
                    lowGrade = g.Id;
                }
                else
                {
                    //LoGrade can be GradeId
                    try
                    {
                        g = gradeList.Find(x => x.Id.Equals(int.Parse(masterStandard.LoGrade)));
                        if (g != null)
                        {
                            lowGrade = g.Id;
                        }
                    }
                    catch
                    {
                        lowGrade = 0;
                    }
                }
            }
            //
            if (masterStandard.HighGradeID > 0)
            {
                hightGrade = masterStandard.HighGradeID.Value;
            }
            else
            {
                //Need to get low grade base on HiGrade, but checking database and see that HiGrade is sometime GradeId, sometime is GradeName
                Grade g = gradeList.Find(x => x.Name.Equals(masterStandard.HiGrade));
                if (g != null)
                {
                    hightGrade = g.Id;
                }
                else
                {
                    //LoGrade can be GradeId
                    try
                    {
                        g = gradeList.Find(x => x.Id.Equals(int.Parse(masterStandard.HiGrade)));
                        if (g != null)
                        {
                            hightGrade = g.Id;
                        }
                    }
                    catch
                    {
                        hightGrade = 0;
                    }
                }
            }

            int lowGradeOrder = 0;
            int hightGradeOrder = 0;
            try
            {
                var g = gradeList.Find(x => x.Id == lowGrade);
                if(g!=null)
                {
                    lowGradeOrder = g.Order;
                }
            }
            catch
            {
                lowGradeOrder = 0;
            }

            try
            {
                var g = gradeList.Find(x => x.Id == hightGrade);
                if (g != null)
                {
                    hightGradeOrder = g.Order;
                }
            }
            catch
            {
                hightGradeOrder = 0;
            }

            return (hightGradeOrder > 0 && lowGradeOrder > 0 && (lowGradeOrder <= searchGradeOrder && searchGradeOrder <= hightGradeOrder));
        }
        public ActionResult GetAssignedMasterStandardsByState(string standardIdString, int? stateId)
        {
            List<int> assignedMasterStandardIdList = ParseIdFromString(standardIdString);
            var data = this.parameters.MasterStandardService.GetMasterStandards().Where(x => assignedMasterStandardIdList.Contains(x.MasterStandardID));
            if (stateId.HasValue)
            {
                data = data.Where(x => x.StateId.Equals(stateId.Value));
            }
            var data1 = data.Select(x => new MasterStandardResourceAssignedViewModel
                                        {
                                            MasterStandardId =x.MasterStandardID,
                                            Number = x.Number,
                                            Description = x.Description,
                                            State = x.State

                                        });
            var parser = new DataTableParser<MasterStandardResourceAssignedViewModel>();
            return Json(parser.Parse(data1, true), JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetStateCode(int? stateId)
        {
            var data = parameters.StateService.GetStates().Where(x => x.Id == (stateId??0)).FirstOrDefault();
            string stateCd = string.Empty;
            if(data!=null)
            {
                stateCd = data.Code;
            }
            return Json(new { StateCd = stateCd }, JsonRequestBehavior.AllowGet);
        }
        #endregion Assign Master Standard

        
    }
}

