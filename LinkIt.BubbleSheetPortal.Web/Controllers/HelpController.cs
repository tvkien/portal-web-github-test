using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Services;
using System.Net;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System.Web.SessionState;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    [VersionFilter]
    public class HelpController : BaseController
    {
        private readonly WhatNewService _downloadPdfService;
        private readonly ConfigurationService _configurationService;
        private readonly XLIMenuPermissionService _xliMenuPermissionService;
        private readonly DistrictDecodeService _districtDecodeService;
        public HelpController(ConfigurationService configurationService, XLIMenuPermissionService xliMenuPermissionService, WhatNewService downloadPdfService, DistrictDecodeService districtDecodeService)
        {
            _configurationService = configurationService;
            _xliMenuPermissionService = xliMenuPermissionService;
            _downloadPdfService = downloadPdfService;
            _districtDecodeService = districtDecodeService;
        }

        ActionResult ErrorDownload()
        {
            return Content("<script language='javascript' type='text/javascript'>alert('File is invalid.');</script>");
        }


        [HttpPost]
        public ActionResult DownloadFile(Guid? id)
        {
            if (id == null)
                return ErrorDownload();

            var pdf = _downloadPdfService.GetDownloadFile(id.Value);

            if (pdf != null && !string.IsNullOrEmpty(pdf.FilePath)
                && (pdf.FilePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || pdf.FilePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                WebClient webClient = new WebClient();
                byte[] documentData = webClient.DownloadData(pdf.FilePath);

                string fileName = string.Empty;

                try
                {
                    fileName = System.IO.Path.GetFileName(pdf.FilePath);
                }
                catch (Exception)
                {
                    fileName = string.Empty;
                }

                if (string.IsNullOrEmpty(fileName))
                    fileName = id.Value.ToString();

                return File(documentData, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
                return ErrorDownload();
        }

        [HttpGet]
        public ActionResult DownloadFile(string id)
        {
            ViewBag.WhatNewLinkDownload = id;
            return View();
        }

        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.HelpIntroduction)]
        public ActionResult Introduction()
        {
            ViewBag.WhatNewHtmlContent = this._configurationService.GetConfigurationByKeyWithDefaultValue("WhatNewHtmlContent", string.Empty);

            return View();
        }


        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.HelpVideotutorials)]
        public ActionResult VideoTutorials()
        {
            return View();
        }


        [AuthorizeDistrict(Order = 4, UrlCode = ContaintUtil.HelpGuide)]
        public ActionResult UserGuide()
        {
            ViewBag.URLReturn = string.Empty;
            var email = CurrentUser.EmailAddress;
            if (!string.IsNullOrWhiteSpace(email))
            {
                string key = Util.ReadValue("SSOKey", "11d5d4e0423feb4fa3c19eb841e86a61");
                const string pathTemplate =
                    "https://linkithelp.freshdesk.com/login/sso?name={0}&email={1}&timestamp={2}&hash={3}";

                var username = CurrentUser.UserName;
                string timems = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
                var hash = GetHash(key, username, email, timems);
                var path = String.Format(pathTemplate, Server.UrlEncode(username), Server.UrlEncode(email), timems,
                    hash);
                ViewBag.URLReturn = path;
            }
            return View();
        }

        public ActionResult GetHelpTextModule(string displayName)
        {          
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                if (displayName == ContaintUtil.Home)
                    displayName = ContaintUtil.HomeItem;

                if (CurrentUser.IsParent)
                {
                    var studentNavigation = (StudentNavigationViewModel)Session["MailBox_StudentNavigationViewModel"];
                    if (studentNavigation != null &&
                        studentNavigation.StudentsOfParent.Any(
                            x => string.Format("{0} {1}", x.StudentFirstName, x.StudentLastName) == displayName))
                        displayName = ContaintUtil.MailBox;
                }

                MenuAccessItems objMenu = (MenuAccessItems)Session["MenuItem"];
                if (objMenu == null)
                {
                    var item = _xliMenuPermissionService.GetXliModule(displayName);
                    if (item != null)
                        return Json(item.HelpText, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var subMenu = objMenu.SubMenuItems.Values.FirstOrDefault(x => x.Label == displayName);
                    if (subMenu != null)
                        return Json(subMenu.HelpText, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        private static string GetHash(string secret, string name, string email, string timems)
        {
            string input = name + email + timems;
            var keybytes = Encoding.Default.GetBytes(secret);
            var inputBytes = Encoding.Default.GetBytes(input);

            var crypto = new HMACMD5(keybytes);
            byte[] hash = crypto.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                string hexValue = b.ToString("X").ToLower(); // Lowercase for compatibility on case-sensitive systems
                sb.Append((hexValue.Length == 1 ? "0" : "") + hexValue);
            }
            return sb.ToString();
        }
    }
}
