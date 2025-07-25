using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.QTIRefObject;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models.DataTable;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.PassageEditor;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class PassageController : BaseController
    {
        private readonly QTIRefObjectService _qtiPassageService;
        private readonly QTIITemService _qtiItemService;
        private readonly QtiItemRefObjectService _qtiItemRefObjectService;
        private readonly QtiItemQTI3pPassageService _qtiItemQTI3pPassageService;
        private readonly VulnerabilityService _vulnerabilityService;
        private readonly VirtualQuestionService _virtualQuestionService;
        private readonly VirtualTestService _virtualTestService;
        private readonly IS3Service _s3Service;

        public PassageController(QTIRefObjectService qtiPassageService, QTIITemService qtiItemService, QtiItemRefObjectService qtiItemRefObjectService,
            QtiItemQTI3pPassageService qtiItemQTI3pPassageService,
            VulnerabilityService vulnerabilityService, VirtualQuestionService virtualQuestionService,
            VirtualTestService virtualTestService, IS3Service s3Service)
        {
            this._qtiPassageService = qtiPassageService;
            this._qtiItemService = qtiItemService;
            this._qtiItemRefObjectService = qtiItemRefObjectService;
            this._qtiItemQTI3pPassageService = qtiItemQTI3pPassageService;
            this._vulnerabilityService = vulnerabilityService;
            _virtualQuestionService = virtualQuestionService;
            _virtualTestService = virtualTestService;
            _s3Service = s3Service;
        }
        [HttpGet]
        public ActionResult GetQtiPassageSubjects(bool isIncludeQti3p = false)
        {
            var list = _qtiPassageService.GetPassageSubjects(isIncludeQti3p);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPassage(GetQtiRefObjectFilterRequest request)
        {
            var result = SearchPassage(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnAssignedPassage(GetQtiRefObjectFilterRequest request)
        {
            var result = SearchPassage(request);
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private FormatedList SearchPassage(GetQtiRefObjectFilterRequest request)
        {
            var filter = MappingRequest(request);
            if (!string.IsNullOrEmpty(request.AssignedObjectIdList))
            {
                filter.ExcludeQTIRefObjectIDs = string.Join(",", request.AssignedObjectIdList.Split(';'));
            }

            var data = _qtiPassageService.GetQtiRefObject(filter);

            var parser = new DataTableParser<QtiPassageViewModel>();
            var totalRecord = data.FirstOrDefault()?.TotalCount ?? 0;
            var pagedResults = data.Select(x => new QtiPassageViewModel
            {
                QTIRefObjectID = x.QTIRefObjectID,
                Name = x.Name.ReplaceWeirdCharacters()
            }).AsQueryable();

            return parser.Parse(pagedResults, totalRecord);
        }

        private GetQtiRefObjectFilter MappingRequest(GetQtiRefObjectFilterRequest criteria)
        {
            var request = new GetQtiRefObjectFilter
            {
                UserId = CurrentUser.Id,
                GradeId = criteria.GradeId.HasValue && criteria.GradeId.Value == 0 ? null : criteria.GradeId,
                Subject = HttpUtility.UrlDecode(criteria.Subject),
                TextTypeId = criteria.TextTypeId.HasValue && criteria.TextTypeId.Value == 0 ? null : criteria.TextTypeId,
                TextSubTypeId = criteria.TextSubTypeId.HasValue && criteria.TextSubTypeId.Value == 0 ? null : criteria.TextSubTypeId,
                FleschKincaidId = criteria.FleschKincaidId.HasValue && criteria.FleschKincaidId.Value == 0 ? null : criteria.FleschKincaidId,
                Name = HttpUtility.UrlDecode(criteria.NameSearch),
                PassageNumber = criteria.PassageNumber,
                DistrictId = criteria.DistrictId ?? CurrentUser.DistrictId.GetValueOrDefault(),
                PageSize = criteria.iDisplayLength > 0 ? criteria.iDisplayLength : 20,
                StartRow = criteria.iDisplayStart,
                GeneralSearch = criteria.sSearch,
            };
            if (!string.IsNullOrWhiteSpace(criteria.sColumns) && criteria.iSortCol_0.HasValue)
            {
                var columns = criteria.sColumns.Split(',');
                request.SortColumn = columns[criteria.iSortCol_0.Value];
                request.SortDirection = string.Compare(criteria.sSortDir_0, "desc") == 0 ? "DESC" : "ASC";
            }
            return request;
        }

        [HttpGet]
        public ActionResult GetMutualAssignedPassageIdString(string qtiItemIdString)
        {
            List<int> idList = GetIdListFromIdString(qtiItemIdString);
            var data = _qtiItemService.GetAllQtiItem().Where(x => idList.Contains(x.QTIItemID)).ToList();
            var qTIRefObjectIdList = new List<int>();
            foreach (var qtiItem in data)
            {
                var qtiRefObjectIDs = ParseQtiRefObjectIDFromXmlContent(qtiItem.XmlContent);
                qTIRefObjectIdList.AddRange(qtiRefObjectIDs);
            }
            //distinct qTIRefObjectIdList
            var distinctQTIRefObjectIdList = qTIRefObjectIdList.Distinct();

            List<int> mutualQTIRefObjectId = new List<int>();
            foreach (var id in distinctQTIRefObjectIdList)
            {
                var count = data.Where(x => ParseQtiRefObjectIDFromXmlContent(x.XmlContent).Contains(id)).Count();
                if (count == idList.Count)//the mutual passage
                {
                    mutualQTIRefObjectId.Add(id);
                }
            }

            string result = string.Empty;
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                foreach (var id in mutualQTIRefObjectId)
                {
                    result += string.Format(",-{0}-", id);
                }
            }

            return Json(new { QtiItemAssignedPassageIdString = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPassageByStandardIdString(string qTIRefObjectIdString, string qtiItemIdString)
        {
            List<int> idList = new List<int>();
            if (!string.IsNullOrEmpty(qTIRefObjectIdString))
            {
                string[] idArray = qTIRefObjectIdString.Split(',');
                foreach (var id in idArray)
                {
                    if (id.Length > 0)
                    {
                        try
                        {
                            idList.Add(Int32.Parse(id.Replace("-", "")));

                        }
                        catch (Exception)
                        {
                        }

                    }
                }
            }

            var parser = new DataTableParser<QtiPassageViewModel>();
            var data = _qtiPassageService.GetAll().Where(x => idList.Contains(x.QTIRefObjectID)).Select(x => new QtiPassageViewModel
            {
                QTIRefObjectID = x.QTIRefObjectID,
                Name = x.Name.ReplaceWeirdCharacters()
            });

            List<QtiPassageViewModel> result = new List<QtiPassageViewModel>();
            result.AddRange(data.ToList());
            //Alwsay add link passage
            result.AddRange(GetMutalAssignedPassageLink(qtiItemIdString));
            return Json(parser.Parse(result.AsQueryable()), JsonRequestBehavior.AllowGet);
        }
        private List<QtiPassageViewModel> GetMutalAssignedPassageLink(string qtiItemIdString)
        {
            //Normal passage(s) are store in database QTIRefObject and QtiItem.XmlContent has tag like <object class="referenceObject" stylename="referenceObject" refObjectID="1025" />
            //Special passage(s) are only in links and QtiItem.XmlContent has tag like <object type="text/html" data="http://www.linkit.com/NWEA00/Production/01 Full Item Bank/04 96DPI JPG and MathML/01 ELA 96DPI JPG and MathML/Grade 01Language Arts-0/passages/3035.htm" class="referenceObject" stylename="referenceObject" />
            //This function will return the mutual passage(s) in link for displaying 
            var result = new List<QtiPassageViewModel>();

            List<int> idList = GetIdListFromIdString(qtiItemIdString);
            var data = _qtiItemService.GetAllQtiItem().Where(x => idList.Contains(x.QTIItemID)).ToList();
            Dictionary<string, List<int>> dictionary = new Dictionary<string, List<int>>();
            int count = 0;
            foreach (var qtiItem in data)
            {
                var links = ParseQtiRefObjectLinkFromXmlContent(qtiItem.XmlContent);
                foreach (var link in links)
                {
                    if (!string.IsNullOrEmpty(link))
                    {
                        if (dictionary.ContainsKey(link))
                        {
                            dictionary[link].Add(qtiItem.QTIItemID);
                        }
                        else
                        {
                            dictionary.Add(link, new List<int>() { qtiItem.QTIItemID });
                        }
                    }
                }

                count++;
            }

            //get the mutual link passage only
            foreach (var item in dictionary)
            {
                if (item.Value.Distinct().Count() == count)
                {
                    result.Add(new QtiPassageViewModel
                    {
                        QTIRefObjectID = 0, //Assign 0 as dump id for QtiPassageViewModel
                        Name = item.Key
                    });
                }
            }

            return result;
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignPassageForManyQtiItems(string qtiItemIdString, int qtiRefObjectID)
        {
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                var qtiItemIdList = qtiItemIdString.ParseIdsFromString();
                //avoid someone modify ajax parameter qtiItemIdString
                var authorizedQtiItemList = new List<QTIItemData>();
                if (!_vulnerabilityService.HasRightToEditQtiItems(CurrentUser, qtiItemIdList, out authorizedQtiItemList))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right to assign passage for one or more items." }, JsonRequestBehavior.AllowGet);
                }

                foreach (var qtiItem in authorizedQtiItemList)
                {
                    try
                    {
                        if (qtiItem.XmlContent == null)
                        {
                            qtiItem.XmlContent = string.Empty;
                        }
                        try
                        {
                            //Update QtiItemRefObject
                            _qtiItemRefObjectService.Assign(qtiItem.QTIItemID, qtiRefObjectID);
                        }
                        catch (Exception)
                        {
                            return Json(new
                            {
                                Success = "Fail",
                                errorMessage = "Error assigning new Passage"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        try
                        {
                            var updatedXmlContent = AddQtiRefObjectIDToXmlContent(qtiItem.XmlContent, qtiRefObjectID);
                            if (!qtiItem.XmlContent.Equals(updatedXmlContent))
                            {
                                //update xmlContent to qtiItem
                                qtiItem.XmlContent = updatedXmlContent;
                                //save qtiItem
                                _qtiItemService.UpdateQtiItem(qtiItem);

                                Util.UploadMultiVirtualTestJsonFileToS3(qtiItem.QTIItemID, _virtualQuestionService, _virtualTestService, _s3Service);
                            }
                        }
                        catch (Exception)
                        {
                            _qtiItemRefObjectService.Deassign(qtiItem.QTIItemID, qtiRefObjectID);
                            return Json(new
                            {
                                Success = "Fail",
                                errorMessage = "Error assigning new Passage"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new
                        {
                            Success = "Fail",
                            errorMessage = string.Format("Error assigning new Passage: {0}", ex.Message)
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }

        }
        private string AddQtiRefObjectIDToXmlContent(string xmlContent, int qtiRefObjectID)
        {
            //architecture of xmlContent
            //<assessmentItem><itemBody><object class="referenceObject" stylename="referenceObject" refObjectID="6206"/><object class="referenceObject" stylename="referenceObject" refObjectID="9183"/>...</itemBody></assessmentItem>
            //each passage is an <object class="referenceObject" stylename="referenceObject" refObjectID="6206"/> with refObjectID Property is qtiRefObjectID
            var doc = ServiceUtil.LoadXmlDocument(xmlContent);

            //check if there is already qtiRefObjectID for qtiItem or not
            var itemBodyNode = doc.GetElementsByTagName("itemBody")[0];
            //var objectNodes = itemBodyNode.SelectNodes("/object");//sometime it doesn't work like this
            var objectNodes = doc.GetElementsByTagName("object");
            if (objectNodes != null)
            {
                for (int i = 0; i < objectNodes.Count; i++)
                {
                    var node = objectNodes[i];

                    try
                    {
                        var existingQtiRefObjectId = Int32.Parse(node.Attributes["refObjectID"].Value);
                        if (existingQtiRefObjectId == qtiRefObjectID)
                        {
                            // No need to add
                            return xmlContent;
                        }
                    }
                    catch
                    {

                    }

                }
            }

            //There's no refObjectID like qtiRefObjectID, add one
            XmlNode newObjectNode = doc.CreateNode(XmlNodeType.Element, "object", doc.DocumentElement.NamespaceURI);
            var classAttr = doc.CreateAttribute("class");
            classAttr.Value = "referenceObject";

            var stylenameAttr = doc.CreateAttribute("stylename");
            stylenameAttr.Value = "referenceObject";

            var refObjectIDAttr = doc.CreateAttribute("refObjectID");
            refObjectIDAttr.Value = qtiRefObjectID.ToString();

            newObjectNode.Attributes.Append(classAttr);
            newObjectNode.Attributes.Append(stylenameAttr);
            newObjectNode.Attributes.Append(refObjectIDAttr);

            //itemBodyNode.AppendChild(newObjectNode);
            if (itemBodyNode.ChildNodes.Count > 0)
            {
                itemBodyNode.InsertBefore(newObjectNode, itemBodyNode.ChildNodes[0]);
            }
            else
            {
                itemBodyNode.AppendChild(newObjectNode);
            }
            var result = doc.OuterXml;
            return result;
        }

        private string RemoveQtiRefObjectIDToXmlContent(string xmlContent, int qtiRefObjectID)
        {
            //architecture of xmlContent
            //<assessmentItem><itemBody><object class="referenceObject" stylename="referenceObject" refObjectID="6206"/><object class="referenceObject" stylename="referenceObject" refObjectID="9183"/>...</itemBody></assessmentItem>
            //each passage is an <object class="referenceObject" stylename="referenceObject" refObjectID="6206"/> with refObjectID Property is qtiRefObjectID
            var doc = ServiceUtil.LoadXmlDocument(xmlContent);

            //check if there is already qtiRefObjectID for qtiItem or not
            var itemBodyNode = doc.GetElementsByTagName("itemBody")[0];
            //var objectNodes = itemBodyNode.SelectNodes("/object");//sometime it doesn't work like this
            var objectNodes = doc.GetElementsByTagName("object");
            XmlNode removedObject = null;
            if (objectNodes != null)
            {
                for (int i = 0; i < objectNodes.Count; i++)
                {
                    var node = objectNodes[i];

                    try
                    {
                        var existingQtiRefObjectId = Int32.Parse(node.Attributes["refObjectID"].Value);
                        if (existingQtiRefObjectId == qtiRefObjectID)
                        {
                            // Remove this node
                            removedObject = node;
                            break;
                        }
                    }
                    catch
                    {

                    }

                }
            }
            if (removedObject != null)
            {
                itemBodyNode.RemoveChild(removedObject);
            }
            var result = doc.OuterXml;
            return result;
        }

        private List<int> ParseQtiRefObjectIDFromXmlContent(string xmlContent)
        {
            //architecture of xmlContent
            //<assessmentItem><itemBody><object class="referenceObject" stylename="referenceObject" refObjectID="6206"/><object class="referenceObject" stylename="referenceObject" refObjectID="9183"/>...</itemBody></assessmentItem>
            //each passage is an <object class="referenceObject" stylename="referenceObject" refObjectID="6206"/> with refObjectID Property is qtiRefObjectID
            var doc = ServiceUtil.LoadXmlDocument(xmlContent);

            //check if there is already qtiRefObjectID for qtiItem or not
            var itemBodyNode = doc.GetElementsByTagName("itemBody")[0];
            var objectNodes = doc.GetElementsByTagName("object");
            List<int> result = new List<int>();
            if (objectNodes != null)
            {
                for (int i = 0; i < objectNodes.Count; i++)
                {
                    var node = objectNodes[i];

                    try
                    {
                        var qtiRefObjectId = Int32.Parse(node.Attributes["refObjectID"].Value);
                        result.Add(qtiRefObjectId);
                    }
                    catch
                    {

                    }

                }
            }

            return result;
        }
        private List<string> ParseQtiRefObjectLinkFromXmlContent(string xmlContent)
        {
            //architecture of xmlContent
            //<object type="text/html" data="http://www.linkit.com/NWEA00/Production/01 Full Item Bank/04 96DPI JPG and MathML/01 ELA 96DPI JPG and MathML/Grade 01Language Arts-0/passages/3035.htm" class="referenceObject" stylename="referenceObject" />
            List<string> result = new List<string>();
            string link = string.Empty;

            var doc = ServiceUtil.LoadXmlDocument(xmlContent);
            var objectNodes = doc.GetElementsByTagName("object");
            if (objectNodes != null)
            {
                for (int i = 0; i < objectNodes.Count; i++)
                {
                    var node = objectNodes[i];

                    try
                    {
                        link = node.Attributes["data"].Value;
                        result.Add(link);
                    }
                    catch
                    {
                    }

                }
            }

            return result;
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemovePassageForManyQtiItems(string qtiItemIdString, int qtiRefObjectID)
        {
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                var qtiItemIdList = qtiItemIdString.ParseIdsFromString();
                //avoid someone modify ajax parameter qtiItemIdString
                var authorizedQtiItemList = new List<QTIItemData>();
                if (!_vulnerabilityService.HasRightToEditQtiItems(CurrentUser, qtiItemIdList, out authorizedQtiItemList))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right to deassign passage for one or more items." }, JsonRequestBehavior.AllowGet);
                }


                foreach (var qtiItem in authorizedQtiItemList)
                {
                    try
                    {
                        //update qtiRefObjectID to XmlContent
                        if (qtiItem != null)
                        {
                            if (qtiItem.XmlContent == null)
                            {
                                qtiItem.XmlContent = string.Empty;
                            }
                            try
                            {
                                _qtiItemRefObjectService.Deassign(qtiItem.QTIItemID, qtiRefObjectID);
                            }
                            catch (Exception)
                            {

                                return
                                    Json(
                                        new
                                        {
                                            Success = "Fail",
                                            errorMessage = "Error deassigning passage"
                                        },
                                        JsonRequestBehavior.AllowGet);
                            }
                            try
                            {
                                var updatedXmlContent = RemoveQtiRefObjectIDToXmlContent(qtiItem.XmlContent, qtiRefObjectID);
                                if (!qtiItem.XmlContent.Equals(updatedXmlContent))
                                {
                                    //update xmlContent to qtiItem
                                    qtiItem.XmlContent = updatedXmlContent;
                                    //save qtiItem
                                    _qtiItemService.UpdateQtiItem(qtiItem);
                                }
                            }
                            catch (Exception)
                            {
                                _qtiItemRefObjectService.Assign(qtiItem.QTIItemID, qtiRefObjectID);
                                return
                                   Json(
                                       new
                                       {
                                           Success = "Fail",
                                           errorMessage = "Error deassigning passage"
                                       },
                                       JsonRequestBehavior.AllowGet);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return
                            Json(
                                new
                                {
                                    Success = "Fail",
                                    errorMessage =
                                string.Format("Error assigning new Passage: {0}", ex.Message)
                                },
                                JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }

        }

        private List<int> GetIdListFromIdString(string idString)
        {
            List<int> idList = new List<int>();
            if (!string.IsNullOrEmpty(idString))
            {
                string[] idArray = idString.Split(',');
                foreach (var id in idArray)
                {
                    if (id.Length > 0)
                    {
                        idList.Add(Int32.Parse(id));
                    }
                }

            }
            return idList;
        }
        public ActionResult RemovePassageLinkForManyQtiItems(string qtiItemIdString, string link)
        {
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                string[] qtiItemIds = qtiItemIdString.Split(',');
                foreach (var qtiItemId in qtiItemIds)
                {
                    try
                    {
                        //get the QtiItem
                        var qtiItem = _qtiItemService.GetQtiItemById(Int32.Parse(qtiItemId));
                        //update qtiRefObjectID to XmlContent
                        if (qtiItem != null)
                        {
                            if (qtiItem.XmlContent == null)
                            {
                                qtiItem.XmlContent = string.Empty;
                            }
                            link = HttpUtility.UrlDecode(link);
                            try
                            {
                                _qtiItemQTI3pPassageService.Deassign(qtiItem.QTIItemID, link);
                            }
                            catch (Exception)
                            {
                                return
                                    Json(
                                        new
                                        {
                                            Success = "Fail",
                                            errorMessage = "Error deassigning passage"
                                        },
                                        JsonRequestBehavior.AllowGet);
                            }
                            var updatedXmlContent = RemovePassageLinkToXmlContent(qtiItem.XmlContent, link);
                            if (!qtiItem.XmlContent.Equals(updatedXmlContent))
                            {
                                //update xmlContent to qtiItem
                                qtiItem.XmlContent = updatedXmlContent;
                                //save qtiItem
                                _qtiItemService.UpdateQtiItem(qtiItem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return
                            Json(
                                new
                                {
                                    Success = "Fail",
                                    errorMessage =
                                string.Format("Error assigning new Passage: {0}", ex.Message)
                                },
                                JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }

        }
        private string RemovePassageLinkToXmlContent(string xmlContent, string link)
        {
            //architecture of xmlContent
            //<assessmentItem><itemBody><object class="referenceObject" stylename="referenceObject" refObjectID="6206"/><object class="referenceObject" stylename="referenceObject" refObjectID="9183"/>...</itemBody></assessmentItem>
            //each passage is an <object class="referenceObject" stylename="referenceObject" refObjectID="6206"/> with refObjectID Property is qtiRefObjectID
            var doc = ServiceUtil.LoadXmlDocument(xmlContent);
            //check if there is already qtiRefObjectID for qtiItem or not
            var itemBodyNode = doc.GetElementsByTagName("itemBody")[0];
            //var objectNodes = itemBodyNode.SelectNodes("object");//sometime it does not work ?
            var objectNodes = doc.GetElementsByTagName("object");
            XmlNode removedObject = null;
            if (objectNodes != null)
            {
                for (int i = 0; i < objectNodes.Count; i++)
                {
                    var node = objectNodes[i];

                    try
                    {
                        var data = node.Attributes["data"].Value;
                        if (data.ToLower().Equals(link.ToLower()))
                        {
                            // Remove this node
                            removedObject = node;
                            break;
                        }
                    }
                    catch
                    {

                    }

                }
            }
            if (removedObject != null)
            {
                itemBodyNode.RemoveChild(removedObject);
            }
            var result = doc.OuterXml;
            return result;
        }
    }
}
