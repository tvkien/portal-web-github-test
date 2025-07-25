using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class APILogController : Controller
    {
        private readonly APIAccountService apiAccountService;
        private readonly DistrictService districtService;
        private readonly UserService userService;

        public APILogController(APIAccountService apiAccountService, DistrictService districtService, UserService userService)
        {
            this.apiAccountService = apiAccountService;
            this.districtService = districtService;
            this.userService = userService;
        }

        // GET: /APILog/
        //[AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.TechAPILog)]
        [NonAction]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadAPILog(APILogFilter obj)
        {
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteAPILogByFilter(APILogFilter obj)
        {
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteAPILogByListIDs(string strListIDs)
        {
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadAPILogDetail(int aPILogId)
        {
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        private string GetAccesskeyByURL(string strURL)
        {
            string strTMP = strURL.Trim().ToLower();
            string strReturn = string.Empty;
            int index = strTMP.IndexOf("accesskey=");
            if (index > 0)
            {
                strTMP = strTMP.Substring(index);
                index = strTMP.IndexOf("&");
                if (index > 10) //TODO "accesskey=" with length = 10
                    strTMP = strTMP.Substring(10, index -10);
                strReturn = strTMP;
            }
            return strReturn;
        }

        private void MapAPIAccountInfor(APILogDetail obj)
        {
            string apiAccountType = string.Empty;
            string districtCustom = string.Empty;
            string userCustom = string.Empty;
            string strAccount = GetAccesskeyByURL(obj.RequestURL);
            var apiAccount = apiAccountService.GetAPIAccountByClientAccessKey(strAccount);
            if (apiAccount != null)
            {
                if (apiAccount.APIAccountTypeID == 1)
                {
                    apiAccountType = "District";
                    var vDistrict = districtService.GetDistrictById(apiAccount.TargetID);
                    if (vDistrict != null)
                        districtCustom = string.Format("{0} ({1})", vDistrict.Name, vDistrict.Id);
                }
                else
                {
                    apiAccountType = "User";
                    var vUser = userService.GetUserById(apiAccount.TargetID);
                    if (vUser != null)
                    {
                        userCustom = string.Format("{0} {1} ({2})", vUser.FirstName, vUser.LastName, vUser.UserName);
                        if (vUser.DistrictId.HasValue)
                        {
                            var vDistrict = districtService.GetDistrictById(vUser.DistrictId.Value);
                            if (vDistrict != null)
                                districtCustom = string.Format("{0} ({1})", vDistrict.Name, vDistrict.Id);
                        }
                    }
                }
            }
            obj.Account = strAccount;
            obj.AccountType = apiAccountType;
            obj.DistrictCustom = districtCustom;
            obj.UserCustom = userCustom;
        }
    }
}
