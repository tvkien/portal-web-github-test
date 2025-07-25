using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Enums;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Services;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.QuestionGroup;
using LinkIt.BubbleSheetPortal.Models.TestMaker;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.AuthorizeItemLibServices;
using LinkIt.BubbleSheetPortal.Web.BusinessObjects;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using LinkIt.BubbleSheetPortal.Web.Print;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using S3Library;
using LinkIt.BubbleSheetPortal.Models.DTOs.VirtualTest;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.Constants;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Web.ViewModels.VirtualTest;
using LinkIt.BubbleSheetPortal.Services.Survey;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [UrlReturnDecode]
    [VersionFilter]
    public class VirtualTestController : BaseController
    {
        private readonly VirtualTestService _virtualTestService;
        private readonly VirtualSectionService _virtualSectionService;
        private readonly TestScoreMethodService _testScoreMethodService;
        private readonly QTIRefObjectService _passageService;
        private readonly QTIITemService _qtiItemService;
        private readonly ConversionSetService _conversionSetService;
        private readonly VirtualQuestionTopicService _virtualQuestionTopicService;
        private readonly VirtualQuestionLessonOneService _virtualQuestionLessonOneService;
        private readonly VirtualQuestionLessonTwoService _virtualQuestionLessonTwoService;
        private readonly VirtualQuestionItemTagService _virtualQuestionItemTagService;
        private readonly TopicService _topicService;
        private readonly LessonOneService _lessonOneService;
        private readonly LessonTwoService _lessonTwoService;
        private readonly ItemTagService _itemTagService;
        private readonly VirtualSectionQuestionService _virtualSectionQuestionService;
        private readonly VirtualQuestionService _virtualQuestionService;
        private readonly AuthorizeItemLibService _authorizeItemLibService;
        private readonly VirtualQuestionAnswerScoreService _virtualQuestionAnswerScoreService;
        private readonly QtiBankService _qtiBankService;
        private readonly Qti3pPassageService _qti3pPassageService;
        private readonly QtiGroupService _qtiGroupService;
        private readonly DistrictService _districtService;
        private readonly StateService _stateService;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly VulnerabilityService _vulnerabilityService;
        private readonly VirtualQuestionBranchingService _virtualQuestionBranchingService;
        private readonly QuestionGroupService _questionGroupService;
        private readonly ConfigurationService _configurationService;
        private readonly RestrictionBO _restrictionBO;
        private readonly RubricModuleQueryService _rubricModuleQueryService;
        private readonly RubricModuleCommandService _rubricModuleCommandService;
        private readonly ManageSurveyService _manageSurveyService;
        private readonly IS3Service _s3Service;
        private readonly QTIOnlineTestSessionService _qTIOnlineTestSessionService;

        public VirtualTestController(VirtualTestService virtualTestService, VirtualSectionService virtualSectionService,
            TestScoreMethodService testScoreMethodService, QTIRefObjectService passageService,
            QTIITemService qtiItemService,
            ConversionSetService conversionSetService, VirtualQuestionTopicService virtualQuestionTopicService,
            VirtualQuestionLessonOneService virtualQuestionLessonOneService,
            VirtualQuestionLessonTwoService virtualQuestionLessonTwoService,
            VirtualQuestionItemTagService virtualQuestionItemTagService,
            TopicService topicService, LessonOneService lessonOneService, LessonTwoService lessonTwoService,
            ItemTagService itemTagService,
            VirtualSectionQuestionService virtualSectionQuestionService, VirtualQuestionService virtualQuestionService,
            AuthorizeItemLibService authorizeItemLibService,
            VirtualQuestionAnswerScoreService virtualQuestionAnswerScoreService, QtiBankService qtiBankService,
            QtiGroupService qtiGroupService, Qti3pPassageService qti3pPassageService,
            DistrictService districtService, StateService stateService,
            DistrictDecodeService districtDecodeService,
            VirtualQuestionBranchingService virtualQuestionBranchingService,
            VulnerabilityService vulnerabilityService,
            QuestionGroupService questionGroupService,
            ConfigurationService configurationService,
            RestrictionBO restrictionBO,
            RubricModuleQueryService rubricModuleQueryService,
            RubricModuleCommandService rubricModuleCommandService,
            ManageSurveyService manageSurveyService, IS3Service s3Service,
            QTIOnlineTestSessionService qTIOnlineTestSessionService)
        {
            _virtualTestService = virtualTestService;
            _virtualSectionService = virtualSectionService;
            _testScoreMethodService = testScoreMethodService;
            _passageService = passageService;
            _qtiItemService = qtiItemService;
            _conversionSetService = conversionSetService;
            _virtualQuestionTopicService = virtualQuestionTopicService;
            _virtualSectionQuestionService = virtualSectionQuestionService;
            _virtualQuestionService = virtualQuestionService;
            _virtualQuestionLessonOneService = virtualQuestionLessonOneService;
            _virtualQuestionLessonTwoService = virtualQuestionLessonTwoService;
            _virtualQuestionItemTagService = virtualQuestionItemTagService;
            _topicService = topicService;
            _lessonOneService = lessonOneService;
            _lessonTwoService = lessonTwoService;
            _itemTagService = itemTagService;
            _qtiGroupService = qtiGroupService;
            _authorizeItemLibService = authorizeItemLibService;
            _virtualQuestionAnswerScoreService = virtualQuestionAnswerScoreService;
            _qtiBankService = qtiBankService;
            _qti3pPassageService = qti3pPassageService;
            _qtiGroupService = qtiGroupService;
            _districtService = districtService;
            _stateService = stateService;
            _districtDecodeService = districtDecodeService;
            _vulnerabilityService = vulnerabilityService;
            _virtualQuestionBranchingService = virtualQuestionBranchingService;
            _questionGroupService = questionGroupService;
            _configurationService = configurationService;
            _restrictionBO = restrictionBO;
            _rubricModuleQueryService = rubricModuleQueryService;
            _rubricModuleCommandService = rubricModuleCommandService;
            _manageSurveyService = manageSurveyService;
            _s3Service = s3Service;
            _qTIOnlineTestSessionService = qTIOnlineTestSessionService;
        }

        public ActionResult Index(int virtualTestId, int? virtualQuestionId, int? qtiItemId, int districtId = 0)
        {
            var virtualTest = _virtualTestService.GetLinkitTestById(virtualTestId);
            if (virtualTest != null)
            {
                var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
                if (!hasPermission)
                {
                    return RedirectToAction("Index", "ManageTest");
                }

                var model = BuildVirtualTestViewModelByVirtualTestID(virtualTestId);
                ViewBag.TestItemMediaPath = string.Empty;
                if (virtualQuestionId.HasValue)
                {
                    model.VirtualQuestionId = virtualQuestionId.Value;
                    model.QtiItemId = qtiItemId.GetValueOrDefault();
                }

                var sections = _virtualSectionService.GetVirtualSectionByVirtualTest(virtualTestId);
                ViewBag.HasMoreThanOneSection = (sections != null && sections.Count > 1);

                model.IsSupportQuestionGroup = CheckUserSupportQuestionGroup();

                if (!CurrentUser.IsCorrectDistrict(districtId))
                {
                    districtId = CurrentUser.DistrictId ?? 0;
                }
                
                if (districtId > 0)
                {
                    IsCheckRestrictionRules(virtualTestId, districtId, virtualTest, model);
                }

                ViewBag.DistrictId = districtId;
                return View(model);
            }
            return RedirectToAction("Index", "ManageTest");
        }

        private void IsCheckRestrictionRules(int virtualTestId, int districtId, VirtualTestData virtualTest, VirtualTestViewModel model)
        {
            model.RestrictionAccessList.AllowToPrint = _restrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
            {
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                ModuleCode = RestrictionConstant.Module_PrintTest,
                TestId = virtualTestId,
                DistrictId = districtId,
                ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test,
                BankId = virtualTest.BankID
            });

            model.RestrictionAccessList.AllowToReviewOnline = _restrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
            {
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                ModuleCode = RestrictionConstant.Module_ReviewOnline,
                TestId = virtualTestId,
                DistrictId = districtId,
                ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test,
                BankId = virtualTest.BankID
            });
        }

        public ActionResult RefreshSectionQuestionData(int virtualTestId, bool? numberingItemBySection)
        {
            var model = BuildVirtualTestViewModelByVirtualTestID(virtualTestId, numberingItemBySection ?? false);
            var jsonResult = Json(new { Success = true, newData = model });
            var js = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            var jsonStringResult = js.Serialize(jsonResult.Data);
            return Content(jsonStringResult, "application/json");
        }

        private VirtualTestViewModel BuildVirtualTestViewModelByVirtualTestID(int virtualTestId, bool numberingItemBySection = false)
        {
            //var useS3Content = _districtDecodeService.UseS3Content(CurrentUser.DistrictId.GetValueOrDefault());
            var virtualTest = _virtualTestService.GetTestById(virtualTestId);
            VirtualTestViewModel model = new VirtualTestViewModel();
            if (virtualTest != null)
            {
                //TODO: build QuestionGroup
                var lstQuestionGroup = _questionGroupService.GetListQuestionGroupByVirtualTestId(virtualTest.VirtualTestID);
                var lstVirtualQuestionGroup = _questionGroupService.GetListVirtualQuestionGroupByVirtualTestId(virtualTest.VirtualTestID);
                model.NavigationMethodID = virtualTest.NavigationMethodID;
                model.VirtualTestId = virtualTest.VirtualTestID;
                model.Name = virtualTest.Name;
                model.VirtualTestSubTypeID = virtualTest.VirtualTestSubTypeID ?? 0;
                model.VirtualTestSourceId = virtualTest.VirtualTestSourceID;
                model.IsNumberQuestions = virtualTest.IsNumberQuestions.GetValueOrDefault();
                model.IsSurvey = virtualTest.DatasetOriginID == (int)DataSetOriginEnum.Survey;
                model.HasRetakeRequest = _virtualTestService.IsVirtualTestHasRetake(virtualTestId);
                var sectionQuestionQtiItemList = _virtualTestService.GetVirtualSectionQuestionQtiItem(virtualTestId).OrderBy(x => x.QuestionOrder).ToList();
                foreach (var item in sectionQuestionQtiItemList)
                {
                    var virtualQeustionGroup = lstVirtualQuestionGroup.FirstOrDefault(o => o.VirtualQuestionID == item.VirtualQuestionID);
                    if (virtualQeustionGroup != null)
                    {
                        item.QuestionGroupID = virtualQeustionGroup.QuestionGroupID;
                    }
                }

                var assignedVirtualQuestionIdList = sectionQuestionQtiItemList.Select(x => x.VirtualQuestionID).ToList();
                //If there's any unassigned virtual question ( virtualquestion not in virtualsectionquestion), assign unassigned virtualquestion to default section (section id = 0)
                var unAssignedVirtualQuestions = _virtualQuestionService.Select().Where(
                        x => x.VirtualTestID == virtualTestId && !assignedVirtualQuestionIdList.Contains(x.VirtualQuestionID)).ToList();
                if (unAssignedVirtualQuestions?.Count > 0)
                {
                    foreach (var unAssignedVirtualQuestion in unAssignedVirtualQuestions)
                    {
                        var newVirtualSectionQuestion = new VirtualSectionQuestion
                        {
                            Order = unAssignedVirtualQuestion.QuestionOrder,
                            VirtualQuestionId = unAssignedVirtualQuestion.VirtualQuestionID,
                            VirtualSectionId = 0,
                            //assign unassigned virtualquestion to default section
                            VirtualSectionQuestionId = 0
                        };
                        _virtualSectionQuestionService.Save(newVirtualSectionQuestion);
                    }
                    //Reset QuestionOrder of VirtualQuestion
                    ReAssignQuestionOrderForVirtualQuestionWithSection(virtualTestId);
                    //get the list again (now all unassigned virtual questions have been assigned to section id 0 (default section)
                    sectionQuestionQtiItemList = _virtualTestService.GetVirtualSectionQuestionQtiItem(virtualTestId).OrderBy(x => x.QuestionOrder).ToList();

                    var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestId);
                    Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);
                }

                var sectionList = sectionQuestionQtiItemList.Select(x => new
                {
                    VirtualSectionId = x.VirtualSectionID,
                    VirtualTestId = virtualTestId,
                    Order = x.VirtualSectionOrder ?? -1,
                    Title = x.VirtualSectionID == 0 ? "Default Section" : (
                            string.IsNullOrWhiteSpace(x.VirtualSectionTitle)
                                ? "Section " + (x.VirtualSectionOrder.HasValue ? x.VirtualSectionOrder.Value.ToString() : "1")
                                : x.VirtualSectionTitle)
                }).OrderBy(x => x.Order).ToList();
                var sectionIdList = sectionList.Select(x => x.VirtualSectionId).Distinct().ToList();
                foreach (var sectionID in sectionIdList)
                {
                    var lstQuestionGroupOnSection = lstQuestionGroup.Where(o => o.VirtualSectionID == sectionID)
                        .Select(o => new QuestionGroupData()
                        {
                            QuestionGroupID = o.QuestionGroupID,
                            Order = o.Order,
                            XmlContent = o.XmlContent,
                            IsEmpty = !lstVirtualQuestionGroup.Any(x => x.QuestionGroupID == o.QuestionGroupID)
                        }).ToList();
                    if (sectionID == 0)
                    {
                        lstQuestionGroupOnSection = lstQuestionGroup.Where(o => !o.VirtualSectionID.HasValue || o.VirtualSectionID == 0)
                        .Select(o => new QuestionGroupData()
                        {
                            QuestionGroupID = o.QuestionGroupID,
                            Order = o.Order,
                            XmlContent = o.XmlContent,
                            IsEmpty = !lstVirtualQuestionGroup.Any(x => x.QuestionGroupID == o.QuestionGroupID)
                        }).ToList();
                    }
                    model.VirtualSectionList.Add(sectionList.Where(x => x.VirtualSectionId == sectionID)
                        .Select(x => new VirtualSectionViewModel
                        {
                            VirtualSectionId = x.VirtualSectionId,
                            VirtualTestId = x.VirtualTestId,
                            Order = x.Order,
                            Title = x.Title,
                            QuestionGroupList = lstQuestionGroupOnSection.Count > 0 ? lstQuestionGroupOnSection : new List<QuestionGroupData>()
                        }).First());
                }

                foreach (var virtualSection in model.VirtualSectionList)
                {
                    virtualSection.SectionQuestionQtiItemList.AddRange(sectionQuestionQtiItemList
                        .Where(x => x.VirtualSectionID == virtualSection.VirtualSectionId)
                        .OrderBy(x => x.QuestionOrder));
                }

                //load sections that don't have question
                var virtualSectionList =
                    _virtualSectionService.GetVirtualSectionByVirtualTest(virtualTestId)
                        .Where(x => !sectionIdList.Contains(x.VirtualSectionId))
                        .OrderBy(x => x.Order)
                        .ToList();
                model.VirtualSectionList.AddRange(virtualSectionList.Select(x => new VirtualSectionViewModel
                {
                    VirtualSectionId = x.VirtualSectionId,
                    VirtualTestId = x.VirtualTestId,
                    Order = x.Order,
                    Title = string.IsNullOrWhiteSpace(x.Title) ? "Section " + x.Order.ToString() : x.Title,
                    QuestionGroupList = lstQuestionGroup
                                            .Where(o => o.VirtualSectionID == x.VirtualSectionId)
                                            .Select(o => new QuestionGroupData()
                                            {
                                                QuestionGroupID = o.QuestionGroupID,
                                                XmlContent = o.XmlContent,
                                                Order = o.Order,
                                                IsEmpty = !lstVirtualQuestionGroup.Any(k => k.QuestionGroupID == o.QuestionGroupID)
                                            }).ToList()
                }));

                model.VirtualSectionList = model.VirtualSectionList.OrderBy(x => x.Order).ToList();
                //if there's no section, just add a section with id = 0 and order = 0 to show on UI
                if (model.VirtualSectionList == null)
                {
                    model.VirtualSectionList = new List<VirtualSectionViewModel>();
                }
                if (model.VirtualSectionList.Count == 0)
                {
                    model.VirtualSectionList.Add(new VirtualSectionViewModel
                    {
                        VirtualSectionId = 0,
                        Order = 0,
                        Title = "Default Section",
                        VirtualTestId = virtualTestId,
                        QuestionGroupList = lstQuestionGroup
                        .Where(o => !o.VirtualSectionID.HasValue || o.VirtualSectionID.Value == 0)
                        .Select(o => new QuestionGroupData()
                        {
                            QuestionGroupID = o.QuestionGroupID,
                            XmlContent = o.XmlContent,
                            Order = o.Order,
                            IsEmpty = !lstVirtualQuestionGroup.Any(x => x.QuestionGroupID == o.QuestionGroupID)
                        }).ToList()
                    });
                }
                //According to Flash, if there's only one Default Section (sectionId=0) then the title should be "Default Section"
                if (model.VirtualSectionList.Count == 1 && model.VirtualSectionList[0].VirtualSectionId == 0)
                {
                    model.VirtualSectionList[0].Title = "Default Section";
                }

                var virtualQuestionIds = new List<int>();
                int firstItemNumber = 0;
                foreach (var section in model.VirtualSectionList)
                {
                    foreach (var sectionQuestion in section.SectionQuestionQtiItemList)
                    {
                        virtualQuestionIds.Add(sectionQuestion.VirtualQuestionID);
                        sectionQuestion.XmlContent = AdjustXmlContent(sectionQuestion.XmlContent);
                        sectionQuestion.XmlContent = XmlUtils.RemoveAllNamespacesPrefix(sectionQuestion.XmlContent);
                        sectionQuestion.XmlContent = Util.UpdateS3LinkForItemMedia(sectionQuestion.XmlContent);
                        sectionQuestion.XmlContent = Util.UpdateS3LinkForPassageLink(sectionQuestion.XmlContent);

                        if (numberingItemBySection)
                        {
                            sectionQuestion.ItemNumber = sectionQuestion.Order;
                        }
                        else
                        {
                            if (firstItemNumber == 0 && sectionQuestion.QuestionOrder != 1)
                            {
                                _virtualTestService.ReassignVirtualQuestionOrder(virtualTestId);
                            }
                            firstItemNumber++;
                            sectionQuestion.ItemNumber = firstItemNumber;
                        }
                    }
                }

                AddRubricBaseQuestions(model, virtualQuestionIds.Distinct());
            }
            return model;
        }

        private void AddRubricBaseQuestions(VirtualTestViewModel model, IEnumerable<int> virtualQuestionIds)
        {
            var rubricQuestionCategories = _rubricModuleQueryService.GetSelectListRubricQuestionCategoryByQuestionIds(virtualQuestionIds);
            foreach (var section in model.VirtualSectionList)
            {
                foreach (var sectionQuestion in section.SectionQuestionQtiItemList)
                {
                    var rubricQuestion = rubricQuestionCategories.FirstOrDefault(x => x.VirtualQuestionID == sectionQuestion.VirtualQuestionID);
                    if (rubricQuestion != null)
                    {
                        sectionQuestion.IsRubricBasedQuestion = true;
                        sectionQuestion.RubricQuestionCategoryies = rubricQuestionCategories.Where(x => x.VirtualQuestionID == sectionQuestion.VirtualQuestionID).ToList();
                    }
                }
            }
        }

        public ActionResult CheckIfCurrentUserIsAuthor(int virtualTestId)
        {
            var test = _virtualTestService.GetTestById(virtualTestId);
            var isAuthor = test != null && test.AuthorUserID == CurrentUser.Id;
            return Json(new { IsAuthor = isAuthor }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBanksByUser(int userID)
        {
            return null;
        }

        public ActionResult LoadVirtualTests(int bankId)
        {
            //Get the virtualTestId from url
            string s = HttpContext.Request["virtualTestId"];
            var data = new List<VirtualTestViewModel>();
            var parser = new DataTableParser<VirtualTestViewModel>();
            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadTestBanks()
        {
            var data = new List<TestBankViewModel>();
            var parser = new DataTableParser<TestBankViewModel>();
            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> GetAvailableNavigationMethods(int districtId, string nameOrLabel)
        {
            var selectListItems = new List<SelectListItem>();
            var listNavigationMothed = new List<TestNavigationMethodCustomDto>();

            var configurationTestNavigationMethodsInDistrict = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtId, nameOrLabel);
            if (configurationTestNavigationMethodsInDistrict != null && !string.IsNullOrEmpty(configurationTestNavigationMethodsInDistrict.Value))
            {
                listNavigationMothed = JsonConvert.DeserializeObject<List<TestNavigationMethodCustomDto>>(configurationTestNavigationMethodsInDistrict.Value);
            }
            else
            {
                var configurationTestNavigationMethods = _configurationService.GetConfigurationByKeyWithDefaultValue(nameOrLabel, string.Empty);
                if (string.IsNullOrEmpty(configurationTestNavigationMethods))
                {
                    listNavigationMothed = JsonConvert.DeserializeObject<List<TestNavigationMethodCustomDto>>(configurationTestNavigationMethods);
                }
            }

            if (listNavigationMothed.Count > 0)
            {
                selectListItems = listNavigationMothed.Select(item => new SelectListItem
                {
                    Value = item.VirtualTestSubType.ToString(),
                    Text = item.Name
                }).ToList();
            }
            else
            {
                selectListItems.Add(new SelectListItem() { Value = "1", Text = "Linear Test", Selected = true });
                selectListItems.Add(new SelectListItem() { Value = "2", Text = "Normal Branching" });
                selectListItems.Add(new SelectListItem() { Value = "3", Text = "Section-based Branching" });
                selectListItems.Add(new SelectListItem() { Value = "4", Text = "Algorithmic Branching" });
            }

            return selectListItems;
        }

        [HttpGet]
        public ActionResult LoadVirtualTestProperties(int virtualTestId)
        {
            VirtualTestData vtd = _virtualTestService.GetTestById(virtualTestId);
            var vTest = new VirtualTestPropertiesViewModel()
            {
                VirtualTestId = 0,
                AvailableNavigationMethods = new List<SelectListItem>(),
                DatasetCategoryID = -1
            };
            int districtId = CurrentUser.DistrictId.GetValueOrDefault();

            vTest.AvailableNavigationMethods = GetAvailableNavigationMethods(districtId, TextConstants.TESTDESIGN_CONFIG_NAVIGATION_METHODS);
            if (vtd.DatasetOriginID == (int)DataSetOriginEnum.Survey)
                vTest.AvailableNavigationMethods = vTest.AvailableNavigationMethods.Where(x => x.Value == "1" || x.Value == "2").ToList();

            if (vtd != null)
            {
                //Calculate the Total Points Possible
                var totalPointsPossible = _virtualQuestionService.GetTotalPointsPossible(virtualTestId);
                if (vtd.TestScoreMethodID.HasValue && vtd.TestScoreMethodID.Value == (int)TestScoreMethodEnum.Subtract_From_100 && totalPointsPossible.HasValue && totalPointsPossible.Value < 100)
                {
                    totalPointsPossible = 100;
                }
                vTest.DatasetCategoryID = vtd.DatasetCategoryID ?? -1;

                vTest.VirtualTestId = vtd.VirtualTestID;
                vTest.Name = vtd.Name;
                vTest.Instruction = vtd.Instruction.ReplaceWeirdCharacters();
                vTest.TestScoreMethodID = vtd.TestScoreMethodID ?? -1;
                vTest.AvailableScoringMethods = new List<SelectListItem>();
                vTest.IsBranchingTest = vtd.VirtualTestSubTypeID == (int)VirtualTestSubType.Branching;
                vTest.IsTeacherLed = vtd.IsTeacherLed.GetValueOrDefault();
                vTest.IsSectionBranchingTest = false;
                vTest.TotalPointsPossible = totalPointsPossible;
                vTest.BasicSciencePaletteSymbol = _configurationService.GetConfigurationByKeyWithDefaultValue("BasicSciencePaletteSymbol", string.Empty);
                vTest.CurrentNavigationMethodID = vtd.NavigationMethodID.GetValueOrDefault();
                vTest.NavigationMethodID = vtd.NavigationMethodID;
                vTest.IsOverwriteTestResults = !vtd.IsMultipleTestResult.GetValueOrDefault();
                vTest.IsSurvey = vtd.DatasetOriginID == (int)DataSetOriginEnum.Survey;
                vTest.HasRetakeRequest = _virtualTestService.IsVirtualTestHasRetake(virtualTestId);

                if (vtd.VirtualTestSubTypeID == (int)VirtualTestSubType.SECTION_BRANCHING)
                {
                    vTest.IsSectionBranchingTest = true;
                }
                vTest.NavigationMethodID = vtd.NavigationMethodID;
                vTest.VirtualTestSubTypeID = vtd.NavigationMethodID;
                //check IsCustomItemNaming
                vTest.IsCustomItemNaming = _virtualTestService.IsCustomItemNaming(virtualTestId);
                ViewBag.IsIsPublisherOrNetworkAdmin = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin;
                ViewBag.AllowChangeDataSetCategory = DoesEnableAbilityToChangeTestCategory(CurrentUser.DistrictId);
                return PartialView("_VirtualTestProperties", vTest);
            }
            else
            {
                ViewBag.IsIsPublisherOrNetworkAdmin = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin; ;
                return PartialView("_VirtualTestProperties", vTest);
            }
        }
        private bool DoesEnableAbilityToChangeTestCategory(int? districtId)
        {
            if (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin)
                return true;

            string districtDecode = _districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId.GetValueOrDefault(), Constanst.EnableAbilityToChangeTestCategory).FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(districtDecode))
            {
                districtDecode = _configurationService.GetConfigurationByKey(Constanst.EnableAbilityToChangeTestCategory)?.Value;
            }
            return districtDecode?.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Any(c => string.Compare(c, "TestDesign", true) == 0) ?? false;
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTestProperties(EditTestPropertiesViewModel model)
        {
            int navigationMethodID;
            int? totalPointsPossible = 0;
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return Json(new { Success = false, ErrorMessage = "Please input Name!" });
            }
            model.Name = model.Name.Trim();
            if (model.Name.Length > 150)
            {
                return Json(new { Success = false, ErrorMessage = "Length of Name should not over 150 characters!" });
            }
            if (!string.IsNullOrWhiteSpace(model.XmlContent))
            {
                model.XmlContent = model.XmlContent.Trim();
            }
            try
            {
                //Get the virtual test
                var virtualTest = _virtualTestService.GetTestById(model.VirtualTestId);
                if (virtualTest == null)
                {
                    return Json(new { Success = false, ErrorMessage = "Can not find the test!" });
                }
                //check to avoid modifying ajax parameter bankId)
                var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
                if (!hasPermission)
                {
                    return Json(new { Success = false, ErrorMessage = "Has no right to update this test!" });
                }

                _virtualTestService.UpdateAssociatedVirtualTestRetakes(model.VirtualTestId, virtualTest.Name, model.Name);

                virtualTest.Name = model.Name;
                virtualTest.DatasetCategoryID = model.DatasetCategoryID;
                virtualTest.Instruction = model.XmlContent.ConvertFromUnicodeToWindow1252();
                if (model.TestScoreMethodID < 0)
                {
                    model.TestScoreMethodID = null;
                }
                virtualTest.TestScoreMethodID = model.TestScoreMethodID;
                if (virtualTest.DatasetOriginID == (int)DataSetOriginEnum.Survey)
                    virtualTest.TestScoreMethodID = (int)TestScoreMethodEnum.Survey;

                virtualTest.IsTeacherLed = model.IsTeacherLed;
                navigationMethodID = model.NavigationMethodID ?? (int)NavigationMethodEnum.NO_BRANCHING;
                virtualTest.IsNumberQuestions = model.IsNumberQuestions;
                virtualTest.IsMultipleTestResult = !model.IsOverwriteTestResults;
                virtualTest.NavigationMethodID = model.NavigationMethodID;

                var isFDOI = model.DatasetCategoryID == (int)DataSetCategoryEnum.VDET_FDOI;
                var isEOI = model.DatasetCategoryID == (int)DataSetCategoryEnum.VDET_EOI;

                if (!isFDOI && !isEOI)
                {
                    var categoryId = model.NavigationMethodID;
                    if (categoryId > 0 && categoryId == (int)NavigationMethodEnum.NORMAL_BRANCHING)
                    {
                        virtualTest.VirtualTestSubTypeID = 5;
                        virtualTest.VirtualTestSourceID = 1;
                    }
                    else if (categoryId > 0 && categoryId == (int)NavigationMethodEnum.SECTION_BASE_BRANCHING)
                    {
                        virtualTest.VirtualTestSubTypeID = 13;
                        virtualTest.VirtualTestSourceID = 1;
                    }
                    else if (categoryId > 0 && categoryId == (int)NavigationMethodEnum.NO_BRANCHING)
                    {
                        virtualTest.VirtualTestSubTypeID = 1;
                        virtualTest.VirtualTestSourceID = 1;
                    }
                    else if (categoryId > 0 && categoryId == (int)NavigationMethodEnum.ALGORITHMIC_BRANCHING)
                    {
                        if (model.DatasetCategoryID == (int)DataSetCategoryEnum.MOI)
                        {
                            virtualTest.VirtualTestSubTypeID = 10;
                            virtualTest.VirtualTestSourceID = 1;
                        }
                        else if (model.DatasetCategoryID == 2 && model.TestScoreMethodID == (int)TestScoreMethodEnum.APAK)
                        {
                            virtualTest.VirtualTestSubTypeID = null;
                            virtualTest.VirtualTestSourceID = 8;
                        }
                    }
                }
                if (model.TestScoreMethodID == (int)TestScoreMethodEnum.New_ACT)
                {
                    virtualTest.VirtualTestSubTypeID = (int)VirtualTestSubType.NewACT;
                }
                else if (model.TestScoreMethodID == (int)TestScoreMethodEnum.New_SAT)
                {
                    virtualTest.VirtualTestSubTypeID = (int)VirtualTestSubType.NewSAT;
                }
                _virtualTestService.Save(virtualTest);

                if (model.NavigationMethodID != (int)NavigationMethodEnum.NORMAL_BRANCHING) // NavigationMethodID = 2
                {
                    //Delete Config Normal Branching
                    _virtualTestService.DeleteVirtualQuestionBranchingByTestID(virtualTest.VirtualTestID);
                }
                else if (virtualTest.NavigationMethodID != (int)NavigationMethodEnum.SECTION_BASE_BRANCHING) // NavigationMethodID = 3
                {
                    //Delete Config Section Base Branching
                    _virtualTestService.DeleteVirtualSectionBranchingByTestID(virtualTest.VirtualTestID);
                }
                else if (virtualTest.NavigationMethodID != (int)NavigationMethodEnum.ALGORITHMIC_BRANCHING) // NavigationMethodID = 4
                {
                    //Delete Config ALGORITHMIC
                    _virtualTestService.DeleteVirtualQuestionBranchingAlgorithmByTestID(virtualTest.VirtualTestID);
                }
                totalPointsPossible = _virtualQuestionService.GetTotalPointsPossible(model.VirtualTestId);
                if (virtualTest.TestScoreMethodID.HasValue && virtualTest.TestScoreMethodID.Value == 2 && totalPointsPossible.HasValue && totalPointsPossible.Value < 100)
                {
                    totalPointsPossible = 100;
                }

                if (model.IsCustomItemNaming)
                {
                    var virtualtestMeta = new VirtualTestMeta()
                    {
                        VirtualTestID = virtualTest.VirtualTestID,
                        Name = Constanst.IsCustomItemNaming,
                        Data = string.Empty
                    };
                    _virtualTestService.SaveVirtualTestMeta(virtualtestMeta);
                }
                else
                {
                    var virtualtestMeta = new VirtualTestMeta()
                    {
                        VirtualTestID = virtualTest.VirtualTestID,
                        Name = Constanst.IsCustomItemNaming,
                        Data = string.Empty
                    };
                    _virtualTestService.DeleteVirtualTestMeta(virtualtestMeta);
                    _virtualQuestionService.ClearQuestionLabelQuestionLNumber(virtualTest.VirtualTestID);
                }

                var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTest.VirtualTestID);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                if (!s3Result.IsSuccess)
                {
                    return Json(new
                    {
                        Success = false,
                        ErrorMessage =
                                        "Virtual Test has been updated successfully but uploading json file to S3 fail: " +
                                        s3Result.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { Success = false, ErrorMessage = "There was some error. Can not update Test Properties right now." });
            }

            return Json(new { Success = true, NavigationMethodID = navigationMethodID, TestScoreMethodID = model.TestScoreMethodID, TotalPointsPossible = totalPointsPossible });
        }

        public ActionResult GetTestScoreMethods()
        {
            List<ListItem> data;
            data = _testScoreMethodService.GetAll().Select(x => new ListItem { Name = x.Name, Id = x.TestScoreMethodId }).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [UrlReturnDecode]
        public ActionResult LoadVirtualQuestionProperties(int virtualQuestionId, int qtiItemId)
        {
            VirtualQuestionProperties prop = _virtualTestService.GetVirtualQuestionProperties(virtualQuestionId);
            var model = new VirtualQuestionPropertiesViewModel();
            if (prop != null)
            {
                model.VirtualTestId = prop.VirtualTestID;
                model.VirtualQuestionId = prop.VirtualQuestionID;
                model.QtiItemId = qtiItemId;
                model.ItemBank = prop.ItemBank;
                model.ItemSet = prop.ItemSet;
                model.ItemType = prop.QtiSchemaDes;
                model.PointPossible = prop.PointsPossible;
                model.OringinalPointPossible = prop.QtiPointsPossible;
                model.ScoreName = prop.ScoreName;
                model.HasRetakeRequest = _virtualTestService.IsVirtualTestHasRetake(prop.VirtualTestID);

                var virtualTest = _virtualTestService.GetTestById(prop.VirtualTestID);
                model.IsSurvey = virtualTest.DatasetOriginID == (int) DataSetOriginEnum.Survey;
                model.HasTestResult = _virtualTestService.CheckHasTestResultByTestId(prop.VirtualTestID);
                //TODO: Support Algorithmic //TODO:
                if (prop.ResponseProcessingTypeId == (int)ResponseProcessingTypeIdEnum.AlgorithmicScoring)
                {
                    var objMaxPoint = _virtualTestService.GetMaxPointAlgorithmicByVirtualQuestionIDAndQTIItemID(virtualQuestionId, qtiItemId);
                    model.PointPossible = objMaxPoint.QuestionMaxPoint;
                    model.OringinalPointPossible = objMaxPoint.QTIItemMaxPoint;
                }
                model.BaseVirtualQuestionId = prop.BaseVirtualQuestionId;

                //User has right to edit this qtiItem if he/she has right to edit itemset
                var qtiItem = _qtiItemService.GetQtiItemById(qtiItemId);
                var hasPermission = false;
                if (qtiItem != null)
                {
                    hasPermission = _qtiGroupService.HasRightToEditQtiGroup(qtiItem.QTIGroupID, CurrentUser);
                }

                model.CanEditQTIITem = hasPermission;

                var districtTagDic = Util.ParseDistrictTag(prop.ItemTagList);

                StringBuilder districtTagBuilder = new StringBuilder();
                if (districtTagDic.Count > 0)
                {
                    var firstCat = true;
                    foreach (var category in districtTagDic)
                    {
                        if (firstCat)
                        {
                            districtTagBuilder.Append(string.Format("{0}: ", category.Key));
                            firstCat = false;
                        }
                        else
                        {
                            districtTagBuilder.Append(string.Format("<br/> {0}: ", category.Key));
                        }

                        if (category.Value != null && category.Value.Count > 0)
                        {
                            districtTagBuilder.Append(string.Join(", ", category.Value));
                        }
                    }
                }
                string districtTag = districtTagBuilder.ToString();
                var userStateIdList = _stateService.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);

                prop.XmlContent = prop.XmlContent ?? string.Empty;

                XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
                var xmlContent = prop.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);

                xmlContent = xmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();

                StringBuilder passagesStringBuilder = new StringBuilder();

                var passageNameList = Util.GetPassageNameList(xmlContent, _passageService, _qti3pPassageService, true);

                if (passageNameList.Count > 0)
                {
                    passagesStringBuilder.Append("&nbsp;+&nbsp;" + passageNameList[0]);

                    for (int i = 1; i < passageNameList.Count; i++)
                    {
                        passagesStringBuilder.Append(string.Format("<br>&nbsp;+&nbsp;{0}", passageNameList[i]));
                    }
                }
                model.Passages = passagesStringBuilder.ToString();
                if (!prop.IsRubricBasedQuestion)
                {
                    model.Standards = string.Join(", ", Util.ParseStandardNumber(prop.StandardNumberList, CurrentUser.RoleId, userStateIdList));
                    model.Topics = string.Join(", ", Util.ParseTopic(prop.TopicList));
                    model.Skills = string.Join(", ", Util.ParseSkill(prop.SkillList));
                    model.Others = string.Join(", ", Util.ParseOther(prop.OtherList));
                    model.ItemTags = districtTag;
                }
                else
                {
                    model.IsRubricBasedQuestion = true;
                    var acceptableStandards = Util.ParseStandardNumber(prop.StandardNumberList, CurrentUser.RoleId, userStateIdList);
                    var rubricTagsByQuestion = _rubricModuleQueryService.GetRubricTagsByQuestionIds(virtualQuestionId.ToString(), acceptableStandards)
                        .FirstOrDefault(x => x.VirtualQuestionID == virtualQuestionId);

                    if (rubricTagsByQuestion != null)
                    {
                        model.Standards = string.Join("<br />", rubricTagsByQuestion.Standards);
                        model.Topics = string.Join("<br />", rubricTagsByQuestion.Topics);
                        model.Skills = string.Join("<br />", rubricTagsByQuestion.Skills);
                        model.Others = string.Join("<br />", rubricTagsByQuestion.Others);
                        model.ItemTags = string.Join("", rubricTagsByQuestion.Customs);
                    }
                }

                model.QtiSchemaId = prop.QtiSchemaId;

                if (model.QtiSchemaId == (int)QTISchemaEnum.UploadComposite)
                {
                    model.IsComplexItem = true;
                }
                else
                {
                    model.EditPointPossibleDirectly = true;
                }
                if (model.QtiSchemaId == (int)QTISchemaEnum.ChoiceMultipleVariable)
                {
                    model.IsSingleCardinality = Util.IsSingleCardinality(xmlContent);
                    if (model.IsSingleCardinality)
                    {
                        model.ItemType = "Single-Select-Variable";
                    }
                }
                if (model.QtiSchemaId == (int)QTISchemaEnum.ExtendedText)
                {
                    var drawable = Util.IsDrawable(xmlContent);
                    if (drawable)
                    {
                        model.ItemType = "Drawing Response";
                    }
                }
                var childQuestions =
                    _virtualQuestionService.Select().Where(x => x.BaseVirtualQuestionId == model.VirtualQuestionId);
                model.HasChildQuestion = childQuestions.Any();

                model.ResponseProcessingTypeId = prop.ResponseProcessingTypeId;

                model.IsCustomItemNaming = _virtualTestService.IsCustomItemNaming(prop.VirtualTestID);
                model.QuestionLabel = prop.QuestionLabel;
                model.QuestionNumber = prop.QuestionNumber;

                if (!string.IsNullOrEmpty(prop.XmlContent))
                {
                    model.ScoringMethod = QtiItemScoringMethodHelper.GetQtiItemScoringMethod(prop.XmlContent, model.QtiSchemaId, model.IsRubricBasedQuestion);
                    model.AttachmentSetting = _districtDecodeService.GetAssessmentArtifactFileTypeGroups(CurrentUser.DistrictId.GetValueOrDefault()).Any()
                        ? Util.ParseAttachmentSetting(prop.XmlContent) : null;
                }

                ViewBag.HasMoreThanOneSection = false;
                var sections = _virtualSectionService.GetVirtualSectionByVirtualTest(model.VirtualTestId);
                if (sections != null && sections.Count > 1)
                {
                    ViewBag.HasMoreThanOneSection = sections.Count > 1;
                    var virtualSection = _virtualSectionQuestionService.GetByVirtualQuestionId(virtualQuestionId);
                    model.VirtualSectionId = virtualSection != null ? virtualSection.VirtualSectionId : 0;
                }
            }

            return PartialView("_VirtualQuestionProperties", model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveVirtualQuestionProperties(VirtualQuestionPropertyViewModel model)
        {
            try
            {
                if (model.PointPossible < 0)
                {
                    return Json(new { Success = false, ErrorMessage = "Point possible should not be less than 0." });
                }

                if ((model.QtiSchemaId == (int)QTISchemaEnum.TextEntry || model.QtiSchemaId == (int)QTISchemaEnum.ExtendedText) && model.ResponseProcessingTypeId == 3 && model.PointPossible > 0)
                {
                    return Json(new { Success = false, Ungraded = true, ErrorMessage = "Point possible of Ungraded Question must be 0!" });
                }

                List<ComplexVirtualQuestionAnswerScoreItemListViewModel> updatedScoreComplexs = null;
                if (model.QtiSchemaId == (int)QTISchemaEnum.UploadComposite)
                {
                    updatedScoreComplexs = ParseXmlPossiblePoints(model.XmlPossiblePoints);
                    if (updatedScoreComplexs != null)
                    {
                        foreach (var score in updatedScoreComplexs)
                        {
                            if ((score.QTISchemaID == (int)QTISchemaEnum.TextEntry || score.QTISchemaID == (int)QTISchemaEnum.ExtendedText)
                                && score.ResponseProcessingTypeId == 3 && score.TestScore > 0)
                                return Json(new { Success = false, Ungraded = true, ErrorMessage = "Point possible of Ungraded Question must be 0!" });
                        }
                    }
                }

                var vq = _virtualQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionID == model.VirtualQuestionId);
                if (vq == null)
                {
                    return Json(new { Success = false, ErrorMessage = "Can not find the question!" });
                }

                var virtualTest = _virtualTestService.GetTestById(vq.VirtualTestID);
                if (virtualTest == null)
                {
                    return Json(new { Success = false, ErrorMessage = "Can not find the test!" });
                }

                //validate ScoreName
                if (!string.IsNullOrEmpty(model.ScoreName) && vq.ScoreName != model.ScoreName)
                {
                    model.ScoreName = model.ScoreName.Trim();
                    var isExistScoreName = _virtualQuestionService.IsExistScoreName(virtualTest.VirtualTestID, model.ScoreName);
                    if (isExistScoreName)
                        return Json(new { Success = false, ErrorMessage = "This score name is already in use. Please use a different score name." });
                }
                int totalPointPossibleFromEachAnswer = 0;
                List<ChoiceVariableVirtualQuestionAnswerScoreItemListViewModel> updatedScoreChoiceVariables = null;
                var isSurvey = virtualTest.DatasetOriginID == (int)DataSetOriginEnum.Survey;
                if (model.QtiSchemaId == (int)QTISchemaEnum.ChoiceMultipleVariable && !string.IsNullOrEmpty(model.XmlPossiblePoints))
                {
                    try
                    {
                        updatedScoreChoiceVariables = ParseXmlPossiblePointsChoiceVariable(model.XmlPossiblePoints);
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new { Success = false, ErrorMessage = ex.Message });
                    }

                    if (updatedScoreChoiceVariables != null)
                    {
                        foreach (var score in updatedScoreChoiceVariables)
                        {
                            if (score.TestScore > 0)
                            {
                                totalPointPossibleFromEachAnswer += score.TestScore;
                                if (score.TestScore > model.PointPossible)
                                {
                                    return Json(new { Success = false, ErrorMessage = "Invidual choices can not exceed Point Possible." });
                                }
                            }
                        }
                    }

                    if (model.PointPossible > totalPointPossibleFromEachAnswer)
                    {
                        return Json(new { Success = false, ErrorMessage = "Student will not be able to earn the maximum point possible on this question based on the current point allocation. You can 1) reduce the total points possible on the item, 2) increase the maximum number of individual choices a student can select, and/or 3) increase the points earned by certain individual choices." });
                    }
                }

                var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
                if (!hasPermission)
                {
                    return Json(new { Success = false, ErrorMessage = "Has no right to update this test!" });
                }

                bool usedToBeAGhostQuestion = (vq.BaseVirtualQuestionId.HasValue && vq.BaseVirtualQuestionId > 0);
                var hasChildQuestion =
                    _virtualQuestionService.Select().Any(x => x.BaseVirtualQuestionId == model.VirtualQuestionId);
                if (vq.IsRubricBasedQuestion == null && !isSurvey)
                {
                    vq.PointsPossible = model.PointPossible;
                }
                if (model.IsGhostQuestion)
                {
                    if (hasChildQuestion)
                    {
                        return Json(new { Success = false, ErrorMessage = "This question has some children!" });
                    }

                    if (!model.BaseQuestionId.HasValue)
                    {
                        return Json(new { Success = false, ErrorMessage = "Please select base question!" });
                    }

                    if (model.BaseQuestionId.Value <= 0)
                    {
                        return Json(new { Success = false, ErrorMessage = "Please select base question!" });
                    }

                    if (model.PointPossible <= 0)
                    {
                        return Json(new { Success = false, ErrorMessage = "Point possible must be greater than 0!" });
                    }

                    vq.BaseVirtualQuestionId = model.BaseQuestionId.Value;
                }
                else
                {
                    if (hasChildQuestion && model.PointPossible > 0)
                    {
                        return Json(new { Success = false, ErrorMessage = "Point possible of Base Question must be 0!" });
                    }

                    vq.BaseVirtualQuestionId = null;
                }

                if (model.IsCustomLevelNaming.HasValue && model.IsCustomLevelNaming.Value)
                {
                    vq.QuestionLabel = model.QuestionLabel;
                    vq.QuestionNumber = model.QuestionNumber;
                }
                var oldScoreName = vq.ScoreName;
                vq.ScoreName = model.ScoreName;
                _virtualQuestionService.Save(vq);

                if (isSurvey && oldScoreName != model.ScoreName)
                    _manageSurveyService.ProcessingScoreNameSurveyTemplate(vq, oldScoreName, model.ItemNumberLabel, model.QtiSchemaId, CurrentUser);

                var sectionQuestion =
                       _virtualSectionQuestionService.Select().Where(
                           x => x.VirtualTestId == vq.VirtualTestID && x.VirtualQuestionId == vq.VirtualQuestionID).
                           FirstOrDefault();

                if (sectionQuestion != null)
                {
                    if (model.IsGhostQuestion)
                    {
                        sectionQuestion.Order = int.MaxValue;
                        _virtualSectionQuestionService.Save(sectionQuestion);
                        _virtualTestService.ReassignBaseVirtualSectionQuestionOrder(vq.VirtualTestID, sectionQuestion.VirtualSectionId);
                    }

                    if (usedToBeAGhostQuestion)
                    {
                        _virtualTestService.ReassignBaseVirtualSectionQuestionOrder(vq.VirtualTestID, sectionQuestion.VirtualSectionId);
                    }
                }

                if (model.VirtualSectionId.Value > 0 && model.VirtualSectionId.Value != sectionQuestion.VirtualSectionId)
                    _virtualTestService.UpdateVirtualSection(model.VirtualQuestionId, model.VirtualSectionId.Value);

                if (model.QtiSchemaId == (int)QTISchemaEnum.Choice || model.QtiSchemaId == (int)QTISchemaEnum.InlineChoice
                    || model.QtiSchemaId == (int)QTISchemaEnum.ExtendedText)
                {
                    var virtualQuestionAnswerScore =
                        _virtualQuestionAnswerScoreService.Select().FirstOrDefault(
                            x => x.VirtualQuestionId == vq.VirtualQuestionID);
                    if (virtualQuestionAnswerScore != null)
                    {
                        virtualQuestionAnswerScore.Score = model.PointPossible;
                        _virtualQuestionAnswerScoreService.Save(virtualQuestionAnswerScore);
                    }
                }

                if (model.QtiSchemaId == (int)QTISchemaEnum.TextEntry)
                {
                    foreach (var virtualQuestionAnswerScore in _virtualQuestionAnswerScoreService.Select().Where(x => x.VirtualQuestionId == model.VirtualQuestionId))
                    {
                        virtualQuestionAnswerScore.Score = model.PointPossible;
                        _virtualQuestionAnswerScoreService.Save(virtualQuestionAnswerScore);
                    }
                }

                if (model.QtiSchemaId == (int)QTISchemaEnum.UploadComposite)
                {
                    string error = string.Empty;
                    if (updatedScoreComplexs != null)
                    {
                        foreach (var score in updatedScoreComplexs)
                        {
                            if (score.QTISchemaID == (int)QTISchemaEnum.ChoiceMultiple)
                            {
                                _virtualTestService.UpdateComplexVirtualQuestionAnswerScores(model.VirtualQuestionId, score.ResponseIdentifier, 0, score.TestScore, model.PointPossible, out error);
                            }
                            else
                            {
                                _virtualTestService.UpdateComplexVirtualQuestionAnswerScores(model.VirtualQuestionId, score.ResponseIdentifier, score.TestScore, score.TestScore, model.PointPossible, out error);
                            }

                            if (!string.IsNullOrWhiteSpace(error))
                            {
                                return
                                    Json(
                                        new
                                        {
                                            Success = false,
                                            ErrorMessage =
                                        "Virtual Question has been updated successfully but uploading virtual test score fail: " + error
                                        }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
                if (model.QtiSchemaId == (int)QTISchemaEnum.ChoiceMultipleVariable && updatedScoreChoiceVariables != null)
                {
                    try
                    {
                        foreach (var score in updatedScoreChoiceVariables)
                        {
                            _virtualQuestionAnswerScoreService.UpdateVirtualQuestionAnswerScore(
                                score.VirtualQuestionAnswerScoreId, score.TestScore);
                        }
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return
                            Json(
                                new
                                {
                                    Success = false,
                                    ErrorMessage =
                                        "Virtual Question has been updated successfully but updating virtual test score fail. Please try again or contact admin. "
                                }, JsonRequestBehavior.AllowGet);
                    }

                }

                var s3VirtualTest = _virtualTestService.CreateS3Object(vq.VirtualTestID);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                if (!s3Result.IsSuccess)
                {
                    return
                        Json(
                            new
                            {
                                Success = false,
                                ErrorMessage =
                            "Virtual Question has been updated successfully but uploading virtual test json file to S3 fail: " +
                            s3Result.ErrorMessage
                            }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { Success = false, ErrorMessage = "There was some error. Can not update Point Possible right now." });
            }

            return Json(new { Success = true });
        }

        [HttpGet]
        public ActionResult LoadQtiItemReview(int qtiItemId, int virtualTestId = 0)
        {
            ViewBag.QtiItemId = qtiItemId;

            //Get the QtiItem
            var qtiItem = _qtiItemService.GetQtiItemById(qtiItemId);

            qtiItem.XmlContent = qtiItem.XmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();

            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            qtiItem.XmlContent = qtiItem.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);

            qtiItem.XmlContent = Util.UpdateS3LinkForItemMedia(qtiItem.XmlContent);
            qtiItem.XmlContent = Util.UpdateS3LinkForPassageLink(qtiItem.XmlContent);

            var model = ItemSetPrinting.TransformXmlContentToHtml(qtiItem.XmlContent, qtiItem.UrlPath, false, _s3Service);

            model.XmlContent = model.XmlContent.RecoverXmlSpecialChars(xmlSpecialCharToken);

            ViewBag.Css = string.Empty;
            string htmlContent = string.Empty;
            string originalXmlContent = qtiItem.XmlContent;

            htmlContent = model.XmlContent;

            htmlContent = Util.ReplaceTagListByTagOl(htmlContent);
            originalXmlContent = Util.ReplaceTagListByTagOl(originalXmlContent);
            htmlContent = XmlUtils.RemoveAllNamespacesPrefix(htmlContent);

            ViewBag.HtmlContent = htmlContent;
            var niceXmlContent = AdjustXmlContent(originalXmlContent.RecoverXmlSpecialChars(xmlSpecialCharToken));
            ViewBag.XmlContent = XmlUtils.RemoveAllNamespacesPrefix(niceXmlContent);

            List<PassageViewModel> passageList = Util.GetPassageList(ViewBag.XmlContent, false, null, false, false, _qti3pPassageService, _passageService);
            var passgeIds = passageList.Select(x => x.QtiRefObjectID).ToArray();

            var passages = _passageService.GetQtiRefObjects(passgeIds);
            foreach (var item in passageList)
            {
                var findExists = passages.FirstOrDefault(x => x.QTIRefObjectID == item.QtiRefObjectID);
                if (findExists != null)
                {
                    item.Name = string.IsNullOrEmpty(findExists.Name) ? "[unnamed]" : findExists.Name;
                }
            }
            ViewBag.PassageList = passageList;

            ViewBag.TestItemMediaPath = string.Empty;
            ViewBag.VirtualTestId = virtualTestId;
            return PartialView("_QtiItemDetail");
        }

        private string AdjustXmlContent(string xmlContent)
        {
            string result = xmlContent.ReplaceWeirdCharacters();
            result = Util.ReplaceTagListByTagOl(result);
            result = ItemSetPrinting.AdjustXmlContentFloatImg(result);
            result = Util.ReplaceVideoTag(result);
            result = result.Replace(" Your browser does not support the video tag.", "");
            return result;
        }

        [HttpGet]
        [UrlReturnDecode]
        public ActionResult LoadVirtualSectionProperties(int virtualSectionId, int virtualTestId)
        {
            VirtualSection vs = _virtualSectionService.GetVirtualSectionById(virtualSectionId);
            var virtualSections = _virtualSectionService.GetVirtualSectionByVirtualTest(virtualTestId);
            var sectionOrders = virtualSections != null ? virtualSections
                .OrderBy(x => x.Order)
                .Select(o => new SelectListItem
                {
                    Text = o.Order.ToString(),
                    Value = o.Order.ToString()
                }).ToList() : new List<SelectListItem>();

            var model = new VirtualSectionTestPropertiesViewModel();
            if (vs != null)
            {
                model = new VirtualSectionTestPropertiesViewModel()
                {
                    VirtualSectionId = vs.VirtualSectionId,
                    VirtualTestId = vs.VirtualTestId,
                    Title = vs.Title,
                    Instruction = vs.Instruction.ReplaceWeirdCharacters(),
                    ConversionSetId = vs.ConversionSetId,
                    AvailableConversionSets = new List<SelectListItem>(),
                    AudioRef = vs.AudioRef,
                    IsTutorialSection = vs.Mode == 1,
                    VirtualSectionOrder = vs.Order,
                    VirtualSectionOrders = sectionOrders
                };
            }
            model.VirtualTestId = virtualTestId;

            return PartialView("_VirtualSectionProperties", model);
        }

        public ActionResult GetConversionSets()
        {
            List<ListItem> data;
            data = _conversionSetService.GetAllConversionSet().Select(x => new ListItem { Name = x.Name, Id = x.ConverstionSetID }).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTestSectionProperties(EditSectionTestPropertiesViewModel model)
        {
            //Get the virtual test
            var virtualTest = _virtualTestService.GetTestById(model.VirtualTestId);
            if (virtualTest == null)
            {
                return Json(new { Success = false, ErrorMessage = "Can not find the test!" });
            }
            //check to avoid modifying ajax parameter bankId)
            var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
            if (!hasPermission)
            {
                return Json(new { Success = false, ErrorMessage = "Has no right to update this test!" });
            }

            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                model.Title = model.Title.Trim();
                if (model.Title.Length > 100)
                {
                    return Json(new { Success = false, ErrorMessage = "Section name is too long, its length can not over 100 charaters!" });
                }
            }
            int virtualSectionId = model.VirtualSectionId;
            try
            {
                if (virtualSectionId == 0)
                {
                    //User update the default section
                    var sections = _virtualSectionService.GetVirtualSectionByVirtualTest(model.VirtualTestId);
                    //If there's no section then create a new real Default section
                    if (sections.Count == 0)
                    {
                        virtualSectionId = FixSectionDataForVirtualTest(model.VirtualTestId);
                    }
                }
                //Get the virtual section test
                var vt = _virtualSectionService.GetVirtualSectionById(virtualSectionId);
                if (vt == null)
                {
                    return Json(new { Success = false, ErrorMessage = "Can not find the section test!" });
                }

                // Update Virtual Test data
                vt.Title = model.Title;
                vt.Instruction = model.XmlContent.ConvertFromUnicodeToWindow1252();
                vt.AudioRef = model.AudioRef;
                vt.Mode = model.IsTutorialSection ? 1 : 0;
                if (model.ConversionSetId > 0)
                {
                    vt.ConversionSetId = model.ConversionSetId;
                }

                _virtualSectionService.Save(vt);

                if (vt.Order != model.VirtualSectionOrder && model.VirtualSectionOrder > 0)
                    _virtualTestService.ChangePositionVirtualSection(model.VirtualTestId, vt.Order, model.VirtualSectionOrder);

                if (!string.IsNullOrEmpty(vt.AudioRef) && vt.AudioRef.ToLower().IndexOf("section_0") > 0)
                {
                    var mediaModel = new MediaModel();
                    var keyName = string.Empty;
                    if (string.IsNullOrEmpty(mediaModel.AUVirtualTestFolder))
                    {
                        keyName = vt.AudioRef.RemoveStartSlash();
                    }
                    else
                    {
                        keyName = string.Format("{0}/{1}", mediaModel.AUVirtualTestFolder.RemoveStartSlash().RemoveEndSlash(), vt.AudioRef.RemoveStartSlash());
                    }
                    var downloadResult = _s3Service.DownloadFile(mediaModel.UpLoadBucketName, keyName);
                    if (downloadResult.IsSuccess)
                    {
                        var audioFileName = Path.GetFileName(vt.AudioRef);
                        vt.AudioRef = string.Format("/SectionMedia/Section_{0}/{1}", vt.VirtualSectionId, audioFileName);

                        if (string.IsNullOrEmpty(mediaModel.AUVirtualTestFolder))
                        {
                            keyName = vt.AudioRef.RemoveStartSlash();
                        }
                        else
                        {
                            keyName = string.Format("{0}/{1}", mediaModel.AUVirtualTestFolder.RemoveStartSlash().RemoveEndSlash(), vt.AudioRef.RemoveStartSlash());
                        }

                        var uploadResult = _s3Service.UploadRubricFile(mediaModel.UpLoadBucketName, keyName,
                            new MemoryStream(downloadResult.ReturnStream));
                        if (!uploadResult.IsSuccess)
                        {
                            return Json(new { Success = false, ErrorMessage = "There was some error. Can not update Section Test Properties right now." });
                        }
                    }

                    _virtualSectionService.Save(vt);
                }

                var s3VirtualTest = _virtualTestService.CreateS3Object(vt.VirtualTestId);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                if (!s3Result.IsSuccess)
                {
                    return
                        Json(
                            new
                            {
                                Success = false,
                                VirtualSectionId = vt.VirtualSectionId,
                                ErrorMessage =
                                "Virtual Section has been updated successfully but uploading virtual test json file to S3 fail: " +
                                s3Result.ErrorMessage
                            }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { Success = false, ErrorMessage = "There was some error. Can not update Section Test Properties right now." });
            }

            return Json(new { Success = true, VirtualSectionId = virtualSectionId, }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SectionAudioUpload(int id, HttpPostedFileBase file)
        {
            var model = new MediaModel
            {
                ID = id,
                PostedFile = file,
                MediaType = MediaType.Audio
            };

            var result = MediaHelper.UploadSectionMedia(model, _s3Service);
            var jsonResult = Json(new { success = result.Success, fileName = model.FileName, url = result.MediaPath, errorMessage = result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            var jsonStringResult = new JavaScriptSerializer().Serialize(jsonResult.Data);

            return Content(jsonStringResult, "text/plain");
        }

        public ActionResult GetSectionAudioUrl(string audioRef)
        {
            var mediaUrl = "";
            audioRef = audioRef.DecodeParameters();

            if (string.IsNullOrEmpty(audioRef))
            {
                mediaUrl = audioRef;
            }
            else if (audioRef.ToLower().StartsWith("http"))
            {
                mediaUrl = audioRef;
            }
            else
            {
                var model = new MediaModel();
                string subdomain = UrlUtil.GenerateS3Subdomain(model.S3Domain, model.UpLoadBucketName);
                var s3AbsolutePath = string.Empty;

                if (string.IsNullOrEmpty(model.AUVirtualTestFolder))
                {
                    s3AbsolutePath = string.Format("{0}/{1}", subdomain.RemoveEndSlash(), audioRef.RemoveStartSlash());
                }
                else
                {
                    s3AbsolutePath = string.Format("{0}/{1}/{2}", subdomain.RemoveEndSlash(), model.AUVirtualTestFolder.RemoveEndSlash().RemoveStartSlash(), audioRef.RemoveStartSlash());
                }
                mediaUrl = s3AbsolutePath;
            }

            return Json(new { Url = mediaUrl }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult QuestionGroupUpload(int id, HttpPostedFileBase file)
        {
            var model = new MediaModel
            {
                ID = id,
                PostedFile = file,
                MediaType = MediaType.Audio
            };

            var result = MediaHelper.UploadQuestionGroupFile(model, _s3Service);

            var jsonResult = Json(new { success = result.Success, fileName = model.FileName, url = result.MediaPath, errorMessage = result.ErrorMessage }, JsonRequestBehavior.AllowGet);
            var jsonStringResult = new JavaScriptSerializer().Serialize(jsonResult.Data);

            return Content(jsonStringResult, "text/plain");
        }

        public ActionResult GetQuestionGroupFileUrl(string fileRef)
        {
            var mediaUrl = "";
            fileRef = fileRef.DecodeParameters();

            if (string.IsNullOrEmpty(fileRef))
            {
                mediaUrl = fileRef;
            }
            else if (fileRef.ToLower().StartsWith("http"))
            {
                mediaUrl = fileRef;
            }
            else
            {
                var model = new MediaModel();
                string subdomain = UrlUtil.GenerateS3Subdomain(model.S3Domain, model.UpLoadBucketName);
                var s3AbsolutePath = string.Empty;
                if (string.IsNullOrEmpty(model.AUVirtualTestFolder))
                {
                    s3AbsolutePath = string.Format("{0}/{1}", subdomain.RemoveEndSlash(), fileRef.RemoveStartSlash());
                }
                else
                {
                    s3AbsolutePath = string.Format("{0}/{1}/{2}", subdomain.RemoveEndSlash(), model.AUVirtualTestFolder.RemoveEndSlash().RemoveStartSlash(), fileRef.RemoveStartSlash());
                }
                mediaUrl = s3AbsolutePath;
            }

            return Json(new { Url = mediaUrl }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddSectionForTest(int virtualTestID)
        {
            //Get the virtual test
            var virtualTest = _virtualTestService.GetTestById(virtualTestID);
            if (virtualTest == null)
            {
                return Json(new { Success = false, ErrorMessage = "Can not find the test!" });
            }
            //check to avoid modifying ajax parameter bankId)
            var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
            if (!hasPermission)
            {
                return Json(new { Success = false, ErrorMessage = "Has no right to update this test!" });
            }

            var section = _virtualSectionService.GetVirtualSectionByVirtualTest(virtualTestID);
            var sectionCount = section.Count;
            if (sectionCount == 0)
            {
                //Check if there's any question of virtual test, so all virtual quesions are now belong to Default Section ( sectionId =0).
                //It's needed to create a new real section with order = 1 for Default Section, then assign all virtual question to this section
                sectionCount = 1;
                var defaultSection = new VirtualSection
                {
                    VirtualTestId = virtualTestID,
                    Order = sectionCount,
                    AudioRef = null,
                    Title = string.Empty,
                    MediaReference = null,
                    VideoRef = null,
                    MediaSource = null,
                    ConversionSetId = null
                };
                _virtualSectionService.Save(defaultSection);
                var sectionQuestions =
                    _virtualSectionQuestionService.Select().Where(
                        x => x.VirtualSectionId == 0 && x.VirtualTestId == virtualTestID);
                foreach (var sectionQuestion in sectionQuestions)
                {
                    sectionQuestion.VirtualSectionId = defaultSection.VirtualSectionId;
                    _virtualSectionQuestionService.Save(sectionQuestion);
                }

                UpdateSectionIdOfQuestionGroupOnSectionDefault(virtualTestID, defaultSection.VirtualSectionId);
            }

            var newSection = new VirtualSection
            {
                VirtualTestId = virtualTestID,
                Order = sectionCount + 1,
                AudioRef = null,
                Title = string.Empty,
                MediaReference = null,
                VideoRef = null,
                MediaSource = null,
                ConversionSetId = null
            };
            _virtualSectionService.Save(newSection);

            var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestID);
            var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

            if (!s3Result.IsSuccess)
            {
                return
                    Json(
                        new
                        {
                            Success = false,
                            ErrorMessage =
                            "Section has been added successfully but uploading virtual test json file to S3 fail: " +
                            s3Result.ErrorMessage
                        }, JsonRequestBehavior.AllowGet);
            }

            var model = BuildVirtualTestViewModelByVirtualTestID(virtualTestID);
            var jsonResult = Json(new { Success = true, newData = model });

            var js = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            var jsonStringResult = js.Serialize(jsonResult.Data);
            return Content(jsonStringResult, "application/json");
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteVirtualSection(int virtualSectionId, int virtualTestId)
        {
            try
            {
                var virtualTest = _virtualTestService.GetTestById(virtualTestId);
                if (virtualTest == null)
                {
                    return Json(new { Success = false, ErrorMessage = "Can not find the test!" });
                }

                var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
                if (!hasPermission)
                {
                    return Json(new { Success = false, ErrorMessage = "Has no right to update this test!" });
                }

                string error;
                _virtualTestService.RemoveVirtualSection(virtualSectionId, out error);

                if (!string.IsNullOrWhiteSpace(error))
                {
                    return Json(new { success = false, ErrorMessage = error }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestId);
                    var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                    if (!s3Result.IsSuccess)
                    {
                        return
                            Json(
                                new
                                {
                                    Success = false,
                                    ErrorMessage =
                                "Virtual Section has been deleted successfully but uploading virtual test json file to S3 fail: " +
                                s3Result.ErrorMessage
                                }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, ErrorMessage = "There was some error, can not delete section right now." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetRubricCategoryByQuestionIds(string virtualQuestionIds)
        {
            if (!string.IsNullOrEmpty(virtualQuestionIds))
            {
                var ids = virtualQuestionIds.ToIntArray();
                var rubricQuestionCategories = _rubricModuleQueryService.GetSelectListRubricQuestionCategoryItem(ids);

                return Json(rubricQuestionCategories, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, ErrorMessage = "There was some error, cannot get Rubric Category right now" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowStandardPopup(string virtualQuestionIds, bool isRubricBasedQuestion = false)
        {
            ViewBag.VirtualQuestionString = virtualQuestionIds;

            if (!string.IsNullOrEmpty(virtualQuestionIds))
            {
                var ids = virtualQuestionIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                ViewBag.IsMultipleQuestion = ids?.Length > 1;
            }
            ViewBag.IsRubricBasedQuestion = isRubricBasedQuestion;

            return PartialView("_ListMasterStandard");
        }

        public ActionResult ShowTagPopup(string virtualQuestionIdString, bool isRubricBasedQuestion = false)
        {
            ViewBag.VirtualQuestionIdString = virtualQuestionIdString;
            ViewBag.RoleId = CurrentUser.RoleId;
            ViewBag.DistrictId = CurrentUser.DistrictId.Value;
            ViewBag.StateId = CurrentUser.StateId;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;

            if (!string.IsNullOrEmpty(virtualQuestionIdString))
            {
                var ids = virtualQuestionIdString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                ViewBag.IsMultipleQuestion = ids?.Length > 1;
            }
            ViewBag.IsRubricBasedQuestion = isRubricBasedQuestion;

            return PartialView("_Tag");
        }

        public ActionResult LoadTagLinkitDefaultPartialView(string virtualQuestionIdString)
        {
            ViewBag.VirtualQuestionIdString = virtualQuestionIdString;
            ViewBag.DistrictId = CurrentUser.DistrictId.Value;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.HasManyQtiItem = false;
            if (!string.IsNullOrEmpty(virtualQuestionIdString))
            {
                string[] ids = virtualQuestionIdString.Split(',');
                if (ids != null && ids.Count() > 1)
                {
                    ViewBag.HasManyQtiItem = true;
                }
            }
            return PartialView("_TagLinkitDefault");
        }

        #region Topics Tag

        public ActionResult GetMutualTopicsOfVirtualQuestions(string virtualQuestionIdString, int? rubricCategoryId = 0)
        {
            var parser = new DataTableParser<Topic>();
            var listResults = new List<Topic>();
            if (!string.IsNullOrEmpty(virtualQuestionIdString))
            {
                var virtualQuestionCount = virtualQuestionIdString.Split(',')?.Count();
                if (virtualQuestionCount == 1)
                {
                    var getVirtualQuestionRubric = _virtualQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionID == int.Parse(virtualQuestionIdString));
                    if (getVirtualQuestionRubric != null && getVirtualQuestionRubric.IsRubricBasedQuestion == true)
                    {
                        var rubricTags = _rubricModuleQueryService.GetRubricCategoryTagSelectListByIds(virtualQuestionIdString.ToIntArray(), TagTypeEnum.Topics, rubricCategoryId).ToList();
                        if (rubricTags?.Count > 0)
                        {
                            var standardsRubricTags = rubricTags.GroupBy(x => new { x.TagID, x.RubricQuestionCategoryID }).Select(y => y.FirstOrDefault()).ToList();
                            var cateCount = rubricCategoryId == 0 ? _rubricModuleQueryService.GetRubicQuestionCategoriesByVirtualQuestionIds(int.Parse(virtualQuestionIdString)).Count() : 1;
                            foreach (var item in standardsRubricTags)
                            {
                                var count = standardsRubricTags.Count(x => x.TagID == item.TagID);
                                if (count >= cateCount)
                                {
                                    listResults.Add(new Topic
                                    {
                                        TopicID = item.TagID,
                                        Name = $"{item.TagName}",
                                        RubricQuestionCategoryID = item.RubricQuestionCategoryID
                                    });
                                }
                            }
                        }
                        listResults = listResults.GroupBy(x => x.TopicID).Select(y => y.FirstOrDefault()).ToList();
                        return Json(parser.Parse(listResults.AsQueryable()), JsonRequestBehavior.AllowGet);
                    }
                }

                var data = _virtualQuestionTopicService.GetMutualTopicsOfVirtualQuestions(virtualQuestionIdString);

                return Json(parser.Parse(data.AsQueryable()), JsonRequestBehavior.AllowGet);
            }
            return Json(parser.Parse(listResults.AsQueryable(), true),
                       JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignTopicTagForVirtualQuestions(string virtualQuestionIdString, string name, List<RubricQuestionCategoryTag> questionCategoryTags)
        {
            var authorizedVirtualQuestionList = new List<VirtualQuestionData>();
            if (!_virtualQuestionService.HasRightToEditVirtualQuestions(virtualQuestionIdString.ParseIdsFromString(),
                    CurrentUser, out authorizedVirtualQuestionList, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = "Fail", errorMessage = "Has no right on one or more questions." }, JsonRequestBehavior.AllowGet);
            }

            name = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(name));
            var status = _virtualQuestionTopicService.AssignTopicTagForVirtualQuestions(virtualQuestionIdString, name);
            if (!string.IsNullOrWhiteSpace(status.Error))
            {
                return Json(new
                {
                    Success = "Fail",
                    errorMessage = status.Error
                },
                                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                foreach (var item in authorizedVirtualQuestionList)
                {
                    var newQuestionCategoryTags = questionCategoryTags.Where(x => x.VirtualQuestionID == item.VirtualQuestionID).ToList();
                    _rubricModuleCommandService.AssignCategoryTagByQuestionIds(new List<VirtualQuestionData> { item }, status.Id, newQuestionCategoryTags, TagTypeEnum.Topics);
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveTopicTagForVirtualQuestions(string virtualQuestionIdString, int topicId, string rubricQuestionCategoryId)
        {
            var authorizedVirtualQuestionList = new List<VirtualQuestionData>();

            if (!_virtualQuestionService.HasRightToEditVirtualQuestions(virtualQuestionIdString.ParseIdsFromString(),
                    CurrentUser, out authorizedVirtualQuestionList, CurrentUser.GetMemberListDistrictId()))
            {
                return JsonResultFail("Has no right on one or more questions.");
            }
            string error = string.Empty;
            int deleteRubricTagCount = 0;
            var rubricQuestionCategoryIds = rubricQuestionCategoryId.ToIntArray();
            if (authorizedVirtualQuestionList.Count > 1)
            {
                var virtualQuestionIds = authorizedVirtualQuestionList.Select(x => x.VirtualQuestionID).ToArray();
                rubricQuestionCategoryIds = _rubricModuleQueryService.GetAllTagsOfRubricByTagId(topicId, virtualQuestionIds).Select(x => x.RubricQuestionCategoryID).ToArray();
            }
            foreach (var item in authorizedVirtualQuestionList)
            {
                if (item.IsRubricBasedQuestion == true)
                {
                    deleteRubricTagCount = _rubricModuleCommandService.DeleteCategoryTagByQuestionIds(item.VirtualQuestionID, topicId, rubricQuestionCategoryIds, TagTypeEnum.Topics);
                }
            }
            if (deleteRubricTagCount == 0)
            {
                error = _virtualQuestionTopicService.RemoveTopicTagForVirtualQuestions(virtualQuestionIdString, topicId);
            }
            if (!string.IsNullOrWhiteSpace(error))
            {
                return JsonResultFail(error);
            }
            else
            {
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Topics Tag

        private ActionResult JsonResultFail(string error)
        {
            return Json(new
            {
                Success = "Fail",
                errorMessage = error
            }, JsonRequestBehavior.AllowGet);
        }

        #region Skills Tag

        public ActionResult GetMutualSkillsVirtualQuestions(string virtualQuestionIdString, int? rubricCategoryId = 0)
        {
            var parser = new DataTableParser<LessonOne>();

            var listResults = new List<LessonOne>();
            if (!string.IsNullOrEmpty(virtualQuestionIdString))
            {
                var virtualQuestionCount = virtualQuestionIdString.Split(',')?.Count();
                if (virtualQuestionCount == 1)
                {
                    var getVirtualQuestionRubric = _virtualQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionID == int.Parse(virtualQuestionIdString));
                    if (getVirtualQuestionRubric != null && getVirtualQuestionRubric.IsRubricBasedQuestion == true)
                    {
                        var rubricTags = _rubricModuleQueryService.GetRubricCategoryTagSelectListByIds(virtualQuestionIdString.ToIntArray(), TagTypeEnum.Skills, rubricCategoryId).ToList();
                        if (rubricTags?.Count > 0)
                        {
                            var standardsRubricTags = rubricTags.GroupBy(x => new { x.TagID, x.RubricQuestionCategoryID }).Select(y => y.FirstOrDefault()).ToList();
                            var cateCount = rubricCategoryId == 0 ? _rubricModuleQueryService.GetRubicQuestionCategoriesByVirtualQuestionIds(int.Parse(virtualQuestionIdString)).Count() : 1;
                            foreach (var item in standardsRubricTags)
                            {
                                var count = standardsRubricTags.Count(x => x.TagID == item.TagID);
                                if (count >= cateCount)
                                {
                                    listResults.Add(new LessonOne
                                    {
                                        LessonOneID = item.TagID,
                                        Name = $"{item.TagName}",
                                        RubricQuestionCategoryID = item.RubricQuestionCategoryID
                                    });
                                }
                            }
                        }
                        listResults = listResults.GroupBy(x => x.LessonOneID).Select(y => y.FirstOrDefault()).ToList();
                        return Json(parser.Parse(listResults.AsQueryable()), JsonRequestBehavior.AllowGet);
                    }
                }
            }

            var data = _virtualQuestionLessonOneService.GetMutualSkillsVirtualQuestions(virtualQuestionIdString);
            return Json(parser.Parse(data.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignSkillTagForVirtualQuestions(string virtualQuestionIdString, string name, List<RubricQuestionCategoryTag> questionCategoryTags)
        {
            //avoid someone modify ajax parameter qtiItemIdString
            var authorizedVirtualQuestionList = new List<VirtualQuestionData>();
            if (!_virtualQuestionService.HasRightToEditVirtualQuestions(virtualQuestionIdString.ParseIdsFromString(),
                    CurrentUser, out authorizedVirtualQuestionList, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = "Fail", errorMessage = "Has no right on one or more questions." }, JsonRequestBehavior.AllowGet);
            }
            name = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(name));
            var status = _virtualQuestionLessonOneService.AssignSkillTagForVirtualQuestions(virtualQuestionIdString, name);
            if (!string.IsNullOrWhiteSpace(status.Error))
            {
                return
                    Json(
                        new
                        {
                            Success = "Fail",
                            errorMessage = status.Error
                        },
                        JsonRequestBehavior.AllowGet);
            }
            else
            {
                foreach (var item in authorizedVirtualQuestionList)
                {
                    var newQuestionCategoryTags = questionCategoryTags.Where(x => x.VirtualQuestionID == item.VirtualQuestionID).ToList();
                    _rubricModuleCommandService.AssignCategoryTagByQuestionIds(new List<VirtualQuestionData> { item }, status.Id, newQuestionCategoryTags, TagTypeEnum.Skills);
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveSkillTagForVirtualQuestions(string virtualQuestionIdString, int lessonOneId, string rubricQuestionCategoryId)
        {
            var authorizedVirtualQuestionList = new List<VirtualQuestionData>();
            if (!_virtualQuestionService.HasRightToEditVirtualQuestions(virtualQuestionIdString.ParseIdsFromString(),
                    CurrentUser, out authorizedVirtualQuestionList, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = "Fail", errorMessage = "Has no right on one or more questions." }, JsonRequestBehavior.AllowGet);
            }
            string error = string.Empty;
            int deleteRubricTagCount = 0;
            var rubricQuestionCategoryIds = rubricQuestionCategoryId.ToIntArray();
            if (authorizedVirtualQuestionList.Count > 1)
            {
                var virtualQuestionIds = authorizedVirtualQuestionList.Select(x => x.VirtualQuestionID).ToArray();
                rubricQuestionCategoryIds = _rubricModuleQueryService.GetAllTagsOfRubricByTagId(lessonOneId, virtualQuestionIds).Select(x => x.RubricQuestionCategoryID).ToArray();
            }
            foreach (var item in authorizedVirtualQuestionList)
            {
                if (item.IsRubricBasedQuestion == true)
                {
                    deleteRubricTagCount = _rubricModuleCommandService.DeleteCategoryTagByQuestionIds(item.VirtualQuestionID, lessonOneId, rubricQuestionCategoryIds, TagTypeEnum.Skills);
                }
            }
            if (deleteRubricTagCount == 0)
            {
                error = _virtualQuestionLessonOneService.RemoveSkillTagForVirtualQuestions(virtualQuestionIdString, lessonOneId);
            }
            if (!string.IsNullOrWhiteSpace(error))
            {
                return JsonResultFail(error);
            }
            else
            {
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Skills Tag

        #region Others Tag

        public ActionResult GetMutualOthersOfVirtualQuestions(string virtualQuestionIdString, int? rubricCategoryId = 0)
        {
            var parser = new DataTableParser<LessonTwo>();

            var listResults = new List<LessonTwo>();
            if (!string.IsNullOrEmpty(virtualQuestionIdString))
            {
                var virtualQuestionCount = virtualQuestionIdString.Split(',')?.Count();
                if (virtualQuestionCount == 1)
                {
                    var getVirtualQuestionRubric = _virtualQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionID == int.Parse(virtualQuestionIdString));
                    if (getVirtualQuestionRubric != null && getVirtualQuestionRubric.IsRubricBasedQuestion == true)
                    {
                        var rubricTags = _rubricModuleQueryService.GetRubricCategoryTagSelectListByIds(virtualQuestionIdString.ToIntArray(), TagTypeEnum.Others, rubricCategoryId).ToList();
                        if (rubricTags?.Count > 0)
                        {
                            var standardsRubricTags = rubricTags.GroupBy(x => new { x.TagID, x.RubricQuestionCategoryID }).Select(y => y.FirstOrDefault()).ToList();
                            var cateCount = rubricCategoryId == 0 ? _rubricModuleQueryService.GetRubicQuestionCategoriesByVirtualQuestionIds(int.Parse(virtualQuestionIdString)).Count() : 1;
                            foreach (var item in standardsRubricTags)
                            {
                                var count = standardsRubricTags.Count(x => x.TagID == item.TagID);
                                if (count >= cateCount)
                                {
                                    listResults.Add(new LessonTwo
                                    {
                                        LessonTwoID = item.TagID,
                                        Name = $"{item.TagName}",
                                        RubricQuestionCategoryID = item.RubricQuestionCategoryID
                                    });
                                }
                            }
                        }
                        listResults = listResults.GroupBy(x => x.LessonTwoID).Select(y => y.FirstOrDefault()).ToList();
                        return Json(parser.Parse(listResults.AsQueryable()), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            var data = _virtualQuestionLessonTwoService.GetMutualOthersOfVirtualQuestions(virtualQuestionIdString);

            return Json(parser.Parse(data.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignOtherTagForVirtualQuestions(string virtualQuestionIdString, string name, List<RubricQuestionCategoryTag> questionCategoryTags)
        {
            var authorizedVirtualQuestionList = new List<VirtualQuestionData>();
            if (!_virtualQuestionService.HasRightToEditVirtualQuestions(virtualQuestionIdString.ParseIdsFromString(),
                    CurrentUser, out authorizedVirtualQuestionList, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = "Fail", errorMessage = "Has no right on one or more questions." }, JsonRequestBehavior.AllowGet);
            }

            name = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(name));
            var status = _virtualQuestionLessonTwoService.AssignOtherTagForVirtualQuestions(virtualQuestionIdString, name);
            if (!string.IsNullOrWhiteSpace(status.Error))
            {
                return
                    Json(
                        new
                        {
                            Success = "Fail",
                            errorMessage = status.Error
                        },
                        JsonRequestBehavior.AllowGet);
            }
            else
            {
                foreach (var item in authorizedVirtualQuestionList)
                {
                    var newQuestionCategoryTags = questionCategoryTags.Where(x => x.VirtualQuestionID == item.VirtualQuestionID).ToList();
                    _rubricModuleCommandService.AssignCategoryTagByQuestionIds(new List<VirtualQuestionData> { item }, status.Id, newQuestionCategoryTags, TagTypeEnum.Others);
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveOtherTagForVirtualQuestions(string virtualQuestionIdString, int lessonTwoId, string rubricQuestionCategoryId)
        {
            var authorizedVirtualQuestionList = new List<VirtualQuestionData>();
            if (!_virtualQuestionService.HasRightToEditVirtualQuestions(virtualQuestionIdString.ParseIdsFromString(),
                    CurrentUser, out authorizedVirtualQuestionList, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = "Fail", errorMessage = "Has no right on one or more questions." }, JsonRequestBehavior.AllowGet);
            }
            string error = string.Empty;
            int deleteRubricTagCount = 0;
            var rubricQuestionCategoryIds = rubricQuestionCategoryId.ToIntArray();
            if (authorizedVirtualQuestionList.Count > 1)
            {
                var virtualQuestionIds = authorizedVirtualQuestionList.Select(x => x.VirtualQuestionID).ToArray();
                rubricQuestionCategoryIds = _rubricModuleQueryService.GetAllTagsOfRubricByTagId(lessonTwoId, virtualQuestionIds).Select(x => x.RubricQuestionCategoryID).ToArray();
            }
            foreach (var item in authorizedVirtualQuestionList)
            {
                if (item.IsRubricBasedQuestion == true)
                {
                    deleteRubricTagCount = _rubricModuleCommandService.DeleteCategoryTagByQuestionIds(item.VirtualQuestionID, lessonTwoId, rubricQuestionCategoryIds, TagTypeEnum.Others);
                }
            }
            if (deleteRubricTagCount == 0)
            {
                error = _virtualQuestionLessonTwoService.RemoveOtherTagForVirtualQuestions(virtualQuestionIdString, lessonTwoId);
            }
            if (!string.IsNullOrWhiteSpace(error))
            {
                return JsonResultFail(error);
            }
            else
            {
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Others Tag

        #region District Tag

        public ActionResult GetMutualAssignedDistrictTagIdString(string virtualQuestionIdString)
        {
            List<ItemTag> itemTags = _virtualQuestionItemTagService.GetMutualItemTagOfVirtualQuestions(virtualQuestionIdString);
            string mutualItemTagIdString = string.Empty;
            if (!string.IsNullOrEmpty(virtualQuestionIdString))
            {
                foreach (var tag in itemTags)
                {
                    mutualItemTagIdString += string.Format(",-{0}-", tag.ItemTagID);
                }
            }
            return Json(new { MutualItemTagIdString = mutualItemTagIdString }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadListDistrictTagAvailablePartialView(string virtualQuestionIdString)
        {
            ViewBag.VirtualQuestionIdString = virtualQuestionIdString;
            ViewBag.DistrictId = CurrentUser.DistrictId.Value;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            return PartialView("_ListDistrictTagAvailable");
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignDistrictTagForVirtualQuestions(string virtualQuestionIdString, string tagIdString, int itemTagCategoryId = 0, List<RubricQuestionCategoryTag> questionCategoryTags = null)
        {
            var authorizedVirtualQuestionList = new List<VirtualQuestionData>();
            var tagIds = tagIdString.ParseIdsFromString();
            if (!_virtualQuestionService.HasRightToEditVirtualQuestions(virtualQuestionIdString.ParseIdsFromString(),
                    CurrentUser, out authorizedVirtualQuestionList, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = "Fail", errorMessage = "Has no right on one or more questions." }, JsonRequestBehavior.AllowGet);
            }

            if (!_itemTagService.HasRightToEditItemTags(tagIds, CurrentUser, CurrentUser.GetMemberListDistrictId()))
            {
                return
                          Json(new { Success = "Fail", errorMessage = "Has no right on one or more tags." }, JsonRequestBehavior.AllowGet);
            }

            string error = _virtualQuestionItemTagService.AssignDistrictTagForVirtualQuestions(virtualQuestionIdString, tagIdString);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return
                    Json(
                        new
                        {
                            Success = "Fail",
                            errorMessage = error
                        },
                        JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (tagIds?.Count > 0)
                {
                    foreach (var tagId in tagIds)
                    {
                        foreach (var virtualQuestion in authorizedVirtualQuestionList)
                        {
                            var newQuestionCategoryTags = questionCategoryTags.Where(x => x.VirtualQuestionID == virtualQuestion.VirtualQuestionID).ToList();
                            _rubricModuleCommandService.AssignCategoryTagByQuestionIds(new List<VirtualQuestionData> { virtualQuestion }, tagId, newQuestionCategoryTags, TagTypeEnum.Customs, itemTagCategoryId);
                        }
                    }
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadListDistrictTagAssignedPartialView(string virtualQuestionIdString)
        {
            ViewBag.VirtualQuestionIdString = virtualQuestionIdString;
            ViewBag.DistrictId = CurrentUser.DistrictId.Value;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            return PartialView("_ListDistrictTagAssigned");
        }

        public ActionResult GetMutualAssignedDistrictTags(string mutualItemTagIdString, string virtualQuestionIds, int? rubricCategoryId = 0)
        {
            List<int> itemTagIdList = mutualItemTagIdString.ParseIdsFromString();
            var parser = new DataTableParser<QtiItemTagAssignViewModel>();
            var listResults = new List<QtiItemTagAssignViewModel>();

            if (!string.IsNullOrEmpty(virtualQuestionIds))
            {
                var virtualQuestionCount = virtualQuestionIds.Split(',')?.Count();
                if (virtualQuestionCount == 1)
                {
                    var getVirtualQuestionRubric = _virtualQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionID == int.Parse(virtualQuestionIds));
                    if (getVirtualQuestionRubric != null && getVirtualQuestionRubric.IsRubricBasedQuestion == true)
                    {
                        var rubricTags = _rubricModuleQueryService.GetRubricCategoryTagSelectListByIds(virtualQuestionIds.ToIntArray(), TagTypeEnum.Customs, rubricCategoryId).ToList();
                        if (rubricTags?.Count > 0)
                        {
                            var standardsRubricTags = rubricTags.GroupBy(x => new { x.TagID, x.RubricQuestionCategoryID }).Select(y => y.FirstOrDefault()).ToList();
                            var cateCount = rubricCategoryId == 0 ? _rubricModuleQueryService.GetRubicQuestionCategoriesByVirtualQuestionIds(getVirtualQuestionRubric.VirtualQuestionID).Count() : 1;
                            foreach (var item in standardsRubricTags)
                            {
                                var count = standardsRubricTags.Count(x => x.TagID == item.TagID);
                                if (count >= cateCount)
                                {
                                    listResults.Add(new QtiItemTagAssignViewModel
                                    {
                                        ItemTagId = item.TagID,
                                        CategoryName = item.TagCategoryName,
                                        TagName = $"{item.TagName}",
                                        RubricQuestionCategoryID = item.RubricQuestionCategoryID
                                    });
                                }
                            }
                        }
                        listResults = listResults.GroupBy(x => x.ItemTagId).Select(y => y.FirstOrDefault()).ToList();
                        return Json(parser.Parse(listResults.AsQueryable()), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            var data = _itemTagService.GetAllItemTag().Where(x => itemTagIdList.Contains(x.ItemTagID)).Select(
               x => new QtiItemTagAssignViewModel
               {
                   ItemTagId = x.ItemTagID,
                   CategoryName = x.Category,
                   TagName = x.Name
               }).ToList();

            return Json(parser.Parse(data.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveDistrictTagForVirtualQuestions(string virtualQuestionIdString, int itemTagId, string rubricQuestionCategoryId)
        {
            var authorizedVirtualQuestionList = new List<VirtualQuestionData>();
            if (!_virtualQuestionService.HasRightToEditVirtualQuestions(virtualQuestionIdString.ParseIdsFromString(),
                    CurrentUser, out authorizedVirtualQuestionList, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = "Fail", errorMessage = "Has no right on one or more questions." }, JsonRequestBehavior.AllowGet);
            }

            if (!_itemTagService.HasRightToEditItemTags(new List<int> { itemTagId }, CurrentUser, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = "Fail", errorMessage = "Has no right on one or more tags." }, JsonRequestBehavior.AllowGet);
            }
            string error = string.Empty;
            int deleteRubricTagCount = 0;
            var rubricQuestionCategoryIds = rubricQuestionCategoryId.ToIntArray();
            if (authorizedVirtualQuestionList.Count > 1)
            {
                var virtualQuestionIds = authorizedVirtualQuestionList.Select(x => x.VirtualQuestionID).ToArray();
                rubricQuestionCategoryIds = _rubricModuleQueryService.GetAllTagsOfRubricByTagId(itemTagId, virtualQuestionIds).Select(x => x.RubricQuestionCategoryID).ToArray();
            }
            foreach (var item in authorizedVirtualQuestionList)
            {
                if (item.IsRubricBasedQuestion == true)
                {
                    deleteRubricTagCount = _rubricModuleCommandService.DeleteCategoryTagByQuestionIds(item.VirtualQuestionID, itemTagId, rubricQuestionCategoryIds, TagTypeEnum.Customs);
                }
            }
            if (deleteRubricTagCount == 0)
            {
                error = _virtualQuestionItemTagService.RemoveDistrictTagForVirtualQuestions(virtualQuestionIdString, itemTagId);
            }
            if (!string.IsNullOrWhiteSpace(error))
            {
                return JsonResultFail(error);
            }
            else
            {
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion District Tag

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult ReorderQuestion(int sourceIndex, int targetIndex, int sourceSectionID, int targetSectionID, int virtualTestID)
        {
            if (sourceSectionID == 0 && targetSectionID == 0)
            {
                var questions = _virtualQuestionService.Select().Where(x => x.VirtualTestID == virtualTestID).Select(x => x.VirtualQuestionID).ToList();
                var virtualSectionQuestion =
                    _virtualSectionQuestionService.Select().Where(x => questions.Contains(x.VirtualQuestionId)).OrderBy(x => x.Order).ToList();
                var sourceVirtualSectionQuestion = virtualSectionQuestion[sourceIndex];
                var temp = sourceVirtualSectionQuestion.Order;
                var targetVirtualSectionQuestion = virtualSectionQuestion[targetIndex];
                sourceVirtualSectionQuestion.Order = targetVirtualSectionQuestion.Order;
                targetVirtualSectionQuestion.Order = temp;

                _virtualSectionQuestionService.Save(sourceVirtualSectionQuestion);
                _virtualSectionQuestionService.Save(targetVirtualSectionQuestion);

                ReAssignQuestionOrderForVirtualQuestionWithSection(virtualTestID);

                var model = BuildVirtualTestViewModelByVirtualTestID(virtualTestID);

                var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestID);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                if (!s3Result.IsSuccess)
                {
                    return
                        Json(
                            new
                            {
                                Success = false,
                                ErrorMessage =
                            "Virtual Test has been updated successfully but uploading json file to S3 fail: " +
                            s3Result.ErrorMessage
                            }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Success = true, newData = model });
            }

            if (sourceSectionID == 0)
            {
                sourceSectionID = FixSectionDataForVirtualTest(virtualTestID);
            }
            if (targetSectionID == 0)
            {
                targetSectionID = FixSectionDataForVirtualTest(virtualTestID);
            }

            var sourceSectionQuestion =
                _virtualSectionQuestionService.GetVirtualSectionQuestionBySection(virtualTestID, sourceSectionID);
            sourceSectionQuestion = ReassignOrderForSectionQuestion(sourceSectionQuestion);

            if (sourceSectionID == targetSectionID)
            {
                var oldPositionItem = sourceSectionQuestion[sourceIndex];
                var newPositionItem = new VirtualSectionQuestion
                {
                    VirtualQuestionId = oldPositionItem.VirtualQuestionId,
                    VirtualSectionId = targetSectionID
                };

                _virtualSectionQuestionService.Delete(oldPositionItem);
                sourceSectionQuestion =
                    _virtualSectionQuestionService.GetVirtualSectionQuestionBySection(virtualTestID, sourceSectionID)
                        .OrderBy(x => x.Order)
                        .ToList();
                sourceSectionQuestion = ReassignOrderForSectionQuestion(sourceSectionQuestion);
                sourceSectionQuestion.Insert(targetIndex, newPositionItem);
                sourceSectionQuestion = ReassignOrderForSectionQuestion(sourceSectionQuestion, false);
                SaveListVirtualSectionQuestion(sourceSectionQuestion);
            }
            else
            {
                var targetSectionQuestion =
                _virtualSectionQuestionService.GetVirtualSectionQuestionBySection(virtualTestID, targetSectionID);
                targetSectionQuestion = ReassignOrderForSectionQuestion(targetSectionQuestion);
                var oldPositionItem = sourceSectionQuestion[sourceIndex];
                var newPositionItem = new VirtualSectionQuestion
                {
                    VirtualQuestionId = oldPositionItem.VirtualQuestionId,
                    VirtualSectionId = targetSectionID
                };
                _virtualSectionQuestionService.Delete(oldPositionItem);
                targetSectionQuestion.Insert(targetIndex, newPositionItem);
                targetSectionQuestion = ReassignOrderForSectionQuestion(targetSectionQuestion, false);
                SaveListVirtualSectionQuestion(targetSectionQuestion);
            }

            ReAssignQuestionOrderForVirtualQuestionWithSection(virtualTestID);

            var model1 = BuildVirtualTestViewModelByVirtualTestID(virtualTestID);

            var s3VirtualTest1 = _virtualTestService.CreateS3Object(virtualTestID);
            var s3Result1 = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest1, _s3Service);

            if (!s3Result1.IsSuccess)
            {
                return
                    Json(
                        new
                        {
                            Success = false,
                            ErrorMessage =
                        "Virtual Test has been updated successfully but uploading json file to S3 fail: " +
                        s3Result1.ErrorMessage
                        }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true, newData = model1 });
        }

        private int FixSectionDataForVirtualTest(int virtualTestID, bool checkExistSection = false)
        {
            var existedSectionList = _virtualSectionService.GetVirtualSectionByVirtualTest(virtualTestID);
            if (checkExistSection == true)
            {
                if (existedSectionList != null && existedSectionList.Any())
                {
                    return 0;
                }
            }
            var newSection = new VirtualSection
            {
                VirtualTestId = virtualTestID,
                Order = 1,
                AudioRef = null,
                Title = string.Empty,
                MediaReference = null,
                VideoRef = null,
                MediaSource = null,
                ConversionSetId = null
            };

            foreach (var section in existedSectionList)
            {
                section.Order = section.Order + 1;
                _virtualSectionService.Save(section);
            }
            _virtualSectionService.Save(newSection);
            var sectionQuestion =
                _virtualTestService.GetVirtualSectionQuestionQtiItem(virtualTestID)
                    .Where(x => x.VirtualSectionID == 0)
                    .ToList();
            foreach (var item in sectionQuestion)
            {
                var virtualQuestionSection = new VirtualSectionQuestion
                {
                    Order = item.Order,
                    VirtualQuestionId = item.VirtualQuestionID,
                    VirtualSectionId = newSection.VirtualSectionId,
                    VirtualSectionQuestionId = item.VirtualSectionQuestionID
                };
                _virtualSectionQuestionService.Save(virtualQuestionSection);
            }

            UpdateSectionIdOfQuestionGroupOnSectionDefault(virtualTestID, newSection.VirtualSectionId);

            return newSection.VirtualSectionId;
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult ReorderSection(int sourceIndex, int targetIndex, int virtualTestID)
        {
            var sectionQuestionList = _virtualTestService.GetVirtualSectionQuestionQtiItem(virtualTestID).ToList();
            var sectionIdList = sectionQuestionList.Select(x => x.VirtualSectionID).Distinct().ToList();
            if (sectionIdList.Any(x => x == 0))
            {
                FixSectionDataForVirtualTest(virtualTestID);
            }

            var sectionList =
                _virtualSectionService.GetVirtualSectionByVirtualTest(virtualTestID).OrderBy(x => x.Order).ToList();
            ReassignOrderForSection(sectionList);
            sectionList[sourceIndex].Order = targetIndex + 1;
            sectionList[targetIndex].Order = sourceIndex + 1;
            foreach (var virtualSection in sectionList)
            {
                _virtualSectionService.Save(virtualSection);
            }

            ReAssignQuestionOrderForVirtualQuestionWithSection(virtualTestID);

            var model = BuildVirtualTestViewModelByVirtualTestID(virtualTestID);

            var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestID);
            var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

            if (!s3Result.IsSuccess)
            {
                return
                    Json(
                        new
                        {
                            Success = false,
                            ErrorMessage =
                        "Virtual Test has been updated successfully but uploading json file to S3 fail: " +
                        s3Result.ErrorMessage
                        }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true, newData = model });
        }

        private void ReAssignQuestionOrderForVirtualQuestionWithSection(int virtualTestID)
        {
            _virtualTestService.ReassignVirtualQuestionOrder(virtualTestID);
        }

        private void ReassignOrderForSection(List<VirtualSection> listVirtualSection)
        {
            for (int index = 0; index < listVirtualSection.Count; index++)
            {
                listVirtualSection[index].Order = index + 1;
            }
        }

        private List<VirtualSectionQuestion> ReassignOrderForSectionQuestion(List<VirtualSectionQuestion> listVirtualSectionQuestions, bool reOrderList = true)
        {
            if (reOrderList)
            {
                listVirtualSectionQuestions = listVirtualSectionQuestions.OrderBy(x => x.Order).ToList();
            }
            for (int index = 0; index < listVirtualSectionQuestions.Count; index++)
            {
                listVirtualSectionQuestions[index].Order = index + 1;
            }
            listVirtualSectionQuestions = listVirtualSectionQuestions.OrderBy(x => x.Order).ToList();
            return listVirtualSectionQuestions;
        }

        private void SaveListVirtualSectionQuestion(List<VirtualSectionQuestion> listVirtualSectionQuestions)
        {
            foreach (var virtualSectionQuestion in listVirtualSectionQuestions)
            {
                _virtualSectionQuestionService.Save(virtualSectionQuestion);
            }
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult MoveVirtualSectionQuestion(int sourceIndex, int targetIndex, int sourceSectionID, int targetSectionID, int virtualTestID, int virtualQuestionID
            , int? baseVirtualQuestionId, bool? numberingItemBySection
            , int? sourceQuestionGroupId, int? targetQuestionGroupId, int? targetQuestionGroupIndex)
        {
            numberingItemBySection = numberingItemBySection ?? false;
            string errorMessage = string.Empty;
            try
            {
                var targetSectionQuestions = _virtualSectionQuestionService.GetVirtualSectionQuestionBySection(virtualTestID, targetSectionID).OrderBy(x => x.Order).ToList();
                VirtualSectionQuestion targetSectionQuestion = null;
                VirtualQuestionData targetQuestion = null;

                if (targetSectionQuestions.Count > targetIndex)
                {
                    targetSectionQuestion = targetSectionQuestions[targetIndex];
                    targetQuestion = _virtualQuestionService.Select().Where(
                        x => x.VirtualQuestionID == targetSectionQuestion.VirtualQuestionId).FirstOrDefault();
                }

                //check if the source question is a ghost question or not
                if (baseVirtualQuestionId.HasValue && baseVirtualQuestionId.Value > 0)
                {
                    //Check if moving within the same base question's section or not
                    if (sourceSectionID != targetSectionID)
                    {
                        return
                            Json(
                                new
                                {
                                    Success = false,
                                    ErrorMessage = "All criteria-based scoring questions must fall directly below their base question."
                                }, JsonRequestBehavior.AllowGet);
                    }
                    //A user can only move a ghost item within the group of ghost items associated with its' base item
                    //get the target question in the section
                    if (targetQuestion != null)
                    {
                        if (!targetQuestion.BaseVirtualQuestionId.HasValue || (targetQuestion.BaseVirtualQuestionId.HasValue && targetQuestion.BaseVirtualQuestionId.Value != baseVirtualQuestionId.Value))
                        {
                            return
                                Json(
                                    new
                                    {
                                        Success = false,
                                        ErrorMessage =
                                    "All criteria-based scoring questions must fall directly below their base question."
                                    }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                //Not allow user to move a non-ghost question inside a group of ghost questions
                if (targetQuestion != null && targetQuestion.BaseVirtualQuestionId.HasValue && targetQuestion.BaseVirtualQuestionId > 0)
                {
                    if (!baseVirtualQuestionId.HasValue || baseVirtualQuestionId.Value <= 0)
                    {
                        return
                                Json(
                                    new
                                    {
                                        Success = false,
                                        ErrorMessage =
                                    "Only criteria-based scoring questions can fall directly below a base question."
                                    }, JsonRequestBehavior.AllowGet);
                    }
                }

                _virtualTestService.MoveManyVirtualQuestionGroup(virtualQuestionID, virtualQuestionID.ToString(), sourceQuestionGroupId, targetSectionID, targetQuestionGroupId, targetQuestionGroupIndex.GetValueOrDefault());
                _virtualTestService.MoveVirtualSectionQuestion(virtualTestID, sourceIndex, sourceSectionID, targetIndex, targetSectionID);

                _virtualTestService.ReassignBaseVirtualSectionQuestionOrder(virtualTestID, sourceSectionID);
                if (targetSectionID != sourceSectionID)
                {
                    _virtualTestService.ReassignBaseVirtualSectionQuestionOrder(virtualTestID, targetSectionID);
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                errorMessage = ex.Message;
            }

            var model = BuildVirtualTestViewModelByVirtualTestID(virtualTestID, numberingItemBySection.GetValueOrDefault());

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestID);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                if (!s3Result.IsSuccess)
                {
                    return
                        Json(
                            new
                            {
                                Success = false,
                                ErrorMessage =
                            "Virtual Test has been updated successfully but uploading json file to S3 fail: " +
                            s3Result.ErrorMessage
                            }, JsonRequestBehavior.AllowGet);
                }
            }

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                return Json(new { Success = true, newData = model });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = errorMessage, newData = model });
            }
        }

        #region Import Item

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.Testdesign)]
        public ActionResult ShowImportItemsFromLibrary(int virtualTestId, int? districtId)
        {
            var xliAccess = _authorizeItemLibService.GetXliFunctionAccess(CurrentUser.Id, CurrentUser.RoleId,
                CurrentUser.DistrictId ?? 0);

            ViewBag.XliFunctionAccess = xliAccess;
            List<ListItem> virtualSections = GetDisplayedSections(virtualTestId);

            ViewBag.VirtualTestId = virtualTestId;
            ViewBag.IsPublisher = CurrentUser.IsPublisher();
            ViewBag.DistrictId = (!districtId.HasValue || CurrentUser.IsCorrectDistrict(districtId.Value)) ? districtId : CurrentUser.DistrictId;
            ViewBag.VirtualSections = virtualSections;

            return View("_ImportItemsFromLibrary");
        }

        public ActionResult ShowSelectSectionDialog(int virtualTestId, string qtiItemIdString, string onPopup)
        {
            ViewBag.VirtualTestId = virtualTestId;
            var sections = GetDisplayedSections(virtualTestId);

            ViewBag.HasMoreThanOneSection = sections.Count > 1;
            ViewBag.UniqueVirtualSectionId = sections[0].Id;
            ViewBag.OnPopup = onPopup ?? string.Empty;

            ViewBag.HasQuestionGroups = _questionGroupService.HasQuestionGroup(virtualTestId);
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                int countRubricBasedQuestion = 0;

                var qtiItemIds = qtiItemIdString.ParseIdsFromStringAsNullableInt();
                var newQtiItemIds = _virtualQuestionService.Select().Where(x => qtiItemIds.Contains(x.QTIItemID) && x.IsRubricBasedQuestion == true).Select(x => x.QTIItemID).Distinct().ToList();
                countRubricBasedQuestion = newQtiItemIds?.Count ?? 0;
                ViewBag.CountRubricBasedQuestion = countRubricBasedQuestion > 0;
            }
            else
            {
                ViewBag.CountRubricBasedQuestion = false;
            }
            return PartialView("_SelectSection");
        }

        public ActionResult GetSectionItem(int virtualSectionId, int virtualTestId)
        {
            var parser = new DataTableParser<VirtualSectionQuestionItemListViewModel>();
            List<VirtualSectionQuestionItemListViewModel> data = new List<VirtualSectionQuestionItemListViewModel>();
            var virtualSection = _virtualSectionService.GetVirtualSectionById(virtualSectionId);

            data =
                     _virtualSectionQuestionService.Select().Where(
                         en => en.VirtualSectionId == virtualSectionId && en.VirtualTestId == virtualTestId)
                         .Select(x => new VirtualSectionQuestionItemListViewModel
                         {
                             Order = x.Order,
                             //QuestionOrder = x.Order,//Now display Order in section not in the whole test
                             QtiItemId = x.QtiItemId,
                             Name = AdjustXmlContent(x.XmlContent),
                             QuestionOrder = x.QuestionOrder,
                             SectionOrder = virtualSection == null ? 1 : virtualSection.Order
                         }).ToList();

            foreach (var item in data)
            {
                item.Name = XmlUtils.RemoveAllNamespacesPrefix(item.Name);
                item.Name = Util.UpdateS3LinkForItemMedia(item.Name);
                item.Name = Util.UpdateS3LinkForPassageLink(item.Name);
            }

            var jsonResult = Json(parser.Parse(data.AsQueryable()), JsonRequestBehavior.AllowGet);
            var js = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            var jsonStringResult = js.Serialize(jsonResult.Data);
            return Content(jsonStringResult, "application/json");
        }

        [HttpGet]
        public ActionResult GetSections(int virtualTestId)
        {
            List<ListItem> data = new List<ListItem>();

            if (virtualTestId <= 0)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            data = GetDisplayedSections(virtualTestId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<ListItem> GetDisplayedSections(int virtualTestId)
        {
            List<ListItem> data = new List<ListItem>();

            data = _virtualSectionService.GetVirtualSectionByVirtualTest(virtualTestId).OrderBy(x => x.Order)
                .Select(x => new ListItem
                {
                    Name = string.IsNullOrWhiteSpace(x.Title) ? "Section " + x.Order.ToString() : x.Title,
                    Id = x.VirtualSectionId
                }).ToList();

            if (data.Count == 0)
            {
                data.Add(new ListItem
                {
                    Id = 0,
                    Name = "Default Section"
                });
            }

            if (_virtualSectionQuestionService.Select().Any(x => x.VirtualSectionId == 0 && x.VirtualTestId == virtualTestId)
                && !data.Any(x => x.Id == 0))
            {
                data.Insert(0, new ListItem
                {
                    Id = 0,
                    Name = "Default Section"
                });
            }
            return data;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddQtiItemsToVirtualSection(int virtualTestId, string qtiItemIdString, int virtualSectionId, bool isCloned, int? questionGroupId)
        {
            try
            {
                //Get the virtual test
                var virtualTest = _virtualTestService.GetTestById(virtualTestId);
                if (virtualTest == null)
                {
                    return Json(new { Success = false, errorMessage = "Can not find the test!" });
                }

                //check to avoid modifying ajax parameter
                var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
                if (!hasPermission)
                {
                    return Json(new { Success = false, errorMessage = "Has no right to update this test!" });
                }

                if (!_vulnerabilityService.HasRightToAddQtiItems(CurrentUser, qtiItemIdString.ParseIdsFromString()))
                {
                    return Json(new { Success = false, errorMessage = "Has no right on one or more items selected to added." });
                }
                //[LNKT-64906] prevent add new question when exists test inprocess.
                if (_qTIOnlineTestSessionService.HasExistTestInProgress(virtualTest.VirtualTestID))
                {
                    return Json(new { success = false, errorMessage = TextConstants.EXIST_TEST_IN_PROGRESS }, JsonRequestBehavior.AllowGet);
                }

                var qtiItemIdStringNew = qtiItemIdString;
                if (!isCloned)
                {
                    var qtiItemIds = qtiItemIdString.ParseIdsFromStringAsNullableInt();
                    var newQtiItemIds = _virtualQuestionService.Select().Where(x => qtiItemIds.Contains(x.QTIItemID) && x.IsRubricBasedQuestion == true).Select(x => x.QTIItemID).Distinct().ToList();
                    qtiItemIds.RemoveAll(x => newQtiItemIds.Contains(x));

                    qtiItemIdStringNew = string.Join(",", qtiItemIds);
                }
                string errorMessage = string.Empty;
                var result = Util.AddQtiItemsToVirtualSection(virtualTestId, qtiItemIdStringNew, virtualSectionId, isCloned,
                                                              CurrentUser.UserName,
                                                              CurrentUser.Id, CurrentUser.StateId ?? 0,
                                                              _virtualTestService, _qtiBankService, _qtiGroupService,
                                                              _qtiItemService, out errorMessage, questionGroupId, _s3Service);
                _virtualTestService.ReassignVirtualQuestionOrder(virtualTestId);

                if (result)
                {
                    return CreateJsonResult("Success", string.Empty);
                }
                else
                {
                    return CreateJsonResult("Fail", errorMessage);
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return CreateJsonResult("Fail",
                    string.Format("Fail to add item(s) to virtual section, error detail:" + ex.Message));
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddQti3ItemsToVirtualSection(int virtualTestId, string qtiItemIdString, int virtualSectionId, bool? is3pUpload, int? questionGroupId)
        {
            try
            {
                var virtualTest = _virtualTestService.GetTestById(virtualTestId);
                if (virtualTest == null)
                {
                    return Json(new { Success = false, errorMessage = "Can not find the test!" });
                }
                //check to avoid modifying ajax parameter
                var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
                if (!hasPermission)
                {
                    return Json(new { Success = false, errorMessage = "Has no right to update this test!" });
                }

                //[LNKT-64906] prevent add new question when exists test inprocess.
                if (_qTIOnlineTestSessionService.HasExistTestInProgress(virtualTest.VirtualTestID))
                {
                    return Json(new { Success = false, errorMessage = TextConstants.EXIST_TEST_IN_PROGRESS });
                }

                var qtiBank = _qtiBankService.GetDefaultQTIBank(CurrentUser.UserName, CurrentUser.Id);
                var itemSet = _qtiGroupService.GetDefaultQTIGroup(CurrentUser.Id, qtiBank.QtiBankId, virtualTest.Name);
                var qtiGroupID = itemSet.QtiGroupId;

                var order = _virtualTestService.GetMaxQuestionOrderInVirtualTest(virtualTestId);
                var orderInSection = _virtualTestService.GetMaxQuestionOrderInVirtualSection(virtualTestId, virtualSectionId);
                if (virtualSectionId <= 0) orderInSection = order;

                var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
                var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;

                var listQtiitemId = new List<int>();
                var qtiItemIDs = qtiItemIdString.Split(',').Select(id => {
                    var qtiItem3ID = 0;
                    int.TryParse(id, out qtiItem3ID);
                    return qtiItem3ID;
                }).Where(id => id != 0);

                var importItems = _qtiItemService.GetQti3pItemByIds(qtiItemIDs).ToList();
                var isInvalidMultipart = importItems.Any(qti => qti.QTISchemaID == (int)QtiSchemaEnum.MultiPart && !Util.IsValidMultiPartXmlContent(qti.XmlContent));
                if (isInvalidMultipart)
                    return Json(new { Success = false, errorMessage = TextConstants.IMPORT_INVALID_MULTIPART });

                foreach (var qtiItem3ID in qtiItemIDs)
                {
                    var qtiItemID = 0;

                    if (is3pUpload.HasValue && is3pUpload.Value)
                    {
                        var qtiItem = _qtiItemService.ConvertFrom3pItemUploadedToItem(qtiItem3ID, qtiGroupID, CurrentUser.Id, true,
                        LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName,
                        LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder);
                        qtiItemID = qtiItem == null ? 0 : qtiItem.QTIItemID;

                        UpdateItemPassage(qtiItemID);
                    }
                    else
                    {
                        var qti3pItem = _qtiItemService.GetQti3pItemById(qtiItem3ID);
                        if (qti3pItem != null)
                        {
                            if (qti3pItem.From3pUpload)
                            {
                                var qtiItem = _qtiItemService.ConvertFrom3pItemUploadedToItem(qtiItem3ID, qtiGroupID,
                                CurrentUser.Id, true,
                                LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName,
                                LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder);
                                qtiItemID = qtiItem == null ? 0 : qtiItem.QTIItemID;

                                UpdateItemPassage(qtiItemID);
                            }
                            else
                            {
                                var qtiItem = _qtiItemService.TMAddItemFromLibrary(qtiItem3ID, qtiGroupID,
                                    CurrentUser.Id);
                                qtiItemID = qtiItem == null ? 0 : qtiItem.QTIItemID;
                            }
                        }
                    }

                    if (qtiItemID <= 0) continue;
                    listQtiitemId.Add(qtiItemID);
                }

                if (listQtiitemId.Any())
                {
                    var qtiitemstr = string.Join(",", listQtiitemId);
                    _virtualTestService.AddQtiItemToVirtualSection(virtualTestId, virtualSectionId, CurrentUser.StateId ?? 0, qtiitemstr, questionGroupId);
                    _virtualTestService.ReassignVirtualQuestionOrder(virtualTestId);

                    var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestId);
                    var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                    if (s3Result.IsSuccess) return CreateJsonResult("Success", string.Empty);
                    return CreateJsonResult("false",
                        "Virtual Test has been imported successfully but uploading json file to S3 fail: " +
                        s3Result.ErrorMessage);
                }

                return CreateJsonResult("Success", string.Empty);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return CreateJsonResult("Fail",
                    string.Format("Fail to add item(s) to virtual section, error detail:" + ex.Message));
            }
        }

        private void UpdateItemPassage(int qtiItemId)
        {
            try
            {
                var qtiItem = _qtiItemService.GetQtiItemById(qtiItemId);
                XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
                qtiItem.XmlContent = qtiItem.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
                qtiItem.XmlContent = qtiItem.XmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();
                List<PassageViewModel> passageList = Util.GetPassageList(qtiItem.XmlContent, false);
                if (passageList != null)
                {
                    _qtiItemService.UpdateItemPassage(qtiItemId, passageList.Select(x => x.QtiRefObjectID).ToList(),
                        passageList.Select(x => x.RefNumber).ToList());
                }
            }
            catch
            {
            }
        }

        private JsonResult CreateJsonResult(string success, string message)
        {
            return Json(new { Success = success, errorMessage = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion Import Item

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteVirtualQuestion(int virtualQuestionId, int virtualTestId)
        {
            //Get the virtual test
            var virtualTest = _virtualTestService.GetTestById(virtualTestId);
            if (virtualTest == null)
            {
                return Json(new { success = false, ErrorMessage = "Can not find the test!" });
            }
            //check to avoid modifying ajax parameter bankId)
            var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
            if (!hasPermission)
            {
                return Json(new { success = false, ErrorMessage = "Has no right to update this test!" });
            }

            if (virtualQuestionId > 0)
            {
                string errorMessage;
                var canRemoveQuestion = _virtualTestService.CanRemoveVirtualQuestion(virtualQuestionId.ToString(), out errorMessage);
                if (!canRemoveQuestion)
                    return Json(new { success = false, ErrorMessage = errorMessage });
            }

            try
            {
                string error;
                var sectionQuesion =
                    _virtualSectionQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionId == virtualQuestionId && x.VirtualTestId == virtualTestId);
                _virtualTestService.RemoveVirtualQuestion(virtualQuestionId, out error);
                if (sectionQuesion != null)
                {
                    _virtualTestService.ReassignVirtualSectionQuestionOrder(virtualTestId,
                                                                            sectionQuesion.VirtualSectionId);
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    return Json(new { success = false, ErrorMessage = error }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestId);
                    var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                    if (!s3Result.IsSuccess)
                    {
                        return
                            Json(
                                new
                                {
                                    Success = false,
                                    ErrorMessage =
                                "Virtual Question has been deleted successfully but uploading virtual test json file to S3 fail: " +
                                s3Result.ErrorMessage
                                }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, ErrorMessage = "There was some error, can not delete question right now." }, JsonRequestBehavior.AllowGet);
            }
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteVirtualQuestionSeleted(string lstVirtualQuestionIds, int virtualTestId)
        {
            //Get the virtual test
            var virtualTest = _virtualTestService.GetTestById(virtualTestId);
            if (virtualTest == null)
            {
                return Json(new { success = false, ErrorMessage = "Can not find the test!" });
            }
            //check to avoid modifying ajax parameter bankId)
            var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
            if (!hasPermission)
            {
                return Json(new { success = false, ErrorMessage = "Has no right to update this test!" });
            }

            if (!string.IsNullOrEmpty(lstVirtualQuestionIds))
            {
                string errorMessage;
                var canRemoveQuestion = _virtualTestService.CanRemoveVirtualQuestion(lstVirtualQuestionIds, out errorMessage);
                if (!canRemoveQuestion)
                    return Json(new { success = false, ErrorMessage = errorMessage });
            }
            string[] arr = lstVirtualQuestionIds.Split(',');
            var arrError = new List<string>();
            var dc = new Dictionary<string, string>();
            foreach (var s in arr)
            {
                int questionId = 0;
                if (int.TryParse(s, out questionId))
                {
                    try
                    {
                        string error;
                        var sectionQuesion = _virtualSectionQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionId == questionId && x.VirtualTestId == virtualTestId);
                        _virtualTestService.RemoveVirtualQuestion(questionId, out error);
                        if (sectionQuesion != null)
                        {
                            _virtualTestService.ReassignVirtualSectionQuestionOrder(virtualTestId, sectionQuesion.VirtualSectionId);
                        }
                        if (!string.IsNullOrWhiteSpace(error))
                        {
                            var obj = _virtualQuestionService.GetQuestionDataById(questionId);
                            if (dc.ContainsKey(error))
                            {
                                dc[error] = obj.QuestionOrder + ", " + dc[error];
                            }
                            else
                            {
                                dc.Add(error, obj.QuestionOrder.ToString());
                            }
                        }
                    }
                    catch (Exception)
                    {
                        var obj = _virtualQuestionService.GetQuestionDataById(questionId);
                        string orderQuestion = obj != null ? obj.QuestionOrder.ToString() : string.Empty;
                        string message = "There was some error, can not delete question right now.";
                        if (dc.ContainsKey(message))
                        {
                            dc[message] = orderQuestion + ", " + dc[message];
                        }
                        else
                        {
                            dc.Add(message, orderQuestion);
                        }
                    }
                }
            }
            if (arr.Any())
            {
                try
                {
                    var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestId);
                    var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);
                    if (!s3Result.IsSuccess)
                    {
                        dc.Add(string.Format("Virtual Question has been deleted successfully but uploading virtual test json file to S3 fail: {0}", s3Result.ErrorMessage), string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    dc.Add("Virtual Question has been deleted successfully but uploading virtual test json file to S3 fail.", string.Empty);
                }
            }

            if (dc.Count > 0)
            {
                if (arr.Count() == 1)
                {
                    return Json(new { success = false, ErrorMessage = dc.First().Key }, JsonRequestBehavior.AllowGet);
                }
                foreach (KeyValuePair<string, string> pair in dc)
                {
                    if (string.IsNullOrEmpty(pair.Value))
                    {
                        arrError.Add(pair.Key);
                    }
                    else
                    {
                        string[] arrOrderQuestion = pair.Value.Split(',');
                        Array.Sort(arrOrderQuestion);

                        string tmp = string.Empty;
                        foreach (string s in arrOrderQuestion)
                        {
                            if (!string.IsNullOrEmpty(s.Trim()))
                            {
                                tmp += ", " + s.Trim();
                            }
                        }
                        if (tmp.Length > 2)
                            tmp = tmp.Substring(2);

                        arrError.Add(string.Format("({0}) {1}", tmp, pair.Key));
                    }
                }
                return Json(new { success = false, ErrorMessage = arrError.ToArray() }, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LoadComplexAnswerScores(int virtualQuestionId)
        {
            return PartialView("_ComplexItemPossiblePoints", virtualQuestionId);
        }

        public ActionResult GetComplexVirtualQuestionAnswerScoresTable(int virtualQuestionId)
        {
            var result = GetComplexVirtualQuestionAnswerScores(virtualQuestionId);
            var parser = new DataTableParser<ComplexVirtualQuestionAnswerScoreItemListViewModel>();
            return Json(parser.Parse(result.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult LoadResponseDeclarations(int virtualQuestionId)
        {
            var result = GetComplexVirtualQuestionAnswerScores(virtualQuestionId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<ComplexVirtualQuestionAnswerScoreItemListViewModel> GetComplexVirtualQuestionAnswerScores(int virtualQuestionId)
        {
            var data =
                _virtualTestService.GetComplexVirtualQuestionAnswerScores(virtualQuestionId).Select(
                    x => new { x.ResponseIdentifier, x.SubPointsPossible, x.QiSubCorrectAnswer, x.QiSubPointsPossible, x.QTISchemaID, x.ResponseProcessingTypeId }).
                    Distinct().OrderBy(x => x.ResponseIdentifier).ToList();
            var result = new List<ComplexVirtualQuestionAnswerScoreItemListViewModel>();
            if (data != null)
            {
                result = data.Select(x => new ComplexVirtualQuestionAnswerScoreItemListViewModel
                {
                    ResponseIdentifier = x.ResponseIdentifier,
                    CorrectAnswer = x.QiSubCorrectAnswer.ConvertFromWindow1252ToUnicode(),
                    QtiItemScore = x.QiSubPointsPossible,
                    TestScore = string.IsNullOrEmpty(x.SubPointsPossible) ? 0 : int.Parse(x.SubPointsPossible),
                    QTISchemaID = x.QTISchemaID,
                    ResponseProcessingTypeId = x.ResponseProcessingTypeId
                }).ToList();
            }
            return result;
        }

        private List<ComplexVirtualQuestionAnswerScoreItemListViewModel> ParseXmlPossiblePoints(string xml)
        {
            var result = new List<ComplexVirtualQuestionAnswerScoreItemListViewModel>();
            XmlDocument doc = ServiceUtil.LoadXmlDocument(xml);
            var nodes = doc.GetElementsByTagName("PossiblePoint");

            foreach (XmlNode node in nodes)
            {
                var item = new ComplexVirtualQuestionAnswerScoreItemListViewModel();
                item.ResponseIdentifier = node.Attributes["ResponseIdentifier"].Value;
                item.TestScore = int.Parse(node.Attributes["Score"].Value);
                item.QTISchemaID = int.Parse(node.Attributes["QtiSchemaId"].Value);
                var responseProcessingTypeId = node.Attributes["ResponseProcessingTypeId"].Value;
                if (!string.IsNullOrWhiteSpace(responseProcessingTypeId))
                    item.ResponseProcessingTypeId = int.Parse(responseProcessingTypeId);
                result.Add(item);
            }
            return result;
        }

        public ActionResult LoadListItemsFromLibrary(int? virtualTestId)
        {
            ViewBag.HasQuestionGroups = _questionGroupService.HasQuestionGroup(virtualTestId ?? 0);
            return PartialView("_ListItemsFromLibrary");
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult MoveSection(int sourceIndex, int targetIndex, int virtualTestID, bool? numberingItemBySection)
        {
            numberingItemBySection = numberingItemBySection ?? false;
            string errorMessage = string.Empty;
            try
            {
                _virtualTestService.ChangePositionVirtualSection(virtualTestID, sourceIndex + 1, targetIndex + 1);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                errorMessage = ex.Message;
            }

            var model = BuildVirtualTestViewModelByVirtualTestID(virtualTestID, numberingItemBySection.GetValueOrDefault());

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestID);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                if (!s3Result.IsSuccess)
                {
                    return
                        Json(
                            new
                            {
                                Success = false,
                                ErrorMessage =
                            "Virtual Test has been updated successfully but uploading json file to S3 fail: " +
                            s3Result.ErrorMessage
                            }, JsonRequestBehavior.AllowGet);
                }
            }
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                return Json(new { Success = true, newData = model });
            }
            else
            {
                return Json(new { Success = false, newData = model });
            }
        }

        [HttpGet]
        public ActionResult LoadEditQtiItem(int qtiItemId)
        {
            var qtiItem = _qtiItemService.GetQtiItemById(qtiItemId);
            ViewBag.QtiItemId = qtiItemId;
            ViewBag.XmlContent = string.Empty;
            if (qtiItem != null)
            {
                var xmlContent = AdjustXmlContent(qtiItem.XmlContent);
                ViewBag.XmlContent = XmlUtils.RemoveAllNamespacesPrefix(xmlContent);
            }
            ViewBag.HasTest = true;
            ViewBag.QtiItemGroupId = qtiItem.QTIGroupID;

            return PartialView("_EditQtiItem");
        }

        [HttpGet]
        public ActionResult LoadDetailQtiItem(int qtiItemId)
        {
            var qtiItem = _qtiItemService.GetQtiItemById(qtiItemId);
            ViewBag.QtiItemId = qtiItemId;
            ViewBag.XmlContent = string.Empty;

            if (qtiItem != null)
            {
                var xmlContent = AdjustXmlContent(qtiItem.XmlContent);
                ViewBag.XmlContent = XmlUtils.RemoveAllNamespacesPrefix(xmlContent);
            }

            ViewBag.XmlContent = Util.UpdateS3LinkForItemMedia(ViewBag.XmlContent);
            ViewBag.XmlContent = Util.UpdateS3LinkForPassageLink(ViewBag.XmlContent);
            ViewBag.HasTest = true;
            return PartialView("_QtiItemDetail");
        }

        public ActionResult ChooseSectionToAddQTI3(int virtualTestId, string onPopup)
        {
            ViewBag.VirtualTestId = virtualTestId;
            var sections = GetDisplayedSections(virtualTestId);

            ViewBag.HasMoreThanOneSection = sections.Count > 1;
            ViewBag.UniqueVirtualSectionId = sections[0].Id;
            ViewBag.OnPopup = onPopup ?? string.Empty;
            ViewBag.HasQuestionGroups = _questionGroupService.HasQuestionGroup(virtualTestId);
            return PartialView("_ChooseSectionToAddQTI3");
        }

        public ActionResult GetBaseQuestions(int virtualTestId, int virtualQuestionId)
        {
            List<ListItem> data = new List<ListItem>();
            var baseQuestions = _virtualTestService.GetBaseQuestions(virtualTestId, virtualQuestionId);
            if (baseQuestions != null)
            {
                data = baseQuestions.Select(x => new ListItem { Id = x.VirtualQuestionId, Name = x.Order.ToString() }).ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult MoveManyVirtualSectionQuestions(int sourceIndex, int targetIndex, int sourceSectionID, int targetSectionID, int virtualTestID, int virtualQuestionID
            , string selectedVirtualQuestionIds, int? baseVirtualQuestionId, bool? numberingItemBySection
            , int? sourceQuestionGroupId, int? targetQuestionGroupId, int? targetQuestionGroupIndex)
        {
            numberingItemBySection = numberingItemBySection ?? false;
            if (selectedVirtualQuestionIds == null)
            {
                selectedVirtualQuestionIds = string.Empty;
            }

            List<int> virtualQuestionIdList = ServiceUtil.GetIdListFromIdString(selectedVirtualQuestionIds);
            if (!virtualQuestionIdList.Contains(virtualQuestionID))
            {
                virtualQuestionIdList.Add(virtualQuestionID);
                selectedVirtualQuestionIds = string.Join(",", virtualQuestionIdList);
            }

            var targetSectionQuestions = _virtualSectionQuestionService.GetVirtualSectionQuestionBySection(virtualTestID, targetSectionID).OrderBy(x => x.Order).ToList();
            VirtualSectionQuestion targetSectionQuestion = null;
            VirtualQuestionData targetQuestion = null;

            if (targetSectionQuestions.Count > targetIndex)
            {
                targetSectionQuestion = targetSectionQuestions[targetIndex];
                targetQuestion = _virtualQuestionService.Select().Where(
                    x => x.VirtualQuestionID == targetSectionQuestion.VirtualQuestionId).FirstOrDefault();
            }

            //check if the source question is a ghost question or not
            //note: ghost question is also called as criteria-based question
            if (baseVirtualQuestionId.HasValue && baseVirtualQuestionId.Value > 0)
            {
                //Check if moving within the same base question's section or not
                if (sourceSectionID != targetSectionID)
                {
                    return
                        Json(
                            new
                            {
                                Success = false,
                                ErrorMessage = "All criteria-based scoring questions must fall directly below their base question."
                            }, JsonRequestBehavior.AllowGet);
                }
                //A user can only move a ghost item within the group of ghost items associated with its' base item
                //get the target question in the section
                if (targetQuestion != null)
                {
                    if (!targetQuestion.BaseVirtualQuestionId.HasValue || (targetQuestion.BaseVirtualQuestionId.HasValue && targetQuestion.BaseVirtualQuestionId.Value != baseVirtualQuestionId.Value))
                    {
                        return
                            Json(
                                new
                                {
                                    Success = false,
                                    ErrorMessage =
                                "All criteria-based scoring questions must fall directly below their base question."
                                }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            //Not allow user to move a non-ghost question inside a group of ghost questions
            if (targetQuestion != null && targetQuestion.BaseVirtualQuestionId.HasValue && targetQuestion.BaseVirtualQuestionId > 0)
            {
                if (!baseVirtualQuestionId.HasValue || baseVirtualQuestionId.Value <= 0)
                {
                    return
                            Json(
                                new
                                {
                                    Success = false,
                                    ErrorMessage =
                                "Only criteria-based scoring questions can fall directly below a base question."
                                }, JsonRequestBehavior.AllowGet);
                }
            }

            string errorMessage = string.Empty;
            try
            {
                _virtualTestService.MoveManyVirtualQuestionGroup(virtualTestID, selectedVirtualQuestionIds, sourceQuestionGroupId, targetSectionID, targetQuestionGroupId, targetQuestionGroupIndex.GetValueOrDefault());
                _virtualTestService.MoveManyVirtualSectionQuestion(virtualTestID, selectedVirtualQuestionIds, sourceIndex, sourceSectionID, targetIndex, targetSectionID, out errorMessage);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                errorMessage = ex.Message;
            }

            var model = BuildVirtualTestViewModelByVirtualTestID(virtualTestID, numberingItemBySection.GetValueOrDefault());
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestID);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                if (!s3Result.IsSuccess)
                {
                    return
                        Json(
                            new
                            {
                                Success = false,
                                ErrorMessage =
                            "Virtual Test has been updated successfully but uploading json file to S3 fail: " +
                            s3Result.ErrorMessage
                            }, JsonRequestBehavior.AllowGet);
                }
            }

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                return Json(new { Success = true, newData = model });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = errorMessage, newData = model });
            }
        }

        public ActionResult GetSearchAllOthersTags(string tagSearch, string virtualQuestionIdString)
        {
            tagSearch = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(tagSearch));
            var data = _lessonTwoService.GetLessonTwosBySearchText(tagSearch, virtualQuestionIdString, ContaintUtil.VirtualQuestion);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSearchAllTopicTags(string tagSearch, string virtualQuestionIdString)
        {
            tagSearch = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(tagSearch));
            var data = _topicService.GetTopicsBySearchText(tagSearch, virtualQuestionIdString, ContaintUtil.VirtualQuestion);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSearchAllSkillTags(string tagSearch, string virtualQuestionIdString)
        {
            tagSearch = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(tagSearch));
            var data = _lessonOneService.GetLessonOnesBySearchText(tagSearch, virtualQuestionIdString, ContaintUtil.VirtualQuestion);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LoadChoiceVariableAnswerScores(int virtualQuestionId)
        {
            return PartialView("_MultipleChoiceVariablePossiblePoints", virtualQuestionId);
        }

        public ActionResult GetChoiceVariableVirtualQuestionAnswerScoresTable(int virtualQuestionId)
        {
            var result = GetChoiceVariableVirtualQuestionAnswerScores(virtualQuestionId);
            var parser = new DataTableParser<ChoiceVariableVirtualQuestionAnswerScoreItemListViewModel>();
            return Json(parser.Parse(result.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        private List<ChoiceVariableVirtualQuestionAnswerScoreItemListViewModel> GetChoiceVariableVirtualQuestionAnswerScores(int virtualQuestionId)
        {
            var data =
                _virtualTestService.GetChoiceVariableVirtualQuestionAnswerScores(virtualQuestionId).ToList();
            var result = new List<ChoiceVariableVirtualQuestionAnswerScoreItemListViewModel>();
            if (data != null)
            {
                result = data.Select(x => new ChoiceVariableVirtualQuestionAnswerScoreItemListViewModel
                {
                    VirtualQuestionAnswerScoreId = x.VirtualQuestionAnswerScoreId,
                    Answer = x.Answer,
                    QtiItemScore = x.QtiItemScore,
                    TestScore = x.Score,
                }).ToList();
            }
            return result;
        }

        private List<ChoiceVariableVirtualQuestionAnswerScoreItemListViewModel> ParseXmlPossiblePointsChoiceVariable(string xml)
        {
            var result = new List<ChoiceVariableVirtualQuestionAnswerScoreItemListViewModel>();
            XmlDocument doc = ServiceUtil.LoadXmlDocument(xml);
            var nodes = doc.GetElementsByTagName("PossiblePoint");

            foreach (XmlNode node in nodes)
            {
                var item = new ChoiceVariableVirtualQuestionAnswerScoreItemListViewModel();
                try
                {
                    item.VirtualQuestionAnswerScoreId = int.Parse(node.Attributes["VirtualQuestionAnswerScoreId"].Value);
                    item.TestScore = int.Parse(node.Attributes["Score"].Value);
                }
                catch
                {
                    throw new Exception("Point must be an integer.");
                }

                result.Add(item);
            }
            return result;
        }

        public ActionResult TestBranching(TestBranchingViewModel model)
        {
            var virtualTestViewModel = BuildVirtualTestViewModelByVirtualTestID(model.VirtualTestId);
            model.VirtualSectionList = virtualTestViewModel.VirtualSectionList;

            var virtualQuestionBranchings = _virtualQuestionBranchingService.GetVirtualQuestionBranching(model.VirtualQuestionId, model.VirtualTestId);
            model.VirtualQuestionBranchingList = virtualQuestionBranchings;
            //Remove Current Question. ( One question or Group Question ).
            var objSectionCurrentQuestion = model.VirtualSectionList.First(s => s.SectionQuestionQtiItemList.Any(q => q.VirtualQuestionID == model.VirtualQuestionId));
            int totalQuestionInGroup = 0;
            int iCurrentGroupID = 0;
            int totalGroups = 0;
            int iTotalQuestionSection = 0;
            //Fix case QuestionGroup.VirtualSectionID difference VirtualSection.VirtualSectionID per Test.
            var lstVirtualQuestionGroup = _questionGroupService.GetListVirtualQuestionGroupByVirtualTestId(model.VirtualTestId);
            bool numberingItemBySection = _virtualTestService.GetNumberQuestionsByTestId(model.VirtualTestId);
            for (int sectionIndex = 0; sectionIndex < model.VirtualSectionList.Count; sectionIndex++)
            {
                if (sectionIndex > 0)
                {
                    iTotalQuestionSection += model.VirtualSectionList[sectionIndex - 1].SectionQuestionQtiItemList.Count;
                }
                if (numberingItemBySection)
                {
                    iCurrentGroupID = 0;
                    totalGroups = 0;
                    totalQuestionInGroup = 0;
                }

                foreach (var question in model.VirtualSectionList[sectionIndex].SectionQuestionQtiItemList)
                {
                    if (question.QuestionGroupID.HasValue && lstVirtualQuestionGroup.Count > 0)
                    {
                        if (iCurrentGroupID != question.QuestionGroupID)
                        {
                            iCurrentGroupID = question.QuestionGroupID.GetValueOrDefault();
                            totalGroups += 1;
                        }
                        totalQuestionInGroup += 1;
                        var currentGroup = lstVirtualQuestionGroup.FirstOrDefault(o => o.QuestionGroupID == question.QuestionGroupID);
                        //sectionList.QuestionGroupList.FirstOrDefault(g => g.QuestionGroupID == question.QuestionGroupID);
                        if (currentGroup != null)
                        {
                            var listQuestionPerGroup = model.VirtualSectionList[sectionIndex].SectionQuestionQtiItemList.Where(o => o.QuestionGroupID == currentGroup.QuestionGroupID)
                            .OrderBy(o => o.QuestionOrder).ToList();

                            question.GroupOrder = question.QuestionOrder - totalQuestionInGroup + totalGroups;
                            int indexQuestionInGroup = listQuestionPerGroup.IndexOf(question);
                            question.DisplayOrder = string.Format("{0}{1}", question.GroupOrder, ConvertValue.GetAlphabets(indexQuestionInGroup));
                            if (numberingItemBySection && iTotalQuestionSection > 0)
                            {
                                question.DisplayOrder = string.Format("{0}{1}", question.GroupOrder - iTotalQuestionSection, ConvertValue.GetAlphabets(indexQuestionInGroup));
                                question.GroupOrder = question.GroupOrder - iTotalQuestionSection;
                            }
                        }
                    }
                    else
                    {
                        question.DisplayOrder = question.QuestionOrder.ToString();
                        if (totalGroups > 0)
                        {
                            question.DisplayOrder = (question.QuestionOrder - totalQuestionInGroup + totalGroups).ToString();
                        }
                        if (numberingItemBySection && iTotalQuestionSection > 0) //Support NumberQuestion
                        {
                            question.DisplayOrder = (question.QuestionOrder - iTotalQuestionSection).ToString();
                            if (totalGroups > 0)
                            {
                                question.DisplayOrder = (question.QuestionOrder - totalQuestionInGroup + totalGroups - iTotalQuestionSection).ToString();
                            }
                        }
                    }
                }
            }
            return PartialView("_TestBranching", model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveTestBranching(string jsonString)
        {
            var virtualQuestionBranchings =
                (List<VirtualQuestionBranching>)
                    Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString,
                        typeof(List<VirtualQuestionBranching>));

            _virtualQuestionBranchingService.UpdateVirtualQuestionBranching(virtualQuestionBranchings);

            return Json(1);
        }

        #region Quick View

        [HttpGet]
        public ActionResult LoadItemListAnswerKeyQuickView(int? virtualTestId)
        {
            ViewBag.VirtualTestId = virtualTestId;
            return PartialView("_ItemListAnswerKeyQuickView");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetVirtualQuestionAnswerView(int? virtualTestId)
        {
            var parser = new DataTableParser<VirtualQuestionViewModel>();
            var emptyResult = new List<VirtualQuestionViewModel>().AsQueryable();
            if (!virtualTestId.HasValue || virtualTestId.Value == 0)
                return Json(parser.Parse(emptyResult));

            var result = _virtualTestService.GetVirtualQuestionWithCorrectAnswer(virtualTestId.Value).Select(x => new VirtualQuestionViewModel()
            {
                QTIItemID = x.QTIItemID,
                QuestionOrder = x.QuestionOrder,
                CorrectAnswer = x.CorrectAnswer,
                NumberOfChoices = 0,
                PointsPossible = x.PointsPossible,
                QTISchemaID = x.QTISchemaID,
                AnswerIdentifiers = x.AnswerIdentifiers,
                VirtualQuestionId = x.VirtualQuestionID,
                IsRubricBasedQuestion = x.IsRubricBasedQuestion
            }).ToList();

            return Json(parser.Parse(result.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateInput(false)]
        public ActionResult SaveAnswerKey(string answerKeysXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(answerKeysXml);
            //parse data xml sent from client
            var deSerializer = new XmlSerializer(typeof(VirtualQuestionsXml), "http://www.imsglobal.org/xsd/imsqti_v2p0");
            VirtualQuestionsXml answerKeys = null;
            bool isOK = true;
            Dictionary<int, string> errorSaving = new Dictionary<int, string>();
            try
            {
                using (TextReader reader = new StringReader(answerKeysXml))
                {
                    answerKeys = deSerializer.Deserialize(reader) as VirtualQuestionsXml;
                }
            }
            catch
            {
                isOK = false;
            }
            if (isOK)
            {
                if (answerKeys != null)
                {
                    if (answerKeys.AnswerKeys != null)
                    {
                        string error;
                        int questionOrder = 0;

                        var isHavingStudentTakeTest = _qtiItemService.IsHavingStudentTakeTest(answerKeys.AnswerKeys.Select(x => (x.QtiItemId, x.ExtendedText)));
                        if (isHavingStudentTakeTest)
                        {
                            string errorMessage = "You are attempting to change question(s) which students have already tested against. This is not permitted.";
                            return Json(new { success = "false", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
                        }

                        foreach (var answerKey in answerKeys.AnswerKeys)
                        {
                            try
                            {
                                _qtiItemService.SaveAnswerKey(int.Parse(answerKey.QtiItemId),
                                                                          answerKey.CorrectAnswer,
                                                                          int.Parse(answerKey.NumberOfChoices),
                                                                          null,
                                                                          bool.Parse(answerKey.ExtendedText),
                                                                          out error,
                                                                          out questionOrder);
                                if (!string.IsNullOrWhiteSpace(error))
                                {
                                    errorSaving.Add(int.Parse(answerKey.QtiItemId), error);
                                }
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    try
                                    {
                                        Util.UploadMultiVirtualTestJsonFileToS3(int.Parse(answerKey.QtiItemId), _virtualQuestionService, _virtualTestService, _s3Service);
                                    }
                                    catch (Exception ex)
                                    {
                                        PortalAuditManager.LogException(ex);
                                        errorSaving.Add(int.Parse(answerKey.QtiItemId), string.Format("Question Order {0} is saved successfully but uploading associated virtual test(s) to S3 failed. Detail error", questionOrder, ex.Message));
                                    }
                                }

                                int qtiSchemaId = int.Parse(answerKey.QtiSchemaId);
                                int pointPossible = int.Parse(answerKey.Points);
                                int virtualQuestionId = int.Parse(answerKey.VirtualQuestionId);

                                //Get the virtual question
                                var vq = _virtualQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionID == virtualQuestionId);
                                if (vq == null)
                                    continue;

                                //Get the virtual test
                                var virtualTest = _virtualTestService.GetTestById(vq.VirtualTestID);
                                if (virtualTest == null)
                                    continue;

                                //check to avoid modifying ajax parameter bankId)
                                var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
                                if (!hasPermission)
                                    continue;

                                bool usedToBeAGhostQuestion = (vq.BaseVirtualQuestionId.HasValue && vq.BaseVirtualQuestionId > 0);
                                var hasChildQuestion =
                                    _virtualQuestionService.Select().Any(x => x.BaseVirtualQuestionId == virtualQuestionId);

                                //Update Point Possible
                                if (vq.IsRubricBasedQuestion == null)
                                {
                                    vq.PointsPossible = pointPossible;
                                }
                                if (hasChildQuestion && pointPossible > 0)
                                    continue;

                                vq.BaseVirtualQuestionId = null;

                                _virtualQuestionService.Save(vq);
                                //if this is a ghost question, append it to the ghost question list of its base question
                                var sectionQuestion = _virtualSectionQuestionService.Select().Where(x => x.VirtualTestId == vq.VirtualTestID && x.VirtualQuestionId == vq.VirtualQuestionID).FirstOrDefault();
                                if (sectionQuestion != null)
                                    if (usedToBeAGhostQuestion)
                                        _virtualTestService.ReassignBaseVirtualSectionQuestionOrder(vq.VirtualTestID, sectionQuestion.VirtualSectionId);

                                //Flash does not update VirtualQuestionAnswerScore for QTISchemaId = 3 (Multiselect)
                                if (qtiSchemaId == (int)QTISchemaEnum.Choice || qtiSchemaId == (int)QTISchemaEnum.InlineChoice || qtiSchemaId == (int)QTISchemaEnum.ExtendedText)
                                {
                                    //Update VirtualQuestionAnswerScore according to
                                    var virtualQuestionAnswerScore =
                                        _virtualQuestionAnswerScoreService.Select().FirstOrDefault(
                                            x => x.VirtualQuestionId == vq.VirtualQuestionID);
                                    if (virtualQuestionAnswerScore != null)
                                    {
                                        virtualQuestionAnswerScore.Score = pointPossible;
                                        _virtualQuestionAnswerScoreService.Save(virtualQuestionAnswerScore);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                PortalAuditManager.LogException(ex);
                                errorSaving.Add(int.Parse(answerKey.QtiItemId), ex.Message);
                            }
                        }
                    }
                }
            }
            if (!errorSaving.Any())
            {
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string errorMessage = "There was some error when saving, please try again later.";
                foreach (var error in errorSaving)
                {
                    errorMessage += "<br>" + error.Value;
                }
                return Json(new { success = "false", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [UrlReturnDecode]
        [AjaxOnly]
        public ActionResult InsertDefaultMultipleChoices(int numberOfChoice, string correctAnswer, int virtualTestID, int? selectedVirtualSectionId)
        {
            string error;

            try
            {
                selectedVirtualSectionId = selectedVirtualSectionId ?? 0;

                var virtualTest = _virtualTestService.GetTestById(virtualTestID);
                var qtiBank = _qtiBankService.GetDefaultQTIBank(CurrentUser.UserName, CurrentUser.Id);
                var itemSet = _qtiGroupService.GetDefaultQTIGroup(CurrentUser.Id, qtiBank.QtiBankId, virtualTest.Name);

                correctAnswer = correctAnswer.ToUpper();
                var qtiItemId = _qtiItemService.InsertDefaultMultipleChoices(CurrentUser.Id, itemSet.QtiGroupId, numberOfChoice, correctAnswer, out error);
                if (!string.IsNullOrWhiteSpace(error) || qtiItemId == 0)
                    error = "There was some error when saving, please try again later. " + error;
                else if (_qTIOnlineTestSessionService.HasExistTestInProgress(virtualTest.VirtualTestID))
                {
                    error = TextConstants.EXIST_TEST_IN_PROGRESS;
                }
                else
                {
                    int virtualSectionId = 0;
                    var sections = _virtualSectionService.GetVirtualSectionByVirtualTest(virtualTestID);
                    if (sections == null)
                    {
                        sections = new List<VirtualSection>();
                    }
                    if (sections.Count > 1)
                    {
                        //There's more than one section -> add to the selected section
                        if (virtualTestID > 0 && selectedVirtualSectionId.HasValue && selectedVirtualSectionId.Value >= 0)
                        {
                            virtualSectionId = selectedVirtualSectionId.Value;
                        }
                    }
                    else
                    {
                        if (sections.Count == 1)
                        {
                            //The only one virtual section
                            virtualSectionId = sections[0].VirtualSectionId;
                        }
                        else if (sections.Count == 0)
                        {
                            virtualSectionId = 0;
                        }
                    }

                    var result = Util.AddQtiItemsToVirtualSection(virtualTestID, qtiItemId.ToString(),
                                                          virtualSectionId, false,
                                                          CurrentUser.UserName,
                                                          CurrentUser.Id, CurrentUser.StateId ?? 0,
                                                          _virtualTestService, null, null,
                                                          null, out error, null, _s3Service);

                    if (!string.IsNullOrWhiteSpace(error) || !result)
                        error = "There was some error when saving, please try again later. " + error;
                    else
                        _virtualTestService.ReassignVirtualQuestionOrder(virtualTestID);
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                error = "There was some error when saving, please try again later.";
            }
            if (string.IsNullOrWhiteSpace(error))
            {
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = "false", ErrorMessage = error }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Quick View

        public ActionResult LoadListItemsFromLibraryUpload(int? virtualTestId)
        {
            ViewBag.HasQuestionGroups = _questionGroupService.HasQuestionGroup(virtualTestId ?? 0);
            return PartialView("_ListItemsFromLibraryUpload");
        }

        public ActionResult ShowPassageItem3pForm(int virtualTestId, int qti3pPassageId, string matchItemXml, int? qti3pSource)
        {
            List<ListItem> virtualSections = GetDisplayedSections(virtualTestId);

            var model = new ShowPassageFormViewModel()
            {
                Qti3pPassageID = qti3pPassageId,
                Qti3pSourceID = qti3pSource ?? 0,
                ShownItemXml = matchItemXml.DecodeParameters(),
                VirtualTestId = virtualTestId,
                IsPublisher = CurrentUser.IsPublisher(),
                DistrictId = CurrentUser.DistrictId ?? 0,
                VirtualSections = virtualSections,
                Is3pItem = true
            };

            return PartialView("_PassageItem", model);
        }

        public ActionResult ShowPassageItemForm(int virtualTestId, int qtiRefObjectID, string matchItemXml)
        {
            List<ListItem> virtualSections = GetDisplayedSections(virtualTestId);
            var model = new ShowPassageFormViewModel()
            {
                QTIRefObjectID = qtiRefObjectID,
                ShownItemXml = matchItemXml.DecodeParameters(),
                VirtualTestId = virtualTestId,
                IsPublisher = CurrentUser.IsPublisher(),
                DistrictId = CurrentUser.DistrictId ?? 0,
                VirtualSections = virtualSections
            };
            return PartialView("_PassageItem", model);
        }

        public ActionResult IsBranchBySectionScore(int virtualTestId)
        {
            var isBranchBySectionScore = _virtualSectionService.IsBranchBySectionScore(virtualTestId);
            return Json(BaseResponseModel<bool>.InstanceSuccess(isBranchBySectionScore, "", ""), JsonRequestBehavior.AllowGet);
        }

        public ActionResult InitSectionBranching()
        {
            return PartialView("_SectionBranching");
        }

        public ActionResult LoadSectionBranching(int virtualTestId)
        {
            //TODO:
            var lstSectionBranching = _virtualSectionService.GetVirtualSectionBranchingByVirtualtestId(virtualTestId);
            var data = MapSectionBranching(lstSectionBranching, virtualTestId);
            var parser = new DataTableParser<SectionBranchingViewModel>();
            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateBranchingMethod(UpdateBranchingMethodDto updateBranchingMethod)
        {
            var updateStatus = _virtualSectionService.UpdateBranchingMethod(updateBranchingMethod);
            return Json(updateStatus, JsonRequestBehavior.AllowGet);
        }

        private List<SectionBranchingViewModel> MapSectionBranching(List<VirtualSectionBranching> lst, int virtualtestId)
        {
            if (lst != null && lst.Count > 0)
            {
                var listSectionBranching = new List<SectionBranchingViewModel>();
                var listVirtualSection = _virtualSectionService.GetVirtualSectionByVirtualTest(virtualtestId);
                foreach (var item in lst)
                {
                    if (item.TargetVirtualSectionID == 0)
                    {
                        listSectionBranching.Add(new SectionBranchingViewModel()
                        {
                            HighScore = (int)item.HighScore,
                            LowScore = (int)item.LowScore,
                            VirtualSectionBranchingID = item.VirtualSectionBranchingID,
                            TargetVirtualSection = "End Test",
                            TestletPath = BuildCustomSectionName(item.TestletPath, listVirtualSection)
                        });
                    }
                    else
                    {
                        var obj = listVirtualSection.FirstOrDefault(o => o.VirtualSectionId == item.TargetVirtualSectionID.GetValueOrDefault());
                        if (obj != null)
                        {
                            listSectionBranching.Add(new SectionBranchingViewModel()
                            {
                                HighScore = (int)item.HighScore,
                                LowScore = (int)item.LowScore,
                                VirtualSectionBranchingID = item.VirtualSectionBranchingID,
                                TargetVirtualSection = string.Format("[{0}_{1}]", obj.Order, obj.Title),
                                TestletPath = BuildCustomSectionName(item.TestletPath, listVirtualSection)
                            });
                        }
                    }
                }
                return listSectionBranching;
            }
            return new List<SectionBranchingViewModel>();
        }

        private string BuildCustomSectionName(string sectionList, List<VirtualSection> lstVirtualSection)
        {
            string strResult = string.Empty;
            string[] arr = sectionList.Split('-');
            if (arr.Length > 0)
            {
                int virualsectionId = 0;
                foreach (var item in arr)
                {
                    int.TryParse(item.Trim(), out virualsectionId);
                    var obj = lstVirtualSection.FirstOrDefault(o => o.VirtualSectionId == virualsectionId);
                    if (obj != null)
                    {
                        strResult += "-[" + obj.Order + "_" + obj.Title.Trim() + "]";
                    }
                }
            }
            else
            {
                int virualsectionId = 0;
                int.TryParse(sectionList.Trim(), out virualsectionId);
                var obj = lstVirtualSection.FirstOrDefault(o => o.VirtualSectionId == virualsectionId);
                if (obj != null)
                {
                    strResult = "[" + obj.Order + "_" + obj.Title + "]";
                }
            }
            if (strResult.Length > 0 && strResult.Substring(0, 1).Equals("-"))
            {
                return strResult.Substring(1);
            }
            return strResult;
        }

        public ActionResult GetSectionDropDownByVirtualtestId(int virtualtestId)
        {
            var listVirtualSection = _virtualSectionService.GetVirtualSectionByVirtualTest(virtualtestId);
            var data = listVirtualSection
                .Select(x => new ListItem { Id = x.VirtualSectionId, Name = "[" + x.Order + "_" + x.Title + "] " })
                .OrderBy(o => o.Name)
                .ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertSectionPath(List<SectionPathInsertViewModel> sectionPaths)
        {
            if (sectionPaths != null)
            {
                string errMessage = ValidationSectionPath(sectionPaths);
                if (!string.IsNullOrEmpty(errMessage))
                {
                    return Json(new { success = false, ErrMessage = errMessage });
                }
                var newVirtualSectionPaths = new List<VirtualSectionBranching>();
                foreach (var sectionPath in sectionPaths)
                {
                    var virtualSectionBranching = new VirtualSectionBranching()
                    {
                        HighScore = sectionPath.MaxValue,
                        LowScore = sectionPath.MinValue,
                        TargetVirtualSectionID = sectionPath.TargetId,
                        TestletPath = sectionPaths[0].sectionselected.Replace(',', '-'),
                        VirtualTestID = sectionPaths[0].VirtualTestId,
                        CreatedUserID = CurrentUser.Id,
                        CreatedDateTime = DateTime.UtcNow,
                        IsBranchBySectionScore = sectionPath.IsBranchBySectionScore
                    };
                    newVirtualSectionPaths.Add(virtualSectionBranching);
                }

                var result = _virtualSectionService.InsertListVirtualSectionBranching(newVirtualSectionPaths);
                if (result != null)
                    return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        private string ValidationSectionPath(List<SectionPathInsertViewModel> obj)
        {
            string errorMessage = string.Empty;

            if (obj == null || obj.Count == 0)
            {
                errorMessage = "Please select a target section and input score.";
                return errorMessage;
            }
            else
            {
                foreach (var item in obj)
                {
                    if (item.MinValue > item.MaxValue)
                    {
                        errorMessage = "Min score must be equal or smaller than max score.";
                        return errorMessage;
                    }
                }
            }

            //TODO: check invalid SectionTarget & SectionPath.
            string[] arrSectionIds = obj[0].sectionselected.Split(',');
            if (arrSectionIds.Length > 0 && obj.Any(o => arrSectionIds.Contains(o.TargetId.ToString())))
            {
                errorMessage = "The target section must be different with the section in the path.";
                return errorMessage;
            }
            string pathInsert = obj[0].sectionselected.Replace(',', '-');
            var lstScore = obj.OrderBy(o => o.MinValue).ToList();
            var isOverlap = 0;
            for (var i = 0; i < lstScore.Count; i++)
            {
                if (i == 0 && lstScore[i].MinValue != 0)
                {
                    isOverlap += 1;
                }
                else if (i == lstScore.Count - 1 && lstScore[i].MaxValue != 9999)
                {
                    isOverlap += 1;
                }
                else if (i > 0 && i < lstScore.Count - 1)
                {
                    if (lstScore[i].MinValue != (lstScore[i - 1].MaxValue + 1)
                        || (lstScore[i].MaxValue + 1) != lstScore[i + 1].MinValue)
                    {
                        isOverlap += 1;
                    }
                }
            }
            if (isOverlap > 0)
            {
                errorMessage = "Min score must be greater than the max score of the previous sections path.";
                return errorMessage;
            }
            return errorMessage;
        }

        public ActionResult DeleteSectionBranchingById(int sectionBranchingId)
        {
            if (sectionBranchingId > 0)
            {
                _virtualSectionService.DeleteFullPathVirtualSectionBranching(sectionBranchingId);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditSectionPath(int sectionBranchingId)
        {
            var mydata = _virtualSectionService.GetSectionBranchingBySectionPathId(sectionBranchingId)
                .Select(o => new SectionPathInsertViewModel()
                {
                    MaxValue = (int)o.HighScore.GetValueOrDefault(),
                    MinValue = (int)o.LowScore.GetValueOrDefault(),
                    TargetId = o.TargetVirtualSectionID.GetValueOrDefault(),
                    VirtualTestId = o.VirtualTestID.GetValueOrDefault(),
                    sectionselected = o.TestletPath
                }).ToList();
            return Json(new { success = true, data = mydata }, JsonRequestBehavior.AllowGet);
        }

        #region QuestionGroup

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddEmptyQuestionGroupForSection(int virtualTestId, int? virtualSectionId)
        {
            var bIsSupportQuestionGroup = CheckUserSupportQuestionGroup();
            if (!bIsSupportQuestionGroup)
            {
                return Json(new { Success = false, ErrorMessage = "District not support questiongroup" });
            }
            var virtualtest = _virtualTestService.GetTestById(virtualTestId);
            if (virtualtest == null)
            {
                return Json(new { Success = false, ErrorMessage = "Can not find the Test!" });
            }
            if (virtualSectionId.HasValue && virtualSectionId.Value > 0)
            {
                var virtualsection = _virtualSectionService.GetVirtualSectionById(virtualSectionId.GetValueOrDefault());
                if (virtualsection == null)
                {
                    return Json(new { Success = false, ErrorMessage = "Can not find the Section!" });
                }
            }
            //check to avoid modifying ajax parameter bankId)
            var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualtest.BankID, CurrentUser.GetMemberListDistrictId());
            if (!hasPermission)
            {
                return Json(new { Success = false, ErrorMessage = "Has no right to update this test!" });
            }

            //TODO: Add Empty Group to Secton
            int questionGroupOrder = _questionGroupService.CountQuestionGroupPerSection(virtualTestId, virtualSectionId) + 1;
            var emptyQuestionGroup = new QuestionGroup
            {
                XmlContent = string.Empty,
                Order = questionGroupOrder,
                VirtualTestId = virtualTestId,
                DisplayPosition = 0, //TODO: default display On Top of Question Group Items
            };
            if (virtualSectionId.HasValue && virtualSectionId.Value > 0)
            {
                emptyQuestionGroup.VirtualSectionID = virtualSectionId.Value;
            }
            _questionGroupService.SaveQuestionGroup(emptyQuestionGroup);

            var s3Result = UpdateTestToS3(virtualtest.VirtualTestID);
            if (!s3Result.IsSuccess)
            {
                return Json(new
                {
                    Success = false,
                    ErrorMessage = "QuestionGroup has been added successfully but uploading virtual test json file to S3 fail: " + s3Result.ErrorMessage
                }, JsonRequestBehavior.AllowGet);
            }

            var model = BuildVirtualTestViewModelByVirtualTestID(virtualtest.VirtualTestID);
            var jsonResult = Json(new { Success = true, newData = model });

            var js = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            var jsonStringResult = js.Serialize(jsonResult.Data);

            return Content(jsonStringResult, "application/json");
        }

        [HttpGet]
        [UrlReturnDecode]
        public ActionResult LoadQuestionGroupProperties(int questionGroupId)
        {
            QuestionGroup qg = _questionGroupService.GetQuestionGroupById(questionGroupId);

            var model = new QuestionGroupPropertiesViewModel();
            if (qg != null)
            {
                model = new QuestionGroupPropertiesViewModel()
                {
                    Instruction = qg.XmlContent.Trim(),
                    VirtualSectionId = qg.VirtualSectionID.GetValueOrDefault(),
                    VirtualTestId = qg.VirtualTestId,
                    QuestionGroupId = qg.QuestionGroupID,
                    DisplayPosition = qg.DisplayPosition,
                    QuestionGroupTitle = qg.Title
                };
            }
            var virtualTest = _virtualTestService.GetVirtualTestById(model.VirtualTestId);
            model.IsShowNormalBranchingButton = virtualTest?.NavigationMethodID.GetValueOrDefault() == (int)NavigationMethodEnum.NORMAL_BRANCHING;
            model.FirstQuestionInGroup = _questionGroupService.GetFirstQuestionInGroup(questionGroupId);
            model.Instruction = qg.XmlContent.ReplaceWeirdCharacters();
            model.Instruction = Util.UpdateS3LinkForItemMediaQuestionGroup(model.Instruction);
            return PartialView("_QuestionGroupProperties", model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuestionGroupProperties(QuestionGroupPropertiesViewModel model)
        {
            var virtualTest = _virtualTestService.GetTestById(model.VirtualTestId);
            if (virtualTest == null)
            {
                return Json(new { Success = false, ErrorMessage = "Can not find the Test!" });
            }

            if (model.VirtualSectionId > 0)
            {
                var virtualsection = _virtualSectionService.GetVirtualSectionById(model.VirtualSectionId);
                if (virtualsection == null)
                {
                    return Json(new { Success = false, ErrorMessage = "Can not find the section!" });
                }
            }

            //check to avoid modifying ajax parameter bankId)
            var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId());
            if (!hasPermission)
            {
                return Json(new { Success = false, ErrorMessage = "Has no right to update this test!" });
            }

            try
            {
                //Get the Question Group
                var qg = _questionGroupService.GetQuestionGroupById(model.QuestionGroupId);
                if (qg == null)
                {
                    return Json(new { Success = false, ErrorMessage = "Question group is not existed" });
                }
                qg.XmlContent = model.Instruction.ConvertFromUnicodeToWindow1252();

                if (model.VirtualSectionId > 0)
                {
                    qg.VirtualSectionID = model.VirtualSectionId;
                }
                qg.VirtualTestId = model.VirtualTestId;
                qg.DisplayPosition = model.DisplayPosition;
                qg.Title = model.QuestionGroupTitle;
                _questionGroupService.SaveQuestionGroup(qg);

                var s3Result = UpdateTestToS3(virtualTest.VirtualTestID);
                if (!s3Result.IsSuccess)
                {
                    return Json(new
                    {
                        Success = false,
                        VirtualSectionId = model.VirtualSectionId,
                        VirtualTestId = model.VirtualTestId,
                        QuestionGroupId = model.QuestionGroupId,
                        ErrorMessage = "Virtual Section has been updated successfully but uploading virtual test json file to S3 fail: " + s3Result.ErrorMessage
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { Success = false, ErrorMessage = "There was some error. Can not update question group properties right now." });
            }

            return Json(new { Success = true, QuestionGroupId = model.QuestionGroupId });
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteQuestionGroup(int virtualTestId, int questiongroupId, bool isDeleteOnlyQuestionGroup = false)
        {
            try
            {
                //Get the question group
                var questionGroup = _questionGroupService.GetQuestionGroupById(questiongroupId);
                if (questionGroup == null)
                {
                    return Json(new { success = false, ErrorMessage = "Can not find the question group!" });
                }
                var virtualtest = _virtualTestService.GetTestById(virtualTestId);
                if (virtualtest == null)
                {
                    return Json(new { success = false, ErrorMessage = "Can not find the test!" });
                }

                //check to avoid modifying ajax parameter bankId)
                var hasPermission = _vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualtest.BankID, CurrentUser.GetMemberListDistrictId());
                if (!hasPermission)
                {
                    return Json(new { success = false, ErrorMessage = "Has no right to update this test!" });
                }

                Dictionary<string, string> dc = new Dictionary<string, string>();
                List<string> arrError = new List<string>();
                List<int> listQuestionIds = _questionGroupService.GetQuestionIdsInQuestionGroup(questionGroup.QuestionGroupID);

                if (isDeleteOnlyQuestionGroup) //Only delete QuestionGroup, keep all question on this section.
                {
                    _questionGroupService.DeleteQuestionGroup(questionGroup);
                    _virtualTestService.ReassignVirtualSectionQuestionOrder(virtualTestId, 0);
                }
                else //TODO: delete question group and delete all question on this group.
                {
                    //validate data before can delete
                    if (listQuestionIds.Any())
                    {
                        string errorMessage;
                        var canRemoveQuestion = _virtualTestService.CanRemoveVirtualQuestion(string.Join(",", listQuestionIds), out errorMessage);
                        if (!canRemoveQuestion)
                            return Json(new { success = false, ErrorMessage = errorMessage });
                    }
                    //TODO: Delete QuestionGroup
                    _questionGroupService.DeleteQuestionGroup(questionGroup);
                    //TODO: Delete All Questions in Group
                    foreach (var questionId in listQuestionIds)
                    {
                        try
                        {
                            string error;
                            var sectionQuesion = _virtualSectionQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionId == questionId && x.VirtualTestId == virtualTestId);
                            _virtualTestService.RemoveVirtualQuestion(questionId, out error);
                            if (sectionQuesion != null)
                            {
                                _virtualTestService.ReassignVirtualSectionQuestionOrder(virtualTestId, sectionQuesion.VirtualSectionId);
                            }
                            if (!string.IsNullOrWhiteSpace(error))
                            {
                                var obj = _virtualQuestionService.GetQuestionDataById(questionId);
                                if (dc.ContainsKey(error))
                                {
                                    dc[error] = obj.QuestionOrder + ", " + dc[error];
                                }
                                else
                                {
                                    dc.Add(error, obj.QuestionOrder.ToString());
                                }
                            }
                        }
                        catch (Exception)
                        {
                            var obj = _virtualQuestionService.GetQuestionDataById(questionId);
                            string orderQuestion = obj != null ? obj.QuestionOrder.ToString() : string.Empty;
                            string message = "There was some error, can not delete question right now.";
                            if (dc.ContainsKey(message))
                            {
                                dc[message] = orderQuestion + ", " + dc[message];
                            }
                            else
                            {
                                dc.Add(message, orderQuestion);
                            }
                        }
                    }
                }

                if (listQuestionIds.Any())// Handle update S3 file on both case delete all question and delete group only. Ticket: LNKT-56564
                {
                    try
                    {
                        {
                            var s3Result = UpdateTestToS3(virtualTestId);
                            if (s3Result.IsSuccess == false)
                            {
                                dc.Add(string.Format("Virtual Question has been deleted successfully but uploading virtual test json file to S3 fail: {0}", s3Result.ErrorMessage), string.Empty);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        dc.Add("Virtual Question has been deleted successfully but uploading virtual test json file to S3 fail.", string.Empty);
                    }
                }
                if (dc.Count > 0)
                {
                    if (listQuestionIds.Count() == 1)
                    {
                        return Json(new { success = false, ErrorMessage = dc.First().Key }, JsonRequestBehavior.AllowGet);
                    }
                    foreach (KeyValuePair<string, string> pair in dc)
                    {
                        if (string.IsNullOrEmpty(pair.Value))
                        {
                            arrError.Add(pair.Key);
                        }
                        else
                        {
                            string[] arrOrderQuestion = pair.Value.Split(',');
                            Array.Sort(arrOrderQuestion);
                            string tmp = string.Empty;
                            foreach (string s in arrOrderQuestion)
                            {
                                if (!string.IsNullOrEmpty(s.Trim()))
                                {
                                    tmp += ", " + s.Trim();
                                }
                            }
                            if (tmp.Length > 2)
                                tmp = tmp.Substring(2);

                            arrError.Add(string.Format("({0}) {1}", tmp, pair.Key));
                        }
                    }
                    return Json(new { success = false, ErrorMessage = arrError.ToArray() }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, ErrorMessage = "There was some error, can not delete question group right now." }, JsonRequestBehavior.AllowGet);
            }
        }

        private S3Result UpdateTestToS3(int virtualTestId)
        {
            var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTestId);
            var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);
            return s3Result;
        }

        private void UpdateSectionIdOfQuestionGroupOnSectionDefault(int virtualtestId, int newSectionId)
        {
            var lstQuestionGroup = _questionGroupService
                .GetListQuestionGroupByVirtualTestId(virtualtestId)
                .Where(o => o.VirtualSectionID == 0 || o.VirtualSectionID.HasValue == false).ToList();
            _questionGroupService.UpdateSectionIdToQuestionGroup(newSectionId, lstQuestionGroup);
        }

        private bool CheckUserSupportQuestionGroup()
        {
            //TODO: Maybe check Publisher or NetworkAdmin
            int districtId = CurrentUser.DistrictId.GetValueOrDefault();
            if (CurrentUser.RoleId == (int)Permissions.Publisher || CurrentUser.RoleId == (int)Permissions.NetworkAdmin)
            {
                try
                {
                    var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                    districtId = _districtService.GetLiCodeBySubDomain(subDomain);
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    //TODO: should catch exception here
                }
            }
            var vConfiguration = _configurationService.GetConfigurationByKey(Constanst.IsSupportQuestionGroup);
            if (vConfiguration != null && vConfiguration.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                return true;
            return _districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.IsSupportQuestionGroup);
        }

        [HttpGet]
        public ActionResult GetQuestionGroupByVirtualtest(int virtualTestId, int? sectionId)
        {
            List<ListItem> data = new List<ListItem>();

            if (virtualTestId <= 0)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            data = GetDisplayedQuestionGroup(virtualTestId, sectionId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<ListItem> GetDisplayedQuestionGroup(int virtualTestId, int? sectionId)
        {
            List<ListItem> data = new List<ListItem>();
            var vquery = _questionGroupService.GetListQuestionGroupByVirtualTestId(virtualTestId).ToList();
            if (sectionId.HasValue && sectionId.Value > 0)
            {
                data = vquery.Where(x => x.VirtualSectionID == sectionId).OrderBy(x => x.Order).Select(o => new ListItem
                {
                    Id = o.QuestionGroupID,
                    Name = o.Order.ToString()
                }).ToList();
            }
            else
            {
                data = vquery.OrderBy(x => x.Order).Select(o => new ListItem
                {
                    Id = o.QuestionGroupID,
                    Name = o.Order.ToString()
                }).ToList();
            }
            return data;
        }

        #endregion QuestionGroup

        [HttpPost]
        public ActionResult UpdateNumberQuestionByTestId(int virtualTestId, bool isNumberQuestion)
        {
            _virtualTestService.UpdateNumberQuestionByTestId(virtualTestId, isNumberQuestion);
            return Json(new { success = true });
        }
    }
}
