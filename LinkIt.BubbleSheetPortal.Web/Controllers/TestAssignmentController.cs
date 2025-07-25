using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using FluentValidation.Results;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.ManagePreference;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using Configuration = LinkIt.BubbleSheetPortal.Models.Configuration;
using Tag = LinkIt.BubbleSheetPortal.Models.ManagePreference.Tag;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories.Helper;
using LinkIt.BubbleSheetPortal.Data;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Models.DTOs.VirtualSection;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestPreferences;
using LinkIt.BubbleSheetPortal.Web.ViewModels.AssignmentRetake;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting;
using LinkIt.BubbleSheetPortal.Models.QuestionGroup;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Web.Constant;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Models.DTOs.Retake;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.Resolver;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;
using static DevExpress.Utils.Drawing.Helpers.NativeMethods;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using System.Globalization;
using LinkIt.BubbleSheetPortal.Models.DTOs.RetakeAssignment;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class TestAssignmentController : BaseController
    {
        private readonly VirtualTestService virtualTestService;
        private readonly TestAssignmentControllerParameters parameters;
        private readonly IValidator<TestAssignmentData> testAssignmentValidator;
        private readonly IConnectionString _connectionString;
        private readonly VirtualSectionService _virtualSectionService;
        private readonly StateService _stateService;
        private readonly IS3Service _s3Service;
        private readonly UserService _userService;
        private readonly BankService _bankService;

        private Dictionary<int, string> levels = new Dictionary<int, string>()
        {
            { (int)TestPreferenceLevel.Enterprise, ContaintUtil.TestPreferenceLevelEnterprise },
            { (int)TestPreferenceLevel.SurveyEnterprise, ContaintUtil.TestPreferenceLevelEnterprise },
            { (int)TestPreferenceLevel.District, ContaintUtil.TestPreferenceLevelDistrict },
            { (int)TestPreferenceLevel.SurveyDistrict, ContaintUtil.TestPreferenceLevelDistrict },
            { (int)TestPreferenceLevel.User, ContaintUtil.TestPreferenceLevelUser },
            { (int)TestPreferenceLevel.TestDesign, ContaintUtil.TestPreferenceLevelTest },
            { (int)TestPreferenceLevel.TestAssignment, ContaintUtil.TestPreferenceLevelTestAssignment },
            { (int)TestPreferenceLevel.School, ContaintUtil.TestPreferenceLevelSchool}
        };

        public TestAssignmentController(TestAssignmentControllerParameters parameters,
            VirtualTestService _virtualTestService,
            IValidator<TestAssignmentData> testAssignmentValidator, IConnectionString connectionString,
            VirtualSectionService virtualSectionService, StateService stateService, IS3Service s3Service,
            UserService userService, BankService bankService)
        {
            this.parameters = parameters;
            this.testAssignmentValidator = testAssignmentValidator;
            this.virtualTestService = _virtualTestService;
            this._connectionString = connectionString;
            this._virtualSectionService = virtualSectionService;
            _stateService = stateService;
            _s3Service = s3Service;
            _userService = userService;
            _bankService = bankService;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentRewrite)]
        public ActionResult Index()
        {
            //Session["PrintResult"] = null;
            var model = new TestAssignmentViewModel()
            {
                IsAdmin = IsUserAdmin(),
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher,
                IsTeacher = CurrentUser.IsTeacher,
                IsSchoolAdmin = CurrentUser.RoleId.Equals(8),
                IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher),
                DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0
            };
            model.IsNetworkAdmin = false;
            if (CurrentUser.IsNetworkAdmin)
            {
                model.IsNetworkAdmin = true;
                model.ListDistricIds = CurrentUser.GetMemberListDistrictId();
            }

            model.IsShowTutorialMode = parameters.DistrictDecodeService.GetDistrictDecodeOrConfigurationByLabel(model.DistrictId, Util.IsShowTutorialMode, true);

            model.UseRostersAtTimeOfTestTaking = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(ContaintUtil.UseRostersAtTimeOfTestTaking, true);
            model.UseRostersAtTimeOfTestTakingWording =
                parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(
                    ContaintUtil.UseRostersAtTimeOfTestTakingWording, ContaintUtil.UseRostersAtTimeOfTestTakingWordingDefault);
            if (CurrentUser.IsTeacher || CurrentUser.IsDistrictAdmin || CurrentUser.IsSchoolAdmin)
            {
                var userRoster =
                    parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                        CurrentUser.DistrictId.Value, ContaintUtil.UseRostersAtTimeOfTestTaking).FirstOrDefault();
                bool useRosterValue;
                if (userRoster != null && bool.TryParse(userRoster.Value, out useRosterValue))
                    model.UseRostersAtTimeOfTestTaking = useRosterValue;
            }

            model.EnableStudentLevelAssignment = parameters.QTITestStudentAssignmentServices.CheckAbleToAssignStudentLevel(model.DistrictId);
            model.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            model.IsLaunchTeacherLedTest = CheckIsLaunchTeacherLedTest();
            ViewBag.DateFormat = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;
            return View(model);
        }

        [HttpPost]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentRewrite)]
        public ActionResult Index(TestAssignmentData data, TestPreferenceModel objTestPreferenceModel)
        {
            // Restriction
            var isAllow = parameters.RestrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
            {
                BankId = data.BankId,
                TestId = data.TestId,
                DistrictId = data.DistrictId,
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                ModuleCode = RestrictionConstant.Module_Assign,
                ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test
            });

            if (isAllow == false)
            {
                return Json(new { error = "Permission to assign this test has been restricted." }, JsonRequestBehavior.AllowGet);
            }

            if (ValidatePermissionTestAssignmentData(data) == false)
            {
                return Json(new { error = "Has no right to the assignment." }, JsonRequestBehavior.AllowGet);
            }

            if (CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher)
            {
                if (objTestPreferenceModel != null && objTestPreferenceModel.OptionTags != null)
                {
                    var oldModel = GetTestPreferenceWithTestId(data.TestId, data.DistrictId, (int)TestPreferenceLevel.User, null);
                    var overrideAutoGradedTextEntryNewValue = objTestPreferenceModel.OptionTags.Single(x => x.Key == "overrideAutoGradedTextEntry").Value;
                    var overrideAutoGradedTextEntryOldValue = oldModel.TestPreferenceModel.OptionTags.Single(x => x.Key == "overrideAutoGradedTextEntry").Value;
                    if (overrideAutoGradedTextEntryNewValue != overrideAutoGradedTextEntryOldValue)
                    {
                        return Json(new { error = "Has no right to change 'Override Auto Graded Items'." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            var obj = new TestAssignResultViewModel();
            data.ObjTestPreferenceModel = parameters.PreferencesServices.ClearAllAtribute(objTestPreferenceModel);

            SetModelPermissions(data);
            if (data.UserId.Equals(0))
            {
                data.UserId = CurrentUser.Id;
            }
            data.SetValidator(testAssignmentValidator);

            //validate current user has permission to access school, class, student list, test
            if (ValidatePermissionTestAssignmentData(data) == false)
            {
                if (Session["PrintResult"] != null)
                {
                    obj.CurrentTab = ((TestAssignResultViewModel)Session["PrintResult"]).CurrentTab;
                }
                obj.ErrorList = new List<ValidationFailure>
                                {
                                    new ValidationFailure("Permission", "You do not have enough permission to this resource")
                                };
                return PartialView("_TestAssignResult", obj);
            }

            if (!data.IsValid)
            {
                if (Session["PrintResult"] != null)
                {
                    obj.CurrentTab = ((TestAssignResultViewModel)Session["PrintResult"]).CurrentTab;
                }
                obj.ErrorList = data.ValidationErrors.ToList();
                return PartialView("_TestAssignResult", obj);
            }

            if (!parameters.QTITestStudentAssignmentServices.CheckAbleToAssignStudentLevel(data.DistrictId)
                && (data.AssignmentType == (int)TestAssignmentTypeEnum.SingleClassStudent || data.AssignmentType == (int)TestAssignmentTypeEnum.MultiClassStudent))
            {
                return Json(new { error = "This district does not allow Student Level Test Assignment." }, JsonRequestBehavior.AllowGet);
            }

            //TODO: check IsTeacherLed
            var virtualTest = parameters.VirtualTestService.GetTestById(data.TestId);
            if (virtualTest != null)
            {
                data.IsTeacherLed = virtualTest.IsTeacherLed.GetValueOrDefault();
                //Force branchingTest Value
                data.ObjTestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals("branchingTest")).Value = GetOptionBranchingTestByTest(virtualTest.NavigationMethodID);

                data.RequireTestTakerAuthentication = data.ObjTestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals(Constanst.REQUIRE_TEST_TAKER_AUTHENTICATION))?.Value == "1";
            }

            switch (data.AssignmentType)
            {
                case (int)TestAssignmentTypeEnum.SingleClass:
                    {
                        obj.IsClassAssign = true;
                        obj.IsStudentAssign = false;
                        AssignTestToClass(data, obj);
                        obj.CurrentTab = (int)DisplayAssignmentType.ClassAssignment;
                        break;
                    }
                case (int)TestAssignmentTypeEnum.SingleClassStudent:
                    {
                        obj.IsClassAssign = true;
                        obj.IsStudentAssign = true;
                        AssignTestToStudent(data, obj);
                        obj.CurrentTab = (int)DisplayAssignmentType.StudentAssignment;
                        break;
                    }
                case (int)TestAssignmentTypeEnum.MultiClass:
                    {
                        var lstClassId = parameters.ClassPrintingGroupServices.GetClassesByGroupId(data.GroupId);
                        if (lstClassId.Any())
                        {
                            obj.IsClassAssign = false;
                            obj.IsStudentAssign = false;
                            AssignTestToMultiClass(data, lstClassId.ToList(), obj);
                            obj.CurrentTab = (int)DisplayAssignmentType.ClassAssignment;
                        }
                        else
                        {
                            obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Selected group has no class") };
                        }
                        break;
                    }
                case (int)TestAssignmentTypeEnum.MultiClassStudent:
                    {
                        var lstClassId = parameters.ClassPrintingGroupServices.GetClassesByGroupId(data.GroupId);
                        if (lstClassId.Any())
                        {
                            obj.IsClassAssign = false;
                            obj.IsStudentAssign = true;
                            AssignTestToStudentMultiClass(data, lstClassId.ToList(), obj);
                            obj.CurrentTab = (int)DisplayAssignmentType.StudentGroupAssingnment;
                        }
                        else
                        {
                            obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Selected group has no class") };
                        }
                        break;
                    }
            }
            if (obj.ListStudentAssign.Count > 0 || obj.ListClassAssign.Count > 0)
            {
                obj.ListStudentAssign = obj.ListStudentAssign.OrderBy(o => o.StudentFirstName)
                                     .ThenBy(o => o.StudentLastName).ThenBy(o => o.StudentId).ThenBy(o => o.TestCode)
                                     .ThenBy(o => o.ShortGUID).ToList();
                obj.ListClassAssign = obj.ListClassAssign.OrderBy(o => o.SchoolName)
                                         .ThenBy(o => o.ClassName).ThenBy(o => o.DistrictTermName).ThenBy(o => o.TeacherLastName)
                                         .ThenBy(o => o.TeacherFirstName).ToList();
                if (obj.ErrorList != null) obj.ErrorList.Clear();
            }
            obj.TestAssingCustomName = string.Format("{0} ({1} {2}, {3})", LabelHelper.GradeLabelShort, data.TestName, data.GradeName, data.SubjectName);

            if (Session["PrintResult"] == null)
            {
                Session["PrintResult"] = obj;
            }
            else
            {
                if (obj.ErrorList == null || obj.ErrorList.Count == 0)
                {
                    ((TestAssignResultViewModel)Session["PrintResult"]).CurrentTab = obj.CurrentTab;
                    ((TestAssignResultViewModel)Session["PrintResult"]).TestAssingCustomName = obj.TestAssingCustomName;
                }
                ((TestAssignResultViewModel)Session["PrintResult"]).ErrorList = obj.ErrorList;
                if (obj.ListClassAssign != null && obj.ListClassAssign.Count > 0)
                {
                    ((TestAssignResultViewModel)Session["PrintResult"]).ListClassAssign.AddRange(obj.ListClassAssign);
                }
                if (obj.ListGroupStudent != null && obj.ListGroupStudent.Count > 0)
                {
                    ((TestAssignResultViewModel)Session["PrintResult"]).ListGroupStudent.AddRange(obj.ListGroupStudent);
                }
                if (obj.ListStudentAssign != null && obj.ListStudentAssign.Count > 0)
                {
                    ((TestAssignResultViewModel)Session["PrintResult"]).ListStudentAssign.AddRange(obj.ListStudentAssign);
                }
            }

            if (obj.ListClassAssign != null && obj.ListClassAssign.Count > 0)
            {
                return Json(new { Success = true, IsTeacherLed = data.IsTeacherLed, HyperLink = obj.ListClassAssign[0].PortalHyperLinkTestCode, TestCode = obj.ListClassAssign[0].TestCode });
            }
            return Json(new { Success = true, IsTeacherLed = data.IsTeacherLed, HyperLink = "" });
        }

        [HttpGet]
        public ActionResult GetDefaultUserRosterValue(int? districtId)
        {
            bool? result = null;
            if (districtId.HasValue && districtId.Value > 0)
            {
                var userRoster =
                    parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                        districtId.Value, ContaintUtil.UseRostersAtTimeOfTestTaking).FirstOrDefault();
                bool useRosterValue;
                if (userRoster != null && bool.TryParse(userRoster.Value, out useRosterValue))
                    result = useRosterValue;
                else
                {
                    result = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(ContaintUtil.UseRostersAtTimeOfTestTaking, true);
                }
            }
            else
            {
                result = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(ContaintUtil.UseRostersAtTimeOfTestTaking, true);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private bool ValidatePermissionTestAssignmentData(TestAssignmentData data)
        {
            if (data.DistrictId > 0)
            {
                if (!parameters.VulnerabilityService.HasRightOnDistrict(CurrentUser,
                    CurrentUser.GetMemberListDistrictId(), data.DistrictId))
                {
                    return false;
                }
            }
            //Check if user can access class
            if (data.ClassId > 0 &&
                parameters.VulnerabilityService.CheckUserPermissionOnClassOrSchool(CurrentUser.Id, CurrentUser.RoleId,
                    data.ClassId, "Class") == false)
            {
                return false;
            }

            //Check if user can access test
            if (data.TestId > 0 && parameters.VulnerabilityService.HasRightToAccessVirtualTest(CurrentUser, data.TestId, CurrentUser.GetMemberListDistrictId()) == false)
            {
                return false;
            }

            //check if user can access student
            if (data.StudentIdList.Any() &&
                parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser,
                    string.Join(",", data.StudentIdList)) == false)
            {
                return false;
            }

            return true;
        }

        [HttpPost]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentRewrite)]
        [AjaxOnly]
        public ActionResult CheckAssignSameTest(TestAssignmentData data)
        {
            if (ValidatePermissionTestAssignmentData(data) == false)
            {
                return Json(new { StudentCount = 0 });
            }

            var studentIDs = data.StudentIdList != null ? string.Join(",", data.StudentIdList) : string.Empty;
            var result = parameters.QTITestClassAssignmentServices.CheckAssignSameTest(data.AssignmentType, studentIDs,
                data.ClassId.ToString(), data.TestId, data.IsUseRoster, data.GroupId);

            return Json(new { StudentCount = (result == null ? 0 : result.Count()) });
        }

        [HttpPost]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentRewrite)]
        [AjaxOnly]
        public ActionResult CheckAssignTest(TestAssignmentData data)
        {
            if (ValidatePermissionTestAssignmentData(data) == false)
            {
                return Json(new { StudentOnlineTest = 0, StudentBBS = 0 });
            }

            if (data.IsTutorialMode == 2)//tutorial mode
            {
                if (parameters.BankDistrictService.IsLocked(data.BankId, CurrentUser.DistrictId.GetValueOrDefault()))
                {
                    return Json(new { IsTutorialLocked = true }, JsonRequestBehavior.AllowGet);
                }
            }

            // Check Authorize for locked test
            if (data.AssignmentType == 5)
            {
                if (!CheckAuthorizeLockedTest(data))
                {
                    return Json(new { IsCompatible = true, IsTutorialLocked = false, AuthorizeForPreview = false }, JsonRequestBehavior.AllowGet);
                }
            }
            //check if the test is SAT or ACT. Preview function should not be affected
            var virtualTest = parameters.VirtualTestService.GetTestById(data.TestId);
            if (virtualTest.IsMultipleTestResult.GetValueOrDefault())
                return Json(new { IsMultipleTestResult = true }, JsonRequestBehavior.AllowGet);

            if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.ACT || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.SAT)
            {
                return Json(new { IsCompatible = false }, JsonRequestBehavior.AllowGet);
            }

            // Check Assign Same Test logic
            var studentIDs = data.StudentIdList != null ? data.StudentIdList.Select(x => int.Parse(x)).ToList() : new List<int>();
            var studentOnlineTest = 0;
            var studentBBS = 0;
            if (virtualTest.DatasetCategoryID != (int)DataSetCategoryEnum.MOI &&
                virtualTest.DatasetCategoryID != (int)DataSetCategoryEnum.VDET_EOI &&
                virtualTest.DatasetCategoryID != (int)DataSetCategoryEnum.VDET_FDOI
                )
            {
                var studentAssigment = parameters.QTITestClassAssignmentServices.GetStudentAssginmentGrouping(data.TestId, studentIDs, data.GroupId, data.ClassId.ToString());
                studentOnlineTest = studentAssigment.OnlineTest.Where(x => studentIDs.Contains(x.StudentId)).DistinctBy(x => x.StudentId).Count();
                studentBBS = studentAssigment.BubbleSheet.Where(x => studentIDs.Contains(x.StudentId)).DistinctBy(x => x.StudentId).Count();
            }
            var codesToBeGenerated = CountCodesToBeGenerated(data);

            return Json(new { IsCompatible = true, StudentOnlineTest = studentOnlineTest, StudentBBS = studentBBS, IsTutorialLocked = false, CodesToBeGenerated = codesToBeGenerated }, JsonRequestBehavior.AllowGet);
        }

        private int CountCodesToBeGenerated(TestAssignmentData data)
        {
            switch (data.AssignmentType)
            {
                case (int)TestAssignmentTypeEnum.SingleClass:
                    {
                        return 1;
                    }
                case (int)TestAssignmentTypeEnum.SingleClassStudent:
                    {
                        return data.StudentIdList?.Count ?? 0;
                    }
                case (int)TestAssignmentTypeEnum.MultiClass:
                    {
                        return parameters.ClassPrintingGroupServices.GetClassesByGroupId(data.GroupId).Count();
                    }
                case (int)TestAssignmentTypeEnum.MultiClassStudent:
                    {
                        return parameters.ClassPrintingGroupServices.CountActiveStudentInGroup(data.GroupId);
                    }
                default:
                    return 0;
            }
        }

        private bool CheckAuthorizeLockedTest(TestAssignmentData data)
        {
            if (CurrentUser.IsPublisherOrNetworkAdmin || CurrentUser.IsDistrictAdmin)
            {
                return true;
            }

            if (IsLockedBank(data.DistrictId, data.BankId)) // this bank is locked
            {
                var virtualTest = parameters.VirtualTestService.GetTestById(data.TestId);

                // don't lock the author
                return virtualTest != null && virtualTest.AuthorUserID == CurrentUser.Id;
            }

            return true;
        }

        private bool IsLockedBank(int districtId, int bankId)
        {
            var districtBank = parameters.ListBankServices.GetBankDistrict(districtId, bankId);
            bool isLockedBank = districtBank != null && (districtBank.BankAccessId == (int)LockBankStatus.Restricted || districtBank.BankDistrictAccessId == (int)LockBankStatus.Restricted);

            return isLockedBank;
        }

        private string CheckRightTestAssignSameTestParam(TestAssignSameTestParam data)
        {
            if (data.StudentIdList.Count > 0)
            {
                if (
                    !parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser,
                        string.Join(",", data.StudentIdList)))
                {
                    return "Has no right to access one or more students";
                }
            }

            if (data.ClassId > 0)
            {
                if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, data.ClassId))
                {
                    return "Has no right to access the class";
                }
            }
            if (data.TestId > 0)
            {
                if (!parameters.VulnerabilityService.HasRightToAccessVirtualTest(CurrentUser, data.TestId, CurrentUser.GetMemberListDistrictId()))
                {
                    return "Has no right to access the test";
                }
            }
            return string.Empty;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentRewrite)]
        public ActionResult TempAssignSameTest(TestAssignSameTestParam model)
        {
            string result = CheckRightTestAssignSameTestParam(model);
            if (!string.IsNullOrEmpty(result))
            {
                return Json(new { error = result }, JsonRequestBehavior.AllowGet);
            }
            model.DistrictID = model.DistrictID.HasValue ? model.DistrictID.Value
                    : (CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0);
            return View("TempAssignSameTest", model);
        }

        [HttpPost]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentRewrite)]
        public ActionResult StudentsAssignSameTest(TestAssignSameTestParam model)
        {
            string result = CheckRightTestAssignSameTestParam(model);
            if (!string.IsNullOrEmpty(result))
            {
                return Json(new { error = result }, JsonRequestBehavior.AllowGet);
            }
            if (model.IsGroupPrinting)
                return View("StudentAssignSameTestGroupPrinting", model);

            return View("StudentAssignSameTest", model);
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentRewrite)]
        public ActionResult GetStudentAssignSameTest(TestAssignSameTestParam data)
        {
            string resultCheck = CheckRightTestAssignSameTestParam(data);
            if (!string.IsNullOrEmpty(resultCheck))
            {
                return Json(new { error = resultCheck }, JsonRequestBehavior.AllowGet);
            }

            var parser = new DataTableParserProc<AssignSameTestViewModel>();
            var studentIDs = !string.IsNullOrEmpty(data.StudentIds) ? data.StudentIds?.ToIntArray(",")?.ToList() : new List<int>();
            var results = parameters.QTITestClassAssignmentServices.GetStudentAssginmentGrouping(data.TestId, studentIDs, data.GroupId, data.ClassId.ToString(), data.SSearch, parser.SortableColumns, parser.PageSize).OnlineTest.Select(
                    s => new AssignSameTestViewModel
                    {
                        StudentId = s.StudentId,
                        FullName = s.FullName,
                        Code = s.Code,
                        QTITestClassAssignmentID = s.QTITestClassAssignmentID,
                        AssignmentDate = s.AssignmentDate,
                        ResultDate = s.ResultDate
                    }).AsQueryable();

            int totalRecords = results.Count();
            results = results.Skip(parser.StartIndex).Take(Math.Min(parser.PageSize, totalRecords - parser.StartIndex));
            return Json(parser.Parse(results, totalRecords), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentRewrite)]
        public ActionResult GetStudentAssignSameTestGroup(TestAssignSameTestParam data)
        {
            string resultCheck = CheckRightTestAssignSameTestParam(data);
            if (!string.IsNullOrEmpty(resultCheck))
            {
                return Json(new { error = resultCheck }, JsonRequestBehavior.AllowGet);
            }
            var parser = new DataTableParserProc<AssignSameTestGroupPrintingViewModel>();
            var studentIDs = !string.IsNullOrEmpty(data.StudentIds) ? data.StudentIds.Split(',').Select(x => { return int.Parse(x); }).ToList() : new List<int>();
            var results = parameters.QTITestClassAssignmentServices.GetStudentAssginmentGrouping(data.TestId, studentIDs, data.GroupId, data.ClassId.ToString(), data.SSearch, parser.SortableColumns, parser.PageSize).OnlineTest.Select(
                    s => new AssignSameTestGroupPrintingViewModel
                    {
                        StudentId = s.StudentId,
                        FullName = $"{StringToStudentNameMacroAfter(s.LastName)}{StringToStudentNameMacro(s.FirstName)}",
                        AssignmentDate = s.AssignmentDate
                    }).AsQueryable();
            int totalRecords = results.Count();
            results = results.Skip(parser.StartIndex).Take(Math.Min(parser.PageSize, totalRecords - parser.StartIndex));
            return Json(parser.Parse(results, totalRecords), JsonRequestBehavior.AllowGet);
        }

        private static string StringToStudentNameMacro(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : ", " + value;
        }

        private static string StringToStudentNameMacroAfter(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : value;
        }

        public ActionResult TestSettings(int districtId, int? virtualTestID, bool isPartialRetake = false)
        {
            ViewBag.IsPreventEditOverrideGrade = (CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher) ? true : false;
            var hideSupportHightlightText = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.HideSupportHighlightText, "false");
            ViewBag.HideSupportHighlightText = hideSupportHightlightText;
            ViewBag.IsSupportQuestionGroup = CheckUserSupportQuestionGroup(districtId);
            ViewBag.IsSettingScope = true;


            var doesUseTestExtract = parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.DistrictDecode_TestScoreExtract).Any();
            var gradebookSISValue = parameters.DistrictDecodeService
                .GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.DistrictDecode_SendTestResultToGenesis).Select(c => c.Value)
                .FirstOrDefault();
            var testExtractOptions = new TestExtractOptions
            {
                IsUseTestExtract = doesUseTestExtract || (gradebookSISValue != null && gradebookSISValue != "0")
            };

            if (doesUseTestExtract)
            {
                testExtractOptions.Gradebook = true;
            }

            if (gradebookSISValue != null)
            {
                var gradebookSISIds = gradebookSISValue.ToIntArray("|").Where(c => c > 0).ToArray();
                if (gradebookSISIds.Any())
                {
                    if (gradebookSISIds.Contains((int)GradebookSIS.CleverApi))
                    {
                        testExtractOptions.CleverApi = true;
                    }
                    testExtractOptions.Gradebook = true;

                    if (gradebookSISIds.Contains((int)GradebookSIS.Realtime))
                    {
                        testExtractOptions.StudentRecord = true;
                    }
                    if (gradebookSISIds.DoesShowExportScoreTypeOption())
                    {
                        testExtractOptions.ShowRawOrPercentOption = true;
                    }
                }
            }
            var virtualTest = virtualTestService.GetVirtualTestById(virtualTestID ?? 0);
            if (virtualTest != null)
            {
                ViewBag.NavigationMethodID = virtualTest.NavigationMethodID;
            }
            else
            {
                ViewBag.NavigationMethodID = -1;
            }
            ViewBag.EnableLock = false;
            ViewBag.TestExtractOptions = testExtractOptions;
            ViewBag.DisableSectionAvailability = isPartialRetake;
            return PartialView("_TestSettings");
        }

        public ActionResult DefaultSettingsOld(int districtId, bool isMultiClass, int bankId, bool isReload, int testId)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right to district." }, JsonRequestBehavior.AllowGet);
            }
            if (!parameters.VulnerabilityService.HasRightToAccessTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right to the test." }, JsonRequestBehavior.AllowGet);
            }
            if (!parameters.VulnerabilityService.HasRightToAccessVirtualTest(CurrentUser, testId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right to the test." }, JsonRequestBehavior.AllowGet);
            }

            var districtBank = parameters.ListBankServices.GetBankDistrictByDistrictIdAndBankId(districtId, bankId);
            bool isLockedBank = districtBank != null && (districtBank.BankDistrictAccessId == (int)LockBankStatus.Restricted || districtBank.BankAccessId == (int)LockBankStatus.Restricted);
            var currentDistrictId = districtId;
            if (currentDistrictId == 0 || !CurrentUser.IsPublisher)
            {
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            }
            var model = GetTestPreference(isMultiClass, isLockedBank, isReload, currentDistrictId, testId);
            var tmp = DateTime.Now;
            if (DateTime.TryParse(model.TestSettingViewModel.Deadline, out tmp))
            {
                model.TestSettingViewModel.DeadlineDisplay = tmp;
            }
            return PartialView("DefaultSettings", model);
        }

        //Edit
        public ActionResult DefaultSettings(int districtId, int testId, int? schoolId, int? groupClassId, bool? isOldUI, bool? isPartialRetake, string guid, string studentIds)
        {
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right to district." }, JsonRequestBehavior.AllowGet);
            }
            if (!parameters.VulnerabilityService.HasRightToAccessVirtualTest(CurrentUser, testId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right to the test." }, JsonRequestBehavior.AllowGet);
            }
            if (groupClassId.HasValue && groupClassId.Value > 0)
            {
                var lstClassId = parameters.ClassPrintingGroupServices.GetClassesByGroupId(groupClassId.Value).Distinct().ToList();
                if (lstClassId != null && lstClassId.Any())
                {
                    var classInfos = parameters.ClassServices.GetClassesByIds(lstClassId).Select(s => s.SchoolId).Distinct().ToList();
                    schoolId = (classInfos != null && classInfos.Count == 1) ? classInfos[0] : 0;
                }
            }
            schoolId = (isOldUI.HasValue && isOldUI.Value == true) ? 0 : schoolId;
            int CurrrentLevelId = (int)TestPreferenceLevel.User;

            GetTestPreferencePartialRetakeModel partialRetakeInfo = null;
            if (isPartialRetake.HasValue && isPartialRetake.Value)
            {
                partialRetakeInfo = new GetTestPreferencePartialRetakeModel
                {
                    GUID = guid,
                    StudentIDs = studentIds,
                    VirtualTestID = testId
                };
            }
            var model = GetTestPreferenceWithTestId(testId, districtId, CurrrentLevelId, schoolId, partialRetakeInfo);

            var hideSupportHightlightText = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.HideSupportHighlightText, "false");
            ViewBag.HideSupportHighlightText = hideSupportHightlightText;

            model.IsSupportQuestionGroup = CheckUserSupportQuestionGroup(districtId);
            model.ShowAssignmentSettings = !model.IslockedBank || model.CreatedByUserId == CurrentUser.Id || CurrentUser.IsDistrictAdminOrPublisher || CurrentUser.IsDistrictAdmin;
            ViewBag.IsEditable = false;

            var timeLimitValue = model.TestPreferenceModel.OptionTags.Find(t => t.Key == "timeLimit").Value;
            ViewBag.EnableTimeLimit = timeLimitValue == "1" ? true : false;
            ViewBag.DateFormat = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;
            ViewBag.LastUpdatedDate = model.TestPreferenceModel.LastUpdatedDateString;
            return PartialView("DefaultSettings", model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdatePreference(TestPreferenceModel model)
        {
            var value = parameters.PreferencesServices.ConvertTestPreferenceModelToString(model);
            return Json(new { Sucess = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult TestSettings(TestSettingsViewModel vOptions, TestSettingToolViewModel vTools, int testId)
        {
            if (!parameters.VulnerabilityService.HasRightToAccessVirtualTest(CurrentUser, testId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right on the test." }, JsonRequestBehavior.AllowGet);
            }
            if (vOptions != null && vTools != null)
            {
                string strSessionKey = string.Format("Session_{0}_{1}", vOptions.DistrictId, testId);
                TestSettingsMap tsm = new TestSettingsMap() { TestSettingViewModel = vOptions, TestSettingToolViewModel = vTools };
                ETLXmlSerialization<TestSettingsMap> obj = new ETLXmlSerialization<TestSettingsMap>();
                if (Session[strSessionKey] != null)//Track LockItems
                {
                    var v = (Preferences)Session[strSessionKey];
                    TestSettingsMap vTestSettingsMap = obj.DeserializeXmlToObject(v.Value);
                    vTestSettingsMap.InitDefault();
                    tsm.LockItemViewModel = vTestSettingsMap.LockItemViewModel;
                }
                string result = obj.SerializeObjectToXmlWithOutHeader(tsm);
                var pre = new Preferences() { Value = result, Label = ContaintUtil.TestPreferenceLabelTest };
                //pre.Level = ContaintUtil.TestPreferenceLevelTestAssignment;
                Session[strSessionKey] = pre;
                return Json(new { Sucess = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Sucess = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintTest(int testId, int bankId, int districtId)
        {
            if (!parameters.VulnerabilityService.HasRightToAccessVirtualTest(CurrentUser, testId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right on the test." }, JsonRequestBehavior.AllowGet);
            }
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right on the district." }, JsonRequestBehavior.AllowGet);
            }
            if (!parameters.VulnerabilityService.HasRightToAccessTestBank(CurrentUser, bankId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right on the bank." }, JsonRequestBehavior.AllowGet);
            }

            var districtBank = parameters.ListBankServices.GetBankDistrictByDistrictIdAndBankId(districtId, bankId);
            bool isLockedBank = districtBank != null && (districtBank.BankDistrictAccessId == (int)LockBankStatus.Restricted || districtBank.BankAccessId == (int)LockBankStatus.Restricted);

            var test = parameters.TestServices.GetTestById(testId);
            string answerKeyURL = ConfigurationManager.AppSettings["PrintAnswerKeyURL"];
            string testKeyURL = ConfigurationManager.AppSettings["PrintTestURL"];
            if (testId == null) return null;
            PrintTestRequest obj = new PrintTestRequest()
            {
                IsLockbank = isLockedBank,
                VirtualTestID = testId.ToString(),
                TestTitle = test.Name,
                PrintTestURL = testKeyURL,
                PrintAnswerKeyURL = answerKeyURL
            };
            return PartialView("_PrintTest", obj);
        }

        public ActionResult PrintTestReview(int testId, int? districtID)
        {
            if (!parameters.VulnerabilityService.HasRightToAccessVirtualTest(CurrentUser, testId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right on the test." }, JsonRequestBehavior.AllowGet);
            }
            if (districtID.HasValue)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, districtID.Value))
                {
                    return Json(new { error = "Has no right on the district." }, JsonRequestBehavior.AllowGet);
                }
            }

            var vTest = parameters.TestServices.GetTestById(testId);
            int bankId = vTest != null ? vTest.BankId : 0;
            int iDistrictID = 0;
            if (CurrentUser.IsPublisher && districtID.HasValue)
            {
                iDistrictID = districtID.Value;
            }
            else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                iDistrictID = CurrentUser.DistrictId.Value;
            }

            var districtBank = parameters.ListBankServices.GetBankDistrictByDistrictIdAndBankId(iDistrictID, bankId);
            bool isLockedBank = districtBank != null && (districtBank.BankDistrictAccessId == (int)LockBankStatus.Restricted || districtBank.BankAccessId == (int)LockBankStatus.Restricted);

            var test = parameters.TestServices.GetTestById(testId);
            string answerKeyURL = ConfigurationManager.AppSettings["PrintAnswerKeyURL"];
            string testKeyURL = ConfigurationManager.AppSettings["PrintTestURL"];

            PrintTestRequest obj = new PrintTestRequest()
            {
                IsLockbank = isLockedBank,
                VirtualTestID = testId.ToString(),
                TestTitle = test.Name,
                PrintTestURL = testKeyURL,
                PrintAnswerKeyURL = answerKeyURL
            };
            return PartialView("_PrintTest", obj);
        }

        public ActionResult PrintAssignment(int type)
        {
            TestAssignResultViewModel obj = new TestAssignResultViewModel();
            if (Session[SessionKey.PRINT_RESULT] != null)
            {
                obj = (TestAssignResultViewModel)Session[SessionKey.PRINT_RESULT];
                obj.PringtType = type;
                var timeZoneId = parameters.StateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
                var dateTimeFormat = parameters.DistrictDecodeService.GetDateFormat(CurrentUser.DistrictId.GetValueOrDefault());
                ViewBag.CurrentDateAssigned = DateTime.UtcNow.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(dateTimeFormat.DateFormat, dateTimeFormat.TimeFormat, false);
                obj.ListClassAssign = obj.ListClassAssign.OrderByDescending(x => x.Assigned).ToList();
                obj.ListClassAssign.ForEach(x =>
                {
                    x.AssignmentDate = x.Assigned.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(dateTimeFormat.DateFormat, dateTimeFormat.TimeFormat, true);
                });

                obj.ListGroupStudent = obj.ListGroupStudent.OrderByDescending(x => x.Assigned).ToList();
                obj.ListGroupStudent.ForEach(x =>
                {
                    x.AssignmentDate = x.Assigned.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(dateTimeFormat.DateFormat, dateTimeFormat.TimeFormat, true);
                });

                obj.ListStudentAssign = obj.ListStudentAssign.OrderByDescending(x => x.Assigned).ToList();
                obj.ListStudentAssign.ForEach(x =>
                {
                    x.AssignmentDate = x.Assigned.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(dateTimeFormat.DateFormat, dateTimeFormat.TimeFormat, true);
                });
                var checkResult = CheckTestAssignResultViewModel(obj);
                if (!string.IsNullOrEmpty(checkResult))
                {
                    return Json(new { error = checkResult }, JsonRequestBehavior.AllowGet);
                }
            }
            return View("PrintAssignment", obj);
        }

        public ActionResult PrintAssignmentRetake(GetTestClassAssignmentCriteria criteria)
        {
            TestAssignResultViewModel obj = null;
            if (criteria != null)
            {
                var request = new LoadAssignmentRetakeRequest();
                MappingLoadAssignmentForRetakeRequest(criteria, request);
                request.StartRow = -1;

                obj = new TestAssignResultViewModel();
                obj.ListStudentAssign = parameters.QTITestClassAssignmentServices.GetRetakeTestAssignResults(request)
                    .RetakeTestAssignResults
                    .Select(x => new TestStudentAssignResultViewModel
                    {
                        ID = x.ID,
                        Assigned = x.Assigned,
                        ClassName = x.ClassName,
                        IsActive = x.IsActive,
                        StudentCode = x.StudentCode,
                        StudentFirstName = x.StudentFirstName,
                        StudentLastName = x.StudentLastName,
                        StudentId = x.StudentId,
                        StudentName = x.StudentName,
                        Test = x.Test,
                        TestCode = x.TestCode,
                        AuthenticationCode = x.AuthenticationCode
                    }).ToList();
                obj.IsStudentAssign = true;
                obj.PringtType = 2;

                var checkResult = CheckTestAssignResultViewModel(obj);
                if (!string.IsNullOrEmpty(checkResult))
                {
                    return Json(new { error = checkResult }, JsonRequestBehavior.AllowGet);
                }
            }

            return View("PrintAssignment", obj);
        }

        private string CheckTestAssignResultViewModel(TestAssignResultViewModel model)
        {
            if (model != null)
            {
                if (model.IsClassAssign)
                {
                    if (model.ListClassAssign != null && model.ListClassAssign.Count > 0)
                    {
                        var classIdList = model.ListClassAssign.Select(x => x.ClassId).ToList();
                        if (!parameters.VulnerabilityService.CheckUserCanAccessClass(CurrentUser, classIdList))
                        {
                            return "Has no right on one ore more classes.";
                        }
                    }
                }
                if (model.IsStudentAssign)
                {
                    if (model.ListStudentAssign != null && model.ListStudentAssign.Count > 0)
                    {
                        var studentIdList = model.ListStudentAssign.Select(x => x.StudentId).ToList();
                        if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentIdList))
                        {
                            return "Has no right on one ore more students.";
                        }
                    }
                }
                if (model.ListGroupStudent != null && model.ListGroupStudent.Count > 0)
                {
                    var studentIdList = model.ListStudentAssign.Select(x => x.StudentId).ToList();
                    if (!parameters.VulnerabilityService.CheckUserPermissionOnStudent(CurrentUser, studentIdList))
                    {
                        return "Has no right on one ore more students.";
                    }
                }
            }
            return string.Empty;
        }

        public ActionResult LoadExistAssignment(string sessionKey = "PrintResult")
        {
            var model = (TestAssignResultViewModel)Session[sessionKey];
            var checkResult = CheckTestAssignResultViewModel(model);
            if (!string.IsNullOrEmpty(checkResult))
            {
                return Json(new { error = checkResult }, JsonRequestBehavior.AllowGet);
            }
            if (model != null)
            {
                var timeZoneId = parameters.StateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
                var dateTimeFormat = parameters.DistrictDecodeService.GetDateFormat(CurrentUser.DistrictId.GetValueOrDefault());
                if (model.ListClassAssign.Any())
                {
                    model.ListClassAssign.ForEach(x =>
                    {
                        x.AssignmentDate = x.Assigned.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(dateTimeFormat.DateFormat, dateTimeFormat.TimeFormat, true);
                    });
                }

                if (model.ListStudentAssign.Any())
                {
                    model.ListStudentAssign.ForEach(x =>
                    {
                        x.AssignmentDate = x.Assigned.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(dateTimeFormat.DateFormat, dateTimeFormat.TimeFormat, true);
                    });
                }

                if (model.ListGroupStudent.Any())
                {
                    model.ListGroupStudent.ForEach(x =>
                    {
                        x.AssignmentDate = x.Assigned.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(dateTimeFormat.DateFormat, dateTimeFormat.TimeFormat, true);
                    });
                }
            }

            return PartialView("_TestAssignResult", Session[sessionKey]);
        }

        #region Private Method

        private bool IsUserAdmin()
        {
            return parameters.UserServices.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
        }

        private string GenerateClassTestCode()
        {
            Configuration objCon = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.ClassTestCodeLength);
            int iTestCodeLength = 5;
            if (objCon != null)
            {
                iTestCodeLength = CommonUtils.ConverStringToInt(objCon.Value, 5);
            }

            var testCode = parameters.TestCodeGenerator.GenerateTestCode(iTestCodeLength,
                LinkitConfigurationManager.GetLinkitSettings().DatabaseID);

            return testCode;
        }

        private string GenerateStudentTestCode(string studentTestCodeLength = null)
        {
            Random r = new Random();
            int iTestCodeLength = 9;

            if (string.IsNullOrEmpty(studentTestCodeLength))
            {
                studentTestCodeLength = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.StudentTestCodeLength)?.Value;
            }

            if (!string.IsNullOrEmpty(studentTestCodeLength))
            {
                string[] arr = studentTestCodeLength.Split(',');
                int iLength = r.Next(0, arr.Length);
                iTestCodeLength = CommonUtils.ConverStringToInt(arr[iLength], 9);
            }

            var testCode = parameters.TestCodeGenerator.GenerateTestCode(iTestCodeLength, LinkitConfigurationManager.GetLinkitSettings().DatabaseID);

            return testCode;
        }

        private bool SaveListTestClassStudent(List<int> lstStudentId, int classAssignmentId)
        {
            if (lstStudentId.Count > 0 && classAssignmentId > 0)
            {
                string tempTableName = "#tmpQTITest";
                string tempTableCreationScript = $"create table {tempTableName}(StudentId int, QTITestClassAssignmentId int)";
                var tempAssignmentObjects = lstStudentId.Select(c => new
                {
                    StudentId = c,
                    QTITestClassAssignmentId = classAssignmentId
                }).ToArray();

                var finalizeQuery = $"BulkHelperAddOrUpdate";

                new BulkHelper(_connectionString).BulkCopy(tempTableCreationScript,
                    tempTableName, tempAssignmentObjects,
                    finalizeQuery,
                    "@TempTableName", tempTableName,
                     "@DestinationTableName", "QTITestStudentAssignment",
                     "@KeyColumns", "",
                     "@UniqueColumns", "StudentId,QTITestClassAssignmentId"
                    );

                return true;
            }
            return false;
        }

        private bool SaveTestClassStudent(int studentId, int classAssignmentId)
        {
            if (studentId > 0 && classAssignmentId > 0)
            {
                QTITestStudentAssignmentData obj = new QTITestStudentAssignmentData()
                {
                    StudentId = studentId,
                    QTITestClassAssignmentId = classAssignmentId,
                    IsHide = false,
                    Status = 1
                };
                parameters.QTITestStudentAssignmentServices.AssignStudent(obj);

                return true;
            }
            return false;
        }

        private bool SaveTestClassStudents(List<int> lstStudentId, int classAssignmentId)
        {
            if ((lstStudentId != null && lstStudentId.Count > 0) && classAssignmentId > 0)
            {
                var objs = lstStudentId.Select(s => new QTITestStudentAssignmentData()
                {
                    StudentId = s,
                    QTITestClassAssignmentId = classAssignmentId,
                    Status = 1,
                    IsHide = false
                }).ToList();
                parameters.QTITestStudentAssignmentServices.AssignStudents(objs);

                return true;
            }
            return false;
        }

        private void AssignTestToMultiClass(TestAssignmentData data, List<int> lstClassId, TestAssignResultViewModel obj)
        {
            foreach (int classId in lstClassId)
            {
                var vList = parameters.classStudentCustomServices.GetStudentActiveByClassId(classId)
                        .Select(c => c.StudentId).ToList();
                if (vList?.Count > 0)
                {
                    AssignSingleClass(classId, vList, data, obj);
                }
                //if (data.IsUseRoster)
                //{
                //    AssignSingleClass(classId, new List<int>(), data, obj);
                //}
                //else
                //{
                //    var vList = parameters.classStudentCustomServices.GetStudentActiveByClassId(classId)
                //        .Select(c => c.StudentId).ToList();
                //    if (vList?.Count > 0)
                //    {
                //        AssignSingleClass(classId, vList, data, obj);
                //    }
                //}
            }
            if (obj.ListClassAssign == null || obj.ListClassAssign.Count == 0)
            {
                obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Group have no student active") };
            }
        }

        private void AssignTestToStudentMultiClass(TestAssignmentData data, List<int> lstClassId, TestAssignResultViewModel obj)
        {
            foreach (int classId in lstClassId)
            {
                var lstStudent = parameters.classStudentCustomServices.GetStudentActiveByClassId(classId);
                if (lstStudent.Any())
                {
                    AssignGroupStudent(classId, lstStudent.ToList(), data, obj);
                }
                //TODO: else invalid Class in group
            }
            if (obj.ListGroupStudent == null || obj.ListGroupStudent.Count == 0)
            {
                obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Group have no student active") };
            }
        }

        private void AssignTestToClass(TestAssignmentData data, TestAssignResultViewModel obj)
        {
            if (data.IsTeacherLed && data.IsLaunchTeacherLedTest && (CurrentUser.IsDistrictAdmin || CurrentUser.IsNetworkAdmin))
            {
                parameters.QTITestClassAssignmentServices.DeactivatePreviousAssignments(data.TestId, data.ClassId, CurrentUser.Id);
            }

            var lstStudentId = new List<int>();
            foreach (var item in data.StudentIdList)
            {
                int studentId = 0;
                if (int.TryParse(item, out studentId))
                {
                    lstStudentId.Add(studentId);
                }
            }
            if (lstStudentId.Count > 0)
            {
                AssignSingleClass(data.ClassId, lstStudentId, data, obj);
            }
            else
            {
                obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Class have no student active") };
            }
        }

        private void AssignTestToStudent(TestAssignmentData data, TestAssignResultViewModel obj)
        {
            List<Student> lstStudent = new List<Student>();
            foreach (var item in data.StudentIdList)
            {
                int studentId = 0;
                if (int.TryParse(item, out studentId))
                {
                    var currentStudent = parameters.StudentServices.GetStudentById(studentId);
                    if (currentStudent != null)
                        lstStudent.Add(currentStudent);
                }
            }
            if (lstStudent.Count > 0)
            {
                AssignStudent(data.ClassId, lstStudent, data, obj);
            }
            else
            {
                obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Class have no student active") };
            }
        }

        private void AssignTestToStudents(IEnumerable<TestAssignmentData> assignmentDatas)
        {
            var students = parameters.StudentServices.GetStudents(assignmentDatas.Select(x => x.StudentID));

            if (students == null || students.Count() == 0)
            {
                return;
            }

            assignmentDatas = assignmentDatas.Where(x => students.Select(s => s.Id).Contains(x.StudentID));
            var data = assignmentDatas.FirstOrDefault();

            var isNewSkin = parameters.DistrictDecodeService.DistrictSupporPortalNewSkin(data.DistrictId);
            var testPreferenceString = ParseTestPreferenceToString(data.ObjTestPreferenceModel, isNewSkin);

            DateTime currentDateTime = DateTime.UtcNow;
            var expirationDate = DateTime.UtcNow;

            if (isNewSkin && data.RequireTestTakerAuthentication)
            {
                expirationDate = parameters.AuthenticationCodeGenerator.GetExpirationDateByDistrictID(data.DistrictId);
            }

            var studentTestCodeLength = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.StudentTestCodeLength)?.Value;
            var comparisonPasscodeLength = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.ComparisonPasscodeLength)?.Value;

            var qtiTestClassAssignmentDatas = new List<QTITestClassAssignmentData>();

            foreach (var item in assignmentDatas)
            {
                var testCode = GenerateStudentTestCode(studentTestCodeLength);

                while (qtiTestClassAssignmentDatas.Any(x => x.Code == testCode))
                {
                    testCode = GenerateStudentTestCode(studentTestCodeLength);
                }

                var qtiTest = new QTITestClassAssignmentData()
                {
                    VirtualTestId = data.TestId,
                    ClassId = item.ClassId,
                    StudentID = item.StudentID,
                    AssignmentDate = currentDateTime,
                    Code = testCode,
                    CodeTimestamp = currentDateTime,
                    AssignmentGuId = Guid.NewGuid().ToString(),
                    TestSetting = string.Empty,
                    Status = 1,
                    ModifiedDate = currentDateTime,
                    ModifiedUserId = CurrentUser.Id,
                    ComparisonPasscodeLength = RanComparisonPasscodeLength(comparisonPasscodeLength),
                    Type = (int)AssignmentType.Student,
                    ModifiedBy = Constanst.PortalContain,
                    TutorialMode = data.IsTutorialMode,
                    DistrictID = data.DistrictId,
                    ListOfDisplayQuestions = item.ListOfDisplayQuestions
                };

                if (isNewSkin && data.RequireTestTakerAuthentication)
                {
                    var authenticationCode = parameters.AuthenticationCodeGenerator.GenerateAuthenticationCode();
                    qtiTest.SetAuthenticationCode(authenticationCode, expirationDate);
                }

                qtiTestClassAssignmentDatas.Add(qtiTest);
            }

            parameters.QTITestClassAssignmentServices.InsertMultipleRecord(qtiTestClassAssignmentDatas);

            var qtiTestStudentAssignmentDatas = qtiTestClassAssignmentDatas
                .Where(x=> x.StudentID >0 && x.QTITestClassAssignmentId >0)
                .Select(x=> new QTITestStudentAssignmentData
                {
                    StudentId = x.StudentID,
                    QTITestClassAssignmentId = x.QTITestClassAssignmentId,
                    IsHide = false,
                    Status = 1
                })
                .ToList();

            parameters.QTITestStudentAssignmentServices.InsertMultipleRecord(qtiTestStudentAssignmentDatas);

            var preferences = qtiTestClassAssignmentDatas
                .Where(x => x.QTITestClassAssignmentId > 0)
                .Select(x => new Preferences
                {
                    Id = x.QTITestClassAssignmentId,
                    Level = ContaintUtil.TestPreferenceLevelTestAssignment,
                    Label = ContaintUtil.TestPreferenceLabelTest,
                    Value = testPreferenceString,
                    UpdatedBy = CurrentUser.Id
                })
                .ToList();
            
            parameters.PreferencesServices.InsertMultipleRecord(preferences);
        }

        //ComparisonPasscodeLength: 6,7,8,9
        private int RanComparisonPasscodeLength(string comparisonPasscodeLength = null)
        {
            Random r = new Random();
            int iComparisonPasscodeLength = 9;

            if (string.IsNullOrEmpty(comparisonPasscodeLength))
            {
                comparisonPasscodeLength = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.ComparisonPasscodeLength)?.Value;
            }

            if (!string.IsNullOrEmpty(comparisonPasscodeLength))
            {
                string[] arr = comparisonPasscodeLength.Split(',');
                int iLength = r.Next(0, arr.Length);
                iComparisonPasscodeLength = CommonUtils.ConverStringToInt(arr[iLength], 9);
            }

            return iComparisonPasscodeLength;
        }

        private void SetModelPermissions(TestAssignmentData model)
        {
            model.IsPublisher = CurrentUser.IsPublisher();
            model.IsSchoolAdmin = parameters.UserServices.GetSchoolAdminByUserId(CurrentUser.Id).IsNotNull();
            model.IsAdmin = IsUserAdmin();
        }

        private void AssignSingleClass(int classId, List<int> lstStudentId, TestAssignmentData data, TestAssignResultViewModel obj)
        {
            try
            {
                DateTime currentDateTime = DateTime.UtcNow;
                var vClass = parameters.ClassCustomServices.GetClassById(classId);
                if (vClass != null && lstStudentId != null && (lstStudentId.Count > 0 || data.IsUseRoster))
                {
                    var isNewSkin = parameters.DistrictDecodeService.DistrictSupporPortalNewSkin(data.DistrictId);
                    var testPreferenceString = ParseTestPreferenceToString(data.ObjTestPreferenceModel, isNewSkin);
                    var qtiTest = new QTITestClassAssignmentData()
                    {
                        VirtualTestId = data.TestId,
                        ClassId = classId,
                        AssignmentDate = currentDateTime,
                        Code = GenerateClassTestCode(),
                        CodeTimestamp = currentDateTime,
                        AssignmentGuId = Guid.NewGuid().ToString(),
                        TestSetting = string.Empty,
                        Status = 1,
                        ModifiedDate = currentDateTime,
                        ModifiedUserId = CurrentUser.Id,
                        //ComparisonPasscodeLength = RanComparisonPasscodeLength(),
                        Type = data.IsUseRoster == true ? (int)AssignmentType.Roster : (int)AssignmentType.Class,
                        ModifiedBy = Constanst.PortalContain,
                        TutorialMode = data.IsTutorialMode,
                        DistrictID = data.DistrictId
                    };
                    if (isNewSkin && data.RequireTestTakerAuthentication)
                    {
                        var authenticationCode = parameters.AuthenticationCodeGenerator.GenerateAuthenticationCode();
                        var expirationDate = parameters.AuthenticationCodeGenerator.GetExpirationDate(classId);
                        qtiTest.SetAuthenticationCode(authenticationCode, expirationDate);
                    }
                    parameters.QTITestClassAssignmentServices.AssignClass(qtiTest);
                    //if (!data.IsUseRoster)
                    //{
                    //    SaveTestClassStudents(lstStudentId, qtiTest.QTITestClassAssignmentId);
                    //}
                    SaveTestClassStudents(lstStudentId, qtiTest.QTITestClassAssignmentId);
                    //TODO: save Setting
                    bool isAssignGroup = data.AssignmentType == 3;
                    //SaveSettingPreference(qtiTest.QTITestClassAssignmentId, data.DistrictId, data.BankId, isAssignGroup, data.TestId);
                    SaveSettingPreferenceNew(qtiTest.QTITestClassAssignmentId, testPreferenceString);
                    string strClassName = vClass.Name;
                    if (!string.IsNullOrWhiteSpace(vClass.Course) || !string.IsNullOrWhiteSpace(vClass.Section))
                    {
                        strClassName = string.Format("{0} {1}", vClass.Course, vClass.Section);
                    }
                    var testClass = new TestClassAssignResultViewModel()
                    {
                        ID = qtiTest.QTITestClassAssignmentId,
                        ClassId = qtiTest.ClassId,
                        SchoolName = vClass.SchoolName,
                        ClassName = strClassName.Trim(),
                        DistrictTermName = vClass.DistrictTermName,
                        TeacherFirstName = vClass.TeacherFirstName,
                        TeacherLastName = vClass.TeacherLastName,
                        TestCode = qtiTest.Code,
                        Assigned = qtiTest.AssignmentDate,
                        Test = data.TestName,
                        TeacherName = string.Format("{0} {1}", vClass.TeacherFirstName, vClass.TeacherLastName),
                        Tutorial = data.IsTutorialMode == 2 ? "Tutorial " : string.Empty,
                        PortalHyperLinkTestCode = string.Empty,
                        AuthenticationCode = qtiTest.AuthenticationCode
                    };
                    if (data.IsTeacherLed)
                    {
                        testClass.PortalHyperLinkTestCode = BuildPortalHyperLinkTestCode(qtiTest.Code);
                    }
                    obj.ListClassAssign.Add(testClass);
                }
                else
                {
                    obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Class have no student active") };
                }
            }
            catch (Exception exp)
            {
                PortalAuditManager.LogException(exp);
                obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Internal error. Please try again.") };
            }
        }

        private void AssignStudent(int classId, List<Student> lstStudent, TestAssignmentData data, TestAssignResultViewModel obj)
        {
            try
            {
                var teacher = parameters.UserServices.GetUserById(data.UserId);
                DateTime currentDateTime = DateTime.UtcNow;
                if (data != null && lstStudent.Count > 0 && teacher != null)
                {
                    var isNewSkin = parameters.DistrictDecodeService.DistrictSupporPortalNewSkin(data.DistrictId);
                    var testPreferenceString = ParseTestPreferenceToString(data.ObjTestPreferenceModel, isNewSkin);
                    var objClass = parameters.ClassServices.GetClassById(classId);
                    foreach (var item in lstStudent)
                    {
                        var qtiTest = new QTITestClassAssignmentData()
                        {
                            VirtualTestId = data.TestId,
                            ClassId = classId,
                            AssignmentDate = currentDateTime,
                            Code = GenerateStudentTestCode(),
                            CodeTimestamp = currentDateTime,
                            AssignmentGuId = Guid.NewGuid().ToString(),
                            TestSetting = string.Empty,
                            Status = 1,
                            ModifiedDate = currentDateTime,
                            ModifiedUserId = CurrentUser.Id,
                            ComparisonPasscodeLength = RanComparisonPasscodeLength(),
                            Type = (int)AssignmentType.Student,
                            ModifiedBy = Constanst.PortalContain,
                            TutorialMode = data.IsTutorialMode,
                            DistrictID = data.DistrictId,
                            ListOfDisplayQuestions = data.ListOfDisplayQuestions
                        };
                        if (isNewSkin && data.RequireTestTakerAuthentication)
                        {
                            var authenticationCode = parameters.AuthenticationCodeGenerator.GenerateAuthenticationCode();
                            var expirationDate = parameters.AuthenticationCodeGenerator.GetExpirationDate(classId);
                            qtiTest.SetAuthenticationCode(authenticationCode, expirationDate);
                        }
                        parameters.QTITestClassAssignmentServices.AssignClass(qtiTest);
                        SaveTestClassStudent(item.Id, qtiTest.QTITestClassAssignmentId);
                        //TODO: Save Setting
                        //SaveSettingPreference(qtiTest.QTITestClassAssignmentId, data.DistrictId, data.BankId, false, data.TestId);
                        SaveSettingPreferenceNew(qtiTest.QTITestClassAssignmentId, testPreferenceString);
                        if (qtiTest.ComparisonPasscodeLength != null && qtiTest.AssignmentGuId.Length > qtiTest.ComparisonPasscodeLength)
                        {
                            var testStudent = new TestStudentAssignResultViewModel()
                            {
                                ID = qtiTest.QTITestClassAssignmentId,
                                StudentId = item.Id,
                                TestCode = qtiTest.Code,
                                StudentCode = item.Code,
                                ShortGUID = qtiTest.AssignmentGuId.Substring(0, qtiTest.ComparisonPasscodeLength.Value),
                                StudentFirstName = item.FirstName,
                                StudentLastName = item.LastName,
                                Assigned = qtiTest.AssignmentDate,
                                Test = data.TestName,
                                TeacherName = string.Format("{0} {1}", teacher.FirstName, teacher.LastName),
                                StudentName = string.Format("{0} {1}", item.FirstName, item.LastName),
                                Tutorial = data.IsTutorialMode == 2 ? "Tutorial " : string.Empty,
                                PortalHyperLinkTestCode = string.Empty,
                                ClassName = objClass == null ? string.Empty : objClass.Name,
                                AuthenticationCode = qtiTest.AuthenticationCode
                            };
                            if (data.IsTeacherLed)
                            {
                                testStudent.PortalHyperLinkTestCode = BuildPortalHyperLinkTestCode(qtiTest.Code);
                            }
                            obj.ListStudentAssign.Add(testStudent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Internal error. Please try again.") };
            }
        }

        private void AssignGroupStudent(int classId, List<ClassStudentCustom> lstStudent, TestAssignmentData data, TestAssignResultViewModel obj)
        {
            try
            {
                var vClass = parameters.ClassCustomServices.GetClassById(classId);
                DateTime currentDateTime = DateTime.UtcNow;
                if (data != null && lstStudent.Count > 0 && vClass != null)
                {
                    var isNewSkin = parameters.DistrictDecodeService.DistrictSupporPortalNewSkin(data.DistrictId);
                    var testPreferenceString = ParseTestPreferenceToString(data.ObjTestPreferenceModel, isNewSkin);

                    foreach (var item in lstStudent)
                    {
                        var qtiTest = new QTITestClassAssignmentData()
                        {
                            VirtualTestId = data.TestId,
                            ClassId = classId,
                            AssignmentDate = currentDateTime,
                            Code = GenerateStudentTestCode(),
                            CodeTimestamp = currentDateTime,
                            AssignmentGuId = Guid.NewGuid().ToString(),
                            TestSetting = string.Empty,
                            Status = 1,
                            ModifiedDate = currentDateTime,
                            ModifiedUserId = CurrentUser.Id,
                            ComparisonPasscodeLength = RanComparisonPasscodeLength(),
                            Type = (int)AssignmentType.Student,
                            ModifiedBy = Constanst.PortalContain,
                            TutorialMode = data.IsTutorialMode,
                            DistrictID = data.DistrictId
                        };
                        if (isNewSkin && data.RequireTestTakerAuthentication)
                        {
                            var authenticationCode = parameters.AuthenticationCodeGenerator.GenerateAuthenticationCode();
                            var expirationDate = parameters.AuthenticationCodeGenerator.GetExpirationDate(classId);
                            qtiTest.SetAuthenticationCode(authenticationCode, expirationDate);
                        }
                        parameters.QTITestClassAssignmentServices.AssignClass(qtiTest);
                        SaveTestClassStudent(item.StudentId, qtiTest.QTITestClassAssignmentId);
                        //TODO: Save Setting
                        //SaveSettingPreference(qtiTest.QTITestClassAssignmentId, data.DistrictId, data.BankId, true, data.TestId);
                        SaveSettingPreferenceNew(qtiTest.QTITestClassAssignmentId, testPreferenceString);
                        if (qtiTest.ComparisonPasscodeLength != null && qtiTest.AssignmentGuId.Length > qtiTest.ComparisonPasscodeLength)
                        {
                            var testGropuStudent = new TestGroupStudentAssignResultViewModel()
                            {
                                ID = qtiTest.QTITestClassAssignmentId,
                                SchoolName = vClass.SchoolName,
                                DistrictTermName = vClass.DistrictTermName,
                                TeacherFirstName = vClass.TeacherFirstName,
                                TeacherLastName = vClass.TeacherLastName,
                                ClassName = vClass.Name,
                                StudentId = item.StudentId,
                                StudentCode = item.Code,
                                TestCode = qtiTest.Code,
                                ShortGUID = qtiTest.AssignmentGuId.Substring(0, qtiTest.ComparisonPasscodeLength.Value),
                                StudentFirstName = item.FirstName,
                                StudentLastName = item.LastName,
                                Assigned = qtiTest.AssignmentDate,
                                Test = data.TestName,
                                TeacherName = string.Format("{0} {1}", vClass.TeacherFirstName, vClass.TeacherLastName),
                                StudentName = string.Format("{0} {1}", item.FirstName, item.LastName),
                                Tutorial = data.IsTutorialMode == 2 ? "Tutorial " : string.Empty,
                                PortalHyperLinkTestCode = string.Empty,
                                AuthenticationCode = qtiTest.AuthenticationCode
                            };
                            if (data.IsTeacherLed)
                            {
                                testGropuStudent.PortalHyperLinkTestCode = BuildPortalHyperLinkTestCode(qtiTest.Code);
                            }
                            obj.ListGroupStudent.Add(testGropuStudent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Internal error. Please try again.") };
            }
        }

        private string ParseTestPreferenceToString(TestPreferenceModel objTestPreferenceModel, bool isNewSkin)
        {
            var hideSupportHightlightText = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.HideSupportHighlightText, "false");
            if (hideSupportHightlightText.ToLower() == "true")
            {
                parameters.PreferencesServices.OffSupportHighLightText(objTestPreferenceModel);
            }

            if(!isNewSkin)
            {
                parameters.PreferencesServices.OffRequireTestTakerAuthentication(objTestPreferenceModel);
            }

            return parameters.PreferencesServices.ConvertTestPreferenceModelToString(objTestPreferenceModel);
        }

        private void SaveSettingPreferenceNew(int id, string testPreferenceString)
        {
            var preferences = new Preferences()
            {
                Id = id,
                Level = ContaintUtil.TestPreferenceLevelTestAssignment,
                Label = ContaintUtil.TestPreferenceLabelTest,
                Value = testPreferenceString,
                UpdatedBy = CurrentUser.Id
            };
            parameters.PreferencesServices.SaveAssignment(preferences);
        }

        private void SaveSettingPreferenceOnLinePreview(int assignmentId, Preferences objPreference)
        {
            objPreference.Id = assignmentId;
            objPreference.Level = ContaintUtil.TestPreferenceLevelTestAssignment;
            parameters.PreferencesServices.SaveAssignment(objPreference);
        }

        private TestSettingsMap GetTestPreference(bool isMultiClass, bool isLockedBank, bool isReload, int currentDistrictId, int testId)
        {
            var obj = new ETLXmlSerialization<TestSettingsMap>();
            int levelAccess = (int)TestPreferenceLevel.User;
            var model = new TestSettingsMap();
            string strSessionKey = string.Format("Session_{0}_{1}", currentDistrictId, testId);

            var preference = new Preferences();
            var vPre = new Preferences();

            string strOAGradedItems = string.Empty;
            var preOAGradedItems = parameters.PreferencesServices.GetPreferenceByLevelAndId(CurrentUser.Id, currentDistrictId, ContaintUtil.TestPreferenceLevelDistrict);
            if (preOAGradedItems != null)
            {
                var mapOAGradedItems = obj.DeserializeXmlToObject(preOAGradedItems.Value);
                strOAGradedItems = mapOAGradedItems.TestSettingViewModel.OverrideAutoGradedTextEntry;
            }

            if (isReload && Session[strSessionKey] != null)
            {
                preference = (Preferences)Session[strSessionKey];
            }
            else
            {
                preference = parameters.PreferencesServices.GetPreferenceByLevelAndID(testId, ContaintUtil.TestPreferenceLevelTest);
                if (preference == null)
                {
                    if (isLockedBank) //TODO: isMultiClass ||
                    {
                        preference = preOAGradedItems;
                        //preference = parameters.PreferencesServices.GetPreferenceByLevelAndId(CurrentUser.Id, currentDistrictId, ContaintUtil.TestPreferenceLevelDistrict);
                    }
                    else
                    {
                        preference = parameters.PreferencesServices.GetPreferenceByLevelAndId(CurrentUser.Id,
                            currentDistrictId, ContaintUtil.TestPreferenceLevelUser);
                    }
                }
            }

            var preferenceValue = "";
            if (preference != null)
            {
                preferenceValue = preference.Value;
                var tsmap = obj.DeserializeXmlToObject(preference.Value);
                if (tsmap != null)
                {
                    tsmap.InitDefault();
                    if (tsmap.LockItemViewModel == null) tsmap.LockItemViewModel = new LockItemViewModel();

                    //TODO: TestExtract
                    var lstDistrictDecode = parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(currentDistrictId, Util.DistrictDecode_TestScoreExtract);

                    tsmap.TestSettingViewModel.IsUseTestExtract = lstDistrictDecode.Any();
                    //\End TestExtract

                    if (CheckNeedSetDefaultValue(tsmap))
                    {
                        if (isLockedBank)
                            levelAccess = (int)TestPreferenceLevel.District;
                        SetDefaultValueOptionLevel(tsmap, levelAccess, CurrentUser.Id, currentDistrictId);
                    }
                    var test = parameters.TestServices.GetTestById(testId);
                    model = tsmap;
                    model.TestId = testId;
                    if (test != null)
                    {
                        model.VirtualTestSubTypeID = test.VirtualTestSubTypeId ?? 0;
                        model.VirtualTestSourceId = test.VirtualTestSourceId;
                        model.NavigationMethodID = test.NavigationMethodId;
                        model.DataSetCaterogyID = test.DataSetCategoryID;
                    }
                    model.TestSettingViewModel.SettingLevel = preference.Level;
                    model.TestSettingViewModel.IslockedBank = isLockedBank;
                    model.TestSettingViewModel.DistrictId = currentDistrictId;
                    model.TestSettingViewModel.IsAssignGroup = isMultiClass;
                    model.LockedAll = CheckLockedAll(model);

                    if (string.IsNullOrEmpty(strOAGradedItems))
                    {
                        model.TestSettingViewModel.OverrideAutoGradedTextEntry = strOAGradedItems;
                    }
                    preference.Value = obj.SerializeObjectToXmlWithOutHeader(model);
                    Session[strSessionKey] = preference;
                }

                model.TestPreferenceModel = parameters.PreferencesServices.ConvertToTestPreferenceModel(preferenceValue);
            }
            return model;
        }

        private bool CheckLockedAll(TestSettingsMap obj)
        {
            LockItemViewModel myObject = obj.LockItemViewModel;
            TestSettingsViewModel settings = obj.TestSettingViewModel;
            var properties = myObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                var value = prop.GetValue(myObject, null);
                if (prop.Name.Equals("LockShowTimeLimitWarning"))
                {
                    if (settings.TimeLimit.Equals("1") && !value.Equals("1"))
                        return false;
                }
                else if (prop.Name.Equals("LockTestExtract"))
                {
                    if (settings.IsUseTestExtract && !value.Equals("1"))
                        return false;
                }
                else if (prop.Name.Equals("LockAdaptiveTest"))
                {
                    if (obj.DataSetCaterogyID == (int)DataSetCategoryEnum.LUMINA && !value.Equals("1"))
                        return false;
                }
                else
                {
                    if (!value.Equals("1"))
                        return false;
                }
            }
            return true;
        }

        private bool CheckNeedSetDefaultValue(TestSettingsMap obj)
        {
            if (string.IsNullOrEmpty(obj.TestSettingViewModel.OverrideAutoGradedTextEntry)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.PassagePositioninTestTaker)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.SupportHighlightText)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.EliminateChoiceTool)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.FlagItemTool)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.TimeLimit)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.MultipleChoiceClickMethod)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.EnableVideoControls)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.TestExtract)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.SectionBasedTesting)
                || string.IsNullOrEmpty(obj.TestSettingToolViewModel.SimplePalette)
                || string.IsNullOrEmpty(obj.TestSettingToolViewModel.FrenchPalette)
                || string.IsNullOrEmpty(obj.TestSettingToolViewModel.MathPalette)
                || string.IsNullOrEmpty(obj.TestSettingToolViewModel.SpanishPalette)
                || string.IsNullOrEmpty(obj.TestSettingToolViewModel.Protractor)
                || string.IsNullOrEmpty(obj.TestSettingToolViewModel.SupportCalculator)
                || string.IsNullOrEmpty(obj.TestSettingToolViewModel.ScientificCalculator)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.LockedDownTestTaker)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.BrowserLockdownMode)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.AdaptiveTest)
                || string.IsNullOrEmpty(obj.TestSettingViewModel.EnableAudio)
               )
                return true;
            return false;
        }

        private void SetDefaultValueOptionLevel(TestSettingsMap obj, int levelSetting, int currentUserId, int currentDistrictId)
        {
            var vPreEnterprise = parameters.PreferencesServices.GetPreferenceByLevelAndId(currentUserId, currentDistrictId, ContaintUtil.TestPreferenceLevelEnterprise);
            var vTSMSerial = new ETLXmlSerialization<TestSettingsMap>();
            var vTestSettingMapEnterprise = vTSMSerial.DeserializeXmlToObject(vPreEnterprise.Value);
            if (vTestSettingMapEnterprise != null)
                vTestSettingMapEnterprise.InitDefault();

            switch (levelSetting)
            {
                case (int)TestPreferenceLevel.Enterprise:
                    {
                        //TODO: Set Default Value
                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.PassagePositioninTestTaker))
                            obj.TestSettingViewModel.PassagePositioninTestTaker = "0";
                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.SupportHighlightText))
                            obj.TestSettingViewModel.SupportHighlightText = "0";
                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.EliminateChoiceTool))
                            obj.TestSettingViewModel.EliminateChoiceTool = "0";
                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.FlagItemTool))
                            obj.TestSettingViewModel.FlagItemTool = "0";
                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.TimeLimit))
                        {
                            obj.TestSettingViewModel.FlagItemTool = "0";
                            obj.TestSettingViewModel.Duration = 0;
                            obj.TestSettingViewModel.TimeLimitDurationType = 1;
                            obj.TestSettingViewModel.Deadline = string.Empty;
                        }
                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.MultipleChoiceClickMethod))
                            obj.TestSettingViewModel.MultipleChoiceClickMethod = "0";
                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.EnableVideoControls))
                            obj.TestSettingViewModel.MultipleChoiceClickMethod = "1";
                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.TestExtract))
                            obj.TestSettingViewModel.TestExtract = "1";
                        if (string.IsNullOrEmpty(obj.TestSettingToolViewModel.Protractor))
                            obj.TestSettingToolViewModel.Protractor = "0";
                        if (string.IsNullOrEmpty(obj.TestSettingToolViewModel.FrenchPalette))
                            obj.TestSettingToolViewModel.FrenchPalette = "1";
                        if (string.IsNullOrEmpty(obj.TestSettingToolViewModel.SupportCalculator))
                            obj.TestSettingToolViewModel.SupportCalculator = "0";
                        if (string.IsNullOrEmpty(obj.TestSettingToolViewModel.ScientificCalculator))
                            obj.TestSettingToolViewModel.ScientificCalculator = "0";
                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.LockedDownTestTaker))
                            obj.TestSettingViewModel.LockedDownTestTaker = "1";

                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.BrowserLockdownMode))
                            obj.TestSettingViewModel.BrowserLockdownMode = "1";

                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.EnableAudio))
                            obj.TestSettingViewModel.LockedDownTestTaker = "1";

                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.SectionBasedTesting))
                            obj.TestSettingViewModel.SectionBasedTesting = "0";
                        if (string.IsNullOrEmpty(obj.TestSettingViewModel.AdaptiveTest))
                            obj.TestSettingViewModel.AdaptiveTest = "0";
                    }
                    break;

                case (int)TestPreferenceLevel.District:
                    {
                        SetDefaultValueOptionDistrict(vTestSettingMapEnterprise, obj);
                    }
                    break;

                default:
                    {
                        var vPreDistrict = parameters.PreferencesServices.GetPreferenceByLevelAndId(currentUserId, currentDistrictId, ContaintUtil.TestPreferenceLevelDistrict);
                        var vTestSettingMapDistrict = vTSMSerial.DeserializeXmlToObject(vPreDistrict.Value);
                        if (vTestSettingMapDistrict != null)
                        {
                            vTestSettingMapDistrict.InitDefault();
                        }
                        SetDefaultValueOptionUser(vTestSettingMapEnterprise, vTestSettingMapDistrict, obj);
                    }
                    break;
            }
        }

        private void SetDefaultValueOptionDistrict(TestSettingsMap objEnterprise, TestSettingsMap objDistrict)
        {
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.OverrideAutoGradedTextEntry))
            {
                objDistrict.TestSettingViewModel.OverrideAutoGradedTextEntry =
                    objEnterprise.TestSettingViewModel.OverrideAutoGradedTextEntry;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.PassagePositioninTestTaker))
            {
                objDistrict.TestSettingViewModel.PassagePositioninTestTaker =
                    objEnterprise.TestSettingViewModel.PassagePositioninTestTaker;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.SupportHighlightText))
            {
                objDistrict.TestSettingViewModel.SupportHighlightText =
                    objEnterprise.TestSettingViewModel.SupportHighlightText;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.EliminateChoiceTool))
            {
                objDistrict.TestSettingViewModel.EliminateChoiceTool =
                    objEnterprise.TestSettingViewModel.EliminateChoiceTool;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.FlagItemTool))
            {
                objDistrict.TestSettingViewModel.FlagItemTool =
                    objEnterprise.TestSettingViewModel.FlagItemTool;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.TimeLimit))
            {
                objDistrict.TestSettingViewModel.TimeLimit = objEnterprise.TestSettingViewModel.TimeLimit;
                objDistrict.TestSettingViewModel.Duration = objEnterprise.TestSettingViewModel.Duration;
                objDistrict.TestSettingViewModel.TimeLimitDurationType = objEnterprise.TestSettingViewModel.TimeLimitDurationType;
                objDistrict.TestSettingViewModel.Deadline = objEnterprise.TestSettingViewModel.Deadline;
                objDistrict.TestSettingViewModel.ShowTimeLimitWarning = objEnterprise.TestSettingViewModel.ShowTimeLimitWarning;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.MultipleChoiceClickMethod))
            {
                objDistrict.TestSettingViewModel.MultipleChoiceClickMethod = objEnterprise.TestSettingViewModel.MultipleChoiceClickMethod;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.TestExtract))
            {
                objDistrict.TestSettingViewModel.TestExtract = objEnterprise.TestSettingViewModel.TestExtract;
            }

            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.EnableVideoControls))
            {
                objDistrict.TestSettingViewModel.EnableVideoControls =
                    objEnterprise.TestSettingViewModel.EnableVideoControls;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.FrenchPalette))
            {
                objDistrict.TestSettingToolViewModel.FrenchPalette =
                    objEnterprise.TestSettingToolViewModel.FrenchPalette;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.Protractor))
            {
                objDistrict.TestSettingToolViewModel.Protractor =
                    objEnterprise.TestSettingToolViewModel.Protractor;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.SupportCalculator))
            {
                objDistrict.TestSettingToolViewModel.SupportCalculator =
                    objEnterprise.TestSettingToolViewModel.SupportCalculator;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.ScientificCalculator))
            {
                objDistrict.TestSettingToolViewModel.ScientificCalculator =
                    objEnterprise.TestSettingToolViewModel.ScientificCalculator;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.LockedDownTestTaker))
            {
                objDistrict.TestSettingViewModel.LockedDownTestTaker =
                    objEnterprise.TestSettingViewModel.LockedDownTestTaker;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.BrowserLockdownMode))
            {
                objDistrict.TestSettingViewModel.BrowserLockdownMode =
                    objEnterprise.TestSettingViewModel.BrowserLockdownMode;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.EnableAudio))
            {
                objDistrict.TestSettingViewModel.EnableAudio =
                    objEnterprise.TestSettingViewModel.EnableAudio;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.SectionBasedTesting))
            {
                objDistrict.TestSettingViewModel.SectionBasedTesting = objEnterprise.TestSettingViewModel.SectionBasedTesting;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.AdaptiveTest))
            {
                objDistrict.TestSettingViewModel.AdaptiveTest = objEnterprise.TestSettingViewModel.AdaptiveTest;
            }

            if (string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.SimplePalette))
            {
                objDistrict.TestSettingToolViewModel.SimplePalette =
                    objEnterprise.TestSettingToolViewModel.SimplePalette;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.MathPalette))
            {
                objDistrict.TestSettingToolViewModel.MathPalette =
                    objEnterprise.TestSettingToolViewModel.MathPalette;
            }
            if (string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.SpanishPalette))
            {
                objDistrict.TestSettingToolViewModel.SpanishPalette =
                    objEnterprise.TestSettingToolViewModel.SpanishPalette;
            }
        }

        private void SetDefaultValueOptionUser(TestSettingsMap objEnterprise, TestSettingsMap objDistrict, TestSettingsMap objUser)
        {
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.OverrideAutoGradedTextEntry))
            {
                objUser.TestSettingViewModel.OverrideAutoGradedTextEntry =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.OverrideAutoGradedTextEntry) ?
                    objEnterprise.TestSettingViewModel.OverrideAutoGradedTextEntry : objDistrict.TestSettingViewModel.OverrideAutoGradedTextEntry;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.PassagePositioninTestTaker))
            {
                objUser.TestSettingViewModel.PassagePositioninTestTaker =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.PassagePositioninTestTaker) ?
                    objEnterprise.TestSettingViewModel.PassagePositioninTestTaker : objDistrict.TestSettingViewModel.PassagePositioninTestTaker;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.SupportHighlightText))
            {
                objUser.TestSettingViewModel.SupportHighlightText =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.SupportHighlightText) ?
                    objEnterprise.TestSettingViewModel.SupportHighlightText : objDistrict.TestSettingViewModel.SupportHighlightText;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.EliminateChoiceTool))
            {
                objUser.TestSettingViewModel.EliminateChoiceTool =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.EliminateChoiceTool) ?
                    objEnterprise.TestSettingViewModel.EliminateChoiceTool : objDistrict.TestSettingViewModel.EliminateChoiceTool;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.FlagItemTool))
            {
                objUser.TestSettingViewModel.FlagItemTool =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.FlagItemTool) ?
                    objEnterprise.TestSettingViewModel.FlagItemTool : objDistrict.TestSettingViewModel.FlagItemTool;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.TimeLimit))
            {
                if (string.IsNullOrEmpty(objDistrict.TestSettingViewModel.TimeLimit))
                {
                    objUser.TestSettingViewModel.TimeLimit = objEnterprise.TestSettingViewModel.TimeLimit;
                    objUser.TestSettingViewModel.Duration = objEnterprise.TestSettingViewModel.Duration;
                    objUser.TestSettingViewModel.Deadline = objEnterprise.TestSettingViewModel.Deadline;
                    objUser.TestSettingViewModel.ShowTimeLimitWarning = objEnterprise.TestSettingViewModel.ShowTimeLimitWarning;
                }
                else
                {
                    objUser.TestSettingViewModel.TimeLimit = objDistrict.TestSettingViewModel.TimeLimit;
                    objUser.TestSettingViewModel.Duration = objDistrict.TestSettingViewModel.Duration;
                    objUser.TestSettingViewModel.TimeLimitDurationType = objUser.TestSettingViewModel.TimeLimitDurationType;
                    objUser.TestSettingViewModel.Deadline = objDistrict.TestSettingViewModel.Deadline;
                    objUser.TestSettingViewModel.ShowTimeLimitWarning = objDistrict.TestSettingViewModel.ShowTimeLimitWarning;
                }
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.MultipleChoiceClickMethod))
            {
                objUser.TestSettingViewModel.MultipleChoiceClickMethod =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.MultipleChoiceClickMethod) ?
                    objEnterprise.TestSettingViewModel.MultipleChoiceClickMethod : objDistrict.TestSettingViewModel.MultipleChoiceClickMethod;
            }

            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.EnableVideoControls))
            {
                objUser.TestSettingViewModel.EnableVideoControls =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.EnableVideoControls) ?
                    objEnterprise.TestSettingViewModel.EnableVideoControls : objDistrict.TestSettingViewModel.EnableVideoControls;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.TestExtract))
            {
                objUser.TestSettingViewModel.TestExtract =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.TestExtract) ?
                    objEnterprise.TestSettingViewModel.TestExtract : objDistrict.TestSettingViewModel.TestExtract;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingToolViewModel.FrenchPalette))
            {
                objUser.TestSettingToolViewModel.FrenchPalette =
                    string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.FrenchPalette) ?
                    objEnterprise.TestSettingToolViewModel.FrenchPalette : objDistrict.TestSettingToolViewModel.FrenchPalette;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingToolViewModel.SimplePalette))
            {
                objUser.TestSettingToolViewModel.SimplePalette =
                    string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.SimplePalette) ?
                    objEnterprise.TestSettingToolViewModel.SimplePalette : objDistrict.TestSettingToolViewModel.SimplePalette;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingToolViewModel.MathPalette))
            {
                objUser.TestSettingToolViewModel.MathPalette =
                    string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.MathPalette) ?
                    objEnterprise.TestSettingToolViewModel.MathPalette : objDistrict.TestSettingToolViewModel.MathPalette;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingToolViewModel.SpanishPalette))
            {
                objUser.TestSettingToolViewModel.SpanishPalette =
                    string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.SpanishPalette) ?
                    objEnterprise.TestSettingToolViewModel.SpanishPalette : objDistrict.TestSettingToolViewModel.SpanishPalette;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingToolViewModel.Protractor))
            {
                objUser.TestSettingToolViewModel.Protractor =
                    string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.Protractor)
                        ? objEnterprise.TestSettingToolViewModel.Protractor
                        : objDistrict.TestSettingToolViewModel.Protractor;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingToolViewModel.SupportCalculator))
            {
                objUser.TestSettingToolViewModel.SupportCalculator =
                    string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.SupportCalculator) ?
                    objEnterprise.TestSettingToolViewModel.SupportCalculator : objDistrict.TestSettingToolViewModel.SupportCalculator;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingToolViewModel.ScientificCalculator))
            {
                objUser.TestSettingToolViewModel.ScientificCalculator =
                    string.IsNullOrEmpty(objDistrict.TestSettingToolViewModel.ScientificCalculator) ?
                    objEnterprise.TestSettingToolViewModel.ScientificCalculator : objDistrict.TestSettingToolViewModel.ScientificCalculator;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.SectionBasedTesting))
            {
                objUser.TestSettingViewModel.SectionBasedTesting =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.SectionBasedTesting) ?
                    objEnterprise.TestSettingViewModel.SectionBasedTesting : objDistrict.TestSettingViewModel.SectionBasedTesting;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.LockedDownTestTaker))
            {
                objUser.TestSettingViewModel.LockedDownTestTaker =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.LockedDownTestTaker) ?
                    objEnterprise.TestSettingViewModel.LockedDownTestTaker : objDistrict.TestSettingViewModel.LockedDownTestTaker;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.BrowserLockdownMode))
            {
                objUser.TestSettingViewModel.BrowserLockdownMode =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.BrowserLockdownMode) ?
                    objEnterprise.TestSettingViewModel.BrowserLockdownMode : objDistrict.TestSettingViewModel.BrowserLockdownMode;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.EnableAudio))
            {
                objUser.TestSettingViewModel.EnableAudio =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.EnableAudio) ?
                    objEnterprise.TestSettingViewModel.EnableAudio : objDistrict.TestSettingViewModel.EnableAudio;
            }
            if (string.IsNullOrEmpty(objUser.TestSettingViewModel.AdaptiveTest))
            {
                objUser.TestSettingViewModel.AdaptiveTest =
                    string.IsNullOrEmpty(objDistrict.TestSettingViewModel.AdaptiveTest) ?
                    objEnterprise.TestSettingViewModel.AdaptiveTest : objDistrict.TestSettingViewModel.AdaptiveTest;
            }
        }

        private void AssignTeacherPreview(TestAssignmentData data, TestAssignResultViewModel obj, Preferences pre)
        {
            try
            {
                DateTime currentDateTime = DateTime.UtcNow;
                var vClass = parameters.ClassCustomServices.GetClassById(data.ClassId);
                int vStudentId = 0;
                int.TryParse(data.StudentIdList.First(), out vStudentId);
                var vStudent = parameters.StudentServices.GetStudentById(vStudentId);
                if (vClass != null && vStudent != null)
                {
                    var isNewSkin = parameters.DistrictDecodeService.DistrictSupporPortalNewSkin(data.DistrictId);
                    var testPreferenceString = ParseTestPreferenceToString(data.ObjTestPreferenceModel, isNewSkin);

                    var qtiTest = new QTITestClassAssignmentData()
                    {
                        VirtualTestId = data.TestId,
                        ClassId = data.ClassId,
                        AssignmentDate = currentDateTime,
                        Code = GenerateStudentTestCode(),
                        CodeTimestamp = currentDateTime,
                        AssignmentGuId = Guid.NewGuid().ToString(),
                        TestSetting = string.Empty,
                        Status = 1,
                        ModifiedDate = currentDateTime,
                        ModifiedUserId = CurrentUser.Id,
                        ComparisonPasscodeLength = RanComparisonPasscodeLength(),
                        Type = (int)AssignmentType.TeacherPreview,
                        ModifiedBy = Constanst.PortalContain,
                        DistrictID = data.DistrictId
                    };
                    if (isNewSkin && data.RequireTestTakerAuthentication)
                    {
                        var authenticationCode = parameters.AuthenticationCodeGenerator.GenerateAuthenticationCode();
                        var expirationDate = parameters.AuthenticationCodeGenerator.GetExpirationDate(data.ClassId);
                        qtiTest.SetAuthenticationCode(authenticationCode, expirationDate);
                    }

                    parameters.QTITestClassAssignmentServices.AssignClass(qtiTest);
                    SaveTestClassStudent(vStudentId, qtiTest.QTITestClassAssignmentId);
                    //TODO: Save Setting
                    if (pre == null) //Teacher Preview on Assignment page
                    {
                        //SaveSettingPreference(qtiTest.QTITestClassAssignmentId, data.DistrictId, data.BankId, false, data.TestId);
                        SaveSettingPreferenceNew(qtiTest.QTITestClassAssignmentId, testPreferenceString);
                    }
                    else // Teacher preview on TestEditer page
                    {
                        SaveSettingPreferenceOnLinePreview(qtiTest.QTITestClassAssignmentId, pre);
                    }

                    if (qtiTest.ComparisonPasscodeLength != null && qtiTest.AssignmentGuId.Length > qtiTest.ComparisonPasscodeLength)
                    {
                        var testClass = new TestClassAssignResultViewModel()
                        {
                            ID = qtiTest.QTITestClassAssignmentId,
                            ClassId = qtiTest.ClassId,
                            SchoolName = vClass.SchoolName,
                            ClassName = vClass.Name.Trim(),
                            DistrictTermName = vClass.DistrictTermName,
                            TeacherFirstName = vClass.TeacherFirstName,
                            TeacherLastName = vClass.TeacherLastName,
                            TestCode = qtiTest.Code,
                            Assigned = qtiTest.AssignmentDate,
                            Test = data.TestName,
                            TeacherName = string.Format("{0} {1}", vClass.TeacherFirstName, vClass.TeacherLastName),
                            AuthenticationCode = qtiTest.AuthenticationCode
                        };
                        obj.ListClassAssign.Add(testClass);
                    }
                }
                else
                {
                    obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Default Class, school, teacher, student or ClassUser not set") };
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                obj.ErrorList = new List<ValidationFailure> { new ValidationFailure("error", "Internal error. Please try again.") };
            }
        }

        private void InitDefaultTeacherPreview(TestAssignmentData data)
        {
            data.ClassId = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.PreviewTestClassID, 0);
            data.DistrictTermId = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.PreviewTestTermID, 0);
            data.SchoolId = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.PreviewTestSchoolID, 0);
            data.UserId = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.PreviewTestTeacherID, 0);
            data.StudentIdList.Add(parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.PreviewTestStudentID, "0"));
            data.DistrictId = CurrentUser.DistrictId.HasValue
                             ? CurrentUser.DistrictId.Value
                             : parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.PreviewTestDistrictID, 272);
        }

        [HttpPost]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentRewrite)]
        [AjaxOnly]
        public ActionResult OnlineTestPreview(TestAssignmentData data, TestPreferenceModel objTestPreferenceModel)
        {
            int currentDistrictId = data.DistrictId;
            if (currentDistrictId <= 0)
                currentDistrictId = CurrentUser.DistrictId.GetValueOrDefault();

            // Restriction
            var isAllow = parameters.RestrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
            {
                BankId = data.BankId,
                TestId = data.TestId,
                DistrictId = data.DistrictId,
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                ModuleCode = RestrictionConstant.Module_ReviewOnline,
                ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test
            });

            if (isAllow == false)
            {
                return Json(new { error = "Permission to preview this test has been restricted." }, JsonRequestBehavior.AllowGet);
            }

            if (ValidatePermissionTestAssignmentData(data) == false)
            {
                return Json(new { error = "Has no right to the assignment." }, JsonRequestBehavior.AllowGet);
            }

            var obj = new TestAssignResultViewModel();
            data.ObjTestPreferenceModel = parameters.PreferencesServices.ClearAllAtribute(objTestPreferenceModel);
            data.RequireTestTakerAuthentication = data.ObjTestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals(Constanst.REQUIRE_TEST_TAKER_AUTHENTICATION))?.Value == "1";
            SetModelPermissions(data);
            if (data.UserId.Equals(0))
            {
                data.UserId = CurrentUser.Id;
            }
            if (data.AssignmentType == 5)
            {
                //Init School, Teacher, Term, Class & Student
                InitDefaultTeacherPreview(data);
            }
            data.SetValidator(testAssignmentValidator);
            if (!data.IsValid)
            {
                obj.ErrorList = data.ValidationErrors.ToList();
                string strError = obj.ErrorList.Aggregate(string.Empty, (current, e) => current + (e.ErrorMessage + "<br />"));

                return Json(new { sucess = false, error = strError });
            }

            if (!CheckAuthorizeLockedTest(data))
            {
                return Json(new { success = false, error = "The test that you have selected is in a locked bank. Teacher Preview is not available." });
            }

            if (data.AssignmentType == 5)
            {
                AssignTeacherPreview(data, obj, null); //Auto get default preference
                var teacherPreview = obj.ListClassAssign.Count > 0 ? obj.ListClassAssign.First() : new TestClassAssignResultViewModel();
                string strUrl = BuildTestTakerURLForReview(teacherPreview.TestCode, data.StudentIdList.First());
                strUrl = PassThroughSupportTestTakerNewSkin(currentDistrictId, strUrl); //PassThrough Support TestTaker New Skin                
                return Json(new { success = true, code = teacherPreview.TestCode, testtakerUrl = strUrl });
            }

            return Json(new { success = false, error = "Invalid AssignmentType. Please, try again" });
        }

        [NonAction]
        private string BuildTestTakerURLForReview(string testCode, string strStudentId)
        {
            if (!string.IsNullOrEmpty(testCode))
            {
                int districtId = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.PreviewTestDistrictID, 272);
                APIAccount apiAccount = parameters.APIAccountServices.GetAPIAccountByDistrictId(districtId);
                if (apiAccount != null)
                {
                    var assignmentData = parameters.QTITestClassAssignmentServices.GetAssignmentByTestCode(testCode);

                    var timestamp = DateTime.UtcNow.ToString();

                    var privateKey = apiAccount.LinkitPrivateKey;

                    var rawDataObject = new
                    {
                        StudentID = strStudentId,
                        TestCode = testCode,
                        AssignmentGUID = assignmentData.AssignmentGuId,
                        Timestamp = timestamp,
                        IsFromSPP = true
                    };
                    string rawData = JsonConvert.SerializeObject(rawDataObject, Formatting.Indented);

                    string strData = Util.EncryptString(rawData, privateKey);
                    var url = Util.GetConfigByKey("OnlineTestPreviewURL", "");
                    var accessKey = apiAccount.ClientAccessKeyID;
                    var env = LinkitConfigurationManager.Vault.DatabaseID;
                    var redirectUrl = string.Format(url, HttpUtility.UrlEncode(strData), HttpUtility.UrlEncode(accessKey.ToString()), env);
                    return redirectUrl;
                }
            }
            return string.Empty;
        }

        private bool CheckUserSupportQuestionGroup(int? pDistrictId)
        {
            //TODO: Maybe check Publisher or NetworkAdmin
            int districtId = pDistrictId ?? 0;
            if (districtId == 0)
            {
                if (CurrentUser.RoleId == (int)Permissions.Publisher || CurrentUser.RoleId == (int)Permissions.NetworkAdmin)
                {
                    try
                    {
                        var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                        districtId = parameters.DistrictServices.GetLiCodeBySubDomain(subDomain);
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                    }
                }
                else
                {
                    districtId = CurrentUser.DistrictId.GetValueOrDefault();
                }
            }
            var vConfiguration = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.IsSupportQuestionGroup);
            if (vConfiguration != null && vConfiguration.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                return true;
            return parameters.DistrictDecodeServices.GetDistrictDecodeByLabel(districtId, Constanst.IsSupportQuestionGroup);
        }

        #endregion Private Method

        #region Preview Online On TestDesign

        public ActionResult PreviewOnlineSettingForTestDesign(int testId)
        {
            if (!parameters.VulnerabilityService.HasRightToAccessVirtualTest(CurrentUser, testId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right on the test." }, JsonRequestBehavior.AllowGet);
            }
            int currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;

            var model = GetTestPreferenceWithTestId(testId, currentDistrictId, (int)TestPreferenceLevel.TestDesign, null);

            var hideSupportHightlightText = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.HideSupportHighlightText, "false");
            ViewBag.HideSupportHighlightText = hideSupportHightlightText;

            model.IsSupportQuestionGroup = CheckUserSupportQuestionGroup(currentDistrictId);
            ViewBag.DateFormat = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;
            ViewBag.VirtualTestId = testId;
            ViewBag.LastUpdatedDate = model.TestPreferenceModel.LastUpdatedDateString;
            ViewBag.IsTeacherOrSchoolAdmin = CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher;
            if (model.DataSetOriginId == (int)DataSetOriginEnum.Survey)
                return PartialView("_PreviewSurvey", model);
            else
                return PartialView("_TestSettingPreviewOnline", model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult OnlineTestPreviewForTestDesign(TestPreferenceModel objTestPreferenceModel, int testId)
        {
            if (!parameters.VulnerabilityService.HasRightToAccessVirtualTest(CurrentUser, testId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right on the test." }, JsonRequestBehavior.AllowGet);
            }
            var objTest = parameters.TestServices.GetTestById(testId);
            if (objTest == null)
                return Json(new { sucess = false, error = "Invalid TestId" });
            var data = new TestAssignmentData() { TestId = testId, BankId = objTest.BankId };
            var obj = new TestAssignResultViewModel();

            data.UserId = CurrentUser.Id;
            data.DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            data.AssignmentType = 5;
            data.RequireTestTakerAuthentication = objTestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals(Constanst.REQUIRE_TEST_TAKER_AUTHENTICATION))?.Value == "1";

            var pre = new Preferences() { Value = string.Empty, Label = ContaintUtil.TestPreferenceLabelTest };
            if (objTestPreferenceModel != null)
            {
                var hideSupportHightlightText = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.HideSupportHighlightText, "false");
                if (hideSupportHightlightText.ToLower() == "true")
                {
                    parameters.PreferencesServices.OffSupportHighLightText(objTestPreferenceModel);
                }
                pre.Value = parameters.PreferencesServices.ConvertTestPreferenceModelToString(parameters.PreferencesServices.ClearAllAtribute(objTestPreferenceModel));
            }
            //Init School, Teacher, Term, Class & Student
            InitDefaultTeacherPreview(data);
            AssignTeacherPreview(data, obj, pre);
            var teacherPreview = obj.ListClassAssign.Count > 0 ? obj.ListClassAssign.First() : new TestClassAssignResultViewModel();
            string strUrl = BuildTestTakerURLForReview(teacherPreview.TestCode, data.StudentIdList.First());
            strUrl = PassThroughSupportTestTakerNewSkin(data.DistrictId, strUrl); //Check PassThrough Support TestTaker New Skin.
            return Json(new { success = true, code = teacherPreview.TestCode, testtakerUrl = strUrl }, JsonRequestBehavior.AllowGet);
        }

        #endregion Preview Online On TestDesign

        #region TestSetting Property

        public ActionResult TestSettingForTestProperty(int testId, bool? layoutV2 = false)
        {
            if (!parameters.VulnerabilityService.HasRighToEditVirtualTest(CurrentUser, testId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right on the test." }, JsonRequestBehavior.AllowGet);
            }
            int currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            int currentLevel = (int)TestPreferenceLevel.TestDesign;
            var model = GetTestPreferenceWithTestId(testId, currentDistrictId, currentLevel, null);

            var hideSupportHightlightText = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.HideSupportHighlightText, "false");
            ViewBag.HideSupportHighlightText = hideSupportHightlightText;
            model.IsSupportQuestionGroup = CheckUserSupportQuestionGroup(currentDistrictId);

            ViewBag.EnableLock = true;
            ViewBag.DateFormat = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;
            ViewBag.LastUpdatedDate = model.TestPreferenceModel.LastUpdatedDateString;
            ViewBag.IsTeacherOrSchoolAdmin = CurrentUser.IsSchoolAdmin || CurrentUser.IsTeacher;
            if (layoutV2 == true)
            {
                return PartialView("v2/_TestSettingTestProperty", model);
            }
            return PartialView("_TestSettingTestProperty", model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SaveTestSettingForTestProperty(TestPreferenceModel obj, int testId)
        {
            if (!parameters.VulnerabilityService.HasRighToEditVirtualTest(CurrentUser, testId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right on the test." }, JsonRequestBehavior.AllowGet);
            }
            var objTest = parameters.TestServices.GetTestById(testId);
            if (objTest == null)
                return Json(new { sucess = false, error = "Invalid TestId" });
            if (obj != null)
            {
                var pre = parameters.PreferencesServices.GetPreferenceByLevelAndID(testId, ContaintUtil.TestPreferenceLevelTest) ??
                          new Preferences()
                          {
                              Label = ContaintUtil.TestPreferenceLabelTest,
                              Level = ContaintUtil.TestPreferenceLevelTest,
                              Id = testId
                          };
                var hideSupportHightlightText = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.HideSupportHighlightText, "false");
                if (hideSupportHightlightText.ToLower() == "true")
                {
                    parameters.PreferencesServices.OffSupportHighLightText(obj);
                }

                pre.Value = parameters.PreferencesServices.ConvertTestPreferenceModelToString(obj);
                pre.UpdatedBy = CurrentUser.Id;
                parameters.PreferencesServices.Save(pre);
                return Json(new { success = true, inforUpdated = GetInforUpdated() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        #endregion TestSetting Property

        private TestSettingsMap GetTestPreferenceWithTestId(int testId, int currentDistrictId, int currentLevelId, int? schoolId, GetTestPreferencePartialRetakeModel partialRetakeInfo = null)
        {
            var model = new TestSettingsMap();
            var objTest = parameters.TestServices.GetTestById(testId);
            if (objTest == null)
                return model;
            var districtBank = parameters.ListBankServices.GetBankDistrictByDistrictIdAndBankId(currentDistrictId, objTest.BankId);
            bool isLockedBank = districtBank != null && (districtBank.BankDistrictAccessId == (int)LockBankStatus.Restricted || districtBank.BankAccessId == (int)LockBankStatus.Restricted);
            model = BindDataToTestPrefenrenceModel(objTest, isLockedBank, currentDistrictId, currentLevelId, schoolId, partialRetakeInfo);
            model.CreatedByUserId = objTest.AuthorUserID;

            var testSections = parameters.VirtualSectionServices.GetVirtualSectionByVirtualTest(testId, partialRetakeInfo);
            var sectionOptTag = model.TestPreferenceModel.OptionTags.FirstOrDefault(tag => tag.Key == "sectionBasedTesting");
            if (testSections.Count <= 1 && sectionOptTag != null) {
                sectionOptTag.Value = "0";
                var lockAttr = sectionOptTag.Attributes.FirstOrDefault(attr => attr.Key == "lock");
                if (lockAttr == null)
                {
                    lockAttr = new TagAttr() { Key = "lock", Value = "true" };
                    sectionOptTag.Attributes.Add(lockAttr);
                } else
                {
                    lockAttr.Value = "true";
                }
            }

            return model;
        }

        public ActionResult GetTestAssignmentSetting(int testAssingmentId, bool isPartialRetake)
        {
            if (!parameters.VulnerabilityService.HasRigtToEditQtiTestClassAssignment(CurrentUser, testAssingmentId))
            {
                return Json(new { error = "Has no right to assignment." }, JsonRequestBehavior.AllowGet);
            }
            //\TungLittle Edit
            var model = new TestSettingsMap()
            {
                TestPreferenceModel = new TestPreferenceModel()
            };
            int vVirtualTestSubTypeId = 0;
            int virtualTestSourceId = 0;
            int? navigationMethodId = 0;
            int? dataSetCategoryID = 0;
            var virtualTestId = 0;
            var vTestClassAssignment = parameters.QTITestClassAssignmentServices.GetQtiTestClassAssignment(testAssingmentId);

            if (vTestClassAssignment != null)
            {
                var vVirtualtest = parameters.VirtualTestService.GetTestById(vTestClassAssignment.VirtualTestId);
                if (vVirtualtest != null)
                {
                    vVirtualTestSubTypeId = vVirtualtest.VirtualTestSubTypeID.GetValueOrDefault();
                    dataSetCategoryID = vVirtualtest.DatasetCategoryID;
                    virtualTestSourceId = vVirtualtest.VirtualTestSourceID;
                    navigationMethodId = vVirtualtest.NavigationMethodID;
                    virtualTestId = vVirtualtest.VirtualTestID;
                }
            }

            var preferenceAssignment = parameters.PreferencesServices.GetPreferenceByAssignmentLeveAndID(testAssingmentId);
            if (preferenceAssignment != null)
            {
                model.TestPreferenceModel = parameters.PreferencesServices.ConvertToTestPreferenceModel(preferenceAssignment.Value);
                model.TestPreferenceModel.VirtualSections = _virtualSectionService.GetVirtualSectionByVirtualTest(virtualTestId)
                                                              .Select(o => new VirtualSectionDto
                                                              {
                                                                  VirtualSectionId = o.VirtualSectionId,
                                                                  VirtualTestId = o.VirtualTestId,
                                                                  Order = o.Order,
                                                                  Title = o.Title
                                                              })
                                                              .OrderBy(o => o.Order)
                                                              .ToList();

                parameters.PreferencesServices.AddDefaultTagOptions(model.TestPreferenceModel.OptionTags);
                BuildLockAttributes(model.TestPreferenceModel, testAssingmentId, vTestClassAssignment.DistrictID, vTestClassAssignment.VirtualTestId);
            }
            model.VirtualTestSubTypeID = vVirtualTestSubTypeId;
            model.VirtualTestSourceId = virtualTestSourceId;
            model.NavigationMethodID = navigationMethodId;
            model.DataSetCaterogyID = dataSetCategoryID;
            var lstDistrictDecode = parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId.GetValueOrDefault(), Util.DistrictDecode_TestScoreExtract);
            var gradebookSISValue = parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId.GetValueOrDefault(), Util.DistrictDecode_SendTestResultToGenesis).Select(c => c.Value).FirstOrDefault();

            model.TestExtractOptions = new TestExtractOptions
            {
                IsUseTestExtract = lstDistrictDecode.Any() || (gradebookSISValue != null && gradebookSISValue != "0")
            };

            if (lstDistrictDecode.Any())
            {
                model.TestExtractOptions.Gradebook = true;
            }
            if (gradebookSISValue != null)
            {
                var gradebookSISIds = gradebookSISValue.ToIntArray("|").Where(c => c > 0).ToArray();
                if (gradebookSISIds.Any())
                {
                    if (gradebookSISIds.Contains((int)GradebookSIS.CleverApi))
                    {
                        model.TestExtractOptions.CleverApi = true;
                    }
                    model.TestExtractOptions.Gradebook = true;

                    if (gradebookSISIds.Contains((int)GradebookSIS.Realtime))
                    {
                        model.TestExtractOptions.StudentRecord = true;
                    }

                    if (gradebookSISIds.DoesShowExportScoreTypeOption())
                    {
                        model.TestExtractOptions.ShowRawOrPercentOption = true;
                    }
                }
            }

            foreach (var tag in model.TestPreferenceModel.OptionTags)
            {
                if (tag.Key == "deadline")
                {
                    model.Deadline = tag.Value;
                }

                if (tag.Key == "duration")
                {
                    model.Duration = tag.Value;
                }
            }
            RenderAttributesOnCaseWithoutNotSectionAvailability(model.TestPreferenceModel.OptionTags);
            var hideSupportHightlightText = parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue(Util.HideSupportHighlightText, "false");
            ViewBag.HideSupportHighlightText = hideSupportHightlightText;

            model.IsSupportQuestionGroup = CheckUserSupportQuestionGroup(CurrentUser.DistrictId.GetValueOrDefault());
            ViewBag.IsReview = true;

            var timeLimitValue = model.TestPreferenceModel.OptionTags.Find(t => t.Key == "timeLimit").Value;

            ViewBag.EnableTimeLimit = timeLimitValue == "1" ? true : false;

            ViewBag.AllowAdjustTestChedule = vTestClassAssignment.Status == (int)QTITestClassAssignmentStatusEnum.Active;

            var timeZoneId = _stateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
            ViewBag.LastUpdatedDate = preferenceAssignment.UpdatedDate.HasValue ? preferenceAssignment.UpdatedDate.Value.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(true).ToString() : preferenceAssignment.UpdatedDate.GetValueOrDefault().ToString();
            var user = parameters.UserServices.GetUserById(preferenceAssignment.UpdatedBy.GetValueOrDefault());
            var userName = user != null ? $"{user.LastName}, {user.FirstName}" : string.Empty;
            ViewBag.LastUpdatedByUser = userName;

            ViewBag.IsEditable = true;

            var testScheduleLock = model.TestPreferenceModel.OptionTags.FirstOrDefault(x => x.Key == "testSchedule");
            ViewBag.IsTestScheduleEditable = testScheduleLock != null && (!testScheduleLock.Locked || (testScheduleLock.Locked && CurrentUser.IsDistrictAdminOrPublisher));

            var sectionAvailabilityLock = model.TestPreferenceModel.OptionTags.FirstOrDefault(x => x.Key == "sectionAvailability");
            ViewBag.IsSectionAvailabilityEditable = sectionAvailabilityLock != null && (!sectionAvailabilityLock.Locked || (sectionAvailabilityLock.Locked && CurrentUser.IsDistrictAdminOrPublisher)) && !isPartialRetake;

            var requireTestTakerAuthenticationLock = model.TestPreferenceModel.OptionTags.FirstOrDefault(x => x.Key == Constanst.REQUIRE_TEST_TAKER_AUTHENTICATION);
            ViewBag.IsRequireTestTakerAuthenticationEditable = requireTestTakerAuthenticationLock != null && (!requireTestTakerAuthenticationLock.Locked || (requireTestTakerAuthenticationLock.Locked && CurrentUser.IsDistrictAdminOrPublisher));
            ViewBag.AllowAdjustRequireTestTakerAuthentication = vTestClassAssignment.Status == (int)QTITestClassAssignmentStatusEnum.Active;

            return PartialView("DefaultSettings", model);
        }
        private void BuildLockAttributes(TestPreferenceModel testPreferenceModel, int testAssignment, int districtId, int testId)
        {
            var preferences = parameters.PreferencesServices.GetPreferenceInCurrentLevel(new GetPreferencesParams
            {
                CurrrentLevelId = (int)TestPreferenceLevel.TestAssignment,
                DistrictId = districtId,
                TestAssignmentId = testAssignment,
                UserId = CurrentUser.Id,
                UserRoleId = CurrentUser.RoleId,
                IsSurvey = false,
                StateId = CurrentUser.StateId.GetValueOrDefault(),
                VirtualTestId = testId
            });

            testPreferenceModel.OptionTags.ForEach(x =>
            {
                x.Attributes = preferences.OptionTags.FirstOrDefault(y => y.Key == x.Key)?.Attributes;
            });

            testPreferenceModel.ToolTags.ForEach(x =>
            {
                x.Attributes = preferences.ToolTags.FirstOrDefault(y => y.Key == x.Key)?.Attributes;
            });
        }

        #region Optimize Preferences

        [NonAction]
        private TestSettingsMap BindDataToTestPrefenrenceModel(Test objTest, bool islockedbank, int districtId, int currentLevelId, int? schoolId, GetTestPreferencePartialRetakeModel partialRetakeInfo)
        {
            int currentDistrictId = districtId;
            int currentUserId = CurrentUser.Id;
            if (!CurrentUser.IsPublisher())
            {
                currentDistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0;
            }
            var tsmapTest = new TestSettingsMap()
            {
                IslockedBank = islockedbank,
                VirtualTestSubTypeID = objTest.VirtualTestSubTypeId.GetValueOrDefault(),
                VirtualTestSourceId = objTest.VirtualTestSourceId,
                DataSetCaterogyID = objTest.DataSetCategoryID,
                NavigationMethodID = objTest.NavigationMethodId,
                TestId = objTest.Id,
                DistrictId = districtId,
                SettingLevelID = 0,
                TestPreferenceModel = new TestPreferenceModel(),
                DataSetOriginId = objTest.DataSetOriginID.GetValueOrDefault()
            };
            var isSurvey = objTest.DataSetOriginID == (int)DataSetOriginEnum.Survey;

            var preferences = parameters.PreferencesServices.GetPreferenceInCurrentLevel(new GetPreferencesParams
            {
                CurrrentLevelId = currentLevelId,
                DistrictId = districtId,
                UserId = CurrentUser.Id,
                UserRoleId = CurrentUser.RoleId,
                VirtualTestId = objTest.Id,
                StateId = CurrentUser.StateId.GetValueOrDefault(),
                SchoolId = schoolId ?? 0
            });
            if (preferences != null)
            {
                AddTooltipForPreferences(preferences);
                tsmapTest.TestPreferenceModel = preferences;
                var levelName = string.Empty;
                levels.TryGetValue(currentLevelId, out levelName);
                tsmapTest.SettingLevel = levelName;
                CorrectSectionBasedTesting(preferences);
            }

            tsmapTest.TestPreferenceModel.VirtualSections = _virtualSectionService.GetVirtualSectionByVirtualTest(objTest.Id, partialRetakeInfo)
                   .Select(o => new VirtualSectionDto
                   {
                       VirtualSectionId = o.VirtualSectionId,
                       VirtualTestId = o.VirtualTestId,
                       Order = o.Order,
                       Title = o.Title
                   })
                   .OrderBy(o => o.Order)
                   .ToList();

            var lstDistrictDecode = parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.DistrictDecode_TestScoreExtract);
            var gradebookSISValue = parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.DistrictDecode_SendTestResultToGenesis)
                .Select(c => c.Value).FirstOrDefault();

            tsmapTest.TestExtractOptions = new TestExtractOptions
            {
                IsUseTestExtract = lstDistrictDecode.Any() || (gradebookSISValue != null && gradebookSISValue != "0")
            };

            if (lstDistrictDecode.Any())
            {
                tsmapTest.TestExtractOptions.Gradebook = true;
            }

            if (gradebookSISValue != null)
            {
                var gradebookSISIds = gradebookSISValue.ToIntArray("|").Where(c => c > 0).ToArray();
                if (gradebookSISIds.Any())
                {
                    if (gradebookSISIds.Contains((int)GradebookSIS.CleverApi))
                    {
                        tsmapTest.TestExtractOptions.CleverApi = true;
                    }
                    tsmapTest.TestExtractOptions.Gradebook = true;

                    if (gradebookSISIds.Contains((int)GradebookSIS.Realtime))
                    {
                        tsmapTest.TestExtractOptions.StudentRecord = true;
                    }
                    if (gradebookSISIds.DoesShowExportScoreTypeOption())
                    {
                        tsmapTest.TestExtractOptions.ShowRawOrPercentOption = true;
                    }
                }
            }

            if (!tsmapTest.IsUseTestExtract)
            {
                //TODO: only use thi value when " + LabelHelper.DistrictLabel + " have record on DistrictDecode
                var tmp = tsmapTest.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals("TestExtract"));
                if (tmp != null)
                    tmp.Value = "0";
            }

            if (CurrentUser.RoleId == (int)Permissions.Publisher ||
                CurrentUser.RoleId == (int)Permissions.DistrictAdmin ||
                CurrentUser.RoleId == (int)Permissions.NetworkAdmin)
            {
                tsmapTest.CanEditOverrideAutoGradedItems = true;
            }
            RenderAttributesOnCaseWithoutNotSectionAvailability(tsmapTest.TestPreferenceModel.OptionTags);
            var vDeadLine = tsmapTest.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals("deadline"));
            var vDuration = tsmapTest.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals("duration"));
            if (vDeadLine != null)
                tsmapTest.Deadline = vDeadLine.Value;
            if (vDuration != null)
                tsmapTest.Duration = vDuration.Value;
            var vDurationType = tsmapTest.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals("timeLimitDurationType"));
            if (vDurationType != null)
                tsmapTest.TimeLimitDurationType = vDurationType.Value;

            var vDataTime = DateTime.Now;
            if (DateTime.TryParse(tsmapTest.Deadline, out vDataTime))
            {
                tsmapTest.DeadlineDisplay = vDataTime;
            }
            if (CurrentUser.IsTeacher)
            {
                //Check if this bank is locked for the district of current teacher user or not
                //Teacher can not change setting if the bank is locked
                var virtualTest = parameters.VirtualTestService.GetTestById(objTest.Id);
                tsmapTest.IslockedBank = parameters.BankDistrictService.IsLocked(virtualTest.BankID, CurrentUser.DistrictId.GetValueOrDefault());
                //get the bank of the test
            }

            FixDistrictLabelForTestPreferenceModel(tsmapTest);
            var optionBranchingTest = tsmapTest.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals("branchingTest"));
            //TODO: Auto set value to key: branchingTest
            optionBranchingTest.Value = GetOptionBranchingTestByTest(objTest.NavigationMethodId);
            return tsmapTest;
        }

        private static void RenderAttributesOnCaseWithoutNotSectionAvailability(List<Tag> optionTags)
        {
            var sectionBasedTesting = optionTags.FirstOrDefault(t => t.Key.Equals("sectionBasedTesting"));
            var aritributeOnBased = sectionBasedTesting.Attributes.FirstOrDefault(a => a.Key.Equals("on"));
            var valueOnBased = aritributeOnBased != null ? aritributeOnBased.Value : "0";
            var aritributeLockBased = sectionBasedTesting.Attributes.FirstOrDefault(a => a.Key.Equals("lock"));
            var valueLockBased = aritributeLockBased != null ? aritributeLockBased.Value : "false";
            var sectionAvailability = optionTags.FirstOrDefault(t => t.Key.Equals("sectionAvailability"));
            if (sectionAvailability != null)
            {
                if (sectionAvailability.Attributes == null)
                {
                    sectionAvailability.Attributes = sectionBasedTesting.Attributes;
                }
                else
                {
                    var valueOnAvailability = sectionAvailability.Attributes.FirstOrDefault(a => a.Key.Equals("on"));
                    var valueLockAvailability = sectionAvailability.Attributes.FirstOrDefault(a => a.Key.Equals("lock"));
                    sectionAvailability.Attributes = new List<TagAttr>() { new TagAttr() { Key = "on", Value = valueOnAvailability != null ? valueOnAvailability.Value : valueOnBased }, new TagAttr() { Key = "lock", Value = valueLockAvailability != null ? valueLockAvailability.Value : (sectionBasedTesting != null && sectionBasedTesting.Value == "1") ? "false" : valueLockBased } };
                }
            }
            else
            {
                sectionAvailability = new Tag() { Key = "sectionAvailability" };
                sectionAvailability.Attributes = new List<TagAttr>() { new TagAttr() { Key = "on", Value = valueOnBased }, new TagAttr() { Key = "lock", Value = valueLockBased } };
                optionTags.Add(sectionAvailability);
            };
        }

        private void CorrectSectionBasedTesting(TestPreferenceModel preference)
        {
            var sectionBaseTesting = preference.OptionTags.FirstOrDefault(t => t.Key.Equals("sectionBasedTesting"));
            if (sectionBaseTesting != null && sectionBaseTesting.Value != "1")
            {
                var (shouldBeChecked, shouldBeLocked) = CheckSectionBaseTesting(preference);

                if (shouldBeChecked)
                {
                    sectionBaseTesting.Value = "1";
                }

                if (shouldBeLocked)
                {
                    var lockAttr = sectionBaseTesting.Attributes?.FirstOrDefault(a => a.Key.Equals("lock"));
                    if (lockAttr != null)
                    {
                        lockAttr.Value = "true";
                    }
                    else
                    {
                        if (sectionBaseTesting.Attributes == null)
                        {
                            sectionBaseTesting.Attributes = new List<TagAttr>();
                        }

                        sectionBaseTesting.Attributes.Add(new TagAttr
                        {
                            Key = "lock",
                            Value = "true"
                        });
                    }
                }
            }
        }

        private (bool shouldBeChecked, bool shouldBeLocked) CheckSectionBaseTesting(TestPreferenceModel preference)
        {
            var shouldBeChecked = false;
            var shouldBeLocked = false;

            var sectionAvailability = preference.OptionTags.FirstOrDefault(t => t.Key.Equals("sectionAvailability"));
            if (sectionAvailability?.Attributes != null)
            {
                var valueAttr = sectionAvailability.Attributes.FirstOrDefault(a => a.Key.Equals("on"));
                if (valueAttr != null && valueAttr.Value == "1")
                {
                    shouldBeChecked = true;
                    if (sectionAvailability.Locked)
                    {
                        shouldBeLocked = true;
                    }
                }
            }

            if (!shouldBeChecked)
            {
                var allTags = preference.OptionTags.Concat(preference.ToolTags);
                var allSetAtSectionItemTags = allTags.Where(t => SectionLevelList.AllTagMapping.Keys.Contains(t.Key));
                if (allSetAtSectionItemTags.Any())
                {
                    shouldBeChecked = true;
                    foreach (var setAtSectionItemTag in allSetAtSectionItemTags)
                    {
                        var lockKey = SectionLevelList.AllTagMapping[setAtSectionItemTag.Key];
                        var locktag = allTags.FirstOrDefault(t => t.Key.Equals(lockKey));
                        if (locktag != null && locktag.Locked)
                        {
                            shouldBeLocked = true;
                            break;
                        }
                    }
                }
            }

            return (shouldBeChecked, shouldBeLocked);
        }

        private string GetOptionBranchingTestByTest(int? navigationMethodId)
        {
            switch (navigationMethodId)
            {
                case (int)NavigationMethodEnum.NORMAL_BRANCHING:
                    return "1";

                case (int)NavigationMethodEnum.SECTION_BASE_BRANCHING:
                    return "3";

                case (int)NavigationMethodEnum.ALGORITHMIC_BRANCHING:
                    return "2";

                default:
                    return "0";
            }
        }

        private void FixDistrictLabelForTestPreferenceModel(TestSettingsMap tsmapTest)
        {
            if (tsmapTest.SettingLevel.Equals(ContaintUtil.TestPreferenceLevelDistrict,
                StringComparison.OrdinalIgnoreCase))
            {
                tsmapTest.SettingLevel = LabelHelper.DistrictLabel;
            }
        }

        [NonAction]
        private void InitDefaultValueLevelDistrict(TestSettingsMap tsmapEnterPrise, TestSettingsMap tsmap)
        {
            foreach (var optionTag in tsmapEnterPrise.TestPreferenceModel.OptionTags)
            {
                var currentTagOption = tsmap.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals(optionTag.Key));
                if (currentTagOption == null || string.IsNullOrEmpty(currentTagOption.Value))
                {
                    if (currentTagOption == null)
                    {
                        tsmap.TestPreferenceModel.OptionTags.Add(new Tag() { Key = optionTag.Key, Value = optionTag.Value });
                    }
                    else
                    {
                        currentTagOption.Value = optionTag.Value;
                    }
                }
            }
            foreach (var toolTag in tsmapEnterPrise.TestPreferenceModel.ToolTags)
            {
                var currentTagTool = tsmap.TestPreferenceModel.ToolTags.FirstOrDefault(o => o.Key.Equals(toolTag.Key));
                if (currentTagTool == null || string.IsNullOrEmpty(currentTagTool.Value))
                {
                    if (currentTagTool == null)
                    {
                        tsmap.TestPreferenceModel.ToolTags.Add(new Tag() { Key = toolTag.Key, Value = toolTag.Value });
                    }
                    else
                    {
                        currentTagTool.Value = toolTag.Value;
                    }
                }
            }
        }

        [NonAction]
        private void InitDefaultValueLevelUser(TestSettingsMap tsmapEnterPrise, TestSettingsMap tsmapDistrict, TestSettingsMap tsmap)
        {
            foreach (var optionTag in tsmapEnterPrise.TestPreferenceModel.OptionTags)
            {
                var currentTagOption = tsmap.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals(optionTag.Key));
                var vValueOption = string.Empty;
                if (currentTagOption == null || string.IsNullOrEmpty(currentTagOption.Value))
                {
                    var currentDistrictTagOption = tsmapDistrict.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals(optionTag.Key));
                    if (currentDistrictTagOption != null && !string.IsNullOrEmpty(currentDistrictTagOption.Value))
                    {
                        vValueOption = currentDistrictTagOption.Value;
                    }
                    else
                    {
                        vValueOption = optionTag.Value;
                    }

                    if (currentTagOption == null)
                    {
                        tsmap.TestPreferenceModel.OptionTags.Add(new Tag() { Key = optionTag.Key, Value = vValueOption });
                    }
                    else
                    {
                        currentTagOption.Value = vValueOption;
                    }
                }
            }
            foreach (var toolTag in tsmapEnterPrise.TestPreferenceModel.ToolTags)
            {
                var currentTagTool = tsmap.TestPreferenceModel.ToolTags.FirstOrDefault(o => o.Key.Equals(toolTag.Key));
                var vValueTool = string.Empty;
                if (currentTagTool == null || string.IsNullOrEmpty(currentTagTool.Value))
                {
                    var currentDistrictTagTool = tsmapDistrict.TestPreferenceModel.ToolTags.FirstOrDefault(o => o.Key.Equals(toolTag.Key));
                    if (currentDistrictTagTool != null && !string.IsNullOrEmpty(currentDistrictTagTool.Value))
                    {
                        vValueTool = currentDistrictTagTool.Value;
                    }
                    else
                    {
                        vValueTool = toolTag.Value;
                    }
                    //Set Value
                    if (currentTagTool == null)
                    {
                        tsmap.TestPreferenceModel.ToolTags.Add(new Tag() { Key = toolTag.Key, Value = vValueTool });
                    }
                    else
                    {
                        currentTagTool.Value = vValueTool;
                    }
                }
            }
        }

        [NonAction]
        private void InitDefaultValueLevelTest(TestSettingsMap tsmapEnterPrise, TestSettingsMap tsmapDistrict, TestSettingsMap tsmapUser, TestSettingsMap tsmap)
        {
            foreach (var optionTag in tsmapEnterPrise.TestPreferenceModel.OptionTags)
            {
                var currentTagOption = tsmap.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals(optionTag.Key));
                if (currentTagOption == null || string.IsNullOrEmpty(currentTagOption.Value))
                {
                    var vValueOption = string.Empty;
                    var userTagOption = tsmapUser.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals(optionTag.Key));
                    if (userTagOption != null && !string.IsNullOrEmpty(userTagOption.Value))
                    {
                        vValueOption = userTagOption.Value;
                    }
                    else
                    {
                        var districtTagOption = tsmapDistrict.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals(optionTag.Key));
                        if (districtTagOption != null && !string.IsNullOrEmpty(districtTagOption.Value))
                        {
                            vValueOption = districtTagOption.Value;
                        }
                        else
                        {
                            vValueOption = optionTag.Value;
                        }
                    }
                    //Set
                    if (currentTagOption == null)
                    {
                        tsmap.TestPreferenceModel.OptionTags.Add(new Tag() { Key = optionTag.Key, Value = vValueOption });
                    }
                    else
                    {
                        currentTagOption.Value = vValueOption;
                    }
                }
            }
            foreach (var toolTag in tsmapEnterPrise.TestPreferenceModel.ToolTags)
            {
                var currentTagTool = tsmap.TestPreferenceModel.ToolTags.FirstOrDefault(o => o.Key.Equals(toolTag.Key));
                var vValueTool = string.Empty;
                if (currentTagTool == null || string.IsNullOrEmpty(currentTagTool.Value))
                {
                    var userTagTool = tsmapUser.TestPreferenceModel.ToolTags.FirstOrDefault(o => o.Key.Equals(toolTag.Key));
                    if (userTagTool != null && !string.IsNullOrEmpty(userTagTool.Value))
                    {
                        vValueTool = userTagTool.Value;
                    }
                    else
                    {
                        var districtTagTool = tsmapDistrict.TestPreferenceModel.ToolTags.FirstOrDefault(o => o.Key.Equals(toolTag.Key));
                        if (districtTagTool != null && !string.IsNullOrEmpty(districtTagTool.Value))
                        {
                            vValueTool = districtTagTool.Value;
                        }
                        else
                        {
                            vValueTool = toolTag.Value;
                        }
                    }
                    if (currentTagTool == null)
                    {
                        tsmap.TestPreferenceModel.ToolTags.Add(new Tag() { Key = toolTag.Key, Value = vValueTool });
                    }
                    else
                    {
                        currentTagTool.Value = vValueTool;
                    }
                }
            }
        }

        [NonAction]
        private string BuildPortalHyperLinkTestCode(string testCode)
        {
            var url = Util.GetConfigByKey("PortalhyperlinkTestCode", "");
            var redirectUrl = url + testCode;
            return redirectUrl;
        }

        #endregion Optimize Preferences

        public ActionResult PortalLink(string code, int districtId)
        {
            if (!string.IsNullOrEmpty(code) && districtId > 0)
            {
                var assignment = parameters.QTITestClassAssignmentServices.GetTestClassAssignmentsPassThrough(code, false, false, CurrentUser.Id, districtId);
                if (assignment.Any())
                {
                    var strUrl = BuildPortalHyperLinkTestCode(code);
                    return Redirect(strUrl);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public JsonResult CheckTeacherLedAssignSameTest(bool isTeacherLed, int assignmentType, int testId, int classId)
        {
            // If the test is teacher-led Check previous assignment
            if (isTeacherLed && assignmentType == 1)
            {
                var previousAssignment = parameters.QTITestClassAssignmentServices.GetLatestActiveAssignmentByVirtualTestIAndClass(testId, classId);
                if (previousAssignment != null)
                {
                    return Json(new { Success = true, IsAssignSameTest = true, HyperLink = BuildPortalHyperLinkTestCode(previousAssignment.Code), TestCode = previousAssignment.Code });
                }
            }

            return Json(new { Success = true, IsAssignSameTest = false, HyperLink = string.Empty });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateTestSchedule(int testAssignmentId, List<Tag> scheduleValues)
        {
            QTITestClassAssignmentData vTestClassAssignment = parameters.QTITestClassAssignmentServices.GetQtiTestClassAssignment(testAssignmentId);
            if (vTestClassAssignment != null && vTestClassAssignment.Status == (int)QTITestClassAssignmentStatusEnum.Active)
            {
                var result = parameters.PreferencesServices.UpdateOptionTags(testAssignmentId, scheduleValues);
                return Json(new
                {
                    result.Status,
                    UpdatedDate = GetUpdatedDateStr(result.UpdatedDate)
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateRequireTestTakerAuthentication(int testAssignmentId, List<Tag> tags)
        {
            QTITestClassAssignmentData vTestClassAssignment = parameters.QTITestClassAssignmentServices.GetQtiTestClassAssignment(testAssignmentId);

            if (vTestClassAssignment == null || vTestClassAssignment.Status != (int)QTITestClassAssignmentStatusEnum.Active)
            {
                return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
            }

            var requireTestTakerAuthentication = tags?.FirstOrDefault(x => x.Key == Constanst.REQUIRE_TEST_TAKER_AUTHENTICATION);
            if (requireTestTakerAuthentication == null)
            {
                return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
            }

            var requireTestTakerAuthenticationValue = requireTestTakerAuthentication.Value == "1" ? "1" : "0";

            var tagsToUpdate = new List<Tag> {
                new Tag {
                    Key = Constanst.REQUIRE_TEST_TAKER_AUTHENTICATION,
                    Value = requireTestTakerAuthenticationValue
                }
            };

            var result = parameters.PreferencesServices.UpdateOptionTags(testAssignmentId, tagsToUpdate);
            if (result.Status)
            {
                parameters.QTITestClassAssignmentServices.GenerateAuthenticationCode(testAssignmentId, requireTestTakerAuthenticationValue == "1");
            }

            return Json(new
            {
                result.Status,
                UpdatedDate = GetUpdatedDateStr(result.UpdatedDate)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateSectionAvailability(int testAssignmentId, List<Tag> sectionAvailabilityTags)
        {
            QTITestClassAssignmentData vTestClassAssignment = parameters.QTITestClassAssignmentServices.GetQtiTestClassAssignment(testAssignmentId);
            if (vTestClassAssignment != null && vTestClassAssignment.Status == (int)QTITestClassAssignmentStatusEnum.Active)
            {
                var result = parameters.PreferencesServices.UpdatePartOfPreference(testAssignmentId, sectionAvailabilityTags);
                return Json(new
                {
                    result.Status,
                    UpdatedDate = GetUpdatedDateStr(result.UpdatedDate)
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdjustEndDateOfTestPreferencesSchedule(int testAssignmentId, string endDate, int offset)
        {
            QTITestClassAssignmentData vTestClassAssignment = parameters.QTITestClassAssignmentServices.GetQtiTestClassAssignment(testAssignmentId);
            if (!CurrentUser.IsTeacher && !CurrentUser.IsSchoolAdmin && vTestClassAssignment != null && vTestClassAssignment.Status == (int)QTITestClassAssignmentStatusEnum.Active)
            {
                var result = parameters.PreferencesServices.AdjustEndDateOfTestPreferencesSchedule(testAssignmentId, endDate, offset);
                return Json(new { Status = result }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckIsLaunchTeacherLedTest()
        {
            int districtId = GetCurrentDistrictId();

            bool result = parameters.DistrictDecodeService.GetDistrictDecodeOrConfigurationByLabel(districtId, Constanst.IsLaunchTeacherLedTest, defaultValue: false);
            return result;

            //var vConfiguration = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.IsLaunchTeacherLedTest);
            //if (vConfiguration != null && vConfiguration.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            //    return true;
            //return parameters.DistrictDecodeServices.GetDistrictDecodeByLabel(districtId, Constanst.IsLaunchTeacherLedTest);
        }

        private int GetCurrentDistrictId()
        {
            int districtId = 0;
            if (CurrentUser.RoleId == (int)Permissions.Publisher || CurrentUser.RoleId == (int)Permissions.NetworkAdmin)
            {
                var subDomain = HelperExtensions.GetSubdomain();
                districtId = parameters.DistrictServices.GetLiCodeBySubDomain(subDomain);
            }
            else
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }

            return districtId;
        }

        public JsonResult GetIsShowTutorialMode(int districtId)
        {
            bool isShowTutorialMode = parameters.DistrictDecodeService.GetDistrictDecodeOrConfigurationByLabel(districtId, Util.IsShowTutorialMode, true);
            return Json(isShowTutorialMode, JsonRequestBehavior.AllowGet);
        }

        private void AddTooltipForPreferences(TestPreferenceModel model)
        {
            model.OptionTags.ForEach(o =>
            {
                o.Tooltips = LocalizeHelper.LocalizedToString($"TestPreference_{o.Key}").ToString();
            });

            model.ToolTags.ForEach(o =>
            {
                o.Tooltips = LocalizeHelper.LocalizedToString($"TestPreference_{o.Key}").ToString();
            });
        }

        private object GetInforUpdated()
        {
            var now = DateTime.UtcNow;
            var timeZoneId = _stateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
            var lastUpdatedDate = !string.IsNullOrEmpty(timeZoneId) ? now.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(true) : now.DisplayDateWithFormat(true);
            var user = parameters.UserServices.GetUserById(CurrentUser.Id);
            var fullName = user != null ? $"{user.LastName}, {user.FirstName}" : string.Empty;

            return new { LastUpdatedDate = lastUpdatedDate, LastUpdatedByUser = fullName };
        }

        private string PassThroughSupportTestTakerNewSkin(int districtId, string strUrlPassThrough)
        {
            strUrlPassThrough = string.Format("{0}&districtId={1}", strUrlPassThrough, districtId);
            if (districtId > 0 && parameters.DistrictDecodeServices.DistrictSupportTestTakerNewSkin(districtId))
            {
                return string.Format("{0}&{1}", strUrlPassThrough, ContaintUtil.TESTTAKER_NEWSKIN);
            }

            return strUrlPassThrough;
        }


        [HttpGet]
        //[AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentRewrite)]
        public ActionResult AssignmentRetake(Guid? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            var model = new AssignmentRetakeViewModel()
            {
                DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0,
                GUID = id.ToString()
            };

            //TODO: Validation           


            var selectTest = parameters.QTITestClassAssignmentServices.GetGradeSubjectBankTestForRetake(model.GUID);
            if (selectTest != null)
            {
                model.TestName = selectTest.TestName;
                model.BankName = selectTest.BankName;
                model.SubjectName = selectTest.SubjectName;
                model.GradeName = selectTest.GradeName;
                model.OriginalVirtualTestID = selectTest.VirtualTestID;
                model.CurrentVirtualTestID = selectTest.CurrentVirtualTestID;
                GetStudentRetakes(model);

                if (CurrentUser.IsDistrictAdminOrPublisher)
                {
                    model.DistrictId = selectTest.DistrictID;
                }
            }

            ViewBag.HasPrintResultRetake = true;
            ViewBag.DateFormat = parameters.ConfigurationServices.GetConfigurationByKey(Constanst.JQueryDateFormat)?.Value;
            return View(model);
        }


        [HttpGet]
        public PartialViewResult GetAssignmentRetakeStudentTable(Guid guid)
        {
            var model = new AssignmentRetakeViewModel()
            {
                DistrictId = CurrentUser.DistrictId.HasValue ? CurrentUser.DistrictId.Value : 0,
                GUID = guid.ToString()
            };
            var selectTest = parameters.QTITestClassAssignmentServices.GetGradeSubjectBankTestForRetake(model.GUID);
            if (selectTest != null)
            {
                model.TestName = selectTest.TestName;
                model.BankName = selectTest.BankName;
                model.SubjectName = selectTest.SubjectName;
                model.GradeName = selectTest.GradeName;
                model.OriginalVirtualTestID = selectTest.VirtualTestID;
                model.CurrentVirtualTestID = selectTest.CurrentVirtualTestID;
                GetStudentRetakes(model);
            }
            return PartialView("_AssigmentRetakeStudentTable", model);
        }

        [HttpPost]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.OnlineTestAssignmentRewrite)]
        public ActionResult AssignmentRetake(TestRetakeData data, TestPreferenceModel testPreferenceModel)
        {
            if (data != null && data.StudentList != null && data.StudentList.Any())
            {
                var retakeTestName = $"{data.TestName} FR{data.TestRetakeNumber}";
                if (data.RetakeType == AssignmentRetakeConstants.RETAKE_TYPE_PARTIAL)
                {
                    retakeTestName = $"{data.TestName} PR{data.TestRetakeNumber}";
                }
                retakeTestName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(retakeTestName));
                CopyVirtualTestMappingOuputModel mappingOutput = null;
                var test = parameters.VirtualTestService.GetRetakeVirtualTestByTestName(data.OriginalTestId, retakeTestName);
                if (test == null)
                {
                    test = parameters.VirtualTestService.GetVirtualTestById(data.TestId);
                    test.Name = retakeTestName;
                    test.DatasetOriginID = (int)DataSetOriginEnum.Item_Based_Score_Retake;
                    test.OriginalTestID = data.OriginalTestId;
                    test.ParentTestID = data.TestId;
                    CopyVirtualTest(test, out mappingOutput);
                    parameters.RubricModuleCommandService.CloneVirtualTest(test.OriginalTestID.Value, test.VirtualTestID);
                    var s3VirtualTest = parameters.VirtualTestService.CreateS3Object(test.VirtualTestID);//New VirtualTestID
                    var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                    // Copy ALL PBS configurations from the ParentTest test.
                    if (test.ParentTestID.HasValue && test.VirtualTestID > 0)
                    {
                        parameters.VirtualTestService.ClonePBSForTestRetake(test.ParentTestID.Value, test.VirtualTestID);
                    }

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
                else
                {
                    mappingOutput = new CopyVirtualTestMappingOuputModel {  SectionsMapping = new Dictionary<int, int>() };
                    var originalSections = parameters.VirtualSectionServices.GetVirtualSectionByVirtualTest(data.OriginalTestId)
                        .OrderBy(x => x.Order).ToArray();
                    var retakeSections = parameters.VirtualSectionServices.GetVirtualSectionByVirtualTest(test.VirtualTestID)
                        .OrderBy(x => x.Order).ToArray();

                    if (originalSections.Any() && retakeSections.Any())
                    {
                        for (int i = 0; i < originalSections.Length; i++)
                        {
                            var foundRetakeSection = retakeSections.FirstOrDefault(x => x.Order == originalSections[i].Order);
                            if (foundRetakeSection != null)
                            {
                                mappingOutput.SectionsMapping.Add(originalSections[i].VirtualSectionId, foundRetakeSection.VirtualSectionId);
                            }
                        }
                    }
                }
                var newStudentList = parameters.QTITestClassAssignmentServices.GetRetakeDataStudentInfo(test.VirtualTestID, data.StudentList);
                if(newStudentList != null)
                {
                    data.StudentList = newStudentList;
                }

                data.TestPreferenceModel = parameters.PreferencesServices.ClearAllAtribute(testPreferenceModel);
                RebindSectionPreferences(data.TestPreferenceModel, mappingOutput.SectionsMapping);
                var studentAndListOfDisplayQuestions = new List<RetakeListOfDisplayQuestionsDto>();
                if (data.RetakeType == AssignmentRetakeConstants.RETAKE_TYPE_PARTIAL)
                {
                    var studentIds = string.Join(",", data.StudentList.Select(x => x.StudentId).ToArray());
                    studentAndListOfDisplayQuestions = parameters.QTITestClassAssignmentServices.GetRetakeListOfDisplayQuestions(data.TestId, studentIds, data.GUID, test.Name);
                }
                var requireTestTakerAuthentication = data.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals(Constanst.REQUIRE_TEST_TAKER_AUTHENTICATION))?.Value == "1";

                var assigmentsData = data.StudentList
                    .Distinct()
                    .Select(x => new TestAssignmentData
                    {
                        AssignmentType = (int)DisplayAssignmentType.StudentAssignment,
                        DistrictId = data.DistrictId,
                        ClassId = x.ClassId,
                        StudentID = x.StudentId,
                        IsTutorialMode = 0,
                        TestId = test.VirtualTestID,
                        TestName = test.Name,
                        UserId = CurrentUser.Id,
                        ObjTestPreferenceModel = data.TestPreferenceModel,
                        ListOfDisplayQuestions = studentAndListOfDisplayQuestions.FirstOrDefault(y => y.ClassID == x.ClassId && y.StudentID == x.StudentId)?.ListOfDisplayQuestions,
                        RequireTestTakerAuthentication = requireTestTakerAuthentication
                    })
                    .ToList();
                
                AssignTestToStudents(assigmentsData);

                return Json(new { Success = true, IsTeacherLed = false, HyperLink = "" });
            }

            return Json(new { Success = false, IsTeacherLed = false, HyperLink = "" });
        }

        private void RebindSectionPreferences(TestPreferenceModel testPreferenceModel, Dictionary<int, int> sectionsMapping)
        {
            if (testPreferenceModel.ToolTags != null && testPreferenceModel.ToolTags.Any())
            {
                testPreferenceModel.ToolTags = testPreferenceModel.ToolTags.Select(toolTag => new Tag
                {
                    Key = toolTag.Key,
                    Value = toolTag.Value,
                    Attributes = toolTag.Attributes,
                    SectionItems = toolTag.SectionItems.Select(o => new Tag
                    {
                        Key = o.Key,
                        Value = o.Value,
                        Attributes = o.Attributes.Select(y =>
                        {
                            var value = y.Value;
                            if (y.Key == "sectionId" && int.TryParse(y.Value, out int sectionId)
                                && sectionsMapping.TryGetValue(sectionId, out int mappingSectionId))
                            {
                                value = mappingSectionId.ToString();
                            }

                            return new TagAttr
                            {
                                Key = y.Key,
                                Value = value
                            };
                        }).ToList()
                    }).ToList()
                }).ToList();
            }

            if (testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.Any())
            {
                testPreferenceModel.OptionTags = testPreferenceModel.OptionTags.Select(optionTag => new Tag()
                {
                    Key = optionTag.Key,
                    Value = optionTag.Value,
                    Attributes = optionTag.Attributes,
                    SectionItems = optionTag.SectionItems.Select(o => new Tag
                    {
                        Key = o.Key,
                        Value = o.Value,
                        Attributes = o.Attributes.Select(y =>
                        {
                            var value = y.Value;
                            if (y.Key == "sectionId" && int.TryParse(y.Value, out int sectionId)
                                && sectionsMapping.TryGetValue(sectionId, out int mappingSectionId))
                            {
                                value = mappingSectionId.ToString();
                            }

                            return new TagAttr
                            {
                                Key = y.Key,
                                Value = value
                            };
                        }).ToList()
                    }).ToList()
                }).ToList();
            }
        }

        public ActionResult LoadExistAssignmentForRetake(Guid guid)
        {
            TestAssignResultViewModel model = new TestAssignResultViewModel
            {
                IsStudentAssign = true
            };

            ViewBag.RetakeAssignmentRequestGuid = guid;
            return PartialView("_TestRetakeAssignResult", model);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult LoadAssignmentForRetake(GetTestClassAssignmentCriteria criteria)
        {
            var request = new LoadAssignmentRetakeRequest();
            MappingLoadAssignmentForRetakeRequest(criteria, request);
            var retakeTestAssignResult = parameters.QTITestClassAssignmentServices.GetRetakeTestAssignResults(request);

            //var parser = new DataTableParser<RetakeTestAssignResultViewModel>();

            //return Json(parser.Parse(retakeTestAssignResult.AsQueryable(), true), JsonRequestBehavior.AllowGet);

            var result = new GenericDataTableResponse<RetakeTestAssignResultViewModel>()
            {
                aaData = new List<RetakeTestAssignResultViewModel>(),
                iTotalDisplayRecords = 0,
                iTotalRecords = 0,
                sColumns = criteria.sColumns,
                sEcho = criteria.sEcho
            };

            if (retakeTestAssignResult != null && retakeTestAssignResult.RetakeTestAssignResults.Count > 0)
            {
                result.aaData = retakeTestAssignResult.RetakeTestAssignResults;// retakeTestAssignResult.Skip(criteria.iDisplayStart).Take(criteria.iDisplayLength).ToList();
                result.iTotalDisplayRecords = retakeTestAssignResult.TotalRecord;
                result.iTotalRecords = retakeTestAssignResult.TotalRecord;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpPost]
        public ActionResult GenerateAuthenticationCode(TestAssignmentGenerateAuthenticationCode request)
        {
            var authenticationCode = parameters.QTITestClassAssignmentServices.GenerateAuthenticationCode(request);

            if (!string.IsNullOrEmpty(authenticationCode))
            {
                return Json(new
                {
                    Status = true,
                    AuthenticationCode = authenticationCode
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
        }

        #region Clone Virtual Test
        [NonAction]
        private void CopyVirtualTest(VirtualTestData virtualTestData, out CopyVirtualTestMappingOuputModel mappingOutput)
        {
            mappingOutput = new CopyVirtualTestMappingOuputModel();
            int oldTestId = virtualTestData.VirtualTestID;
            virtualTestData.VirtualTestID = 0;
            virtualTestData.CreatedDate = DateTime.UtcNow;
            virtualTestData.UpdatedDate = DateTime.UtcNow;
            parameters.VirtualTestService.Save(virtualTestData);
            //TODO: Clone Relationship
            CloneRelationShipVirtualTest(oldTestId, virtualTestData.VirtualTestID);

            //TODO: Clone Question, VirtualSection, VirtualSectionQuestion

            CloneQuestions(oldTestId, virtualTestData.VirtualTestID, virtualTestData.Name, out Dictionary<int, int> sectionsMapping);
            mappingOutput.SectionsMapping = sectionsMapping;
            parameters.VirtualTestService.UpdateBaseVirtualQuestionClone(oldTestId, virtualTestData.VirtualTestID);
        }

        [NonAction]
        private void CloneQuestions(int oldTestId, int newTestId, string testName, out Dictionary<int, int> sectionsMapping)
        {
            var cloneQTIItemModelsByOldQuestionId = new Dictionary<int, CloneQTIItemModel>();
            var cloneVirtualQuestion = new List<CloneVirtualQuestion>();

            //TODO: Create VirtualQuestion
            CloneQuestionsByTestId(oldTestId, newTestId, cloneQTIItemModelsByOldQuestionId, cloneVirtualQuestion);

            //TODO: Clone Question RelationShip
            CloneQuestionRelationship(cloneVirtualQuestion);

            //TODO: Create Section & SectionQuestion
            CloneVirtualSection(oldTestId, newTestId, cloneQTIItemModelsByOldQuestionId, out sectionsMapping);

            //TODO: Create QtiItem
            CloneQTIITemAndUpdateQuestions(testName, cloneQTIItemModelsByOldQuestionId);
        }

        private void CloneQuestionRelationship(List<CloneVirtualQuestion> cloneVQs)
        {
            if (cloneVQs.Count == 0)
            {
                return;
            }

            parameters.VirtualQuestionItemTagServices.CloneVirtualQuestionItemTag(cloneVQs);

            //TODO: Clone VirtualQuestionLessonOne
            parameters.VirtualQuestionLessonOneServices.CloneVirtualQuestionLessonOne(cloneVQs);

            //TODO: Clone VirtualQuestionLessonTwo
            parameters.VirtualQuestionLessonTwoServices.CloneVirtualQuestionLessonTwo(cloneVQs);

            //TODO: Clone VirtualQuestionStandard (not use )

            //TODO: Clone VirtualQuestionStateStandard
            parameters.MasterStandardServices.CloneVirtualQuestionStateStandard(cloneVQs);

            //TODO: Clone VirtualQuestionTopic
            parameters.VirtualQuestionTopicServices.CloneVirtualQuestionTopic(cloneVQs);
        }

        private void CloneQTIITemAndUpdateQuestions(string testName, Dictionary<int, CloneQTIItemModel> cloneQTIItemModelsByOldQuestionId)
        {
            //TODO: Create ItemBank (QTIIBank)
            var vQTIBank = parameters.QtiBankServices.CreateQTIBankByUserName(CurrentUser.UserName, CurrentUser.Id);

            //TODO: Create ItemSet (QTIGroup)
            var vQtiGroup = parameters.QtiGroupServices.CreateItemSetByUserId(CurrentUser.Id, vQTIBank.QtiBankId, testName);

            //TODO: CloneQTIITem
            CloneQTIItemWithQTIIGroupId(vQtiGroup.QtiGroupId, cloneQTIItemModelsByOldQuestionId);
        }

        private void CloneQTIItemWithQTIIGroupId(int qtigroupId, Dictionary<int, CloneQTIItemModel> cloneQTIItemModelsByOldQuestionId)
        {
            if (qtigroupId > 0 && cloneQTIItemModelsByOldQuestionId.Count > 0)
            {
                //Need to upload media file (image,audio),if any, to S3
                var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
                var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;

                foreach (KeyValuePair<int, CloneQTIItemModel> qtiItemModelPair in cloneQTIItemModelsByOldQuestionId)
                {
                    //TODO: Clone QTIITem
                    int newQTIITeamId = parameters.QTIITemServices.DuplicateQTIItemForTest(CurrentUser.Id,
                        qtiItemModelPair.Value.OldQTIITemID, qtigroupId, qtiItemModelPair.Key, qtiItemModelPair.Value.NewQuestionID, true, bucketName,
                        folder, LinkitConfigurationManager.GetS3Settings().S3Domain);
                    cloneQTIItemModelsByOldQuestionId[qtiItemModelPair.Key].NewQTIITemID = newQTIITeamId;

                    //TODO: Update QuestionID
                    parameters.VirtualQuestionServices.UpdateQIITemIdbyQuestionId(qtiItemModelPair.Value.NewQuestionID, newQTIITeamId);
                    UpdateItemPassage(newQTIITeamId);
                }
            }
        }
        private void UpdateItemPassage(int qtiItemId)
        {
            var qtiItem = parameters.QTIITemServices.GetQtiItemById(qtiItemId);
            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            qtiItem.XmlContent = qtiItem.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
            qtiItem.XmlContent = qtiItem.XmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();
            List<PassageViewModel> passageList = Util.GetPassageList(qtiItem.XmlContent, false);
            if (passageList != null)
            {
                parameters.QTIITemServices.UpdateItemPassage(qtiItemId, passageList.Select(x => x.QtiRefObjectID).ToList(),
                    passageList.Select(x => x.RefNumber).ToList());
            }
        }
        private void CloneQuestionsByTestId(int oldTestId, int newTestId, Dictionary<int, CloneQTIItemModel> cloneQTIItemModelsByOldQuestionId, List<CloneVirtualQuestion> cloneVQs)
        {
            var lstOldQuestions = parameters.VirtualQuestionServices.GetVirtualQuestionByVirtualTestID(oldTestId);

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

            parameters.VirtualQuestionServices.InsertMultipleRecord(newQuestions);

            oldQuestions.ForEach(oldQuestion =>
            {
                var newQuestion = newQuestions.FirstOrDefault(x => x.QTIItemID == oldQuestion.QTIItemID && x.QuestionOrder == oldQuestion.QuestionOrder);

                cloneQTIItemModelsByOldQuestionId.Add(oldQuestion.VirtualQuestionID, new CloneQTIItemModel()
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
            var virtualQuestionGroups = parameters.VirtualQuestionGroupService.GetVirtualQuestionGroupsByVirtualQuestionIds(oldVirtualQeustionIDs);

            if (virtualQuestionGroups.Any())
            {
                var questionGroupIDs = virtualQuestionGroups.Select(x => x.QuestionGroupID).ToList();
                var questionGroups = parameters.VirtualQuestionGroupService.GetQuestionGroups(oldTestId, questionGroupIDs);

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
                            parameters.VirtualQuestionGroupService.SaveQuestionGroup(questionGroupItem);
                            dcQuestionGroupId.Add(vQuestionGroup.QuestionGroupID, questionGroupItem.QuestionGroupID);
                        }

                        VirtualQuestionGroup itemvirtualQuestionGroup = new VirtualQuestionGroup();
                        itemvirtualQuestionGroup.VirtualQuestionGroupID = 0;
                        itemvirtualQuestionGroup.VirtualQuestionID = newVirtualQuestionID;
                        itemvirtualQuestionGroup.QuestionGroupID = (dcQuestionGroupId[vQuestionGroup.QuestionGroupID] != 0) ? dcQuestionGroupId[vQuestionGroup.QuestionGroupID] : questionGroupItem.QuestionGroupID;
                        itemvirtualQuestionGroup.Order = vQuestionGroup.Order;
                        parameters.VirtualQuestionGroupService.SaveVirtualQuestionGroup(itemvirtualQuestionGroup);
                    }
                }
            }
        }
        private void CloneVirtualSection(int oldTestId, int newTestId, Dictionary<int, CloneQTIItemModel> cloneQTIItemModelsByOldQuestionId, out Dictionary<int, int> sectionsMapping)
        {
            sectionsMapping = new Dictionary<int, int>();
            var sections = parameters.VirtualSectionServices.GetVirtualSectionByVirtualTest(oldTestId);
            if (sections.Count > 0)
            {
                foreach (VirtualSection section in sections)
                {
                    var currentSection = parameters.VirtualSectionServices.GetVirtualSectionById(section.VirtualSectionId);
                    int oldVirtualSectionId = currentSection.VirtualSectionId;
                    currentSection.VirtualTestId = newTestId;
                    currentSection.VirtualSectionId = 0;
                    parameters.VirtualSectionServices.Save(currentSection);
                    sectionsMapping.Add(oldVirtualSectionId, currentSection.VirtualSectionId);
                    CloneVirtualSectionQuestion(oldTestId, oldVirtualSectionId, currentSection, cloneQTIItemModelsByOldQuestionId);
                    var listQuestionGroup = parameters.VirtualQuestionGroupService.GetListQuestionGroupByVirtualTestIdAndSectionId(newTestId, oldVirtualSectionId);
                    parameters.VirtualQuestionGroupService.UpdateSectionIdToQuestionGroup(currentSection.VirtualSectionId, listQuestionGroup);
                }
            }
        }

        private void CloneVirtualSectionQuestion(int oldVirtualTestId, int oldVirtualSectionId, VirtualSection newVirtualSection, Dictionary<int, CloneQTIItemModel> cloneQTIItemModelsByOldQuestionId)
        {
            var sectionQuestions = parameters.VirtualSectionQuestionServices.GetVirtualSectionQuestionBySection(oldVirtualTestId, oldVirtualSectionId);
            if (sectionQuestions.Count > 0)
            {
                foreach (VirtualSectionQuestion sectionQuestion in sectionQuestions)
                {
                    var currentSectionQuestion = parameters.VirtualSectionQuestionServices.GetVirtualSectionQuestionById(sectionQuestion.VirtualSectionQuestionId);
                    int oldVirtualQuestionId = currentSectionQuestion.VirtualQuestionId;
                    int newVirtualQuestionId = cloneQTIItemModelsByOldQuestionId[oldVirtualQuestionId].NewQuestionID;
                    currentSectionQuestion.VirtualSectionQuestionId = 0;
                    currentSectionQuestion.VirtualSectionId = newVirtualSection.VirtualSectionId;
                    currentSectionQuestion.VirtualQuestionId = newVirtualQuestionId;
                    parameters.VirtualSectionQuestionServices.Save(currentSectionQuestion);
                }
            }
        }
        private void CloneRelationShipVirtualTest(int oldTestId, int newTestId)
        {
            var virtualTestFile = parameters.VirtualTestFileServices.GetFirstOrDefaultByVirtualTest(oldTestId);
            if (virtualTestFile != null)
            {
                virtualTestFile.VirtualTestFileId = 0;
                virtualTestFile.VirtualTestId = newTestId;
                parameters.VirtualTestFileServices.Save(virtualTestFile);
            }
        }

        #endregion
        private void GetStudentRetakes(AssignmentRetakeViewModel model)
        {
            var studentRetakes = parameters.QTITestClassAssignmentServices.GetStudentStatusForTestRetake(model.OriginalVirtualTestID, model.GUID, model.TestName);
            if (studentRetakes != null)
            {
                model.StudentRetakes = studentRetakes.GroupBy(x => x.StudentID).Select(x => new StudentRetakeViewModel()
                {
                    StudentID = x.Key,
                    FullName = x.FirstOrDefault().FullName,
                    ClassID = x.FirstOrDefault().ClassID,
                    CurrentVirtualTestID = model.CurrentVirtualTestID,
                    RetakeType = x.FirstOrDefault().RetakeType,
                    VirtualTests = x.Select(o => new StudentRetakeTestInfoViewModel()
                    {
                        VirtualTestName = o.VirtualTestName,
                        VirtualTestDisplayName = o.VirtualTestDisplayName,
                        TestStatus = o.TestStatus,
                        VirtualTestID = o.VirtualTestID,
                    }).OrderBy(s => s.VirtualTestID).ToList()
                }).ToList();
                model.TestRetakeNumber = model.StudentRetakes.FirstOrDefault(x => x.IsValid)?.VirtualTests?.Count ?? 1;
                List<int> virtualtestIds = studentRetakes.Select(o => o.VirtualTestID).Distinct().OrderBy(o => o).ToList();
                model.StudentRetakes.ForEach(item =>
                {
                    List<StudentRetakeTestInfoViewModel> virtualTestsPerStudent = new List<StudentRetakeTestInfoViewModel>();
                    for (int i = 0; i < virtualtestIds.Count; i++)
                    {
                        var testInforViewModel = item.VirtualTests.FirstOrDefault(o => o.VirtualTestID == virtualtestIds[i]);
                        if (testInforViewModel != null)
                        {
                            virtualTestsPerStudent.Add(testInforViewModel);
                        }
                        else
                        {
                            virtualTestsPerStudent.Add(new StudentRetakeTestInfoViewModel());
                        }
                    }
                    item.VirtualTests = virtualTestsPerStudent;
                });
                if (studentRetakes != null && studentRetakes.Count > 0)
                {
                    model.VirtualTestsDisplay = studentRetakes.OrderBy(x => x.VirtualTestID).Select(x => x.VirtualTestDisplayName).Distinct();
                }
            }
        }

        private void MappingLoadAssignmentForRetakeRequest(GetTestClassAssignmentCriteria criteria, LoadAssignmentRetakeRequest request)
        {
            request.PageSize = criteria.iDisplayLength;
            request.StartRow = criteria.iDisplayStart;
            request.GeneralSearch = criteria.sSearch;
            request.RetakeRequestGuid = criteria.RetakeAssignmentRequestGuid;

            if (!string.IsNullOrWhiteSpace(criteria.sColumns) && criteria.iSortCol_0.HasValue)
            {
                var columns = criteria.sColumns.Split(',');
                request.SortColumn = columns[criteria.iSortCol_0.Value];
                request.SortDirection = criteria.sSortDir_0.Equals("desc") ? "DESC" : "ASC";
            }
        }

        private string GetUpdatedDateStr(DateTime? updatedDate)
        {
            var timeZoneId = _stateService.GetTimeZoneId(CurrentUser.StateId.GetValueOrDefault());
            return updatedDate.HasValue
                    ? updatedDate.Value.ConvertTimeFromUtc(timeZoneId).DisplayDateWithFormat(true).ToString()
                    : updatedDate.GetValueOrDefault().ToString();
        }

        public ActionResult GetSelectTestFilterConfig(int districtID)
        {
            var config = parameters.DistrictDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtID, Util.AssignOnlineTest_DefaultTestFilterMode);
            return Json(new { Mode = config.Value }, JsonRequestBehavior.AllowGet);
        }
    }
}
