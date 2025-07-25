using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Xml.Serialization;
using DevExpress.Data.WcfLinq.Helpers;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraRichEdit.Model;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using System.Linq;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.SGO;
using Lokad.Cloud.Storage;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.Helpers.SGO;
using LinkIt.BubbleSheetPortal.Models.DTOs.SGO;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class SGOSelectDataPointController : BaseController
    {
        private const string MaxSGODataPointErrorFormat = "You are allowed a maximum of {0} pre-assessment data point(s) and {1} post-assessment data point(s).";


        private readonly SGOSelectDataPointControllerParameters _parameters;

        #region public functions

        private readonly IS3Service _s3Service;

        public SGOSelectDataPointController(SGOSelectDataPointControllerParameters parameters, IS3Service s3Service)
        {
            _parameters = parameters;
            _s3Service = s3Service;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemSGOManager)]
        public ActionResult Index(int sgoId = 0)
        {
            //Use Navigation
            ViewBag.SGOID = sgoId;
            var model = new SelectDataPointViewModel
            {
                SgoId = sgoId
            };

            var objSgo = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (objSgo == null)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            var vPermissionAccess = _parameters.SGOObjectService.GetPermissionAccessSgoDataPoint(CurrentUser.Id, objSgo);
            if (vPermissionAccess.Status == (int)SGOPermissionEnum.NotAvalible)
            {
                return RedirectToAction("Index", "SGOManage");
            }
            model.PermissionAccess = vPermissionAccess.Status;
            model.SgoStatusId = objSgo.SGOStatusID;
            UpdateModelWithDistrictDecode(model);

            var lst = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId).ToList();
            var sgoDataPointIdData = lst
                .Select(x => x.SGODataPointID + ":" + GenerateDataPointGroupType(lst.FirstOrDefault(k=>k.SGODataPointID == x.SGODataPointID))).ToList();
            model.SgoDataPointIds = string.Join(";", sgoDataPointIdData); // (E.g: 1:PreAssessment;2:PostAssessment)

            var sgoDataPointPostAssessmentLinkit = lst.FirstOrDefault(o => o.Type == (int)SGODataPointTypeEnum.PostAssessment
                || o.Type == (int)SGODataPointTypeEnum.PostAssessmentToBeCreated);
            model.PostAssessmentDataPointId = sgoDataPointPostAssessmentLinkit == null
                ? -1
                : sgoDataPointPostAssessmentLinkit.SGODataPointID;
            model.DirectionConfigurationValue = GetDistrictDecodeValue(objSgo.DistrictID, Util.DistrictDecode_SGODataPointDirection);

            return View(model);
        }

        private int GetPermissionAccessSgoDataPoint(int sgoId)
        {
            var objSgo = _parameters.SGOObjectService.GetSGOByID(sgoId);
            if (objSgo == null)
            {
                return (int)SGOPermissionEnum.NotAvalible;
            }
            return _parameters.SGOObjectService.GetPermissionAccessSgoDataPoint(CurrentUser.Id, objSgo).Status;
        }
        
        private string GetDistrictDecodeValue(int districtId, string label)
        {
            var districtDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, label).FirstOrDefault();
            if (districtDecode != null)
                return districtDecode.Value;

            return "";
        }

        public ActionResult Continue(int sgoId)
        {
            var url = Url.Action("EstablishStudentGroups", "SGOManage") + "?id=" + sgoId.ToString();
            return Redirect(url);
        }

        public ActionResult LoadSelectTestType(DataPointViewModel model)
        {
            if (model.SGODataPointId > 0)
            {
                var sgoDataPoint = _parameters.SgoDataPointService.GetById(model.SGODataPointId);
                if (sgoDataPoint != null)
                {
                    model.TestType = GenerateTestType(sgoDataPoint);
                    model.DataPointGroupType = GenerateDataPointGroupType(sgoDataPoint);
                    model.Name = sgoDataPoint.Name;
                }
                else
                {
                    return new EmptyResult();
                }
            }

            return PartialView("_SelectTestType", model);
        }

        private string GenerateTestType(SGODataPoint sgoDataPoint)
        {
            switch (sgoDataPoint.Type)
            {
                case 1:
                    return "PostAssessment";
                case 2:
                    return "PreAssessment";
                case 3:
                    return "PreAssessmentHistorical_" + sgoDataPoint.DataSetCategoryID;
                case 4:
                    return "PreAssessmentExternal";
                case 5:
                    return "PostAssessmentToBeCreated";
                case 6:
                    return "PreAssessmentCustom_" + sgoDataPoint.AchievementLevelSettingID; 
                case 7:
                    return "PostAssessmentCustom_" + sgoDataPoint.AchievementLevelSettingID;
                case 8:
                    return "PostAssessmentExternal";
                case 9:
                    return "PostAssessmentHistorical_" + sgoDataPoint.DataSetCategoryID; 
                default:
                    return "";
            }
        }

        private string GenerateDataPointGroupType(SGODataPoint sgoDataPoint)
        {
            switch (sgoDataPoint.Type)
            {
                case 1:
                case 5:
                case 7:
                case 8:
                case 9:
                    return "PostAssessment";
                case 2:
                case 3:
                case 4:
                case 6:
                    return "PreAssessment";
                default:
                    return "";
            }
        }

        public ActionResult LoadPostAssessmentTest(DataPointViewModel model, bool? layoutV2 = false)
        {
            if (model.SGODataPointId > 0)
            {
                var dataPointIndex = model.DataPointIndex;
                model = LoadDataPointData(model.SGODataPointId);
                model.DataPointIndex = dataPointIndex;
            }
            else
            {
                LoadDataPointDataWhenChangingFromToBeCreated(model);
            }

            if (layoutV2 == true)
            {
                return PartialView("v2/_PostAssessmentTest", model);

            }
            return PartialView("_PostAssessmentTest", model);
        }

        public ActionResult LoadPreAssessmentTest(DataPointViewModel model, bool? layoutV2 = false)
        {
            if (model.SGODataPointId > 0)
            {
                var dataPointIndex = model.DataPointIndex;
                model = LoadDataPointData(model.SGODataPointId);
                model.DataPointIndex = dataPointIndex;
            }
            if (layoutV2 == true)
            {
                return PartialView("v2/_PreAssessmentTest", model);

            }
            return PartialView("_PreAssessmentTest", model);
        }

        public ActionResult LoadPreAssessmentHistoricalTest(DataPointViewModel model, bool? layoutV2 = false)
        {
            if (model.SGODataPointId > 0)
            {
                var dataPointIndex = model.DataPointIndex;
                model = LoadDataPointData(model.SGODataPointId);
                model.DataPointIndex = dataPointIndex;
            }
            if (layoutV2 == true)
            {
                return PartialView("v2/_PreAssessmentHistoricalTest", model);
            }
            return PartialView("_PreAssessmentHistoricalTest", model);
        }

        public ActionResult LoadPostAssessmentHistoricalTest(DataPointViewModel model, bool? layoutV2 = false)
        {
            if (model.SGODataPointId > 0)
            {
                var dataPointIndex = model.DataPointIndex;
                model = LoadDataPointData(model.SGODataPointId);
                model.DataPointIndex = dataPointIndex;
            }
            else
            {
                LoadDataPointDataWhenChangingFromToBeCreated(model);
            }
            if (layoutV2 == true)
            {
                return PartialView("v2/_PostAssessmentHistoricalTest", model);
            }
            return PartialView("_PostAssessmentHistoricalTest", model);
        }

        public ActionResult LoadPreAssessmentExternalTest(DataPointViewModel model, bool? layoutV2 = false)
        {
            if (model.SGODataPointId > 0)
            {
                var dataPointIndex = model.DataPointIndex;
                model = LoadDataPointData(model.SGODataPointId);
                model.DataPointIndex = dataPointIndex;
            }
            if (layoutV2 == true)
            {
                return PartialView("v2/_PreAssessmentExternalTest", model);
            }
            return PartialView("_PreAssessmentExternalTest", model);
        }

        public ActionResult LoadPostAssessmentExternalTest(DataPointViewModel model, bool? layoutV2 = false)
        {
            if (model.SGODataPointId > 0)
            {
                var dataPointIndex = model.DataPointIndex;
                model = LoadDataPointData(model.SGODataPointId);
                model.DataPointIndex = dataPointIndex;
            }
            else
            {
                LoadDataPointDataWhenChangingFromToBeCreated(model);
            }
            if (layoutV2 == true)
            {
                return PartialView("v2/_PostAssessmentExternalTest", model);
            }

            return PartialView("_PostAssessmentExternalTest", model);
        }

        private void LoadDataPointDataWhenChangingFromToBeCreated(DataPointViewModel model)
        {
            var sgoObject = _parameters.SGOObjectService.GetSGOByID(model.SGOId);
            if (sgoObject.SGOStatusID == (int) SGOStatusType.PreparationApproved)
            {
                var sgoDataPointToBeCreated = _parameters.SgoDataPointService.GetDataPointBySGOID(model.SGOId)
                    .FirstOrDefault(x => x.Type == (int) SGODataPointTypeEnum.PostAssessmentToBeCreated);

                if (sgoDataPointToBeCreated != null)
                {
                    model.RationaleGuidance = sgoDataPointToBeCreated.RationaleGuidance;
                    model.AttactScoreUrl = sgoDataPointToBeCreated.AttachScoreUrl;
                    model.AttactScoreDownloadLink = GetLinkToDownloadAttachment(model.AttactScoreUrl);
                    model.DirectionConfigurationValue = GetDistrictDecodeValue(sgoObject.DistrictID,
                        Util.DistrictDecode_SGORationaleAndPostAssessmentGuidanceDirection);
                }
            }
        }

        public ActionResult LoadPostAssessmentToBeCreatedTest(DataPointViewModel model, bool? layoutV2 = false)
        {
            if (model.SGODataPointId > 0)
            {
                var dataPointIndex = model.DataPointIndex;
                model = LoadDataPointData(model.SGODataPointId);
                model.DataPointIndex = dataPointIndex;
            }

            var sgoObject = _parameters.SGOObjectService.GetSGOByID(model.SGOId);
            if (sgoObject != null)
            {
                model.DirectionConfigurationValue = GetDistrictDecodeValue(sgoObject.DistrictID,
                    Util.DistrictDecode_SGORationaleAndPostAssessmentGuidanceDirection);
            }
            if (layoutV2 == true)
            {
                return PartialView("v2/_PostAssessmentToBeCreatedTest", model);
            }

            return PartialView("_PostAssessmentToBeCreatedTest", model);
        }

        public ActionResult LoadPreAssessmentCustomTest(DataPointViewModel model, bool? layoutV2 = false)
        {
            if (model.SGODataPointId > 0)
            {
                var dataPointIndex = model.DataPointIndex;
                model = LoadDataPointData(model.SGODataPointId);
                model.DataPointIndex = dataPointIndex;
            }
            if (layoutV2 == true)
            {
                return PartialView("v2/_PreAssessmentCustomTest", model);

            }
            return PartialView("_PreAssessmentCustomTest", model);
        }

        public ActionResult LoadPostAssessmentCustomTest(DataPointViewModel model)
        {
            if (model.SGODataPointId > 0)
            {
                var dataPointIndex = model.DataPointIndex;
                model = LoadDataPointData(model.SGODataPointId);
                model.DataPointIndex = dataPointIndex;
            }
            else
            {
                LoadDataPointDataWhenChangingFromToBeCreated(model);
            }

            return PartialView("_PostAssessmentCustomTest", model);
        }

        private DataPointViewModel LoadDataPointData(int sgoDataPointId)
        {
            var dataPoint = _parameters.SgoDataPointService.GetById(sgoDataPointId);
            var subjectAndGradeInfo = new SubjectAndGradeDto();
            var gradeId = dataPoint.GradeID;
            var subjectName = dataPoint.SubjectName;
            if (dataPoint.Type != (int)SGODataPointTypeEnum.PostAssessmentToBeCreated)
            {
                subjectAndGradeInfo = _parameters.SgoSelectDataPointService.GetSubjectAndGradeByVirtualTestId(dataPoint.VirtualTestID.GetValueOrDefault());
                gradeId = subjectAndGradeInfo.GradeId;
                subjectName = subjectAndGradeInfo.SubjectName;
            }

            var model = new DataPointViewModel
            {
                SGODataPointId = dataPoint.SGODataPointID,
                DataSetCategoryID = dataPoint.AchievementLevelSettingID,
                AttactScoreUrl = dataPoint.AttachScoreUrl,
                GradeId = gradeId,
                Name = dataPoint.Name,
                RationaleGuidance = dataPoint.RationaleGuidance,
                ResultDate = dataPoint.ResultDate,
                SGOId = dataPoint.SGOID,
                SubjectName = subjectName,
                TestType = GenerateTestType(dataPoint),
                VirtualTestId = dataPoint.VirtualTestID,
                Weight = dataPoint.Weight,
                TotalPoints = dataPoint.TotalPoints,
                ScoreType = dataPoint.ScoreType ?? 1
            };

            var dataPointFilters =
                _parameters.SgoDataPointFilterService.GetDataPointFilterBySGODataPointID(model.SGODataPointId);

            var masterStandardIds = dataPointFilters.Where(
                x => x.FilterType == (int)SGODataPointFilterEnum.StateStandard)
                .Select(x => x.FilterID).ToList();

            if (masterStandardIds.Any())
            {
                var masterStandards =
                _parameters.MasterStandardService.GetAll().Where(x => masterStandardIds.Contains(x.MasterStandardID)).ToList();

                // Sort list by level to view the first level having filter on layout
                model.StateStandardFilters = string.Join(";",
                    masterStandards.OrderBy(x => x.Level).Select(x => x.MasterStandardID));
            }

            model.TopicFilters = string.Join(";",
                dataPointFilters.Where(x => x.FilterType == (int)SGODataPointFilterEnum.Topic).Select(x => x.FilterID));
            model.SkillFilters = string.Join(";",
                dataPointFilters.Where(x => x.FilterType == (int)SGODataPointFilterEnum.Skill).Select(x => x.FilterID));
            model.OtherFilters = string.Join(";",
                dataPointFilters.Where(x => x.FilterType == (int)SGODataPointFilterEnum.Other).Select(x => x.FilterID));

            var clusterScoreFilters =
                _parameters.SgoDataPointClusterScoreService.GetDataPointClusterScoreBySGODataPointID(
                    model.SGODataPointId);
            model.ClusterScoreFilters = string.Join(";",
                clusterScoreFilters.Select(x => x.TestResultSubScoreName));
            var clusterScore = clusterScoreFilters.FirstOrDefault(
                    x => x.VirtualTestCustomSubScoreId.HasValue && x.VirtualTestCustomSubScoreId > 0);
            if (clusterScore != null)
                model.VirtualTestCustomSubScoreId = clusterScore.VirtualTestCustomSubScoreId;

            model.AttactScoreDownloadLink = GetLinkToDownloadAttachment(model.AttactScoreUrl);

            return model;

        }
        
        public string GetLinkToDownloadAttachment(string fileName)
        {
            var s3Domail = LinkitConfigurationManager.GetS3Settings().S3Domain;
            var sgobuketName = LinkitConfigurationManager.GetS3Settings().SGOBucketName;
            var sgoFolder = LinkitConfigurationManager.GetS3Settings().SGOFolder;
            var sgoFileName = string.Format("{0}/{1}/{2}/{3}", s3Domail, sgobuketName, sgoFolder, fileName);

            return sgoFileName;
        }

        public ActionResult GetTestType(int sgoId, string dataPointGroupType, int hasReview = 0)
        {
            var data = new List<SGOTestTypeItem>();
            var previewTestTeacherId = GetPreviewTestTeacherId();

            var sgoObject = _parameters.SGOObjectService.GetSGOByID(sgoId);
            var virtualTestCustomScores =
                    _parameters.VirtualTestCustomScoreService.Select().Where(x => x.DistrictId == sgoObject.DistrictID && !(x.IsMultiDate.HasValue && x.IsMultiDate.Value)).OrderBy(x => x.Name).ToList();

            var dataSetCategories = _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId)
                .Where(x => x.DataSetOriginID == 3 || x.DataSetOriginID == 11 || // Get legacy virtualtest only
                        (x.DataSetOriginID == 4 && x.BankAuthorId != previewTestTeacherId)) 
                .Select(x => new ListItem
                {
                    Id = x.DataSetCategoryID.GetValueOrDefault(),
                    Name = x.DataSetCategoryName
                }).Distinct().ToList()
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .ToList();

            if (hasReview == 1)
            {
                // Append more data when SGO is edit or review mode
                var datasetCategoryIds = _parameters.SgoDataPointService.GetPreAssessmentDataPointBySGOID(sgoId)
                                                                                .Where(x => x.DataSetCategoryID > 0)
                                                                                .Select(x => x.DataSetCategoryID)
                                                                                .ToArray();
                var expiredDatasetCategoryIds = datasetCategoryIds.Where(x => !dataSetCategories.Select(y => y.Id).Contains(x)).ToList();
                if (expiredDatasetCategoryIds.Count > 0)
                {
                    var expiredDatasetCategories = _parameters.DataSetCategoriesService.DataSetCategories
                        .Where(x => expiredDatasetCategoryIds.Contains(x.DataSetCategoryID))
                        .Select(x => new ListItem
                        {
                            Id = x.DataSetCategoryID,
                            Name = x.DataSetCategoryName
                        })
                        .ToList();

                    if (expiredDatasetCategories.Count > 0)
                    {
                        dataSetCategories.AddRange(expiredDatasetCategories);
                    }
                }
            }
            dataSetCategories = dataSetCategories.OrderBy(x => x.Name).ToList();

            if (dataPointGroupType == "PreAssessment")
            {
                data.Add(
                new SGOTestTypeItem
                {
                    Id = "PreAssessment",
                    Name = "Pre-Assessment LinkIt!" // OriginID = 1
                });

                data.AddRange(dataSetCategories.Select(item => new SGOTestTypeItem
                {
                    Id = "PreAssessmentHistorical_" + item.Id,
                    Name = "Pre-Assessment " + item.Name
                }));

                data.AddRange(virtualTestCustomScores.Select(item => new SGOTestTypeItem
                {
                    Id = "PreAssessmentCustom_" + item.VirtualTestCustomScoreId,
                    Name = "Pre-Assessment " + item.Name
                }));

                data.Add(
                    new SGOTestTypeItem
                    {
                        Id = "PreAssessmentExternal",
                        Name = "Pre-Assessment External Test" // OriginID 4
                    });
            }

            if (dataPointGroupType == "PostAssessment")
            {
                data.Add(
                new SGOTestTypeItem
                {
                    Id = "PostAssessment",
                    Name = "Post-Assessment LinkIt!" // OriginID = 1
                });

                var hasPreAssessmentCustom = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                    .FirstOrDefault(x => x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom) != null;

                // Just show Post Assessment datapoint at Preparation Approved step when already having PreAssessment Custom datapoint
                if (sgoObject.SGOStatusID != (int)SGOStatusType.PreparationApproved || hasPreAssessmentCustom)
                {
                    data.AddRange(virtualTestCustomScores.Select(item => new SGOTestTypeItem
                    {
                        Id = "PostAssessmentCustom_" + item.VirtualTestCustomScoreId,
                        Name = "Post-Assessment " + item.Name
                    }));
                }

                if (sgoObject.SGOStatusID == (int)SGOStatusType.PreparationApproved)
                {
                    data.AddRange(dataSetCategories
                        .Select(item => new SGOTestTypeItem
                        {
                            Id = "PostAssessmentHistorical_" + item.Id,
                            Name = "Post-Assessment " + item.Name
                        }));

                    data.Add(
                        new SGOTestTypeItem
                        {
                            Id = "PostAssessmentExternal",
                            Name = "Post-Assessment External Test"
                        });
                }

                data = AppendAlreadyExistedPostAssessmentTestType(sgoId, data, virtualTestCustomScores, dataSetCategories);
                
                data.Add(
                    new SGOTestTypeItem
                    {
                        Id = "PostAssessmentToBeCreated",
                        Name = "Post-Assessment - To Be Created"
                    });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<SGOTestTypeItem> AppendAlreadyExistedPostAssessmentTestType(int sgoId, List<SGOTestTypeItem> data, List<VirtualTestCustomScore> virtualTestCustomScores
            , IEnumerable<ListItem> achievementLevelSettings)
        {
            var postAssessmentSgoDatapoints = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId)
                    .Where(x => x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom
                        || x.Type == (int)SGODataPointTypeEnum.PostAssessmentExternal
                        || x.Type == (int)SGODataPointTypeEnum.PostAssessmentHistorical).ToList();
            foreach (var sgoDatapoint in postAssessmentSgoDatapoints)
            {
                var TestType = GenerateTestType(sgoDatapoint);
                if (data.All(x => x.Id != TestType))
                {
                    if (sgoDatapoint.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom)
                    {
                        data.AddRange(virtualTestCustomScores
                            .Where(x => "PostAssessmentCustom_" + x.VirtualTestCustomScoreId == TestType)
                            .Select(item => new SGOTestTypeItem
                            {
                                Id = "PostAssessmentCustom_" + item.VirtualTestCustomScoreId,
                                Name = "Post-Assessment " + item.Name
                            }));
                    }
                    else if (sgoDatapoint.Type == (int)SGODataPointTypeEnum.PostAssessmentHistorical)
                    {
                        data.AddRange(achievementLevelSettings
                            .Where(x => "PostAssessmentHistorical_" + x.Id == TestType)
                            .Select(item => new SGOTestTypeItem
                            {
                                Id = "PostAssessmentHistorical_" + item.Id,
                                Name = "Post-Assessment " + item.Name
                            }));
                    }
                    else if (sgoDatapoint.Type == (int)SGODataPointTypeEnum.PostAssessmentExternal)
                    {
                        data.Add(
                            new SGOTestTypeItem
                            {
                                Id = "PostAssessmentExternal",
                                Name = "Post-Assessment External Test"
                            });
                    }
                }
            }

            return data;
        }

        public ActionResult GetState(int sgoId)
        {
            var data = _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId)
                .Select(x => new { x.StateId, x.StateName })
                .ToList().Distinct().OrderBy(x => x.StateName)
                .Select(x => new ListItem
                {
                    Id = x.StateId,
                    Name = x.StateName
                });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterStandardState(int virtualTestId)
        {
            var data = _parameters.SgoSelectDataPointService.GetStateStandardsForSGO
                ("", "", "", CurrentUser.Id, virtualTestId)
                .Select(x => new { x.StateId, x.StateName }).Distinct()
                .Select(x => new ListItem
                {
                    Id = x.StateId,
                    Name = x.StateName
                }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterStandardSubject(int stateId, int virtualTestId)
        {
            var data = _parameters.SgoSelectDataPointService.GetStateStandardsForSGO
                ("", "", "", CurrentUser.Id, virtualTestId)
                .Where(x => x.StateId == stateId)
                .Select(x => new { x.SubjectName }).Distinct()
                .Select(x => new ListSubjectItem
                {
                    Id = x.SubjectName,
                    Name = x.SubjectName
                }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMasterStandardGrade(int stateId, string subjectName, int virtualTestId)
        {
            var data = _parameters.SgoSelectDataPointService.GetStateStandardsForSGO
                ("", "", "", CurrentUser.Id, virtualTestId)
                .Where(x => x.StateId == stateId && x.SubjectName.ToLower() == subjectName.ToLower())
                .Select(x => new { x.GradeId, x.GradeName }).Distinct()
                .Select(x => new ListItem
                {
                    Id = x.GradeId,
                    Name = x.GradeName
                }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStateStandards(int? stateId, string subjectName, string gradeName, int virtualTestId, int? parentMasterStandardId, int? childMasterStandardId, int? currentMasterStandardId)
        {
            var parser = new DataTableParser<MasterStandardViewModel>();
            if (virtualTestId == 0) // Return empty list to clear datatable
            {
                return Json(parser.Parse((new List<MasterStandardViewModel>()).AsQueryable()),
                JsonRequestBehavior.AllowGet);
            }

            var stateCode = "";
            if (stateId.HasValue)
            {
                var state = _parameters.StateService.GetStateById(stateId);
                stateCode = state.Code;
            }
            subjectName = HttpUtility.UrlDecode(subjectName) ?? "";
            gradeName = HttpUtility.UrlDecode(gradeName) ?? "";

            var query = _parameters.SgoSelectDataPointService.GetStateStandardsForSGO(stateCode, subjectName, gradeName,
                CurrentUser.Id, virtualTestId);
            if (currentMasterStandardId.HasValue)
            {
                var masterStandard =
                    _parameters.MasterStandardService.GetStandardsById(currentMasterStandardId.GetValueOrDefault());
                if (masterStandard != null)
                {
                    if (masterStandard.Level == 1)
                    {
                        query = query.Where(x => x.Level == 1).ToList();
                    }
                    else
                    {
                        query = query.Where(x => x.ParentGUID == masterStandard.ParentGUID).ToList();
                    }

                    // In case filter become invalid (post-assessment virtual test has test result)
                    if (query.Count == 0)
                    {
                        query = _parameters.SgoSelectDataPointService.GetStateStandardsForSGO(stateCode, subjectName,
                                    gradeName,
                                    CurrentUser.Id, virtualTestId).Where(x => x.Level == 1).ToList();
                    }
                }
            }
            else if (parentMasterStandardId.HasValue)
            {
                var masterStandard =
                    _parameters.MasterStandardService.GetStandardsById(parentMasterStandardId.GetValueOrDefault());

                if (masterStandard != null)
                {
                    query = query.Where(x => x.ParentGUID == masterStandard.GUID).ToList();
                }
            }
            else if (childMasterStandardId.HasValue)
            {
                var masterStandard =
                    _parameters.MasterStandardService.GetStandardsById(childMasterStandardId.GetValueOrDefault());
                if (masterStandard != null)
                {
                    if (masterStandard.Level == 2)
                    {
                        query = query.Where(x => x.Level == 1).ToList();
                    }
                    else
                    {
                        var parentMasterStandard = _parameters.MasterStandardService.GetStandardsByGuid(masterStandard.ParentGUID);
                        if (parentMasterStandard != null)
                        {
                            query = query.Where(x => x.ParentGUID == parentMasterStandard.ParentGUID).ToList();
                        }
                    }
                }                
            }
            else
            {
                query = query.Where(x => x.Level == 1).ToList();
            }

  
            var listStandard = query.GroupBy(x=> new { x.GUID , x.MasterStandardId , x.Description, x.Number, x.Level, x.Children, x.ParentGUID })                    
                    .Select(x => new MasterStandardViewModel
                    {
                        GUID = x.Key.GUID,
                        MasterStandardID = x.Key.MasterStandardId,
                        Description = x.Key.Description.ReplaceWeirdCharacters(),
                        Number = x.Key.Number,
                        Level = x.Key.Level,
                        Children = x.Key.Children.GetValueOrDefault(),
                        ParentGUID = x.Key.ParentGUID,
                        DescendantItemCount = x.Key.Children.GetValueOrDefault()
                    }).AsQueryable();


            //var listStandard = query

            //        .Select(x => new MasterStandardViewModel
            //        {
            //            GUID = x.GUID,
            //            MasterStandardID = x.MasterStandardId,
            //            Description = x.Description,
            //            Number = x.Number,
            //            Level = x.Level,
            //            Children = x.Children.GetValueOrDefault(),
            //            ParentGUID = x.ParentGUID,
            //            DescendantItemCount = x.Children.GetValueOrDefault()
            //        }).ToList().Distinct().AsQueryable();

            return Json(parser.Parse(listStandard, true),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTopicsOfVirtualTest(int virtualTestId)
        {
            var parser = new DataTableParser<Topic>();
            if (virtualTestId == 0)
            {
                return Json(parser.Parse(new List<Topic>().AsQueryable()), JsonRequestBehavior.AllowGet);
            }

            var data =
                _parameters.VirtualQuestionTopicService.Select().Where(x => x.VirtualTestId == virtualTestId)
                    .Select(x => new Topic
                    {
                        TopicID = x.TopicId,
                        Name = x.Name
                    }).Distinct().OrderBy(x => x.Name);
            return Json(parser.Parse(data.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSkillsOfVirtualTest(int virtualTestId)
        {
            var parser = new DataTableParser<LessonOne>();

            if (virtualTestId == 0)
            {
                return Json(parser.Parse(new List<LessonOne>().AsQueryable()), JsonRequestBehavior.AllowGet);
            }

            var data =
                _parameters.VirtualQuestionLessonOneService.Select().Where(x => x.VirtualTestId == virtualTestId)
                    .Select(x => new LessonOne
                    {
                        LessonOneID = x.LessonOneId,
                        Name = x.Name
                    }).Distinct().OrderBy(x => x.Name);
            return Json(parser.Parse(data.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOthersOfVirtualTest(int virtualTestId)
        {
            var parser = new DataTableParser<LessonTwo>();
            if (virtualTestId == 0)
            {
                return Json(parser.Parse(new List<LessonTwo>().AsQueryable()), JsonRequestBehavior.AllowGet);
            }

            var data =
                _parameters.VirtualQuestionLessonTwoService.Select().Where(x => x.VirtualTestId == virtualTestId)
                    .Select(x => new LessonTwo
                    {
                        LessonTwoID = x.LessonTwoId,
                        Name = x.Name
                    }).Distinct().OrderBy(x => x.Name);
            return Json(parser.Parse(data.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [SGOManagerLogFilter]
        public ActionResult DeleteDataPoint(DataPointViewModel model)
        {
            var sgoDataPoint = _parameters.SgoDataPointService.GetById(model.SGODataPointId);
            if (sgoDataPoint == null)
            {
                return Json(new { Success = false, ErrorMessage = "There is no Data Point" }, JsonRequestBehavior.AllowGet);
            }

            var permissionStatus = GetPermissionAccessSgoDataPoint(sgoDataPoint.SGOID);
            if (permissionStatus != (int)SGOPermissionEnum.FullUpdate)
            {
                return Json(new { Success = false, ErrorMessage = "Has no permision" }, JsonRequestBehavior.AllowGet);
            }

            RemoveDataPointRelevantData(model.SGODataPointId);
            _parameters.SgoDataPointService.Delete(new SGODataPoint { SGODataPointID = model.SGODataPointId });

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        private void SaveFilters(int sgoDataPointId, string filters, int filterType)
        {
            if (!string.IsNullOrEmpty(filters))
            {
                var filterIds = filters.Split(';');
                foreach (var filterId in filterIds)
                {
                    if (filterType == (int)SGODataPointFilterEnum.ClusterScore)
                    {
                        _parameters.SgoDataPointClusterScoreService.Save(new SGODataPointClusterScore
                        {
                            TestResultSubScoreName = filterId,
                            SGODataPointID = sgoDataPointId
                        });
                    }
                    else
                    {
                        _parameters.SgoDataPointFilterService.Save(new SGODataPointFilter
                        {
                            FilterType = filterType,
                            FilterID = Convert.ToInt32(filterId),
                            SGODataPointID = sgoDataPointId
                        });
                    }
                }
            }
        }

        private void RemoveDataPointRelevantData(int sgoDataPointId)
        {
            _parameters.SgoSelectDataPointService.RemoveDataPointRelevantData(sgoDataPointId);
        }

        private void SaveStudentDataPointFromVirtualTest(int sgoId, int sgoDataPointId, int virtualTestId)
        {
            _parameters.SgoSelectDataPointService.SaveStudentDataPointFromVirtualTest(sgoId, sgoDataPointId, virtualTestId);
        }

        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadAttachResult(HttpPostedFileBase postedFile)
        {
            string sgobuketName = LinkitConfigurationManager.GetS3Settings().SGOBucketName;
            string sgoFolder = LinkitConfigurationManager.GetS3Settings().SGOFolder;
            try
            {
                var fileName = string.Format("{0}_{1}", DateTime.Now.Ticks, postedFile.FileName);
                fileName = fileName.Replace("#", "_").Replace("%", "_").Replace("\\", "_").Replace("/", "_").Replace("+","_");
                string sgoFileName = string.Format("{0}/{1}", sgoFolder, fileName);
                var s3Result = _s3Service.UploadRubricFile(sgobuketName, sgoFileName, postedFile.InputStream);
                if (s3Result.IsSuccess)
                {
                    return Json(new { Success = true, FileName = fileName, attactscoredownloadlink = GetLinkToDownloadAttachment(fileName) }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Success = false, ErrorMessage = s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string ValidateDataPointNumberOfSgo(SGODataPoint sgoDataPoint)
        {
            if (sgoDataPoint == null || !CurrentUser.DistrictId.HasValue) return null;
            var dto = new SGODataPointSaveDTO();
            dto.SGODataPoint = sgoDataPoint;

            var districtID = CurrentUser.DistrictId.Value;
            dto.SGOMAXPostAssessment = _parameters.DistrictDecodeService.GetSGOMAXPostAssessment(districtID);
            dto.SGOMAXPreAssessment = _parameters.DistrictDecodeService.GetSGOMAXPreAssessment(districtID);

            var valid = _parameters.SgoDataPointService.CanSaveSGODataPoint(dto);
            if (valid) return null;

            var errorMessage = string.Format(MaxSGODataPointErrorFormat, dto.SGOMAXPreAssessment,
                dto.SGOMAXPostAssessment);

            return errorMessage;
        }

        //private bool ValidateDataPointNumberOfSgo(int sgoId, int sgoDataPointId, int type, ref string errorMessage)
        //{
        //    var dataPoints = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId).Where(x=>x.SGODataPointID != sgoDataPointId);

        //    if (type == (int) SGODataPointTypeEnum.PostAssessment ||
        //        type == (int) SGODataPointTypeEnum.PostAssessmentToBeCreated)
        //    {
        //        if (dataPoints.Any(x => x.Type == (int) SGODataPointTypeEnum.PostAssessment ||
        //                                x.Type == (int) SGODataPointTypeEnum.PostAssessmentToBeCreated))
        //        {
        //            errorMessage = "Maximum of one Post Assessment Data Point is allowed";
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        if (dataPoints.Count(x => x.Type != (int) SGODataPointTypeEnum.PostAssessment &&
        //                                  x.Type != (int) SGODataPointTypeEnum.PostAssessmentToBeCreated) >= 4)
        //        {
        //            errorMessage = "Maximum of 4 Pre-Assessment Data Points are allowed";
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        public ActionResult GetStartingPointInstruction(int? districtId)
        {
            if (!districtId.HasValue)
                districtId = CurrentUser.DistrictId;

            var isUseNewDesign = HelperExtensions.IsUseNewDesign(districtId.GetValueOrDefault());
            var labelSGOStartingPoints = isUseNewDesign ? Util.DistrictDecode_SGOStartingPoints + "_NewSkin" : Util.DistrictDecode_SGOStartingPoints;

            var districtDecode = _parameters.DistrictDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId.GetValueOrDefault(), labelSGOStartingPoints);

            var instructionMessage = "<div style='width: 500px'></div>";

            if (districtDecode != null)
                instructionMessage = districtDecode.Value;

            return Json(new { InstructionMessage = instructionMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRationaleInstruction(int? districtId)
        {
            if (!districtId.HasValue)
                districtId = CurrentUser.DistrictId;

            var isUseNewDesign = HelperExtensions.IsUseNewDesign(districtId.GetValueOrDefault());
            var labelSGORationale = isUseNewDesign ? Util.DistrictDecode_SGORationale + "_NewSkin" : Util.DistrictDecode_SGORationale;

            var districtDecode = _parameters.DistrictDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId.GetValueOrDefault(), labelSGORationale);

            var instructionMessage = "<div style='width: 500px'></div>";

            if (districtDecode != null)
                instructionMessage = districtDecode.Value;

            return Json(new { InstructionMessage = instructionMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomScoreType(int virtualTestCustomScoreId, int sgoId, bool? isPostAssignment = false, string scoreType = "")
        {
            return Json(GetAssessmentScoreTypes(0, sgoId, virtualTestCustomScoreId, isPostAssignment, scoreType), JsonRequestBehavior.AllowGet);
        }

        private List<ListItemStr> BuildSubScoreType(VirtualTestCustomSubScore virtualTestCustomSubScore)
        {
            var data = new List<ListItemStr>();
            if (virtualTestCustomSubScore.UseRaw)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreRaw, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Raw", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UseScaled)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreScaled, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Scaled", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UsePercentile)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScorePercentage, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Percentage", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UsePercent)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScorePercent, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Percent", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UseIndex)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreIndex, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Index", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UseLexile)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreLexile, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - Score Lexile", virtualTestCustomSubScore.Name) });

            if (virtualTestCustomSubScore.UseCustomN1 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomN1, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomN1Label) });

            if (virtualTestCustomSubScore.UseCustomN2 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomN2, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomN2Label) });

            if (virtualTestCustomSubScore.UseCustomN3 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomN3, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomN3Label) });

            if (virtualTestCustomSubScore.UseCustomN4 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomN4, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomN4Label) });

            if (virtualTestCustomSubScore.UseCustomA1 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomA1, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomA1Label) });

            if (virtualTestCustomSubScore.UseCustomA2 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomA2, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomA2Label) });

            if (virtualTestCustomSubScore.UseCustomA3 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomA3, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomA3Label) });

            if (virtualTestCustomSubScore.UseCustomA4 ?? false)
                data.Add(new ListItemStr { Id = string.Format("{0}_{1}", (int)SGOScoreTypeEnum.ScoreCustomA4, virtualTestCustomSubScore.VirtualTestCustomSubScoreId), Name = string.Format("{0} - {1}", virtualTestCustomSubScore.Name, virtualTestCustomSubScore.CustomA4Label) });

            return data;
        } 

        public ActionResult GetAssessmentScoreType(int virtualTestId, int sgoId = 0, int? customScoreId = null, bool? isPostAssignment = false, string scoreType = "")
        {
            return Json(GetAssessmentScoreTypes(virtualTestId, sgoId, customScoreId, isPostAssignment, scoreType), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssessmentHistoricalScoreType(int virtualTestId, int sgoId = 0, bool? isPostAssignment = false, string scoreType = "")
        {
            return Json(GetAssessmentScoreTypes(virtualTestId, sgoId, isPostAssignment: isPostAssignment, scoreType: scoreType), JsonRequestBehavior.AllowGet);
        }

        private VirtualTestData GetVirtualTestOfDataPoint(int sgoDataPointId)
        {
            var sgoDataPoint = _parameters.SgoDataPointService.GetById(sgoDataPointId);
            if (sgoDataPoint != null)
            {
                return 
                    _parameters.VirtualTestService.GetTestById(sgoDataPoint.VirtualTestID.GetValueOrDefault());
            }

            return null;
        }

        private Subject GetSubjectOfDataPoint(int sgoDataPointId)
        {
            var virtualTest = GetVirtualTestOfDataPoint(sgoDataPointId);
            if (virtualTest != null)
            {
                var bank = _parameters.BankService.GetBankById(virtualTest.BankID);

                if (bank != null)
                {
                    return _parameters.SubjectService.GetSubjectById(bank.SubjectID);
                }
            }

            return null;
        }

        private Grade GetGradeOfDataPoint(int sgoDataPointId)
        {
            var subject = GetSubjectOfDataPoint(sgoDataPointId);
            if (subject != null)
            {
                return _parameters.GradeService.GetGradeById(subject.GradeId);
            }

            return null;
        }

        private void AppendCurrentSubjectOfDataPoint(int? sgoDataPointId, SGODataPointTypeEnum dataPointType, List<ListSubjectItem> subjectItems)
        {
            if (sgoDataPointId.HasValue && sgoDataPointId.Value > 0)
            {
                var sgoDataPoint = _parameters.SgoDataPointService.GetById(sgoDataPointId.Value);
                if (sgoDataPoint != null && sgoDataPoint.Type == (int) dataPointType) // Just append when reload datapoint (do not append when use change datapoint type)
                {
                    var subject = GetSubjectOfDataPoint(sgoDataPointId.Value);
                    if (subject != null && subjectItems.All(x => x.Name != subject.Name))
                    {
                        subjectItems.Add(new ListSubjectItem
                        {
                            Id = subject.Name,
                            Name = subject.Name
                        });
                    }
                    subjectItems = subjectItems.OrderBy(x => x.Name).ToList();
                }
            }            
        }

        private void AppendCurrentGradeOfDataPoint(int? sgoDataPointId, SGODataPointTypeEnum dataPointType, string subjectName, List<ListItem> gradeItems)
        {
            if (sgoDataPointId.HasValue && sgoDataPointId.Value > 0)
            {
                var sgoDataPoint = _parameters.SgoDataPointService.GetById(sgoDataPointId.Value);
                if (sgoDataPoint.Type == (int) dataPointType)
                    // Just append when reload datapoint (do not append when use change datapoint type)
                {
                    var grade = GetGradeOfDataPoint(sgoDataPointId.Value);
                    if (grade != null)
                    {
                        if (gradeItems.All(x => x.Id != grade.Id))
                        {
                            var subject = GetSubjectOfDataPoint(sgoDataPointId.Value);
                            // Just append when selected subject is matched with current subject of datapoint
                            if (subject.Name == subjectName)
                            {
                                gradeItems.Add(new ListItem
                                {
                                    Id = grade.Id,
                                    Name = grade.Name
                                });
                            }
                        }
                            
                    }
                }
            }
        }

        private void AppendCurrentVirtualTestOfDataPoint(int? sgoDataPointId, SGODataPointTypeEnum dataPointType, string subjectName, int gradeId,
            List<ListItem> virtualTestItems)
        {
            if (sgoDataPointId.HasValue && sgoDataPointId.Value > 0)
            {
                var sgoDataPoint = _parameters.SgoDataPointService.GetById(sgoDataPointId.Value);
                if (sgoDataPoint.Type == (int) dataPointType)
                    // Just append when reload datapoint (do not append when use change datapoint type)
                {
                    var virtualTest = GetVirtualTestOfDataPoint(sgoDataPointId.Value);
                    if (virtualTest != null)
                    {
                        if (virtualTestItems.All(x => x.Id != virtualTest.VirtualTestID))
                        {
                            var subject = GetSubjectOfDataPoint(sgoDataPointId.Value);
                            var grade = GetGradeOfDataPoint(sgoDataPointId.Value);
                            
                            // Just append when selected subject & grade is matched with current subject & grade of datapoint
                            if (subject.Name == subjectName && grade.Id == gradeId)
                            {
                                virtualTestItems.Add(new ListItem
                                {
                                    Id = virtualTest.VirtualTestID,
                                    Name = virtualTest.Name
                                });
                            }
                        }

                        virtualTestItems = virtualTestItems.OrderBy(x => x.Name).ToList();
                    }
                }
            }
        }

        private void UpdateModelWithDistrictDecode(SelectDataPointViewModel model)
        {
            var districtID = CurrentUser.DistrictId;
            if (!districtID.HasValue) return;

            model.SGOMAXPreAssessment = _parameters.DistrictDecodeService.GetSGOMAXPreAssessment(districtID.Value);
            model.SGOMAXPostAssessment = _parameters.DistrictDecodeService.GetSGOMAXPostAssessment(districtID.Value);
        }

        private void SyncScoreTypeOfCustomDataPoint(SGODataPoint currentDataPoint)
        {
            var customDataPoints =
                _parameters.SgoDataPointService.GetDataPointBySGOID(currentDataPoint.SGOID)
                    .Where(x => x.SGODataPointID != currentDataPoint.SGODataPointID
                                && (x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom ||
                                    x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom));
            foreach (var sgoDataPoint in customDataPoints)
            {
                if (sgoDataPoint.ScoreType != currentDataPoint.ScoreType)
                {
                    sgoDataPoint.ScoreType = currentDataPoint.ScoreType;
                    _parameters.SgoDataPointService.Save(sgoDataPoint);
                }
            }
        }

        private bool IsCustomDataPointTypeValid(DataPointViewModel model)
        {
            var customSgoDataPoint = _parameters.SgoDataPointService.GetDataPointBySGOID(model.SGOId)
                .FirstOrDefault(x => x.SGODataPointID != model.SGODataPointId &&
                                     (x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom ||
                                      x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom));
            if (customSgoDataPoint != null &&
                customSgoDataPoint.AchievementLevelSettingID != model.DataSetCategoryID)
            {
                return false;
            }

            return true;
        }

        private SGODataPoint AssignSgoDataPointData(SGODataPoint sgoDataPoint)
        {
            if (sgoDataPoint.SGODataPointID == 0)
            {
                return sgoDataPoint;
            }

            var dbSgoDataPoint = _parameters.SgoDataPointService.GetById(sgoDataPoint.SGODataPointID);
            if (dbSgoDataPoint != null)
            {
                dbSgoDataPoint.Type = sgoDataPoint.Type;
                dbSgoDataPoint.GradeID = sgoDataPoint.GradeID;
                dbSgoDataPoint.Name = sgoDataPoint.Name;
                dbSgoDataPoint.RationaleGuidance = sgoDataPoint.RationaleGuidance;
                dbSgoDataPoint.SGODataPointID = sgoDataPoint.SGODataPointID;
                dbSgoDataPoint.SGOID = sgoDataPoint.SGOID;
                dbSgoDataPoint.SubjectName = sgoDataPoint.SubjectName;
                dbSgoDataPoint.VirtualTestID = sgoDataPoint.VirtualTestID;
                dbSgoDataPoint.ScoreType = sgoDataPoint.ScoreType;
                dbSgoDataPoint.AchievementLevelSettingID = sgoDataPoint.AchievementLevelSettingID;
                dbSgoDataPoint.TotalPoints = sgoDataPoint.TotalPoints;
                dbSgoDataPoint.ResultDate = sgoDataPoint.ResultDate;
                dbSgoDataPoint.AttachScoreUrl = sgoDataPoint.AttachScoreUrl;
            }

            return dbSgoDataPoint;
        }

        #endregion

        #region Post Assessment
        public ActionResult GetSubjectsPostAssessment(int sgoId, int? sgoDataPointId)
        {
            var sgoObject = _parameters.SGOObjectService.GetSGOByID(sgoId);
            // If SGO at Preparation Approved step ==> Load test that students have taken (the same with pre-assessment list)
            if (sgoObject.SGOStatusID == (int)SGOStatusType.PreparationApproved)
            {
                var data = PreAssessmentGetSubject(sgoId, sgoDataPointId, SGODataPointTypeEnum.PostAssessment, null);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            var criteria = new SearchBankCriteria
            {
                DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0,
                UserId = CurrentUser.Id,
                UserRole = CurrentUser.RoleId
            };

            var subjects = _parameters.SubjectService.GetSubjectsByGradeIdAndAuthor(criteria); // by-pass grade information to get all subject
            if (subjects != null)
            {                
                var data = subjects.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).ToList();

                AppendCurrentSubjectOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PostAssessment, data);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<ListSubjectItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGradesPostAssessment(int sgoId, int? sgoDataPointId, string subjectName)
        {
            var sgoObject = _parameters.SGOObjectService.GetSGOByID(sgoId);
            // If SGO at Preparation Approved step ==> Load test that students have taken (the same with pre-assessment list)
            if (sgoObject.SGOStatusID == (int)SGOStatusType.PreparationApproved)
            {
                var data = PreAssessmentGetGrade(sgoId, sgoDataPointId, SGODataPointTypeEnum.PostAssessment, subjectName);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            var criteria = new SearchBankCriteria
            {
                DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0,
                UserId = CurrentUser.Id,
                UserRole = CurrentUser.RoleId
            };

            var subjects = _parameters.SubjectService.GetSubjectsByGradeIdAndAuthor(criteria); // by-pass grade information to get all subject
            if (subjects != null)
            {
                var gradeIds = subjects.Where(x => x.Name == subjectName).Select(x => x.GradeId).Distinct().ToList();
                var grades = _parameters.GradeService.GetGradesByGradeIdList(gradeIds).OrderBy(x => x.Order);

                var data = grades.Select(x => new ListItem { Id = x.Id, Name = x.Name }).ToList();
                AppendCurrentGradeOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PostAssessment, subjectName, data);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestsPostAssessment(int sgoId, int? sgoDataPointId, string subjectName, int gradeId)
        {
            var sgoObject = _parameters.SGOObjectService.GetSGOByID(sgoId);
            // If SGO at Preparation Approved step ==> Load test that students have taken (the same with pre-assessment list)
            if (sgoObject.SGOStatusID == (int)SGOStatusType.PreparationApproved)
            {
                var data = PreAssessmentGetTest(sgoId, sgoDataPointId, SGODataPointTypeEnum.PostAssessment, subjectName, gradeId);
                return LargeJson(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var takenVirtualTestIds = _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId)
                //    .Select(x => x.VirtualTestId).Distinct().ToList();

                var districtId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

                // Get all virtual tests haven't been taken by these students
                var data = _parameters.SgoSelectDataPointService.GetAccessVirtualTests(null, subjectName, districtId,
                    gradeId, CurrentUser.Id, CurrentUser.RoleId).ToList()
                    //.Where(x => !takenVirtualTestIds.Contains(x.VirtualTestId))
                    .Select(x => new ListItem { Id = x.VirtualTestId, Name = x.Name })
                    .OrderBy(x => x.Name).ToList();

                AppendCurrentVirtualTestOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PostAssessment, subjectName, gradeId, data);
                return LargeJson(data, JsonRequestBehavior.AllowGet);
            }
        }

        private int GetPermissionAccessPostAssessment(DataPointViewModel model)
        {
            // Allow to save PostAssessment in case change from Post To Be Created datapoint
            if (model.SGODataPointId > 0)
            {
                var currentSgoDataPoint = _parameters.SgoDataPointService.GetById(model.SGODataPointId);
                if (currentSgoDataPoint != null &&
                    currentSgoDataPoint.Type == (int) SGODataPointTypeEnum.PostAssessmentToBeCreated)
                {
                    return (int) SGOPermissionEnum.FullUpdate;
                }
            }

            var permissionStatus = GetPermissionAccessSgoDataPoint(model.SGOId);

            //if (!_parameters.VulnerabilityService.ChecUserCanAccessVirtualTest(CurrentUser,
            //    model.VirtualTestId.GetValueOrDefault()))
            //{
            //    permissionStatus = (int)SGOPermissionEnum.NotAvalible;
            //}

            return permissionStatus;
        }

        [SGOManagerLogFilter]
        [ValidateInput(false)]
        public ActionResult SavePostAssessment(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessPostAssessment(model);
            if (permissionStatus != (int)SGOPermissionEnum.FullUpdate)
            {                
                return Json(new { Success = false, ErrorMessage = "Has no permision" }, JsonRequestBehavior.AllowGet);
            }

            var sgoDataPoint = new SGODataPoint
            {
                Type = (int)SGODataPointTypeEnum.PostAssessment,
                Name = model.Name ?? "",
                RationaleGuidance = model.RationaleGuidance ?? "",
                SGODataPointID = model.SGODataPointId,
                SGOID = model.SGOId,
                VirtualTestID = model.VirtualTestId,
                ScoreType = model.ScoreType,
                AttachScoreUrl = model.AttactScoreUrl,
            };

            if (sgoDataPoint.SGODataPointID > 0)
                sgoDataPoint = AssignSgoDataPointData(sgoDataPoint);

            var errorMessage = ValidateDataPointNumberOfSgo(sgoDataPoint);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                return Json(new { Success = false, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            _parameters.SgoDataPointService.Save(sgoDataPoint);

            if (!CheckImprovementScoringPlanDataPointType(model) || !CheckImprovementScoringPlanScoreType(model))
            {
                var sgo = _parameters.SGOObjectService.GetSGOByID(model.SGOId);
                sgo.TargetScoreType = (int)SGOTargetScoreTypeEnum.ManualScoring;
                _parameters.SGOObjectService.Save(sgo);
            }

            if (model.SGODataPointId > 0)
                RemoveDataPointRelevantData(sgoDataPoint.SGODataPointID);

            // Init when create new
            if (model.SGODataPointId == 0)
                _parameters.SgoDataPointService.InitDefaultBandForDataPoint(sgoDataPoint.SGODataPointID);

            SaveFilters(sgoDataPoint.SGODataPointID, model.StateStandardFilters, (int)SGODataPointFilterEnum.StateStandard);
            SaveFilters(sgoDataPoint.SGODataPointID, model.TopicFilters, (int)SGODataPointFilterEnum.Topic);
            SaveFilters(sgoDataPoint.SGODataPointID, model.SkillFilters, (int)SGODataPointFilterEnum.Skill);
            SaveFilters(sgoDataPoint.SGODataPointID, model.OtherFilters, (int)SGODataPointFilterEnum.Other);

            return Json(new { Success = true, SgoDataPointId = sgoDataPoint.SGODataPointID }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidateImprovementScoringPlanDataPointTypeAndScoreType(DataPointViewModel model)
        {
            //if (!CheckImprovementScoringPlanDataPointType(model))
            //{
            //    return Json(new { Success = false, ErrorMessage = "The score type you have chosen for your post-assessment cannot be compared with the baseline pre-assessment selected or cannot be used for auto-scoring your SGO. Would you like to proceed with manual scoring?" }, JsonRequestBehavior.AllowGet);
            //}

            //if (!CheckImprovementScoringPlanScoreType(model))
            //{
            //    return Json(new { Success = false, ErrorMessage = "Your SGO cannot be auto-scored based on the post-assessment you have selected. This could be because you have selected a post-assessment with a score type is incompatible with the pre-assessment. If you would like to proceed, you will need to manually score your SGO." }, JsonRequestBehavior.AllowGet);
            //}
            var result = CheckImprovementScoringPlanDataPointType(model) && CheckImprovementScoringPlanScoreType(model);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        public bool CheckImprovementScoringPlanDataPointType(DataPointViewModel model)
        {
            var sgo = _parameters.SGOObjectService.GetSGOByID(model.SGOId);

            if (sgo.SGOStatusID == (int) SGOStatusType.PreparationApproved)
            {
                if (model.TestType.Contains("PostAssessmentCustom_") &&
                    (model.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA1 ||
                    model.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA2 ||
                    model.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA3 ||
                    model.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA4) &&
                    sgo.TargetScoreType != (int) SGOTargetScoreTypeEnum.ManualScoring)
                {
                    return false;
                }

                if (sgo.TargetScoreType == (int) SGOTargetScoreTypeEnum.ImproveOnPostAssessment)
                {
                    var preAssessmentDatapoint = _parameters.SgoDataPointService.GetDataPointBySGOID(model.SGOId)
                        .FirstOrDefault(
                            x =>
                                (x.Type == (int) SGODataPointTypeEnum.PreAssessment ||
                                 x.Type == (int) SGODataPointTypeEnum.PreAssessmentExternal ||
                                 x.Type == (int) SGODataPointTypeEnum.PreAssessmentHistorical)
                                && x.ImprovementBasedDataPoint == 1);

                    if (preAssessmentDatapoint != null)
                    {
                        var postAssessmentTestType = 0;
                        if (model.TestType == "PostAssessment")
                            postAssessmentTestType = (int) SGODataPointTypeEnum.PostAssessment;
                        if (model.TestType == "PostAssessmentExternal")
                            postAssessmentTestType = (int) SGODataPointTypeEnum.PostAssessmentExternal;
                        if (model.TestType.Contains("PostAssessmentHistorical_"))
                            postAssessmentTestType = (int) SGODataPointTypeEnum.PostAssessmentHistorical;

                        if ((postAssessmentTestType == (int) SGODataPointTypeEnum.PostAssessment &&
                             preAssessmentDatapoint.Type != (int) SGODataPointTypeEnum.PreAssessment)
                            || (postAssessmentTestType == (int) SGODataPointTypeEnum.PostAssessmentExternal &&
                                preAssessmentDatapoint.Type != (int) SGODataPointTypeEnum.PreAssessmentExternal)
                            || (postAssessmentTestType == (int) SGODataPointTypeEnum.PostAssessmentHistorical &&
                                preAssessmentDatapoint.Type != (int) SGODataPointTypeEnum.PreAssessmentHistorical))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool CheckImprovementScoringPlanScoreType(DataPointViewModel model)
        {
            var sgo = _parameters.SGOObjectService.GetSGOByID(model.SGOId);
            var sgoDataPoint = new SGODataPoint
            {
                Type = (int) SGODataPointTypeEnum.PostAssessment,
                GradeID = model.GradeId,
                Name = model.Name ?? "",
                RationaleGuidance = model.RationaleGuidance ?? "",
                SGODataPointID = model.SGODataPointId,
                SGOID = model.SGOId,
                SubjectName = model.SubjectName ?? "",
                VirtualTestID = model.VirtualTestId,
                ScoreType = model.ScoreType
            };

            if (sgo.TargetScoreType == (int)SGOTargetScoreTypeEnum.ImproveOnPostAssessment && sgo.SGOStatusID == (int)SGOStatusType.PreparationApproved)
            {
                if (!sgoDataPoint.ScoreType.HasValue)
                    return false;

                var preAssessmentDatapoint = _parameters.SgoDataPointService.GetDataPointBySGOID(model.SGOId)
                    .FirstOrDefault(
                        x => x.ImprovementBasedDataPoint == 1
                             && (x.Type == (int) SGODataPointTypeEnum.PreAssessment ||
                                 x.Type == (int) SGODataPointTypeEnum.PreAssessmentHistorical ||
                                 x.Type == (int) SGODataPointTypeEnum.PreAssessmentCustom));

                if (preAssessmentDatapoint != null &&
                    preAssessmentDatapoint.ScoreType != sgoDataPoint.ScoreType)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Pre Assessment
        public ActionResult GetSubjectPreAssessment(int sgoId, int? sgoDataPointId, int? stateId)
        {
            var data = PreAssessmentGetSubject(sgoId, sgoDataPointId, SGODataPointTypeEnum.PreAssessment, stateId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<ListSubjectItem> PreAssessmentGetSubject(int sgoId, int? sgoDataPointId, SGODataPointTypeEnum dataPointType, int? stateId)
        {
            var subjectItems = _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId)
                .Where(x => (!stateId.HasValue || x.StateId == stateId.GetValueOrDefault()) && x.VirtualTestSourceId == 1)
                .GroupBy(x => x.SubjectName).OrderBy(x => x.Key).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).ToList();

            AppendCurrentSubjectOfDataPoint(sgoDataPointId, dataPointType, subjectItems);
            return subjectItems;
        }

        public ActionResult GetGradePreAssessment(int sgoId, int? sgoDataPointId, string subjectName)
        {
            var data = PreAssessmentGetGrade(sgoId, sgoDataPointId, SGODataPointTypeEnum.PreAssessment, subjectName);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<ListItem> PreAssessmentGetGrade(int sgoId, int? sgoDataPointId, SGODataPointTypeEnum dataPointType, string subjectName)
        {
            var listItems = _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId)
                .Where(x => x.SubjectName == subjectName && x.VirtualTestSourceId == 1)
                .Select(x => new { x.GradeId, x.GradeName, x.GradeOrder }).Distinct()
                .OrderBy(x => x.GradeOrder)
                .Select(x => new ListItem
                {
                    Id = x.GradeId,
                    Name = x.GradeName
                }).ToList();

            AppendCurrentGradeOfDataPoint(sgoDataPointId, dataPointType, subjectName, listItems);
            return listItems;
        }

        public ActionResult GetTestPreAssessment(int sgoId, int? sgoDataPointId, string subjectName, int gradeId)
        {
            var data = PreAssessmentGetTest(sgoId, sgoDataPointId, SGODataPointTypeEnum.PreAssessment,  subjectName, gradeId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<ListItem> PreAssessmentGetTest(int sgoId, int? sgoDataPointId, SGODataPointTypeEnum dataPointType, string subjectName, int gradeId)
        {
            var listItems = _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId)
                .Where(x => x.SubjectName == subjectName && x.GradeId == gradeId && x.VirtualTestSourceId == 1)
                .Select(x => new { x.VirtualTestId, x.VirtualTestName }).Distinct().OrderBy(x => x.VirtualTestName)
                .Select(x => new ListItem
                {
                    Id = x.VirtualTestId,
                    Name = x.VirtualTestName
                }).ToList();

            AppendCurrentVirtualTestOfDataPoint(sgoDataPointId, dataPointType, subjectName, gradeId, listItems);
            return listItems;
        }

        private int GetPermissionAccessPreAssessment(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessSgoDataPoint(model.SGOId);
            
            //if (!_parameters.VulnerabilityService.ChecUserCanAccessVirtualTest(CurrentUser,
            //    model.VirtualTestId.GetValueOrDefault()))
            //{
            //    permissionStatus = (int)SGOPermissionEnum.NotAvalible;
            //}
            
            return permissionStatus;
        }

        [SGOManagerLogFilter]
        public ActionResult SavePreAssessment(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessPreAssessment(model);
            if (permissionStatus != (int)SGOPermissionEnum.MinorUpdate && permissionStatus != (int)SGOPermissionEnum.FullUpdate)
            {
                return Json(new { Success = false, ErrorMessage = "Has no permission" }, JsonRequestBehavior.AllowGet);
            }
            //Check virtual test
            var virtualTests = PreAssessmentGetTest(model.SGOId, model.SGODataPointId, SGODataPointTypeEnum.PostAssessment, model.SubjectName, model.GradeId);
            if(!virtualTests.Any(x=>x.Id==model.VirtualTestId))
            {
                return Json(new { Success = false, ErrorMessage = "Virtual Test is invalid" }, JsonRequestBehavior.AllowGet);
            }
            var sgoDataPoint = new SGODataPoint
            {
                Type = (int)SGODataPointTypeEnum.PreAssessment,
                Name = model.Name ?? "",
                RationaleGuidance = model.RationaleGuidance ?? "",
                SGODataPointID = model.SGODataPointId,
                SGOID = model.SGOId,
                VirtualTestID = model.VirtualTestId,
                ScoreType = model.ScoreType,
                IsTemporary = model.CreateTemporaryExternalVirtualTest ?? false
            };

            if (sgoDataPoint.SGODataPointID > 0)
                sgoDataPoint = AssignSgoDataPointData(sgoDataPoint);

            var errorMessage = model.BypassDataPointNumberRestriction == 1 ? null: ValidateDataPointNumberOfSgo(sgoDataPoint);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                return Json(new { Success = false, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            _parameters.SgoDataPointService.Save(sgoDataPoint);

            if (model.SGODataPointId > 0)
                RemoveDataPointRelevantData(sgoDataPoint.SGODataPointID);

            // Init when create new
            if (model.SGODataPointId == 0)
                _parameters.SgoDataPointService.InitDefaultBandForDataPoint(sgoDataPoint.SGODataPointID);

            SaveStudentDataPointFromVirtualTest(sgoDataPoint.SGOID, sgoDataPoint.SGODataPointID, sgoDataPoint.VirtualTestID.GetValueOrDefault());
            SaveFilters(sgoDataPoint.SGODataPointID, model.StateStandardFilters, (int)SGODataPointFilterEnum.StateStandard);
            SaveFilters(sgoDataPoint.SGODataPointID, model.TopicFilters, (int)SGODataPointFilterEnum.Topic);
            SaveFilters(sgoDataPoint.SGODataPointID, model.SkillFilters, (int)SGODataPointFilterEnum.Skill);
            SaveFilters(sgoDataPoint.SGODataPointID, model.OtherFilters, (int)SGODataPointFilterEnum.Other);

            return Json(new { Success = true, SgoDataPointId = sgoDataPoint.SGODataPointID }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Pre Assessment Historical
        public ActionResult GetSubjectPreAssessmentHistorical(int sgoId, int? sgoDataPointId, int dataSetCategoryID, int? stateId)
        {
            var previewTestTeacherId = GetPreviewTestTeacherId();

            var data = _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId)
                .Where(x => x.DataSetCategoryID == dataSetCategoryID &&
                            (!stateId.HasValue || x.StateId == stateId.GetValueOrDefault()) && x.VirtualTestType == null &&
                            x.VirtualTestSourceId == 3
                            && x.BankAuthorId != previewTestTeacherId // Include External virtual test (created in preview test teacher banks)
                            )
                .GroupBy(x => x.SubjectName)
                .OrderBy(x => x.Key)
                .Select(x => new ListSubjectItem {Id = x.Key, Name = x.Key})
                .ToList();

            AppendCurrentSubjectOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PreAssessmentHistorical, data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGradePreAssessmentHistorical(int sgoId, int? sgoDataPointId, int dataSetCategoryID, string subjectName)
        {
            var previewTestTeacherId = GetPreviewTestTeacherId();

            var data = _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId)
                .Where(x =>
                    x.DataSetCategoryID == dataSetCategoryID && x.SubjectName == subjectName
                    && x.VirtualTestType == null
                    && x.VirtualTestSourceId == 3
                    && x.BankAuthorId != previewTestTeacherId // Include External virtual test (created in preview test teacher banks)
                    )
                .Select(x => new {x.GradeId, x.GradeName, x.GradeOrder})
                .Distinct()
                .OrderBy(x => x.GradeOrder)
                .Select(x => new ListItem
                {
                    Id = x.GradeId,
                    Name = x.GradeName
                }).ToList();

            AppendCurrentGradeOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PreAssessmentHistorical, subjectName, data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestPreAssessmentHistorical(int sgoId, int? sgoDataPointId, int dataSetCategoryID, string subjectName, int gradeId)
        {
            var data = _parameters.SgoSelectDataPointService.GetTestPreAssessmentHistorical(sgoId, dataSetCategoryID, subjectName, gradeId, GetPreviewTestTeacherId());

            AppendCurrentVirtualTestOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PreAssessmentHistorical, subjectName, gradeId, data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private int GetPreviewTestTeacherId()
        {
            var previewTestTeacherId = 0;
            var configurationValue =
                _parameters.ConfigurationService.Select()
                    .Where(o => o.Name == Constanst.Configuration_PreviewTestTeacherID)
                    .Select(o => o.Value)
                    .FirstOrDefault();
            int.TryParse(configurationValue, out previewTestTeacherId);

            if (previewTestTeacherId == 0)
                previewTestTeacherId = CurrentUser.Id;

            return previewTestTeacherId;
        }

        public ActionResult GetTestResultSubScoreNameOfVirtualTest(int virtualTestId)
        {
            var parser = new DataTableParser<SGOGetTestResultSubScoreNameOfVirtualTestData>();

            if (virtualTestId == 0)
            {
                return Json(parser.Parse(new List<SGOGetTestResultSubScoreNameOfVirtualTestData>().AsQueryable()), JsonRequestBehavior.AllowGet);
            }

            var data =
                _parameters.SgoSelectDataPointService.GetTestResultSubScoreNameOfVirtualTest(virtualTestId);

            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        private int GetPermissionAccessPreAssessmentHistorical(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessSgoDataPoint(model.SGOId);

            //if (!_parameters.VulnerabilityService.ChecUserCanAccessVirtualTest(CurrentUser,
            //    model.VirtualTestId.GetValueOrDefault()))
            //{
            //    permissionStatus = (int)SGOPermissionEnum.NotAvalible;
            //}

            return permissionStatus;
        }

        [SGOManagerLogFilter]
        public ActionResult SavePreAssessmentHistorical(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessPreAssessmentHistorical(model);
            if (permissionStatus != (int)SGOPermissionEnum.FullUpdate && permissionStatus != (int)SGOPermissionEnum.MinorUpdate)
            {
                return Json(new { Success = false, ErrorMessage = "Has no permision" }, JsonRequestBehavior.AllowGet);
            }
            //Check virtual test
            var virtualTests = _parameters.SgoSelectDataPointService.GetTestPreAssessmentHistorical(model.SGOId,
                model.DataSetCategoryID.GetValueOrDefault(), model.SubjectName, model.GradeId, GetPreviewTestTeacherId());
            if (!virtualTests.Any(x => x.Id == model.VirtualTestId))
            {
                return Json(new { Success = false, ErrorMessage = "Virtual Test is invalid" }, JsonRequestBehavior.AllowGet);
            }


            var sgoDataPoint = new SGODataPoint
            {
                Type = (int)SGODataPointTypeEnum.PreAssessmentHistorical,
                Name = model.Name ?? "",
                RationaleGuidance = model.RationaleGuidance ?? "",
                SGODataPointID = model.SGODataPointId,
                SGOID = model.SGOId,
                VirtualTestID = model.VirtualTestId,
                //TODO: [TestType] will remove AchievementLevelSettingID 
                AchievementLevelSettingID =  model.DataSetCategoryID.GetValueOrDefault(), 
                ScoreType = model.ScoreType,
                IsTemporary = model.CreateTemporaryExternalVirtualTest ?? false,
            };

            if (sgoDataPoint.SGODataPointID > 0)
                sgoDataPoint = AssignSgoDataPointData(sgoDataPoint);

            var errorMessage = model.BypassDataPointNumberRestriction == 1
                ? null
                : ValidateDataPointNumberOfSgo(sgoDataPoint);

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                return Json(new { Success = false, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            _parameters.SgoDataPointService.Save(sgoDataPoint);

            if (model.SGODataPointId > 0)
                RemoveDataPointRelevantData(sgoDataPoint.SGODataPointID);

            // Init when create new
            if (model.SGODataPointId == 0)
                _parameters.SgoDataPointService.InitDefaultBandForDataPoint(sgoDataPoint.SGODataPointID);

            SaveStudentDataPointFromVirtualTest(sgoDataPoint.SGOID, sgoDataPoint.SGODataPointID, sgoDataPoint.VirtualTestID.GetValueOrDefault());
            SaveFilters(sgoDataPoint.SGODataPointID, model.ClusterScoreFilters, (int)SGODataPointFilterEnum.ClusterScore);

            return Json(new { Success = true, SgoDataPointId = sgoDataPoint.SGODataPointID }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Pre Assessment External
        public ActionResult GetSubjectsPreAssessmentExternal(int? sgoID, int? sgoDataPointId)
        {
            if (!sgoID.HasValue) return Json(new List<ListSubjectItem>(), JsonRequestBehavior.AllowGet);

            var sgo = _parameters.SGOObjectService.GetSGOByID(sgoID.Value);
            if (sgo == null) return Json(new List<ListSubjectItem>(), JsonRequestBehavior.AllowGet);


            var subjects = _parameters.SubjectService.SGOGetSubjectsForCreateExternalTest(sgo);
            var data = subjects.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).ToList();

            AppendCurrentSubjectOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PreAssessmentExternal, data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGradesPreAssessmentExternal(int? sgoID, int? sgoDataPointId, string subjectName)
        {
            if (!sgoID.HasValue) return Json(new List<ListSubjectItem>(), JsonRequestBehavior.AllowGet);

            var sgo = _parameters.SGOObjectService.GetSGOByID(sgoID.Value);
            if (sgo == null) return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);

            var gradeIDs =
                _parameters.SubjectService.SGOGetSubjectsForCreateExternalTest(sgo)
                    .Where(o => o.Name == subjectName)
                    .Select(o => o.GradeId).ToList();
            if (gradeIDs.Count > 0)
            {
                var grades = _parameters.GradeService.GetGradesByGradeIdList(gradeIDs).OrderBy(x => x.Order);
                var data = grades.Select(x => new ListItem { Id = x.Id, Name = x.Name }).ToList();

                AppendCurrentGradeOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PreAssessmentExternal, subjectName, data);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestsPreAssessmentExternal(int sgoId, int? sgoDataPointId, string subjectName, int gradeId)
        {
            var data = _parameters.SgoSelectDataPointService.GetExternalVirtualTest(CurrentUser.Id, subjectName, gradeId).ToList()
                .OrderBy(x => x.Name)
                .Select(x => new ListItem { Id = x.VirtualTestId, Name = x.Name }).ToList();

            AppendCurrentVirtualTestOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PreAssessmentExternal, subjectName, gradeId, data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudentDataPoint(int sgoId, int sgoDataPointId, int? virtualTestId)
        {
            if (!virtualTestId.HasValue)
            {
                if (sgoDataPointId > 0)
                    virtualTestId = _parameters.SgoDataPointService.GetById(sgoDataPointId).VirtualTestID;
                else
                    virtualTestId = 0;
                // just get list of student when creating new data point and did not select external test yet
            }
            var studentDataPoints = _parameters.SgoSelectDataPointService.GetStudentDataPoint(sgoId,
                virtualTestId.GetValueOrDefault());

            DateTime maxResultDate = DateTime.MinValue;
            decimal pointsPossible = 0;
            if (studentDataPoints.Any())
            {
                maxResultDate = studentDataPoints.Max(x => x.ResultDate).GetValueOrDefault();
                var maxTestResultId =
                    studentDataPoints.Where(x => x.ResultDate == maxResultDate).Max(x => x.TestResultId);
                pointsPossible =
                    studentDataPoints.FirstOrDefault(x => x.TestResultId == maxTestResultId)
                        .PointsPossible.GetValueOrDefault();

            }

            var resultDate = "";
            if (maxResultDate > DateTime.MinValue)
                resultDate = maxResultDate.DisplayDateWithFormat();

            return Json(new { studentDataPoints, resultDate, pointsPossible }, JsonRequestBehavior.AllowGet);
        }

        private int GetPermissionAccessPreAssessmentExternal(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessSgoDataPoint(model.SGOId);

            // Temporary remove check access student logic to fix bug can not save data when remove student out of class. Task LNKT-22173
            //var js = new JavaScriptSerializer();
            //var studentDataPoints =
            //    js.Deserialize<List<SGOGetStudentDataPointData>>(model.StudentDataPointData)
            //        .Where(x => x.ScoreRaw.HasValue).ToList();

            //if (studentDataPoints.Any())
            //{
            //    if (!_parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser,
            //        string.Join(",", studentDataPoints.Select(x => x.StudentId))))
            //    {
            //        permissionStatus = (int) SGOPermissionEnum.NotAvalible;
            //    }
            //}

            return permissionStatus;
        }

        [SGOManagerLogFilter]
        [ValidateInput(false)]
        public ActionResult SavePreAssessmentExternal(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessPreAssessmentExternal(model);
            if (permissionStatus != (int) SGOPermissionEnum.FullUpdate &&
                permissionStatus != (int) SGOPermissionEnum.MinorUpdate)
            {
                return Json(new {Success = false, ErrorMessage = "Has no permision"}, JsonRequestBehavior.AllowGet);
            }

            SGODataPoint sgoDataPoint;
            if (permissionStatus == (int) SGOPermissionEnum.MinorUpdate && model.SGODataPointId > 0)
            {
                sgoDataPoint = _parameters.SgoDataPointService.GetById(model.SGODataPointId);
                sgoDataPoint.RationaleGuidance = model.RationaleGuidance ?? "";
                _parameters.SgoDataPointService.Save(sgoDataPoint);

                var js = new JavaScriptSerializer();
                var studentDataPoints =
                    js.Deserialize<List<SGOGetStudentDataPointData>>(model.StudentDataPointData)
                        .Where(x => x.ScoreRaw.HasValue)
                        .ToList();

                // Allow to save External Test Result when having student added at Prepareness group page but does not inputed test result
                if (sgoDataPoint.VirtualTestID.GetValueOrDefault() > 0 && studentDataPoints.Any(x=>x.SGOStudentType > 0 && x.TestResultId.GetValueOrDefault() == 0))
                {
                    _parameters.SgoSelectDataPointService.UpdateStudentDataPointRoster(sgoDataPoint.SGOID,
                        sgoDataPoint.SGODataPointID,
                        sgoDataPoint.VirtualTestID.GetValueOrDefault(), sgoDataPoint.ResultDate.GetValueOrDefault(),
                        sgoDataPoint.TotalPoints,
                        CurrentUser.Id,
                        CurrentUser.Id,
                        BuildStudentDataPointXml(studentDataPoints));
                }
            }
            else
            {
                sgoDataPoint = new SGODataPoint
                {
                    Type = (int) SGODataPointTypeEnum.PreAssessmentExternal,
                    SGOID = model.SGOId,
                    RationaleGuidance = model.RationaleGuidance ?? "",
                    SGODataPointID = model.SGODataPointId,
                    TotalPoints = model.TotalPoints,
                    ResultDate = model.ResultDate,
                    AttachScoreUrl = model.AttactScoreUrl,
                    ScoreType = null,
                    IsTemporary = model.CreateTemporaryExternalVirtualTest ?? false,
                };

                if (sgoDataPoint.SGODataPointID > 0)
                    sgoDataPoint = AssignSgoDataPointData(sgoDataPoint);

                var errorMessage = model.BypassDataPointNumberRestriction == 1
                    ? null
                    : ValidateDataPointNumberOfSgo(sgoDataPoint);

                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    return Json(new {Success = false, ErrorMessage = errorMessage}, JsonRequestBehavior.AllowGet);
                }

                if (model.CreateTemporaryExternalVirtualTest.HasValue && model.CreateTemporaryExternalVirtualTest.Value == true)
                {
                    var externalVirtualTest = _parameters.VirtualTestService.GetTestById(model.VirtualTestId.GetValueOrDefault());
                    if (externalVirtualTest != null)
                    {
                        sgoDataPoint.Name = externalVirtualTest.Name;
                        model.Name = externalVirtualTest.Name;
                    }
                }

                var virtualTest = GetOrCreateExternalVirtualTest(model, ref errorMessage);
                if (virtualTest == null)
                    return Json(new {Success = false, ErrorMessage = errorMessage}, JsonRequestBehavior.AllowGet);

                sgoDataPoint.VirtualTestID = virtualTest.VirtualTestID;
				if (string.IsNullOrEmpty(sgoDataPoint.Name))
                {
                    sgoDataPoint.Name = virtualTest.Name;
                }

                // get virtual test name in case selecting old external virtual test
                sgoDataPoint.UserID = CurrentUser.Id;

                _parameters.SgoDataPointService.Save(sgoDataPoint);

                if (model.SGODataPointId > 0)
                    RemoveDataPointRelevantData(model.SGODataPointId);

                // Init when create new
                if (model.SGODataPointId == 0)
                    _parameters.SgoDataPointService.InitDefaultBandForDataPoint(sgoDataPoint.SGODataPointID);

                var js = new JavaScriptSerializer();
                var studentDataPoints =
                    js.Deserialize<List<SGOGetStudentDataPointData>>(model.StudentDataPointData)
                        .Where(x => x.ScoreRaw.HasValue)
                        .ToList();

                if (sgoDataPoint.VirtualTestID.GetValueOrDefault() > 0)
                {
                    _parameters.SgoSelectDataPointService.UpdateStudentDataPointRoster(sgoDataPoint.SGOID,
                        sgoDataPoint.SGODataPointID,
                        sgoDataPoint.VirtualTestID.GetValueOrDefault(), sgoDataPoint.ResultDate.GetValueOrDefault(),
                        sgoDataPoint.TotalPoints,
                        CurrentUser.Id,
                        CurrentUser.Id,
                        BuildStudentDataPointXml(studentDataPoints));
                }
            }

            return
                Json(
                    new
                    {
                        Success = true,
                        SgoDataPointId = sgoDataPoint.SGODataPointID,
                        VirtualTestId = sgoDataPoint.VirtualTestID
                    }, JsonRequestBehavior.AllowGet);
        }

        private VirtualTestData GetOrCreateExternalVirtualTest(DataPointViewModel model, ref string errorMessage)
        {
            VirtualTestData virtualTest = null;

            if (!string.IsNullOrEmpty(model.Name))
            {
                var subject = _parameters.SgoDataPointService.GetSubjectToCreateExternalBank(model.SubjectName,
                    model.GradeId, model.SGOId);
                if (subject == null)
                {
                    errorMessage = "Can not create bank for this test.";
                    return null;
                }

                var bank = _parameters.SgoDataPointService.GetOrCreateBankForExternalTest(subject.Id, model.GradeId,
                    null);
                if (bank == null)
                {
                    errorMessage = "Can not create bank for this test.";
                    return null;
                }

                var sgoObject = _parameters.SGOObjectService.GetSGOByID(model.SGOId);
                var district = _parameters.DistrictService.GetDistrictById(sgoObject.DistrictID);
                var sgoStateId = district == null ? 0 : district.StateId;

                virtualTest = new VirtualTestData
                {
                    StateID = sgoStateId,
                    VirtualTestSourceID = 3,
                    // External tests should have VirtualTestSourceID = 3 because they look like legacy tests (summary, no item-level scores)
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    Archived = false,
                    VirtualTestType = null,
                    TestScoreMethodID = 1,
                    Name = model.Name,
                    BankID = bank.Id,
                    DatasetOriginID = (int)DataSetOriginEnum.External_Data_Entry_Deprecated,
                    DatasetCategoryID = (int)DataSetCategoryEnum.SGO
                };

                var achievementLevelSetting = _parameters.AchievementLevelSettingService.GetByName("SGO");
                if (achievementLevelSetting != null)
                {
                    virtualTest.AchievementLevelSettingID = achievementLevelSetting.AchievementLevelSettingID;
                }

                var createTemporaryExternalVirtualTest = model.CreateTemporaryExternalVirtualTest.HasValue && model.CreateTemporaryExternalVirtualTest.Value;
                if (!createTemporaryExternalVirtualTest)
                {
                    virtualTest.AuthorUserID = CurrentUser.Id;
                    var conflictName = _parameters.VirtualTestService.ConflictNameInBank(virtualTest);
                    if (conflictName)
                    {
                        errorMessage =
                            string.Format("A test with name {0} already exists in test bank.", virtualTest.Name);
                        return null;
                    }
                }

                _parameters.VirtualTestService.Save(virtualTest);
            }
            else
            {
                virtualTest = _parameters.VirtualTestService.GetTestById(model.VirtualTestId.GetValueOrDefault());
            }

            return virtualTest;
        }


        public ActionResult VerifyExternalVirtualTestName(DataPointViewModel model)
        {
            var errorMessage = "";
            var result = true;
            if (!string.IsNullOrEmpty(model.Name))
            {
                var subject = _parameters.SgoDataPointService.GetSubjectToCreateExternalBank(model.SubjectName,
                    model.GradeId, model.SGOId);
                if (subject == null)
                {
                    errorMessage = "Unable to create a bank for this test.";
                    result = false;
                }

                if (result)
                {
                    var bank = _parameters.SgoDataPointService.GetOrCreateBankForExternalTest(subject.Id, model.GradeId,
                    null);
                    if (bank == null)
                    {
                        errorMessage = "Unable to create a bank for this test.";
                        result = false;
                    }

                    if (result)
                    {
                        var virtualTest = new VirtualTestData
                        {
                            Name = model.Name,
                            BankID = bank.Id,
                            AuthorUserID = CurrentUser.Id
                        };

                        var conflictName = _parameters.VirtualTestService.ConflictNameInBank(virtualTest);
                        if (conflictName)
                        {
                            errorMessage = "AN EXTERNAL TEST WITH THAT NAME, SUBJECT, AND GRADE HAS ALREADY BEEN CREATED. PLEASE USE A DIFFERENT NAME, SUBJECT, OR GRADE.";
                            result = false;
                        }
                    }
                }
            }
            else
            {
                errorMessage = "Test name is empty.";
                result = false;
            }

            return Json(new
            {
                Success = result,
                ErrorMessage = errorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        private string BuildStudentDataPointXml(List<SGOGetStudentDataPointData> studentDataPoints)
        {
            using (var sw = new StringWriter())
            {
                var xs = new XmlSerializer(typeof(List<SGOGetStudentDataPointData>));
                xs.Serialize(sw, studentDataPoints);
                try
                {
                    var xml = sw.ToString();
                    return xml;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        #endregion

        #region Post Assessment To Be Created
        public ActionResult GetSubjectsPostAssessmentToBeCreated(int? sgoID)
        {
            if (!sgoID.HasValue) return Json(new List<ListSubjectItem>(), JsonRequestBehavior.AllowGet);

            var sgo = _parameters.SGOObjectService.GetSGOByID(sgoID.Value);
            if (sgo == null) return Json(new List<ListSubjectItem>(), JsonRequestBehavior.AllowGet);


            var subjects = _parameters.SubjectService.SGOGetSubjectsForCreateExternalTest(sgo);
            var data = subjects.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);


            //var criteria = new SearchBankCriteria();

            //criteria.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            //criteria.UserId = CurrentUser.Id;
            //criteria.UserRole = CurrentUser.RoleId;

            //var subjects = _parameters.SubjectService.GetSubjectsByGradeIdAndAuthor(criteria); // by-pass grade information to get all subject
            //if (subjects != null)
            //{
            //    var data = subjects.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).ToList();
            //    return Json(data, JsonRequestBehavior.AllowGet);
            //}
            //return Json(new List<ListSubjectItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGradesPostAssessmentToBeCreated(int? sgoID, string subjectName)
        {
            if (!sgoID.HasValue) return Json(new List<ListSubjectItem>(), JsonRequestBehavior.AllowGet);

            var sgo = _parameters.SGOObjectService.GetSGOByID(sgoID.Value);
            if (sgo == null) return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);

            var gradeIDs =
                _parameters.SubjectService.SGOGetSubjectsForCreateExternalTest(sgo)
                    .Where(o => o.Name == subjectName)
                    .Select(o => o.GradeId).ToList();
            if (gradeIDs.Count > 0)
            {
                var grades = _parameters.GradeService.GetGradesByGradeIdList(gradeIDs).OrderBy(x => x.Order);
                var data = grades.Select(x => new ListItem { Id = x.Id, Name = x.Name });

                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);

            //var criteria = new SearchBankCriteria();

            //criteria.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            //criteria.UserId = CurrentUser.Id;
            //criteria.UserRole = CurrentUser.RoleId;

            //var subjects = _parameters.SubjectService.GetSubjectsByGradeIdAndAuthor(criteria); // by-pass grade information to get all subject
            //if (subjects != null)
            //{
            //    var gradeIds = subjects.Where(x => x.Name == subjectName).Select(x => x.GradeId).Distinct().ToList();
            //    var grades = _parameters.GradeService.GetGradesByGradeIdList(gradeIds).OrderBy(x => x.Order);

            //    var data = grades.Select(x => new ListItem { Id = x.Id, Name = x.Name });
            //    return Json(data, JsonRequestBehavior.AllowGet);
            //}

            //return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        private int GetPermissionAccessPostAssessmentToBeCreated(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessSgoDataPoint(model.SGOId);

            if (permissionStatus == (int)SGOPermissionEnum.MinorUpdate && model.SGODataPointId == 0)
            {
                permissionStatus = (int)SGOPermissionEnum.ReadOnly;
            }
            
            return permissionStatus;
        }

        [SGOManagerLogFilter]
        [ValidateInput(false)]
        public ActionResult SavePostAssessmentToBeCreated(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessPostAssessmentToBeCreated(model);
            if (permissionStatus != (int) SGOPermissionEnum.FullUpdate
                 && permissionStatus != (int) SGOPermissionEnum.MinorUpdate)                
            {
                return Json(new {Success = false, ErrorMessage = "Has no permision" }, JsonRequestBehavior.AllowGet);
            }

            SGODataPoint sgoDataPoint;
            if (permissionStatus == (int) SGOPermissionEnum.MinorUpdate)
            {
                sgoDataPoint = _parameters.SgoDataPointService.GetById(model.SGODataPointId);
                sgoDataPoint.RationaleGuidance = model.RationaleGuidance ?? "";
                _parameters.SgoDataPointService.Save(sgoDataPoint);
            }
            else
            {
                sgoDataPoint = new SGODataPoint
                {
                    Type = (int) SGODataPointTypeEnum.PostAssessmentToBeCreated,
                    SGOID = model.SGOId,
                    RationaleGuidance = model.RationaleGuidance ?? "",
                    SGODataPointID = model.SGODataPointId,
                    Name = model.Name ?? "",
                    AttachScoreUrl = model.AttactScoreUrl,
                    ScoreType = null,
                    UserID = CurrentUser.Id
                };

            if (sgoDataPoint.SGODataPointID > 0)
                sgoDataPoint = AssignSgoDataPointData(sgoDataPoint);

                var errorMessage = ValidateDataPointNumberOfSgo(sgoDataPoint);
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    return Json(new {Success = false, ErrorMessage = errorMessage}, JsonRequestBehavior.AllowGet);
                }

                _parameters.SgoDataPointService.Save(sgoDataPoint);

                if (model.SGODataPointId > 0)
                    RemoveDataPointRelevantData(model.SGODataPointId);

                // Init when create new
                if (model.SGODataPointId == 0)
                    _parameters.SgoDataPointService.InitDefaultBandForDataPoint(sgoDataPoint.SGODataPointID);
            }

            return Json(new {Success = true, SgoDataPointId = sgoDataPoint.SGODataPointID}, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Pre Assessment Custom
        public ActionResult GetSubjectPreAssessmentCustom(int sgoId, int? sgoDataPointId, int virtualTestCustomScoreId)
        {
            var data = PreAssessmentGetSubjectCustom(sgoId, sgoDataPointId, SGODataPointTypeEnum.PreAssessmentCustom, virtualTestCustomScoreId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<ListSubjectItem> PreAssessmentGetSubjectCustom(int sgoId, int? sgoDataPointId, SGODataPointTypeEnum dataPointType, int virtualTestCustomScoreId)
        {
            var subjectItems = _parameters.SgoSelectDataPointService.GetVirtualTestCustomScore(sgoId,virtualTestCustomScoreId, true)
                .GroupBy(x => x.SubjectName).OrderBy(x => x.Key).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).ToList();

            AppendCurrentSubjectOfDataPoint(sgoDataPointId, dataPointType, subjectItems);
            return subjectItems;
        }

        public ActionResult GetGradePreAssessmentCustom(int sgoId, int? sgoDataPointId, string subjectName, int virtualTestCustomScoreId)
        {
            var data = PreAssessmentGetGradeCustom(sgoId, sgoDataPointId, SGODataPointTypeEnum.PreAssessmentCustom, subjectName, virtualTestCustomScoreId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<ListItem> PreAssessmentGetGradeCustom(int sgoId, int? sgoDataPoint, SGODataPointTypeEnum dataPointType, string subjectName, int virtualTestCustomScoreId)
        {
            var listItems = _parameters.SgoSelectDataPointService.GetVirtualTestCustomScore(sgoId, virtualTestCustomScoreId, true)
                .Where(x => x.SubjectName == subjectName)
                .Select(x => new { x.GradeId, x.GradeName, x.GradeOrder }).Distinct()
                .OrderBy(x => x.GradeOrder)
                .Select(x => new ListItem
                {
                    Id = x.GradeId,
                    Name = x.GradeName
                }).ToList();

            AppendCurrentGradeOfDataPoint(sgoDataPoint, dataPointType, subjectName, listItems);
            return listItems;
        }

        public ActionResult GetTestPreAssessmentCustom(int sgoId, int? sgoDataPointId, string subjectName, int gradeId, int virtualTestCustomScoreId)
        {
            var data = PreAssessmentGetTestCustom(sgoId, sgoDataPointId, SGODataPointTypeEnum.PreAssessmentCustom, subjectName, gradeId, virtualTestCustomScoreId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public List<ListItem> PreAssessmentGetTestCustom(int sgoId, int? sgoDataPointId, SGODataPointTypeEnum dataPointType, string subjectName, int gradeId, int virtualTestCustomScoreId)
        {
            var listItems = _parameters.SgoSelectDataPointService.GetVirtualTestCustomScore(sgoId, virtualTestCustomScoreId, true)
                .Where(x => x.SubjectName == subjectName && x.GradeId == gradeId)
                .Select(x => new { x.VirtualTestId, x.VirtualTestName }).Distinct().OrderBy(x => x.VirtualTestName)
                .Select(x => new ListItem
                {
                    Id = x.VirtualTestId,
                    Name = x.VirtualTestName
                }).ToList();

            AppendCurrentVirtualTestOfDataPoint(sgoDataPointId, dataPointType, subjectName, gradeId, listItems);
            return listItems;
        }

        [SGOManagerLogFilter]
        public ActionResult SavePreAssessmentCustom(DataPointViewModel model)
        {
            model.DataSetCategoryID = Convert.ToInt32(model.TestType.Split('_')[1]);

            // Do not check all custom datapoint must have the same type ==> will check at Scoring Pland step
            //if (!IsCustomDataPointTypeValid(model))
            //{
            //    return Json(new { Success = false, ErrorMessage = "All custom datapoints must have a same type." }, JsonRequestBehavior.AllowGet);
            //}

            var sgoDataPoint = new SGODataPoint
            {
                Type = (int)SGODataPointTypeEnum.PreAssessmentCustom,
                Name = model.Name ?? "",
                RationaleGuidance = model.RationaleGuidance ?? "",
                SGODataPointID = model.SGODataPointId,
                SGOID = model.SGOId,
                VirtualTestID = model.VirtualTestId,
                AchievementLevelSettingID = model.DataSetCategoryID,
                ScoreType = model.ScoreType,
                IsTemporary = model.CreateTemporaryExternalVirtualTest ?? false
            };

            if (sgoDataPoint.SGODataPointID > 0)
                sgoDataPoint = AssignSgoDataPointData(sgoDataPoint);

            var errorMessage = model.BypassDataPointNumberRestriction == 1 ? null : ValidateDataPointNumberOfSgo(sgoDataPoint);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                return Json(new { Success = false, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            _parameters.SgoDataPointService.Save(sgoDataPoint);            

            //SyncScoreTypeOfCustomDataPoint(sgoDataPoint);

            if (model.SGODataPointId > 0)
            {
                RemoveDataPointRelevantData(sgoDataPoint.SGODataPointID);                
            }

            if (model.VirtualTestCustomSubScoreId.HasValue && model.VirtualTestCustomSubScoreId > 0)
            {
                _parameters.SgoDataPointClusterScoreService.Save(new SGODataPointClusterScore
                {
                    VirtualTestCustomSubScoreId = model.VirtualTestCustomSubScoreId.GetValueOrDefault(),
                    TestResultSubScoreName = string.Empty,
                    SGODataPointID = sgoDataPoint.SGODataPointID
                });
            }

            // Init when create new
            if (model.SGODataPointId == 0)
                _parameters.SgoDataPointService.InitDefaultBandForDataPoint(sgoDataPoint.SGODataPointID);

            SaveStudentDataPointFromVirtualTest(sgoDataPoint.SGOID, sgoDataPoint.SGODataPointID, sgoDataPoint.VirtualTestID.GetValueOrDefault());
            
            return Json(new { Success = true, SgoDataPointId = sgoDataPoint.SGODataPointID }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Post Assessment Custom
        public ActionResult GetSubjectPostAssessmentCustom(int sgoId, int? sgoDataPointId, int virtualTestCustomScoreId)
        {
            var sgoObject = _parameters.SGOObjectService.GetSGOByID(sgoId);
            // If SGO at Preparation Approved step ==> Load test that students have taken (the same with pre-assessment list)
            if (sgoObject.SGOStatusID == (int) SGOStatusType.PreparationApproved)
            {
                var data = PreAssessmentGetSubjectCustom(sgoId, sgoDataPointId,
                    SGODataPointTypeEnum.PostAssessmentCustom, virtualTestCustomScoreId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = PostAssessmentGetSubjectCustom(sgoId, sgoDataPointId,
                    SGODataPointTypeEnum.PostAssessmentCustom, virtualTestCustomScoreId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }            
        }

        private List<ListSubjectItem> PostAssessmentGetSubjectCustom(int sgoId, int? sgoDataPointId, SGODataPointTypeEnum dataPointType, int virtualTestCustomScoreId)
        {
            var subjectItems = _parameters.SgoSelectDataPointService.GetVirtualTestCustomScore(sgoId, virtualTestCustomScoreId, null)
                .GroupBy(x => x.SubjectName).OrderBy(x => x.Key).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).ToList();

            AppendCurrentSubjectOfDataPoint(sgoDataPointId, dataPointType, subjectItems);
            return subjectItems;
        }

        public ActionResult GetGradePostAssessmentCustom(int sgoId, int? sgoDataPointId, string subjectName, int virtualTestCustomScoreId)
        {
            var sgoObject = _parameters.SGOObjectService.GetSGOByID(sgoId);
            // If SGO at Preparation Approved step ==> Load test that students have taken (the same with pre-assessment list)
            if (sgoObject.SGOStatusID == (int) SGOStatusType.PreparationApproved)
            {
                var data = PreAssessmentGetGradeCustom(sgoId, sgoDataPointId, SGODataPointTypeEnum.PostAssessmentCustom,
                    subjectName, virtualTestCustomScoreId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = PostAssessmentGetGradeCustom(sgoId, sgoDataPointId, SGODataPointTypeEnum.PostAssessmentCustom,
                    subjectName, virtualTestCustomScoreId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        private List<ListItem> PostAssessmentGetGradeCustom(int sgoId, int? sgoDataPoint, SGODataPointTypeEnum dataPointType, string subjectName, int virtualTestCustomScoreId)
        {
            var listItems = _parameters.SgoSelectDataPointService.GetVirtualTestCustomScore(sgoId, virtualTestCustomScoreId, null)
                .Where(x => x.SubjectName == subjectName)
                .Select(x => new { x.GradeId, x.GradeName, x.GradeOrder }).Distinct()
                .OrderBy(x => x.GradeOrder)
                .Select(x => new ListItem
                {
                    Id = x.GradeId,
                    Name = x.GradeName
                }).ToList();

            AppendCurrentGradeOfDataPoint(sgoDataPoint, dataPointType, subjectName, listItems);
            return listItems;
        }		

        public ActionResult GetTestPostAssessmentCustom(int sgoId, int? sgoDataPointId, string subjectName, int gradeId,
            int virtualTestCustomScoreId)
        {
            var sgoObject = _parameters.SGOObjectService.GetSGOByID(sgoId);
            // If SGO at Preparation Approved step ==> Load test that students have taken (the same with pre-assessment list)
            if (sgoObject.SGOStatusID == (int) SGOStatusType.PreparationApproved)
            {
                var data = PreAssessmentGetTestCustom(sgoId, sgoDataPointId, SGODataPointTypeEnum.PostAssessmentCustom,
                    subjectName, gradeId, virtualTestCustomScoreId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = PostAssessmentGetTestCustom(sgoId, sgoDataPointId, SGODataPointTypeEnum.PostAssessmentCustom,
                    subjectName, gradeId, virtualTestCustomScoreId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public List<ListItem> PostAssessmentGetTestCustom(int sgoId, int? sgoDataPointId, SGODataPointTypeEnum dataPointType, string subjectName, int gradeId, int virtualTestCustomScoreId)
        {
            var listItems = _parameters.SgoSelectDataPointService.GetVirtualTestCustomScore(sgoId, virtualTestCustomScoreId, null)
                .Where(x => x.SubjectName == subjectName && x.GradeId == gradeId)
                .Select(x => new { x.VirtualTestId, x.VirtualTestName }).Distinct().OrderBy(x => x.VirtualTestName)
                .Select(x => new ListItem
                {
                    Id = x.VirtualTestId,
                    Name = x.VirtualTestName
                }).ToList();

            AppendCurrentVirtualTestOfDataPoint(sgoDataPointId, dataPointType, subjectName, gradeId, listItems);
            return listItems;
        }

        [SGOManagerLogFilter]
        [ValidateInput(false)]
        public ActionResult SavePostAssessmentCustom(DataPointViewModel model)
        {
            model.DataSetCategoryID = Convert.ToInt32(model.TestType.Split('_')[1]);

            // Do not check all custom datapoint must have the same type ==> will check at Scoring Pland step
            //if (!IsCustomDataPointTypeValid(model))
            //{
            //    return Json(new { Success = false, ErrorMessage = "All custom datapoints must have a same type." }, JsonRequestBehavior.AllowGet);
            //}

            var sgoDataPoint = new SGODataPoint
            {
                Type = (int) SGODataPointTypeEnum.PostAssessmentCustom,
                Name = model.Name ?? "",
                RationaleGuidance = model.RationaleGuidance ?? "",
                SGODataPointID = model.SGODataPointId,
                SGOID = model.SGOId,
                VirtualTestID = model.VirtualTestId,
                //TODO: [TestType] will remove AchievementLevelSettingID
                AchievementLevelSettingID = model.DataSetCategoryID, 
                ScoreType = model.ScoreType,                
                AttachScoreUrl = model.AttactScoreUrl,

            };

            if (sgoDataPoint.SGODataPointID > 0)
                sgoDataPoint = AssignSgoDataPointData(sgoDataPoint);

            var errorMessage = ValidateDataPointNumberOfSgo(sgoDataPoint);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                return Json(new { Success = false, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            _parameters.SgoDataPointService.Save(sgoDataPoint);
            //SyncScoreTypeOfCustomDataPoint(sgoDataPoint);
            
            // Always set ManualScoring target score when save Post assessment custom at Preparation approved step
            // set ManualScoring target score when ScoreType is CustomText
            var sgo = _parameters.SGOObjectService.GetSGOByID(model.SGOId);
            if (sgo.SGOStatusID == (int)SGOStatusType.PreparationApproved && (model.ScoreType >= (int)SGOScoreTypeEnum.ScoreCustomA1 && model.ScoreType <= (int)SGOScoreTypeEnum.ScoreCustomA4))
            {
                sgo.TargetScoreType = (int)SGOTargetScoreTypeEnum.ManualScoring;
                _parameters.SGOObjectService.Save(sgo);

                AutoSetPreAssessmentCustomAsImprovementBasedDatapoint(model.SGOId);
            }

            if (model.SGODataPointId > 0)
            {
                RemoveDataPointRelevantData(sgoDataPoint.SGODataPointID);                
            }
            //insert SubscoreName to know scoreType of overall or subscore
            if (model.VirtualTestCustomSubScoreId.HasValue && model.VirtualTestCustomSubScoreId > 0)
            {
                _parameters.SgoDataPointClusterScoreService.Save(new SGODataPointClusterScore
                {
                    VirtualTestCustomSubScoreId = model.VirtualTestCustomSubScoreId.GetValueOrDefault(),
                    TestResultSubScoreName = string.Empty,
                    SGODataPointID = sgoDataPoint.SGODataPointID
                });
            }
            // Init when create new
            if (model.SGODataPointId == 0)
                _parameters.SgoDataPointService.InitDefaultBandForDataPoint(sgoDataPoint.SGODataPointID);

            return Json(new { Success = true, SgoDataPointId = sgoDataPoint.SGODataPointID }, JsonRequestBehavior.AllowGet);
        }                 

        private void AutoSetPreAssessmentCustomAsImprovementBasedDatapoint(int sgoId)
        {
            var sgoDataPoints = _parameters.SgoDataPointService.GetDataPointBySGOID(sgoId);

            foreach (var sgoDataPoint in sgoDataPoints)
            {
                if (sgoDataPoint.Type == (int) SGODataPointTypeEnum.PreAssessmentCustom)
                {
                    sgoDataPoint.ImprovementBasedDataPoint = 1;
                    _parameters.SgoDataPointService.Save(sgoDataPoint);
                }else if (sgoDataPoint.ImprovementBasedDataPoint == 1)
                {
                    sgoDataPoint.ImprovementBasedDataPoint = 0;
                    _parameters.SgoDataPointService.Save(sgoDataPoint);
                }
            }
        }
        #endregion        
        #region Post Assessment External
        public ActionResult GetSubjectsPostAssessmentExternal(int? sgoID, int? sgoDataPointId)
        {
            if (!sgoID.HasValue) return Json(new List<ListSubjectItem>(), JsonRequestBehavior.AllowGet);

            var sgo = _parameters.SGOObjectService.GetSGOByID(sgoID.Value);
            if (sgo == null) return Json(new List<ListSubjectItem>(), JsonRequestBehavior.AllowGet);


            var subjects = _parameters.SubjectService.SGOGetSubjectsForCreateExternalTest(sgo);
            var data = subjects.GroupBy(s => s.Name).Select(x => new ListSubjectItem { Id = x.Key, Name = x.Key }).ToList();

            AppendCurrentSubjectOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PostAssessmentExternal, data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGradesPostAssessmentExternal(int? sgoID, int? sgoDataPointId, string subjectName)
        {
            if (!sgoID.HasValue) return Json(new List<ListSubjectItem>(), JsonRequestBehavior.AllowGet);

            var sgo = _parameters.SGOObjectService.GetSGOByID(sgoID.Value);
            if (sgo == null) return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);

            var gradeIDs =
                _parameters.SubjectService.SGOGetSubjectsForCreateExternalTest(sgo)
                    .Where(o => o.Name == subjectName)
                    .Select(o => o.GradeId).ToList();
            if (gradeIDs.Count > 0)
            {
                var grades = _parameters.GradeService.GetGradesByGradeIdList(gradeIDs).OrderBy(x => x.Order);
                var data = grades.Select(x => new ListItem { Id = x.Id, Name = x.Name }).ToList();

                AppendCurrentGradeOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PostAssessmentExternal, subjectName, data);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(new List<ListItem>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestsPostAssessmentExternal(int sgoId, int? sgoDataPointId, string subjectName, int gradeId)
        {
            var data = _parameters.SgoSelectDataPointService.GetExternalVirtualTest(CurrentUser.Id, subjectName, gradeId).ToList()
                .OrderBy(x => x.Name)
                .Select(x => new ListItem { Id = x.VirtualTestId, Name = x.Name }).ToList();

            AppendCurrentVirtualTestOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PostAssessmentExternal, subjectName, gradeId, data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestsPostAssessmentExternalForProgressMonitoring(int sgoId, string subjectName, int gradeId)
        {
            var data = _parameters.SgoSelectDataPointService.GetExternalVirtualTestForProgressMonitoring(sgoId, CurrentUser.Id, subjectName, gradeId).ToList()
                .OrderBy(x => x.Name)
                .Select(x => new ListItem { Id = x.VirtualTestId, Name = x.Name }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private int GetPermissionAccessPostAssessmentExternal(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessSgoDataPoint(model.SGOId);

            // Allow to save PostAssessmentExternal in case change from Post To Be Created datapoint
            if (model.SGODataPointId > 0)
            {
                var currentSgoDataPoint = _parameters.SgoDataPointService.GetById(model.SGODataPointId);
                if (currentSgoDataPoint != null &&
                    currentSgoDataPoint.Type == (int)SGODataPointTypeEnum.PostAssessmentToBeCreated)
                {
                    permissionStatus = (int)SGOPermissionEnum.FullUpdate;
                }
            }
            
            if (permissionStatus == (int)SGOPermissionEnum.MinorUpdate && model.SGODataPointId == 0)
            {
                permissionStatus = (int)SGOPermissionEnum.ReadOnly;
            }
            else if (permissionStatus == (int)SGOPermissionEnum.MinorUpdate && model.SGODataPointId > 0) // Allow to edit Post External datapoint at Minor Update status                
            {
                permissionStatus = (int) SGOPermissionEnum.FullUpdate;
            }

            // Temporary remove check access student logic to fix bug can not save data when remove student out of class. Task LNKT-22173
            //var js = new JavaScriptSerializer();
            //var studentDataPoints =
            //    js.Deserialize<List<SGOGetStudentDataPointData>>(model.StudentDataPointData)
            //        .Where(x => x.ScoreRaw.HasValue).ToList();

            //if (studentDataPoints.Any())
            //{
            //    if (!_parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser,
            //        string.Join(",", studentDataPoints.Select(x => x.StudentId))))
            //    {
            //        permissionStatus = (int)SGOPermissionEnum.NotAvalible;
            //    }
            //}

            return permissionStatus;
        }

        [SGOManagerLogFilter]
        [ValidateInput(false)]
        public ActionResult SavePostAssessmentExternal(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessPostAssessmentExternal(model);
            if (permissionStatus != (int)SGOPermissionEnum.FullUpdate &&
                permissionStatus != (int)SGOPermissionEnum.MinorUpdate)
            {
                return Json(new { Success = false, ErrorMessage = "Has no permision" }, JsonRequestBehavior.AllowGet);
            }

            SGODataPoint sgoDataPoint;
            if (permissionStatus == (int)SGOPermissionEnum.MinorUpdate)
            {
                sgoDataPoint = _parameters.SgoDataPointService.GetById(model.SGODataPointId);
                sgoDataPoint.RationaleGuidance = model.RationaleGuidance ?? "";
                _parameters.SgoDataPointService.Save(sgoDataPoint);
            }
            else
            {
                sgoDataPoint = new SGODataPoint
                {
                    Type = (int)SGODataPointTypeEnum.PostAssessmentExternal,
                    SGOID = model.SGOId,
                    RationaleGuidance = model.RationaleGuidance ?? "",
                    SGODataPointID = model.SGODataPointId,
                    TotalPoints = model.TotalPoints,
                    ResultDate = model.ResultDate,
                    AttachScoreUrl = model.AttactScoreUrl,
                    ScoreType = null

                };

                if (sgoDataPoint.SGODataPointID > 0)
                    sgoDataPoint = AssignSgoDataPointData(sgoDataPoint);

                var errorMessage = ValidateDataPointNumberOfSgo(sgoDataPoint);
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    return Json(new { Success = false, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
                }

                var virtualTest = GetOrCreateExternalVirtualTest(model, ref errorMessage);
                if (virtualTest == null)
                    return Json(new { Success = false, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);

                sgoDataPoint.VirtualTestID = virtualTest.VirtualTestID;
                sgoDataPoint.Name = virtualTest.Name; // get virtual test name in case selecting old external virtual test
                sgoDataPoint.UserID = CurrentUser.Id;

                _parameters.SgoDataPointService.Save(sgoDataPoint);

                if (!CheckImprovementScoringPlanDataPointType(model) || !CheckImprovementScoringPlanScoreType(model))
                {
                    var sgo = _parameters.SGOObjectService.GetSGOByID(model.SGOId);
                    sgo.TargetScoreType = (int)SGOTargetScoreTypeEnum.ManualScoring;
                    _parameters.SGOObjectService.Save(sgo);
                }

                if (model.SGODataPointId > 0)
                    RemoveDataPointRelevantData(model.SGODataPointId);

                // Init when create new
                if (model.SGODataPointId == 0)
                    _parameters.SgoDataPointService.InitDefaultBandForDataPoint(sgoDataPoint.SGODataPointID);

                var js = new JavaScriptSerializer();
                var studentDataPoints = js.Deserialize<List<SGOGetStudentDataPointData>>(model.StudentDataPointData).Where(x => x.ScoreRaw.HasValue).ToList();

                if (sgoDataPoint.VirtualTestID.GetValueOrDefault() > 0)
                {
                    _parameters.SgoSelectDataPointService.UpdateStudentDataPointRoster(sgoDataPoint.SGOID, sgoDataPoint.SGODataPointID,
                        sgoDataPoint.VirtualTestID.GetValueOrDefault(), sgoDataPoint.ResultDate.GetValueOrDefault(), sgoDataPoint.TotalPoints, CurrentUser.Id, CurrentUser.Id,
                        BuildStudentDataPointXml(studentDataPoints));
                }
            }

            return Json(new { Success = true, SgoDataPointId = sgoDataPoint.SGODataPointID, VirtualTestId = sgoDataPoint.VirtualTestID }, JsonRequestBehavior.AllowGet);
        }        
        #endregion

        #region Pre Assessment Historical

        public ActionResult GetSubjectPostAssessmentHistorical(int sgoId, int? sgoDataPointId,
            int dataSetCategoryID, int? stateId)
        {
            var previewTestTeacherId = GetPreviewTestTeacherId();

            var a = _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId).ToList();
            var data = _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId)
                .Where(x => x.DataSetCategoryID == dataSetCategoryID &&
                            (!stateId.HasValue || x.StateId == stateId.GetValueOrDefault()) && x.VirtualTestType == null
                            && x.VirtualTestSourceId == 3
                            && x.BankAuthorId != previewTestTeacherId
                // Include External virtual test (created in preview test teacher banks)
                )
                .GroupBy(x => x.SubjectName)
                .OrderBy(x => x.Key)
                .Select(x => new ListSubjectItem {Id = x.Key, Name = x.Key})
                .ToList();

            AppendCurrentSubjectOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PostAssessmentHistorical, data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGradePostAssessmentHistorical(int sgoId, int? sgoDataPointId,
            int dataSetCategoryID, string subjectName)
        {
            var previewTestTeacherId = GetPreviewTestTeacherId();

            var data = _parameters.SgoSelectDataPointService.GetSGOStudentTestData(sgoId)
                .Where(x =>
                    x.DataSetCategoryID == dataSetCategoryID && x.SubjectName == subjectName
                    && x.VirtualTestType == null
                    && x.VirtualTestSourceId == 3
                    && x.BankAuthorId != previewTestTeacherId
                // Include External virtual test (created in preview test teacher banks)
                )
                .Select(x => new {x.GradeId, x.GradeName, x.GradeOrder})
                .Distinct()
                .OrderBy(x => x.GradeOrder)
                .Select(x => new ListItem
                {

                    Id = x.GradeId,
                    Name = x.GradeName
                }).ToList();

            AppendCurrentGradeOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PostAssessmentHistorical, subjectName,
                data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestPostAssessmentHistorical(int sgoId, int? sgoDataPointId, int dataSetCategoryID, string subjectName, int gradeId)
        {
            var data = _parameters.SgoSelectDataPointService.GetTestPostAssessmentHistorical(sgoId,
                dataSetCategoryID, subjectName, gradeId, GetPreviewTestTeacherId());

            AppendCurrentVirtualTestOfDataPoint(sgoDataPointId, SGODataPointTypeEnum.PostAssessmentHistorical, subjectName, gradeId, data);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private int GetPermissionAccessPostAssessmentHistorical(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessSgoDataPoint(model.SGOId);

            return permissionStatus;
        }

        [SGOManagerLogFilter]
        public ActionResult SavePostAssessmentHistorical(DataPointViewModel model)
        {
            var permissionStatus = GetPermissionAccessPostAssessmentHistorical(model);
            if (permissionStatus != (int)SGOPermissionEnum.FullUpdate && permissionStatus != (int)SGOPermissionEnum.MinorUpdate)
            {
                return Json(new { Success = false, ErrorMessage = "Has no permision" }, JsonRequestBehavior.AllowGet);
            }

            //Check virtual test
            var virtualTests = _parameters.SgoSelectDataPointService.GetTestPostAssessmentHistorical(model.SGOId,
                model.DataSetCategoryID.GetValueOrDefault(), model.SubjectName, model.GradeId, GetPreviewTestTeacherId());

            if (virtualTests.All(x => x.Id != model.VirtualTestId))
            {
                return Json(new { Success = false, ErrorMessage = "Virtual Test is invalid" }, JsonRequestBehavior.AllowGet);
            }


            var sgoDataPoint = new SGODataPoint
            {
                Type = (int)SGODataPointTypeEnum.PostAssessmentHistorical,
                Name = model.Name ?? "",
                RationaleGuidance = model.RationaleGuidance ?? "",
                SGODataPointID = model.SGODataPointId,
                SGOID = model.SGOId,
                VirtualTestID = model.VirtualTestId,
                //TODO: [TestType] will remove AchievementLevelSettingID
                //AchievementLevelSettingID = model.DataSetCategoryID.GetValueOrDefault(), 
                ScoreType = model.ScoreType,
                AttachScoreUrl = model.AttactScoreUrl,

            };

            if (sgoDataPoint.SGODataPointID > 0)
                sgoDataPoint = AssignSgoDataPointData(sgoDataPoint);

            var errorMessage = model.BypassDataPointNumberRestriction == 1
                ? null
                : ValidateDataPointNumberOfSgo(sgoDataPoint);

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                return Json(new { Success = false, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            _parameters.SgoDataPointService.Save(sgoDataPoint);

            if (!CheckImprovementScoringPlanDataPointType(model) || !CheckImprovementScoringPlanScoreType(model))
            {
                var sgo = _parameters.SGOObjectService.GetSGOByID(model.SGOId);
                sgo.TargetScoreType = (int)SGOTargetScoreTypeEnum.ManualScoring;
                _parameters.SGOObjectService.Save(sgo);
            }

            if (model.SGODataPointId > 0)
                RemoveDataPointRelevantData(sgoDataPoint.SGODataPointID);

            // Init when create new
            if (model.SGODataPointId == 0)
                _parameters.SgoDataPointService.InitDefaultBandForDataPoint(sgoDataPoint.SGODataPointID);

            SaveStudentDataPointFromVirtualTest(sgoDataPoint.SGOID, sgoDataPoint.SGODataPointID, sgoDataPoint.VirtualTestID.GetValueOrDefault());
            SaveFilters(sgoDataPoint.SGODataPointID, model.ClusterScoreFilters, (int)SGODataPointFilterEnum.ClusterScore);

            return Json(new { Success = true, SgoDataPointId = sgoDataPoint.SGODataPointID }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        private List<ListItemStr> GetAssessmentScoreTypes(int virtualTestId, int sgoId, int? customScoreId = null, bool? isPostAssignment = false, string scoreType = "")
        {
            var scoreTypes = new List<ListItemStr>();
            var sgo = _parameters.SGOObjectService.GetSGOByID(sgoId);

            int sgoDataPointScoreType = 0;

            if (!string.IsNullOrEmpty(scoreType) && scoreType == "PreAssessment")
            {
                sgoDataPointScoreType = (int)SGODataPointTypeEnum.PreAssessment;
            }
            else if (!string.IsNullOrEmpty(scoreType) && scoreType == "PostAssessment")
            {
                sgoDataPointScoreType = (int)SGODataPointTypeEnum.PostAssessment;
            }

            if (sgo != null)
            {

                int districtId = sgo.DistrictID;
                var virtualTestCustomScore =
                    _parameters.SgoSelectDataPointService.SGOGetAssessmentScoreType(virtualTestId, districtId, customScoreId, sgoId, isPostAssignment, sgoDataPointScoreType);
                if (virtualTestCustomScore.UsePercent)
                    scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScorePercent).ToString(), Name = "Score Percent" });

                if (virtualTestCustomScore.UsePercentile)
                    scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScorePercentage).ToString(), Name = "Score Percentage" });

                if (virtualTestCustomScore.UseRaw)
                    scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreRaw).ToString(), Name = "Score Raw" });

                if (virtualTestCustomScore.UseScaled)
                    scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreScaled).ToString(), Name = "Score Scaled" });

                if (virtualTestCustomScore.UseIndex)
                    scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreIndex).ToString(), Name = "Score Index" });

                if (virtualTestCustomScore.UseLexile)
                    scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreLexile).ToString(), Name = "Score Lexile" });

                if (virtualTestCustomScore.UseCustomN1 ?? false)
                    scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomN1).ToString(), Name = virtualTestCustomScore.CustomN1Label ?? SGOScoreTypeEnum.ScoreCustomN1.DescriptionAttr() });

                if (virtualTestCustomScore.UseCustomN2 ?? false)
                    scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomN2).ToString(), Name = virtualTestCustomScore.CustomN2Label ?? SGOScoreTypeEnum.ScoreCustomN2.DescriptionAttr() });

                if (virtualTestCustomScore.UseCustomN3 ?? false)
                    scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomN3).ToString(), Name = virtualTestCustomScore.CustomN3Label ?? SGOScoreTypeEnum.ScoreCustomN3.DescriptionAttr() });

                if (virtualTestCustomScore.UseCustomN4 ?? false)
                    scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomN4).ToString(), Name = virtualTestCustomScore.CustomN4Label ?? SGOScoreTypeEnum.ScoreCustomN4.DescriptionAttr() });

                if (customScoreId.HasValue)
                {
                    if (virtualTestCustomScore.UseCustomA1 ?? false)
                        scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomA1).ToString(), Name = virtualTestCustomScore.CustomA1Label ?? SGOScoreTypeEnum.ScoreCustomA1.DescriptionAttr() });

                    if (virtualTestCustomScore.UseCustomA2 ?? false)
                        scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomA2).ToString(), Name = virtualTestCustomScore.CustomA2Label ?? SGOScoreTypeEnum.ScoreCustomA2.DescriptionAttr() });

                    if (virtualTestCustomScore.UseCustomA3 ?? false)
                        scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomA3).ToString(), Name = virtualTestCustomScore.CustomA3Label ?? SGOScoreTypeEnum.ScoreCustomA3.DescriptionAttr() });

                    if (virtualTestCustomScore.UseCustomA4 ?? false)
                        scoreTypes.Add(new ListItemStr { Id = ((int)SGOScoreTypeEnum.ScoreCustomA4).ToString(), Name = virtualTestCustomScore.CustomA4Label ?? SGOScoreTypeEnum.ScoreCustomA4.DescriptionAttr() });
                }
                var virtualTestCustomSubScores = _parameters.VirtualTestCustomSubScoreService.Select().Where(x => x.VirtualTestCustomScoreId == customScoreId).OrderBy(x => x.Sequence);
                foreach (var virtualTestCustomSubScore in virtualTestCustomSubScores)
                {
                    var subScoreTypes = BuildSubScoreType(virtualTestCustomSubScore);
                    scoreTypes.AddRange(subScoreTypes);
                }
            }

            return scoreTypes;
        }
    }
}
