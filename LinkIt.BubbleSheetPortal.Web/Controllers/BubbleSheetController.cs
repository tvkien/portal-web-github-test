using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.CommonServices;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Security;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class BubbleSheetController : BubbleSheetBaseController
    {
        private readonly BubbleSheetFileService bubbleSheetFileService;
        private readonly BubbleSheetErrorService bubbleSheetErrorService;

        public BubbleSheetController(
            BubbleSheetFileService bubbleSheetFileService,
            BubbleSheetErrorService bubbleSheetErrorService,
            BubbleSheetService bubbleSheetService,
            IValidator<BubbleSheet> bubbleSheetValidator
            , ClassService classService
            , VirtualTestService virtualTestService
            , BubbleSheetCommonService bubbleSheetCommonService
            , UserService userService)
            : base(bubbleSheetService, bubbleSheetValidator, classService, virtualTestService, bubbleSheetCommonService, userService)
        {
            this.bubbleSheetFileService = bubbleSheetFileService;
            this.bubbleSheetErrorService = bubbleSheetErrorService;
        }

        [HttpGet]
        [VersionFilter]
        public ActionResult Print(string Id)
        {
            //if (string.IsNullOrEmpty(Id))
            //{
            //    return RedirectToAction("Generate", "GenerateBubbleSheet");
            //}
            ViewBag.PrintModels = (List<PrintBubbleSheetDownloadModel>)Session["PrintBubbleSheetDownload"];
            return PartialView("Print", model: Id);
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ManagebubblesheetsGrade)]
        public ActionResult Grade()
        {
            return View();
        }

        //[HttpGet, AjaxOnly]
        [HttpGet]
        public JsonResult GetGradedStatus(string fileName, string uploadedTime)
        {
            DateTime uploadDate;
            if (DateTime.TryParse(uploadedTime, out uploadDate))
            {
                uploadDate = uploadDate.ToUniversalTime();
            }
            else
            {
                uploadDate = DateTime.UtcNow;
            }

            //var errorCount = bubbleSheetErrorService.GetBubbleSheetErrorCount(fileName, uploadDate, CurrentUser.Id);
            var errorList = bubbleSheetErrorService.GetBubbleSheetErrorByFileNameAndUserId(fileName, CurrentUser.Id);
            var errorCount = errorList.Count;
            var errorTestCount = errorList.Where(x => x.BubbleSheetId != null).Select(x => x.BubbleSheetId).Distinct().Count();

            var listErrorBubbleSheetID =
                errorList.Where(x => x.BubbleSheetId != null)
                    .Select(x => new {BubbleSheetId = x.BubbleSheetId ?? 0, x.RosterPosition})
                    .Distinct()
                    .ToList();

            var testList = bubbleSheetFileService.GetBubbleSheetTest(fileName, CurrentUser.Id).ToList(); //get BubbleSheetFile
            //var testSubList = bubbleSheetFileService.GetBubbleSheetFileSubByFileName(fileName); //get BubbleSheetFileSub
            var pageCount = bubbleSheetFileService.GetBubbleSheetFileCount(fileName, uploadDate, CurrentUser.Id);

            var testCount = testList.Count(x => listErrorBubbleSheetID.Any(
                                y => y.BubbleSheetId == x.BubbleSheetId && y.RosterPosition == x.RosterPosition) ==
                            false);

            var listGradedBubbleSheetID =
                testList.Select(x => new {x.BubbleSheetId, x.RosterPosition})
                    .Where(
                        x =>
                            listErrorBubbleSheetID.Any(
                                y => y.BubbleSheetId == x.BubbleSheetId && y.RosterPosition == x.RosterPosition) ==
                            false)
                    .Distinct()
                    .ToList();
            //var listTotalBubbleSheetID = listErrorBubbleSheetID.AddRange(listGradedBubbleSheetID.Select(x => x.BubbleSheetId)).ToList();
            var listTotalBubbleSheetIDCount = listErrorBubbleSheetID.Count + listGradedBubbleSheetID.Count;

            var totalPages = BubbleSheetWsHelper.GetTotalPages(fileName, ConfigurationManager.AppSettings["ApiKey"]);
            var totalTestPage = listTotalBubbleSheetIDCount;

            var finish = 0;

            var errorCountText = string.Format("{0} ({1})", errorTestCount, errorList.Count);
            var fileCountText = string.Format("{0} ({1})", testCount, pageCount);
            var totalPageText = string.Format("{0} ({1})", totalTestPage, totalPages);

            var totalCount = errorCount + pageCount;
            if (totalPages >= 0 && totalCount >= totalPages) finish = 1;

            return Json(new { ErrorCount = errorCountText, FileCount = fileCountText, TotalPage = totalPages, TotalPageText = totalPageText, Finish = finish }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDownloadUrl(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            //validate ticket
            var hasBubbleSheet = base.BubbleSheetService.GetBubbleSheetByTicket(id).Any();
            if (hasBubbleSheet == false)
            {
                return Json(new {notValidTicket = true}, JsonRequestBehavior.AllowGet);
            }

            //var requestResultResponse = BubbleSheetService.HandleRequestSheetResult(id, ConfigurationManager.AppSettings["ApiKey"]);
            var requestResultResponse = BubbleSheetWsHelper.HandleRequestSheetResult(id, ConfigurationManager.AppSettings["ApiKey"]);
            if (requestResultResponse != null && requestResultResponse.Data != null && string.IsNullOrEmpty(requestResultResponse.Data.DownloadUrl) == false)
            {
                var downloadUrl = requestResultResponse.Data.DownloadUrl;

                // Store download link into session
                var downloadModels = (List<PrintBubbleSheetDownloadModel>)Session["PrintBubbleSheetDownload"];
                if (downloadModels != null)
                {
                    var lastDownloadModel = downloadModels.FirstOrDefault(x => string.IsNullOrEmpty(x.DownloadUrl));
                    if (lastDownloadModel != null)
                        lastDownloadModel.DownloadUrl = downloadUrl;
                }

                return Json(new {downloadUrl}, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveErrorDownloadLink()
        {
            // Store download link into session
            var downloadModels = (List<PrintBubbleSheetDownloadModel>) Session["PrintBubbleSheetDownload"];
            if (downloadModels != null)
            {
                var lastDownloadModel = downloadModels.FirstOrDefault(x => string.IsNullOrEmpty(x.DownloadUrl));
                if (lastDownloadModel != null)
                    downloadModels.Remove(lastDownloadModel);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [UploadifyPrincipal(Order = 1)]
        public string UploadBubbleSheets(HttpPostedFileBase fileData)
        {
            UploadBubbleSheetResponse response;
            if (fileData.IsNull())
            {
                response = new UploadBubbleSheetResponse
                               {
                                   ErrorMessage = "not ok",
                                   FileName = string.Empty,
                                   IsSuccess = false
                               };
                return JsonConvert.SerializeObject(response);
            }
            var readRequest = CreateReadRequest(fileData);

            string errorMessage;
            BubbleSheetWsHelper.CreateReadRequest(readRequest, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                response = new UploadBubbleSheetResponse
                {
                    ErrorMessage = errorMessage,
                    FileName = string.Empty,
                    IsSuccess = false
                };
                return JsonConvert.SerializeObject(response);
            }
            //return "ok";
            response = new UploadBubbleSheetResponse
            {
                ErrorMessage = string.Empty,
                FileName = readRequest.Filename,
                IsSuccess = true
            };
            return JsonConvert.SerializeObject(response);
        }
    }
}
