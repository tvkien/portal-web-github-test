using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class ItemTagController : BaseController
    {
        private readonly ItemTagService _itemTagService ;
        private readonly QtiItemItemTagService _qtiItemItemTagService;

        public ItemTagController(ItemTagService itemTagService, QtiItemItemTagService qtiItemItemTagService)
        {
            _itemTagService = itemTagService;
            _qtiItemItemTagService = qtiItemItemTagService;
        }
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignTags)]
        public ActionResult Index()
        {
            ItemTagCategoryViewModel model = new ItemTagCategoryViewModel();
            model.RoleId = CurrentUser.RoleId;
            return View(model);
        }
        #region ItemTagCategory
        public ActionResult LoadItemTagCategory(int? stateId, int? districtId, string searchBoxText)
        {
            ItemTagCategoryViewModel model = new ItemTagCategoryViewModel();
            model.RoleId = CurrentUser.RoleId;
            model.DistrictId = CurrentUser.DistrictId.Value;
            model.SelectedStateId = stateId ?? 0;
            model.SelectedDistrictId = districtId ?? 0;
            model.SearchBoxText = HttpUtility.UrlDecode(searchBoxText);
            return PartialView("Index",model);
        }
        public ActionResult SearchItemTagCategory(int districtId, string categoryToSearch,int? stateId)
        {
            categoryToSearch = Util.ConvertByteArrayStringToUtf8String(categoryToSearch);
            categoryToSearch = Util.ProcessWildCharacters(categoryToSearch);

            var data =
                _itemTagService.GetAllItemCategory().Where(
                    x =>(x.Name.Contains(categoryToSearch) || x.Description.Contains(categoryToSearch)));
            if(CurrentUser.IsPublisher || CurrentUser.IsNetworkAdmin)
            {
                if (districtId >= 0)
                {
                    data = data.Where(x => x.DistrictID == districtId);
                }
                //else
                //{
                //    if(stateId.HasValue && stateId.Value >= 0)
                //    {
                //        data = data.Where(x => x.StateId == stateId.Value);
                //    }
                //}
            }
            else
            {
                if(CurrentUser.IsDistrictAdmin)
                {
                    data = data.Where(x => x.DistrictID == CurrentUser.DistrictId);
                }
            }
            var data1 = data.ToList().Select(x => new ItemTagCategoryListItemViewModel
                                             {
                                                 ItemTagCategoryID = x.ItemTagCategoryID,
                                                 District = x.District,
                                                 Name = Server.HtmlEncode(x.Name),
                                                 Description = Server.HtmlEncode(x.Description),
                                                 CountQtiItem = x.CountQtiItem
                                             });
            var parser = new DataTableParser<ItemTagCategoryListItemViewModel>();
            return Json(parser.Parse(data1.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadCreateItemTagCategory(int districtId)
        {
            ItemTagCategoryViewModel model = new ItemTagCategoryViewModel();
            model.RoleId = CurrentUser.RoleId;
            if(districtId==0)
            {
                districtId = CurrentUser.DistrictId.Value;
            }
            model.DistrictId = districtId;
            return PartialView("_CreateItemTagCategory", model);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateItemTagCategory(int districtId,string categoryName,string description)
        {

            int district = 0;
            if(CurrentUser.IsPublisher||CurrentUser.IsNetworkAdmin)
            {
                district = districtId;
            }
            else
            {

                district = CurrentUser.DistrictId.Value;
            }
            if (!Util.HasRightOnDistrict(CurrentUser, district))
            {
                return Json(new { ErrorList = new[] { new { ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "!" } }, success = false },
                       JsonRequestBehavior.AllowGet);
            }
            //It's impossible to have two category within a district

            categoryName = HttpUtility.UrlDecode(categoryName);
            description = HttpUtility.UrlDecode(description);

            bool isExistCategory = _itemTagService.IsExistCategory(district, categoryName);
            if (!isExistCategory)
            {
                var obj = new ItemTagCategory()
                {
                    Name = categoryName,
                    Description = description,
                    DistrictID = district
                };
                _itemTagService.SaveItemTagCategory(obj);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ErrorList = new[] { new { ErrorMessage = "The category name you specified is already in use. Please use a new name or refer to that pre-existing category." } }, success = false },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadEditItemTagCategory(int itemTagCategoryID)
        {
            ItemTagCategoryViewModel model = new ItemTagCategoryViewModel();
            model.RoleId = CurrentUser.RoleId;
            ItemTagCategory category = _itemTagService.GetItemCategory(itemTagCategoryID);
            if(category!=null)
            {
                model.Name = category.Name;
                model.Description = category.Description;
                model.ItemTagCategoryID = category.ItemTagCategoryID;
                model.DistrictId = category.DistrictID;
            }
            return PartialView("_EditItemTagCategory", model);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateItemTagCategory(int itemTagCategoryID,int districtId, string categoryName, string description)
        {
            //It's possible to have two category within a district
            if (districtId == 0)
            {
                districtId = CurrentUser.DistrictId.Value;
            }
            //avoid modifying ajax parameters
            if (!Util.HasRightOnDistrict(CurrentUser, districtId))
            {
                return Json(new { ErrorList = new[] { new { ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "!" } }, success = false },
                       JsonRequestBehavior.AllowGet);
            }
            var itemTagCategory = _itemTagService.GetItemCategory(itemTagCategoryID);
            if (itemTagCategory != null)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, itemTagCategory.DistrictID))
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "!" } }, success = false },
                           JsonRequestBehavior.AllowGet);
                }
            }
            categoryName = HttpUtility.UrlDecode(categoryName);
            description = HttpUtility.UrlDecode(description);

            bool isExistCategory = _itemTagService.IsExistCategory(districtId,itemTagCategoryID, categoryName);
            if (!isExistCategory)
            {
                var obj = new ItemTagCategory()
                {
                    ItemTagCategoryID =  itemTagCategoryID,
                    Name = categoryName,
                    Description = description,
                    DistrictID = districtId
                };
                _itemTagService.SaveItemTagCategory(obj);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ErrorList = new[] { new { ErrorMessage = "The category name you specified is already in use. Please use a new name or refer to that pre-existing category." } }, success = false },
                        JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAllDistrictCategory(int? districtId)
        {
            int filterDistrictId = districtId ?? 0;
            if (!CurrentUser.IsPublisher && !CurrentUser.IsNetworkAdmin)
            {
                filterDistrictId = CurrentUser.DistrictId.Value;
            }

            var data = _itemTagService.GetAllItemCategory().Where(x => x.DistrictID == filterDistrictId).Select(o => new ListItem { Id = o.ItemTagCategoryID, Name = o.Name });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDistrictCategory()
        {
            if (CurrentUser.IsPublisher)
            {
                var data = _itemTagService.GetAllItemCategory().Select(o => new ListItem { Id = o.ItemTagCategoryID, Name = o.Name });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if(CurrentUser.IsNetworkAdmin)
            {
                var data = _itemTagService.GetAllItemCategory().Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictID)).Select(o => new ListItem { Id = o.ItemTagCategoryID, Name = o.Name });
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = _itemTagService.GetAllItemCategory().Where(x => x.DistrictID == CurrentUser.DistrictId.Value).Select(o => new ListItem { Id = o.ItemTagCategoryID, Name = o.Name });
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteItemTagCategory(int itemTagCategoryId)
        {
            var itemTagCategory = _itemTagService.GetItemCategory(itemTagCategoryId);
            if (itemTagCategory != null)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, itemTagCategory.DistrictID))
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "!" } }, success = false },
                           JsonRequestBehavior.AllowGet);
                }
            }

            try
            {
                //delete all qtiitem association with this category through its tag
                var itemTags = _itemTagService.GetAllItemTagByCategory(itemTagCategoryId);
                foreach (var itemTag in itemTags)
                {
                    _qtiItemItemTagService.DeleteQtiItemTagOfTag(itemTag.ItemTagID);
                    _itemTagService.DeleteItemTag(itemTag);
                }
                _itemTagService.DeleteItemTagCategory(itemTagCategoryId);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult EncryptByteString(string str)
        {
            str = HttpUtility.UrlDecode(str);
            string result = Util.ConvertUtf8StringToUtf8ByteString(str);
            return Json(new { EncryptString = result }, JsonRequestBehavior.AllowGet);
        }
        #endregion ItemTagCategory
        #region ItemTag
        public ActionResult ShowItemTagPage(int itemTagCategoryID, int? stateId, int? districtId, string searchBoxText)
        {
            //avoid modifying url parameters
            var itemTagCategory = _itemTagService.GetItemCategory(itemTagCategoryID);
            if (itemTagCategory != null)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, itemTagCategory.DistrictID))
                {
                    return RedirectToAction("Index", "QTIItemTag");
                }
            }
            else
            {
                return RedirectToAction("Index", "QTIItemTag");
            }

            //categoryName = HttpUtility.UrlDecode(categoryName);
            //get tag

            ItemTagViewModel model = new ItemTagViewModel();
            model.RoleId = CurrentUser.RoleId;
            model.ItemTagCategoryID = itemTagCategoryID;
            model.SelectedStateId = stateId ?? 0;
            model.SelectedDistrictId = districtId ?? 0;
            model.SearchBoxText = HttpUtility.UrlDecode(searchBoxText);
            var category = _itemTagService.GetItemCategory(itemTagCategoryID);
            if (category != null)
            {
                model.CategoryName = Server.HtmlEncode(category.Name);
            }

            return View("ItemTag",model);
        }
        public ActionResult SearchItemTag(int itemTagCategoryId, string textToSearch)
        {

            textToSearch = HttpUtility.HtmlEncode(HttpUtility.UrlDecode(textToSearch));
            textToSearch = Util.ProcessWildCharacters(textToSearch);
            //check right
            if (itemTagCategoryId > 0)
            {
                var itemTagCategory = _itemTagService.GetItemCategory(itemTagCategoryId);
                if (itemTagCategory == null)
                {
                    return Json(new { error = "Category does not exist." }, JsonRequestBehavior.AllowGet);
                }
                if (!Util.HasRightOnDistrict(CurrentUser, itemTagCategory.DistrictID))
                {
                    return Json(new { error = "Has no right to access  Category." }, JsonRequestBehavior.AllowGet);
                }
            }

            var data = _itemTagService.GetAllItemTagByCategory(itemTagCategoryId);
            if (!string.IsNullOrWhiteSpace(textToSearch))
            {
                data = data.Where(
                    x => (x.Name.Contains(textToSearch) || x.Description.Contains(textToSearch)));
            }
            var result = data.ToList().Select(x => new ItemTagListItemViewModel
                    {
                        ItemTagID = x.ItemTagID,
                        Name = Server.HtmlEncode(x.Name),
                        Description = Server.HtmlEncode(x.Description),
                        CountQtiItem = x.CountQtiItem
                    });

            var parser = new DataTableParser<ItemTagListItemViewModel>();
            return Json(parser.Parse(result.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadCreateItemTag(int itemTagCategoryId, int districtId)
        {
            ItemTagViewModel model = new ItemTagViewModel();
            model.RoleId = CurrentUser.RoleId;
            model.ItemTagCategoryID = itemTagCategoryId;
            model.DistrictId = districtId;
            if(!CurrentUser.IsPublisher)
                model.DistrictId = CurrentUser.DistrictId ?? 0;
            return PartialView("_CreateItemTag", model);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateItemTag(int itemTagCategoryId, string tagName, string description)
        {
            //avoid modifying ajax parameters
            var itemTagCategory = _itemTagService.GetItemCategory(itemTagCategoryId);
            if (itemTagCategory != null)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, itemTagCategory.DistrictID))
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "!" } }, success = false },
                           JsonRequestBehavior.AllowGet);
                }
            }

            //It's impossible to have two tag within a category
            tagName = HttpUtility.UrlDecode(tagName);
            description = HttpUtility.UrlDecode(description);

            bool isExist = _itemTagService.IsExistTag(itemTagCategoryId, tagName);
            if (!isExist)
            {
                var obj = new ItemTag()
                {
                    ItemTagCategoryID = itemTagCategoryId,
                    Name = tagName,
                    Description = description,
                };
                _itemTagService.SaveItemTag(obj);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ErrorList = new[] { new { ErrorMessage = "The tag name you specified is already in use for this category. Please use a new name or refer to that pre-existing tag." } }, success = false },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadEditItemTag(int itemTagId, int districtId)
        {
            ItemTagViewModel model = new ItemTagViewModel();
            model.RoleId = CurrentUser.RoleId;
            model.DistrictId = districtId;
            if (!CurrentUser.IsPublisher)
                model.DistrictId = CurrentUser.DistrictId ?? 0;
            ItemTag tag = _itemTagService.GetItemTag(itemTagId);
            if (tag != null)
            {
                model.ItemTagID = itemTagId;
                model.Name = Server.HtmlEncode(tag.Name);
                model.Description = Server.HtmlEncode(tag.Description);
                model.ItemTagCategoryID = tag.ItemTagCategoryID;
            }
            return PartialView("_EditItemTag", model);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateItemTag(int itemTagCategoryId, int itemTagId, string tagName, string description)
        {
            //avoid modifying ajax parameters
            var itemTag = _itemTagService.GetItemTag(itemTagId);
            if (itemTag == null)
            {
                return Json(new { ErrorList = new[] { new { ErrorMessage = "Item tag does not exist!" } }, success = false },
                          JsonRequestBehavior.AllowGet);
            }
            if (itemTag.ItemTagCategoryID != itemTagCategoryId)
            {
                return Json(new { ErrorList = new[] { new { ErrorMessage = "Item tag does not belong to the category!" } }, success = false },
                        JsonRequestBehavior.AllowGet);
            }
            var itemTagCategory = _itemTagService.GetItemCategory(itemTagCategoryId);
            if (itemTagCategory != null)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, itemTagCategory.DistrictID))
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "!" } }, success = false },
                           JsonRequestBehavior.AllowGet);
                }
            }

            //It's impossible to have two tag within a category
            tagName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(tagName));
            description = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(description));


            bool isExist = _itemTagService.IsExistTag(itemTagCategoryId,itemTagId, tagName);
            if (!isExist)
            {
                var obj = new ItemTag()
                {
                    ItemTagID = itemTagId,
                    ItemTagCategoryID = itemTagCategoryId,
                    Name = tagName,
                    Description = description,
                };
                _itemTagService.SaveItemTag(obj);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ErrorList = new[] { new { ErrorMessage = "The tag name you specified is already in use for this category. Please use a new name or refer to that pre-existing tag." } }, success = false },
                        JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteItemTag(int itemTagId)
        {
            //avoid modifying ajax parameters
            var itemTag = _itemTagService.GetItemTag(itemTagId);
            if (itemTag == null)
            {
                return Json(new { ErrorList = new[] { new { ErrorMessage = "Item tag does not exist!" } }, success = false },
                          JsonRequestBehavior.AllowGet);
            }

            var itemTagCategory = _itemTagService.GetItemCategory(itemTag.ItemTagCategoryID);
            if (itemTagCategory != null)
            {
                if (!Util.HasRightOnDistrict(CurrentUser, itemTagCategory.DistrictID))
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = "Has no right on this " + LabelHelper.DistrictLabel + "!" } }, success = false },
                           JsonRequestBehavior.AllowGet);
                }
            }

            try
            {
                //delete all qtiitem association with this tag
                _qtiItemItemTagService.DeleteQtiItemTagOfTag(itemTagId);
                var obj = new ItemTag()
                {
                    ItemTagID = itemTagId
                };
                _itemTagService.DeleteItemTag(obj);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDistrictTags(int? districtCategoryId)
        {
            if (districtCategoryId.HasValue)
            {
                var data = _itemTagService.GetAllItemTagByCategory(districtCategoryId.Value).Select(
                        o => new ListItem {Id = o.ItemTagID, Name = o.Name});
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = new List<ListItem>();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSuggestTags(int districtId, string textToSearch)
        {
            textToSearch = HttpUtility.UrlDecode(textToSearch);
            var data = _itemTagService.GetSuggestTags(districtId, textToSearch)
                .ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetItemTags(int itemTagCategoryId)
        {
            var tags = _itemTagService.GetAllItemTagByCategory(itemTagCategoryId)
                .Select(x => new ListItem { Id = x.ItemTagID, Name = x.Name })
                .ToList();

            return Json(tags, JsonRequestBehavior.AllowGet);
        }

        #endregion ItemTag
    }
}
