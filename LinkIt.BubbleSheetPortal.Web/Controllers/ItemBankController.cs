using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Xml.Linq;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraPrinting.Native;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.DataFileUpload;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.TestMaker.S3VirtualTest;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Helpers.Media;
using LinkIt.BubbleSheetPortal.Web.Print;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using S3Library;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    [VersionFilter]

    public class ItemBankController : BaseController
    {
        private readonly ItemBankControllerParameters parameters;
        private readonly IS3Service s3Service;
        private readonly VulnerabilityService _vulnerabilityService;
        public ItemBankController(ItemBankControllerParameters parameters, IS3Service s3Service, VulnerabilityService vulnerabilityService)
        {
            this.parameters = parameters;
            this.s3Service = s3Service;
            _vulnerabilityService = vulnerabilityService;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestdesignAssessmentItemHTML)]
        public ActionResult Index(int? itemBankId)
        {
            ViewBag.ItemBankId = null;
            if (itemBankId.HasValue && itemBankId.Value > 0)
            {
                ViewBag.ItemBankId = itemBankId.Value;
            }
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.StateId = CurrentUser.StateId;
            ViewBag.DistrictId = CurrentUser.DistrictId;
            ViewBag.AbleToViewHideTeacherBanks = (CurrentUser.IsSchoolAdmin || CurrentUser.IsDistrictAdmin ||
                                                  CurrentUser.IsNetworkAdmin || CurrentUser.IsPublisher);
            return View();
        }

        #region Clone with filter user

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TestCloneItemBanks)]
        public ActionResult IndexCloneItemBank(int? itemBankId)
        {
            ViewBag.ItemBankId = null;
            if (itemBankId.HasValue && itemBankId.Value > 0)
            {
                ViewBag.ItemBankId = itemBankId.Value;
            }

            ViewBag.IsPublisher = CurrentUser.IsPublisher();
            //ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.StateId = CurrentUser.StateId;
            //ViewBag.DistrictId = CurrentUser.DistrictId;

            return View();
        }

        public ActionResult LoadMoveItemSetWithFilterUser(string itemSetIdList)
        {
            ViewBag.IsPublisher = CurrentUser.IsPublisher();
            //ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            ViewBag.StateId = CurrentUser.StateId;
            //ViewBag.DistrictId = CurrentUser.DistrictId;

            ViewBag.ItemSetIdList = itemSetIdList;

            return PartialView("_MoveItemSetWithFilterUser");
        }

        public ActionResult LoadItemBanksWithFilterUser(int? stateId, int? districtId, int? userId)
        {
            var data = new List<QtiBank>();

            if (districtId.HasValue && stateId.HasValue && userId.HasValue)
            {
                data.AddRange(parameters.QtiBankServices.GetItemBanksPersonal(userId.Value, districtId.GetValueOrDefault()).ToList());
            }

            var result = data.
                Select(o => new ItemBankViewModel()
                {
                    Name = o.Name,
                    QTIBankId = o.QtiBankId,
                });
            var parser = new DataTableParser<ItemBankViewModel>();
            return Json(parser.Parse(result.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadItemBanksWithFilterUser2(int? userId)
        {
            var data = new List<QtiBank>();

            if (userId.HasValue)
            {
                data.AddRange(parameters.QtiBankServices.GetOwnerItemBanks(userId.Value).ToList());
            }

            var result = data.
                Select(o => new ItemBankViewModel()
                {
                    Name = o.Name,
                    QTIBankId = o.QtiBankId,
                });
            var parser = new DataTableParser<ItemBankViewModel>();
            return Json(parser.Parse(result.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetManagedUsers(int districtId)
        {
            var data = parameters.UserService.GetManagedUsersByDistrictId(districtId).Select(x => new
            {
                Id = x.Id,
                Name = (x.LastName ?? string.Empty) + ((!(x.LastName == null || x.LastName == string.Empty) && !(x.FirstName == null || x.FirstName == string.Empty)) ? ", " : string.Empty) + (x.FirstName ?? string.Empty) + " (" + x.UserName + ")",
                FirstName = x.FirstName,
                LastName = x.LastName,
                //DistrictId = x.DistrictId,
                //StateId = x.StateId
            }).OrderBy(x => x.Name).ToList();

            //return Json(data, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(new { Data = data });
            var js = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            var jsonStringResult = js.Serialize(jsonResult.Data);
            return Content(jsonStringResult, "application/json");
        }

        [HttpGet]
        public ActionResult GetActiveManagedUsers(int districtId)
        {
            var data = parameters.UserService.GetActiveManagedUsersByDistrictId(districtId).Select(x => new
            {
                Id = x.Id,
                Name = (x.LastName ?? string.Empty) + ((!(x.LastName == null || x.LastName == string.Empty) && !(x.FirstName == null || x.FirstName == string.Empty)) ? ", " : string.Empty) + (x.FirstName ?? string.Empty) + " (" + x.UserName + ")",
                FirstName = x.FirstName,
                LastName = x.LastName,
                //DistrictId = x.DistrictId,
                //StateId = x.StateId
            }).OrderBy(x => x.Name).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadItemSetsWithFilterUser(int itemBankId, int? districtId, int? userId)
        {
            if (itemBankId == 0 || !userId.HasValue)
            {
                var emptyData = new List<ItemSetViewModel>();
                return Json(new DataTableParser<ItemSetViewModel>().Parse(emptyData.AsQueryable(), true), JsonRequestBehavior.AllowGet);
            }

            var districtIdFilter = parameters.UserService.GetUserById(userId.Value).DistrictId.GetValueOrDefault();
            if (districtId.HasValue && districtId.Value > 0)
            {
                districtIdFilter = districtId.GetValueOrDefault();
            }
            var data =
                parameters.QtiGroupServices.GetListQtiGroupByQtiBankId(itemBankId, userId.Value, CurrentUser.RoleId,
                                                                      districtIdFilter).
                    ToList()
                    .Select(o => new ItemSetViewModel()
                    {
                        Name = o.Name,
                        QTIGroupId = o.QtiGroupId,
                        AuthorGroup = o.AuthorGroupName,
                        AuthorGroupId = o.AuthorGroupId ?? 0,
                        AuthorName = o.AuthorLastName + " " + o.AuthorFirstName
                    }).AsQueryable();
            var parser = new DataTableParser<ItemSetViewModel>();
            return Json(parser.Parse(data, true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadItemSetsWithFilterUser2(int itemBankId, int? districtId, int? userId)
        {
            if (itemBankId == 0)
            {
                var emptyData = new List<ItemSetViewModel>();
                return Json(new DataTableParser<ItemSetViewModel>().Parse(emptyData.AsQueryable(), true), JsonRequestBehavior.AllowGet);
            }

            var data = parameters.QtiGroupServices.GetOwnerListQtiGroupByQtiBankId(itemBankId).ToList()
                    .Select(o => new ItemSetViewModel()
                    {
                        Name = o.Name,
                        QTIGroupId = o.QtiGroupId,
                        AuthorGroup = o.AuthorGroupName,
                        AuthorGroupId = o.AuthorGroupId ?? 0,
                        AuthorName = o.AuthorLastName + " " + o.AuthorFirstName
                    }).AsQueryable();
            var parser = new DataTableParser<ItemSetViewModel>();
            return Json(parser.Parse(data, true), JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        public ActionResult MoveItemSetList(string qtiGroupIdList, int toQtiBankId, bool createACopy, int userId)
        {
            try
            {
                var idList = new List<int>();
                foreach (var item in qtiGroupIdList.Split(';'))
                    idList.Add(int.Parse(item));

                var listSet = parameters.QtiGroupServices.GetByIdList(idList).ToList();
                var listSetNameList = listSet.Select(x => x.Name).ToList();

                //check if new item set name is existing in the new item bank or not
                bool isExistSetName = parameters.QtiGroupServices.ExistItemSetList(userId, toQtiBankId, listSetNameList);
                if (isExistSetName)
                {
                    return Json(new { Success = "Failed", ExistSetName = "1" }, JsonRequestBehavior.AllowGet);
                }
                if (!createACopy)
                {
                    parameters.QtiGroupServices.MoveToOtherItemBank(listSet, toQtiBankId, userId);
                }
                else
                {
                    Dictionary<int, int> newOldQtiGroupIdMap;

                    var newListSet = parameters.QtiGroupServices.CloneToItemBank(listSet, toQtiBankId, userId, out newOldQtiGroupIdMap);
                    var mediaModel = new MediaModel();
                    foreach (var qtiGroup in listSet)
                    {
                        // Copy all QtiItem from old qtiGroup to new qtiGroup
                        var qtiItems = parameters.QTIITemServices.SelectQTIItems().Where(en => en.QTIGroupID == qtiGroup.QtiGroupId).OrderBy(x => x.QuestionOrder); //clone item by QuestionOrder
                        var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
                        var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;

                        foreach (var qtiItemData in qtiItems)
                        {
                            var newQTIItemData = parameters.QTIITemServices.DuplicateQTIItem(userId, qtiItemData.QTIItemID,
                                newOldQtiGroupIdMap[qtiGroup.QtiGroupId], true, bucketName,
                                folder, LinkitConfigurationManager.GetS3Settings().S3Domain);

                            // Update Item Passage
                            if (newQTIItemData != null)
                                UpdateItemPassage(newQTIItemData.QTIItemID);
                        }
                    }
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return
                    Json(
                        new
                        {
                            Success = "Fail",
                            errorMessage = string.Format("Move Item Set  fail, error detail:" + ex.Message)
                        },
                        JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Clone with filter user

        public ActionResult LoadItemBanks(int? districtId, bool? hideTeacherBanks, bool? hideOtherPeopleBanks, int? itemBankId)
        {
            if (districtId == null)
                districtId = CurrentUser.DistrictId.GetValueOrDefault();

            //TODO: get list here;
            var data = parameters.QtiBankServices.GetItemBanks(CurrentUser.Id, CurrentUser.RoleId,
                                                               districtId.GetValueOrDefault(), hideTeacherBanks, hideOtherPeopleBanks).ToList().
                Select(o => new ItemBankViewModel()
                {
                    Name = Server.HtmlEncode(o.Name),
                    QTIBankId = o.QTIBankId,
                    AuthorGroup = Server.HtmlEncode(o.AuthorGroupName),
                    AuthorGroupId = o.AuthorGroupId ?? 0,
                    AuthorName = Server.HtmlEncode(o.AuthorLastName) + " " + Server.HtmlEncode(o.AuthorFirstName),
                }).ToList();

            var parser = new DataTableParser<ItemBankViewModel>();
            return Json(parser.Parse(data.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadItemBanksForNetworkAdmin(int? stateId, int? districtId, bool? hideTeacherBanks, bool? hideOtherPeopleBanks, int? itemBankId)
        {
            var data = new List<QtiBankCustom>();
            if (districtId.HasValue)
            {
                //get data for the district only
                data = parameters.QtiBankServices.GetItemBanks(CurrentUser.Id, CurrentUser.RoleId,
                    districtId.GetValueOrDefault(), hideTeacherBanks, hideOtherPeopleBanks).ToList();
            }

            var result = data.DistinctBy(o => o.QTIBankId)
                .Select(o => new ItemBankViewModel()
                {
                    Name = Server.HtmlEncode(o.Name),
                    QTIBankId = o.QTIBankId,
                    AuthorGroup = Server.HtmlEncode(o.AuthorGroupName),
                    AuthorGroupId = o.AuthorGroupId ?? 0,
                    AuthorName = Server.HtmlEncode(o.AuthorLastName + " " + o.AuthorFirstName),
                }).ToList();

            var parser = new DataTableParser<ItemBankViewModel>();
            return Json(parser.Parse(result.AsQueryable(), true), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteItemBank(int itemBankId)
        {
            if (itemBankId > 0)
            {
                //check the right of current user if he/she can update qti bank or not ( avoid someone modify ajax parameter itemBankId )
                if (!parameters.VulnerabilityService.HasRightToUpdateItemBank(CurrentUser, itemBankId))
                {
                    return Json(new { message = "Has no right to delete bank name.", success = false }, JsonRequestBehavior.AllowGet);
                }

                //Check if item bank has any item set
                if (parameters.QtiGroupServices.GetQtiGroupsByBank(itemBankId).Any())
                {
                    //Can not delete
                    return Json(new { message = "This bank has item sets associated with it. Please delete the item sets before removing the bank.", success = false }, JsonRequestBehavior.AllowGet);
                }

                //TODO: Check QtiBankId exist on QTIGroup table
                //if exist
                //var obj = parameters.QtiBankServices.GetById(itemBankId);
                //return Json(new { message = "This bank has item sets associated with it.  Please delete the item sets before removing the bank.", success = false }, JsonRequestBehavior.AllowGet);
                //else
                //if (CurrentUser.RoleId == (int)Permissions.Publisher ||
                //    CurrentUser.RoleId == (int)Permissions.DistrictAdmin ||
                //    CurrentUser.RoleId == (int)Permissions.SchoolAdmin ||
                //    CurrentUser.Id == obj.UserId)
                //{
                //    parameters.QtiBankServices.Delete(itemBankId);
                //    return Json(true, JsonRequestBehavior.AllowGet);
                //}
                //return Json(new { message = "Insufficient privileges", success = false }, JsonRequestBehavior.AllowGet);

                // Allow user to remove any viewable item
                parameters.QtiBankServices.Delete(itemBankId);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "Invalid Item Bank", success = false }, JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult RemoveAuthorGroup(int itemBankId, int authorGroupId)
        {
            if (itemBankId > 0 && authorGroupId > 0)
            {
                var obj = parameters.QtiBankServices.GetById(itemBankId);
                if (obj != null)
                {
                    obj.AuthorGroupId = null;
                    parameters.QtiBankServices.Save(obj);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { message = "An error has occurred.  Please try again.", success = false },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditItemBank(int itemBankId)
        {
            var obj = parameters.QtiBankServices.GetById(itemBankId);

            ViewBag.OriginalAuthor = "";
            var originalAuthor = parameters.UserService.GetUserById(obj.UserId);
            if (originalAuthor != null)
            {
                ViewBag.OriginalAuthor = originalAuthor.LastName +
                                         (string.IsNullOrEmpty(originalAuthor.FirstName)
                                              ? ""
                                              : ", " + originalAuthor.FirstName) +
                                         (string.IsNullOrEmpty(originalAuthor.UserName)
                                              ? ""
                                              : " (" + originalAuthor.LastName + ")");
            }
            obj.ModifiedDate = parameters.QtiBankServices.GetQTIBankModifiedDate(itemBankId);

            return PartialView("_EditItemBank", obj);
        }

        public ActionResult LoadCreateItemBank()
        {
            return PartialView("_CreateItemBank");
        }

        public ActionResult LoadMoveItemSet(int itemSetId, string itemSetName)
        {
            //avoid someone modify ajax parameter
            ViewBag.ErrorMessage = string.Empty;
            var obj = parameters.QtiGroupServices.GetById(itemSetId);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "Item set does not exist.";
                return PartialView("_MoveItemSet");
            }

            //Check if current user has right to move this item set

            if (
                !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, obj))
            {
                ViewBag.ErrorMessage = "Has no right to move this item set.";
                return PartialView("_MoveItemSet");
            }
            ViewBag.ItemSetId = itemSetId;
            ViewBag.ItemSetName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(itemSetName));//ViewBag.ItemSetName is the original name
            return PartialView("_MoveItemSet");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateItemBank(QtiBank objBank)
        {
            if (objBank != null)
            {
                //check if the name is null or empty ( incase someone modify ajax parameter )
                if (string.IsNullOrWhiteSpace(objBank.Name))
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = "Please input bank name!" } }, success = false },
                                JsonRequestBehavior.AllowGet);
                }
                bool isExistBankName = parameters.QtiBankServices.CheckExistBankName(objBank.QtiBankId, objBank.Name,
                                                                                     CurrentUser.Id);
                if (!isExistBankName)
                {
                    var obj = parameters.QtiBankServices.GetById(objBank.QtiBankId);
                    if (obj != null)
                    {
                        //check the right of current user if he/she can update qti bank or not ( avoid someone modify ajax parameter QtiBankID )
                        if (!parameters.VulnerabilityService.HasRightToUpdateItemBank(CurrentUser, objBank.QtiBankId))
                        {
                            return
                                Json(new { ErrorList = new[] { new { ErrorMessage = "Has no right to update bank name!" } }, success = false },
                                    JsonRequestBehavior.AllowGet);
                        }

                        obj.Name = objBank.Name;
                        parameters.QtiBankServices.Save(obj);
                        return Json(true);
                    }
                    else
                    {
                        return
                            Json(new { ErrorList = new[] { new { ErrorMessage = "Bank does not exist." } }, success = false },
                                JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { ErrorList = new[] { new { ErrorMessage = "An item bank with that name already exists. Please use a different name." } }, success = false },
                            JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { ErrorList = new[] { new { ErrorMessage = "invalid bank info!" } }, success = false },
                        JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateItemBankOfUser(string bankName, int userId, int districtId)
        {
            bool isExistBankName = parameters.QtiBankServices.CheckExistBankName(null, bankName, userId);
            if (!isExistBankName)
            {
                var obj = new QtiBank()
                {
                    Name = bankName,
                    AccessId = 3,
                    UserId = userId,
                    //DistrictId = districtId
                };
                parameters.QtiBankServices.Save(obj);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ErrorList = new[] { new { ErrorMessage = "exist bank name!" } }, success = false },
                        JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateItemBank(string bankName)
        {
            bankName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(bankName));
            bool isExistBankName = parameters.QtiBankServices.CheckExistBankName(null, bankName, CurrentUser.Id);
            if (!isExistBankName)
            {
                var obj = new QtiBank()
                {
                    Name = bankName,
                    AccessId = 3,
                    UserId = CurrentUser.Id
                };
                parameters.QtiBankServices.Save(obj);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ErrorList = new[] { new { ErrorMessage = "An item bank with that name already exists. Please use a different name." } }, success = false },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadItemSets(int itemBankId, int? districtId)
        {
            if (itemBankId == 0)
            {
                var emptyData = new List<ItemSetViewModel>();
                return Json(new DataTableParser<ItemSetViewModel>().Parse(emptyData.AsQueryable(), true), JsonRequestBehavior.AllowGet);
            }

            //check the right of current user if he/she can update qti bank or not ( avoid someone modify ajax parameter itemBankId )
            if (!parameters.VulnerabilityService.HasRightToUpdateItemBank(CurrentUser, itemBankId, districtId.GetValueOrDefault()))
            {
                var emptyData = new List<ItemSetViewModel>();
                return Json(new DataTableParser<ItemSetViewModel>().Parse(emptyData.AsQueryable(), true), JsonRequestBehavior.AllowGet);
            }

            var districtIdFilter = CurrentUser.DistrictId.GetValueOrDefault();
            if (districtId.HasValue && districtId.Value > 0)
            {
                districtIdFilter = districtId.GetValueOrDefault();
            }
            var data =
                parameters.QtiGroupServices.GetListQtiGroupByQtiBankId(itemBankId, CurrentUser.Id, CurrentUser.RoleId,
                                                                      districtIdFilter).
                    ToList()
                    .Select(o => new ItemSetViewModel()
                    {
                        Name = Server.HtmlEncode(o.Name),
                        QTIGroupId = o.QtiGroupId,
                        AuthorGroup = Server.HtmlEncode(o.AuthorGroupName),
                        AuthorGroupId = o.AuthorGroupId ?? 0,
                        AuthorName = Server.HtmlEncode(o.AuthorLastName + " " + o.AuthorFirstName)
                    }).AsQueryable();
            var parser = new DataTableParser<ItemSetViewModel>();
            return Json(parser.Parse(data, true), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DeleteItemSet(int itemSetId)
        {
            if (itemSetId > 0)
            {
                //avoid someone modify ajax parameter
                var obj = parameters.QtiGroupServices.GetById(itemSetId);
                if (obj == null)
                {
                    return Json(new { message = "Item set does not exist.", success = false }, JsonRequestBehavior.AllowGet);
                }
                if (
                   !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, obj))
                {
                    return Json(new { message = "Has no right to delete this item set.", success = false }, JsonRequestBehavior.AllowGet);
                }

                //var obj = parameters.QtiGroupServices.GetById(itemSetId);
                //if (CurrentUser.RoleId == (int)Permissions.Publisher ||
                //    CurrentUser.RoleId == (int)Permissions.DistrictAdmin ||
                //    CurrentUser.RoleId == (int)Permissions.SchoolAdmin ||
                //    CurrentUser.Id == obj.UserId)
                //{
                //    string str = parameters.QtiGroupServices.DeleteItemSetAndItems(itemSetId, CurrentUser.Id);
                //    if (str.Equals("Group deleted"))
                //    {
                //        return Json(true, JsonRequestBehavior.AllowGet);
                //    }
                //    return Json(new { message = str, success = false }, JsonRequestBehavior.AllowGet);
                //}
                //return Json(new { message = "Insufficient privileges", success = false }, JsonRequestBehavior.AllowGet);

                // Allow user to remove any viewable item
                string str = parameters.QtiGroupServices.DeleteItemSetAndItems(itemSetId, CurrentUser.Id);
                if (str.Equals("Group deleted"))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(new { message = str, success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "Invalid Item Set", success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditItemSet(int itemSetId)
        {
            var obj = parameters.QtiGroupServices.GetById(itemSetId);

            ViewBag.OriginalAuthor = "";
            var originalAuthor = parameters.UserService.GetUserById(obj.UserId);
            if (originalAuthor != null)
            {
                ViewBag.OriginalAuthor = originalAuthor.LastName +
                                         (string.IsNullOrEmpty(originalAuthor.FirstName)
                                              ? ""
                                              : ", " + originalAuthor.FirstName) +
                                         (string.IsNullOrEmpty(originalAuthor.UserName)
                                              ? ""
                                              : " (" + originalAuthor.LastName + ")");
            }
            obj.ModifiedDate = parameters.QtiGroupServices.GetQTIGroupModifiedDate(itemSetId, obj.ModifiedDate);

            return PartialView("_EditItemSet", obj);
        }

        public ActionResult LoadCreateItemSet()
        {
            return PartialView("_CreateItemSet");
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult CreateItemSet(string itemSetName, int iQtiBankId)
        {
            itemSetName = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(itemSetName));
            //avoid someone modify ajax parameter
            var qtiBank = parameters.QtiBankServices.GetById(iQtiBankId);
            if (qtiBank == null)
            {
                return Json(new { message = "Bank does not exist.", success = false }, JsonRequestBehavior.AllowGet);
            }
            if (!parameters.VulnerabilityService.HasRightToUpdateItemBank(CurrentUser, iQtiBankId))
            {
                return Json(new { message = "Has no right to create item set for this item bank.", success = false }, JsonRequestBehavior.AllowGet);
            }

            bool isExistSetName = parameters.QtiGroupServices.CheckExistSetName(itemSetName, CurrentUser.Id, iQtiBankId,
                                                                                null);
            if (iQtiBankId > 0 && !string.IsNullOrEmpty(itemSetName) && !isExistSetName)
            {
                var obj = new QtiGroup()
                {
                    Name = itemSetName,
                    Source = "itemset_temp",
                    Type = "normal",
                    AccessId = 1,
                    UserId = CurrentUser.Id,
                    QtiBankId = iQtiBankId,
                    OwnershipType = 1
                };
                parameters.QtiGroupServices.Save(obj);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "An item set with that name already exists in this item bank. Please use a different name.", success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult UpdateItemSet(QtiGroup objSet)
        {
            if (objSet != null)
            {
                //avoid someone modify ajax parameter
                var obj = parameters.QtiGroupServices.GetById(objSet.QtiGroupId);
                if (obj == null)
                {
                    return
                        Json(new { ErrorList = new[] { new { ErrorMessage = "Item set does not exist." } }, success = false },
                            JsonRequestBehavior.AllowGet);
                }

                var newName = objSet.Name;
                if (string.IsNullOrWhiteSpace(newName))
                {
                    return
                        Json(
                            new
                            {
                                ErrorList = new[] { new { ErrorMessage = "Please input new name for item set." } },
                                success = false
                            },
                            JsonRequestBehavior.AllowGet);
                }

                //Check if current user has right to update this item set
                if (
                    !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, obj))
                {
                    return
                        Json(
                            new
                            {
                                ErrorList = new[] { new { ErrorMessage = "Has no right to update this item set." } },
                                success = false
                            },
                            JsonRequestBehavior.AllowGet);
                }
                bool isExistBankSet = parameters.QtiGroupServices.CheckExistSetName(newName,
                    CurrentUser.Id,
                    obj.QtiBankId.Value,
                    obj.QtiGroupId);
                if (!isExistBankSet)
                {
                    obj.Name = objSet.Name;
                    parameters.QtiGroupServices.Save(obj);
                    return Json(true);
                }
                else
                {
                    return
                       Json(
                           new
                           {
                               ErrorList = new[] { new { ErrorMessage = "An item set with that name already exists in this item bank. Please use a different name." } },
                               success = false
                           },
                           JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { ErrorList = new[] { new { ErrorMessage = "invalid set info!" } }, success = false },
                        JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult ItemSetRemoveAuthorGroup(int itemSetId, int authorGroupId)
        {
            if (itemSetId > 0 && authorGroupId > 0)
            {
                var obj = parameters.QtiGroupServices.GetById(itemSetId);
                if (obj != null)
                {
                    obj.AuthorGroupId = null;
                    parameters.QtiGroupServices.Save(obj);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { message = "An error has occurred.  Please try again.", success = false },
                        JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddAuthorGroupById(int id, int authorGroupId, string type)
        {
            if (type.Equals(Util.ItemBankConstant, StringComparison.CurrentCultureIgnoreCase))
            {
                //TODO: Add group to item bank.
                parameters.QtiBankServices.UpdateAuthorGroupId(id, authorGroupId);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else if (type.Equals(Util.ItemSetConstant, StringComparison.CurrentCultureIgnoreCase))
            {
                //TODO: Add group to item bank.
                parameters.QtiGroupServices.UpdateAuthorGroupId(id, authorGroupId);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "An error has occurred.  Please try again.", success = false },
                        JsonRequestBehavior.AllowGet);
        }

        //TODO: List Items from library to add to Item Set
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.Testdesign)]
        public ActionResult ItemsFromLibrary()
        {
            return View();
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.Testdesign)]
        public ActionResult ShowAddItemsFromLibrary(int qTIItemGroupID)
        {
            var xliAccess = parameters.AuthItemLibService.GetXliFunctionAccess(CurrentUser.Id, CurrentUser.RoleId, CurrentUser.DistrictId ?? 0);
            ViewBag.XliFunctionAccess = xliAccess;
            ViewBag.LibraryTypeResult =
            ViewBag.QIItemGroupID = qTIItemGroupID;
            ViewBag.IsPublisher = CurrentUser.IsPublisher();
            ViewBag.DistrictId = CurrentUser.DistrictId;

            var qtiGroupName = "";
            //Check permission
            var hasPermission = parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, qTIItemGroupID);
            if (!hasPermission)
            {
                return RedirectToAction("Index", "ItemBank");
            }

            var qtiGroup = parameters.QtiGroupServices.GetById(qTIItemGroupID);
            if (qtiGroup == null)
            {
                return RedirectToAction("Index", "ItemBank");
            }
            ViewBag.QtiGroupName = qtiGroup.Name;
            return View("ItemsFromLibrary");
        }

        public ActionResult LoadItemFromLibraryByFilter(QTI3pItemFilters obj)
        {
            obj.Searchkey = obj.Searchkey.DecodeParameters();
            obj.PassageSubject = obj.PassageSubject.DecodeParameters();
            obj.Subject = obj.Subject.DecodeParameters();
            obj.Difficulty = obj.Difficulty.DecodeParameters();
            obj.Blooms = obj.Blooms.DecodeParameters();
            obj.PassageTitle = string.Empty;//now search title in Passage Name dropdownlist ( use obj.PassageId )
            obj.ItemTitle = obj.ItemTitle?.DecodeParameters();
            obj.ItemDescription = obj.ItemDescription?.DecodeParameters();

            var parser = new DataTableParserProc<ItemLibraryViewModel>();
            if (Request["bSortable_0"] == null || (obj.FirstLoadListItemsFromLibrary.HasValue && obj.FirstLoadListItemsFromLibrary.Value))
            {
                var emptyResult = new List<ItemLibraryViewModel>();
                return Json(parser.Parse(emptyResult.AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }

            var sortColumns = parser.SortableColumns;
            var searchColumns = parser.SearchableColumns;

            if (obj == null)
                obj = new QTI3pItemFilters();

            if (string.IsNullOrEmpty(sortColumns))
            {
                sortColumns = "QTI3pItemID asc"; //alwasy need atleast one sortable column for paging
            }
            
            if (obj.Subject != null && obj.Subject.Equals("All"))
            {
                obj.Subject = string.Empty;
            }
            if (obj.Difficulty != null && obj.Difficulty.Equals("All"))
            {
                obj.Difficulty = string.Empty;
            }
            if (obj.Blooms != null && obj.Blooms.Equals("All"))
            {
                obj.Blooms = string.Empty;
            }
            if (obj.QTI3pItemLanguage != null && obj.QTI3pItemLanguage.ToLower().Equals("all"))
            {
                obj.QTI3pItemLanguage = string.Empty;
            }
            if (obj.QTI3pPassageLanguage != null && obj.QTI3pPassageLanguage.ToLower().Equals("all"))
            {
                obj.QTI3pPassageLanguage = string.Empty;
            }
            obj.CurrentUserId = CurrentUser.Id;
            if (obj.PassageId < 0)
            {
                obj.PassageId = 0;
            }

            var strDistrictIds = string.Empty;
            if (obj.Qti3pSourceId.HasValue && obj.Qti3pSourceId.Value == (int)QTI3pSourceEnum.Mastery)
            {
                var districtIds = new List<int>();
                if (CurrentUser.IsNetworkAdmin)
                {
                    districtIds = CurrentUser.GetMemberListDistrictId();
                }
                else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
                {
                    districtIds.Add(CurrentUser.DistrictId.Value);
                }
                if (districtIds.Any())
                    strDistrictIds = string.Format(",{0},", string.Join(",", districtIds));
            }

            // Check international customer
            obj.IsRestricted = CheckIsInternationalState(CurrentUser.Id);
            obj.IgnoreFilterByPassage();
            obj.StateStandardIdString = obj.StateStandardIdString.ToIntCommaSeparatedString();
            var data =
                parameters.QTI3Services.GetQti3PItemsByFilter(obj, parser.StartIndex, parser.PageSize, sortColumns,
                                                              searchColumns, parser.SearchInBoxXML, strDistrictIds).ToList();

            var qti3pItemIdList = data.Select(x => x.QTI3pItemID).ToList();

            var userStateIdList = parameters.StateServices.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);

            List<Qti3pItemStandardXml> masterStandardXML = parameters.MasterStandardService.GetQti3pItemStandardXml(qti3pItemIdList);
            Dictionary<int, List<string>> dicMasterStandard = Util.ParseStandardNumberQti3pItem(masterStandardXML, CurrentUser.RoleId, userStateIdList);

            int totalRow = 0;
            if (data.Count > 0)
            {
                totalRow = data[0].TotalRow;
            }

            var result = data.Select(x => new ItemLibraryViewModel
            {
                QTI3pItemID = x.QTI3pItemID,
                Name = AdjustXmlContent(x.XmlContent, true, obj.Qti3pSourceId, x.UrlPath),
                ToolTip =
                                                      BuildToolTipItem(x.QTI3pItemID, x.Subject, x.Difficulty, x.BloomsTaxonomy,
                                                                       x.GradeName, x.PassageNumber,
                                                                       x.PassageGrade, x.PassageSubject,
                                                                       x.PassageWordCount, x.PassageTextType,
                                                                       x.PassageTextSubType, x.PassageFleschKinkaid, x.Qti3pItemDOK, dicMasterStandard),
                From3pUpload = x.From3pUpload,
            }).AsQueryable();

            return Json(parser.Parse(result, totalRow), JsonRequestBehavior.AllowGet);
        }

        private bool CheckIsInternationalState(int userId)
        {
            var isInternational = false;

            var schoolIDs = parameters.UserSchoolService.GetListSchoolIdByUserId(userId);

            var stateIds = parameters.SchoolService.GetAll().Where(m => schoolIDs.Contains(m.Id)).Select(m => m.StateId).Distinct().ToList();

            var currentStateId = parameters.DistrictService.GetStateIdByDictricIds(new List<int> { CurrentUser.DistrictId.GetValueOrDefault() }).FirstOrDefault();

            stateIds.Add(currentStateId);

            if (stateIds.Any())
            {
                isInternational = parameters.StateServices.GetStatesByIds(stateIds.Where(s => s.HasValue).Cast<int>().ToList())
                    .ToList().Any(m => m.International == true);
            }

            return isInternational;
        }

        private string AdjustXmlContent(string xmlContent, bool isThirdParty, int? qti3pSourceId, string urlPath)
        {
            xmlContent = xmlContent.ReplaceWeirdCharacters();
            xmlContent = Util.UpdateS3LinkForItemMedia(xmlContent);
            xmlContent = Util.UpdateS3LinkForPassageLink(xmlContent);
            if (isThirdParty)
            {
                if (qti3pSourceId.HasValue && qti3pSourceId.Value == (int)QTI3pSourceEnum.Mastery)
                {
                    xmlContent = HtmlUtils.UpdateImageUrls(urlPath, xmlContent);
                }
            }

            xmlContent = ItemSetPrinting.AdjustXmlContentFloatImg(xmlContent);
            xmlContent = Util.ReplaceTagListByTagOl(xmlContent);
            xmlContent = Util.ReplaceVideoTag(xmlContent);
            xmlContent = XmlUtils.RemoveAllNamespacesPrefix(xmlContent);
            return xmlContent;
        }

        private string BuildToolTipItem(int qti3pItemId, string subject, string difficulty, string bloomsTaxonomy, string gradeName,
                                        string passageNumber, string passageGrade,
                                        string passageSubject, string passageWordCount, string passageTextType,
                                        string passageTextSubType, string passageFleschKinkaid, string qti3pItemDOK, Dictionary<int, List<string>> dicMasterStandard, string standardNumbers = null)
        {
            StringBuilder tooltip = new StringBuilder();
            tooltip.Append(string.Format("- QTI 3P Item ID: {0} <br>", qti3pItemId));
            if (!string.IsNullOrEmpty(subject))
            {
                tooltip.Append(string.Format("- Subject: {0}", Server.HtmlEncode(subject)));
            }
            if (!string.IsNullOrEmpty(difficulty))
            {
                tooltip.Append(string.Format("<br>- Difficulty: {0}", Server.HtmlEncode(difficulty)));
            }
            if (!string.IsNullOrEmpty(bloomsTaxonomy))
            {
                tooltip.Append(string.Format("<br>- Blooms Taxonomy: {0}", Server.HtmlEncode(bloomsTaxonomy)));
            }
            if (!string.IsNullOrEmpty(gradeName))
            {
                tooltip.Append(string.Format("<br>- " + LabelHelper.GradeLabel + ": {0}", Server.HtmlEncode(gradeName)));
            }

            if (dicMasterStandard != null && dicMasterStandard.ContainsKey(qti3pItemId))
            {
                var assignedStandards = dicMasterStandard[qti3pItemId];
                if (assignedStandards != null && assignedStandards.Count > 0)
                {
                    tooltip.Append("<br>- Standards: ");
                    var standardDisplayed = string.Format(", {0}", string.Join(", ", assignedStandards));
                    standardDisplayed = Server.HtmlEncode(standardDisplayed);
                    if (!string.IsNullOrEmpty(standardDisplayed))
                    {
                        if (standardDisplayed.StartsWith(","))
                        {
                            standardDisplayed = standardDisplayed.Remove(0, 1);
                        }
                    }
                    tooltip.Append(standardDisplayed);
                }
            }
            else if (!string.IsNullOrEmpty(standardNumbers))
            {
                tooltip.Append($"<br>- Standards: {standardNumbers}");
            }

            if (!string.IsNullOrEmpty(qti3pItemDOK))
            {
                tooltip.Append(string.Format("<br>- DOK(s): {0}", Server.HtmlEncode(qti3pItemDOK)));
            }
            if (!string.IsNullOrEmpty(passageNumber))
            {
                tooltip.Append(string.Format("<br>- Passage: {0}", ""));
                tooltip.Append(string.Format("<br>&nbsp;&nbsp;+ Number: {0}", Server.HtmlEncode(passageNumber)));
            }
            if (!string.IsNullOrEmpty(passageGrade))
            {
                tooltip.Append(string.Format("<br>&nbsp;&nbsp;+ " + LabelHelper.GradeLabel + ": {0}", Server.HtmlEncode(passageGrade)));
            }
            if (!string.IsNullOrEmpty(passageSubject))
            {
                tooltip.Append(string.Format("<br>&nbsp;&nbsp;+ Subject: {0}", Server.HtmlEncode(passageSubject)));
            }
            if (!string.IsNullOrEmpty(passageWordCount))
            {
                tooltip.Append(string.Format("<br>&nbsp;&nbsp;+ Word Count: {0}", Server.HtmlEncode(passageWordCount)));
            }
            if (!string.IsNullOrEmpty(passageTextType))
            {
                tooltip.Append(string.Format("<br>&nbsp;&nbsp;+ Text Type: {0}", Server.HtmlEncode(passageTextType)));
            }
            if (!string.IsNullOrEmpty(passageTextSubType))
            {
                tooltip.Append(string.Format("<br>&nbsp;&nbsp;+ Text Sub Type: {0}", Server.HtmlEncode(passageTextSubType)));
            }
            if (!string.IsNullOrEmpty(passageFleschKinkaid))
            {
                tooltip.Append(string.Format("<br>&nbsp;&nbsp;+ Flesch Kincaid: {0}", Server.HtmlEncode(passageFleschKinkaid)));
            }

            return tooltip.ToString();
        }

        public ActionResult GetItemBanks()
        {
            var data = parameters.QtiBankServices.GetItemBanks(CurrentUser.Id, CurrentUser.RoleId,
                                                               CurrentUser.DistrictId.GetValueOrDefault(), null, null).ToList()
                .Select(o => new ListItem() { Id = o.QTIBankId, Name = o.Name }).OrderBy(x => x.Name);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemBanksPersonal()
        {
            var result = new List<ListItem>();
            if (CurrentUser.IsPublisher)
            {
                result =
                    parameters.QtiBankServices.GetAll().Select(o => new ListItem() { Id = o.QtiBankId, Name = o.Name }).
                        OrderBy(x => x.Name).ToList();
            }
            else
            {
                var data = parameters.QtiBankServices.GetItemBanksPersonal(CurrentUser.Id,
                                                                           CurrentUser.DistrictId.GetValueOrDefault());
                result = data.Select(o => new ListItem() { Id = o.QtiBankId, Name = o.Name }).
                        OrderBy(x => x.Name).ToList();
            }

            return new LargeJsonResult
            {
                Data = result,
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult GetItemDistrict()
        {
            var data = parameters.QtiBankServices.GetQtiBankDistricts(CurrentUser.RoleId, CurrentUser.DistrictId.GetValueOrDefault(), CurrentUser.DistrictId.GetValueOrDefault(), SessionManager.ListDistrictId)
                 .Select(o => new ListItem() { Id = o.QtiBankId, Name = o.Name }).OrderBy(x => x.Name);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemBanksPersonalAndDistrict()
        {
            var result = new List<ListItem>();
            if (CurrentUser.IsPublisher)
            {
                result =
                    parameters.QtiBankServices.GetAll().Select(o => new ListItem() { Id = o.QtiBankId, Name = o.Name }).ToList();
            }
            else
            {
                var data = parameters.QtiBankServices.GetItemBanksPersonal(CurrentUser.Id,
                                                                           CurrentUser.DistrictId.GetValueOrDefault());
                result = data.Select(o => new ListItem() { Id = o.QtiBankId, Name = o.Name }).ToList();
            }
            var bankDistricts = parameters.QtiBankServices.GetQtiBankDistricts(CurrentUser.RoleId, CurrentUser.DistrictId.GetValueOrDefault(), CurrentUser.DistrictId.GetValueOrDefault(), SessionManager.ListDistrictId)
                 .Select(o => new ListItem() { Id = o.QtiBankId, Name = o.Name }).ToList();
            result.AddRange(bankDistricts);
            result = result.DistinctBy(x => x.Id).OrderBy(x => x.Name).ToList();

            return new LargeJsonResult
            {
                Data = result,
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpGet]
        public ActionResult GetCriteriaSchemaQtiItem(int? ItemBankID, int? QtiGroupID, bool IsGet3pItem = false, bool IsGetByPersonal = false, bool IsGetByDistrict = false)
        {
            var listSchemaID = new List<int>();
            if (IsGet3pItem)
            {
                var strDistrictIds = string.Empty;
                var districtIds = new List<int>();
                if (CurrentUser.IsNetworkAdmin)
                {
                    districtIds = CurrentUser.GetMemberListDistrictId();
                }
                else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
                {
                    districtIds.Add(CurrentUser.DistrictId.Value);
                }
                if (districtIds.Any())
                    strDistrictIds = string.Format(",{0},", string.Join(",", districtIds));
                listSchemaID = parameters.QTI3Services.GetCriteriaSchemaQti3pItem(strDistrictIds).Select(x => x.QTISchemaID).ToList();
            }
            else
            {
                listSchemaID = parameters.QTI3Services.GetCriteriaSchemasQtiItem(CurrentUser.Id, CurrentUser.DistrictId.Value, ItemBankID, QtiGroupID, IsGetByPersonal, IsGetByDistrict).Select(x => x.QTISchemaID).ToList();
            }
            var listItemType = this.GetListItemTypeByQtiSchemaID(listSchemaID);

            return Json(listItemType, JsonRequestBehavior.AllowGet);
        }

        private List<ListItem> GetListItemTypeByQtiSchemaID(List<int> listQtiSchemaID)
        {
            var listItemTypeDefault = new List<ListItem>()
            {
                new ListItem{ Id = 1, Name = "Multiple Choice and True/False"},
                new ListItem{ Id = 37, Name = "Multiple Choice Variable"},
                new ListItem{ Id = 8, Name = "Inline Choice"},
                new ListItem{ Id = 9, Name = "Fill-in-the-Blank"},
                new ListItem{ Id = 10, Name = "Extended Text and Drawing Response"},
                new ListItem{ Id = 21, Name = "Multi-Part"},
                new ListItem{ Id = 30, Name = "Drag-and-Drop"},
                new ListItem{ Id = 31, Name = "Text Hot Spot"},
                new ListItem{ Id = 32, Name = "Image Hot Spot"},
                new ListItem{ Id = 33, Name = "Table Hot Spot"},
                new ListItem{ Id = 34, Name = "Number Line Hot Spot"},
                new ListItem{ Id = 35, Name = "Drag and Drop Numerical"},
                new ListItem{ Id = 36, Name = "Drag-and-Drop Sequence/Order"},
            };

            return listItemTypeDefault.Where(x => listQtiSchemaID.Contains(x.Id)).ToList();
        }

        public ActionResult GetQTI3PSubject()
        {
            var data = parameters.QTI3Services.GetQti3PSubjects().Select(o => new ListItem()
            {
                Id = o.SubjectID,
                Name = o.Name
            }).ToList();
            var districtIds = new List<int>();
            if (CurrentUser.IsNetworkAdmin)
            {
                districtIds = CurrentUser.GetMemberListDistrictId();
            }
            else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                districtIds.Add(CurrentUser.DistrictId.Value);
            }
            if (districtIds.Count > 0)
            {
                var exceptSubjects =
                    parameters.QTI3Services.GetSubjectLicenses(districtIds).Select(x => x.Subject).ToList();
                if (exceptSubjects.Any())
                    data = data.Where(x => !exceptSubjects.Contains(x.Name)).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQTI3PDifficulty()
        {
            var data = parameters.QTI3Services.GetQti3pDifficulties().Select(o => new ListItem()
            {
                Id = o.ItemDifficultyID,
                Name = o.Name
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQTI3PBlooms()
        {
            var data = parameters.QTI3Services.GetQti3pBloomses().Select(o => new ListItem()
            {
                Id = o.BloomsId,
                Name = o.Name
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQTI3PPassageNumber(int qti3pSourceID)
        {
            var data = parameters.QTI3Services.GetQTI3PPassageNumber(qti3pSourceID).ToList();
            return Json(data.Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQTI3PFleschKincaid()
        {
            var data = parameters.QTI3Services.GetFleschKinkaids().Select(o => new ListItem()
            {
                Id = o.FleschKincaidID,
                Name = o.Name
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQTI3PTextSubType()
        {
            var data = parameters.QTI3Services.GetQti3PTextTypes(2).Select(o => new ListItem()
            {
                Id = o.TextTypeID,
                Name = o.Name
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQTI3PTextType()
        {
            var data = parameters.QTI3Services.GetQti3PTextTypes(1).Select(o => new ListItem()
            {
                Id = o.TextTypeID,
                Name = o.Name
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQTI3PWordCount()
        {
            var data = parameters.QTI3Services.GetQti3PWordCounts().Select(o => new ListItem()
            {
                Id = o.WordCountID,
                Name = o.Name
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQTI3PPassageSubject()
        {
            var data = parameters.QTI3Services.GetPassageSubject().Select(o => new ListItemStr()
            {
                Id = o,
                Name = o
            }).ToList();

            var districtIds = new List<int>();
            if (CurrentUser.IsNetworkAdmin)
            {
                districtIds = CurrentUser.GetMemberListDistrictId();
            }
            else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                districtIds.Add(CurrentUser.DistrictId.Value);
            }

            if (districtIds.Count > 0)
            {
                var exceptSubjects =
                    parameters.QTI3Services.GetSubjectLicenses(districtIds).Select(x => x.Subject).ToList();
                if (exceptSubjects.Any())
                    data = data.Where(x => !exceptSubjects.Contains(x.Name)).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        public ActionResult GetStateStandardSubjectByState(int? stateId, bool qti3p, string personal, int? qti3pSourceId, string districtSearch)
        {
            var data = new List<ListItem>();
            if (qti3p)
            {
                if (stateId.HasValue)
                {
                    var obj = parameters.StateServices.GetStateById(stateId);
                    data = parameters.QTI3Services.GetStateStandardSubjectsForItem3pLibraryFilter(obj.Code, qti3pSourceId).Select(o => new ListItem()
                    {
                        Id = o.StateId,
                        Name = o.SubjectName
                    }).ToList();

                    if (qti3pSourceId.HasValue && qti3pSourceId == (int)QTI3pSourceEnum.Mastery)
                    {
                        var districtIds = new List<int>();
                        if (CurrentUser.IsNetworkAdmin)
                        {
                            districtIds = CurrentUser.GetMemberListDistrictId();
                        }
                        else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
                        {
                            districtIds.Add(CurrentUser.DistrictId.Value);
                        }

                        if (districtIds.Count > 0)
                        {
                            var exceptSubjects =
                                parameters.QTI3Services.GetSubjectLicenses(districtIds).Select(x => x.Subject).ToList();
                            if (exceptSubjects.Any())
                                data = data.Where(x => !exceptSubjects.Contains(x.Name)).ToList();
                        }
                    }
                }
            }
            else
            {
                if (stateId.HasValue)
                {
                    var obj = parameters.StateServices.GetStateById(stateId);
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
                    data = parameters.QTIITemServices.GetStateStandardSubjectsForItemLibraryFilter(obj.Code, userId, districtId).Select(o => new ListItem()
                    {
                        Id = o.StateId,
                        Name = o.SubjectName
                    }).ToList();
                }
            }
            data = data.OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemSets(int? itemBankId)
        {
            if (itemBankId.HasValue)
            {
                var data = parameters.QtiGroupServices.GetQtiGroupsByBank(itemBankId ?? 0).ToList()
                        .Select(o => new ListItem()
                        {
                            Id = o.QtiGroupId,
                            Name = o.Name
                        }).AsQueryable().OrderBy(x => x.Name);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = new object();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [UrlReturnDecode]
        public ActionResult GetStateStandardGradeByStateAndSubject(int? stateId, string subjectName, bool qti3p, string personal, int? qti3pSourceId, string districtSearch)
        {
            var data = new List<ListItem>();
            if (!qti3pSourceId.HasValue)
            {
                qti3pSourceId = 0;
            }
            subjectName = HttpUtility.UrlDecode(subjectName);
            if (qti3p)
            {
                if (stateId.HasValue && !string.IsNullOrEmpty(subjectName))
                {
                    var obj = parameters.StateServices.GetStateById(stateId);
                    data =
                        parameters.QTI3Services.GetGradeByStateCodeAndSubject(obj.Code, subjectName, qti3pSourceId.Value)
                            .Select(
                                o => new ListItem()
                                {
                                    Id = o.GradeID,
                                    Name = o.GradeName
                                }).ToList();

                    if (qti3pSourceId.Value == (int)QTI3pSourceEnum.Mastery)
                    {
                        var districtIds = new List<int>();
                        if (CurrentUser.IsNetworkAdmin)
                        {
                            districtIds = CurrentUser.GetMemberListDistrictId();
                        }
                        else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
                        {
                            districtIds.Add(CurrentUser.DistrictId.Value);
                        }

                        if (districtIds.Count > 0)
                        {
                            var exceptGrades =
                                parameters.QTI3Services.GetGradetLicenses(districtIds, subjectName)
                                    .Select(x => x.GradeID)
                                    .ToList();
                            if (exceptGrades.Any())
                                data = data.Where(x => !exceptGrades.Contains(x.Id)).ToList();
                        }
                    }
                }
            }
            else
            {
                if (stateId.HasValue && !string.IsNullOrEmpty(subjectName))
                {
                    var obj = parameters.StateServices.GetStateById(stateId);
                    int? userId = null;
                    int? districtId = null;
                    if (!string.IsNullOrEmpty(personal) && personal.Equals("true") &&
                        !string.IsNullOrEmpty(districtSearch) && districtSearch.Equals("true"))
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
                    data =
                        parameters.QTIITemServices.GetGradesByStateAndSubjectForItemLibraryFilter(obj.Code, subjectName,
                            userId, districtId).Select(
                                o => new ListItem()
                                {
                                    Id = o.GradeID,
                                    Name = o.GradeName
                                }).ToList();
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        public ActionResult GetStateStandardsByStateAndSubjectAndGrade(int? stateId, string subjectName,
                                                                       string gradeName)
        {
            var data = new List<ListItemExtra>();
            data.Insert(0, new ListItemExtra()
            {
                Id = 0,
                Name = "Select State Standard",
                Description = string.Empty
            });
            if (stateId.HasValue && !string.IsNullOrEmpty(subjectName) & !string.IsNullOrEmpty(subjectName))
            {
                var obj = parameters.StateServices.GetStateById(stateId);
                data =
                    parameters.QTI3Services.GetQti3PMasterStandardByStateSubjectAndGrade(obj.Code, subjectName,
                                                                                         gradeName).Select(
                                                                                             o => new ListItemExtra()
                                                                                             {
                                                                                                 Id =
                                                                                                              o.
                                                                                                              MasterStandardID,
                                                                                                 Name =
                                                                                                              o.Number,
                                                                                                 Description =
                                                                                                              o.
                                                                                                              Description
                                                                                             }).ToList();
                data.Insert(0, new ListItemExtra()
                {
                    Id = 0,
                    Name = "Select State Standard",
                    Description = string.Empty
                });
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        public ActionResult LoadItemFromLibraryByFilterNew(QtiItemFilters filter)
        {
            var parser = new DataTableParserProc<ItemLibraryQtiItemViewModel>();
            if (filter.IsDistrictSearch == false && filter.IsPersonalSearch == false)
            {
                var res = new List<ItemLibraryQtiItemViewModel>();
                return Json(parser.Parse(res.AsQueryable(), res.Count), JsonRequestBehavior.AllowGet);
            }

            filter.Keyword = filter.Keyword.DecodeParameters();
            filter.Topic = filter.Topic.DecodeParameters();
            filter.Skill = filter.Skill.DecodeParameters();
            filter.Other = filter.Other.DecodeParameters();
            filter.DistrictTag = filter.DistrictTag.DecodeParameters();
            filter.RefObjectTitle = filter.RefObjectTitle.DecodeParameters();
            filter.PassageSubject = filter.PassageSubject.DecodeParameters();
            filter.ItemDescription = filter.ItemDescription?.DecodeParameters();
            filter.ItemTitle = filter.ItemTitle?.DecodeParameters();

            if (Request["bSortable_0"] == null || (filter.FirstLoadListItemsFromLibraryNew.HasValue && filter.FirstLoadListItemsFromLibraryNew.Value))
            {
                var emptyResult = new List<ItemLibraryQtiItemViewModel>();
                return Json(parser.Parse(emptyResult.AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }
            string sortColumns = parser.SortableColumns;
            string searchColumns = parser.SearchableColumns;

            if (string.IsNullOrEmpty(sortColumns))
            {
                sortColumns = "QtiItemId asc"; //alwasy need atleast one sortable column for paging
            }

            if (!string.IsNullOrEmpty(filter.SelectedTags))
            {
                //remove the last ','
                filter.SelectedTags = filter.SelectedTags.Substring(0, filter.SelectedTags.Length - 1);
            }
            //Personal," + LabelHelper.DistrictLabel + " do not user Number, so use RefObjectTitle instead
            filter.PassageNumber = filter.RefObjectTitle;

            filter.IgnoreFilterByPassage();
            var data =
                parameters.QTIITemServices.GetQtiItemsByFilter(filter, CurrentUser.Id,
                                                               CurrentUser.DistrictId.GetValueOrDefault(),
                                                               parser.StartIndex, parser.PageSize, sortColumns,
                                                               searchColumns,
                                                               parser.SearchInBoxXML).ToList();
            int totalRow = 0;

            if (data.Count > 0)
            {
                totalRow = data[0].TotalRow;
            }

            var userStateIdList = parameters.StateServices.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);

            var refObjectIds = new List<int>();
            foreach (var qtiItem in data)
            {
                Util.GetPassageIdList(qtiItem.XmlContent.ReplaceWeirdCharacters(), ref refObjectIds);
            }

            var refObjects =
                parameters.PassageService.GetAll().Where(x => refObjectIds.Contains(x.QTIRefObjectID)).ToList();

            var qtiItems = data.Select(x => new ItemLibraryQtiItemViewModel
            {
                QtiItemId = x.QtiItemId,
                Name = AdjustXmlContent(x.XmlContent, false, null, null),
                ToolTip = BuildToolTipItemNew(x.XmlContent.ReplaceWeirdCharacters(),
                                                                          x.GroupName, x.BankName, x.Topic, x.Skill,
                                                                          x.Other, x.Standard, x.DistrictTag, userStateIdList, refObjects)
            }).ToList();

            var qtiItemIds = qtiItems.Select(x => new Nullable<int>(x.QtiItemId)).ToArray();
            var virtualQuestionOfQtiItems = parameters.VirtualQuestionServices.Select().Where(x => qtiItemIds.Contains(x.QTIItemID)).Select(x => new { x.QTIItemID, x.IsRubricBasedQuestion }).ToList();
            foreach (var item in qtiItems)
            {
                var countVirtualQuestion = virtualQuestionOfQtiItems.Count(x => x.QTIItemID == item.QtiItemId);
                var countVirtualQuestionRubric = virtualQuestionOfQtiItems.Count(x => x.QTIItemID == item.QtiItemId && x.IsRubricBasedQuestion == true);
                item.VirtualQuestionCount = countVirtualQuestion;
                item.VirtualQuestionRubricCount = countVirtualQuestionRubric;
            }

            var jsonResult = parser.Parse(qtiItems.AsQueryable(), totalRow);

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        private string BuildToolTipItemNew(string xmlContent, string groupName, string bankName, string topic,
                                           string skill, string other,
                                           string standard, string districtTag, List<int> userStateIdList, List<QtiRefObject> refObjects, int? qtyItemId= null)
        {
            StringBuilder tooltip = new StringBuilder();
            if (qtyItemId.HasValue)
                tooltip.Append(string.Format("- QTI Item ID: {0} <br>", qtyItemId.Value));
            tooltip.Append(string.Format("- Item Bank: {0}", bankName == null ? string.Empty : Server.HtmlEncode(bankName)));
            tooltip.Append(string.Format("<br>- Item Set: {0}", groupName == null ? string.Empty : Server.HtmlEncode(groupName)));

            //parse the passage
            try
            {
                List<string> passageNameList = Util.GetPassageNameListOptimize(xmlContent, parameters.Qti3pPassageService, refObjects, true);
                if (passageNameList.Count > 0)
                {
                    tooltip.Append("<br>- Passages: ");
                    tooltip.Append("<br> &nbsp;&nbsp;&nbsp;+&nbsp;" + Server.HtmlEncode(passageNameList[0]));

                    for (int i = 1; i < passageNameList.Count; i++)
                    {
                        tooltip.AppendLine(string.Format("<br> &nbsp;&nbsp;&nbsp;+&nbsp;{0}", Server.HtmlEncode(passageNameList[i])));
                    }
                }
            }
            catch (Exception)
            {
            }

            if (!string.IsNullOrEmpty(standard))
            {
                var standardDisplayed = BuildStandardNumber(standard, userStateIdList);
                if (!string.IsNullOrEmpty(standardDisplayed))
                {
                    tooltip.Append("<br>- Standards: ");
                    tooltip.Append(Server.HtmlEncode(standardDisplayed));
                }
            }

            if (!string.IsNullOrEmpty(topic))
            {
                tooltip.Append(string.Format("<br>- Topics: {0}", Server.HtmlEncode(topic)));
            }
            if (!string.IsNullOrEmpty(skill))
            {
                tooltip.Append(string.Format("<br>- Skills: {0}", Server.HtmlEncode(skill)));
            }
            if (!string.IsNullOrEmpty(other))
            {
                tooltip.Append(string.Format("<br>- Other: {0}", Server.HtmlEncode(other)));
            }

            Dictionary<string, List<string>> districtTagDic = ParseDistrictTag(districtTag);
            if (districtTagDic.Count > 0)
            {
                tooltip.Append("<br>" + LabelHelper.DistrictLabel + " Tag: ");
                foreach (var category in districtTagDic)
                {
                    tooltip.Append(string.Format("<br>- {0}: ", Server.HtmlEncode(category.Key)));
                    if (category.Value != null && category.Value.Count > 0)
                    {
                        tooltip.Append(category.Value[0]);
                        for (int i = 1; i < category.Value.Count; i++)
                        {
                            tooltip.Append(string.Format(", {0}", Server.HtmlEncode(category.Value[i])));
                        }
                    }
                }
            }

            return tooltip.ToString();
        }

        public Dictionary<string, List<string>> ParseDistrictTag(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return new Dictionary<string, List<string>>();
            var xdoc = XDocument.Parse(xml);
            Dictionary<string, List<string>> districtTagDic = new Dictionary<string, List<string>>();
            string category;
            string tag;
            foreach (var node in xdoc.Element("ItemTagList").Elements("ItemTag"))
            {
                category = GetStringValue(node.Element("Category"));
                tag = GetStringValue(node.Element("Tag"));
                if (!districtTagDic.ContainsKey(category))
                {
                    districtTagDic.Add(category, new List<string> { tag });
                }
                else
                {
                    districtTagDic[category].Add(tag);
                }
            }
            return districtTagDic;
        }

        private string GetStringValue(XElement element)
        {
            if (element == null) return string.Empty;
            return element.Value;
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddQTI3ToItemSet(int qti3pItemId, int qtiGroupId)
        {
            try
            {
                //avoid someone modify ajax parameter
                var obj = parameters.QtiGroupServices.GetById(qtiGroupId);
                if (obj == null)
                {
                    return Json(new { message = "Item set does not exist.", success = false }, JsonRequestBehavior.AllowGet);
                }
                if (
                   !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, obj))
                {
                    return Json(new { message = "Has no right to add items to this item set.", success = false }, JsonRequestBehavior.AllowGet);
                }

                if (qti3pItemId > 0 && qtiGroupId > 0)
                {
                    parameters.QTIITemServices.TMAddItemFromLibrary(qti3pItemId, qtiGroupId, CurrentUser.Id);
                }
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return
                    Json(
                        new
                        {
                            Success = "Fail",
                            errorMessage = string.Format("Fail to add QTI3 to item set, error detail:" + ex.Message)
                        },
                        JsonRequestBehavior.AllowGet);
            }
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddQTI3ToItemSetMany(string qti3pItemIdString, int qtiGroupId)
        {
            try
            {
                //avoid someone modify ajax parameter
                var obj = parameters.QtiGroupServices.GetById(qtiGroupId);
                if (obj == null)
                {
                    return Json(new { message = "Item set does not exist.", success = false }, JsonRequestBehavior.AllowGet);
                }
                if (
                   !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, obj))
                {
                    return Json(new { message = "Has no right to add items to this item set.", success = false }, JsonRequestBehavior.AllowGet);
                }

                if (qti3pItemIdString == null)
                {
                    qti3pItemIdString = string.Empty;
                }

                string[] idList = qti3pItemIdString.Split(',');

                var qti3pDictionary = new Dictionary<string, QTI3pItem>();
                foreach (var id in idList)
                    if (Int32.Parse(id) > 0 && qtiGroupId > 0)
                        qti3pDictionary.Add(id, parameters.QTIITemServices.GetQti3pItemById(Int32.Parse(id)));

                var isInvalidMultipart = qti3pDictionary.Values.Any(qti => qti.QTISchemaID == (int)QtiSchemaEnum.MultiPart && !Util.IsValidMultiPartXmlContent(qti.XmlContent));
                if (isInvalidMultipart)
                    return Json(new { message = TextConstants.IMPORT_INVALID_MULTIPART, success = false }, JsonRequestBehavior.AllowGet);

                foreach (var id in idList)
                {
                    int qti3pItemId = Int32.Parse(id);
                    var qti3pItem = qti3pDictionary[id];
                    if (qti3pItem != null)
                    {
                        if (qti3pItem.From3pUpload)
                        {
                            var qtiItem = parameters.QTIITemServices.ConvertFrom3pItemUploadedToItem(qti3pItemId, qtiGroupId, CurrentUser.Id, true,
                                     LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName,
                                     LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder);

                            UpdateItemPassage(qtiItem?.QTIItemID ?? 0);
                        }
                        else
                        {
                            parameters.QTIITemServices.TMAddItemFromLibrary(qti3pItemId, qtiGroupId, CurrentUser.Id);
                        }
                    }
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return
                    Json(
                        new
                        {
                            Success = "Fail",
                            errorMessage = string.Format("Fail to add QTI3 to item set, error detail:" + ex.Message)
                        },
                        JsonRequestBehavior.AllowGet);
            }
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddQTIToItemSet(int qtiItemId, int qtiGroupId)
        {
            try
            {
                //avoid someone modify ajax parameter
                var obj = parameters.QtiGroupServices.GetById(qtiGroupId);
                if (obj == null)
                {
                    return Json(new { message = "Item set does not exist.", success = false }, JsonRequestBehavior.AllowGet);
                }
                if (
                   !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, obj))
                {
                    return Json(new { message = "Has no right to add items to this item set.", success = false }, JsonRequestBehavior.AllowGet);
                }

                if (qtiItemId > 0 && qtiGroupId > 0)
                {
                    //TODO
                    parameters.QTIITemServices.DuplicateQTIItem(CurrentUser.Id, qtiItemId, qtiGroupId, true,
                        LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName,
                        LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder,
                        LinkitConfigurationManager.GetS3Settings().S3Domain);
                }
                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return
                    Json(
                        new
                        {
                            Success = "Fail",
                            errorMessage = string.Format("Fail to add QTI to item set, error detail:" + ex.Message)
                        },
                        JsonRequestBehavior.AllowGet);
            }
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddQTIToItemSetMany(string qtiItemIdString, int qtiGroupId)
        {
            try
            {
                //avoid someone modify ajax parameter
                var obj = parameters.QtiGroupServices.GetById(qtiGroupId);
                if (obj == null)
                {
                    return Json(new { message = "Item set does not exist.", success = false }, JsonRequestBehavior.AllowGet);
                }
                if (
                   !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, obj))
                {
                    return Json(new { message = "Has no right to add items to this item set.", success = false }, JsonRequestBehavior.AllowGet);
                }
                if (!parameters.VulnerabilityService.HasRightToAddQtiItems(CurrentUser, qtiItemIdString.ParseIdsFromString()))
                {
                    return Json(new { message = "Has no right on one or more items selected to added.", success = false }, JsonRequestBehavior.AllowGet);
                }

                //Need to upload media file (image,audio),if any, to S3
                var qtiItemIdStringNew = qtiItemIdString;

                var qtiItemIds = qtiItemIdString.ParseIdsFromStringAsNullableInt();
                var newQtiItemIds = parameters.VirtualQuestionServices.Select().Where(x => qtiItemIds.Contains(x.QTIItemID) && x.IsRubricBasedQuestion == true).Select(x => x.QTIItemID).Distinct().ToList();
                if (newQtiItemIds?.Count > 0)
                {
                    qtiItemIds.RemoveAll(x => newQtiItemIds.Contains(x));

                    qtiItemIdStringNew = string.Join(",", qtiItemIds);
                }
                var mediaModel = new MediaModel();

                var newQTIItemIdString = parameters.QTIITemServices.DuplicateListQTIItem(CurrentUser.Id, qtiItemIdStringNew, qtiGroupId, true,
                    LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName,
                    LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder,
                    LinkitConfigurationManager.GetS3Settings().S3Domain
                    );

                // Update Item Passage
                if (!string.IsNullOrWhiteSpace(newQTIItemIdString))
                {
                    var newQTIItemIds = newQTIItemIdString.Split(',');
                    if (newQTIItemIds != null && newQTIItemIds.Length > 0)
                    {
                        foreach (var stringId in newQTIItemIds)
                        {
                            int id = 0;
                            int.TryParse(stringId, out id);
                            if (id > 0)
                                UpdateItemPassage(id);
                        }
                    }
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
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
                            string.Format("Fail to add item(s) to item set, error detail:" + ex.Message)
                        }, JsonRequestBehavior.AllowGet);
            }
        }

        private void UpdateItemPassage(int qtiItemId)
        {
            try
            {
                var qtiItem = parameters.QTIITemServices.GetQtiItemById(qtiItemId);
                XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
                qtiItem.XmlContent = qtiItem.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
                qtiItem.XmlContent = qtiItem.XmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();
                List<PassageViewModel> passageList = Util.GetPassageList(qtiItem.XmlContent, false);
                if (passageList != null)
                {
                    parameters.QTIITemServices.UpdateItemPassage(qtiItemId, passageList.Select(x => x.QtiRefObjectID).ToList(),
                        passageList.Select(x => x.RefNumber).ToList());
                }
            }
            catch
            {
            }
        }

        [HttpGet]
        public ActionResult GetCriteriaGrades()
        {
            var data =
                parameters.QTI3Services.GetCriteriaGrades().Select(x => new ListItem { Id = x.GradeId, Name = x.Name }).
                    ToList();

            var districtIds = new List<int>();
            if (CurrentUser.IsNetworkAdmin)
            {
                districtIds = CurrentUser.GetMemberListDistrictId();
            }
            else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                districtIds.Add(CurrentUser.DistrictId.Value);
            }

            if (districtIds.Count > 0)
            {
                var exceptGrades =
                            parameters.QTI3Services.GetGradetLicenses(districtIds).Select(x => x.GradeID).ToList();
                if (exceptGrades.Any())
                    data = data.Where(x => !exceptGrades.Contains(x.Id)).ToList();
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCriteriaQTI3PDOK(int? qti3pSourceId)
        {
            if (!qti3pSourceId.HasValue)
            {
                qti3pSourceId = 0;
            }
            //TODO:
            var data =
                parameters.QTI3Services.GetQti3pDOKs(qti3pSourceId.Value).Select(x => new ListItem { Id = x.QTI3pDOKID, Name = x.Name }).
                    ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetPassageGrades()
        {
            var data =
                parameters.QTI3Services.GetPassageGrades().Select(x => new ListItem { Id = x.GradeId, Name = x.Name }).
                    ToList();
            var districtIds = new List<int>();
            if (CurrentUser.IsNetworkAdmin)
            {
                districtIds = CurrentUser.GetMemberListDistrictId();
            }
            else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
            {
                districtIds.Add(CurrentUser.DistrictId.Value);
            }
            if (districtIds.Count > 0)
            {
                var exceptGrades =
                    parameters.QTI3Services.GetGradetLicenses(districtIds).Select(x => x.GradeID).ToList();
                if (exceptGrades.Any())
                    data = data.Where(x => !exceptGrades.Contains(x.Id)).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetPassageGradesQti()
        {
            var data =
                parameters.QTIITemServices.GetPassageQtiGrade().OrderBy(x => x.Order).Select(x => new ListItem { Id = x.GradeId, Name = x.Name }).
                    ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        public ActionResult ShowEditQtiItemItem(int qtiItemId, int showPassage, int? qti3pItemIdUploaded, int? qtiItemHistoryId)
        {
            ViewBag.QtiItemId = qtiItemId;

            //Get the QtiItem
            var originalXmlContent = string.Empty;
            var xmlContent = string.Empty;
            string urlPath = string.Empty;
            if (qti3pItemIdUploaded.HasValue)
            {
                //used for item 3p upload
                var qtiItem = parameters.QTIITemServices.GetQti3pItemById(qti3pItemIdUploaded.Value);
                xmlContent = qtiItem.XmlContent; 

                urlPath = qtiItem.UrlPath;
            }
            else if (qtiItemHistoryId.HasValue && qtiItemHistoryId.Value > 0)
            {
                var qtiItemHistory = parameters.QTIITemServices.GetQTIItemHistoryByQtiItemHistoryId(qtiItemHistoryId.Value);
                xmlContent = qtiItemHistory?.XmlContent;
            }
            else
            {
                var qtiItem = parameters.QTIITemServices.GetQtiItemById(qtiItemId);
                xmlContent = qtiItem.XmlContent;

                urlPath = qtiItem.UrlPath;
            }

            xmlContent = xmlContent.ReplaceWeirdCharacters();
            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            xmlContent = xmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);

            //var useS3Content = parameters.DistrictDecodeService.UseS3Content(CurrentUser.DistrictId.GetValueOrDefault());
            //if (useS3Content)
            {
                xmlContent = Util.UpdateS3LinkForItemMedia(xmlContent);
                xmlContent = Util.UpdateS3LinkForPassageLink(xmlContent);
            }
            originalXmlContent = xmlContent;
            var model = ItemSetPrinting.TransformXmlContentToHtml(xmlContent, urlPath, false, s3Service);

            //model.XmlContent = model.XmlContent.Replace(temp, "&#");//ItemSetPrinting.TransformXmlContentToHtml will replace all &#160; by space (in XmlDocument.LoadXml), so that it's necessary to replace by temp to recover &#160;
            model.XmlContent = model.XmlContent.RecoverXmlSpecialChars(xmlSpecialCharToken);

            //get CSS
            ViewBag.Css = string.Empty;
            string htmlContent = string.Empty;

            //htmlContent = Util.ReplaceWeirdCharacters(model.XmlContent);
            htmlContent = model.XmlContent;
            htmlContent = Util.ReplaceTagListByTagOl(htmlContent);
            htmlContent = Util.ReplaceVideoTag(htmlContent);
            htmlContent = XmlUtils.RemoveAllNamespacesPrefix(htmlContent);
            ViewBag.HtmlContent = System.Net.WebUtility.HtmlDecode(htmlContent);
            var niceXmlContent = AjustXMLContent(originalXmlContent.RecoverXmlSpecialChars(xmlSpecialCharToken));
            ViewBag.XmlContent = XmlUtils.RemoveAllNamespacesPrefix(niceXmlContent);           
            ViewBag.PassageList = new List<PassageViewModel>();
            if (showPassage == 1)
            {
                //get the list of passage of item
                ViewBag.PassageList = Util.GetPassageList(ViewBag.XmlContent, false, null, false, false, parameters.Qti3pPassageService, parameters.qtiRefObjectService, parameters.dataFileUploadPassageService);
            }

            ViewBag.TestItemMediaPath = string.Empty;
            return PartialView("_QtiItemDetail");
        }

        private string AjustXMLContent(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent)) return string.Empty;

            xmlContent = xmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();
            xmlContent = Util.ReplaceTagListByTagOl(xmlContent);
            xmlContent = Util.ReplaceVideoTag(xmlContent);

            return xmlContent;
        }

        [UrlReturnDecode]
        public ActionResult ShowEditQti3pItemItem(int qti3pItemId, int showPassage)
        {
            ViewBag.Qti3pItemId = qti3pItemId;

            //Get the QtiItem
            var qti3pItem = parameters.QTIITemServices.GetQti3pItemById(qti3pItemId);
            //var temp = Guid.NewGuid().ToString();
            //var useS3Content = parameters.DistrictDecodeService.UseS3Content(CurrentUser.DistrictId.GetValueOrDefault());
            //qti3pItem.XmlContent = qti3pItem.XmlContent.Replace("&#", temp);
            var model = ItemSetPrinting.TransformXmlContentToHtml(qti3pItem.XmlContent, qti3pItem.UrlPath, true, s3Service);
            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            qti3pItem.XmlContent = qti3pItem.XmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
            //get CSS
            ViewBag.Css = string.Empty;

            //model.XmlContent = model.XmlContent.Replace(temp, "&#");
            model.XmlContent = model.XmlContent.RecoverXmlSpecialChars(xmlSpecialCharToken);

            model.XmlContent = model.XmlContent.ReplaceWeirdCharacters();
            ViewBag.HtmlContent = XmlUtils.RemoveAllNamespacesPrefix(model.XmlContent);
            ViewBag.XmlContent = qti3pItem.XmlContent.RemoveLineBreaks().ReplaceWeirdCharacters();
            if (showPassage == 1)
            {
                //get the list of passage of item
                ViewBag.PassageList = Util.GetPassageList(ViewBag.XmlContent, true, qti3pItem.UrlPath);
            }
            return PartialView("_Qti3pItemDetail");
        }

        public string CssPath(string fileName)
        {
            var path = HttpContext.Server.MapPath("~/Content/themes/Print/ItemSets/");
            var result = Path.Combine(path, fileName);
            return result;
        }

        public ActionResult LoadListItemsFromLibraryNew()
        {
            return PartialView("_ListItemsFromLibrary");
        }

        public ActionResult LoadStandardFilter()
        {
            return PartialView("_StandardFilter");
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult MoveItemSet(int qtiGroupId, int toQtiBankId, bool createACopy, string newItemSetName)
        {
            //avoid someone modify ajax parameter
            var obj = parameters.QtiGroupServices.GetById(qtiGroupId);
            if (obj == null)
            {
                return
                    Json(
                        new
                        {
                            Success = "Fail",
                            errorMessage = string.Format("Item set does not exist.")
                        },
                        JsonRequestBehavior.AllowGet);
            }

            //Check if current user has right to move this item set

            if (
                !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, obj))
            {
                return
                    Json(
                        new
                        {
                            Success = "Fail",
                            errorMessage = string.Format("Has no right to move this item set.")
                        },
                        JsonRequestBehavior.AllowGet);
            }
            var toQtiBank = parameters.QtiBankServices.GetById(toQtiBankId);
            if (toQtiBank == null)
            {
                return
                   Json(
                       new
                       {
                           Success = "Fail",
                           errorMessage = string.Format("Bank does not exist.")
                       },
                       JsonRequestBehavior.AllowGet);
            }

            newItemSetName = HttpUtility.UrlDecode(newItemSetName);
            if (string.IsNullOrWhiteSpace(newItemSetName))
            {
                return
                  Json(
                      new
                      {
                          Success = "Fail",
                          errorMessage = string.Format("Please input name for item set.")
                      },
                      JsonRequestBehavior.AllowGet);
            }
            try
            {
                //check if new item set name is existing in the new item bank or not
                bool isExistSetName = parameters.QtiGroupServices.CheckExistSetName(newItemSetName, CurrentUser.Id,
                                                                                    toQtiBankId, null);
                if (isExistSetName)
                {
                    return Json(new { Success = "Failed", ExistSetName = "1" }, JsonRequestBehavior.AllowGet);
                }
                if (!createACopy)
                {
                    parameters.QtiGroupServices.MoveToOtherItemBank(qtiGroupId, toQtiBankId, newItemSetName);
                }
                else
                {
                    var newQtiGroup = parameters.QtiGroupServices.CloneToItemBank(qtiGroupId, toQtiBankId,
                                                                                  CurrentUser.Id, newItemSetName);

                    // Copy all QtiItem from old qtiGroup to new qtiGroup
                    var qtiItems = parameters.QTIITemServices.SelectQTIItems().Where(en => en.QTIGroupID == qtiGroupId).OrderBy(x => x.QuestionOrder); //clone item by QuestionOrder
                    var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
                    var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;

                    foreach (var qtiItemData in qtiItems)
                    {
                        var newQTIItemData = parameters.QTIITemServices.DuplicateQTIItem(CurrentUser.Id, qtiItemData.QTIItemID,
                            newQtiGroup.QtiGroupId, true, bucketName, folder,
                            LinkitConfigurationManager.GetS3Settings().S3Domain);

                        // Update Item Passage
                        if (newQTIItemData != null)
                            UpdateItemPassage(newQTIItemData.QTIItemID);
                    }
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return
                    Json(
                        new
                        {
                            Success = "Fail",
                            errorMessage = string.Format("Move Item Set  fail, error detail:" + ex.Message)
                        },
                        JsonRequestBehavior.AllowGet);
            }
        }

        [UrlReturnDecode]
        public ActionResult LoadMoveItemSetConfirmDialog(int qtiGroupId, int toQtiBankId, bool createACopy)

        {
            //avoid someone modify ajax parameter
            ViewBag.ErrorMessage = string.Empty;
            var obj = parameters.QtiGroupServices.GetById(qtiGroupId);
            if (obj == null)
            {
                ViewBag.ErrorMessage = "Item set does not exist.";
                return PartialView("_MoveItemSetCOnfirm");
            }

            //Check if current user has right to move this item set

            if (
                !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, obj))
            {
                ViewBag.ErrorMessage = "Has no right to move this item set.";
                return PartialView("_MoveItemSetCOnfirm");
            }

            var qtiGroup = parameters.QtiGroupServices.GetById(qtiGroupId);
            if (qtiGroup != null)
            {
                ViewBag.QtiGroupId = qtiGroup.QtiGroupId;
                ViewBag.ItemSetName = qtiGroup.Name;
            }
            else
            {
                ViewBag.ErrorMessage = "Item bank does not exist.";
                return PartialView("_MoveItemSetCOnfirm");
            }

            ViewBag.QtiGroupId = qtiGroupId;
            ViewBag.ToQtiBankId = toQtiBankId;
            ViewBag.CreateACopy = createACopy;

            return PartialView("_MoveItemSetCOnfirm");
        }

        public ActionResult ShowTagFilter()
        {
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            return PartialView("_TagFilter");
        }

        public ActionResult LoadListLinkitDefaultTagAvailableFilterPartialView()
        {
            return PartialView("_ListLinkitDefaultTagAvailableFilter");
        }

        public ActionResult LoadListLinkitDefaultTagSelectedFilterPartialView()
        {
            return PartialView("_ListLinkitDefaultTagSelectedFilter");
        }

        [UrlReturnDecode]
        public ActionResult GetSelectedLinkitDefaultTags(string selectedTopicTags, string selectedSkillTags, string selectedOtherTags)
        {
            //for Topic,Skill and Other
            List<int> topicId = ParseIdListFromIdString(selectedTopicTags);
            List<int> skillId = ParseIdListFromIdString(selectedSkillTags);
            List<int> otherId = ParseIdListFromIdString(selectedOtherTags);

            var topics = parameters.TopicService.GetAll().Where(x => topicId.Contains(x.TopicID)).ToList();
            var skills = parameters.LessonOneService.GetAll().Where(x => skillId.Contains(x.LessonOneID)).ToList();
            var others = parameters.LessonTwoService.GetAll().Where(x => otherId.Contains(x.LessonTwoID)).ToList();

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

        public ActionResult LoadDistrictTagAvailableFilterPartialView()
        {
            return PartialView("_ListDistrictTagAvailableFilter");
        }

        public ActionResult LoadDistrictTagSelectedFilterPartialView()
        {
            return PartialView("_ListDistrictTagSelectedFilter");
        }

        private List<int> ParseIdListFromIdString(string idString)
        {
            //idString looks like -10-,-20-,-30-,
            List<int> idList = new List<int>();
            if (!string.IsNullOrEmpty(idString))
            {
                string[] idArray = idString.Split(',');
                foreach (var id in idArray)
                {
                    if (id.Length > 0)
                    {
                        idList.Add(Int32.Parse(id.Replace("-", "")));
                    }
                }
            }
            return idList;
        }

        public ActionResult GetSelectedDistrictTags(string selectedDistrictTags)
        {
            List<int> itemTagIdList = ParseIdListFromIdString(selectedDistrictTags);
            var data = parameters.ItemTagService.GetAllItemTag().Where(x => itemTagIdList.Contains(x.ItemTagID)).Select(
                x => new QtiItemTagAssignViewModel
                {
                    ItemTagId = x.ItemTagID,
                    CategoryName = x.Category,
                    TagName = x.Name
                });

            var parser = new DataTableParser<QtiItemTagAssignViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        public string ParseSelectedTagsToXml(string selectedTags)
        {
            string xmlResult = string.Empty;
            string[] categoryTags = selectedTags.Split(new string[] { ",|," }, StringSplitOptions.None);
            foreach (var categoryTag in categoryTags)
            {
            }

            return xmlResult;
        }

        private string BuildStandardNumber(string standardXmlString, List<int> userStateIdList)
        {
            string standardDisplayed = string.Empty;

            List<string> standardNumberList = Util.ParseStandardNumber(standardXmlString, CurrentUser.RoleId, userStateIdList);
            if (standardNumberList.Count > 0)
            {
                standardDisplayed = string.Format(", {0}", string.Join(", ", standardNumberList));
                if (!string.IsNullOrEmpty(standardDisplayed))
                {
                    if (standardDisplayed.StartsWith(","))
                    {
                        standardDisplayed = standardDisplayed.Remove(0, 1);
                    }
                }
            }
            return standardDisplayed;
        }

        [UrlReturnDecode]
        public ActionResult CheckQtiItemExists(int qtiItemId)
        {
            var qtiItem = parameters.QTIITemServices.GetQtiItemById(qtiItemId);
            if (qtiItem == null)
            {
                return
                  Json(
                      new
                      {
                          Exists = "False",
                          errorMessage = string.Format("This item has been deleted by someone else already!")
                      },
                      JsonRequestBehavior.AllowGet);
            }
            else
            {
                return
                  Json(
                      new
                      {
                          Exists = "True",
                      },
                      JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetQtiRefObjectNumber()
        {
            var data = parameters.PassageService.GetQtiRefObjectNumber();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadConfirmDeleteItemBank(int itembankId, string itembankName)
        {
            //check the right of current user if he/she can update qti bank or not ( avoid someone modify ajax parameter QtiBankID )
            ViewBag.ErrorMessage = string.Empty;
            if (!parameters.VulnerabilityService.HasRightToUpdateItemBank(CurrentUser, itembankId))
            {
                ViewBag.ErrorMessage = "Has no right to delete item bank !";
                return PartialView("_ConfirmDeleteItemBank", new ListItemsViewModel());
            }
            itembankName = HttpUtility.UrlDecode(itembankName);
            var obj = new ListItemsViewModel()
            {
                Id = itembankId,
                Name = HttpUtility.HtmlDecode(itembankName)
            };

            return PartialView("_ConfirmDeleteItemBank", obj);
        }

        public ActionResult LoadConfirmDeleteItemSet(int itemSetId, string itemSetName)
        {
            //avoid someone modify ajax parameter
            ViewBag.ErrorMessage = string.Empty;
            var itemSet = parameters.QtiGroupServices.GetById(itemSetId);
            if (itemSet == null)
            {
                ViewBag.ErrorMessage = "Item set does not exist.";
                return PartialView("_ConfirmDeleteItemSet", new ListItemsViewModel());
            }
            //Check if current user has right to update this item set
            if (
                !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, itemSet))
            {
                ViewBag.ErrorMessage = "Has no right to delete this item set.";
                return PartialView("_ConfirmDeleteItemSet", new ListItemsViewModel());
            }

            itemSetName = HttpUtility.UrlDecode(itemSetName);
            var obj = new ListItemsViewModel()
            {
                Id = itemSetId,
                Name = HttpUtility.HtmlDecode(itemSetName)
            };

            return PartialView("_ConfirmDeleteItemSet", obj);
        }

        [UrlReturnDecode]
        public ActionResult GetQtiItemsByFiltersPassage(QtiItemFilters filer)
        {
            var parser = new DataTableParserProc<PassageItemViewModel>();
            if (Request["bSortable_0"] == null || (filer.FirstLoadListItemsFromLibraryNew.HasValue && filer.FirstLoadListItemsFromLibraryNew.Value))
            {
                var emptyResult = new List<PassageItemViewModel>();
                return Json(parser.Parse(emptyResult.AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }

            string sortColumns = parser.SortableColumns;
            string searchColumns = parser.SearchableColumns;

            filer.Keyword = filer.Keyword.DecodeParameters();
            filer.Topic = filer.Topic.DecodeParameters();
            filer.Skill = filer.Skill.DecodeParameters();
            filer.Other = filer.Other.DecodeParameters();
            filer.DistrictTag = filer.DistrictTag.DecodeParameters();
            filer.RefObjectTitle = filer.RefObjectTitle.DecodeParameters();
            filer.PassageSubject = filer.PassageSubject.DecodeParameters();
            filer.Standard = filer.Standard.DecodeParameters();

            if (filer.QtiBankId.HasValue && filer.QtiBankId.Value < 0)
            {
                filer.QtiBankId = null;
            }
            if (filer.ItemSetId.HasValue && filer.ItemSetId.Value < 0)
            {
                filer.ItemSetId = null;
            }

            if (!string.IsNullOrEmpty(filer.SelectedTags))
            {
                //remove the last ','
                filer.SelectedTags = filer.SelectedTags.Substring(0, filer.SelectedTags.Length - 1);
            }

            //Personal," + LabelHelper.DistrictLabel + " do not user Number, so use RefObjectTitle instead
            filer.PassageNumber = filer.RefObjectTitle;

            if (filer.IsShowPassageForFoundItem)
            {
                filer.IgnoreFilterByPassage();
            }

            var data =
                parameters.QTIITemServices.GetQtiItemsByFiltersPassage(filer, CurrentUser.Id,
                                                               CurrentUser.DistrictId.GetValueOrDefault(),
                                                               parser.StartIndex, parser.PageSize, sortColumns,
                                                               searchColumns,
                                                               parser.SearchInBoxXML).ToList();
            int totalRow = 0;

            if (data.Count > 0)
            {
                totalRow = data[0].TotalRow;
            }
            var result = data.Select(x => new PassageItemViewModel
            {
                QTIRefObjectID = x.QTIRefObjectID,
                Source = x.Source,
                Name = x.Name,
                Number = x.Number ?? string.Empty,
                Subject = x.Subject,
                GradeName = x.GradeName,
                TextType = x.TextType,
                TextSubType = x.TextSubType,
                FleschKinkaidName = x.FleschKinkaidName,
                ItemsMatchCount = x.ItemsMatchCount,
                ItemsAllCount = x.ItemsAllCount,
                ItemsMatchXml = x.ItemsMatchXml,
                ItemsAllXml = x.ItemsAllXml,
                HasQTI3pPassage = x.HasQTI3pPassage,
                CanEdit = x.CanEdit
            });

            var jsonResult = parser.Parse(result.AsQueryable(), totalRow);

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        public ActionResult GetQtiItemsByFiltersPassageFromItemLibrary(QtiItemFilters filer)
        {
            var parser = new DataTableParserProc<PassageItemViewModel>();
            if (Request["bSortable_0"] == null || (filer.FirstLoadListItemsFromLibraryNew.HasValue && filer.FirstLoadListItemsFromLibraryNew.Value))
            {
                var emptyResult = new List<PassageItemViewModel>();
                return Json(parser.Parse(emptyResult.AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }

            string sortColumns = parser.SortableColumns;
            string searchColumns = parser.SearchableColumns;

            filer.Keyword = filer.Keyword.DecodeParameters();
            filer.Topic = filer.Topic.DecodeParameters();
            filer.Skill = filer.Skill.DecodeParameters();
            filer.Other = filer.Other.DecodeParameters();
            filer.DistrictTag = filer.DistrictTag.DecodeParameters();
            filer.RefObjectTitle = filer.RefObjectTitle.DecodeParameters();
            filer.PassageSubject = filer.PassageSubject.DecodeParameters();
            filer.Standard = filer.Standard.DecodeParameters();

            if (filer.QtiBankId.HasValue && filer.QtiBankId.Value < 0)
            {
                filer.QtiBankId = null;
            }
            if (filer.ItemSetId.HasValue && filer.ItemSetId.Value < 0)
            {
                filer.ItemSetId = null;
            }

            if (!string.IsNullOrEmpty(filer.SelectedTags))
            {
                //remove the last ','
                filer.SelectedTags = filer.SelectedTags.Substring(0, filer.SelectedTags.Length - 1);
            }

            //Personal," + LabelHelper.DistrictLabel + " do not user Number, so use RefObjectTitle instead
            filer.PassageNumber = filer.RefObjectTitle;

            if (filer.IsShowPassageForFoundItem)
            {
                filer.IgnoreFilterByPassage();
            }

            var data =
                parameters.QTIITemServices.GetQtiItemsByFiltersPassageFromItemLibrary(filer, CurrentUser.Id,
                                                               CurrentUser.DistrictId.GetValueOrDefault(),
                                                               parser.StartIndex, parser.PageSize, sortColumns,
                                                               searchColumns,
                                                               parser.SearchInBoxXML).ToList();
            int totalRow = 0;

            if (data.Count > 0)
            {
                totalRow = data[0].TotalRow;
            }
            var result = data.Select(x => new PassageItemViewModel
            {
                QTIRefObjectID = x.QTIRefObjectID,
                Source = x.Source,
                Name = x.Name,
                Number = x.Number ?? string.Empty,
                Subject = x.Subject,
                GradeName = x.GradeName,
                TextType = x.TextType,
                TextSubType = x.TextSubType,
                FleschKinkaidName = x.FleschKinkaidName,
                ItemsAllCount = x.ItemsAllCount,
                ItemsAllXml = x.ItemsAllXml,
                HasQTI3pPassage = x.HasQTI3pPassage
            });

            var jsonResult = parser.Parse(result.AsQueryable(), totalRow);

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        public ActionResult GetQti3pItemsByFiltersPassage(QTI3pItemFilters obj)
        {
            obj.Searchkey = obj.Searchkey.DecodeParameters();
            obj.PassageSubject = obj.PassageSubject.DecodeParameters();
            obj.Subject = obj.Subject.DecodeParameters();
            obj.Difficulty = obj.Difficulty.DecodeParameters();
            obj.Blooms = obj.Blooms.DecodeParameters();
            //obj.PassageTitle = obj.PassageTitle.DecodeParameters();
            obj.PassageTitle = string.Empty;//now search title by dropdownlist ( in obj.PassageId )

            var parser = new DataTableParserProc<PassageItem3pViewModel>();
            if (Request["bSortable_0"] == null || (obj.FirstLoadListItemsFromLibrary.HasValue && obj.FirstLoadListItemsFromLibrary.Value))
            {
                var emptyResult = new List<PassageItem3pViewModel>();
                return Json(parser.Parse(emptyResult.AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }
            var sortColumns = parser.SortableColumns;
            var searchColumns = parser.SearchableColumns;

            if (obj == null)
            {
                obj = new QTI3pItemFilters();
            }

            if (obj.Subject != null && obj.Subject.Equals("All"))
            {
                obj.Subject = string.Empty;
            }
            if (obj.Difficulty != null && obj.Difficulty.Equals("All"))
            {
                obj.Difficulty = string.Empty;
            }
            if (obj.Blooms != null && obj.Blooms.Equals("All"))
            {
                obj.Blooms = string.Empty;
            }
            obj.CurrentUserId = CurrentUser.Id;
            if (obj.PassageId < 0)
            {
                obj.PassageId = 0;
            }
            if (obj.ItemTypeId < 0)
            {
                obj.ItemTypeId = 0;
            }
            var strDistrictIds = string.Empty;
            if (obj.Qti3pSourceId.HasValue && obj.Qti3pSourceId.Value == (int)QTI3pSourceEnum.Mastery)
            {
                var districtIds = new List<int>();
                if (CurrentUser.IsNetworkAdmin)
                {
                    districtIds = CurrentUser.GetMemberListDistrictId();
                }
                else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
                {
                    districtIds.Add(CurrentUser.DistrictId.Value);
                }
                if (districtIds.Any())
                    strDistrictIds = string.Format(",{0},", string.Join(",", districtIds));
            }

            obj.IsRestricted = CheckIsInternationalState(CurrentUser.Id);

            if (obj.IsShowPassageForFoundItem)
            {
                obj.IgnoreFilterByPassage();
            }

            var data =
                parameters.QTI3Services.GetQti3pItemsByFiltersPassage(obj, CurrentUser.Id, parser.StartIndex,
                    parser.PageSize, sortColumns,
                    searchColumns, parser.SearchInBoxXML, strDistrictIds).ToList();

            int totalRow = 0;
            if (data.Count > 0)
            {
                totalRow = data[0].TotalRow;
            }
            var result = data.Select(x => new PassageItem3pViewModel
            {
                QTI3pPassageID = x.QTI3pPassageID,
                Source = x.Source == "NWEA" ? QTI3pSourceDisplayEnum.Mastery.ToString() : x.Source,
                Name = x.Name,
                Number = x.Number,
                Subject = x.Subject,
                GradeName = x.GradeName,
                TextType = x.TextType,
                TextSubType = x.TextSubType,
                WordCount = x.WordCound,
                FleschKinkaidName = x.FleschKinkaidName,
                PassageType = x.PassageType,
                PassageGenre = x.PassageGenre,
                
                Spache = x.Spache,
                DaleChall = x.DaleChall,
                RMM = x.RMM,
                ItemsMatchCount = x.ItemsMatchCount,
                ItemsAllCount = x.ItemsAllCount,
                Lexile = x.Lexile.ToString() ?? string.Empty,
                ItemsMatchXml = x.ItemsMatchXml,
                ItemsAllXml = x.ItemsAllXml
            });

            return Json(parser.Parse(result.AsQueryable(), totalRow), JsonRequestBehavior.AllowGet);
        }

        [UrlReturnDecode]
        public ActionResult GetQti3pItemsByFiltersPassageFromItemLibrary(QTI3pItemFilters obj)
        {
            obj.Searchkey = obj.Searchkey.DecodeParameters();
            obj.PassageSubject = obj.PassageSubject.DecodeParameters();
            obj.Subject = obj.Subject.DecodeParameters();
            obj.Difficulty = obj.Difficulty.DecodeParameters();
            obj.Blooms = obj.Blooms.DecodeParameters();
            //obj.PassageTitle = obj.PassageTitle.DecodeParameters();
            obj.PassageTitle = string.Empty;//now search title by dropdownlist ( in obj.PassageId )

            var parser = new DataTableParserProc<PassageItem3pViewModel>();
            if (Request["bSortable_0"] == null || (obj.FirstLoadListItemsFromLibrary.HasValue && obj.FirstLoadListItemsFromLibrary.Value))
            {
                var emptyResult = new List<PassageItem3pViewModel>();
                return Json(parser.Parse(emptyResult.AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }
            var sortColumns = parser.SortableColumns;
            var searchColumns = parser.SearchableColumns;

            if (obj == null)
            {
                obj = new QTI3pItemFilters();
            }

            if (obj.Subject != null && obj.Subject.Equals("All"))
            {
                obj.Subject = string.Empty;
            }
            if (obj.Difficulty != null && obj.Difficulty.Equals("All"))
            {
                obj.Difficulty = string.Empty;
            }
            if (obj.Blooms != null && obj.Blooms.Equals("All"))
            {
                obj.Blooms = string.Empty;
            }
            obj.CurrentUserId = CurrentUser.Id;
            if (obj.PassageId < 0)
            {
                obj.PassageId = 0;
            }
            if (obj.ItemTypeId < 0)
            {
                obj.ItemTypeId = 0;
            }
            var strDistrictIds = string.Empty;
            if (obj.Qti3pSourceId.HasValue && obj.Qti3pSourceId.Value == (int)QTI3pSourceEnum.Mastery)
            {
                var districtIds = new List<int>();
                if (CurrentUser.IsNetworkAdmin)
                {
                    districtIds = CurrentUser.GetMemberListDistrictId();
                }
                else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
                {
                    districtIds.Add(CurrentUser.DistrictId.Value);
                }
                if (districtIds.Any())
                    strDistrictIds = string.Format(",{0},", string.Join(",", districtIds));
            }

            obj.IsRestricted = CheckIsInternationalState(CurrentUser.Id);

            if (obj.IsShowPassageForFoundItem)
            {
                obj.IgnoreFilterByPassage();
            }

            var data =
                parameters.QTI3Services.GetQti3pItemsByFiltersPassageFromItemLibrary(obj, CurrentUser.Id, parser.StartIndex,
                    parser.PageSize, sortColumns,
                    searchColumns, parser.SearchInBoxXML, strDistrictIds).ToList();

            int totalRow = 0;
            if (data.Count > 0)
            {
                totalRow = data[0].TotalRow;
            }
            var result = data.Select(x => new PassageItem3pViewModel
            {
                QTI3pPassageID = x.QTI3pPassageID,
                Source = x.Source == "NWEA" ? QTI3pSourceDisplayEnum.Mastery.ToString() : x.Source,
                Name = x.Name,
                Number = x.Number,
                Subject = x.Subject,
                GradeName = x.GradeName,
                TextType = x.TextType,
                TextSubType = x.TextSubType,
                WordCount = x.WordCound,
                FleschKinkaidName = x.FleschKinkaidName,
                PassageType = x.PassageType,
                PassageGenre = x.PassageGenre,
                Lexile = x.Lexile?.ToString(),
                Spache = x.Spache,
                DaleChall = x.DaleChall,
                RMM = x.RMM,
                ItemsAllCount = x.ItemsAllCount,
                ItemsAllXml = x.ItemsAllXml,
            });

            return Json(parser.Parse(result.AsQueryable(), totalRow), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ShowPassageItem3pForm(int qti3pPassageId, string matchItemXml, int? qti3pSource)
        {
            var model = new ShowPassageFormViewModel()
            {
                Qti3pSourceID = qti3pSource ?? 0,
                Qti3pPassageID = qti3pPassageId,
                Is3pItem = true,
                ShownItemXml = matchItemXml.DecodeParameters()
            };

            return PartialView("_PassageItem", model);
        }

        public ActionResult ShowPassageItemForm(int qtiRefObjectID, string matchItemXml)
        {
            var model = new ShowPassageFormViewModel()
            {
                QTIRefObjectID = qtiRefObjectID,
                ShownItemXml = matchItemXml.DecodeParameters()
            };

            return PartialView("_PassageItem", model);
        }

        public ActionResult LoadItemOnePassage(int qti3pPassageId, string matchItemXml)
        {
            var parser = new DataTableParserProc<ItemLibraryViewModel>();

            var qti3pItemIdList = new List<int>();
            if (string.IsNullOrEmpty(matchItemXml))
            {
                qti3pItemIdList = parameters.QTI3pItemToPassageService.GetAll()
                    .Where(x => x.Qti3pItemPassageId == qti3pPassageId)
                    .Select(x => x.Qti3pItemId)
                    .ToList();
            }
            else
            {
                //get only match item
                //qti3pItemIdList = ParseItemIdFromXml(matchItemXml.DecodeParameters().DecodeParameters(), true);
                qti3pItemIdList = matchItemXml.DecodeParameters().DecodeParameters().ParseIdsFromString();
            }

            var data = parameters.QTI3Services.GetQti3pItems(qti3pItemIdList).ToList();

            var masterStandardXML = parameters.MasterStandardService.GetQti3pItemStandardXml(qti3pItemIdList);

            var userStateIdList = parameters.StateServices.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);

            var dicMasterStandard = Util.ParseStandardNumberQti3pItem(masterStandardXML, CurrentUser.RoleId, userStateIdList);

            var result = data.Select(x => new ItemLibraryViewModel
            {
                QTI3pItemID = x.QTI3pItemID,
                Name = AdjustXmlContent(x.XmlContent, true, x.QTI3pSourceID, x.UrlPath),
                ToolTip =
                    BuildToolTipItem(x.QTI3pItemID, x.Subject, x.Difficulty, x.BloomsTaxonomy,
                                     x.GradeName, x.PassageNumber,
                                     x.PassageGrade, x.PassageSubject,
                                     x.PassageWordCount, x.PassageTextType,
                                     x.PassageTextSubType, x.PassageFleschKinkaid, x.Qti3pItemDOK, dicMasterStandard),
                From3pUpload = x.From3pUpload
            }).AsQueryable();
            return Json(parser.Parse(result, data.Count), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadItemOnePassageNew(int qtiRefObjectId, string matchItemXml)
        {
            var parser = new DataTableParserProc<ItemLibraryQtiItemViewModel>();

            var qtiItemIdList = new List<int>();
            if (string.IsNullOrEmpty(matchItemXml))
            {
                qtiItemIdList = parameters.QtiItemRefObjectService.GetAll()
                    .Where(x => x.QtiRefObjectId == qtiRefObjectId)
                    .Select(x => x.QtiItemId)
                    .ToList();
            }
            else
            {
                //get only match item
                //qtiItemIdList = ParseItemIdFromXml(matchItemXml.DecodeParameters().DecodeParameters(), false);
                qtiItemIdList = matchItemXml.DecodeParameters().DecodeParameters().ParseIdsFromString();
                //check security
            }

            var data = parameters.QTIITemServices.GetAllQtiItem().Where(x => qtiItemIdList.Contains(x.QTIItemID)).ToList();
            int totalRow = 0;

            if (data.Count > 0)
            {
                totalRow = data.Count;
            }
            var userStateIdList = parameters.StateServices.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);

            var refObjectIds = new List<int>();
            foreach (var qtiItem in data)
            {
                Util.GetPassageIdList(qtiItem.XmlContent.ReplaceWeirdCharacters(), ref refObjectIds);
            }

            //Get data for builind tooltip
            var refObjects =
                parameters.PassageService.GetAll().Where(x => refObjectIds.Contains(x.QTIRefObjectID)).ToList();

            var itemSetIdList = data.Select(x => x.QTIGroupID).ToList();
            var itemSet = parameters.QtiGroupServices.GetAll().Where(x => itemSetIdList.Contains(x.QtiGroupId)).ToList();
            var itemBankIdList = itemSet.Select(x => x.QtiBankId).ToList();
            var itemBank = parameters.QtiBankServices.GetAll().Where(x => itemBankIdList.Contains(x.QtiBankId)).ToList();

            var itemTopic = parameters.QTIItemTopicService.GetAll().Where(x => qtiItemIdList.Contains(x.QTIItemID)).ToList();

            var itemSkill = parameters.QTIItemLessonOneService.GetAll().Where(x => qtiItemIdList.Contains(x.QTIItemID)).ToList();

            var itemOther = parameters.QTIItemLessonTwoService.GetAll().Where(x => qtiItemIdList.Contains(x.QTIItemID)).ToList();

            var itemStandard = parameters.MasterStandardService.GetStandardsAssociatedWithItem(qtiItemIdList).ToList();
            var standardIdList = itemStandard.Select(x => x.StateStandardID).Distinct().ToList();
            var standard = parameters.MasterStandardService.GetAll().Where(x => standardIdList.Contains(x.MasterStandardID)).ToList();

            var itemTag = parameters.QtiItemItemTagService.GetAll().Where(x => qtiItemIdList.Contains(x.QtiItemID)).ToList();

            var dicItemItemSet = new Dictionary<int, string>();
            var dicItemItemBank = new Dictionary<int, string>();
            var dicItemTopic = new Dictionary<int, string>();
            var dicItemSkill = new Dictionary<int, string>();
            var dicItemOther = new Dictionary<int, string>();
            var dicItemStandard = new Dictionary<int, string>();
            var dicItemTag = new Dictionary<int, string>();
            foreach (var item in data)
            {
                dicItemItemSet.Add(item.QTIItemID, "");
                dicItemItemBank.Add(item.QTIItemID, "");
                dicItemTopic.Add(item.QTIItemID, "");
                dicItemSkill.Add(item.QTIItemID, "");
                dicItemOther.Add(item.QTIItemID, "");
                dicItemStandard.Add(item.QTIItemID, "");
                dicItemTag.Add(item.QTIItemID, "");

                if (itemSet.Any(x => x.QtiGroupId == item.QTIGroupID))
                {
                    dicItemItemSet[item.QTIItemID] = itemSet.FirstOrDefault(x => x.QtiGroupId == item.QTIGroupID).Name;

                    var itemBankId = itemSet.FirstOrDefault(x => x.QtiGroupId == item.QTIGroupID).QtiBankId;
                    if (itemBank.Any(x => x.QtiBankId == itemBankId))
                    {
                        dicItemItemBank[item.QTIItemID] = itemBank.FirstOrDefault(x => x.QtiBankId == itemBankId).Name;
                    }
                }
                if (itemTopic.Any(x => x.QTIItemID == item.QTIItemID))
                {
                    dicItemTopic[item.QTIItemID] = string.Join(",",
                        itemTopic.Where(x => x.QTIItemID == item.QTIItemID).Select(x => x.Name).ToList());
                }
                if (itemSkill.Any(x => x.QTIItemID == item.QTIItemID))
                {
                    dicItemSkill[item.QTIItemID] = string.Join(",",
                        itemSkill.Where(x => x.QTIItemID == item.QTIItemID).Select(x => x.Name).ToList());
                }
                if (itemOther.Any(x => x.QTIItemID == item.QTIItemID))
                {
                    dicItemOther[item.QTIItemID] = string.Join(",",
                        itemOther.Where(x => x.QTIItemID == item.QTIItemID).Select(x => x.Name).ToList());
                }
                if (itemStandard.Any(x => x.QTIItemID == item.QTIItemID))
                {
                    var standrdIdList = itemStandard.Where(x => x.QTIItemID == item.QTIItemID).Select(x => x.StateStandardID).ToList();

                    var standardNumber = standard.Where(x => standrdIdList.Contains(x.MasterStandardID)).Select(x => x.Number).ToList();

                    dicItemStandard[item.QTIItemID] = string.Join(",", standardNumber);
                }
                if (itemTag.Any(x => x.QtiItemID == item.QTIItemID))
                {
                    dicItemTag[item.QTIItemID] = string.Join(" <br/>",
                        itemTag.Where(x => x.QtiItemID == item.QTIItemID).Select(x => string.Format("{0}: {1}", x.CategoryName, x.TagName)).ToList());
                }
            }

            var result = data.Select(x => new ItemLibraryQtiItemViewModel
            {
                QtiItemId = x.QTIItemID,
                Name = AdjustXmlContent(x.XmlContent, false, null, null),
                ToolTip = BuildToolTipItemPassageNew(x.XmlContent.ReplaceWeirdCharacters(),
                                        dicItemItemSet[x.QTIItemID]
                                        , dicItemItemBank[x.QTIItemID]
                                        , dicItemTopic[x.QTIItemID]
                                        , dicItemSkill[x.QTIItemID]
                                        , dicItemOther[x.QTIItemID]
                                        , dicItemStandard[x.QTIItemID]
                                        , dicItemTag[x.QTIItemID]
                                        , userStateIdList, refObjects)
            }).ToList().AsQueryable();

            var jsonResult = parser.Parse(result, totalRow);

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadItemOnePassageProgress(int qti3pPassageId, string matchItemXml)
        {
            var parser = new DataTableParserProc<ItemLibraryViewModel>();

            var qti3pItemIdList = new List<int>();
            if (string.IsNullOrEmpty(matchItemXml))
            {
                qti3pItemIdList = parameters.QTI3pItemToPassageService.GetAll()
                    .Where(x => x.Qti3pItemPassageId == qti3pPassageId)
                    .Select(x => x.Qti3pItemId)
                    .ToList();
            }
            else
            {
                //get only match item
                //qti3pItemIdList = ParseItemIdFromXml(matchItemXml.DecodeParameters().DecodeParameters(), true);
                qti3pItemIdList = matchItemXml.DecodeParameters().DecodeParameters().ParseIdsFromString();
            }

            var data = parameters.QTI3Services.GetQti3pItems(qti3pItemIdList).ToList();

            var masterStandardXML = parameters.MasterStandardService.GetQti3pItemStandardXml(qti3pItemIdList);

            var userStateIdList = parameters.StateServices.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);

            var dicMasterStandard = Util.ParseStandardNumberQti3pItem(masterStandardXML, CurrentUser.RoleId, userStateIdList);

            var result = data.Select(x => new ItemLibraryViewModel
            {
                QTI3pItemID = x.QTI3pItemID,
                Name = AdjustXmlContent(x.XmlContent, true, x.QTI3pSourceID, x.UrlPath),
                ToolTip =
                    BuildToolTipItem(x.QTI3pItemID, x.Subject, x.Difficulty, x.BloomsTaxonomy,
                                     x.GradeName, x.PassageNumber,
                                     x.PassageGrade, x.PassageSubject,
                                     x.PassageWordCount, x.PassageTextType,
                                     x.PassageTextSubType, x.PassageFleschKinkaid, x.Qti3pItemDOK, dicMasterStandard)
            }).AsQueryable();
            return Json(parser.Parse(result, data.Count), JsonRequestBehavior.AllowGet);
        }

        public List<int> ParseItemIdFromXml(string xml, bool isCertica)
        {
            if (string.IsNullOrWhiteSpace(xml)) return new List<int>();
            var xdoc = XDocument.Parse(xml);
            var result = new List<int>();

            if (isCertica)
            {
                foreach (var node in xdoc.Element("Qti3pItemIdList").Elements("Qti3pItemId"))
                {
                    int id = 0;
                    Int32.TryParse(node.Value, out id);
                    if (id > 0)
                    {
                        result.Add(id);
                    }
                }
            }
            else
            {
                foreach (var node in xdoc.Element("QtiItemIdList").Elements("QtiItemId"))
                {
                    int id = 0;
                    Int32.TryParse(node.Value, out id);
                    if (id > 0)
                    {
                        result.Add(id);
                    }
                }
            }

            return result;
        }

        private string BuildToolTipItemPassageNew(string xmlContent, string groupName, string bankName, string topic,
            string skill, string other,
            string standard, string districtTag, List<int> userStateIdList, List<QtiRefObject> refObjects)
        {
            StringBuilder tooltip = new StringBuilder();
            tooltip.Append(string.Format("- Item Bank: {0}", bankName == null ? string.Empty : bankName));
            tooltip.Append(string.Format("<br>- Item Set: {0}", groupName == null ? string.Empty : groupName));

            //parse the passage
            try
            {
                List<string> passageNameList = Util.GetPassageNameListOptimize(xmlContent,
                    parameters.Qti3pPassageService, refObjects, true);
                if (passageNameList.Count > 0)
                {
                    tooltip.Append("<br>- Passages: ");
                    tooltip.Append("<br> &nbsp;&nbsp;&nbsp;+&nbsp;" + passageNameList[0]);

                    for (int i = 1; i < passageNameList.Count; i++)
                    {
                        tooltip.AppendLine(string.Format("<br> &nbsp;&nbsp;&nbsp;+&nbsp;{0}", passageNameList[i]));
                    }
                }
            }
            catch (Exception)
            {
            }

            if (!string.IsNullOrEmpty(standard))
            {
                tooltip.Append("<br>- Standards: ");
                tooltip.Append(standard);
            }

            if (!string.IsNullOrEmpty(topic))
            {
                tooltip.Append(string.Format("<br>- Topics: {0}", topic));
            }
            if (!string.IsNullOrEmpty(skill))
            {
                tooltip.Append(string.Format("<br>- Skills: {0}", skill));
            }
            if (!string.IsNullOrEmpty(other))
            {
                tooltip.Append(string.Format("<br>- Other: {0}", other));
            }
            tooltip.Append("<br>" + LabelHelper.DistrictLabel + " Tag: ");
            tooltip.Append(districtTag);

            return tooltip.ToString();
        }

        public ActionResult GetQTI3PPassageTitle(int? qti3pSourceId)
        {
            if (!qti3pSourceId.HasValue)
            {
                qti3pSourceId = (int)QTI3pSourceEnum.Mastery;
            }
            var data = parameters.QTI3Services.GetQTI3PPassageTitle(qti3pSourceId.Value).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQTI3pSource()
        {
            var xliFunctionAccess = parameters.AuthItemLibService.GetXliFunctionAccess(CurrentUser.Id, CurrentUser.RoleId,
                CurrentUser.DistrictId ?? 0);
            var qTI3pSource = parameters.QTI3pSourceService.GetAuthorizeQti3pSource(xliFunctionAccess).Where(x => x.QTI3pSourceId == (int)QTI3pSourceEnum.Mastery || x.QTI3pSourceId == (int)QTI3pSourceEnum.Progress)
                .OrderBy(x => x.QTI3pSourceId).Select(x => new ListItem { Id = x.QTI3pSourceId, Name = Enum.GetName(typeof(QTI3pSourceDisplayEnum), x.QTI3pSourceId) });
            return Json(qTI3pSource, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadListItemsFromLibraryUpload()
        {
            return PartialView("_ListItemsFromLibraryUpload");
        }

        [UrlReturnDecode]
        [HttpPost]
        [AjaxOnly]
        public ActionResult AddQTI3UploadToItemSetMany(string qti3pItemIdString, int qtiGroupId)
        {
            try
            {
                //avoid someone modify ajax parameter
                var obj = parameters.QtiGroupServices.GetById(qtiGroupId);
                if (obj == null)
                {
                    return Json(new { message = "Item set does not exist.", success = false }, JsonRequestBehavior.AllowGet);
                }
                if (
                   !parameters.VulnerabilityService.HasRightToUpdateItemSet(CurrentUser, obj))
                {
                    return Json(new { message = "Has no right to add items to this item set.", success = false }, JsonRequestBehavior.AllowGet);
                }

                if (qti3pItemIdString == null)
                {
                    qti3pItemIdString = string.Empty;
                }
                var bucketName = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
                var folder = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;
                //var uploadS3 = Util.UploadTestItemMediaToS3;
                List<int> idList = qti3pItemIdString.ParseIdsFromString();
                foreach (var id in idList)
                {
                    if (id > 0 && qtiGroupId > 0)
                    {
                        var qtiItem = parameters.QTIITemServices.ConvertFrom3pItemUploadedToItem(id, qtiGroupId, CurrentUser.Id, true,
                            bucketName, folder);
                        UpdateItemPassage(qtiItem?.QTIItemID ?? 0);

                        if (qtiItem == null)
                        {
                            return
                                   Json(
                                       new
                                       {
                                           Success = "Fail",
                                           errorMessage = "Can not add selected item(s)"
                                       },
                                       JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                return Json(new { Success = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return
                    Json(
                        new
                        {
                            Success = "Fail",
                            errorMessage = string.Format("Fail to add QTI3 to item set, error detail:" + ex.Message)
                        },
                        JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetQti3pProgressPassageType()
        {
            var data = parameters.QTI3Services.GetQti3pProgressPassageType().Select(o => new ListItem()
            {
                Id = o.Qti3pProgressPassageTypeID,
                Name = o.Qti3pProgressPassageTypeName
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQti3pProgressPassageGenre()
        {
            var data = parameters.QTI3Services.GetQti3pProgressPassageGenre().Select(o => new ListItem()
            {
                Id = o.Qti3pProgressPassageGenreID,
                Name = o.Qti3pProgressPassageGenreName
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadImportItemFromLibraryScript()
        {
            return PartialView("_ImportItemFromLibraryScript");
        }

        [UrlReturnDecode]
        public ActionResult LoadItemFromItemLibraryByFilter(QtiItemFilters filer)
        {
            var parser = new DataTableParserProc<ItemLibraryQtiItemModel>();
            if (!filer.IsDistrictSearch && !filer.IsPersonalSearch)
            {
                var res = new List<ItemLibraryQtiItemModel>();
                return Json(parser.Parse(res.AsQueryable(), res.Count), JsonRequestBehavior.AllowGet);
            }

            filer.Keyword = filer.Keyword.DecodeParameters();
            filer.Topic = filer.Topic.DecodeParameters();
            filer.Skill = filer.Skill.DecodeParameters();
            filer.Other = filer.Other.DecodeParameters();
            filer.DistrictTag = filer.DistrictTag.DecodeParameters();
            filer.RefObjectTitle = filer.RefObjectTitle.DecodeParameters();
            filer.PassageSubject = filer.PassageSubject.DecodeParameters();
            filer.ItemDescription = filer.ItemDescription?.DecodeParameters();
            filer.ItemTitle = filer.ItemTitle?.DecodeParameters();
            filer.sSearch = filer.sSearch?.DecodeParameters();
            filer.SelectedItemIds = filer.SelectedItemIds?.DecodeParameters();

            if (Request["bSortable_0"] == null || (filer.FirstLoadListItemsFromLibraryNew.HasValue && filer.FirstLoadListItemsFromLibraryNew.Value))
            {
                var emptyResult = new List<ItemLibraryQtiItemModel>();
                return Json(parser.Parse(emptyResult.AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }
            string sortColumns = parser.SortableColumns;
            string searchColumns = parser.SearchableColumns;

            if (string.IsNullOrEmpty(sortColumns))
            {
                sortColumns = "QtiItemId asc"; //alwasy need atleast one sortable column for paging
            }

            if (!string.IsNullOrEmpty(filer.SelectedTags))
            {
                //remove the last ','
                filer.SelectedTags = filer.SelectedTags.Substring(0, filer.SelectedTags.Length - 1);
            }
            //Personal," + LabelHelper.DistrictLabel + " do not user Number, so use RefObjectTitle instead
            filer.PassageNumber = filer.RefObjectTitle;

            filer.IgnoreFilterByPassage();
            var data =
                parameters.QTIITemServices.GetQtiItemsByFilter(filer, CurrentUser.Id,
                                                               CurrentUser.DistrictId.GetValueOrDefault(),
                                                               parser.StartIndex, parser.PageSize, sortColumns,
                                                               searchColumns,
                                                               parser.SearchInBoxXML).ToList();
            int totalRow = 0;

            if (data.Count > 0)
            {
                totalRow = data[0].TotalRow;
            }
            var refObjectIds = new List<int>();
            foreach (var qtiItem in data)
            {
                Util.GetPassageIdList(qtiItem.XmlContent.ReplaceWeirdCharacters(), ref refObjectIds);
            }

            var refObjects =
                parameters.PassageService.GetAll().Where(x => refObjectIds.Contains(x.QTIRefObjectID)).ToList();
            var userStateIdList = parameters.StateServices.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);
            var result = data.Select(x => new ItemLibraryQtiItemModel
            {
                QtiItemId = x.QtiItemId,
                Title = x.Title,
                Description = x.Description,
                BankName = x.BankName,
                GroupName = x.GroupName,
                Standard = x.Standard,
                DistrictTag = x.DistrictTag,
                Content = AdjustXmlContent(x.XmlContent, false, null, null),
                QTIBankID = x.QTIBankID,
                QTIGroupID = x.QTIGroupID,
                ToolTip = BuildToolTipItemNew(x.XmlContent.ReplaceWeirdCharacters(), x.GroupName, x.BankName, x.Topic, x.Skill, x.Other, x.Standard, x.DistrictTag, userStateIdList, refObjects, x.QtiItemId)
            }).ToList();

            result.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.Standard))
                {
                    StringBuilder sb = new StringBuilder();
                    var standardDisplayed = BuildStandardNumber(x.Standard, userStateIdList);
                    if (!string.IsNullOrEmpty(standardDisplayed))
                    {
                        sb.Append(Server.HtmlEncode(standardDisplayed));
                    }
                    x.Standard = sb.ToString();
                }

                if (!string.IsNullOrEmpty(x.DistrictTag))
                {
                    Dictionary<string, List<string>> districtTagDic = ParseDistrictTag(x.DistrictTag);
                    StringBuilder sb = new StringBuilder();
                    if (districtTagDic.Count > 0)
                    {
                        foreach (var category in districtTagDic)
                        {
                            sb.Append(string.Format("- {0}: ", Server.HtmlEncode(category.Key)));
                            if (category.Value != null && category.Value.Count > 0)
                            {
                                sb.Append(category.Value[0]);
                                for (int i = 1; i < category.Value.Count; i++)
                                {
                                    sb.Append(string.Format(", {0}", Server.HtmlEncode(category.Value[i])));
                                }
                            }
                            sb.Append("<br/>");
                        }
                    }

                    x.DistrictTag = sb.ToString();
                }

                if (x.QTIGroupID.HasValue)
                {
                    x.HasPermissionEditQTItem = _vulnerabilityService.HasRightToUpdateItemSet(CurrentUser, x.QTIGroupID.Value);
                }
            });

            var jsonResult = parser.Parse(result.AsQueryable(), totalRow);

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Load3rdItemFromItemLibraryByFilter(QTI3pItemFilters obj)
        {
            obj.Searchkey = obj.Searchkey.DecodeParameters();
            obj.PassageSubject = obj.PassageSubject.DecodeParameters();
            obj.Subject = obj.Subject.DecodeParameters();
            obj.Difficulty = obj.Difficulty.DecodeParameters();
            obj.Blooms = obj.Blooms.DecodeParameters();
            obj.PassageTitle = string.Empty;//now search title in Passage Name dropdownlist ( use obj.PassageId )
            obj.ItemTitle = obj.ItemTitle?.DecodeParameters();
            obj.ItemDescription = obj.ItemDescription?.DecodeParameters();
            obj.sSearch = obj.sSearch?.DecodeParameters();
            obj.SelectedItemIds = obj.SelectedItemIds?.DecodeParameters();

            var parser = new DataTableParserProc<ThirdPartyItemLibraryViewModel>();
            if (Request["bSortable_0"] == null || (obj.FirstLoadListItemsFromLibrary.HasValue && obj.FirstLoadListItemsFromLibrary.Value))
            {
                var emptyResult = new List<ThirdPartyItemLibraryViewModel>();
                return Json(parser.Parse(emptyResult.AsQueryable(), 0), JsonRequestBehavior.AllowGet);
            }

            var sortColumns = parser.SortableColumns;
            var searchColumns = parser.SearchableColumns;

            if (obj == null)
                obj = new QTI3pItemFilters();

            if (string.IsNullOrEmpty(sortColumns))
            {
                sortColumns = "QTI3pItemID asc"; //alwasy need atleast one sortable column for paging
            }

            if (obj.Subject != null && obj.Subject.Equals("All"))
            {
                obj.Subject = string.Empty;
            }
            if (obj.Difficulty != null && obj.Difficulty.Equals("All"))
            {
                obj.Difficulty = string.Empty;
            }
            if (obj.Blooms != null && obj.Blooms.Equals("All"))
            {
                obj.Blooms = string.Empty;
            }
            if (obj.QTI3pItemLanguage != null && obj.QTI3pItemLanguage.ToLower().Equals("all"))
            {
                obj.QTI3pItemLanguage = string.Empty;
            }
            if (obj.QTI3pPassageLanguage != null && obj.QTI3pPassageLanguage.ToLower().Equals("all"))
            {
                obj.QTI3pPassageLanguage = string.Empty;
            }
            obj.CurrentUserId = CurrentUser.Id;
            if (obj.PassageId < 0)
            {
                obj.PassageId = 0;
            }

            var strDistrictIds = string.Empty;
            if (obj.Qti3pSourceId.HasValue && obj.Qti3pSourceId.Value == (int)QTI3pSourceEnum.Mastery)
            {
                var districtIds = new List<int>();
                if (CurrentUser.IsNetworkAdmin)
                {
                    districtIds = CurrentUser.GetMemberListDistrictId();
                }
                else if (!CurrentUser.IsPublisher && CurrentUser.DistrictId.HasValue)
                {
                    districtIds.Add(CurrentUser.DistrictId.Value);
                }
                if (districtIds.Any())
                    strDistrictIds = string.Format(",{0},", string.Join(",", districtIds));
            }

            // Check international customer
            obj.IsRestricted = CheckIsInternationalState(CurrentUser.Id);
            obj.IgnoreFilterByPassage();
            obj.StateStandardIdString = obj.StateStandardIdString.ToIntCommaSeparatedString();
            var data =
                parameters.QTI3Services.GetQti3PItemsByFilter(obj, parser.StartIndex, parser.PageSize, sortColumns,
                                                              searchColumns, parser.SearchInBoxXML, strDistrictIds).ToList();

            var qti3pItemIdList = data.Select(x => x.QTI3pItemID).ToList();

            var userStateIdList = parameters.StateServices.GetStateIdForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, CurrentUser.IsDistrictAdmin);

            List<Qti3pItemStandardXml> masterStandardXML = parameters.MasterStandardService.GetQti3pItemStandardXml(qti3pItemIdList);
            Dictionary<int, List<string>> dicMasterStandard = Util.ParseStandardNumberQti3pItem(masterStandardXML, CurrentUser.RoleId, userStateIdList);

            int totalRow = 0;
            if (data.Count > 0)
            {
                totalRow = data[0].TotalRow;
            }

            var results = data.Select(x => new ThirdPartyItemLibraryViewModel
            {
                QTI3pItemID = x.QTI3pItemID,
                Title = x.Title,
                Description = x.Description,
                Content = AdjustXmlContent(x.XmlContent, true, obj.Qti3pSourceId, x.UrlPath),
                ToolTip = BuildToolTipItem(x.QTI3pItemID, x.Subject, x.Difficulty, x.BloomsTaxonomy,
                                                                       x.GradeName, x.PassageNumber,
                                                                       x.PassageGrade, x.PassageSubject,
                                                                       x.PassageWordCount, x.PassageTextType,
                                                                       x.PassageTextSubType, x.PassageFleschKinkaid, x.Qti3pItemDOK, dicMasterStandard),
                From3pUpload = x.From3pUpload
            }).ToList();

            results.ForEach(x =>
            {
                if (dicMasterStandard != null && dicMasterStandard.ContainsKey(x.QTI3pItemID))
                {
                    var sb = new StringBuilder();
                    var assignedStandards = dicMasterStandard[x.QTI3pItemID];
                    if (assignedStandards != null && assignedStandards.Count > 0)
                    {
                        var standardDisplayed = string.Format(", {0}", string.Join(", ", assignedStandards));
                        standardDisplayed = Server.HtmlEncode(standardDisplayed);
                        if (!string.IsNullOrEmpty(standardDisplayed) && standardDisplayed.StartsWith(","))
                        {
                            standardDisplayed = standardDisplayed.Remove(0, 1);
                        }
                        sb.Append(standardDisplayed);

                        x.Standard = sb.ToString();
                    }
                }
            });

            return Json(parser.Parse(results.AsQueryable(), totalRow), JsonRequestBehavior.AllowGet);
        }
    }
}
