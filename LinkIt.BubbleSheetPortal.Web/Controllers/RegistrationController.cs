using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    public class RegistrationController : BaseController
    {
        private readonly RegistrationControllerParameters _parameters;
        public RegistrationController(RegistrationControllerParameters parameters)
        {
            this._parameters = parameters;
        }

        bool GetIsAccessRegistration()
        {
            var subdomain = HelperExtensions.GetSubdomain();
            if (!string.IsNullOrEmpty(subdomain) && subdomain.Contains('.'))
                subdomain = subdomain.Split('.')[0]; // subdomain = LICode now
            var dictrictId = this._parameters.DistrictServices.GetLiCodeBySubDomain(subdomain);
            return this._parameters.DistrictDecodeServices.GetAccessRegistration(dictrictId);
        }

        [HttpGet]
        public ActionResult Index(string email)
        {
            if (!GetIsAccessRegistration())
                return new HttpNotFoundResult();

            var obj = new RegistrationViewModel();
            string strReferrrer = Request.UrlReferrer != null ? Request.UrlReferrer.Host.ToString() : string.Empty; // you can deny server call here.
            string domainAllow = Util.GetConfigByKey("DomainCEE", "*");
            if (!domainAllow.Equals("*"))
            {
                var domainRef = GetDomain(strReferrrer);
                var domainCEE = GetDomain(domainAllow);
                if (!domainRef.Equals(domainCEE))
                {
                    obj.MessageError = "Unauthorized User!";
                    return View(obj);
                }
            }
            obj.GoBackURL = Util.GetConfigByKey("HostCEE", "http://cee.linkitdev.com");
            if (!obj.GoBackURL.StartsWith("http"))
            {
                obj.GoBackURL = string.Format("http://{0}", obj.GoBackURL);
            }

            //TODO: Must check authorize
            int districtId = _parameters.DistrictServices.GetLiCodeBySubDomain("cee");
            var vTermsOfUseContent = _parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, "CEETermsOfUse");
            if (vTermsOfUseContent.Any())
            {
                obj.TermsofUseContent = vTermsOfUseContent.First().Value;
            }
            if (!string.IsNullOrEmpty(email))
            {
                var vUser = _parameters.UserServices.GetUserByUsernameAndDistrict(email, districtId);
                //TODO: Check UserRegister after validate
                if (vUser != null && vUser.UserStatusId == (int)UserStatus.Active)
                {
                    var urlCEELink = Util.GetConfigByKey("HostCEE", "http://cee.linkitdev.com");
                    if (!urlCEELink.StartsWith("http"))
                    {
                        return Redirect(string.Format("http://{0}", urlCEELink));
                    }
                    return Redirect(urlCEELink);
                }
                else
                {
                    obj.Email = email;
                    obj.UserName = email;
                    return View(obj);
                }
            }
            return View(obj);
        }

        [NonAction]
        private APIAccount GetPrivateKey(string accessKey)
        {
            string pathAndQuery = Request.Url.ToString();
            APIAccount apiAccount = _parameters.APIAccountServices.GetAPIAccountByClientAccessKey(accessKey);
            if (apiAccount != null)
            {
                //TODO: valid permission
                List<int> listFunctionId = _parameters.APIPermissionServices.GetAPIPermissionByTaget(apiAccount.TargetID);
                if (_parameters.APIFunctionServices.CheckValidURL(listFunctionId, pathAndQuery))
                    return apiAccount;
            }
            return null;
        }

        [NonAction]
        private CEEAuthorizeViewModel parseCEEAuthorize(string strData, string strPrivateKey)
        {
            string tmp = string.Empty;
            try
            {
                tmp = DecryptString(strData, strPrivateKey);
            }
            catch (Exception exception)
            {
                PortalAuditManager.LogException(exception);
            }
            if (!string.IsNullOrEmpty(tmp))
            {
                try
                {
                    var obj = new JavaScriptSerializer().Deserialize<CEEAuthorizeViewModel>(tmp);
                    return obj;
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                }
            }
            return null;
        }

        [NonAction]
        private string EncryptString(string message, string passphrase)
        {
            byte[] Results;
            var UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            var HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            var TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(message);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(Results);
        }

        [NonAction]
        private string DecryptString(string message, string passphrase)
        {
            byte[] Results;
            var UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            var HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            var TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(message);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }

        [HttpGet]
        public ActionResult BuildCEERegistration()
        {
            var obj = new RegistrationViewModel();
            obj.AccessKey = "902f40b4-d6cc-41fc-b32c-90b298c4e7e3";
            return View(obj);
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public ActionResult GenerateURL(RegistrationViewModel obj)
        {
            var apiAccount = _parameters.APIAccountServices.GetAPIAccountByClientAccessKey(obj.AccessKey);
            if (apiAccount != null)
            {
                var c = new CEEAuthorizeViewModel()
                {
                    Email = obj.Email,
                    Timestamp = DateTime.UtcNow
                };
                if (string.IsNullOrEmpty(obj.LinkitURL))
                {
                    obj.LinkitURL = Util.GetConfigByKey("HostCEE", "http://cee.linkitdev.com");
                }
                string rawData = new JavaScriptSerializer().Serialize(c);
                string strData = EncryptString(rawData, apiAccount.LinkitPrivateKey);
                string strRequest = string.Format("{0}/Registration?q={1}&k={2}", obj.LinkitURL, HttpUtility.UrlEncode(strData), HttpUtility.UrlEncode(obj.AccessKey));
                return Json(new { success = true, data = strRequest }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, data = "Invalid AccessKey" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult RegisterTeacher(RegistrationViewModel model)
        {
            if (!GetIsAccessRegistration())
                return new HttpNotFoundResult();

            model.SetValidator(_parameters.RegistrationViewModelValidator);
            if (!model.IsValid)
            {
                return Json(new { Success = false, ErrorList = model.ValidationErrors });
            }
            int districtId = _parameters.DistrictServices.GetLiCodeBySubDomain("CEE");
            var vDistrict = _parameters.DistrictServices.GetDistrictById(districtId);
            if (districtId <= 0 || vDistrict == null)
            {
                return ShowJsonResultException(model, "Not exist district CEE");
            }

            var vUserEmail = _parameters.UserServices.GetUserByEmailAndDistrict(model.Email, districtId);
            if (vUserEmail != null)
            {
                if (vUserEmail.UserStatusId == (int)UserStatus.Active)
                {
                    return ShowJsonResultException(model, "THIS EMAIL ADDRESS IS CURRENTLY REGISTERED");
                }
                else
                {
                    vUserEmail.UserStatusId = (int)UserStatus.Active;
                    vUserEmail.FirstName = model.FirstName;
                    vUserEmail.LastName = model.LastName;
                    vUserEmail.HashedPassword = Crypto.HashPassword(model.Password);
                    vUserEmail.HasTemporaryPassword = false;
                    vUserEmail.PasswordLastSetDate = DateTime.UtcNow.Date;
                    vUserEmail.StateId = vDistrict.StateId;
                    vUserEmail.ModifiedDate = DateTime.UtcNow;
                    _parameters.UserServices.SaveUser(vUserEmail);

                    SendEmailRegister(vUserEmail);
                    var userActive = _parameters.UserServices.GetUserById(vUserEmail.Id);

                    _parameters.FormsAuthenticationService.SignOut();
                    _parameters.FormsAuthenticationService.SignIn(userActive, false);

                    return Json(new { userId = userActive.Id, Success = true }, JsonRequestBehavior.AllowGet);
                }
            }

            var tmpUser = _parameters.UserServices.GetUserByUsernameAndDistrict(model.UserName, districtId);
            if (tmpUser != null)
            {
                return Redirect(Util.GetConfigByKey("HostCEE", "http://cee.linkitdev.com"));
            }
            var vUser = new User()
            {
                DistrictId = districtId,
                UserName = model.Email.Trim(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.Email.Trim(),
                RoleId = (int)Permissions.Teacher,
                Password = model.Password,
                HashedPassword = Crypto.HashPassword(model.Password),
                HasTemporaryPassword = false,
                UserStatusId = (int)UserStatus.Active, //Active,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                StateId = vDistrict.StateId,
                LastLoginDate = DateTime.UtcNow
            };
            _parameters.UserServices.SaveUser(vUser);

            SendEmailRegister(vUser);
            var user = _parameters.UserServices.GetUserById(vUser.Id);

            _parameters.FormsAuthenticationService.SignOut();
            _parameters.FormsAuthenticationService.SignIn(user, false);

            var redirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), vDistrict.LICode.ToLower(), vUser);

            return Json(new { data = redirectUrl, userId = user.Id, Success = true }, JsonRequestBehavior.AllowGet);

        }

        private void SendEmailRegister(User vUser)
        {
            //Send email for user to confirm email
            int districtId = _parameters.DistrictServices.GetLiCodeBySubDomain("CEE");
            string subject = _parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, "RegistrationEmailSubject").First().Value;
            string strBody = _parameters.DistrictDecodeServices.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, "RegistrationEmailContent").First().Value;
            strBody = strBody.Replace("#USERNAME_INPUT#", vUser.EmailAddress);

            Util.SendMail(strBody, subject, vUser.EmailAddress);
        }

        private void SendEmailReActive(User vUser, string passWord)
        {
            string urlLogin = Util.GetConfigByKey("HostCEE", "http://cee.linkitdev.com");
            string subject = "Your LinkIt! Password has been reset";
            var strBody = _parameters.EmailService.GetForgotPasswordEmailTemplate(vUser.DistrictId.GetValueOrDefault(), urlLogin, passWord, string.Empty);
            Util.SendMail(strBody, subject, vUser.EmailAddress);
        }

        private string GetDomain(string host)
        {
            Match match = Regex.Match(host, "([^.]+\\.[^.]{1,3}(\\.[^.]{1,3})?)$");
            string domain = match.Groups[1].Success ? match.Groups[1].Value : string.Empty;
            return domain;
        }

        [NewAllowCrossSiteJson]
        public ActionResult GetBrowserSupport()
        {
            string vSupportedBrowsers = GetSupportedBrowsers();

            if (vSupportedBrowsers.Length > 0)
            {
                var lstBrowser = new List<BrowserSupport>();
                string[] arr = vSupportedBrowsers.Split(';');
                if (arr.Length > 0)
                {
                    foreach (var s in arr)
                    {
                        try
                        {
                            string[] v = s.Split(':');
                            if (v.Length == 2)
                            {
                                lstBrowser.Add(new BrowserSupport()
                                {
                                    Name = v[0],
                                    Value = v[1]
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            //TODO: 
                        }
                    }
                    var obj = new BrowserData()
                    {
                        BrowserSupports = lstBrowser,
                        MessageAlert = GetMessageAlertKey()
                    };

                    if (lstBrowser.Count > 0)
                        return Json(obj, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Browser = "All" }, JsonRequestBehavior.AllowGet);
        }

        private string GetSupportedBrowsers()
        {
            var vSupportedBrowsersKey = "Cache_vSupportedBrowsers";
            string vSupportedBrowsers = "";

            if (!CacheHelper.Exists(vSupportedBrowsersKey))
            {
                var configItem = _parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue("SupportedBrowsers", string.Empty);
                CacheHelper.Add(configItem, vSupportedBrowsersKey, GetDefaultCacheInMinute());
            }

            CacheHelper.Get(vSupportedBrowsersKey, out vSupportedBrowsers);
            return vSupportedBrowsers;
        }

        private string GetMessageAlertKey()
        {
            var vMessageAlertKey = "Cache_vMessageAlert";
            string vMessageAlert = "";

            if (!CacheHelper.Exists(vMessageAlertKey))
            {
                var configItem = _parameters.ConfigurationServices.GetConfigurationByKeyWithDefaultValue("OutdatedBrowserNotificationText", string.Empty);
                CacheHelper.Add(configItem, vMessageAlertKey, GetDefaultCacheInMinute());
            }

            CacheHelper.Get(vMessageAlertKey, out vMessageAlert);
            return vMessageAlert;
        }

        private int GetDefaultCacheInMinute()
        {
            int defaultCacheInMinute = 60;
            if (ConfigurationManager.AppSettings["DefaultCacheInMinute"] != null)
            {
                int.TryParse(ConfigurationManager.AppSettings["DefaultCacheInMinute"].ToString(), out defaultCacheInMinute);
            }

            return defaultCacheInMinute;
        }
    }
}
