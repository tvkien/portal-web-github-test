using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using System.Linq;
using LinkIt.BubbleSheetPortal.Web.ViewModels.SGO;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers.SGO;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class SGOScoringPlanTargetController : BaseController
    {
        private readonly SGOScoringPlanTargetControllerParameters _parameters;
        private List<int> excludedGroupIds = new List<int> { 98, 99 };

        private readonly IS3Service _s3Service;

        public SGOScoringPlanTargetController(SGOScoringPlanTargetControllerParameters parameters, IS3Service s3Service)
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
            var vPermissionAccess = _parameters.SgoObjectService.GetPermissionAccessSgoStudentPopulate(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status == (int)SGOPermissionEnum.NotAvalible)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            ViewBag.PermissionAccess = vPermissionAccess.Status;
            ViewBag.SgoId = sgoId;
            ViewBag.SgoType = objSgo.Type;
            ViewBag.ApproverUserID = objSgo.ApproverUserID.GetValueOrDefault();
            ViewBag.IsAttainmentDataCompleted = IsAttainmentDataCompleted(sgoId) ? 1 : 0;
            ViewBag.DirectionConfigurationValue = GetDistrictDecodeValue(objSgo.DistrictID,
                Util.DistrictDecode_SGOScoringPlansDirection);

            var hasPostAssessmentDataPoint = _parameters.SgoDataPointService.HasPostAssessment(sgoId);
            if (!hasPostAssessmentDataPoint && objSgo.Type != (int)SGOTypeEnum.UnstructuredData)
                ViewBag.PermissionAccess = (int)SGOPermissionEnum.ReadOnly;

            if (hasPostAssessmentDataPoint)
            {
                //get scoreType
                var dataPoints = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId).ToList();
                var postAssessment =
                    dataPoints.FirstOrDefault(x => x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom);
                if (postAssessment != null)
                    ViewBag.IsNumbericScoreTypePostAssessment = (postAssessment.ScoreType != (int)SGOScoreTypeEnum.ScoreCustomA1 &&
                                                                 postAssessment.ScoreType != (int)SGOScoreTypeEnum.ScoreCustomA2 &&
                                                                 postAssessment.ScoreType != (int)SGOScoreTypeEnum.ScoreCustomA3 &&
                                                                 postAssessment.ScoreType != (int)SGOScoreTypeEnum.ScoreCustomA4);

                var scoreTypePreAssesments = dataPoints.Where(x => x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom
                    && (x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA1 || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA2 || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA3 || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA4)
                    ).Select(x => new { SgoDataPointId = x.SGODataPointID });
                ViewBag.ScoreTypePreAssessment = scoreTypePreAssesments;
            }

            return View();
        }

        private string GetDistrictDecodeValue(int districtId, string label)
        {
            var districtDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, label).FirstOrDefault();
            if (districtDecode != null)
                return districtDecode.Value;

            return "";
        }

        public ActionResult LoadTargetScoreContent(int sgoId)
        {
            var model = BuildSgoScoringPlanTargetViewModel(sgoId);
            return PartialView("_TargetScoreContent", model);
        }

        private SGOScoringPlanTargetViewModel BuildSgoScoringPlanTargetViewModel(int sgoId)
        {
            var sgoObject = _parameters.SgoObjectService.GetSGOByID(sgoId);

            var model = new SGOScoringPlanTargetViewModel
            {
                SgoId = sgoId,
                SgoGroups = GetSgoGroup(sgoId).OrderBy(x => x.Order).ToList(),
                SgoAttainmentGoals = GetSgoAttainmentGoal(sgoId).OrderByDescending(x => x.Order).ToList(),
                ToBeCreatedTotalPointPossible =
                    (int)_parameters.SgoDataPointService.ToBeCreatedTotalPointPossibleBySgoId(sgoId),
                SgoType = sgoObject.Type,
                AttachUnstructuredUrl = sgoObject.AttachUnstructuredScoringUrl,
                RationaleUnstructured = sgoObject.RationaleUnstructuredScoring,
                AttachUnstructuredDownloadUrl =
                    string.IsNullOrEmpty(sgoObject.AttachUnstructuredScoringUrl)
                        ? ""
                        : GetLinkToDownloadAttachment(sgoObject.AttachUnstructuredScoringUrl)
            };

            model.SgoAttainmentGroups =
                _parameters.SgoAttainmentGroupService.GetBySgoGroupIds(
                    model.SgoGroups.Select(x => x.SGOGroupID).ToList()).ToList();

            var sgoDataPoints = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId).ToList();

            model.PreAssessmentSelectListItems = GetPreAssessmentSelectListItems(sgoDataPoints);
            model.PreAssessmentCustomSelectListItems = GetCustomPreAssessmentSelectListItems(sgoDataPoints);

            model.PostAssessmentLinkitTotalPointPossible = GetPostAssessmentPointsPossible(sgoDataPoints);
            model.HavePostAssessmentToBeCreated =
                sgoDataPoints.Any(x => x.Type == (int)SGODataPointTypeEnum.PostAssessmentToBeCreated);

            model.HavePostAssessment = _parameters.SgoDataPointService.HasPostAssessment(model.SgoId);
            model.HavePostAssessmentCustom =
                sgoDataPoints.Any(x => x.Type == (int)SGODataPointTypeEnum.PostAssessment || x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom);

            model.TargetScoreType = sgoObject.TargetScoreType == 0
                ? ((model.HavePostAssessmentToBeCreated || model.HavePostAssessment)
                    ? 1
                    : (model.SgoType == (int)SGOTypeEnum.UnstructuredData ? 4 : 0))
                : sgoObject.TargetScoreType;

            var postAssessmentCustom =
                sgoDataPoints.FirstOrDefault(x => x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom);
            if (postAssessmentCustom != null &&
                (postAssessmentCustom.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA1 ||
                 postAssessmentCustom.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA2 ||
                 postAssessmentCustom.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA3 ||
                 postAssessmentCustom.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA4))
                model.IsCustomTextScoreTypePostAssessment = true;

            return model;
        }

        private List<SelectListItem> GetPreAssessmentSelectListItems(List<SGODataPoint> sgoDataPoints)
        {
            var preAssessmentDataPoints =
                sgoDataPoints
                    .Where(x => x.Type == (int)SGODataPointTypeEnum.PreAssessment
                                || x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom
                                || x.Type == (int)SGODataPointTypeEnum.PreAssessmentExternal
                                || x.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical)
                    .OrderBy(x => x.Name);

            var preAssessmentSelectListItems = preAssessmentDataPoints.Select(
                x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.SGODataPointID.ToString(),
                    Selected = x.ImprovementBasedDataPoint == 1
                }).ToList();

            preAssessmentSelectListItems.Insert(0, new SelectListItem
            {
                Text = "Select Data Point",
                Value = "select"
            });

            return preAssessmentSelectListItems;
        }

        private List<SelectListItem> GetCustomPreAssessmentSelectListItems(List<SGODataPoint> sgoDataPoints)
        {
            var postAssessmentCustomAchievementLevelSettingId = 0;
            var postAssessementCustom =
                sgoDataPoints.FirstOrDefault(x => x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom);
            if (postAssessementCustom != null)
            {
                postAssessmentCustomAchievementLevelSettingId = postAssessementCustom.AchievementLevelSettingID.GetValueOrDefault();
            }

            // Get PreAssessment customs have same AchievementLevelSetting with PostAssessmentCustom
            var customPreAssessmentDataPoints =
                sgoDataPoints
                    .Where(x =>
                        (x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom)
                        || x.Type == (int)SGODataPointTypeEnum.PreAssessment
                        || x.Type == (int)SGODataPointTypeEnum.PreAssessmentExternal
                        || x.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical)
                    .OrderBy(x => x.Name);

            var customPreAssessmentSelectListItems = customPreAssessmentDataPoints.Select(
                x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.SGODataPointID.ToString(),
                    Selected = x.ImprovementBasedDataPoint == 1
                }).ToList();

            customPreAssessmentSelectListItems.Insert(0, new SelectListItem
            {
                Text = "It does not compare to a pre-assessment",
                Value = "select"
            });

            return customPreAssessmentSelectListItems;
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

        private bool IsAttainmentDataCompleted(int sgoId)
        {
            var sgoGroups = GetSgoGroup(sgoId);
            var sgoAttainmentGoals = GetSgoAttainmentGoal(sgoId);
            var sgoAttainmentGroups =
                _parameters.SgoAttainmentGroupService.GetBySgoGroupIds(
                    sgoGroups.Select(x => x.SGOGroupID).ToList()).ToList();

            var sgoObject = _parameters.SgoObjectService.GetSGOByID(sgoId);

            if (sgoObject.TargetScoreType == 2)
            {
                var dataPoints = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId);
                var postAssessmentDataPoint = dataPoints.FirstOrDefault(x =>
                                x.Type == (int)SGODataPointTypeEnum.PostAssessment ||
                                x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom ||
                                x.Type == (int)SGODataPointTypeEnum.PostAssessmentHistorical);
                var preAssessmentDatapoint = dataPoints.FirstOrDefault(x => x.ImprovementBasedDataPoint == 1 &&
                                (x.Type == (int)SGODataPointTypeEnum.PreAssessment
                                || x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom
                                || x.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical));
                if (postAssessmentDataPoint != null && postAssessmentDataPoint.ScoreType.HasValue && preAssessmentDatapoint != null &&
                        preAssessmentDatapoint.ScoreType != postAssessmentDataPoint.ScoreType)
                {
                    return false;
                }
            }

            if (sgoObject.TargetScoreType > 0
                && sgoGroups.Any() // for unstructured data sgo
                && sgoGroups.All(x => x.TargetScore.HasValue || !string.IsNullOrEmpty(x.TargetScoreCustom))
                && sgoAttainmentGroups.Count == sgoGroups.Count * sgoAttainmentGoals.Count)
            {
                return true;
            }

            return false;
        }

        public ActionResult CheckAttainmentDataCompleted(int sgoId)
        {
            return Json(new { Success = IsAttainmentDataCompleted(sgoId) }, JsonRequestBehavior.AllowGet);
        }

        private List<SGOAttainmentGoal> GetSgoAttainmentGoal(int sgoId)
        {
            var sgoAttainmentGoals = _parameters.SGOAttainmentGoalService.GetBySgoId(sgoId).ToList();
            if (!sgoAttainmentGoals.Any())
            {
                var districtDecodes = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                    CurrentUser.DistrictId.GetValueOrDefault(), "SGOAttainmentGoal").ToList();

                if (districtDecodes.Any())
                {
                    sgoAttainmentGoals = districtDecodes.Select(x => new SGOAttainmentGoal()
                    {
                        SGOId = sgoId,
                        Order = Convert.ToInt32(x.Value.Split('|')[0]),
                        Name = x.Value.Split('|')[1],
                        DefaultGoal = Convert.ToInt32(x.Value.Split('|')[2]),
                        ComparisonType = x.Value.Split('|')[3]
                    }).ToList();
                }
                else
                {
                    var districtConfigurations =
                        _parameters.ConfigurationService.Select().Where(x => x.Name == "SGOAttainmentGoal").ToList();

                    sgoAttainmentGoals = districtConfigurations.Select(x => new SGOAttainmentGoal()
                    {
                        SGOId = sgoId,
                        Order = Convert.ToInt32(x.Value.Split('|')[0]),
                        Name = x.Value.Split('|')[1],
                        DefaultGoal = Convert.ToInt32(x.Value.Split('|')[2]),
                        ComparisonType = x.Value.Split('|')[3]
                    }).ToList();
                }

                foreach (var sgoAttainmentGoal in sgoAttainmentGoals)
                {
                    _parameters.SGOAttainmentGoalService.Save(sgoAttainmentGoal);
                }
            }

            return sgoAttainmentGoals;
        }

        public ActionResult LoadSetTargetScore(int sgoId, int targetScoreType, int? improvementBasedDataPoint, int? improvementBasedDataPointCustom)
        {
            var sgoGroups = _parameters.SgoGroupService.GetGroupBySgoID(sgoId).Where(x => !excludedGroupIds.Contains(x.Order)).OrderBy(x => x.Order).ToList();
            ViewBag.TargetScoreType = targetScoreType;
            return PartialView("_SetTargetScore", sgoGroups);
        }

        [SGOManagerLogFilter]
        public ActionResult SetTargetScore(int targetScoreType, string sgoGroupData, int? improvementBasedDataPoint,
            int? improvementBasedDataPointCustom)
        {
            var js = new JavaScriptSerializer();
            var sgoGroups = js.Deserialize<List<SGOGroup>>(sgoGroupData).ToList();

            if (!sgoGroups.Any())
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

            var sgoId = _parameters.SgoGroupService.GetGroupById(sgoGroups[0].SGOGroupID).SGOID;

            if (!IsAllowEditScoringPlan(sgoId) && !IsAllowSetManualScoringTarget(sgoId))
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

            if (!ValidateAndUpdateImprovementBasedDataPoint(sgoId, targetScoreType, improvementBasedDataPoint,
                improvementBasedDataPointCustom))
            {
                return Json(new { Success = false, ErrorMessage = "The score type on the post-assessment does not match that found on the pre-assessment. Please either modify the data points you are using, or choose the manual scoring method." }, JsonRequestBehavior.AllowGet);
            }

            foreach (var sgoGroup in sgoGroups)
            {
                var sgoGroupEntity = _parameters.SgoGroupService.GetGroupById(sgoGroup.SGOGroupID);
                sgoGroupEntity.TargetScore = targetScoreType == 4 ? null : sgoGroup.TargetScore;
                sgoGroupEntity.TargetScoreCustom = targetScoreType == 4 ? sgoGroup.TargetScoreCustom : null;
                _parameters.SgoGroupService.Save(sgoGroupEntity);
            }

            if (sgoId > 0)
            {
                var sgoObject = _parameters.SgoObjectService.GetSGOByID(sgoId);
                sgoObject.TargetScoreType = targetScoreType;
                _parameters.SgoObjectService.Save(sgoObject);

                // Populate default value into SGOAttainmentGroup after target score is set
                _parameters.SgoObjectService.PopulateDefaultAttainmentGroup(sgoId);


            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        private bool IsAllowEditScoringPlan(int sgoId)
        {
            var sgoObject = _parameters.SgoObjectService.GetSGOByID(sgoId);
            var sgoPermission = _parameters.SgoObjectService.GetPermissionAccessSgoScoringPlan(CurrentUser.Id, sgoObject);

            return sgoPermission.Status == (int)SGOPermissionEnum.FullUpdate;
        }

        private bool IsAllowSetManualScoringTarget(int sgoId)
        {
            // Allow to set Custom Target Score when SGO at PreparationApproved step and Custom Target Score is not set (caused by updated at Select SGO datapoint)
            var sgoObject = _parameters.SgoObjectService.GetSGOByID(sgoId);

            if (sgoObject.SGOStatusID == (int)SGOStatusType.PreparationApproved &&
                sgoObject.TargetScoreType == (int)SGOTargetScoreTypeEnum.ManualScoring)
            {
                if (
                    _parameters.SgoGroupService.GetGroupBySgoID(sgoId)
                        .Any(x => x.TargetScoreCustom == null || x.TargetScoreCustom == ""))
                {
                    return true;
                }
            }

            return true;
        }

        public ActionResult LoadSetAttainment(int sgoId, int sgoAttainmentGoalId, int targetScoreType)
        {
            var model = new SetAttainmentViewModel
            {
                SgoAttainmentGoal = _parameters.SGOAttainmentGoalService.GetById(sgoAttainmentGoalId),
                TargetScoreType = targetScoreType,
                TotalPointsPossibleOfPostAssessmentValue = GetPostAssessmentPointsPossible(sgoId),
                SgoGroups = _parameters.SgoGroupService.GetGroupBySgoID(sgoId)
                    .Where(x => !excludedGroupIds.Contains(x.Order)).OrderBy(x => x.Order).ToList(),
                SgoAttainmentGroups = _parameters.SgoAttainmentGroupService.GetBySgoAttainmentGoalId(sgoAttainmentGoalId).ToList()
            };

            return PartialView("_SetAttainment", model);
        }

        private string GetPostAssessmentPointsPossible(List<SGODataPoint> sgoDataPoints)
        {
            var postAssessmentDataPoint =
                sgoDataPoints
                    .FirstOrDefault(x => x.Type == (int)SGODataPointTypeEnum.PostAssessment);

            if (postAssessmentDataPoint != null)
            {
                if (postAssessmentDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScorePercent)
                {
                    return "100%";
                }
                else if (postAssessmentDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScoreRaw)
                {
                    var virtualQuestions =
                        _parameters.VirtualQuestionService.GetVirtualQuestionByVirtualTestID(
                            postAssessmentDataPoint.VirtualTestID.GetValueOrDefault());
                    if (virtualQuestions.Any())
                        return virtualQuestions.Sum(x => x.PointsPossible).ToString();
                }
                else
                {
                    return "";
                }
            }

            return "total points possible";
        }

        private string GetPostAssessmentPointsPossible(int sgoId)
        {
            var postAssessmentDataPoint =
                _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                    .FirstOrDefault(x => x.Type == (int)SGODataPointTypeEnum.PostAssessment);

            if (postAssessmentDataPoint != null)
            {
                if (postAssessmentDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScorePercent)
                {
                    return "100%";
                }
                else if (postAssessmentDataPoint.ScoreType == (int)SGOScoreTypeEnum.ScoreRaw)
                {
                    var virtualQuestions =
                        _parameters.VirtualQuestionService.GetVirtualQuestionByVirtualTestID(
                            postAssessmentDataPoint.VirtualTestID.GetValueOrDefault());
                    if (virtualQuestions.Any())
                        return virtualQuestions.Sum(x => x.PointsPossible).ToString();
                }
                else
                {
                    return "the score";
                }
            }

            return "total points possible";
        }

        [SGOManagerLogFilter]
        public ActionResult SetAttainment(int sgoId, string sgoAttainmentGroupData, int sgoAttainmentGoalId)
        {
            if (sgoId > 0 && !IsAllowEditScoringPlan(sgoId))
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

            var js = new JavaScriptSerializer();
            var sgoAttainmentGroups = js.Deserialize<List<SGOAttainmentGroup>>(sgoAttainmentGroupData).ToList();

            var sgoAttainmentGoal =
                _parameters.SGOAttainmentGoalService.GetById(sgoAttainmentGoalId);

            var sgoAttainmentGroupEntities =
                _parameters.SgoAttainmentGroupService.GetBySgoAttainmentGoalId(sgoAttainmentGoalId).ToList();

            foreach (var sgoAttainmentGroup in sgoAttainmentGroups)
            {
                var sgoAttainmentGroupEntity =
                    sgoAttainmentGroupEntities.FirstOrDefault(
                        x =>
                            x.SGOGroupId == sgoAttainmentGroup.SGOGroupId &&
                            x.SGOAttainmentGoalId == sgoAttainmentGoalId);

                if (sgoAttainmentGroupEntity != null)
                {
                    sgoAttainmentGroupEntity.GoalValue = sgoAttainmentGroup.GoalValue;
                }
                else
                {
                    sgoAttainmentGroupEntity = new SGOAttainmentGroup
                    {
                        GoalValue = sgoAttainmentGroup.GoalValue,
                        Order = sgoAttainmentGoal.Order,
                        SGOGroupId = sgoAttainmentGroup.SGOGroupId,
                        Name = sgoAttainmentGoal.Name,
                        SGOAttainmentGoalId = sgoAttainmentGoal.SGOAttainmentGoalId
                    };
                }

                _parameters.SgoAttainmentGroupService.Save(sgoAttainmentGroupEntity);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        private bool ValidateAndUpdateImprovementBasedDataPoint(int sgoId, int targetScoreType,
            int? improvementBasedDataPoint,
            int? improvementBasedDataPointCustom)
        {
            var improvementBasedDataPointId = targetScoreType == 2
                ? improvementBasedDataPoint.GetValueOrDefault()
                : targetScoreType == 4 ? improvementBasedDataPointCustom.GetValueOrDefault() : 0;

            var preAssessmentDataPoints =
                _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                    .Where(x =>
                        x.Type == (int)SGODataPointTypeEnum.PreAssessment ||
                        x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom ||
                        x.Type == (int)SGODataPointTypeEnum.PreAssessmentExternal ||
                        x.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical)
                    .ToList();

            if (targetScoreType == 2)
            {
                var preAssessmentDatapoint =
                    preAssessmentDataPoints.FirstOrDefault(x => x.SGODataPointID == improvementBasedDataPointId);
                if (preAssessmentDatapoint != null && preAssessmentDatapoint.Type == (int)SGODataPointTypeEnum.PreAssessmentExternal)
                    preAssessmentDatapoint.ScoreType = (int)SGOScoreTypeEnum.ScoreRaw;
                if (!ValidateImprovementScoringPlanScoreType(sgoId, preAssessmentDatapoint))
                {
                    return false;
                }
            }

            foreach (var sgoDataPoint in preAssessmentDataPoints)
            {
                sgoDataPoint.ImprovementBasedDataPoint = sgoDataPoint.SGODataPointID == improvementBasedDataPointId
                    ? 1
                    : 0;
                _parameters.SgoDataPointService.Save(sgoDataPoint);
            }

            return true;
        }

        private bool ValidateImprovementScoringPlanScoreType(int sgoId, SGODataPoint preAssessmentDatapoint)
        {
            var postAssessmentDataPoint =
                    _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                        .FirstOrDefault(x =>
                            x.Type == (int)SGODataPointTypeEnum.PostAssessment ||
                            x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom ||
                            x.Type == (int)SGODataPointTypeEnum.PostAssessmentHistorical);

            if (postAssessmentDataPoint != null && postAssessmentDataPoint.ScoreType.HasValue)
            {
                if (preAssessmentDatapoint != null &&
                    preAssessmentDatapoint.ScoreType != postAssessmentDataPoint.ScoreType)
                {
                    return false;
                }
            }

            return true;
        }

        [SGOManagerLogFilter]
        public ActionResult SaveScoringPlan(int sgoId, int targetScoreType, int? improvementBasedDataPoint, int? improvementBasedDataPointCustom, decimal? totalPointPossible)
        {
            if (sgoId > 0 && !IsAllowEditScoringPlan(sgoId))
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

            if (!ValidateAndUpdateImprovementBasedDataPoint(sgoId, targetScoreType, improvementBasedDataPoint,
                improvementBasedDataPointCustom))
            {
                return Json(new { Success = false, ErrorMessage = "The score types you have chosen for your pre and post-assessment cannot be compared or cannot be used for auto-scoring your SGO. Please return to Step 3 and change your score types or select a different scoring option." }, JsonRequestBehavior.AllowGet);
            }

            var sgoObject = _parameters.SgoObjectService.GetSGOByID(sgoId);
            sgoObject.TargetScoreType = targetScoreType;
            _parameters.SgoObjectService.Save(sgoObject);

            //Update TotalPointPossible for ToBeCreated
            if (totalPointPossible.HasValue && totalPointPossible.Value >= 0)
            {
                var vToBeCreated = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                    .FirstOrDefault(x => x.Type == (int)SGODataPointTypeEnum.PostAssessmentToBeCreated);
                if (vToBeCreated != null)
                {
                    vToBeCreated.TotalPoints = totalPointPossible.Value;
                    _parameters.SgoDataPointService.Save(vToBeCreated);
                }
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [SGOManagerLogFilter]
        public ActionResult SaveUnstructuredScoringPlan(int sgoId, int targetScoreType, string rationaleUnstructured, string attachUnstructuredUrl, string sgoGroupData, string sgoAttainmentGroupData)
        {
            var js = new JavaScriptSerializer();
            var sgoGroups = js.Deserialize<List<SGOGroup>>(sgoGroupData).ToList();
            var sgoAttainmentGroups = js.Deserialize<List<SGOAttainmentGroup>>(sgoAttainmentGroupData).ToList();

            if (sgoGroups.Any() && sgoAttainmentGroups.Any())
            {
                var sgoObject = _parameters.SgoObjectService.GetSGOByID(sgoId);
                sgoObject.TargetScoreType = targetScoreType;
                sgoObject.RationaleUnstructuredScoring = rationaleUnstructured;
                sgoObject.AttachUnstructuredScoringUrl = attachUnstructuredUrl;
                _parameters.SgoObjectService.Save(sgoObject);

                DeleteUnstructuredGroup(sgoId, sgoGroups);

                foreach (var sgoGroup in sgoGroups)
                {
                    var sgoGroupEntity = new SGOGroup();

                    // Update SGOGroup information
                    if (sgoGroup.SGOGroupID > 0)
                    {
                        sgoGroupEntity = _parameters.SgoGroupService.GetGroupById(sgoGroup.SGOGroupID);
                        sgoGroupEntity.TargetScoreCustom = sgoGroup.TargetScoreCustom;
                    }
                    else
                    {
                        sgoGroupEntity.SGOID = sgoId;
                        sgoGroupEntity.TargetScoreCustom = sgoGroup.TargetScoreCustom;
                        sgoGroupEntity.Order = sgoGroup.Order;
                        sgoGroupEntity.Name = sgoGroup.Order.ToString();
                    }
                    _parameters.SgoGroupService.Save(sgoGroupEntity);

                    // Update SGOAttainmentGroup information
                    foreach (var sgoAttainmentGroup in sgoAttainmentGroups.Where(x => x.Order == sgoGroup.Order))
                    {
                        var sgoAttainmentGroupEntity = new SGOAttainmentGroup();
                        if (sgoAttainmentGroup.SGOAttainmentGroupId > 0)
                        {
                            sgoAttainmentGroupEntity =
                                _parameters.SgoAttainmentGroupService.GetBySgoAttainmentGroupId(
                                    sgoAttainmentGroup.SGOAttainmentGroupId);
                            sgoAttainmentGroupEntity.GoalValueCustom = sgoAttainmentGroup.GoalValueCustom;
                        }
                        else
                        {
                            var sgoAttainmentGoal =
                                _parameters.SGOAttainmentGoalService.GetById(sgoAttainmentGroup.SGOAttainmentGoalId);

                            sgoAttainmentGroupEntity.GoalValueCustom = sgoAttainmentGroup.GoalValueCustom;
                            sgoAttainmentGroupEntity.SGOAttainmentGoalId = sgoAttainmentGroup.SGOAttainmentGoalId;
                            sgoAttainmentGroupEntity.SGOGroupId = sgoGroupEntity.SGOGroupID;
                            sgoAttainmentGroupEntity.Order = sgoAttainmentGoal.Order;
                            sgoAttainmentGroupEntity.Name = sgoAttainmentGoal.Name;
                        }

                        _parameters.SgoAttainmentGroupService.Save(sgoAttainmentGroupEntity);
                    }
                }
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [SGOManagerLogFilter]
        private void DeleteUnstructuredGroup(int sgoId, List<SGOGroup> newSgoGroups)
        {
            var sgoGroupCurrent = _parameters.SgoGroupService.GetGroupWithOut9899BySgoID(sgoId);
            var sgoAttainmentGroupCurrent = _parameters.SgoAttainmentGroupService.GetBySgoGroupIds(sgoGroupCurrent.Select(x => x.SGOGroupID).ToList());

            foreach (var sgoAttainmentGroup in sgoAttainmentGroupCurrent)
            {
                if (newSgoGroups.All(x => x.SGOGroupID != sgoAttainmentGroup.SGOGroupId))
                {
                    _parameters.SgoAttainmentGroupService.Delete(sgoAttainmentGroup);
                }
            }

            foreach (var sgoGroup in sgoGroupCurrent)
            {
                if (newSgoGroups.All(x => x.SGOGroupID != sgoGroup.SGOGroupID))
                {
                    _parameters.SgoGroupService.Delete(sgoGroup);
                }
            }
        }

        [SGOManagerLogFilter]
        public ActionResult SetImprovementBasedDataPoint(int sgoId, int? sgoDataPointId)
        {
            if (sgoId > 0 && !IsAllowEditScoringPlan(sgoId))
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

            var preAssessmentDataPoints =
                _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                    .Where(x => x.Type == (int)SGODataPointTypeEnum.PreAssessment)
                    .ToList();
            foreach (var sgoDataPoint in preAssessmentDataPoints)
            {
                sgoDataPoint.ImprovementBasedDataPoint = sgoDataPoint.SGODataPointID == sgoDataPointId.GetValueOrDefault() ? 1 : 0;
                _parameters.SgoDataPointService.Save(sgoDataPoint);
            }

            return Json(new { Success = "true" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetScoringPlanInstruction(int? districtId)
        {
            if (!districtId.HasValue)
                districtId = CurrentUser.DistrictId;

            var districtDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId.GetValueOrDefault(), Util.DistrictDecode_SGOScoringPlan).FirstOrDefault();

            var instructionMessage = "";

            if (districtDecode != null)
                instructionMessage = districtDecode.Value;

            return Json(new { InstructionMessage = instructionMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnstructuredScoringPlanInstruction(int? districtId)
        {
            if (!districtId.HasValue)
                districtId = CurrentUser.DistrictId;

            var districtDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId.GetValueOrDefault(), Util.DistrictDecode_SGOUnstructuredScoringPlan).FirstOrDefault();

            var instructionMessage = "";

            if (districtDecode != null)
                instructionMessage = districtDecode.Value;

            return Json(new { InstructionMessage = instructionMessage }, JsonRequestBehavior.AllowGet);
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
    }
}
