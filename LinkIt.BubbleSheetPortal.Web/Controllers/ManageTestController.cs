using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ManageTest;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Models.ManageTest;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.AuthorGroup;
using LinkIt.BubbleSheetPortal.Models.Enum;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.QuestionGroup;
using LinkIt.BubbleSheetPortal.Common.Enum;
using S3Library;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;
using LinkIt.BubbleSheetPortal.Models.Constants;
using System.Text;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class ManageTestController : BaseController
    {
        private readonly ManageTestControllerParameters _parameters;
        private readonly IS3Service _s3Service;
        public ManageTestController(ManageTestControllerParameters parameters, IS3Service s3Service)
        {
            this._parameters = parameters;
            _s3Service = s3Service;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignManageTest)]
        public ActionResult Index()
        {
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.StateId = CurrentUser.StateId;
            ViewBag.DistrictId = CurrentUser.DistrictId;
            ViewBag.AbleToViewHideTeacherBanks = (CurrentUser.IsSchoolAdmin || CurrentUser.IsDistrictAdmin ||
                                               CurrentUser.IsNetworkAdmin || CurrentUser.IsPublisher);
            return View();
        }

        public ActionResult LoadVirtualTests(int bankId, int districtId = 0, string moduleCode = "")
        {
            var parser = new DataTableParser<VirtualTestListItemViewModel>();

            //check to avoid modifying ajax parameter bankId)
            //if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            //{
            //    //return empty list
            //    return Json(parser.Parse(new List<VirtualTestListItemViewModel>().AsQueryable(), true), JsonRequestBehavior.AllowGet);

            //}
            var query = _parameters.VirtualTestService.Select()
                .Where(o => o.BankID == bankId && o.VirtualTestSourceID != 3) // without Legacy Test
                .Where(x => x.OriginalTestID == null && x.DatasetOriginID != (int)DataSetOriginEnum.Item_Based_Score_Retake) // Exclude test retake
                .ToList();
            var data = query.Select(o => new VirtualTestListItemViewModel()
            {
                Name = Server.HtmlEncode(o.Name),
                VirtualTestId = o.VirtualTestID
            }).ToList();

            if (!string.IsNullOrEmpty(moduleCode))
            {
                if (districtId == 0) districtId = CurrentUser.DistrictId.GetValueOrDefault();

                var allowTests = _parameters.RestrictionBO.FilterTests(new Models.RestrictionDTO.FilterTestQueryDTO
                {
                    DistrictId = districtId,
                    BankId = bankId,
                    Tests = data.Select(m => new ListItem { Id = m.VirtualTestId, Name = m.Name }).ToList(),
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    ModuleCode = moduleCode
                }).Select(m => m.Id);

                data = data.Where(m => allowTests.Contains(m.VirtualTestId)).ToList();
            }

            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadTestBanks(int? districtId, bool? showArchived, bool? hideTeacherBanks, bool? hideOtherPeopleBanks, bool? hideBankOnlyForm, string moduleCode = "")
        {
            var filter = new GetBanksByUserIDFilter
            {
                UserID = CurrentUser.Id,
                RoleID = CurrentUser.RoleId,
                DistrictID = CurrentDistrict(districtId),
                HideTeacherBanks = hideTeacherBanks ?? false,
                HideOtherPeopleBanks = hideOtherPeopleBanks ?? false,
                HideBankOnlyForm = hideBankOnlyForm ?? false,
                ShowArchived = showArchived ?? false,
            };

            var data = _parameters.ManageTestService.GetBanksByUserID(filter)
                .Select(o => new TestBankViewModel()
                {
                    BankName = Server.HtmlEncode(o.BankName),
                    Subject = Server.HtmlEncode(o.SubjectName),
                    GradeOrder = o.GradeOrder,
                    BankID = o.BankID,
                    Grade = Server.HtmlEncode(o.GradeName),
                    Archived = o.Archived
                }).ToList();

            // Apply restriction rule
            if (!string.IsNullOrEmpty(moduleCode))
            {
                var bankIds = data.Select(m => new ListItem
                {
                    Id = m.BankID,
                    Name = m.BankName
                }).ToList();

                var allowBankIds = _parameters.RestrictionBO.FilterBanks(new Models.RestrictionDTO.FilterBankQueryDTO
                {
                    Banks = bankIds,
                    DistrictId = CurrentDistrict(districtId),
                    ModuleCode = moduleCode,
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId
                }).Select(m => m.Id).ToList();
                if (allowBankIds.Count > 0)
                {
                    var result = data.AsParallel().Where(m => allowBankIds.Contains(m.BankID)).ToList();
                    return Json(new DataTableParser<TestBankViewModel>().Parse(result.AsQueryable(), true), JsonRequestBehavior.AllowGet);
                }
            }

            var parser = new DataTableParser<TestBankViewModel>();
            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadTestBanksForNetworkAdmin(int? stateId, int? districtId, bool? showArchived, bool? hideTeacherBanks, bool? hideOtherPeopleBanks, bool? hideBankOnlyForm, string moduleCode = "")
        {
            var filter = new GetBanksByUserIDFilter
            {
                UserID = CurrentUser.Id,
                RoleID = CurrentUser.RoleId,
                DistrictID = CurrentDistrict(districtId),
                HideTeacherBanks = hideTeacherBanks ?? false,
                HideOtherPeopleBanks = hideOtherPeopleBanks ?? false,
                HideBankOnlyForm = hideBankOnlyForm ?? false,
                ShowArchived = showArchived ?? false,
            };

            var data = new List<BankData>();
            if (districtId.HasValue)
            {
                data = _parameters.ManageTestService.GetBanksByUserID(filter);
            }
            else
            {
                if (stateId.HasValue)
                {
                    //get all member districts that belong to this state
                    var memberDistricts =
                        _parameters.DistrictServices.GetDistricts()
                            .Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id) && x.StateId == stateId.Value).ToList();
                    foreach (var memberDistrict in memberDistricts)
                    {
                        filter.DistrictID = memberDistrict.Id;
                        data.AddRange(_parameters.ManageTestService.GetBanksByUserID(filter));
                    }
                }
                else
                {
                    //get all member districts
                    //var memberDistricts =
                    //    _parameters.DistrictServices.GetDistricts()
                    //        .Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id)).ToList();
                    //foreach (var memberDistrict in memberDistricts)
                    //{
                    //    data.AddRange(_parameters.ManageTestService.GetBanksByUserID(CurrentUser.Id, CurrentUser.RoleId,
                    //        CurrentUser.SchoolId.GetValueOrDefault(), memberDistrict.Id, hideTeacherBanks, hideOtherPeopleBanks, showArchived ?? false));
                    //}
                }
            }

            //TODO: get list here;
            var result = data
                .Select(o => new TestBankViewModel()
                {
                    BankName = Server.HtmlEncode(o.BankName),
                    Subject = Server.HtmlEncode(o.SubjectName),
                    GradeOrder = o.GradeOrder,
                    BankID = o.BankID,
                    Grade = Server.HtmlEncode(o.GradeName),
                    Archived = o.Archived
                }).ToList();

            // apply restriction rule

            if (!string.IsNullOrEmpty(moduleCode))
            {
                var bankIds = result.Select(m => new ListItem
                {
                    Id = m.BankID,
                    Name = m.BankName
                }).ToList();

                var allowBankIds = _parameters.RestrictionBO.FilterBanks(new Models.RestrictionDTO.FilterBankQueryDTO
                {
                    Banks = bankIds,
                    DistrictId = districtId.GetValueOrDefault(),
                    ModuleCode = moduleCode,
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId
                }).Select(m => m.Id).ToList();

                if (allowBankIds.Count > 0)
                {
                    result = result.AsParallel().Where(m => allowBankIds.Contains(m.BankID)).ToList();
                }

            }

            var parser = new DataTableParser<TestBankViewModel>();
            return Json(parser.Parse(result.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadCreateTestBank()
        {
            var obj = new ManageTestViewModel();
            if (CurrentUser.IsPublisher || CurrentUser.RoleId == (int)Permissions.NetworkAdmin)
            {
                obj.IsPublisher = CurrentUser.IsPublisher;
                obj.IsNetWorkAdmin = CurrentUser.RoleId == (int)Permissions.NetworkAdmin;
                obj.StateId = -1;
            }
            else
            {
                obj.IsPublisher = false;
                var vDistrict = CurrentUser.DistrictId.HasValue
                    ? _parameters.DistrictServices.GetDistrictById(CurrentUser.DistrictId.Value)
                    : null;
                if (vDistrict != null)
                {
                    obj.StateId = vDistrict.StateId;
                }
            }
            return PartialView("_CreateTestBank", obj);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateTestBank(int subjectId, string strTestName)
        {
            //check subjectId
            if (!_parameters.VulnerabilityService.HasRigtToAccessSubject(CurrentUser, subjectId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = false, Error = "Has no right on the subject." }, JsonRequestBehavior.AllowGet);
            }
            var bank = new Bank()
            {
                BankAccessID = 1,
                CreatedByUserId = CurrentUser.Id,
                Name = strTestName.Trim().UrlDecode(),
                SubjectID = subjectId,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            _parameters.BankServices.Save(bank);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadCreateVirtualTest()
        {
            ViewBag.IsIsPublisherOrNetworkAdmin = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin;
            ViewBag.AllowChangeDataSetCategory = DoesEnableAbilityToChangeTestCategory(CurrentUser.DistrictId);
            return PartialView("_CreateVirtualTest");
        }
        private bool DoesEnableAbilityToChangeTestCategory(int? districtId)
        {
            if (CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin)
                return true;

            string districtDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId.GetValueOrDefault(), Constanst.EnableAbilityToChangeTestCategory).FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(districtDecode))
            {
                districtDecode = _parameters.ConfigurationService.GetConfigurationByKey(Constanst.EnableAbilityToChangeTestCategory)?.Value;
            }
            return districtDecode?.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Any(c => string.Compare(c, "TestDesign", true) == 0) ?? false;
        }


        public ActionResult LoadQuickAddVirtualTest(int virtualTestId)
        {
            var vTest = _parameters.VirtualTestService.GetTestById(virtualTestId);
            ViewBag.VirtualTestId = virtualTestId;
            ViewBag.VirtualTestName = vTest.Name;
            var sections = _parameters.VirtualSectionServices.GetVirtualSectionByVirtualTest(virtualTestId);
            ViewBag.HasMoreThanOneSection = (sections != null && sections.Count > 1);

            return PartialView("_QuickAddVirtualTest");
        }

        #region Quick Create Question Item

        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateVirtualTest(int bankId, string strTestName, int? categoryId, int? mcNumber, int? crNumber)
        {
            if (!DoesEnableAbilityToChangeTestCategory(CurrentUser.DistrictId) && categoryId.HasValue
                && categoryId.Value != (int)DataSetCategoryEnum.LINKIT)
            {
                return Json(new { success = false, Error = "Invalid Category" }, JsonRequestBehavior.AllowGet);
            }

            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, Error = "Has no right to add virtual test for this test bank." }, JsonRequestBehavior.AllowGet);
            }
            //Check if this bank is existing or not
            var bank = _parameters.BankServices.GetBankById(bankId);
            if (bank == null)
            {
                return Json(new { success = false, Error = "Bank does not exist or it has been deleted already." }, JsonRequestBehavior.AllowGet);
            }
            strTestName = strTestName.UrlDecode();
            if (_parameters.VirtualTestService.ExistTestName(bankId, strTestName))
            {
                return Json(new { success = false, Error = "A test with name " + strTestName + " already exists in this bank." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var vBank = _parameters.BankServices.GetBankById(bankId);
                var vSubejct = _parameters.SubjectServices.GetSubjectById(vBank.SubjectID);
                var vTest = new VirtualTestData()
                {
                    BankID = bankId,
                    Name = strTestName,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    AuthorUserID = CurrentUser.Id,
                    StateID = vSubejct.StateId,
                    TestScoreMethodID = 1,
                    VirtualTestSourceID = 1,
                    VirtualTestSubTypeID = 1,
                    VirtualTestType = 3,
                    DatasetOriginID = (int)DataSetOriginEnum.Item_Based_Score,
                    DatasetCategoryID = categoryId,
                    NavigationMethodID = (int)NavigationMethodEnum.NO_BRANCHING,
                    IsMultipleTestResult = !_parameters.VirtualTestService.GetIsOverwriteValue(CurrentUser.DistrictId.GetValueOrDefault())
                };

                _parameters.VirtualTestService.Save(vTest);

                #region Create Question Item

                mcNumber = mcNumber ?? 0;
                crNumber = crNumber ?? 0;

                string error;
                for (int i = 0; i < mcNumber; i++)
                {
                    error = QuickCreateVirtualQuestion(strTestName, vTest, null, (qtiBank, itemSet) =>
                    {
                        return _parameters.QTIITemServices.InsertDefaultMultipleChoices(CurrentUser.Id, itemSet.QtiGroupId, 4, "A", out error);
                    });

                    if (!string.IsNullOrEmpty(error))
                        return Json(new { success = false, Error = error }, JsonRequestBehavior.AllowGet);
                }

                for (int i = 0; i < crNumber; i++)
                {
                    error = QuickCreateVirtualQuestion(strTestName, vTest, null, (qtiBank, itemSet) =>
                    {
                        return _parameters.QTIITemServices.InsertDefaultExtendedText(CurrentUser.Id, itemSet.QtiGroupId, out error);
                    });

                    if (!string.IsNullOrEmpty(error))
                        return Json(new { success = false, Error = error }, JsonRequestBehavior.AllowGet);
                }

                #endregion Create Question Item

                //if (Util.UploadTestItemMediaToS3)
                {
                    var s3VirtualTest = _parameters.VirtualTestService.CreateS3Object(vTest.VirtualTestID);
                    var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                    if (!s3Result.IsSuccess)
                    {
                        return
                            Json(
                                new
                                {
                                    success = false,
                                    Error =
                                    "Virtual Test has been created successfully but uploading json file to S3 fail: " +
                                    s3Result.ErrorMessage
                                }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, virtualTestId = vTest.VirtualTestID }, JsonRequestBehavior.AllowGet);
            }
        }

        private delegate int QuickCreateVirtualItemHandler(QtiBank qtiBank, QtiGroup itemSet);

        private string QuickCreateVirtualQuestion(string strTestName, VirtualTestData vTest, int? selectedVirtualSectionId, QuickCreateVirtualItemHandler quickCreateVirtualItemHandler)
        {
            string error = string.Empty;

            var qtiBank = _parameters.QtiBankServices.GetDefaultQTIBank(CurrentUser.UserName, CurrentUser.Id);
            var itemSet = _parameters.QtiGroupServices.GetDefaultQTIGroup(CurrentUser.Id, qtiBank.QtiBankId, strTestName);

            int qtiItemId = quickCreateVirtualItemHandler(qtiBank, itemSet);

            if (!string.IsNullOrWhiteSpace(error) || qtiItemId == 0)
                error = "There was some error when saving, please try again later. " + error;
            else
            {
                int virtualSectionId = 0;
                if (selectedVirtualSectionId.HasValue)
                {
                    var sections = _parameters.VirtualSectionServices.GetVirtualSectionByVirtualTest(vTest.VirtualTestID);
                    if (sections == null)
                    {
                        sections = new List<VirtualSection>();
                    }
                    if (sections.Count > 1)
                    {
                        //There's more than one section -> add to the selected section
                        if (vTest.VirtualTestID > 0 && selectedVirtualSectionId.HasValue && selectedVirtualSectionId.Value >= 0)
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
                        else if (sections.Count == 0)//There's no section -> add to default section ( virtualsectionId = 0 )
                        {
                            virtualSectionId = 0;
                        }
                    }
                }

                var result = Util.AddQtiItemsToVirtualSection(vTest.VirtualTestID, qtiItemId.ToString(),
                                                          virtualSectionId, false,
                                                          CurrentUser.UserName,
                                                          CurrentUser.Id, CurrentUser.StateId ?? 0,
                                                          _parameters.VirtualTestService, null, null,
                                                          null, out error, null, _s3Service);
                if (!string.IsNullOrWhiteSpace(error) || !result)
                    error = "There was some error when saving, please try again later. " + error;
                else
                    _parameters.VirtualTestService.ReassignVirtualQuestionOrder(vTest.VirtualTestID);
            }
            return error;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddVirtualTest(int virtualTestId, int? mcNumber, int? crNumber, int? selectedVirtualSectionId)
        {
            selectedVirtualSectionId = selectedVirtualSectionId ?? 0;
            var vTest = _parameters.VirtualTestService.GetTestById(virtualTestId);

            if (vTest == null)
            {
                return Json(new { success = false, Error = "Invalid virtual test." }, JsonRequestBehavior.AllowGet);
            }

            //[LNKT-64906] prevent add new question when exists test inprocess.
            if(_parameters.QTIOnlineTestSessionService.HasExistTestInProgress(vTest.VirtualTestID))
            {
                return Json(new { success = false, Error = TextConstants.EXIST_TEST_IN_PROGRESS }, JsonRequestBehavior.AllowGet);
            }

            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, vTest.BankID, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, Error = "Has no right to add virtual test for this test bank." }, JsonRequestBehavior.AllowGet);
            }

            var vBank = _parameters.BankServices.GetBankById(vTest.BankID);

            #region Create Question Item

            mcNumber = mcNumber ?? 0;
            crNumber = crNumber ?? 0;

            string error;
            for (int i = 0; i < mcNumber; i++)
            {
                error = QuickCreateVirtualQuestion(vTest.Name, vTest, selectedVirtualSectionId, (qtiBank, itemSet) =>
                {
                    return _parameters.QTIITemServices.InsertDefaultMultipleChoices(CurrentUser.Id, itemSet.QtiGroupId, 4, "A", out error);
                });

                if (!string.IsNullOrEmpty(error))
                    return Json(new { success = false, Error = error }, JsonRequestBehavior.AllowGet);
            }

            for (int i = 0; i < crNumber; i++)
            {
                error = QuickCreateVirtualQuestion(vTest.Name, vTest, selectedVirtualSectionId, (qtiBank, itemSet) =>
                {
                    return _parameters.QTIITemServices.InsertDefaultExtendedText(CurrentUser.Id, itemSet.QtiGroupId, out error);
                });

                if (!string.IsNullOrEmpty(error))
                    return Json(new { success = false, Error = error }, JsonRequestBehavior.AllowGet);
            }

            #endregion Create Question Item

            //if (Util.UploadTestItemMediaToS3)
            {
                var s3VirtualTest = _parameters.VirtualTestService.CreateS3Object(vTest.VirtualTestID);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                if (!s3Result.IsSuccess)
                {
                    return
                        Json(
                            new
                            {
                                success = false,
                                Error =
                            "Virtual Test has been created successfully but uploading json file to S3 fail: " +
                            s3Result.ErrorMessage
                            }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = true, virtualTestId = vTest.VirtualTestID }, JsonRequestBehavior.AllowGet);
        }

        #endregion Quick Create Question Item

        [HttpGet]
        public ActionResult LoadPropertiesTestBank(int bankId)
        {
            BankProperty bank = _parameters.BankServices.GetBankProperty(bankId);

            var bankDistricts = _parameters.BankDistrictService.GetBankDistrictByBankId(bankId).ToList();
            if (CurrentUser.IsDistrictAdmin)
            {
                bankDistricts = bankDistricts.Where(x => x.DistrictId == CurrentUser.DistrictId).ToList();
            }
            if (CurrentUser.IsNetworkAdmin)
            {
                bankDistricts = bankDistricts.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId)).ToList();
            }

            string bankDistrictName = "";
            if (bankDistricts.Count > 0)

            {
                bankDistrictName = string.Join(", ", bankDistricts.Select(x => x.Name).ToList());
            }
            //Get BankSchool share
            var bankSchools = _parameters.BankSchoolService.GetBankSchoolByBankId(bankId);
            if (CurrentUser.IsDistrictAdmin)
            {
                bankSchools = bankSchools.Where(x => x.DistrictId == CurrentUser.DistrictId);
            }
            if (CurrentUser.IsNetworkAdmin)
            {
                bankSchools = bankSchools.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId));
            }
            string bankSchoolName = string.Empty;
            if (bankSchools != null)
            {
                bankSchoolName = string.Join(",", bankSchools.Select(x => x.Name).ToList());
            }
            //Get author group shared
            var authorGroupShared = _parameters.AuthorGroupService.GetAuthorGroupBanks(bankId, CurrentUser.Id);
            string authorGroupSharedName = "";
            if (authorGroupShared != null)
            {
                authorGroupSharedName = string.Join(", ", authorGroupShared.Select(x => x.Name).ToList());
            }
            var vBank = new TestBankPropertiesViewModel()
            {//TODO: Schould call store fill all infor
                TestBankName = bank.Name,
                TestBankId = bank.Id,
                CreatedBy = HttpUtility.HtmlDecode(Server.HtmlEncode(bank.Author)),
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
                //SchoolPublished = "N/A",
                IsPublisher = CurrentUser.IsPublisher,
                IsDistrictAdmin = CurrentUser.IsDistrictAdmin,
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                PublishedToDistrictDistrictAdminOnly = false,
                Archived = bank.Archived
            };
            var config =
                _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                    CurrentUser.DistrictId.GetValueOrDefault(), Util.PublishedToDistrictDistrictAdminOnly).
                    FirstOrDefault();
            if (config != null)
            {
                if (!string.IsNullOrEmpty(config.Value) && config.Value.ToUpper() == "TRUE")
                {
                    vBank.PublishedToDistrictDistrictAdminOnly = true;
                }
            }

            var query = _parameters.VirtualTestService.Select()
                .Where(o => o.BankID == bankId && o.VirtualTestSourceID != 3)
                .Where(x => x.OriginalTestID == null && x.DatasetOriginID != (int)DataSetOriginEnum.Item_Based_Score_Retake)
                .ToList();

            var validTests = query.Select(x => new ListItem()
            {
                Id = x.VirtualTestID,
                Name = Server.HtmlEncode(x.Name)
            }).ToList();

            var allowTestIds = _parameters.RestrictionBO.FilterTests(new Models.RestrictionDTO.FilterTestQueryDTO
            {
                BankId = bankId,
                DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                ModuleCode = TestRestrictionModuleConstant.ManageTest,
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                Tests = validTests
            }).Select(m => m.Id);

            var isHaveTestNotAllowExport = false;

            foreach (var testID in allowTestIds)
            {
                var result = _parameters.RestrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
                {
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    ModuleCode = RestrictionConstant.Module_ExportTest,
                    TestId = testID,
                    DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                    ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test,
                    BankId = bankId,
                    IsExport = true
                });
                if (result == false)
                {
                    vBank.RestrictionAccessList.AllowToExport = false;
                    isHaveTestNotAllowExport = true;
                    break;
                }
            }

            if (!isHaveTestNotAllowExport)
            {
                vBank.RestrictionAccessList.AllowToExport = _parameters.RestrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
                {
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    ModuleCode = RestrictionConstant.Module_ExportTest,
                    DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                    ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Bank,
                    BankId = bank.Id,
                    IsExport = true
                });
            }

            return PartialView("_TestBankProperties", vBank);
        }

        [HttpGet]
        public ActionResult LoadPropertiesVirtualTest(int virtualTestId, int districtId = 0)
        {
            ViewBag.IsShowManagerAccess = JsonConvert.SerializeObject(CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin || CurrentUser.IsDistrictAdmin);
            VirtualTestProperty vtd = _parameters.VirtualTestService.GetVirtualTestProperty(virtualTestId, CurrentUser.RoleId, CurrentUser.DistrictId.GetValueOrDefault());
            var virtualTest = _parameters.VirtualTestService.GetTestById(virtualTestId);
            var totalPointsPossible = _parameters.VirtualQuestionServices.GetTotalPointsPossible(virtualTestId);
            if (virtualTest.TestScoreMethodID.HasValue && virtualTest.TestScoreMethodID.Value == 2)
            {
                //Substract from 100
                if (totalPointsPossible.HasValue && totalPointsPossible.Value < 100)
                {
                    totalPointsPossible = 100;
                }
            }

            var vTest = new TestPropertiesViewModel()
            {//TODO: should call store fill all infor
                TestName = vtd.Name,
                CreatedDate = vtd.CreatedDate,
                CreatedBy = vtd.Author,
                EarliestResultDate = vtd.MinResultDate,
                MostRecentResultDate = vtd.MaxResultDate,
                TestId = vtd.VirtualTestId,
                TotalQuestion = vtd.TotalQuestion,
                TotalTestResult = vtd.TotalTestResult,
                UpdatedDate = vtd.UpdatedDate,
                IsAuthor = CurrentUser.Id == vtd.AuthorUserId,
                TotalPointsPossible = totalPointsPossible
            };

            if (districtId == 0)
            {
                districtId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            }

            if (districtId > 0)
            {
                vTest.RestrictionAccessList.AllowToPrint = _parameters.RestrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
                {
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    ModuleCode = RestrictionConstant.Module_PrintTest,
                    TestId = virtualTestId,
                    DistrictId = districtId,
                    ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test,
                    BankId = virtualTest.BankID
                });

                vTest.RestrictionAccessList.AllowToExport = _parameters.RestrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
                {
                    UserId = CurrentUser.Id,
                    RoleId = CurrentUser.RoleId,
                    ModuleCode = RestrictionConstant.Module_ExportTest,
                    TestId = virtualTestId,
                    DistrictId = districtId,
                    ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test,
                    BankId = virtualTest.BankID,
                    IsExport = true
                });
            }

            var isShowTemplate = _parameters.DistrictDecodeService.GetDistrictDecodeByLabel(districtId, DistrictDecodeLabel.UseTemplateForOnlineTest);
            if (isShowTemplate)
            {
                var virtualTestVirtualTestCustomScore = _parameters.VirtualTestVirtualTestCustomScoreService.GetByVirtualTestId(virtualTestId);
                var virtualTestCustomScore = virtualTestVirtualTestCustomScore != null
                    ? _parameters.VirtualTestCustomScoreService.GetCustomScoreByID(virtualTestVirtualTestCustomScore.VirtualTestCustomScoreId) : null;

                vTest.IsShowTemplate = true;
                vTest.TemplateId = virtualTestCustomScore?.VirtualTestCustomScoreId;
                vTest.TemplateName = virtualTestCustomScore?.Name;
            }

            vTest.HasRetakeRequest = _parameters.VirtualTestService.IsVirtualTestHasRetake(virtualTestId);

            return PartialView("_VirtualTestProperties", vTest);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateTestBank(string strTestBankName, int subjectId, int bankId)
        {
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            strTestBankName = RemoveLineBreak(strTestBankName);

            _parameters.BankServices.UpdateBank(strTestBankName, subjectId, bankId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadConfirmDeleteBank(int bankId, string bankName)
        {
            var obj = new ListItemsViewModel()
            {
                Id = bankId,
                Name = bankName
            };
            bool canDeleteBank = _parameters.BankServices.CanDeleteBankByID(bankId);
            ViewBag.CanDeleteBank = canDeleteBank;
            return PartialView("_ConfirmDeleteBank", obj);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateTest(int bankId, int testId, string testName)
        {
            testName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(testName));
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, Error = "Has no right to update this test." }, JsonRequestBehavior.AllowGet);
            }

            testName = RemoveLineBreak(testName);

            if (_parameters.VirtualTestService.ExistTestNameUpdate(bankId, testId, testName))
            {
                return Json(new { success = false, Error = "A test with name " + testName + " already exists in this bank." }, JsonRequestBehavior.AllowGet);
            }
            if (_parameters.VirtualTestService.UpdateTestName(testId, testName))
            {
                //if (Util.UploadTestItemMediaToS3)
                {
                    var s3VirtualTest = _parameters.VirtualTestService.CreateS3Object(testId);
                    var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                    if (!s3Result.IsSuccess)
                    {
                        return
                            Json(
                                new
                                {
                                    success = false,
                                    Error =
                                    "Virtual Test has been updated successfully but uploading json file to S3 fail: " +
                                    s3Result.ErrorMessage
                                }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = true });
            }

            return Json(new { success = false, Error = "Can not update virtual test right now." }, JsonRequestBehavior.AllowGet);
        }

        private string RemoveLineBreak(string input)
        {
            // Remove line break out of test name
            if (!string.IsNullOrEmpty(input))
            {
                // Try to replace "\n " to "\n" to make sure double space ("  ") not existed
                input = input.Replace("\n ", "\n").Replace("\n", " ").Trim();
            }

            return input;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteTestBank(int bankId)
        {
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, error = "Has no right to delete test bank." }, JsonRequestBehavior.AllowGet);
            }
            _parameters.BankServices.DeleteBankByID(bankId);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadConfirmDeleteVirtualTest(int virtualTestId, string strTestName)
        {
            var obj = new ListItemsViewModel()
            {
                Id = virtualTestId
            };
            strTestName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(strTestName));
            ViewBag.DeleteTestTitle = string.Empty;
            int canDeleteVirtualTest = _parameters.VirtualTestService.CanDeleteVirtualTestByID(virtualTestId);
            //-- 0: can't delete//-- 1: can delete//-- 2: can delete ( with warning )
            if (canDeleteVirtualTest == 0)
            {
                ViewBag.DeleteTestTitle = "Warning";
                obj.Name = "This test has online test assignments, bubble sheets or test results; it cannot be deleted.";
                obj.Id = 0;
            }
            else if (canDeleteVirtualTest == 1 || canDeleteVirtualTest == 2)
            {
                ViewBag.DeleteTestTitle = "Delete Test Confirmation";
                obj.Id = virtualTestId;
                obj.Name = string.Format("Are you sure you want to delete {0}. This action will permanently delete the test and all questions contained in it. Are you sure you want to continue?", strTestName);
                if (canDeleteVirtualTest == 2)
                    obj.Name =
                        "This test has online test assignments associated with it which can no longer be accessed once this test is deleted. Are you sure you want to delete this test.";
            }
            return PartialView("__ConfirmDeleteVirtualTest", obj);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteVirtualTest(int virtualTestId)
        {
            //check to avoid modifying ajax parameter bankId)
            var virtualTest = _parameters.VirtualTestService.GetTestById(virtualTestId);
            if (virtualTest == null)
            {
                return Json(new { success = false, errorMessage = "Virtual test does not exist." }, JsonRequestBehavior.AllowGet);
            }
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, errorMessage = "Has no right to delete this test." }, JsonRequestBehavior.AllowGet);
            }
            string error = string.Empty;
            _parameters.VirtualTestService.DeleteVirtualTestByID(virtualTestId, CurrentUser.Id, CurrentUser.RoleId, out error);
            if (string.IsNullOrWhiteSpace(error))
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, errorMessage = error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadMoveVirtualTest(int virtualTestId, string virtualTestName)
        {
            ViewBag.VirtualTestId = virtualTestId;
            ViewBag.VirtualTestName = HttpUtility.UrlDecode(virtualTestName);
            return PartialView("_MoveVirtualTest");
        }

        public ActionResult LoadMoveVirtualTestConfirmDialog(int virtualTestId, int toBankId, bool createACopy, string virtualTestName)
        {
            ViewBag.VirtualTestId = virtualTestId;
            ViewBag.ToBankId = toBankId;
            ViewBag.CreateACopy = createACopy;
            ViewBag.VirtualTestName = string.Empty;
            var virtualTest = _parameters.VirtualTestService.GetTestById(virtualTestId);
            if (virtualTest != null)
            {
                ViewBag.VirtualTestName = virtualTest.Name;
            }

            return PartialView("_MoveVirtualTestConfirm");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult MoveVirtualTest(int virtualTestId, int newBankId)
        {
            if (virtualTestId > 0 && newBankId > 0)
            {
                //check to avoid modifying ajax parameter bankId)
                var virtualTest = _parameters.VirtualTestService.GetTestById(virtualTestId);
                if (virtualTest == null)
                {
                    return Json(new { Success = "Fail", Error = "Virtual test does not exist." }, JsonRequestBehavior.AllowGet);
                }
                if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", Error = "Has no right to move this virtual test." }, JsonRequestBehavior.AllowGet);
                }
                if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, newBankId, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", Error = "Has no right on new bank." }, JsonRequestBehavior.AllowGet);
                }
                bool isDuplicated = false;
                bool rs = _parameters.VirtualTestService.UpdateBankId(virtualTestId, newBankId, ref isDuplicated);
                if (rs)
                    return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                else if (!rs && isDuplicated)
                    return Json(new { Success = "Fail", Error = "The destination test bank already contains a test with the same name. Please rename the test prior to moving it to this test bank or move it to another bank." });
            }
            return Json(new { Success = "Fail", Error = "Invalid data post" });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CopyVirtualTest(int virtualTestId, int newBankId, string testName)
        {
            if (virtualTestId > 0 && newBankId > 0 && !string.IsNullOrEmpty(testName))
            {
                //check to avoid modifying ajax parameter bankId)
                VirtualTestData virtualTest = _parameters.VirtualTestService.GetTestById(virtualTestId);
                if (virtualTest == null)
                {
                    return Json(new { Success = "Fail", Error = "Virtual test does not exist." }, JsonRequestBehavior.AllowGet);
                }
                if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", Error = "Has no right to move this virtual test." }, JsonRequestBehavior.AllowGet);
                }
                if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, newBankId, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", Error = "Has no right on new bank." }, JsonRequestBehavior.AllowGet);
                }

                if (_parameters.VirtualTestService.ExistTestName(newBankId, testName))
                {
                    return Json(new { Success = "Fail", Error = "Tests with the same name cannot exist within a test bank." });
                }

                virtualTest.BankID = newBankId;
                virtualTest.Name = testName;
                var oldVirtualTestId = virtualTest.VirtualTestID;
                try
                {
                    CopyVirtualTest(virtualTest);
                    //if (Util.UploadTestItemMediaToS3)
                    _parameters.RubricModuleCommandService.CloneVirtualTest(virtualTestId, virtualTest.VirtualTestID);
                    var s3VirtualTest = _parameters.VirtualTestService.CreateS3Object(virtualTest.VirtualTestID);//New VirtualTestID
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
                            _parameters.VirtualTestService.DeleteVirtualTestByID(virtualTest.VirtualTestID, CurrentUser.Id, CurrentUser.RoleId, out string error);
                        }
                    }
                    catch { }
                    return Json(new { Success = "Fail", Error = "Cannot copy virtual test, please try again" });
                }
            }
            return Json(new { Success = "Fail", Error = "Invalid data post" });
        }

        public ActionResult LoadTestBankforMove(GetBanksByUserIDRequest request)
        {
            //TODO: get list here;
            var filter = MappingRequest(request);

            var vdata = _parameters.ManageTestService.GetBanksByUserID(filter);

            if (request.IsCopy == false)
            {
                vdata = vdata.Where(o => o.BankID != request.CurrentBankId).ToList();
            }

            vdata = vdata.Where(x => !x.Archived).ToList();
            var totalRecord = vdata.FirstOrDefault()?.TotalRecords ?? 0;

            var parser = new DataTableParser<TestBankMoveViewModel>();
            var data = vdata.Select(o => new TestBankMoveViewModel()
            {
                BankID = o.BankID,
                BankName = Server.HtmlEncode(o.BankName),
                Subject = Server.HtmlEncode(o.SubjectName),
                GradeOrder = o.GradeOrder,
                Grade = Server.HtmlEncode(o.GradeName),
            }).AsQueryable();

            return Json(parser.Parse(data, totalRecord), JsonRequestBehavior.AllowGet);
        }

        #region Clone VirtualTest

        [NonAction]
        private void CopyVirtualTest(VirtualTestData virtualTestData)
        {
            int oldTestId = virtualTestData.VirtualTestID;
            virtualTestData.VirtualTestID = 0;
            virtualTestData.CreatedDate = DateTime.UtcNow;
            virtualTestData.UpdatedDate = DateTime.UtcNow;
            virtualTestData.AuthorUserID = CurrentUser.Id;

            _parameters.VirtualTestService.Save(virtualTestData);
            //TODO: Clone Relationship
            CloneRelationShipVirtualTest(oldTestId, virtualTestData.VirtualTestID);

            //TODO: Clone Question, VirtualSection, VirtualSectionQuestion

            CloneQuestions(oldTestId, virtualTestData.VirtualTestID, virtualTestData.Name);
            //Udpate BaseVirtualQuestion
            _parameters.VirtualTestService.UpdateBaseVirtualQuestionClone(oldTestId, virtualTestData.VirtualTestID);
        }

        [NonAction]
        private void CloneQuestions(int oldTestId, int newTestId, string testName)
        {
            var dc = new Dictionary<int, CloneQTIItemModel>();
            var cloneVirtualQuestion = new List<CloneVirtualQuestion>();

            //TODO: Create VirtualQuestion
            CloneQuestionsByTestId(oldTestId, newTestId, dc, cloneVirtualQuestion);

            //TODO: Clone Question RelationShip
            CloneQuestionRelationship(cloneVirtualQuestion);

            //TODO: Create Section & SectionQuestion
            CloneVirtualSection(oldTestId, newTestId, dc);

            //TODO: Create QtiItem
            CloneQTIITemAndUpdateQuestions(testName, dc);
        }

        private void CloneQuestionRelationship(List<CloneVirtualQuestion> cloneVirtualQuestions)
        {
            if (cloneVirtualQuestions.Count == 0)
            {
                return;
            }

            _parameters.VirtualQuestionItemTagServices.CloneVirtualQuestionItemTag(cloneVirtualQuestions);
            _parameters.VirtualQuestionLessonOneServices.CloneVirtualQuestionLessonOne(cloneVirtualQuestions);
            _parameters.VirtualQuestionLessonTwoServices.CloneVirtualQuestionLessonTwo(cloneVirtualQuestions);
            _parameters.MasterStandardServices.CloneVirtualQuestionStateStandard(cloneVirtualQuestions);
            _parameters.VirtualQuestionTopicServices.CloneVirtualQuestionTopic(cloneVirtualQuestions);
        }

        private void CloneQTIITemAndUpdateQuestions(string testName, Dictionary<int, CloneQTIItemModel> dc)
        {
            //TODO: Create ItemBank (QTIIBank)
            var vQTIBank = _parameters.QtiBankServices.CreateQTIBankByUserName(CurrentUser.UserName, CurrentUser.Id);

            //TODO: Create ItemSet (QTIGroup)
            var vQtiGroup = _parameters.QtiGroupServices.CreateItemSetByUserId(CurrentUser.Id, vQTIBank.QtiBankId, testName);

            //TODO: CloneQTIITem
            CloneQTIItemWithQTIIGroupId(vQtiGroup.QtiGroupId, dc);
        }

        private void CloneQTIItemWithQTIIGroupId(int qtiigroupId, Dictionary<int, CloneQTIItemModel> dc)
        {
            if (qtiigroupId > 0 && dc.Count > 0)
            {
                //Need to upload media file (image,audio),if any, to S3
                var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
                var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;

                foreach (KeyValuePair<int, CloneQTIItemModel> pair in dc)
                {
                    //TODO: Clone QTIITem
                    int newQTIITeamId = _parameters.QTIITemServices.DuplicateQTIItemForTest(CurrentUser.Id,
                        pair.Value.OldQTIITemID, qtiigroupId, pair.Key, pair.Value.NewQuestionID, true, bucketName,
                        folder, LinkitConfigurationManager.GetS3Settings().S3Domain);
                    dc[pair.Key].NewQTIITemID = newQTIITeamId;

                    //TODO: Update QuestionID
                    _parameters.VirtualQuestionServices.UpdateQIITemIdbyQuestionId(pair.Value.NewQuestionID, newQTIITeamId);
                    UpdateItemPassage(newQTIITeamId);

                    _parameters.VirtualQuestionServices.CloneAlgorithmQTIItemGrading(pair.Value.OldQTIITemID, newQTIITeamId);
                }
            }
        }

        private void UpdateItemPassage(int qtiItemId)
        {
            var qtiItem = _parameters.QTIITemServices.GetQtiItemById(qtiItemId);
            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            qtiItem.XmlContent = qtiItem.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
            qtiItem.XmlContent = qtiItem.XmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();
            List<PassageViewModel> passageList = Util.GetPassageList(qtiItem.XmlContent, false);
            if (passageList != null)
            {
                _parameters.QTIITemServices.UpdateItemPassage(qtiItemId, passageList.Select(x => x.QtiRefObjectID).ToList(),
                    passageList.Select(x => x.RefNumber).ToList());
            }
        }

        private void CloneVirtualSection(int oldTestId, int newTestId, Dictionary<int, CloneQTIItemModel> dc)
        {
            var lstSections = _parameters.VirtualSectionServices.GetVirtualSectionByVirtualTest(oldTestId);
            if (lstSections.Count > 0)
            {
                foreach (VirtualSection section in lstSections)
                {
                    int oldVirtualSectionId = section.VirtualSectionId;
                    section.VirtualTestId = newTestId;
                    section.VirtualSectionId = 0;
                    _parameters.VirtualSectionServices.Save(section);
                    CloneVirtualSectionQuestion(oldTestId, oldVirtualSectionId, section, dc);
                    var listQuestionGroup = _parameters.VirtualQuestionGroupService.GetListQuestionGroupByVirtualTestIdAndSectionId(newTestId, oldVirtualSectionId);
                    _parameters.VirtualQuestionGroupService.UpdateSectionIdToQuestionGroup(section.VirtualSectionId, listQuestionGroup);
                }
            }
        }

        private void CloneVirtualSectionQuestion(int oldVirtualTestId, int oldVirtualSectionId, VirtualSection newVirtualSection, Dictionary<int, CloneQTIItemModel> dc)
        {
            var lstSectionQuestions = _parameters.VirtualSectionQuestionServices.GetVirtualSectionQuestionBySection(oldVirtualTestId, oldVirtualSectionId);
            if (lstSectionQuestions.Count > 0)
            {
                lstSectionQuestions.ForEach(item =>
                {
                    int newVirtualQuestionId = dc[item.VirtualQuestionId].NewQuestionID;
                    item.VirtualSectionQuestionId = 0;
                    item.VirtualSectionId = newVirtualSection.VirtualSectionId;
                    item.VirtualQuestionId = newVirtualQuestionId;
                });

                _parameters.VirtualSectionQuestionServices.InsertMultipleRecord(lstSectionQuestions);
            }
        }

        private void CloneQuestionsByTestId(int oldTestId, int newTestId, Dictionary<int, CloneQTIItemModel> dc, List<CloneVirtualQuestion> cloneVQs)
        {
            var lstOldQuestions = _parameters.VirtualQuestionServices.GetVirtualQuestionByVirtualTestID(oldTestId);

            if (lstOldQuestions.Count == 0)
            {
                return;
            }

            var oldQuestions = lstOldQuestions.Select(x => new { x.VirtualQuestionID, x.QTIItemID, x.QuestionOrder }).ToList();

            var newQuestions = lstOldQuestions.Clone();
            newQuestions.ForEach(item =>
            {
                item.VirtualQuestionID = 0;
                item.VirtualTestID = newTestId;
            });

            _parameters.VirtualQuestionServices.InsertMultipleRecord(newQuestions);

            oldQuestions.ForEach(oldQuestion =>
            {
                var newQuestion = newQuestions.FirstOrDefault(x => x.QTIItemID == oldQuestion.QTIItemID && x.QuestionOrder == oldQuestion.QuestionOrder);

                dc.Add(oldQuestion.VirtualQuestionID, new CloneQTIItemModel()
                {
                    NewQuestionID = newQuestion.VirtualQuestionID,
                    OldQTIITemID = oldQuestion.QTIItemID ?? 0
                });

                cloneVQs.Add(new CloneVirtualQuestion
                {
                    OldVirtualQuestionID = oldQuestion.VirtualQuestionID,
                    NewVirtualQuestionID = newQuestion.VirtualQuestionID,
                });
            });

            var dcQuestionGroupId = new Dictionary<int, int>();

            var oldVirtualQeustionIDs = oldQuestions.Select(x => x.VirtualQuestionID).ToList();
            var virtualQuestionGroups = _parameters.VirtualQuestionGroupService.GetVirtualQuestionGroupsByVirtualQuestionIds(oldVirtualQeustionIDs);

            if (virtualQuestionGroups.Any())
            {
                var questionGroupIDs = virtualQuestionGroups.Select(x => x.QuestionGroupID).ToList();
                var questionGroups = _parameters.VirtualQuestionGroupService.GetQuestionGroups(oldTestId, questionGroupIDs);

                foreach (var oldQuestion in lstOldQuestions)
                {
                    var newVirtualQuestionID = newQuestions.FirstOrDefault(x => x.QTIItemID == oldQuestion.QTIItemID && x.QuestionOrder == oldQuestion.QuestionOrder).VirtualQuestionID;
                    var vQuestionGroup = virtualQuestionGroups.FirstOrDefault(x => x.VirtualQuestionID == oldQuestion.VirtualQuestionID);

                    if (vQuestionGroup != null)
                    {
                        var questionGroup = questionGroups.FirstOrDefault(x => x.QuestionGroupID == vQuestionGroup.QuestionGroupID);

                        QuestionGroup questionGroupItem = new QuestionGroup();
                        if (!dcQuestionGroupId.ContainsKey(vQuestionGroup.QuestionGroupID))
                        {
                            questionGroupItem.QuestionGroupID = 0;
                            questionGroupItem.VirtualTestId = newTestId;
                            questionGroupItem.Order = vQuestionGroup.Order;
                            questionGroupItem.XmlContent = questionGroup.XmlContent;
                            questionGroupItem.VirtualSectionID = questionGroup.VirtualSectionID;
                            questionGroupItem.Title = questionGroup.Title;
                            questionGroupItem.DisplayPosition = questionGroup.DisplayPosition;
                            _parameters.VirtualQuestionGroupService.SaveQuestionGroup(questionGroupItem);
                            dcQuestionGroupId.Add(vQuestionGroup.QuestionGroupID, questionGroupItem.QuestionGroupID);
                        }

                        VirtualQuestionGroup itemvirtualQuestionGroup = new VirtualQuestionGroup();
                        itemvirtualQuestionGroup.VirtualQuestionGroupID = 0;
                        itemvirtualQuestionGroup.VirtualQuestionID = newVirtualQuestionID;
                        itemvirtualQuestionGroup.QuestionGroupID = (dcQuestionGroupId[vQuestionGroup.QuestionGroupID] != 0) ? dcQuestionGroupId[vQuestionGroup.QuestionGroupID] : questionGroupItem.QuestionGroupID;
                        itemvirtualQuestionGroup.Order = vQuestionGroup.Order;
                        _parameters.VirtualQuestionGroupService.SaveVirtualQuestionGroup(itemvirtualQuestionGroup);
                    }
                }
            }

            _parameters.VirtualQuestionServices.CloneAlgorithmicVirtualQuestionGradingMultiple(cloneVQs);
        }

        private void CloneRelationShipVirtualTest(int oldTestId, int newTestId)
        {
            var virtualTestFile = _parameters.VirtualTestFileServices.GetFirstOrDefaultByVirtualTest(oldTestId);
            if (virtualTestFile != null)
            {
                virtualTestFile.VirtualTestFileId = 0;
                virtualTestFile.VirtualTestId = newTestId;
                _parameters.VirtualTestFileServices.Save(virtualTestFile);
            }
        }

        #endregion Clone VirtualTest

        #region Publish Bank To " + LabelHelper.DistrictLabel + "

        public ActionResult LoadListOrShareDistrict(int bankId)
        {
            return PartialView("_ListOrShareDistrict", bankId);
        }

        public ActionResult LoadListDistrictByBank(int bankId)
        {
            var isSurveyBank = _parameters.VulnerabilityService.IsSurveyBank(bankId);
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;

            ViewBag.IsShowRemovePublishDistrict = JsonConvert.SerializeObject(CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin || CurrentUser.IsDistrictAdmin);
            ViewBag.IsShowManagerAccess = isSurveyBank ? "false" : JsonConvert.SerializeObject(CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin || CurrentUser.IsDistrictAdmin);
            return PartialView("_ListDistrictByBank", bankId);
        }

        [HttpGet]
        public ActionResult GetPublishedDistrict(int bankId)
        {
            var hasRightToEditTestBank = _parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId());

            IQueryable<BankPublishedDistrictViewModel> data = _parameters.BankDistrictService.GetBankDistrictByBankId(bankId)
                .Select(x => new BankPublishedDistrictViewModel
                {
                    DistrictId = x.DistrictId,
                    Name = x.Name,
                    BankDistrictId = x.BankDistrictId,
                    BankId = x.BankId,
                    IsRightDeleteDistrictBank = hasRightToEditTestBank && Util.HasRightOnDistrict(CurrentUser, x.DistrictId)
                }
                );
            if (CurrentUser.IsDistrictAdmin)
            {
                data = data.Where(x => x.DistrictId == CurrentUser.DistrictId);
            }
            if (CurrentUser.IsNetworkAdmin)
            {
                data = data.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId));
            }
            var parser = new DataTableParser<BankPublishedDistrictViewModel>();
            return new JsonNetResult { Data = parser.Parse(data) };
        }

        public ActionResult LoadPublishToDistrict(int bankId)
        {
            var model = new BankPublishToDistrictViewModel();
            model.BankId = bankId;
            model.IsPublisher = CurrentUser.IsPublisher;
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_PublishToDistrict", model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PublishToDistrict(BankPublishToDistrictViewModel model)
        {
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, model.BankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this bank." }, JsonRequestBehavior.AllowGet);
            }
            if (!Util.HasRightOnDistrict(CurrentUser, model.DistrictId))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "." }, JsonRequestBehavior.AllowGet);
            }
            var bank = _parameters.BankServices.GetBankById(model.BankId);
            if (bank == null)
            {
                return Json(new { Success = false, ErrorMessage = "Bank does not exist." }, JsonRequestBehavior.AllowGet);
            }

            var district = _parameters.DistrictServices.GetDistrictById(model.DistrictId);
            if (district == null)
            {
                return Json(new { Success = false, ErrorMessage = "" + LabelHelper.DistrictLabel + " does not exist." }, JsonRequestBehavior.AllowGet);
            }

            var bankDistrict = _parameters.BankDistrictService.Select().FirstOrDefault(
                    x => x.DistrictId.Equals(model.DistrictId) && x.BankId.Equals(model.BankId));
            if (bankDistrict != null)
            {
                return Json(new { Success = false, ErrorMessage = "Bank is already shared to this " + LabelHelper.DistrictLabel + "." }, JsonRequestBehavior.AllowGet);
            }
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddYears(10);//Linkit add default ten years from now
            var newBankDistrict = new BankDistrict
            {
                DistrictId = model.DistrictId,
                EditedByUserId = CurrentUser.Id,
                BankId = model.BankId,
                StartDate = startDate,
                //EndDate = endDate
                EndDate = DateTime.MaxValue,
                BankDistrictAccessId = 1
            };
            try
            {
                _parameters.BankDistrictService.Save(newBankDistrict);
            }
            catch (Exception)
            {
                return Json(new { Success = false, ErrorMessage = "There was some error. Can not share bank for this " + LabelHelper.DistrictLabel + " right now." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PublishToMyDistrict(int bankId)
        {
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this bank." }, JsonRequestBehavior.AllowGet);
            }

            var bank = _parameters.BankServices.GetBankById(bankId);
            if (bank == null)
            {
                return Json(new { Success = false, ErrorMessage = "Bank does not exist." }, JsonRequestBehavior.AllowGet);
            }

            var district = _parameters.DistrictServices.GetDistrictById(CurrentUser.DistrictId ?? 0);
            if (district == null)
            {
                return Json(new { Success = false, ErrorMessage = "" + LabelHelper.DistrictLabel + " does not exist." }, JsonRequestBehavior.AllowGet);
            }

            var bankDistrict = _parameters.BankDistrictService.Select().FirstOrDefault(
                    x => x.DistrictId.Equals(CurrentUser.DistrictId ?? 0) && x.BankId.Equals(bankId));
            if (bankDistrict != null)
            {
                return Json(new { Success = false, ErrorMessage = "Bank is already shared to this " + LabelHelper.DistrictLabel + "." }, JsonRequestBehavior.AllowGet);
            }
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddYears(10);//Linkit add default ten years from now
            var newBankDistrict = new BankDistrict
            {
                DistrictId = CurrentUser.DistrictId ?? 0,
                EditedByUserId = CurrentUser.Id,
                BankId = bankId,
                StartDate = startDate,
                //EndDate = endDate
                EndDate = DateTime.MaxValue,
                BankDistrictAccessId = 1
            };
            try
            {
                _parameters.BankDistrictService.Save(newBankDistrict);
            }
            catch (Exception)
            {
                return Json(new { Success = false, ErrorMessage = "There was some error. Can not share bank this " + LabelHelper.DistrictLabel + " right now." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetUnPublishedDistrictsByState(int stateId, int bankId)
        {
            var data = _parameters.BankDistrictService.GetUnPublishedDistrict(stateId, bankId).ToList();
            if (CurrentUser.IsNetworkAdmin)
            {
                data = data.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id)).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DepublishDistrict(int bankDistrictId)
        {
            var bankDistrict = _parameters.BankDistrictService.GetBankDistrictById(bankDistrictId);
            if (bankDistrict.IsNull())
            {
                return Json(false);
            }
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankDistrict.BankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this bank." }, JsonRequestBehavior.AllowGet);
            }
            if (!Util.HasRightOnDistrict(CurrentUser, bankDistrict.DistrictId))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "." }, JsonRequestBehavior.AllowGet);
            }
            var districtbank = _parameters.BankDistrictService.GetBankDistrictById(bankDistrictId);
            _parameters.BankDistrictService.Delete(bankDistrict);
            _parameters.TestRestrictionModuleService.DeleteAllRetrictTestBanFromBankIdAndDistrict
                (new DeleteAllRetrictBankTestFromBankIdAndDistrictRequestDTO
                {
                    BankID = districtbank.BankId,
                    DistrictID = districtbank.DistrictId
                });
            return Json(true);
        }

        #endregion Publish Bank To " + LabelHelper.DistrictLabel + "

        #region Publish Bank To School

        public ActionResult LoadListOrShareSchool(int bankId)
        {
            return PartialView("_ListOrShareSchool", bankId);
        }

        public ActionResult LoadListSchoolByBank(int bankId)
        {
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            return PartialView("_ListSchoolByBank", bankId);
        }

        [HttpGet]
        public ActionResult GetPublishedSchool(int bankId)
        {
            var data = GetPublishedSchools(bankId);

            IQueryable<BankPublishedSchoolViewModel> result =
                data.Select(x => new BankPublishedSchoolViewModel
                {
                    DistrictName = x.DistrictName,
                    Name = x.Name,
                    BankId = x.BankId,
                    BankSchoolId = x.BankSchoolId,
                    SchoolId = x.SchoolId
                }
                );

            var parser = new DataTableParser<BankPublishedSchoolViewModel>();
            return new JsonNetResult { Data = parser.Parse(result) };
        }

        private IQueryable<BankSchool> GetPublishedSchools(int bankId)
        {
            var data = _parameters.BankSchoolService.GetBankSchoolByBankId(bankId);

            if (CurrentUser.IsDistrictAdmin)
            {
                data = data.Where(x => x.DistrictId == CurrentUser.DistrictId);
            }
            //if(CurrentUser.IsTeacher || CurrentUser.IsSchoolAdmin)
            //{
            //    var userSchools = _parameters.UserSchoolService.GetSchoolsUserHasAccessTo(CurrentUser.Id).Select(x=>x.SchoolId).ToList();
            //    data = data.Where(x => userSchools.Contains(x.SchoolId));
            //}
            if (CurrentUser.IsNetworkAdmin)
            {
                data = data.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId));
            }

            return data;
        }

        public ActionResult LoadPublishToSchool(int bankId)
        {
            var model = new BankPublishToSchoolViewModel();
            model.BankId = bankId;
            model.IsPublisher = CurrentUser.IsPublisher;
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_PublishToSchool", model);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetUnPublishedSchoolsByDistrict(int districtId, int bankId)
        {
            var data = GetUnPublishedSchools(districtId, bankId);
            return Json(data.OrderBy(x => x.Name).AsQueryable(), JsonRequestBehavior.AllowGet);
        }

        private List<School> GetUnPublishedSchools(int districtId, int bankId)
        {
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId ?? 0;
            }

            var data = _parameters.BankSchoolService.GetUnPublishedSchool(districtId, bankId).ToList();
            if (CurrentUser.IsNetworkAdmin)
            {
                data = data.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId)).ToList();
            }

            return data;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DepublishSchool(int bankSchoolId)
        {
            var bankSchool = _parameters.BankSchoolService.GetBankSchoolById(bankSchoolId);
            if (bankSchool.IsNull())
            {
                return Json(false);
            }
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankSchool.BankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this bank." }, JsonRequestBehavior.AllowGet);
            }

            //School
            var publishedSchools = GetPublishedSchools(bankSchool.BankId).ToList();
            if (publishedSchools.All(x => x.BankSchoolId != bankSchoolId))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this school." }, JsonRequestBehavior.AllowGet);
            }

            _parameters.BankSchoolService.Delete(bankSchool);
            return Json(true);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PublishToSchool(BankPublishToSchoolViewModel model)
        {
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, model.BankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this bank." }, JsonRequestBehavior.AllowGet);
            }

            var unPublishedDistricts = GetUnPublishedSchools(model.DistrictId, model.BankId);
            //School
            if (unPublishedDistricts.All(x => x.Id != model.SchoolId))
            {
                return Json(new { Success = false, ErrorMessage = "Has no right on this school." }, JsonRequestBehavior.AllowGet);
            }

            var bank = _parameters.BankServices.GetBankById(model.BankId);
            if (bank == null)
            {
                return Json(new { Success = false, ErrorMessage = "Bank does not exist." }, JsonRequestBehavior.AllowGet);
            }

            var school = _parameters.SchoolService.GetSchoolById(model.SchoolId);
            if (school == null)
            {
                return Json(new { Success = false, ErrorMessage = "School does not exist." }, JsonRequestBehavior.AllowGet);
            }

            var bankSchool =
                _parameters.BankSchoolService.Select().FirstOrDefault(
                    x => x.SchoolId.Equals(model.SchoolId) && x.BankId.Equals(model.BankId));
            if (bankSchool != null)
            {
                return Json(new { Success = false, ErrorMessage = "Bank is already shared to this school." }, JsonRequestBehavior.AllowGet);
            }
            var newBankSchool = new BankSchool
            {
                SchoolId = model.SchoolId,
                EditedByUserId = CurrentUser.Id,
                BankId = model.BankId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.MaxValue
            };
            try
            {
                _parameters.BankSchoolService.Save(newBankSchool);
            }
            catch (Exception)
            {
                return Json(new { Success = false, ErrorMessage = "There was some error. Can not share bank for this school right now." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion Publish Bank To School

        [HttpGet, AjaxOnly]
        public ActionResult AssignAuthorGroup(int? bankID)
        {
            var model = new AssignAuthorGroupModel();
            model.BankID = bankID;
            model.IsPublisher = CurrentUser.IsPublisher();
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_AssignAuthorGroup", model);
        }

        [HttpPost, AjaxOnly]
        public ActionResult SearchAuthorGroups(int bankID)
        {
            var user = _parameters.UserService.GetUserById(CurrentUser.Id);

            if (user != null)
            {
                CurrentUser.SchoolId = user.SchoolId;
            }
            var model = new AuthorGroupListViewModel
            {
                IsDistrictAdmin = CurrentUser.RoleId.Equals((int)Permissions.DistrictAdmin),
                IsSchoolAdmin = CurrentUser.IsSchoolAdmin,
                IsPublisher = CurrentUser.IsPublisher,
                IsTeacher = CurrentUser.IsTeacher,
                DistrictId = CurrentUser.DistrictId.GetValueOrDefault(),
                StateId = CurrentUser.StateId.GetValueOrDefault(),
                SchoolId = CurrentUser.SchoolId.GetValueOrDefault(),
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                BankID = bankID
            };

            return PartialView("_SearchAuthorGroups", model);
        }

        [HttpPost, AjaxOnly]
        public ActionResult AddAuthorGroupBank(int id, int authorGroupID)
        {
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, id, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (!_parameters.VulnerabilityService.HasRigtToAccessAuthorGroup(CurrentUser, authorGroupID))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            _parameters.AuthorGroupService.AddAuthorGroupBank(authorGroupID, id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, AjaxOnly]
        public ActionResult RemoveAuthorGroupBank(int id, int authorGroupID)
        {
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, id, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            _parameters.AuthorGroupService.RemoveAuthorGroupBank(authorGroupID, id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateBankArchive(int bankId, bool archived)
        {
            if (bankId > 0)
            {
                try
                {
                    _parameters.BankServices.UpdateBankArchive(bankId, archived);
                    return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    return Json(new { Data = "Update archived fail." }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Data = "Update archived fail." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, AjaxOnly]
        public ActionResult SaveRestrictionForm(int bankId, int bankDistrictId, List<TestRestrictionModuleMatrixDTO> listmoduleroles)
        {
            var districtbank = _parameters.BankDistrictService.GetBankDistrictById(bankDistrictId);
            var user = _parameters.UserService.GetUserById(CurrentUser.Id);
            if (districtbank != null)
                this._parameters.TestRestrictionModuleService.SaveTestRestrictionModule(new SaveTestRestrictionModuleRequestDTO
                {
                    ModifiedUser = user.Id,
                    ModifiedDate = DateTime.Now,
                    PublishedLevelID = districtbank.DistrictId,
                    PublishedLevelName = "district",
                    RestrictedObjectID = bankId,
                    RestrictedObjectName = "bank",
                    ListTestRestrictionModulesRoleMatrix = listmoduleroles,
                    Roleuserid = user.RoleId
                });
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveVirtualTestRestrictionForm(int testId, int districtId, List<TestRestrictionModuleMatrixDTO> listmoduleroles)
        {
            //var districtbank = _parameters.BankDistrictService.GetBankDistrictById(bankDistrictId);
            var user = _parameters.UserService.GetUserById(CurrentUser.Id);
            this._parameters.TestRestrictionModuleService.SaveTestRestrictionModule(new SaveTestRestrictionModuleRequestDTO
            {
                ModifiedUser = user.Id,
                ModifiedDate = DateTime.Now,
                PublishedLevelID = districtId,
                PublishedLevelName = "district",
                RestrictedObjectID = testId,
                RestrictedObjectName = "test",
                ListTestRestrictionModulesRoleMatrix = listmoduleroles,
                Roleuserid = user.RoleId
            });
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadRestrictionFormTest()
        {
            var list = this._parameters.TestRestrictionModuleService.
                GetTestRestrictionModuleRoleByBankAndDistrict(new GetTestRestrictionModuleRoleByBankAndDistrictReuqestDTO
                {
                    PublishedLevelID = 123,
                    PublishedLevelName = "district",
                    RestrictedObjectID = 234,
                    RestrictedObjectName = "bank",
                    Rolename = "Administrator"
                });
            //list[0].ListRoles[0].IsChecked = true;
            //list[0].ListRoles[1].IsChecked = true;
            //list[0].ListRoles[2].IsChecked = true;

            //this._parameters.TestRestrictionModuleService.SaveTestRestrictionModule(new SaveTestRestrictionModuleRequestDTO
            //{
            //    ModifiedUser = 12,
            //    ModifiedDate = DateTime.Now,
            //    PublishedLevelID = 123,
            //    PublishedLevelName = "district",
            //    RestrictedObjectID = 234,
            //    RestrictedObjectName = "bank",
            //    ListTestRestrictionModules= list

            //});
            return PartialView("_RestrictionForm");
        }

        public ActionResult LoadRestrictionForm(int bankId, int bankDistrictId)
        {
            var districtbank = _parameters.BankDistrictService.GetBankDistrictById(bankDistrictId);

            var user = _parameters.UserService.GetUserById(CurrentUser.Id);
            var list = new List<TestRestrictionModuleDTO>();
            if (districtbank != null)
            {
                list = this._parameters.TestRestrictionModuleService.
                 GetTestRestrictionModuleRoleByBankAndDistrict(new GetTestRestrictionModuleRoleByBankAndDistrictReuqestDTO
                 {
                     PublishedLevelID = districtbank.DistrictId,
                     PublishedLevelName = "district",
                     RestrictedObjectID = bankId,
                     RestrictedObjectName = "bank",
                     RoleID = user.RoleId,
                     UserDistrictID = user.DistrictId
                 });
            }
            ViewBag.DistrictId = bankDistrictId;
            ViewBag.bankId = bankId;

            return PartialView("_RestrictionForm", list);
        }

        public ActionResult LoadVirtualTestRestrictionForm(int bankId, int testId)
        {
            IQueryable<BankPublishedDistrictViewModel> data = _parameters.BankDistrictService.GetBankDistrictByBankId(bankId)
              .Select(x => new BankPublishedDistrictViewModel
              {
                  DistrictId = x.DistrictId,
                  Name = x.Name,
                  BankDistrictId = x.BankDistrictId,
                  BankId = x.BankId
              }
              );
            if (CurrentUser.IsDistrictAdmin)
            {
                data = data.Where(x => x.DistrictId == CurrentUser.DistrictId);
            }
            if (CurrentUser.IsNetworkAdmin)
            {
                data = data.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId));
            }

            var datadistrictpublish = new List<BankPublishedDistrictViewModel>();

            datadistrictpublish = data.ToList();
            datadistrictpublish.Insert(0, new BankPublishedDistrictViewModel
            {
                Name = "Select District",
                DistrictId = 0
            });
            ViewBag.datadistrictpublish = datadistrictpublish;
            //list.Count
            ViewBag.bankId = bankId;
            ViewBag.testId = testId;

            return PartialView("_RestrictionVirtualTestForm");
        }

        public ActionResult LoadVirtualTestRestrictionDataHTML(int bankId, int testId, int districtId, bool? layoutV2 = false)
        {
            var user = _parameters.UserService.GetUserById(CurrentUser.Id);
            var list = new List<TestRestrictionModuleDTO>();
            if (bankId > 0 && testId > 0 && districtId > 0)
            {
                list = this._parameters.TestRestrictionModuleService.
                    GetTestRestrictionModuleRoleByBankTestAndDistrict(new GetTestRestrictionModuleRoleByBankAndDistrictReuqestDTO
                    {
                        PublishedLevelID = districtId,
                        PublishedLevelName = "district",
                        RestrictedObjectID = testId,
                        RestrictedObjectName = "test",
                        RoleID = user.RoleId,
                        BankID = bankId,
                        UserDistrictID = user.DistrictId
                    });
            }
            //list.Count
            ViewBag.bankId = bankId;
            ViewBag.testId = testId;
            ViewBag.districtId = districtId;
            if (layoutV2 == true)
            {
                return PartialView("v2/_RestrictionVirtualTestDataHTML", list);

            }
            return PartialView("_RestrictionVirtualTestDataHTML", list);
        }

        private GetBanksByUserIDFilter MappingRequest(GetBanksByUserIDRequest request)
        {
            var getBanksByUserIDFilter = new GetBanksByUserIDFilter
            {
                UserID = CurrentUser.Id,
                RoleID = CurrentUser.RoleId,
                DistrictID = CurrentUser.DistrictId.GetValueOrDefault(),
                PageIndex = request.iDisplayStart,
                PageSize = request.iDisplayLength,
                GeneralSearch = request.sSearch
            };

            if (!string.IsNullOrWhiteSpace(request.sColumns) && request.iSortCol_0.HasValue)
            {
                var columns = request.sColumns.Split(',');
                getBanksByUserIDFilter.SortColumn = columns[request.iSortCol_0.Value];
                getBanksByUserIDFilter.SortDirection = string.Compare(request.sSortDir_0, "desc") == 0 ? "DESC" : "ASC";
            }

            return getBanksByUserIDFilter;
        }

        [HttpGet]
        public ActionResult ExportTestPropertyToCSV(string strVirtualTestID)
        {
            try
            {
                var virtualTestIDs = strVirtualTestID.Split(',')
                .Select(x => int.TryParse(x, out int virtualTestID) ? virtualTestID : default)
                .Where(x => x != default);

                if (!virtualTestIDs.Any()) return RedirectToAction("Index");

                var result = _parameters.VirtualTestService.ProcessExportTestProperty(CurrentUser.DistrictId.GetValueOrDefault(), virtualTestIDs.ToIntCommaSeparatedString());

                return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = int.MaxValue };
            }
            catch (Exception ex)
            {
                return new JsonResult() { Data = "ExportTestPropertyToCSV-Exception:" + ex.ToString(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            
        }


        public ActionResult ChooseTemplate(int virtualTestId, int? templateId)
        {
            var templates = _parameters.VirtualTestCustomScoreService.Select()
                .Where(x => x.DistrictId == CurrentUser.DistrictId)
                .OrderBy(x => x.Name)
                .Select(x => new ListItem
                {
                    Id = x.VirtualTestCustomScoreId,
                    Name = x.Name
                })
                .ToList();

            templates.Insert(0, new ListItem
            {
                Id = 0,
                Name = "Choose Template"
            });

            ViewBag.Templates = templates;
            ViewBag.TemplateId = templateId ?? 0;
            ViewBag.VirtualTestId = virtualTestId;

            return PartialView("_ChooseTemplate");
        }

        [HttpPost, AjaxOnly]
        public ActionResult SaveTemplateForVirtualTest(SaveTemplateForVirtualTestRequest request)
        {
            var template = _parameters.VirtualTestVirtualTestCustomScoreService.GetByVirtualTestId(request.VirtualTestId);
            if (request.TemplateId > 0 && template == null)
            {
                _parameters.VirtualTestVirtualTestCustomScoreService.Save(new VirtualTestVirtualTestCustomScore
                {
                    VirtualTestId = request.VirtualTestId,
                    VirtualTestCustomScoreId = request.TemplateId.Value
                });
            }
            else if (request.TemplateId > 0)
            {
                template.VirtualTestCustomScoreId = request.TemplateId.Value;
                _parameters.VirtualTestVirtualTestCustomScoreService.Save(template);
            }
            else if (template != null)
            {
                _parameters.VirtualTestVirtualTestCustomScoreService.Delete(template);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
