using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using LinkIt.BubbleService.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Models.SGO;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels.SGO;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.Helpers.SGO;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class SGOProgressMonitorController : BaseController
    {
        private readonly SGOProgressMonitorControllerParameters _parameters;
        private List<int> excludedGroupIds = new List<int> { 98, 99 };

        private readonly IS3Service _s3Service;

        public SGOProgressMonitorController(SGOProgressMonitorControllerParameters parameters, IS3Service s3Service)
        {
            _parameters = parameters;
            _s3Service = s3Service;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemSGOManager)]
        public ActionResult Index(int sgoId)
        {
            var objSgo = _parameters.SgoObjectService.GetSGOByID(sgoId);
            if (objSgo == null)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            var vPermissionAccess = _parameters.SgoObjectService.GetPermissionAccessSgoProPressMonitoring(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status == (int)SGOPermissionEnum.NotAvalible)
            {
                return RedirectToAction("Index", "SGOManage");
            }

            var validatePostAssessmentLinkit = _parameters.SgoDataPointService.ValidatePostAssessmentLinkit(sgoId);
            var model = new SGOMonitorScoreViewModel()
            {
                PermissionAccess = vPermissionAccess.Status,
                SgoId = sgoId,
                ApproverUserID = objSgo.ApproverUserID.GetValueOrDefault(),
                EducatorComments = objSgo.EducatorComment,
                IsReviewer = objSgo.ApproverUserID == CurrentUser.Id,
                IsSaveResultScore = _parameters.SgoGroupService.CheckSavedResultScore(sgoId),
                DirectionConfigurationValue = GetDistrictDecodeValue(objSgo.DistrictID, Util.DistrictDecode_SGOProgressMonitorAndScoreDirection),
                SgoType = objSgo.Type,
                HavePostAssessmentLinkit = validatePostAssessmentLinkit > 0,
                IsPostAssessmentLinkitHasTestResult = validatePostAssessmentLinkit == 1,
                AttachUnstructuredUrl = objSgo.AttachUnstructuredProgressUrl,
                AttachUnstructuredDownloadUrl = string.IsNullOrEmpty(objSgo.AttachUnstructuredProgressUrl)
                        ? ""
                        : GetLinkToDownloadAttachment(objSgo.AttachUnstructuredProgressUrl)
            };

            if (objSgo.GenerateResultDate.HasValue)
            {
                model.GeneratedDate = objSgo.GenerateResultDate.Value.ToString("F");
            }
            return View(model);
        }

        private string GetDistrictDecodeValue(int districtId, string label)
        {
            var districtDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, label).FirstOrDefault();
            if (districtDecode != null)
                return districtDecode.Value;

            return "";
        }

        public ActionResult LoadSgoResultContent(int sgoId)
        {
            var objSgo = _parameters.SgoObjectService.GetSGOByID(sgoId);
            if (objSgo == null)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            var vPermissionAccess = _parameters.SgoObjectService.GetPermissionAccessSgoProPressMonitoring(CurrentUser.Id, objSgo);

            var validatePostAssessmentLinkit = _parameters.SgoDataPointService.ValidatePostAssessmentLinkit(sgoId);

            var isExistStudentHasScoreNull = _parameters.SgoDataPointService.IsExistStudentHasScoreNull(sgoId, 0);

            var model = new SGOProgressMonitorViewModel
            {
                SgoId = sgoId,
                PermissionAccess = vPermissionAccess.Status,
                SgoGroups = GetSgoGroup(sgoId).OrderBy(x => x.Order).ToList(),
                DescriptiveLabel = string.Empty,
                TotalTeacherSGOScore = -1,
                HavePostAssessmentLinkit = validatePostAssessmentLinkit > 0,
                IsPostAssessmentLinkitHasTestResult = validatePostAssessmentLinkit == 1,
                IsPreOrPostAssessmentHasScoreNull = isExistStudentHasScoreNull
            };
            var lstAttainmentGoal = GetSgoAttainmentGoal(sgoId) ?? new List<SGOAttainmentGoal>();
            if (model.SgoGroups.Count > 0)
            {
                model.SgoCalculateScoreResults = new List<SGOCalculateScoreResult>();
                foreach (var group in model.SgoGroups)
                {
                    var vGoal = lstAttainmentGoal.FirstOrDefault(o => o.Order == (int)group.TeacherSGOScore.GetValueOrDefault());
                    string strAttainmentGoal = vGoal == null ? string.Empty : vGoal.Name;
                    if (group.PercentStudentAtTargetScore.HasValue)
                    {
                        model.SgoCalculateScoreResults.Add(new SGOCalculateScoreResult()
                        {
                            SGOGroupID = group.SGOGroupID,
                            Name = group.Name,
                            PercentStudentAtTargetScore = group.PercentStudentAtTargetScore.GetValueOrDefault(),
                            TeacherScore = group.TeacherSGOScore.GetValueOrDefault(),
                            Weight = group.Weight.GetValueOrDefault(),
                            WeightedScore = group.TeacherSGOScore.GetValueOrDefault() * group.Weight.GetValueOrDefault(),
                            AttainmentGoal = strAttainmentGoal
                        });
                    }
                }
            }

            InitSgoResultContent(model, lstAttainmentGoal);
            return PartialView("_SgoResultContent", model);
        }

        public ActionResult IsPreOrPostAssessmentHasResult(int sgoId, int virtualTestId)
        {
            bool isSGOHasResult = false;

            isSGOHasResult = _parameters.SgoDataPointService.CheckScoreTestResultStudent(sgoId, virtualTestId);
            if (isSGOHasResult)
            {
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult IsPreOrPostAssessmentHasStudentNoScore(int sgoId, int? sgoDataPointId)
        {
            var isExistStudentHasScoreNull = _parameters.SgoDataPointService.IsExistStudentHasScoreNull(sgoId, sgoDataPointId);
            return Json(new { Result = isExistStudentHasScoreNull }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadUnstructuredSgoResultContent(int sgoId)
        {
            var objSgo = _parameters.SgoObjectService.GetSGOByID(sgoId);
            if (objSgo == null)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            var vPermissionAccess = _parameters.SgoObjectService.GetPermissionAccessSgoProPressMonitoring(CurrentUser.Id, objSgo);

            var model = new SGOProgressMonitorViewModel
            {
                SgoId = sgoId,
                PermissionAccess = vPermissionAccess.Status,
                SgoGroups = _parameters.SgoGroupService.GetGroupWithOut9899BySgoID(sgoId).OrderBy(x => x.Order).ToList(),
                TotalTeacherSGOScoreCustom = objSgo.TotalTeacherSGOScoreCustom,
            };

            if (objSgo.GenerateResultDate.HasValue)
            {
                model.GeneratedDate = objSgo.GenerateResultDate.Value.ToString("F");
            }

            return PartialView("_SgoUnstructuredResultContent", model);
        }

        public ActionResult LoadSetUnstructuredTargetScore(int sgoId)
        {
            var objSgo = _parameters.SgoObjectService.GetSGOByID(sgoId);
            if (objSgo == null)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            var vPermissionAccess = _parameters.SgoObjectService.GetPermissionAccessSgoProPressMonitoring(
                CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate)
            {
                return RedirectToAction("Index", "SGOManage");
            }

            var sgoGroups = _parameters.SgoGroupService.GetGroupWithOut9899BySgoID(sgoId).ToList();
            ViewBag.TotalTeacherSGOSCoreCustom = objSgo.TotalTeacherSGOScoreCustom;

            return PartialView("_SetUnstructuredTargetScore", sgoGroups);
        }

        [SGOManagerLogFilter]
        public ActionResult SetTeacherScoreCustom(int sgoId, string sgoGroupData, string totalTeacherSGOScoreCustom)
        {
            var objSgo = _parameters.SgoObjectService.GetSGOByID(sgoId);
            if (objSgo == null)
            {
                return Json(new { result = false });
            }

            var vPermissionAccess = _parameters.SgoObjectService.GetPermissionAccessSgoProPressMonitoring(
                CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate)
            {
                return Json(new { result = false });
            }

            var js = new JavaScriptSerializer();
            var sgoGroups = js.Deserialize<List<SGOGroup>>(sgoGroupData).ToList();
            var sgoGroupEntities = _parameters.SgoGroupService.GetGroupWithOut9899BySgoID(sgoId);
            foreach (var sgoGroup in sgoGroups)
            {
                var sgoGroupEntity = sgoGroupEntities.FirstOrDefault(x => x.SGOGroupID == sgoGroup.SGOGroupID);
                if (sgoGroupEntity != null)
                {
                    sgoGroupEntity.TeacherSGOScoreCustom = sgoGroup.TeacherSGOScoreCustom;
                    _parameters.SgoGroupService.Save(sgoGroupEntity);
                }
            }

            objSgo.GenerateResultDate = DateTime.UtcNow;
            try
            {
                objSgo.TotalTeacherSGOScoreCustom = totalTeacherSGOScoreCustom;
                _parameters.SgoObjectService.Save(objSgo);
            }
            catch
            {
                //nothing
            }

            return Json(new { result = true });
        }

        private SGOProgressMonitorViewModel InitSgoResultContent(SGOProgressMonitorViewModel model, List<SGOAttainmentGoal> lstAttainmentGoal)
        {
            var vSgo = _parameters.SgoObjectService.GetSGOByID(model.SgoId);
            if (vSgo != null && vSgo.GenerateResultDate.HasValue)
            {
                model.GeneratedDate = vSgo.GenerateResultDate.Value.ToString("F");
            }
            if (model.SgoCalculateScoreResults != null && model.SgoCalculateScoreResults.Any())
            {
                var vTotalTeacherSGOScore = Math.Round(model.SgoCalculateScoreResults.Sum(x => x.WeightedScore).GetValueOrDefault(), 3);
                model.TotalTeacherSGOScore = vTotalTeacherSGOScore;
                model.DescriptiveLabel = string.Empty;
                lstAttainmentGoal = lstAttainmentGoal.OrderByDescending(o => o.Order).ToList();
                foreach (var att in lstAttainmentGoal)
                {
                    if (vTotalTeacherSGOScore >= att.Order)
                    {
                        model.DescriptiveLabel = att.Name;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(model.DescriptiveLabel))
                {
                    var vMinAttainmentGoal =
                        lstAttainmentGoal.FirstOrDefault(o => o.Order == lstAttainmentGoal.Min(v => v.Order));
                    if (vMinAttainmentGoal != null && vMinAttainmentGoal.Order >= vTotalTeacherSGOScore)
                    {
                        model.DescriptiveLabel = vMinAttainmentGoal.Name;
                    }
                }
            }
            return model;
        }

        [SGOManagerLogFilter]
        public ActionResult CalculateSgoResult(int sgoId, int? sgoDataPointId)
        {
            // Do not restrict having post assessment datapoints when using Monitor progress function (just calcalate without saving data to database)
            // Validate when not calculate Monitor Progress function
            if (!sgoDataPointId.HasValue)
            {
                var redirectToAction = ValidateForCalculateSgoResult(sgoId);

                if (redirectToAction != null)
                    return redirectToAction;
            }

            var model = new SGOProgressMonitorViewModel
            {
                SgoId = sgoId,
                SgoGroups = GetSgoGroup(sgoId).OrderBy(x => x.Order).ToList(),
            };

            var sgoObject = _parameters.SgoObjectService.GetSGOByID(sgoId);

            bool isHavePosstAssessment = false;
            var dataPointId = 0;
            if (sgoDataPointId.HasValue)
            {
                dataPointId = sgoDataPointId.Value;
            }
            else
            {
                SGODataPoint postAssessmentDataPoint = null;

                postAssessmentDataPoint =
                       _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                           .FirstOrDefault(x => x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom
                           || x.Type == (int)SGODataPointTypeEnum.PostAssessmentExternal
                           || x.Type == (int)SGODataPointTypeEnum.PostAssessmentHistorical
                           || x.Type == (int)SGODataPointTypeEnum.PostAssessment);

                if (postAssessmentDataPoint != null)
                {
                    isHavePosstAssessment = true;
                    dataPointId = postAssessmentDataPoint.SGODataPointID;
                    _parameters.SgoObjectService.UpdateGenerateResultDate(sgoId);
                }
            }

            model.SgoCalculateScoreResults = _parameters.SgoObjectService.GetSGOCalculateScoreResult(sgoId, dataPointId);
            if (!sgoDataPointId.HasValue && isHavePosstAssessment)
            {
                UpdateSGOResult(model.SgoCalculateScoreResults);
            }

            if (sgoDataPointId.HasValue)
            {
                RemoveTempDataPoint(sgoDataPointId.Value);
                _parameters.SGOAuditTrailService.AddSGOAuditTrail(
                    new AddSGOAuditTrailsDTO
                    {
                        ActionDetail = "Monitored Progress",
                        ChangedByUserID = CurrentUser.Id,
                        SGOActionTypeID = 11,
                        SGOID = sgoId
                    });
            }
            else
            {
                _parameters.SGOAuditTrailService.AddSGOAuditTrail(
                    new AddSGOAuditTrailsDTO
                    {
                        ActionDetail = "Scored SGO",
                        ChangedByUserID = CurrentUser.Id,
                        SGOActionTypeID = 12,
                        SGOID = sgoId
                    });
            }


            if (!sgoDataPointId.HasValue && !isHavePosstAssessment)
                model.SgoCalculateScoreResults.Clear();

            model.IsHavePosstAssessment = isHavePosstAssessment;

            var lstAttainmentGoal = GetSgoAttainmentGoal(sgoId) ?? new List<SGOAttainmentGoal>();
            foreach (var item in model.SgoCalculateScoreResults)
            {
                var vGoal = lstAttainmentGoal.FirstOrDefault(o => o.Order == (int)item.TeacherScore.GetValueOrDefault());
                if (vGoal != null)
                    item.AttainmentGoal = vGoal.Name;
            }

            var vPermissionAccess = _parameters.SgoObjectService.GetPermissionAccessSgoProPressMonitoring(CurrentUser.Id, sgoObject);
            model.PermissionAccess = vPermissionAccess.Status;

            var validatePostAssessmentLinkit = _parameters.SgoDataPointService.ValidatePostAssessmentLinkit(sgoId);
            model.HavePostAssessmentLinkit = validatePostAssessmentLinkit > 0;
            model.IsPostAssessmentLinkitHasTestResult = validatePostAssessmentLinkit == 1;
            model.IsPreOrPostAssessmentHasScoreNull = _parameters.SgoDataPointService.IsExistStudentHasScoreNull(sgoId, 0);
            InitSgoResultContent(model, lstAttainmentGoal);
            return PartialView("_SgoResultContent", model);
        }

        private ActionResult ValidateForCalculateSgoResult(int sgoId)
        {
            var sgoObject = _parameters.SgoObjectService.GetSGOByID(sgoId);
            var vPermissionAccess = _parameters.SgoObjectService.GetPermissionAccessSgoProPressMonitoring(
                CurrentUser.Id,
                sgoObject);
            if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate)
                return RedirectToAction("LoadSgoResultContent", "SGOProgressMonitor", new { sgoId = sgoId });

            var validatePostAssessmentLinkit = _parameters.SgoDataPointService.ValidatePostAssessmentLinkit(sgoId);
            if (validatePostAssessmentLinkit != 1)
                return RedirectToAction("LoadSgoResultContent", "SGOProgressMonitor", new { sgoId = sgoId });

            return null;
        }

        [SGOManagerLogFilter]
        private void UpdateSGOResult(List<SGOCalculateScoreResult> lst)
        {
            if (lst.Any())
            {
                var vList = lst.Select(o => new SGOGroup()
                {
                    SGOGroupID = o.SGOGroupID,
                    PercentStudentAtTargetScore = o.PercentStudentAtTargetScore,
                    TeacherSGOScore = o.TeacherScore,
                    Weight = Math.Round(o.Weight.GetValueOrDefault(), 3)
                }).ToList();
                _parameters.SgoGroupService.UpdateSGOResult(vList);
            }
        }

        private void RemoveTempDataPoint(int sgoDataPointId)
        {
            var temporaryExternalVirtualTestId = 0;
            var sgoDatapoint = _parameters.SgoDataPointService.GetById(sgoDataPointId);
            if (sgoDatapoint.Type == (int)SGODataPointTypeEnum.PreAssessmentExternal)
            {
                temporaryExternalVirtualTestId = sgoDatapoint.VirtualTestID.GetValueOrDefault();
            }

            _parameters.SgoSelectDataPointService.RemoveDataPointRelevantData(sgoDataPointId);
            var sgoDataPointBands = _parameters.SgoDataPointService.GetDataPointBandByDataPointID(sgoDataPointId);
            _parameters.SgoDataPointService.DeleteDataPointBand(sgoDataPointBands);
            _parameters.SgoDataPointService.Delete(new SGODataPoint { SGODataPointID = sgoDataPointId });

            // Remove temporary external virtual test data after showing score
            if (temporaryExternalVirtualTestId > 0)
            {
                _parameters.SgoSelectDataPointService.SGORemoveTemporaryExternalTest(temporaryExternalVirtualTestId);
            }
        }

        public ActionResult LoadMonitorProgress(int sgoId)
        {
            return PartialView("_MonitorProgress");
        }

        [SGOManagerLogFilter]
        public ActionResult LoadScoringDetail(int sgoId, int? sgoDataPointId)
        {
            var model = new ScoringDetailModel();
            model.IsTemporaryScoring = sgoDataPointId.HasValue;
            model.SgoScoringDetails = _parameters.SgoObjectService.GetSgoScoringDetail(sgoId, sgoDataPointId);

            var sgoObject = _parameters.SgoObjectService.GetSGOByID(sgoId);
            model.TargetScoreType = sgoObject.TargetScoreType;

            var sgoDataPoints = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId);
            SGODataPoint sgoDataPoint;

            if (sgoDataPointId.HasValue)
            {
                sgoDataPoint = sgoDataPoints.FirstOrDefault(x => x.SGODataPointID == sgoDataPointId.Value);
                model.PostAssessmentTestName = sgoDataPoint == null ? "" : sgoDataPoint.Name;
            }
            else
            {
                sgoDataPoint =
                        sgoDataPoints.FirstOrDefault(x => x.Type == (int)SGODataPointTypeEnum.PostAssessment
                                                          || x.Type == (int)SGODataPointTypeEnum.PostAssessmentExternal
                                                          || x.Type == (int)SGODataPointTypeEnum.PostAssessmentHistorical
                                                          || x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom);
                model.PostAssessmentTestName = sgoDataPoint == null ? "" : sgoDataPoint.Name;
            }

            //processing to display label/value column for PostAssessment custom, PreAssessmentCustom for in case Monitor
            if (sgoDataPoint != null && (sgoDataPoint.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom || sgoDataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom)
                && (sgoDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomN1 || sgoDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomN2
                || sgoDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomN3 || sgoDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomN4))
            {
                var labelValueDic = GetLabelValueOptions(sgoDataPoint);
                if (labelValueDic.Any())
                {
                    foreach (var scoringDetail in model.SgoScoringDetails)
                    {
                        if (scoringDetail.PostScore != null)
                        {
                            string scoreText;
                            decimal score;
                            decimal.TryParse(scoringDetail.PostScore, out score);
                            labelValueDic.TryGetValue(score, out scoreText);
                            scoringDetail.PostScore = scoreText;
                        }
                    }
                }
            }

            //get PreAssessment
            sgoDataPoint =
                sgoDataPoints.FirstOrDefault(x => x.ImprovementBasedDataPoint == 1);
            model.PreAssessmentTestName = sgoDataPoint == null ? "" : sgoDataPoint.Name;

            model.GroupCount = _parameters.SgoGroupService.GetGroupWithOut9899BySgoID(sgoId).Count();

            //processing to display label/value column for PreAssessment custom
            if (sgoDataPoint != null && sgoDataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom
                && (sgoDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomN1 || sgoDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomN2
                || sgoDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomN3 || sgoDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomN4))
            {
                var labelValueDic = GetLabelValueOptions(sgoDataPoint);
                if (labelValueDic.Any())
                {
                    foreach (var scoringDetail in model.SgoScoringDetails)
                    {
                        if (scoringDetail.BasedScore != null)
                        {
                            string scoreText;
                            decimal score;
                            decimal.TryParse(scoringDetail.BasedScore, out score);
                            labelValueDic.TryGetValue(score, out scoreText);
                            scoringDetail.BasedScore = scoreText;
                        }
                    }
                }
            }

            if (sgoDataPointId.HasValue)
            {
                RemoveTempDataPoint(sgoDataPointId.Value);
            }

            return PartialView("_ScoringDetail", model);
        }

        [SGOManagerLogFilter]
        public ActionResult RemoveTempDataPoint(int? sgoDataPointId)
        {
            if (sgoDataPointId.HasValue)
            {
                RemoveTempDataPoint(sgoDataPointId.Value);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private Dictionary<decimal, string> GetLabelValueOptions(SGODataPoint sgoDataPoint)
        {
            var labelValueDic = new Dictionary<decimal, string>();
            var metaData = new VirtualTestCustomMetaModel();
            //get metadata to check label/value type
            var virtualTestCustomSubScoreId = 0;
            var clusterScore = _parameters.SgoDataPointClusterScoreService.GetDataPointClusterScoreBySGODataPointID(sgoDataPoint.SGODataPointID).FirstOrDefault();
            if (clusterScore != null)
                virtualTestCustomSubScoreId = clusterScore.VirtualTestCustomSubScoreId.GetValueOrDefault();

            if (virtualTestCustomSubScoreId > 0)
            {
                var scoreTypeName = Enum.GetName(typeof(DataLockerScoreTypeEnum), sgoDataPoint.ScoreType);
                var virtualTestCustomSubMetaData = _parameters.DataLockerService.GetVirtualTestCustomMetaDataByVirtualTestCustomSubScoreID(virtualTestCustomSubScoreId).FirstOrDefault(x => x.ScoreType == scoreTypeName);
                if (virtualTestCustomSubMetaData != null)
                    metaData = JsonConvert.DeserializeObject<VirtualTestCustomMetaModel>(virtualTestCustomSubMetaData.MetaData);
            }
            else if (sgoDataPoint.AchievementLevelSettingID.GetValueOrDefault() > 0)
            {
                var scoreTypeName = Enum.GetName(typeof(DataLockerScoreTypeEnum), sgoDataPoint.ScoreType);
                var virtualTestCustomMetaData =
                    _parameters.DataLockerService.GetVirtualTestCustomMetaDataByVirtualTestCustomScoreId(
                        sgoDataPoint.AchievementLevelSettingID.GetValueOrDefault()).FirstOrDefault(x => !x.VirtualTestCustomSubScoreID.HasValue && x.ScoreType == scoreTypeName);
                if (virtualTestCustomMetaData != null)
                    metaData = JsonConvert.DeserializeObject<VirtualTestCustomMetaModel>(virtualTestCustomMetaData.MetaData);
            }

            if (metaData != null && metaData.FormatOption != null && metaData.FormatOption.ToLower() == "labelvaluetext")
            {
                foreach (var selectOption in metaData.SelectListOptions)
                {
                    decimal option;
                    if (decimal.TryParse(selectOption.Option, out option))
                    {
                        if (metaData.DisplayOption == "label")
                        {
                            labelValueDic.Add(option, selectOption.Label);
                        }
                        else if (metaData.DisplayOption == "both")
                        {
                            labelValueDic.Add(option, string.Format("{0} ({1})", selectOption.Label, selectOption.Option));
                        }
                    }
                }
            }
            return labelValueDic;
        }
        [SGOManagerLogFilter]
        public ActionResult UpdateStudentAchievedTarget(int sgoId, List<SGOScoringDetail> sgoScoringDetails)
        {
            var sgoStudents = _parameters.SgoStudentService.GetListStudentBySGOID(sgoId);

            foreach (var sgoScoringDetail in sgoScoringDetails)
            {
                var sgoStudent = sgoStudents.FirstOrDefault(x => x.SGOStudentID == sgoScoringDetail.SgoStudentId);
                if (sgoStudent != null)
                {
                    sgoStudent.ArchievedTarget = sgoScoringDetail.AchievedTarget;
                    _parameters.SgoStudentService.SaveSGOStudent(sgoStudent);
                }
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        private List<SGOGroup> GetSgoGroup(int sgoId)
        {
            var sgoGroups =
                _parameters.SgoGroupService.GetGroupBySgoID(sgoId)
                    .Where(x => !excludedGroupIds.Contains(x.Order))
                    .OrderBy(x => x.Order)
                    .ToList();

            var studentNumberInGroups =
                _parameters.SgoStudentService.GetListStudentBySGOID(sgoId)
                    .GroupBy(x => x.SGOGroupID)
                    .Select(x => new { SgoGroupId = x.Key, NumOfStudent = x.Count() }).ToList();

            foreach (var sgoGroup in sgoGroups)
            {
                var studentNumberInGroup = studentNumberInGroups.FirstOrDefault(x => x.SgoGroupId == sgoGroup.SGOGroupID);
                if (studentNumberInGroup != null)
                    sgoGroup.StudentNumberInGroup = studentNumberInGroup.NumOfStudent;
            }

            return sgoGroups;
        }

        public ActionResult GetTestType(int sgoId)
        {
            var data = new List<SGOTestTypeItem>
            {
                new SGOTestTypeItem
                {
                    Id = "PreAssessment",
                    Name = "Progress Monitoring LinkIt!"
                }
            };

            data.Add(new SGOTestTypeItem
            {
                Id = "PreAssessmentExternal",
                Name = "Progress Monitoring External"
            });

            var sgoObject = _parameters.SGOObjectService.GetSGOByID(sgoId);
            var virtualTestCustomScores =
                    _parameters.VirtualTestCustomScoreService.Select().Where(x => x.DistrictId == sgoObject.DistrictID && !(x.IsMultiDate.HasValue && x.IsMultiDate.Value)).OrderBy(x => x.Name).ToList();

            var achievementLevelSettings =
                _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId)
                    .Where(x => x.DataSetOriginID == 3 || x.DataSetOriginID == 11) // Get legacy virtualtest only
                    .Select(x => new { x.DataSetCategoryID, x.DataSetCategoryName }).Distinct().ToList()
                    .Where(x => !string.IsNullOrEmpty(x.DataSetCategoryName))
                    .OrderBy(x => x.DataSetCategoryName);

            data.AddRange(virtualTestCustomScores.Select(item => new SGOTestTypeItem
            {
                Id = "PreAssessmentCustom_" + item.VirtualTestCustomScoreId,
                Name = "Progress Monitoring " + item.Name
            }));

            data.AddRange(achievementLevelSettings.Select(item => new SGOTestTypeItem
            {
                Id = "PreAssessmentHistorical_" + item.DataSetCategoryID,
                Name = "Progress Monitoring " + item.DataSetCategoryName
            }));

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadPreAssessmentTest(DataPointViewModel model)
        {
            return PartialView("_PreAssessmentTest", model);
        }

        public ActionResult LoadPreAssessmentHistoricalTest(DataPointViewModel model)
        {
            return PartialView("_PreAssessmentHistoricalTest", model);
        }

        public ActionResult LoadPreAssessmentExternal(DataPointViewModel model)
        {
            return PartialView("_PreAssessmentExternalTest", model);
        }

        public ActionResult LoadPreAssessmentCustom(DataPointViewModel model)
        {
            return PartialView("_PreAssessmentCustomTest", model);
        }

        [SGOManagerLogFilter]
        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult SaveMonitorAndScore(int sgoId, string strComments, string attachUnstructuredProgressUrl)
        {
            var obj = _parameters.SgoObjectService.GetSGOByID(sgoId);

            var vPermissionAccess = _parameters.SgoObjectService.GetPermissionAccessSgoProPressMonitoring(CurrentUser.Id, obj);
            if (vPermissionAccess.Status != (int)SGOPermissionEnum.FullUpdate)
            {
                return Json(false);
            }

            if (!_parameters.SgoGroupService.CheckSavedResultScore(sgoId))
            {
                return Json(false);
            }

            //Check valid

            if (obj != null)
            {

                if (obj.Type != (int)SGOTypeEnum.UnstructuredData)
                {
                    var vPostAssessmentLinkit = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                        .FirstOrDefault(
                            o =>
                                o.Type == (int)SGODataPointTypeEnum.PostAssessment
                                || o.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom
                                || o.Type == (int)SGODataPointTypeEnum.PostAssessmentExternal
                                || o.Type == (int)SGODataPointTypeEnum.PostAssessmentHistorical);
                    if (vPostAssessmentLinkit == null) return Json(false);
                }

                _parameters.SGOMilestoneService.CreateMilestoneWithStatus(obj.SGOID, CurrentUser.Id,
                    (int)SGOStatusType.EvaluationSubmittedForApproval);
                obj.EducatorComment = strComments; //Should store on EducatorComments
                obj.SGOStatusID = (int)SGOStatusType.EvaluationSubmittedForApproval;
                obj.AttachUnstructuredProgressUrl = attachUnstructuredProgressUrl;
                _parameters.SgoObjectService.Save(obj);
                SendEmailSubmitForApproval(obj, strComments);
                return Json(true);
            }

            return Json(false);
        }

        private void SendEmailSubmitForApproval(SGOObject obj, string educatorComment)
        {
            string strBody = "Body SGO SubmitForApproval";
            string strSubject = "SGO SubmitForApproval";
            var objEmailTemplate = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(obj.DistrictID, Constanst.Configuration_SGOEmailTemplateSubmitForApproval).FirstOrDefault();

            if (objEmailTemplate != null)
            {
                strBody = objEmailTemplate.Value;
            }

            strBody = strBody.Replace("<SGOName>", obj.Name);
            strBody = strBody.Replace("<TeacherName>", GetUserFullName(obj.OwnerUserID));
            strBody = strBody.Replace("<Educatorcomment>", WebUtility.HtmlEncode(educatorComment).Replace("\n", "<br />"));
            var vUser = _parameters.UserService.GetUserById(obj.OwnerUserID);
            var vReviewUser = _parameters.UserService.GetUserById(obj.ApproverUserID.GetValueOrDefault());
            if (vUser != null && vReviewUser != null)
            {
                strBody = strBody.Replace("<OwnerUserID>", string.Format("{0}, {1}", vUser.LastName, vUser.FirstName));
                string strEmail = vReviewUser.EmailAddress;
                Util.SendMailSGO(strBody, strSubject, strEmail);
            }
        }

        private string GetUserFullName(int userId)
        {
            var fullName = string.Empty;
            var user = _parameters.UserService.GetUserById(userId);
            if (user != null)
            {
                fullName = string.Format("{0}, {1}", user.LastName, user.FirstName);
            }

            return fullName;
        }

        private List<SGOAttainmentGoal> GetSgoAttainmentGoal(int sgoId)
        {
            var sgoAttainmentGoals = _parameters.SGOAttainmentGoalService.GetBySgoId(sgoId).ToList();
            if (!sgoAttainmentGoals.Any())
            {
                var districtDecodes = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                CurrentUser.DistrictId.GetValueOrDefault(), "SGOAttainmentGoal").ToList();

                sgoAttainmentGoals = districtDecodes.Select(x => new SGOAttainmentGoal()
                {
                    SGOId = sgoId,
                    Order = Convert.ToInt32(x.Value.Split('|')[0]),
                    Name = x.Value.Split('|')[1],
                    DefaultGoal = Convert.ToInt32(x.Value.Split('|')[2]),
                    ComparisonType = x.Value.Split('|')[3]
                }).ToList();

                foreach (var sgoAttainmentGoal in sgoAttainmentGoals)
                {
                    _parameters.SGOAttainmentGoalService.Save(sgoAttainmentGoal);
                }
            }

            return sgoAttainmentGoals;
        }

        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadAttachment(HttpPostedFileBase postedFile)
        {
            string sgobuketName = LinkitConfigurationManager.GetS3Settings().SGOBucketName;
            string sgoFolder = System.Configuration.ConfigurationManager.AppSettings["SGOFolder"];
            try
            {
                var fileName = string.Format("{0}_{1}", DateTime.Now.Ticks, postedFile.FileName);
                string sgoFileName = string.Format("{0}/{1}", sgoFolder, fileName);
                var s3Result = _s3Service.UploadRubricFile(sgobuketName, sgoFileName, postedFile.InputStream);
                if (s3Result.IsSuccess)
                {
                    return Json(new { Success = true, FileName = fileName, downloadUrl = GetLinkToDownloadAttachment(fileName) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Success = false, ErrorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetLinkToDownloadAttachment(string fileName)
        {
            var s3Domail = System.Configuration.ConfigurationManager.AppSettings["S3Domain"];
            var sgobuketName = LinkitConfigurationManager.GetS3Settings().SGOBucketName;
            var sgoFolder = System.Configuration.ConfigurationManager.AppSettings["SGOFolder"];
            var sgoFileName = string.Format("{0}/{1}/{2}/{3}", s3Domail, sgobuketName, sgoFolder, fileName);

            return sgoFileName;
        }

        public ActionResult ValidateImprovementScoringPlanDataPointTypeAndScoreType(DataPointViewModel model)
        {
            if (!CheckImprovementScoringPlanDataPointType(model))
            {
                return Json(new { Success = false, ErrorMessage = "Your SGO cannot be auto-scored based on the post-assessment you have selected. This could be because you have selected a post-assessment with a custom score type or a type is incompatible with the pre-assessment. If you would like to proceed, you will need to manually score your SGO." }, JsonRequestBehavior.AllowGet);
            }

            if (!CheckImprovementScoringPlanScoreType(model))
            {
                return Json(new { Success = false, ErrorMessage = "Your SGO cannot be auto-scored based on the post-assessment you have selected. This could be because you have selected a post-assessment with a score type is incompatible with the pre-assessment. If you would like to proceed, you will need to manually score your SGO." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public bool CheckImprovementScoringPlanDataPointType(DataPointViewModel model)
        {
            var sgo = _parameters.SGOObjectService.GetSGOByID(model.SGOId);

            if (sgo.TargetScoreType == (int)SGOTargetScoreTypeEnum.ImproveOnPostAssessment && sgo.SGOStatusID == (int)SGOStatusType.PreparationApproved)
            {
                if (model.TestType.Contains("PostAssessmentCustom_"))
                {
                    return false;
                }

                var preAssessmentDatapoint = _parameters.SgoDataPointService.GetDataPointBySGOID(model.SGOId)
                    .FirstOrDefault(
                        x =>
                            (x.Type == (int)SGODataPointTypeEnum.PreAssessment ||
                             x.Type == (int)SGODataPointTypeEnum.PreAssessmentExternal ||
                             x.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical)
                            && x.ImprovementBasedDataPoint == 1);

                if (preAssessmentDatapoint != null)
                {
                    var postAssessmentTestType = 0;
                    if (model.TestType == "PreAssessment")
                        postAssessmentTestType = (int)SGODataPointTypeEnum.PreAssessment;
                    if (model.TestType == "PreAssessmentExternal")
                        postAssessmentTestType = (int)SGODataPointTypeEnum.PreAssessmentExternal;
                    if (model.TestType.Contains("PreAssessmentHistorical_"))
                        postAssessmentTestType = (int)SGODataPointTypeEnum.PreAssessmentHistorical;

                    if ((postAssessmentTestType == (int)SGODataPointTypeEnum.PreAssessment &&
                        preAssessmentDatapoint.Type != (int)SGODataPointTypeEnum.PreAssessment)
                        || (postAssessmentTestType == (int)SGODataPointTypeEnum.PreAssessmentExternal &&
                        preAssessmentDatapoint.Type != (int)SGODataPointTypeEnum.PreAssessmentExternal)
                        || (postAssessmentTestType == (int)SGODataPointTypeEnum.PreAssessmentHistorical &&
                        preAssessmentDatapoint.Type != (int)SGODataPointTypeEnum.PreAssessmentHistorical))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CheckImprovementScoringPlanScoreType(DataPointViewModel model)
        {
            var sgoDataPoint = new SGODataPoint
            {
                Type = (int)SGODataPointTypeEnum.PostAssessment,
                GradeID = model.GradeId,
                Name = model.Name ?? "",
                RationaleGuidance = model.RationaleGuidance ?? "",
                SGODataPointID = model.SGODataPointId,
                SGOID = model.SGOId,
                SubjectName = model.SubjectName ?? "",
                VirtualTestID = model.VirtualTestId,
                ScoreType = model.ScoreType
            };

            var preAssessmentDatapoint = _parameters.SgoDataPointService.GetDataPointBySGOID(model.SGOId)
                .FirstOrDefault(
                    x => x.ImprovementBasedDataPoint == 1
                         && (x.Type == (int)SGODataPointTypeEnum.PreAssessment ||
                             x.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical));

            if (preAssessmentDatapoint != null &&
                preAssessmentDatapoint.ScoreType != sgoDataPoint.ScoreType)
            {
                return false;
            }

            return true;
        }
    }
}

