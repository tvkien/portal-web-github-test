using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.TestMaker;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.TestMaker;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.QTIItemPreviewRequest;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TestMaker;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]

    public class TestMakerController : BaseController
    {
        public readonly QTIItemConvert _qTIItemConvert;
        private readonly TestMakerParameters parameters;
        private readonly IS3Service s3Service;
        public TestMakerController(TestMakerParameters parameters, QTIItemConvert qTIItemConvert, IS3Service s3Service)
        {
            this.parameters = parameters;
            this._qTIItemConvert = qTIItemConvert;
            this.s3Service = s3Service;
        }

        [HttpGet]
        [UrlReturnDecode]
        public ActionResult Index(int qtiItemGroupId, string qtiSchemaId, int? virtualTestId, int districtId = 0)
        {
            //User has right to edit this qtiItem if he/she has right to edit itemset
            if (qtiItemGroupId > 0)
            {
                var hasPermission = parameters.QtiGroupService.HasRightToEditQtiGroup(qtiItemGroupId, CurrentUser);
                if (!hasPermission)
                {
                    return RedirectToAction("Index", "ItemBank");
                }
            }
            if (qtiItemGroupId == 0 && !virtualTestId.HasValue)
            {
                return RedirectToAction("Index", "ItemBank");
            }
            if (qtiItemGroupId == 0 && virtualTestId.HasValue && virtualTestId.Value == 0)
            {
                return RedirectToAction("Index", "ItemBank");
            }
            virtualTestId = virtualTestId ?? 0;
            ViewBag.DistrictId = CurrentUser.IsCorrectDistrict(districtId) ? districtId : CurrentUser.DistrictId ?? 0;
            ViewBag.VirtualTestId = virtualTestId ?? 0;
            ViewBag.HasMoreThanOneSection = false;
            ViewBag.MediaModel = new MediaModel
            {
                ID = qtiItemGroupId,
                UseS3Content = true
                //parameters.DistrictDecodeService.UseS3Content(CurrentUser.DistrictId.GetValueOrDefault()) //alwasy use s3 now
            };
            if (virtualTestId.HasValue && virtualTestId.Value > 0)
            {
                //check if user has right to edit virtual test
                var virtualTest = parameters.VirtualTestService.GetTestById(virtualTestId.Value);
                var isSurvey = virtualTest != null && virtualTest.DatasetOriginID.Value == (int)DataSetOriginEnum.Survey;
                if (!parameters.VulnerabilityService.HasRighToEditVirtualTest(CurrentUser, virtualTestId.Value, CurrentUser.GetMemberListDistrictId()))
                {
                    if (!isSurvey)
                    {
                        return RedirectToAction("Index", "ManageTest");
                    }
                    return RedirectToAction("Index", "ManageSurvey");
                }
                var qtiBank = parameters.QtiBankService.GetDefaultQTIBank(CurrentUser.UserName, CurrentUser.Id);
                var itemSet = parameters.QtiGroupService.GetDefaultQTIGroup(CurrentUser.Id, qtiBank.QtiBankId, virtualTest.Name, isSurvey);
                qtiItemGroupId = itemSet.QtiGroupId;
                var sections = parameters.VirtualSectionService.GetVirtualSectionByVirtualTest(virtualTestId.Value);
                if (sections != null && sections.Count > 1)
                {
                    ViewBag.HasMoreThanOneSection = sections.Count > 1;
                }
                var countVirtualQuestion = parameters.VirtualQuestionService.Select().Where(x => x.VirtualTestID == virtualTestId).Select(x => x.VirtualQuestionID).Count();
                if (countVirtualQuestion > 0)
                {
                    //get the max VirtualQuestionOrder
                    var maxQuestionOrder = parameters.VirtualQuestionService.Select()
                        .Where(x => x.VirtualTestID == virtualTestId.GetValueOrDefault())
                        .Select(x => x.QuestionOrder).Max();
                    if (countVirtualQuestion != maxQuestionOrder)
                    {
                        parameters.VirtualTestService.ReassignVirtualQuestionOrder(virtualTestId.GetValueOrDefault());
                    }
                }
                ViewBag.CountVirtualQuestions = countVirtualQuestion + 1;
                ViewBag.VirtualQuestionOrder = countVirtualQuestion + 1;
                ViewBag.IsSurvey = isSurvey ? 1 : 0;
            }
            else
                ViewBag.IsSurvey = 0;

            string qtiGroupName = string.Empty;
            var qtiGroup = parameters.QtiGroupService.GetById(qtiItemGroupId);
            if (qtiGroup == null)
            {
                return RedirectToAction("Index", "ItemBank");
            }
            qtiGroupName = qtiGroup.Name;

            ViewBag.QtiItemGroupId = qtiItemGroupId;
            ViewBag.QtiSchemaId = qtiSchemaId;
            ViewBag.QtiGroupName = qtiGroupName;
            ViewBag.VirtualTestId = virtualTestId;
            //Get the number of qti items in the item set
            if (virtualTestId <= 0)
            {
                var countQtiItems = parameters.QtiItemService.GetAllQtiItem()
               .Where(x => x.QTIGroupID == qtiItemGroupId).Select(x => x.QTIItemID).Count();
                ViewBag.CountQtiItems = countQtiItems + 1;
                ViewBag.QuestionOrder = countQtiItems + 1;
            }

            ViewBag.WarningTimeoutMinues = parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue("TestMakerWarningTimeOutMinute", 5);
            ViewBag.DefaultCookieTimeOutMinutes = parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue("DefaultCookieTimeOutMinutes", 30);
            ViewBag.KeepAliveDistanceSecond = parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue("KeepAliveDistanceSecond", 15);
            ViewBag.BasicSciencePaletteSymbol = parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue("BasicSciencePaletteSymbol", string.Empty);

            ViewBag.HasQuestionGroups = parameters.QuestionGroupServices.HasQuestionGroup(virtualTestId.GetValueOrDefault());
            if (ConvertValue.ToInt(qtiSchemaId) == (int)QtiSchemaEnum.MultiPart || virtualTestId < 1)
            {
                ViewBag.IsAllowRubricGradingMode = 0;
            }
            else
            {
                ViewBag.IsAllowRubricGradingMode = 1;
            }

            var assessmentArtifactFileTypeGroups = Mapper.Map<IEnumerable<AssessmentArtifactFileTypeGroupViewModel>>
            (parameters.DistrictDecodeService
                .GetAssessmentArtifactFileTypeGroups(CurrentUser.DistrictId.GetValueOrDefault()));

            var recordingOptions = Util.GetAssessmentArtifactRecordingOptions();

            ViewBag.AssessmentArtifactConfiguration =  new AssessmentArtifactConfigurationViewModel(assessmentArtifactFileTypeGroups, recordingOptions)
                .SerializeObject(isCamelCase: true);

            return View();
        }

        public ActionResult SelectQuestionType(int qtiItemGroupId, int? virtualTestId)
        {
            ViewBag.QtiItemGroupId = qtiItemGroupId;
            ViewBag.VirtualTestId = virtualTestId ?? 0;
            if (virtualTestId.HasValue && virtualTestId.Value > 0)
            {
                var qtiBank = parameters.QtiBankService.GetDefaultQTIBank(CurrentUser.UserName, CurrentUser.Id);
                var virtualTest = parameters.VirtualTestService.GetTestById(virtualTestId.Value);
                var itemSet = parameters.QtiGroupService.GetDefaultQTIGroup(CurrentUser.Id, qtiBank.QtiBankId, virtualTest.Name);
                ViewBag.QtiItemGroupId = itemSet.QtiGroupId;
            }
            return PartialView("_SelectQuestionType");
        }

        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult CreateItem(CreateItemViewModel model)
        {
            //avoid someone modify ajax parameter
            //Check if current user has right to update this item set
            var itemSet = parameters.QtiGroupService.GetById(model.QTIGroupId);
            if (itemSet == null)
            {
                return
                    Json(new { ErrorList = new[] { new { ErrorMessage = "Item set does not exist." } }, success = false },
                        JsonRequestBehavior.AllowGet);
            }
            if (
                !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, itemSet))
            {
                return
                    Json(
                        new
                        {
                            ErrorList = new[] { new { ErrorMessage = "Has no right to work on this item set." } },
                            success = false
                        },
                        JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                model.Title = string.Empty;
            }
            // Remove unhandle control characters
            var xmlContent = model.XMLContent.CleanUpXmlContentInput();
            var noshufflePassageList = new List<PassageViewModel>();
            var virtualTestId = model.VirtualTestId ?? 0;
            var selectedVirtualSectionId = model.SelectedVirtualSectionId ?? 0;
            var isSurvey = false;
            if (virtualTestId > 0)
            {
                //get list passage noshuffle
                var xmlcontentUpdated = string.Empty;
                noshufflePassageList = Util.GetPassageNoshuffleList(xmlContent, out xmlcontentUpdated);
                xmlContent = xmlcontentUpdated;
                //Get the virtual test
                var virtualTest = parameters.VirtualTestService.GetTestById(virtualTestId);
                if (virtualTest == null)
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = "Can not find the test." } }, success = false },
                        JsonRequestBehavior.AllowGet);
                }
                //check to avoid modifying ajax parameter bankId)
                var hasPermission = parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
                if (!hasPermission)
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = "Has no right to update this test." } }, success = false },
                       JsonRequestBehavior.AllowGet);
                }

                //[LNKT-64906] prevent add new question when exists test inprocess.
                if (parameters.QTIOnlineTestSessionService.HasExistTestInProgress(virtualTest.VirtualTestID))
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = TextConstants.EXIST_TEST_IN_PROGRESS } }, success = false },
                       JsonRequestBehavior.AllowGet);
                }

                isSurvey = virtualTest.DatasetOriginID == (int)DataSetOriginEnum.Survey;
            }
            //TODO: [LNKT-30407]
            if (model?.RubricQuestionCategories?.Count > 0)
            {
                var pointsPossible = model?.RubricQuestionCategories.Sum(x => x?.RubricCategoryTiers.Max(t => t?.Point ?? 0) ?? 0);
                model.PointsPossible = pointsPossible;
            }
            string error = string.Empty;
            var qTIItemID = parameters.QtiItemService.CreateQTIItem(CurrentUser.Id, model.QTIGroupId, xmlContent, model.NoDuplicateAnswers, out error, null, null, isSurvey, title: model.Title, description: model.Description);
            var qtiSchemaId = parameters.QTIItemConvert.ConvertFromXmlContent(xmlContent).QTISchemaID;

            if (string.IsNullOrWhiteSpace(error) && qTIItemID > 0)
            {
                UpdateItemPassage(qTIItemID);
                if (virtualTestId <= 0)
                {
                    //save expresstion for qtiitem
                    parameters.AlgorithmicScoreService.AlgorithmicSaveExpression(qTIItemID, 0, parameters.QtiItemService.BuildExpressionXml(model.ListExpression), CurrentUser.Id);
                    if (qtiSchemaId == (int)QtiSchemaEnum.MultiPart)
                    {
                        parameters.MultiPartExpressionService.SaveExpression(qTIItemID, 0, parameters.QtiItemService.BuildMultiPartExpressionXml(model.ListMultiPartExpression), CurrentUser.Id);
                    }
                    return Json(new { success = true, qtiItemId = qTIItemID, virtualQuestionId = 0 }, JsonRequestBehavior.AllowGet);
                }
                int virtualSectionId = 0;
                var sections = parameters.VirtualSectionService.GetVirtualSectionByVirtualTest(virtualTestId);
                if (sections == null)
                {
                    sections = new List<VirtualSection>();
                }
                if (sections.Count > 1)
                {
                    //There's more than one section -> add to the selected section
                    if (virtualTestId > 0 && selectedVirtualSectionId >= 0)
                    {
                        virtualSectionId = selectedVirtualSectionId;
                    }
                }
                else
                {
                    if (sections.Count == 1)
                    {
                        //The only one virtual section
                        virtualSectionId = sections[0].VirtualSectionId;
                    }
                    else if (sections.Count == 0)//There's no section -> add to default section ( virtualsectionId = 0 )
                    {
                        virtualSectionId = 0;
                    }
                }

                //Add this new item to virtual test
                string errorMessage = string.Empty;
                var result = Util.AddQtiItemsToVirtualSection(virtualTestId, qTIItemID.ToString(),
                                                              virtualSectionId, false,
                                                              CurrentUser.UserName,
                                                              CurrentUser.Id, CurrentUser.StateId ?? 0,
                                                              parameters.VirtualTestService, null, null,
                                                              null, out errorMessage, model.QuestionGroupId, s3Service);
                parameters.VirtualTestService.ReassignVirtualQuestionOrder(virtualTestId);

                if (result)
                {
                    var virtualQuestionID = 0;
                    var virtualQuestion =
                           parameters.VirtualQuestionService.Select()
                               .FirstOrDefault(x => x.VirtualTestID == virtualTestId && x.QTIItemID == qTIItemID);
                    if (virtualQuestion != null)
                    {
                        virtualQuestionID = virtualQuestion.VirtualQuestionID;
                        //insert passage noshuffle
                        foreach (var passage in noshufflePassageList)
                        {
                            var item = new VirtualQuestionPassageNoShuffle()
                            {
                                VirtualQuestionID = virtualQuestionID,
                                QTIRefObjectID = passage.QtiRefObjectID,
                                QTI3pPassageID = passage.Qti3pPassageID,
                                DataFileUploadPassageID = passage.DataFileUploadPassageID,
                                PassageURL = passage.Data
                            };

                            parameters.VirtualQuestionPassageNoShuffleService.Save(item);
                        }

                        //save expresstion for qtiitem
                        parameters.AlgorithmicScoreService.AlgorithmicSaveExpression(qTIItemID, virtualQuestionID, parameters.QtiItemService.BuildExpressionXml(model.ListExpression), CurrentUser.Id);
                        if (qtiSchemaId == (int)QtiSchemaEnum.MultiPart)
                        {
                            parameters.MultiPartExpressionService.SaveExpression(qTIItemID, virtualQuestionID, parameters.QtiItemService.BuildMultiPartExpressionXml(model.ListMultiPartExpression), CurrentUser.Id);
                        }
                        if (qtiSchemaId == (int)QtiSchemaEnum.ExtendedText && model?.RubricQuestionCategories?.Count > 0)
                        {
                            foreach (var rubricQuestionCategory in model.RubricQuestionCategories)
                            {
                                rubricQuestionCategory.VirtualQuestionID = virtualQuestionID;
                            }
                            parameters.RubricModuleCommandService.AddRubricCategories(model.RubricQuestionCategories, CurrentUser.Id);

                            parameters.RubricModuleCommandService.UpdateVirtualQuestionToRubricBase(virtualQuestion, true, model.PointsPossible);
                        }
                    }
                    return Json(new { success = true, qtiItemId = qTIItemID, virtualQuestionId = virtualQuestionID }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = errorMessage } }, success = false }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { ErrorList = new[] { new { ErrorMessage = error } }, success = false }, JsonRequestBehavior.AllowGet);
        }

        private void UpdateItemQuestionBasedRubric(VirtualQuestionData virtualQuestion, UpdateItemViewModel model)
        {
            foreach (var rubricQuestionCategory in model.RubricQuestionCategories)
            {
                rubricQuestionCategory.VirtualQuestionID = model.VirtualquestionId.Value;
            }
            var inserted = parameters.RubricModuleCommandService.SaveRubricCategories(model.RubricQuestionCategories, CurrentUser.Id);

            parameters.RubricModuleCommandService.UpdateVirtualQuestionToRubricBase(virtualQuestion, true, model.PointsPossible);

            var findTagOfRubrics = parameters.RubricModuleQueryService.GetAllTagsOfRubricByVirtualQuestion(model.VirtualquestionId.Value);
            if (findTagOfRubrics?.Count() == 0)
            {
                var findAllTagsExistsed = parameters.RubricModuleQueryService.GetAllTagsByVirtualQuestion(model.VirtualquestionId.Value.ToString());
                if (findAllTagsExistsed?.Count() > 0)
                {
                    List<RubricCategoryTagDto> rubricCategoryTagDtos = new List<RubricCategoryTagDto>();
                    foreach (var rubricQuestionCategory in inserted)
                    {
                        foreach (var tag in findAllTagsExistsed)
                        {
                            rubricCategoryTagDtos.Add(new RubricCategoryTagDto
                            {
                                VirtualQuestionID = model.VirtualquestionId.Value,
                                RubricQuestionCategoryID = rubricQuestionCategory.RubricQuestionCategoryID,
                                TagCategoryID = tag.TagCategoryID,
                                TagID = tag.TagID,
                                TagName = tag.TagName,
                                TagDescription = tag.TagDescription,
                                TagType = tag.TagType,
                                TagCategoryName = tag.TagCategoryName
                            });
                        }
                    }

                    parameters.RubricModuleCommandService.AssignCategoryTagByQuestionIds(model.VirtualquestionId.Value, rubricCategoryTagDtos);
                }
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult UpdateItem(UpdateItemViewModel model)
        {
            //avoid someone modify ajax parameter
            //Check if current user has right to update this item
            if(string.IsNullOrWhiteSpace(model.Title))
            {
                model.Title = string.Empty;
            }
            var item = parameters.QtiItemService.GetQtiItemById(model.QtiItemId);
            if (item == null)
            {
                return
                    Json(new { ErrorList = new[] { new { ErrorMessage = "Item does not exist." } }, success = false },
                        JsonRequestBehavior.AllowGet);
            }
            var itemSet = parameters.QtiGroupService.GetById(item.QTIGroupID);
            if (!parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, itemSet))
            {
                return
                    Json(
                        new
                        {
                            ErrorList = new[] { new { ErrorMessage = "Has no right to update on this item." } },
                            success = false
                        },
                        JsonRequestBehavior.AllowGet);
            }
            int resetRubric = 0;
            // Remove unhandle control characters
            var xmlContent = model.XMLContent.CleanUpXmlContentInput();
            var isSurvey = false;
            if (model.VirtualquestionId.HasValue && model.VirtualquestionId.Value > 0) //fromvirtualquestionproperty
            {
                //update passge noshuffle
                var xmlcontentUpdated = string.Empty;
                var noshufflePassageList = Util.GetPassageNoshuffleList(xmlContent, out xmlcontentUpdated);
                xmlContent = xmlcontentUpdated;
                parameters.VirtualQuestionPassageNoShuffleService.DeleteAllPassageNoshuffle(model.VirtualquestionId.Value);
                foreach (var passage in noshufflePassageList)
                {
                    var noshuffleItem = new VirtualQuestionPassageNoShuffle()
                    {
                        VirtualQuestionID = model.VirtualquestionId.Value,
                        QTIRefObjectID = passage.QtiRefObjectID,
                        QTI3pPassageID = passage.Qti3pPassageID,
                        DataFileUploadPassageID = passage.DataFileUploadPassageID,
                        PassageURL = passage.Data
                    };

                    parameters.VirtualQuestionPassageNoShuffleService.Save(noshuffleItem);
                }

                var virtualQuestionExists = parameters.VirtualQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionID == model.VirtualquestionId);
                if (item.QTISchemaID == (int)QTISchemaEnum.ExtendedText && virtualQuestionExists != null)
                {
                    if (model.RubricQuestionCategories?.Count > 0)
                    {
                        resetRubric = 0;
                        var pointsPossible = model.RubricQuestionCategories.Sum(x => x?.RubricCategoryTiers.Max(t => t?.Point ?? 0) ?? 0);
                        model.PointsPossible = pointsPossible;
                        UpdateItemQuestionBasedRubric(virtualQuestionExists, model);
                    }
                    else
                    {
                        resetRubric = 1;
                        parameters.RubricModuleCommandService.UpdateVirtualQuestionToRubricBase(virtualQuestionExists, null);
                    }
                } else if (virtualQuestionExists != null)
                {
                    parameters.RubricModuleCommandService.UpdateVirtualQuestionToRubricBase(virtualQuestionExists, null);
                }

                if (item.QTISchemaID == (int)QTISchemaEnum.ChoiceMultipleVariable)
                {
                    var virtualQuestion = parameters.VirtualQuestionService.GetQuestionDataById(model.VirtualquestionId.GetValueOrDefault());
                    if (virtualQuestion != null)
                    {
                        var virtualTest = parameters.VirtualTestService.GetTestById(virtualQuestion.VirtualTestID);
                        isSurvey = virtualTest?.DatasetOriginID == (int)DataSetOriginEnum.Survey;
                    }
                }
            }
            else
            {
                if (item.QTISchemaID == (int)QTISchemaEnum.ExtendedText)
                {
                    var virtualQuestionExists = parameters.VirtualQuestionService.Select().FirstOrDefault(x => x.QTIItemID == item.QTIItemID && x.IsRubricBasedQuestion == true);
                    if (virtualQuestionExists != null)
                    {
                        VirtualTestProperty vtd = parameters.VirtualTestService.GetVirtualTestProperty(virtualQuestionExists.VirtualTestID, CurrentUser.RoleId, CurrentUser.DistrictId.GetValueOrDefault());
                        if (vtd != null && vtd.TotalTestResult > 0)
                        {
                            resetRubric = 0;
                        }
                    }
                    var oldXmlContent = item.XmlContent;
                    var oldScoringMethod = QtiItemScoringMethodHelper.GetQtiItemScoringMethod(oldXmlContent, item.QTISchemaID, false);
                    var newScoringMethod = QtiItemScoringMethodHelper.GetQtiItemScoringMethod(xmlContent, item.QTISchemaID, false);
                    if (oldScoringMethod != newScoringMethod)
                    {
                        resetRubric = 1;
                    }
                    var qtiItemTestTaker = _qTIItemConvert.ConvertFromXmlContent(xmlContent);
                    var oldPointPossible = item.PointsPossible;
                    var newPointPossible = qtiItemTestTaker.PointsPossible;
                    if (oldPointPossible != newPointPossible)
                    {
                        resetRubric = 1;
                    }
                }
            }
            string error = string.Empty;
            var qTIItemID = parameters.QtiItemService.UpdateItem(CurrentUser.Id, model.QtiItemId, xmlContent, model.NoDuplicateAnswers, resetRubric: resetRubric, out error, model.Title, model.Description, isSurvey);
            var qtiSchemaId = parameters.QTIItemConvert.ConvertFromXmlContent(xmlContent).QTISchemaID;

            if (model.VirtualTestId > 0)
            {
                var sectionQuestion = parameters.VirtualSectionQuestionService.Select()
                    .Where(x => x.VirtualTestId == model.VirtualTestId && x.VirtualQuestionId == model.VirtualquestionId)
                    .FirstOrDefault();
                if (model.VirtualquestionId.HasValue && model.VirtualSectionId.HasValue && model.VirtualSectionId != sectionQuestion.VirtualSectionId)
                    parameters.VirtualTestService.UpdateVirtualSection(model.VirtualquestionId.Value, model.VirtualSectionId.Value);
            }

            if (string.IsNullOrWhiteSpace(error) && qTIItemID > 0)
            {
                //if (Util.UploadTestItemMediaToS3)//it's now alway upload to S3, no web server more

                Util.UploadMultiVirtualTestJsonFileToS3(qTIItemID, parameters.VirtualQuestionService, parameters.VirtualTestService, s3Service);

                UpdateItemPassage(qTIItemID);

                //save expresstion for qtiitem
                parameters.AlgorithmicScoreService.AlgorithmicSaveExpression(qTIItemID, model.VirtualquestionId ?? 0, parameters.QtiItemService.BuildExpressionXml(model.ListExpression), CurrentUser.Id);
                if (qtiSchemaId == (int)QtiSchemaEnum.MultiPart)
                {
                    parameters.MultiPartExpressionService.SaveExpression(qTIItemID, model.VirtualquestionId ?? 0, parameters.QtiItemService.BuildMultiPartExpressionXml(model.ListMultiPartExpression), CurrentUser.Id);
                }

                parameters.ManageSurveyService.ProcessingUpdateQuestionType(item.QTISchemaID, qtiSchemaId, qTIItemID, model.VirtualquestionId ?? 0);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { ErrorList = new[] { new { ErrorMessage = error } }, success = false }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult GeneratePreviewRequest(QTIItemPreviewRequestViewModel model)
        {
            // Remove unhandle control characters
            model.XmlContent = model.XmlContent.CleanUpXmlContentInput();
            model.XmlContent = model.XmlContent.ConvertFromUnicodeToWindow1252();

            var qtiItemPreviewRequest = new QTIItemPreviewRequest
            {
                CreatedDate = DateTime.UtcNow,
                XmlContent = model.XmlContent,
                VirtualTestId = model.VirtualTestId
            };

            parameters.QTIItemPreviewRequestService.Save(qtiItemPreviewRequest);
            if (model.MultiPartExpressions != null)
            {
                parameters.QTIItemPreviewRequestService.SaveMultiPartExpression(model.MultiPartExpressions, qtiItemPreviewRequest.QTIItemPreviewRequestId);
            }

            var testTakerUrl = BuildTestTakerQTIItemPreviewURL(qtiItemPreviewRequest.QTIItemPreviewRequestId);
            testTakerUrl = string.Format("{0}&districtId={1}", testTakerUrl, CurrentUser.DistrictId.GetValueOrDefault());
            if (parameters.DistrictDecodeService.DistrictSupportTestTakerNewSkin(CurrentUser.DistrictId.GetValueOrDefault()))
            {
                testTakerUrl = string.Format("{0}&{1}", testTakerUrl, ContaintUtil.TESTTAKER_NEWSKIN);
            }
            return Json(new { success = true, testTakerUrl = testTakerUrl }, JsonRequestBehavior.AllowGet);
        }

        private string BuildTestTakerQTIItemPreviewURL(string qtiItemPreviewRequestId)
        {
            if (!string.IsNullOrEmpty(qtiItemPreviewRequestId))
            {
                var previewQTIItemTestCode = parameters.ConfigurationService.GetConfigurationByKey(Util.PreviewQTIItemTestCode);

                if (previewQTIItemTestCode != null)
                {
                    var url = Util.GetConfigByKey("PortalhyperlinkPreviewQuestion", "");
                    var redirectUrl = string.Format(url, previewQTIItemTestCode.Value, qtiItemPreviewRequestId);
                    return redirectUrl;
                }
            }
            return string.Empty;
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AudioUpload(HttpPostedFileBase file, int id = 0)
        {
            var model = new MediaModel
            {
                ID = id,
                PostedFile = file,
                MediaType = MediaType.Audio,
                UseS3Content = true
            };

            var result = MediaHelper.UploadTestMedia(model, s3Service);
            //build new absolute url for preview on popup
            string absoluteUrl = string.Empty;
            if (string.IsNullOrEmpty(model.AUVirtualTestFolder))
            {
                absoluteUrl = string.Format("{0}/{1}", UrlUtil.GenerateS3Subdomain(model.S3Domain, model.UpLoadBucketName).RemoveEndSlash(), result.MediaPath.RemoveStartSlash());
            }
            else
            {
                absoluteUrl = string.Format("{0}/{1}/{2}", UrlUtil.GenerateS3Subdomain(model.S3Domain, model.UpLoadBucketName).RemoveEndSlash(), model.AUVirtualTestFolder.RemoveEndSlash().RemoveStartSlash(), result.MediaPath.RemoveStartSlash());
            }
            var jsonResult = Json(new { success = result.Success, url = absoluteUrl, absoluteUrl = absoluteUrl, errorMessage = result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            var jsonStringResult = new JavaScriptSerializer().Serialize(jsonResult.Data);

            return Content(jsonStringResult, "text/plain");
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult VideoUpload(int id, HttpPostedFileBase file)
        {
            //var useS3Content =
            //       parameters.DistrictDecodeService.UseS3Content(CurrentUser.DistrictId.GetValueOrDefault());
            var model = new MediaModel
            {
                ID = id,
                PostedFile = file,
                MediaType = MediaType.Video,
                UseS3Content = true
            };

            var result = MediaHelper.UploadTestMedia(model, s3Service);
            var mediaPath = Util.ChangeS3BucketNameAsSubdomain(model, result.MediaPath);
            var jsonResult = Json(new { success = result.Success, ReturnValue = mediaPath, errorMessage = result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            var jsonStringResult = new JavaScriptSerializer().Serialize(jsonResult.Data);

            return Content(jsonStringResult, "text/plain");
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ImageUpload(int id, HttpPostedFileBase file)
        {
            //var useS3Content =
            //        parameters.DistrictDecodeService.UseS3Content(CurrentUser.DistrictId.GetValueOrDefault());
            var model = new MediaModel
            {
                ID = id,
                PostedFile = file,
                MediaType = MediaType.Image,
                UseS3Content = true
            };

            var result = MediaHelper.UploadTestMedia(model, s3Service);
            //build new absolute url for preview on popup
            string absoluteUrl = string.Empty;
            if (string.IsNullOrEmpty(model.AUVirtualTestFolder))
            {
                absoluteUrl = string.Format("{0}/{1}", UrlUtil.GenerateS3Subdomain(model.S3Domain, model.UpLoadBucketName).RemoveEndSlash(), result.MediaPath.RemoveStartSlash());
            }
            else
            {
                absoluteUrl = string.Format("{0}/{1}/{2}", UrlUtil.GenerateS3Subdomain(model.S3Domain, model.UpLoadBucketName).RemoveEndSlash(), model.AUVirtualTestFolder.RemoveEndSlash().RemoveEndSlash(), result.MediaPath.RemoveStartSlash());
            }

            var jsonResult = Json(new { success = result.Success, url = absoluteUrl, absoluteUrl = absoluteUrl, errorMessage = result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            var jsonStringResult = new JavaScriptSerializer().Serialize(jsonResult.Data);

            return Content(jsonStringResult, "text/plain");
        }

        public ActionResult GetAudio(string id)
        {
            var testItemMediaPath = string.Empty;
            if (string.IsNullOrWhiteSpace(testItemMediaPath))
            {
                return new FileContentResult(new byte[0], "audio/mpeg");
            }

            var roFilePath = Path.Combine(testItemMediaPath, id.Replace("|", "/"));
            if (!System.IO.File.Exists(roFilePath))
            {
                return new FileContentResult(new byte[0], "audio/mpeg");
            }

            byte[] file = System.IO.File.ReadAllBytes(roFilePath);
            Response.ContentType = "audio/mpeg";
            return File(file, "audio/mpeg");
        }

        public ActionResult GetRefObjectContent(string refObjectId)
        {
            string refObjectContent = "";

            var testItemMediaPath = string.Empty;
            var roFilePath = Path.Combine(testItemMediaPath, "RO\\RO_" + refObjectId + ".xml");
            if (System.IO.File.Exists(roFilePath))
            {
                refObjectContent = System.IO.File.ReadAllText(roFilePath);
            }
            else
            {
                refObjectContent = "error File not Found " + roFilePath;
            }

            ViewBag.RefObjectContent = refObjectContent;

            return PartialView("_RefObjectContent");
        }

        public ActionResult ShowPassageAvailable(string assignedObjectIdList, int qtiItemGroupId, int? virtualTestId = 0, bool? layoutV2 = false)
        {
            ViewBag.AssignedObjectIdList = assignedObjectIdList;
            ViewBag.QtiItemGroupId = qtiItemGroupId;
            ViewBag.VirtualTestId = virtualTestId;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            if (layoutV2 == true)
            {
                return PartialView("v2/_PassageAssignForm");
            }
            return PartialView("_PassageAssignForm");

        }

        //[HttpGet]
        [UrlReturnDecode]
        public ActionResult Edit(int? qtiItemId, bool? fromVirtualQuestionProperty, int? virtualTestId, int? virtualQuestionId, int districtId = 0, bool fromItemLibrary = false)
        {
            if (!qtiItemId.HasValue)
                return RedirectToAction("Index", "ItemBank");

            var qtiItem = parameters.QtiItemService.GetQtiItemById(qtiItemId.Value);
            if (qtiItem != null)
            {
                //User has right to edit this qtiItem if he/she has right to edit itemset
                var hasPermission = parameters.QtiGroupService.HasRightToEditQtiGroup(qtiItem.QTIGroupID, CurrentUser);
                if (!hasPermission)
                {
                    return RedirectToAction("Index", "ItemBank");
                }
                
                var model = new QtiItemEditViewModel();
                model.IsSurvey = 0;
                model.FromItemLibrary = fromItemLibrary;
                if (virtualTestId.HasValue && virtualTestId > 0)
                {
                    var virtualTest = parameters.VirtualTestService.GetTestById(virtualTestId.GetValueOrDefault());
                    if (virtualTest?.DatasetOriginID == (int)DataSetOriginEnum.Survey)
                    {
                        model.IsSurvey = 1;
                    }

                    model.IsVirtualTestHasRetakeRequest = parameters.VirtualTestService.IsVirtualTestHasRetake(virtualTestId.Value);
                }
                model.QtiItemId = qtiItemId.Value;
                model.QTISchemaId = qtiItem.QTISchemaID;
                model.PointsPossible = qtiItem.PointsPossible;
                model.Title = qtiItem.Title;
                model.Description = qtiItem.Description;
                if (qtiItem.ResponseProcessingTypeID == (int)ResponseProcessingTypeIdEnum.AlgorithmicScoring)
                {
                    var objMaxPoint = parameters.VirtualTestService.GetMaxPointAlgorithmicByVirtualQuestionIDAndQTIItemID(0, qtiItemId.Value);
                    model.PointsPossible = objMaxPoint.QTIItemMaxPoint;
                    model.ResponseProcessingTypeID = qtiItem.ResponseProcessingTypeID.GetValueOrDefault();
                }
                model.ResponseIdentifier = qtiItem.ResponseIdentifier;
                model.QtiItemGroupId = qtiItem.QTIGroupID;
                model.MediaModel = new MediaModel
                {
                    ID = model.QtiItemGroupId
                };

                model.XmlContent = MakeValidXmlContent(qtiItem.XmlContent);

                if (virtualQuestionId.HasValue && virtualQuestionId.Value > 0)
                {
                    //bind noshuffle attribute
                    var passageNoshuffleList =
                        parameters.VirtualQuestionPassageNoShuffleService.Select()
                            .Where(x => x.VirtualQuestionID == virtualQuestionId.Value).ToList();
                    if (passageNoshuffleList.Any())
                        model.XmlContent = Util.AddNoshuffleAttrForPassage(model.XmlContent, passageNoshuffleList);
                }
                else
                {
                    if (qtiItem.QTISchemaID == (int)QTISchemaEnum.ExtendedText)
                    {
                        var virtualQuestionOfQtiItems = parameters.VirtualQuestionService.Select().Count(x => x.QTIItemID == qtiItem.QTIItemID && x.IsRubricBasedQuestion == true);

                        model.VirtualQuestionRubricCount = virtualQuestionOfQtiItems;
                    }
                }
                var qtiGroup = parameters.QtiGroupService.GetById(qtiItem.QTIGroupID);
                model.QtiGroupName = qtiGroup?.Name;

                if (fromVirtualQuestionProperty.HasValue && fromVirtualQuestionProperty == true)
                {
                    model.FromVirtualQuestionProperty = true;
                    model.VirtualTestId = virtualTestId.GetValueOrDefault();
                    model.VirtualQuestionId = virtualQuestionId.GetValueOrDefault();
                    model.HasTest = true;
                    var tests = parameters.VirtualQuestionService.Select().Where(x => x.VirtualTestID == virtualTestId).OrderBy(x => x.QuestionOrder).ToList();

                    //Get the number of virtual question in the virtual test
                    model.CountVirtualQuestions = tests.Count;
                    //get the max VirtualQuestionOrder
                    var maxQuestionOrder = parameters.VirtualQuestionService.Select()
                        .Where(x => x.VirtualTestID == virtualTestId.GetValueOrDefault())
                        .Select(x => x.QuestionOrder).Max();
                    if (model.CountVirtualQuestions > 0 && model.CountVirtualQuestions != maxQuestionOrder)
                    {
                        parameters.VirtualTestService.ReassignVirtualQuestionOrder(virtualTestId.GetValueOrDefault());
                    }

                    var test = tests.FirstOrDefault(x => x.VirtualQuestionID == virtualQuestionId.GetValueOrDefault());
                    if (test != null)
                    {
                        model.VirtualQuestionOrder = test == null ? 0 : test.QuestionOrder;
                        var nextItem = tests.FirstOrDefault(x => x.QuestionOrder == model.VirtualQuestionOrder + 1);
                        var preItem = tests.FirstOrDefault(x => x.QuestionOrder == model.VirtualQuestionOrder - 1);

                        model.PreviousQtiItemId = preItem == null ? 0 : preItem.QTIItemID.Value;
                        model.NextQtiItemId = nextItem == null ? 0 : nextItem.QTIItemID.Value;

                        model.PreviousVirtualQuesionId = preItem == null ? 0 : preItem.VirtualQuestionID;
                        model.NextVirtualQuesionId = nextItem == null ? 0 : nextItem.VirtualQuestionID;

                        if (test.IsRubricBasedQuestion == true)
                        {
                            model.IsAllowRubricGradingMode = 1;
                            var findVirtualQuestionIdsOfRubricBase = new int[] { test.VirtualQuestionID };
                            if (findVirtualQuestionIdsOfRubricBase?.Length > 0)
                            {
                                var rubricQuestionCategories = parameters.RubricModuleQueryService.GetRubicQuestionCategoriesByVirtualQuestionIds(findVirtualQuestionIdsOfRubricBase).ToList();
                                model.RubricQuestionCategories = rubricQuestionCategories;
                            }
                        }
                        else
                        {
                            model.IsAllowRubricGradingMode = 0;
                            model.RubricQuestionCategories = new List<RubricQuestionCategoryDto>();
                        }
                    }
                }

                if (!model.FromVirtualQuestionProperty)
                {
                    //check if this item has associated virtual test or not
                    model.HasTest = false;
                    var hasTest = parameters.VirtualQuestionService.Select().Any(x => x.QTIItemID == qtiItemId);
                    model.HasTest = hasTest;
                    //Check if the item is the first or last item in item set (order by QuestionOrder)
                    model.PreviousQtiItemId = 0;
                    model.NextQtiItemId = 0;

                    var previousItem =
                        parameters.QtiItemService.GetAllQtiItem().FirstOrDefault(
                            x => x.QTIGroupID == qtiItem.QTIGroupID && x.QuestionOrder == qtiItem.QuestionOrder - 1);

                    if (previousItem != null)
                    {
                        model.PreviousQtiItemId = previousItem.QTIItemID;
                    }
                    var nextItem =
                        parameters.QtiItemService.GetAllQtiItem().FirstOrDefault(
                            x => x.QTIGroupID == qtiItem.QTIGroupID && x.QuestionOrder == qtiItem.QuestionOrder + 1);
                    if (nextItem != null)
                    {
                        model.NextQtiItemId = nextItem.QTIItemID;
                    }
                    //Get the number of qti items in the item set
                    var countQtiItems = parameters.QtiItemService.GetAllQtiItem()
                        .Where(x => x.QTIGroupID == model.QtiItemGroupId).Select(x => x.QTIItemID).Count();
                    model.CountQtiItems = countQtiItems;
                    model.QuestionOrder = qtiItem.QuestionOrder;
                }

                model.NoDuplicateAnswer = false;
                if (qtiItem.QTISchemaID == (int)QTISchemaEnum.UploadComposite && qtiItem.ResponseProcessingTypeID.HasValue && qtiItem.ResponseProcessingTypeID.GetValueOrDefault() == 5)
                {
                    model.NoDuplicateAnswer = true;
                }

                model.WarningTimeoutMinues = parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue("TestMakerWarningTimeOutMinute", 5);
                model.DefaultCookieTimeOutMinutes = parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue("DefaultCookieTimeOutMinutes", 30);
                model.KeepAliveDistanceSecond = parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue("KeepAliveDistanceSecond", 15);
                model.BasicSciencePaletteSymbol =
                parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue("BasicSciencePaletteSymbol", string.Empty);

                if (qtiItem.ResponseProcessingTypeID.HasValue &&
                    qtiItem.ResponseProcessingTypeID == (int)ResponseProcessingTypeIdEnum.AlgorithmicScoring)
                {
                    model.ListExpression = parameters.AlgorithmicScoreService.GetListExpression(qtiItemId.Value,
                        virtualQuestionId);
                }
                if ((virtualTestId ?? 0) < 1)
                {
                    model.IsAllowRubricGradingMode = 0;
                }
                if (qtiItem.QTISchemaID == (int)QtiSchemaEnum.MultiPart || (virtualTestId ?? 0) < 1)
                {
                    model.IsAllowRubricGradingMode = 0;
                    model.ListMultiPartExpression = parameters.MultiPartExpressionService.GetListExpression(qtiItemId.Value, virtualQuestionId);
                    model.RubricQuestionCategories = new List<RubricQuestionCategoryDto>();
                }
                if (qtiItem.QTISchemaID == (int)QtiSchemaEnum.ExtendedText && virtualTestId > 0)
                {
                    model.IsAllowRubricGradingMode = 1;
                }

                ViewBag.HasMoreThanOneSection = false;
                if (virtualTestId.HasValue && virtualQuestionId.HasValue)
                {
                    var sections = parameters.VirtualSectionService.GetVirtualSectionByVirtualTest(virtualTestId.Value);
                    if (sections != null && sections.Count > 1)
                    {
                        ViewBag.HasMoreThanOneSection = sections.Count > 1;
                        var virtualSection = parameters.VirtualSectionQuestionService.GetByVirtualQuestionId(virtualQuestionId.Value);
                        model.VirtualSectionId = virtualSection != null ? virtualSection.VirtualSectionId : 0;
                    }
                }


                List<PassageViewModel> passageList = Util.GetPassageList(model.XmlContent, false, null, false, false, parameters.qti3pPassageService, parameters.PassageService,parameters.dataFileUploadPassageService);
                var passgeIds = passageList.Select(x => x.QtiRefObjectID).ToArray();

                var passages = parameters.PassageService.GetQtiRefObjects(passgeIds);
                foreach (var item in passageList)
                {
                    var findExists = passages.FirstOrDefault(x => x.QTIRefObjectID == item.QtiRefObjectID);
                    if (findExists != null)
                    {
                        item.Name = string.IsNullOrEmpty(findExists.Name) ? "[unnamed]" : findExists.Name;
                    }
                }
                model.PassageList = passageList;


                var assessmentArtifactFileTypeGroups = Mapper.Map<IEnumerable<AssessmentArtifactFileTypeGroupViewModel>>
                (parameters.DistrictDecodeService
                    .GetAssessmentArtifactFileTypeGroups(CurrentUser.DistrictId.GetValueOrDefault()));

                var recordingOptions = Util.GetAssessmentArtifactRecordingOptions();

                ViewBag.AssessmentArtifactConfiguration =  new AssessmentArtifactConfigurationViewModel(assessmentArtifactFileTypeGroups, recordingOptions)
                    .SerializeObject(isCamelCase: true);
                ViewBag.DistrictId = CurrentUser.IsCorrectDistrict(districtId) ? districtId : CurrentUser.DistrictId ?? 0;

                return View(model);
            }

            //Can not find item, redirect to main page
            return RedirectToAction("Index", "ItemBank");
        }

        [HttpGet]
        public ActionResult GetMostRecentItemVersions(int qtiItemId, int numberOfVersions = 10)
        {
            var versions = parameters.QtiItemService.GetMostRecentItemVersions(CurrentUser.StateId ?? 0, qtiItemId, numberOfVersions);
            foreach (var version in versions)
            {
                version.XmlContent = MakeValidXmlContent(version.XmlContent);
            }

            return Json(new { versions }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RevertItem(RevertItemViewModel model)
        {
            var isSuccessful = parameters.QtiItemService.RevertItem(CurrentUser.Id, model.VirtualQuestionId, model.QtiItemId, model.QtiItemHistoryId, out RevertItemOutputDto output);

            if (isSuccessful)
            {
                Util.UploadMultiVirtualTestJsonFileToS3(model.QtiItemId, parameters.VirtualQuestionService, parameters.VirtualTestService, s3Service);
                UpdateItemPassage(model.QtiItemId);

                foreach (var question in output.VirtualQuestions)
                {
                    parameters.ManageSurveyService.ProcessingUpdateQuestionType(output.OldQTISchemaID, output.NewQTISchemaID, model.QtiItemId, question.VirtualQuestionID);
                }
            }

            return Json(new { isSuccessful });
        }

        private void UpdateItemPassage(int qtiItemId)
        {
            try
            {
                var qtiItem = parameters.QtiItemService.GetQtiItemById(qtiItemId);
                XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
                qtiItem.XmlContent = qtiItem.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
                qtiItem.XmlContent = qtiItem.XmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();
                List<PassageViewModel> passageList = Util.GetPassageList(qtiItem.XmlContent, false);
                if (passageList != null)
                {
                    parameters.QtiItemService.UpdateItemPassage(qtiItemId, passageList.Select(x => x.QtiRefObjectID).ToList(),
                        passageList.Select(x => x.RefNumber).ToList());
                }
            }
            catch
            {
                //nothing to do
            }
        }

        private string MakeValidXmlContent(string xmlContent)
        {
            var validXmlContent = xmlContent.ReplaceWeirdCharacters().ReplaceWeirdCharactersXmlContent();
            if (!Util.IsValidXmlContent(validXmlContent))
            {
                validXmlContent = xmlContent.ReplaceWeirdCharacters();
            }

            validXmlContent = Util.UpdateS3LinkForItemMedia(validXmlContent); //always use s3 content now
            validXmlContent = Util.UpdateS3LinkForPassageLink(validXmlContent);
            validXmlContent = Util.ReplaceVideoTag(validXmlContent);
            validXmlContent = XmlUtils.RemoveAllNamespacesPrefix(validXmlContent);

            return validXmlContent;
        }

        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult CheckIsConflictConstraintUpdate(UpdateItemViewModel model)
        {
            var xmlContent = model.XMLContent.RemoveLineBreaks().ReplaceWeirdCharacters();
            var errorMessage = "This question cannot be changed because test results exist.";
            var qtiItem = parameters.QtiItemService.GetQtiItemById(model.QtiItemId);
            QTIItemCheckConflictConstrainParameter checkConflictConstrainParameter = new QTIItemCheckConflictConstrainParameter
            {
                QtiItemId = model.QtiItemId,
                XmlContent = xmlContent,
                IsChangeAnswerChoice = model.IsChangeAnswerChoice,
            };

            if (model.VirtualTestId > 0)
            {
                checkConflictConstrainParameter.IsVirtualTestHasRetakeRequest = parameters.VirtualTestService.IsVirtualTestHasRetake(model.VirtualTestId);
            }

            var isConflictContraint = parameters.QtiItemService.CheckIsConflictConstraintUpdate(checkConflictConstrainParameter);
            var virtualTestCounting = parameters.VirtualTestService.GetVirtualTestIdsByQTIItemId(model.QtiItemId)
                .Where(x => x != model.VirtualTestId).Count();
            if (qtiItem != null && qtiItem.QTISchemaID == (int)QTISchemaEnum.ExtendedText)
            {
                if (model.VirtualquestionId > 0)
                {
                    if (model.RubricQuestionCategories != null && model.RubricQuestionCategories?.Count > 0)
                    {
                        var findVirtualQuestionByQTIItem = parameters.VirtualQuestionService.Select().Count(x => x.QTIItemID == qtiItem.QTIItemID);
                        if (findVirtualQuestionByQTIItem > 1)
                        {
                            return Json(new { success = true, message = "Rubric-based scoring cannot be added to this question because it is being used in other places. Please choose a different scoring method." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    var virtualQuestionExists = parameters.VirtualQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionID == model.VirtualquestionId && x.IsRubricBasedQuestion == true);
                    if (virtualQuestionExists != null)
                    {
                        VirtualTestProperty vtd = parameters.VirtualTestService.GetVirtualTestProperty(virtualQuestionExists.VirtualTestID, CurrentUser.RoleId, CurrentUser.DistrictId.GetValueOrDefault());

                        if (vtd != null && vtd.TotalTestResult > 0 && model.RubricQuestionCategories == null)
                        {
                            return Json(new { success = true, message = errorMessage }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    var virtualQuestionExists = parameters.VirtualQuestionService.Select().FirstOrDefault(x => x.QTIItemID == qtiItem.QTIItemID && x.IsRubricBasedQuestion == true);
                    if (virtualQuestionExists != null)
                    {
                        VirtualTestProperty vtd = parameters.VirtualTestService.GetVirtualTestProperty(virtualQuestionExists.VirtualTestID, CurrentUser.RoleId, CurrentUser.DistrictId.GetValueOrDefault());
                        if (vtd != null && vtd.TotalTestResult > 0)
                        {
                            var oldXmlContent = qtiItem.XmlContent;
                            var oldScoringMethod = QtiItemScoringMethodHelper.GetQtiItemScoringMethod(oldXmlContent, qtiItem.QTISchemaID, false);
                            var newScoringMethod = QtiItemScoringMethodHelper.GetQtiItemScoringMethod(xmlContent, qtiItem.QTISchemaID, false);
                            if (oldScoringMethod != newScoringMethod)
                            {
                                return Json(new { success = true, message = errorMessage }, JsonRequestBehavior.AllowGet);
                            }
                            var oldPointPossible = qtiItem.PointsPossible;
                            var newQtiItem = _qTIItemConvert.ConvertFromXmlContent(xmlContent);
                            if (newQtiItem != null && oldPointPossible != newQtiItem.PointsPossible)
                            {
                                return Json(new { success = true, message = errorMessage }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            return Json(new { success = isConflictContraint, message = "You are attempting to change question(s) which students have already tested against. This is not permitted.", virtualTestCounting = virtualTestCounting }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestMakerWarningTimeout()
        {
            int iWarningTimeoutMinues = parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue("TestMakerWarningTimeOutMinute", 5);
            int iDefaultCookieTimeOutMinutes = parameters.ConfigurationService.GetConfigurationByKeyWithDefaultValue("DefaultCookieTimeOutMinutes", 5);
            return Json(new { success = true, defaultcookietimeout = iDefaultCookieTimeOutMinutes, warningTimeout = iWarningTimeoutMinues }, JsonRequestBehavior.AllowGet);
        }
    }
}
