using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using System.Collections.Generic;
using System.Web.Routing;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using System.Globalization;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public static class HelperExtensions
    {
        private static List<KeyValuePair<string, string>> mappingURLs;
        public static List<KeyValuePair<string, string>> MappingURLs
        {
            get
            {
                if (mappingURLs == null)
                {
                    HttpContextWrapper httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
                    UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, RouteTable.Routes.GetRouteData(httpContextWrapper)));

                    mappingURLs = new List<KeyValuePair<string, string>>();

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.Home, urlHelper.Action("Index", "Home")));

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TestdesignAssement, urlHelper.Action("AssessmentItems", "Test")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TestdesignPassages, urlHelper.Action("Passages", "Test")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TestdesignTests, urlHelper.Action("Tests", "Test")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TestdesignAssementOld, urlHelper.Action("AssessmentItemsOld", "Test")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TestdesignPassagesOld, urlHelper.Action("PassagesOld", "Test")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TestdesignTestsOld, urlHelper.Action("TestsOld", "Test")));

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TestdesignRubric, urlHelper.Action("Index", "Rubric")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.ItemLibraryManagement, urlHelper.Action("Index", "ItemLibraryManagement")));

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.ManagebubblesheetsCreate, urlHelper.Action("Generate", "GenerateBubbleSheet")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.ManagebubblesheetsGrade, urlHelper.Action("Grade", "BubbleSheet")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.ManagebubblesheetsReview, urlHelper.Action("Index", "BubbleSheetReview")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.ManagebubblesheetsError, urlHelper.Action("ProcessErrors", "ErrorCorrection")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.ManagebubblesheetsGimport, urlHelper.Action("ImportGroup", "GroupPrinting")));

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.OnlinetestingItem, urlHelper.Action("AssignTests", "Test", new { id = "testAssignment" })));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.OnlineTestAssignmentRewrite, urlHelper.Action("Index", "TestAssignment")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.OnlineTestAssignmentReview, urlHelper.Action("Index", "TestAssignmentReview")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.OnlinetestPreference, urlHelper.Action("Index", "TestPreference")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.OnlinetestLockUnlockBank, urlHelper.Action("LockUnlockTestBanks", "TestPreference")));

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.ManageSurveys, urlHelper.Action("Index", "ManageSurvey")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.AssignSurveys, urlHelper.Action("Index", "AssignSurvey")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.ReviewSurveys, urlHelper.Action("Index", "ReviewSurvey")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TakeSurveys, urlHelper.Action("Index", "TakeSurvey")));

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TmgmtUploadassessmentresults, urlHelper.Action("UploadAssessmentResults", "Admin")));
                    //mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TmgmtEinstructionimport, urlHelper.Action("Import", "EInstruction")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TmgmtTestresultremover, urlHelper.Action("TestResultRemover", "Admin")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TmgmtTestregrader, urlHelper.Action("TestRegrader", "Admin")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TmgmtPurgetest, urlHelper.Action("PurgeTest", "Admin")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TmgmtPrinttest, urlHelper.Action("Index", "PrintTest")));
                    //mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TmgmtCustomAssessments, urlHelper.Action("CustomAssessments", "Admin")));

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.LearningLibrarySearch, urlHelper.Action("Index", "LearningLibrarySearch")));

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.DadManageuser, urlHelper.Action("ManageUsers", "Admin")));
                    //mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.DadDistrictReferencedata, urlHelper.Action("DistrictReferenceData", "Admin")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.DadDistrictUsage, urlHelper.Action("DistrictUsage", "ReportService")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.DadManageSchools, urlHelper.Action("ManageSchoolAndClass", "ManageClasses")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.DadManageClasses, urlHelper.Action("ManageClass", "ManageClasses")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.DadManageRosters, urlHelper.Action("ManageRosters", "Admin")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.DadManageProgram, urlHelper.Action("Index", "ManageProgram")));

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.HelpIntroduction, urlHelper.Action("Introduction", "Help")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.HelpGuide, "http://help.linkit.com"));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.HelpVideotutorials, urlHelper.Action("VideoTutorials", "Help")));

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.MessageInbox, urlHelper.Action("Inbox", "MailBox")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.DadManageParents, urlHelper.Action("ManageParents", "Parent")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.PContactInfo, urlHelper.Action("ParentContactInfo", "MailBox")));

                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.SettingItem, urlHelper.Action("Settings", "Account")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TechMgmtReports, urlHelper.Action("Index", "Report")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TechETLtool, urlHelper.Action("ManageMapping", "Admin")));
                    mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TechUserImpersonation, urlHelper.Action("UserImpersonation", "Admin")));
                    //mappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.TechAPILog, urlHelper.Action("Index", "APILog")));

                }

                return mappingURLs;
            }
            set
            {
                mappingURLs = value;
            }
        }

        private static List<KeyValuePair<string, string>> parrentMappingURLs;
        public static List<KeyValuePair<string, string>> ParrentMappingURLs
        {
            get
            {
                if (parrentMappingURLs == null)
                {
                    var httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
                    var urlHelper = new UrlHelper(new RequestContext(httpContextWrapper, RouteTable.Routes.GetRouteData(httpContextWrapper)));

                    parrentMappingURLs = new List<KeyValuePair<string, string>>();

                    parrentMappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.MessageInbox, urlHelper.Action("Inbox", "MailBox")));
                    parrentMappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.DadManageParents, urlHelper.Action("ManageParents", "Parent")));
                    parrentMappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.PContactInfo, urlHelper.Action("ParentContactInfo", "MailBox")));

                    parrentMappingURLs.Add(new KeyValuePair<string, string>(ContaintUtil.SettingItem, urlHelper.Action("Settings", "Account")));
                }

                return parrentMappingURLs;
            }
            set
            {
                parrentMappingURLs = value;
            }
        }

        public static string GetStartUrlForAuthenticatedUser(string Scheme, string subDomain, User currentUser, bool isImpersonate = false, string endPath = "")
        {
            var result = "/";

            if (!isImpersonate) // Always redirect to Home when impersonate => to fix logout error when redirect to readonly session page right after impersonate
            {
                var menu = GetMenuForDistrict(currentUser);
                if (menu != null)
                {
                    var mappingUrls = currentUser.IsParent ? ParrentMappingURLs : MappingURLs;
                    foreach (var mappingUrl in mappingUrls)
                    {
                        if (menu.HasDisplayedItem(mappingUrl.Key))
                        {
                            result = mappingUrl.Value;
                            break;
                        }
                    }
                }
            }

            var url = ConfigurationManager.AppSettings["LinkItUrl"];
            if (!string.IsNullOrEmpty(endPath))
                result = string.Format("{0}://{1}.{2}{3}", Scheme, subDomain, url, endPath);
            else
                result = string.Format("{0}://{1}.{2}{3}", Scheme, subDomain, url, result);

            return result;
        }

        public static int GetDistrictIdBySubdomain()
        {
            int subDomainDistrictId = 0;
            if (HttpContext.Current.Session["SubDomainDistrictID"] == null)
            {
                DistrictService districtService = DependencyResolver.Current.GetService<DistrictService>();
                var subDomain = HttpContext.Current.Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                subDomainDistrictId = districtService.GetLiCodeBySubDomain(subDomain);
                HttpContext.Current.Session["SubDomainDistrictID"] = subDomainDistrictId;
            }
            else
            {
                subDomainDistrictId = (int)HttpContext.Current.Session["SubDomainDistrictID"];
            }
            return subDomainDistrictId;
        }

        public static int GetDistrictIdBySubdomainV2()
        {
            int subDomainDistrictId = 0;
            DistrictService districtService = DependencyResolver.Current.GetService<DistrictService>();
            var subDomain = HttpContext.Current.Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            subDomainDistrictId = districtService.GetLiCodeBySubDomain(subDomain);
            return subDomainDistrictId;
        }

        public static string GetSubdomain()
        {
            if (HttpContext.Current != null)
            {
                var subDomain = HttpContext.Current.Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                return subDomain;
            }
            return string.Empty;
        }

        public static int LoginDistrict
        {
            get
            {
                if (HttpContext.Current.Session["LoginDistrict"] == null)
                {
                    DistrictService districtService = DependencyResolver.Current.GetService<DistrictService>();
                    var subDomain = HttpContext.Current.Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                    HttpContext.Current.Session["LoginDistrict"] = districtService.GetLiCodeBySubDomain(subDomain);
                }

                return (int)HttpContext.Current.Session["LoginDistrict"];
            }
            set
            {
                HttpContext.Current.Session["LoginDistrict"] = value;
            }
        }

        public static int DistrictIdBySubDomain
        {
            get
            {
                DistrictService districtService = DependencyResolver.Current.GetService<DistrictService>();
                var subDomain = HttpContext.Current.Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                return districtService.GetLiCodeBySubDomain(subDomain);
            }

        }
        public static bool IsShowIconHelpTextInfo(int districtid)
        {
            if (HttpContext.Current.Session["IsShowIconHelpTextInfo"] == null)
            {
                //check districtdecode to detect district ON/OFF icon helptext
                var isShow = true;
                DistrictDecodeService districtDecodeService =
                    DependencyResolver.Current.GetService<DistrictDecodeService>();
                var districtDecode =
                    districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(
                        districtid, Util.IsShowIconHelpTextInfo).FirstOrDefault();
                if (districtDecode != null)
                {
                    var value = districtDecode.Value;
                    if (!string.IsNullOrEmpty(value) && value.ToLower() == "false")
                    {
                        isShow = false;
                    }
                }
                HttpContext.Current.Session["IsShowIconHelpTextInfo"] = isShow;
            }

            return (bool)HttpContext.Current.Session["IsShowIconHelpTextInfo"];
        }
        public static string GetGoogleAnalyticsTrackingScript()
        {
            if (HttpContext.Current.Session["GoogleAnalyticsTrackingScript"] == null)
            {
                ConfigurationService configurationService =
                    DependencyResolver.Current.GetService<ConfigurationService>();
                var configuration =
                    configurationService.GetConfigurationByKey(Util.GoogleAnalyticsTrackingScript);
                if (configuration != null)
                {
                    HttpContext.Current.Session["GoogleAnalyticsTrackingScript"] = configuration.Value;

                }
            }

            if (HttpContext.Current.Session["GoogleAnalyticsTrackingScript"] == null)
                return string.Empty;

            return (string)HttpContext.Current.Session["GoogleAnalyticsTrackingScript"];
        }
        public static MenuAccessItems GetMenuForDistrict(User oUser)
        {
            MenuAccessItems objMenu = (MenuAccessItems)HttpContext.Current.Session["MenuItem"];
            if (objMenu == null || objMenu.RoleId != oUser.RoleId)
            {
                XLIMenuPermissionService bizXLIMenuPermission = DependencyResolver.Current.GetService<XLIMenuPermissionService>();
                objMenu = bizXLIMenuPermission.GetMenuAccessByDistrict(oUser);
                FillMenuItemLabels(objMenu);
                HttpContext.Current.Session["MenuItem"] = objMenu;
            }
            return objMenu;
        }

        public static void FillMenuItemLabels(MenuAccessItems objMenu)
        {
            XLIMenuPermissionService service = DependencyResolver.Current.GetService<XLIMenuPermissionService>();
            var mainMenu = service.GetMainMenus();
            var subMenu = service.GetSubMenus();

            objMenu.MainMenuItems.Clear();
            foreach (var item in mainMenu)
            {
                var menu = new MenuItemLabel(item.Code, item.DisplayName, item.DisplayTooltip, string.Empty);
                menu.SubMenuItems = subMenu.Where(x => x.XliAreaId == item.XliAreaId).Select(x => new MenuItemLabel(x.Code, x.DisplayName, x.DisplayTooltip, x.HelpText, x.Path)).ToList();
                objMenu.MainMenuItems.Add(item.Code, menu);
            }
                
            objMenu.SubMenuItems.Clear();
            foreach (var item in subMenu)
                objMenu.SubMenuItems.Add(item.Code, new MenuItemLabel(item.Code, item.DisplayName, item.DisplayTooltip, item.HelpText, item.Path));
        }

        public static string GetS3CSSURL()
        {
            var tmp = LinkitConfigurationManager.GetS3Settings().S3CSSKey;
            if (!string.IsNullOrEmpty(tmp))
                return tmp;
            return string.Empty;
        }

        public static string GetSlideShowKey()
        {
            var tmp = LinkitConfigurationManager.GetS3Settings().SlideShowKey;
            if (!string.IsNullOrEmpty(tmp))
                return tmp;
            return string.Empty;
        }

        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }

        public static string BuildUserGuide(User user)
        {
            var email = user.EmailAddress;
            if (!string.IsNullOrWhiteSpace(email))
            {
                string key = Util.ReadValue("SSOKey", "11d5d4e0423feb4fa3c19eb841e86a61");
                const string pathTemplate =
                    "https://linkithelp.freshdesk.com/login/sso?name={0}&email={1}&timestamp={2}&hash={3}";

                var username = user.UserName;
                string timems = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
                var hash = Util.GetHash(key, username, email, timems);
                var path = String.Format(pathTemplate, HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(email), timems, hash);
                return path;
            }
            return string.Empty;
        }
        public static MvcHtmlString DisplayStaticHtml(this HtmlHelper helper, string url)
        {
            var str = string.Empty;
            if (!string.IsNullOrWhiteSpace(url))
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    str = response.Content.ReadAsStringAsync().Result;
                }

                if (str.Contains("<Error><Code>NoSuchKey</Code><Message>The specified key does not exist.</Message>"))
                    str = string.Empty;
            }

            return new MvcHtmlString(str);
        }

        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, TEnum selectedValue)
        {
            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>();

            IEnumerable<SelectListItem> items =
                from value in values
                select new SelectListItem
                {
                    Text = GetEnumDescription(value),
                    Value = value.ToString(),
                    Selected = (value.Equals(selectedValue))
                };

            return htmlHelper.DropDownList(
                name,
                items
                );
        }

        public static string BuildStartUrlForAuthenticatedUser(string scheme, string subDomain)
        {
            var url = ConfigurationManager.AppSettings["LinkItUrl"];
            var result = string.Format("{0}://{1}.{2}", scheme, subDomain, url);
            return result;
        }

        public static string GetSubdomain(string domain = null)
        {
            var subdomain = HttpContext.Current.Request.Url.Host;
            if (subdomain != null)
            {
                if (domain == null)
                {
                    // Since we were not provided with a known domain, assume that second-to-last period divides the subdomain from the domain.
                    var nodes = HttpContext.Current.Request.Url.Host.Split('.');
                    var lastNodeIndex = nodes.Length - 1;
                    if (lastNodeIndex > 0)
                        domain = nodes[lastNodeIndex - 1] + "." + nodes[lastNodeIndex];
                }

                // Verify that what we think is the domain is truly the ending of the hostname... otherwise we're hooped.
                if (!subdomain.EndsWith(domain))
                    throw new ArgumentException("Site was not loaded from the expected domain");

                // Quash the domain portion, which should leave us with the subdomain and a trailing dot IF there is a subdomain.
                subdomain = subdomain.Replace(domain, "");
                // Check if we have anything left.  If we don't, there was no subdomain, the request was directly to the root domain:
                if (string.IsNullOrWhiteSpace(subdomain))
                    return null;

                // Quash any trailing periods
                subdomain = subdomain.TrimEnd(new[] { '.' });
            }

            return subdomain;
        }

        public static bool DisableNotificationFeature
        {
            get
            {
                if (HttpContext.Current.Session["DisableNotificationFeature"] == null)
                {
                    var disableNotificationFeature = true;

                    var currentUser = HttpContext.Current.GetCurrentUser();

                    // Only support notification feature for these roles
                    if (currentUser.IsPublisher || currentUser.IsNetworkAdmin || currentUser.IsDistrictAdmin || currentUser.IsSchoolAdmin || currentUser.IsTeacher)
                    {
                        DistrictDecodeService districtDecodeService = DependencyResolver.Current.GetService<DistrictDecodeService>();
                        disableNotificationFeature =
                            districtDecodeService.GetDistrictDecodeByLabel(currentUser.DistrictId.GetValueOrDefault(), "DisableNotificationFeature");
                    }

                    HttpContext.Current.Session["DisableNotificationFeature"] = disableNotificationFeature;
                }

                return (bool)HttpContext.Current.Session["DisableNotificationFeature"];
            }
        }

        public static bool HasDisableNotificationFeature(User currentUser)
        {
            if (HttpContext.Current.Session["DisableNotificationFeature"] == null)
            {
                var disableNotificationFeature = true;

                // Only support notification feature for these roles
                if (currentUser.IsPublisher || currentUser.IsNetworkAdmin || currentUser.IsDistrictAdmin || currentUser.IsSchoolAdmin || currentUser.IsTeacher)
                {
                    DistrictDecodeService districtDecodeService = DependencyResolver.Current.GetService<DistrictDecodeService>();
                    disableNotificationFeature =
                        districtDecodeService.GetDistrictDecodeByLabel(currentUser.DistrictId.GetValueOrDefault(), "DisableNotificationFeature");
                }

                HttpContext.Current.Session["DisableNotificationFeature"] = disableNotificationFeature;
            }

            return (bool)HttpContext.Current.Session["DisableNotificationFeature"];
        }


        public static string GetHttpOrHttps()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request.Url.ToString().Contains("https"))
                {
                    return "https://";
                }
            }
            return "http://";
        }

        public static string GetHTTPProtocal(HttpRequestBase httpRequestBase)
        {
            if (string.Equals(httpRequestBase.Headers["X-Forwarded-Proto"], "https", StringComparison.InvariantCultureIgnoreCase))
            {
                return "https";
            }
            return "http";
        }

        public static bool OpenAllApplicationInSameTab
        {
            get
            {
                if (HttpContext.Current.Session["OpenAllApplicationInSameTab"] == null)
                {
                    var openAllApplicationInSameTab = true;

                    var currentUser = HttpContext.Current.GetCurrentUser();

                    if (currentUser.IsStudent || currentUser.IsParent)
                    {
                        DistrictDecodeService districtDecodeService = DependencyResolver.Current.GetService<DistrictDecodeService>();
                        var districtDecode =
                            districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(currentUser.DistrictId.GetValueOrDefault(), Util.DistrictDecode_OpenAllApplicationInSameTab);
                        if (districtDecode != null)
                        {
                            bool.TryParse(districtDecode.Value, out openAllApplicationInSameTab);
                        }
                    }

                    HttpContext.Current.Session["OpenAllApplicationInSameTab"] = openAllApplicationInSameTab;
                }

                return (bool)HttpContext.Current.Session["OpenAllApplicationInSameTab"];
            }
        }

        public static bool IsUseNewDesign(int districtid)
        {
            string key = $"{Util.DistrictDecode_Portal_UseNewDesign}_{districtid}";
            if (HttpContext.Current.Session[key] == null)
            {
                var isUseNewDesign = false;
                DistrictDecodeService districtDecodeService =
                DependencyResolver.Current.GetService<DistrictDecodeService>();

                var districtDecode =
                districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(
                districtid, Util.DistrictDecode_Portal_UseNewDesign);

                if (districtDecode != null)
                {
                    var value = districtDecode.Value;
                    if (!string.IsNullOrEmpty(value) && value.ToLower() == "true")
                    {
                        isUseNewDesign = true;
                    }
                }
                HttpContext.Current.Session[key] = isUseNewDesign;
            }

            return (bool)HttpContext.Current.Session[key];
        }

        public static string GetNameByCode(string code, bool isSub)
        {            
            var name = string.Empty;
            var currentUser = HttpContext.Current.GetCurrentUser();
            var objMenu = GetMenuForDistrict(currentUser);

            if (objMenu != null)
            {
                if (isSub && objMenu.SubMenuItems != null && objMenu.SubMenuItems.TryGetValue(code, out var area))
                {
                    name = area.Label;
                }
                else if (!isSub && objMenu.MainMenuItems != null && objMenu.MainMenuItems.TryGetValue(code, out area))
                {
                    name = area.Label;
                }
            }

            return name;
        }

        public static string FormatPageTitle(string areaCode, string title, bool isSub = false)
        {
            try
            {
                var category = GetNameByCode(areaCode, isSub);
                return $"[{category}] - {title}";
            }
            catch (Exception)
            {
                return title;
            }
        }

        public static string CapitalizeText(this string text)
        {
            var result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
            if (text.ToLower().Contains("einstruction import") || text.ToLower().Contains("import einstruction"))
            {
                return result.Replace("Einstruction", "eInstruction");
            };
            return result;
        }
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
