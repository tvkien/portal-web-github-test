using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.SessionState;
using FluentValidation;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Web.Mvc;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.DataTables;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using LinkIt.BubbleSheetPortal.Models.SSO;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Services.CommonServices;
using LinkIt.BubbleSheetPortal.Models.DTOs.SSO;
using LinkIt.BubbleSheetPortal.Common.SSO;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Web.Models.AccountDto;
using System.Net.Security;
using IdentityModel;
using IdentityModel.Client;
using LinkIt.BubbleSheetPortal.Web.Helpers.LTI;
using LinkIt.BubbleSheetPortal.Models.DTOs.LTI;
using System.Security.Cryptography;
using LinkIt.BubbleSheetPortal.Common.CustomException;
using LinkIt.BubbleSheetPortal.Models.DTOs.SSO.Clever;
using LinkIt.BubbleSheetPortal.VaultProvider;
using LinkIt.BubbleSheetPortal.Web.CustomException;
using LinkIt.BubbleSheetPortal.Web.Helpers.SSO;
using LinkIt.BubbleSheetPortal.Web.Constant;
using LinkIt.BubbleSheetPortal.Services.Reporting;
using static LinkIt.BubbleSheetPortal.Common.Constanst;
using LinkIt.BubbleSheetPortal.Web.Helpers.SAML;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [VersionFilter]
    public class AccountController : BaseController
    {
        private readonly UserService userService;
        private readonly IFormsAuthenticationService formsAuthenticationService;
        private readonly StateService stateService;
        private readonly DistrictService districtService;
        private readonly PasswordResetQuestionService passwordResetQuestionService;
        private readonly IValidator<AccountInformationViewModel> accountInformationViewModelValidator;
        private readonly EmailService emailService;
        private readonly DSPDistrictService dspDistrictService;
        private readonly DistrictDecodeService districtDecodeService;
        private readonly ImpersonateLogService impersonateLogService;
        private readonly ConfigurationService _configurationService;
        private readonly UserLogonService _userLogonService;
        private readonly UserMetaService _userMetaService;
        private readonly ResetPasswordLogService _resetPasswordLogService;
        private readonly IShortLinkService _shortLinkService;
        private readonly StudentService _studentService;
        private readonly LTISingleSignOnService _ltiSingleSignOnService;
        private readonly IVaultProvider _vaultProvider;
        private readonly IReportingHttpClient _reportingHttpClient;

        public AccountController(UserService userService,
            IFormsAuthenticationService formsAuthenticationService,
            StateService stateService,
            DistrictService districtService,
            PasswordResetQuestionService passwordResetQuestionService,
            IValidator<AccountInformationViewModel> accountInformationViewModelValidator,
            EmailService emailService,
            DistrictDecodeService districtDecodeService,
            DSPDistrictService dspDistrictService,
            ConfigurationService configurationService,
            ImpersonateLogService impersonateLogService,
            UserLogonService userLogonService,
            UserMetaService userMetaService,
            ResetPasswordLogService resetPasswordService,
             IShortLinkService shortLinkService,
             StudentService studentService,
             LTISingleSignOnService ltiSingleSignOnService,
             IVaultProvider vaultProvider,
             IReportingHttpClient reportingHttpClient)
        {
            this.userService = userService;
            this.formsAuthenticationService = formsAuthenticationService;
            this.stateService = stateService;
            this.districtService = districtService;
            this.passwordResetQuestionService = passwordResetQuestionService;
            this.accountInformationViewModelValidator = accountInformationViewModelValidator;
            this.emailService = emailService;
            this.districtDecodeService = districtDecodeService;
            this.dspDistrictService = dspDistrictService;
            this._configurationService = configurationService;
            this.impersonateLogService = impersonateLogService;
            _userLogonService = userLogonService;
            _userMetaService = userMetaService;
            _resetPasswordLogService = resetPasswordService;
            _shortLinkService = shortLinkService;
            _studentService = studentService;
            _ltiSingleSignOnService = ltiSingleSignOnService;
            _vaultProvider = vaultProvider;
            _reportingHttpClient = reportingHttpClient;
        }

        private const string AutoLogOnLabel = "AutoLogOn";
        private const string Cookie_SSOLogin = "SSOLogin";
        private const string TempPasswordExpired = "TempPasswordExpired";

        private bool IsUseQuestion(User user)
        {
            var ssoInfo = userService.GetConfigLogonBySSO(user.DistrictId ?? 0);
            if (ssoInfo != null) return false;

            var val = districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(user.DistrictId ?? 0, Constanst.DistrictDecode_NotUseSecureQuestion).SingleOrDefault();
            return val == null || !bool.Parse(val.Value);
        }

        public ActionResult LogOn(bool? hasTemporaryPassword)
        {
            Response.Headers.Add("X-Frame-Options", "DENY");
            Response.Cookies.Remove(Cookie_SSOLogin);

            var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];

            var configHideLogOnPage = ConfigurationManager.AppSettings["InsightHideLogonPage"];
            var hideLogonPage = false;
            if (!string.IsNullOrEmpty(configHideLogOnPage))
            {
                bool.TryParse(configHideLogOnPage, out hideLogonPage);
            }

            if (hideLogonPage)
            {
                var loginRedirectHost = ConfigurationManager.AppSettings["InsightLoginRedirectHost"];
                var loginRedirectUrl = ConfigurationManager.AppSettings["InsightLoginRedirectUrl"];

                if (!string.IsNullOrEmpty(loginRedirectHost) && !string.IsNullOrEmpty(loginRedirectUrl)
                        && Request.Headers["HOST"].ToLower().Contains(loginRedirectHost.ToLower()))
                    return Redirect(loginRedirectUrl);
            }

            var districtId = districtService.GetLiCodeBySubDomain(subDomain);

            var logOnHeaderHtmlContent = string.Empty;
            if (districtId > 0)
            {
                var useManualLogin = Request.QueryString["useManualLogin"];
                if (string.IsNullOrEmpty(useManualLogin) || !useManualLogin.ToLower().Trim().Equals("true"))
                {
                    var logOnViaSsoConfig = districtDecodeService.GetLogOnViaSsoConfig(districtId);
                    if (logOnViaSsoConfig.LogOnViaSSO && !string.IsNullOrEmpty(logOnViaSsoConfig.SSOLogonURL))
                    {
                        return Redirect(logOnViaSsoConfig.SSOLogonURL);
                    }
                }

                using (var client = new HttpClient())
                {
                    var url = string.Format("{0}{1}/{2}", LinkitConfigurationManager.GetS3Settings().S3CSSKey, districtId, "LogOn.html");
                    try
                    {
                        HttpResponseMessage response = client.GetAsync(url).Result;
                        if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotModified)
                        {
                            logOnHeaderHtmlContent = response.Content.ReadAsStringAsync().Result;
                        }
                    }
                    catch (Exception)
                    {
                        // do nothing
                    }
                }
            }

            var obj = new LoginAccountViewModel
            {
                District = districtId,
                HasTemporaryPassword = hasTemporaryPassword.GetValueOrDefault(),
                LogOnHeaderHtmlContent = logOnHeaderHtmlContent,
                ShowAnnouncement = _configurationService.GetShowAnnouncement(),
                AnnouncementText = _configurationService.GetAnnouncementText(),
                EnableLoginByGoogle = districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.SHOW_GOOGLE_LOGIN_BUTTON),
                EnableLoginByMicrosoft = districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.SHOW_MICROSOFT_LOGIN_BUTTON),
                EnableLoginByClever = districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.SHOW_CLEVER_LOGIN_BUTTON),
                IsPortalNewSkinNotLogin = districtDecodeService.DistrictSupporPortalNewSkin(districtId),
                HideLoginCredentials = districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.HIDE_LOGIN_CREDENTIALS),
                EnableLoginByNYC = districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.SHOW_NYC_LOGIN_BUTTON),
                EnableLoginByClassLink = districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.SHOW_CLASSLINK_LOGIN_BUTTON)
            };

            return View(obj);
        }

        [HttpPost]
        public ActionResult LogOn(LoginAccountViewModel model)
        {
            return ProcessLogon(model);
        }

        [HttpGet]
        public ActionResult SetAccountInformation(int? id)
        {
            var beforeLoginSession = HttpContext.Session["BeforeLoginSession"] as Tuple<LoginAccountViewModel, int, int>;
            User user;
            if (beforeLoginSession == null || beforeLoginSession.Item2 != id || !id.HasValue)
            {
                if (Request.Url.Host.ToLower().Split('.')[0] == Constanst.StudentLoginFlag)
                    return RedirectToAction("Index", "StudentLogin");
                return RedirectToAction("LogOn");
            }
            user = userService.GetUserById(id.GetValueOrDefault());

            bool isUseQuestion = IsUseQuestion(user);
            var model = new AccountInformationViewModel
            {
                HasEmailAddress = !user.EmailAddress.Equals(string.Empty),
                HasSecurityQuestion = !isUseQuestion || !string.IsNullOrEmpty(user.PasswordQuestion),
                HasTemporaryPassword = user.HasTemporaryPassword || beforeLoginSession.Item1.HasTemporaryPassword,
                UserID = id.GetValueOrDefault(),
                Questions =
                    new List<SelectListItem>(
                        passwordResetQuestionService.GetPasswordResetQuestions(beforeLoginSession.Item3)
                            .Select(x => new SelectListItem { Text = x.Question, Value = x.Question })),

                ShowDisclaimerContent = GetShowDisclaimerContentConfigurationValue(user.DistrictId ?? 0)
                && !user.TermOfUseAccepted.HasValue,
                DisclaimerContent =
                    GetDistrictDeocdeOrConfigurationValue(user.DistrictId ?? 0,
                        DistrictDecodeLabelConstant.FistLogin_DisclaimerContent),
                DisclaimerCheckboxLabel =
                    GetDistrictDeocdeOrConfigurationValue(user.DistrictId ?? 0,
                        DistrictDecodeLabelConstant.FistLogin_DisclaimerCheckboxLabel),

                UserName = user.UserName
            };
            if (model.HasEmailAddress && model.HasSecurityQuestion && !model.HasTemporaryPassword && !model.ShowDisclaimerContent)
            {
                var setAccountInformationResult = SetAccountInformationReturnObject(new AccountInformationViewModel()
                {
                    HasEmailAddress = model.HasEmailAddress,
                    HasSecurityQuestion = model.HasSecurityQuestion,
                    HasTemporaryPassword = model.HasTemporaryPassword,
                    Email = null,
                    Question = null,
                    Answer = null,
                    UserID = model.UserID,
                    Password = null,
                    ConfirmPassword = null,
                    TermOfUse = false,
                    ChangePasswordToken = ""
                });
                if (setAccountInformationResult.Success)
                {
                    return Redirect(setAccountInformationResult.RedirectUrl);
                }
            }
            return View(model);
        }

        private bool GetShowDisclaimerContentConfigurationValue(int districtId)
        {
            var showDisclaimerContent = false;
            var districtDecode = districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId,
                DistrictDecodeLabelConstant.FistLogin_ShowDisclaimerContent).FirstOrDefault();
            if (districtDecode != null)
            {
                showDisclaimerContent = Convert.ToBoolean(districtDecode.Value);
            }
            else
            {
                var configuration =
                    _configurationService.GetConfigurationByKey(
                        DistrictDecodeLabelConstant.FistLogin_ShowDisclaimerContent);
                if (configuration != null)
                {
                    showDisclaimerContent = Convert.ToBoolean(configuration.Value);
                }
            }

            return showDisclaimerContent;
        }

        private string GetDistrictDeocdeOrConfigurationValue(int districtId, string label)
        {
            var configurationValue = "";

            var districtDecode =
                districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, label).FirstOrDefault();
            if (districtDecode != null)
            {
                configurationValue = districtDecode.Value;
            }
            else
            {
                var configuration =
                    _configurationService.GetConfigurationByKey(label);
                if (configuration != null)
                {
                    configurationValue = configuration.Value;
                }
            }

            return configurationValue;
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult SetAccountInformation(AccountInformationViewModel model)
        {
            return Json(SetAccountInformationReturnObject(model), JsonRequestBehavior.AllowGet);
        }

        private SetUserInformationResultDto SetAccountInformationReturnObject(AccountInformationViewModel model)
        {
            try
            {
                model.SetValidator(accountInformationViewModelValidator);
                if (!model.IsValid)
                {
                    return new SetUserInformationResultDto()
                    {
                        Success = false,
                        ErrorList = model.ValidationErrors
                    }; ;
                }

                // Change password
                var userInfo = ReadForgotPasswordToken(model.ChangePasswordToken);
                var subDomain = "portal";

                if (userInfo != null)
                {
                    userService.SetPassword(userInfo, model.Password);
                    var url = string.Empty;

                    if (userInfo.IsStudent || userInfo.IsParent)
                    {
                        if (userInfo.DistrictId.HasValue)
                        {
                            var district = districtService.GetDistrictById(userInfo.DistrictId.Value);
                            subDomain = district.IsNull() ? subDomain : district.LICode.ToLower();
                        }
                        var routePage = userInfo.IsParent ? "Parent" : "Student";
                        url = string.Format("{0}://{1}.{2}/{3}", HelperExtensions.GetHTTPProtocal(Request), subDomain, ConfigurationManager.AppSettings["LinkItUrl"], routePage);
                    }
                    else
                    {
                        url = Request.Url.GetLeftPart(UriPartial.Authority);
                    }

                    return new SetUserInformationResultDto()
                    {
                        Success = true,
                        RedirectUrl = url
                    };
                }

                var user = userService.GetUserById(model.UserID);

                // check security
                var beforeLoginSession = HttpContext.Session["BeforeLoginSession"] as Tuple<LoginAccountViewModel, int, int>;
                if (beforeLoginSession == null)
                {
                    return new SetUserInformationResultDto()
                    {
                        Success = false,
                        ErrorMessage = "Data is not saved. Please try again."
                    };
                }
                bool isUseQuestion = IsUseQuestion(user);
                var validOldVals = new
                {
                    HasEmailAddress = !user.EmailAddress.Equals(string.Empty),
                    HasSecurityQuestion = !isUseQuestion || !string.IsNullOrEmpty(user.PasswordQuestion),
                    HasTemporaryPassword = user.HasTemporaryPassword || beforeLoginSession.Item1.HasTemporaryPassword,
                };
                if (!validOldVals.HasEmailAddress && string.IsNullOrEmpty(model.Email))
                    return new SetUserInformationResultDto()
                    {
                        Success = false,
                        ErrorList = new FluentValidation.Results.ValidationFailure[1] { new FluentValidation.Results.ValidationFailure("security", "Invalid request!!!") }
                    };
                if (!validOldVals.HasSecurityQuestion && string.IsNullOrEmpty(model.Answer))
                    return new SetUserInformationResultDto()
                    {
                        Success = false,
                        ErrorList = new FluentValidation.Results.ValidationFailure[1] { new FluentValidation.Results.ValidationFailure("security", "Invalid request!!!") }
                    };
                if (validOldVals.HasTemporaryPassword && string.IsNullOrEmpty(model.Password))
                    return new SetUserInformationResultDto()
                    {
                        Success = false,
                        ErrorList = new FluentValidation.Results.ValidationFailure[1] { new FluentValidation.Results.ValidationFailure("security", "Invalid request!!!") }
                    };

                if (user.RoleId.Equals(5) || user.RoleId.Equals(15))
                    user.IsAdmin = true;
                
                subDomain = user.DistrictId.HasValue
                    ? districtService.GetDistrictById(user.DistrictId.Value)?.LICode ?? "portal"
                    : "portal";

                SetUserInformation(model, user, isUseQuestion, model.TermOfUse);
                userService.SaveUser(user);

                var studentModel = HttpContext.Session["StudentBeforeLoginSession"] as LoginAccountViewModel;
                var isFromStudentLogin = studentModel != null;

                if (!string.IsNullOrEmpty(model.Password))
                    beforeLoginSession.Item1.Password = model.Password;
                if (beforeLoginSession.Item1.IsNotNull() && isFromStudentLogin && beforeLoginSession.Item1.RoleId == 0)
                    beforeLoginSession.Item1.RoleId = beforeLoginSession.Item3;
                AuthenticateUser(beforeLoginSession.Item1, user, true, isFromStudentLogin);

                // Redirect to home page when update student information (the same student login page)
                if (isFromStudentLogin)
                {
                    model.RedirectUrl = "/";
                }
                else
                {
                    model.RedirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), subDomain, user);
                }

                HttpContext.Session.Remove("BeforeLoginSession");
                HttpContext.Session.Remove("StudentBeforeLoginSession");

                return new SetUserInformationResultDto()
                {
                    Success = true,
                    RedirectUrl = model.RedirectUrl
                };
            }
            catch (Exception)
            {
                return new SetUserInformationResultDto()
                {
                    Success = false,
                    ErrorMessage = "An unexpected error occurred. Please refresh the page and try again."
                };
            }
        }

        private void SetUserInformation(AccountInformationViewModel model, User user, bool isUseQuestion, bool termOfUseAccepted)
        {
            if (user.EmailAddress.Equals(string.Empty) || user.EmailAddress.IsNull())
            {
                user.EmailAddress = model.Email;
            }
            if (isUseQuestion && (user.PasswordQuestion.IsNull() || user.PasswordQuestion.Equals(string.Empty)))
            {
                user.PasswordQuestion = model.Question;
                user.PasswordAnswer = Crypto.HashPassword(model.Answer);
            }
            if (user.HasTemporaryPassword || model.Password.IsNotNull())
            {
                userService.SetPassword(user, model.Password);
                user.HasTemporaryPassword = false;
            }

            if (GetShowDisclaimerContentConfigurationValue(user.DistrictId ?? 0) && termOfUseAccepted && !user.TermOfUseAccepted.HasValue)
            {
                user.TermOfUseAccepted = DateTime.UtcNow;
            }
        }

        private void AuthenticateUser(LoginAccountViewModel model, User currentUser, bool isForceUpdateLastLogin = false, bool isStudenlogin = false, bool isSSO = false)
        {
            if (!string.IsNullOrEmpty(model.Password))
                model.Password = model.Password.Trim();

            if (currentUser == null || (!userService.IsValidUser(currentUser, model.Password) && !isSSO)) return;
            if (!currentUser.UserStatusId.Equals((int)UserStatus.Active))
            {
                model.Message = "Your username and password do not match our records.";
                return;
            }

            // Prevented user is student
            if (!isStudenlogin && currentUser.IsStudent && !isSSO)
            {
                return;
            }

            if (currentUser.IsParent && !isSSO)
            {
                return;
            }

            User user = null;
            var district = districtService.GetDistrictById(model.District);
            var subdomain = HelperExtensions.GetSubdomain().ToLower();
            var isDiffDomain = subdomain != district.LICode.ToLower();

            var roleIds = model.RoleId > 0 ?
                new List<int> { model.RoleId } :
                new List<int> {
                    (int)RoleEnum.Publisher,
                    (int)RoleEnum.NetworkAdmin,
                    (int)RoleEnum.DistrictAdmin,
                    (int)RoleEnum.SchoolAdmin,
                    (int)RoleEnum.Teacher,
                };

            if (isSSO && isDiffDomain)
            {
                user = userService.GetUserByUsernameAndDistrict(model.UserName, model.District, roleIds);
            }
            else
            {
                if (currentUser.IsAdmin)
                {
                    user = currentUser;
                }
                else
                {
                    user = userService.GetUserByUsernameAndDistrict(model.UserName, model.District, roleIds);
                }
            }

            user.Password = model.Password;
            model.HasEmailAddress = !user.EmailAddress.Equals(string.Empty);
            model.HasSecurityQuestion = !string.IsNullOrEmpty(user.PasswordQuestion);
            var passwordResetYears = 0;
            passwordResetYears = int.TryParse(ConfigurationManager.AppSettings["PasswordResetTime"], out passwordResetYears) ? passwordResetYears : 5;
            model.HasTemporaryPassword = !isSSO && (user.HasTemporaryPassword || !Regex.IsMatch(model.Password, ConfigurationManager.AppSettings["PasswordRegex"])
                                                                   || user.LastLoginDate < DateTime.MinValue.AddYears(1990)
                                                                   || user.LastLoginDate < DateTime.UtcNow.AddYears(passwordResetYears * -1));

            bool isUseQuestion = IsUseQuestion(user);
            model.HasSecurityQuestion = isSSO || !isUseQuestion || model.HasSecurityQuestion;

            var hasDistrict = true;
            if (currentUser.IsNetworkAdmin)
            {
                var dspDistrics = dspDistrictService.GetDistrictsByUserId(user.Id);
                if (dspDistrics.IsNotNull() && dspDistrics.Any())
                {
                    SessionManager.ListDistrictId = dspDistrics;

                    //Set default selected district member
                    //If district of networkadmin is not one of his/her district members, assign the first
                    if (!dspDistrics.Contains(currentUser.DistrictId.Value))
                    {
                        user.DistrictId = dspDistrics[0];
                        currentUser.DistrictId = dspDistrics[0];
                    }
                }
                else
                {
                    hasDistrict = false;
                }
            }
            //generate a SessionCookieGUID for ImpersonateLog
            user.SessionCookieGUID = Guid.NewGuid().ToString();
            user.GUIDSession = Guid.NewGuid().ToString();

            if (!user.IsNull())
                LoadUserMetaData(user);

            if (isSSO)
            {
                if (isDiffDomain)
                {
                    user.OriginalID = user.Id;
                    user.ImpersonatedSubdomain = district != null ? district.LICode : "demo";
                    user.ImpersonateLogActivity = ImpersonateLog.ActionTypeEnum.Impersonate;
                }

                formsAuthenticationService.SignIn(user, model.IsKeepLoggedIn, true);
            }
            else
            {
                formsAuthenticationService.SignIn(user, model.IsKeepLoggedIn);
            }

            if (hasDistrict)
                model.IsAuthenticated = true;
            if (model.IsAuthenticated)
            {
                var userLogon = new UserLogon()
                {
                    UserID = user.Id,
                    GUIDSession = user.GUIDSession
                };
                _userLogonService.Save(userLogon);

                // write to DefaultDateFormat cookie
                Util.LoadDateFormatToCookies(user.DistrictId.GetValueOrDefault(), districtDecodeService);
            }

            if (!model.IsFirstStudentLogonSSO && (isForceUpdateLastLogin || (model.HasEmailAddress && model.HasSecurityQuestion && !model.HasTemporaryPassword)))
            {
                userService.UpdateLastLogin(user.Id);
            }

            model.UserID = user.Id;
        }

        private void LoadUserMetaData(User currentUser)
        {
            var userMeta = _userMetaService.GetByUserId(currentUser.Id, UserMetaLabelConst.NOTIFICATION);
            if (userMeta == null)
            {
                userMeta = new UserMeta
                {
                    UserId = currentUser.Id,
                    MetaLabel = UserMetaLabelConst.NOTIFICATION,
                    UserMetaValue = new UserMetaValue
                    {
                        LatestNotificationClicked = DateTime.MinValue
                    }
                };
                _userMetaService.Save(userMeta);
            }
            currentUser.UserMetaValue = userMeta.UserMetaValue;
        }

        private User GetAdminUser(string username)
        {
            return userService.GetAdminByUsername(username);
        }

        private User GetUser(LoginAccountViewModel model)
        {
            var roleIds = model.RoleId > 0 ?
                new List<int> { model.RoleId } :
                new List<int> {
                    (int)RoleEnum.Publisher,
                    (int)RoleEnum.NetworkAdmin,
                    (int)RoleEnum.DistrictAdmin,
                    (int)RoleEnum.SchoolAdmin,
                    (int)RoleEnum.Teacher,
                };

            return userService.GetUserByUsernameAndDistrict(model.UserName, model.District, roleIds);
        }

        public ActionResult LogOff(string returnUrl)
        {
            ClearCache();

            // check support SSO
            var isLogOnSSO = Request.Cookies[Cookie_SSOLogin];
            var redirectURL = string.Empty;

            if (isLogOnSSO != null)
            {
                var ssoInfo = userService.GetConfigLogonBySSO(HelperExtensions.GetDistrictIdBySubdomain());

                if (ssoInfo != null && ssoInfo.SSOInformation != null)
                {
                    if (!string.IsNullOrEmpty(ssoInfo.SSOInformation.UrlLogoutPage))
                    {
                        redirectURL = ssoInfo.SSOInformation.UrlLogoutPage;
                    }
                    else
                    {
                        redirectURL = HttpContext.Request.Url.ToString().Replace(Server.UrlDecode(HttpContext.Request.RawUrl), "") + "/account/logonsso";
                    }
                }
            }

            RemoveUserLogon();

            var isStudent = CurrentUser.IsStudent;
            var district = districtService.GetDistrictById(CurrentUser.DistrictId.GetValueOrDefault());

            formsAuthenticationService.SignOut();

            HttpCookie passThroughCookie = Request.Cookies["UserPassThrough"];
            if (passThroughCookie != null)
            {
                string userId = string.Empty;
                userId = passThroughCookie["PassThroughUserID"];

                if (!string.IsNullOrEmpty(userId))
                {
                    redirectURL = passThroughCookie["PassThroughReturnURL"];
                }
            }

            if (Session != null && Session["MenuItem"] != null)
                Session["MenuItem"] = null;
            if (Session != null && Session["SubDomainDistrictID"] != null)
                Session["SubDomainDistrictID"] = null;

            if (!string.IsNullOrEmpty(redirectURL))
            {
                return Redirect(redirectURL);
            }
            else if (isStudent || CurrentUser.IsParent)
            {
                if (district != null && district.LICode.ToUpper().Equals("CHYTEN"))
                {
                    return RedirectToAction("Index", "StudentLogin");
                }

                var routePage = CurrentUser.IsParent ? "Parent" : "Student";
                var url = string.Format("{0}/{1}", Request.Url.GetLeftPart(UriPartial.Authority), routePage);
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    url = returnUrl;
                }
                return Redirect(url);
            }

            return RedirectToAction("LogOn", "Account");
        }

        private void RemoveUserLogon()
        {
            try
            {
                if (CurrentUser == null) return;
                var userLogon = _userLogonService.GetById(CurrentUser.Id);
                if (userLogon != null)
                {
                    _userLogonService.Delete(userLogon);
                }
            }
            catch
            {
                //cannot cast GenericUser to User
                //it means user has been logged out
                //do nothing here
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ResetPassword(LoginAccountViewModel model)
        {
            ViewBag.DistrictLable = LabelHelper.DistrictLabel;
            // Log reset password
            LogResetPassword(model.District, model.UserName);

            var expireMinutes = GetTempPasswordExpired();
            var message = string.Format(TextConstants.PASSWORD_RESET_MESSAGE, expireMinutes);

            var adminUser = userService.DoesNotRequireDistrictToBeSelected(model.UserName);
            if (adminUser.IsNotNull())
            {
                SendTokenResetPassword(adminUser, expireMinutes);
                return Json(new { type = "success", message }, JsonRequestBehavior.AllowGet);
            }

            if (model.District > 0)
            {
                var user = userService.GetUserByUsernameAndDistrict(model.UserName, model.District);
                if (user?.UserStatusId == (int)UserStatus.Active)
                {
                    SendTokenResetPassword(user, expireMinutes);
                    return Json(new { type = "success", message }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { type = "success", message }, JsonRequestBehavior.AllowGet);
        }

        private ActionResult ResetUsersPassword(LoginAccountViewModel model, User user)
        {
            var accountModel = new ResetPasswordViewModel { UserName = model.UserName };
            if (string.IsNullOrEmpty(user.PasswordQuestion) && string.IsNullOrEmpty(user.EmailAddress))
            {
                return Json(new
                {
                    type = "error"
                }, JsonRequestBehavior.AllowGet);
            }

            bool isUseQuestion = IsUseQuestion(user);

            accountModel.PasswordQuestion = user.PasswordQuestion;
            accountModel.UserID = user.Id;
            accountModel.HasEmailAddress = !user.EmailAddress.Equals(string.Empty);
            accountModel.HasSecurityQuestion = isUseQuestion && !string.IsNullOrEmpty(user.PasswordQuestion);

            return View("ResetPassword", accountModel);
        }

        private void LogResetPassword(int districtID, string userName)
        {
            try
            {
                var ipAddress = GetClientIpAddress(ControllerContext.HttpContext.Request);
                userName = string.IsNullOrEmpty(userName) ? "" : userName;
                _resetPasswordLogService.Save(new ResetPasswordLog()
                {
                    DistrictCode = districtID,
                    IPAddress = ipAddress,
                    RequestDate = DateTime.Now,
                    UserName = userName
                });
            }
            catch
            {
                // This function is used for reset password logging
            }
        }

        public string GetClientIpAddress(HttpRequestBase request)
        {
            string szRemoteAddr = request.UserHostAddress;
            string szXForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];
            string szIP = "";

            if (szXForwardedFor == null)
            {
                szIP = szRemoteAddr;
            }
            else
            {
                szIP = szXForwardedFor;
                if (szIP.IndexOf(",") > -1 && szIP[0] != ',')
                {
                    string[] arIPs = szIP.Split(',');

                    foreach (string item in arIPs)
                    {
                        if (!IsPrivateIpAddress(item))
                        {
                            return item;
                        }
                    }
                }
            }
            return szIP;
        }

        private static bool IsPrivateIpAddress(string ipAddress)
        {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are:
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            var ip = IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock) return true; // Return to prevent further processing

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) return true; // Return to prevent further processing

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) return true; // Return to prevent further processing

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }

        [HttpPost]
        public ActionResult SubmitPasswordReset(ResetPasswordViewModel model)
        {
            if (IsValidPasswordRequest(model))
            {
                model.NewPassword = model.NewPassword.Trim();
                model.ConfirmNewPassword = model.ConfirmNewPassword.Trim();
                var user = GetByUserName(model.UserName);
                if (user.IsNotNull() && userService.IsCorrectPasswordAnswer(user, model.PasswordAnswer))
                {
                    if (!Regex.IsMatch(model.NewPassword, ConfigurationManager.AppSettings["PasswordRegex"]))
                    {
                        model.SubmittedCorrectPasswordAnswer = true;
                        ModelState.AddModelError("invalidPassword", ConfigurationManager.AppSettings["PasswordRequirements"]);
                        return View("ResetPassword", model);
                    }
                    userService.ResetUsersPassword(user, model.NewPassword);
                    model.PasswordHasBeenReset = true;
                    return View("ResetPassword", model);
                }
                model.SubmittedCorrectPasswordAnswer = true;
                ModelState.AddModelError("invalidUser", "Invalid User.  Please try again.");
                return View("ResetPassword", model);
            }
            model.SubmittedCorrectPasswordAnswer = true;
            ModelState.AddModelError("passwords", "Passwords do not match.  Please try again.");
            return View("ResetPassword", model);
        }

        [HttpPost]
        public ActionResult SubmitPasswordAnswer(ResetPasswordViewModel model)
        {
            var user = GetByUserName(model.UserName);

            if (user.IsNull())
            {
                user = userService.GetAdminByUsername(model.UserName);

                if (user == null || user.RoleId != (int)Permissions.Publisher)
                {
                    ModelState.AddModelError("invalidUser", "Invalid User.  Please try again.");
                    return View("ResetPassword", model);
                }
            }

            bool isUseQuestion = IsUseQuestion(user);

            if (!isUseQuestion)
                return new EmptyResult();

            model.HasSecurityQuestion = true;
            model.HasEmailAddress = !user.EmailAddress.Equals(string.Empty);
            model.PasswordQuestion = user.PasswordQuestion;

            ViewBag.IsSubmitPasswordAnswer = true;

            if (SessionManager.ShowCaptcha)
            {
                model.ShowCaptcha = true;

                var recaptcharResponse = this.Request.Form["g-recaptcha-response"];//modify g-recaptcha-response to g_recaptcha_response
                if (string.IsNullOrEmpty(recaptcharResponse))
                {
                    ModelState.AddModelError("invalidCaptcha", "Can not verify captcha. Please reload page");
                    return View("ResetPassword", model);
                }
                //secret that was generated in key value pair
                string secret = ConfigurationManager.AppSettings["RECAPTCHA_SECRETKEY"];
                string googleCaptchaUrl = ConfigurationManager.AppSettings["RECAPTCHA_URL"];
                var verifyResponse = CaptchaHelper.Verify(secret, googleCaptchaUrl, recaptcharResponse);
                if (!verifyResponse.Success)
                {
                    ModelState.AddModelError("invalidCaptcha", "Can not verify captcha. Please reload page");
                    return View("ResetPassword", model);
                }
            }

            if (model.PasswordAnswer.IsNull())
            {
                model.PasswordQuestion = user.PasswordQuestion;
                ModelState.AddModelError("passwordAnswer", "Please input a password answer.");
                return View("ResetPassword", model);
            }
            if (user.IsNotNull())
            {
                if (userService.IsCorrectPasswordAnswer(user, model.PasswordAnswer))
                {
                    // reset counter
                    SessionManager.ResetPwCount = 0;
                    SessionManager.ShowCaptcha = false;
                    model.SubmittedCorrectPasswordAnswer = true;
                    return View("ResetPassword", model);
                }

                ProcessInvalidSecurityQuestion(model);
                return View("ResetPassword", model);
            }
            model.PasswordQuestion = user.PasswordQuestion;
            ModelState.AddModelError("invalidUser", "Invalid User.  Please try again.");
            return View("ResetPassword", model);
        }

        private void ProcessInvalidSecurityQuestion(ResetPasswordViewModel model)
        {
            SessionManager.ResetPwCount++;
            if (SessionManager.ResetPwCount >
                _configurationService.GetConfigurationByKeyWithDefaultValue(Constanst.ResetPasswordLimit,
                    Constanst.ResetPasswordLimitDefault))
            {
                //display recaptcha
                model.ShowCaptcha = true;
                SessionManager.ShowCaptcha = true;
            }

            ModelState.AddModelError("passwordAnswer", "Supplied answer is incorrect.  Please try again.");
        }

        private bool IsValidPasswordRequest(ResetPasswordViewModel model)
        {
            return !string.IsNullOrEmpty(model.NewPassword) && model.NewPassword.Equals(model.ConfirmNewPassword);
        }

        private User GetByUserName(string userName)
        {
            var districtID = HelperExtensions.GetDistrictIdBySubdomain();
            return userService.GetUserByUsernameAndDistrict(userName, districtID);
        }

        [AjaxAwareAuthorize]
        public ActionResult ChangePassword()
        {
            return PartialView();
        }

        [AjaxAwareAuthorize]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.SettingItem)]
        public ActionResult Settings()
        {
            ViewBag.IsUseQuestion = IsUseQuestion(CurrentUser);
            return View();
        }

        [HttpPost]
        [AjaxAwareAuthorize]
        [AjaxOnly]
        public ActionResult ChangePassword(ChangePassword model)
        {
            if (!string.IsNullOrEmpty(model.OldPassword))
                model.OldPassword = model.OldPassword.Trim();
            if (!string.IsNullOrEmpty(model.NewPassword))
                model.NewPassword = model.NewPassword.Trim();
            if (!string.IsNullOrEmpty(model.ConfirmPassword))
                model.ConfirmPassword = model.ConfirmPassword.Trim();

            if (model.OldPassword.Equals(model.NewPassword))
            {
                return Json(new { message = "New password cannot be the same as old password.", type = "error" }, JsonRequestBehavior.AllowGet);
            }
            if (!ModelState.IsValid)
            {
                return Json(new { message = "All fields are required. New password must match confirm password.", type = "error" }, JsonRequestBehavior.AllowGet);
            }
            if (!Regex.IsMatch(model.NewPassword, ConfigurationManager.AppSettings["PasswordRegex"]))
            {
                return Json(new { message = ConfigurationManager.AppSettings["PasswordRequirements"], type = "error" }, JsonRequestBehavior.AllowGet);
            }
            var currentUser = userService.GetUserById(CurrentUser.Id);
            if (currentUser.IsNull()) { return Json(new { message = "An error has occurred.  Please try again.", type = "error" }, JsonRequestBehavior.AllowGet); }
            if (userService.ChangePassword(currentUser, model.OldPassword, model.NewPassword))
            {
                return Json(new { message = "Your password has been successfully changed.", type = "success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "Provided password is incorrect.  Please try again.", type = "error" }, JsonRequestBehavior.AllowGet);
        }

        [AjaxAwareAuthorize]
        public ActionResult ChangeQuestionAnswer()
        {
            var model = new ChangeQuestionAnswer
            {
                Questions = new List<SelectListItem>(passwordResetQuestionService.GetPasswordResetQuestions(CurrentUser.RoleId).Select(x => new SelectListItem { Text = x.Question, Value = x.Question })),
                CurrentSecurityQuestion = userService.GetSecurityQuestionByUserId(CurrentUser.Id)
            };

            return PartialView(model);
        }

        [AjaxAwareAuthorize]
        [HttpPost]
        [AjaxOnly]
        public ActionResult ChangeQuestionAnswer(ChangeQuestionAnswer model)
        {
            bool isUseQuestion = IsUseQuestion(CurrentUser);
            if (!isUseQuestion)
            {
                return new EmptyResult();
            }

            if (!ModelState.IsValid)
            {
                return Json(new { message = "All fields are required.", type = "error" }, JsonRequestBehavior.AllowGet);
            }

            var currentUser = userService.GetUserById(CurrentUser.Id);
            if (currentUser.IsNull()) { return Json(new { message = "An error has occurred. Please try again.", type = "error" }, JsonRequestBehavior.AllowGet); }
            if (userService.ChangeQuestionAnswer(currentUser, model.Password, model.SelectedQuestion, model.Answer))
            {
                return Json(new { message = "Your Security Question and Answer have been successfully changed.", type = "success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "Provided password is incorrect.  Please try again.", type = "error" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendTemporaryPassword(string username)
        {
            try
            {
                var districtID = HelperExtensions.GetDistrictIdBySubdomain();
                var user = userService.GetUserByUsernameAndDistrict(username, districtID);
                if (user.IsNull())
                {
                    user = userService.GetAdminByUsername(username);

                    if (user == null || user.RoleId != (int)Permissions.Publisher)
                    {
                        return Json(new { message = "INVALID_USERNAME", type = "error" }, JsonRequestBehavior.AllowGet);
                    }
                }

                var expiredHours = _configurationService.GetConfigurationByKey(TempPasswordExpired);
                var minutes = 60;
                if (expiredHours != null)
                {
                    int.TryParse(expiredHours.Value, out minutes);
                }
                var token = Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ApproveChangePassword?token=" + Util.GenerateToken(user.UserName, user.DistrictId.GetValueOrDefault(), minutes);
                var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.LinkItUseEmailCredentialKey);
                emailService.SendTokenResetPassword(user, token, emailCredentialSetting);

                return Json(new { message = "", type = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { message = "", type = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ApproveChangePassword(string token)
        {
            var userInfo = ReadForgotPasswordToken(token);
            if (userInfo != null)
            {
                var model = new AccountInformationViewModel
                {
                    HasSecurityQuestion = true,
                    HasEmailAddress = true,
                    HasTemporaryPassword = true,
                    ChangePasswordToken = token
                };

                return View("SetAccountInformation", model);
            }
            else
            {
                return View("ExpiredTokenResetPassword");
            }
        }

        [HttpPost]
        public ActionResult GetStateByDistrictId(int? districtId)
        {
            var district = districtService.GetDistrictById(districtId.GetValueOrDefault());
            var stateId = district.IsNull() ? 0 : district.StateId;
            return Json(stateId);
        }

        [AjaxAwareAuthorize]
        public ActionResult ChangeName()
        {
            return PartialView();
        }

        [HttpPost]
        [AjaxAwareAuthorize]
        [AjaxOnly]
        public ActionResult ChangeName(ChangeName model)
        {
            var userId = CurrentUser.Id;
            var user = userService.GetUserById(userId);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            userService.SaveUser(user);

            var impersonates = impersonateLogService.GetImpersonateLogsBySessionCookieGUID(CurrentUser.SessionCookieGUID);

            string url = Url.Action("LogOn", "Account", new { ReturnUrl = "/Account/Settings" });
            if (CurrentUser.IsStudent && !impersonates.Any())
            {
                url = Url.Action("LogOn", "Student", new { ReturnUrl = "/Account/Settings" });
            }

            formsAuthenticationService.SignOut();

            return Json(new { RedirectUrl = url });
        }

        [HttpGet]
        public ActionResult AutoLogOn(int? userId)
        {
            if (!userId.HasValue) return RedirectToAction("LogOn");

            //check userId in white-list?
            var isUserInWhiteList = districtDecodeService.GetDistrictDecodesByLabel(AutoLogOnLabel).Any(x => x.Value == userId.ToString());

            if (isUserInWhiteList)
            {
                //get user from DB
                var user = userService.GetUserById(userId.Value);

                if (user != null && !user.IsPublisher)
                {
                    formsAuthenticationService.SignOut();
                    formsAuthenticationService.SignIn(user, false);

                    var districtSubDomain = "portal";
                    var district = districtService.GetDistrictById(user.DistrictId.Value);
                    districtSubDomain = district.IsNull() ? districtSubDomain : district.LICode.ToLower();

                    var redirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), districtSubDomain, user);
                    return Redirect(redirectUrl);
                }
            }
            return RedirectToAction("LogOn");
        }

        [AjaxAwareAuthorize]
        public ActionResult LoadNetworkAdminSelect()
        {
            return PartialView("NetworkAdminSelect");
        }

        [AjaxAwareAuthorize]
        public ActionResult LoadDSPDistrict(bool? firstTimeLogOn)
        {
            if (firstTimeLogOn.HasValue && firstTimeLogOn == true)
            {
                var data = dspDistrictService.GetDistrictMembers(CurrentUser.DistrictId.GetValueOrDefault())
                  .Select(o => new DistrictMemberViewModel()
                  {
                      State = o.StateName,
                      Name = o.Name,
                      LiCode = o.LICode,
                      DistrictId = o.Id,
                      StateId = o.StateId,
                  }).AsQueryable();
                var parser = new DataTableParser<DistrictMemberViewModel>();
                return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
            }

            if (CurrentUser.IsNetworkAdmin)
            {
                var user = userService.GetUserById(CurrentUser.Id);
                int organizationDistrictId = user.DistrictId ?? 0;

                var data = dspDistrictService.GetDistrictMembers(organizationDistrictId)
                .Select(o => new DistrictMemberViewModel()
                {
                    State = o.StateName,
                    Name = o.Name,
                    LiCode = o.LICode,
                    DistrictId = o.Id,
                    StateId = o.StateId,
                }).AsQueryable();
                data = data.Where(x => x.DistrictId != CurrentUser.DistrictId.Value);
                var parser = new DataTableParser<DistrictMemberViewModel>();
                return Json(parser.Parse(data), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = new List<DistrictMemberViewModel>();
                var parser = new DataTableParser<DistrictMemberViewModel>();
                return Json(parser.Parse(data.AsQueryable()), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AjaxAwareAuthorize]
        [AjaxOnly]
        public ActionResult NetworkAdminImpersontateDistrictAdmin(int districtId, int stateId, string LiCode)
        {
            //CurrentUser does not contains HashedPassword so it's necessary to get HashedPassword to create cookie LKARCookie ( avoid badcredentials)
            var user = userService.GetUserById(CurrentUser.Id);
            CurrentUser.HashedPassword = user.HashedPassword;
            //Remember the selected member districtId of NetworkAdmin
            CurrentUser.OriginalNetworkAdminDistrictId = districtId;

            CurrentUser.DistrictId = districtId; //assign selected district to current user
            CurrentUser.StateId = stateId;

            HttpContext.Session["MenuItem"] = null;

            //formsAuthenticationService.SignOut();//SignOut here will clear all session above, so all setting to Session variable should be done here
            System.Web.Security.FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session.Clear();

            System.Web.HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            HttpCookie httpCookie;
            foreach (string cookie in System.Web.HttpContext.Current.Request.Cookies.AllKeys)
                if (cookie != "cksession" && cookie != Constanst.LKARCookie)
                {
                    httpCookie = System.Web.HttpContext.Current.Response.Cookies[cookie];
                    if (httpCookie == null) httpCookie = new HttpCookie(cookie, "");
                    httpCookie.Expires = DateTime.Now.AddYears(-1);
                    System.Web.HttpContext.Current.Response.Cookies.Add(httpCookie);
                }

            formsAuthenticationService.SignIn(CurrentUser, false, true);
            userService.UpdateLastLogin(CurrentUser.Id);
            if (string.IsNullOrEmpty(LiCode))
            {
                LiCode = "portal";
            }
            var redirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), LiCode, CurrentUser, isImpersonate: true);
            return Json(new { Success = "Success", RedirectUrl = redirectUrl }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowNetworkAdminSelectLayout()
        {
            return PartialView("_NetworkAdminSelect");
        }

        public ActionResult ShowNetworkAdminSelectLayoutV2()
        {
            return PartialView("v2/_NetworkAdminSelect");
        }

        [NonAction]
        private void InitWarningMessage(LoginAccountViewModel model, string districtSubDomain)
        {
            if (model == null) return;

            var scheme = "http";
            if (string.Equals(Request.Headers["X-Forwarded-Proto"], "https", StringComparison.InvariantCultureIgnoreCase))
            {
                scheme = "https";
            }
            model.UrlRecomment = HelperExtensions.BuildStartUrlForAuthenticatedUser(scheme, districtSubDomain);

            model.IsShowWarningLogOnUser = false;
            if (Request.Url != null && (Request.Url.AbsoluteUri.StartsWith("http://portal.")
                                        || Request.Url.AbsoluteUri.StartsWith("https://portal.")
                                        || Request.Url.AbsoluteUri.StartsWith("http://admin.")
                                        || Request.Url.AbsoluteUri.StartsWith("https://admin."))
                )
            {
                var userLogon = _configurationService.GetConfigurationByKey("ShowWarningLogOnUser");
                if (userLogon != null && userLogon.Value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    model.IsShowWarningLogOnUser = true;
                }
                var messageWarninguser = _configurationService.GetConfigurationByKey("MessageWarningLogOnUser");
                if (messageWarninguser != null)
                {
                    model.MessageWarningLogOnUser = messageWarninguser.Value.Replace("[NewURL-LogOn]", model.UrlRecomment);
                }
            }
        }

        private int GetUserId(User user)
        {
            if (user != null)
            {
                return user.Id;
            }
            return -1;
        }

        public ActionResult LogOnSSO()
        {
            var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            var districtId = districtService.GetLiCodeBySubDomain(subDomain);

            // check support SSO
            var ssoInfo = userService.GetConfigLogonBySSO(districtId);

            if (ssoInfo == null) return RedirectToAction("LogOn");

            HttpCookie SSOLogin = new HttpCookie(Cookie_SSOLogin);
            SSOLogin.Value = "1";
            SSOLogin.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(SSOLogin);

            var client = new AuthenticationApiClient(
                new Uri(string.Format("https://{0}", ConfigurationManager.AppSettings["auth0:Domain"])));

            var request = this.Request;
            var redirectUri = new UriBuilder(HelperExtensions.GetHTTPProtocal(Request), request.Url.Host, this.Request.Url.IsDefaultPort ? -1 : request.Url.Port, "account/ssocallback");

            var authorizeUrlBuilder = client.BuildAuthorizationUrl()
                .WithClient(ssoInfo.SSOInformation.Auth0ClientId)
                .WithConnection(ssoInfo.SSOInformation.DefaultConnection)
                .WithRedirectUrl(redirectUri.ToString())
                .WithResponseType(AuthorizationResponseType.IdToken)
                .WithScope("openid profile")
                .WithNonce("abc");

            var returnURL = request.Params["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnURL))
            {
                var state = "ru=" + HttpUtility.UrlEncode(returnURL);
                authorizeUrlBuilder.WithState(state);
            }

            var url = authorizeUrlBuilder.Build().ToString();
            return new RedirectResult(url);
        }

        private string invalidUserMessage = "Invalid User.";

        public ActionResult SSOBridgeLogout(string props)
        {
            string[] arrs = props.Split(new[] { "!$!$" }, StringSplitOptions.None);
            return RedirectToAction("SSOChooseDistrict", new { username = arrs[0], districtId = arrs[1], returnUrl = string.Empty });
        }

        public ActionResult SSOCallback()
        {
            ViewBag.Auth0LogoutURL = "https://" + ConfigurationManager.AppSettings["auth0:Domain"] + "/v2/logout?returnTo=";
            return View();
        }

        public ActionResult Auth0LogoutURL(string token)
        {
            var adUser = ValidJWTToken(token);
            if (adUser != null)
            {
                // Valid username in database
                var userName = adUser.nickname;
                var subDomain = HelperExtensions.GetSubdomain().ToLower();
                var district = districtService.GetDistrictByCode(subDomain);

                // Check mapping user
                var linkitUser = userService.GetLinkitUserFromMapping(adUser.nickname, district.Id);
                if (!string.IsNullOrEmpty(linkitUser.UserName))
                {
                    userName = linkitUser.UserName;
                }

                var user = userService.GetUserByUsernameAndDistrict(userName, district.Id);
                var ssoInfo = userService.GetConfigLogonBySSO(district.Id);
                if (user != null)
                {
                    var result = ProcessLogon(new LoginAccountViewModel
                    {
                        UserName = userName,
                        Password = "Password@9999",
                        District = district.Id,
                        SSOInformationId = ssoInfo.SSOInformationID,
                        SSOType = SSOProvider.AUTH0,
                        RoleId = linkitUser.RoleId
                    }, true);

                    if (result.Data != null)
                    {
                        var data = result.Data as LoginAccountViewModel;
                        if (data.IsAuthenticated)
                        {
                            return Redirect(data.RedirectUrl);
                        }
                    }
                }
            }

            ViewBag.Message = invalidUserMessage;
            return View("SSOCallback");
        }

        private ADUserObject ValidJWTToken(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                var cer = ConfigurationManager.AppSettings["auth0:Certificate"];
                byte[] bytes = Encoding.UTF8.GetBytes(cer);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new X509SecurityKey(new X509Certificate2(bytes))
                };

                Microsoft.IdentityModel.Tokens.SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);

                return new ADUserObject()
                {
                    nickname = principal.Claims.FirstOrDefault(c => c.Type == "nickname").Value,
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private User ReadForgotPasswordToken(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Util.Secret)),
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);

                var userName = principal.Claims.FirstOrDefault(m => m.Type == "UserName").Value;
                var districtId = int.Parse(principal.Claims.FirstOrDefault(m => m.Type == "DistrictID").Value);

                var user = userService.GetUserByUsernameAndDistrict(userName, districtId);

                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private JsonResult ProcessLogon(LoginAccountViewModel model, bool isSSO = false)
        {
            if (model.IsDisableLoginForm)
                return Json(new EmptyResult());

            HttpContext.Session.Remove("BeforeLoginSession");
            HttpContext.Session.Remove("StudentBeforeLoginSession");

            if (SessionManager.ShowCaptcha && !isSSO)
            {
                var recaptcharResponse = this.Request.Form["g_recaptcha_response"];//modify g-recaptcha-response to g_recaptcha_response
                if (string.IsNullOrEmpty(recaptcharResponse))
                {
                    return Json(new { Message = "You must satisfy the CAPTCHA verification requirements to indicate that you are an authorized user.", Type = "error", ShowCaptcha = true });
                }
                //secret that was generated in key value pair
                string secret = ConfigurationManager.AppSettings["RECAPTCHA_SECRETKEY"];
                string googleCaptchaUrl = ConfigurationManager.AppSettings["RECAPTCHA_URL"];
                var verifyResponse = CaptchaHelper.Verify(secret, googleCaptchaUrl, recaptcharResponse);
                if (!verifyResponse.Success)
                {
                    return Json(new { Message = verifyResponse.ErrorCodes ?? new List<string> { "Can not verify captcha. Please reload page" }, Type = "error", ShowCaptcha = true });
                }
            }
            string districtSubDomain = "portal";
            var currentUser = userService.DoesNotRequireDistrictToBeSelected(model.UserName);
            if (isSSO)
            {
                currentUser = null;
            }

            if (currentUser != null)
            {
                districtSubDomain = "demo";
                currentUser.IsAdmin = true;
            }
            else if (model.District == 0)
            {
                //display reCAPTCHA
                model.ShowCaptcha = false;
                model.IsAuthenticated = false;
                userService.IncreaseUserLoginFailCount(GetUserId(currentUser));
                var loginCount = userService.GetUserLoginFailCount(GetUserId(currentUser));

                if (loginCount >
                    _configurationService.GetConfigurationByKeyWithDefaultValue(Constanst.LoginLimit, Constanst.LoginLimitDefault))
                {
                    //display recaptcha
                    model.ShowCaptcha = true;
                    SessionManager.ShowCaptcha = true;
                    return Json(new { Message = "Please select a " + LabelHelper.DistrictLabel + " first.", Type = "info", ShowCaptcha = true });
                }
                else
                {
                    return Json(new { Message = "Please select a " + LabelHelper.DistrictLabel + " first.", Type = "info" });
                }
            }
            else
            {
                currentUser = GetUser(model);
            }

            var district = districtService.GetDistrictById(model.District);
            districtSubDomain = district.IsNull() ? districtSubDomain : district.LICode.ToLower();

            if (!district.IsNull())
                HelperExtensions.LoginDistrict = district.Id;

            model.Type = "error";
            model.Message = "Your username and password do not match our records.";
            model.IsAuthenticated = false;

            AuthenticateUser(model, currentUser, false, false, isSSO);
            if (currentUser != null)
            {
                if (currentUser.IsNetworkAdmin)
                {
                    district = districtService.GetDistrictById(currentUser.DistrictId.Value);//NetworkAdmin default selected district member
                    districtSubDomain = district.IsNull() ? districtSubDomain : district.LICode.ToLower();
                }

                //check redirectUrl to prevent open redirection attack
                if (model.RedirectUrl == null || (!Url.IsLocalUrl(model.RedirectUrl) && !IsSubApp(model.RedirectUrl)))
                {
                    model.RedirectUrl = string.Empty;
                }

                if (string.IsNullOrWhiteSpace(model.RedirectUrl) || model.RedirectUrl == "/")
                {
                    var redirectUrl = _ltiSingleSignOnService.GetRedirectUrl(model.SSOInformationId, model.RoleId, model.SSOType);
                    model.RedirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), districtSubDomain, currentUser, false, redirectUrl);

                    if (isSSO && currentUser != null && (currentUser.RoleId == (int)RoleEnum.Student || currentUser.RoleId == (int)RoleEnum.Parent))
                    {
                        if(string.IsNullOrEmpty(redirectUrl))
                        {
                            model.RedirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), districtSubDomain, currentUser, false, "/Home");
                        }

                        var ssoRedirectUrl = _ltiSingleSignOnService.GetObjectRedirectUrl(model.SSOInformationId, model.RoleId, model.SSOType);
                        var objMenu = HelperExtensions.GetMenuForDistrict(currentUser);
                        if (ssoRedirectUrl != null && objMenu != null && objMenu.HasDisplayedItem(ssoRedirectUrl.XLIModuleCode) == false)
                        {
                            model.RedirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), districtSubDomain, currentUser, false, "/Home");
                        }
                    }
                }
                Session["MenuItem"] = null;
            }
            //display reCAPTCHA
            model.ShowCaptcha = false;
            int loginLimit = _configurationService.GetConfigurationByKeyWithDefaultValue(Constanst.LoginLimit, Constanst.LoginLimitDefault);
            if (!SessionManager.ShowCaptcha && !isSSO && userService.GetUserLoginFailCount(GetUserId(currentUser)) > loginLimit)
            {
                if (model.IsAuthenticated)
                {
                    userService.DeleteUserLoginFailCount(GetUserId(currentUser));
                }
                SessionManager.ShowCaptcha = true;
                return Json(new { Message = "You must satisfy the CAPTCHA verification requirements to indicate that you are an authorized user.", Type = "error", ShowCaptcha = true });
            }

            if (currentUser == null || !model.IsAuthenticated)
            {
                model.IsAuthenticated = false;
                userService.IncreaseUserLoginFailCount(GetUserId(currentUser));
                if (userService.GetUserLoginFailCount(GetUserId(currentUser)) > loginLimit)
                {
                    //display recaptcha
                    model.ShowCaptcha = true;
                    SessionManager.ShowCaptcha = true;
                }
            }
            else
            {
                userService.DeleteUserLoginFailCount(GetUserId(currentUser));
                if (!isSSO && (!model.HasEmailAddress || !model.HasSecurityQuestion || model.HasTemporaryPassword))
                {
                    // clear cookies
                    foreach (var item in Response.Cookies.AllKeys)
                    {
                        if (item.ToString() != Constanst.ASPNETSessionId)
                            Response.Cookies.Remove(item.ToString());
                    }

                    // save info to session
                    HttpContext.Session["BeforeLoginSession"] = new Tuple<LoginAccountViewModel, int, int>(model, currentUser.Id, currentUser.RoleId);
                    // redirect to change question
                    model.RedirectUrl = string.Format("{0}?id={1}",
                        new UrlHelper(Request.RequestContext).Action("SetAccountInformation", "Account"),
                        currentUser.Id);
                }

                if (GetShowDisclaimerContentConfigurationValue(currentUser.DistrictId ?? 0))
                {
                    if (!currentUser.TermOfUseAccepted.HasValue && !model.TermsOfUse)
                    {
                        // clear cookies
                        foreach (var item in Response.Cookies.AllKeys)
                        {
                            if (item != Constanst.ASPNETSessionId)
                                Response.Cookies.Remove(item);
                        }

                        model.ShowDisclaimerContent = true;
                        model.DisclaimerContent = GetDistrictDeocdeOrConfigurationValue(currentUser.DistrictId ?? 0,
                            DistrictDecodeLabelConstant.FistLogin_DisclaimerContent);
                        model.DisclaimerCheckboxLabel =
                            GetDistrictDeocdeOrConfigurationValue(currentUser.DistrictId ?? 0,
                                DistrictDecodeLabelConstant.FistLogin_DisclaimerCheckboxLabel);
                    }
                    else
                    {
                        var termUser = userService.GetUserById(currentUser.Id);
                        if (termUser != null)
                        {
                            termUser.TermOfUseAccepted = DateTime.UtcNow;
                            userService.SaveUser(termUser);
                        }
                    }
                }
            }
            //Init warning Message
            InitWarningMessage(model, districtSubDomain);

            return Json(model);
        }

        private bool IsSubApp(string redirectUrl)
        {
            var subApps = new string[] { "im/#/" };

            foreach (var app in subApps)
            {
                if (redirectUrl.ToLower().Contains(app)) return true;
            }

            return false;
        }

        public ActionResult SingleSignOn(string provider, string loginType = "", string returnUrl = "")
        {
            var requestUrl = string.Empty;
            var subdomain = HelperExtensions.GetSubdomain().ToLower();
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                TempData["ReturnUrl"] = returnUrl;
            }

            if (provider == SSOProvider.CLASSLINK)
            {
                var ssoInfo = userService.GetSSOInformationByType(provider);
                if (ssoInfo == null)
                {
                    ViewBag.Message = "Your district does not allow to login by ClassLink.";
                    return View("SSOError");
                }

                var callbackUrl = $"https://portal.{ConfigurationManager.AppSettings["LinkItUrl"]}/account/classlinkcallback";
                requestUrl = ExternalURLs.CLASSLINK_REQUEST_AUTH
                       .Replace("{client_id}", ssoInfo.Auth0ClientId)
                       .Replace("{redirect_uri}", callbackUrl)
                       .Replace("{state}", subdomain);
            }
            else if (provider == SSOProvider.GOOGLE)
            {
                var ssoInfo = userService.GetSSOInformationByType(SSOProvider.GOOGLE);

                if (ssoInfo != null)
                {
                    var callbackUrl = $"https://portal.{ConfigurationManager.AppSettings["LinkItUrl"]}/account/googlecallback";

                    if (!string.IsNullOrEmpty(loginType))
                    {
                        callbackUrl = loginType == TextConstants.LOGIN_ROLE_STUDENT
                            ? callbackUrl.Replace("/account/googlecallback", "/student/googlecallback")
                            : callbackUrl.Replace("/account/googlecallback", "/manageparent/googlecallback");
                    }

                    requestUrl = ExternalURLs.GOOLE_REQUEST_CODE
                       .Replace("{client_id}", ssoInfo.Auth0ClientId)
                       .Replace("{redirect_uri}", callbackUrl)
                       .Replace("{state}", subdomain);
                }
                else
                {
                    ViewBag.Message = "Your district does not allow to login by Google.";
                    return View("SSOError");
                }
            }
            else if (provider == SSOProvider.MICROSOFT)
            {
                var ssoInfo = userService.GetSSOInformationByType(SSOProvider.MICROSOFT);

                if (ssoInfo != null)
                {
                    var callbackUrl = $"https://portal.{ConfigurationManager.AppSettings["LinkItUrl"]}/account/microsoftcallback";

                    if (!string.IsNullOrEmpty(loginType))
                    {
                        callbackUrl = loginType == TextConstants.LOGIN_ROLE_STUDENT
                            ? callbackUrl.Replace("/account/microsoftcallback", "/student/microsoftcallback")
                            : callbackUrl.Replace("/account/microsoftcallback", "/manageparent/microsoftcallback");
                    }

                    requestUrl = ExternalURLs.MICROSOFT_REQUEST_CODE_MULTITENANT
                       .Replace("{client_id}", ssoInfo.Auth0ClientId)
                       .Replace("{redirect_uri}", callbackUrl)
                       .Replace("{state}", subdomain);
                }
                else
                {
                    ViewBag.Message = "Your district does not allow to login by Microsoft.";
                    return View("SimpleSSOError");
                }
            }
            else if (provider == SSOProvider.CLEVER)
            {
                var ssoInfo = userService.GetConfigLogonBySSO(HelperExtensions.GetDistrictIdBySubdomain(), SSOProvider.CLEVER);
                if (ssoInfo != null && ssoInfo.SSOInformation != null)
                {
                    var callbackUrl = CleverHelper.CreateCallbackUrl(Request, loginType);

                    requestUrl = ExternalURLs.CLEVER_REQUEST_CODE
                        .Replace("{client_id}", ssoInfo.SSOInformation.Auth0ClientId)
                        .Replace("{redirect_uri}", callbackUrl)
                        .Replace("{state}", HelperExtensions.GetSubdomain().ToLower());
                }
                else
                {
                    ViewBag.Message = "Your district does not allow to login by Clever.";
                    return View("SimpleSSOError");
                }
            }

            return Redirect(requestUrl);
        }


        public ActionResult ClassLinkCallback(string code, string state)
        {
            var urlRedirect = Request.Url.ToString();
            urlRedirect = Regex.Replace(urlRedirect, "portal", state, RegexOptions.IgnoreCase);
            urlRedirect = Regex.Replace(urlRedirect, "classlinkcallback", "ClassLinkCallbackProcessing", RegexOptions.IgnoreCase);

            return Redirect(urlRedirect);
        }

        public ActionResult ClassLinkCallbackProcessing(string code)
        {
            try
            {
                var result = ClassLinkProcessLoginToLinkIt(code);

                if (result.Status && !string.IsNullOrEmpty(result.Url))
                {
                    return Redirect(result.Url);
                }

                ViewBag.Message = result.Message;
                return View("SSOError");
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                ViewBag.Message = $"Code: {SSOErrorCode.CLASSLINK_500}. An unknown error has occurred. Please contact your system administrator.";
                return View("SSOError");
            }
        }

        public ActionResult GoogleCallback(string code, string state, string loginType = "")
        {
            var urlRedirect = Request.Url.ToString();
            urlRedirect = Regex.Replace(urlRedirect, "portal", state, RegexOptions.IgnoreCase);
            urlRedirect = Regex.Replace(urlRedirect, "googlecallback", "googlecallbackprocessing", RegexOptions.IgnoreCase);

            return Redirect(urlRedirect);
        }

        public ActionResult GoogleCallbackProcessing(string code, string loginType = "")
        {
            var ssoInfo = userService.GetSSOInformationByType(SSOProvider.GOOGLE);
            var tokenInfo = GoogleGetAccessToken(ssoInfo, code, loginType);

            if (tokenInfo == null || string.IsNullOrEmpty(tokenInfo.AccessToken))
            {
                var warningMessage = $"Code: {SSOErrorCode.GOOGLE_501}. An unknown error has occurred. Please contact your system administrator.";
                return RedirectSSOWarningForm(warningMessage, loginType);
            }

            var googleUserInfo = GoogleGetUserInfo(tokenInfo.AccessToken);

            if (googleUserInfo == null)
            {
                var warningMessage = $"Code: {SSOErrorCode.GOOGLE_502}. An unknown error has occurred. Please contact your system administrator.";
                return RedirectSSOWarningForm(warningMessage, loginType);
            }

            return ProcessSSOLogin(SSOProvider.GOOGLE, googleUserInfo.Email, loginType, ssoInfo.SSOInformationID);
        }

        public ActionResult MicrosoftCallback(string code, string state, string loginType = "")
        {
            var urlRedirect = Request.Url.ToString();
            urlRedirect = Regex.Replace(urlRedirect, "portal", state, RegexOptions.IgnoreCase);
            urlRedirect = Regex.Replace(urlRedirect, "microsoftcallback", "microsoftcallbackprocessing", RegexOptions.IgnoreCase);

            return Redirect(urlRedirect);
        }

        public ActionResult MicrosoftCallbackProcessing(string code, string loginType = "")
        {
            var ssoInfo = userService.GetSSOInformationByType(SSOProvider.MICROSOFT);

            var tokenInfo = MicrosoftGetAccessToken(ssoInfo, code, loginType);
            var email = DecodeToken(tokenInfo?.IdToken);

            if (string.IsNullOrEmpty(email))
            {
                email = DecodeToken(tokenInfo?.AccessToken);
            }

            if (string.IsNullOrEmpty(email))
            {
                var warningMessage = $"Code: {SSOErrorCode.MICROSOFT_501}. An unknown error has occurred. Please contact your system administrator.";
                return RedirectSSOWarningForm(warningMessage, loginType);
            }

            return ProcessSSOLogin(SSOProvider.MICROSOFT, email, loginType, ssoInfo.SSOInformationID);
        }

        public ActionResult  PrimaryCleverCallback(string code)
        {
            try
            {
                var ssoInformation = userService.GetSSOInformationByType(SSOProvider.CLEVER);
                if (ssoInformation == null)
                    throw new NotFoundException("Dont find clever sso");

                var accessToken = CleverGetAccessToken(ssoInformation, code: code, loginType: string.Empty,
                    isPrimaryCallBack: true);

                if (accessToken == null || string.IsNullOrEmpty(accessToken?.AccessToken))
                    throw new ArgumentException("Cannot get clever accessToken");


                var cleverUserInfo = GetCleverUserInfo<LoginInstantCleverUserInfoDto>(accessToken.AccessToken);

                if (string.IsNullOrEmpty(cleverUserInfo.District))
                    throw new ArgumentException("CleverDistrictId is empty");

                string liCode = GetLICodeFromCleverDistrictId(cleverUserInfo.District);

                string token =
                    CleverHelper.GeneratePrimaryCleverCallBackToken(accessToken: accessToken.AccessToken, cleverUserInfo.IsStudent);

                string processPrimaryCleverCallbackUrl = CleverHelper.CreatePrimaryProcessCleverCallBackUrl(token: token, liCode: liCode, request: Request);

                return Redirect(processPrimaryCleverCallbackUrl);
            }
            catch (NotFoundCleverDistrictIdOnVaultException)
            {
                ViewBag.IsNotDetectedRole = true;
                ViewBag.Message = LocalizeHelper.LocalizedToString(TextConstants.NOT_FOUND_CLEVER_DISTRICT_ID_ON_VAULT);
                return View("SimpleSSOError");
            }
            catch (Exception e)
            {
                ViewBag.IsNotDetectedRole = true;
                ViewBag.Message = "An unknown error has occurred. Please contact administrator.";
                return View("SimpleSSOError");
            }
        }

        public ActionResult ProcessPrimaryCleverCallback(string token)
        {
            try
            {
                var tokenInfo = CleverHelper.ParseCleverToken(token);
                string loginType = tokenInfo.IsStudent ? TextConstants.LOGIN_ROLE_STUDENT : string.Empty;
                bool isStudent = tokenInfo.IsStudent;

                if (tokenInfo.ExpireOn < DateTime.UtcNow)
                {
                    ViewBag.IsStudent = isStudent;
                    ViewBag.Message = "Token expired";
                    return View("SimpleSSOError");
                }

                var result = CleverProcessLoginToLinkIt(code: string.Empty, loginType, tokenInfo.AccessToken);

                if (!result.Status && result.AllowLinkAccount && !string.IsNullOrEmpty(result.Token))
                {
                    ViewBag.DisplayAccountInfo = result.DisplayAccountInfo;
                    ViewBag.IsStudent = isStudent;
                    if (TempData.ContainsKey("ReturnUrl"))
                    {
                        TempData.Keep("ReturnUrl");
                    }
                    return View("CleverLinkUser", (object)result.Token);
                }
                else if (!result.Status)
                {
                    ViewBag.IsStudent = isStudent;
                    ViewBag.Message = result.Message;
                    return View("SimpleSSOError");
                }

                return Redirect(result.Url);
            }
            catch (Exception e)
            {
                ViewBag.IsNotDetectedRole = true;
                ViewBag.Message = "An unknown error has occurred. Please contact administrator.";
                return View("SimpleSSOError");
            }
        }

        public ActionResult CleverCallback(string code, string state)
        {
            string requestUrl = CleverHelper.CreateProcessCallBackUrl(Request.Url.AbsoluteUri, licode: state);
            return Redirect(requestUrl);
        }

        public ActionResult CleverProcessCallBack(string code, string loginType = "")
        {
            bool isStudent = loginType == TextConstants.LOGIN_ROLE_STUDENT;
            try
            {
                var result = CleverProcessLoginToLinkIt(code, loginType);

                if (!result.Status && result.AllowLinkAccount && !string.IsNullOrEmpty(result.Token))
                {
                    ViewBag.DisplayAccountInfo = result.DisplayAccountInfo;
                    ViewBag.IsStudent = isStudent;
                    return View("CleverLinkUser", (object)result.Token);
                }
                else if (!result.Status)
                {
                    ViewBag.IsStudent = isStudent;
                    ViewBag.Message = result.Message;
                    return View("SimpleSSOError");
                }

                return Redirect(result.Url);
            }
            catch (Exception e)
            {
                ViewBag.IsStudent = isStudent;
                ViewBag.Message = "An unknown error has occurred. Please contact administrator.";
                return View("SimpleSSOError");
            }
        }

        [HttpPost]
        public ActionResult ProcessLinkGoogleAccount(LoginAccountViewModel model)
        {
            var message = string.Empty;

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Util.Secret)),
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(Request.Headers["Auth"],
                      parameters, out securityToken);

                var userName = principal.Claims.FirstOrDefault(m => m.Type == "UserName").Value;
                var districtId = int.Parse(principal.Claims.FirstOrDefault(m => m.Type == "DistrictID").Value);
                var isStudent = bool.Parse(principal.Claims.FirstOrDefault(m => m.Type == "IsStudent").Value);
                var isParent = bool.Parse(principal.Claims.FirstOrDefault(m => m.Type == "IsParent").Value);
                var roleIds = GetRoleIds(isStudent, isParent);
                var ssoInfo = userService.GetSSOInformationByType(SSOProvider.GOOGLE);

                if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                {
                    message = "Please input username and password";
                }
                else
                {
                    var user = userService.GetUserByUsernameAndDistrict(model.UserName, districtId, roleIds);

                    if (user != null && userService.IsValidUser(user, model.Password))
                    {
                        if (isStudent && !user.IsStudent)
                        {
                            message = "You aren't a student.";
                        }
                        else
                        {
                            userService.SaveSSOUserMapping(userName, user.Id, districtId, SSOProvider.GOOGLE);

                            var result = ProcessLogon(new LoginAccountViewModel
                            {
                                UserName = model.UserName,
                                Password = "Password@9999",
                                District = districtId,
                                SSOInformationId = ssoInfo.SSOInformationID,
                                SSOType = SSOProvider.GOOGLE,
                                RoleId = user.RoleId
                            }, true);

                            if (result.Data != null)
                            {
                                var data = result.Data as LoginAccountViewModel;

                                if (data == null || !data.IsAuthenticated)
                                {
                                    message = "An unknown error has occurred.";
                                }
                            }
                        }
                    }
                    else
                    {
                        message = "Your username and password do not match our records.";
                    }
                }
            }
            catch (Exception)
            {
                message = "Invalid session, please try login again.";
            }
            var returnUrl = TempData.ContainsKey("ReturnUrl") ? TempData["ReturnUrl"]?.ToString() : "";
            return Json(new { Message = message, ReturnUrl = returnUrl }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ProcessLinkMicrosoftAccount(LoginAccountViewModel model)
        {
            var message = string.Empty;

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Util.Secret)),
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(Request.Headers["Auth"],
                      parameters, out securityToken);

                var userName = principal.Claims.FirstOrDefault(m => m.Type == "UserName").Value;
                var districtId = int.Parse(principal.Claims.FirstOrDefault(m => m.Type == "DistrictID").Value);
                var isStudent = bool.Parse(principal.Claims.FirstOrDefault(m => m.Type == "IsStudent").Value);
                var isParent = bool.Parse(principal.Claims.FirstOrDefault(m => m.Type == "IsParent").Value);
                var roleIds = GetRoleIds(isStudent, isParent);
                var ssoInfo = userService.GetSSOInformationByType(SSOProvider.MICROSOFT);

                if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                {
                    message = "Please input username and password";
                }
                else
                {
                    var user = userService.GetUserByUsernameAndDistrict(model.UserName, districtId, roleIds);

                    if (user != null && userService.IsValidUser(user, model.Password))
                    {
                        if (isStudent && !user.IsStudent)
                        {
                            message = "You aren't a student.";
                        }
                        else
                        {
                            userService.SaveSSOUserMapping(userName, user.Id, districtId, SSOProvider.MICROSOFT);

                            var result = ProcessLogon(new LoginAccountViewModel
                            {
                                UserName = model.UserName,
                                Password = "Password@9999",
                                District = districtId,
                                SSOInformationId = ssoInfo.SSOInformationID,
                                SSOType = SSOProvider.MICROSOFT,
                                RoleId = user.RoleId
                            }, true);

                            if (result.Data != null)
                            {
                                var data = result.Data as LoginAccountViewModel;

                                if (data == null || !data.IsAuthenticated)
                                {
                                    message = "An unknown error has occurred.";
                                }
                            }
                        }
                    }
                    else
                    {
                        message = "Your username and password do not match our records.";
                    }
                }
            }
            catch (Exception)
            {
                message = "Invalid session, please try login again.";
            }
            var returnUrl = TempData.ContainsKey("ReturnUrl") ? TempData["ReturnUrl"]?.ToString() : "";
            return Json(new { Message = message, ReturnUrl = returnUrl }, JsonRequestBehavior.AllowGet);
        }

        private SSOResultDTO ClassLinkProcessLoginToLinkIt(string code)
        {
            var result = new SSOResultDTO();

            if (string.IsNullOrEmpty(code))
            {
                result.Message = $"Code: {SSOErrorCode.CLASSLINK_500}. An unknown error has occurred. Please contact your system administrator.";
                return result;
            }

            var tokenInfo = ClassLinkGetAccessToken(code);

            if (tokenInfo == null || string.IsNullOrEmpty(tokenInfo.AccessToken))
            {
                result.Message = $"Code: {SSOErrorCode.CLASSLINK_501}. An unknown error has occurred. Please contact your system administrator.";
                return result;
            }

            var profile = GetClassLinkUserInfo(tokenInfo.AccessToken);

            if (profile == null)
            {
                result.Message = $"Code: {SSOErrorCode.CLASSLINK_502}. An unknown error has occurred. Please contact your system administrator.";
                return result;
            }

            var districtGroup = userService.GetSSODistrictGroupByTenantID(profile.TenantId, SSOProvider.CLASSLINK);
            var districtId = districtGroup?.DistrictID ?? 0;
            var liCode = districtGroup?.District.LICode ?? string.Empty;

            var classLinkUserResult = userService.GetClassLinkUserMapping(districtId, profile);

            if (classLinkUserResult == null)
            {
                result.Message = $"Cannot map ClassLink user's role \"{profile.Role}\" to Linkit's role";
                return result;
            }

            if (classLinkUserResult.Users.Count == 0
                && classLinkUserResult.Students.Count == 0
                && classLinkUserResult.Parents.Count == 0)
            {
                result.Message = "Cannot find LinkIt user with your ClassLink information.";
                return result;
            }

            if (classLinkUserResult.Users.Count > 1
                || classLinkUserResult.Students.Count > 1
                || classLinkUserResult.Parents.Count > 1)
            {
                result.Message = $"There are multiple users who have the same username {profile.LoginId}. Please contact your system administrator.";
                return result;
            }

            if (classLinkUserResult.Users.Count == 1)
            {
                var user = classLinkUserResult.Users.FirstOrDefault();

                if (profile.IsStudent && !user.IsStudent)
                {
                    result.Message = $"Code: {SSOErrorCode.CLASSLINK_401}. An unknown error has occurred. Please contact your system administrator.";
                    return result;
                }

                if (user.UserStatusId != (int)UserStatus.Active)
                {
                    result.Message = "Your email address has been suspended. Please contact your system administrator.";
                    return result;
                }

                return GenerateSuccessClassLinkSSO(user, false, liCode);
            }

            if (classLinkUserResult.Students.Count == 1)
            {
                var student = classLinkUserResult.Students.FirstOrDefault();
                var user = userService.CreateUserByStudent(student);

                if (user.UserStatusId != (int)UserStatus.Active)
                {
                    result.Message = "Your email address has been suspended. Please contact your system administrator.";
                    return result;
                }

                return GenerateSuccessClassLinkSSO(user, true, liCode);
            }

            if (classLinkUserResult.Parents.Count == 1)
            {
                var parent = classLinkUserResult.Parents.FirstOrDefault();
                var user = userService.GetUserById(parent.UserID);

                if (user.UserStatusId != (int)UserStatus.Active)
                {
                    result.Message = "Your email address has been suspended. Please contact your system administrator.";
                    return result;
                }

                return GenerateSuccessClassLinkSSO(user, false, liCode);
            }

            result.Message = $"Code: {SSOErrorCode.CLASSLINK_500}. An unknown error has occurred. Please contact your system administrator.";
            return result;
        }

        [HttpPost]
        public ActionResult ProcessLinkCleverAccount(LoginAccountViewModel model)
        {
            var message = string.Empty;

            try
            {
                var tokenInfo = CleverHelper.ExtractInfoFromToken(Request.Headers["Auth"]);
                var districtId = tokenInfo.DistrictId;
                var isStudent = tokenInfo.IsStudent;
                var isParent = tokenInfo.IsParent;
                var cleverUserId = tokenInfo.UserName;
                var roleIds = GetRoleIds(isStudent, isParent);

                var ssoInfo = userService.GetConfigLogonBySSO(districtId, SSOProvider.CLEVER);

                if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                {
                    message = "Please input username and password";
                }
                else
                {
                    var user = userService.GetUserByUsernameAndDistrict(model.UserName, districtId, roleIds);

                    if (user != null && userService.IsValidUser(user, model.Password))
                    {
                        if (isStudent && !user.IsStudent)
                        {
                            message = "You aren't a student.";
                        }
                        else if (user.IsParent)
                        {
                            message = "We don't support parent.";
                        }
                        else if (isStudent != user.IsStudent)
                        {
                            message = "Staff cannot be mapped to student, and vice versa.";
                        }
                        else
                        {
                            userService.SaveSSOUserMapping(cleverUserId, user.Id, districtId, SSOProvider.CLEVER);

                            var result = ProcessLogon(new LoginAccountViewModel
                            {
                                UserName = model.UserName,
                                Password = "Password@9999",
                                District = districtId,
                                SSOInformationId = ssoInfo.SSOInformationID,
                                SSOType = SSOProvider.CLEVER,
                                RoleId = user.RoleId
                            }, true);

                            if (result.Data != null)
                            {
                                var data = result.Data as LoginAccountViewModel;

                                if (data == null || !data.IsAuthenticated)
                                {
                                    message = "An unknown error has occurred.";
                                }
                            }
                        }
                    }
                    else
                    {
                        message = "Your username and password do not match our records.";
                    }
                }
            }
            catch (Exception)
            {
                message = "Invalid session, please try login again.";
            }
            var returnUrl = TempData.ContainsKey("ReturnUrl") ? TempData["ReturnUrl"]?.ToString() : "";
            return Json(new { Message = message, ReturnUrl = returnUrl }, JsonRequestBehavior.AllowGet);
        }

        private SSOResultDTO GenerateSuccessClassLinkSSO(User user, bool isFirstLogin, string liCode)
        {
            var claims = new Dictionary<string, string>
            {
                { "IsFirstStudentLogon", isFirstLogin.ToString() },
                { "RoleId", user.RoleId.ToString() }
            };

            var token = Util.GenerateToken(user.UserName, user.DistrictId.Value, 1, claims);
            return new SSOResultDTO
            {
                Status = true,
                Url = $"{Request.Url.AbsoluteUri.ToString().Replace(Request.RawUrl, "").Replace("portal", liCode.ToLower())}/account/classlinklogon?token={token}"
            };
        }

        private OAuth2TokenResponseDTO ClassLinkGetAccessToken(string code)
        {
            OAuth2TokenResponseDTO tokenInfo = null;

            var ssoInfo = userService.GetSSOInformationByType(SSOProvider.CLASSLINK);

            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.Parameters.Clear();
            request.AddParameter("code", code);
            request.AddParameter("client_id", ssoInfo.Auth0ClientId);
            request.AddParameter("client_secret", ssoInfo.Auth0ClientSecret);

            var client = new RestSharp.RestClient(ExternalURLs.CLASSLINK_REQUEST_TOKEN);
            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                tokenInfo = JsonConvert.DeserializeObject<OAuth2TokenResponseDTO>(response.Content);
            }

            return tokenInfo;
        }

        public ActionResult ClassLinkLogOn(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters parameters = new TokenValidationParameters()
            {
                ValidateLifetime = false,
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Util.Secret)),
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken securityToken;
            ClaimsPrincipal principal = null;

            try
            {
                principal = tokenHandler.ValidateToken(token,
                  parameters, out securityToken);
            }
            catch (Exception)
            {
                ViewBag.Message = $"Code: {SSOErrorCode.CLASSLINK_503}. An unknown error has occurred. Please contact your system administrator.";
                return View("SSOError");
            }

            var userName = principal.Claims.FirstOrDefault(m => m.Type == "UserName").Value;
            var districtId = int.Parse(principal.Claims.FirstOrDefault(m => m.Type == "DistrictID").Value);
            var isFirstStudentLogon = principal.Claims.FirstOrDefault(m => m.Type == "IsFirstStudentLogon").Value == "True";
            var roleId = int.Parse(principal.Claims.FirstOrDefault(m => m.Type == "RoleId").Value);
            var ssoInfo = userService.GetSSOInformationByType(SSOProvider.CLASSLINK);

            var result = ProcessLogon(new LoginAccountViewModel
            {
                UserName = userName,
                Password = "Password@9999",
                District = districtId,
                IsFirstStudentLogonSSO = isFirstStudentLogon,
                SSOInformationId = ssoInfo.SSOInformationID,
                SSOType = SSOProvider.CLASSLINK,
                RoleId = roleId
            }, true);

            if (result.Data != null)
            {
                var data = result.Data as LoginAccountViewModel;

                if (data != null && data.IsAuthenticated)
                {
                    return Redirect(data.RedirectUrl);
                }
            }

            ViewBag.Message = "Unauthorize on ClassLink.";
            return View("SSOError");
        }

        private ClassLinkProfileDTO GetClassLinkUserInfo(string access_token)
        {
            var client = new RestSharp.RestClient("https://nodeapi.classlink.com/v2/my/info");
            var request = new RestSharp.RestRequest(RestSharp.Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", String.Format("Bearer {0}", access_token));
            RestSharp.IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<ClassLinkProfileDTO>(response.Content);
            }

            return null;
        }

        private OAuth2TokenResponseDTO GoogleGetAccessToken(SSOInformation ssoInfo, string code, string loginType = "")
        {
            var callbackUrl = $"https://portal.{ConfigurationManager.AppSettings["LinkItUrl"]}/account/googlecallback";

            if (!string.IsNullOrEmpty(loginType))
            {
                callbackUrl = loginType == TextConstants.LOGIN_ROLE_STUDENT
                    ? callbackUrl.Replace("/account/googlecallback", "/student/googlecallback")
                    : callbackUrl.Replace("/account/googlecallback", "/manageparent/googlecallback");
            }

            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.Parameters.Clear();
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("access_type", "offline");
            request.AddParameter("redirect_uri", callbackUrl);
            request.AddParameter("code", code);
            request.AddParameter("client_id", ssoInfo.Auth0ClientId);
            request.AddParameter("client_secret", ssoInfo.Auth0ClientSecret);

            var client = new RestSharp.RestClient(ExternalURLs.GOOGLE_REQUEST_TOKEN);
            var response = client.Execute(request);

            return response.StatusCode == HttpStatusCode.OK
                ? JsonConvert.DeserializeObject<OAuth2TokenResponseDTO>(response.Content)
                : null;
        }

        private OAuth2TokenResponseDTO MicrosoftGetAccessToken(SSOInformation ssoInfo, string code, string loginType = "")
        {
            var callbackUrl = $"https://portal.{ConfigurationManager.AppSettings["LinkItUrl"]}/account/microsoftcallback";

            if (!string.IsNullOrEmpty(loginType))
            {
                callbackUrl = loginType == TextConstants.LOGIN_ROLE_STUDENT
                    ? callbackUrl.Replace("/account/microsoftcallback", "/student/microsoftcallback")
                    : callbackUrl.Replace("/account/microsoftcallback", "/manageparent/microsoftcallback");
            }

            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.Parameters.Clear();
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("access_type", "offline");
            request.AddParameter("redirect_uri", callbackUrl);
            request.AddParameter("code", code);
            request.AddParameter("client_id", ssoInfo.Auth0ClientId);
            request.AddParameter("client_secret", ssoInfo.Auth0ClientSecret);

            var urlEndpoint = ExternalURLs.MICROSOFT_REQUEST_TOKEN_MULTITENANT;
            var client = new RestSharp.RestClient(urlEndpoint);
            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<OAuth2TokenResponseDTO>(response.Content);
            }

            return null;
        }

        private string ReplaceDomainByRole(string loginType, string domain)
        {
            var url = loginType == TextConstants.LOGIN_ROLE_STUDENT ? "/student/googlecallback" : "/manageparent/googlecallback";
            return domain.Replace("/account/googlecallback", url);
        }

        private GoogleProfileDTO GoogleGetUserInfo(string access_token)
        {
            var request = new RestSharp.RestRequest(RestSharp.Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", $"Bearer {access_token}");

            var client = new RestSharp.RestClient(ExternalURLs.GOOGL_REQUEST_USER_INFO);
            var response = client.Execute(request);

            return response.StatusCode == HttpStatusCode.OK
                ? JsonConvert.DeserializeObject<GoogleProfileDTO>(response.Content)
                : null;
        }

        private IEnumerable<int> GetRoleIds(string loginType)
        {
            if (loginType == TextConstants.LOGIN_ROLE_STUDENT)
            {
                return new List<int> { (int)RoleEnum.Student };
            }

            if (loginType == TextConstants.LOGIN_ROLE_PARENT)
            {
                return new List<int> { (int)RoleEnum.Parent };
            }

            return new List<int> {
                (int)RoleEnum.Publisher,
                (int)RoleEnum.NetworkAdmin,
                (int)RoleEnum.DistrictAdmin,
                (int)RoleEnum.SchoolAdmin,
                (int)RoleEnum.Teacher,
            };
        }

        private IEnumerable<int> GetRoleIds(bool isStudent, bool isParent)
        {
            if (isStudent)
            {
                return new List<int> { (int)RoleEnum.Student };
            }

            if (isParent)
            {
                return new List<int> { (int)RoleEnum.Parent };
            }

            return new List<int> {
                (int)RoleEnum.Publisher,
                (int)RoleEnum.NetworkAdmin,
                (int)RoleEnum.DistrictAdmin,
                (int)RoleEnum.SchoolAdmin,
                (int)RoleEnum.Teacher,
            };
        }

        private string DecodeToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return string.Empty;
            }

            var handler = new JwtSecurityTokenHandler();
            var decodedValue = handler.ReadJwtToken(accessToken);
            var email = decodedValue.Claims.FirstOrDefault(x => x.Type == "email")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                email = decodedValue.Claims.FirstOrDefault(x => x.Type == "unique_name")?.Value;
            }

            if (string.IsNullOrEmpty(email))
            {
                email = decodedValue.Claims.FirstOrDefault(x => x.Type == "upn")?.Value;
            }

            return email;
        }

        private string AllowUserLinkSSOAccount(string email, int districtId, bool isStudent, bool isParent)
        {
            var claims = new Dictionary<string, string>();
            claims.Add("IsStudent", (isStudent).ToString());
            claims.Add("IsParent", (isParent).ToString());
            var token = Util.GenerateToken(email, districtId, 30, claims);
            return token;
        }

        public ActionResult LTIRedirectEndpoint()
        {
            var result = new SSOResultDTO();
            LtiParams ltiParams = new LtiParams(Request.Params);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var idToken = ltiParams.ltiParams[LtiConstants.AuthorizeRequest.ResponseType];
            var state = ltiParams.ltiParams[LtiConstants.AuthorizeRequest.State];

            if (string.IsNullOrEmpty(idToken))
                result.Message = $"Code: {SSOErrorCode.CANVAS_501}. id_token is missing or empty.";

            if (!tokenHandler.CanReadToken(idToken))
                result.Message = $"Code: {SSOErrorCode.CANVAS_501}. Cannot read id_token.";

            var token = tokenHandler.ReadJwtToken(idToken);
            var ltiRequest = new LtiDeepLinkingRequest(token.Payload);
            var idTokenDto = new IdTokenDto
            {
                Iss = ltiRequest.Iss,
                Nonce = ltiRequest.Nonce,
                Aud = ltiRequest.Aud,
                Iat = ltiRequest.Iat,
                Exp = ltiRequest.Exp,
                Azp = ltiRequest.Azp,
                State = state,
                DeploymentId = ltiRequest.DeploymentId
            };
            var ltiInformation = _ltiSingleSignOnService.GetLTIInformationByDeploymentId(idTokenDto.DeploymentId);
            if (ltiInformation == null)
                return View("SSOError");

            var districtId = ltiInformation.DistrictID;

            if (!IsValidLtiToken(token, ltiInformation, idToken, idTokenDto.Iss))
                return View("SSOError");

            if (_ltiSingleSignOnService.ValidationAuthorize(idTokenDto))
            {
                var userMappingResult = userService.GetCanvasUserMapping(districtId, new UserMappingDto { PersonSourcedId = ltiRequest.Lis?.PersonSourcedId, Sub = ltiRequest.Sub, Email = ltiRequest.Email });
                if (!string.IsNullOrEmpty(userMappingResult.UserName))
                {
                    var loginResult = ProcessLogon(new LoginAccountViewModel
                    {
                        UserName = userMappingResult.UserName,
                        Password = "Password@9999",
                        District = districtId,
                        IsFirstStudentLogonSSO = userMappingResult.IsFirstStudentLogonSSO,
                        SSOInformationId = ltiInformation.LTIInformationID,
                        SSOType = SSOProvider.CANVAS,
                        RoleId = userMappingResult.RoleId
                    }, true);
                    if (loginResult.Data != null)
                    {
                        var data = loginResult.Data as LoginAccountViewModel;
                        if (data != null && data.IsAuthenticated)
                        {
                            _ltiSingleSignOnService.UpdateStatus(idTokenDto.Nonce, true);
                            return Redirect(data.RedirectUrl);
                        }
                    }
                }

                ViewBag.Message = "The user information did not match SISID, AltCode, LocalCode, or Email in the LinkIt! portal. Please contact support@linkit.com for assistance.";
                return View("SSOError");
            }

            return View("SSOError");
        }

        public ActionResult OIDCCallback()
        {
            LtiParams ltiParams = new LtiParams(Request.Form);

            if (ltiParams.ltiParams.Count > 0)
            {
                ServicePointManager.ServerCertificateValidationCallback = new
                    RemoteCertificateValidationCallback
                    (
                       delegate { return true; }
                    );

                var _state = CryptoRandom.CreateUniqueId();
                var _nonce = CryptoRandom.CreateUniqueId();

                var ltiAuthorizeDto = new LtiAuthorizeDto()
                {
                    ClientId = ltiParams.ltiParams[LtiConstants.AuthorizeRequest.ClientId],
                    LoginHint = ltiParams.ltiParams[LtiConstants.AuthorizeRequest.LoginHint],
                    LtiMessageHint = ltiParams.ltiParams[LtiConstants.AuthorizeRequest.Lti_Message_Hint],
                    Nonce = _nonce,
                    Prompt = LtiConstants.AuthorizeRequest.Prompt,
                    RedirectUri = $"{HelperExtensions.GetHTTPProtocal(Request)}://{Request.Url.Host}/Account/LTIRedirectEndpoint",
                    ResponseMode = LtiConstants.AuthorizeRequest.ResponseMode,
                    ResponseType = LtiConstants.AuthorizeRequest.ResponseType,
                    Scope = LtiConstants.AuthorizeRequest.Scope,
                    State = _state,
                    PlatformID = ltiParams.ltiParams[LtiConstants.AuthorizeRequest.Iss],
                };

                var ltiInformation = _ltiSingleSignOnService.GetLTIInformation(ltiAuthorizeDto.ClientId);
                if (ltiInformation == null)
                    return View("SSOError");

                if (_ltiSingleSignOnService.LtiParamIsValid(ltiAuthorizeDto, ltiInformation))
                    return View("SSOError");

                ltiAuthorizeDto.DeploymentId = ltiInformation.DeploymentID;
                _ltiSingleSignOnService.SaveLTIRequestHistory(ltiAuthorizeDto);

                var rootDomain = ltiInformation.AuthenticationRequestURL;
                var requestUrl = new RequestUrl(rootDomain);
                var url = requestUrl.CreateAuthorizeUrl
                (
                    clientId: ltiAuthorizeDto.ClientId,
                    responseType: ltiAuthorizeDto.ResponseType,
                    responseMode: ltiAuthorizeDto.ResponseMode,
                    redirectUri: ltiAuthorizeDto.RedirectUri,
                    scope: ltiAuthorizeDto.Scope,
                    state: ltiAuthorizeDto.State,
                    loginHint: ltiAuthorizeDto.LoginHint,
                    nonce: ltiAuthorizeDto.Nonce,
                    prompt: ltiAuthorizeDto.Prompt,
                    extra: new { lti_message_hint = ltiAuthorizeDto.LtiMessageHint }
                );
                return Redirect(url);
            }
            return View("SSOError");
        }

        private bool IsValidLtiToken(JwtSecurityToken jwt, LTIInformation ltiInformation, string token, string iss)
        {
            using (var client = new HttpClient())
            {
                var publicKey = ltiInformation?.PublicKey;
                var keySetJson = client.GetStringAsync(publicKey).Result;
                var keySet = JsonConvert.DeserializeObject<JsonWebKeySet>(keySetJson);
                var key = keySet.Keys.FirstOrDefault(k => k.Kid == jwt.Header.Kid.Replace("\\", "").Replace("\"", ""));
                if (key == null)
                    return false;

                var rsaParameters = new RSAParameters
                {
                    Modulus = Base64UrlEncoder.DecodeBytes(key.N),
                    Exponent = Base64UrlEncoder.DecodeBytes(key.E)
                };

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateTokenReplay = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = ltiInformation.ClientID,
                    ValidIssuer = iss,
                    IssuerSigningKey = new RsaSecurityKey(rsaParameters),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5.0)
                };

                try
                {
                    ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                    return principal.Identity.IsAuthenticated;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        private void SendTokenResetPassword(User user, int expireMinutes)
        {
            if (user.IsNull() || string.IsNullOrEmpty(user.EmailAddress)) return;

            var token = Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ApproveChangePassword?token=" + Util.GenerateToken(user.UserName, user.DistrictId.GetValueOrDefault(), expireMinutes);
            var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.LinkItUseEmailCredentialKey);
            emailService.SendTokenResetPassword(user, token, emailCredentialSetting);
        }

        private int GetTempPasswordExpired()
        {
            var expiredMinutes = _configurationService.GetConfigurationByKey(TempPasswordExpired);
            if (int.TryParse(expiredMinutes?.Value, out int minutes))
            {
                return minutes;
            }
            return ContaintUtil.TEMP_PASSWORD_EXPIRED;
        }

        private SSOResultDTO CleverProcessLoginToLinkIt(string code, string loginType = "", string accessToken = "")
        {
            var isStudent = loginType == TextConstants.LOGIN_ROLE_STUDENT;
            var isParent = loginType == TextConstants.LOGIN_ROLE_PARENT;
            var roleIds = GetRoleIds(loginType);
            var result = new SSOResultDTO();

            var districtId = HelperExtensions.GetDistrictIdBySubdomainV2();
            var ssoInfo = userService.GetConfigLogonBySSO(districtId, SSOProvider.CLEVER);

            if (ssoInfo == null)
            {
                result.Message = "Your district does not allow login via Clever.";
                return result;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                var tokenInfo = CleverGetAccessToken(ssoInfo.SSOInformation, code, loginType);
                accessToken = tokenInfo.AccessToken;
            }

            var userInfo = GetCleverUserInfo<BaseCleverUserInfoDto>(accessToken);

            if (userInfo == null)
            {
                result.Message = $"Code: {SSOErrorCode.CLEVER_501}. An unknown error has occurred. Please contact your system administrator.";
                return result;
            }

            if (isStudent != userInfo.IsStudent)
            {
                result.Message = $"Code: {SSOErrorCode.CLEVER_502}. An unknown error has occurred. Please contact your system administrator.";
                return result;
            }

            result.DisplayAccountInfo = string.IsNullOrEmpty(userInfo.Email) ?  userInfo.Name.ToFullName() : userInfo.Email;

            string email = userInfo.Email;

            var linkItUserInfo = new List<User>();
            if (!string.IsNullOrEmpty(email))
            {
                linkItUserInfo = userService.Find(email, districtId, roleIds);
            }
            var isFirstStudentLogon = false;

            if (isStudent)
            {
                if (linkItUserInfo.Count > 0 && linkItUserInfo[0].RoleId != (int)RoleEnum.Student)
                {
                    result.Message = "Your email address has been used by another user. Please contact your system administrator.";
                }
                else if (linkItUserInfo.Count == 0)
                {
                    Student student = null;
                    if (!string.IsNullOrEmpty(email))
                    {
                        student = _studentService.GetStudentByEmail(districtId, email);
                    }

                    if (student == null)
                    {
                        result.AllowLinkAccount = true;
                        // email from clever call be null or empty => use CleverUserId to unique key instead of email
                        result.Token = AllowUserLinkSSOAccount(userInfo.Id, districtId, isStudent, isParent);
                    }
                    else
                    {
                        if (student.Status != (int)UserStatus.Active)
                        {
                            result.Message = "Your email address has been suspended. Please contact your system administrator.";
                        }
                        else
                        {
                            var user = userService.CreateUserByStudent(student);
                            linkItUserInfo.Add(user);
                            isFirstStudentLogon = true;
                        }
                    }
                }
            }
            else
            {
                if (linkItUserInfo.Count > 0 && linkItUserInfo[0].RoleId == (int)RoleEnum.Parent)
                {
                    result.Message = "Clever SSO don't support parent";
                }
            }

            if (linkItUserInfo.Count == 1 && linkItUserInfo.First().UserStatusId != (int)UserStatus.Active)
            {
                result.Message = "Your email address has been suspended. Please contact your system administrator.";
            }

            if (!string.IsNullOrEmpty(result.Message))
            {
                return result;
            }

            User linkItUser;

            if (linkItUserInfo.Count == 1)
            {
                linkItUser = linkItUserInfo.First();
            }
            else
            {
                linkItUser = userService.GetLinkitUserFromMapping(userInfo.Id, districtId, SSOProvider.CLEVER, roleIds);
            }

            if (!string.IsNullOrEmpty(linkItUser.UserName))
            {
                var linItLoginResult = ProcessLogon(new LoginAccountViewModel
                {
                    UserName = linkItUser.UserName,
                    Password = "Password@9999",
                    District = districtId,
                    IsFirstStudentLogonSSO = isFirstStudentLogon,
                    SSOInformationId = ssoInfo.SSOInformationID,
                    SSOType = SSOProvider.CLEVER,
                    RoleId = linkItUser.RoleId,
                    RedirectUrl = TempData.ContainsKey("ReturnUrl") ? TempData["ReturnUrl"].ToString() : ""
                }, true);

                if (linItLoginResult.Data != null)
                {
                    var data = linItLoginResult.Data as LoginAccountViewModel;

                    if (data == null || !data.IsAuthenticated)
                    {
                        result.Message = $"Code: {SSOErrorCode.CLEVER_503}. An unknown error has occurred. Please contact your system administrator.";
                    }
                    else
                    {
                        result.Status = true;
                        result.Url = data.RedirectUrl;
                    }
                }
            }
            else
            {
                result.AllowLinkAccount = true;
                // email from clever call be null or empty => use CleverUserId to unique key instead of email
                result.Token = AllowUserLinkSSOAccount(userInfo.Id, districtId, isStudent, isParent);
            }

            return result;
        }

        private OAuth2TokenResponseDTO CleverGetAccessToken(SSOInformation ssoInfo, string code, string loginType = "",
            bool isPrimaryCallBack = false)
        {
            var callbackUrl = CleverHelper.CreateCallbackUrl(Request, loginType, isPrimaryCallBack);

            var client = new RestSharp.RestClient(ExternalURLs.CLEVER_REQUEST_TOKEN);
            var request = new RestSharp.RestRequest(RestSharp.Method.POST);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", $"Basic {CleverHelper.CreateCleverAuthorizeToken(ssoInfo.Auth0ClientId, ssoInfo.Auth0ClientSecret)}");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", new CleverGetAccessTokenDto(code, callbackUrl).SerializeObject(true), RestSharp.ParameterType.RequestBody);
            RestSharp.IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<OAuth2TokenResponseDTO>(response.Content);
            }

            return null;
        }

        private string GetCleverUserId(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return string.Empty;
            }

            string resource = ExternalURLs.CLEVER_GET_TOKEN_INFO;
            var response = CleverHelper.SendGetRequest(resource, accessToken);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<CleverTokenInfoResponse>(response.Content).Data.UserId;
            }

            return string.Empty;
        }

        private T GetCleverUserInfo<T>(string accessToken)
        {
            var userId = GetCleverUserId(accessToken);
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Cant not found clever user info");

            string resource = string.Format(ExternalURLs.CLEVER_GET_USER_INFO, userId);
            var response = CleverHelper.SendGetRequest(resource, accessToken);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<CleverUserInfoResponseDto<T>>(response.Content).Data;
            }

            throw new ArgumentException("Cant get clever user info");
        }

        private string GetLICodeFromCleverDistrictId(string cleverDistrictId)
        {
            try
            {
                string liCode = _vaultProvider.GetLICodeByCleveDistrictId(cleverDistrictId);
                return liCode;
            }
            catch (NotFoundException)
            {
                throw new NotFoundCleverDistrictIdOnVaultException("Not found cleverDistrictId on vault");
            }
        }

        private void ClearCache()
        {
            try
            {
                _reportingHttpClient.Put<object>(AdminReporting.Endpoints.CLEAR_CACHE_MANAGER);
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        public ActionResult NYCCallbackProcessing(string SAMLResponse, string loginType = "")
        {
            var configuration = SAMLConfigurationBuilder.GetConfiguration(SSOClient.Nyc);
            var nycSamlResponse = SAML20Assertion.ParseNYCSamlResponse(SAMLResponse);

            if (nycSamlResponse == null || string.IsNullOrEmpty(nycSamlResponse.CertString))
            {
                return RedirectToAction("Error", new { message = Util.PassThroughVDETMessage105 });
            }

            if (string.IsNullOrEmpty(nycSamlResponse.Email))
            {
                return RedirectToAction("Error", new { message = Util.PassThroughVDETMessage114 });
            }

            var isValidateCertSignature = SAML20Assertion.ValidateX509CertificateSignature(nycSamlResponse.CertString, configuration.CertString);

            if (!isValidateCertSignature)
            {
                return RedirectToAction("Error", new { message = Util.PassThroughVDETMessage104 });
            }

            return ProcessSSOLogin(SSOProvider.NYC, nycSamlResponse.Email, loginType, 0);
        }

        [HttpPost]
        public ActionResult ProcessLinkNYCAccount(LoginAccountViewModel model)
        {
            var message = string.Empty;

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Util.Secret)),
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(Request.Headers["Auth"],
                      parameters, out securityToken);

                var userName = principal.Claims.FirstOrDefault(m => m.Type == "UserName").Value;
                var districtId = int.Parse(principal.Claims.FirstOrDefault(m => m.Type == "DistrictID").Value);
                var isStudent = bool.Parse(principal.Claims.FirstOrDefault(m => m.Type == "IsStudent").Value);
                var isParent = bool.Parse(principal.Claims.FirstOrDefault(m => m.Type == "IsParent").Value);
                var roleIds = GetRoleIds(isStudent, isParent);

                if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                {
                    message = "Please input username and password";
                }
                else
                {
                    var user = userService.GetUserByUsernameAndDistrict(model.UserName, districtId, roleIds);

                    if (user != null && userService.IsValidUser(user, model.Password))
                    {
                        if (isStudent && !user.IsStudent)
                        {
                            message = "You aren't a student.";
                        }
                        else
                        {
                            userService.SaveSSOUserMapping(userName, user.Id, districtId, SSOProvider.NYC);

                            var result = ProcessLogon(new LoginAccountViewModel
                            {
                                UserName = model.UserName,
                                Password = "Password@9999",
                                District = districtId,
                                SSOType = SSOProvider.NYC,
                                RoleId = user.RoleId
                            }, true);

                            if (result.Data != null)
                            {
                                var data = result.Data as LoginAccountViewModel;

                                if (data == null || !data.IsAuthenticated)
                                {
                                    message = "An unknown error has occurred.";
                                }
                            }
                        }
                    }
                    else
                    {
                        message = "Your username and password do not match our records.";
                    }
                }
            }
            catch (Exception)
            {
                message = "Invalid session, please try login again.";
            }
            var returnUrl = TempData.ContainsKey("ReturnUrl") ? TempData["ReturnUrl"]?.ToString() : "";
            return Json(new { Message = message, ReturnUrl = returnUrl }, JsonRequestBehavior.AllowGet);
        }

        private ActionResult ProcessSSOLogin(string providerType, string emailAddress, string loginType, int ssoInformationID)
        {
            string warningMessage = string.Empty;
            var isStudent = loginType == TextConstants.LOGIN_ROLE_STUDENT;

            var districtID = HelperExtensions.GetDistrictIdBySubdomain();

            var roleIds = GetRoleIds(loginType);
            var linkItUserInfo = userService.Find(emailAddress, districtID, roleIds);
            bool isFirstStudentLogon = false;

            if (isStudent)
            {
                if (linkItUserInfo.Count == 0)
                {
                    var student = _studentService.GetStudentByEmail(districtID, emailAddress);

                    if (student == null)
                    {
                        return RedirectSSOLinkUserForm(providerType, emailAddress, loginType);
                    }

                    if (student.Status != (int)UserStatus.Active)
                    {
                        warningMessage = "Your email address has been suspended. Please contact your system administrator.";
                        return RedirectSSOWarningForm(warningMessage, loginType);
                    }

                    var user = userService.CreateUserByStudent(student);
                    linkItUserInfo.Add(user);
                    isFirstStudentLogon = true;
                }
                else if (linkItUserInfo.FirstOrDefault().RoleId != (int)RoleEnum.Student)
                {
                    warningMessage = "Your email address has been used by another user. Please contact your system administrator.";
                    return RedirectSSOWarningForm(warningMessage, loginType);
                }
            }

            var linkItUser = linkItUserInfo.Count == 1
                ? linkItUserInfo.First()
                : userService.GetLinkitUserFromMapping(emailAddress, districtID, providerType, roleIds);

            if (string.IsNullOrEmpty(linkItUser.UserName))
            {
                return RedirectSSOLinkUserForm(providerType, emailAddress, loginType);
            }

            if (linkItUser.UserStatusId != (int)UserStatus.Active)
            {
                warningMessage = "Your email address has been suspended. Please contact your system administrator.";
                return RedirectSSOWarningForm(warningMessage, loginType);
            }

            var linkItLoginResult = ProcessLogon(new LoginAccountViewModel
            {
                UserName = linkItUser.UserName,
                Password = "Password@9999",
                District = districtID,
                IsFirstStudentLogonSSO = isFirstStudentLogon,
                SSOInformationId = ssoInformationID,
                SSOType = providerType,
                RoleId = linkItUser.RoleId,
                RedirectUrl = TempData.ContainsKey("ReturnUrl") ? TempData["ReturnUrl"].ToString() : ""
            }, true);

            warningMessage = $"Code: {providerType}_503. An unknown error has occurred. Please contact your system administrator.";

            if (linkItLoginResult == null || linkItLoginResult.Data == null)
            {
                return RedirectSSOWarningForm(warningMessage, loginType);
            }

            var data = linkItLoginResult.Data as LoginAccountViewModel;

            if (data == null || !data.IsAuthenticated)
            {
                return RedirectSSOWarningForm(warningMessage, loginType);
            }

            return Redirect(data.RedirectUrl);
        }

        private ActionResult RedirectSSOWarningForm(string warningMessage, string loginType)
        {
            ViewBag.IsStudent = loginType == TextConstants.LOGIN_ROLE_STUDENT;
            ViewBag.IsParent = loginType == TextConstants.LOGIN_ROLE_PARENT;
            ViewBag.Message = warningMessage;
            return View("SimpleSSOError");
        }

        private ActionResult RedirectSSOLinkUserForm(string providerType, string emailAddress, string loginType)
        {
            var isStudent = loginType == TextConstants.LOGIN_ROLE_STUDENT;
            var isParent = loginType == TextConstants.LOGIN_ROLE_PARENT;

            var districtID = HelperExtensions.GetDistrictIdBySubdomain();
            var token = AllowUserLinkSSOAccount(emailAddress, districtID, isStudent, isParent);

            ViewBag.Email = emailAddress;
            ViewBag.IsStudent = loginType == TextConstants.LOGIN_ROLE_STUDENT;
            if (TempData.ContainsKey("ReturnUrl"))
            {
                TempData.Keep("ReturnUrl");
            }

            var linkUserSSOView = string.Empty;

            switch (providerType)
            {
                case SSOProvider.MICROSOFT:
                    linkUserSSOView = "MicrosoftLinkUser";
                    break;
                case SSOProvider.NYC:
                    linkUserSSOView = "NYCLinkUser";
                    break;
                case SSOProvider.GOOGLE:
                    linkUserSSOView = "GoogleLinkUser";
                    break;

                default:
                    linkUserSSOView = string.Empty;
                    break;
            }

            return View(linkUserSSOView, (object)token);
        }
    }
}
