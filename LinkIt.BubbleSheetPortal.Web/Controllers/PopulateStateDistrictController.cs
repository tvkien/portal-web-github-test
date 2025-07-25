using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    [AjaxAwareAuthorize]
    public class PopulateStateDistrictController : BaseController
    {
        private readonly StateService stateService;
        private readonly DistrictService districtService;
        private readonly UserSchoolService userSchoolService;
        private readonly SchoolService schoolService;
        private readonly DSPDistrictService dspDistrictService;
        private readonly QTIITemService qTIITemService;
        private readonly UserService _userService;
        private readonly ConfigurationService _configurationService;
        public PopulateStateDistrictController(StateService stateService, DistrictService districtService, UserSchoolService userSchoolService, SchoolService schoolService, QTIITemService qTIITemService, DSPDistrictService dspDistrictService, UserService userService, ConfigurationService configurationService)
        {
            this.stateService = stateService;
            this.districtService = districtService;
            this.userSchoolService = userSchoolService;
            this.schoolService = schoolService;
            this.dspDistrictService = dspDistrictService;
            this.qTIITemService = qTIITemService;
            this._userService = userService;
            this._configurationService = configurationService;
        }

        [HttpGet]
        public ActionResult GetStates()
        {
            IQueryable<State> data = stateService.GetStates();
            data = data.OrderBy(x => x.Name);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetStatesForNetworkAdmin(int? organizationDistrictId)
        {
            if (organizationDistrictId == null)
            {
                var stateIdList =
                    districtService.GetDistricts().Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id)).Select(
                        x => x.StateId).ToList();
                IQueryable<State> data = stateService.GetStates().Where(x => stateIdList.Contains(x.Id));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //Get states of member districts of an orginization district
                List<int> memberDistrictIds = dspDistrictService.GetMemberDistrictId(organizationDistrictId.Value);
                var stateIdList =
                   districtService.GetDistricts().Where(x => memberDistrictIds.Contains(x.Id)).Select(
                       x => x.StateId).ToList();
                IQueryable<State> data = stateService.GetStates().Where(x => stateIdList.Contains(x.Id));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetDistricts(int? stateId)
        {
            IQueryable<District> districts = stateId.HasValue ? districtService.GetDistrictsByStateId(stateId.GetValueOrDefault()) : districtService.GetDistricts();
            IOrderedQueryable<ListItem> data = districts.Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDistrictsForNetworkAdmin(int? stateId)
        {
            IQueryable<District> districts = stateId.HasValue ? districtService.GetDistrictsByStateId(stateId.GetValueOrDefault()) : districtService.GetDistricts();
            districts = districts.Where(x => CurrentUser.GetMemberListDistrictId().Contains(x.Id));
            IOrderedQueryable<ListItem> data = districts.Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMemberDistricts(int organizationDistrictId, int? stateId)
        {
            if (stateId == null)
            {
                List<int> memberDistrictIds = dspDistrictService.GetMemberDistrictId(organizationDistrictId);
                IQueryable<District> districts = districtService.GetDistricts().Where(x => memberDistrictIds.Contains(x.Id));
                IOrderedQueryable<ListItem> data = districts.Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<int> memberDistrictIds = dspDistrictService.GetMemberDistrictId(organizationDistrictId);
                IQueryable<District> districts = districtService.GetDistricts().Where(x => memberDistrictIds.Contains(x.Id) && x.StateId == stateId.Value);
                IOrderedQueryable<ListItem> data = districts.Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        ///  get list state by list dictricsId
        /// </summary>
        /// <param name="dictricIds"></param>
        /// <returns></returns>
        public ActionResult GetStatesByDictricIds(string dictricIds)
        {
            var disIds = ConvertStringIdsToListId(dictricIds);
            var stateIds = districtService.GetStateIdByDictricIds(disIds);//Get list id state
            IQueryable<State> data = stateService.GetStatesByIds(stateIds);//Get state by list state id
            data = data.OrderBy(x => x.Name);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSingleDistrict(int? districtId)
        {
            var district = districtId.HasValue ? districtService.GetDistrictById(districtId.GetValueOrDefault()) : null;
            var listDistrict = new List<District>();
            if (district != null)
            {
                listDistrict.Add(district);
            }
            var data = listDistrict.Select(x => new ListItem { Id = x.Id, Name = x.Name }).OrderBy(x => x.Name);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// convert from string id to List id type int
        /// </summary>
        /// <param name="strIds"></param>
        /// <returns></returns>
        private List<int> ConvertStringIdsToListId(string strIds)
        {
            var arrIds = strIds.Split(new[] { ',' });//split string id to array id
            return arrIds.Select(int.Parse).ToList();//convert to list id type int
        }
        [HttpGet]
        public ActionResult GetStateStandard()
        {
            var data = stateService.GetStates();
            if (!CurrentUser.IsPublisher && CurrentUser.StateId.HasValue)
            {
                data = stateService.GetStates().Where(o => o.Code == "CC" || o.Id == CurrentUser.StateId.Value);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<State> GetStateStandardForAssessment()
        {
            // Common Core + standards from the state of the Schools that the user is associated with. 
            // If StateID is null on the school, use the StateID of the district. 
            //if current user is Network admin, add his/her states of districts that he/she has access to
            List<State> result = stateService.GetStateForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), true, CurrentUser.IsDistrictAdmin);
            return result;
        }
        [HttpGet]
        public ActionResult GetStateStandardWithCC()
        {
            List<State> result = stateService.GetStateForUser(CurrentUser.Id, CurrentUser.DistrictId.GetValueOrDefault(), false, true);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetStateStandardWithCCForFilter(bool qti3p, bool personal, int? qti3pSourceId, bool districtSearch)
        {
            List<ListItem> result = new List<ListItem>();
            List<State> qtiState = new List<State>();

            if (qti3p)
            {
                qtiState = qTIITemService.GetStatesQTI3pItem(qti3pSourceId, CurrentUser.Id).ToList();
            }
            else
            {
                int? userId = null;
                int? districtId = null;
                if (personal && districtSearch)
                {
                    userId = CurrentUser.Id;
                    districtId = CurrentUser.DistrictId.Value;
                }
                else if (personal)
                {
                    userId = CurrentUser.Id;
                    districtId = -1;
                }
                else
                {
                    districtId = CurrentUser.DistrictId.Value;
                    userId = -1;
                }

                qtiState = qTIITemService.GetStatesQTIItem(userId, districtId, CurrentUser.Id).ToList();
            }

            result = qtiState.Select(x => new ListItem { Id = x.Id, Name = x.Name }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStateByPublisherOrNetworkAdmin()
        {
            //This function only support publisher & NetworkAdmin
            if (CurrentUser.RoleId == (int)Permissions.NetworkAdmin)
            {
                var vUser = _userService.GetUserById(CurrentUser.Id);
                int districtId = vUser.DistrictId.HasValue ? vUser.DistrictId.Value : 0;
                var v = dspDistrictService.GetStateByDistrictNetWorkAdmin(districtId).OrderBy(x => x.Name);
                return Json(v, JsonRequestBehavior.AllowGet);
            }
            else if (CurrentUser.RoleId == (int)Permissions.Publisher)
            {
                IQueryable<State> data = stateService.GetStates();
                data = data.OrderBy(x => x.Name);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchoolsByDistrict(int districtId)
        {
            var schools = schoolService.GetSchoolsByDistrictId(districtId).Select(x => new SchoolListViewModel
            {
                SchoolID = x.Id,
                SchoolName = x.Name,
                Code = x.Code,
                StateCode = x.StateCode
            });
            return Json(schools, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLanguageSupportImportQTI3pItem()
        {
            var config = _configurationService.GetConfigurationByKey(ConfigurationKey.LanguageSupportImportQTI3pItem);
            if (config != null)
            {
                var lstLanguage = JsonConvert.DeserializeObject<List<SelectItemCustom>>(config.Value);
                return Json(lstLanguage, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

    }
}
