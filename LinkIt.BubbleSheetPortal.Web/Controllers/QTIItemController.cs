using Envoc.Core.Shared.Extensions;
using Ionic.Zip;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.DataFileUpload;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DataFileUpload;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using LinkIt.BubbleSheetPortal.Web.Models.AssessmentItem;
using LinkIt.BubbleSheetPortal.Web.Print;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ServiceConsumer;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using S3Library;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    [VersionFilter]
    public class QTIItemController : BaseController
    {
        private readonly QTIItemControllerParameters _parameters;
        private readonly IS3Service _s3Service;

        public QTIItemController(QTIItemControllerParameters parameters, IS3Service s3Service)
        {
            _parameters = parameters;
            _s3Service = s3Service;
        }

        public string CssPath(string fileName)
        {
            var path = HttpContext.Server.MapPath("~/Content/themes/Print/ItemSets/");
            var result = Path.Combine(path, fileName);
            return result;
        }

        public string JSPath(string fileName)
        {
            var path = HttpContext.Server.MapPath("~/Scripts/");
            var result = Path.Combine(path, fileName);
            return result;
        }

        [HttpGet]
        public ActionResult Index(int? qtiItemGroupID, int districtId = 0)
        {
            var qtiGroupName = "";
            if (qtiItemGroupID.HasValue)
            {
                //Check permission
                var hasPermission = _parameters.QtiGroupService.HasRightToEditQtiGroup(qtiItemGroupID.Value, CurrentUser);

                if (!hasPermission)
                {
                    return RedirectToAction("Index", "ItemBank");
                }

                var qtiGroup = _parameters.QtiGroupService.GetById(qtiItemGroupID.Value);
                if (qtiGroup != null)
                {
                    qtiGroupName = qtiGroup.Name;
                }
                else
                {
                    return RedirectToAction("Index", "ItemBank");
                }
            }

            ViewBag.QTIItemGroupID = qtiItemGroupID;
            ViewBag.QtiGroupName = qtiGroupName;
            ViewBag.TestItemMediaPath = string.Empty;
            ViewBag.IsIsPublisherOrNetworkAdmin = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin;
            ViewBag.DistrictId = CurrentUser.IsCorrectDistrict(districtId) ? districtId : CurrentUser.DistrictId ?? 0;
            return PartialView("Index");
        }

        public ActionResult GetQTIItems(int? qtiItemGroupID, bool? returnEmptyData)
        {
            var parser = new DataTableParser<QTIItemDataForDataTable>();

            if (returnEmptyData.HasValue && returnEmptyData == true)
            {
                //return empty value
                var emptyData = new List<QTIItemDataForDataTable>();
                return Json(parser.Parse(emptyData.AsQueryable()), JsonRequestBehavior.AllowGet);
            }

            var emptyResult = new List<QTIItemDataForDataTable>().AsQueryable();
            if (!qtiItemGroupID.HasValue || qtiItemGroupID.Value == 0) return Json(parser.Parse(emptyResult));
            var id = qtiItemGroupID.Value;
            var userStateIdList = _parameters.StateService.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);
            var qtiItems =
                _parameters.QTIITemServices.SelectQTIItems().Where(o => o.QTIGroupID == id).OrderBy(x => x.QuestionOrder)
                    .Select(x => new QTIItemDataForDataTable
                    {
                        QTIItemID = x.QTIItemID,
                        Title = AdjustXmlContent(x.XmlContent, true),
                        QuestionOrder = x.QuestionOrder,
                        TestList = ParseTests(x.Tests),
                        ToolTip = BuildToolTip(x.QTIItemID, x.PointsPossible, x.XmlContent, x.StandardNumberList, x.TopicList, x.SkillList, x.OtherList, x.ItemTagList, userStateIdList)
                    }).ToList();

            var qtiItemIds = qtiItems.Select(x => (int?)x.QTIItemID).ToList();
            int batchSize = 2000;
            int totalItems = qtiItemIds.Count;
            var virtualQuestionOfQtiItems = new List<dynamic>();
            for (int i = 0; i < totalItems; i += batchSize)
            {
                var batch = qtiItemIds.Skip(i).Take(batchSize).ToList();
                var results = _parameters.VirtualQuestionServices.Select().Where(e => batch.Contains(e.QTIItemID)).Select(x => new { x.QTIItemID, x.IsRubricBasedQuestion }).ToList();
                virtualQuestionOfQtiItems.AddRange(results);
            }

            foreach (var item in qtiItems)
            {
                var countVirtualQuestion = virtualQuestionOfQtiItems.Count(x => x.QTIItemID == item.QTIItemID);
                var countVirtualQuestionRubric = virtualQuestionOfQtiItems.Count(x => x.QTIItemID == item.QTIItemID && x.IsRubricBasedQuestion == true);
                item.VirtualQuestionCount = countVirtualQuestion;
                item.VirtualQuestionRubricCount = countVirtualQuestionRubric;               
                item.Title = XmlUtils.RemoveAllNamespacesPrefix(item.Title);
            }

            return new LargeJsonResult {
                Data = parser.Parse(qtiItems.AsQueryable()),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        private string AdjustXmlContent(string xmlContent, bool useS3Content)
        {           
            string result = xmlContent.ReplaceWeirdCharacters();
            result = Util.ReplaceTagListByTagOl(result);
            result = ItemSetPrinting.AdjustXmlContentFloatImg(result);
            result = Util.UpdateS3LinkForItemMedia(result); //always update s3
            result = Util.UpdateS3LinkForPassageLink(result);
            result = Util.ReplaceVideoTag(result);
            return result;
        }

        private string BuildToolTip(int qtiItemId, int pointsPossible, string xmlContent, string xmlStandardNumberList, string xmlTopicList, string xmlSkillList, string xmlOtherList, string xmlItemTagList, List<int> userStateIdList)
        {
            //Build the tooltip
            /*  - Points Possible:
                - Passages: (display the name of the passages)
                - Standards: (display the number of the standards)
                - Topics: (display the number of the topics)
                - Skills: (display the number of the Skills)
                - Other: (display the number of the Others) */
            StringBuilder tooltip = new StringBuilder();
            //string htmlNewLineTab = "<br>&nbsp;&nbsp;&nbsp;&nbsp;";
            string htmlNewLineTab = "";
            int standardItemLength = 100;
            tooltip.Append(string.Format("- Points Possible: {0}", pointsPossible));

            //parse the passage
            List<string> passageNameList = new List<string>();
            try
            {
                passageNameList = Util.GetPassageNameList(xmlContent, _parameters.PassageService, _parameters.Qti3pPassageService, true);
            }
            catch (Exception)
            {
            }

            if (passageNameList.Count > 0)
            {
                tooltip.Append("<br>- Passages: ");
                tooltip.Append("<br> &nbsp;&nbsp;&nbsp;+&nbsp;" + Server.HtmlEncode(passageNameList[0]));

                for (int i = 1; i < passageNameList.Count; i++)
                {
                    tooltip.Append(string.Format("<br> &nbsp;&nbsp;&nbsp;+&nbsp;{0}{1}", htmlNewLineTab, Server.HtmlEncode(passageNameList[i])));
                }
            }

            //get standard of a qtiItem

            List<string> standardNumberList = Util.ParseStandardNumber(xmlStandardNumberList, CurrentUser.RoleId, userStateIdList);
            if (standardNumberList.Count > 0)
            {
                tooltip.Append("<br>- Standards: ");
                tooltip.Append(Server.HtmlEncode(standardNumberList[0]));
                for (int i = 1; i < standardNumberList.Count; i++)
                {
                    tooltip.Append(string.Format(", {0}{1}", htmlNewLineTab, Server.HtmlEncode(standardNumberList[i])));
                }
            }

            List<string> topicList = Util.ParseTopic(xmlTopicList);
            if (topicList.Count > 0)
            {
                tooltip.Append("<br>- Topics: ");
                tooltip.Append(Server.HtmlEncode(topicList[0]));
                for (int i = 1; i < topicList.Count; i++)
                {
                    tooltip.Append(string.Format(", {0}{1}", htmlNewLineTab, Server.HtmlEncode(topicList[i])));
                }
            }

            List<string> skillList = Util.ParseSkill(xmlSkillList);
            if (skillList.Count > 0)
            {
                tooltip.Append("<br>- Skills: ");
                tooltip.Append(Server.HtmlEncode(skillList[0]));
                for (int i = 1; i < skillList.Count; i++)
                {
                    tooltip.Append(string.Format(", {0}{1}", htmlNewLineTab, Server.HtmlEncode(skillList[i])));
                }
            }

            List<string> otherList = Util.ParseOther(xmlOtherList);
            if (otherList.Count > 0)
            {
                tooltip.Append("<br>- Other: ");
                tooltip.Append(Server.HtmlEncode(otherList[0]));
                for (int i = 1; i < otherList.Count; i++)
                {
                    tooltip.Append(string.Format(", {0}{1}", htmlNewLineTab, Server.HtmlEncode(otherList[i])));
                }
            }

            Dictionary<string, List<string>> districtTagDic = Util.ParseDistrictTag(xmlItemTagList);
            if (districtTagDic.Count > 0)
            {
                tooltip.Append("<br>" + LabelHelper.DistrictLabel + " Tag: ");
                foreach (var category in districtTagDic)
                {
                    tooltip.Append(string.Format("<br>- {0}: ", Server.HtmlEncode(category.Key)));
                    if (category.Value != null && category.Value.Count > 0)
                    {
                        tooltip.Append(Server.HtmlEncode(category.Value[0]));
                        for (int i = 1; i < category.Value.Count; i++)
                        {
                            tooltip.Append(string.Format(", {0}{1}", htmlNewLineTab, Server.HtmlEncode(category.Value[i])));
                        }
                    }
                }
            }
            return tooltip.ToString();
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DuplicateQTIItem(int? qtiItemID)
        {
            if (!qtiItemID.HasValue || qtiItemID.Value == 0) return Json(new { success = "true" });
            //Duplicate qtiitem within an Item set, so there's no need to clone media files
            _parameters.QTIITemServices.DuplicateQTIItem(CurrentUser.Id, qtiItemID.Value, null, null, null, null, null);

            return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [UrlReturnDecode]
        [AjaxOnly]
        public ActionResult UpdateQuestionOrder(int id, int fromPosition, int toPosition)
        {
            var currentItem = _parameters.QTIITemServices.GetQtiItemById(id);
            if (currentItem != null)
            {
                var listItem =
                    _parameters.QTIITemServices.GetQtiItemByQtiBankId(currentItem.QTIGroupID)
                        .OrderBy(x => x.QuestionOrder).ToList();
                var index1 = fromPosition - 1;
                var index2 = toPosition - 1;
                if (index1 >= 0 && index2 >= 0)
                {
                    listItem.RemoveAt(index1);
                    listItem.Insert(index2, currentItem);
                    for (int i = 0; i < listItem.Count; i++)
                    {
                        listItem[i].QuestionOrder = i + 1;
                        _parameters.QTIITemServices.UpdateQtiItem(listItem[i]);
                    }
                }
            }
            return Json(true);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteQtiItem(int qtiItemId)
        {
            if (qtiItemId > 0)
            {
                try
                {
                    //avoid someone modify ajax parameter qtiItemId
                    var qtiItem = _parameters.QtiItemService.GetQtiItemById(qtiItemId);
                    if (qtiItem == null)
                    {
                        return Json(new { success = "false", errorMessage = "Item does not exist." }, JsonRequestBehavior.AllowGet);
                    }
                    if (!_parameters.QtiGroupService.HasRightToEditQtiGroup(qtiItem.QTIGroupID, CurrentUser))
                    {
                        return Json(new { success = "false", errorMessage = "Has no right to delete." }, JsonRequestBehavior.AllowGet);
                    }

                    string result = _parameters.QTIITemServices.Delete(qtiItemId, CurrentUser.Id);
                    if (string.IsNullOrEmpty(result))
                    {
                        return Json(new { success = "false", errorMessage = "Can not delete item right now." }, JsonRequestBehavior.AllowGet);
                    }
                    else if (result.Equals("QTIItem deleted"))
                    {
                        return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = "false", errorMessage = result }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    return Json(new { success = "false", errorMessage = "Can not delete, error detail: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = "false" }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        public ActionResult CreateVirtualTestIndex(int? qtiItemGroupID)
        {
            ViewBag.QTIItemGroupID = qtiItemGroupID;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            if (!CurrentUser.IsPublisher)
            {
                ViewBag.StateId = CurrentUser.StateId;
                ViewBag.DistrictId = CurrentUser.DistrictId;
            }
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.IsIsPublisherOrNetworkAdmin = CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin;
            return PartialView("_CreateVirtualTestIndex");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateVirtualTest(CreateVirtualTestModel model)
        {
            model.BankName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(model.BankName));
            model.VirtualTestName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(model.VirtualTestName));
            if (!model.QTIItemGroupID.HasValue || model.QTIItemGroupID.Value == 0) return Json(new { success = "false" });
            //check right

            if (!_parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, model.QTIItemGroupID.Value))
            {
                return Json(new { success = "false", message = "Has no right on item set." }, JsonRequestBehavior.AllowGet);
            }
            if (!model.IsExistingBank) //New bank
            {
                var subjectIds = model.SubjectIDs.Split(',');

                var subjectId = int.Parse(subjectIds.First());
                var bank = new Bank
                {
                    Name = model.BankName,
                    CreatedByUserId = CurrentUser.Id,
                    SubjectID = subjectId,
                    BankAccessID = 1
                };
                try
                {
                    _parameters.BankServices.Save(bank);
                    model.BankID = bank.Id;
                }
                catch (Exception)
                {
                    return Json(new { success = "false", message = "Could not create test bank." }, JsonRequestBehavior.AllowGet);
                }
            }
            else //Existing bank
            {
                if (!model.BankID.HasValue)
                {
                    return Json(new { success = "false", message = "Please select a bank." });
                }
                //Check if this bank is existing or not
                var bank = _parameters.BankServices.GetBankById(model.BankID.Value);
                if (bank == null)
                {
                    return Json(new { success = false, message = "Bank does not exist or it has been deleted already." }, JsonRequestBehavior.AllowGet);
                }
                //check right on bank
                if (!_parameters.VulnerabilityService.HasRightToEditTestBank(CurrentUser, bank.Id, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { success = "false", message = "Has no right on item bank." }, JsonRequestBehavior.AllowGet);
                }
            }

            var isTestNameExist =
                _parameters.VirtualTestServices.Select()
                    .Any(o => o.Name == model.VirtualTestName && o.BankID == model.BankID);
            if (isTestNameExist)
                return
                    Json(
                        new
                        {
                            success = "false",
                            message =
                                string.Format("A test with name {0} already exists in this bank.",
                                    model.VirtualTestName)
                        });

            var virtualTest = new VirtualTestData()
            {
                Name = model.VirtualTestName,
                AuthorUserID = CurrentUser.Id,
                BankID = model.BankID.Value,
                VirtualTestType = 3,
                VirtualTestSourceID = 1,
                DatasetOriginID = (int)DataSetOriginEnum.Item_Based_Score,
                DatasetCategoryID = model.DatasetCategoryId,
                StateID = model.StateID.Value,
                CreatedDate = DateTime.UtcNow,
                TestScoreMethodID = 1,
                VirtualTestSubTypeID = 1,
                NavigationMethodID = (int)NavigationMethodEnum.NO_BRANCHING,
                IsMultipleTestResult = !GetIsOverwriteValue()
            };
            try
            {
                _parameters.VirtualTestServices.Save(virtualTest);
            }
            catch (Exception)
            {
                return Json(new { success = "false", message = "Could not create virtual test." });
            }
            // Flash does not create a real section,so remove this code
            //var virtualSection = new VirtualSection
            //{
            //    VirtualTestId = virtualTest.VirtualTestID,
            //    Order = 1
            //};
            //try
            //{
            //    _parameters.VirtualSectionService.Save(virtualSection);
            //}
            //catch (Exception)
            //{
            //    return Json(new { success = "false", message = "Could not create virtual section." });
            //}

            var id = model.QTIItemGroupID.Value;
            var qtiItems = _parameters.QTIITemServices.SelectQTIItems().Where(o => o.QTIGroupID == id).ToList();
            try
            {
                foreach (var qtiItem in qtiItems)
                {
                    var virtualQuestion = new VirtualQuestionData
                    {
                        VirtualTestID = virtualTest.VirtualTestID,
                        QTIItemID = qtiItem.QTIItemID,
                        PointsPossible = qtiItem.PointsPossible,
                        QuestionOrder = qtiItem.QuestionOrder
                    };

                    _parameters.VirtualQuestionServices.Save(virtualQuestion);

                    _parameters.QTIITemServices.TMCopyStandardsFromQTIItem(virtualQuestion.VirtualQuestionID, qtiItem.QTIItemID, model.StateID.Value);

                    _parameters.QTIITemServices.CopyConditionalLogicsFromQTIItemToNewVirtualQuestion(virtualQuestion.VirtualQuestionID, qtiItem.QTIItemID);
                    //Flash does not create a real section,Flash assigns all VirtualSectionQuestion to VirtualSectionId = 0
                    var virtualSectionQuestion = new VirtualSectionQuestion
                    {
                        Order = virtualQuestion.QuestionOrder,
                        VirtualQuestionId = virtualQuestion.VirtualQuestionID,
                        VirtualSectionId = 0
                    };
                    _parameters.VirtualSectionQuestionService.Save(virtualSectionQuestion);
                }
            }
            catch (Exception)
            {
                return Json(new { success = "false", message = "Could not create virtual question." });
            }
            //if (Util.UploadTestItemMediaToS3)//it's now alway upload to S3, no web server more
            {
                var s3VirtualTest = _parameters.VirtualTestServices.CreateS3Object(virtualTest.VirtualTestID);
                var s3Result = Util.UploadVirtualTestJsonFileToS3(s3VirtualTest, _s3Service);

                if (!s3Result.IsSuccess)
                {
                    return
                        Json(new { success = "false", message = "Update json file to S3 fail: " + s3Result.ErrorMessage });
                }
            }

            return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
        }

        private bool GetIsOverwriteValue()
        {
            var overWriteDistrictDecode = _parameters.DistrictDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(CurrentUser.DistrictId.GetValueOrDefault(), TextConstants.IS_OVERWRITE_RESULTS).FirstOrDefault();
            var value = false;

            if (overWriteDistrictDecode != null)
            {
                bool.TryParse(overWriteDistrictDecode.Value, out value);
            }
            else
            {
                var isOverwriteResults = _parameters.ConfigurationService.GetConfigurationByKey(TextConstants.IS_OVERWRITE_RESULTS);
                if (isOverwriteResults != null && !string.IsNullOrEmpty(isOverwriteResults.Value))
                    bool.TryParse(isOverwriteResults.Value, out value);
            }
            return value;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PrintQTIItemGroup(int? qtiItemGroupID)
        {
            var qtiGroup = _parameters.QtiGroupService.GetById(qtiItemGroupID.Value);
            if (qtiGroup == null)
            {
                return Json(new { success = "false", errorMessage = "There is no item set." }, JsonRequestBehavior.AllowGet);
            }
            if (!_parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, qtiGroup))
            {
                return Json(new { success = "false", errorMessage = "Has no right to print this item set." }, JsonRequestBehavior.AllowGet);
            }
            var qtiItems =
                _parameters.QTIITemServices.SelectQTIItems().Where(o => o.QTIGroupID == qtiItemGroupID).ToList();
            var useS3Content = true;
            //_parameters.DistrictDecodeService.UseS3Content(CurrentUser.DistrictId.GetValueOrDefault());
            var model = new ItemSetPrintingModel
            {
                Items = TransformToModel(qtiItems),
                TestTitle = qtiGroup.Name,
                Css =
                    System.IO.File.ReadAllText(CssPath("ItemSet.css")),
                JS =
                    System.IO.File.ReadAllText(JSPath("jquery-1.7.1.js")) +
                    System.IO.File.ReadAllText(JSPath("imagesloaded.pkgd.js"))
                    + System.IO.File.ReadAllText(JSPath("PrintTest/PrintTest.js")),
            };

            var html = ItemSetPrinting.GenerateHtml(this, model, true, _s3Service);
            var pdfData = InvokePdfGeneratorService(html, model.TestTitle);
            return Json(pdfData);
        }

        private string InvokePdfGeneratorService(string html, string testTitle)
        {
            var pdfUrl = PdfGeneratorConsumer.InvokePdfGeneratorService(html, testTitle, "ItemSets", CurrentUser.UserName);

            if (string.IsNullOrWhiteSpace(pdfUrl)) return string.Empty;

            var downloadPdfData = new DownloadPdfData { FilePath = pdfUrl, UserID = CurrentUser.Id, CreatedDate = DateTime.UtcNow };
            _parameters.DownloadPdfService.SaveDownloadPdfData(downloadPdfData);
            var downLoadUrl = Url.Action("Index", "DownloadPdf", new { pdfID = downloadPdfData.DownloadPdfID }, HelperExtensions.GetHTTPProtocal(Request));

            return downLoadUrl;
        }

        public ActionResult ShowTagPopup(string qtiItemIdString)
        {
            ViewBag.QtiItemIdString = qtiItemIdString;
            ViewBag.RoleId = CurrentUser.RoleId;
            ViewBag.DistrictId = CurrentUser.DistrictId.Value;
            ViewBag.StateId = CurrentUser.StateId;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_Tag");
        }

        public ActionResult ShowStandardPopup(int qtiItemId)
        {
            ViewBag.QtiItemId = qtiItemId;
            return PartialView("_ListMasterStandard");
        }

        [HttpGet]
        public ActionResult LoadItemListNormalView(int? qtiItemGroupID)
        {
            ViewBag.QTIItemGroupID = qtiItemGroupID;
            return PartialView("_ItemListNormalView");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetQTIItemsAnswerView(int? qtiItemGroupID)
        {
            var parser = new DataTableParser<QtiItemAnswerViewModel>();
            var emptyResult = new List<QtiItemAnswerViewModel>().AsQueryable();
            if (!qtiItemGroupID.HasValue || qtiItemGroupID.Value == 0) return Json(parser.Parse(emptyResult));

            var id = qtiItemGroupID.Value;
            var qtiItems = _parameters.QTIITemServices.SelectQTIItems().Where(o => o.QTIGroupID == id).Select(x => new QtiItemAnswerViewModel
            {
                QTIItemID = x.QTIItemID,
                QuestionOrder = x.QuestionOrder,
                CorrectAnswer = x.CorrectAnswer,
                NumberOfChoices = x.NumberOfChoices,
                PointsPossible = x.PointsPossible,
                QTISchemaID = x.QTISchemaID,
                AnswerIdentifiers = x.AnswerIdentifiers
            }).ToList();

            var qtiItemIds = qtiItems.Select(x => (int?)x.QTIItemID).ToList();
            int batchSize = 2000;
            int totalItems = qtiItemIds.Count;
            var virtualQuestionOfQtiItems = new List<dynamic>();
            for (int i = 0; i < totalItems; i += batchSize)
            {
                var batch = qtiItemIds.Skip(i).Take(batchSize).ToList();
                var results = _parameters.VirtualQuestionServices.Select().Where(e => batch.Contains(e.QTIItemID)).Select(x => new { x.QTIItemID, x.IsRubricBasedQuestion }).ToList();
                virtualQuestionOfQtiItems.AddRange(results);
            }

            foreach (var item in qtiItems)
            {
                var countVirtualQuestion = virtualQuestionOfQtiItems.Count(x => x.QTIItemID == item.QTIItemID);
                var countVirtualQuestionRubric = virtualQuestionOfQtiItems.Count(x => x.QTIItemID == item.QTIItemID && x.IsRubricBasedQuestion == true);
                item.VirtualQuestionCount = countVirtualQuestion;
                item.VirtualQuestionRubricCount = countVirtualQuestionRubric;
            }

            return Json(parser.Parse(qtiItems.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LoadItemListAnswerKeyView(int? qtiItemGroupID)
        {
            ViewBag.QTIItemGroupID = qtiItemGroupID;
            return PartialView("_ItemListAnswerKeyView");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DuplicateQTIItems(string qtiItemIdString)
        {
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                string[] qtiItemIds = qtiItemIdString.Split(',');
                foreach (var qtiItemId in qtiItemIds)
                {
                    try
                    {
                        // Duplicate qtiitem within an Item set, so there's no need to clone media files
                        var newQTIItem = _parameters.QTIITemServices.DuplicateQTIItem(CurrentUser.Id, Int32.Parse(qtiItemId),
                            null, null, null, null, null);

                        // Update Item Passage
                        if (newQTIItem != null)
                            UpdateItemPassage(newQTIItem.QTIItemID);
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new { success = "false", errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
        }

        private void UpdateItemPassage(int qtiItemId)
        {
            try
            {
                var qtiItem = _parameters.QtiItemService.GetQtiItemById(qtiItemId);
                XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
                qtiItem.XmlContent = qtiItem.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
                qtiItem.XmlContent = qtiItem.XmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();
                List<PassageViewModel> passageList = Util.GetPassageList(qtiItem.XmlContent, false);
                if (passageList != null)
                {
                    _parameters.QtiItemService.UpdateItemPassage(qtiItemId, passageList.Select(x => x.QtiRefObjectID).ToList(),
                        passageList.Select(x => x.RefNumber).ToList());
                }
            }
            catch
            {
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteQtiItems(string qtiItemIdString)
        {
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                var qtiItemIdList = qtiItemIdString.ParseIdsFromString();
                //avoid someone modify ajax parameter qtiItemIdString
                var authorizedQtiItemList = new List<QTIItemData>();
                if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser, qtiItemIdList, out authorizedQtiItemList))
                {
                    return Json(new { success = "false", errorMessage = "Has no right to delete one or more items." }, JsonRequestBehavior.AllowGet);
                }

                foreach (var qtiItemId in authorizedQtiItemList)
                {
                    try
                    {
                        string result = _parameters.QTIITemServices.Delete(qtiItemId.QTIItemID, CurrentUser.Id);
                        if (string.IsNullOrEmpty(result))
                        {
                            return Json(new { success = "false", errorMessage = "Can not delete item right now." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (!result.Equals("QTIItem deleted"))
                            {
                                return Json(new { success = "false", errorMessage = result }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new { success = "false", errorMessage = "Can not delete, error detail:" + ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateItemSet(QtiGroup objItemSet)
        {
            if (objItemSet != null)
            {
                var obj = _parameters.QtiGroupService.GetById(objItemSet.QtiGroupId);
                if (obj != null)
                {
                    obj.Name = objItemSet.Name;
                    _parameters.QtiGroupService.Save(obj);
                    return Json(true);
                }
            }
            return Json(false);
        }

        public ActionResult ShowStandardPopupForManyQtiItem(string qtiItemIdString)
        {
            ViewBag.QtiItemIdString = qtiItemIdString;
            return PartialView("_ListMasterStandardMany");
        }

        private static List<ItemModel> TransformToModel(List<QTIItemData> qtiItems)
        {
            var result = new List<ItemModel>();
            foreach (var qtiItem in qtiItems)
            {
                var item = TransformToModel(qtiItem);
                if (item == null) continue;
                result.Add(item);
            }

            return result;
        }

        private static ItemModel TransformToModel(QTIItemData qtiItem)
        {
            if (qtiItem == null) return null;
            var item = new ItemModel
            {
                QuestionOrder = qtiItem.QuestionOrder,
                XmlContent = qtiItem.XmlContent,
                Title = qtiItem.Title,
                UrlPath = qtiItem.UrlPath
            };

            var xmlContentProcessing = new XmlContentProcessing(item.XmlContent);
            xmlContentProcessing.ScaleTable(260);

            item.XmlContent = xmlContentProcessing.GetXmlContent();

            return item;
        }

        #region Parse Test from Xml to Object

        public List<VirtualTestData> ParseTests(string xmlTests)
        {
            if (string.IsNullOrWhiteSpace(xmlTests)) return new List<VirtualTestData>();
            var xdoc = XDocument.Parse(xmlTests);
            var result = new List<VirtualTestData>();

            foreach (var node in xdoc.Element("Tests").Elements("Test"))
            {
                var p = new VirtualTestData();
                p.VirtualTestID = Util.GetIntValue(node.Element("VirtualTestID"));
                p.Name = Util.GetStringValue(node.Element("Name"));
                result.Add(p);
            }
            return result;
        }

        #endregion Parse Test from Xml to Object

        #region Passage

        public ActionResult ShowPassagePopupForManyQtiItem(string qtiItemIdString="", string selectedQtiItemId = "", int? virtualTestId = 0)
        {
            ViewBag.QtiItemIdString = qtiItemIdString;
            ViewBag.SelectedQtiItemId = selectedQtiItemId;
            ViewBag.VirtualTestId = virtualTestId;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.DistrictId = CurrentUser.DistrictId;
            return PartialView("_PassageAssignForm");
        }

        [UrlReturnDecode]
        public ActionResult ShowPassageDetail(string refObjectID, string data, int? qti3pPassageId, int? qti3pSourceId, int? dataFileUploadPassageID, int? dataFileUploadTypeID, int? qtiRefObjectHistoryId, bool? isShowShuffleCheckbox = false, bool? noShuffle = false, int? virtualTestId = 0)
        {
            ViewBag.IsQti3pPassage = false;
            if (refObjectID.Equals("0"))
            {
                refObjectID = string.Empty;
                ViewBag.IsQti3pPassage = true;
            }
            if (string.IsNullOrEmpty(refObjectID))
            {
                data = HttpUtility.UrlDecode(data);
            }
            else
            {
                data = string.Empty;
            }
            //data = "http://www.linkit.com/NWEA00/Production/01 Full Item Bank/04 96DPI JPG and MathML/01 ELA 96DPI JPG and MathML/Grade 01Language Arts-0/passages/3035.htm";
            var mediaModel = new MediaModel();

            string passageHtmlContent = null;
            if (qtiRefObjectHistoryId.HasValue && qtiRefObjectHistoryId.Value > 0)
            {
                var qtiRefObjectHistory = _parameters.PassageHistoryService.GetById(qtiRefObjectHistoryId.Value);
                passageHtmlContent = qtiRefObjectHistory?.XmlContent;
            }
            else
            {
                passageHtmlContent = ItemSetPrinting.GetReferenceHtml(refObjectID, data, _s3Service);
            }

            passageHtmlContent = PassageUtil.UpdateS3LinkForPassageMedia(passageHtmlContent, mediaModel.S3Domain, mediaModel.UpLoadBucketName, mediaModel.AUVirtualTestROFolder);
            passageHtmlContent = ItemSetPrinting.AdjustXmlContentFloatImg(passageHtmlContent);
            passageHtmlContent = Util.ReplaceTagListByTagOlForPassage(passageHtmlContent, false);

            if (string.IsNullOrEmpty(passageHtmlContent))
            {
                passageHtmlContent = ItemSetPrinting.GetOriginalReferenceHtml(data);
            }
            ViewBag.PassageHtmlContent = passageHtmlContent;
            //replace the first div
            var objectID = !string.IsNullOrWhiteSpace(refObjectID) ? int.Parse(refObjectID) : 0;
            var qtiObject = _parameters.PassageService.GetById(objectID);
            if (qtiObject != null)
            {
                ViewBag.Name = string.IsNullOrEmpty(qtiObject.Name) ? "[unnamed]" : qtiObject.Name;
            }
            else
            {
                ViewBag.Name = string.Empty;
            }
            ViewBag.TestItemMediaPath = string.Empty;
            ViewBag.IsShowShuffleCheckbox = isShowShuffleCheckbox;
            ViewBag.NoShuffle = noShuffle;
            ViewBag.RefObjectID = refObjectID;
            ViewBag.VirtualTestId = virtualTestId;
            return PartialView("_PassageDetail");
        }

        #endregion Passage

        public ActionResult LoadListLinkitDefaultTagAvailablePartialView(string qtiItemIdString)
        {
            ViewBag.QtiItemIdString = qtiItemIdString;
            return PartialView("_ListLinkitDefaultTagAvailable");
        }

        public ActionResult LoadListLinkitDefaultTagAssignedPartialView(string qtiItemIdString)
        {
            ViewBag.QtiItemIdString = qtiItemIdString;
            return PartialView("_ListLinkitDefaultTagAssigned");
        }

        public ActionResult LoadListDistrictTagAvailablePartialView(string qtiItemIdString)
        {
            ViewBag.QtiItemIdString = qtiItemIdString;
            ViewBag.DistrictId = CurrentUser.DistrictId.Value;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            return PartialView("_ListDistrictTagAvailable");
        }

        public ActionResult LoadListDistrictTagAssignedPartialView(string qtiItemIdString)
        {
            ViewBag.QtiItemIdString = qtiItemIdString;
            ViewBag.DistrictId = CurrentUser.DistrictId.Value;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            return PartialView("_ListDistrictTagAssigned");
        }

        public ActionResult LoadTagLinkitDefaultPartialView(string qtiItemIdString)
        {
            ViewBag.QtiItemIdString = qtiItemIdString;
            ViewBag.DistrictId = CurrentUser.DistrictId.Value;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.HasManyQtiItem = false;
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                string[] ids = qtiItemIdString.Split(',');
                if (ids != null && ids.Count() > 1)
                {
                    ViewBag.HasManyQtiItem = true;
                }
            }
            return PartialView("_TagLinkitDefault");
        }

        [HttpPost]
        [ValidateInput(false)]
        [AjaxOnly]
        public ActionResult SaveAnswerKey(string answerKeysXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(answerKeysXml);
            //parse data xml sent from client
            var deSerializer = new XmlSerializer(typeof(QtiItemAnswerKeysXml), "http://www.imsglobal.org/xsd/imsqti_v2p0");
            QtiItemAnswerKeysXml answerKeys = null;
            bool isOK = true;
            Dictionary<int, string> errorSaving = new Dictionary<int, string>();
            try
            {
                using (TextReader reader = new StringReader(answerKeysXml))
                {
                    answerKeys = deSerializer.Deserialize(reader) as QtiItemAnswerKeysXml;
                }
            }
            catch
            {
                isOK = false;
            }
            if (isOK)
            {
                if (answerKeys != null)
                {
                    if (answerKeys.AnswerKeys != null)
                    {
                        string error;
                        int questionOrder = 0;

                        var isHavingStudentTakeTest = _parameters.QTIITemServices.IsHavingStudentTakeTest(answerKeys.AnswerKeys.Select(x => (x.QtiItemId, x.ExtendedText)));
                        if (isHavingStudentTakeTest)
                        {
                            string errorMessage = "You are attempting to change question(s) which students have already tested against. This is not permitted.";
                            return Json(new { success = "false", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
                        }

                        foreach (var answerKey in answerKeys.AnswerKeys)
                        {
                            try
                            {
                                _parameters.QTIITemServices.SaveAnswerKey(int.Parse(answerKey.QtiItemId),
                                                                          answerKey.CorrectAnswer,
                                                                          int.Parse(answerKey.NumberOfChoices),
                                                                          int.Parse(answerKey.Points),
                                                                          bool.Parse(answerKey.ExtendedText),
                                                                          out error,
                                                                          out questionOrder);
                                if (!string.IsNullOrWhiteSpace(error))
                                {
                                    errorSaving.Add(int.Parse(answerKey.QtiItemId), error);
                                }
                                if (string.IsNullOrWhiteSpace(error))
                                {
                                    //if (Util.UploadTestItemMediaToS3)//it's now alway upload to S3, no web server more
                                    {
                                        try
                                        {
                                            Util.UploadMultiVirtualTestJsonFileToS3(int.Parse(answerKey.QtiItemId), _parameters.VirtualQuestionServices, _parameters.VirtualTestServices, _s3Service);
                                        }
                                        catch (Exception ex)
                                        {
                                            PortalAuditManager.LogException(ex);
                                            errorSaving.Add(int.Parse(answerKey.QtiItemId), string.Format("Question Order {0} is saved successfully but uploading associated virtual test(s) to S3 failed. Detail error", questionOrder, ex.Message));
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                PortalAuditManager.LogException(ex);
                                errorSaving.Add(int.Parse(answerKey.QtiItemId), ex.Message);
                            }
                        }
                    }
                }
            }
            if (!errorSaving.Any())
            {
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string errorMessage = "There was some error when saving, please try again later.";
                foreach (var error in errorSaving)
                {
                    errorMessage += "<br>" + error.Value;
                }
                return Json(new { success = "false", ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadAnswerKey(HttpPostedFileBase postedFile, int qtiItemGroupId)
        {
            bool success = true;
            string errorMessage = string.Empty;
            Dictionary<int, string> errorProcess = new Dictionary<int, string>();
            List<string> lines = new List<string>();
            try
            {
                using (StreamReader reader = new StreamReader(postedFile.InputStream))
                {
                    try
                    {
                        _parameters.QTIITemServices.SaveAnswerKeyByUploadedFile(qtiItemGroupId, reader, out success, out errorMessage,
                                                                          out errorProcess);
                    }
                    catch (Exception)
                    {
                        success = false;
                    }
                }
            }
            catch (Exception)
            {
                success = false;
                errorMessage = "Can not read file " + postedFile.FileName;
            }
            if (errorProcess.Any())
            {
                errorMessage = "There was some errors.";
                foreach (var error in errorProcess)
                {
                    errorMessage += "<br>" + error.Value;
                }
                success = false;
            }

            return Json(new { success = success, errorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        [UrlReturnDecode]
        [AjaxOnly]
        public ActionResult InsertDefaultMultipleChoices(int qtiGroupId, int numberOfChoice, string correctAnswer)
        {
            string error;
            if (qtiGroupId == 0)
            {
                error = "Invalid group, can not create item!";
            }
            try
            {
                correctAnswer = correctAnswer.ToUpper();
                var qtiItemId = _parameters.QTIITemServices.InsertDefaultMultipleChoices(CurrentUser.Id, qtiGroupId, numberOfChoice, correctAnswer, out error);
                if (!string.IsNullOrWhiteSpace(error) || qtiItemId == 0)
                {
                    error = "There was some error when saving, please try again later. " + error;
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                error = "There was some error when saving, please try again later.";
            }
            if (string.IsNullOrWhiteSpace(error))
            {
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = "false", ErrorMessage = error }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Copy/Move Item

        public ActionResult ShowCopyMoveItemPopup(string qtiItemIdString)
        {
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.DistrictId = CurrentUser.DistrictId;
            ViewBag.StateId = CurrentUser.StateId;
            ViewBag.QtiItemIdString = qtiItemIdString;
            return PartialView("_CopyMoveItem");
        }

        public ActionResult LoadItemBanks()
        {
            //TODO: get list here;
            var data = _parameters.QtiBankService.GetItemBanks(CurrentUser.Id, CurrentUser.RoleId,
                                                               CurrentUser.DistrictId.GetValueOrDefault(), null, null).ToList().
                Select(o => new ListItem()
                {
                    Id = o.QTIBankId,
                    Name = o.Name
                });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult CopyMoveItems(string selectedQtiItemIds, int toQtiGroupId, bool createCopy)
        {
            if (string.IsNullOrEmpty(selectedQtiItemIds))
            {
                selectedQtiItemIds = string.Empty;
            }
            var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
            var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;

            //check the right of current user if he/she can update qti bank or not ( avoid someone modify ajax parameter toQtiGroupId )
            var itemSet = _parameters.QtiGroupService.GetById(toQtiGroupId);
            if (itemSet == null)
            {
                return Json(new { Success = "Fail", message = "Item set does not exist" }, JsonRequestBehavior.AllowGet);
            }
            if (
                   !_parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, itemSet))
            {
                return Json(new { Success = "Fail", message = "Has no right to work this item set" }, JsonRequestBehavior.AllowGet);
            }
            string action = "move";
            if (createCopy)
            {
                action = "copy";
            }

            //avoid someone modify ajax parameter selectedQtiItemIds
            var qtiItemIdList = selectedQtiItemIds.ParseIdsFromString();
            //avoid someone modify ajax parameter qtiItemIdString
            var authorizedQtiItemList = new List<QTIItemData>();
            if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser, qtiItemIdList, out authorizedQtiItemList))
            {
                return Json(new { Success = "Fail", message = string.Format("Has no right to {0} one or more items.", action) }, JsonRequestBehavior.AllowGet);
            }

            foreach (var authorizedQtiItem in authorizedQtiItemList)
            {
                try
                {
                    var qtiItemId = authorizedQtiItem.QTIItemID;
                    if (!createCopy)
                    {
                        _parameters.QTIITemServices.MoveItems(authorizedQtiItem.QTIItemID, toQtiGroupId, CurrentUser.Id, true,
                            LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName,
                            LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder,
                            LinkitConfigurationManager.GetS3Settings().S3Domain);
                    }
                    else
                    {
                        var newQTIItem = _parameters.QTIITemServices.CopyItems(authorizedQtiItem.QTIItemID, toQtiGroupId, CurrentUser.Id, true,
                             LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName,
                            LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder,
                            LinkitConfigurationManager.GetS3Settings().S3Domain);

                        // Update Item Passage
                        if (newQTIItem != null)
                            UpdateItemPassage(newQTIItem.QTIItemID);
                    }
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    var message = string.Format("There was some error. Can not {0} items now.", action);
                    return
                        Json(
                            new
                            {
                                Success = "Fail",
                                message = message
                            },
                            JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion Copy/Move Item

        [UploadifyPrincipal(Order = 1)]
        public ActionResult UploadExamview(HttpPostedFileBase postedFile, int? qtiItemGroupID)
        {
            if (!qtiItemGroupID.HasValue)
            {
                return Json(new { message = "There is no item set specified.", success = false, type = "error" },
                          JsonRequestBehavior.AllowGet);
            }
            if (!IsValidPostedFile(postedFile))
            {
                return Json(new { message = "Invalid file, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }

            try
            {
                var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
                using (ZipFile zip = ZipFile.Read(postedFile.InputStream, options))
                {
                    try
                    {
                        var examviewUploadPath = ConfigurationManager.AppSettings["ExamviewUploadPath"];
                        if (string.IsNullOrEmpty(examviewUploadPath))
                        {
                            return Json(new { message = "ExamviewUploadPath wa not configed. Please contact admin.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                        }
                        examviewUploadPath = examviewUploadPath.Replace("\\", "/");
                        if (examviewUploadPath[examviewUploadPath.Length - 1] == '/')
                        {
                            examviewUploadPath = examviewUploadPath.Substring(0, examviewUploadPath.Length - 1);//make sure examviewUploadPath does not end with '/'
                        }
                        var tempFolder = string.Format("{0}/tmp/{1}-{2}-{3}", examviewUploadPath, qtiItemGroupID.Value,
                            postedFile.FileName.Split('.')[0], Guid.NewGuid().ToString());
                        //try to create this temp folder
                        try
                        {
                            Directory.CreateDirectory(tempFolder);
                        }
                        catch
                        {
                            return Json(new { message = "Can not create temporary folder for extracting file, please contact admin.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                        }

                        try
                        {
                            zip.ExtractAll(tempFolder);
                        }
                        catch
                        {
                            return Json(new { message = "Can not extract file, please try again.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                        }

                        try
                        {
                            //process extracted files
                            var result = _parameters.QTIITemServices.ProcessExamviewUploadFiles(CurrentUser.Id,
                                qtiItemGroupID.Value, tempFolder);
                            if (!string.IsNullOrEmpty(result))
                            {
                                return
                                    Json(
                                        new
                                        {
                                            message = result,
                                            success = false,
                                            type = "error"
                                        },
                                        JsonRequestBehavior.AllowGet);
                            }

                            //upload to S3
                            //if (Util.UploadTestItemMediaToS3)//it's now alway upload to S3, no web server more
                            {
                                var imsmanifestPath = string.Format("{0}/{1}", tempFolder, "imsmanifest.xml");
                                if (System.IO.File.Exists(imsmanifestPath))
                                {
                                    var imsmanifestContent = System.IO.File.ReadAllText(imsmanifestPath);
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(imsmanifestContent);
                                    var baseurl = doc.GetElementsByTagName("resource")[0].Attributes["baseurl"].Value;

                                    var fileNodes = doc.GetElementsByTagName("file");
                                    for (int i = 0; i < fileNodes.Count; i++)
                                    {
                                        try
                                        {
                                            //get attribute href
                                            var fileLocalPath = fileNodes[i].Attributes["href"].Value;
                                            if (!string.IsNullOrWhiteSpace(fileLocalPath))
                                            {
                                                var fileFullPath = string.Format("{0}/{1}/{2}", tempFolder, baseurl, fileLocalPath);
                                                fileFullPath = fileFullPath.Replace("\\", "/");
                                                fileFullPath = fileFullPath.Replace("//", "/");
                                                var model = new MediaModel
                                                {
                                                    ID = qtiItemGroupID.Value,
                                                    FileName = fileLocalPath.Replace("\\", "/").Replace("//", "/"),
                                                    MediaType = MediaType.Image
                                                };
                                                using (FileStream fsSource = new FileStream(fileFullPath,
                                                    FileMode.Open, FileAccess.Read))
                                                {
                                                    MediaHelper.UploadTestMediaToS3(model, fsSource, _s3Service);
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            //ignore this file in S3
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            return
                                Json(
                                    new
                                    {
                                        message = "There was some errors happened, please try again.",
                                        success = false,
                                        type = "error"
                                    },
                                    JsonRequestBehavior.AllowGet);
                        }

                        var deleteTmpFiles = false;
                        var examviewUploadDeleteTemp = ConfigurationManager.AppSettings["ExamviewUploadDeleteTemp"];
                        if (!string.IsNullOrEmpty(examviewUploadDeleteTemp))
                        {
                            bool.TryParse(examviewUploadDeleteTemp, out deleteTmpFiles);
                        }

                        try
                        {
                            if (deleteTmpFiles)
                            {
                                Directory.Delete(tempFolder, true);
                            }
                        }
                        catch
                        {
                            return Json(new { message = "Your .zip file was uploaded and imported but the temporary media files can not be deleted.", success = true, type = "error" },
                          JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch
                    {
                        return Json(new { message = "There was some errors happened, please try again.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch
            {
                return Json(new { message = "Can not read file, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Your items were successfully uploaded." });
        }

        private bool IsValidPostedFile(HttpPostedFileBase file)
        {
            if (file.IsNull())
            {
                return false;
            }
            return !string.IsNullOrEmpty(file.FileName) && file.InputStream.IsNotNull();
        }

        [UrlReturnDecode]
        public ActionResult ShowPassage3pDetail(int qti3pPassageID)
        {
            //Get urlPath of passage , it must get one qti3pItem associated with the passage
            string urlPath = string.Empty;
            var qti3pItemPassage = _parameters.QTI3pItemToPassageService.GetAllQTI3pPassageById(qti3pPassageID);

            if (qti3pItemPassage != null)
            {
                urlPath = qti3pItemPassage.Fullpath;
            }

            string passageHtmlContent;
            if (string.IsNullOrEmpty(urlPath))
            {
                passageHtmlContent = string.Empty;
            }
            else
            {
                passageHtmlContent = ItemSetPrinting.GetReferenceHtml(string.Empty, urlPath, _s3Service);
            }

            if (!string.IsNullOrEmpty(passageHtmlContent))
            {
                var mediaModel = new MediaModel();
                passageHtmlContent = PassageUtil.UpdateS3LinkForPassageMedia(passageHtmlContent, mediaModel.S3Domain, mediaModel.UpLoadBucketName, mediaModel.AUVirtualTestFolder);
            }

            passageHtmlContent = ItemSetPrinting.AdjustXmlContentFloatImg(passageHtmlContent);
            passageHtmlContent = Util.ReplaceTagListByTagOlForPassage(passageHtmlContent, false);

            ViewBag.Name = string.IsNullOrEmpty(qti3pItemPassage?.PassageTitle) ? string.Empty : qti3pItemPassage.PassageTitle;

            if (string.IsNullOrEmpty(passageHtmlContent)) //try get original passage from s3.
            {
                passageHtmlContent = ItemSetPrinting.GetOriginalReferenceHtml(urlPath);
            }
            ViewBag.PassageHtmlContent = passageHtmlContent;
            ViewBag.IsQti3pPassage = true;
            ViewBag.TestItemMediaPath = string.Empty;//no use TestItemMediaPath any more
            return PartialView("_PassageDetail");
        }

        #region Data File Upload

        [UploadifyPrincipal(Order = 1)]
        [HttpPost]
        public ActionResult UploadDataFile(HttpPostedFileBase postedFile, int? qtiItemGroupID)
        {
            if (!qtiItemGroupID.HasValue)
            {
                return Json(new { message = "There is no item set specified.", success = false, type = "error" },
                          JsonRequestBehavior.AllowGet);
            }
            if (!IsValidPostedFile(postedFile))
            {
                return Json(new { message = "Invalid file, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }
            int dataFileUploadLogId = 0;
            try
            {
                var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
                using (ZipFile zip = ZipFile.Read(postedFile.InputStream, options))
                {
                    try
                    {
                        var dataFileUploadPath = LinkitConfigurationManager.AppSettings.DataFileUploadPath;
                        if (string.IsNullOrEmpty(dataFileUploadPath))
                        {
                            return Json(new { message = "DataFileUploadPath wa not configed. Please contact admin.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                        }
                        var dataFileUploadPathLocalAppServer = LinkitConfigurationManager.AppSettings.DataFileUploadPathLocalAppServer;
                        if (string.IsNullOrEmpty(dataFileUploadPathLocalAppServer))
                        {
                            return Json(new { message = "DataFileUploadPathLocalAppServer wa not configed. Please contact admin.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                        }

                        dataFileUploadPath = dataFileUploadPath.Replace("\\", "/");

                        var fileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
                        //Add timestamp to file name
                        fileName = fileName.AddTimestampToFileName();

                        var tempFolder = string.Format("{0}/tmp/{1}", dataFileUploadPath.RemoveEndSlash(), fileName);
                        //try to create this temp folder
                        try
                        {
                            Directory.CreateDirectory(tempFolder);
                        }
                        catch
                        {
                            return Json(new { message = "Can not create temporary folder for extracting file, please contact admin.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
                        }

                        try
                        {
                            zip.ExtractAll(tempFolder);
                        }
                        catch
                        {
                            return Json(new { message = "Can not extract file, please try again.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                        }
                        try
                        {
                            // Check if the ItemSet media folder exists, if not exist, create it
                            //var testItemMediaPath = ConfigurationManager.AppSettings["TestItemMediaPath"];
                            //var AUVirtualTestFolder = ConfigurationManager.AppSettings["AUVirtualTestFolder"];
                            //if (string.IsNullOrEmpty(testItemMediaPath))
                            //{
                            //    return Json(new { message = "Can not find config TestItemMediaPath, please contact Admin.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
                            //}
                            //if (string.IsNullOrEmpty(AUVirtualTestFolder))
                            //{
                            //    return Json(new { message = "Can not find config AUVirtualTestFolder, please contact Admin.", success = false, type = "error" }, JsonRequestBehavior.AllowGet);
                            //}
                            //testItemMediaPath = testItemMediaPath.Replace("\\", "/");
                            //if (testItemMediaPath[testItemMediaPath.Length - 1] == '/')
                            //{
                            //    testItemMediaPath = testItemMediaPath.Substring(0, testItemMediaPath.Length - 1);//make sure examviewUploadPath does not end with '/'
                            //}
                            //var itemSetPath = string.Format("{0}/ItemSet_{1}", testItemMediaPath, qtiItemGroupID.Value);
                            //try
                            //{
                            //    if (!Directory.Exists(itemSetPath))
                            //    {
                            //        Directory.CreateDirectory(itemSetPath);
                            //    }
                            //}
                            //catch
                            //{
                            //    return Json(new { message = string.Format("Can not create folder ItemSet_{0}, please contact Admin", qtiItemGroupID.Value), success = false, type = "error" }, JsonRequestBehavior.AllowGet);
                            //}

                            //Now storing media on S3 only
                            var itemSetPath = string.Format("ItemSet_{0}", qtiItemGroupID.Value);
                            //add to queue
                            //Change to user local path on app server

                            tempFolder = string.Format("{0}\\tmp\\{1}", dataFileUploadPathLocalAppServer.RemoveEndSlash(), fileName);
                            //itemSetPath = string.Format("{0}\\ItemSet_{1}", testItemMediaPathLocalAppServer, qtiItemGroupID.Value);

                            var dataFileUploadLog = new DataFileUploadLog()
                            {
                                CurrentUserId = CurrentUser.Id,
                                ExtractedFoler = tempFolder,
                                ItemSetPath = itemSetPath,
                                QtiGroupId = qtiItemGroupID.Value,
                                FileName = postedFile.FileName,
                                DateStart = DateTime.UtcNow,
                                DataFileUploadTypeId = 0,
                                DateEnd = DateTime.UtcNow,
                                //QTI3pSourceId = qti3pSourceId ?? 0,
                                Status = (int)DataFileUploadProcessingEnum.NotProcess // not yet processing
                            };
                            _parameters.DataFileUploadLogService.CreateDataFileUploadLog(dataFileUploadLog);

                            dataFileUploadLogId = dataFileUploadLog.DataFileUploadLogId;
                            //process extracted files
                            //var model = new MediaModel()
                            //{
                            //    ID = qtiItemGroupID.Value
                            //};

                            //DataFileUploaderParameter parameter = new DataFileUploaderParameter()
                            //{
                            //    CurrentUserId = CurrentUser.Id,
                            //    ExtractedFoler = tempFolder,
                            //    ItemSetPath = itemSetPath,
                            //    QtiGroupId = qtiItemGroupID.Value,
                            //    ZipFileName = postedFile.FileName,
                            //    UploadS3 = Util.UploadTestItemMediaToS3,
                            //    AUVirtualTestBucketName = model.UpLoadBucketName,
                            //    S3TestMedia = model.S3TestMedia,
                            //    AUVirtualTestFolder = AUVirtualTestFolder,
                            //    S3Domain = model.S3Domain
                            //};

                            //var result = _parameters.QTIITemServices.ProcessDataUploadFiles(parameter);

                            //if (!string.IsNullOrEmpty(result))
                            //{
                            //    return
                            //        Json(
                            //            new
                            //            {
                            //                message = result,
                            //                success = false,
                            //                type = "error"
                            //            },
                            //            JsonRequestBehavior.AllowGet);
                            //}
                        }
                        catch (Exception ex)
                        {
                            PortalAuditManager.LogException(ex);
                            return
                                Json(
                                    new
                                    {
                                        message = "There was some errors happened, please try again.",
                                        success = false,
                                        type = "error"
                                    },
                                    JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch
                    {
                        return Json(new { message = "There was some errors happened, please try again.", success = false, type = "error" },
                           JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch
            {
                return Json(new { message = "Can not read file, please try again.", success = false, type = "error" },
                            JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, dataFileUploadLogId = dataFileUploadLogId, JsonRequestBehavior.AllowGet });
        }

        [HttpGet]
        public ActionResult CheckStatusUploadDataFile(int dataFileUploadLogId)
        {
            var dataFileUploadLog = _parameters.DataFileUploadLogService.GetDataFileUploadLogById(dataFileUploadLogId);
            if (dataFileUploadLog != null)
            {
                if (dataFileUploadLog.Status == (int)DataFileUploadProcessingEnum.Finish)
                {
                    return Json(new { success = true, message = "Your items were successfully uploaded.", processingStatus = "finish" }, JsonRequestBehavior.AllowGet);
                }
                else if (dataFileUploadLog.Status == (int)DataFileUploadProcessingEnum.Error)
                {
                    return Json(new { success = false, message = dataFileUploadLog.Result }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion Data File Upload

        public ActionResult ShowTagPopupItemLibrary(string qtiItemIdString)
        {
            ViewBag.QtiItemIdString = qtiItemIdString;
            ViewBag.RoleId = CurrentUser.RoleId;
            ViewBag.DistrictId = CurrentUser.DistrictId.Value;
            ViewBag.StateId = CurrentUser.StateId;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_TagItemLibrary");
        }

        public ActionResult ShowPassagePopupForManyQtiItemItemLibrary(string qtiItemIdString = "", string selectedQtiItemId = "", int? virtualTestId = 0)
        {
            ViewBag.QtiItemIdString = qtiItemIdString;
            ViewBag.SelectedQtiItemId = selectedQtiItemId;
            ViewBag.VirtualTestId = virtualTestId;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.DistrictId = CurrentUser.DistrictId;
            return PartialView("_PassageAssignFormItemLibrary");
        }

        public ActionResult LoadDistrictTagsAvailablePartialView(string qtiItemIdString)
        {
            ViewBag.QtiItemIdString = qtiItemIdString;
            ViewBag.DistrictId = CurrentUser.DistrictId.Value;
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            return PartialView("_ListDistrictTagItemLibraryAvailable");
        }
    }
}
