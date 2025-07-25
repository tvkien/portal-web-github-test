using AutoMapper;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.PDFGenerator;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ServiceConsumer;
using LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker.EnterResult;
using LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TestMaker;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using StringExtensions = LinkIt.BubbleSheetPortal.Common.StringExtensions;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestUtilitiesDefineTemplates;
using OfficeOpenXml;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class TestUtilitiesDefineTemplatesController : BaseController
    {
        private readonly TestUtilitiesDefineTemplatesControllerParameters _parameters;

        public TestUtilitiesDefineTemplatesController(TestUtilitiesDefineTemplatesControllerParameters parameters)
        {
            _parameters = parameters;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtDefineTemplates)]
        public ActionResult Index()
        {
            var user = _parameters.UserService.GetUserById(CurrentUser.Id);
            ViewBag.CanPublish = user.RoleId == (int)Permissions.Publisher || user.RoleId == (int)Permissions.NetworkAdmin;
            return View();
        }

        public ActionResult AddTemplate()
        {
            var useMutiDate =
          _parameters.DistrictDecodeService.GetDistrictDecodeByLabel(
              CurrentUser.DistrictId ?? 0, Constanst.UseMultiDateTemplate);
            ViewBag.useMutiDate = useMutiDate;
            return View();
        }

        [HttpPost]
        public ActionResult AddNewTemplate(VirtualTestCustomScore model)
        {
            model.Name = model.Name.UrlDecode();
            model.Name = model.Name.Trim();
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return Json(new { success = false, error = "Please input the name of new template." }, JsonRequestBehavior.AllowGet);
            }

            var existingObj =
                _parameters.VirtualTestCustomScoreService.GetCustomScoreByNameAndDistrictID(
                    CurrentUser.DistrictId ?? 0, model.Name);
            if (existingObj != null)
            {
                return Json(new { success = false, error = "This template name is already in use. Please refer to that template or use a different name." }, JsonRequestBehavior.AllowGet);
            }

            var newItem = _parameters.VirtualTestCustomScoreService.CreateCustomScore(model.Name, model.IsMultiDate, CurrentUser);

            return Json(new { success = true, templateId = newItem.VirtualTestCustomScoreId }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadListTemplate(bool archived)
        {
            try
            {
                var user = _parameters.UserService.GetUserById(CurrentUser.Id);
                IQueryable<ResultEntryTemplateItemList> data = _parameters.DataLockerTemplateService.GetTemplate(CurrentUser.Id, CurrentUser.RoleId, CurrentUser.DistrictId.GetValueOrDefault(), archived).Select(x => new ResultEntryTemplateItemList()
                {
                    VirtualTestCustomScoreID = x.VirtualTestCustomScoreID,
                    Name = x.Name,
                    Author = string.Empty,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    FirstName = x.NameFirst,
                    LastName = x.NameLast,
                    TotalVirtualTestAssociated = x.TotalVirtualTestAssociated.GetValueOrDefault(),
                    PublishedDistricts = string.Join(", ", x.PublishedDistricts.ToArray()),
                    IsPublished = x.IsPublished,
                    Archived = x.Archived
                });

                var parser = new DataTableParser<ResultEntryTemplateItemList>();
                return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadListOrShareDistrict(int templateId)
        {
            return PartialView("_ListOrShareDistrict", templateId);
        }

        public ActionResult LoadListDistrictByTemplate(int templateId)
        {
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_ListDistrictByTemplate", templateId);
        }

        [HttpPost]
        public ActionResult PublishToDistrict(TemplatePublishToDistrictVM viewModel)
        {
            try
            {
                var user = _parameters.UserService.GetUserById(CurrentUser.Id);
                var canPublish = user.RoleId == (int)Permissions.Publisher || user.RoleId == (int)Permissions.NetworkAdmin;
                if (canPublish)
                {
                    var templateInfo = _parameters.VirtualTestCustomScoreService.GetCustomScoreByID(viewModel.TemplateId);
                    _parameters.DataLockerTemplateService.PublicTemplateToDistrict(viewModel.TemplateId, viewModel.DistrictId, user.Id);
                    if (templateInfo.IsMultiDate.HasValue && templateInfo.IsMultiDate.Value == true)
                    {
                        var canPublishForDistrict = _parameters.DistrictDecodeService.GetDistrictDecodeByLabel(
                            viewModel.DistrictId, Constanst.UseMultiDateTemplate);
                        if (canPublishForDistrict)
                        {
                            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = false, error = "This district does not have access to multi-date templates. The template will still be published, but will not be viewable by the district." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, error = "Not Authorized" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetUnPublishedDistrictsByState(int stateId, int templateId)
        {
            var CurrentDistrict = CurrentUser.DistrictId.GetValueOrDefault();
            var data = _parameters.DataLockerTemplateService.GetUnPublishedDistrict(stateId, templateId).ToList();
            if (CurrentUser.IsNetworkAdmin)
            {
                data = data.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id)).ToList();
            }

            data = data.Where(d => d.Id != CurrentDistrict).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadPublishedDistricts(int templateId)
        {
            return PartialView("_ListOrShareDistrict", templateId);
        }

        public ActionResult GetPublishedDistricts(int templateId)
        {
            try
            {
                IQueryable<TemplateDatalockerDistrictVM> data = _parameters.DataLockerTemplateService.GetTemplateDistricts(templateId)
                    .Select(x => new TemplateDatalockerDistrictVM
                    {
                        TemplateDatalockerDistrictId = x.VirtualTestCustomScoreDistrictShareID,
                        DistrictId = x.DistrictId,
                        DistrictName = x.DistrictName,
                        TemplateId = templateId
                    });

                var parser = new DataTableParser<TemplateDatalockerDistrictVM>();
                return new JsonNetResult { Data = parser.Parse(data) };
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadPublishToDistrict(int templateId)
        {
            var model = new TemplatePublishToDistrictVM();
            model.TemplateId = templateId;
            model.IsPublisher = CurrentUser.IsPublisher;
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_PublishToDistrict", model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DepublishDistrict(int templateDistrictId)
        {
            var templateDistrict = _parameters.DataLockerTemplateService.GetTemplateDistrictById(templateDistrictId);
            if (templateDistrict.IsNull())
            {
                return Json(false);
            }
            //check to avoid modifying ajax parameter bankId)
            if (!(CurrentUser.RoleId == (int)Permissions.NetworkAdmin || CurrentUser.RoleId == (int)Permissions.Publisher))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this bank." }, JsonRequestBehavior.AllowGet);
            }
            if (!Util.HasRightOnDistrict(CurrentUser, templateDistrict.DistrictId))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                _parameters.DataLockerTemplateService.DepublicTemplateDistrictById(templateDistrict);
                return Json(true);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadCreateOverallScore(int templateId, int? subscoreId)
        {
            var model = new CreateOverallScoreModel();
            model.TemplateId = templateId;
            //subscoreId has value if this dialog is used to create a score type fof subscore
            model.SubscoreId = subscoreId ?? 0;

            var fileType = string.Empty;
            var ddFileType =
                _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                   CurrentUser.DistrictId ?? 0, ContaintUtil.DATALOCKER_ARTIFACTFILETYPE).FirstOrDefault();
            if (ddFileType != null)
                fileType = ddFileType.Value;
            else
            {
                fileType = _parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue(ContaintUtil.DATALOCKER_ARTIFACTFILETYPE, "pdf");
            }
            if (!string.IsNullOrEmpty(fileType))
                model.UploadFileTypeList = fileType.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var fileSizeString = string.Empty;
            var ddFileSize =
                _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                    CurrentUser.DistrictId ?? 0, "DataLocker_ArtifactMaxFileSize").FirstOrDefault();
            if (ddFileSize != null)
                fileSizeString = ddFileSize.Value;
            else
            {
                fileSizeString = _parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue("DataLocker_ArtifactMaxFileSize", "10");
            }
            int maxFileSize;
            if (int.TryParse(fileSizeString, out maxFileSize))
                model.MaxFileSize = maxFileSize;

            var assessmentArtifactFileTypeGroups = Mapper.Map<IEnumerable<AssessmentArtifactFileTypeGroupViewModel>>
           (_parameters.DistrictDecodeService
                   .GetAssessmentArtifactFileTypeGroups(CurrentUser.DistrictId ?? 0));
            model.AssessmentArtifactFileTypeGroupViewModel = assessmentArtifactFileTypeGroups.ToList();

            return PartialView("_CreateOverallScore", model);
        }

        public ActionResult LoadScoreTypeList(int templateId)
        {
            var data = new List<ScoreTypeListItem>();

            var scoreTypeList = _parameters.DataLockerTemplateService.GetScoreTypesOfTemplate(templateId);

            var noteScoreType = scoreTypeList.FirstOrDefault(x => x.ScoreTypeCode == VirtualTestCustomMetaData.NOTE_COMMENT);
            if (noteScoreType != null)
            {
                scoreTypeList.Remove(noteScoreType);
            }

            if (noteScoreType != null && noteScoreType.ListNoteComment != null)
            {
                foreach (var note in noteScoreType.ListNoteComment)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        DisplayOrder = note.Order ?? 0,
                        ScoreTypeCode = noteScoreType.ScoreTypeCode,
                        //ScoreTypeName = noteScoreType.ScoreTypeName,
                        Description = note.Description,
                        CustomScoreName = note.NoteName,
                        ScoreName = noteScoreType.ScoreName,
                        ScoreType = noteScoreType.ScoreType
                    });
                }
            }

            scoreTypeList = scoreTypeList.OrderBy(x => x.DisplayOrder).ToList();
            var assessmentArtifactFileTypeGroups = Mapper.Map<IEnumerable<AssessmentArtifactFileTypeGroupEditScoreModel>>
                 (_parameters.DistrictDecodeService
                  .GetAssessmentArtifactFileTypeGroups(CurrentUser.DistrictId ?? 0)).ToList();
            data = scoreTypeList.Select(x => new ScoreTypeListItem()
            {
                Id = x.DisplayOrder,
                ScoreTypeCode = x.ScoreTypeCode,
                ScoreTypeName = x.ScoreTypeName,
                ScoreName = !string.IsNullOrEmpty(x.CustomScoreName) ? x.CustomScoreName : x.ScoreName,
                Description = x.Description,
                Overview = x.ScoreTypeCode != ScoreTypeModel.ARTIFACT_SCORE ? x.Overview : GetStringDisplayNameUploadFileType(assessmentArtifactFileTypeGroups, x.UploadFileTypes),
                ShortOverview = x.ShortOverview,
                IsPredefinedList = x.IsListText,
                ShortScoreType = x.ScoreType,
                IsAutoCalculation = x.Meta.IsAutoCalculation,
                MaxScore = GetMaxScoreValue(x.ListText, x.IsNumeric, x.Meta.MaxValue.GetValueOrDefault()),
                VirtualTestCustomMetaDataID = x.VirtualTestCustomMetaDataID
            }).ToList();

            var parser = new DataTableParser<ScoreTypeListItem>();

            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        private decimal GetMaxScoreValue(List<string> listValue, bool isNumeric, decimal maxValue)
        {
            decimal result = 0;
            if (isNumeric)
            {
                if (listValue != null && listValue.Count > 0)
                {
                    var maxVal = listValue.OrderByDescending(x => decimal.Parse(x)).FirstOrDefault();
                    if (maxVal != null)
                        result = decimal.Parse(maxVal);
                }
                else
                {
                    result = maxValue;
                }
            }
            return result;
        }

        public ActionResult ViewTemplate(int templateId)
        {
            var model = _parameters.VirtualTestCustomScoreService.GetCustomScoreByID(templateId);

            if (model == null)
            {
                return RedirectToAction("Index", "TestUtilitiesDefineTemplates");
            }
            model.SubscoreIdList = _parameters.VirtualTestCustomSubScoreService.GetSubscoreOfTemplate(templateId)
                .OrderBy(x => x.Sequence)
               .Select(x => x.VirtualTestCustomSubScoreId)
               .ToList();
            return View(model);
        }

        public ActionResult LoadViewSubscore(int VirtualTestCustomScoreId, int virtualTestCustomSubScoreID)
        {
            var subscore = _parameters.VirtualTestCustomSubScoreService.GetById(virtualTestCustomSubScoreID);
            return PartialView("_ViewSubscore", subscore);
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TmgmtDefineTemplates)]
        public ActionResult EditTemplate(int id)
        {
            var userDistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            var useMutiDate =
           _parameters.DistrictDecodeService.GetDistrictDecodeByLabel(
               CurrentUser.DistrictId ?? 0, Constanst.UseMultiDateTemplate);
            ViewBag.useMutiDate = useMutiDate;
            if (_parameters.DataLockerTemplateService.CheckUserPermissionOnTemplate(id, userDistrictId))
            {
                var model = _parameters.VirtualTestCustomScoreService.GetCustomScoreByID(id);

                if (model == null)
                {
                    return RedirectToAction("Index", "TestUtilitiesDefineTemplates");
                }
                model.SubscoreIdList = _parameters.VirtualTestCustomSubScoreService.GetSubscoreOfTemplate(id)
                    .OrderBy(x => x.Sequence)
                   .Select(x => x.VirtualTestCustomSubScoreId)
                   .ToList();
                model.HasAssociatedTest = _parameters.DataLockerTemplateService.HasAssociatedTest(id);
                model.HasAssociatedTestResult = _parameters.DataLockerTemplateService.HasAssociatedTestResult(id);
                model.HasAssociatedAutoSave = _parameters.DataLockerTemplateService.HasAssociatedAutoSave(id);
                model.HasImDataPointAssigned = HasImDataPointAssigned(id);
                model.HasConversionSet = HasConversionSet(id);

                return View(model);
            }
            return RedirectToAction("Index", "TestUtilitiesDefineTemplates");
        }

        public ActionResult CopyTemplate(int id, string templateName)
        {
            //TODO: Validataion TemplateName.
            templateName = templateName.UrlDecode();
            var existingObj = _parameters.VirtualTestCustomScoreService.GetCustomScoreByNameAndDistrictID(CurrentUser.DistrictId ?? 0, templateName);
            if (existingObj != null)
            {
                return Json(new { Success = false, ErrorMessage = "Existed Template Name." }, JsonRequestBehavior.AllowGet);
            }

            //TOOD: Process Clone Template Return TemplateId
            var newTemplateId = _parameters.DataLockerTemplateService.CopyTemplateByID(id, CurrentUser.Id, templateName);
            return Json(new { Success = true, templateID = newTemplateId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddNewScoreType(ScoreTypeModel model)
        {
            model.CorrectModelData();
            model.CustomScoreName = model.CustomScoreName.UrlDecode();
            var scoreType = string.Empty;
            if (model.CustomScoreName != null)
            {
                model.CustomScoreName = model.CustomScoreName.Trim();
            }
            if (model.ScoreName != null)
            {
                model.ScoreName = model.ScoreName.Trim();
            }
            if (string.IsNullOrEmpty(model.ScoreNameAbsolute))
            {
                return Json(new
                {
                    success = false,
                    error = "Please input name."
                }, JsonRequestBehavior.AllowGet);
            }
            if (model.IsNumeric)
            {
                if (model.MinScore.HasValue && model.MaxScore.HasValue)
                {
                    if (model.MaxScore.Value < model.MinScore.Value)
                    {
                        return Json(new
                        {
                            success = false,
                            error = "Max Score must equal or greater than Min Score."
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            model.Description = model.Description.UrlDecode();
            if (!string.IsNullOrEmpty(model.ListTextString) && !model.ListTextString.Contains("Option"))
            {
                model.ListTextString = model.ListTextString.UrlDecode();
            }

            model.ListArtifactTagString = model.ListArtifactTagString.UrlDecode();
            if (_parameters.DataLockerTemplateService.HasAssociatedTestResult(model.TemplateID))
            {
                return Json(new
                {
                    success = false,
                    error = "Unable to add new score type when the template has associated test result."
                }, JsonRequestBehavior.AllowGet);
            }
            if (_parameters.DataLockerTemplateService.HasAssociatedAutoSave(model.TemplateID))
            {
                return Json(new
                {
                    success = false,
                    error = "Unable to delete subscore when the template has associated with previously entered results that were not saved."
                }, JsonRequestBehavior.AllowGet);
            }
            var validationMessage = model.ValidateChange(false, null);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                return Json(new
                {
                    success = false,
                    error = validationMessage
                }, JsonRequestBehavior.AllowGet);
            }

            if (model.SubscoreId.HasValue && model.SubscoreId.Value > 0)
            {
                //create a score type for subscore
                //check the name
                var validation =
                  _parameters.DataLockerTemplateService.CheckScoreTypeBeforeAddingToSubscore(CurrentUser.Id, model);
                if (!string.IsNullOrEmpty(validation))
                {
                    return Json(new
                    {
                        success = false,
                        error = validation
                    }, JsonRequestBehavior.AllowGet);
                }
                var existing = _parameters.DataLockerTemplateService.SubscoreScoreTypeIsExisting(model.SubscoreId ?? 0, model);
                if (existing)
                {
                    return Json(new
                    {
                        success = false,
                        error = model.GetDuplicateNameErrorMessage()
                    }, JsonRequestBehavior.AllowGet);
                }
                scoreType = _parameters.DataLockerTemplateService.CreateNewSubscoreScoreType(CurrentUser.Id, model);
            }
            else
            {
                //create a score type for template
                //TODO: If Noete/Coment -> should check Note Title Uniqueue on a TemplateID
                var validation = _parameters.DataLockerTemplateService.CheckScoreTypeBeforeAddingToTemplate(CurrentUser.Id, model);
                if (!string.IsNullOrEmpty(validation))
                {
                    return Json(new
                    {
                        success = false,
                        error = validation
                    }, JsonRequestBehavior.AllowGet);
                }

                //check the name
                var existing = _parameters.DataLockerTemplateService.TemplateScoreTypeIsExisting(model.TemplateID, model);
                if (existing)
                {
                    return Json(new
                    {
                        success = false,
                        error = model.GetDuplicateNameErrorMessage()
                    }, JsonRequestBehavior.AllowGet);
                }
                scoreType = _parameters.DataLockerTemplateService.CreateNewScoreType(CurrentUser.Id, model);
            }

            //_parameters.VirtualTestCustomMetaDataService.ReassignMetaDataOrder(model.TemplateID, model.SubscoreId);

            return Json(new { success = true, scoreType = scoreType }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, ValidateInput(false)]
        public ActionResult LoadEditOverallScore(int templateId, string name, string scoreTypeName, int? subscoreId, string subscoreName)
        {
            name = name.UrlDecode();
            ViewBag.TemplateId = templateId;
            ViewBag.SubscoreId = subscoreId ?? 0;
            var assessmentArtifactFileTypeGroups = Mapper.Map<IEnumerable<AssessmentArtifactFileTypeGroupEditScoreModel>>
                 (_parameters.DistrictDecodeService
                  .GetAssessmentArtifactFileTypeGroups(CurrentUser.DistrictId ?? 0)).ToList();
            ViewBag.UploadFileTypeList = assessmentArtifactFileTypeGroups.ToList();

            if (scoreTypeName.Equals("Notes/Comments"))
            {
                return LoadEditNoteScore(templateId, name, scoreTypeName, subscoreId, subscoreName);
            }
            var scoreType = new ScoreTypeModel();
            if (subscoreId.HasValue && subscoreId.Value > 0)
            {
                //score type of subscore
                scoreType = _parameters.DataLockerTemplateService.GetScoreTypeOfSubscore(subscoreId.Value, name);
                scoreType.HasAssociatedAutoSave = _parameters.DataLockerService.HasAssociatedAutoSave(templateId);
            }
            else
            {
                //score type of Template
                scoreType = _parameters.DataLockerTemplateService.GetScoreTypeOfTemplate(templateId, name);
            }

            scoreType.AssessmentArtifactFileTypeGroupViewModel = assessmentArtifactFileTypeGroups.ToList();
            scoreType.HasAssociatedAutoSave = _parameters.DataLockerService.HasAssociatedAutoSave(templateId);
            scoreType.HasImDataPointAssigned = HasImDataPointAssigned(templateId, name, subscoreName);

            return PartialView("_EditOverallScore", scoreType);
        }
        #region for clone score
        [HttpGet, ValidateInput(false)]
        public ActionResult LoadCloneOverallScore(int templateId, string name, string scoreTypeName, int? subscoreId, string subscoreName)
        {
            name = name.UrlDecode();
            var assessmentArtifactFileTypeGroups = Mapper.Map<IEnumerable<AssessmentArtifactFileTypeGroupEditScoreModel>>
                 (_parameters.DistrictDecodeService
                  .GetAssessmentArtifactFileTypeGroups(CurrentUser.DistrictId ?? 0)).ToList();

            if (scoreTypeName.Equals("Notes/Comments"))
                return LoadCloneNoteScore(templateId, name, scoreTypeName, subscoreId, subscoreName);

            var scoreType = new ScoreTypeModel();
            if (subscoreId.HasValue && subscoreId.Value > 0)
                scoreType = _parameters.DataLockerTemplateService.GetScoreTypeOfSubscore(subscoreId.Value, name);
            else scoreType = _parameters.DataLockerTemplateService.GetScoreTypeOfTemplate(templateId, name);

            scoreType.AssessmentArtifactFileTypeGroupViewModel = assessmentArtifactFileTypeGroups.ToList();
            scoreType.ScoreName = string.Empty;
            scoreType.CustomScoreName = string.Empty;
            return PartialView("_CloneOverallScore", scoreType);
        }
        public ActionResult LoadCloneNoteScore(int templateId, string name, string scoreTypeName, int? subscoreId, string subscoreName)
        {
            var model = new ScoreTypeModel();
            if (subscoreId.HasValue && subscoreId.Value > 0)
                model = _parameters.DataLockerTemplateService.GetNoteScoreOfSubScore(templateId, subscoreId.Value);
            else model = _parameters.DataLockerTemplateService.GetNoteScoreOfTemplate(templateId);

            var note = model.ListNoteComment.FirstOrDefault(x => x.NoteName == name);
            model.NoteDefaultValue = note.DefaultValue;
            model.Description = note.Description;
            model.NoteType = note.NoteType;
            model.DisplayOrder = note.Order.HasValue ? note.Order.Value : 0;
            model.ScoreName = string.Empty;
            model.CustomScoreName = string.Empty;

            return PartialView("_CloneOverallScore", model);
        }

        [HttpPost]
        public ActionResult CloneScoreType(ScoreTypeModel model, int? scoreId, int? subScoreId)
        {
            model.CorrectModelData();
            model.CustomScoreName = model.CustomScoreName.UrlDecode();
            var scoreType = string.Empty;
            if (model.CustomScoreName != null)
            {
                model.CustomScoreName = model.CustomScoreName.Trim();
            }
            if (model.ScoreName != null)
            {
                model.ScoreName = model.ScoreName.Trim();
            }
            if (string.IsNullOrEmpty(model.ScoreNameAbsolute))
            {
                return Json(new
                {
                    success = false,
                    error = "Please input name."
                }, JsonRequestBehavior.AllowGet);
            }
            if (model.IsNumeric)
            {
                if (model.MinScore.HasValue && model.MaxScore.HasValue && model.MaxScore.Value < model.MinScore.Value)
                {
                    return Json(new
                    {
                        success = false,
                        error = "Max Score must equal or greater than Min Score."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            model.Description = model.Description.UrlDecode();
            if (!string.IsNullOrEmpty(model.ListTextString) && !model.ListTextString.Contains("Option"))
            {
                model.ListTextString = model.ListTextString.UrlDecode();
            }

            model.ListArtifactTagString = model.ListArtifactTagString.UrlDecode();
            if (_parameters.DataLockerTemplateService.HasAssociatedTestResult(model.TemplateID))
            {
                return Json(new
                {
                    success = false,
                    error = "Unable to add new score type when the template has associated test result."
                }, JsonRequestBehavior.AllowGet);
            }
            if (_parameters.DataLockerTemplateService.HasAssociatedAutoSave(model.TemplateID))
            {
                return Json(new
                {
                    success = false,
                    error = "Unable to delete subscore when the template has associated with previously entered results that were not saved."
                }, JsonRequestBehavior.AllowGet);
            }
            var validationMessage = model.ValidateChange(false, null);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                return Json(new
                {
                    success = false,
                    error = validationMessage
                }, JsonRequestBehavior.AllowGet);
            }

            if (model.SubscoreId.HasValue && model.SubscoreId.Value > 0)
            {
                var validation =
                  _parameters.DataLockerTemplateService.CheckScoreTypeBeforeAddingToSubscore(CurrentUser.Id, model);
                if (!string.IsNullOrEmpty(validation))
                {
                    return Json(new
                    {
                        success = false,
                        error = validation
                    }, JsonRequestBehavior.AllowGet);
                }
                var existing = _parameters.DataLockerTemplateService.SubscoreScoreTypeIsExisting(model.SubscoreId ?? 0, model);
                if (existing)
                {
                    return Json(new
                    {
                        success = false,
                        error = model.GetDuplicateNameErrorMessage()
                    }, JsonRequestBehavior.AllowGet);
                }
                scoreType = _parameters.DataLockerTemplateService.CreateNewSubscoreScoreType(CurrentUser.Id, model);
            }
            else
            {
                var validation = _parameters.DataLockerTemplateService.CheckScoreTypeBeforeAddingToTemplate(CurrentUser.Id, model);
                if (!string.IsNullOrEmpty(validation))
                {
                    return Json(new
                    {
                        success = false,
                        error = validation
                    }, JsonRequestBehavior.AllowGet);
                }
                var existing = _parameters.DataLockerTemplateService.TemplateScoreTypeIsExisting(model.TemplateID, model);
                if (existing)
                {
                    return Json(new
                    {
                        success = false,
                        error = model.GetDuplicateNameErrorMessage()
                    }, JsonRequestBehavior.AllowGet);
                }
                scoreType = _parameters.DataLockerTemplateService.CreateNewScoreType(CurrentUser.Id, model);
            }
            UpdateMetaDataOrderForCloneScoreType(scoreId, subScoreId, model.DisplayOrder, scoreType);

            return Json(new { success = true, scoreType = scoreType }, JsonRequestBehavior.AllowGet);
        }
        private bool UpdateMetaDataOrderForCloneScoreType(int? scoreId, int? subScoreId, int orderClone, string scoreTypeNew)
        {
            try
            {
                var metaData = new List<VirtualTestCustomMetaData>();
                if (subScoreId.HasValue && subScoreId.Value > 0)
                {
                    metaData = _parameters.VirtualTestCustomMetaDataService.GetMetaDataOfSubscore(subScoreId.Value);
                }
                else if (scoreId.HasValue && scoreId.Value > 0)
                {
                    metaData = _parameters.VirtualTestCustomMetaDataService.GetMetaDataOfTemplate(scoreId.Value);
                }
                if (metaData == null || !metaData.Any()) return false;

                var noteComment = metaData.FirstOrDefault(f => f.ScoreType == ScoreTypeModel.NOTE_COMMENT);
                var noteMetaObject = JsonConvert.DeserializeObject<VirtualTestCustomMetaModel>(noteComment?.MetaData ?? string.Empty);
                if (noteMetaObject != null)
                {
                    noteMetaObject.ListNoteComment.OrderBy(o => o.Order).ToList().ForEach(item => { if (item.Order > orderClone) item.Order++; });

                    if (scoreTypeNew == ScoreTypeModel.NOTE_COMMENT)
                    {
                        noteMetaObject.ListNoteComment.Last().Order = orderClone + 1;
                    }
                    noteComment.MetaData = noteMetaObject.GetJsonString();
                    _parameters.VirtualTestCustomMetaDataService.Save(noteComment);
                }
                var scoreTypeUpdate = metaData.Where(w => w.ScoreType != ScoreTypeModel.NOTE_COMMENT).Select(s => new UpdateOrderScoreTypeModel
                {
                    ScoreType = s.ScoreType,
                    Order = s.Order
                }).OrderBy(o => o.Order).ToList();
                if (scoreTypeUpdate.Any())
                {
                    scoreTypeUpdate.ForEach(item => { if (item.Order > orderClone) item.Order++; });

                    if (scoreTypeNew != ScoreTypeModel.NOTE_COMMENT)
                    {
                        scoreTypeUpdate.Last().Order = orderClone + 1;
                    }
                    var tem = XmlUtils.BuildXml(scoreTypeUpdate);
                    _parameters.VirtualTestCustomMetaDataService.UpdateOrderForMetaData(scoreId, subScoreId,
                        XmlUtils.BuildXml(scoreTypeUpdate));
                }
                return true;
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return false;
            }
        }
        #endregion
        public ActionResult LoadEditNoteScore(int templateId, string name, string scoreTypeName, int? subscoreId, string subscoreName)
        {
            var model = new ScoreTypeModel();
            if (subscoreId.HasValue && subscoreId.Value > 0)
            {
                model = _parameters.DataLockerTemplateService.GetNoteScoreOfSubScore(templateId, subscoreId.Value);
            }
            else
            {
                model = _parameters.DataLockerTemplateService.GetNoteScoreOfTemplate(templateId);
            }

            var note = model.ListNoteComment.FirstOrDefault(x => x.NoteName == name);
            model.NoteDefaultValue = note.DefaultValue;
            model.ScoreName = note.NoteName;
            model.CustomScoreName = note.NoteName;
            model.Description = note.Description;
            model.NoteType = note.NoteType;
            model.HasImDataPointAssigned = HasImDataPointAssigned(templateId, name, subscoreName);

            return PartialView("_EditOverallScore", model);
        }
        public ActionResult UpdateNoteScoreType(string oldName, ScoreTypeModel model)
        {
            string error = _parameters.DataLockerTemplateService.UpdateNoteScoreType(CurrentUser.Id, oldName, model);
            if (!string.IsNullOrEmpty(error))
            {
                return Json(new { success = false, error = error }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateScoreType(string oldName, ScoreTypeModel model)
        {
            model.CorrectModelData();
            oldName = string.IsNullOrEmpty(oldName) ? string.Empty : oldName.UrlDecode();//oldName play role as key
            model.CustomScoreName = model.CustomScoreName.UrlDecode();
            model.Description = model.Description.UrlDecode();
            if (!string.IsNullOrEmpty(model.ListTextString) && !model.ListTextString.Contains("Option"))
            {
                model.ListTextString = model.ListTextString.UrlDecode();
            }
            model.ListArtifactTagString = model.ListArtifactTagString.UrlDecode();
            if (string.IsNullOrEmpty(model.ScoreNameAbsolute))
            {
                return Json(new
                {
                    success = false,
                    error = "Please input name."
                }, JsonRequestBehavior.AllowGet);
            }
            if (model.IsNumeric && model.MinScore.HasValue && model.MaxScore.HasValue && model.MaxScore.Value < model.MinScore.Value)
            {
                return Json(new
                {
                    success = false,
                    error = "Max Score must equal or greater than Min Score."
                }, JsonRequestBehavior.AllowGet);
            }
            //check the name
            var name = string.Empty;
            if (model.IsCustomScoreType)
            {
                name = model.CustomScoreName;
            }
            else
            {
                name = model.ScoreName;
            }

            if (model.ScoreTypeCode == ScoreTypeModel.NOTE_COMMENT)
            {
                return UpdateNoteScoreType(oldName, model);
            }

            ScoreTypeModel existingScoreType = null;

            if (model.SubscoreId.HasValue && model.SubscoreId.Value > 0)
            {
                //score type of subscore
                if (!oldName.ToLower().Equals(name.ToLower()))
                {
                    //user has tried to change the name -> check to make sure the name is not dupliated
                    var anotherScoreType = _parameters.DataLockerTemplateService.GetScoreTypeOfSubscore(model.SubscoreId.Value,
                        model.ScoreNameAbsolute);
                    if (anotherScoreType != null)//there's an existing score type has the same name
                    {
                        return Json(new { success = false, error = model.GetDuplicateNameErrorMessage() },
                                 JsonRequestBehavior.AllowGet);
                    }
                }
                if (_parameters.DataLockerTemplateService.HasAssociatedTestResult(model.TemplateID))
                {
                    //get the existing scoretype
                    existingScoreType = _parameters.DataLockerTemplateService.GetScoreTypeOfSubscore(model.SubscoreId.Value,
                       oldName);//include meta information
                    var validateMessage = model.ValidateChange(true, existingScoreType);
                    if (!string.IsNullOrEmpty(validateMessage))
                    {
                        return Json(new
                        {
                            success = false,
                            error = validateMessage
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var validateMessage = model.ValidateChange(false, null);
                    if (!string.IsNullOrEmpty(validateMessage))
                    {
                        return Json(new
                        {
                            success = false,
                            error = validateMessage
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                _parameters.DataLockerTemplateService.UpdateScoreType(CurrentUser.Id, oldName, model);
                return Json(new
                {
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //score type of Template
                if (!oldName.ToLower().Equals(name.ToLower()))
                {
                    //user has tried to change the name -> check to make sure the name is not dupliated
                    var existing = _parameters.DataLockerTemplateService.TemplateScoreTypeIsExisting(model.TemplateID,
                   model);
                    if (existing)
                    {
                        return Json(new
                        {
                            success = false,
                            error = model.GetDuplicateNameErrorMessage()
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (_parameters.DataLockerTemplateService.HasAssociatedTestResult(model.TemplateID))
                {
                    //get the existing scoretype
                    existingScoreType = _parameters.DataLockerTemplateService.GetScoreTypeOfTemplate(model.TemplateID,
                        oldName); //include meta information
                    var validateMessage = model.ValidateChange(true, existingScoreType);
                    if (!string.IsNullOrEmpty(validateMessage))
                    {
                        return Json(new { success = false, error = validateMessage },
                            JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var validateMessage = model.ValidateChange(false, null);
                    if (!string.IsNullOrEmpty(validateMessage))
                    {
                        return Json(new { success = false, error = validateMessage },
                            JsonRequestBehavior.AllowGet);
                    }
                }

                _parameters.DataLockerTemplateService.UpdateScoreType(CurrentUser.Id, oldName, model);

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteScoreType(int templateId, string name, string scoreTypeName, int? subscoreId)
        {
            name = name.UrlDecode();//name play role as key
            if (_parameters.DataLockerTemplateService.HasAssociatedTestResult(templateId))
            {
                return Json(new
                {
                    success = false,
                    error = "Unable to delete score type when the template has associated test result."
                }, JsonRequestBehavior.AllowGet);
            }
            if (_parameters.DataLockerTemplateService.HasAssociatedAutoSave(templateId))
            {
                return Json(new
                {
                    success = false,
                    error = "Unable to delete subscore when the template has associated with previously entered results that were not saved."
                }, JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrEmpty(scoreTypeName) && scoreTypeName.Equals("Notes/Comments"))
            {
                _parameters.DataLockerTemplateService.DeleteScoreTypeNote(CurrentUser.Id, templateId, name, subscoreId);
            }
            else
            {
                _parameters.DataLockerTemplateService.DeleteScoreType(CurrentUser.Id, templateId, name, subscoreId);
            }

            //_parameters.VirtualTestCustomMetaDataService.ReassignMetaDataOrder(templateId, subscoreId);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadAddSubscore(int templateId)
        {
            var model = new VirtualTestCustomSubScore()
            {
                VirtualTestCustomScoreId = templateId
            };
            return PartialView("_AddSubscore", model);
        }

        public ActionResult AddNewSubscore(VirtualTestCustomSubScore model)
        {
            model.Name = model.Name.UrlDecode();
            model.Name = model.Name.Trim();
            if (string.IsNullOrEmpty(model.Name))
            {
                return Json(new
                {
                    success = false,
                    error = "Please input the name of new subscore."
                }, JsonRequestBehavior.AllowGet);
            }
            if (_parameters.DataLockerTemplateService.HasAssociatedTestResult(model.VirtualTestCustomScoreId))
            {
                return Json(new
                {
                    success = false,
                    error = "Unable to add new subscore when the template has associated test result."
                }, JsonRequestBehavior.AllowGet);
            }
            //check existing
            var existingSubscore = _parameters.VirtualTestCustomSubScoreService.GetByName(
                model.VirtualTestCustomScoreId, model.Name);
            if (existingSubscore != null)
            {
                return Json(new
                {
                    success = false,
                    error = "A subscore with the same name already exists."
                }, JsonRequestBehavior.AllowGet);
            }
            //get the max sequence
            var maxSequence =
                _parameters.VirtualTestCustomSubScoreService.GetMaxSequenceOfSubscores(model.VirtualTestCustomScoreId);
            model.Sequence = maxSequence + 1;
            model = _parameters.VirtualTestCustomSubScoreService.Create(CurrentUser, model);
            return Json(new
            {
                success = true,
                SubscoreId = model.VirtualTestCustomSubScoreId
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadEditSubscore(int VirtualTestCustomScoreId, int virtualTestCustomSubScoreID, bool hasAssociatedTestResult, bool hasAssociatedAutoSave)
        {
            var subscore = _parameters.VirtualTestCustomSubScoreService.GetById(virtualTestCustomSubScoreID);
            subscore.HasAssociatedTestResult = hasAssociatedTestResult;
            subscore.HasAssociatedAutoSave = hasAssociatedAutoSave;
            subscore.HasCustomScoreType = HasCustomScoreType(VirtualTestCustomScoreId, virtualTestCustomSubScoreID);
            subscore.HasConversionSet = HasConversionSet(VirtualTestCustomScoreId, virtualTestCustomSubScoreID);
            return PartialView("_EditSubscore", subscore);
        }

        [HttpPost]
        public ActionResult UpdateTemplateName(int templateId, string name, bool? isMultiDate)
        {
            name = name.UrlDecode();
            name = name.Trim();
            if (string.IsNullOrEmpty(name))
            {
                return Json(new { success = false, error = "Please input the name of Template." }, JsonRequestBehavior.AllowGet);
            }
            var existingObj =
              _parameters.VirtualTestCustomScoreService.GetCustomScoreByNameAndDistrictID(
                  CurrentUser.DistrictId ?? 0, name);
            if (existingObj != null)
            {
                if (existingObj.VirtualTestCustomScoreId != templateId)
                {
                    return Json(new { success = false, error = "This template name is already in use. Please refer to that template or use a different name." }, JsonRequestBehavior.AllowGet);
                }
            }

            var newItem = _parameters.VirtualTestCustomScoreService.UpdateCustomScoreName(CurrentUser.Id, templateId, name, isMultiDate);

            if (isMultiDate.HasValue && isMultiDate.Value)
            {
                var districtIdsPublish = _parameters.DataLockerTemplateService.GetPublishedDistrictIdsForTemplateId(templateId);
                if (districtIdsPublish != null && districtIdsPublish.Count() > 0)
                {
                    var checkDistrictDecodeExistDistricts = _parameters.DistrictDecodeService.CheckDistrictDecodeExistDistricts(
                            districtIdsPublish, Constanst.UseMultiDateTemplate);
                    if (!checkDistrictDecodeExistDistricts)
                    {
                        return Json(new { success = true, message = "This template is published to districts without access to multi-date templates. Changing this template to multi-date will no longer allow those districts to view the template." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { success = true, name = newItem.Name, message = "" },
                            JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveSubscoreName(VirtualTestCustomSubScore model)
        {
            model.Name = model.Name.UrlDecode();
            model.Name = model.Name.Trim();

            if (string.IsNullOrEmpty(model.Name))
            {
                return Json(new
                {
                    success = false,
                    error = "Please input the name of new subscore."
                }, JsonRequestBehavior.AllowGet);
            }
            if (_parameters.DataLockerTemplateService.HasAssociatedTestResult(model.VirtualTestCustomScoreId))
            {
                return Json(new
                {
                    success = false,
                    error = "Unable to update subscore when the template has associated test result."
                }, JsonRequestBehavior.AllowGet);
            }
            //check existing
            var existingSubScoreName = _parameters.VirtualTestCustomSubScoreService.CheckExistSubScoreName(
                model.VirtualTestCustomScoreId, model.VirtualTestCustomSubScoreId, model.Name);

            if (existingSubScoreName)
            {
                return Json(new { success = false, error = "A subscore with the same name already exists." },
                                 JsonRequestBehavior.AllowGet);
            }
            var newItem = _parameters.DataLockerTemplateService.UpdateSubscoreName(model.VirtualTestCustomSubScoreId, model.Name);
            return Json(new { success = true, SubscoreId = newItem.VirtualTestCustomSubScoreId },
                            JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadSubscoreScoreTypeList(int virtualTestCustomSubScoreID)
        {
            var data = new List<ScoreTypeListItem>();
            //int id = 1;

            var scoreTypeList = _parameters.DataLockerTemplateService.GetScoreTypesOfSubscore(virtualTestCustomSubScoreID);

            var noteScoreType = scoreTypeList.FirstOrDefault(x => x.ScoreTypeCode == VirtualTestCustomMetaData.NOTE_COMMENT);

            if (noteScoreType != null)
            {
                scoreTypeList.Remove(noteScoreType);
            }

            if (noteScoreType != null && noteScoreType.ListNoteComment != null)
            {
                foreach (var note in noteScoreType.ListNoteComment)
                {
                    scoreTypeList.Add(new ScoreTypeModel()
                    {
                        DisplayOrder = note.Order ?? 0,
                        ScoreTypeCode = noteScoreType.ScoreTypeCode,
                        Description = note.Description,
                        CustomScoreName = note.NoteName,
                        ScoreName = noteScoreType.ScoreName,
                        ScoreType = noteScoreType.ScoreType
                    });
                }
            }
            var assessmentArtifactFileTypeGroups = Mapper.Map<IEnumerable<AssessmentArtifactFileTypeGroupEditScoreModel>>
                 (_parameters.DistrictDecodeService
                  .GetAssessmentArtifactFileTypeGroups(CurrentUser.DistrictId ?? 0)).ToList();
            data = scoreTypeList.OrderBy(x => x.DisplayOrder).Select(x => new ScoreTypeListItem()
            {
                Id = x.DisplayOrder,
                ScoreTypeCode = x.ScoreTypeCode,
                ScoreTypeName = x.ScoreTypeName,
                ScoreName = !string.IsNullOrEmpty(x.CustomScoreName) ? x.CustomScoreName : x.ScoreName,
                Description = x.Description,
                Overview = x.ScoreTypeCode != ScoreTypeModel.ARTIFACT_SCORE ? x.Overview : GetStringDisplayNameUploadFileType(assessmentArtifactFileTypeGroups, x.UploadFileTypes),
                VirtualTestCustomSubScoreID = x.SubscoreId,
                ShortOverview = x.ShortOverview,
                IsPredefinedList = x.IsListText,
                ShortScoreType = x.ScoreType,
                IsAutoCalculation = x.Meta.IsAutoCalculation,
                MaxScore = GetMaxScoreValue(x.ListText, x.IsNumeric, x.Meta.MaxValue.GetValueOrDefault()),
                VirtualTestCustomMetaDataID = x.VirtualTestCustomMetaDataID
            }).ToList();

            var parser = new DataTableParser<ScoreTypeListItem>();
            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        private string GetStringDisplayNameUploadFileType(List<AssessmentArtifactFileTypeGroupEditScoreModel> fileTypeConfigs, string fileType)
        {
            if (fileTypeConfigs != null && !string.IsNullOrWhiteSpace(fileType))
            {
                var listUploadFileTypes = fileType.Split(',');
                var typeJoin = string.Join(", ", fileTypeConfigs.Where(w => listUploadFileTypes.Contains(w.Name)).Select(s => s.DisplayName).ToList());
                return string.Format("- File Types: {0}", typeJoin);
            }
            return "";
        }

        [HttpPost]
        public ActionResult DeleteSubscore(int virtualTestCustomScoreId, int virtualTestCustomSubScoreId)
        {
            if (_parameters.DataLockerTemplateService.HasAssociatedTestResult(virtualTestCustomScoreId))
            {
                return Json(new
                {
                    success = false,
                    error = "Unable to delete subscore when the template has associated test result."
                }, JsonRequestBehavior.AllowGet);
            }
            if (_parameters.DataLockerTemplateService.HasAssociatedAutoSave(virtualTestCustomScoreId))
            {
                return Json(new
                {
                    success = false,
                    error = "Unable to delete subscore when the template has associated with previously entered results that were not saved."
                }, JsonRequestBehavior.AllowGet);
            }
            if (_parameters.VirtualTestCustomMetaDataService.HasIsAutoCalculationScoreType(virtualTestCustomScoreId, virtualTestCustomSubScoreId))
            {
                return Json(new
                {
                    success = false,
                    error = "You cannot delete this subscore. It has the column(s) that are being used by a calculation. Please remove the calculation first."
                }, JsonRequestBehavior.AllowGet);
            }
            _parameters.DataLockerTemplateService.DeleteSubscore(virtualTestCustomSubScoreId);
            return Json(new { success = true, subscoreId = virtualTestCustomSubScoreId },
                            JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteTemplate(int templateId)
        {
            if (_parameters.DataLockerTemplateService.HasAssociatedTest(templateId))
            {
                return Json(new
                {
                    success = false,
                    error = "Unable to delete Template when The template has associated Test."
                }, JsonRequestBehavior.AllowGet);
            }
            _parameters.DataLockerTemplateService.DeleteTemplate(CurrentUser.Id, templateId);

            return Json(new { success = true },
                            JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ArchiveTemplate(int templateId, bool archived)
        {
            _parameters.DataLockerTemplateService.ArchiveTemplate(templateId, archived);

            return Json(new { success = true },
                            JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateSubscoreSequence(string subscoreIdListString)
        {
            var subscoreIdList = subscoreIdListString.ParseIdsFromString();
            if (subscoreIdList != null)
            {
                for (int i = 0; i < subscoreIdList.Count; i++)
                {
                    var subscore = _parameters.VirtualTestCustomSubScoreService.GetById(subscoreIdList[i]);
                    subscore.Sequence = i + 1;//Sequence starts from 1
                    _parameters.VirtualTestCustomSubScoreService.Save(subscore);
                }
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadViewCloneTemplate()
        {
            return PartialView("_CloneTemplate");
        }

        [HttpPost]
        public ActionResult UpdateMetaDataOrder(OrderScoreTypeUpdateModel scores)
        {
            bool result = UpdateVirtualTestCustomMetaDataOrder(scores);
            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }

        private bool UpdateVirtualTestCustomMetaDataOrder(OrderScoreTypeUpdateModel score)
        {
            try
            {
                var metaDataNote = new VirtualTestCustomMetaData();
                var metaModel = new VirtualTestCustomMetaModel() { ListNoteComment = new List<VirtualTestCustomMetaNoteCommentModel>() };
                //case add new
                if (score.RawIndex == null)
                {
                    //case  add new for sub score
                    if (score.SubScoreId != null && score.SubScoreId > 0)
                    {
                        metaDataNote =
                            _parameters.VirtualTestCustomMetaDataService.Select()
                                .FirstOrDefault(x => x.VirtualTestCustomScoreID == score.ScoreId
                                                     && x.VirtualTestCustomSubScoreID == score.SubScoreId
                                                     && x.ScoreType == ScoreTypeModel.NOTE_COMMENT);
                        if (metaDataNote != null)
                        {
                            metaModel = metaDataNote.ParseMetaToObject();
                        }
                        var scoreSubAll = _parameters.DataLockerTemplateService.GetScoreTypesOfSubscore(score.SubScoreId.Value);
                        int lastedOrder = scoreSubAll.OrderByDescending(o => o.DisplayOrder).First()?.DisplayOrder ?? 0;
                        if (metaModel != null && metaModel.ListNoteComment != null && metaModel.ListNoteComment.Any())
                        {
                            var lastedOrderComment = metaModel.ListNoteComment.OrderByDescending(o => o.Order).First()?.Order ?? 0;
                            if (lastedOrderComment > lastedOrder)
                            {
                                lastedOrder = lastedOrderComment;
                            }
                        }
                        foreach (var item in scoreSubAll)
                        {
                            if (item.ScoreType == score.ScoreType)
                            {
                                if (item.ScoreType == ScoreTypeModel.NOTE_COMMENT)
                                {
                                    if (item.ListNoteComment != null && item.ListNoteComment.Count == 1)
                                    {
                                        item.DisplayOrder = lastedOrder + 1;
                                        item.ListNoteComment[0].Order = lastedOrder + 1;
                                    }
                                    else
                                    {
                                        var note = item.ListNoteComment.FirstOrDefault(x => x.NoteName == score.Name);
                                        if (note != null && (note.Order == null || note.Order == -1))
                                            note.Order = lastedOrder + 1;
                                    }
                                }
                                else
                                {
                                    item.DisplayOrder = lastedOrder + 1;
                                }
                            }
                        }
                        var noteUpdate = scoreSubAll.FirstOrDefault(f => f.ScoreType == ScoreTypeModel.NOTE_COMMENT);
                        if (noteUpdate != null && noteUpdate.ListNoteComment != null && noteUpdate.ListNoteComment.Count > 0)
                        {
                            metaModel.ListNoteComment = noteUpdate.ListNoteComment;
                            metaDataNote.MetaData = metaModel.GetJsonString();
                            _parameters.VirtualTestCustomMetaDataService.Save(metaDataNote);
                        }
                        //update order for another score
                        var scoreTypeUpdated = scoreSubAll.Where(x => x.ScoreType != ScoreTypeModel.NOTE_COMMENT).Select(s =>
                                new UpdateOrderScoreTypeModel()
                                {
                                    ScoreType = s.ScoreType,
                                    Name = s.ScoreTypeName,
                                    Order = s.DisplayOrder
                                }).ToList();
                        _parameters.VirtualTestCustomMetaDataService.UpdateOrderForMetaData(score.ScoreId, score.SubScoreId,
                            XmlUtils.BuildXml(scoreTypeUpdated));
                    }
                    else //case  add new for overall
                    {
                        metaDataNote =
                            _parameters.VirtualTestCustomMetaDataService.Select()
                                .FirstOrDefault(x => x.VirtualTestCustomScoreID == score.ScoreId
                                                     && x.VirtualTestCustomSubScoreID.HasValue == false
                                                     && x.ScoreType == ScoreTypeModel.NOTE_COMMENT);
                        if (metaDataNote != null)
                        {
                            metaModel = metaDataNote.ParseMetaToObject();
                        }

                        var scoreOverallAll = _parameters.DataLockerTemplateService.GetScoreTypesOfTemplate(score.ScoreId);
                        int lastedOrder = scoreOverallAll?.OrderByDescending(o => o.DisplayOrder).First()?.DisplayOrder ?? 0;
                        if (metaModel != null && metaModel.ListNoteComment != null && metaModel.ListNoteComment.Any())
                        {
                            var lastedOrderComment = metaModel.ListNoteComment.OrderByDescending(o => o.Order).First()?.Order ?? 0;
                            if (lastedOrderComment > lastedOrder)
                            {
                                lastedOrder = lastedOrderComment;
                            }
                        }
                        foreach (var item in scoreOverallAll)
                        {
                            if (item.ScoreType == score.ScoreType)
                            {
                                if (item.ScoreType == ScoreTypeModel.NOTE_COMMENT)
                                {
                                    if (item.ListNoteComment != null && item.ListNoteComment.Count == 1)
                                    {
                                        item.ListNoteComment[0].Order = lastedOrder;
                                    }
                                    else if (item.ListNoteComment != null)
                                    {
                                        var note = item.ListNoteComment.FirstOrDefault(x => x.NoteName == score.Name);
                                        if (note != null && (!note.Order.HasValue || note.Order == -1))
                                        {
                                            note.Order = lastedOrder + 1;
                                        }
                                    }
                                }
                                else
                                {
                                    item.DisplayOrder = lastedOrder + 1;
                                }
                            }
                        }
                        var noteUpdate = scoreOverallAll.FirstOrDefault(f => f.ScoreType == ScoreTypeModel.NOTE_COMMENT);
                        if (noteUpdate != null && noteUpdate.ListNoteComment != null && noteUpdate.ListNoteComment.Count > 0)
                        {
                            metaModel.ListNoteComment = noteUpdate.ListNoteComment;
                            metaDataNote.MetaData = metaModel.GetJsonString();
                            _parameters.VirtualTestCustomMetaDataService.Save(metaDataNote);
                        }
                        //update order for another score
                        var scoreTypeUpdated = scoreOverallAll.Where(x => x.ScoreType != ScoreTypeModel.NOTE_COMMENT).Select(s =>
                            new UpdateOrderScoreTypeModel()
                            {
                                ScoreType = s.ScoreType,
                                Name = s.ScoreTypeName,
                                Order = s.DisplayOrder
                            }).ToList();
                        _parameters.VirtualTestCustomMetaDataService.UpdateOrderForMetaData(score.ScoreId, score.SubScoreId,
                            XmlUtils.BuildXml(scoreTypeUpdated));
                    }
                }
                else // case drag and drop or clone
                {
                    //case  add new for sub score
                    if (score.SubScoreId != null && score.SubScoreId > 0)
                    {
                        var scoreSubAll = _parameters.DataLockerTemplateService.GetScoreTypesOfSubscore(score.SubScoreId.Value);
                        var noteScoreType = scoreSubAll.FirstOrDefault(x => x.ScoreTypeCode == VirtualTestCustomMetaData.NOTE_COMMENT);

                        var allScores = scoreSubAll;
                        if (noteScoreType != null)
                        {
                            allScores.Remove(noteScoreType);
                        }

                        if (noteScoreType != null && noteScoreType.ListNoteComment != null)
                        {
                            foreach (var note in noteScoreType.ListNoteComment)
                            {
                                allScores.Add(new ScoreTypeModel()
                                {
                                    DisplayOrder = note.Order ?? 0,
                                    ScoreTypeCode = noteScoreType.ScoreTypeCode,
                                    Description = note.Description,
                                    CustomScoreName = note.NoteName,
                                    ScoreName = noteScoreType.ScoreName,
                                    ScoreType = noteScoreType.ScoreType
                                });
                            }
                        }
                        allScores = allScores.OrderBy(x => x.DisplayOrder).ToList();

                        var rawUpdatedFrom = allScores
                            .Select((item, index) => new { item, index })
                            .FirstOrDefault(x => x.index + 1 == score.RawIndex)?.item
                            ?? new ScoreTypeModel();
                        var fixedIndex = rawUpdatedFrom.DisplayOrder;
                        foreach (var item in scoreSubAll)
                        {
                            if (item.ScoreType == score.ScoreType)
                            {
                                if (item.ScoreType == ScoreTypeModel.NOTE_COMMENT)
                                {
                                    var reOrderNotes = item.ListNoteComment.OrderByDescending(w => w.Order).ToList();
                                    foreach (var note in reOrderNotes)
                                    {
                                        if (note.NoteName == score.Name)
                                        {
                                            note.Order = fixedIndex;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    item.DisplayOrder = fixedIndex;
                                }
                            }
                            else if (item.DisplayOrder >= fixedIndex)
                            {
                                if (item.ScoreType == ScoreTypeModel.NOTE_COMMENT)
                                {
                                    var reOrderNotes = item.ListNoteComment.OrderByDescending(w => w.Order).ToList();
                                    foreach (var note in reOrderNotes)
                                    {
                                        if (note.Order >= fixedIndex)
                                        {
                                            note.Order = note.Order + 1;
                                        }
                                    }
                                }
                                else
                                {
                                    item.DisplayOrder = item.DisplayOrder + 1;
                                }
                            }
                        }
                        var noteUpdate = scoreSubAll.FirstOrDefault(f => f.ScoreType == ScoreTypeModel.NOTE_COMMENT);
                        if (noteUpdate != null && noteUpdate.ListNoteComment != null && noteUpdate.ListNoteComment.Count > 0)
                        {
                            metaModel.ListNoteComment = noteUpdate.ListNoteComment;
                            metaDataNote.MetaData = metaModel.GetJsonString();
                            _parameters.VirtualTestCustomMetaDataService.Save(metaDataNote);
                        }
                        //update order for another score
                        var scoreTypeUpdated = scoreSubAll.Where(x => x.ScoreType != ScoreTypeModel.NOTE_COMMENT).Select(s =>
                                new UpdateOrderScoreTypeModel()
                                {
                                    ScoreType = s.ScoreType,
                                    Name = s.ScoreTypeName,
                                    Order = s.DisplayOrder
                                }).ToList();
                        _parameters.VirtualTestCustomMetaDataService.UpdateOrderForMetaData(score.ScoreId, score.SubScoreId,
                            XmlUtils.BuildXml(scoreTypeUpdated));
                    }
                    else //case  add new for overall
                    {
                        var scoreOverallAll = _parameters.DataLockerTemplateService.GetScoreTypesOfTemplate(score.ScoreId);

                        var noteScoreType = scoreOverallAll.FirstOrDefault(x => x.ScoreTypeCode == VirtualTestCustomMetaData.NOTE_COMMENT);

                        var allScores = scoreOverallAll;
                        if (noteScoreType != null)
                        {
                            allScores.Remove(noteScoreType);
                        }

                        if (noteScoreType != null && noteScoreType.ListNoteComment != null)
                        {
                            foreach (var note in noteScoreType.ListNoteComment)
                            {
                                allScores.Add(new ScoreTypeModel()
                                {
                                    DisplayOrder = note.Order ?? 0,
                                    ScoreTypeCode = noteScoreType.ScoreTypeCode,
                                    Description = note.Description,
                                    CustomScoreName = note.NoteName,
                                    ScoreName = noteScoreType.ScoreName,
                                    ScoreType = noteScoreType.ScoreType
                                });
                            }
                        }
                        allScores = allScores.OrderBy(x => x.DisplayOrder).ToList();

                        var rawUpdatedFrom = allScores
                            .Select((item, index) => new { item, index })
                            .FirstOrDefault(x => x.index + 1 == score.RawIndex)?.item
                            ?? new ScoreTypeModel();
                        var fixedIndex = rawUpdatedFrom.DisplayOrder;
                        foreach (var item in scoreOverallAll)
                        {
                            if (item.ScoreType == score.ScoreType)
                            {
                                if (item.ScoreType == ScoreTypeModel.NOTE_COMMENT)
                                {
                                    var reOrderNotes = item.ListNoteComment.OrderByDescending(w => w.Order).ToList();
                                    foreach (var note in reOrderNotes)
                                    {
                                        if (note.NoteName == score.Name)
                                        {
                                            note.Order = fixedIndex;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    item.DisplayOrder = fixedIndex;
                                }
                            }
                            else if (item.DisplayOrder >= fixedIndex)
                            {
                                if (item.ScoreType == ScoreTypeModel.NOTE_COMMENT)
                                {
                                    var reOrderNotes = item.ListNoteComment.OrderByDescending(w => w.Order).ToList();
                                    foreach (var note in reOrderNotes)
                                    {
                                        if (note.Order >= fixedIndex)
                                        {
                                            note.Order = note.Order + 1;
                                        }
                                    }
                                }
                                else
                                {
                                    item.DisplayOrder = item.DisplayOrder + 1;
                                }
                            }
                        }
                        var noteUpdate = scoreOverallAll.FirstOrDefault(f => f.ScoreType == ScoreTypeModel.NOTE_COMMENT);
                        if (noteUpdate != null && noteUpdate.ListNoteComment != null && noteUpdate.ListNoteComment.Count > 0)
                        {
                            metaModel.ListNoteComment = noteUpdate.ListNoteComment;
                            metaDataNote.MetaData = metaModel.GetJsonString();
                            _parameters.VirtualTestCustomMetaDataService.Save(metaDataNote);
                        }
                        //update order for another score
                        var scoreTypeUpdated = scoreOverallAll.Where(x => x.ScoreType != ScoreTypeModel.NOTE_COMMENT).Select(s =>
                                new UpdateOrderScoreTypeModel()
                                {
                                    ScoreType = s.ScoreType,
                                    Name = s.ScoreTypeName,
                                    Order = s.DisplayOrder
                                }).ToList();
                        _parameters.VirtualTestCustomMetaDataService.UpdateOrderForMetaData(score.ScoreId, score.SubScoreId,
                            XmlUtils.BuildXml(scoreTypeUpdated));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return false;
            }
        }

        [HttpPost]
        public ActionResult UpdateMetaDataRaw(ScoreTypeDrapDropModel model)
        {
            try
            {
                model.Name = HttpUtility.UrlDecode(HttpUtility.UrlDecode(model.Name));
                bool result = true;
                var scoreAdd = new ScoreTypeModel();
                var scoreTypesCompare = new List<string>()
                {
                    VirtualTestCustomMetaData.CustomN_1, VirtualTestCustomMetaData.CustomN_2, VirtualTestCustomMetaData.CustomN_3, VirtualTestCustomMetaData.CustomN_4,
                    VirtualTestCustomMetaData.CustomA_1, VirtualTestCustomMetaData.CustomA_2, VirtualTestCustomMetaData.CustomA_3, VirtualTestCustomMetaData.CustomA_4
                };

                if (_parameters.DataLockerTemplateService.HasAssociatedAutoSave(model.ScoreId))
                {
                    return Json(new
                    {
                        success = false,
                        error = "Unable to drag and drop when the template has associated with previously entered results that were not saved."
                    }, JsonRequestBehavior.AllowGet);
                }

                //delete scope type has exits
                if (model.IsFromScore)
                {
                    if (model.ScoreTypeName.Equals(ScoreTypeConstants.NoteComment))
                    {
                        scoreAdd = _parameters.DataLockerTemplateService.GetNoteScoreOfTemplate(model.From);
                        var note = scoreAdd.ListNoteComment.FirstOrDefault(x => x.NoteName == model.Name);
                        scoreAdd.NoteDefaultValue = note.DefaultValue;
                        scoreAdd.ScoreName = note.NoteName;
                        scoreAdd.CustomScoreName = note.NoteName;
                        scoreAdd.Description = note.Description;
                        scoreAdd.NoteType = note.NoteType;
                    }
                    else
                    {
                        scoreAdd = _parameters.DataLockerTemplateService.GetScoreTypeOfTemplate(model.From, model.Name);
                    }

                    if (scoreAdd != null && scoreAdd.IsAutoCalculation && scoreAdd.IsCustomScoreType)
                    {
                        return Json(new { success = false, message = "Can not move this column" }, JsonRequestBehavior.AllowGet);
                    }
                    //delete row
                    if (!string.IsNullOrEmpty(model.ScoreTypeName) && model.ScoreTypeName.Equals(ScoreTypeConstants.NoteComment))
                    {
                        _parameters.DataLockerTemplateService.DeleteScoreTypeNote(CurrentUser.Id, model.ScoreId, model.Name, null);
                    }
                    else
                    {
                        _parameters.DataLockerTemplateService.DeleteScoreType(CurrentUser.Id, model.From, model.Name, null);
                    }
                    //delete row duplicate scoreType
                    if (model.IsReplace.HasValue && model.IsReplace.Value)
                    {
                        model.Name = model.NameReplace;
                        model.ScoreTypeName = model.ScoreTypeNameReplace;
                    }
                    var scopeTypeSubScopeOld = _parameters.VirtualTestCustomMetaDataService.Select()
                            .FirstOrDefault(x => x.VirtualTestCustomSubScoreID == model.To && x.ScoreType == model.ScoreTypeName);
                    if (scopeTypeSubScopeOld != null)
                    {
                        if (scoreTypesCompare.Contains(model.ScoreTypeName))
                        {
                            var subScoreInfor = _parameters.VirtualTestCustomSubScoreService.GetById(scopeTypeSubScopeOld.VirtualTestCustomSubScoreID.Value);
                            if (subScoreInfor != null)
                            {
                                var oldName = subScoreInfor.GetType().GetProperty(string.Format("{0}Label", model.ScoreTypeName.Replace("_", ""))).GetValue(subScoreInfor, null).ToString();
                                if (!string.IsNullOrEmpty(oldName) && oldName == model.Name)
                                {
                                    _parameters.DataLockerTemplateService.DeleteScoreType(CurrentUser.Id, model.ScoreId, oldName, model.To);
                                }
                            }
                        }
                        else
                        {
                            if (model.ScoreTypeName == VirtualTestCustomMetaData.NOTE_COMMENT)
                            {
                                var notInfor = _parameters.DataLockerTemplateService.GetNoteScoreOfSubScore(model.ScoreId, scopeTypeSubScopeOld.VirtualTestCustomSubScoreID.Value);
                                if (notInfor != null)
                                {
                                    var note = notInfor.ListNoteComment.FirstOrDefault(x => x.NoteName == model.Name);
                                    if (note != null)
                                    {
                                        _parameters.DataLockerTemplateService.DeleteScoreTypeNote(CurrentUser.Id, model.ScoreId, model.Name, model.To);
                                    }
                                }
                            }
                            else
                            {
                                _parameters.DataLockerTemplateService.DeleteScoreType(CurrentUser.Id, model.ScoreId, model.Name, model.To);
                            }
                        }
                    }
                    scoreAdd.SubscoreId = model.To;
                    _parameters.DataLockerTemplateService.CreateNewSubscoreScoreType(CurrentUser.Id, scoreAdd);
                    result = UpdateVirtualTestCustomMetaDataOrder(new OrderScoreTypeUpdateModel
                    {
                        ScoreId = model.ScoreId,
                        SubScoreId = scoreAdd.SubscoreId,
                        ScoreType = scoreAdd.ScoreType,
                        Name = scoreAdd.ScoreName,
                        RawIndex = model.RawIndex
                    });
                }
                else
                {
                    if (model.ScoreTypeName.Equals(ScoreTypeConstants.NoteComment))
                    {
                        scoreAdd = _parameters.DataLockerTemplateService.GetNoteScoreOfSubScore(model.ScoreId, model.From);
                        var note = scoreAdd.ListNoteComment.FirstOrDefault(x => x.NoteName == model.Name);
                        scoreAdd.NoteDefaultValue = note.DefaultValue;
                        scoreAdd.ScoreName = note.NoteName;
                        scoreAdd.CustomScoreName = note.NoteName;
                        scoreAdd.Description = note.Description;
                        scoreAdd.NoteType = note.NoteType;
                    }
                    else
                    {
                        scoreAdd = _parameters.DataLockerTemplateService.GetScoreTypeOfSubscore(model.From, model.Name);
                    }

                    if (scoreAdd != null && scoreAdd.IsAutoCalculation && scoreAdd.IsCustomScoreType)
                    {
                        return Json(new { success = false, message = "Can not move this column" }, JsonRequestBehavior.AllowGet);
                    }
                    //delete row
                    if (!string.IsNullOrEmpty(model.ScoreTypeName) && model.ScoreTypeName.Equals(ScoreTypeConstants.NoteComment))
                    {
                        _parameters.DataLockerTemplateService.DeleteScoreTypeNote(CurrentUser.Id, model.ScoreId, model.Name, model.From);
                    }
                    else
                    {
                        _parameters.DataLockerTemplateService.DeleteScoreType(CurrentUser.Id, model.ScoreId, model.Name, model.From);
                    }
                    //delete row duplicate scoreType
                    if (model.IsReplace.HasValue && model.IsReplace.Value)
                    {
                        model.Name = model.NameReplace;
                        model.ScoreTypeName = model.ScoreTypeNameReplace;
                    }
                    if (model.IsSubSub.HasValue && model.IsSubSub.Value)
                    {
                        //delete row duplicate scoreType
                        var scopeTypeScopeOld = _parameters.VirtualTestCustomMetaDataService.Select()
                                .FirstOrDefault(x => x.VirtualTestCustomSubScoreID == model.To && x.ScoreType == model.ScoreTypeName);
                        //delete row
                        if (scopeTypeScopeOld != null)
                        {
                            if (scoreTypesCompare.Contains(model.ScoreTypeName))
                            {
                                var subScoreInfor = _parameters.VirtualTestCustomSubScoreService.GetById(scopeTypeScopeOld.VirtualTestCustomSubScoreID.Value);
                                if (subScoreInfor != null)
                                {
                                    var oldName = subScoreInfor.GetType().GetProperty(string.Format("{0}Label", model.ScoreTypeName.Replace("_", ""))).GetValue(subScoreInfor, null).ToString();
                                    if (!string.IsNullOrEmpty(oldName) && oldName == model.Name)
                                    {
                                        _parameters.DataLockerTemplateService.DeleteScoreType(CurrentUser.Id, model.ScoreId, oldName, model.To);
                                    }
                                }
                            }
                            else
                            {
                                if (model.ScoreTypeName == VirtualTestCustomMetaData.NOTE_COMMENT)
                                {
                                    var notInfor = _parameters.DataLockerTemplateService.GetNoteScoreOfSubScore(model.ScoreId, scopeTypeScopeOld.VirtualTestCustomSubScoreID.Value);
                                    if (notInfor != null)
                                    {
                                        var note = notInfor.ListNoteComment.FirstOrDefault(x => x.NoteName == model.Name);
                                        if (note != null)
                                        {
                                            _parameters.DataLockerTemplateService.DeleteScoreTypeNote(CurrentUser.Id, model.ScoreId, model.Name, model.To);
                                        }
                                    }
                                }
                                else
                                {
                                    _parameters.DataLockerTemplateService.DeleteScoreType(CurrentUser.Id, model.ScoreId, model.Name, model.To);
                                }
                            }
                        }
                        scoreAdd.SubscoreId = model.To;
                        _parameters.DataLockerTemplateService.CreateNewSubscoreScoreType(CurrentUser.Id, scoreAdd);
                        result = UpdateVirtualTestCustomMetaDataOrder(new OrderScoreTypeUpdateModel
                        {
                            ScoreId = model.ScoreId,
                            SubScoreId = scoreAdd.SubscoreId,
                            ScoreType = scoreAdd.ScoreType,
                            Name = scoreAdd.ScoreName,
                            RawIndex = model.RawIndex
                        });
                    }
                    else
                    {
                        //delete row duplicate scoreType
                        var scopeTypeScopeOld = _parameters.VirtualTestCustomMetaDataService.Select()
                                .FirstOrDefault(x => x.VirtualTestCustomScoreID == model.To && x.ScoreType == model.ScoreTypeName && x.VirtualTestCustomSubScoreID == null);
                        //delete row
                        if (scopeTypeScopeOld != null)
                        {
                            if (scoreTypesCompare.Contains(model.ScoreTypeName))
                            {
                                var scoreInfor = _parameters.VirtualTestCustomScoreService.GetCustomScoreByID(scopeTypeScopeOld.VirtualTestCustomScoreID);
                                if (scoreInfor != null)
                                {
                                    var oldName = scoreInfor.GetType().GetProperty(string.Format("{0}Label", model.ScoreTypeName.Replace("_", ""))).GetValue(scoreInfor, null).ToString();
                                    if (!string.IsNullOrEmpty(oldName) && oldName == model.Name)
                                    {
                                        _parameters.DataLockerTemplateService.DeleteScoreType(CurrentUser.Id, model.ScoreId, model.Name, null);
                                    }
                                }
                            }
                            else
                            {
                                if (model.ScoreTypeName == VirtualTestCustomMetaData.NOTE_COMMENT)
                                {
                                    var notInfor = _parameters.DataLockerTemplateService.GetNoteScoreOfTemplate(scopeTypeScopeOld.VirtualTestCustomScoreID);
                                    if (notInfor != null)
                                    {
                                        var note = notInfor.ListNoteComment.FirstOrDefault(x => x.NoteName == model.Name);
                                        if (note != null)
                                        {
                                            _parameters.DataLockerTemplateService.DeleteScoreTypeNote(CurrentUser.Id, model.ScoreId, model.Name, null);
                                        }
                                    }
                                }
                                else
                                {
                                    _parameters.DataLockerTemplateService.DeleteScoreType(CurrentUser.Id, model.ScoreId, model.Name, null);
                                }
                            }
                        }
                        scoreAdd.SubscoreId = null;
                        _parameters.DataLockerTemplateService.CreateNewScoreType(CurrentUser.Id, scoreAdd);
                        result = UpdateVirtualTestCustomMetaDataOrder(new OrderScoreTypeUpdateModel
                        {
                            ScoreId = model.ScoreId,
                            SubScoreId = scoreAdd.SubscoreId,
                            ScoreType = scoreAdd.ScoreType,
                            Name = scoreAdd.ScoreName,
                            RawIndex = model.RawIndex
                        });
                    }
                }
                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PreviewTemplate(int templateId)
        {
            var model = new EntryResultModel(); // (EntryResultModel)Session["EntryResultModel"];

            try
            {
                var virtualTestCustomScore = _parameters.DataLockerService.GetVirtualTestCustomScoreByVirtualTestCustomScoreID(templateId);
                model.TemplateId = templateId;
                model.TemplateName = virtualTestCustomScore.Name;
                model.DateFormatModel = _parameters.DistrictDecodeService.GetDateFormat(CurrentUser.DistrictId ?? 0);
                return View(model);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = false, ErrorMessage = ex.Message });
            }
        }

        public ActionResult GetInfoPreviewTemplate(int templateId)
        {
            var model = new ResultEntryInputScoreModel();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var virtualTestCustomScore = _parameters.DataLockerService.GetVirtualTestCustomScoreByVirtualTestCustomScoreID(templateId);
            var virtualTestCustomMetaDatas = _parameters.DataLockerService.GetVirtualTestCustomMetaDataByVirtualTestCustomScoreId(virtualTestCustomScore.VirtualTestCustomScoreId);
            if (virtualTestCustomScore != null)
            {
                model.CustomScore = new ResultEntryCustomScore
                {
                    Name = virtualTestCustomScore.Name,
                    VirtualTestCustomScoreId = virtualTestCustomScore.VirtualTestCustomScoreId,
                    ScoreInfos = _parameters.DataLockerService.GetVirtualTestCustomScoreInfo(virtualTestCustomScore, virtualTestCustomMetaDatas, CurrentUser.DistrictId ?? 0)
                };
            }

            var virtualTestCustomSubScores = _parameters.DataLockerService.GetVirtualTestCustomSubScores(virtualTestCustomScore.VirtualTestCustomScoreId)
                .OrderBy(o => o.Sequence);
            model.CustomSubScores = new List<ResultEntryCustomSubScore>();
            if (virtualTestCustomSubScores != null)
            {
                foreach (var virtualTestCustomSubScore in virtualTestCustomSubScores)
                {
                    var virtualTestCustomSubMetaDatas = _parameters.DataLockerService.GetVirtualTestCustomMetaDataByVirtualTestCustomSubScoreID(virtualTestCustomSubScore.VirtualTestCustomSubScoreId);
                    model.CustomSubScores.Add(new ResultEntryCustomSubScore
                    {
                        Name = virtualTestCustomSubScore.Name,
                        VirtualTestCustomSubScoreId = virtualTestCustomSubScore.VirtualTestCustomSubScoreId,
                        ScoreInfos = _parameters.DataLockerService.GetVirtualTestCustomScoreInfo(virtualTestCustomSubScore, virtualTestCustomSubMetaDatas, CurrentUser.DistrictId ?? 0)
                    });
                }
            }
            var studentTestResultScores = new List<DTLStudentAndTestResultScore> {
                new DTLStudentAndTestResultScore
                {
                    FirstName = "Preview",
                    MiddleName = "",
                    LastName = "Student",
                    VirtualTestID = templateId,
                    StudentID = 0
                }
            };
            var studentTestResultSubScores = new List<DTLStudentAndTestResultSubScore> {
                new DTLStudentAndTestResultSubScore
                {
                    FirstName = "Preview",
                    MiddleName = "",
                    LastName = "Student",
                    VirtualTestID = templateId,
                    StudentID = 0
                }
            };
            model.StudentTestResultScores = serializer.Serialize(studentTestResultScores);
            model.StudentTestResultSubScores = serializer.Serialize(studentTestResultSubScores);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GeneratePdf(ResultEntryPrintModel printModel)
        {
            var templateTest = _parameters.DataLockerService.GetVirtualTestCustomScoreByVirtualTestCustomScoreID(printModel.TemplateId);
            if (templateTest == null)
            {
                return Json(new { Success = false, ErrorMessage = "There is no virtual test!" });
            }

            var model = BuildResultEntryPrintingModel(printModel);
            model.TestTitle = templateTest.Name;
            model.RubricDescription = templateTest.Name.ReplaceWeirdCharacters();

            var mapPath = HttpContext.Server.MapPath("~/");
            model.Css = new List<string>();
            model.Css.Add(System.IO.File.ReadAllText(string.Format("{0}/{1}", mapPath, "Content/themes/Print/DataLockerEntryResult/DataLockerEntryResult.css")));
            var html = this.RenderRazorViewToString("EntryResultPDFTemplate", model);

            var pdfGeneratorModel = new PdfGeneratorModel()
            {
                Html = html,
                FileName = model.TestTitle + Guid.NewGuid().ToString().Substring(0, 8),
                Folder = "TestUtilitiesDefineTemplates"
            };
            var pdfData = InvokePdfGeneratorService(pdfGeneratorModel);

            return Json(pdfData, JsonRequestBehavior.AllowGet);
        }
        private string InvokePdfGeneratorService(PdfGeneratorModel model)
        {
            var pdfUrl = PdfGeneratorConsumer.InvokePdfGeneratorService(model.Html, model.FileName, model.Folder, CurrentUser.UserName);

            if (string.IsNullOrWhiteSpace(pdfUrl)) return string.Empty;

            var downloadPdfData = new DownloadPdfData { FilePath = pdfUrl, UserID = CurrentUser.Id, CreatedDate = DateTime.UtcNow };
            _parameters.DownloadPdfService.SaveDownloadPdfData(downloadPdfData);
            var downLoadUrl = Url.Action("Index", "DownloadPdf", new { pdfID = downloadPdfData.DownloadPdfID }, HelperExtensions.GetHTTPProtocal(Request));

            return downLoadUrl;
        }
        private ResultEntryDataPrintModel BuildResultEntryPrintingModel(ResultEntryPrintModel printModel)
        {
            var model = new ResultEntryDataPrintModel();
            var virtualTestCustomScore = _parameters.DataLockerService.GetVirtualTestCustomScoreByVirtualTestCustomScoreID(printModel.TemplateId);
            var virtualTestCustomMetaDatas = _parameters.DataLockerService.GetVirtualTestCustomMetaDataByVirtualTestCustomScoreId(virtualTestCustomScore.VirtualTestCustomScoreId);
            if (virtualTestCustomScore != null)
            {
                model.CustomScore = new ResultEntryCustomScore
                {
                    Name = virtualTestCustomScore.Name,
                    VirtualTestCustomScoreId = virtualTestCustomScore.VirtualTestCustomScoreId,
                    ScoreInfos = _parameters.DataLockerService.GetVirtualTestCustomScoreInfo(virtualTestCustomScore, virtualTestCustomMetaDatas, CurrentUser.DistrictId ?? 0)
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
                    ScoreInfos = _parameters.DataLockerService.GetVirtualTestCustomScoreInfo(virtualTestCustomSubScore, virtualTestCustomSubMetaDatas, CurrentUser.DistrictId ?? 0)
                });
            }

            if (!string.IsNullOrEmpty(printModel.StudentTestResultScores) || !string.IsNullOrEmpty(printModel.StudentTestResultSubscores))
            {
                var dataOverrallScores = StringExtensions.DeserializeObject<List<DTLStudentAndTestResultScore>>(printModel.StudentTestResultScores).Where(x => x != null).ToList();
                var fullName = dataOverrallScores.Select(x => x.FullName).FirstOrDefault();
                if (dataOverrallScores.Any() && !string.IsNullOrEmpty(fullName))
                {
                    var nameValue = fullName.Split(',');
                    dataOverrallScores.ForEach(c =>
                    {
                        c.LastName = nameValue[0];
                        c.FirstName = nameValue[1];
                    });
                }
                model.StudentTestResultScores = dataOverrallScores;

                var dataSubScores = StringExtensions.DeserializeObject<List<DTLStudentAndTestResultSubScore>>(printModel.StudentTestResultSubscores).Where(x => x != null).ToList();
                fullName = dataSubScores.Select(x => x.FullName).FirstOrDefault();
                if (dataSubScores.Any() && !string.IsNullOrEmpty(fullName))
                {
                    var nameValue = fullName.Split(',');
                    dataSubScores.ForEach(c => { c.LastName = nameValue[0]; c.FirstName = nameValue[1]; });
                }
                model.StudentTestResultSubScores = dataSubScores;
            }
            else
            {
                model.StudentTestResultScores = _parameters.DataLockerService.GetStudentAndTestResultScore(printModel.VirtualtestId, printModel.ClassId, printModel.StudentsIdSelectedString)
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
                model.StudentTestResultSubScores = _parameters.DataLockerService.GetStudentAndTestResultSubScore(printModel.VirtualtestId, printModel.ClassId, printModel.StudentsIdSelectedString);
            }
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
        public string JSPath(string fileName)
        {
            var path = HttpContext.Server.MapPath("~/Scripts/");
            var result = Path.Combine(path, fileName);
            return result;
        }
        private bool HasImDataPointAssigned(int templateId, string scoreName = null, string subscoreName = null)
        {
            var endpoint = Constanst.AdminReporting.Endpoints.DATALOCKER_HAS_ASSIGNED_DATA_POINT;
            var url = $"{endpoint}?templateId={templateId}";

            if (!string.IsNullOrEmpty(scoreName))
            {
                url += $"&scoreName={scoreName.UrlEncode()}";
            }

            if (!string.IsNullOrEmpty(subscoreName))
            {
                url += $"&subscoreName={subscoreName.UrlEncode()}";
            }

            return false; // _parameters.ReportingHttpClient.Get<bool>(url);
        }

        public ActionResult ShowAddTagPopup(int virtualTestCustomSubScoreId)
        {
            var model = new ShowAddTagPopupModel
            {
                VirtualTestCustomSubScoreId = virtualTestCustomSubScoreId,
                IsPublisher = CurrentUser.IsPublisher,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                DistrictID = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin ? 0 : CurrentUser.DistrictId.Value
            };

            var virtualTestCustomSubScore = _parameters.VirtualTestCustomSubScoreService.GetById(virtualTestCustomSubScoreId);
            if (virtualTestCustomSubScore.ItemTagID.HasValue)
            {
                var itemTag = _parameters.ItemTagService.GetItemTagInfo(virtualTestCustomSubScore.ItemTagID.Value);
                if (itemTag != null)
                {
                    model.ItemTagID = itemTag.ItemTagID;
                    model.ItemTagCategoryID = itemTag.ItemTagCategoryID;
                    model.DistrictID = itemTag.DistrictID;
                    var district = _parameters.DistrictService.GetDistrictById(itemTag.DistrictID);
                    model.StateID = district?.StateId ?? 0;
                }
            }
            
            return PartialView("_AddTag", model);
        }

        [HttpPost]
        public ActionResult AddTagForSubScore(int virtualTestCustomSubScoreId, int itemTagId)
        {
            _parameters.VirtualTestCustomSubScoreService.SaveItemTag(virtualTestCustomSubScoreId, itemTagId);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowUploadConversionTablePopup(int virtualTestCustomScoreId, int? virtualTestCustomSubScoreId)
        {
            ViewBag.VirtualTestCustomScoreId = virtualTestCustomScoreId;
            ViewBag.VirtualTestCustomSubScoreId = virtualTestCustomSubScoreId ?? 0;
            var items = _parameters.VirtualTestCustomMetaDataService.Select().Where(x => x.VirtualTestCustomScoreID == virtualTestCustomScoreId).ToList();
            ViewBag.HasColumns = items.Any(x => x.VirtualTestCustomSubScoreID == virtualTestCustomSubScoreId);

            return PartialView("_UploadConversionTablePopup");
        }

        [HttpPost]
        public ActionResult UploadConversionTable(int virtualTestCustomScoreId, int virtualTestCustomSubScoreId, bool isReplace)
        {
            var errorMessage = string.Empty;
            var template = new TemplateConversionSetDto();

            try
            {
                if (Request.Files.Count > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(Request.Files[0].FileName);
                    var fileStream = Request.Files[0].InputStream;

                    var excel = GetConversionTableExcelItems(Request.Files[0].InputStream);

                    var errorColumnIndex = ValidateConversionTableExcel(excel.Items);
                    if (errorColumnIndex.HasValue)
                    {
                        var type = errorColumnIndex == 0 ? "integer" : "numeric";
                        errorMessage = $"The column '{excel.ExcelHeaders[errorColumnIndex.Value]}' in the uploaded file has an incorrect format. It must contain {type} values.";
                    }
                    else if (virtualTestCustomSubScoreId > 0)
                        template = _parameters.VirtualTestCustomSubScoreService.ImportConversionTable(virtualTestCustomSubScoreId, fileName, excel.Headers, excel.Items, isReplace);
                    else
                        template = _parameters.VirtualTestCustomScoreService.ImportConversionTable(virtualTestCustomScoreId, fileName, excel.Headers, excel.Items, isReplace);
                }
            }
            catch (Exception ex)
            {
            }

            return Json(new { template, errorMessage });
        }

        private bool HasCustomScoreType(int virtualTestCustomScoreID, int virtualTestCustomSubScoreID = 0)
        {
            var scoreTypes = virtualTestCustomSubScoreID > 0
                ? _parameters.DataLockerTemplateService.GetScoreTypesOfSubscore(virtualTestCustomSubScoreID)
                : _parameters.DataLockerTemplateService.GetScoreTypesOfTemplate(virtualTestCustomScoreID);

            return scoreTypes.Any(x => x.IsCustomScoreType);
        }

        private bool HasConversionSet(int virtualTestCustomScoreID, int virtualTestCustomSubScoreID = 0)
        {
            return virtualTestCustomSubScoreID > 0
                ? _parameters.VirtualTestCustomSubScoreService.HasConversionSet(virtualTestCustomSubScoreID)
                : _parameters.VirtualTestCustomScoreService.HasConversionSet(virtualTestCustomScoreID);
        }

        private int? ValidateConversionTableExcel(IEnumerable<ConversionTableExcelItem> excelItems)
        {
            foreach (var item in excelItems)
            {
                if (!string.IsNullOrEmpty(item.RawScore) && !int.TryParse(item.RawScore, out var score))
                    return 0;

                if (!string.IsNullOrEmpty(item.CustomNumeric1) && !decimal.TryParse(item.CustomNumeric1, out var score1))
                    return 1;

                if (!string.IsNullOrEmpty(item.CustomNumeric2) && !decimal.TryParse(item.CustomNumeric2, out var score2))
                    return 3;

                if (!string.IsNullOrEmpty(item.CustomNumeric3) && !decimal.TryParse(item.CustomNumeric3, out var score3))
                    return 5;

                if (!string.IsNullOrEmpty(item.CustomNumeric4) && !decimal.TryParse(item.CustomNumeric4, out var score4))
                    return 7;
            }

            return null;
        }

        public static (List<string> Headers, List<string> ExcelHeaders, List<ConversionTableExcelItem> Items) GetConversionTableExcelItems(Stream stream)
        {
            var headers = new List<string>();
            var excelHeaders = new List<string>();
            var items = new List<ConversionTableExcelItem>();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                var rows = worksheet.Dimension.Rows;

                for (int col = 1; col <= 9; col++)
                {
                    var cell = worksheet.Cells[1, col];
                    headers.Add(col == 1 ? "Raw" : cell.Text);

                    var excelHeader = !string.IsNullOrEmpty(cell.Text) ? cell.Text : $"{cell.Address.First()}";
                    excelHeaders.Add(excelHeader);
                }

                for (int row = 2; row <= rows; row++)
                {
                    var rowScore = worksheet.Cells[row, 1].Text;
                    if (string.IsNullOrEmpty(rowScore)) continue;

                    items.Add(new ConversionTableExcelItem
                    {
                        RawScore = worksheet.Cells[row, 1].Text,
                        CustomNumeric1 = worksheet.Cells[row, 2].Text,
                        CustomText1 = worksheet.Cells[row, 3].Text,
                        CustomNumeric2 = worksheet.Cells[row, 4].Text,
                        CustomText2 = worksheet.Cells[row, 5].Text,
                        CustomNumeric3 = worksheet.Cells[row, 6].Text,
                        CustomText3 = worksheet.Cells[row, 7].Text,
                        CustomNumeric4 = worksheet.Cells[row, 8].Text,
                        CustomText4 = worksheet.Cells[row, 9].Text
                    });
                }
            }

            return (headers, excelHeaders, items);
        }

    }
}
