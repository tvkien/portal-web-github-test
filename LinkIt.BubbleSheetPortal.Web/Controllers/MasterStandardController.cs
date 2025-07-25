using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Models.Enums;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Services;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize(Order = 2)]
    public class MasterStandardController : BaseController
    {
        private readonly MasterStandardService masterStandardService;
        private readonly StateService stateService;
        private readonly VirtualQuestionService virtualQuestionService;
        private readonly VulnerabilityService vulnerabilityService;
        private readonly RubricModuleQueryService rubricModuleQueryService;
        private readonly RubricModuleCommandService rubricModuleCommandService;

        public MasterStandardController(MasterStandardService masterStandardService, StateService stateService, VirtualQuestionService virtualQuestionService, VulnerabilityService vulnerabilityService,
            RubricModuleQueryService rubricQuestionCategoryService, RubricModuleCommandService rubricModuleCommandService)
        {
            this.masterStandardService = masterStandardService;
            this.stateService = stateService;
            this.virtualQuestionService = virtualQuestionService;
            this.vulnerabilityService = vulnerabilityService;
            this.rubricModuleQueryService = rubricQuestionCategoryService;
            this.rubricModuleCommandService = rubricModuleCommandService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetStandardSubjectByStateId(int stateId)
        {
            var state = stateService.GetStateById(stateId);
            if (state != null)
            {
                var listStandardSubject = masterStandardService.GetStateStandardsByStateCode(state.Code);
                return Json(new { Success = true, Data = listStandardSubject });
            }
            return Json(new { Success = false });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult GetStandardGradeByStateAndSubject(int stateId, string subject)
        {
            var state = stateService.GetStateById(stateId);
            if (state != null)
            {
                var listStandardGrade = masterStandardService.GetStateSubjectGradeByStateAndSubject(state.Code, subject);
                return Json(new { Success = true, Data = listStandardGrade });
            }
            return Json(new { Success = false });
        }

        public ActionResult GetStandardByStateAndSubjectAndGradeTopLevel(int stateId, string subject, string grade)
        {
            var parser = new DataTableParser<MasterStandardViewModel>();
            var state = stateService.GetStateById(stateId);
            if (state != null)
            {
                var listStandard = masterStandardService.GetStandardsByStateCodeAndSubjectAndGradeTopLevel(state.Code, subject, grade).Select(x => new MasterStandardViewModel
                {
                    GUID = x.GUID,
                    MasterStandardID = x.MasterStandardID,
                    Description = x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters(),
                    Number = string.IsNullOrEmpty(x.Number) ? x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters() : x.Number,
                    Level = x.Level,
                    Children = x.Children,
                    ParentGUID = x.ParentGUID
                }).AsQueryable();

                return Json(parser.Parse(listStandard, true),
                    JsonRequestBehavior.AllowGet);
            }

            return Json(parser.Parse((new List<MasterStandardViewModel>()).AsQueryable()),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStandardByParentStandardId(int parentId)
        {
            var standard =
                masterStandardService.GetStandardsByParentId(parentId)
                    .Select(x => new MasterStandardViewModel
                    {
                        Description = x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters(),
                        Number = string.IsNullOrEmpty(x.Number) ? x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters() : x.Number,
                        MasterStandardID =
                                         x.MasterStandardID,
                        Level = x.Level,
                        Children = x.Children,
                        GUID = x.GUID
                    }).AsQueryable();
            var parser = new DataTableParser<MasterStandardViewModel>();
            return Json(parser.Parse(standard), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStandardByChildStandardId(int childId, string grade)
        {
            grade = HttpUtility.UrlDecode(grade);
            var standard =
                masterStandardService.GetParentStandardsByChildId(childId, grade)
                    .Select(x => new MasterStandardViewModel
                    {
                        Description = x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters(),
                        Number = string.IsNullOrEmpty(x.Number) ? x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters() : x.Number,
                        MasterStandardID =
                            x.MasterStandardID,
                        Level = x.Level,
                        Children = x.Children,
                        GUID = x.GUID
                    }).AsQueryable();
            var parser = new DataTableParser<MasterStandardViewModel>();
            return Json(parser.Parse(standard), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssignedStandard(int qtiItemId)
        {
            var parser = new DataTableParser<MasterStandardViewModel>();
            var data = masterStandardService.GetStandardsAssociatedWithItem(qtiItemId).Where(x => !x.Archived);
            if (!CurrentUser.IsPublisher)
            {
                var stateOfUser = stateService.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);
                data = data.Where(x => stateOfUser.Contains(x.StateId.GetValueOrDefault())).ToList();
            }
            var listStandard = data.Select(x => new MasterStandardViewModel
            {
                GUID = x.GUID,
                MasterStandardID = x.MasterStandardID,
                Number = string.IsNullOrEmpty(x.Number) ? x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters() : x.Number,
                Description = x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters(),
            }).AsQueryable();

            return Json(parser.Parse(listStandard, true),
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignStandard(int qtiItemId, int masterStandardId)
        {
            try
            {
                //check if this standard is existing or not
                var item = masterStandardService.GetStandardsAssociatedWithItem(qtiItemId).Where(
                    x => x.MasterStandardID == masterStandardId).FirstOrDefault();
                if (item != null)
                {
                    return Json(new { Success = "Fail", errorMessage = string.Format("This Master Standard has been assigned already.") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //insert standard
                    masterStandardService.AddStandardAssocoatedWithItem(qtiItemId, masterStandardId);
                    return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error assigning new Master Standard: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveStandard(int qtiItemId, int masterStandardId)
        {
            try
            {
                masterStandardService.RemoveStandardAssociatedWithItem(qtiItemId, masterStandardId);

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return Json(new { Success = "Fail", errorMessage = string.Format("Error removing Master Standard: {0}", ex.Message) }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetAssignStandardIdString(int qtiItemId)
        {
            var data = masterStandardService.GetStandardIdAssociatedWithItem(qtiItemId).ToList();
            string result = string.Empty;
            foreach (var id in data)
            {
                result += string.Format(",-{0}-", id);
            }
            return Json(new { QtiItemAssignedStandardIdString = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStandardsByStandardIdString(string standardIdString, string virtualQuestionIds, int? rubricCategoryId = 0)
        {
            var listResults = new List<MasterStandardViewModel>();
            var parser = new DataTableParser<MasterStandardViewModel>();
            var listTagDtos = new List<RubricCategoryTagDto>();
            if (!string.IsNullOrEmpty(virtualQuestionIds))
            {
                List<int> idList = new List<int>();
                if (!string.IsNullOrEmpty(standardIdString))
                {
                    string[] idArray = standardIdString.Split(',');
                    foreach (var id in idArray)
                    {
                        if (id.Length > 0)
                        {
                            idList.Add(int.Parse(id.Replace("-", "")));
                        }
                    }
                }
                var _virtualQuestionIds = virtualQuestionIds.ToIntArray();

                var dataTagOfQuestion = masterStandardService.GetStandardsAssociatedWithVirtualQuestions(_virtualQuestionIds.ToList()).ToList();

                var data = masterStandardService.GetAll().Where(x => idList.Contains(x.MasterStandardID)).Where(x => !x.Archived);

                var listStandard = data.Select(x => new MasterStandardViewModel
                {
                    GUID = x.GUID,
                    MasterStandardID = x.MasterStandardID,
                    Number = string.IsNullOrEmpty(x.Number) ? x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters() : x.Number,
                    Description = x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters(),
                }).ToList();

                var standardsRubricMulti = rubricModuleQueryService.GetRubricCategoryTagSelectListByIds(_virtualQuestionIds, TagTypeEnum.Standards, rubricCategoryId)
                    .Where(c => listStandard.Select(cc => cc.MasterStandardID).Contains(c.TagID))
                    .OrderBy(x => x.TagID)
                    .ToList();
                if (standardsRubricMulti?.Count > 0)
                {
                    listTagDtos.AddRange(standardsRubricMulti);
                }


                if (listStandard?.Count > 0)
                {
                    foreach (var item in dataTagOfQuestion)
                    {
                        var findByTagId = listStandard.Where(x => x.MasterStandardID == item.StateStandardId).ToList();
                        foreach (var tag in findByTagId)
                        {
                            var getTagModel = findByTagId.FirstOrDefault(t => t.MasterStandardID == tag.MasterStandardID);
                            listTagDtos.Add(new RubricCategoryTagDto
                            {
                                TagID = tag.MasterStandardID,
                                TagDescription = getTagModel.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters(),
                                TagName = getTagModel.Number,
                                VirtualQuestionID = item.VirtualQuestionId,
                                TagType = TagTypeEnum.Standards.ToString()
                            });
                        }
                    }
                }
                listTagDtos = listTagDtos.GroupBy(x => new { x.TagID, x.VirtualQuestionID }).Select(y => y.FirstOrDefault()).ToList();
                if (rubricCategoryId > 0)
                {
                    listTagDtos = listTagDtos.Where(x => x.RubricQuestionCategoryID == rubricCategoryId).ToList();
                }
                foreach (var item in listTagDtos)
                {
                    var count = listTagDtos.Count(x => x.TagID == item.TagID);
                    if (count == _virtualQuestionIds.Length)
                    {
                        listResults.Add(new MasterStandardViewModel
                        {
                            GUID = Guid.NewGuid().ToString(),
                            MasterStandardID = item.TagID,
                            Number = $"{item.TagName}",
                            Description = item.TagDescription.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters(),
                            RubricQuestionCategoryID = item.RubricQuestionCategoryID
                        });
                    }
                }
                if (_virtualQuestionIds.Length == 1)
                {
                    var detectRubricQuestion = virtualQuestionService.Select().FirstOrDefault(x => x.VirtualQuestionID == _virtualQuestionIds[0]);
                    if (detectRubricQuestion != null && detectRubricQuestion.IsRubricBasedQuestion == true)
                    {
                        var standardsRubricTags = standardsRubricMulti.GroupBy(x => new { x.TagID, x.RubricQuestionCategoryID }).Select(y => y.FirstOrDefault()).ToList();
                        listResults.Clear();
                        var cateCount = rubricCategoryId == 0 ? rubricModuleQueryService.GetRubicQuestionCategoriesByVirtualQuestionIds(detectRubricQuestion.VirtualQuestionID).Count() : 1;
                        foreach (var item in standardsRubricTags)
                        {
                            var count = standardsRubricTags.Count(x => x.TagID == item.TagID);
                            if (count == cateCount)
                            {
                                listResults.Add(new MasterStandardViewModel
                                {
                                    GUID = Guid.NewGuid().ToString(),
                                    MasterStandardID = item.TagID,
                                    Number = $"{item.TagName}",
                                    Description = item.TagDescription.ReplaceWeirdCharacters(),
                                    RubricQuestionCategoryID = item.RubricQuestionCategoryID
                                });
                            }
                        }
                    }
                }

                listResults = listResults.GroupBy(x => x.MasterStandardID).Select(y => y.FirstOrDefault()).ToList();

                return Json(parser.Parse(listResults.AsQueryable(), true),
                    JsonRequestBehavior.AllowGet);
            }
            return Json(parser.Parse(listResults.AsQueryable(), true),
                       JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMultiStandardsByStandardIdString(string standardIdString)
        {
            var idList = standardIdString?.Replace("-", "").ToIntArray(",");

            var parser = new DataTableParser<MasterStandardViewModel>();
            var data = masterStandardService.GetAll().Where(x => idList.Contains(x.MasterStandardID) && !x.Archived);

            if (!CurrentUser.IsPublisher)
            {
                var stateOfUser = stateService.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);
                data = data.Where(x => stateOfUser.Contains(x.StateId.GetValueOrDefault()));
            }

            var listStandard = data
                .Select(x => new MasterStandardViewModel
                {
                    GUID = x.GUID,
                    MasterStandardID = x.MasterStandardID,
                    Number = x.Number,
                    Description = x.Description,
                })
                .GroupBy(x => x.MasterStandardID)
                .Select(x => x.FirstOrDefault())
                .ToList()
                .Select(x =>
                {
                    x.Description = x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters();
                    return x;
                });

            return Json(parser.Parse(listStandard.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAssignStandardIdStringMany(string qtiItemIdString)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                string[] qtiItemIds = qtiItemIdString.Split(',');
                foreach (var qtiItemId in qtiItemIds)
                {
                    var data = masterStandardService.GetStandardIdAssociatedWithItem(Int32.Parse(qtiItemId)).ToList();
                    foreach (var id in data)
                    {
                        if (!result.Contains(string.Format(",-{0}-", id)))
                        {
                            result += string.Format(",-{0}-", id);
                        }
                    }
                }
            }

            return Json(new { QtiItemAssignedStandardIdString = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMutualStandardIdStringMany(string qtiItemIdString)
        {
            List<int> idList = GetIdListFromIdString(qtiItemIdString);
            var data = masterStandardService.GetStandardsAssociatedWithItem(idList).ToList();

            var stateStandardIdList = data.GroupBy(x => x.StateStandardID).Select(g => g.First()).Select(x => x.StateStandardID);//select distinct

            List<int> mutualStandardId = new List<int>();
            foreach (var stateStandardId in stateStandardIdList)
            {
                var count = data.Count(x => x.StateStandardID == stateStandardId);
                if (count >= idList.Count)//the mutual standard
                {
                    mutualStandardId.Add(stateStandardId);
                }
            }

            string result = string.Empty;
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                foreach (var stateStandardId in mutualStandardId)
                {
                    result += string.Format(",-{0}-", stateStandardId);
                }
            }

            return Json(new { QtiItemAssignedStandardIdString = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignStandardForManyQtiItems(string qtiItemIdString, int masterStandardId)
        {
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                var qtiItemIdList = qtiItemIdString.ParseIdsFromString();
                //avoid someone modify ajax parameter qtiItemIdString
                var authorizedQtiItemList = new List<QTIItemData>();
                if (!vulnerabilityService.HasRightToEditQtiItems(CurrentUser, qtiItemIdList, out authorizedQtiItemList))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right to assign standard for one or more items." }, JsonRequestBehavior.AllowGet);
                }

                foreach (var qtiItem in authorizedQtiItemList)
                {
                    try
                    {
                        //check if this standard is existing or not
                        var item = masterStandardService.GetStandardsAssociatedWithItem(qtiItem.QTIItemID).FirstOrDefault(x => x.MasterStandardID == masterStandardId);
                        if (item == null)
                        {
                            masterStandardService.AddStandardAssocoatedWithItem(qtiItem.QTIItemID, masterStandardId);
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
                                    string.Format("Error assigning new Master Standard: {0}", ex.Message)
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

        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveStandardForManyQtiItems(string qtiItemIdString, int masterStandardId)
        {
            if (!string.IsNullOrEmpty(qtiItemIdString))
            {
                var qtiItemIdList = qtiItemIdString.ParseIdsFromString();
                //avoid someone modify ajax parameter qtiItemIdString
                var authorizedQtiItemList = new List<QTIItemData>();
                if (!vulnerabilityService.HasRightToEditQtiItems(CurrentUser, qtiItemIdList, out authorizedQtiItemList))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right to remove standard for one or more items." }, JsonRequestBehavior.AllowGet);
                }

                foreach (var qtiItem in authorizedQtiItemList)
                {
                    try
                    {
                        //check if this standard is existing or not
                        var item = masterStandardService.GetStandardsAssociatedWithItem(qtiItem.QTIItemID).FirstOrDefault(x => x.MasterStandardID == masterStandardId);
                        if (item != null)
                        {
                            masterStandardService.RemoveStandardAssociatedWithItem(qtiItem.QTIItemID, masterStandardId);
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
                                string.Format("Error removing Master Standard: {0}", ex.Message)
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

        public ActionResult GetStateStandardsForItemLibraryFilterTopLevel(int stateId, string subject, string grade, bool qti3p, string personal, int? qti3pSourceId, string districtSearch)
        {
            var parser = new DataTableParser<MasterStandardViewModel>();
            var state = stateService.GetStateById(stateId);
            subject = HttpUtility.UrlDecode(subject);
            grade = HttpUtility.UrlDecode(grade);
            if (state != null)
            {
                if (qti3p)
                {
                    var listStandard = masterStandardService.GetStateStandardsForItem3pLibraryFilterTopLevel(state.Code,
                    subject, grade, qti3pSourceId).Select(x => new MasterStandardViewModel
                    {
                        GUID = x.GUID,
                        MasterStandardID = x.MasterStandardID,
                        Description = x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters(),
                        Number = string.IsNullOrEmpty(x.Number) ? x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters() : x.Number,
                        Level = x.Level,
                        Children = x.Children,
                        ParentGUID = x.ParentGUID,
                        DescendantItemCount = x.DescendantItemCount
                    });

                    return PartialView("StandardTreeView", listStandard);
                }
                else
                {
                    int? userId = null;
                    int? districtId = null;
                    if (!string.IsNullOrEmpty(personal) && personal.Equals("true") && !string.IsNullOrEmpty(districtSearch) && districtSearch.Equals("true"))
                    {
                        userId = CurrentUser.Id;
                        districtId = CurrentUser.DistrictId.Value;
                    }
                    else if (personal.Equals("true"))
                    {
                        userId = CurrentUser.Id;
                    }
                    else
                    {
                        districtId = CurrentUser.DistrictId.Value;
                    }
                    var listStandard = masterStandardService.GetStateStandardsForItemLibraryFilterTopLevel(state.Code,
                    subject, grade, userId, districtId).Select(x => new MasterStandardViewModel
                    {
                        GUID = x.GUID,
                        MasterStandardID = x.MasterStandardID,
                        Description = x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters(),
                        Number = string.IsNullOrEmpty(x.Number) ? x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters() : x.Number,
                        Level = x.Level,
                        Children = x.Children,
                        ParentGUID = x.ParentGUID,
                        DescendantItemCount = x.DescendantItemCount
                    });

                    return PartialView("StandardTreeView", listStandard);
                }
            }
            return PartialView("StandardTreeView", new List<MasterStandardViewModel>());
        }

        public ActionResult GetStateStandardsNextLevelForItemLibraryFilter(int parentId, string subject, string grade, bool qti3p, string personal, int? qti3pSourceId, string districtSearch, int stateId = 0)
        {
            subject = HttpUtility.UrlDecode(subject);
            grade = HttpUtility.UrlDecode(grade);
            var state = stateService.GetStateById(stateId);
            subject = HttpUtility.UrlDecode(subject);
            grade = HttpUtility.UrlDecode(grade);
            var parser = new DataTableParser<MasterStandardViewModel>();
            if (state != null)
            {
                int? userId = null;
                int? districtId = null;
                if (!string.IsNullOrEmpty(personal) && personal.Equals("true") && !string.IsNullOrEmpty(districtSearch) && districtSearch.Equals("true"))
                {
                    userId = CurrentUser.Id;
                    districtId = CurrentUser.DistrictId.Value;
                }
                else if (personal.Equals("true"))
                {
                    userId = CurrentUser.Id;
                }
                else
                {
                    districtId = CurrentUser.DistrictId.Value;
                }
                var standard =
                    masterStandardService.GetStateStandardsNextLevelForItemLibraryFilter(parentId, state.Code, subject, grade,
                                                                                         qti3p, userId, districtId, qti3pSourceId)
                        .Select(x => new MasterStandardViewModel
                        {
                            Description = x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters(),
                            Number = string.IsNullOrEmpty(x.Number) ? x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters() : x.Number,
                            MasterStandardID = x.MasterStandardID,
                            Level = x.Level,
                            Children = x.Children,
                            ParentGUID = x.ParentGUID,
                            DescendantItemCount = x.DescendantItemCount,
                            ParentId = parentId
                        });

                return PartialView("StandardNodeView", standard);
            }
            return PartialView("StandardNodeView", new List<MasterStandardViewModel>());
        }

        public ActionResult GetStateStandardsPreviousLevelForItemLibraryFilter(int childId, int stateId, string subject, string grade, bool qti3p, string personal, int? qti3pSourceId)
        {
            grade = HttpUtility.UrlDecode(grade);
            var state = stateService.GetStateById(stateId);
            subject = HttpUtility.UrlDecode(subject);
            grade = HttpUtility.UrlDecode(grade);
            var parser = new DataTableParser<MasterStandardViewModel>();
            if (state != null)
            {
                int? userId = null;
                int? districtId = null;
                if (personal.Equals("true"))
                {
                    userId = CurrentUser.Id;
                }
                else
                {
                    districtId = CurrentUser.DistrictId.Value;
                }
                var standard =
                    masterStandardService.GetStateStandardsPreviousLevelForItemLibraryFilter(childId, state.Code, subject, grade, qti3p, userId, districtId, qti3pSourceId)
                        .Select(x => new MasterStandardViewModel
                        {
                            Description = x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters(),
                            Number = string.IsNullOrEmpty(x.Number) ? x.Description.ConvertFromUnicodeToWindow1252().ReplaceWeirdCharacters() : x.Number,
                            MasterStandardID =
                                x.MasterStandardID,
                            Level = x.Level,
                            Children = x.Children,
                            ParentGUID = x.ParentGUID,
                            DescendantItemCount = x.DescendantItemCount
                        }).AsQueryable();

                return Json(parser.Parse(standard), JsonRequestBehavior.AllowGet);
            }
            return Json(parser.Parse((new List<MasterStandardViewModel>()).AsQueryable()),
               JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMutualStandardIdStringOfVirtualQuestions(string virtualQuestionString)
        {
            List<int> idList = GetIdListFromIdString(virtualQuestionString);
            string result = string.Empty;
            if (idList.Any())
            {
                var data = masterStandardService.GetStandardsAssociatedWithVirtualQuestions(idList).ToList();

                if (!CurrentUser.IsPublisher)
                {
                    var stateOfUser = stateService.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);
                    var standards = masterStandardService.GetStandardsAssociatedWithVirtualQuestion(idList.FirstOrDefault())
                        .Where(x => stateOfUser.Contains(x.StateId.GetValueOrDefault()));

                    var standardsGroups = standards.GroupBy(x => x.Number);
                    var groupResult = standardsGroups.Select(group =>
                    {
                        var standardsInState = group.Where(x => x.StateId == CurrentUser.StateId);
                        var standardsFinal = standardsInState.Any() ? standardsInState : group;
                        return standardsFinal.OrderByDescending(m => m.MasterStandardID).First();
                    }).ToList();

                    data = data.Where(x => groupResult.Select(r => r.MasterStandardID).Contains(x.StateStandardId)).ToList();
                }

                var stateStandardIdList = data.GroupBy(x => x.StateStandardId).Select(g => g.First()).Select(x => x.StateStandardId);//select distinct

                List<int> mutualStandardId = new List<int>();
                foreach (var stateStandardId in stateStandardIdList)
                {
                    var count = data.Count(x => x.StateStandardId == stateStandardId);
                    if (count >= idList.Count)//the mutual standard ,set == to >= to allow duplicate state standard can be shown
                    {
                        mutualStandardId.Add(stateStandardId);
                    }
                }

                foreach (var stateStandardId in mutualStandardId)
                {
                    result += string.Format(",-{0}-", stateStandardId);
                }
            }
            return Json(new { VirtualQuestionAssignedStandardIdString = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult AssignStandardForVirtualQuestions(string virtualQuestionString, int masterStandardId, List<RubricQuestionCategoryTag> questionCategoryTags)
        {
            if (!string.IsNullOrEmpty(virtualQuestionString))
            {
                //avoid ajax modify parameter
                var authorizedVirtualQuestionList = new List<VirtualQuestionData>();
                if (!virtualQuestionService.HasRightToEditVirtualQuestions(virtualQuestionString.ParseIdsFromString(),
                        CurrentUser, out authorizedVirtualQuestionList, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right on one or more questions." }, JsonRequestBehavior.AllowGet);
                }

                foreach (var virtualQuestion in authorizedVirtualQuestionList)
                {
                    try
                    {
                        //check if this standard is existing or not
                        var item = masterStandardService.GetStandardsAssociatedWithVirtualQuestion(virtualQuestion.VirtualQuestionID).FirstOrDefault(x => x.MasterStandardID == masterStandardId);
                        if (item == null)
                        {
                            masterStandardService.AddStandardAssociatedWithVirtualQuestion(virtualQuestion.VirtualQuestionID, masterStandardId);
                        }
                        if (virtualQuestion.IsRubricBasedQuestion == true)
                        {
                            var newQuestionCategoryTags = questionCategoryTags.Where(x => x.VirtualQuestionID == virtualQuestion.VirtualQuestionID).ToList();
                            rubricModuleCommandService.AssignCategoryTagByQuestionIds(new List<VirtualQuestionData> { virtualQuestion }, masterStandardId, newQuestionCategoryTags, TagTypeEnum.Standards);
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
                                string.Format("Error assigning new Master Standard: {0}", ex.Message)
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

        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveStandardForVirtualQuestions(string virtualQuestionString, int masterStandardId, string rubricQuestionCategoryId)
        {
            if (!string.IsNullOrEmpty(virtualQuestionString))
            {
                //avoid ajax modify parameter
                var authorizedVirtualQuestionList = new List<VirtualQuestionData>();
                if (!virtualQuestionService.HasRightToEditVirtualQuestions(virtualQuestionString.ParseIdsFromString(),
                        CurrentUser, out authorizedVirtualQuestionList, CurrentUser.GetMemberListDistrictId()))
                {
                    return Json(new { Success = "Fail", errorMessage = "Has no right on one or more questions." }, JsonRequestBehavior.AllowGet);
                }
                var rubricQuestionCategoryIds = rubricQuestionCategoryId.ToIntArray();
                if (authorizedVirtualQuestionList.Count > 1)
                {
                    var virtualQuestionIds = authorizedVirtualQuestionList.Select(x => x.VirtualQuestionID).ToArray();
                    rubricQuestionCategoryIds = rubricModuleQueryService.GetAllTagsOfRubricByTagId(masterStandardId, virtualQuestionIds).Select(x => x.RubricQuestionCategoryID).ToArray();
                }
                foreach (var virtualQuestion in authorizedVirtualQuestionList)
                {
                    try
                    {
                        if (virtualQuestion.IsRubricBasedQuestion == true)
                        {
                            var rubricCategoryTagDeleted = rubricModuleCommandService.DeleteCategoryTagByQuestionIds(virtualQuestion.VirtualQuestionID, masterStandardId, rubricQuestionCategoryIds, TagTypeEnum.Standards);
                            if (rubricCategoryTagDeleted == 0)
                            {
                                var item = masterStandardService.GetStandardsAssociatedWithVirtualQuestion(virtualQuestion.VirtualQuestionID).FirstOrDefault(x => x.MasterStandardID == masterStandardId);
                                if (item != null)
                                {
                                    masterStandardService.RemoveStandardAssociatedWithVirtualQuestion(virtualQuestion.VirtualQuestionID, masterStandardId);
                                }
                            }
                        }
                        else
                        {
                            var item = masterStandardService.GetStandardsAssociatedWithVirtualQuestion(virtualQuestion.VirtualQuestionID).FirstOrDefault(x => x.MasterStandardID == masterStandardId);
                            if (item != null)
                            {
                                masterStandardService.RemoveStandardAssociatedWithVirtualQuestion(virtualQuestion.VirtualQuestionID, masterStandardId);
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
                                string.Format("Error removing Master Standard: {0}", ex.Message)
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
    }
}
