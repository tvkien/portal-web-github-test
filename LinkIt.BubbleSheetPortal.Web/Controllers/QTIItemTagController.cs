using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models.AssessmentItem;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class QTIItemTagController : BaseController
    {
        private readonly QTIItemTagControllerParameters _parameters;

        public QTIItemTagController(QTIItemTagControllerParameters parameters)
        {
            _parameters = parameters;
        }
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignTags)]
        [UrlReturnDecode]
        public ActionResult Index(string option, int? stateId, int? districtId, string searchBoxText)
        {
            ViewBag.SelectOption = "";
            ViewBag.StateId = stateId ?? 0;
            ViewBag.DistrictId = districtId ?? 0;
            ViewBag.SearchBoxText = searchBoxText;
            if (!string.IsNullOrEmpty(option))
            {
                ViewBag.SelectOption = option;
            }
            return View();
        }
        public ActionResult GetTopics(int qtiItemId)
        {
            var parser = new DataTableParser<QTIItemTopic>();
            var data = this._parameters.QTIItemTopicService.GetAll().Where(x => x.QTIItemID == qtiItemId).Select(x => new QTIItemTopic { QTIItemTopicID = x.QTIItemTopicID, Name = x.Name, TopicId = x.TopicId });
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSkills(int qtiItemId)
        {
            var parser = new DataTableParser<QTIItemLessonOne>();
            var data = this._parameters.QTIItemLessonOneService.GetAll().Where(x => x.QTIItemID == qtiItemId).Select(x => new QTIItemLessonOne { QTIItemLessonOneID = x.QTIItemLessonOneID, Name = x.Name, LessonOneID = x.LessonOneID }); ;
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOthers(int qtiItemId)
        {
            var parser = new DataTableParser<QTIItemLessonTwo>();
            var data = this._parameters.QTIItemLessonTwoService.GetAll().Where(x => x.QTIItemID == qtiItemId).Select(x => new QTIItemLessonTwo { QTIItemLessonTwoID = x.QTIItemLessonTwoID, Name = x.Name, LessonTwoID = x.LessonTwoID }); ;
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignTopicTag(int qtiItemId, string name)
        {
            try
            {
                //check if this topic is existing for item or not
                name = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(name));
                var item = _parameters.QTIItemTopicService.GetAll().FirstOrDefault(
                    x => x.QTIItemID == qtiItemId && x.Name.ToLower().Equals(name.ToLower()));

                if (item != null)
                {
                    return Json(new { Success = "Fail", errorMessage = string.Format("Have a tag already.") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //if tag already existed, no need to insert
                    var topic = _parameters.TopicService.GetAll().Where(x => x.Name.ToLower().Equals(name.ToLower())).FirstOrDefault();
                    int topicId = 0;
                    if(topic ==null)
                    {
                        //insert topic
                        var newItem = new Topic();
                        newItem.Name = name;
                        _parameters.TopicService.Save(newItem);
                        topicId = newItem.TopicID;
                    }
                    else
                    {
                        topicId = topic.TopicID;
                    }


                    if (topicId > 0)
                    {
                        var tag = new QTIItemTopic();
                        tag.QTIItemID = qtiItemId;
                        tag.TopicId = topicId;

                        _parameters.QTIItemTopicService.Save(tag);
                        if (tag.QTIItemTopicID == 0)
                        {
                            return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting tag to question.") }, JsonRequestBehavior.AllowGet);
                        }

                        return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting new tag.") }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting new tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }


        }
        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveTopicTag(int qtiItemTopicId, int topicId)
        {
            try
            {
                //delete associationg first
                _parameters.QTIItemTopicService.Delete(qtiItemTopicId);
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error deleting tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }

        }
        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignSkillTag(int qtiItemId, string name)
        {
            try
            {
                //check if this topic is existing or not
                name = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(name));
                var item = _parameters.QTIItemLessonOneService.GetAll().FirstOrDefault(
                    x => x.QTIItemID == qtiItemId && x.Name.ToLower().Equals(name.ToLower()));
                
                if (item != null)
                {
                    return Json(new { Success = "Fail", errorMessage = string.Format("Have a tag already.") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //if there's already a skill tag, no need to insert
                    var skill = _parameters.LessonOneService.GetAll().Where(x => x.Name.ToLower().Equals(name.ToLower())).FirstOrDefault();
                    int skillId = 0;
                    if(skill==null)
                    {
                        //insert topic
                        var newItem = new LessonOne();
                        newItem.Name = name;
                        _parameters.LessonOneService.Save(newItem);
                        skillId = newItem.LessonOneID;
                    }
                    else
                    {
                        skillId = skill.LessonOneID;
                    }

                    if (skillId > 0)
                    {
                        var tag = new QTIItemLessonOne();
                        tag.QTIItemID = qtiItemId;
                        tag.LessonOneID = skillId;

                        _parameters.QTIItemLessonOneService.Save(tag);
                        if (tag.QTIItemLessonOneID == 0)
                        {
                            return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting tag to question.") }, JsonRequestBehavior.AllowGet);
                        }

                        return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting new tag.") }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting new tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }


        }
        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveSkillTag(int qtiItemLessonOneId, int lessonOneId)
        {
            try
            {
                //delete associationg first
                _parameters.QTIItemLessonOneService.Delete(qtiItemLessonOneId);
                //delete topic
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error deleting tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }

        }
        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignOtherTag(int qtiItemId, string name)
        {
            try
            {
                //check if this topic is existing or not
                name = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(name));
                var item = _parameters.QTIItemLessonTwoService.GetAll().FirstOrDefault(
                    x => x.QTIItemID == qtiItemId && x.Name.ToLower().Equals(name.ToLower()));

                if (item != null)
                {
                    return Json(new { Success = "Fail", errorMessage = string.Format("Have a tag already.") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //if there's already a other tag, no need to insert
                    var other = _parameters.LessonTwoService.GetAll().Where(x => x.Name.ToLower().Equals(name.ToLower())).FirstOrDefault();
                    int otherId = 0;
                    if(other ==null)
                    {
                        //insert topic
                        var newItem = new LessonTwo();
                        newItem.Name = name;
                        _parameters.LessonTwoService.Save(newItem);
                        otherId = newItem.LessonTwoID;
                    }
                    else
                    {
                        otherId = other.LessonTwoID;
                    }

                    if (otherId > 0)
                    {
                        var tag = new QTIItemLessonTwo();
                        tag.QTIItemID = qtiItemId;
                        tag.LessonTwoID = otherId;

                        _parameters.QTIItemLessonTwoService.Save(tag);
                        if (tag.QTIItemLessonTwoID == 0)
                        {
                            return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting tag to question.") }, JsonRequestBehavior.AllowGet);
                        }

                        return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting new tag.") }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting new tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }


        }
        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveOtherTag(int qtiItemLessonTwoId, int lessonTwoId)
        {
            try
            {
                //delete associationg first
                _parameters.QTIItemLessonTwoService.Delete(qtiItemLessonTwoId);
                //delete topic
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error deleting tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetTopicsOfManyQtiItems(string qtiItemIdString)
        {
            List<int> idList = qtiItemIdString.ParseIdsFromString();
            var parser = new DataTableParser<Topic>();
            var data = this._parameters.QTIItemTopicService.GetAll().Where(x => idList.Contains(x.QTIItemID)).Select(x => new Topic { TopicID = x.TopicId, Name = x.Name }).Distinct();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSkillsOfManyQtiItems(string qtiItemIdString)
        {
            List<int> idList = qtiItemIdString.ParseIdsFromString();
            var parser = new DataTableParser<LessonOne>();
            var data = this._parameters.QTIItemLessonOneService.GetAll().Where(x => idList.Contains(x.QTIItemID)).Select(x => new LessonOne { LessonOneID = x.LessonOneID, Name = x.Name }).Distinct(); 
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOthersOfManyQtiItems(string qtiItemIdString)
        {
            List<int> idList = qtiItemIdString.ParseIdsFromString();
            var parser = new DataTableParser<LessonTwo>();
            var data = this._parameters.QTIItemLessonTwoService.GetAll().Where(x => idList.Contains(x.QTIItemID)).Select(x => new LessonTwo { LessonTwoID = x.LessonTwoID, Name = x.Name }).Distinct();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveTopicTagForManyQtiItems(string qtiItemIdString, int topicId)
        {
            try
            {
                List<int> idList = qtiItemIdString.ParseIdsFromString();
                //avoid someone modify ajax parameter qtiItemIdString
                var authorizedQtiItemList = new List<QTIItemData>();
                if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser,idList, out authorizedQtiItemList))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right to remove topic tag for one or more items." }, JsonRequestBehavior.AllowGet);
                }
                foreach (int id in idList)
                {
                    _parameters.QTIItemTopicService.Delete(id, topicId);
                }
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error deleting tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }

        }
        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignTopicTagForManyQtiItems(string qtiItemIdString, string name)
        {

            try
            {
                var qtiItemIdList = qtiItemIdString.ParseIdsFromString();
                //avoid someone modify ajax parameter qtiItemIdString
                var authorizedQtiItemList = new List<QTIItemData>();
                if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser,qtiItemIdList, out authorizedQtiItemList))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right to assign topic tag for one or more items." }, JsonRequestBehavior.AllowGet);
                }


                name = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(name));
                //if tag already existed, no need to insert
                var topic =  _parameters.TopicService.GetAll().Where(x => x.Name.ToLower().Equals(name.ToLower())).FirstOrDefault();
                int topicId = 0;
                if (topic == null)
                {
                    //insert topic
                    var newItem = new Topic();
                    newItem.Name = name;
                    _parameters.TopicService.Save(newItem);
                    topicId = newItem.TopicID;
                }
                else
                {
                    topicId = topic.TopicID;
                }

                if (topicId > 0)
                {
                    List<int> idList = qtiItemIdString.ParseIdsFromString();
                    foreach (var qtiItem in authorizedQtiItemList)
                    {
                        //check if this topic is existing for item or not
                        var item = _parameters.QTIItemTopicService.GetAll().FirstOrDefault(x => x.QTIItemID == qtiItem.QTIItemID && x.TopicId == topicId);
                        if (item == null)
                        {
                            var tag = new QTIItemTopic();
                            tag.QTIItemID = qtiItem.QTIItemID;
                            tag.TopicId = topicId;

                            _parameters.QTIItemTopicService.Save(tag);
                            if (tag.QTIItemTopicID == 0)
                            {
                                return
                                    Json(
                                        new
                                        {
                                            Success = "Fail",
                                            errorMessage = string.Format("Error inserting tag to question.")
                                        },
                                        JsonRequestBehavior.AllowGet);
                            }
                        }
                        
                    }
                    return Json(new {Success = "Success"}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new {Success = "Fail", errorMessage = string.Format("Error inserting new tag.")},
                                JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting new tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }
        }
        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveSkillTagForManyQtiItems(string qtiItemIdString, int lessonOneId)
        {
            try
            {
                List<int> idList = qtiItemIdString.ParseIdsFromString();
                //avoid someone modify ajax parameter qtiItemIdString
                var authorizedQtiItemList = new List<QTIItemData>();
                if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser,idList, out authorizedQtiItemList))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right to remove skill tag for one or more items." }, JsonRequestBehavior.AllowGet);
                }

                foreach (int id in idList)
                {
                    _parameters.QTIItemLessonOneService.Delete(id, lessonOneId);
                }
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error deleting tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }

        }
        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignSkillTagForManyQtiItems(string qtiItemIdString, string name)
        {
            try
            {
                var qtiItemIdList = qtiItemIdString.ParseIdsFromString();
                //avoid someone modify ajax parameter qtiItemIdString
                var authorizedQtiItemList = new List<QTIItemData>();
                if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser,qtiItemIdList, out authorizedQtiItemList))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right to assign skill tag for one or more items." }, JsonRequestBehavior.AllowGet);
                }


                //if there's already a skill tag, no need to insert
                name = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(name));
                var skill =
                    _parameters.LessonOneService.GetAll().Where(x => x.Name.ToLower().Equals(name.ToLower())).FirstOrDefault();
                int skillId = 0;
                if (skill == null)
                {
                    //insert topic
                    var newItem = new LessonOne();
                    newItem.Name = name;
                    _parameters.LessonOneService.Save(newItem);
                    skillId = newItem.LessonOneID;
                }
                else
                {
                    skillId = skill.LessonOneID;
                }
                if (skillId > 0)
                {
                    foreach (var qtiItemId in qtiItemIdList)
                    {
                        var item = _parameters.QTIItemLessonOneService.GetAll().FirstOrDefault(
                            x => x.QTIItemID == qtiItemId && x.LessonOneID == skillId);
                        if (item == null)
                        {
                            var tag = new QTIItemLessonOne();
                            tag.QTIItemID = qtiItemId;
                            tag.LessonOneID = skillId;

                            _parameters.QTIItemLessonOneService.Save(tag);
                            if (tag.QTIItemLessonOneID == 0)
                            {
                                return
                                    Json(
                                        new
                                            {
                                                Success = "Fail",
                                                errorMessage = string.Format("Error inserting tag to question.")
                                            },
                                        JsonRequestBehavior.AllowGet);
                            }

                        }
                    }

                    return Json(new {Success = "Success"}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new {Success = "Fail", errorMessage = string.Format("Error inserting new tag.")},
                                JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting new tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }


        }
        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveOtherTagForManyQtiItems(string qtiItemIdString, int lessonTwoId)
        {
            try
            {
                List<int> idList = qtiItemIdString.ParseIdsFromString();
                //avoid someone modify ajax parameter qtiItemIdString
                var authorizedQtiItemList = new List<QTIItemData>();
                if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser,idList, out authorizedQtiItemList))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right to remove other tag for one or more items." }, JsonRequestBehavior.AllowGet);
                }
                foreach (int id in idList)
                {
                    _parameters.QTIItemLessonTwoService.Delete(id, lessonTwoId);
                }
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error deleting tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }

        }
        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignOtherTagForManyQtiItems(string qtiItemIdString, string name)
        {
            try
            {
                var qtiItemIdList = qtiItemIdString.ParseIdsFromString();
                //avoid someone modify ajax parameter qtiItemIdString
                var authorizedQtiItemList = new List<QTIItemData>();
                if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser,qtiItemIdList, out authorizedQtiItemList))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right to assign other tag for one or more items." }, JsonRequestBehavior.AllowGet);
                }

                name = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(name));
                //if there's already a other tag, no need to insert
                var other =
                    _parameters.LessonTwoService.GetAll().Where(x => x.Name.ToLower().Equals(name.ToLower())).
                        FirstOrDefault();
                int otherId = 0;
                if (other == null)
                {
                    //insert topic
                    var newItem = new LessonTwo();
                    newItem.Name = name;
                    _parameters.LessonTwoService.Save(newItem);
                    otherId = newItem.LessonTwoID;
                }
                else
                {
                    otherId = other.LessonTwoID;
                }
                if (otherId > 0)
                {
                    foreach (var qtiItemId in qtiItemIdList)
                    {
                        var item = _parameters.QTIItemLessonTwoService.GetAll().FirstOrDefault(
                            x => x.QTIItemID == qtiItemId && x.LessonTwoID == otherId);
                        if (item == null)
                        {
                            var tag = new QTIItemLessonTwo();
                            tag.QTIItemID = qtiItemId;
                            tag.LessonTwoID = otherId;

                            _parameters.QTIItemLessonTwoService.Save(tag);
                            if (tag.QTIItemLessonTwoID == 0)
                            {
                                return
                                    Json(
                                        new
                                            {
                                                Success = "Fail",
                                                errorMessage = string.Format("Error inserting tag to question.")
                                            },
                                        JsonRequestBehavior.AllowGet);
                            }
                        }
                    }


                    return Json(new {Success = "Success"}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new {Success = "Fail", errorMessage = string.Format("Error inserting new tag.")},
                                JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error inserting new tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult EncryptByteString(string str)
        {
            str = HttpUtility.UrlDecode(str);
            string result = Util.ConvertUtf8StringToUtf8ByteString(str);
            return Json(new { EncryptString = result},JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLinkitDefaultTags(string category, bool? filterEmpty = false)
        {
            var parser = new DataTableParser<LinkitDefaultTagListItemViewModel>();
            IEnumerable<LinkitDefaultTagListItemViewModel> linkDefault = null;
             switch (category.ToLower().Trim())
            {
                case "topic":
                    linkDefault = GetLinkitDefaultTopicTags();
                    break;
                case "skill":
                    linkDefault = GetLinkitDefaultSkillTags();
                    break;
                case "other":
                    linkDefault = GetLinkitDefaultOtherTags();
                    break;
                default:
                    linkDefault = new List<LinkitDefaultTagListItemViewModel>();
                    break;
            }
            if (filterEmpty.Value)
            {
                linkDefault = linkDefault.Where(p => p.TagName.Trim().Length > 0);
            }

            return Json(parser.Parse(linkDefault.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }
        private List<LinkitDefaultTagListItemViewModel> GetLinkitDefaultTopicTags()
        {

            var data = this._parameters.TopicService.GetAll();

            var t = data.ToList();
            var data1 = t.Select(x => new LinkitDefaultTagListItemViewModel { TagName = x.Name }).ToList();
            return data1;
        }
        private IQueryable<LinkitDefaultTagListItemViewModel> GetLinkitDefaultSkillTags()
        {
            var data = this._parameters.LessonOneService.GetAll();
            var data1 = data.Select(x => new LinkitDefaultTagListItemViewModel { TagName = x.Name });
            return data1;
        }
        private IQueryable<LinkitDefaultTagListItemViewModel> GetLinkitDefaultOtherTags()
        {
            var data = this._parameters.LessonTwoService.GetAll();
           
            var data1 = data.Select(x => new LinkitDefaultTagListItemViewModel { TagName = x.Name });
            return data1;
        }

        public ActionResult SearchLinkitDefaultTags(string category, string tagToSearch)
        {
            tagToSearch = Util.ConvertByteArrayStringToUtf8String(tagToSearch);
            tagToSearch = Util.ProcessWildCharacters(tagToSearch);

            var parser = new DataTableParser<ListItem>();

            switch (category.ToLower().Trim())
            {
                case "topic":
                    return Json(parser.Parse(SearchLinkitDefaultTopicTags(tagToSearch).AsQueryable()), JsonRequestBehavior.AllowGet);
                    break;
                case "skill":
                    return Json(parser.Parse(SearchLinkitDefaultSkillTags(tagToSearch)), JsonRequestBehavior.AllowGet);
                    break;
                case "other":
                    return Json(parser.Parse(SearchLinkitDefaultOtherTags(tagToSearch)), JsonRequestBehavior.AllowGet);
                    break;
                default:
                    List<ListItem> data = new List<ListItem>();
                    return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
            }

        }

        
        private List<ListItem> SearchLinkitDefaultTopicTags(string tagToSearch)
        {

            var data = this._parameters.TopicService.GetAll();

            if (!string.IsNullOrEmpty(tagToSearch))
            {
                data = data.Where(x => x.Name.ToLower().Contains(tagToSearch.ToLower()));
            }
            var t = data.ToList();
            var data1 = t.Select(x => new ListItem() { Id=x.TopicID,Name = x.Name }).ToList();
            return data1;
        }
        private IQueryable<ListItem> SearchLinkitDefaultSkillTags(string tagToSearch)
        {
            var data = this._parameters.LessonOneService.GetAll();
            if (!string.IsNullOrEmpty(tagToSearch))
            {
                data = data.Where(x => x.Name.ToLower().Contains(tagToSearch.ToLower()));
            }
            var data1 = data.Select(x => new ListItem {Id = x.LessonOneID,Name = x.Name });
            return data1;
        }
        private IQueryable<ListItem> SearchLinkitDefaultOtherTags(string tagToSearch)
        {
            var data =
                       this._parameters.LessonTwoService.GetAll();
            if (!string.IsNullOrEmpty(tagToSearch))
            {
                data = data.Where(x => x.Name.ToLower().Contains(tagToSearch.ToLower()));
            }
            var data1 = data.Select(x => new ListItem {Id = x.LessonTwoID,Name = x.Name });
            return data1;
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignLinkitDefaultTagForManyQtiItems(string qtiItemIdString, string linkitDefaultCagegory, string tagIdString)
        {
            switch (linkitDefaultCagegory.Trim().ToLower())
            {
                case "topic":
                    return AssignManyTopicTagForManyQtiItems(qtiItemIdString, tagIdString);
                    break;
                case "skill":
                    return AssignManySkillTagForManyQtiItems(qtiItemIdString, tagIdString);
                    break;
                case "other":
                    return AssignManyOtherTagForManyQtiItems(qtiItemIdString, tagIdString);
                    break;
                default:
                    return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                    break;
            }
        }

        private ActionResult AssignManyTopicTagForManyQtiItems(string qtiItemIdString, string tagIdString)
        {
            List<int> qtiItemIdList = qtiItemIdString.ParseIdsFromString();
            List<int> tagIdList = tagIdString.ParseIdsFromString();

            //avoid someone modify ajax parameter qtiItemIdString
            var authorizedQtiItemList = new List<QTIItemData>();
            if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser,qtiItemIdList, out authorizedQtiItemList))
            {
                return  Json(new { Success = "Fail", errorMessage = "Has no right to assign  tag for one or more items." }, JsonRequestBehavior.AllowGet);
            }

            foreach (var qtiItemId in qtiItemIdList)
            {
                foreach (var tagId in tagIdList)
                {
                    //assign each tag to each qtiItem
                    var tag = new QTIItemTopic();
                    tag.QTIItemID = qtiItemId;
                    tag.TopicId = tagId;
                    //check exists before saving
                    var exists = _parameters.QTIItemTopicService.GetAll().Any(x => x.QTIItemID == qtiItemId && x.TopicId == tagId);
                    if (!exists)
                    {
                        _parameters.QTIItemTopicService.Save(tag);
                        if (tag.QTIItemTopicID == 0)
                        {
                            return
                                Json(
                                    new
                                        {
                                            Success = "Fail",
                                            errorMessage = string.Format("Error inserting tag to question.")
                                        },
                                    JsonRequestBehavior.AllowGet);
                            //error message "Error inserting tag to question." is gotten from flash
                        }
                    }
                }
            }
            return Json(new {Success = "Success"}, JsonRequestBehavior.AllowGet);
        }
        private ActionResult AssignManySkillTagForManyQtiItems(string qtiItemIdString, string tagIdString)
        {
            List<int> qtiItemIdList = qtiItemIdString.ParseIdsFromString();
            List<int> tagIdList = tagIdString.ParseIdsFromString();

            //avoid someone modify ajax parameter qtiItemIdString
            var authorizedQtiItemList = new List<QTIItemData>();
            if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser,qtiItemIdList, out authorizedQtiItemList))
            {
                return  Json(new { Success = "Fail", errorMessage = "Has no right to assign  tag for one or more items." }, JsonRequestBehavior.AllowGet);
            }

            foreach (var qtiItemId in qtiItemIdList)
            {
                foreach (var tagId in tagIdList)
                {
                    //assign each tag to each qtiItem
                    var tag = new QTIItemLessonOne();
                    tag.QTIItemID = qtiItemId;
                    tag.LessonOneID = tagId;
                    //check exists before saving
                    var exists = _parameters.QTIItemLessonOneService.GetAll().Any(x => x.QTIItemID == qtiItemId && x.LessonOneID == tagId);
                    if (!exists)
                    {
                        _parameters.QTIItemLessonOneService.Save(tag);
                        if (tag.QTIItemLessonOneID == 0)
                        {
                            return
                                Json(
                                    new
                                    {
                                        Success = "Fail",
                                        errorMessage = string.Format("Error inserting tag to question.")
                                    },
                                    JsonRequestBehavior.AllowGet);
                            //error message "Error inserting tag to question." is gotten from flash
                        }
                    }
                }
            }
            return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
        }
        private ActionResult AssignManyOtherTagForManyQtiItems(string qtiItemIdString, string tagIdString)
        {
            List<int> qtiItemIdList = qtiItemIdString.ParseIdsFromString();
            List<int> tagIdList = tagIdString.ParseIdsFromString();
            //avoid someone modify ajax parameter qtiItemIdString
            var authorizedQtiItemList = new List<QTIItemData>();
            if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser,qtiItemIdList, out authorizedQtiItemList))
            {
                return  Json(new { Success = "Fail", errorMessage = "Has no right to assign tag for one or more items." }, JsonRequestBehavior.AllowGet);
            }
            foreach (var qtiItemId in qtiItemIdList)
            {
                foreach (var tagId in tagIdList)
                {
                    //assign each tag to each qtiItem
                    var tag = new QTIItemLessonTwo();
                    tag.QTIItemID = qtiItemId;
                    tag.LessonTwoID = tagId;
                    //check exists before saving
                    var exists = _parameters.QTIItemLessonTwoService.GetAll().Any(x => x.QTIItemID == qtiItemId && x.LessonTwoID == tagId);
                    if (!exists)
                    {
                        _parameters.QTIItemLessonTwoService.Save(tag);
                        if (tag.QTIItemLessonTwoID == 0)
                        {
                            return
                                Json(
                                    new
                                    {
                                        Success = "Fail",
                                        errorMessage = string.Format("Error inserting tag to question.")
                                    },
                                    JsonRequestBehavior.AllowGet);
                            //error message "Error inserting tag to question." is gotten from flash
                        }
                    }
                }
            }
            return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMutualAssignedLinkitDefaultTagIdString(string qtiItemIdString)
        {
            //for Topic,Skill and Other
            List<Topic> topics = _parameters.QTIItemTopicService.GetMutualTopicsOfManyQtiItems(qtiItemIdString);
            List<LessonOne> skills = _parameters.QTIItemLessonOneService.GetMutualSkillsOfManyQtiItems(qtiItemIdString);
            List<LessonTwo> others = _parameters.QTIItemLessonTwoService.GetMutualOthersOfManyQtiItems(qtiItemIdString);
            string mutualTopicIdString = string.Empty;
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                foreach (var tag in topics)
                {
                    mutualTopicIdString += string.Format(",-{0}-", tag.TopicID);
                }
            }

            string mutualSkillIdString = string.Empty;
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                foreach (var tag in skills)
                {
                    mutualSkillIdString += string.Format(",-{0}-", tag.LessonOneID);
                }
            }

            string mutualOtherIdString = string.Empty;
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                foreach (var tag in others)
                {
                    mutualOtherIdString += string.Format(",-{0}-", tag.LessonTwoID);
                }
            }

            return Json(new { MutualTopicIdString = mutualTopicIdString, MutualSkillIdString = mutualSkillIdString, MutualOtherIdString = mutualOtherIdString }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetMutualAssignedLinkitDefaultTags(string mutualTopicIdString, string mutualSkillIdString, string mutualOtherIdString)
        {
            //for Topic,Skill and Other
            List<int> topicId = mutualTopicIdString.ParseIdsFromString();
            List<Topic> topics = _parameters.TopicService.GetAll().Where(x => topicId.Contains(x.TopicID)).ToList();

            List<int> skillId = mutualSkillIdString.ParseIdsFromString();
            List<LessonOne> skills = _parameters.LessonOneService.GetAll().Where(x => skillId.Contains(x.LessonOneID)).ToList();

            List<int> otherId = mutualOtherIdString.ParseIdsFromString();
            List<LessonTwo> others = _parameters.LessonTwoService.GetAll().Where(x => otherId.Contains(x.LessonTwoID)).ToList();
            List<LinkitDefaultTagAssignedViewModel> result = new List<LinkitDefaultTagAssignedViewModel>();

            result.AddRange(topics.Select(x => new LinkitDefaultTagAssignedViewModel
            {
                LinkitDefaultTagCategory = "Topic",
                TagId = x.TopicID,
                Tag = x.Name
            }));

            result.AddRange(skills.Select(x => new LinkitDefaultTagAssignedViewModel
            {
                LinkitDefaultTagCategory = "Skill",
                TagId = x.LessonOneID,
                Tag = x.Name
            }));


            result.AddRange(others.Select(x => new LinkitDefaultTagAssignedViewModel
            {
                LinkitDefaultTagCategory = "Other",
                TagId = x.LessonTwoID,
                Tag = x.Name
            }));

            var parser = new DataTableParser<LinkitDefaultTagAssignedViewModel>();
            return Json(parser.Parse(result.AsQueryable()), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveLinkitDefaultTagForManyQtiItems(string qtiItemIdString, string linkitDefaultCategory, int tagId)
        {
            linkitDefaultCategory = linkitDefaultCategory.Trim().ToLower();
            List<int> idList = qtiItemIdString.ParseIdsFromString();
            switch (linkitDefaultCategory)
            {
                case "topic":
                    try
                    {
                        foreach (int id in idList)
                        {
                            _parameters.QTIItemTopicService.Delete(id, tagId);
                        }
                        return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new { Success = "Fail", errorMessage = string.Format("Error deleting tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case "skill": 
                    try
                    {
                        
                        foreach (int id in idList)
                        {
                            _parameters.QTIItemLessonOneService.Delete(id, tagId);
                        }
                        return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new { Success = "Fail", errorMessage = string.Format("Error deleting tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
                    }
                    break;
                case "other":
                    try
                    {

                        foreach (int id in idList)
                        {
                            _parameters.QTIItemLessonTwoService.Delete(id, tagId);
                        }
                        return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return Json(new { Success = "Fail", errorMessage = string.Format("Error deleting tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
                    }
                    break;
                default:
                    return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignDistrictTagForManyQtiItems(string qtiItemIdString, string tagIdString)
        {
            List<int> qtiItemIdList = qtiItemIdString.ParseIdsFromString();
            List<int> tagIdList = tagIdString.ParseIdsFromString();

            //avoid someone modify ajax parameter qtiItemIdString
            var authorizedQtiItemList = new List<QTIItemData>();
            if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser,qtiItemIdList, out authorizedQtiItemList))
            {
                return  Json(new { Success = "Fail", errorMessage = "Has no right to assign  tag for one or more items." }, JsonRequestBehavior.AllowGet);
            }

            if (!_parameters.ItemTagService.HasRightToEditItemTags(tagIdList, CurrentUser, CurrentUser.GetMemberListDistrictId()))
            {
                return
                          Json(new { Success = "Fail", errorMessage = "Has no right on one or more tags." }, JsonRequestBehavior.AllowGet);
            }
            foreach (var qtiItemId in qtiItemIdList)
            {
                foreach (var tagId in tagIdList)
                {
                    //assign each tag to each qtiItem
                    var tag = new QtiItemItemTag();
                    tag.QtiItemID = qtiItemId;
                    tag.ItemTagID= tagId;
                    //check exists before saving
                    var exists = _parameters.QtiItemItemTagService.GetAll().Any(x => x.QtiItemID == qtiItemId && x.ItemTagID == tagId);
                    if (!exists)
                    {
                        _parameters.QtiItemItemTagService.Save(tag);
                        if (tag.QtiItemItemTagID == 0)
                        {
                            return
                                Json(
                                    new
                                    {
                                        Success = "Fail",
                                        errorMessage = string.Format("Error inserting tag to question.")
                                    },
                                    JsonRequestBehavior.AllowGet);
                            //error message "Error inserting tag to question." is gotten from flash
                        }
                    }
                }
            }
            return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMutualAssignedDistrictTagIdString(string qtiItemIdString)
        {
            //for Topic,Skill and Other
            List<ItemTag> itemTags = _parameters.QtiItemItemTagService.GetMutualItemTagOfManyQtiItems(qtiItemIdString);
            //var authorizedItemTags = _parameters.ItemTagService.GetAuthorizedItemTags(itemTags, CurrentUser, CurrentUser.GetMemberListDistrictId());
            string mutualItemTagIdString = string.Empty;
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                foreach (var tag in itemTags)
                {
                    mutualItemTagIdString += string.Format(",-{0}-", tag.ItemTagID);
                }
            }
            return Json(new { MutualItemTagIdString = mutualItemTagIdString }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetMutualAssignedDistrictTags(string mutualItemTagIdString)
        {
            List<int> itemTagIdList = mutualItemTagIdString.ParseIdsFromString();
            var data = _parameters.ItemTagService.GetAllItemTag().Where(x => itemTagIdList.Contains(x.ItemTagID)).Select(
                x => new QtiItemTagAssignViewModel
                         {
                             ItemTagId = x.ItemTagID,
                             CategoryName = x.Category,
                             TagName = x.Name
                         });

            var parser = new DataTableParser<QtiItemTagAssignViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveDistrictTagForManyQtiItems(string qtiItemIdString, int itemTagId)
        {
            //avoid someone modify ajax parameter qtiItemIdString
            var authorizedQtiItemList = new List<QTIItemData>();
            if (!_parameters.VulnerabilityService.HasRightToEditQtiItems(CurrentUser,qtiItemIdString.ParseIdsFromString(), out authorizedQtiItemList))
            {
                return Json(new { Success = "Fail", errorMessage = "Has no right to assign  tag for one or more items." }, JsonRequestBehavior.AllowGet);
            }

            //if (!_parameters.ItemTagService.HasRightToEditItemTags(new List<int> { itemTagId }, CurrentUser, CurrentUser.GetMemberListDistrictId()))
            //{
            //    return Json(new { Success = "Fail", errorMessage = "Has no right on one tag." }, JsonRequestBehavior.AllowGet);
            //}

            List<int> idList = qtiItemIdString.ParseIdsFromString();
            try
            {
                foreach (int id in idList)
                {
                    _parameters.QtiItemItemTagService.Delete(id,itemTagId);
                }
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error deleting tag: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult GetMutualTopicsOfManyQtiItems(string qtiItemIdString)
        {
            List<int> idList = qtiItemIdString.ParseIdsFromString();
            var parser = new DataTableParser<Topic>();
            var itemTopics =
                this._parameters.QTIItemTopicService.GetAll().Where(x => idList.Contains(x.QTIItemID)).Select(
                    x => new QTIItemTopic { QTIItemID = x.QTIItemID, TopicId = x.TopicId, Name = x.Name }).ToList();
            var topics = itemTopics.GroupBy(x => x.TopicId).Select(g => g.First()).Select(x => new Topic { TopicID = x.TopicId, Name = x.Name });
            List<Topic> resutl = new List<Topic>();
            foreach (var topic in topics)
            {
                var count = itemTopics.Where(x => x.TopicId == topic.TopicID).Count();
                if (count == idList.Count)//the mutal topic
                {
                    resutl.Add(topic);
                }
            }
            return Json(parser.Parse(resutl.AsQueryable()), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMutualSkillsOfManyQtiItems(string qtiItemIdString)
        {
            List<int> idList = qtiItemIdString.ParseIdsFromString();
            var parser = new DataTableParser<LessonOne>();
            var itemSkills =
               this._parameters.QTIItemLessonOneService.GetAll().Where(x => idList.Contains(x.QTIItemID)).Select(
                   x => new QTIItemLessonOne { QTIItemID = x.QTIItemID, LessonOneID = x.LessonOneID, Name = x.Name }).ToList();
            var skills = itemSkills.GroupBy(x => x.LessonOneID).Select(g => g.First()).Select(x => new LessonOne { LessonOneID = x.LessonOneID, Name = x.Name });
            List<LessonOne> resutl = new List<LessonOne>();
            foreach (var skill in skills)
            {
                var count = itemSkills.Where(x => x.LessonOneID == skill.LessonOneID).Count();
                if (count == idList.Count)//the mutal topic
                {
                    resutl.Add(skill);
                }
            }
            return Json(parser.Parse(resutl.AsQueryable()), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMutualOthersOfManyQtiItems(string qtiItemIdString)
        {
            List<int> idList = qtiItemIdString.ParseIdsFromString();
            var parser = new DataTableParser<LessonTwo>();
            var itemOthers =
               this._parameters.QTIItemLessonTwoService.GetAll().Where(x => idList.Contains(x.QTIItemID)).Select(
                   x => new QTIItemLessonTwo { QTIItemID = x.QTIItemID, LessonTwoID = x.LessonTwoID, Name = x.Name }).ToList();
            var others = itemOthers.GroupBy(x => x.LessonTwoID).Select(g => g.First()).Select(x => new LessonTwo { LessonTwoID = x.LessonTwoID, Name = x.Name });
            List<LessonTwo> resutl = new List<LessonTwo>();
            foreach (var other in others)
            {
                var count = itemOthers.Where(x => x.LessonTwoID == other.LessonTwoID).Count();
                if (count == idList.Count)//the mutal topic
                {
                    resutl.Add(other);
                }
            }

            return Json(parser.Parse(resutl.AsQueryable()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSearchAllOthersTags(string tagSearch, string qtiitemIdString)
        {
            tagSearch = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(tagSearch));
            var data = this._parameters.LessonTwoService.GetLessonTwosBySearchText(tagSearch, qtiitemIdString, ContaintUtil.QTIItem);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSearchAllTopicTags(string tagSearch, string qtiitemIdString)
        {
            tagSearch = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(tagSearch));
            var data = this._parameters.TopicService.GetTopicsBySearchText(tagSearch, qtiitemIdString, ContaintUtil.QTIItem);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSearchAllSkillTags(string tagSearch, string qtiitemIdString)
        {
            tagSearch = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(tagSearch));
            var data = this._parameters.LessonOneService.GetLessonOnesBySearchText(tagSearch, qtiitemIdString, ContaintUtil.QTIItem);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
