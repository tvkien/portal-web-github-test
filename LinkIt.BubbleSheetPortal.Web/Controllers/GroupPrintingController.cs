using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using DevExpress.Office.Utils;
using DevExpress.XtraReports.UI;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using FluentValidation.Results;
using LinkIt.BubbleService.Models.Test;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.GroupPrinting;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using RequestSheet = LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator.RequestSheet;
using DocumentDetails = LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator.DocumentDetails;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting;
using LinkIt.BubbleSheetPortal.Web.ViewModels.Validators;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Services.CommonServices;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class GroupPrintingController : BubbleSheetBaseController
    {
        private readonly GroupPrintingControllerParameters parameters;
        private const string SessionStatusFormat = "GroupPrinting_{0}_Status";
        private static Dictionary<string, Task<JsonResult>> _listTasks = new Dictionary<string, Task<JsonResult>>();
        private readonly DistrictDecodeService districtDecodeService;
        private readonly PreferencesService _preferencesService;

        public GroupPrintingController(GroupPrintingControllerParameters parameters,
            BubbleSheetService bubbleSheetService,
            IValidator<BubbleSheet> bubbleSheetValidator
            , ClassService classService
            , VirtualTestService virtualTestService
            , UserService userService
            , DistrictDecodeService districtDecodeService
            , BubbleSheetCommonService bubbleSheetCommonService
            , PreferencesService preferencesService)
            : base(bubbleSheetService, bubbleSheetValidator, classService, virtualTestService, bubbleSheetCommonService, userService)
        {
            this.parameters = parameters;
            this.districtDecodeService = districtDecodeService;
            _preferencesService = preferencesService;
        }

        private string DuplicatedGroupKey
        {
            get { return string.Format("{0}_GroupDuplicate", CurrentUser.Id); }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PrintGroup(BubbleSheetGroupData model)
        {
            model.SetValidator(parameters.BubbleSheetGroupDataValidator);
            if (!model.IsValid)
            {
                return Json(new { ErrorList = model.ValidationErrors, Success = false });
            }
            if (model.IsIncludeExtraPages && (model.NumberOfGraphExtraPages + model.NumberOfLinedExtraPages + model.NumberOfPlainExtraPages) == 0)
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



            var randomId = Guid.NewGuid().ToString();

            //Session[string.Format(SessionDataFormat, randomId)] = model;
            Session[string.Format(SessionStatusFormat, randomId)] = false;
            int currentUserId = CurrentUser.Id;

            SavePrintBubbleSheetDownloadSession(model);

            string environmentId = LinkitConfigurationManager.Vault.DatabaseID;
            string s3CSSKey = LinkitConfigurationManager.GetS3Settings().S3CSSKey;
            var task = Task.Factory.StartNew(() => BuildGroupData(model, currentUserId, environmentId, s3CSSKey));
            _listTasks.Add(randomId, task);

            return Json(new { Success = true, Id = randomId });
            //return BuildGroupData(model);
        }

        private List<int> GetUnsupportedQuestionList(int testId)
        {
            var virtualTest = parameters.VirtualTestService.Select().SingleOrDefault(en => en.VirtualTestID == testId);
            if (virtualTest != null &&
                (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.SAT ||
                 virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewSAT))
            {
                return GetUnsupportedQuestionListSAT(testId);
            }

            return GetUnsupportedQuestionListBbs(testId);
        }

        private List<int> GetUnsupportedQuestionListSAT(int testId)
        {
            var questions = parameters.QuestionOptionsService.GetQuestionOptionsByTestId(testId).OrderBy(x => x.ProblemNumber).ToList();
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
            var questions = parameters.QuestionOptionsService.GetQuestionOptionsByTestId(testId).OrderBy(x => x.ProblemNumber).ToList();
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

        private void SavePrintBubbleSheetDownloadSession(BubbleSheetGroupData model)
        {
            var printBubbleSheetDownloadModels = new List<PrintBubbleSheetDownloadModel>();
            if (Session["PrintBubbleSheetDownload"] != null)
            {
                printBubbleSheetDownloadModels =
                    (List<PrintBubbleSheetDownloadModel>)Session["PrintBubbleSheetDownload"];
                printBubbleSheetDownloadModels =
                    printBubbleSheetDownloadModels.Where(x => !string.IsNullOrEmpty(x.DownloadUrl)).ToList(); // Remove invalid previous requests

            }

            if (printBubbleSheetDownloadModels != null)
            {
                var printModel = new PrintBubbleSheetDownloadModel
                {
                    GeneratedDateTime = DateTime.UtcNow
                };

                if (model.TestId > 0)
                {
                    var virtualTest = VirtualTestService.Select().FirstOrDefault(x => x.VirtualTestID == model.TestId);
                    if (virtualTest != null)
                    {
                        printModel.TestName = virtualTest.Name;
                    }
                }

                if (model.GroupId > 0)
                {
                    var printingGroup = parameters.GroupPrintingService.GetById(model.GroupId);
                    if (printingGroup != null)
                        printModel.GroupName = printingGroup.Name;
                }

                printBubbleSheetDownloadModels.Add(printModel);
                Session["PrintBubbleSheetDownload"] = printBubbleSheetDownloadModels;
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CheckAssignSameTest(BubbleSheetGroupData model)
        {
            model.SetValidator(new BubbleSheetGroupDataValidatorForCheckTheSameTest());
            if (!model.IsValid)
            {
                return Json(new { Success = false, Errors = model.ValidationErrors });
            }
            var virtualTest = VirtualTestService.GetVirtualTestById(model.TestId);
            if (virtualTest.IsMultipleTestResult.GetValueOrDefault())
                return Json(new { IsMultipleTestResult = true }, JsonRequestBehavior.AllowGet);

            var groupStudents = parameters.GroupPrintingService.GetGroupStudents(model.GroupId).ToList();
            var result = new StudentAssginmentGroupDto();
            var studentAssignments = parameters.QTITestClassAssignmentServices.GetStudentAssginmentGrouping(model.TestId, groupStudents.Select(x => x.StudentId).Distinct().ToList(), model.GroupId);
            result.BubbleSheet.AddRange(studentAssignments.BubbleSheet.Distinct());
            result.OnlineTest.AddRange(studentAssignments.OnlineTest.Distinct());

            return Json(new { StudentOnlineTest = result.OnlineTest.Select(x => x.StudentId).Distinct().Count(), StudentBBS = result.BubbleSheet.Select(x => x.StudentId).Distinct().Count(), StudentList = result.BubbleSheet, TestName = virtualTest.Name }); ;
        }

        [HttpGet]
        public ActionResult CheckPrintGroupStatus(string id)
        {
            Task<JsonResult> task;
            if (_listTasks.TryGetValue(id, out task))
            {
                try
                {
                    if (task.IsCompleted)
                    {
                        _listTasks.Remove(id);
                        var returnValue = task.Result;
                        return returnValue;
                    }
                }
                catch (AggregateException ae)
                {
                    string errorMessage = string.Empty;
                    ae.Handle((x) =>
                              {
                                  errorMessage = x.Message;
                                  return true;
                              });
                    var error = new List<ValidationFailure>
                                {
                                    new ValidationFailure("GenerateFail", errorMessage)
                                };
                    return Json(new { ErrorList = error, Success = false, IsBubbleSheetOutsideCropMark = false }
                        , JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Path = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        private JsonResult BuildGroupData(BubbleSheetGroupData model, int currentUserId, string environmentId, string s3CSSKey)
        {
            var currentUser = parameters.UserService.GetUserById(currentUserId);
            var districtID = model.DistrictId == 0 ? currentUser.DistrictId.GetValueOrDefault() : model.DistrictId;
            var dateTimeFormat = districtDecodeService.GetDateFormat(districtID);
            string nowDateString = DateTime.UtcNow.AddMinutes(model.TimezoneOffset * (-1)).DisplayDateWithFormat(dateTimeFormat.DateFormat, dateTimeFormat.TimeFormat, true);
            var preference = JsonConvert.DeserializeObject<BubbleSheetPreference>(model.BubbleSheetPreference);

            if (model.IsGenericBubbleSheet)
            {
                var groupStudents = parameters.GroupPrintingService.GetGroupStudents(model.GroupId).OrderBy(x => x.ClassId).ThenBy(x => x.StudentName).ToList();
                if (groupStudents.Count == 0)
                {
                    var error = new List<ValidationFailure>
                {
                    new ValidationFailure("groupId", "There are no students in this group.")
                };
                    return Json(new { ErrorList = error, Success = false }, JsonRequestBehavior.AllowGet);
                }
                var assignment = parameters.TestService.GetTestById(model.TestId);
                model.TestName = assignment.Name;
                model.SubjectName = parameters.BankService.GetBankById(assignment.BankId).Name;
                model.ClassIdList = groupStudents.Select(x => x.ClassId).Distinct().ToList();



                var virtualTest = parameters.VirtualTestService.Select().SingleOrDefault(en => en.VirtualTestID == model.TestId);

                var canUseLargeGroup = virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.ACT
                                       || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT
                                       || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.SAT
                                       || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewSAT;

                model.IsLargeClass = model.IsLargeClass && canUseLargeGroup;

                RequestSheet requestSheet = model.IsLargeClass
                    ? CreateGenericRequestLargeClass(groupStudents, assignment, model.GroupId)
                    : CreateGenericRequest(groupStudents, assignment);

                requestSheet.NumberOfGraphExtraPages = model.NumberOfGraphExtraPages;
                requestSheet.NumberOfLinedExtraPages = model.NumberOfLinedExtraPages;
                requestSheet.NumberOfPlainExtraPages = model.NumberOfPlainExtraPages;
                requestSheet.IsGridStype = model.IsGridStype;
                requestSheet.IsExtraPagesOnly = model.IsPrintExtraPageOnly;
                requestSheet.Preference = preference;
                requestSheet.NumberOfPrimaryExtraPages = model.NumberOfPrimaryExtraPages;

                if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.ACT
                    || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT)
                {
                    requestSheet.TemplateText["DistrictLogoLink"] = GetDistrictLogo(districtID, s3CSSKey);
                    if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT)
                    {
                        requestSheet.Template = model.IsIncludeEssayPage
                            ? Constanst.TemplateNewACT
                            : Constanst.TempateNewACTNoEssay;
                    }
                    else
                    {
                        requestSheet.Template = model.IsIncludeEssayPage
                            ? Constanst.TemplateACT
                            : Constanst.TempateACTNoEssay;
                    }

                    requestSheet.BubbleSize = parameters.BubbleSheetPrintingService.GetBubbleSize(0); // Size: 25
                    parameters.BubbleSheetPrintingService.SetSectionQuestions(requestSheet, assignment.Id,
                        model.BubbleFormat,
                        virtualTest,
                        virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT);

                    if (model.IsLargeClass)
                        parameters.BubbleSheetPrintingService.SetPrintStudentIDsListOptionLargeClass(requestSheet, model);
                    else
                        parameters.BubbleSheetPrintingService.SetPrintStudentIDsListOption(requestSheet, model);
                }
                else if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.SAT || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewSAT)
                {
                    requestSheet.TemplateText["DistrictLogoLink"] = GetDistrictLogo(districtID, s3CSSKey);
                    var isIncludeEssayPage = parameters.BubbleSheetPrintingService.CheckSATIncludeEssayPage(virtualTest,
                        districtID);

                    if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.SAT)
                        requestSheet.Template = isIncludeEssayPage
                            ? Constanst.TemplateSAT
                            : Constanst.TemplateSATNoEssay;
                    else
                    {
                        requestSheet.Template = isIncludeEssayPage
                            ? model.IsIncludeEssayPage ? Constanst.TemplateNewSAT : Constanst.TemplateNewSATWritingNoEssay
                            : Constanst.TemplateNewSATNoWriting;
                    }

                    requestSheet.BubbleSize = parameters.BubbleSheetPrintingService.GetBubbleSize(0); // Size: 25
                    parameters.BubbleSheetPrintingService.SetSectionQuestions(requestSheet, assignment.Id,
                        model.BubbleFormat, virtualTest);
                    parameters.BubbleSheetPrintingService.SetShadingOption(requestSheet, model);

                    if (model.IsLargeClass)
                        parameters.BubbleSheetPrintingService.SetPrintStudentIDsListOptionLargeClass(requestSheet, model);
                    else
                        parameters.BubbleSheetPrintingService.SetPrintStudentIDsListOption(requestSheet, model);
                }
                else
                {
                    requestSheet.BubbleSize = parameters.BubbleSheetPrintingService.GetBubbleSize(model.BubbleSizeId);
                    parameters.BubbleSheetPrintingService.SetQuestions(requestSheet, assignment.Id, model.BubbleFormat, model.PaginationQuestionIds);
                    parameters.BubbleSheetPrintingService.SetDefaultSectionQuestions(requestSheet, assignment.Id,
                        model.BubbleFormat, model.PaginationSectionIds);
                    requestSheet.TemplateText["PaginationOption"] = model.PaginationOption.ToString();
                }

                var bubbleSheets = model.IsLargeClass
                    ? CreateGenericBubbleSheetsLargeClass(model, groupStudents, requestSheet, currentUserId, nowDateString)
                    : CreateGenericBubbleSheets(model, groupStudents, requestSheet, currentUserId, nowDateString);

                if (bubbleSheets.Any(b => b.IsValid == false))
                {
                    var bubbleSheet = bubbleSheets.First(b => b.IsValid == false);
                    return Json(new { ErrorList = bubbleSheet.ValidationErrors, Success = false }, JsonRequestBehavior.AllowGet);
                }

                ValidateCropMarkResponse validateBubblesheetOutsideCropmark = BubbleSheetWsHelper.ValidateBubbleOutsideCropMark(requestSheet, environmentId);
                if (validateBubblesheetOutsideCropmark == null)
                {
                    return Json(new { ErrorList = "Server error. Please try again", Success = false, IsBubbleSheetOutsideCropMark = false, SnapshotUrl = string.Empty }, JsonRequestBehavior.AllowGet);
                }
                if (validateBubblesheetOutsideCropmark.IsValid == false)
                {
                    return Json(new { ErrorList = string.Empty, Success = false, IsBubbleSheetOutsideCropMark = true, SnapshotUrl = validateBubblesheetOutsideCropmark.SnapshotUrl }, JsonRequestBehavior.AllowGet);
                }

                if (model.IsGenericBubbleSheet)
                {
                    if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.ACT ||
                        virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.SAT ||
                        virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT
                        || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewSAT)
                    {
                        requestSheet.TemplateText["IsGeneric"] = "true";
                        requestSheet.TemplateText["NumberOfSheet"] = model.NumberOfGenericSheet.ToString();
                    }
                }

                BubbleSheetService.Save(bubbleSheets);

                var barcodes = bubbleSheets.Select(x => x.Id.ToString(CultureInfo.InvariantCulture));
                requestSheet.Barcode1 = barcodes.ToList();

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

                var ticket = CreateResponseTicket(requestSheet, bubbleSheets, model.IsManualEntry, environmentId, model.TestName);

                return ticket;
            }
            else
            {
                var groupStudents = parameters.GroupPrintingService.GetGroupStudents(model.GroupId).OrderBy(x => x.ClassId).ThenBy(x => x.StudentName).ToList();
                if (groupStudents.Count == 0)
                {
                    var error = new List<ValidationFailure>
                {
                    new ValidationFailure("groupId", "There are no students in this group.")
                };
                    return Json(new { ErrorList = error, Success = false }, JsonRequestBehavior.AllowGet);
                }
                var assignment = parameters.TestService.GetTestById(model.TestId);
                model.TestName = assignment.Name;
                model.SubjectName = parameters.BankService.GetBankById(assignment.BankId).Name;

                RequestSheet requestSheet = CreateRequest(groupStudents, assignment);
                requestSheet.NumberOfGraphExtraPages = model.NumberOfGraphExtraPages;
                requestSheet.NumberOfLinedExtraPages = model.NumberOfLinedExtraPages;
                requestSheet.NumberOfPlainExtraPages = model.NumberOfPlainExtraPages;
                requestSheet.IsGridStype = model.IsGridStype;
                requestSheet.IsExtraPagesOnly = model.IsPrintExtraPageOnly;
                requestSheet.Preference = preference;
                requestSheet.NumberOfPrimaryExtraPages = model.NumberOfPrimaryExtraPages;

                var virtualTest = parameters.VirtualTestService.Select().SingleOrDefault(en => en.VirtualTestID == model.TestId);
                if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.ACT
                    || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT)
                {
                    if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT)
                    {
                        requestSheet.Template = model.IsIncludeEssayPage
                            ? Constanst.TemplateNewACT
                            : Constanst.TempateNewACTNoEssay;
                    }
                    else
                    {
                        requestSheet.Template = model.IsIncludeEssayPage
                            ? Constanst.TemplateACT
                            : Constanst.TempateACTNoEssay;
                    }

                    requestSheet.BubbleSize = parameters.BubbleSheetPrintingService.GetBubbleSize(0); // Size: 25
                    parameters.BubbleSheetPrintingService.SetSectionQuestions(requestSheet, assignment.Id, model.BubbleFormat,
                        virtualTest,
                        virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewACT);
                }
                else if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.SAT
                    || virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.NewSAT)
                {
                    var isIncludeEssayPage = parameters.BubbleSheetPrintingService.CheckSATIncludeEssayPage(virtualTest,
                        districtID);

                    if (virtualTest.VirtualTestSubTypeID == (int)VirtualTestSubType.SAT)
                        requestSheet.Template = isIncludeEssayPage ? Constanst.TemplateSAT : Constanst.TemplateSATNoEssay;
                    else
                        requestSheet.Template = isIncludeEssayPage
                            ? model.IsIncludeEssayPage ? Constanst.TemplateNewSAT : Constanst.TemplateNewSATWritingNoEssay
                            : Constanst.TemplateNewSATNoWriting;
                    requestSheet.BubbleSize = parameters.BubbleSheetPrintingService.GetBubbleSize(0); // Size: 25
                    parameters.BubbleSheetPrintingService.SetSectionQuestions(requestSheet, assignment.Id,
                        model.BubbleFormat, virtualTest);
                    parameters.BubbleSheetPrintingService.SetShadingOption(requestSheet, model);
                }
                else
                {
                    requestSheet.BubbleSize = parameters.BubbleSheetPrintingService.GetBubbleSize(model.BubbleSizeId);
                    parameters.BubbleSheetPrintingService.SetQuestions(requestSheet, assignment.Id, model.BubbleFormat, model.PaginationQuestionIds);
                    parameters.BubbleSheetPrintingService.SetDefaultSectionQuestions(requestSheet, assignment.Id,
                        model.BubbleFormat, model.PaginationSectionIds);
                    requestSheet.TemplateText["PaginationOption"] = model.PaginationOption.ToString();
                }

                var bubbleSheets = CreateBubbleSheets(model, groupStudents, requestSheet, currentUserId, nowDateString);

                if (bubbleSheets.Any(b => b.IsValid == false))
                {
                    var bubbleSheet = bubbleSheets.First(b => b.IsValid == false);
                    return Json(new { ErrorList = bubbleSheet.ValidationErrors, Success = false }, JsonRequestBehavior.AllowGet);
                }

                ValidateCropMarkResponse validateBubblesheetOutsideCropmark = BubbleSheetWsHelper.ValidateBubbleOutsideCropMark(requestSheet, environmentId);
                if (validateBubblesheetOutsideCropmark == null)
                {
                    return Json(new { ErrorList = "Server error. Please try again", Success = false, IsBubbleSheetOutsideCropMark = false, SnapshotUrl = string.Empty }, JsonRequestBehavior.AllowGet);
                }
                if (validateBubblesheetOutsideCropmark.IsValid == false)
                {
                    return Json(new { ErrorList = string.Empty, Success = false, IsBubbleSheetOutsideCropMark = true, SnapshotUrl = validateBubblesheetOutsideCropmark.SnapshotUrl }, JsonRequestBehavior.AllowGet);
                }

                BubbleSheetService.Save(bubbleSheets);

                var barcodes = bubbleSheets.Select(x => x.Id.ToString(CultureInfo.InvariantCulture));
                requestSheet.Barcode1 = barcodes.ToList();

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

                var ticket = CreateResponseTicket(requestSheet, bubbleSheets, model.IsManualEntry, environmentId, model.TestName);

                return ticket;
            }
        }

        private List<BubbleSheet> CreateBubbleSheets(BubbleSheetGroupData model, IEnumerable<StudentGroup> groupStudents, RequestSheet requestSheet, int currentUserId, string nowDateString)
        {
            var bubbleSheets = new List<BubbleSheet>();
            AssignPrimaryTeacher(groupStudents);
            foreach (var groupStudent in groupStudents)
            {
                Dictionary<string, string> studentDictionary;
                List<string> listSpecialTemplate = new List<string>
                                                   {
                                                       Constanst.TempateACTNoEssay,
                                                       Constanst.TemplateACT,
                                                       Constanst.TempateNewACTNoEssay,
                                                       Constanst.TemplateNewACT,
                                                       Constanst.TemplateSAT,
                                                       Constanst.TemplateSATNoEssay,
                                                       Constanst.TemplateNewSAT,
                                                       Constanst.TemplateNewSATNoWriting,
                                                       Constanst.TemplateNewSATWritingNoEssay
                                                   };
                if (listSpecialTemplate.Contains(requestSheet.Template))
                {
                    studentDictionary = new Dictionary<string, string>
                                                {
                                                    {"Name", groupStudent.StudentName + " (" + groupStudent.Code + ")"},
                                                    {
                                                        "FooterLeft1",
                                                        ConfigurationManager.AppSettings["BubbleSheetFooterVersion"]
                                                    },
                                                    {
                                                        "FooterLeft2", groupStudent.TeacherName + " - " + groupStudent.ClassName
                                                    },
                                                    {"HeaderRight1", groupStudent.SchoolName},
                                                    {"HeaderRight3", model.TestName},
                                                    {"HeaderLeft1", groupStudent.DistrictName},
                                                    {"PaginationOption", model.PaginationOption.ToString()}
                                                };
                }
                else
                {
                    studentDictionary = new Dictionary<string, string>
                                            {
                                                {"Name", groupStudent.StudentName + " (" + groupStudent.Code + ")"},
                                                {
                                                    "TestDetail3",
                                                    ConfigurationManager.AppSettings["BubbleSheetFooterVersion"]
                                                },
                                                {"TestDetail4", "Created " + nowDateString},
                                                {"Header", groupStudent.TeacherName + " - " + groupStudent.ClassName},
                                                {"NameHeader", groupStudent.SchoolName},
                                                {"TestDetail1", model.SubjectName},
                                                {"TestDetail2", model.TestName},
                                                {"TestDetail5", groupStudent.DistrictName},
                                                {"PaginationOption", model.PaginationOption.ToString()}
                                            };
                }


                requestSheet.PerSheetTemplateText.Add(studentDictionary);

                var bubbleSheet = new BubbleSheet
                {
                    BubbleSheetCode = model.TestId + "." + groupStudent.StudentId,
                    ClassId = groupStudent.ClassId,
                    SchoolId = groupStudent.SchoolId,
                    StudentId = groupStudent.StudentId,
                    TestId = model.TestId,
                    BubbleSize = parameters.BubbleSheetPrintingService.GetBubbleSize(model.BubbleSizeId).ToString(CultureInfo.InvariantCulture),
                    UserId = groupStudent.TeacherId,
                    DistrictTermId = groupStudent.DistrictTermId,
                    SubmittedDate = DateTime.UtcNow,
                    CreatedByUserId = currentUserId,
                    IsManualEntry = model.IsManualEntry,
                };
                bubbleSheet.SetValidator(BubbleSheetValidator);
                bubbleSheets.Add(bubbleSheet);
            }
            return bubbleSheets;
        }

        private List<BubbleSheet> CreateGenericBubbleSheets(BubbleSheetGroupData model, IEnumerable<StudentGroup> groupStudents, RequestSheet requestSheet, int currentUserId, string nowDateString)
        {
            var bubbleSheets = new List<BubbleSheet>();
            AssignPrimaryTeacher(groupStudents);
            var listClassIDs = groupStudents.Select(x => x.ClassId).Distinct().ToList();
            var genericStudentName = "Student Name: _______________________________";
            foreach (var classID in listClassIDs)
            {
                Dictionary<string, string> studentDictionary;
                List<string> listSpecialTemplate = new List<string>
                                                   {
                                                       Constanst.TempateACTNoEssay,
                                                       Constanst.TemplateACT,
                                                       Constanst.TemplateSAT,
                                                       Constanst.TemplateSATNoEssay,
                                                       Constanst.TempateNewACTNoEssay,
                                                       Constanst.TemplateNewACT,
                                                       Constanst.TemplateNewSAT,
                                                       Constanst.TemplateNewSATNoWriting,
                                                       Constanst.TemplateNewSATWritingNoEssay
                                                   };
                var groupStudent = groupStudents.First(x => x.ClassId == classID);
                if (listSpecialTemplate.Contains(requestSheet.Template))
                {
                    studentDictionary = new Dictionary<string, string>
                                                {
                                                    {"Name", genericStudentName},
                                                    {
                                                        "FooterLeft1",
                                                        ConfigurationManager.AppSettings["BubbleSheetFooterVersion"]
                                                    },
                                                    {
                                                        "FooterLeft2", groupStudent.TeacherName + " - " + groupStudent.ClassName
                                                    },
                                                    {"HeaderRight1", groupStudent.SchoolName},
                                                    {"HeaderRight3", model.TestName},
                                                    {"HeaderLeft1", groupStudent.DistrictName}
                                                };
                }
                else
                {
                    studentDictionary = new Dictionary<string, string>
                                            {
                                                {"Name", genericStudentName},
                                                {
                                                    "TestDetail3",
                                                    ConfigurationManager.AppSettings["BubbleSheetFooterVersion"]
                                                },
                                                {"TestDetail4", "Created " + nowDateString},
                                                {"Header", groupStudent.TeacherName + " - " + groupStudent.ClassName},
                                                {"NameHeader", groupStudent.SchoolName},
                                                {"TestDetail1", model.SubjectName},
                                                {"TestDetail2", model.TestName},
                                                {"TestDetail5", groupStudent.DistrictName}
                                            };
                }


                requestSheet.PerSheetTemplateText.Add(studentDictionary);

                var bubbleSheet = new BubbleSheet
                {
                    BubbleSheetCode = "na",
                    ClassId = groupStudent.ClassId,
                    SchoolId = groupStudent.SchoolId,
                    StudentId = 0,
                    StudentIds = string.Empty,
                    TestId = model.TestId,
                    BubbleSize = parameters.BubbleSheetPrintingService.GetBubbleSize(model.BubbleSizeId).ToString(CultureInfo.InvariantCulture),
                    UserId = groupStudent.TeacherId,
                    DistrictTermId = groupStudent.DistrictTermId,
                    SubmittedDate = DateTime.UtcNow,
                    CreatedByUserId = currentUserId,
                    IsManualEntry = false,
                    IsGenericSheet = true
                };
                bubbleSheet.SetValidator(BubbleSheetValidator);
                bubbleSheets.Add(bubbleSheet);
            }
            return bubbleSheets;
        }

        private List<BubbleSheet> CreateGenericBubbleSheetsLargeClass(BubbleSheetGroupData model,
            IEnumerable<StudentGroup> groupStudents, RequestSheet requestSheet, int currentUserId, string nowDateString)
        {
            var bubbleSheets = new List<BubbleSheet>();
            AssignPrimaryTeacher(groupStudents);
            var genericStudentName = "Student Name: _______________________________";

            var printingGroupName = "";
            var printingGroup =
                parameters.PrintingGroupDataService.GetPrintingGroupDataByGroupID(model.GroupId).FirstOrDefault();
            if (printingGroup != null)
                printingGroupName = printingGroup.GroupName;

            Dictionary<string, string> studentDictionary;
            List<string> listSpecialTemplate = new List<string>
            {
                Constanst.TempateACTNoEssay,
                Constanst.TemplateACT,
                Constanst.TemplateSAT,
                Constanst.TemplateSATNoEssay,
                Constanst.TemplateNewACT,
                Constanst.TempateNewACTNoEssay,
                Constanst.TemplateNewSAT,
                Constanst.TemplateNewSATNoWriting,
                Constanst.TemplateNewSATWritingNoEssay
            };
            var firstStudent = groupStudents.First();
            if (listSpecialTemplate.Contains(requestSheet.Template))
            {
                studentDictionary = new Dictionary<string, string>
                {
                    {"Name", genericStudentName},
                    {
                        "FooterLeft1",
                        ConfigurationManager.AppSettings["BubbleSheetFooterVersion"]
                    },
                    {
                        "FooterLeft2", printingGroupName
                    },
                    {"HeaderRight1", firstStudent.SchoolName},
                    {"HeaderRight3", model.TestName},
                    {"HeaderLeft1", firstStudent.DistrictName}
                };
            }
            else
            {
                studentDictionary = new Dictionary<string, string>
                {
                    {"Name", genericStudentName},
                    {
                        "TestDetail3",
                        ConfigurationManager.AppSettings["BubbleSheetFooterVersion"]
                    },
                    {"TestDetail4", "Created " + nowDateString},
                    {"Header", printingGroupName},
                    {"NameHeader", firstStudent.SchoolName},
                    {"TestDetail1", model.SubjectName},
                    {"TestDetail2", model.TestName},
                    {"TestDetail5", firstStudent.DistrictName}
                };
            }


            requestSheet.PerSheetTemplateText.Add(studentDictionary);

            var bubbleSheet = new BubbleSheet
            {
                BubbleSheetCode = "na",
                ClassId = 0,
                SchoolId = firstStudent.SchoolId,
                StudentId = 0,
                StudentIds = string.Empty,
                TestId = model.TestId,
                BubbleSize =
                    parameters.BubbleSheetPrintingService.GetBubbleSize(model.BubbleSizeId)
                        .ToString(CultureInfo.InvariantCulture),
                UserId = firstStudent.TeacherId,
                DistrictTermId = firstStudent.DistrictTermId,
                SubmittedDate = DateTime.UtcNow,
                CreatedByUserId = currentUserId,
                IsManualEntry = false,
                IsGenericSheet = true,
                ClassIds = string.Join(";", groupStudents.Select(x => x.ClassId).Distinct().ToList())
            };
            bubbleSheet.SetValidator(BubbleSheetValidator);
            bubbleSheets.Add(bubbleSheet);

            return bubbleSheets;
        }

        public void AssignPrimaryTeacher(IEnumerable<StudentGroup> students)
        {
            foreach (var classId in students.Select(x => x.ClassId).Distinct())
            {
                var classUser = parameters.ClassUserService.GetPrimaryTeacherByClassId(classId);
                if (classUser.IsNull()) return;

                var primaryTeacher = parameters.UserService.GetUserById(classUser.UserId);
                if (primaryTeacher.IsNull()) return;

                var studentsInClass = students.Where(x => x.ClassId == classId);

                foreach (var studentGroup in studentsInClass)
                {
                    studentGroup.TeacherId = primaryTeacher.Id;
                    studentGroup.TeacherName = primaryTeacher.LastName;
                }
            }
        }

        private RequestSheet CreateRequest(ICollection<StudentGroup> groupStudents, Test assignment)
        {
            var removal = new Regex("[^a-zA-Z0-9 -]");
            var requestSheet = parameters.BubbleSheetPrintingService.InitializeRequestSheet(groupStudents.Count, ConfigurationManager.AppSettings["ApiKey"]);

            //requestSheet.SetValidator(parameters.SheetRequestValidator);
            requestSheet.OutputFormat = "Multi-Pdf";
            requestSheet.QuestionCount = assignment.QuestionCount;
            requestSheet.Barcode2 = string.Format("10001{0}", DateTime.UtcNow.ToString("yyMMdd"));

            requestSheet.DocumentDetails.AddRange(groupStudents.GroupBy(x => x.ClassId).OrderBy(x => x.Key).Select(x =>
              {
                  var first = x.First();
                  return new DocumentDetails
                  {
                      DistrictId = first.DistrictId,
                      PageCount = x.Count(),
                      Title = string.Format("{0}-{1}-{2}-{3}-{4}-{5}",
                                              removal.Replace(first.DistrictName, string.Empty),
                                              removal.Replace(first.SchoolName, string.Empty),
                                              removal.Replace(first.TeacherName, string.Empty),
                                              removal.Replace(first.ClassName, string.Empty),
                                              removal.Replace(assignment.Name, string.Empty),
                                              x.Key)
                  };
              }));
            return requestSheet;
        }

        private RequestSheet CreateGenericRequest(ICollection<StudentGroup> groupStudents, Test assignment)
        {
            var removal = new Regex("[^a-zA-Z0-9 -]");
            var classCount = groupStudents.Select(x => x.ClassId).Distinct().Count();
            var requestSheet = parameters.BubbleSheetPrintingService.InitializeRequestSheet(classCount, ConfigurationManager.AppSettings["ApiKey"]);

            //requestSheet.SetValidator(parameters.SheetRequestValidator);
            requestSheet.OutputFormat = "Multi-Pdf";
            requestSheet.QuestionCount = assignment.QuestionCount;
            requestSheet.Barcode2 = string.Format("10001{0}", DateTime.UtcNow.ToString("yyMMdd"));

            requestSheet.DocumentDetails.AddRange(groupStudents.GroupBy(x => x.ClassId).OrderBy(x => x.Key).Select(x =>
            {
                var first = x.First();
                return new DocumentDetails
                {
                    DistrictId = first.DistrictId,
                    PageCount = 1,
                    Title = string.Format("{0}-{1}-{2}-{3}-{4}-{5}",
                                            removal.Replace(first.DistrictName, string.Empty),
                                            removal.Replace(first.SchoolName, string.Empty),
                                            removal.Replace(first.TeacherName, string.Empty),
                                            removal.Replace(first.ClassName, string.Empty),
                                            removal.Replace(assignment.Name, string.Empty),
                                            x.Key)
                };
            }));
            return requestSheet;
        }

        private RequestSheet CreateGenericRequestLargeClass(ICollection<StudentGroup> groupStudents, Test assignment, int groupId)
        {
            var removal = new Regex("[^a-zA-Z0-9 -]");
            var requestSheet = parameters.BubbleSheetPrintingService.InitializeRequestSheet(1, ConfigurationManager.AppSettings["ApiKey"]);

            requestSheet.OutputFormat = "Multi-Pdf";
            requestSheet.QuestionCount = assignment.QuestionCount;
            requestSheet.Barcode2 = string.Format("10001{0}", DateTime.UtcNow.ToString("yyMMdd"));

            var printingGroupName = "";
            var printingGroup = parameters.PrintingGroupDataService.GetPrintingGroupDataByGroupID(groupId).FirstOrDefault();
            if (printingGroup != null)
                printingGroupName = printingGroup.GroupName;

            var first = groupStudents.First();
            requestSheet.DocumentDetails.Add(new DocumentDetails
            {
                DistrictId = first.DistrictId,
                PageCount = 1,
                Title = string.Format("{0}-{1}",
                    removal.Replace(first.DistrictName, string.Empty),
                    removal.Replace(printingGroupName, string.Empty))
            });

            return requestSheet;
        }

        [VersionFilter]
        public ActionResult LoadListPrintingGroup()
        {
            var model = new GenerateBubbleSheetViewModel
            {
                IsAdmin = parameters.UserService.GetAdminByUsernameAndDistrict(CurrentUser.UserName, CurrentUser.DistrictId.GetValueOrDefault()).IsNotNull(),
                CanSelectTeachers = CurrentUser.IsDistrictAdminOrPublisher,
                IsSchoolAdmin = CurrentUser.RoleId.Equals(8),
                IsPublisher = CurrentUser.RoleId.Equals((int)Permissions.Publisher),
                IsNetworkAdmin = CurrentUser.IsNetworkAdmin
            };
            return PartialView("_ListPrintingGroup", model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AddEditPrintingGroup(AddEditPrintingGroupViewModel model)
        {
            //TODO: check model valid       
            if (model.DistrictId <= 0)
            {
                return Json(new { Success = false, ErrorMessage = "Please select a " + LabelHelper.DistrictLabel + "." });
            }
            if (!string.IsNullOrEmpty(model.Name))
            {
                int groupId = SavePrintingGroup(model);
                SaveClassInGroup(groupId, model.ClassIdList, model.TeacherIdList);
                return Json(new { Success = true, Id = groupId, GroupName = model.Name });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = "Group name is required." });
            }

        }

        [VersionFilter]
        public ActionResult AddEditPrintingGroup(int? groupId, int? districtId)
        {
            //if (CurrentUser.IsLinkItAdminOrPublisher() && districtId.GetValueOrDefault() == 0)
            //{
            //    return Json(new { Success = true, Id = groupId, GroupName = model.Name }); 
            //}
            var printingGroup = new AddEditPrintingGroupViewModel();
            if (groupId > 0)
            {
                var obj = parameters.GroupPrintingService.GetById(groupId.GetValueOrDefault());
                if (obj.IsNotNull())
                {
                    printingGroup.Name = obj.Name;
                    printingGroup.GroupId = obj.Id;
                }
            }

            printingGroup.DistrictId = CurrentUser.IsLinkItAdminOrPublisher() || CurrentUser.IsNetworkAdmin ? districtId.GetValueOrDefault() : GetDistrictIdFromSubdomain();
            printingGroup.CreatedUserId = CurrentUser.Id;
            return PartialView("_AddEditPrintingGroup", printingGroup);
        }

        public ActionResult GetPrintingGroup(int? id)
        {
            var districtSelected = CurrentUser.IsLinkItAdminOrPublisher() || CurrentUser.IsNetworkAdmin ? id.GetValueOrDefault() : GetDistrictIdFromSubdomain();
            var groups = parameters.GroupPrintingService.GetAllByCurrentUser(-1, districtSelected);
            if (CurrentUser.RoleId == (int)Permissions.SchoolAdmin || CurrentUser.RoleId == (int)Permissions.Teacher)
            {
                List<int> lstGroupIdshasAccess =
                    parameters.GroupPrintingService.GetListGroupIdsByUserId(districtSelected, CurrentUser.Id);
                groups = groups.Where(o => lstGroupIdshasAccess.Contains(o.Id));

                if (CurrentUser.RoleId == (int)Permissions.Teacher)
                {
                    groups = groups.Where(m => m.CreatedUserId == CurrentUser.Id);
                }
            }

            var groupmodel = groups.Select(x => new PrintingGroupViewModel()
            {
                Id = x.Id,
                Name = x.Name
            });
            var parser = new DataTableParser<PrintingGroupViewModel>();
            return Json(parser.Parse(groupmodel, true), JsonRequestBehavior.AllowGet);
        }

        private int GetDistrictIdFromSubdomain()
        {
            var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            return parameters.DistrictService.GetLiCodeBySubDomain(subDomain);
        }

        private int SavePrintingGroup(AddEditPrintingGroupViewModel model)
        {
            PrintingGroup obj = new PrintingGroup();
            obj.Id = model.GroupId;
            obj.Name = model.Name;
            obj.DistrictId = model.DistrictId;
            obj.CreatedUserId = CurrentUser.Id;
            obj.IsActive = true;
            parameters.GroupPrintingService.Save(obj);
            return obj.Id;
        }

        public ActionResult GetClassInGroupByGroupId(int groupId)
        {
            var vClasses =
                parameters.PrintingGroupDataService.GetPrintingGroupDataByGroupID(groupId)
                    .Select(
                        x =>
                            new PrintingGroupViewModel
                            {
                                Id = x.ClassID,
                                UserId = x.UserID,
                                Name = x.ClassName,
                                Detail =
                                    x.SchoolName + ", " + x.TeacherName + ", " + x.DistrictTermName + ", " + x.ClassName
                            })
                    .OrderBy(x => x.Name);
            return Json(vClasses, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetClassesBySchoolAndTermsAndUsers(int schoolId, string termIds, string userIds)
        {
            var data = new List<PrintingGroupViewModel>();

            if (!string.IsNullOrEmpty(userIds) && !string.IsNullOrEmpty(termIds))
            {
                var listUserId = userIds.Split(';').Select(x => Convert.ToInt32(x)).ToList();
                var listTermId = termIds.Split(';').Select(x => Convert.ToInt32(x)).ToList();
                data =
                    parameters.TestAssignmentService.GetClassesBySchoolIdAndTermIdsAndUserIds(listTermId, listUserId,
                        schoolId).DistinctBy(x => x.Id)
                        .Select(
                            x =>
                                new PrintingGroupViewModel
                                {
                                    Id = x.Id,
                                    UserId = x.UserId.GetValueOrDefault(),
                                    Name = x.Name,
                                    Detail =
                                        x.SchoolName + ", " +
                                        (x.TeacherFirstName == string.Empty
                                            ? x.TeacherLastName
                                            : x.TeacherLastName + ", " + x.TeacherFirstName) + ", " + x.DistrictTermName +
                                        ", " +
                                        x.Name
                                }).OrderBy(x => x.Name).ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private bool SaveClassInGroup(int groupId, List<string> classIdList, List<int> teacherIdList)
        {
            if (groupId > 0)
            {
                parameters.ClassPrintingGroupService.SaveClassPrintingGroupByGroupId(groupId, classIdList, teacherIdList);
                return true;
            }
            return false;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManagebubblesheetsGimport)]
        public ActionResult ImportGroup()
        {
            var model = new UploadFilesViewModel
            {
                IsPublisherUploading = CurrentUser.RoleId.Equals((int)Permissions.Publisher),

            };
            if (CurrentUser.IsNetworkAdmin)
            {
                model.IsNetworkAdmin = true;
                model.ListDistricIds = CurrentUser.GetMemberListDistrictId();
            }
            return View(model);
        }

        [UploadifyPrincipal(Order = 1)]
        [AjaxOnly]
        public ActionResult ImportGroup(HttpPostedFileBase postedFile, int? districtId)
        {
            var listError = new List<ValidationFailure>();
            int currentDistrictId = GetDistrictIdFromSubdomain();
            currentDistrictId = districtId.HasValue ? districtId.Value : currentDistrictId;
            if (!IsValidPostedFile(postedFile, listError, currentDistrictId))
            {
                return Json(new { ErrorList = listError, success = false, type = "Error" });
            }
            List<ImportGroups> lstImportGroups = BuildImportGroup(postedFile, currentDistrictId, listError);
            if (listError.Count > 0)
            {
                return Json(new { message = "", ErrorList = listError, success = false, type = "Error" });
            }
            if (lstImportGroups.Any(o => o.GroupId > 0))
            {
                return ProcessDuplicateGroup(lstImportGroups);
            }
            CreateGroupImport(lstImportGroups);
            return Json(new { success = true });
        }

        private bool IsValidPostedFile(HttpPostedFileBase file, List<FluentValidation.Results.ValidationFailure> listError, int districtId)
        {
            if (districtId == 0 && CurrentUser.RoleId.Equals((int)Permissions.Publisher))
            {
                listError.Add(new ValidationFailure(string.Empty, "Please select State and " + LabelHelper.DistrictLabel + " then try again"));
                return false;
            }
            if (file.IsNull())
            {
                listError.Add(new ValidationFailure(string.Empty, "Invalid file, please try again."));
                return false;
            }
            string strExtension = Path.GetExtension(file.FileName);
            if (!strExtension.ToLower().Equals(".txt"))
            {
                listError.Add(new ValidationFailure(string.Empty, "Invalid file extension, please try again."));
                return false;
            }
            return !string.IsNullOrEmpty(file.FileName) && file.InputStream.IsNotNull();
        }

        private List<ImportGroups> BuildImportGroup(HttpPostedFileBase postedFile, int districtId, List<FluentValidation.Results.ValidationFailure> lstError)
        {
            string strContent = GetContentFromPostedFile(postedFile);
            string[] arrRows = Regex.Split(strContent, Environment.NewLine);
            List<ImportGroups> lstImportGroup = new List<ImportGroups>();
            for (int i = 1; i < arrRows.Length; i++) //TODO: Ignore the first row
            {
                string[] rowData = arrRows[i].Split("\t".ToCharArray());
                if (CheckFormatRow(rowData, i, lstError))
                {
                    lstImportGroup.Add(new ImportGroups(rowData, i, districtId)); //TODO: only add row valid   
                }
            }
            if (lstImportGroup.Count > 0)
            {
                ValidationImportGroup(lstImportGroup, lstError);
            }
            return lstImportGroup;
        }

        private JsonResult ProcessDuplicateGroup(List<ImportGroups> lstImportGroups)
        {
            var lstValidGroup = lstImportGroups.Where(o => o.GroupId == 0).ToList();
            if (lstValidGroup.Count > 0)  //TODO: import Groups Valid
            {
                CreateGroupImport(lstValidGroup);
            }
            var groupDuplicate = lstImportGroups.Where(o => o.GroupId > 0).Select(o => o.GroupName).Distinct();
            var strNameDuplicate = ", ";
            foreach (string s in groupDuplicate)
            {
                strNameDuplicate += s;
            }
            var strMessage = string.Format("Following group name(s) already exist: {0} .Do you want to create new group(s) anyway?", strNameDuplicate.Substring(1));
            //TODO: cache list obje duplicate
            var lstGroupDuplicate = lstImportGroups.Where(o => o.GroupId > 0).ToList();
            HttpRuntime.Cache.Insert(DuplicatedGroupKey, lstGroupDuplicate, null, DateTime.Now.AddMinutes(3), TimeSpan.Zero);
            return Json(new { message = strMessage, success = false, type = "warning" });
        }

        private bool CreateGroupImport(List<ImportGroups> lstImportGroup)
        {
            if (lstImportGroup.Count == 0) return false;
            IEnumerable<string> lstGroupName = lstImportGroup.Select(o => o.GroupName).Distinct();
            int districtId = lstImportGroup[0].DistrictId;
            foreach (string s in lstGroupName)
            {
                PrintingGroup printingGroup = new PrintingGroup();
                printingGroup.IsActive = true;
                printingGroup.Name = s;
                printingGroup.CreatedUserId = CurrentUser.Id;
                printingGroup.DistrictId = districtId;
                parameters.GroupPrintingService.Save(printingGroup);
                IEnumerable<int> lstClassId = lstImportGroup.Where(o => o.GroupName.Equals(s)).Select(o => o.ClassId).Distinct();
                parameters.ClassPrintingGroupService.SaveClassInGroup(printingGroup.Id, lstClassId);
            }
            return true;
        }

        public JsonResult InsertDuplicateGroup()
        {
            var duplicatedGroupObj = HttpRuntime.Cache.Get(DuplicatedGroupKey);
            var lstDuplicateGroup = duplicatedGroupObj == null
                                        ? new List<ImportGroups>()
                                        : duplicatedGroupObj as List<ImportGroups>;
            if (lstDuplicateGroup.IsNotNull() && lstDuplicateGroup.Any())
            {
                CreateGroupImport(lstDuplicateGroup);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            HttpRuntime.Cache.Remove(DuplicatedGroupKey);
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        private bool CheckFormatRow(string[] rowData, int index, List<ValidationFailure> lstError)
        {
            if (rowData.Length != 5 || (rowData.Length == 5 && (string.IsNullOrWhiteSpace(rowData[0])
                                                                || string.IsNullOrWhiteSpace(rowData[1])
                                                                || string.IsNullOrWhiteSpace(rowData[2])
                                                                || string.IsNullOrWhiteSpace(rowData[3])
                                                                || string.IsNullOrWhiteSpace(rowData[4]))))
            {
                var error = new ValidationFailure("Format", string.Format("Error. Line {0}: Invalid input data format", index + 1));
                lstError.Add(error);
                return false;
            }
            return true;
        }

        private void ValidationImportGroup(List<ImportGroups> lstImportGroup, List<ValidationFailure> lstError)
        {
            int districtId = lstImportGroup[0].DistrictId;
            var groupNames = lstImportGroup.Select(o => o.GroupName).Distinct();
            var lstGroups = parameters.GroupPrintingService.GetPrintingGroupByNames(groupNames, districtId);
            var schoolNames = lstImportGroup.Select(o => o.SchoolName).Distinct();
            var lstSchools = parameters.SchoolService.GetSchoolByNames(schoolNames, districtId);
            var teacherNames = lstImportGroup.Select(o => o.TeacherName).Distinct();
            var lstTeacher = parameters.UserService.GetUserByNames(teacherNames, districtId);
            var districtTermNames = lstImportGroup.Select(o => o.TermName).Distinct();
            var lstDistrictTerm = parameters.DistrictTermService.GetSchoolByNames(districtTermNames, districtId);
            foreach (ImportGroups item in lstImportGroup)
            {
                item.GroupId = lstGroups.Any(o => o.Name.Equals(item.GroupName)) ? lstGroups.First(o => o.Name.Equals(item.GroupName)).Id : 0;
                item.SchoolId = lstSchools.Any(o => o.Name.Equals(item.SchoolName)) ? lstSchools.First(o => o.Name.Equals(item.SchoolName)).Id : 0;
                item.TeacherId = lstTeacher.Any(o => o.UserName.Equals(item.TeacherName)) ? lstTeacher.First(o => o.UserName.Equals(item.TeacherName)).Id : 0;
                item.DistrictTermId = lstDistrictTerm.Any(o => o.Name.Equals(item.TermName)) ? lstDistrictTerm.First(o => o.Name.Equals(item.TermName)).DistrictTermID : 0;
                item.ClassId = parameters.ClassService.GetClassIdByNameSchoolIdUserIdDistrictTermId(item.SchoolId, item.TeacherId, item.DistrictTermId, item.ClassName);
                if (item.DistrictTermId > 0)
                {
                    if (lstDistrictTerm.First(o => o.Name.Equals(item.TermName)).DateEnd.HasValue)
                    {
                        item.IsExpire = lstDistrictTerm.First(o => o.Name.Equals(item.TermName)).DateEnd.Value < DateTime.UtcNow;
                    }
                    else
                    {
                        item.IsExpire = false;
                    }
                }
                CheckValidImportGroup(item, lstError);
            }
        }

        private void CheckValidImportGroup(ImportGroups item, List<ValidationFailure> lstError)
        {
            if (item.SchoolId == 0)
            {
                lstError.Add(new ValidationFailure("SchoolName", string.Format("Error. Line {0}: School {1} not found", item.Index, item.SchoolName)));
            }
            if (item.TeacherId == 0)
            {
                lstError.Add(new ValidationFailure("UserName", string.Format("Error. Line {0}: Teacher {1} not found", item.Index, item.TeacherName)));
            }
            if (item.DistrictTermId == 0)
            {
                lstError.Add(new ValidationFailure("DistrictTermName", string.Format("Error. Line {0}: DistrictTerm {1} not found", item.Index, item.TermName)));
            }
            if (item.ClassId == 0)
            {
                if (item.DistrictTermId > 0 && item.IsExpire)
                {
                    lstError.Add(new ValidationFailure("ClassName", string.Format("Error. Line {0}: Expired Class {1}", item.Index, item.ClassName)));
                }
                else
                {
                    lstError.Add(new ValidationFailure("ClassName", string.Format("Error. Line {0}: Class {1} not found", item.Index, item.ClassName)));
                }
            }
        }

        private string GetContentFromPostedFile(HttpPostedFileBase postedFile)
        {
            StreamReader reader = new StreamReader(postedFile.InputStream);
            string content = reader.ReadToEnd();
            return content;
        }

        public ActionResult CheckUniqueGroupName(int groupId, string strGroupName)
        {
            if (parameters.GroupPrintingService.CheckUniqueGroupName(groupId, CurrentUser.Id, strGroupName))
            {
                return Json(new { Message = "", Success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "Group name must be unique.", Success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeletePrintingGroup(int printinggroupId)
        {
            if (parameters.GroupPrintingService.DeletePrintingGroupById(printinggroupId))
            {
                return Json(new { Success = true, Message = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, Message = "Failed to delete group. Selected group does not exist." }, JsonRequestBehavior.AllowGet);
        }

        private string GetDistrictLogo(int districtId, string s3CSSKey)
        {
            var logoUrl = string.Format("{0}{1}-logo.png", s3CSSKey, districtId);

            if (UrlUtil.CheckUrlStatus(logoUrl))
            {
                return logoUrl;
            }

            return "";
        }
    }
}
