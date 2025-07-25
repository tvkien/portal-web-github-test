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
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Print;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Models.Enum;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class AnswerViewerController : BaseController
    {
        private readonly AnswerViewerControllerParameters _parameters;
        public AnswerViewerController(AnswerViewerControllerParameters parameters)
        {
            _parameters = parameters;
        }

        public ActionResult Index(AnswerViewerModel model)
        {
            var mediaModel = new MediaModel();
            var subDomain = UrlUtil.GenerateS3Subdomain(mediaModel.S3Domain, mediaModel.UpLoadBucketName);
            if (!string.IsNullOrEmpty(mediaModel.AUVirtualTestFolder))
            {
                subDomain = string.Format("{0}/{1}", subDomain.RemoveEndSlash(),
                    mediaModel.AUVirtualTestFolder.RemoveStartSlash().RemoveEndSlash());
            }

            model.S3Config = subDomain;

            if (!model.VirtualQuestionID.HasValue || !model.TestResultID.HasValue || 
                !User.Identity.IsAuthenticated)
            {
                ViewBag.MessageInvaldVirualQuestion = "Unable to display this item";
                return Json("Unable to display this item", JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Check restriction
                // Get bankId of virtual question
                var virtualQuestion = _parameters.VirtualQuestionServices.GetQuestionDataById(model.VirtualQuestionID ?? 0);

                if(virtualQuestion != null)
                {
                    var virtualTest = _parameters.VirtualTestService.GetTestById(virtualQuestion.VirtualTestID);
                    var isAllow = _parameters.RestrictionBO.IsAllowTo(new Models.RestrictionDTO.IsCheckRestrictionObjectDTO
                    {
                        ModuleCode = RestrictionConstant.Module_Reporting,
                        UserId = CurrentUser.Id,
                        RoleId = CurrentUser.RoleId,
                        DistrictId = CurrentUser.DistrictId ?? 0,
                        TestId = virtualTest.VirtualTestID,
                        BankId = virtualTest != null ? virtualTest.BankID : 0,
                        ObjectType = BubbleSheetPortal.Models.RestrictionDTO.RestrictionObjectType.Test
                    });

                    if (!isAllow)
                    {
                        ViewBag.MessageInvaldVirualQuestion = "Access to the item content is restricted";
                        return Json("Permission to access to this item content has been restricted.", JsonRequestBehavior.AllowGet);
                    }

                    //get question group common
                    var questionGroupId =
                        _parameters.QuestionGroupService.GetGroupIdByVirtualQuestionId(model.VirtualQuestionID.GetValueOrDefault());
                    if (questionGroupId > 0)
                    {
                        var questionGroup =
                            _parameters.QuestionGroupService.GetQuestionGroupById(questionGroupId);
                        if(questionGroup != null)
                            ViewBag.QuestionGroupCommon = questionGroup.XmlContent.ReplaceWeirdCharacters();
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult GetQuestionForStudent(int? virtualQuestionId)
        {
            if (!virtualQuestionId.HasValue || !User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Unable to display this item" }, JsonRequestBehavior.AllowGet);
            }

            if (!_parameters.QTIITemServices.CheckShowQtiItem(CurrentUser.Id, virtualQuestionId.Value,
                CurrentUser.DistrictId ?? 0))
            {
                return Json(new { success = false, message = "Unable to display this item" }, JsonRequestBehavior.AllowGet);
            }

            var question = _parameters.VirtualQuestionServices.Select().FirstOrDefault(o => o.VirtualQuestionID == virtualQuestionId.GetValueOrDefault());
            if (question == null || !question.QTIItemID.HasValue)
            {
                return Json(new { success = false, message = "Unable to display this item" }, JsonRequestBehavior.AllowGet);
            }

            //Get the QtiItem
            var qtiItem = _parameters.QTIITemServices.GetQtiItemById(question.QTIItemID.Value);
            qtiItem.XmlContent = Util.ReplaceWeirdCharactersCommon(qtiItem.XmlContent);
            qtiItem.XmlContent = Util.ReplaceVideoTag(qtiItem.XmlContent);
            qtiItem.XmlContent = Util.UpdateS3LinkForItemMedia(qtiItem.XmlContent);
            qtiItem.XmlContent = Util.UpdateS3LinkForPassageLink(qtiItem.XmlContent);

            var passageList = Util.GetPassageList(qtiItem.XmlContent, false);
            
            return Json(
                new {
                    success = true,
                    PassageList = passageList,
                    XmlContent = qtiItem.XmlContent,
                    PointsPossible = qtiItem.PointsPossible,
                    CorrectAnswer = qtiItem.CorrectAnswer,
                    QTISchemaID = qtiItem.QTISchemaID,
                }, JsonRequestBehavior.AllowGet);
        }
    }
}