using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Properties;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using RestSharp.Extensions;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NotFound()
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            if (!LinkitConfigurationManager.CanAccessVault)
            {
                return View("DistrictNotFound");
            }

            if (HttpContext != null && HttpContext.User.Identity.IsAuthenticated)
            {
                var user = (UserPrincipal)HttpContext.User.Identity;
                ViewBag.IsStudent = user.RoleId == (int)RoleEnum.Student;
            }

            return View("NotFound");
        }

        public ActionResult Index()
        {
            return View("Error");
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult ReportError(ReportErrorViewModel postedData)
        {
            var isRecaptchaValid = IsRecaptchaValid(postedData.RecaptchaResponse);
            if (!isRecaptchaValid)
            {
                ModelState.AddModelError("invalidCaptcha", "Can not verify captcha. Please reload page");
                postedData.IsReceived = false;
                postedData.Message = "Can not verify captcha. Please reload page";
                return Json(postedData);
            }

            string body = string.Empty;
            if (HttpContext != null && HttpContext.User.Identity.IsAuthenticated)
            {
                var user = (UserPrincipal)HttpContext.User.Identity;
                body = string.Format("From: {0} ({1}, {2})", user.Name, user.UserName, user.Id);
                
                if (string.IsNullOrWhiteSpace(postedData.Sender) && user.EmailAddress.HasValue())
                {
                    body += "<br />" + "Email: " + user.EmailAddress + "<br />" + postedData.Description;
                }
                else
                {
                    body += "<br />" + "Email: " + postedData.Sender + "<br />" + postedData.Description;
                }
            }
            else
            {
                body = string.Format(Settings.Default.Body, postedData.Sender, postedData.Description);
            }
            var message = new MailMessage(Settings.Default.FromEmail, Settings.Default.ToEmail)
            {
                Subject = string.Format(Settings.Default.Subject, DateTime.UtcNow.ToLongTimeString()),
                Body = body,
                IsBodyHtml = true
            };

            // pending review
            var smtpClient = new SmtpClient(Settings.Default.SmtpServer, Settings.Default.SmtpPort);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            if (Settings.Default.UseCredential)
            {
                smtpClient.Credentials = new NetworkCredential(Settings.Default.SmtpUsername, Settings.Default.SmtpPassword);
            }

            smtpClient.EnableSsl = true;
            smtpClient.Send(message);

            postedData.IsReceived = true;
            postedData.Message = "Your message has been sent, thanks.";
            return Json(postedData);
        }

        public ActionResult StudentLogonError()
        {
            return View("StudentLogonError");
        }

        private bool IsRecaptchaValid(string recaptchaResponse)
        {
            if (string.IsNullOrEmpty(recaptchaResponse))
            {
                return false;
            }

            //secret that was generated in key value pair
            string secret = ConfigurationManager.AppSettings["RECAPTCHA_SECRETKEY"];
            string googleCaptchaUrl = ConfigurationManager.AppSettings["RECAPTCHA_URL"];
            var verifyResponse = CaptchaHelper.Verify(secret, googleCaptchaUrl, recaptchaResponse);

            return verifyResponse.Success;
        }
    }
}
