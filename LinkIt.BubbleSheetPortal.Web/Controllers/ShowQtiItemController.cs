using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Print;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Models.Enum;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
     [AjaxAwareAuthorize]
    public class ShowQtiItemController : BaseController
    {
        private readonly ShowQtiItemControllerParameters _parameters;
        private readonly IS3Service _s3Service;

        public ShowQtiItemController(ShowQtiItemControllerParameters parameters, IS3Service s3Service)
        {
            _parameters = parameters;
            _s3Service = s3Service;
        }

        public ActionResult Index(int? virtualQuestionId)
        {
            ViewBag.PassageList = new List<int>();
            if (!virtualQuestionId.HasValue || !User.Identity.IsAuthenticated)
            {
                ViewBag.MessageInvaldVirualQuestion = "Unable to display this item";
                return View();
            }

            if (!_parameters.QTIITemServices.CheckShowQtiItem(CurrentUser.Id, virtualQuestionId.Value,
                CurrentUser.DistrictId ?? 0))
            {
                ViewBag.MessageInvaldVirualQuestion = "Unable to display this item";
                return View();
            }

            int qtiItemId = 0;
            var vQuestion = _parameters.VirtualQuestionServices.Select().FirstOrDefault(o => o.VirtualQuestionID == virtualQuestionId.GetValueOrDefault());
            if (vQuestion == null || !vQuestion.QTIItemID.HasValue)
            {
                ViewBag.MessageInvaldVirualQuestion = "Unable to display this item";
                return View();
            }

            // Check restriction
            // Get bankId of virtual question
            var virtualTest = _parameters.VirtualTestService.GetTestById(vQuestion.VirtualTestID);
            var isAllow = _parameters.RestrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
            {
                ModuleCode = RestrictionConstant.Module_Reporting,
                UserId = CurrentUser.Id,
                RoleId = CurrentUser.RoleId,
                DistrictId = CurrentUser.DistrictId ?? 0,
                TestId = vQuestion.VirtualTestID,
                BankId = virtualTest != null ? virtualTest.BankID : 0,
                ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test
            });

            if (!isAllow)
            {
                ViewBag.MessageInvaldVirualQuestion = "Permission to access to this item content has been restricted.";
                return View();
            }

            qtiItemId = vQuestion.QTIItemID.Value;

            ViewBag.QtiItemId = qtiItemId;

            //Get the QtiItem
            var qtiItem = _parameters.QTIITemServices.GetQtiItemById(qtiItemId);
            qtiItem.XmlContent = qtiItem.XmlContent.ReplaceWeirdCharacters();

            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            qtiItem.XmlContent = qtiItem.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
            qtiItem.XmlContent = Util.UpdateS3LinkForItemMedia(qtiItem.XmlContent);
            qtiItem.XmlContent = Util.UpdateS3LinkForPassageLink(qtiItem.XmlContent);

            var model = ItemSetPrinting.TransformXmlContentToHtml(qtiItem.XmlContent, qtiItem.UrlPath, false, _s3Service);

            model.XmlContent = model.XmlContent.RecoverXmlSpecialChars(xmlSpecialCharToken);
            // Get CSS
            ViewBag.Css = string.Empty;
            string htmlContent = string.Empty;
            string xmlContent = string.Empty;

            htmlContent = model.XmlContent;
            xmlContent = qtiItem.XmlContent.RemoveLineBreaks();
            xmlContent = xmlContent.RecoverXmlSpecialChars(xmlSpecialCharToken);

            htmlContent = Util.ReplaceTagListByTagOl(htmlContent);
            xmlContent = Util.ReplaceTagListByTagOl(xmlContent);

            ViewBag.HtmlContent = Util.ReplaceVideoTag(htmlContent);
            ViewBag.XmlContent = Util.ReplaceVideoTag(xmlContent);

            ViewBag.PassageList = Util.GetPassageList(ViewBag.XmlContent, false);

            ViewBag.IsStudent = CurrentUser.IsStudent;

            ViewBag.CorrectAnswer = qtiItem.CorrectAnswer;

            //get question group common
            var questionGroupId =
                _parameters.QuestionGroupService.GetGroupIdByVirtualQuestionId(virtualQuestionId.GetValueOrDefault());
            if (questionGroupId > 0)
            {
                var questionGroup =
                    _parameters.QuestionGroupService.GetQuestionGroupById(questionGroupId);
                if (questionGroup != null)
                    ViewBag.QuestionGroupCommon = questionGroup.XmlContent.ReplaceWeirdCharacters();
            }

            return View();
        }

        public ActionResult ShowPassageDetail(string refObjectID, string data, int? qti3pPassageId, int? qti3pSourceId, int? dataFileUploadPassageID, int? dataFileUploadTypeID)
        {
            if (refObjectID.Equals("0"))
            {
                refObjectID = string.Empty;
            }

            if (string.IsNullOrEmpty(refObjectID))
            {
                data = HttpUtility.UrlDecode(data);
            }
            else
            {
                data = string.Empty;
            }

            // data = "http://www.linkit.com/NWEA00/Production/01 Full Item Bank/04 96DPI JPG and MathML/01 ELA 96DPI JPG and MathML/Grade 01Language Arts-0/passages/3035.htm";
            var mediaModel = new Helpers.Media.MediaModel();
            var passageHtmlContent = ItemSetPrinting.GetReferenceHtml(refObjectID, data, _s3Service);

            passageHtmlContent = PassageUtil.UpdateS3LinkForPassageMedia(passageHtmlContent, mediaModel.S3Domain, mediaModel.UpLoadBucketName, mediaModel.AUVirtualTestROFolder);
            passageHtmlContent = ItemSetPrinting.AdjustXmlContentFloatImg(passageHtmlContent);
            passageHtmlContent = Util.ReplaceTagListByTagOlForPassage(passageHtmlContent, false);

            return Json(passageHtmlContent, JsonRequestBehavior.AllowGet);
        }
    }
}
