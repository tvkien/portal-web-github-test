using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevExpress.XtraRichEdit.Internal;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleService.Models.Corrections;
using LinkIt.BubbleService.Models.Reading;
using LinkIt.BubbleService.Models.Test;
using LinkIt.BubbleService.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.CommonServices;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models.ErrorCorrection;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.Practices.ServiceLocation;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class ErrorCorrectionController : BubbleSheetBaseController
    {
        private readonly ErrorCorrectionControllerParameters parameters;

        public ErrorCorrectionController(ErrorCorrectionControllerParameters parameters,
            BubbleSheetService bubbleSheetService,

            IValidator<BubbleSheet> bubbleSheetValidator
            , ClassService classService
            , VirtualTestService virtualTestService
            , BubbleSheetCommonService bubbleSheetCommonService
            , UserService userService)
            : base(bubbleSheetService, bubbleSheetValidator, classService, virtualTestService, bubbleSheetCommonService, userService)
        {
            this.parameters = parameters;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManagebubblesheetsError)]
        public ActionResult ProcessErrors()
        {
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.StateId = CurrentUser.StateId;
            ViewBag.DistrictId = CurrentUser.DistrictId;

            return View();
        }

        public BubbleSheetFile CreateNewBubbleSheetFile(TestQuestionsViewModel model)
        {
            SeperateRelatedImageAndImputFilePath(model);

            return new BubbleSheetFile
            {
                BubbleSheetId = model.BubbleSheetId,
                Date = DateTime.UtcNow.Date,
                Barcode1 = string.Format("10060000{0}", model.BubbleSheetId),
                Barcode2 = string.Format("10001{0}", DateTime.UtcNow.ToString("yyMMdd")),
                InputFileName = model.InputFileName,
                InputFilePath = model.InputFilePath,
                PageNumber = 1,
                OutputFileName = model.RelatedImage,
                RosterPosition = model.RosterPosition,
                UserId = CurrentUser.Id,
                DistrictId = parameters.SchoolService.GetSchoolById(BubbleSheetService.GetBubbleSheetById(model.BubbleSheetId).SchoolId.GetValueOrDefault()).DistrictId
            };
        }

        public TestQuestionsViewModel SeperateRelatedImageAndImputFilePath(TestQuestionsViewModel model)
        {
            var piecesArray = model.RelatedImage.Split(' ');
            if (piecesArray.Length.Equals(2))
            {
                model.RelatedImage = piecesArray[0];
                model.InputFilePath = piecesArray[1];
            }
            else if (piecesArray.Length.Equals(1))
            {
                model.RelatedImage = piecesArray[0];
            }
            return model;
        }

        [HttpGet]
        public ActionResult GetBarcodes(string startsWith)
        {
            var barcodeList =BubbleSheetService.SearchBubbleSheetPageStartWithId(startsWith);
            return Json(barcodeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CorrectBarcodeError(int? id)
        {
            var bubbleSheetError = parameters.BubbleSheetErrorService.GetBubbleSheetErrorById(id.GetValueOrDefault());
            if (bubbleSheetError.IsNull())
            {
                return HttpNotFound();
            }

            var model = GetBubbleSheetErrorDetailsViewModel<TestImage>(id.GetValueOrDefault(), bubbleSheetError.ErrorCode);
            if (model.IsNull())
            {
                return RedirectToAction("ProcessErrors", "ErrorCorrection");
            }
            return View(model);
        }

        public ActionResult CorrectRosterPositionError(int? id)
        {
            var bubbleSheetFile = parameters.BubbleSheetFileService.GetBubbleSheetFileById(id.GetValueOrDefault());
            if (bubbleSheetFile.IsNull())
            {
                return HttpNotFound();
            }
            var model = GetBubbleSheetErrorDetailsViewModel<TestImage>(id.GetValueOrDefault(), bubbleSheetFile.RosterPosition.GetValueOrDefault());
            if (model.IsNull())
            {
                return RedirectToAction("ProcessErrors", "ErrorCorrection");
            }
            return View(model);
        }

        public ActionResult CorrectSheetReadError(int? id)
        {
            var bubbleSheetError = parameters.BubbleSheetErrorService.GetBubbleSheetErrorById(id.GetValueOrDefault());
            if (bubbleSheetError.IsNull())
            {
                return HttpNotFound();
            }

            var model = GetBubbleSheetErrorDetailsViewModel<TestImage>(id.GetValueOrDefault(), bubbleSheetError.ErrorCode);
            if (model.IsNull())
            {
                return RedirectToAction("ProcessErrors", "ErrorCorrection");
            }

            return View(model);
        }

        public ActionResult CorrectNumberOfQuestionsError(int? id)
        {
            var bubbleSheetError = parameters.BubbleSheetErrorService.GetBubbleSheetErrorById(id.GetValueOrDefault());
            if (bubbleSheetError.IsNull())
            {
                return HttpNotFound();
            }

            var model = GetBubbleSheetErrorDetailsViewModel<TestImage>(id.GetValueOrDefault(), bubbleSheetError.ErrorCode);
            if (model.IsNull())
            {
                return RedirectToAction("ProcessErrors", "ErrorCorrection");
            }
            model.UnansweredQuestions = GetUnansweredQuestionsForCorrectNumberOfQuestionsError(bubbleSheetError).ToList();
            GetMaxChoiceFromXlicontent(model.UnansweredQuestions);
            return View(model);
        }

        public ActionResult CorrectCorruptFileError(int? id)
        {
            var bubbleSheetError = parameters.BubbleSheetErrorService.GetBubbleSheetErrorById(id.GetValueOrDefault());
            if (bubbleSheetError.IsNull())
            {
                return HttpNotFound();
            }

            var model = GetBubbleSheetErrorDetailsViewModel<TestImage>(id.GetValueOrDefault(), bubbleSheetError.ErrorCode);
            if (model.IsNull())
            {
                return RedirectToAction("ProcessErrors", "ErrorCorrection");
            }
            return View(model);
        }

        private IEnumerable<UnansweredQuestion> GetUnansweredQuestionsForCorrectNumberOfQuestionsError(BubbleSheetError bubbleSheetError)
        {
            var bubbleSheet = BubbleSheetService.GetBubbleSheetById(bubbleSheetError.BubbleSheetId);
            if (bubbleSheet.IsNull() || bubbleSheet.Ticket.Equals(string.Empty))
            {
                return new BindingList<UnansweredQuestion>();
            }

            var student = parameters.BubbleSheetStudentResultsService.GetBubbleSheetStudentResultsByTicketAndClassId(bubbleSheet.Ticket, bubbleSheet.ClassId.GetValueOrDefault()).FirstOrDefault();
            if (student.IsNull())
            {
                return new BindingList<UnansweredQuestion>();
            }
            return parameters.UnansweredQuestionService.GetQuestionsWithAnswerChoicesForMissingSheets(bubbleSheet.Ticket, student.StudentId, bubbleSheet.ClassId.GetValueOrDefault());
        }

        [HttpGet]
        public ActionResult GetBubbleSheetErrorList(int? districtId)
        {
            var data = GetBubbleSheetErrorListData(districtId);
            var parser = new DataTableParser<BubbleSheetErrorListViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        private IQueryable<BubbleSheetErrorListViewModel> GetBubbleSheetErrorListData(int? districtId)
        {
            return GetFilteredBubbleSheetErrors(districtId).Select(x => new BubbleSheetErrorListViewModel
            {
                BubbleSheetErrorId = x.BubbleSheetErrorId,
                FileName = x.FileName,
                Message = x.Message,
                UploadedBy = x.UploadedBy,
                CreatedDate = x.CreatedDate,
                ErrorCode = x.ErrorCode
            });
        }

        private IQueryable<BubbleSheetError> GetFilteredBubbleSheetErrors(int? districtId)
        {
            return FilterBubbleSheetErrors(parameters.BubbleSheetErrorService.GetBubbleSheetErrors(), districtId);
        }

        private IQueryable<BubbleSheetError> FilterBubbleSheetErrors(IQueryable<BubbleSheetError> errors, int? districtId)
        {
            if (districtId == null)
                districtId = CurrentUser.DistrictId;
            switch (CurrentUser.RoleId)
            {
                case (int)Permissions.Publisher:
                    return errors;

                case (int)Permissions.DistrictAdmin:
                    return errors.Where(x => x.DistrictId.Equals(CurrentUser.DistrictId));

                case (int)Permissions.NetworkAdmin:
                    return errors.Where(x => x.DistrictId.Equals(districtId));

                default:
                    return errors.Where(x => x.UserId.Equals(CurrentUser.Id));
            }
        }

        private TestQuestionsViewModel GetBubbleSheetErrorDetailsViewModel<T>(int id, int errorCode) where T : IFileBlob
        {
            var bubbleSheetError = parameters.BubbleSheetErrorService.GetBubbleSheetErrorFromViewByIdAndErrorCode(id, errorCode);
            if (bubbleSheetError.IsNull())
            {
                return null;
            }

            var viewModel = new TestQuestionsViewModel
            {
                BubbleSheetErrorId = id,
                BubbleSheetId = bubbleSheetError.BubbleSheetId.GetValueOrDefault(),
                ErrorCode = bubbleSheetError.ErrorCode,
                RelatedImage = bubbleSheetError.RelatedImage,
                InputFileName = bubbleSheetError.FileName
            };

            viewModel = SeperateRelatedImageAndImputFilePath(viewModel);
            string imageUrl = string.Empty;
            if (typeof(T) == typeof(TestImage))
            {
                imageUrl = BubbleSheetWsHelper.GetTestImageUrl(viewModel.RelatedImage.Replace("/", "_"),
                    ConfigurationManager.AppSettings["ApiKey"]);
            }
            else if (typeof(T) == typeof(TestFile))
            {
                imageUrl = BubbleSheetWsHelper.GetTestFileUrl(viewModel.RelatedImage,
                    ConfigurationManager.AppSettings["ApiKey"]);
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                viewModel.RelatedImage = imageUrl;
            }
            
            return viewModel;
        }

        public ActionResult CorrectBubbleSheetError(int? id, int errorCode)
        {
            var bubbleSheetError = parameters.BubbleSheetErrorService.GetBubbleSheetErrorFromViewByIdAndErrorCode(id.GetValueOrDefault(), errorCode);
            if (bubbleSheetError.IsNull() || !IsDefinedErrorCode(bubbleSheetError.ErrorCode))
            {
                return Json("ProcessErrors");
            }

            var view = DetermineErrorCorrectionView((ErrorCode)bubbleSheetError.ErrorCode);
            if (view.Equals("CorrectNumberOfQuestionsError") && bubbleSheetError.VirtualTestId.HasValue)
            {
                var vVirtualTest = parameters.VirtualTestService.Select()
                        .FirstOrDefault(o => o.VirtualTestID == bubbleSheetError.VirtualTestId.Value);
                if (vVirtualTest != null && vVirtualTest.VirtualTestSubTypeID.HasValue &&
                    (vVirtualTest.VirtualTestSubTypeID.Value == (int)VirtualTestSubType.ACT))
                {
                    view = "ACTCorrectNumberOfQuestionsError";
                }
                if (vVirtualTest != null && vVirtualTest.VirtualTestSubTypeID.HasValue &&
                    (vVirtualTest.VirtualTestSubTypeID.Value == (int)VirtualTestSubType.SAT))
                {
                    view = "SATCorrectNumberOfQuestionsError";
                }
            }
            return Json(view + '/' + id.GetValueOrDefault());
        }

        private bool IsDefinedErrorCode(int bubbleSheetErrorCode)
        {
            return Enum.IsDefined(typeof(ErrorCode), bubbleSheetErrorCode);
        }

        private BarcodeCorrectionRequest BuildBarcodeCorrectionRequest(TestQuestionsViewModel model)
        {
            var bubbleSheetError = parameters.BubbleSheetErrorService.GetBubbleSheetErrorById(model.BubbleSheetErrorId);

            return new BarcodeCorrectionRequest
            {
                ApiKey = ConfigurationManager.AppSettings["ApiKey"],
                Barcode = model.Barcode,
                FileName = bubbleSheetError.FileName,
                RelatedImage = bubbleSheetError.RelatedImage,
                UploadedUserId = CurrentUser.Id
            };
        }

        public ActionResult SubmitBarcodeError(TestQuestionsViewModel model)
        {
            Guid guid;

            if (model.Barcode.IsNull() || !Guid.TryParse(model.Barcode, out guid))
            {
                return Json(false);
            }

            var barcodeCorrectionRequest = BuildBarcodeCorrectionRequest(model);
            //parameters.SheetCorrectionsService.SubmitManualBarcodeCorrection(barcodeCorrectionRequest);
            try
            {
                var response = BubbleSheetWsHelper.SubmitManualBarcodeCorrection(barcodeCorrectionRequest);
                if (response == null || response.Data == null || response.Data.Value == false)
                {
                    return Json(false);
                }
            }
            catch (WebException)
            {
                return Json(false);
            }
            RemoveBubbleSheetError(model.BubbleSheetErrorId);
            return Json(true);
        }

        public ActionResult SubmitRosterPositionError(TestQuestionsViewModel model)
        {
            var bubbleSheetFile = parameters.BubbleSheetFileService.GetBubbleSheetFileById(model.BubbleSheetErrorId);
            //var bubbleSheetFileCouldEntity = parameters.TestResultService.Select(bubbleSheetFile.InputFilePath, bubbleSheetFile.UrlSafeOutputFileName);
            //if (bubbleSheetFileCouldEntity.IsNull())
            //{
            //    return HttpNotFound();
            //}

            //bubbleSheetFileCouldEntity.Value.RosterPosition = model.RosterPosition.ToString();
            //parameters.TestResultService.Update(bubbleSheetFileCouldEntity);
            //var request = parameters.ReadResultService.CreateGradeRequest(bubbleSheetFileCouldEntity);
            //parameters.GradeRequestService.SendGradeRequest(request);

            var serviceModel = new RosterPositionCorrectionModel
            {
                InputFilePath = bubbleSheetFile.InputFilePath,
                UrlSafeOutputFileName =
                                                                 bubbleSheetFile.UrlSafeOutputFileName,
                NewRosterPosition = model.RosterPosition
            };
            try
            {
                var response = BubbleSheetWsHelper.SubmitRosterPositionError(serviceModel);
                if (response != null && response.Data != null && response.Data.Value == true)
                {
                    RemoveRosterPositionError(model.BubbleSheetErrorId, model.RosterPosition);
                }
            }
            catch (WebException)
            {
                //return Json(false);
            }

            return RedirectToAction("ProcessErrors");
        }

        [UploadifyPrincipal(Order = 1)]
        public ActionResult SubmitNewSheetForError(HttpPostedFileBase postedFile, int bubbleSheetErrorId)
        {
            var readRequest = CreateReadRequest(postedFile);
            string errorMessage;
            BubbleSheetWsHelper.CreateReadRequest(readRequest, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return Json(false);
            }
            RemoveBubbleSheetError(bubbleSheetErrorId);
            return Json(true);
        }

        public ActionResult SubmitCorrectNumberOfQuestionsError(TestQuestionsViewModel model)
        {
            var bubbleSheetItem = parameters.BubbleSheetErrorService.GetBubbleSheetErrorById(model.BubbleSheetErrorId);
            model.RelatedImage = bubbleSheetItem.RelatedImage;
            model.RosterPosition = bubbleSheetItem.RosterPosition;
            var bubbleSheetFile = CreateNewBubbleSheetFile(model);
            //var bubbleSheetCloudEntity = parameters.ReadResultService.CreateNewBubbleSheetFileCloudEntity(bubbleSheetFile);
            var bubbleSheetCloudEntity = parameters.ReadResultService.CreateNewBubbleSheetFile(bubbleSheetFile);

            //parameters.ReadResultService.SetBubbleSheetCloudEntityQuestions(bubbleSheetCloudEntity, model.QuestionCount);
            parameters.ReadResultService.SetBubbleSheetQuestions(bubbleSheetCloudEntity, model.QuestionCount);

            //parameters.TestResubmissionService.AssignNewQuestions(model.UnansweredQuestionAnswers, bubbleSheetCloudEntity.Value);
            parameters.TestResubmissionService.AssignNewQuestions(model.UnansweredQuestionAnswers, bubbleSheetCloudEntity);

            //parameters.TestResultService.Update(bubbleSheetCloudEntity);
            //var request = parameters.ReadResultService.CreateGradeRequest(bubbleSheetCloudEntity);
            //parameters.GradeRequestService.SendGradeRequest(request);
            var apiResponse = BubbleSheetWsHelper.SubmitCorrectNumberOfQuestionsError(bubbleSheetCloudEntity);
            if (apiResponse != null && apiResponse.Data != null && apiResponse.Data.Value == true)
            {
                RemoveBubbleSheetError(model.BubbleSheetErrorId);
            }
            return Json(true);
        }

        [HttpGet]
        public ActionResult GetStudentsByBubbleSheetId(int bubbleSheetId)
        {
            var bubbleSheet = BubbleSheetService.GetBubbleSheetById(bubbleSheetId);
            if (bubbleSheet.IsNull())
            {
                return HttpNotFound();
            }

            var students = bubbleSheet.StudentIds.Split('\n');
            var data = BubbleSheetService.GetStudentsByStudentIds(students);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveBubbleSheetError(int bubbleSheetErrorId)
        {
            parameters.BubbleSheetErrorService.CorrectBubbleSheetError(bubbleSheetErrorId);
            return Json(true);
        }

        public string DetermineErrorCorrectionView(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.BarcodeCouldNotBeRead:
                    return "CorrectBarcodeError";

                case ErrorCode.UnreadableRosterPosition:
                    return "CorrectRosterPositionError";

                case ErrorCode.CouldNotFindAnchors:
                    return "CorrectSheetReadError";

                case ErrorCode.CouldNotLocateBubbles:
                    return "CorrectSheetReadError";

                case ErrorCode.DidNotFindCorrectNumberOfQuestions:
                    return "CorrectNumberOfQuestionsError";

                case ErrorCode.DidNotFindCorrectNumberOfBubblesForQuestion:
                    return "CorrectSheetReadError";

                case ErrorCode.FileWasCorrupt:
                    return "CorrectCorruptFileError";

                case ErrorCode.FileWasUnreadable:
                    return "CorrectCorruptFileError";

                default:
                    return "ProcessErrors";
            }
        }

        public ActionResult RemoveRosterPositionError(int bubbleSheetErrorId, int rosterPosition)
        {
            var bubbleSheetFile = parameters.BubbleSheetFileService.GetBubbleSheetFileById(bubbleSheetErrorId);
            parameters.BubbleSheetFileService.RemoveRosterPositionError(bubbleSheetFile, rosterPosition);
            return Json(true);
        }

        [NonAction]
        private void GetMaxChoiceFromXlicontent(List<UnansweredQuestion> lstUnansweredQuestions)
        {
            if (lstUnansweredQuestions != null && lstUnansweredQuestions.Count > 0)
            {
                for (int i = 0; i < lstUnansweredQuestions.Count; i++)
                {
                    if (lstUnansweredQuestions[i].QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple)
                    {
                        var xmlContent = Util.BindQtiItemXmlContentFromXml(lstUnansweredQuestions[i].XmlContent);
                        if (xmlContent != null)
                            lstUnansweredQuestions[i].MaxChoice = xmlContent.MaxChoices;
                    }
                }
            }
        }

        //\
        public ActionResult ACTCorrectNumberOfQuestionsError(int? id)
        {
            var bubbleSheetError = parameters.BubbleSheetErrorService.GetBubbleSheetErrorById(id.GetValueOrDefault());
            if (bubbleSheetError.IsNull())
            {
                return HttpNotFound();
            }

            var model = ACTGetBubbleSheetErrorDetailsViewModel<TestImage>(id.GetValueOrDefault(), bubbleSheetError.ErrorCode);
            if (model.IsNull())
            {
                return RedirectToAction("ProcessErrors", "ErrorCorrection");
            }
            model.UnansweredQuestions = ACTGetUnansweredQuestionsForCorrectNumberOfQuestionsError(bubbleSheetError).ToList();
            ACTGetMaxChoiceFromXlicontent(model.UnansweredQuestions);

            return View(model);
        }

        public ActionResult ACTSubmitCorrectNumberOfQuestionsError(TestQuestionsViewModel model)
        {
            var bubbleSheetItem = parameters.BubbleSheetErrorService.GetBubbleSheetErrorById(model.BubbleSheetErrorId);
            model.RelatedImage = bubbleSheetItem.RelatedImage;
            var bubbleSheetFile = CreateNewBubbleSheetFile(model);
            var bubbleSheetCloudEntity = parameters.ReadResultService.CreateNewBubbleSheetFile(bubbleSheetFile);
            var lstUnAnswer = ACTGetUnansweredQuestionsForCorrectNumberOfQuestionsError(bubbleSheetItem).ToList();
            //parameters.ReadResultService.SetBubbleSheetQuestions(bubbleSheetCloudEntity, model.QuestionCount);
            List<UnansweredQuestionAnswer> lst = new List<UnansweredQuestionAnswer>();
            List<int> lstQuestionIds = model.UnansweredQuestionAnswers.Select(o => o.QuestionId).ToList();
            lst = lstUnAnswer.Where(o => !lstQuestionIds.Contains(o.QuestionId)).Select(o => new UnansweredQuestionAnswer()
            {
                QuestionId = o.QuestionId,
                QuestionOrder = o.QuestionOrder,
                SectionIndex = o.OrderSectionIndex,
                SectionQuestionIndex = o.OrderSectionQuestionIndex,
                SelectedAnswer = string.Empty
            }).ToList();
            lst.AddRange(model.UnansweredQuestionAnswers);
            parameters.TestResubmissionService.ACTAssignNewQuestions(lst, bubbleSheetCloudEntity);
            bubbleSheetCloudEntity.TestType = 3;
            bubbleSheetCloudEntity.ACTPageIndex = 0;
            var apiResponse = BubbleSheetWsHelper.SubmitCorrectNumberOfQuestionsError(bubbleSheetCloudEntity);
            if (apiResponse != null && apiResponse.Data != null && apiResponse.Data.Value == true)
            {
                RemoveBubbleSheetError(model.BubbleSheetErrorId);
            }
            return Json(true);
        }

        private ACTCorrectQuestionErrorViewModel ACTGetBubbleSheetErrorDetailsViewModel<T>(int id, int errorCode) where T : IFileBlob
        {
            var bubbleSheetError = parameters.BubbleSheetErrorService.GetBubbleSheetErrorFromViewByIdAndErrorCode(id, errorCode);
            if (bubbleSheetError.IsNull())
            {
                return null;
            }

            var viewModel = new ACTCorrectQuestionErrorViewModel
            {
                BubbleSheetErrorId = id,
                BubbleSheetId = bubbleSheetError.BubbleSheetId.GetValueOrDefault(),
                ErrorCode = bubbleSheetError.ErrorCode,
                RelatedImage = bubbleSheetError.RelatedImage,
                InputFileName = bubbleSheetError.FileName
            };

            viewModel = ACTSeperateRelatedImageAndImputFilePath(viewModel);
            string imageUrl = string.Empty;
            if (typeof(T) == typeof(TestImage))
            {
                imageUrl = BubbleSheetWsHelper.GetTestImageUrl(viewModel.RelatedImage.Replace("/", "_"),
                    ConfigurationManager.AppSettings["ApiKey"]);
            }
            else if (typeof(T) == typeof(TestFile))
            {
                imageUrl = BubbleSheetWsHelper.GetTestFileUrl(viewModel.RelatedImage,
                    ConfigurationManager.AppSettings["ApiKey"]);
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                viewModel.RelatedImage = imageUrl;
            }
            
            return viewModel;
        }

        public ACTCorrectQuestionErrorViewModel ACTSeperateRelatedImageAndImputFilePath(ACTCorrectQuestionErrorViewModel model)
        {
            var piecesArray = model.RelatedImage.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (piecesArray.Length.Equals(2))
            {
                model.RelatedImage = piecesArray[0];
                model.InputFilePath = piecesArray[1];
            }
            else if (piecesArray.Length.Equals(1))
            {
                model.RelatedImage = piecesArray[0];
            }
            return model;
        }

        private IEnumerable<ACTUnansweredQuestion> ACTGetUnansweredQuestionsForCorrectNumberOfQuestionsError(BubbleSheetError bubbleSheetError)
        {
            var bubbleSheet = BubbleSheetService.GetBubbleSheetById(bubbleSheetError.BubbleSheetId);
            if (bubbleSheet.IsNull() || bubbleSheet.Ticket.Equals(string.Empty) || !bubbleSheet.TestId.HasValue || !bubbleSheet.StudentId.HasValue)
            {
                return new BindingList<ACTUnansweredQuestion>();
            }
            return parameters.actAnswerQuestionService.GetUnansweredQuestionError(bubbleSheet.TestId.Value,
                bubbleSheet.Id, bubbleSheet.StudentId.Value);
        }

        private void ACTGetMaxChoiceFromXlicontent(List<ACTUnansweredQuestion> lstUnansweredQuestions)
        {
            if (lstUnansweredQuestions != null && lstUnansweredQuestions.Count > 0)
            {
                for (int i = 0; i < lstUnansweredQuestions.Count(); i++)
                {
                    if (lstUnansweredQuestions[i].QTISchemaId == (int)QtiSchemaEnum.ChoiceMultiple)
                    {
                        var xmlContent = Util.BindQtiItemXmlContentFromXml(lstUnansweredQuestions[i].XmlContent);
                        if (xmlContent != null)
                            lstUnansweredQuestions[i].MaxChoice = xmlContent.MaxChoices;
                    }
                }
            }
        }

        //\SAT
        public ActionResult SATCorrectNumberOfQuestionsError(int? id)
        {
            var bubbleSheetError = parameters.BubbleSheetErrorService.GetBubbleSheetErrorById(id.GetValueOrDefault());
            if (bubbleSheetError.IsNull())
            {
                return HttpNotFound();
            }

            var model = SATGetBubbleSheetErrorDetailsViewModel<TestImage>(id.GetValueOrDefault(), bubbleSheetError.ErrorCode);
            if (model.IsNull())
            {
                return RedirectToAction("ProcessErrors", "ErrorCorrection");
            }
            model.UnansweredQuestions = ACTGetUnansweredQuestionsForCorrectNumberOfQuestionsError(bubbleSheetError).ToList();
            ACTGetMaxChoiceFromXlicontent(model.UnansweredQuestions);

            model.ListSection = model.UnansweredQuestions.Select(o => o.VirtualSectionId).Distinct().ToList();
            return View(model);
        }

        public ActionResult SATSubmitCorrectNumberOfQuestionsError(TestQuestionsViewModel model)
        {
            var bubbleSheetItem = parameters.BubbleSheetErrorService.GetBubbleSheetErrorById(model.BubbleSheetErrorId);
            model.RelatedImage = bubbleSheetItem.RelatedImage;
            var bubbleSheetFile = CreateNewBubbleSheetFile(model);
            var bubbleSheetCloudEntity = parameters.ReadResultService.CreateNewBubbleSheetFile(bubbleSheetFile);
            var lstUnAnswer = ACTGetUnansweredQuestionsForCorrectNumberOfQuestionsError(bubbleSheetItem).ToList();
            List<UnansweredQuestionAnswer> lst = new List<UnansweredQuestionAnswer>();
            List<int> lstQuestionIds = model.UnansweredQuestionAnswers.Select(o => o.QuestionId).ToList();
            lst = lstUnAnswer.Where(o => !lstQuestionIds.Contains(o.QuestionId)).Select(o => new UnansweredQuestionAnswer()
            {
                QuestionId = o.QuestionId,
                QuestionOrder = o.QuestionOrder,
                SectionIndex = o.OrderSectionIndex,
                SectionQuestionIndex = o.OrderSectionQuestionIndex,
                SelectedAnswer = string.Empty
            }).ToList();
            lst.AddRange(model.UnansweredQuestionAnswers);
            parameters.TestResubmissionService.SATAssignNewQuestions(lst, bubbleSheetCloudEntity);
            bubbleSheetCloudEntity.TestType = 3;
            bubbleSheetCloudEntity.ACTPageIndex = 0;
            var apiResponse = BubbleSheetWsHelper.SubmitCorrectNumberOfQuestionsError(bubbleSheetCloudEntity);
            if (apiResponse != null && apiResponse.Data != null && apiResponse.Data.Value == true)
            {
                RemoveBubbleSheetError(model.BubbleSheetErrorId);
            }
            return Json(true);
        }

        private SATCorrectQuestionErrorViewModel SATGetBubbleSheetErrorDetailsViewModel<T>(int id, int errorCode) where T : IFileBlob
        {
            var bubbleSheetError = parameters.BubbleSheetErrorService.GetBubbleSheetErrorFromViewByIdAndErrorCode(id, errorCode);
            if (bubbleSheetError.IsNull())
            {
                return null;
            }

            var viewModel = new SATCorrectQuestionErrorViewModel
            {
                BubbleSheetErrorId = id,
                BubbleSheetId = bubbleSheetError.BubbleSheetId.GetValueOrDefault(),
                ErrorCode = bubbleSheetError.ErrorCode,
                RelatedImage = bubbleSheetError.RelatedImage,
                InputFileName = bubbleSheetError.FileName
            };

            viewModel = SATSeperateRelatedImageAndImputFilePath(viewModel);
            string imageUrl = string.Empty;
            if (typeof(T) == typeof(TestImage))
            {
                imageUrl = BubbleSheetWsHelper.GetTestImageUrl(viewModel.RelatedImage.Replace("/", "_"),
                    ConfigurationManager.AppSettings["ApiKey"]);
            }
            else if (typeof(T) == typeof(TestFile))
            {
                imageUrl = BubbleSheetWsHelper.GetTestFileUrl(viewModel.RelatedImage,
                    ConfigurationManager.AppSettings["ApiKey"]);
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                viewModel.RelatedImage = imageUrl;
            }
            
            return viewModel;
        }

        public SATCorrectQuestionErrorViewModel SATSeperateRelatedImageAndImputFilePath(SATCorrectQuestionErrorViewModel model)
        {
            var piecesArray = model.RelatedImage.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (piecesArray.Length.Equals(2))
            {
                model.RelatedImage = piecesArray[0];
                model.InputFilePath = piecesArray[1];
            }
            else if (piecesArray.Length.Equals(1))
            {
                model.RelatedImage = piecesArray[0];
            }
            return model;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteBubbleSheetError(string listBubbleSheetErrorIds)
        {
            try
            {
                string[] arrStrings = listBubbleSheetErrorIds.Split(';');
                foreach (var arrString in arrStrings)
                {
                    var bubbleSheetErrorId = 0;
                    if (int.TryParse(arrString, out bubbleSheetErrorId))
                    {
                        var error = parameters.BubbleSheetErrorService.GetBubbleSheetErrorViewById(bubbleSheetErrorId);
                        if (error != null)
                        {
                            if (error.Message.Equals("Roster position not found"))
                            {
                                var bubbleSheetFile = parameters.BubbleSheetFileService.GetBubbleSheetFileById(bubbleSheetErrorId);
                                if (bubbleSheetFile != null)
                                {
                                    parameters.BubbleSheetFileService.RemoveRosterPositionError(bubbleSheetFile, 0);
                                }
                            }
                            else
                            {
                                parameters.BubbleSheetErrorService.CorrectBubbleSheetError(bubbleSheetErrorId);
                            }
                        }
                    }
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(false);
            }
        }
    }
}
