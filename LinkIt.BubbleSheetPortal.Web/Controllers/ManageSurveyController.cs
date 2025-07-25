using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.ManageTest;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.Survey;
using LinkIt.BubbleSheetPortal.Web.BusinessObjects;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class ManageSurveyController : BaseController
    {
        private readonly ManageSurveyService _manageSurveyService;
        private readonly DistrictService _districtService;
        private readonly SubjectService _subjectService;
        private readonly BankService _bankService;
        private readonly VulnerabilityService _vulnerabilityService;
        private readonly BankDistrictService _bankDistrictService;
        private readonly BankSchoolService _bankSchoolService;
        private readonly AuthorGroupService _authorGroupService;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly VirtualTestService _virtualTestService;
        private readonly RestrictionBO _restrictionBo;
        private readonly ConfigurationService _configurationService;
        private readonly IS3Service _s3Service;
        public ManageSurveyController(ManageSurveyService manageSurveyService, DistrictService districtService,
            SubjectService subjectService, BankService bankService, VulnerabilityService vulnerabilityService,
            BankDistrictService bankDistrictService, BankSchoolService bankSchoolService, AuthorGroupService authorGroupService,
            DistrictDecodeService districtDecodeService, VirtualTestService virtualTestService,
            RestrictionBO restrictionBo, ConfigurationService configurationService, IS3Service s3Service)
        {
            _manageSurveyService = manageSurveyService;
            _districtService = districtService;
            _subjectService = subjectService;
            _bankService = bankService;
            _vulnerabilityService = vulnerabilityService;
            _bankDistrictService = bankDistrictService;
            _bankSchoolService = bankSchoolService;
            _authorGroupService = authorGroupService;
            _districtDecodeService = districtDecodeService;
            _virtualTestService = virtualTestService;
            _restrictionBo = restrictionBo;
            _configurationService = configurationService;
            _s3Service = s3Service;
        }

        public ActionResult Index()
        {
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.StateId = CurrentUser.StateId;
            ViewBag.DistrictId = CurrentUser.DistrictId;
            return View();
        }

        public ActionResult LoadSurveyBanks(int? districtId, bool? showArchived)
        {
            if (districtId == null)
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            var banks = _manageSurveyService.GetSurveyBanksByUserID(CurrentUser.Id, CurrentUser.RoleId, districtId.GetValueOrDefault(), showArchived ?? false, false)
                .Select(o => new TestBankViewModel()
                {
                    BankName = Server.HtmlEncode(o.BankName),
                    Subject = Server.HtmlEncode(o.SubjectName),
                    GradeOrder = o.GradeOrder,
                    BankID = o.BankID,
                    Grade = Server.HtmlEncode(o.GradeName),
                    Archived = o.Archived
                });
            var parser = new DataTableParser<TestBankViewModel>();
            return Json(parser.Parse(banks.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadSurveyBanksForNetworkAdmin(int? stateId, int? districtId, bool? showArchived)
        {
            var banks = new List<BankData>();
            if (districtId.HasValue)
            {
                banks = _manageSurveyService.GetSurveyBanksByUserID(CurrentUser.Id, CurrentUser.RoleId, districtId.GetValueOrDefault(), showArchived ?? false);
            }
            else
            {
                if (stateId.HasValue)
                {
                    //get all member districts that belong to this state
                    var memberDistricts = _districtService.GetDistricts()
                                                    .Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id) && x.StateId == stateId.Value)
                                                    .ToList();
                    foreach (var memberDistrict in memberDistricts)
                    {
                        banks.AddRange(_manageSurveyService.GetSurveyBanksByUserID(CurrentUser.Id, CurrentUser.RoleId, memberDistrict.Id, showArchived ?? false));
                    }
                }
                else
                {
                    //get all member districts
                    var memberDistricts = _districtService.GetDistricts()
                                                    .Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id))
                                                    .ToList();
                    foreach (var memberDistrict in memberDistricts)
                    {
                        banks.AddRange(_manageSurveyService.GetSurveyBanksByUserID(CurrentUser.Id, CurrentUser.RoleId, memberDistrict.Id, showArchived ?? false));
                    }
                }
            }
            var results = banks.Select(o => new TestBankViewModel()
            {
                BankName = Server.HtmlEncode(o.BankName),
                Subject = Server.HtmlEncode(o.SubjectName),
                GradeOrder = o.GradeOrder,
                BankID = o.BankID,
                Grade = Server.HtmlEncode(o.GradeName),
                Archived = o.Archived
            });
            var parser = new DataTableParser<TestBankViewModel>();
            return Json(parser.Parse(results.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateSurveyBank()
        {
            var viewModel = new ManageTestViewModel();
            if (CurrentUser.IsPublisher || CurrentUser.RoleId == (int)Permissions.NetworkAdmin)
            {
                viewModel.IsPublisher = CurrentUser.IsPublisher;
                viewModel.IsNetWorkAdmin = CurrentUser.RoleId == (int)Permissions.NetworkAdmin;
                viewModel.StateId = -1;
            }
            else
            {
                viewModel.IsPublisher = false;
                var district = CurrentUser.DistrictId.HasValue ? _districtService.GetDistrictById(CurrentUser.DistrictId.Value) : null;
                if (district != null)
                    viewModel.StateId = district.StateId;
            }
            return PartialView("_CreateSurveyBank", viewModel);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateSurveyBank(string bankName)
        {
            var stateId = CurrentUser.StateId.GetValueOrDefault();
            var subject = _subjectService.GetSubjectByShortName(SubjectShortName.SURVEY_SHORTNAME, stateId, SubjectShortName.SURVEY_NAME, TextConstants.GRADE_OTHER);
            if (subject == null)
            {
                return Json(new { Success = false, Error = "Has no right on the subject." }, JsonRequestBehavior.AllowGet);
            }
            var bank = new Bank()
            {
                BankAccessID = 1,
                CreatedByUserId = CurrentUser.Id,
                Name = bankName.Trim(),
                SubjectID = subject.Id,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            _bankService.Save(bank);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSurveyBankProperties(int bankId)
        {
            string bankSchoolName = string.Empty;
            string bankDistrictName = string.Empty;
            string authorGroupSharedName = string.Empty;

            BankProperty bank = _bankService.GetBankProperty(bankId);
            var bankDistricts = _bankDistrictService.GetBankDistrictByBankId(bankId).ToList();
            var bankSchools = _bankSchoolService.GetBankSchoolByBankId(bankId);

            if (CurrentUser.IsDistrictAdmin)
            {
                bankDistricts = bankDistricts.Where(x => x.DistrictId == CurrentUser.DistrictId).ToList();
            }
            if (CurrentUser.IsNetworkAdmin)
            {
                bankDistricts = bankDistricts.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId)).ToList();
            }
            if (bankDistricts.Count > 0)
            {
                bankDistrictName = string.Join(", ", bankDistricts.Select(x => x.Name).ToList());
            }
            if (CurrentUser.IsDistrictAdmin)
            {
                bankSchools = bankSchools.Where(x => x.DistrictId == CurrentUser.DistrictId);
            }
            if (CurrentUser.IsNetworkAdmin)
            {
                bankSchools = bankSchools.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId));
            }
            if (bankSchools != null)
            {
                bankSchoolName = string.Join(",", bankSchools.Select(x => x.Name).ToList());
            }
            //Get author group shared
            var authorGroupShared = _authorGroupService.GetAuthorGroupBanks(bankId, CurrentUser.Id);
            if (authorGroupShared != null)
            {
                authorGroupSharedName = string.Join(", ", authorGroupShared.Select(x => x.Name).ToList());
            }

            var bankViewModel = new TestBankPropertiesViewModel()
            {
                TestBankName = bank.Name,
                TestBankId = bank.Id,
                CreatedBy = Server.HtmlEncode(bank.Author),
                CreatedDate = bank.CreatedDate,
                UpdatedDate = bank.UpdatedDate,
                GradeId = bank.GradeId,
                GradeName = Server.HtmlEncode(bank.GradeName),
                SubjectId = bank.SubjectID,
                SubjectName = Server.HtmlEncode(bank.SubjectName),
                StateId = bank.StateId,
                AuthorGroup = Server.HtmlEncode(authorGroupSharedName),
                DistrictPublished = bankDistrictName,
                SchoolPublished = Server.HtmlEncode(bankSchoolName),
                IsPublisher = CurrentUser.IsPublisher,
                IsDistrictAdmin = CurrentUser.IsDistrictAdmin,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                PublishedToDistrictDistrictAdminOnly = false,
                Archived = bank.Archived
            };
            var config = _districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId.GetValueOrDefault(), Util.PublishedToDistrictDistrictAdminOnly).FirstOrDefault();
            if (config != null)
            {
                if (!string.IsNullOrEmpty(config.Value) && config.Value.ToUpper() == "TRUE")
                {
                    bankViewModel.PublishedToDistrictDistrictAdminOnly = true;
                }
            }

            return PartialView("_SurveyBankProperties", bankViewModel);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateSurveyBank(string bankName, int bankId)
        {
            if (!_vulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            _bankService.UpdateBankName(bankName.RemoveLineBreak(), bankId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteSurveyBank(int bankId)
        {
            if (!_vulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, error = "Has no right to delete test bank." }, JsonRequestBehavior.AllowGet);
            }

            _bankService.DeleteBankByID(bankId);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadConfirmDeleteBank(int bankId, string bankName)
        {
            var viewModel = new ListItemsViewModel()
            {
                Id = bankId,
                Name = bankName
            };

            bool canDeleteBank = _bankService.CanDeleteBankByID(bankId);
            ViewBag.CanDeleteBank = canDeleteBank;
            return PartialView("_ConfirmDeleteBank", viewModel);
        }

        public ActionResult LoadSurveyTests(int bankId, int districtId = 0, string moduleCode = "")
        {
            var parser = new DataTableParser<VirtualTestListItemViewModel>();
            var surveyTests = _virtualTestService.Select()
                .Where(o => o.BankID == bankId && o.DatasetOriginID == (int)DataSetOriginEnum.Survey)
                .Select(o => new VirtualTestListItemViewModel()
                {
                    Name = Server.HtmlEncode(o.Name),
                    VirtualTestId = o.VirtualTestID
                })
                .ToList();

            if (!string.IsNullOrEmpty(moduleCode))
            {
                if (districtId == 0)
                    districtId = CurrentUser.DistrictId.GetValueOrDefault();

                var allowTests = _restrictionBo.FilterTests(new Models.RestrictionDTO.FilterTestQueryDTO
                {
                    DistrictId = districtId,
                    BankId = bankId,
                    Tests = surveyTests.Select(m => new ListItem { Id = m.VirtualTestId, Name = m.Name }).ToList(),
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    ModuleCode = moduleCode
                }).Select(m => m.Id);

                surveyTests = surveyTests.Where(m => allowTests.Contains(m.VirtualTestId)).ToList();
            }

            return Json(parser.Parse(surveyTests.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateSurvey()
        {
            ViewBag.AllowChangeDataSetCategory = DoesEnableAbilityToChangeTestCategory(CurrentUser.DistrictId);
            return PartialView("_CreateSurvey");
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
            return districtDecode?.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Any(c => string.Compare(c, "Survey", true) == 0) ?? false;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateSurvey(int bankId, string surveyName, int? categoryId)
        {
            if (!DoesEnableAbilityToChangeTestCategory(CurrentUser.DistrictId) && categoryId.HasValue && categoryId.Value != (int)DataSetCategoryEnum.SURVEY)
            {
                return Json(new { success = false, Error = "Invalid Category" }, JsonRequestBehavior.AllowGet);
            }

            if (!_vulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, Error = "Has no right to add survey for this survey bank." }, JsonRequestBehavior.AllowGet);
            }
            var bank = _bankService.GetBankById(bankId);
            if (bank == null)
            {
                return Json(new { success = false, Error = "Bank does not exist or it has been deleted already." }, JsonRequestBehavior.AllowGet);
            }

            if (_virtualTestService.ExistTestName(bankId, surveyName))
            {
                return Json(new { success = false, Error = "A survey with name " + surveyName + " already exists in this bank." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var subject = _subjectService.GetSubjectById(bank.SubjectID);
                var test = new VirtualTestData()
                {
                    BankID = bankId,
                    Name = surveyName,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    AuthorUserID = CurrentUser.Id,
                    StateID = subject.StateId,
                    TestScoreMethodID = (int)TestScoreMethodEnum.Survey,
                    VirtualTestSourceID = 1,
                    VirtualTestSubTypeID = 1,
                    VirtualTestType = 3,
                    DatasetOriginID = (int)DataSetOriginEnum.Survey,
                    DatasetCategoryID = categoryId,
                    NavigationMethodID = (int)NavigationMethodEnum.NO_BRANCHING,
                    IsMultipleTestResult = true
                };

                _virtualTestService.Save(test);

                var s3VirtualTest = _virtualTestService.CreateS3Object(test.VirtualTestID);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                if (!s3Result.IsSuccess)
                {
                    return Json(new { success = false, Error = "Survey has been created successfully but uploading json file to S3 fail: " + s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, virtualTestId = test.VirtualTestID }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetSurveyProperties(int virtualTestId)
        {
            var survey = _virtualTestService.GetVirtualTestProperty(virtualTestId, CurrentUser.RoleId, CurrentUser.DistrictId.GetValueOrDefault());

            var result = new TestPropertiesViewModel()
            {
                TestName = survey.Name,
                CreatedDate = survey.CreatedDate,
                CreatedBy = survey.Author,
                EarliestResultDate = survey.MinResultDate,
                MostRecentResultDate = survey.MaxResultDate,
                TestId = survey.VirtualTestId,
                TotalQuestion = survey.TotalQuestion,
                TotalTestResult = survey.TotalTestResult,
                UpdatedDate = survey.UpdatedDate,
                IsAuthor = CurrentUser.Id == survey.AuthorUserId,
                Instruction = survey.Instruction.ReplaceWeirdCharacters(),
                DataSetCategoryID = survey.DataSetCategoryID,
            };
            ViewBag.IsIsPublisherOrNetworkAdmin = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin;
            return PartialView("_SurveyProperties", result);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateSurvey(int bankId, int testId, string surveyName)
        {
            surveyName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(surveyName));
            if (!_vulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, Error = "Has no right to update this survey." }, JsonRequestBehavior.AllowGet);
            }
            surveyName = surveyName.RemoveLineBreak();

            if (_virtualTestService.ExistTestNameUpdate(bankId, testId, surveyName))
            {
                return Json(new { success = false, Error = "A survey with name " + surveyName + " already exists in this bank." }, JsonRequestBehavior.AllowGet);
            }
            if (_virtualTestService.UpdateTestName(testId, surveyName))
            {
                var s3VirtualTest = _virtualTestService.CreateS3Object(testId);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                if (!s3Result.IsSuccess)
                {
                    return Json(new { success = false, Error = "Survey has been updated successfully but uploading json file to S3 fail: " + s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true });
            }
            return Json(new { success = false, Error = "Can not update survey right now." }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadConfirmDeleteSurvey(int virtualTestId, string surveyName)
        {
            var viewModel = new ListItemsViewModel()
            {
                Id = virtualTestId
            };
            surveyName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(surveyName));
            ViewBag.DeleteTestTitle = string.Empty;
            int canDeleteVirtualTest = _virtualTestService.CanDeleteVirtualTestByID(virtualTestId);
            //-- 0: can't delete//-- 1: can delete//-- 2: can delete ( with warning )
            if (canDeleteVirtualTest == 0)
            {
                ViewBag.DeleteTestTitle = "Warning";
                viewModel.Name = "This survey has survey assignments or test response; it cannot be deleted.";
                viewModel.Id = 0;
            }
            else if (canDeleteVirtualTest == 1 || canDeleteVirtualTest == 2)
            {
                ViewBag.DeleteTestTitle = "Delete Survey Confirmation";
                viewModel.Id = virtualTestId;
                viewModel.Name = $"Are you sure you want to delete {surveyName}. This action will permanently delete the survey and all questions contained in it. Are you sure you want to continue?";
                if (canDeleteVirtualTest == 2)
                    viewModel.Name = "This survey has survey assignments associated with it which can no longer be accessed once this survey is deleted. Are you sure you want to delete this survey.";
            }
            return PartialView("_ConfirmDeleteSurvey", viewModel);
        }

        public ActionResult LoadMoveSurvey(int virtualTestId, string surveyName)
        {
            ViewBag.VirtualTestId = virtualTestId;
            ViewBag.VirtualTestName = HttpUtility.UrlDecode(surveyName);
            return PartialView("_MoveSurvey");
        }

        public ActionResult LoadSurveyBankForMove(int currentBankId, bool isCopy)
        {
            var banks = _manageSurveyService.GetSurveyBanksByUserID(CurrentUser.Id, CurrentUser.RoleId, CurrentUser.DistrictId.GetValueOrDefault(), false, false);
            if (!isCopy)
            {
                banks = banks.Where(o => o.BankID != currentBankId).ToList();
            }
            var viewModel = banks.Where(x => !x.Archived).Select(o => new TestBankMoveViewModel()
            {
                BankID = o.BankID,
                BankName = Server.HtmlEncode(o.BankName),
                Subject = Server.HtmlEncode(o.SubjectName),
                GradeOrder = o.GradeOrder,
                Grade = Server.HtmlEncode(o.GradeName),
            });
            var parser = new DataTableParser<TestBankMoveViewModel>();
            return Json(parser.Parse(viewModel.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadMoveSurveyConfirmDialog(int virtualTestId, int toBankId, bool createACopy)
        {
            ViewBag.VirtualTestId = virtualTestId;
            ViewBag.ToBankId = toBankId;
            ViewBag.CreateACopy = createACopy;
            ViewBag.VirtualTestName = string.Empty;
            var virtualTest = _virtualTestService.GetTestById(virtualTestId);
            if (virtualTest != null)
            {
                ViewBag.SurveyName = virtualTest.Name;
            }

            return PartialView("_MoveSurveyConfirm");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CopySurvey(int virtualTestId, int newBankId, string surveyName)
        {
            if (virtualTestId > 0 && newBankId > 0 && !string.IsNullOrEmpty(surveyName))
            {
                VirtualTestData virtualTest = _virtualTestService.GetTestById(virtualTestId);
                if (virtualTest == null)
                {
                    return Json(new { Success = "Fail", Error = "Survey does not exist." }, JsonRequestBehavior.AllowGet);
                }
                if (!_vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", Error = "Has no right to move this survey." }, JsonRequestBehavior.AllowGet);
                }
                if (!_vulnerabilityService.HasRightToEditTestBank(CurrentUser, newBankId, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", Error = "Has no right on new bank." }, JsonRequestBehavior.AllowGet);
                }

                if (_virtualTestService.ExistTestName(newBankId, surveyName))
                {
                    return Json(new { Success = "Fail", Error = "Exist Survey Name '" + surveyName + "'" });
                }

                virtualTest.BankID = newBankId;
                virtualTest.Name = surveyName;
                var oldVirtualTestId = virtualTest.VirtualTestID;
                try
                {
                    var cloneInformation = new SurveyCloneInformationDto
                    {
                        UserId = CurrentUser.Id,
                        UserName = CurrentUser.UserName,
                        S3Domain = LinkitConfigurationManager.GetS3Settings().S3Domain,
                        AUVirtualTestBucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName,
                        AUVirtualTestFolder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder
                    };
                    _manageSurveyService.CopySurvey(virtualTest, cloneInformation);

                    var s3VirtualTest = _virtualTestService.CreateS3Object(virtualTest.VirtualTestID);
                    var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);
                    if (!s3Result.IsSuccess)
                    {
                        return Json(new { Success = false, ErrorMessage = "Survey has been updated successfully but uploading json file to S3 fail: " + s3Result.ErrorMessage }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Success = "Success" });
                    }
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    try
                    {
                        if (oldVirtualTestId != virtualTest.VirtualTestID) // please notes that this condition means we created new virtual test with error (in case of virtual test haven't created yet then do nothing)
                        {
                            _virtualTestService.DeleteVirtualTestByID(virtualTest.VirtualTestID, CurrentUser.Id, CurrentUser.RoleId, out string error);
                        }
                    }
                    catch { }
                    return Json(new { Success = "Fail", Error = "Cannot copy survey, please try again" });
                }
            }
            return Json(new { Success = "Fail", Error = "Invalid data post" });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult MoveSurvey(int virtualTestId, int newBankId)
        {
            if (virtualTestId > 0 && newBankId > 0)
            {
                var virtualTest = _virtualTestService.GetTestById(virtualTestId);
                if (virtualTest == null)
                {
                    return Json(new { Success = "Fail", Error = "Survey does not exist." }, JsonRequestBehavior.AllowGet);
                }
                if (!_vulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", Error = "Has no right to move this survey." }, JsonRequestBehavior.AllowGet);
                }
                if (!_vulnerabilityService.HasRightToEditTestBank(CurrentUser, newBankId, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", Error = "Has no right on new bank." }, JsonRequestBehavior.AllowGet);
                }

                bool isDuplicated = false;
                bool result = _virtualTestService.UpdateBankId(virtualTestId, newBankId, ref isDuplicated);

                if (result)
                    return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                else if (!result && isDuplicated)
                    return Json(new { Success = "Fail", Error = "The destination survey bank already contains a test with the same name. Please rename the survey prior to moving it to this survey bank or move it to another bank." });
            }
            return Json(new { Success = "Fail", Error = "Invalid data post" });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateSubScoreLabelSurveyTemplate(SurveyItem item)
        {
            _manageSurveyService.UpdateSubScoreLabelSurveyTemplate(item);
            return Json(new { success = true });
        }
    }
}
