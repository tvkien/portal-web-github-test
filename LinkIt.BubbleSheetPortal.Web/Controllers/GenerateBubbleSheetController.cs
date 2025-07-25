using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using FluentValidation.Results;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System.Linq;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using RequestSheet = LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator.RequestSheet;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator;
using System.Web;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting;
using System;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.ViewModels.Validators;
using LinkIt.BubbleSheetPortal.Services.CommonServices;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class GenerateBubbleSheetController : BubbleSheetBaseController
    {
        private readonly UserService userService;
        private readonly BubbleSheetDataService bubbleSheetDataService;
        private readonly ClassService classService;
        private readonly DistrictService districtService;
        private readonly IncorrectQuestionOrderService incorrectQuestionOrderService;
        private readonly IValidator<BubbleSheetData> bubbleSheetDataValidator;
        private readonly BubbleSheetPrintingService bubbleSheetPrintingService;
        private readonly ClassUserService classUserService;
        private readonly VirtualTestService virtualTestService;
        private readonly GroupPrintingControllerParameters parameters;


        private readonly DistrictDecodeService _districtDecodeService;
        private readonly VirtualQuestionService _virtualQuestionService;
        private readonly QuestionOptionsService questionOptionsService;
        private readonly VirtualSectionService _virtualSectionService;
        private readonly VulnerabilityService _vulnerabilityService;

        private readonly PreferencesService _preferencesService;

        public GenerateBubbleSheetController(UserService userService,
            BubbleSheetDataService bubbleSheetDataService,
            ClassService classService,
            DistrictService districtService,
            IncorrectQuestionOrderService incorrectQuestionOrderService,
            IValidator<BubbleSheetData> bubbleSheetDataValidator,
            BubbleSheetPrintingService bubbleSheetPrintingService,
            ClassUserService classUserService,
            BubbleSheetService bubbleSheetService,
            IValidator<BubbleSheet> bubbleSheetValidator,
            VirtualTestService virtualTestService,
            GroupPrintingControllerParameters parameters,
            DistrictDecodeService districtDecodeService,
            VirtualQuestionService virtualQuestionService,
            QuestionOptionsService questionOptionsService,
            VirtualSectionService virtualSectionService,
            VulnerabilityService vulnerabilityService,
            BubbleSheetCommonService bubbleSheetCommonService,
            PreferencesService preferencesService)
            : base(bubbleSheetService, bubbleSheetValidator, classService, virtualTestService, bubbleSheetCommonService, userService)
        {
            this.userService = userService;
            this.bubbleSheetDataService = bubbleSheetDataService;
            this.classService = classService;
            this.districtService = districtService;
            this.incorrectQuestionOrderService = incorrectQuestionOrderService;
            this.bubbleSheetDataValidator = bubbleSheetDataValidator;
            this.bubbleSheetPrintingService = bubbleSheetPrintingService;
            this.classUserService = classUserService;
            this.virtualTestService = virtualTestService;
            this.parameters = parameters;
            _districtDecodeService = districtDecodeService;
            _virtualQuestionService = virtualQuestionService;
            this.questionOptionsService = questionOptionsService;
            _virtualSectionService = virtualSectionService;
            _vulnerabilityService = vulnerabilityService;
            _preferencesService = preferencesService;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManagebubblesheetsCreate)]
        public ActionResult Generate()
        {
            var model = new GenerateBubbleSheetViewModel
            {
                IsAdmin = IsUserAdmin(),
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher,
                IsSchoolAdmin = CurrentUser.RoleId.Equals(8),
                IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher),
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin,
                ListDistricIds = CurrentUser.IsNetworkAdmin ? CurrentUser.GetMemberListDistrictId() : null
            };

            var lstDistrictDecode = parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId.GetValueOrDefault(), Util.DistrictDecode_TestScoreExtract);
            var gradebookSISValue = parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId.GetValueOrDefault(), Util.DistrictDecode_SendTestResultToGenesis).Select(c => c.Value).FirstOrDefault();



            model.TestExtractOptions = new TestExtractOptions
            {
                IsUseTestExtract = lstDistrictDecode.Any() || (gradebookSISValue != null && gradebookSISValue != "0")
            };

            if (gradebookSISValue != null)
            {
                var gradebookSISIds = gradebookSISValue.ToIntArray("|").Where(c => c > 0).ToArray();
                if (gradebookSISIds.Any())
                {

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
            //Binding Value (GradeBook & StudentRecord)
            if (!CurrentUser.IsPublisher() && !CurrentUser.IsNetworkAdmin())
            {
                var objPreference = GetTestExtractPreferenceForBubbleSheet(0, CurrentUser.DistrictId.GetValueOrDefault(), CurrentUser.Id);
                if (objPreference != null)
                {
                    var testextractGradeBookValue = GetValuePreferenceItem(objPreference, ContaintUtil.TestPreferenceTestExtractGradeBook, false);
                    var testextractStudentRecordValue = GetValuePreferenceItem(objPreference, ContaintUtil.TestPreferenceTestExtractStudentRecord, false);

                    model.TestExtractOptions.GradebookChecked = testextractGradeBookValue == "1";
                    model.TestExtractOptions.StudentRecordChecked = testextractStudentRecordValue == "1";
                }
            }

            return View(model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult Generate(BubbleSheetData model)
        {
            SetModelPermissions(model);
            if (model.UserId.Equals(0))
            {
                model.UserId = CurrentUser.Id;
            }
            if (model.DistrictId.Equals(0))
            {
                model.DistrictId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            model.SetValidator(bubbleSheetDataValidator);
            if (!model.IsValid)
            {
                return Json(new { ErrorList = model.ValidationErrors, Success = false });
            }

            if (model.IsIncludeExtraPages && (model.NumberOfGraphExtraPages + model.NumberOfLinedExtraPages + model.NumberOfPlainExtraPages + model.NumberOfPrimaryExtraPages) == 0)
            {
                return
                    Json(
                        new
                        {
                            ErrorList =
                                new List<ValidationFailure>
                                {
                                    new ValidationFailure("error",
                                        "Please select at least one page for either Lined, Plain, or Graph paper.")
                                },
                            Success = false
                        });
            }

            if (!AssignPrimaryTeacherSuccessfull(model))
            {
                return
                    Json(
                        new
                        {
                            ErrorList =
                                new List<ValidationFailure>
                                {
                                    new ValidationFailure("error",
                                        "A primary teacher for this class could not be found.")
                                },
                            Success = false
                        });
            }

            if (!IsValidBubbleSheetDataModel(model, CurrentUser))
            {
                return
                    Json(
                        new
                        {
                            ErrorList =
                                new List<ValidationFailure>
                                {
                                    new ValidationFailure("error",
                                        "No bindable bubble sheet data exists for given data set.")
                                },
                            Success = false
                        });
            }

            if (!IsValidStudentListDataModel(model, CurrentUser))
            {
                return
                    Json(
                        new
                        {
                            ErrorList =
                                new List<ValidationFailure>
                                {
                                    new ValidationFailure("error",
                                        "There are students that you do not have permission to access.")
                                },
                            Success = false
                        });
            }

            var unsupportedQuestions = GetUnsupportedQuestionList(model.TestId);
            if (unsupportedQuestions.Any())
            {
                return
                    Json(
                        new
                        {
                            UnsupportedList = unsupportedQuestions,
                            Success = false,
                            ExistUnsupportedQuestionList = true,
                            ErrorList = ""
                        });
            }

            var test = bubbleSheetPrintingService.InitializeRequestSheet(model.StudentIdList.Count, ConfigurationManager.AppSettings["ApiKey"]);

            test.NumberOfGraphExtraPages = model.NumberOfGraphExtraPages;
            test.NumberOfLinedExtraPages = model.NumberOfLinedExtraPages;
            test.NumberOfPlainExtraPages = model.NumberOfPlainExtraPages;
            test.NumberOfPrimaryExtraPages = model.NumberOfPrimaryExtraPages;
            test.IsGridStype = model.IsGridStype;
            test.IsExtraPagesOnly = model.IsPrintExtraPageOnly;

            bubbleSheetPrintingService.AssignStudentsToTest(model, test, LinkitConfigurationManager.GetS3Settings().S3CSSKey);
            var preference = JsonConvert.DeserializeObject<BubbleSheetPreference>(model.BubbleSheetPreference);
            test.Preference = preference;

            if (model.IsManualEntry == false)
            {
                ValidateCropMarkResponse validateBubblesheetOutsideCropmark = BubbleSheetWsHelper.ValidateBubbleOutsideCropMark(test);
                if (validateBubblesheetOutsideCropmark == null)
                {
                    return
                        Json(
                            new
                            {
                                ErrorList = "Server error. Please try again",
                                Success = false,
                                IsBubbleSheetOutsideCropMark = false,
                                SnapshotUrl = string.Empty
                            }, JsonRequestBehavior.AllowGet);
                }
                if (validateBubblesheetOutsideCropmark.IsValid == false)
                {
                    return
                        Json(
                            new
                            {
                                ErrorList = validateBubblesheetOutsideCropmark.InvalidErrorMessage,
                                Success = false,
                                IsBubbleSheetOutsideCropMark = true,
                                SnapshotUrl = validateBubblesheetOutsideCropmark.SnapshotUrl
                            },
                            JsonRequestBehavior.AllowGet);
                }
            }

            var bubbleSheets = bubbleSheetPrintingService.CreateBubbleSheets(model, test);

            try
            {
                // create preferences
                if (bubbleSheets != null && bubbleSheets.Count() > 0 && preference != null)
                {
                    model.BubbleSheetPreference = preference.ToXML();
                    var bbsIDs = bubbleSheets.Select(m => m.Id).ToList();
                    foreach (var item in bbsIDs)
                    {
                        _preferencesService.Save(new Preferences
                        {
                            Level = Util.PreBBSLevel,
                            Label = Util.PreBBSLable,
                            Id = item,
                            Value = model.BubbleSheetPreference
                        });
                    }
                }
            }
            catch { }

            return CreateResponseTicket(test, bubbleSheets, model.IsManualEntry, LinkitConfigurationManager.Vault.DatabaseID, model.TestName);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CheckAssignSameTest(BubbleSheetData model)
        {
            model.SetValidator(new BubbleSheetDataValidatorForCheckTheSameTest());
            if (!model.IsValid)
            {
                return Json(new { Success = false, Errors = model.ValidationErrors});
            }

            var virtualTest = parameters.VirtualTestService.GetTestById(model.TestId);
            if (virtualTest.IsMultipleTestResult.GetValueOrDefault())
                return Json(new { IsMultipleTestResult = true }, JsonRequestBehavior.AllowGet);

            var result = parameters.QTITestClassAssignmentServices.GetStudentAssginment(model.TestId, model.StudentIdList.Select(x => int.Parse(x)).ToList(), model.ClassId.ToString());

            var studentIDs = model.StudentIdList != null ? model.StudentIdList.Select(x => int.Parse(x)).ToList() : new List<int>();
            var studentOnlineTests = result.OnlineTest.Where(x => studentIDs.Contains(x.StudentId)).Select(x => x.StudentId).Distinct().Count();
            var studentBBS = result.BubbleSheet.Where(x => studentIDs.Contains(x.StudentId)).Select(x => x.StudentId).Distinct().Count();

            return Json(new { StudentOnlineTest = studentOnlineTests, StudentBBS = studentBBS, StudentList = result.BubbleSheet });
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManagebubblesheetsCreate)]
        public ActionResult TempAssignSameTest(BubbleSheetAssignSameTestParam model)
        {
            return View("TempStudentAssignSameTest", model);
        }

        [HttpPost]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManagebubblesheetsCreate)]
        public ActionResult StudentsAssignSameTest(BubbleSheetAssignSameTestParam model)
        {
            if (model.IsGroupPrinting)
                return View("StudentAssignSameTestGroupPrinting", model);

            return View("StudentAssignSameTest", model);
        }

        [HttpGet]
        public ActionResult GetStudentAssignSameTestGroupPrinting(BubbleSheetAssignSameTestParam model)
        {
            var groupStudents = parameters.GroupPrintingService.GetGroupStudents(model.GroupId).ToList();
            var result = new StudentAssginmentGroupDto();
            var parser = new DataTableParserProc<AssignSameTestGroupPrintingViewModel>();

            var studentAssignments = parameters.QTITestClassAssignmentServices.GetStudentAssginmentGrouping(model.TestId, groupStudents.Select(x => x.StudentId).Distinct().ToList(), model.GroupId, model.ClassId.ToString(), model.SSearch, parser.SortableColumns, parser.PageSize);

            result.BubbleSheet.AddRange(studentAssignments.BubbleSheet.Distinct());
            var results = result.BubbleSheet.Select(o => new AssignSameTestGroupPrintingViewModel
            {
                StudentId = o.StudentId,
                FullName = o.FullName,
                AssignmentDate = o.AssignmentDate,
                ResultDate = o.ResultDate
            }).AsQueryable();
            int totalRecords = results.Count();
            results = results.Skip(parser.StartIndex).Take(Math.Min(parser.PageSize, totalRecords - parser.StartIndex));

            return Json(parser.Parse(results, totalRecords), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStudentAssignSameTest(BubbleSheetAssignSameTestParam model)
        {
            var studentIDs = model.StudentIdList != null ? model.StudentIdList.SelectMany(x => x.Split(',')).Select(o =>
            {
                if (int.TryParse(o, out int _value))
                    return _value;
                return default(int?);
            }).Where(o => o.HasValue)
              .Select(o => o.Value)
              .ToList() : new List<int>();

            var parser = new DataTableParserProc<AssignSameTestGroupPrintingViewModel>();
            var result = parameters.QTITestClassAssignmentServices.GetStudentAssginment(model.TestId, studentIDs, model.ClassId.ToString(), model.SSearch, parser.SortableColumns, parser.PageSize);
            var results = result.BubbleSheet.Select(o => new AssignSameTestGroupPrintingViewModel
            {
                StudentId = o.StudentId,
                FullName = o.FullName,
                AssignmentDate = o.AssignmentDate,
                ResultDate = o.ResultDate
            }).AsQueryable();
            int totalRecords = results.Count();
            results = results.Skip(parser.StartIndex).Take(Math.Min(parser.PageSize, totalRecords - parser.StartIndex));
            return Json(parser.Parse(results, totalRecords), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManagebubblesheetsCreate)]
        public ActionResult CheckTestType(int testId)
        {
            var virtualTest = virtualTestService.Select().SingleOrDefault(en => en.VirtualTestID == testId);

            if (virtualTest != null && (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.ACT || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT))
            {
                return Json("ACT", JsonRequestBehavior.AllowGet);
            }

            if (virtualTest != null && (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.SAT))
            {
                return Json("SAT", JsonRequestBehavior.AllowGet);
            }

            if (virtualTest != null && virtualTest.VirtualTestSubTypeID == 7)
            {
                return Json("NewSAT", JsonRequestBehavior.AllowGet);
            }

            return Json("BubbleSheet", JsonRequestBehavior.AllowGet);
        }

        private bool AssignPrimaryTeacherSuccessfull(BubbleSheetData model)
        {
            var classUser = classUserService.GetPrimaryTeacherByClassId(model.ClassId);
            if (classUser.IsNull())
            {
                return false;
            }

            var primaryTeacher = userService.GetUserById(classUser.UserId);
            if (primaryTeacher.IsNull())
            {
                return false;
            }

            model.UserId = primaryTeacher.Id;
            model.TeacherName = primaryTeacher.LastName;

            return true;
        }

        private void SetModelPermissions(BubbleSheetData model)
        {
            model.IsPublisher = CurrentUser.IsPublisher();
            model.IsSchoolAdmin = userService.GetSchoolAdminByUserId(CurrentUser.Id).IsNotNull();
            model.IsAdmin = IsUserAdmin();
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
        }

        public bool IsValidBubbleSheetDataModel(BubbleSheetData model, User currentUser)
        {
            SetupBubbleSheetDataModel(model, currentUser);
            if (!model.IsAdmin)
            {
                var selectedClass = classService.GetClassById(model.ClassId);
                if (selectedClass.IsNotNull())
                {
                    model.SchoolId = selectedClass.SchoolId.GetValueOrDefault();
                    //model.UserId = selectedClass.UserId.GetValueOrDefault(); //TODO: LNKT-9139 require use teacher select from dropdown
                }
            }
            var bubbleSheetData = bubbleSheetDataService.GetBubbleSheetDataObject(model);
            if (bubbleSheetData.IsNotNull())
            {
                BindBubbleSheetDataModelProperties(model, bubbleSheetData);
                return true;
            }
            return false;
        }

        public bool IsValidStudentListDataModel(BubbleSheetData model, User currentUser)
        {
            var studentIdList = string.Join(",", model.StudentIdList);
            return _vulnerabilityService.CheckUserPermissionOnStudent(currentUser, studentIdList);
        }

        private void SetupBubbleSheetDataModel(BubbleSheetData model, User currentUser)
        {
            if (currentUser.RoleId != (int)Permissions.Publisher &&
                currentUser.RoleId != (int)Permissions.NetworkAdmin)
            {
                model.DistrictId = currentUser.DistrictId.GetValueOrDefault();
            }
            var district = districtService.GetDistrictById(model.DistrictId);
            if (district.IsNotNull())
            {
                model.StateId = district.StateId;
            }
            model.CreatedByUserId = currentUser.Id;
        }

        private void BindBubbleSheetDataModelProperties(BubbleSheetData model, BubbleSheetData bubbleSheetData)
        {
            model.UserId = bubbleSheetData.UserId;
            model.DistrictId = bubbleSheetData.DistrictId;
            model.TeacherName = bubbleSheetData.TeacherName;
            model.ClassName = bubbleSheetData.ClassName;
            model.SchoolName = bubbleSheetData.SchoolName;
            model.SubjectName = bubbleSheetData.SubjectName;
            model.TestName = bubbleSheetData.TestName;
            model.DistrictName = bubbleSheetData.DistrictName;
            model.DistrictTermId = bubbleSheetData.DistrictTermId;
        }

        private bool IsUserAdmin()
        {
            return userService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull();
        }

        //[HttpGet]
        //public ActionResult CheckIfTestRequiresCorrection(int testId)
        //{
        //    var requiresCorrection = incorrectQuestionOrderService.CheckIfTestRequiresCorrection(testId);
        //    return Json(new { Success = requiresCorrection }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult CheckDisplayTestExtract(int districtId, int? testId)
        {
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            if (districtId > 0)
            {
                var vDistrictDecode = _districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.DistrictDecode_TestScoreExtract);
                var isSendTestresultToGenesis = _districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.DistrictDecode_SendTestResultToGenesis).FirstOrDefault();
                bool isGradebookChecked = false;
                bool isStudentRecordChecked = false;
                bool isTestExtractExportRawScoreChecked = false;
                if (testId.HasValue && testId.Value > 0)
                {
                    var objPreference = GetTestExtractPreferenceForBubbleSheet(testId.Value, districtId, CurrentUser.Id);
                    if (objPreference != null)
                    {
                        var testextractGradeBookValue = GetValuePreferenceItem(objPreference, ContaintUtil.TestPreferenceTestExtractGradeBook, false);
                        var testextractStudentRecordValue = GetValuePreferenceItem(objPreference, ContaintUtil.TestPreferenceTestExtractStudentRecord, false);
                        var testExtractExportRawScore = GetValuePreferenceItem(objPreference, ContaintUtil.TestPreferencetTestExtractExportRawScore, false);

                        isGradebookChecked = testextractGradeBookValue == "1";
                        isStudentRecordChecked = testextractStudentRecordValue == "1";
                        isTestExtractExportRawScoreChecked = testExtractExportRawScore == "1";
                    }
                }
                if (vDistrictDecode.Any() || (isSendTestresultToGenesis != null && isSendTestresultToGenesis.Value != "0"))
                    return Json(new
                    {
                        Success = true,
                        IsDisplay = true,
                        GradebookChecked = isGradebookChecked,
                        StudentRecordChecked = isStudentRecordChecked,
                        TestExtractExportRawScoreChecked = isTestExtractExportRawScoreChecked,
                    }, JsonRequestBehavior.AllowGet);
            }
            //Default set false, only binding value when select a Test (VirtualTest)
            return Json(new { Success = true, IsDisplay = false, GradebookChecked = false, StudentRecordChecked = false }, JsonRequestBehavior.AllowGet);
        }

        public bool IsUseTestExtract(int districtId)
        {
            if (districtId > 0)
            {
                var v = _districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.DistrictDecode_TestScoreExtract);
                var isSendTestresultToGenesis = _districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, Util.DistrictDecode_SendTestResultToGenesis).FirstOrDefault();
                if (v.Any() || (isSendTestresultToGenesis != null && isSendTestresultToGenesis.Value != "0"))
                    return true;
            }
            return false;
        }

        [HttpGet]
        public ActionResult GetVirtualQuestions(int virtualTestId)
        {
            var data = _virtualQuestionService.Select().Where(x => x.VirtualTestID == virtualTestId)
                .Select(x => new ListItem { Id = x.QuestionOrder, Name = "Question " + x.QuestionOrder })
                .OrderBy(x => x.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetVirtualSections(int virtualTestId)
        {
            var data = _virtualSectionService.Select().Where(x => x.VirtualTestId == virtualTestId)
                .Select(x => new ListItem { Id = x.Order, Name = "Section " + x.Order }).OrderBy(x => x.Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<int> GetUnsupportedQuestionListSAT(int testId)
        {
            var questions =
                questionOptionsService.GetQuestionOptionsByTestId(testId).OrderBy(x => x.ProblemNumber).ToList();
            var unsupportedQuestions = new List<int>();
            var questionNumber = 1;
            foreach (var question in questions)
            {
                if (question.QtiSchemaId != (int)QtiSchemaEnum.MultipleChoice
                    && question.QtiSchemaId != (int)QtiSchemaEnum.MultiSelect
                    && question.QtiSchemaId != (int)QtiSchemaEnum.TextEntry
                    && question.QtiSchemaId != (int)QtiSchemaEnum.ExtendedText)
                {
                    unsupportedQuestions.Add(questionNumber);
                }

                questionNumber++;
            }

            return unsupportedQuestions;
        }

        private List<int> GetUnsupportedQuestionListBbs(int testId)
        {
            var questions =
                questionOptionsService.GetQuestionOptionsByTestId(testId).OrderBy(x => x.ProblemNumber).ToList();
            var unsupportedQuestions = new List<int>();
            var questionNumber = 1;
            foreach (var question in questions)
            {
                if (question.QtiSchemaId != (int)QtiSchemaEnum.MultipleChoice
                    && question.QtiSchemaId != (int)QtiSchemaEnum.MultiSelect
                    && question.QtiSchemaId != (int)QtiSchemaEnum.ExtendedText
                    && question.QtiSchemaId != (int)QtiSchemaEnum.ChoiceMultipleVariable)
                {
                    unsupportedQuestions.Add(questionNumber);
                }
                questionNumber++;
            }
            return unsupportedQuestions;
        }

        private List<int> GetUnsupportedQuestionList(int testId)
        {
            var virtualTest = virtualTestService.Select().SingleOrDefault(en => en.VirtualTestID == testId);
            if (virtualTest != null &&
                (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.SAT ||
                 virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewSAT))
            {
                return GetUnsupportedQuestionListSAT(testId);
            }

            return GetUnsupportedQuestionListBbs(testId);
        }

        private TestSettingsMap GetTestExtractPreferenceForBubbleSheet(int testId, int currentDistrictId, int currentUserId)
        {
            //Make sure this District IsUseTestExtract
            var bubbleSheePreference = new TestSettingsMap();
            if (testId > 0)
            {
                var objTest = virtualTestService.GetTestById(testId);
                if (objTest == null)
                    return null;

                //Test Preference            
                var preferenceTest = _preferencesService.GetPreferenceByLevelAndID(testId, ContaintUtil.TestPreferenceLevelTest);
                if (preferenceTest != null
                    && !string.IsNullOrEmpty(preferenceTest.Value)
                    && (preferenceTest.Value.Contains(ContaintUtil.TestPreferenceTestExtractGradeBook)
                        ||
                        preferenceTest.Value.Contains(ContaintUtil.TestPreferenceTestExtractStudentRecord))
                    )
                {
                    bubbleSheePreference.TestPreferenceModel = _preferencesService.ConvertToTestPreferenceModel(preferenceTest.Value);
                    return bubbleSheePreference;
                }
            }

            //User Preference
            var preferenceUser = _preferencesService.GetPreferenceByLevelAndId(currentUserId, currentDistrictId, ContaintUtil.TestPreferenceLevelUser);
            if (preferenceUser != null
                && !string.IsNullOrEmpty(preferenceUser.Value)
                && (preferenceUser.Value.Contains(ContaintUtil.TestPreferenceTestExtractGradeBook)
                    ||
                    preferenceUser.Value.Contains(ContaintUtil.TestPreferenceTestExtractStudentRecord))
                )
            {
                bubbleSheePreference.TestPreferenceModel = _preferencesService.ConvertToTestPreferenceModel(preferenceUser.Value);
                return bubbleSheePreference;
            }
            //District Preference
            var preferenceDistrict = _preferencesService.GetPreferenceByLevelAndId(currentUserId, currentDistrictId, ContaintUtil.TestPreferenceLevelDistrict);
            if (preferenceDistrict != null
                && !string.IsNullOrEmpty(preferenceDistrict.Value)
                && (preferenceDistrict.Value.Contains(ContaintUtil.TestPreferenceTestExtractGradeBook)
                    ||
                    preferenceDistrict.Value.Contains(ContaintUtil.TestPreferenceTestExtractStudentRecord))
                )
            {
                bubbleSheePreference.TestPreferenceModel = _preferencesService.ConvertToTestPreferenceModel(preferenceDistrict.Value);
                return bubbleSheePreference;
            }
            //Enterprise Preference
            var preferenceEnterprise = _preferencesService.GetPreferenceByLevelAndId(currentUserId, currentDistrictId, ContaintUtil.TestPreferenceLevelEnterprise);
            if (preferenceEnterprise != null
                && !string.IsNullOrEmpty(preferenceEnterprise.Value)
                && (preferenceEnterprise.Value.Contains(ContaintUtil.TestPreferenceTestExtractGradeBook)
                    ||
                    preferenceEnterprise.Value.Contains(ContaintUtil.TestPreferenceTestExtractStudentRecord))
                )
            {
                bubbleSheePreference.TestPreferenceModel = _preferencesService.ConvertToTestPreferenceModel(preferenceEnterprise.Value);
                return bubbleSheePreference;
            }
            return bubbleSheePreference;
        }

        private string GetValuePreferenceItem(TestSettingsMap objPreference, string ItemKey, bool isToolItem)
        {
            BubbleSheetPortal.Models.ManagePreference.Tag objPreferenceItem;
            if (isToolItem) //ToolTags
            {
                objPreferenceItem = objPreference.TestPreferenceModel.ToolTags.FirstOrDefault(o => o.Key.Equals(ItemKey));
            }
            else //OptionTags
            {
                objPreferenceItem = objPreference.TestPreferenceModel.OptionTags.FirstOrDefault(o => o.Key.Equals(ItemKey));
            }
            return objPreferenceItem == null ? string.Empty : objPreferenceItem.Value;
        }

        [HttpGet]
        public ActionResult NormalTestHaveOpended(int virtualtestId)
        {
            if (virtualtestId > 0)
            {
                var objTest = virtualTestService.GetTestById(virtualtestId);
                //Normal test: not legacy(VirtualTestSourceID == 3)
                if (objTest != null && objTest.VirtualTestSourceID != 3 && objTest.VirtualTestSubTypeID == 1)
                {
                    //_virtualQuestionService
                    int totalOpendedQuestions = VirtualTestService.CountOpendedQuestionPerTest(virtualtestId);
                    if (totalOpendedQuestions > 0)
                    {
                        return Json(new { Success = true, HaveOpended = true }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { Success = true, HaveOpended = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DefaultSettings(int districtId, int testId)
        {
            if (districtId == 0)
            {
                districtId = CurrentUser.DistrictId.GetValueOrDefault();
            }
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { error = "Has no right to district." }, JsonRequestBehavior.AllowGet);
            }
            if (!_vulnerabilityService.HasRightToAccessVirtualTest(CurrentUser, testId, CurrentUser.GetMemberListDistrictId()))
            {
                return Json(new { error = "Has no right to the test." }, JsonRequestBehavior.AllowGet);
            }

            var objPreference = GetTestExtractPreferenceForBubbleSheet(testId, districtId, CurrentUser.Id);
            string questionGroupLabelSchema = "0";
            if (objPreference != null)
            {
                questionGroupLabelSchema = GetValuePreferenceItem(objPreference, ContaintUtil.TestPreferencetQuestiongrouplabelschema, false);
            }

            var virtualTest = virtualTestService.Select().Where(x => x.VirtualTestID == testId)
                .Select(x => new { x.VirtualTestID, x.IsNumberQuestions })
                .FirstOrDefault();

            return Json(new
            {
                QuestionGroupLabelSchema = questionGroupLabelSchema,
                IsNumberQuestions = virtualTest?.IsNumberQuestions ?? false,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
