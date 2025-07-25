using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers.BubbleSheetAws;
using LinkIt.BubbleSheetPortal.Web.Models.BubbleSheetScrapPaper;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class BubbleSheetScrapPaperAPIController : Controller
    {
        private readonly BubbleSheetService _bubbleSheetService;

        public BubbleSheetScrapPaperAPIController(BubbleSheetService bubbleSheetService)
        {
            _bubbleSheetService = bubbleSheetService;
        }

        [HttpPost]
        public JsonResult CheckExistScrapPapers(ScrapPaperParamsViewModel model)
        {
            var scrapPaperImageNames = _bubbleSheetService.GetScrapPaperImageNames(model.StudentId, model.VirtualTestId, model.ClassId);
            return Json(new { HasScrapPaper = scrapPaperImageNames.Count > 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetScrapPapers(ScrapPaperParamsViewModel model)
        {
            var scrapPaperImageNames = _bubbleSheetService.GetScrapPaperImageNames(model.StudentId, model.VirtualTestId, model.ClassId);
            var respone = CreateAPIResponse(scrapPaperImageNames);

            return Json(new { Response = respone }, JsonRequestBehavior.AllowGet);
        }

        private BubbleSheetScrapPaperRespone CreateAPIResponse(List<string> scrapPaperImageNames)
        {
            var scrapPaperImageUrls = new List<string>();

            if (scrapPaperImageNames.Count == 0)
                return new BubbleSheetScrapPaperRespone { HasScrapPaper = false, ScrapPaperImageUrls = scrapPaperImageUrls };

            foreach (var item in scrapPaperImageNames)
            {
                var scrapPaperImageUrl = BubbleSheetWsHelper.GetTestImageUrl(item, ConfigurationManager.AppSettings["ApiKey"]);
                scrapPaperImageUrls.Add(scrapPaperImageUrl);
            }

            var result = new BubbleSheetScrapPaperRespone
            {
                HasScrapPaper = scrapPaperImageUrls.Count > 0 ? true : false,
                ScrapPaperImageUrls = scrapPaperImageUrls
            };

            return result;
        }
    }
}
