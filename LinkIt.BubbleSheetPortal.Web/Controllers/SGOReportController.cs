using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels.ACTReport;
using LinkIt.BubbleSheetPortal.Web.ViewModels.SGO;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.Helpers;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class SGOReportController : BaseController
    {
        private readonly SGOReportControllerParameters _parameters;
        private readonly List<int> excludedGroupIds = new List<int> { 98, 99 };

        private readonly IS3Service _s3Service;

        public SGOReportController(SGOReportControllerParameters parameters, IS3Service s3Service)
        {
            _parameters = parameters;
            _s3Service = s3Service;
        }

        //public ActionResult Index(int id)
        //{
        //    var sgoObj = _parameters.SGOObjectService.GetSGOByID(id);
        //    if (sgoObj == null ||
        //        (sgoObj.OwnerUserID != CurrentUser.Id && sgoObj.ApproverUserID.GetValueOrDefault() != CurrentUser.Id))
        //    {
        //        return RedirectToAction("Index", "SGOManage");
        //    }
        //    var model = new SGOPrintViewModel()
        //    {
        //        SGOId = id
        //    };
        //    return View(model);
        //}

        public ActionResult Generate(SGOReportDataViewModel model)
        {
            string url = string.Empty;
            var dateFormatCookie = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
            model.DateFormat = dateFormatCookie == null ? Constanst.DefaultDateFormatValue : dateFormatCookie.Value;
            var pdf = Print(model, out url);

            var folder = ConfigurationManager.AppSettings["SGOReportFolder"];
            var bucketName = ConfigurationManager.AppSettings["SGOReportBucket"];

            _s3Service.UploadRubricFile(bucketName, folder + "/" + model.ReportFileName, new MemoryStream(pdf));

            return Json(new { IsSuccess = true, Url = url });
        }

        [HttpGet]
        public ActionResult ReportPrinting(SGOReportDataViewModel model)
        {
            var defaultDateFormat = model.DateFormat;
            HttpCookie ckDefaultDateFormat = System.Web.HttpContext.Current.Request.Cookies[Constanst.DefaultDateFormat];
            if (ckDefaultDateFormat == null)
            {
                ckDefaultDateFormat = new HttpCookie(Constanst.DefaultDateFormat, defaultDateFormat);
                System.Web.HttpContext.Current.Response.Cookies.Add(ckDefaultDateFormat);
            }
            else
            {
                ckDefaultDateFormat.Value = defaultDateFormat;
                System.Web.HttpContext.Current.Response.Cookies.Set(ckDefaultDateFormat);
            }
            var masterModel = new SGOReportMasterViewModel { SgoId = model.SgoId };
            BuildGeneralData(masterModel);
            BuildDataPointData(masterModel);
            BuildPreparenessGroupData(masterModel);
            BuildScoringPlanData(masterModel);
            BuildProgressMonitoringData(masterModel);
            BuildScoringDetailData(masterModel);
            BuildAuditTrailData(masterModel);

            return View(masterModel);
        }

        private void BuildGeneralData(SGOReportMasterViewModel masterModel)
        {
            try
            {
                masterModel.SgoObject = _parameters.SGOObjectService.GetSGOByID(masterModel.SgoId);
                masterModel.SgoType = masterModel.SgoObject.Type;
                masterModel.TargetScoreType = masterModel.SgoObject.TargetScoreType;
                masterModel.SgoCustomInformation = _parameters.SGOObjectService.GetSGOCustomById(masterModel.SgoId);
                masterModel.DistrictName = _parameters.DistrictService.GetDistrictById(masterModel.SgoObject.DistrictID).Name;
            }
            catch { }
            finally
            {
                masterModel.SgoObject = masterModel.SgoObject ?? new SGOObject();
                masterModel.SgoCustomInformation = masterModel.SgoCustomInformation ?? new SGOCustomReport();
            }
        }

        private void BuildDataPointData(SGOReportMasterViewModel masterModel)
        {
            try
            {
                var sgoReportDataPoints = _parameters.SGOObjectService.GetSGOReportDataPoint(masterModel.SgoId);
                foreach (var sgoReportDataPoint in sgoReportDataPoints)
                {
                    if (sgoReportDataPoint.VirtualTestId.HasValue)
                    {
                        var subjectAndGrade = _parameters.SgoSelectDataPointService.GetSubjectAndGradeByVirtualTestId(sgoReportDataPoint.VirtualTestId.GetValueOrDefault());
                        sgoReportDataPoint.SubjectName = subjectAndGrade.SubjectName;
                        sgoReportDataPoint.GradeName = subjectAndGrade.GradeName;
                    }
                }

                masterModel.PreAssessmentDataPoints =
                    sgoReportDataPoints.Where(x =>
                        x.Type == (int)SGODataPointTypeEnum.PreAssessment
                        || x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom
                        || x.Type == (int)SGODataPointTypeEnum.PreAssessmentExternal
                        || x.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical
                        ).ToList();

                masterModel.PostAssessmentDataPoints =
                    sgoReportDataPoints.Where(x =>
                        x.Type == (int)SGODataPointTypeEnum.PostAssessment
                        || x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom
                        || x.Type == (int)SGODataPointTypeEnum.PostAssessmentToBeCreated
                        || x.Type == (int)SGODataPointTypeEnum.PostAssessmentExternal
                        || x.Type == (int)SGODataPointTypeEnum.PostAssessmentHistorical
                        ).ToList();

                masterModel.SgoReportDataPointFilters = _parameters.SGOObjectService.GetSGOReportDataPointFilter(masterModel.SgoId).Select(o => { o.FilterName = o.FilterName.ReplaceWeirdCharacters(); return o; }).ToList();
            }
            catch { }
            finally
            {
                masterModel.PreAssessmentDataPoints = masterModel.PreAssessmentDataPoints ?? new List<SGOReportDataPoint>();
                masterModel.PostAssessmentDataPoints = masterModel.PostAssessmentDataPoints ?? new List<SGOReportDataPoint>();
                masterModel.SgoReportDataPointFilters = masterModel.SgoReportDataPointFilters ?? new List<SGOReportDataPointFilter>();
            }
        }

        private void BuildScoringPlanData(SGOReportMasterViewModel masterModel)
        {
            try
            {
                masterModel.SgoGroups = GetSgoGroup(masterModel.SgoId).OrderBy(x => x.Order).ToList();
                masterModel.SgoAttainmentGoals =
                    _parameters.SgoAttainmentGoalService.GetBySgoId(masterModel.SgoId)
                        .OrderByDescending(x => x.Order).ToList();

                masterModel.SgoAttainmentGroups =
                    _parameters.SgoAttainmentGroupService.GetBySgoGroupIds(
                        masterModel.SgoGroups.Select(x => x.SGOGroupID).ToList()).ToList();
            }
            catch { }
            finally
            {
                masterModel.SgoGroups = masterModel.SgoGroups ?? new List<SGOGroup>();
                masterModel.SgoAttainmentGoals = masterModel.SgoAttainmentGoals ?? new List<SGOAttainmentGoal>();
                masterModel.SgoAttainmentGroups = masterModel.SgoAttainmentGroups ?? new List<SGOAttainmentGroup>();
            }
        }

        private void BuildProgressMonitoringData(SGOReportMasterViewModel masterModel)
        {
            try
            {
                var lstAttainmentGoal = _parameters.SgoAttainmentGoalService.GetBySgoId(masterModel.SgoId).ToList();
                masterModel.SgoCalculateScoreResults = new List<SGOCalculateScoreResult>();
                foreach (var group in masterModel.SgoGroups)
                {
                    var vGoal =
                        lstAttainmentGoal.FirstOrDefault(o => o.Order == (int)group.TeacherSGOScore.GetValueOrDefault());
                    string strAttainmentGoal = vGoal == null ? string.Empty : vGoal.Name;
                    if (group.PercentStudentAtTargetScore.HasValue)
                    {
                        masterModel.SgoCalculateScoreResults.Add(new SGOCalculateScoreResult()
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

                if (masterModel.SgoType != (int)SGOTypeEnum.UnstructuredData)
                {
                    masterModel.TotalTeacherSGOScore =
                        Math.Round(masterModel.SgoCalculateScoreResults.Sum(x => x.WeightedScore).GetValueOrDefault(), 3);
                }
                else
                {
                    masterModel.TotalTeacherSGOScoreCustom = masterModel.SgoObject.TotalTeacherSGOScoreCustom;
                }
            }
            catch { }
            finally
            {
                masterModel.SgoCalculateScoreResults = masterModel.SgoCalculateScoreResults ?? new List<SGOCalculateScoreResult>();
            }
        }

        private void BuildScoringDetailData(SGOReportMasterViewModel masterModel)
        {
            try
            {
                masterModel.SgoScoringDetails = _parameters.SGOObjectService.GetSgoScoringDetail(masterModel.SgoId, null);

                SGOReportDataPoint sgoDataPoint;
                if (masterModel.TargetScoreType == (int)SGOTargetScoreTypeEnum.ManualScoring)
                {
                    sgoDataPoint =
                        masterModel.PostAssessmentDataPoints.FirstOrDefault(x => x.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom);
                    masterModel.ScoringDetailPostAssessmentTestName = sgoDataPoint == null ? "" : sgoDataPoint.Name;
                }
                else
                {
                    sgoDataPoint =
                        masterModel.PostAssessmentDataPoints.FirstOrDefault(x => x.Type == (int)SGODataPointTypeEnum.PostAssessment);
                    masterModel.ScoringDetailPostAssessmentTestName = sgoDataPoint == null ? "" : sgoDataPoint.Name;
                }

                sgoDataPoint =
                    masterModel.PreAssessmentDataPoints.FirstOrDefault(x => x.ImprovementBasedDataPoint == 1);
                masterModel.ScoringDetailPreAssessmentTestName = sgoDataPoint == null ? "" : sgoDataPoint.Name;
            }
            catch { }
            finally
            {
                masterModel.SgoScoringDetails = masterModel.SgoScoringDetails ?? new List<SGOScoringDetail>();
            }
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

        private void BuildPreparenessGroupData(SGOReportMasterViewModel model)
        {
            try
            {
                var dataPoints = _parameters.SgoDataPointService.GetDataPointBySGOID(model.SgoId).ToList();
                var bands = _parameters.SgoDataPointService.GetDataPointBandsBySGOID(model.SgoId);

                model.ListDataPoints = dataPoints
                    .Select(x => new SGOReportDataPointViewModel
                    {
                        SgoDataPoint = x,
                        SgoDataPointBands = bands
                                         .Where(y => y.SGODataPointID.Equals(x.SGODataPointID))
                                         .ToList(),
                        WeightPercent =
                                         _parameters.SgoDataPointService.CalculateDataPointWeight(dataPoints,
                                             x.SGODataPointID)
                    })
                    .ToList();
            }
            catch { }
            finally
            {
                model.ListDataPoints = model.ListDataPoints ?? new List<SGOReportDataPointViewModel>();
            }
        }

        private void BuildAuditTrailData(SGOReportMasterViewModel model)
        {
            try
            {
                var auditTrailData = _parameters.SgoAuditTrailService.GetAuditTrailBySGOID(model.SgoId);
                if (auditTrailData != null && auditTrailData.SGOAuditTrailSearchItems != null)
                {
                    model.ListAuditTrail = auditTrailData.SGOAuditTrailSearchItems.OrderBy(x => x.CreatedDate).ToList();
                }
                else
                {
                    model.ListAuditTrail = new List<SGOAuditTrailSearchItem>();
                }
            }
            catch { }
            finally
            {
                model.ListAuditTrail = model.ListAuditTrail ?? new List<SGOAuditTrailSearchItem>();
            }
        }

        #region Print Report SGO
        private byte[] ExportToPDF(string url, int timezoneOffset)
        {
            DateTime dt = DateTime.UtcNow.AddMinutes(timezoneOffset * (-1));

            string footerUrl = Url.Action("RenderFooter", "SGOReport", null, HelperExtensions.GetHTTPProtocal(Request));
            string headerUrl = Url.Action("RenderHeader", "SGOReport", new
            {
                //leftLine1 = "Generated: " + dt.ToString("MM/dd/yyyy h:mm tt")// String.Format("{0:g}", dt)
                leftLine1 = "Generated: " + dt.DisplayDateWithFormat(true)// String.Format("{0:g}", dt)
            }, HelperExtensions.GetHTTPProtocal(Request));

            string args =
                string.Format("--footer-html \"{0}\" --header-html \"{2}\" --header-spacing 5 \"{1}\" - "
                    , footerUrl
                    , url
                    , headerUrl
                    );

            var startInfo = new ProcessStartInfo(Server.MapPath("~/PDFTool/wkhtmltopdf.exe"), args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };
            var proc = new Process { StartInfo = startInfo };
            proc.Start();

            string output = proc.StandardOutput.ReadToEnd();
            byte[] buffer = proc.StandardOutput.CurrentEncoding.GetBytes(output);
            proc.WaitForExit();
            proc.Close();

            return buffer;
        }

        private byte[] Print(SGOReportDataViewModel model, out string url)
        {
            url = Url.Action("ReportPrinting", "SGOReport", model, HelperExtensions.GetHTTPProtocal(Request));
            var pdf = ExportToPDF(url, model.TimezoneOffset);
            return pdf;
        }

        public ActionResult RenderFooter()
        {
            return PartialView("_Footer");
        }

        public ActionResult RenderHeader(string leftLine1)
        {
            var obj = new FooterData { LeftLine1 = leftLine1 };
            return PartialView("_Header", obj);
            
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetSGOReportS3File(string fileName)
        {
            var folder = ConfigurationManager.AppSettings["SGOReportFolder"];
            var bucketName = ConfigurationManager.AppSettings["SGOReportBucket"];
            var result = _s3Service.DownloadFile(bucketName, folder + "/" + fileName);

            if (result.IsSuccess)
            {
                var s3Url = string.Format("https://s3.amazonaws.com/{0}/{1}/{2}", bucketName, folder, fileName);
                return Json(new { Result = true, Url = s3Url });
            }
            else
            {
                return Json(new { Result = false });
            }
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportItemSGOManager)]
        [HttpGet]
        public ActionResult Printing(SGOReportDataViewModel model)
        {
            var vSgo = _parameters.SGOObjectService.GetSGOByID(model.SgoId);
            if (vSgo != null
                && (vSgo.OwnerUserID == CurrentUser.Id || (vSgo.ApproverUserID.HasValue
                                                            && vSgo.ApproverUserID.Value == CurrentUser.Id)
                    )
               )
            {
                var masterModel = new SGOReportMasterViewModel { SgoId = model.SgoId };
                BuildGeneralData(masterModel);
                BuildDataPointData(masterModel);
                BuildPreparenessGroupData(masterModel);
                BuildScoringPlanData(masterModel);
                BuildProgressMonitoringData(masterModel);
                BuildScoringDetailData(masterModel);
                BuildAuditTrailData(masterModel);
                return View(masterModel);
            }
            return RedirectToAction("Index", "SGOManage");
        }
        #endregion
    }
}
