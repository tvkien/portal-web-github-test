using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.SessionState;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class HomeController : BaseController
    {
        private readonly DistrictConfigurationService _districtConfigurationService;
        private readonly DistrictSlideService _districtSlideService;
        private readonly DSPDistrictService _dspDistrictService;
        private readonly IFormsAuthenticationService _formsAuthenticationService;
        private readonly UserService _userService;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly ConfigurationService _configurationService;
        private readonly DistrictService _districtService;

        public HomeController(DistrictConfigurationService districtConfigurationService, DistrictSlideService districtSlideService, DSPDistrictService dspDistrictService,
            IFormsAuthenticationService formsAuthenticationService, UserService userService, DistrictDecodeService districtDecodeService, ConfigurationService configurationService,
            DistrictService districtService)
        {
            this._districtConfigurationService = districtConfigurationService;
            this._districtSlideService = districtSlideService;
            this._dspDistrictService = dspDistrictService;
            this._formsAuthenticationService = formsAuthenticationService;
            this._userService = userService;
            this._districtDecodeService = districtDecodeService;
            this._configurationService = configurationService;
            this._districtService = districtService;
        }
        public ActionResult Index()
        {
            var model = BuildSlideShowData();
            return View(model);
        }

        private HomeViewModel BuildSlideShowData()
        {
            var model = new HomeViewModel();

            //get the sub domain
            int subDomainDistrictId = HelperExtensions.GetDistrictIdBySubdomain();

            var useCustomSlideShow = _districtConfigurationService
                .GetDistrictConfigurationByKey(subDomainDistrictId, CurrentUser.IsStudent
                                                                        ? DistrictConfigurationKey.UseStudentCustomSlideShow
                                                                        : CurrentUser.IsParent
                                                                        ? DistrictConfigurationKey.UseParentCustomSlideShow
                                                                        : DistrictConfigurationKey.UseCustomSlideShow);

            model.UseCustomSlideShow = false;
            model.DistrictSlideList = new List<DistrictSlide>();
            model.ShowWidgets = _districtService.ShowWidgets(subDomainDistrictId);

            if (useCustomSlideShow != null)
            {
                if (!string.IsNullOrEmpty(useCustomSlideShow.Value) && useCustomSlideShow.Value.ToLower().Equals("yes"))
                {
                    model.UseCustomSlideShow = true;
                    //get the slide configuration in DistrictSlide table
                    List<DistrictSlide> districtSlides = CurrentUser.IsStudent
                                                             ? _districtSlideService.GetDistrictStudentSlides(
                                                                 useCustomSlideShow.DistrictId)                                                             
                                                             : CurrentUser.IsParent ? _districtSlideService.GetDistrictParentlides(
                                                                 useCustomSlideShow.DistrictId)
                                                             : _districtSlideService.GetDistrictSlides(
                                                                 useCustomSlideShow.DistrictId);
                    if (districtSlides != null)
                    {
                        string domain = HelperExtensions.GetHTTPProtocal(Request) + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                        foreach (var districtSlide in districtSlides)
                        {
                            //check LinkTo if it's a absolute link or not
                            if (!string.IsNullOrEmpty(districtSlide.LinkTo) && true == districtSlide.LinkTo?.IsRelativeUrl())
                            {
                                //It's an relative link
                                if (districtSlide.LinkTo.StartsWith("/"))
                                {
                                    districtSlide.LinkTo = districtSlide.LinkTo.Remove(0);
                                }
                                districtSlide.LinkTo = string.Format("{0}/{1}", domain, districtSlide.LinkTo);
                            }

                            //if no image found in database, use an image no-slide-image-found.png
                            if (districtSlide.ImageName.Length == 0)
                            {
                                districtSlide.ImageName = string.Format("{0}/no-slide-image-found.png", LinkitConfigurationManager.GetS3Settings().SlideShowKey);
                            }
                            else
                            {
                                //create the full url of the image
                                var imgUrl = string.Format("{0}/{1}/{2}", LinkitConfigurationManager.GetS3Settings().SlideShowKey, useCustomSlideShow.DistrictId, districtSlide.ImageName);
                                //check if there's image or not
                                if (!ValidateLink(imgUrl))
                                {
                                    districtSlide.ImageName = string.Format("{0}/no-slide-image-found.png", LinkitConfigurationManager.GetS3Settings().SlideShowKey);
                                }
                                else
                                {
                                    districtSlide.ImageName = imgUrl;
                                }
                            }
                        }
                        model.DistrictSlideList = districtSlides;
                    }
                }
            }
            switch(CurrentUser.RoleId)
            {
                case 26:
                    {
                        model.RoleUrlConfig = "parent";
                    }
                    break;
                case 28:
                    {
                        model.RoleUrlConfig = "student";
                    }
                    break;
                default:
                    model.RoleUrlConfig = "staff";
                    break;               
            }
            return model;
        }

        private bool ValidateLink(string link)
        {
            try
            {
                if (string.IsNullOrEmpty(link))
                {
                    return false;
                }
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = client.GetAsync(link).Result;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        PortalAuditManager.LogException(ex);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return false;
            }
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.ReportingItemNew)]
        public ActionResult ReportingNew()
        {
            return View();
        }

        public ActionResult NetworkAdminSelect()
        {
            return View();
        }

        public ActionResult LoadDSPDistrict()
        {
            var data = _dspDistrictService.GetDistrictMembers(CurrentUser.DistrictId.Value).ToList()
                    .Select(o => new DistrictMemberViewModel()
                    {
                        Name = o.Name,
                        LiCode = o.LICode,
                        DistrictId = o.Id,
                        StateId = o.StateId,
                    }).AsQueryable();
            var parser = new DataTableParser<DistrictMemberViewModel>();
            return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult NetworkAdminImpersontateDistrictAdmin(int districtId, int stateId, string LiCode)
        {
            CurrentUser.DistrictId = districtId;
            CurrentUser.StateId = stateId;
            _formsAuthenticationService.SignIn(CurrentUser, false);
            _userService.UpdateLastLogin(CurrentUser.Id);
            if (string.IsNullOrEmpty(LiCode))
            {
                LiCode = "portal";
            }
            var redirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), LiCode, CurrentUser, isImpersonate: true);
            return Json(new { Success = "Success", RedirectUrl = redirectUrl }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Demo()
        {
            return View();
        }
    }
}
