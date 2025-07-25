using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Models.PDFGenerator;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.ServiceConsumer;
using LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult;
using S3Library;
using System.Configuration;
using System.Web.Script.Serialization;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestPreferences;
using System.Data;
using LinkIt.BubbleSheetPortal.Common.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class DataLockerEntryMultiDateResultController : BaseController
    {
        private readonly DataLockerControllerParameters _parameters;
        private readonly IS3Service _s3Service;
        private readonly IDocumentManagement _documentManagementService;

        public string DTLBucket
        {
            get
            {
                return LinkitConfigurationManager.GetS3Settings().DTLBucket;//get from Vault
            }
        }
        public string DTLFolder
        {
            get
            {
                return ConfigurationManager.AppSettings[ContaintUtil.AppSettingDTLFolderName];
            }
        }
        public DataLockerEntryMultiDateResultController(
            DataLockerControllerParameters parameters,
            IS3Service s3Service,
            IDocumentManagement documentManagementService)
        {
            _parameters = parameters;
            _s3Service = s3Service;
            _documentManagementService = documentManagementService;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.Definetemplates)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.BuildEntryForms)]
        public ActionResult BuildEntryForms()
        {
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.StateId = CurrentUser.StateId;
            ViewBag.DistrictId = CurrentUser.DistrictId;
            return View();
        }


        private bool IsUserAdmin()
        {
            return _parameters.UserService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.EnterResults)]
        [HttpGet]
        public ActionResult EnterResults()
        {
            var model = new EnterResultModel()
            {
                IsAdmin = IsUserAdmin(),
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher,
                IsSchoolAdmin = CurrentUser.RoleId.Equals(8),
                IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher),
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                ListDistricIds = CurrentUser.IsNetworkAdmin ? CurrentUser.GetMemberListDistrictId() : null
            };
            if (!model.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                model.IsUseTestExtract = true;
            }
            return View(model);
        }

        [UrlReturnDecode]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ResultsEntryDataLocker)]
        public ActionResult EntryResults(int? virtualTestId, int? classId)
        {
            var model = new EntryResultModel();
            if (virtualTestId.HasValue && classId.HasValue)
            {
                model.ClassId = classId.Value;
                model.VirtualTestId = virtualTestId.Value;
                var resultsDates = _parameters.TestResultService.GetResultDates(virtualTestId.Value, classId.Value).OrderByDescending(x => x);
                model.ResultDate = resultsDates.FirstOrDefault().ToString();
                var studentIds = _parameters.ClassStudentService.GetClassStudentsByClassId(classId.Value).Select(x => x.StudentId).Distinct().ToList();
                model.StudentsIdSelectedString = string.Join(",", studentIds);
            }
            else
            {
                if (Session["EntryResultModel"] == null)
                    return RedirectToAction("EnterResults", "DataLockerEntryResult");
                model = (EntryResultModel)Session["EntryResultModel"];
            }

            if (!model.VirtualTestId.HasValue || !model.ClassId.HasValue || string.IsNullOrEmpty(model.ResultDate))
                return RedirectToAction("EnterResults", "DataLockerEntryResult");

            var virtualTestCustomScore = _parameters.DataLockerService.GetVirtualTestCustomScore(model.VirtualTestId.GetValueOrDefault());
            if (virtualTestCustomScore != null && !(virtualTestCustomScore.IsMultiDate.HasValue && virtualTestCustomScore.IsMultiDate.Value))
                return RedirectToAction("EnterResults", "DataLockerEntryResult");

            if (!model.DistrictId.HasValue || model.DistrictId.Value <= 0)
                model.DistrictId = CurrentUser.DistrictId;

            var usingMultiDate = _parameters.DistrictDecodeService.GetDistrictDecodeByLabel(model.DistrictId.GetValueOrDefault(), Constanst.UseMultiDateTemplate);
            if (!usingMultiDate)
                return RedirectToAction("EnterResults", "DataLockerEntryResult");

            //security
            var hasPermissionOnClass = _parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, model.ClassId.GetValueOrDefault());
            if (!hasPermissionOnClass)
                return RedirectToAction("EnterResults", "DataLockerEntryResult");

            try
            {
                model.DateFormatModel = _parameters.DistrictDecodeService.GetDateFormat(model.DistrictId.GetValueOrDefault());
                var virtualTest = _parameters.VirtualTestService.GetTestById(model.VirtualTestId.Value);
                if (virtualTest != null)
                {
                    model.VirtualTestName = virtualTest.Name;
                    model.RubricDescription = virtualTest.Instruction.ReplaceWeirdCharacters();
                    var virtualtestFile =
                        _parameters.VirtualTestFileServices.GetFirstOrDefaultByVirtualTest(virtualTest.VirtualTestID);
                    if (virtualtestFile != null)
                        model.VirtualtestFileKey = virtualtestFile.FileKey;
                    if (virtualTestId.HasValue && classId.HasValue)
                    {
                        var bank = _parameters.BankServices.GetBankById(virtualTest.BankID);
                        var subject = _parameters.SubjectServices.GetSubjectById(bank.SubjectID);
                        model.BankId = virtualTest.BankID;
                        model.SubjectId = bank.SubjectID;
                        model.GradeId = subject.GradeId;
                    }
                }

                var classObject = _parameters.ClassService.GetClassById(model.ClassId.Value);
                if (classObject != null)
                {
                    model.ClassName = classObject.Name;
                }
                //get data preference
                var preferences = _parameters.PreferencesService.GetDataLockerPreferencesLevel(new GetPreferencesParams
                {
                    CurrrentLevelId = (int)DataLockerPreferencesLevel.Form,
                    DistrictId = model.DistrictId.Value,
                    UserId = CurrentUser.Id,
                    UserRoleId = CurrentUser.RoleId,
                    IsSurvey = false,
                    StateId = CurrentUser.StateId.GetValueOrDefault(),
                    VirtualTestId = model.VirtualTestId.Value
                });
                model.AllowChangeResultDate = (preferences != null && preferences.DataSettings != null && preferences.DataSettings.AllowResultDateChange != null) ? preferences.DataSettings.AllowResultDateChange.AllowChange : "0";
                model.HasTestResult = _parameters.VirtualTestService.CheckHasTestResultByTestId(model.VirtualTestId.Value);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = false, ErrorMessage = ex.Message });
            }

            model.CurrentUserDistrictId = CurrentUser.DistrictId;

            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteAutoSaveData(int? virtualTestId, int? classId, DateTime? resultDate)
        {
            try
            {
                _parameters.DataLockerService.DeleteAutoSaveMultiDate(virtualTestId.GetValueOrDefault(),
                    classId.GetValueOrDefault(), CurrentUser.Id, resultDate);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetEntryStudents(int virtualTestId, int classId, string studentsIdSelectedString, DateTime entryResultDate)
        {
            var model = new ResultEntryInputScoreModel();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var virtualTestCustomScore = _parameters.DataLockerService.GetVirtualTestCustomScore(virtualTestId);
            if (virtualTestCustomScore != null)
            {
                var virtualTestCustomMetaDatas = _parameters.DataLockerService.GetVirtualTestCustomMetaDataByVirtualTestCustomScoreId(virtualTestCustomScore.VirtualTestCustomScoreId);
                model.CustomScore = new ResultEntryCustomScore
                {
                    Name = virtualTestCustomScore.Name,
                    VirtualTestCustomScoreId = virtualTestCustomScore.VirtualTestCustomScoreId,
                    ScoreInfos = GetVirtualTestCustomScoreInfo(virtualTestCustomScore, virtualTestCustomMetaDatas)
                };

                var virtualTestCustomSubScores = _parameters.DataLockerService.GetVirtualTestCustomSubScores(virtualTestCustomScore.VirtualTestCustomScoreId).OrderBy(o => o.Sequence);
                model.CustomSubScores = new List<ResultEntryCustomSubScore>();
                foreach (var virtualTestCustomSubScore in virtualTestCustomSubScores)
                {
                    var virtualTestCustomSubMetaDatas = _parameters.DataLockerService.GetVirtualTestCustomMetaDataByVirtualTestCustomSubScoreID(virtualTestCustomSubScore.VirtualTestCustomSubScoreId);
                    model.CustomSubScores.Add(new ResultEntryCustomSubScore
                    {
                        Name = virtualTestCustomSubScore.Name,
                        VirtualTestCustomSubScoreId = virtualTestCustomSubScore.VirtualTestCustomSubScoreId,
                        ScoreInfos = GetVirtualTestCustomScoreInfo(virtualTestCustomSubScore, virtualTestCustomSubMetaDatas)
                    });
                }
            }
            var studentResultDates = _parameters.TestResultService.GetStudentResultDates(virtualTestId, classId);
            model.EntryResultDates = studentResultDates.Select(x => x.ResultDate.ToShortDateString()).Distinct().ToList();
            model.StudentResultDates = studentResultDates.GroupBy(x => x.StudentId)
                .Select(g => new StudentResultDate() { StudentId = g.Key, ResultDates = g.OrderBy(x => x.ResultDate).Select(o => o.ResultDate.ToShortDateString()).Distinct().ToList() }).ToList();

            var studentTestResultScoresResult = new JArray();
            var studentTestResultSubScoresResult = new JArray();
            var autoSave = _parameters.DataLockerService.GetAutoSaveDataBaseOnDate(virtualTestId, classId, CurrentUser.Id, entryResultDate);
            if (autoSave != null)
            {
                if (!string.IsNullOrEmpty(autoSave.StudentTestResultScoresJson))
                {
                    var studentTestResultScoresSaved = autoSave.StudentTestResultScoresJson.ParseToJArray();
                    foreach (var item in studentTestResultScoresSaved)
                    {
                        item[nameof(DTLStudentAndTestResultScore.IsAutoSave)] = true;
                    }
                    studentTestResultScoresResult.Merge(studentTestResultScoresSaved);
                    model.StudentTestResultScores = autoSave.StudentTestResultScoresJson;
                }

                if (!string.IsNullOrEmpty(autoSave.StudentTestResultSubScoresJson))
                {
                    var studentTestResultSubScoresSaved = autoSave.StudentTestResultSubScoresJson.ParseToJArray();
                    foreach (var item in studentTestResultSubScoresSaved)
                    {
                        item[nameof(DTLStudentAndTestResultSubScore.IsAutoSave)] = true;
                    }
                    studentTestResultSubScoresResult.Merge(studentTestResultSubScoresSaved);
                    model.StudentTestResultSubScores = autoSave.StudentTestResultSubScoresJson;
                }

                if (!string.IsNullOrEmpty(autoSave.ActualTestResultScoresJson))
                    model.ActualTestResultScoresJson = autoSave.ActualTestResultScoresJson;
            }
            var studentIdsSaved = studentTestResultScoresResult
                .Select(x => x[nameof(DTLStudentAndTestResultScore.StudentID)]?.Value<int?>())
                .Where(x => x.HasValue)
                .Select(x => (int)x)
                .ToList();
            var studentIdGetting = studentsIdSelectedString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x => Convert.ToInt32(x))
                                    .Except(studentIdsSaved);
            //Check show pbs
            var EntryResultSession = (EntryResultModel)Session["EntryResultModel"];
            int districtFilter = EntryResultSession != null && EntryResultSession.DistrictId.HasValue && EntryResultSession.DistrictId > 0 ? EntryResultSession.DistrictId.Value : CurrentUser.DistrictId.Value;
            var preferences = _parameters.PreferencesService.GetDataLockerPreferencesLevel(new GetPreferencesParams
            {
                CurrrentLevelId = (int)DataLockerPreferencesLevel.Form,
                DistrictId = districtFilter,
                UserId = CurrentUser.Id,
                UserRoleId = CurrentUser.RoleId,
                IsSurvey = false,
                StateId = CurrentUser.StateId.GetValueOrDefault(),
                VirtualTestId = virtualTestId
            });
            if (preferences != null && preferences.DataSettings != null && preferences.DataSettings.DisplayPerformanceBandsInEnterResults != null && preferences.DataSettings.DisplayPerformanceBandsInEnterResults.AllowDisplay == "1")
            {
                var pbsDataSource = _parameters.DataLockerService.GetPBSScoreMetaData(districtFilter, virtualTestId.ToString());
                if (pbsDataSource != null && pbsDataSource.Tables != null && pbsDataSource.Tables.Count > 0)
                {
                    model.PerformanceBandSettingScores = new List<PerformanceBandSettingScoreModel>();
                    model.PerformanceBandSettingSubScores = new List<PerformanceBandSettingScoreModel>();
                    for (int i = 0; i < pbsDataSource.Tables.Count; i++)
                    {
                        DataTable pbsData = pbsDataSource.Tables[i];
                        bool checkLevel = pbsDataSource.Tables[i].AsEnumerable().Any(w => w.Field<int>("Level") == (int)DistrictLevelEnum.Global);
                        if (checkLevel)
                            checkLevel = pbsDataSource.Tables[i].AsEnumerable().Any(w => w.Field<int>("Level") == (int)DistrictLevelEnum.District);
                        if (checkLevel)
                        {
                            checkLevel = pbsDataSource.Tables[i].AsEnumerable().Any(w => w.Field<int>("Level") == (int)DistrictLevelEnum.Global
                            && w.Field<int>("LOCKED") == 1);
                            if (checkLevel)
                                pbsData = pbsData.AsEnumerable().Where(w => w.Field<int>("Level") == (int)DistrictLevelEnum.Global).CopyToDataTable();
                            else
                                pbsData = pbsData.AsEnumerable().Where(w => w.Field<int>("Level") == (int)DistrictLevelEnum.District).CopyToDataTable();
                        }

                        for (int j = 0; j < pbsData.Rows.Count; j++)
                        {
                            PerformanceBandSettingScoreModel psb = new PerformanceBandSettingScoreModel()
                            {
                                VirtualTestID = Convert.ToInt32(pbsData.Rows[j]["VirtualTestID"]),
                                Bands = pbsData.Rows[j]["Bands"].ToString(),
                                Label = pbsData.Rows[j]["Label"].ToString(),
                                Cutoffs = pbsData.Rows[j]["Cutoffs"].ToString(),
                                ScoreType = pbsData.Rows[j]["ScoreType"].ToString(),
                                Color = pbsData.Rows[j]["Color"].ToString(),
                                Level = Convert.ToInt32(pbsData.Rows[j]["Level"]),
                                LOCKED = Convert.ToInt32(pbsData.Rows[j]["LOCKED"])
                            };
                            if (pbsData.Columns.Contains("SubScoreName"))
                            {
                                psb.SubScoreName = pbsData.Rows[j]["SubScoreName"].ToString();
                                model.PerformanceBandSettingSubScores.Add(psb);
                            }
                            else
                            {
                                model.PerformanceBandSettingScores.Add(psb);
                            }
                        }
                    }
                }
            }

            if (studentIdGetting != null && studentIdGetting.Any())
            {
                var studentTestResultScoreQueryable =
                    _parameters.DataLockerService.GetStudentAndTestResultScoreMultiple(virtualTestId, classId, string.Join(",", studentIdGetting), entryResultDate);
                var documentGuidIds = studentTestResultScoreQueryable.Where(w => w.Artifacts != null).SelectMany(s => s.Artifacts.Where(p => p.DocumentGuid != null && p.DocumentGuid != Guid.Empty)?.Select(art => art.DocumentGuid));
                var documentInfos = new List<DocumentInforDto>();
                if (documentGuidIds != null && documentGuidIds.Count() > 0)
                {
                    documentInfos = _documentManagementService.GetDocumentInfoList(documentGuidIds);
                }
                var studentTestResultScores = studentTestResultScoreQueryable.Select(x => new DTLStudentAndTestResultScore()
                {
                    AchievementLevel = x.AchievementLevel,
                    ClassID = x.ClassID ?? classId,
                    Code = x.Code,
                    AltCode = x.AltCode,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MiddleName = x.MiddleName,
                    ResultDate = x.ResultDate.HasValue ?
                        DateTime.SpecifyKind(x.ResultDate.Value, DateTimeKind.Utc) :
                        (DateTime?)null,
                    ScoreCustomA_1 = x.ScoreCustomA_1,
                    ScoreCustomA_2 = x.ScoreCustomA_2,
                    ScoreCustomA_3 = x.ScoreCustomA_3,
                    ScoreCustomA_4 = x.ScoreCustomA_4,
                    ScoreCustomN_1 = x.ScoreCustomN_1,
                    ScoreCustomN_2 = x.ScoreCustomN_2,
                    ScoreCustomN_3 = x.ScoreCustomN_3,
                    ScoreCustomN_4 = x.ScoreCustomN_4,
                    ScorePercent = x.ScorePercent,
                    ScorePercentage = x.ScorePercentage,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    StudentID = x.StudentID,
                    TestResultID = x.TestResultID,
                    TestResultScoreID = x.TestResultScoreID,
                    VirtualTestID = x.VirtualTestID ?? virtualTestId,
                    Artifacts = x.Artifacts?.Select(art =>
                    {
                        var documentInfo = documentInfos?.FirstOrDefault(f => f.DocumentGuid == art.DocumentGuid);
                        return new TestResultScoreArtifact
                        {
                            Name = art.IsLink ? art.Url : documentInfo?.FileName,
                            IsLink = art.IsLink,
                            Url = art.IsLink ? art.Url : documentInfo != null ? _documentManagementService.GetPresignedLinkAsync(art.DocumentGuid.Value) : GetS3Url(art.Name),
                            UploadDate = DateTime.SpecifyKind(art.UploadDate, DateTimeKind.Utc),
                            TagValue = art.TagValue,
                            CreatedBy = art.CreatedBy,
                            DocumentGuid = art.DocumentGuid
                        };

                    }).ToList(),
                    Notes = x.Notes,
                    HasOtherScore = x.HasOtherScore,
                    Colors = _parameters.DataLockerService.GetPBSColorForScoreResult(model.PerformanceBandSettingScores, x, null, false)
                }).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

                var studentTestResultSubScoreQueryable = _parameters.DataLockerService.GetStudentAndTestResultSubScoreMultiple(virtualTestId, classId, string.Join(",", studentIdGetting), entryResultDate);
                documentGuidIds = studentTestResultSubScoreQueryable.Where(w => w.Artifacts != null).SelectMany(s => s.Artifacts.Where(p => p.DocumentGuid != null && p.DocumentGuid != Guid.Empty)?.Select(art => art.DocumentGuid));
                documentInfos = new List<DocumentInforDto>();
                if (documentGuidIds != null && documentGuidIds.Count() > 0)
                {
                    documentInfos = _documentManagementService.GetDocumentInfoList(documentGuidIds);
                }
                var studentTestResultSubScores = studentTestResultSubScoreQueryable.Select(x => new DTLStudentAndTestResultSubScore()
                {
                    ClassID = x.ClassID,
                    Code = x.Code,
                    AltCode = x.AltCode,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MiddleName = x.MiddleName,
                    ResultDate = x.ResultDate.HasValue ?
                        DateTime.SpecifyKind(x.ResultDate.Value, DateTimeKind.Utc) :
                        (DateTime?)null,
                    Name = x.Name,
                    ScoreCustomA_1 = x.ScoreCustomA_1,
                    ScoreCustomA_2 = x.ScoreCustomA_2,
                    ScoreCustomA_3 = x.ScoreCustomA_3,
                    ScoreCustomA_4 = x.ScoreCustomA_4,
                    ScoreCustomN_1 = x.ScoreCustomN_1,
                    ScoreCustomN_2 = x.ScoreCustomN_2,
                    ScoreCustomN_3 = x.ScoreCustomN_3,
                    ScoreCustomN_4 = x.ScoreCustomN_4,
                    ScorePercent = x.ScorePercent,
                    ScorePercentage = x.ScorePercentage,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    StudentID = x.StudentID,
                    TestResultID = x.TestResultID,
                    TestResultScoreID = x.TestResultScoreID,
                    VirtualTestID = x.VirtualTestID,
                    TestResultScoreSubID = x.TestResultScoreSubID,
                    Artifacts = x.Artifacts?.Select(art =>
                    {
                        var documentInfo = documentInfos?.FirstOrDefault(f => f.DocumentGuid == art.DocumentGuid);
                        return new TestResultScoreArtifact
                        {
                            Name = art.IsLink ? art.Url : documentInfos?.FirstOrDefault(f => f.DocumentGuid == art.DocumentGuid)?.FileName,
                            IsLink = art.IsLink,
                            Url = art.IsLink ? art.Url : documentInfo != null ? _documentManagementService.GetPresignedLinkAsync(art.DocumentGuid.Value) : GetS3Url(art.Name),
                            UploadDate = DateTime.SpecifyKind(art.UploadDate, DateTimeKind.Utc),
                            TagValue = art.TagValue,
                            CreatedBy = art.CreatedBy,
                            DocumentGuid = art.DocumentGuid
                        };
                    }).ToList(),
                    Notes = x.Notes,
                    HasOtherScore = x.HasOtherScore,
                    Colors = _parameters.DataLockerService.GetPBSColorForScoreResult(model.PerformanceBandSettingSubScores, null, x, true)
                }).ToList();
                studentTestResultScoresResult.Merge(JArray.FromObject(studentTestResultScores));
                studentTestResultSubScoresResult.Merge(JArray.FromObject(studentTestResultSubScores));
            }            

            model.StudentTestResultScores = JsonConvert.SerializeObject(studentTestResultScoresResult);
            model.StudentTestResultSubScores = JsonConvert.SerializeObject(studentTestResultSubScoresResult);

            return new ContentResult() { Content = serializer.Serialize(model), ContentType = "text/json" };
        }

        [HttpPost]
        public ActionResult GetDateHasResult(int virtualTestId, int studentId)
        {
            var dateResults = _parameters.TestResultService.GetDateHasResultStudent(virtualTestId, studentId).ToArray();
            var dateResultFormat = dateResults.Select(d => d.ToString("dd-MMM-yy")).ToArray();
            return Json(dateResultFormat, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CheckAutoSaveData(int? virtualTestId, int? classId, DateTime dateTime)
        {
            var autoSave = _parameters.DataLockerService.GetAutoSaveDataBaseOnDate(virtualTestId.GetValueOrDefault(), classId.GetValueOrDefault(), CurrentUser.Id, dateTime);
            if (autoSave != null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AutoSaveResult(int? virtualTestId, int? classId, string studentTestResultScores, string studentTestResultSubScores, string actualTestResultScoresJson, DateTime? resultDate)
        {
            var data = new DTLAutoSaveResultData()
            {
                VirtualTestId = virtualTestId ?? 0,
                ClassId = classId ?? 0,
                StudentTestResultScoresJson = studentTestResultScores,
                StudentTestResultSubScoresJson = studentTestResultSubScores,
                ActualTestResultScoresJson = actualTestResultScoresJson,
                UserId = CurrentUser.Id,
                ResultDate = resultDate
            };
            try
            {
                _parameters.DataLockerService.AutoSaveResultBaseDate(data);
            }
            catch
            {
                return Json(new { Success = false, JsonRequestBehavior.AllowGet });
            }
            return Json(new { Success = true, JsonRequestBehavior.AllowGet });
        }
        [HttpPost]
        public ActionResult SaveResults(List<ResultEntrySaveScoreModel> scores)
        {
            if (scores.Any())
            {
                try
                {
                    var classId = scores[0].TestResultScore.ClassID;
                    DateTime dateSave = scores[0].TestResultScore.ResultDate ?? DateTime.Now;
                    var classObject = _parameters.ClassService.GetClassByIdWithoutFilterByActiveTerm(classId ?? 0);
                    var classUser = _parameters.ClassUserService.GetClassUsersByClassId(classId ?? 0).FirstOrDefault(x => x.ClassUserLOEId == 1);
                    var userid = classUser != null ? classUser.UserId : CurrentUser.Id;
                    var districtId = CurrentUser.IsPublisherOrNetworkAdmin
                        ? _parameters.SchoolService.GetDistrictIdBySchoolId(classObject.SchoolId.GetValueOrDefault())
                        : CurrentUser.DistrictId;

                    var virtualtestId = scores[0].TestResultScore.VirtualTestID;
                    var virtualTestCustomScore =
                        _parameters.DataLockerService.GetVirtualTestCustomScore(virtualtestId ?? 0);
                    var virtualTestCustomSubScores =
                        _parameters.DataLockerService.GetVirtualTestCustomSubScores(
                            virtualTestCustomScore.VirtualTestCustomScoreId);

                    var testresultList = new List<TestResult>();
                    var testresultScoreList = new List<ResultEntryTestResultScore>();
                    var testresultSubScoreList = new List<ResultEntryTestResultSubScore>();

                    var testResultScoreNoteList = new List<TestResultScoreNoteViewModel>();
                    var testResultSubScoreNoteList = new List<TestResultSubScoreNoteViewModel>();

                    var testResultScoreUploadFile = new List<TestResultScoreUploadFileMapModel>();
                    var testResultSubScoreUploadFile = new List<TestResultSubScoreUploadFileMapModel>();

                    var testresultIdDelete = new List<int>();
                    var updatedDocumentGuids = new List<Guid?>();

                    foreach (var item in scores)
                    {
                        var isValid = IsValidData(item);
                        if (isValid)
                        {
                            var testResult = new TestResult()
                            {
                                TestResultId = item.TestResultScore.TestResultID ?? 0,
                                VirtualTestId = item.TestResultScore.VirtualTestID ?? 0,
                                StudentId = item.TestResultScore.StudentID,
                                TeacherId = userid,
                                SchoolId = classObject.SchoolId ?? 0,
                                ResultDate = item.TestResultScore.ResultDate ?? DateTime.UtcNow,
                                CreatedDate = DateTime.UtcNow,
                                UpdatedDate = DateTime.UtcNow,
                                ClassId = item.TestResultScore.ClassID ?? 0,
                                GradedById = 0,
                                ScoreType = 1,
                                ScoreValue = 0,
                                DistrictTermId = classObject.DistrictTermId ?? 0,
                                UserId = userid,
                                OriginalUserId = userid,
                                DistrictID = districtId,
                                CreatedBy = CurrentUser.Id,
                                ModifiedBy = CurrentUser.Id
                            };

                            testresultList.Add(testResult);

                            if (item.TestResultScore.Notes != null)
                            {
                                testResultScoreNoteList.AddRange(BuildTestResultScoreNoteViewModel(item.TestResultScore));
                            }

                            if (item.TestResultScore.Artifacts != null && item.TestResultScore.Artifacts.Any())
                            {
                                testResultScoreUploadFile.AddRange(item.TestResultScore.Artifacts.Select(art => new TestResultScoreUploadFileMapModel
                                {
                                    FileName = art.IsLink ? art.Name : null,
                                    IsUrl = art.IsLink,
                                    TestResultScoreID = item.TestResultScore.TestResultScoreID,
                                    UploadDate = art.UploadDate,
                                    VirtualTestID = item.TestResultScore.VirtualTestID ?? 0,
                                    StudentID = item.TestResultScore.StudentID,
                                    ClassID = item.TestResultScore.ClassID ?? 0,
                                    Tag = art.TagValue,
                                    CreatedBy = art.CreatedBy ?? CurrentUser.Id,
                                    DocumentGuid = art.DocumentGuid
                                }).ToList());

                                updatedDocumentGuids.AddRange(item.TestResultScore.Artifacts.Select(p => p.DocumentGuid));
                            }

                            //save testresultScore
                            var testresultScore = new ResultEntryTestResultScore()
                            {
                                VirtualTestID = item.TestResultScore.VirtualTestID ?? 0,
                                StudentID = item.TestResultScore.StudentID,
                                TestResultID = item.TestResultScore.TestResultID ?? 0,
                                TestResultScoreID = item.TestResultScore.TestResultScoreID,
                                ClassID = item.TestResultScore.ClassID ?? 0,
                                //TookTest
                                ScoreRaw = item.TestResultScore.ScoreRaw,
                                ScoreScaled = item.TestResultScore.ScoreScaled,
                                ScorePercent = item.TestResultScore.ScorePercent,
                                ScorePercentage = item.TestResultScore.ScorePercentage,
                                ScoreCustomN_1 = item.TestResultScore.ScoreCustomN_1,
                                ScoreCustomN_2 = item.TestResultScore.ScoreCustomN_2,
                                ScoreCustomN_3 = item.TestResultScore.ScoreCustomN_3,
                                ScoreCustomN_4 = item.TestResultScore.ScoreCustomN_4,
                                ScoreCustomA_1 = item.TestResultScore.ScoreCustomA_1,
                                ScoreCustomA_2 = item.TestResultScore.ScoreCustomA_2,
                                ScoreCustomA_3 = item.TestResultScore.ScoreCustomA_3,
                                ScoreCustomA_4 = item.TestResultScore.ScoreCustomA_4,

                                UseRaw = virtualTestCustomScore.UseRaw,
                                UsePercent = virtualTestCustomScore.UsePercent,
                                UsePercentage = virtualTestCustomScore.UsePercentile,
                                UseScaled = virtualTestCustomScore.UseScaled,
                                AchievementLevel = virtualTestCustomScore.AchievementLevelSettingId,
                                PointsPossible = virtualTestCustomScore.PointsPossible,
                                UseArtifact = virtualTestCustomScore.UseArtifact
                            };
                            testresultScoreList.Add(testresultScore);

                            if (item.TestResultSubScores != null)
                            {
                                foreach (var subItem in item.TestResultSubScores)
                                {
                                    if (subItem != null)
                                    {
                                        var subScoreMeta =
                                            virtualTestCustomSubScores.FirstOrDefault(x => x.Name == subItem.Name);
                                        var testresultSubScore = new ResultEntryTestResultSubScore()
                                        {
                                            TestResultSubScoreID = subItem.TestResultScoreSubID,
                                            StudentID = item.TestResultScore.StudentID,
                                            VirtualTestID = item.TestResultScore.VirtualTestID,
                                            ClassID = item.TestResultScore.ClassID,
                                            Name = subItem.Name,
                                            ScoreRaw = subItem.ScoreRaw,
                                            ScoreScaled = subItem.ScoreScaled,
                                            ScorePercent = subItem.ScorePercent,
                                            ScorePercentage = subItem.ScorePercentage,
                                            ScoreCustomN_1 = subItem.ScoreCustomN_1,
                                            ScoreCustomN_2 = subItem.ScoreCustomN_2,
                                            ScoreCustomN_3 = subItem.ScoreCustomN_3,
                                            ScoreCustomN_4 = subItem.ScoreCustomN_4,
                                            ScoreCustomA_1 = subItem.ScoreCustomA_1,
                                            ScoreCustomA_2 = subItem.ScoreCustomA_2,
                                            ScoreCustomA_3 = subItem.ScoreCustomA_3,
                                            ScoreCustomA_4 = subItem.ScoreCustomA_4,

                                            UseRaw = subScoreMeta != null && subScoreMeta.UseRaw,
                                            UsePercent = subScoreMeta != null && subScoreMeta.UsePercent,
                                            UsePercentage = subScoreMeta != null && subScoreMeta.UsePercentile,
                                            UseScaled = subScoreMeta?.UseScaled ?? false,
                                            AchievementLevel =
                                                subScoreMeta != null ? subScoreMeta.AchievementLevelSettingId : 0,
                                            PointsPossible = subScoreMeta != null ? subScoreMeta.PointsPossible : 0,
                                            UseArtifact = subScoreMeta != null ? subScoreMeta.UseArtifact : false,
                                        };

                                        testresultSubScoreList.Add(testresultSubScore);

                                        // subscore notes
                                        if (subItem.Notes != null)
                                        {
                                            testResultSubScoreNoteList.AddRange(BuildTestResultSubScoreNoteViewModel(item.TestResultScore, subItem));
                                        }

                                        if (subItem.Artifacts != null && subItem.Artifacts.Any())
                                        {
                                            testResultSubScoreUploadFile.AddRange(subItem.Artifacts.Select(art => new TestResultSubScoreUploadFileMapModel
                                            {
                                                FileName = art.IsLink ? art.Name : null,
                                                IsUrl = art.IsLink,
                                                TestResultSubScoreID = subItem.TestResultScoreSubID,
                                                UploadDate = art.UploadDate,
                                                VirtualTestID = item.TestResultScore.VirtualTestID ?? 0,
                                                StudentID = item.TestResultScore.StudentID,
                                                ClassID = item.TestResultScore.ClassID ?? 0,
                                                SubScoreName = subItem.Name,
                                                Tag = art.TagValue,
                                                CreatedBy = art.CreatedBy ?? CurrentUser.Id,
                                                DocumentGuid = art.DocumentGuid
                                            }).ToList());

                                            updatedDocumentGuids.AddRange(subItem.Artifacts.Select(p => p.DocumentGuid));
                                        }
                                    }
                                }
                            }
                        }
                        else if (item.TestResultScore.TestResultID.HasValue && item.TestResultScore.TestResultID.Value > 0)
                        {
                            testresultIdDelete.Add(item.TestResultScore.TestResultID.Value);
                        }
                    }

                    var testResultScoreIds = scores.Where(p => p.TestResultScore?.TestResultScoreID > 0).Select(p => p.TestResultScore.TestResultScoreID);
                    var testResultSubScoreIds = scores.Where(p => p.TestResultSubScores != null).SelectMany(p => p.TestResultSubScores).Select(s => s.TestResultScoreSubID).Where(p => p > 0);
                    _parameters.DataLockerService.DeleteDocumentByTestResult(testResultScoreIds, testResultSubScoreIds, updatedDocumentGuids);

                    _parameters.DataLockerService.SaveEntryResultsMultiple(
                            XmlUtils.BuildXml(testresultList),
                            XmlUtils.BuildXml(testresultScoreList),
                            XmlUtils.BuildXml(testresultSubScoreList),
                            XmlUtils.BuildXml(testResultScoreNoteList),
                            XmlUtils.BuildXml(testResultSubScoreNoteList),
                            XmlUtils.BuildXml(testResultScoreUploadFile),
                            XmlUtils.BuildXml(testResultSubScoreUploadFile),
                            string.Join(",", testresultIdDelete));

                    //delete all autosave
                    _parameters.DataLockerService.DeleteAllAutoSaveDataBaseDate(virtualtestId.GetValueOrDefault(), classId.GetValueOrDefault(), dateSave);
                    return Json(new { Success = true, JsonRequestBehavior.AllowGet });
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    return Json(new { Success = false, Error = ex.Message.ToString(), JsonRequestBehavior.AllowGet });
                }
            }

            return Json(new { Success = true, JsonRequestBehavior.AllowGet });
        }

        private List<TestResultScoreNoteViewModel> BuildTestResultScoreNoteViewModel(DTLStudentAndTestResultScore testResultScore)
        {
            var result = new List<TestResultScoreNoteViewModel>();
            if (testResultScore != null && testResultScore.Notes != null)
            {
                foreach (var item in testResultScore.Notes)
                {
                    if (!string.IsNullOrEmpty(item.Note) && item.NoteContents.Notes.Any())
                    {
                        result.Add(new TestResultScoreNoteViewModel()
                        {
                            Name = item.Name,
                            Note = item.Note,
                            NoteKey = item.NoteKey,
                            TestResultScoreID = item.TestResultScoreID,
                            TestResultScoreNoteID = item.TestResultScoreNoteID,
                            ClassID = testResultScore.ClassID ?? 0,
                            StudentID = testResultScore.StudentID,
                            VirtualTestID = testResultScore.VirtualTestID ?? 0
                        });
                    }                   
                }
            }
            return result;
        }

        private List<TestResultSubScoreNoteViewModel> BuildTestResultSubScoreNoteViewModel(DTLStudentAndTestResultScore testResultScore, DTLStudentAndTestResultSubScore subScore)
        {
            var result = new List<TestResultSubScoreNoteViewModel>();
            if (testResultScore != null && subScore != null && subScore.Notes != null)
            {
                foreach (var item in subScore.Notes)
                {
                    if (!string.IsNullOrEmpty(item.Note) && item.NoteContents.Notes.Any())
                    {
                        result.Add(new TestResultSubScoreNoteViewModel()
                        {
                            Name = item.Name,
                            Note = item.Note,
                            NoteKey = item.NoteKey,
                            ClassID = testResultScore.ClassID ?? 0,
                            StudentID = testResultScore.StudentID,
                            VirtualTestID = testResultScore.VirtualTestID ?? 0,
                            TestResultSubScoreID = item.TestResultSubScoreID,
                            TestResultSubScoreNoteID = item.TestResultSubScoreNoteID,
                            SubScoreName = subScore.Name
                        });
                    }                 
                }
            }
            return result;
        }


        [HttpPost]
        public ActionResult GeneratePdf(ResultEntryPrintModel printModel)
        {
            var virtualTest = _parameters.VirtualTestService.GetTestById(printModel.VirtualtestId);
            if (virtualTest == null)
            {
                return Json(new { Success = false, ErrorMessage = "There is no virtual test!" });
            }

            var model = BuildResultEntryPrintingModel(printModel);
            var classObject = _parameters.ClassService.GetClassById(printModel.ClassId);
            model.TestTitle = virtualTest.Name;
            model.RubricDescription = virtualTest.Instruction.ReplaceWeirdCharacters();
            model.ClassName = classObject.Name;

            var mapPath = HttpContext.Server.MapPath("~/");
            model.Css = new List<string>();
            model.Css.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Content/themes/Print/DataLockerEntryResult/DataLockerEntryResult.css")));
            var html = this.RenderRazorViewToString("EntryResultPDFTemplate", model);

            var pdfGeneratorModel = new PdfGeneratorModel()
            {
                Html = html,
                FileName = model.TestTitle + Guid.NewGuid().ToString().Substring(0, 8),
                Folder = "DataLocker"
            };
            var pdfData = InvokePdfGeneratorService(pdfGeneratorModel);

            return Json(pdfData, JsonRequestBehavior.AllowGet);
        }

        [UploadifyPrincipal(Order = 1)]
        [HttpPost]
        public ActionResult UploadArtifactDataFile(HttpPostedFileBase postedFile)
        {
            try
            {
                var fileName = postedFile.FileName.AddTimestampToFileName();
                var fileNamePath = string.Format("{0}/{1}", DTLFolder.RemoveEndSlash(), fileName);
                var s3Result = _s3Service.UploadRubricFile(DTLBucket, fileNamePath, postedFile.InputStream, false);
                if (s3Result.IsSuccess)
                {
                    return Json(new { Success = true, FileName = fileName, fileNameUrl = GetS3Url(fileName) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Success = false, ErrorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public string JSPath(string fileName)
        {
            var path = HttpContext.Server.MapPath("~/Scripts/");
            var result = Path.Combine(path, fileName);
            return result;
        }

        private ResultEntryDataPrintModel BuildResultEntryPrintingModel(ResultEntryPrintModel printModel)
        {
            var model = new ResultEntryDataPrintModel();
            var virtualTestCustomScore = _parameters.DataLockerService.GetVirtualTestCustomScore(printModel.VirtualtestId);
            var virtualTestCustomMetaDatas = _parameters.DataLockerService.GetVirtualTestCustomMetaDataByVirtualTestCustomScoreId(virtualTestCustomScore.VirtualTestCustomScoreId);
            if (virtualTestCustomScore != null)
            {
                model.CustomScore = new ResultEntryCustomScore
                {
                    Name = virtualTestCustomScore.Name,
                    VirtualTestCustomScoreId = virtualTestCustomScore.VirtualTestCustomScoreId,
                    ScoreInfos = GetVirtualTestCustomScoreInfo(virtualTestCustomScore, virtualTestCustomMetaDatas)
                };
            }

            var virtualTestCustomSubScores = _parameters.DataLockerService.GetVirtualTestCustomSubScores(virtualTestCustomScore.VirtualTestCustomScoreId)
                .OrderBy(o => o.Sequence);
            model.CustomSubScores = new List<ResultEntryCustomSubScore>();
            foreach (var virtualTestCustomSubScore in virtualTestCustomSubScores)
            {
                var virtualTestCustomSubMetaDatas = _parameters.DataLockerService.GetVirtualTestCustomMetaDataByVirtualTestCustomSubScoreID(virtualTestCustomSubScore.VirtualTestCustomSubScoreId);
                model.CustomSubScores.Add(new ResultEntryCustomSubScore
                {
                    Name = virtualTestCustomSubScore.Name,
                    VirtualTestCustomSubScoreId = virtualTestCustomSubScore.VirtualTestCustomSubScoreId,
                    ScoreInfos = GetVirtualTestCustomScoreInfo(virtualTestCustomSubScore, virtualTestCustomSubMetaDatas)
                });
            }
            model.StudentTestResultScores = _parameters.DataLockerService.GetStudentAndTestResultScoreMultiple(printModel.VirtualtestId, printModel.ClassId, printModel.StudentsIdSelectedString, printModel.EntryResultDate)
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
            model.StudentTestResultSubScores = _parameters.DataLockerService.GetStudentAndTestResultSubScoreMultiple(printModel.VirtualtestId, printModel.ClassId, printModel.StudentsIdSelectedString, printModel.EntryResultDate);
            model.StudentTestResultSubScores.ForEach((item) =>
            {
                item.ResultDate = item.ResultDate.HasValue ? DateTime.SpecifyKind(item.ResultDate.Value, DateTimeKind.Utc) : (DateTime?)null;
            });
            model.StudentTestResultScores.ForEach((item) =>
            {
                item.ResultDate = item.ResultDate.HasValue ? DateTime.SpecifyKind(item.ResultDate.Value, DateTimeKind.Utc) : (DateTime?)null;
            });
            RebuildStudentTestResultSubScores(model);

            model.AllColumn = printModel.AllColumn;
            model.OverrallScoreNameList = printModel.OverrallScoreNameList;
            model.SubScorePartList = printModel.SubScorePartList;
            model.Layout = printModel.Layout;
            model.ScoreDescription = printModel.ScoreDescription;
            model.IncludeRubricDescription = printModel.IncludeRubricDescription;
            model.DateFormatPrint = _parameters.DistrictDecodeService.GetDateFormat(CurrentUser.DistrictId ?? 0).HandsonTableDateFormat;

            model.JS = new List<string>();
            model.JS.Add(System.IO.File.ReadAllText(JSPath("moment.min.js")));
            model.JS.Add(System.IO.File.ReadAllText(JSPath("Lib/ramda.min.js")));
            model.JS.Add(System.IO.File.ReadAllText(JSPath("DataLockerEntryResult/DataLockerEntryResultPrint.js")));

            return model;
        }

        private void RebuildStudentTestResultSubScores(ResultEntryDataPrintModel model)
        {
            // model.CustomSubScores
            //model.StudentTestResultSubScores
            if (model.CustomSubScores != null && model.CustomSubScores.Any())
            {
                var studentIds = model.StudentTestResultSubScores.Select(x => x.StudentID).Distinct().ToList();

                foreach (var studentId in studentIds)
                {
                    if (model.StudentTestResultSubScores.Count(x => x.StudentID == studentId && string.IsNullOrEmpty(x.Name)) == 1)
                    {
                        var dtlStudentAndTestResultSubScore = model.StudentTestResultSubScores.FirstOrDefault(x => x.StudentID == studentId);
                        if (dtlStudentAndTestResultSubScore != null)
                        {
                            var resultEntryCustomSubScore = model.CustomSubScores.FirstOrDefault();
                            if (resultEntryCustomSubScore != null)
                                dtlStudentAndTestResultSubScore.Name = resultEntryCustomSubScore.Name;
                        }
                    }

                    foreach (var customSubScore in model.CustomSubScores)
                    {
                        if (!model.StudentTestResultSubScores.Any(x => x.StudentID == studentId && x.Name == customSubScore.Name))
                        {
                            var studentInfo = model.StudentTestResultSubScores.FirstOrDefault(x => x.StudentID == studentId);
                            model.StudentTestResultSubScores.Add(new DTLStudentAndTestResultSubScore
                            {
                                StudentID = studentInfo.StudentID,
                                FirstName = studentInfo.FirstName,
                                LastName = studentInfo.LastName,
                                MiddleName = studentInfo.MiddleName,
                                ResultDate = studentInfo.ResultDate,
                                Name = customSubScore.Name
                            });
                        }
                    }
                }
            }
        }

        private bool IsValidData(ResultEntrySaveScoreModel item)
        {
            var valid = false;
            var trScore = item.TestResultScore;
            bool hasNote = trScore.Notes != null && trScore.Notes.Any(x => !string.IsNullOrEmpty(x.Note));
            bool hasArtifact = trScore.Artifacts != null && trScore.Artifacts.Any(x => !string.IsNullOrEmpty(x.Name));

            if (trScore != null &&
                (trScore.ScoreRaw.HasValue || trScore.ScoreScaled.HasValue
                || trScore.ScorePercent.HasValue || trScore.ScorePercentage.HasValue
                || !string.IsNullOrEmpty(trScore.ScoreCustomA_1)
                || !string.IsNullOrEmpty(trScore.ScoreCustomA_2)
                || !string.IsNullOrEmpty(trScore.ScoreCustomA_3)
                || !string.IsNullOrEmpty(trScore.ScoreCustomA_4)
                || trScore.ScoreCustomN_1.HasValue || trScore.ScoreCustomN_2.HasValue
                || trScore.ScoreCustomN_3.HasValue || trScore.ScoreCustomN_4.HasValue
                || hasNote || hasArtifact
                ))
            {
                valid = true;
            }

            if (item.TestResultSubScores != null)
            {

                foreach (var subItem in item.TestResultSubScores)
                {
                    bool hasSubNote = subItem.Notes != null && subItem.Notes.Any(x => !string.IsNullOrEmpty(x.Note));
                    bool hasSubArtifact = subItem.Artifacts != null && subItem.Artifacts.Any(x => !string.IsNullOrEmpty(x.Name));

                    if (subItem != null &&
                        (subItem.ScoreRaw.HasValue || subItem.ScoreScaled.HasValue
                        || subItem.ScorePercent.HasValue || subItem.ScorePercentage.HasValue
                        || !string.IsNullOrEmpty(subItem.ScoreCustomA_1)
                        || !string.IsNullOrEmpty(subItem.ScoreCustomA_2)
                        || !string.IsNullOrEmpty(subItem.ScoreCustomA_3)
                        || !string.IsNullOrEmpty(subItem.ScoreCustomA_4)
                        || subItem.ScoreCustomN_1.HasValue || subItem.ScoreCustomN_2.HasValue
                        || subItem.ScoreCustomN_3.HasValue || subItem.ScoreCustomN_4.HasValue
                        || hasSubNote || hasSubArtifact
                        ))
                    {
                        valid = true;
                        break;
                    }
                }
            }
            return valid;
        }
        private string InvokePdfGeneratorService(PdfGeneratorModel model)
        {
            var pdfUrl = PdfGeneratorConsumer.InvokePdfGeneratorService(model.Html, model.FileName, model.Folder, CurrentUser.UserName);

            if (string.IsNullOrWhiteSpace(pdfUrl)) return string.Empty;

            var downloadPdfData = new DownloadPdfData { FilePath = pdfUrl, UserID = CurrentUser.Id, CreatedDate = DateTime.UtcNow };
            _parameters.DownloadPdfService.SaveDownloadPdfData(downloadPdfData);
            var downLoadUrl = Url.Action("Index", "DownloadPdf", new { pdfID = downloadPdfData.DownloadPdfID }, Request.Url.Scheme);

            return downLoadUrl;
        }

        private List<ResultEntryScoreModel> GetVirtualTestCustomScoreInfo(VirtualTestCustomScore virtualTestCustomScore, List<VirtualTestCustomMetaData> virtualTestCustomMetaDatas)
        {
            var scoreInfos = new List<ResultEntryScoreModel>();
            if (virtualTestCustomScore.UseRaw)
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "Raw");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "Raw",
                    ScoreLable = "Raw",
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UseScaled)
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "Scaled");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "Scaled",
                    ScoreLable = "Scaled",
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UsePercent)
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "Percent");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "Percent",
                    ScoreLable = "Percent",
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UsePercentile)
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "Percentile");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "Percentile",
                    ScoreLable = "Percentile",
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UseCustomN1.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomN_1");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomN_1",
                    ScoreLable = virtualTestCustomScore.CustomN1Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UseCustomN2.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomN_2");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomN_2",
                    ScoreLable = virtualTestCustomScore.CustomN2Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UseCustomN3.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomN_3");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomN_3",
                    ScoreLable = virtualTestCustomScore.CustomN3Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UseCustomN4.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomN_4");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomN_4",
                    ScoreLable = virtualTestCustomScore.CustomN4Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UseCustomA1.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomA_1");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomA_1",
                    ScoreLable = virtualTestCustomScore.CustomA1Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UseCustomA2.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomA_2");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomA_2",
                    ScoreLable = virtualTestCustomScore.CustomA2Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UseCustomA3.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomA_3");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomA_3",
                    ScoreLable = virtualTestCustomScore.CustomA3Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UseCustomA4.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomA_4");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomA_4",
                    ScoreLable = virtualTestCustomScore.CustomA4Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UseArtifact.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "Artifact");
                var assessmentArtifactFileTypeGroups = Mapper.Map<IEnumerable<EntryResultArtifactFileTypeGroupViewModel>>
               (_parameters.DistrictDecodeService
                   .GetAssessmentArtifactFileTypeGroups(CurrentUser.DistrictId ?? 0));
                metaData.EntryResultArtifactFileTypeGroupViewModel = assessmentArtifactFileTypeGroups.ToList();
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "Artifact",
                    ScoreLable = "Artifact",
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomScore.UseNote.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, VirtualTestCustomMetaData.NOTE_COMMENT);
                if (metaData != null)
                {
                    int count = 0;
                    if (metaData.ListNoteComment != null)
                    {
                        foreach (var note in metaData.ListNoteComment)
                        {
                            count = count + 1;
                            var meta = new VirtualTestCustomMetaModel();
                            meta.DefaultValue = note.DefaultValue;
                            meta.Description = note.Description;
                            meta.ListNoteComment = null;
                            meta.NoteType = note.NoteType;
                            scoreInfos.Add(new ResultEntryScoreModel
                            {
                                ScoreName = string.Format("note_{0}", count),
                                ScoreLable = note.NoteName,
                                MetaData = meta,
                                Order = note.Order.GetValueOrDefault()
                            });
                        }
                    }

                }
            }

            scoreInfos = scoreInfos.OrderBy(x => x.Order).ToList();
            return scoreInfos;
        }

        private List<ResultEntryScoreModel> GetVirtualTestCustomScoreInfo(VirtualTestCustomSubScore virtualTestCustomSubScore, List<VirtualTestCustomMetaData> virtualTestCustomMetaDatas)
        {
            var scoreInfos = new List<ResultEntryScoreModel>();
            if (virtualTestCustomSubScore.UseRaw)
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "Raw");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "Raw",
                    ScoreLable = "Raw",
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UseScaled)
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "Scaled");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "Scaled",
                    ScoreLable = "Scaled",
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UsePercent)
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "Percent");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "Percent",
                    ScoreLable = "Percent",
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UsePercentile)
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "Percentile");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "Percentile",
                    ScoreLable = "Percentile",
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UseCustomN1.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomN_1");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomN_1",
                    ScoreLable = virtualTestCustomSubScore.CustomN1Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UseCustomN2.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomN_2");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomN_2",
                    ScoreLable = virtualTestCustomSubScore.CustomN2Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UseCustomN3.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomN_3");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomN_3",
                    ScoreLable = virtualTestCustomSubScore.CustomN3Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UseCustomN4.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomN_4");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomN_4",
                    ScoreLable = virtualTestCustomSubScore.CustomN4Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UseCustomA1.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomA_1");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomA_1",
                    ScoreLable = virtualTestCustomSubScore.CustomA1Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UseCustomA2.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomA_2");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomA_2",
                    ScoreLable = virtualTestCustomSubScore.CustomA2Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UseCustomA3.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomA_3");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomA_3",
                    ScoreLable = virtualTestCustomSubScore.CustomA3Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UseCustomA4.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "CustomA_4");
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "CustomA_4",
                    ScoreLable = virtualTestCustomSubScore.CustomA4Label,
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UseArtifact.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, "Artifact");
                var assessmentArtifactFileTypeGroups = Mapper.Map<IEnumerable<EntryResultArtifactFileTypeGroupViewModel>>
              (_parameters.DistrictDecodeService
                  .GetAssessmentArtifactFileTypeGroups(CurrentUser.DistrictId ?? 0));
                metaData.EntryResultArtifactFileTypeGroupViewModel = assessmentArtifactFileTypeGroups.ToList();
                scoreInfos.Add(new ResultEntryScoreModel
                {
                    ScoreName = "Artifact",
                    ScoreLable = "Artifact",
                    MetaData = metaData,
                    Order = metaData.Order.GetValueOrDefault()
                });
            }
            if (virtualTestCustomSubScore.UseNote.GetValueOrDefault())
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, VirtualTestCustomMetaData.NOTE_COMMENT);
                if (metaData != null)
                {
                    int count = 0;
                    if (metaData.ListNoteComment != null)
                    {
                        foreach (var note in metaData.ListNoteComment)
                        {
                            count = count + 1;
                            var meta = new VirtualTestCustomMetaModel();
                            meta.DefaultValue = note.DefaultValue;
                            meta.Description = note.Description;
                            meta.ListNoteComment = null;
                            meta.NoteType = note.NoteType;
                            scoreInfos.Add(new ResultEntryScoreModel
                            {
                                ScoreName = string.Format("note_{0}", count),
                                ScoreLable = note.NoteName,
                                MetaData = meta,
                                Order = note.Order.GetValueOrDefault()
                            });
                        }
                    }
                }
            }
            scoreInfos = scoreInfos.OrderBy(x => x.Order).ToList();
            return scoreInfos;
        }

        private VirtualTestCustomMetaModel GetMetaData(List<VirtualTestCustomMetaData> virtualTestCustomMetaDatas, string scoreType)
        {
            var metaData = virtualTestCustomMetaDatas.Where(x => x.ScoreType == scoreType).FirstOrDefault();
            return metaData != null ? metaData.ParseMetaToObject() : null;
        }

        private string GetS3Url(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;
            var fileNamePath = string.Format("{0}/{1}", DTLFolder.RemoveEndSlash(), fileName);
            var s3Url = _s3Service.GetPublicUrl(DTLBucket, fileNamePath);
            return s3Url;
        }
        public ActionResult LoadBankBySubjectId(FormBankCriteria criteria)
        {
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin)
                criteria.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            criteria.UserId = CurrentUser.Id;
            criteria.UserRole = CurrentUser.RoleId;

            var districtId = CurrentUser.DistrictId.GetValueOrDefault();

            var banks = _parameters.UserBankService.GetFormBanksBySubjectId(criteria).ToList();
            var dataLockerBanks = CheckDataLockerBank(banks);
            var bankOrders = _parameters.BankServices.GetBankOrders(districtId).ToList();

            if (!bankOrders.Any())
                return Json(dataLockerBanks, JsonRequestBehavior.AllowGet);

            var result = _parameters.BankServices.SortBanks(dataLockerBanks, bankOrders);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetResultDates(int? virtualTestId, int? classId, int? districtId)
        {
            if (!districtId.HasValue || districtId <= 0)
                districtId = CurrentUser.DistrictId;
            var dateFormatModel = _parameters.DistrictDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId.GetValueOrDefault(), Constanst.DateFormat);
            var dateFormat = (dateFormatModel == null || string.IsNullOrEmpty(dateFormatModel.Value)) ? Constanst.DefaultDateFormatValue : dateFormatModel.Value;
            var data = _parameters.TestResultService.GetResultDates(virtualTestId.GetValueOrDefault(), classId.GetValueOrDefault()).OrderByDescending(x => x)
                .AsEnumerable().Select(x => new ListItemStr() { Id = x.ToShortDateString(), Name = x.ToString(dateFormat) }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        private List<ListItem> CheckDataLockerBank(List<ListItem> banks)
        {
            var bankIds = _parameters.VirtualTestService.GetDataLockerBank(banks.Select(x => x.Id).ToList());
            var bankList = banks.Where(x => bankIds.Contains(x.Id)).ToList();
            return bankList;
        }
    }
}
