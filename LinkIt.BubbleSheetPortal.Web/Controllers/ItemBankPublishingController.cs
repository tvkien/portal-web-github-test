using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    public class ItemBankPublishingController : BaseController
    {
        #region Fields

        private readonly ItemLibraryManagementControllerParameters _parameters;

        private int SelectedQtiBankId
        {
            get { return GetSessionValue("SelectedQtiBankId"); }
            set { System.Web.HttpContext.Current.Session["SelectedQtiBankId"] = value; }
        }

        private int GetSessionValue(string valueName)
        {
            if (System.Web.HttpContext.Current.Session[valueName].IsNull())
            {
                return 0;
            }

            int value;
            if (int.TryParse(System.Web.HttpContext.Current.Session[valueName].ToString(), out value))
            {
                return value;
            }

            return 0;
        }

        #endregion

        #region Ctor

        public ItemBankPublishingController(ItemLibraryManagementControllerParameters parameters)
        {
            _parameters = parameters;
        }

        #endregion

        #region Methods

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ItemLibraryManagement)]
        public ActionResult Index()
        {
            if (CurrentUser.IsPublisher())
                ViewBag.IsPublisher = true;
            else
            {
                ViewBag.IsPublisher = false;
                ViewBag.DistrictId = CurrentUser.DistrictId;
            }
            ViewBag.IsNetworkAdmin = false;
            ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            if (CurrentUser.IsNetworkAdmin)
            {
                ViewBag.IsNetworkAdmin = true;
                ViewBag.ListDictrictIds = ConvertListIdToStringId(CurrentUser.GetMemberListDistrictId());
                ViewBag.DistrictId = CurrentUser.DistrictId;
                ViewBag.StateId = CurrentUser.StateId;
            }
            return View();
        }

        [HttpGet]
        public ActionResult GetItemBankList(int? districtId, string bankName, string author, string publishedTo)
        {
            if (districtId != null && districtId > 0)
            {
                var userDistrict = _parameters.DistrictServices.GetDistrictById(CurrentUser.DistrictId ?? 0);
                string district = userDistrict == null ? string.Empty : userDistrict.Name;
                district = district ?? "";

                IQueryable<QtiBankListViewModel> data = _parameters
                    .QTIBankServices.GetQtiBankList(
                        CurrentUser.Id,
                        districtId.Value,
                        bankName,
                        author,
                        publishedTo)
                    .Select(en => new QtiBankListViewModel
                    {
                        QtiBankId = en.QtiBankId,
                        Name = en.Name,
                        Author = en.Author,
                        QtiGroupSet = en.QtiGroupSet,
                        DistrictNames = RestrictDistrictNames(en.DistrictNames ?? string.Empty, district),
                        SchoolNames = en.SchoolNames ?? string.Empty
                    });

                var parser = new DataTableParser<QtiBankListViewModel>();
                return new JsonNetResult { Data = parser.Parse(data) };
            }
            else
            {
                var data = new List<QtiBankListViewModel>();
                var parser = new DataTableParser<QtiBankListViewModel>();
                return new JsonNetResult { Data = parser.Parse(data.AsQueryable()) };
            }
        }

        [HttpGet]
        public ActionResult GetItemBankListV2(int? districtId, string bankName, string author, string publishedTo)
        {
            if (districtId != null && districtId > 0)
            {
                var userDistrict = _parameters.DistrictServices.GetDistrictById(CurrentUser.DistrictId ?? 0);
                string district = userDistrict == null ? string.Empty : userDistrict.Name;
                district = district ?? "";

                IQueryable<QtiBankListViewModelV2> data = _parameters
                    .QTIBankServices.GetQtiBankList(
                        CurrentUser.Id,
                        districtId.Value,
                        bankName,
                        author,
                        publishedTo)
                    .Select(en => new QtiBankListViewModelV2
                    {
                        QtiBankId = en.QtiBankId,
                        Name = en.Name,
                        Author = en.Author,
                        QtiGroupSet = en.QtiGroupSet,
                        DistrictNames = RestrictDistrictNames(en.DistrictNames ?? string.Empty, district),
                        SchoolNames = en.SchoolNames ?? string.Empty
                    });

                var parser = new DataTableParser<QtiBankListViewModelV2>();
                return new JsonNetResult { Data = parser.Parse(data) };
            }
            else
            {
                var data = new List<QtiBankListViewModelV2>();
                var parser = new DataTableParser<QtiBankListViewModelV2>();
                return new JsonNetResult { Data = parser.Parse(data.AsQueryable()) };
            }
        }

        private string RestrictDistrictNames(string districtNames, string userDistrict)
        {
            string result = districtNames;
            if (!CurrentUser.IsPublisher)
            {
                if (districtNames.ToLower().Contains(userDistrict.ToLower()))
                {
                    result = userDistrict;
                }
                else
                {
                    result = string.Empty;
                }
            }
            return result;
        }
        [HttpGet]
        public ActionResult GetPublishedDistrict(int qtiBankId)
        {
            if (CurrentUser.IsPublisher)
            {
                IQueryable<QtiBankPublishedDistrictViewModel> data = _parameters
                    .QTIBankServices.GetPublishedDistrict(qtiBankId)
                    .Select(x => new QtiBankPublishedDistrictViewModel
                    {
                        DistrictId = x.DistrictId,
                        Name = x.Name,
                        QtiBankDistrictId = x.QtiBankDistrictId,
                        QtiBankId = x.QtiBankId
                    }
                    );
                if (CurrentUser.IsNetworkAdmin)
                {
                    data = data.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.DistrictId));
                }
                var parser = new DataTableParser<QtiBankPublishedDistrictViewModel>();
                return new JsonNetResult { Data = parser.Parse(data) };
            }
            else
            {
                IQueryable<QtiBankPublishedDistrictViewModel> data = _parameters
                    .QTIBankServices.GetPublishedDistrict(qtiBankId).Where(x => x.DistrictId == CurrentUser.DistrictId)
                    .Select(x => new QtiBankPublishedDistrictViewModel
                    {
                        DistrictId = x.DistrictId,
                        Name = x.Name,
                        QtiBankDistrictId = x.QtiBankDistrictId,
                        QtiBankId = x.QtiBankId
                    }
                    );

                var parser = new DataTableParser<QtiBankPublishedDistrictViewModel>();
                return new JsonNetResult { Data = parser.Parse(data) };
            }
        }

        [HttpGet]
        public ActionResult GetPublishedSchool(int qtiBankId)
        {
            IQueryable<QtiBankPublishedSchoolViewModel> data = _parameters
                .QTIBankServices.GetPublishedSchool(qtiBankId)
                .Select(x => new QtiBankPublishedSchoolViewModel
                {
                    DistrictName = x.DistrictName,
                    Name = x.Name,
                    QtiBankId = x.QtiBankId,
                    QtiBankSchoolId = x.QtiBankSchoolId,
                    SchoolId = x.SchoolId
                }
                );

            var parser = new DataTableParser<QtiBankPublishedSchoolViewModel>();
            return new JsonNetResult { Data = parser.Parse(data) };
        }


        public ActionResult LoadListOrShareDistrict(int qtiBankId)
        {
            SelectedQtiBankId = qtiBankId;
            return PartialView("_ListOrShareDistrict", qtiBankId);
        }

        public ActionResult LoadListDistrictByQtiBank(int qtiBankId)
        {
            ViewBag.IsPublisher = CurrentUser.IsPublisher;
            ViewBag.IsDistrictAdmin = CurrentUser.IsDistrictAdmin;
            //When current user is district admin, check if the bank is share to this district or not
            var shareToDistrict = _parameters.QTIBankServices.CheckBankSharedToDistrict(qtiBankId, CurrentUser.DistrictId ?? 0);
            ViewBag.ShareToDistrict = shareToDistrict;
            ViewBag.QtiBankId = qtiBankId;
            return PartialView("_ListDistrictByQtiBank", qtiBankId);
        }

        public ActionResult LoadPublishToDistrict()
        {
            var model = new QtiBankPublishToDistrictViewModel();
            model.IsPublisher = CurrentUser.IsPublisher;
            model.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_PublishToDistrict", model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DepublishDistrict(int qtiBankDistrictId)
        {
            var qtiBankDistrict = _parameters.QtiBankDistrictServices.GetById(qtiBankDistrictId);
            if (qtiBankDistrict.IsNull())
            {
                return Json(false);
            }
            _parameters.QtiBankDistrictServices.Delete(qtiBankDistrict);
            return Json(true);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetUnPublishedDistrictsByState(int stateId)
        {
            var data = _parameters.QtiBankDistrictServices.GetUnPublishedDistrict(stateId, SelectedQtiBankId).OrderBy(x => x.Name).ToList();
            if (CurrentUser.IsNetworkAdmin)
            {
                data = data.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id)).OrderBy(x => x.Name).ToList();
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PublishToDistrict(QtiBankPublishToDistrictViewModel model)
        {
            model.SetValidator(_parameters.QtiBankPublishToDistrictViewModelValidator);
            if (!model.IsValid)
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }

            var qtiBank = _parameters.QTIBankServices.GetById(SelectedQtiBankId);
            if (qtiBank == null)
            {
                return ShowJsonResultException(model, "Item bank does not exist.");
            }

            var district = _parameters.DistrictServices.GetDistrictById(model.DistrictId);
            if (district == null)
            {
                return ShowJsonResultException(model, "" + LabelHelper.DistrictLabel + " does not exist.");
            }

            var qtiBankDistrict =
                _parameters.QtiBankDistrictServices.Select().FirstOrDefault(
                    x => x.DistrictId.Equals(model.DistrictId) && x.QtiBankId.Equals(SelectedQtiBankId));
            if (qtiBankDistrict != null)
            {
                return ShowJsonResultException(model, "Bank is already shared to this " + LabelHelper.DistrictLabel + ".");
            }

            _parameters.QtiBankDistrictServices.Save(InitializeQtiBankDistrict(model.DistrictId));

            return Json(new { Success = true });
        }

        public ActionResult LoadListOrShareSchool(int qtiBankId)
        {
            SelectedQtiBankId = qtiBankId;
            return PartialView("_ListOrShareSchool", qtiBankId);
        }

        public ActionResult LoadListSchoolByQtiBank(int qtiBankId)
        {
            ViewBag.IsPublisher = CurrentUser.IsPublisher();
            return PartialView("_ListSchoolByQtiBank", qtiBankId);
        }

        public ActionResult LoadPublishToSchool()
        {
            if (CurrentUser.IsPublisher())
                ViewBag.IsPublisher = true;
            else
            {
                ViewBag.IsPublisher = false;
                ViewBag.DistrictId = CurrentUser.DistrictId;
            }
            ViewBag.IsNetworkAdmin = CurrentUser.IsNetworkAdmin;
            return PartialView("_PublishToSchool", new QtiBankPublishToSchoolViewModel());
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult DepublishSchool(int qtiBankSchoolId)
        {
            var qtiBankSchool = _parameters.QtiBankSchoolServices.GetById(qtiBankSchoolId);
            if (qtiBankSchool.IsNull())
            {
                return Json(false);
            }
            _parameters.QtiBankSchoolServices.Delete(qtiBankSchool);
            return Json(true);
        }

        [HttpGet, AjaxOnly]
        public ActionResult GetUnPublishedSchoolsByDistrict(int districtId)
        {
            var data = _parameters.QtiBankSchoolServices.GetUnPublishedSchool(districtId, SelectedQtiBankId).OrderBy(x => x.Name).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult PublishToSchool(QtiBankPublishToSchoolViewModel model)
        {
            model.SetValidator(_parameters.QtiBankPublishToSchoolViewModelValidator);
            if (!model.IsValid)
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }

            var qtiBank = _parameters.QTIBankServices.GetById(SelectedQtiBankId);
            if (qtiBank == null)
            {
                return ShowJsonResultException(model, "Item bank does not exist.");
            }

            var school = _parameters.SchoolServices.GetSchoolById(model.SchoolId);
            if (school == null)
            {
                return ShowJsonResultException(model, "School does not exist.");
            }

            var qtiBankSchool =
                _parameters.QtiBankSchoolServices.Select().FirstOrDefault(
                    x => x.SchoolId.Equals(model.SchoolId) && x.QtiBankId.Equals(SelectedQtiBankId));
            if (qtiBankSchool != null)
            {
                return ShowJsonResultException(model, "Bank is already shared to this school.");
            }

            _parameters.QtiBankSchoolServices.Save(InitializeQtiBankSchool(model.SchoolId));

            return Json(new { Success = true });
        }

        private QtiBankDistrict InitializeQtiBankDistrict(int districtId)
        {
            return new QtiBankDistrict
            {
                DistrictId = districtId,
                EditedByUserId = CurrentUser.Id,
                QtiBankId = SelectedQtiBankId
            };
        }

        private QtiBankSchool InitializeQtiBankSchool(int schoolId)
        {
            return new QtiBankSchool
            {
                SchoolId = schoolId,
                EditedByUserId = CurrentUser.Id,
                QtiBankId = SelectedQtiBankId
            };
        }
        [HttpPost]
        [AjaxOnly]
        public ActionResult ShareToMyDistrict(int qtiBankId)
        {
            var qtiBankDistrict = new QtiBankDistrict
            {
                DistrictId = CurrentUser.DistrictId.Value,
                EditedByUserId = CurrentUser.Id,
                QtiBankId = qtiBankId
            };
            _parameters.QtiBankDistrictServices.Save(qtiBankDistrict);

            return Json(new { Success = true });
        }

        public string ConvertListIdToStringId(List<int> ListDistricIds)
        {
            var ids = string.Empty;
            if (!ListDistricIds.Any())
            {
                return ids;
            }
            ids = ListDistricIds.Aggregate(ids, (current, id) => current + (id + ","));
            return ids.TrimEnd(new[] { ',' });
        }

        #endregion
    }
}
