using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.ManageTest;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker;
using LinkIt.BubbleSheetPortal.Common.Enum;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]

    public class DataLockerEntryFormController : BaseController
    {
        private readonly DataLockerControllerParameters _parameters;
        private readonly IS3Service s3Service;

        public DataLockerEntryFormController(
            DataLockerControllerParameters parameters,
            IS3Service s3Service)
        {
            _parameters = parameters;
            this.s3Service = s3Service;
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.BuildEntryForms)]
        public ActionResult Index()
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

        #region Form Test

        public ActionResult LoadTemplateCreateForm(int? districtId)
        {
            int currentdistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            if (districtId.HasValue && districtId > 0)
            {
                currentdistrictId = districtId.Value;
            }

            try
            {
                List<int> publishedTemplateIds = _parameters.DataLockerTemplateService.GetPublishedTemplateIdsFromDistrict(currentdistrictId).ToList();
                //TODO: update VirtualTestTypeId = 5 ( result Entry ).
                var data = _parameters.VirtualTestCustomScoreService.Select()
                    .Where(o => o.Archived == false && (o.DistrictId == CurrentUser.DistrictId.GetValueOrDefault()
                            && o.DataSetOriginID == (int)DataSetOriginEnum.Data_Locker
                            && o.DistrictId == currentdistrictId)
                            || publishedTemplateIds.Contains(o.VirtualTestCustomScoreId));

                var useMultiDate = _parameters.DistrictDecodeService.GetDistrictDecodeOrConfigurationByLabel(currentdistrictId, Constanst.UseMultiDateTemplate);
                if (!useMultiDate)
                    data = data.Where(x => !(x.IsMultiDate.HasValue && x.IsMultiDate.Value));

                var result = data.Select(x => new ListItem { Id = x.VirtualTestCustomScoreId, Name = x.Name })
                .OrderBy(o => o.Name);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { success = false, Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadVirtualTests(int bankId)
        {
            var parser = new DataTableParser<VirtualTestListItemViewModel>();

            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                //return empty list
                return Json(parser.Parse(new List<VirtualTestListItemViewModel>().AsQueryable(), true), JsonRequestBehavior.AllowGet);
            }
            var districtId = CurrentUser.DistrictId.GetValueOrDefault();
            var usingMultiDate = _parameters.DistrictDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.UseMultiDateTemplate);
            var query = _parameters.DataLockerService.GetFormsByBankID(bankId, null, usingMultiDate);

            //TODO: VirtualtestType will update correct for Result Entry
            var data = query.Select(o => new VirtualTestListItemViewModel()
            {
                Name = Server.HtmlEncode(o.Name),
                VirtualTestId = o.Id
            }).ToList();

            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LoadPropertiesTestBank(int bankId)
        {
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                //return empty model
                return PartialView("_FormTestProperties", new TestBankPropertiesViewModel());
            }

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
                bankSchoolName = string.Join(", ", bankSchools.Select(x => x.Name).ToList());
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

            return PartialView("_FormBankProperties", vBank);
        }

        [HttpGet]
        public ActionResult LoadPropertiesVirtualTest(int virtualTestId)
        {
            string strTemplateName = string.Empty;
            VirtualTestProperty vtd = _parameters.VirtualTestService.GetVirtualTestProperty(virtualTestId, CurrentUser.RoleId, CurrentUser.DistrictId.GetValueOrDefault());
            var vv = _parameters.VirtualTestVirtualTestCustomScoreService.Select()
                .FirstOrDefault(o => o.VirtualTestId == virtualTestId);
            if (vv != null)
            {
                var objTemplate =
                    _parameters.VirtualTestCustomScoreService.Select()
                        .FirstOrDefault(o => o.VirtualTestCustomScoreId == vv.VirtualTestCustomScoreId);
                if (objTemplate != null)
                    strTemplateName = objTemplate.Name;
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
                TemplateName = strTemplateName,
                Instruction = vtd.Instruction.ReplaceWeirdCharacters(),
                DataSetCategoryID = vtd.DataSetCategoryID
            };
            ViewBag.IsIsPublisherOrNetworkAdmin = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin;
            ViewBag.AllowChangeDataSetCategory = DoesEnableAbilityToChangeTestCategory(CurrentUser.DistrictId);
            return PartialView("_FormTestProperties", vTest);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateTest(int bankId, int testId, string testName, int? dataSetCategoryId)
        {
            testName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(testName));
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, Error = "Has no right to update this form." }, JsonRequestBehavior.AllowGet);
            }

            testName = RemoveLineBreak(testName);

            if (_parameters.VirtualTestService.ExistTestNameUpdate(bankId, testId, testName))
            {
                return Json(new { success = false, Error = "A form with the name " + testName + " already exists in this bank." }, JsonRequestBehavior.AllowGet);
            }
            if (_parameters.VirtualTestService.UpdateTestName(testId, testName, dataSetCategoryId))
            {
                var s3VirtualTest = _parameters.VirtualTestService.CreateS3Object(testId);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, s3Service);

                if (!s3Result.IsSuccess)
                {
                    return
                        Json(
                            new
                            {
                                success = false,
                                Error =
                                "Form has been updated successfully but uploading json file to S3 fail: " +
                                s3Result.ErrorMessage
                            }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true });
            }

            return Json(new { success = false, Error = "Can not update form right now." }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateRubricDescription(int bankId, int testId, string description)
        {
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, Error = "Has no right to update this form." }, JsonRequestBehavior.AllowGet);
            }

            description = description.Trim().ConvertFromUnicodeToWindow1252();
            var test = _parameters.VirtualTestService.GetTestById(testId);
            if (test != null)
            {
                test.Instruction = description;
                _parameters.VirtualTestService.Save(test);
                return Json(new { success = true });
            }
            return Json(new { success = false, Error = "Can not update form right now." }, JsonRequestBehavior.AllowGet);
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
                obj.Name = "This form has test results and cannot be deleted.";
                obj.Id = 0;
            }
            else if (canDeleteVirtualTest == 1 || canDeleteVirtualTest == 2)
            {
                ViewBag.DeleteTestTitle = "Delete Form Confirmation";
                obj.Id = virtualTestId;
                obj.Name = string.Format("Are you sure you want to delete {0}. This action will permanently delete the form and all questions contained in it. Are you sure you want to continue?", strTestName);
                if (canDeleteVirtualTest == 2)
                    obj.Name =
                        "This form has online test assignments associated with it which can no longer be accessed once this form is deleted. Are you sure you want to delete this form.";
            }
            return PartialView("_ConfirmDeleteVirtualTest", obj);
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
                    return Json(new { Success = "Fail", Error = "Form does not exist." }, JsonRequestBehavior.AllowGet);
                }
                if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", Error = "Has no right to move this form." }, JsonRequestBehavior.AllowGet);
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
                    return Json(new { Success = "Fail", Error = "The destination form bank already contains a form with the same name. Please rename the form prior to moving it to this form bank or move it to another bank." });
            }
            return Json(new { Success = "Fail", Error = "Invalid data post" });
        }

        public ActionResult LoadCreateVirtualTest()
        {
            var obj = new TestFormViewModel()
            {
                IsAdmin = IsUserAdmin(),
                IsSchoolAdmin = CurrentUser.RoleId.Equals(8),
                IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher),
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                AllowChangeDataSetCategory = DoesEnableAbilityToChangeTestCategory(CurrentUser.DistrictId)
            };
            return PartialView("_CreateVirtualTest", obj);
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
            return districtDecode?.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Any(c => string.Compare(c, "DataLocker", true) == 0) ?? false;
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateInput(false)]
        public ActionResult CreateVirtualTest(int bankId, string strTestName, int templateId, int? datasetCategoryID)
        {
            if (!DoesEnableAbilityToChangeTestCategory(CurrentUser.DistrictId) && datasetCategoryID.HasValue
                && datasetCategoryID.Value != (int)DataSetCategoryEnum.DATA_LOCKER)
            {
                return Json(new { success = false, Error = "Invalid Category" }, JsonRequestBehavior.AllowGet);
            }
            if (_parameters.VirtualTestService.ExistTestName(bankId, strTestName))
            {
                return Json(new { success = false, Error = "A form with the name " + strTestName + " already exists in this bank." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var vBank = _parameters.BankServices.GetBankById(bankId);
                var vSubejct = _parameters.SubjectServices.GetSubjectById(vBank.SubjectID);
                var vitualTestCustomScore = GetCustomScoreById(templateId);
                int virtualTestSubTypeId = (int)VirtualTestSubType.Default;
                int testScoreMethodId = (int)TestScoreMethodEnum.Default;

                if (vitualTestCustomScore.VirtualTestSubTypeId == (int)VirtualTestSubType.NewACT ||
                    vitualTestCustomScore.VirtualTestSubTypeId == (int)VirtualTestSubType.NewSAT)
                {
                    switch (vitualTestCustomScore.VirtualTestSubTypeId)
                    {
                        case (int)VirtualTestSubType.NewACT:
                            virtualTestSubTypeId = (int)VirtualTestSubType.NewACT;
                            testScoreMethodId = (int)TestScoreMethodEnum.New_ACT;
                            break;

                        case (int)VirtualTestSubType.NewSAT:
                            virtualTestSubTypeId = (int)VirtualTestSubType.NewSAT;
                            testScoreMethodId = (int)TestScoreMethodEnum.New_SAT;
                            break;

                        default:
                            break;
                    }
                }

                var vTest = new VirtualTestData()
                {
                    BankID = bankId,
                    Name = strTestName,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    AuthorUserID = CurrentUser.Id,
                    VirtualTestType = (int)VirtualTestTypeModule.ResultsEntryDataLocker, //TODO: create new VirtualTestType for ResultEntry ( 5 ).
                    VirtualTestSourceID = (int)VirtualTestSourceEnum.Legacy,
                    StateID = vSubejct.StateId,
                    VirtualTestSubTypeID = virtualTestSubTypeId,
                    TestScoreMethodID = testScoreMethodId,
                    AchievementLevelSettingID = (int)AchievementLevelSettingEnum.Custom,
                    DatasetOriginID = (int)DataSetOriginEnum.Data_Locker,
                    DatasetCategoryID = datasetCategoryID
                };
                _parameters.VirtualTestService.Save(vTest);
                var vtvc = new VirtualTestVirtualTestCustomScore()
                {
                    VirtualTestId = vTest.VirtualTestID,
                    VirtualTestCustomScoreId = templateId
                };
                _parameters.VirtualTestVirtualTestCustomScoreService.Save(vtvc);
                //TODO: This kind of Test, no need to create questions
                var s3VirtualTest = _parameters.VirtualTestService.CreateS3Object(vTest.VirtualTestID);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, s3Service);

                if (!s3Result.IsSuccess)
                {
                    return
                        Json(
                            new
                            {
                                success = false,
                                Error =
                                "Form has been created successfully but uploading json file to S3 fail: " +
                                s3Result.ErrorMessage
                            }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, virtualTestId = vTest.VirtualTestID }, JsonRequestBehavior.AllowGet);
            }
        }

        private VirtualTestCustomScore GetCustomScoreById(int templateId)
        {
            var virtualTestCustomScore = _parameters.VirtualTestCustomScoreService.GetCustomScoreByID(templateId);
            return virtualTestCustomScore;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CopyVirtualTest(int virtualTestId, int newBankId, string testName)
        {
            if (virtualTestId > 0 && newBankId > 0 && !string.IsNullOrEmpty(testName))
            {
                //check to avoid modifying ajax parameter bankId)
                VirtualTestData v = _parameters.VirtualTestService.GetTestById(virtualTestId);
                if (v == null)
                {
                    return Json(new { Success = "Fail", Error = "Form does not exist." }, JsonRequestBehavior.AllowGet);
                }
                if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, v.BankID, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", Error = "Has no right to move this form." }, JsonRequestBehavior.AllowGet);
                }
                if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, newBankId, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", Error = "Has no right on new bank." }, JsonRequestBehavior.AllowGet);
                }

                if (_parameters.VirtualTestService.ExistTestName(newBankId, testName))
                {
                    return Json(new { Success = "Fail", Error = "Exist Form Name '" + testName + "'" });
                }

                v.BankID = newBankId;
                v.Name = testName;
                CopyVirtualTest(v);
                return Json(new { Success = "Success" });
            }
            return Json(new { Success = "Fail", Error = "Invalid data post" });
        }

        public ActionResult LoadTestBankforMove(int currentBankId, bool isCopy)
        {
            //TODO: get list here;
            var vdata = _parameters.ManageTestService.GetFormBanksByUserID(CurrentUser.Id, CurrentUser.RoleId,
                CurrentUser.SchoolId.GetValueOrDefault(), CurrentUser.DistrictId.GetValueOrDefault(), null);
            if (!isCopy)
            {
                vdata = vdata.Where(o => o.BankID != currentBankId).ToList();
            }
            var data = vdata.Where(x => !x.Archived).Select(o => new TestBankMoveViewModel()
            {
                BankID = o.BankID,
                BankName = Server.HtmlEncode(o.BankName),
                Subject = Server.HtmlEncode(o.SubjectName),
                GradeOrder = o.GradeOrder,
                Grade = Server.HtmlEncode(o.GradeName),
            });
            var parser = new DataTableParser<TestBankMoveViewModel>();
            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteVirtualTest(int virtualTestId)
        {
            //check to avoid modifying ajax parameter bankId)
            var virtualTest = _parameters.VirtualTestService.GetTestById(virtualTestId);
            if (virtualTest == null)
            {
                return Json(new { success = false, errorMessage = "Form does not exist." }, JsonRequestBehavior.AllowGet);
            }
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, virtualTest.BankID, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, errorMessage = "Has no right to delete this form." }, JsonRequestBehavior.AllowGet);
            }
            string error = string.Empty;
            _parameters.DataLockerTemplateService.DeleteVirtualTestLegacyById(virtualTestId, CurrentUser.Id, CurrentUser.RoleId, out error);
            if (string.IsNullOrWhiteSpace(error))
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, errorMessage = error }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Form Test

        #region Form Bank

        public ActionResult LoadTestBanks(int? districtId, bool? showArchived, bool? hideBankOnlyTest)
        {
            if (districtId == null)
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            //TODO: get list here;
            var data = _parameters.ManageTestService.GetFormBanksByUserID(CurrentUser.Id, CurrentUser.RoleId,
                CurrentUser.SchoolId.GetValueOrDefault(), districtId.GetValueOrDefault(), hideBankOnlyTest, showArchived ?? false, false)
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
            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
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

        public ActionResult CanDeleteBank(int bankId)
        {
            bool canDeleteBank = _parameters.BankServices.CanDeleteBankByID(bankId);
            return Json(new { canDeleteBank }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteTestBank(int bankId)
        {
            //check to avoid modifying ajax parameter bankId)
            if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { success = false, error = "Has no right to delete form bank." }, JsonRequestBehavior.AllowGet);
            }
            //TODO: Comment for Mockup
            _parameters.BankServices.DeleteBankByID(bankId);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateBankArchive(int bankId, bool archived)
        {
            if (bankId > 0)
            {
                try
                {
                    _parameters.BankServices.UpdateBankArchive(bankId, archived);
                    if (archived)
                    {
                        var publishDistricts = _parameters.BankDistrictService.GetBankDistrictByBankId(bankId)
                                        .Select(x => new BankPublishedDistrictViewModel
                                        {
                                            DistrictId = x.DistrictId,
                                            Name = x.Name,
                                            BankDistrictId = x.BankDistrictId,
                                            BankId = x.BankId
                                        }
                                        );

                        var publishSchools = _parameters.BankSchoolService.GetBankSchoolByBankId(bankId);

                        if (CurrentUser.IsDistrictAdmin)
                        {
                            publishDistricts = publishDistricts.Where(x => x.DistrictId == CurrentUser.DistrictId);
                            publishSchools = publishSchools.Where(x => x.DistrictId == CurrentUser.DistrictId);
                        }
                        if (CurrentUser.IsNetworkAdmin)
                        {
                            publishDistricts = publishDistricts.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId));
                            publishSchools = publishSchools.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId));
                        }
                        //un publish distric
                        if (publishDistricts != null && publishDistricts.Count() > 0)
                        {
                            foreach (var publishDistrict in publishDistricts)
                            {
                                var bankDistrict = _parameters.BankDistrictService.GetBankDistrictById(publishDistrict.BankDistrictId);                               
                                //check to avoid modifying ajax parameter bankId)
                                if (!bankDistrict.IsNull() && _parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankDistrict.BankId, CurrentUser.GetMemberListDistrictId())
                                    && Util.HasRightOnDistrict(CurrentUser, bankDistrict.DistrictId))
                                {
                                    _parameters.BankDistrictService.Delete(bankDistrict);
                                }                            
                            }
                        }
                        //un publish for school
                        if (publishSchools != null && publishSchools.Count() > 0)
                        {
                            foreach (var publishSchool in publishSchools)
                            {
                                var bankSchool = _parameters.BankSchoolService.GetBankSchoolById(publishSchool.BankSchoolId);
                                if (!bankSchool.IsNull() && _parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankSchool.BankId, CurrentUser.GetMemberListDistrictId())
                                    && GetPublishedSchools(bankSchool.BankId).ToList().All(x => x.BankSchoolId != publishSchool.BankSchoolId) == false)
                                {
                                    _parameters.BankSchoolService.Delete(bankSchool);
                                }
                            }
                        }                        
                    }
                   
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

        [HttpPost]
        public ActionResult SaveEnterResults()
        {
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadTestBanksForNetworkAdmin(int? stateId, int? districtId, bool? showArchived, bool? hideBankOnlyTest)
        {
            var data = new List<BankData>();
            if (districtId.HasValue)
            {
                data = _parameters.ManageTestService.GetFormBanksByUserID(CurrentUser.Id, CurrentUser.RoleId,
                    CurrentUser.SchoolId.GetValueOrDefault(), districtId.GetValueOrDefault(), hideBankOnlyTest, showArchived ?? false);
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
                        data.AddRange(_parameters.ManageTestService.GetFormBanksByUserID(CurrentUser.Id, CurrentUser.RoleId,
                            CurrentUser.SchoolId.GetValueOrDefault(), memberDistrict.Id, hideBankOnlyTest, showArchived ?? false));
                    }
                }
                else
                {
                    //get all member districts
                    var memberDistricts =
                        _parameters.DistrictServices.GetDistricts()
                            .Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id)).ToList();
                    foreach (var memberDistrict in memberDistricts)
                    {
                        data.AddRange(_parameters.ManageTestService.GetFormBanksByUserID(CurrentUser.Id, CurrentUser.RoleId,
                            CurrentUser.SchoolId.GetValueOrDefault(), memberDistrict.Id, hideBankOnlyTest, showArchived ?? false));
                    }
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
                });
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
                BankAccessID = (int)BankAccessEnum.Open,
                CreatedByUserId = CurrentUser.Id,
                Name = strTestName.Trim(),
                SubjectID = subjectId,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };
            _parameters.BankServices.Save(bank);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CloneTestBank(int virtualTestId, string virtualTestName)
        {            
            try
            {
                var bankCreate = _parameters.BankServices.GetBankById(virtualTestId);
                //check subjectId
                if (!_parameters.VulnerabilityService.HasRigtToAccessSubject(CurrentUser, bankCreate.SubjectID, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = false, Error = "Has no right on the subject." }, JsonRequestBehavior.AllowGet);
                }
                //insert bank
                var bank = new Bank()
                {
                    BankAccessID = (int)BankAccessEnum.Open,
                    CreatedByUserId = CurrentUser.Id,
                    Name = virtualTestName.Trim(),
                    SubjectID = bankCreate.SubjectID,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                _parameters.BankServices.Save(bank);
                //get virtual test by bank
                var virtualTests = _parameters.VirtualTestService.GetVirtualTestByFormBankId(virtualTestId);
                if (virtualTests != null && virtualTests.Count() > 0)
                {
                    foreach (var virtualTest in virtualTests)
                    {
                        //insert virtual test bank
                        var vSubject = _parameters.SubjectServices.GetSubjectById(bankCreate.SubjectID);
                        var virtualTestCustomScore = _parameters.VirtualTestVirtualTestCustomScoreService.GetByVirtualTestId(virtualTest.VirtualTestCustomScoreId);
                        var vitualTestCustomScore = GetCustomScoreById(virtualTestCustomScore.VirtualTestCustomScoreId);
                        int virtualTestSubTypeId = (int)VirtualTestSubType.Default;
                        int testScoreMethodId = (int)TestScoreMethodEnum.Default;
                        if (vitualTestCustomScore.VirtualTestSubTypeId == (int)VirtualTestSubType.NewACT ||
                            vitualTestCustomScore.VirtualTestSubTypeId == (int)VirtualTestSubType.NewSAT)
                        {
                            switch (vitualTestCustomScore.VirtualTestSubTypeId)
                            {
                                case (int)VirtualTestSubType.NewACT:
                                    virtualTestSubTypeId = (int)VirtualTestSubType.NewACT;
                                    testScoreMethodId = (int)TestScoreMethodEnum.New_ACT;
                                    break;
                                case (int)VirtualTestSubType.NewSAT:
                                    virtualTestSubTypeId = (int)VirtualTestSubType.NewSAT;
                                    testScoreMethodId = (int)TestScoreMethodEnum.New_SAT;
                                    break;
                                default:
                                    break;
                            }
                        }

                        var vTest = new VirtualTestData()
                        {
                            BankID = bank.Id,
                            Name = virtualTest.Name,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow,
                            AuthorUserID = CurrentUser.Id,
                            VirtualTestType = (int)VirtualTestTypeModule.ResultsEntryDataLocker, //TODO: create new VirtualTestType for ResultEntry ( 5 ).
                            VirtualTestSourceID = (int)VirtualTestSourceEnum.Legacy,
                            StateID = vSubject.StateId,
                            VirtualTestSubTypeID = virtualTestSubTypeId,
                            TestScoreMethodID = testScoreMethodId,
                            AchievementLevelSettingID = (int)AchievementLevelSettingEnum.Custom,
                            DatasetOriginID = (int)DataSetOriginEnum.Data_Locker,
                            DatasetCategoryID = virtualTest.DatasetCategoryID
                        };
                        _parameters.VirtualTestService.Save(vTest);
                        var vtvc = new VirtualTestVirtualTestCustomScore()
                        {
                            VirtualTestId = vTest.VirtualTestID,
                            VirtualTestCustomScoreId = virtualTestCustomScore.VirtualTestCustomScoreId
                        };
                        _parameters.VirtualTestVirtualTestCustomScoreService.Save(vtvc);
                        //TODO: This kind of Test, no need to create questions
                        var s3VirtualTest = _parameters.VirtualTestService.CreateS3Object(vTest.VirtualTestID);
                        var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, s3Service);

                        if (!s3Result.IsSuccess)
                        {
                            return
                                Json(
                                    new
                                    {
                                        success = false,
                                        Error =
                                        "Form has been created successfully but uploading json file to S3 fail: " +
                                        s3Result.ErrorMessage
                                    }, JsonRequestBehavior.AllowGet);
                        }                        
                    }
                }
                //Clone publish to district
                var bankPublishedDistricts = _parameters.BankDistrictService.GetBankDistrictByBankId(virtualTestId)
                    .Select(x => new BankPublishedDistrictViewModel
                    {
                        DistrictId = x.DistrictId,
                        Name = x.Name,
                        BankDistrictId = x.BankDistrictId,
                        BankId = x.BankId
                    }
                );
                var bankPublishedSchools = _parameters.BankSchoolService.GetBankSchoolByBankId(virtualTestId);
                if (CurrentUser.IsDistrictAdmin)
                {
                    bankPublishedDistricts = bankPublishedDistricts.Where(x => x.DistrictId == CurrentUser.DistrictId);
                    bankPublishedSchools = bankPublishedSchools.Where(x => x.DistrictId == CurrentUser.DistrictId);
                }
                if (CurrentUser.IsNetworkAdmin)
                {
                    bankPublishedDistricts = bankPublishedDistricts.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId));
                    bankPublishedSchools = bankPublishedSchools.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId));
                }
                //publish district for bank

                if (bankPublishedDistricts != null && bankPublishedDistricts.Count() > 0)
                {
                    foreach (var bankPublishedDistrict in bankPublishedDistricts)
                    {
                        var newBankDistrict = new BankDistrict
                        {
                            DistrictId = bankPublishedDistrict.DistrictId,
                            EditedByUserId = CurrentUser.Id,
                            BankId = bank.Id,
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.MaxValue,
                            BankDistrictAccessId = 1
                        };
                        _parameters.BankDistrictService.Save(newBankDistrict);
                    }
                }                

                //Clone publish for school
                if (bankPublishedSchools != null && bankPublishedSchools.Count() > 0)
                {
                    //check to avoid modifying ajax parameter bankId)
                    if (_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bankCreate.Id, CurrentUser.GetMemberListDistrictId()))
                    {
                        foreach (var bankPublishedSchool in bankPublishedSchools)
                        {
                            var newBankSchool = new BankSchool
                            {
                                SchoolId = bankPublishedSchool.SchoolId,
                                EditedByUserId = CurrentUser.Id,
                                BankId = bank.Id,
                                StartDate = DateTime.UtcNow,
                                EndDate = DateTime.MaxValue
                            };
                            _parameters.BankSchoolService.Save(newBankSchool);
                        }
                    }
                }          
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }            
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadCloneBankConfirmDialog(int virtualTestId, string virtualTestName)
        {
            ViewBag.VirtualTestId = virtualTestId;
            ViewBag.VirtualTestName = virtualTestName;
            return PartialView("_CloneBankConfirm");
        }

        #endregion Form Bank

        #region Publish Bank To " + LabelHelper.DistrictLabel + "

        public ActionResult LoadListOrShareDistrict(int bankId)
        {
            return PartialView("_ListOrShareDistrict", bankId);
        }

        public ActionResult LoadListDistrictByBank(int bankId)
        {
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_ListDistrictByBank", bankId);
        }

        [HttpGet]
        public ActionResult GetPublishedDistrict(int bankId)
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
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
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
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
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

            _parameters.BankDistrictService.Delete(bankDistrict);
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
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = false, ErrorMessage = "There was some error. Can not share bank for this school right now." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion Publish Bank To School

        #region Private method

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
            CloneVirtualTestTemplate(oldTestId, virtualTestData.VirtualTestID);
        }

        private void CloneVirtualTestTemplate(int oldTestId, int virtualTestId)
        {
            var objCustomScore = _parameters.VirtualTestVirtualTestCustomScoreService.Select()
                .FirstOrDefault(o => o.VirtualTestId == oldTestId);
            if (objCustomScore != null)
            {
                objCustomScore.VirtualTestId = virtualTestId;
                objCustomScore.VirtualTestVirtualTestCustomScoreId = 0;
                _parameters.VirtualTestVirtualTestCustomScoreService.Save(objCustomScore);
            }
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

        #endregion Private method

        [HttpGet]
        public ActionResult GetTests(int bankId, bool? isFromMultiDate)
        {
            var districtId = CurrentUser.DistrictId.GetValueOrDefault();
            var usingMultiDate = _parameters.DistrictDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.UseMultiDateTemplate);

            var virtualTests = _parameters.DataLockerService.GetFormsByBankID(bankId, isFromMultiDate, usingMultiDate).ToList();
            var virtualTestOrders = _parameters.VirtualTestService.GetVirtualTestOrders(districtId);

            if (!virtualTestOrders.Any())
                return Json(virtualTests, JsonRequestBehavior.AllowGet);

            var result = SortVirtualTests(virtualTests, virtualTestOrders);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<DTLFormModel> SortVirtualTests(List<DTLFormModel> virtualTests, List<VirtualTestOrder> virtualTestOrders)
        {
            var maxOrder = virtualTestOrders.Max(x => x.Order) + 100;

            var query = from test in virtualTests
                        join order in virtualTestOrders on test.Id equals order.VirtualTestID into ps
                        from p in ps.DefaultIfEmpty()
                        select new { test.Id, test.Name, test.IsMultiDate, Order = p == null ? maxOrder : p.Order };

            var list = query.OrderBy(x => x.Order).ThenBy(x => x.Name).ToList();

            var result = list.Select(o => new DTLFormModel
            {
                Id = o.Id,
                Name = o.Name,
                IsMultiDate = o.IsMultiDate
            }).ToList();

            return result;
        }
    }
}
