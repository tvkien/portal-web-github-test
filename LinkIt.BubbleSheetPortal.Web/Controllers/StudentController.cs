using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.SessionState;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using LinkIt.BubbleSheetPortal.Models.DTOs.MfaSettings;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.MfaServices;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.SSO;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    [VersionFilter]
    public class StudentController : BaseController
    {
        private readonly UserService _userService;
        private readonly IFormsAuthenticationService _formsAuthenticationService;
        private readonly DistrictService _districtService;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly ConfigurationService _configurationService;
        private readonly StudentService _studentService;
        private readonly StudentMetaService _studentMetaService;
        private readonly EmailService _emailService;
        private readonly PasswordResetQuestionService _passwordResetQuestionService;
        private readonly UserLogonService _userLogonService;
        private readonly MfaService _mfaService;

        private const string StudentDefaultPassword = "StudentDefaultPassword";

        public StudentController(UserService userService,
            IFormsAuthenticationService formsAuthenticationService,
            DistrictService districtService,
            DistrictDecodeService districtDecodeService,
            ConfigurationService configurationService,
            StudentService studentService,
            StudentMetaService studentMetaService,
            EmailService emailService,
            PasswordResetQuestionService passwordResetQuestionService,
            UserLogonService userLogonService,
            MfaService mfaService)
        {
            _userService = userService;
            _formsAuthenticationService = formsAuthenticationService;
            _districtService = districtService;
            _districtDecodeService = districtDecodeService;
            _configurationService = configurationService;
            _studentService = studentService;
            _studentMetaService = studentMetaService;
            _emailService = emailService;
            _passwordResetQuestionService = passwordResetQuestionService;
            _userLogonService = userLogonService;
            _mfaService = mfaService;
        }

        private const string TempPasswordExpired = "TempPasswordExpired";

        public ActionResult Index(bool? hasTemporaryPassword, string returnUrl = "/")
        {
            Response.Headers.Add("X-Frame-Options", "DENY");
            bool isStudentLogin = ControllerContext.RouteData.Values["routeName"] != null && ControllerContext.RouteData.Values["routeName"] == "Parent" ? false : true;

            ViewBag.Title = isStudentLogin ? "LinkIt! Student Portal" : "LinkIt! Parent Portal";
            ViewBag.PortalPageType = isStudentLogin ? "Student" : "Parent";
            var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            var districtId = _districtService.GetLiCodeBySubDomain(subDomain);

            if (districtId > 0)
            {
                var model = BuildLoginModel(districtId, hasTemporaryPassword, isStudentLogin, returnUrl);

                //Check Chromebook kiosk mode application
                model.IsRequireKioskMode = _districtDecodeService.StudentLogonRequireKioskMode(districtId);

                return View(model);
            }

            throw new HttpException(404, "NotFound");
        }

        private StudentLoginViewModel BuildLoginModel(int districtId, bool? hasTemporaryPassword, bool isStudentLogin, string returnUrl)
        {
            var studentLogin = new StudentLoginViewModel()
            {
                DistrictId = districtId,
                ShowAnnouncement = _configurationService.GetShowAnnouncement(),
                AnnouncementText = _configurationService.GetAnnouncementText(),
                HasTemporaryPassword = hasTemporaryPassword.GetValueOrDefault(),
                RedirectUrl = returnUrl,
                EnableLoginByGoogle = _districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.SHOW_GOOGLE_LOGIN_BUTTON),
                EnableLoginByMicrosoft = _districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.SHOW_MICROSOFT_LOGIN_BUTTON),
                EnableLoginByClever = _districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.SHOW_CLEVER_LOGIN_BUTTON),
                HideLoginCredentials = _districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.HIDE_LOGIN_CREDENTIALS),
                IsStudentLogin = isStudentLogin,
                EnableLoginByNYC = _districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.SHOW_NYC_LOGIN_BUTTON),
                EnableLoginByClassLink = _districtDecodeService.GetDistrictDecodeByLabel(districtId, Constanst.SHOW_CLASSLINK_LOGIN_BUTTON),
            };

            using (var client = new HttpClient())
            {
                var url = string.Format("{0}{1}/{2}", LinkitConfigurationManager.GetS3Settings().S3CSSKey, districtId, "LogOn.html");
                try
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotModified)
                    {
                        studentLogin.LogOnHeaderHtmlContent = response.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception)
                {

                }
            }

            return studentLogin;
        }
        public ActionResult LogOn(bool? hasTemporaryPassword, string returnUrl = "/")
        {
            Response.Headers.Add("X-Frame-Options", "DENY");
            bool isStudentLogin = ControllerContext.RouteData.Values["routeName"] != null && ControllerContext.RouteData.Values["routeName"] == "Parent" ? false : true;

            ViewBag.Title = isStudentLogin ? "LinkIt! Student Portal" : "LinkIt! Parent Portal";
            var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            var districtId = _districtService.GetLiCodeBySubDomain(subDomain);

            if (districtId > 0)
            {
                var model = BuildLoginModel(districtId, hasTemporaryPassword, isStudentLogin, returnUrl);

                //Check Chromebook kiosk mode application
                model.IsRequireKioskMode = _districtDecodeService.StudentLogonRequireKioskMode(districtId);

                return View("Index", model);
            }

            throw new HttpException(404, "NotFound");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(StudentLoginViewModel model, bool isFromRegistrationCode = false)
        {
            HttpContext.Session.Remove("BeforeLoginSession");
            HttpContext.Session.Remove("StudentBeforeLoginSession");

            model.Type = "error";
            model.Message = "Your username and password do not match our records.";
            model.IsAuthenticated = false;
            if (SessionManager.ShowCaptcha)
            {
                var recaptcharResponse = this.Request.Form["g_recaptcha_response"];//modify g-recaptcha-response to g_recaptcha_response
                if (string.IsNullOrEmpty(recaptcharResponse))
                {
                    return Json(new { Message = "You must satisfy the CAPTCHA verification requirements to indicate that you are an authorized user.", IsAuthenticated = false, Type = "error", ShowCaptcha = true });
                }
                //secret that was generated in key value pair
                string secret = ConfigurationManager.AppSettings["RECAPTCHA_SECRETKEY"];
                string googleCaptchaUrl = ConfigurationManager.AppSettings["RECAPTCHA_URL"];
                var verifyResponse = CaptchaHelper.Verify(secret, googleCaptchaUrl, recaptcharResponse);
                if (!verifyResponse.Success)
                {
                    return Json(new { Message = verifyResponse.ErrorCodes ?? new List<string> { "Can not verify captcha. Please reload page" }, IsAuthenticated = false, Type = "error", ShowCaptcha = true });
                }
            }
            User user = null;
            var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            model.DistrictId = _districtService.GetLiCodeBySubDomain(subDomain);
            if (isFromRegistrationCode)
            {
                user = _userService.GetUserByRegistrationCode(model.RCode);
            }
            else
            {
                var roleName = model.IsStudentLogin ? (int)Permissions.Student : (int)Permissions.Parent;
                user = _userService.GetStudentByUserName(model.UserName, model.DistrictId, roleName);
            }

            MfaSettingDto mfaSetting = null;
            if (user != null)
            {
                mfaSetting = AuthenticateUser(model, user, isFromRegistrationCode);
            }

            model = CheckShowCaptchar(model);

            if (model.IsAuthenticated)
            {
                if (user != null && (model.HasTemporaryPassword || isFromRegistrationCode))
                {
                    // clear cookies
                    foreach (var item in Response.Cookies.AllKeys)
                    {
                        if (item.ToString() != Constanst.ASPNETSessionId)
                            Response.Cookies.Remove(item.ToString());
                    }

                    LoginAccountViewModel accModel = MappingToLoginAccountViewModel(model);

                    // save info to session
                    HttpContext.Session["BeforeLoginSession"] = new Tuple<LoginAccountViewModel, int, int>(accModel,
                        user.Id, user.RoleId);
                    HttpContext.Session["StudentBeforeLoginSession"] = accModel;

                    // redirect to change question
                    model.RedirectUrl = string.Format("{0}?id={1}",
                        new UrlHelper(Request.RequestContext).Action("SetAccountInformation", "Account"),
                        user.Id);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(model.RedirectUrl) || model.RedirectUrl == "/")
                    {
                        if (model.IsStudentLogin)
                        {
                            var menuAccess = HelperExtensions.GetMenuForDistrict(user);
                            if (menuAccess.IsDisplayOnlineTesting)
                            {
                                var url = ConfigurationManager.AppSettings["LinkItUrl"];
                                model.RedirectUrl = string.Format("{0}://{1}.{2}/{3}", HelperExtensions.GetHTTPProtocal(Request), subDomain, url, "StudentOnlineTesting");
                            }
                        }
                        else
                        {
                            model.RedirectUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                        }
                    }

                    if (mfaSetting?.IsEnableMfa == true)
                    {
                        LoginAccountViewModel accModel = MappingToLoginAccountViewModel(model);

                        Session["LoginAccountViewModel"] = accModel;
                        Session["MfaSetting"] = mfaSetting;
                        model.RedirectUrl = new UrlHelper(Request.RequestContext).Action("MfaVerification", "Account");
                    }
                }
            }

            return Json(model);
        }

        private LoginAccountViewModel MappingToLoginAccountViewModel(StudentLoginViewModel model)
        {
            LoginAccountViewModel accModel = new LoginAccountViewModel
            {
                AnnouncementText = model.AnnouncementText,
                District = model.DistrictId,
                HasEmailAddress = model.HasEmailAddress,
                HasSecurityQuestion = model.HasSecurityQuestion,
                HasTemporaryPassword = model.HasTemporaryPassword,
                IsAuthenticated = model.IsAuthenticated,
                IsKeepLoggedIn = model.KeepLogged,
                IsNetworkAdmin = false,
                IsShowWarningLogOnUser = false,
                LogOnHeaderHtmlContent = model.LogOnHeaderHtmlContent,
                Message = model.Message,
                MessageWarningLogOnUser = string.Empty,
                Password = model.Password,
                RedirectUrl = model.RedirectUrl,
                ShowAnnouncement = model.ShowAnnouncement,
                ShowCaptcha = model.ShowCaptcha,
                Type = model.Type,
                UrlRecomment = string.Empty,
                UserID = model.UserID,
                UserName = model.UserName
            };

            return accModel;
        }

        private StudentLoginViewModel CheckShowCaptchar(StudentLoginViewModel model)
        {
            if (!model.IsAuthenticated)
            {
                //display reCAPTCHA
                model.ShowCaptcha = false;
                model.IsAuthenticated = false;
                SessionManager.LoginCount++;
                if (SessionManager.LoginCount >
                    _configurationService.GetConfigurationByKeyWithDefaultValue(Constanst.LoginLimit, Constanst.LoginLimitDefault))
                {
                    //display recaptcha
                    model.ShowCaptcha = true;
                    SessionManager.ShowCaptcha = true;
                }
            }
            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInfo(StudentUpdateInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var district = _districtService.GetDistrictById(model.DistrictId);
                var newUser = new User()
                {
                    DistrictId = district.Id,
                    UserName = model.UserName,
                    LocalCode = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Name = model.FirstName + " " + model.LastName,
                    EmailAddress = model.Email,
                    RoleId = (int)Permissions.Student,
                    Password = model.Password,
                    HashedPassword = Crypto.HashPassword(model.Password),
                    HasTemporaryPassword = false,
                    UserStatusId = (int)UserStatus.Active,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    StateId = district.StateId,
                    PasswordQuestion = model.Question,
                    PasswordAnswer = Crypto.HashPassword(model.Answer),
                    LastLoginDate = DateTime.UtcNow
                };

                _userService.SaveUser(newUser);

                var studentMeta = InitStudentMeta(model.StudentId, Util.UserId, newUser.Id.ToString());
                _studentMetaService.Save(studentMeta);

                newUser.SessionCookieGUID = Guid.NewGuid().ToString();
                newUser.GUIDSession = Guid.NewGuid().ToString();
                _formsAuthenticationService.SignIn(newUser, model.KeepLogged);
                var userLogon = new UserLogon()
                {
                    UserID = newUser.Id,
                    GUIDSession = newUser.GUIDSession
                };
                _userLogonService.Save(userLogon);
                return Redirect(model.RedirectUrl);
            }

            model.Questions =
                new List<SelectListItem>(
                    _passwordResetQuestionService.GetPasswordResetQuestions((int)Permissions.Student)
                        .Select(x => new SelectListItem { Text = x.Question, Value = x.Question }));

            return View(model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ResetPassword(StudentLoginViewModel model)
        {
            var user = model.IsStudentLogin ? _userService.GetStudentByUserName(model.UserName, model.DistrictId)
                                            : _userService.GetParentActiveByUserName(model.UserName, model.DistrictId);

            var expireMinutes = GetTempPasswordExpired();
            var message = string.Format(TextConstants.PASSWORD_RESET_MESSAGE, expireMinutes);

            if (user != null)
            {
                if (string.IsNullOrEmpty(user.HashedPassword))
                {
                    message = string.Format(TextConstants.ACCOUNT_NO_PASSWORD);
                    return Json(new { type = "success", message }, JsonRequestBehavior.AllowGet);
                }

                SendTokenResetPassword(user, expireMinutes);
                return Json(new { type = "success", message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { type = "success", message }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AjaxOnly]
        public ActionResult RegistrationCode(StudentLoginViewModel model)
        {
            var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            model.DistrictId = _districtService.GetLiCodeBySubDomain(subDomain);

            string errorMessage = string.Empty;
            var student = _userService.RegistrationCode(model.RCode, model.DistrictId, model.IsStudentLogin, false, out errorMessage);
            if (student != null)
            {
                model.FirstName = student.FirstName;
                model.LastName = student.LastName;
                model.StudentId = student.Id;

                return View("RegistrationCode", model);
            }

            return Json(new { message = errorMessage, type = "error" });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RegistrationCodeStep2(StudentLoginViewModel model)
        {
            var subDomain = Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
            model.DistrictId = _districtService.GetLiCodeBySubDomain(subDomain);

            string errorMessage = string.Empty;
            var hasGenerateLogin = _districtDecodeService.GetDistrictDecodeOrConfigurationByLabel(model.DistrictId, Constanst.ALLOW_STUDENT_USER_GENERATION);
            var student = _userService.RegistrationCode(model.RCode, model.DistrictId, model.IsStudentLogin, hasGenerateLogin, out errorMessage);
            if (student != null)
            {
                model.FirstName = student.FirstName;
                model.LastName = student.LastName;
                model.StudentId = student.Id;
                model.UserName = student.UserName;
                model.NoAccount = string.IsNullOrEmpty(student.UserName) || !hasGenerateLogin;
                
                return View("RegistrationCodeStep2", model);
            }

            return Json(new { message = errorMessage, type = "error" });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RegistrationCodeStep3(StudentLoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserName))
                return Json(new { message = "Username is required.", type = "error" });
            if (string.IsNullOrEmpty(model.Password))
                return Json(new { message = "Password is required.", type = "error" });
            if (model.Password != model.CPassword)
                return Json(new { message = "Passwords must match.", type = "error" });
            if (!new Regex(ConfigurationManager.AppSettings["PasswordRegex"]).Match(model.Password).Success)
                return Json(new { message = ConfigurationManager.AppSettings["PasswordRequirements"], type = "error" });

            string errorMessage = string.Empty;
            if (model.IsStudentLogin)
            {
                Student student = null;
                var user = _userService.RegistrationCodeCreateUser(model.RCode, model.UserName, model.Password, out student, out errorMessage);
                if (user != null)
                {
                    model.UserID = user.Id;
                    model.UserName = user.UserName;
                    model.Password = user.Password;
                    model.StudentId = student.Id;

                    return LogOn(model, true);
                }
            }
            else
            {
                var parentModel = new ParentInformationDto()
                {
                    DistrictId = model.DistrictId,
                    RegistrationCode = model.RCode,
                    UserId = model.UserID,
                    Password = model.Password,
                    UserName = model.UserName
                };

                var user = _userService.RegistrationCodeParentUser(parentModel, out errorMessage);
                if (user != null)
                {
                    model.UserID = user.Id;
                    model.UserName = user.UserName;
                    model.Password = user.Password;

                    return LogOn(model);
                }
            }


            return Json(new { message = errorMessage, type = "error" });
        }

        private ActionResult ResetUsersPassword(StudentLoginViewModel model, User user)
        {
            var accountModel = new ResetPasswordViewModel { UserName = model.UserName };
            if (string.IsNullOrEmpty(user.PasswordQuestion) && string.IsNullOrEmpty(user.EmailAddress))
            {
                return Json(new
                {
                    message = user.UserName + " does not have a password question set or a valid email address. " +
                              "Please contact your school or district administrator for a temporary password.",
                    type = "error"
                });
            }

            accountModel.PasswordQuestion = user.PasswordQuestion;
            accountModel.UserID = user.Id;
            accountModel.HasEmailAddress = !user.EmailAddress.Equals(string.Empty);
            accountModel.HasSecurityQuestion = user.PasswordQuestion.IsNotNull() && !user.PasswordQuestion.Equals(string.Empty);
            accountModel.IsStudentLogin = model.IsStudentLogin;

            return View("ResetPassword", accountModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitPasswordReset(ResetPasswordViewModel model)
        {
            if (IsValidPasswordRequest(model))
            {
                model.NewPassword = model.NewPassword.Trim();
                model.ConfirmNewPassword = model.ConfirmNewPassword.Trim();
                var user = GetByUserName(model.UserName);
                if (user.IsNotNull())
                {
                    if (_userService.IsCorrectPasswordAnswer(user, model.PasswordAnswer))
                    {
                        _userService.ResetUsersPassword(user, model.NewPassword);
                        model.PasswordHasBeenReset = true;

                        return View("ResetPassword", model);
                    }
                }

                model.SubmittedCorrectPasswordAnswer = true;
                ModelState.AddModelError("invalidUser", "Invalid User.  Please try again.");

                return View("ResetPassword", model);
            }

            model.SubmittedCorrectPasswordAnswer = true;

            return View("ResetPassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitPasswordAnswer(ResetPasswordViewModel model)
        {
            var user = GetByUserName(model.UserName);

            if (model.PasswordAnswer.IsNull())
            {
                model.PasswordQuestion = user.PasswordQuestion;
                ModelState.AddModelError("passwordAnswer", "Please input a security answer.");
                return View("ResetPassword", model);

            }


            if (user.IsNotNull())
            {
                model.HasSecurityQuestion = true;
                model.HasEmailAddress = !user.EmailAddress.Equals(string.Empty);

                ViewBag.IsSubmitPasswordAnswer = true;
                if (_userService.IsCorrectPasswordAnswer(user, model.PasswordAnswer))
                {
                    model.SubmittedCorrectPasswordAnswer = true;
                    return View("ResetPassword", model);
                }

                model.PasswordQuestion = user.PasswordQuestion;
                ModelState.AddModelError("passwordAnswer", "Supplied answer is incorrect.  Please try again.");

                return View("ResetPassword", model);
            }

            model.PasswordQuestion = user.PasswordQuestion;
            ModelState.AddModelError("invalidUser", "Invalid User.  Please try again.");

            return View("ResetPassword", model);
        }

        private string GetRandomPasswordResetQuestion()
        {
            return
                _passwordResetQuestionService.GetPasswordResetQuestions((int)Permissions.Student)
                    .ToList()
                    .OrderBy(x => Guid.NewGuid())
                    .First()
                    .Question;
        }


        public ActionResult SendTemporaryPassword(string username)
        {
            var districtID = HelperExtensions.GetDistrictIdBySubdomain();
            var user = _userService.GetUserByUsernameAndDistrict(username, districtID);
            if (user.IsNull())
            {
                return Json(new { message = "User does not exist." });
            }

            var expiredHours = _configurationService.GetConfigurationByKey(TempPasswordExpired);
            var minutes = 60;
            if (expiredHours != null)
            {
                int.TryParse(expiredHours.Value, out minutes);
            }

            var token = Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ApproveChangePassword?token=" + Util.GenerateToken(user.UserName, user.DistrictId.GetValueOrDefault(), minutes);
            var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.LinkItUseEmailCredentialKey);
            _emailService.SendTokenResetPassword(user, token, emailCredentialSetting);

            return Json(new { message = "", type = "success" }, JsonRequestBehavior.AllowGet);

        }

        private MfaSettingDto AuthenticateUser(StudentLoginViewModel model, User student, bool isFromRegistrationCode = false)
        {
            if (!string.IsNullOrEmpty(model.Password))
                model.Password = model.Password.Trim();
            if (!_userService.IsValidUser(student, model.Password)) return null;

            if (!student.UserStatusId.Equals((int)UserStatus.Active))
            {
                model.Message = "Your username and password do not match our records.";
                return null;
            }

            var passwordResetYears = 0;
            passwordResetYears = int.TryParse(ConfigurationManager.AppSettings["PasswordResetTime"], out passwordResetYears) ? passwordResetYears : 5;
            model.HasTemporaryPassword = student.HasTemporaryPassword
                || student.LastLoginDate < DateTime.MinValue.AddYears(1990)
                || student.LastLoginDate < DateTime.UtcNow.AddYears(passwordResetYears * -1)
                || (!_studentService.HasStudentSecret(student.Id) && !Regex.IsMatch(model.Password, ConfigurationManager.AppSettings["PasswordRegex"]));

            if (!isFromRegistrationCode && !model.HasTemporaryPassword)
            {
                var cognitoCredentialSetting = LinkitConfigurationManager.GetLinkitSettings().CognitoCredentialSetting;
                var mfaSetting = _mfaService.CheckFlowMfa(cognitoCredentialSetting, student);
                if (mfaSetting.IsEnableMfa)
                {
                    model.IsAuthenticated = true;
                    model.UserID = student.Id;
                    return mfaSetting;
                }
            }

            student.SessionCookieGUID = Guid.NewGuid().ToString();
            student.GUIDSession = Guid.NewGuid().ToString();
            _formsAuthenticationService.SignIn(student, model.KeepLogged);
            var userLogon = new UserLogon()
            {
                UserID = student.Id,
                GUIDSession = student.GUIDSession
            };
            _userLogonService.Save(userLogon);

            if (!isFromRegistrationCode)
                _userService.UpdateLastLogin(student.Id);

            model.IsAuthenticated = true;
            model.UserID = student.Id;
            return null;
        }

        private StudentMeta InitStudentMeta(int studentId, string name, string data)
        {
            var studentMeta = new StudentMeta()
            {
                StudentID = studentId,
                Name = name,
                Data = data
            };

            return studentMeta;
        }

        private bool IsValidPasswordRequest(ResetPasswordViewModel model)
        {
            bool result = true;

            if (string.IsNullOrEmpty(model.NewPassword))
            {
                result = false;
                ModelState.AddModelError("passwords", "Please input new password.");
            }
            else if (!Regex.IsMatch(model.NewPassword, ConfigurationManager.AppSettings["PasswordRegex"]))
            {
                result = false;
                ModelState.AddModelError("passwords", ConfigurationManager.AppSettings["PasswordRequirements"]);
            }
            else if (!model.NewPassword.Equals(model.ConfirmNewPassword))
            {
                result = false;
                ModelState.AddModelError("passwords", "Passwords do not match.  Please try again.");
            }

            return result;
        }

        private bool CheckLastNameAndPassword(string lastName, string password)
        {
            var configurationCharacters = _configurationService.GetConfigurationByKey("SpecialCharacters");
            var configurationCharactersMap = _configurationService.GetConfigurationByKey("SpecialCharactersMap");

            string specialCharacters = "", specialCharactersMap = "";
            if (configurationCharacters != null)
            {
                specialCharacters = configurationCharacters.Value;
            }

            if (configurationCharactersMap != null)
            {
                specialCharactersMap = configurationCharactersMap.Value;
            }

            // Convert to lower case
            lastName = lastName.ToLower();
            password = password.ToLower();

            // Change any accented characters
            lastName = ReplaceSpecialCharacters(lastName, specialCharacters, specialCharactersMap);
            password = ReplaceSpecialCharacters(password, specialCharacters, specialCharactersMap);

            // Remove any characters outside of [a-z] range
            lastName = RemoveSpecialCharacters(lastName);
            password = RemoveSpecialCharacters(password);

            return lastName == password;
        }

        private string ReplaceSpecialCharacters(string input, string specialCharacters, string specialCharactersMap)
        {
            try
            {
                foreach (var s in input)
                {
                    var pos = specialCharacters.IndexOf(s);
                    if (pos > -1)
                    {
                        input = input.Replace(s, specialCharactersMap[pos]);
                    }
                }
            }
            catch (Exception)
            {
            }

            return input;
        }

        private string RemoveSpecialCharacters(string input)
        {
            const string words = "abcdefghijklmnopqrstuvwxyz";
            foreach (var s in input)
            {
                var pos = words.IndexOf(s);
                if (pos == -1)
                {
                    input = input.Replace(s.ToString(), "");
                }
            }

            return input;
        }

        private User GetByUserName(string userName)
        {
            var districtID = HelperExtensions.GetDistrictIdBySubdomain();
            return _userService.GetStudentByUserName(userName, districtID);
        }

        public ActionResult SingleSignOn(string provider , bool isStudentLogin, string returnUrl = "")
        {
            var loginType = isStudentLogin ? TextConstants.LOGIN_ROLE_STUDENT : TextConstants.LOGIN_ROLE_PARENT;
            return RedirectToAction("SingleSignOn", "Account", new { provider = provider, loginType = loginType, returnUrl = returnUrl });
        }

        public ActionResult GoogleCallback(string code, string state)
        {
            return RedirectToAction("GoogleCallback", "Account", new { code = code, state = state, loginType = TextConstants.LOGIN_ROLE_STUDENT });
        }

        public ActionResult MicrosoftCallback(string code, string state)
        {
            return RedirectToAction("MicrosoftCallback", "Account", new { code = code, state = state, loginType = TextConstants.LOGIN_ROLE_STUDENT });
        }

        private void SendTokenResetPassword(User user, int expireMinutes)
        {
            if (user.IsNull() || string.IsNullOrEmpty(user.EmailAddress)) return;

            var token = Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ApproveChangePassword?token=" + Util.GenerateToken(user.UserName, user.DistrictId.GetValueOrDefault(), expireMinutes);
            var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.LinkItUseEmailCredentialKey);
            _emailService.SendTokenResetPassword(user, token, emailCredentialSetting);
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

        public ActionResult CleverCallback(string code, string state)
        {
            string requestUrl = CleverHelper.CreateProcessCallBackUrl(Request.Url.AbsoluteUri, licode: state);
            return Redirect(requestUrl);
        }

        public ActionResult CleverProcessCallback(string code, string state)
        {
            return RedirectToAction("CleverProcessCallBack", "Account", new { code = code, state = state, loginType = TextConstants.LOGIN_ROLE_STUDENT });
        }
    }
}
