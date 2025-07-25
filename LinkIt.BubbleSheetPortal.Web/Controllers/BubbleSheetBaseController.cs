using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using FluentValidation.Results;
using LinkIt.BubbleService.Models.Reading;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.CommonServices;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class BubbleSheetBaseController : BaseController
    {
        public readonly BubbleSheetService BubbleSheetService;
        public readonly IValidator<BubbleSheet> BubbleSheetValidator;
        public readonly ClassService ClassService;
        public readonly VirtualTestService VirtualTestService;
        public readonly UserService UserService;
        public readonly BubbleSheetCommonService BubbleSheetCommonService;

        public BubbleSheetBaseController(BubbleSheetService bubbleSheetService,IValidator<BubbleSheet> bubbleSheetValidator
            , ClassService classService
            , VirtualTestService virtualTestService
            , BubbleSheetCommonService bubbleSheetCommonService
            , UserService userService)
        {
            BubbleSheetService = bubbleSheetService;
            BubbleSheetValidator = bubbleSheetValidator;
            ClassService = classService;
            VirtualTestService = virtualTestService;
            UserService = userService;
            BubbleSheetCommonService = bubbleSheetCommonService;
        }

        public ReadRequest CreateReadRequest(HttpPostedFileBase fileData)
        {
            return new ReadRequest
            {
                ApiKey = ConfigurationManager.AppSettings["ApiKey"],
                FileStream = fileData.InputStream,
                Filename = BubbleSheetCommonService.BuildInputFileName(fileData.FileName),
                UserId = CurrentUser.Id
            };
        }

        public JsonResult CreateResponseTicket(RequestSheet test, IEnumerable<BubbleSheet> bubbleSheets, bool isManulEntry, string environmentId, string testName)
        {
            if (isManulEntry)
            {
                var ticket = Guid.NewGuid().ToString();
                var bubbleSheetList = bubbleSheets as List<BubbleSheet> ?? bubbleSheets.ToList();
                BubbleSheetService.UpdateBubbleSheetsWithTicket(bubbleSheetList, ticket);
                if (test.Template == Constanst.TemplateACT || test.Template == Constanst.TempateACTNoEssay
                    || test.Template == Constanst.TemplateNewACT || test.Template == Constanst.TempateNewACTNoEssay)
                {
                    return Json(new { Path = Url.Action("ACTPage", "BubbleSheetReviewDetails", new { id = ticket, classId = bubbleSheetList.First().ClassId, test = testName }), Success = true }, JsonRequestBehavior.AllowGet);
                }
                if (test.Template == Constanst.TemplateSAT || test.Template == Constanst.TemplateSATNoEssay
                    || test.Template == Constanst.TemplateNewSAT
                    || test.Template == Constanst.TemplateNewSATNoWriting
                    || test.Template == Constanst.TemplateNewSATWritingNoEssay)
                {
                    //TODO: create review page for SAT and modify this code
                    return
                        Json(
                            new
                            {
                                Path =
                                    Url.Action("SATPage", "BubbleSheetReviewDetails",
                                        new {id = ticket, classId = bubbleSheetList.First().ClassId, test = testName }),
                                Success = true
                            }, JsonRequestBehavior.AllowGet);
                }

                var path = string.Format("/BubbleSheetReviewDetails/Index/{0}?classId={1}&test={2}", ticket, bubbleSheetList.First().ClassId, System.Web.HttpUtility.HtmlEncode(testName));

                return Json(new { Path = path, Success = true }, JsonRequestBehavior.AllowGet);
            }

            var response = BubbleSheetWsHelper.HandleRequestSheet(test, environmentId);
            if (response != null && response.IsSuccess)
            {
                StorePrintBubbleSheetDownloadSession(bubbleSheets.ToList());
                
                BubbleSheetService.UpdateBubbleSheetsWithTicket(bubbleSheets, response.Data.Ticket);
                
                var returnUrl = string.Format("/BubbleSheet/Print/{0}", response.Data.Ticket);
                return Json(new { Path = returnUrl, Success = true, IsBubbleSheetOutsideCropMark = false }, JsonRequestBehavior.AllowGet);
            }
            else if (response != null && !response.IsSuccess)
            {
                var error = new List<ValidationFailure>
                            {
                                new ValidationFailure("GenerateFail", response.Error.ErrorMessage)
                            };
                return Json(new {ErrorList = error, Success = false, IsBubbleSheetOutsideCropMark = false}
                    , JsonRequestBehavior.AllowGet);
            }
            else
            {
                var error = new List<ValidationFailure>
                            {
                                new ValidationFailure("GenerateFail", "Unknown error.")
                            };
                return Json(new {ErrorList = error, Success = false, IsBubbleSheetOutsideCropMark = false}
                    , JsonRequestBehavior.AllowGet);
            }
        }

        private void StorePrintBubbleSheetDownloadSession(List<BubbleSheet> bubbleSheets)
        {
            if (Session != null)
            {
                var printBubbleSheetDownloadModels = new List<PrintBubbleSheetDownloadModel>();
                if (Session["PrintBubbleSheetDownload"] != null)
                {
                    printBubbleSheetDownloadModels =
                        (List<PrintBubbleSheetDownloadModel>)Session["PrintBubbleSheetDownload"];
                }

                if (printBubbleSheetDownloadModels != null)
                {
                    var printModel = new PrintBubbleSheetDownloadModel
                    {
                        GeneratedDateTime = DateTime.UtcNow
                    };

                    if (bubbleSheets != null && bubbleSheets.Any())
                    {
                        var bubbleSheet = bubbleSheets.FirstOrDefault();
                        if (bubbleSheet != null)
                        {
                            if (bubbleSheet.TestId > 0)
                            {
                                var virtualTest =
                                    VirtualTestService.Select()
                                        .FirstOrDefault(x => x.VirtualTestID == bubbleSheet.TestId);
                                if (virtualTest != null)
                                    printModel.TestName = virtualTest.Name;
                            }

                            if (bubbleSheet.ClassId > 0)
                            {
                                var classEntity =
                                    ClassService.Select().FirstOrDefault(x => x.Id == bubbleSheet.ClassId);
                                if (classEntity != null)
                                    printModel.ClassName = classEntity.Name;
                            }

                            if (bubbleSheet.UserId > 0)
                            {
                                var teacher = UserService.Select().FirstOrDefault(x => x.Id == bubbleSheet.UserId);
                                if (teacher != null)
                                    printModel.TeacherName = teacher.LastName + ", " + teacher.FirstName;
                            }
                        }
                    }

                    printBubbleSheetDownloadModels.Add(printModel);
                }

                Session["PrintBubbleSheetDownload"] = printBubbleSheetDownloadModels;
            }
        }
    }
}
