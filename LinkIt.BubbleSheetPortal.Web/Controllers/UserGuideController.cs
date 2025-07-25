using System;
using System.Configuration;
using System.Net.Mail;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.UserGuide;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels.UserGuide;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class UserGuideController : BaseController
    {
        private readonly UserGuideService _userGuideService;
        private readonly EmailService _emailService;
        private readonly UserService _userService;

        public UserGuideController(UserGuideService userGuideService, EmailService emailService, UserService userService)
        {
            _userGuideService = userGuideService;
            _emailService = emailService;
            _userService = userService;
        }

        [HttpGet]
        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.HelpGuide)]
        public ActionResult Index()
        {
            var lastLoginLog = _userGuideService.GetLastLoginLog(CurrentUser.Id);
            var user = _userService.GetUserById(CurrentUser.Id);
            var email = user == null ? string.Empty : user.EmailAddress;
            if (string.IsNullOrWhiteSpace(email)) return View("EmptyEmail");

            if (lastLoginLog != null
               && string.Compare(lastLoginLog.Email, email, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return RedirectToFreshDesk();
            }

            var currentSecurityCode = _userGuideService.GetCurrentSecurityCode(CurrentUser.Id, email);
            if (currentSecurityCode == null)
            {
                currentSecurityCode = _userGuideService.IssueSecurityCode(CurrentUser.Id, email);
                SendSecurityCodeToUser(currentSecurityCode);
            }

            var model = new UserGuideModel
            {
                SecurityCode = new UserSecurityCodeData(),
                Message = "WE HAVE SENT A SECURITY CODE TO YOUR EMAIL ADDRESS FOR VERIFICATION PURPOSES. PLEASE CHECK YOUR EMAIL AND ENTER THE SECURITY CODE BELOW. THE SECURITY CODE WILL EXPIRE IN FIVE MINUTES."
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult VerifySecurityCode()
        {
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult IssueSecurityCode()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult VerifySecurityCode(UserGuideModel model)
        {
            if (model == null || model.SecurityCode == null)
            {
                model = new UserGuideModel { SecurityCode = new UserSecurityCodeData() };
                return View("Index", model);
            }

            var user = _userService.GetUserById(CurrentUser.Id);
            var email = user == null ? string.Empty : user.EmailAddress;
            var result = _userGuideService.VerifySecurityCode(CurrentUser.Id, email,
                model.SecurityCode.Code);
            if (result)
            {
                
                return RedirectToFreshDesk();
            }

            model.IsError = true;
            model.Message = "THE SECURITY CODE YOU ENTERED IS INVALID. PLEASE ENSURE THAT YOU USED THE CORRECT CODE. CODES ARE CASE SENSITIVE AND EXPIRE AFTER FIVE MINUTES.";
            model.SecurityCode.Code = string.Empty;

            return View("Index", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IssueSecurityCode(UserGuideModel model)
        {
            if (model.SecurityCode != null) model.SecurityCode.Code = string.Empty;

            var user = _userService.GetUserById(CurrentUser.Id);
            var email = user == null ? string.Empty : user.EmailAddress;
            var currentSecurityCode = _userGuideService.IssueSecurityCode(CurrentUser.Id, email);
            SendSecurityCodeToUser(currentSecurityCode);

            model.Message = "WE HAVE SENT A SECURITY CODE TO YOUR EMAIL ADDRESS FOR VERIFICATION PURPOSES. PLEASE CHECK YOUR EMAIL AND ENTER THE SECURITY CODE BELOW. THE SECURITY CODE WILL EXPIRE IN FIVE MINUTES.";

            return View("Index", model);
        }

        private ActionResult RedirectToFreshDesk()
        {
            var url = HelperExtensions.BuildUserGuide(CurrentUser);

            if (string.IsNullOrWhiteSpace(url)) RedirectToAction("UserGuide", "Help");

            var user = _userService.GetUserById(CurrentUser.Id);
            var email = user == null ? string.Empty : user.EmailAddress;
            _userGuideService.LogRedirectFreshdesk(CurrentUser.Id, email);

            return Redirect(url);
        }

        private void SendSecurityCodeToUser(UserSecurityCodeData data)
        {
            if (data == null) return;

            var model = new SecurityCodeMailModel
            {
                SecurityCode = data,
                FirstName = CurrentUser.FirstName,
                LastName = CurrentUser.LastName
            };

            var content = this.RenderRazorViewToString("SecurityCodeTemplate", model);
            var emailCredentialSetting = LinkitConfigurationManager.GetEmailCredentialSetting(EmailSetting.LinkItUseEmailCredentialKey);
            var mailClient = _emailService.SetupMailClient(emailCredentialSetting);
            var user = _userService.GetUserById(CurrentUser.Id);
            var toEmail = user == null ? string.Empty : user.EmailAddress;
            var message = new MailMessage(EmailSetting.LinkItFromEmail, toEmail)
            {
                Subject = "Linkit! Security code",
                IsBodyHtml = true,
                Body = content
            };

            mailClient.Send(message);
        }
    }
}
