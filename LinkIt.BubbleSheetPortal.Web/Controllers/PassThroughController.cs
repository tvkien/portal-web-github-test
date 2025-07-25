using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Helpers.SAML;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using System.Xml;
using System.Diagnostics;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Web.Models.Survey;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Constant;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.Enum;
using DevExpress.Xpo.DB;
using Lokad.Cloud.Storage;
using System.Collections.Specialized;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [VersionFilter]
    public class PassThroughController : BaseController
    {
        private readonly PassThroughControllerParameters parameters;
        private readonly UserMetaService userMetaService;
        private readonly StudentService studentService;

        public PassThroughController(
            PassThroughControllerParameters passThroughControllerParameters,
            UserMetaService userMetaService,
            StudentService studentService)
        {
            this.parameters = passThroughControllerParameters;
            this.userMetaService = userMetaService;
            this.studentService = studentService;
        }

        [HttpGet]
        public ActionResult Index(string q, string k)
        {
            var obj = new PassThroughMessage() { ReturnLoginPage = false, MessageError = string.Empty, RedirectUrl = string.Empty };
            APIAccount apiAccount = GetPrivateKey(HttpUtility.HtmlDecode(k));
            if (apiAccount == null)
            {
                //TODO: throw message and return Login Page
                obj.ReturnLoginPage = true;
                obj.MessageError = "Unable to process this request!";
                return View(obj);
            }
            PassThroughViewModel passThrough = parsePassThrought(HttpUtility.HtmlDecode(q), apiAccount.LinkitPrivateKey);
            if (passThrough == null)
            {
                //TODO: decript error, return login page
                obj.ReturnLoginPage = true;
                obj.MessageError = "Unable to process this request!";
                return View(obj);
            }
            if (ValidParameter(passThrough, obj, apiAccount) != 1)
            {
                return View(obj);
            }
            return PassThroughLogin(passThrough);
        }

        [HttpGet]
        public ActionResult TeacherReviewer(string q, string k)
        {
            var obj = new PassThroughMessage() { ReturnLoginPage = false, MessageError = string.Empty, RedirectUrl = string.Empty };
            APIAccount apiAccount = GetPrivateKey(HttpUtility.HtmlDecode(k));
            if (apiAccount == null)
            {
                obj.ReturnLoginPage = true;
                obj.MessageError = "Unable to process this request!";
                return View("Index", obj);
            }

            PassThroughTeacherReviewerViewModel passThrough = ParseTeacherReviewer(HttpUtility.HtmlDecode(q), apiAccount.LinkitPrivateKey);
            if (passThrough == null)
            {
                obj.ReturnLoginPage = true;
                obj.MessageError = "Unable to process this request!";
                return View("Index", obj);
            }

            if (ValidTeacherReviewer(passThrough, obj, apiAccount) != 1)
            {
                return View("Index", obj);
            }

            return TeacherReviewerLogin(passThrough);
        }

        private string EncodeStudentID(int studentId)
        {
            var numList = studentId.ToString();
            string code = "";
            foreach (var numStr in numList)
            {
                var num = int.Parse(numStr.ToString());
                code += PassThroughConstants.ENCODE_STUDENT_ID_KEY[num];
            }

            return code.ToLower();
        }

        private ActionResult PassThroughLogin(PassThroughViewModel passThrough)
        {
            int userId = 0;
            int.TryParse(passThrough.UserID, out userId);
            var user = parameters.UserServices.GetUserById(userId);
            parameters.AuthenticationServices.SignOut();
            parameters.AuthenticationServices.SignIn(user, false, true);

            var districtSubDomain = "demo";
            var district = parameters.DistrictServices.GetDistrictById(user.DistrictId.Value);
            districtSubDomain = district.IsNull() ? districtSubDomain : district.LICode.ToLower();

            var path = string.Empty;

            if (!string.IsNullOrWhiteSpace(passThrough.LandingPage))
            {
                var landingPage = parameters.ConfigurationService.GetConfigurationByKey(passThrough.LandingPage);
                if (landingPage != null)
                {
                    path = $"/{landingPage.Value}";
                }

                if (passThrough.LandingPage == PassThroughConstants.STUDENT_HUB_PLANS_DOCUMENTS)
                {
                    if (user.RoleId == (int)Permissions.Student)
                    {
                        path = GetStudentOnlineTestingPath(user);
                    }
                    else if (user.RoleId != (int)Permissions.Parent)
                    {
                        var encodedStudenId = EncodeStudentID(passThrough.StudentID.Value);
                        path = string.Format(path, encodedStudenId);
                    }
                }
                else if (passThrough.LandingPage == PassThroughConstants.STUDENT_HUB_PASS_THROUGH_LANDING_PAGE)
                {
                    if (user.RoleId == (int)Permissions.Student || user.RoleId == (int)Permissions.Parent)
                    {
                        path += $"?xliCode={ContaintUtil.ReportingTestResult}";
                    }
                    else
                    {
                        var encodedStudentId = EncodeStudentID(passThrough.StudentID.Value);
                        path += $"/{encodedStudentId}";
                    }

                    if (!string.IsNullOrWhiteSpace(passThrough.ActiveTab))
                    {
                        var separator = path.Contains('?') ? "&" : "?";
                        path += $"{separator}activetab={passThrough.ActiveTab}";
                    }

                    path = HttpUtility.JavaScriptStringEncode(path);
                }
            }
            else if (user.RoleId == (int)Permissions.Student)
            {
                path = GetStudentOnlineTestingPath(user);
            }

            var redirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), districtSubDomain, user, false, path);

            WriteCookiesPassThrough(passThrough.RedirectUrl, userId);

            return View("Redirect", model: redirectUrl);
        }

        private string GetStudentOnlineTestingPath(User user)
        {
            var menuAccess = HelperExtensions.GetMenuForDistrict(user);
            if (menuAccess.IsDisplayOnlineTesting)
            {
                return "/StudentOnlineTesting";
            }

            return string.Empty;
        }

        private ActionResult TeacherReviewerLogin(PassThroughTeacherReviewerViewModel passThrough)
        {
            int userId = 0;
            int.TryParse(passThrough.UserId, out userId);
            var user = parameters.UserServices.GetUserById(userId);
            parameters.AuthenticationServices.SignOut();
            parameters.AuthenticationServices.SignIn(user, false, true);
            WriteCookiesPassThrough(passThrough.RedirectUrl, userId);

            return RedirectToAction("Index", "TestAssignmentReview", new { passThrough.AssignmentCodes });
        }

        [HttpGet]
        public ActionResult GeneratePassThroughURL()
        {
            var obj = new GeneratePassThroughViewModel()
            {
                AccessKey = "00000000-0000-0000-0000-000000000000",
                PrivateKey = string.Empty,
                RawData = string.Empty
            };
            return View(obj);
        }

        [HttpPost]
        public ActionResult GeneratePassThroughURL(GeneratePassThroughViewModel obj)
        {
            APIAccount apiAccount = parameters.APIAccountServices.GetAPIAccountByClientAccessKey(obj.AccessKey);

            obj.Timestamp = DateTime.UtcNow.ToString();
            var passThrough = new PassThroughViewModel()
            {
                UserID = obj.UserID ?? string.Empty,
                LandingPage = obj.LandingPage ?? string.Empty,
                RedirectUrl = obj.RedirectUrl ?? string.Empty,
                Timestamp = obj.Timestamp
            };
            var json = new JavaScriptSerializer().Serialize(passThrough);
            obj.RawData = json;
            obj.PrivateKey = apiAccount == null ? string.Empty : apiAccount.LinkitPrivateKey;
            return View(obj);
        }

        [HttpGet]
        public JsonResult GenerateURL(string rawData, string accessKey, string privateKey)
        {
            string strURL = string.Empty;
            if (!string.IsNullOrEmpty(privateKey))
            {
                string strData = EncryptString(rawData, privateKey);
                var url = ConfigurationManager.AppSettings["LinkItUrl"];
                APIAccount apiAccount = parameters.APIAccountServices.GetAPIAccountByClientAccessKey(accessKey);

                var districtId = 0;
                var subdomain = "demo";
                if (apiAccount.APIAccountTypeID == 1)
                {
                    districtId = apiAccount.TargetID;
                }
                else
                {
                    var user = parameters.UserServices.GetUserById(apiAccount.TargetID);
                    districtId = user.DistrictId ?? 0;
                }
                if (districtId > 0)
                {
                    var district = parameters.DistrictServices.GetDistrictById(districtId);
                    subdomain = district.LICode.ToLower();
                }
                var redirectUrl = string.Format("{0}://{1}.{2}", HelperExtensions.GetHTTPProtocal(Request), subdomain, url);
                strURL = string.Format("{0}/passthrough?q={1}&k={2}", redirectUrl, HttpUtility.UrlEncode(strData), HttpUtility.UrlEncode(accessKey));
            }
            return Json(new { success = true, data = strURL }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GenerateStudentURL(string rawData, string accessKey, string privateKey)
        {
            string strURL = string.Empty;
            if (!string.IsNullOrEmpty(privateKey))
            {
                string strData = EncryptString(rawData, privateKey);
                var url = ConfigurationManager.AppSettings["LinkItUrl"];
                var redirectUrl = string.Format("{0}://demo.{1}", HelperExtensions.GetHTTPProtocal(Request), url);
                strURL = string.Format("{0}/studentpassthrough?q={1}&k={2}", redirectUrl, HttpUtility.UrlEncode(strData), HttpUtility.UrlEncode(accessKey));
            }
            return Json(new { success = true, data = strURL }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GenerateStudentPassThroughURL()
        {
            var obj = new GenerateStudentPassThroughViewModel()
            {
                AccessKey = "00000000-0000-0000-0000-000000000000",
                PrivateKey = string.Empty,
                RawData = string.Empty
            };
            return View(obj);
        }

        [HttpPost]
        public ActionResult GenerateStudentPassThroughURL(GenerateStudentPassThroughViewModel obj)
        {
            APIAccount apiAccount = parameters.APIAccountServices.GetAPIAccountByClientAccessKey(obj.AccessKey);

            obj.Timestamp = DateTime.UtcNow.ToString();
            var passThrough = new StudentPassThroughViewModel()
            {
                StudentID = obj.StudentID ?? string.Empty,
                RedirectUrl = obj.RedirectUrl ?? string.Empty,
                Timestamp = obj.Timestamp
            };
            var json = new JavaScriptSerializer().Serialize(passThrough);
            obj.RawData = json;
            obj.PrivateKey = apiAccount == null ? string.Empty : apiAccount.LinkitPrivateKey;
            return View(obj);
        }

        #region NonAction
        [NonAction]
        private int ValidParameter(PassThroughViewModel passThrough, PassThroughMessage obj, APIAccount apiAccount)
        {
            bool bProcessFault = false;
            //TODO: Check Valid Time connect
            DateTime dt = DateTime.UtcNow.AddDays(-1);
            if (DateTime.TryParse(passThrough.Timestamp, out dt))
            {
                TimeSpan deplayTimeSpan = DateTime.UtcNow - dt;
                if (deplayTimeSpan.TotalSeconds > 60)
                {
                    //TODO: Raise message timeout
                    bProcessFault = true;
                    obj.MessageError = "Request timeout!";
                }
            }
            else
            {
                bProcessFault = true;
                obj.MessageError = "Data not found!";
            }
            var user = new User();
            //TODO: Check Valid User]
            if (string.IsNullOrEmpty(passThrough.UserID))
            {
                bProcessFault = true;
                obj.MessageError = "Data not found!";
            }
            else
            {
                int userId = 0;
                int.TryParse(passThrough.UserID, out userId);
                user = parameters.UserServices.GetUserById(userId);
                if (user.IsNull() || !user.DistrictId.HasValue || user.UserStatusId != (int)UserStatus.Active
                   || user.RoleId == (int)Permissions.Publisher || !ValidUserRole(apiAccount, user))
                {
                    bProcessFault = true;
                    obj.MessageError = "User not authorized!";
                }
            }
            //Check valid Student
            if (!string.IsNullOrWhiteSpace(passThrough.LandingPage)
                && (passThrough.LandingPage == PassThroughConstants.STUDENT_HUB_PLANS_DOCUMENTS || passThrough.LandingPage == PassThroughConstants.STUDENT_HUB_PASS_THROUGH_LANDING_PAGE)
                && user.RoleId != (int)Permissions.Student
                && user.RoleId != (int)Permissions.Parent)
            {
                if (passThrough.StudentID.HasValue)
                {
                    var student = studentService.GetStudentById(passThrough.StudentID.Value);
                    if (student == null)
                    {
                        bProcessFault = true;
                        obj.MessageError = "Data not found!";
                    }
                }
                else
                {
                    bProcessFault = true;
                    obj.MessageError = "Data not found!";
                }
            }
            bool isValidURL = true;
            if (!string.IsNullOrEmpty(passThrough.RedirectUrl))
            {
                if (!passThrough.RedirectUrl.StartsWith("http"))
                {
                    passThrough.RedirectUrl = string.Format("http://{0}", passThrough.RedirectUrl.Trim());
                }
                isValidURL = Regex.IsMatch(passThrough.RedirectUrl, @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            }
            if (!isValidURL)
            {
                passThrough.RedirectUrl = "http://www.google.com"; //TODO: hardcode
            }
            if (bProcessFault)
            {
                if (!string.IsNullOrEmpty(passThrough.RedirectUrl))
                {
                    obj.RedirectUrl = passThrough.RedirectUrl;
                    obj.ReturnLoginPage = false;
                    return 0;
                }
                else
                {
                    //return RedirectToAction("Index", "Home");
                    obj.ReturnLoginPage = true;
                    obj.RedirectUrl = string.Empty;
                    return 2;
                }
            }
            return 1;
        }

        private int ValidTeacherReviewer(PassThroughTeacherReviewerViewModel passThrough, PassThroughMessage obj, APIAccount apiAccount)
        {
            bool bProcessFault = false;

            DateTime dt = DateTime.UtcNow.AddDays(-1);
            if (DateTime.TryParse(passThrough.Timestamp, out dt))
            {
                TimeSpan deplayTimeSpan = DateTime.UtcNow - dt;
                if (deplayTimeSpan.TotalSeconds > 60)
                {
                    bProcessFault = true;
                    obj.MessageError = "Request timeout!";
                }
            }
            else
            {
                bProcessFault = true;
                obj.MessageError = "Data not found!";
            }

            if (string.IsNullOrEmpty(passThrough.UserId))
            {
                bProcessFault = true;
                obj.MessageError = "Data not found!";
            }
            else
            {
                int userId = 0;
                int.TryParse(passThrough.UserId, out userId);
                var user = parameters.UserServices.GetUserById(userId);
                if (user.IsNull() || !user.DistrictId.HasValue || user.UserStatusId != (int)UserStatus.Active
                    || (user.RoleId != (int)Permissions.Teacher && user.RoleId != (int)Permissions.SchoolAdmin &&
                        user.RoleId != (int)Permissions.DistrictAdmin) || !ValidUserRole(apiAccount, user))
                {
                    bProcessFault = true;
                    obj.MessageError = "User not authorized!";
                }
            }

            bool isValidURL = true;
            if (!string.IsNullOrEmpty(passThrough.RedirectUrl))
            {
                if (!passThrough.RedirectUrl.StartsWith("http"))
                {
                    passThrough.RedirectUrl = string.Format("http://{0}", passThrough.RedirectUrl.Trim());
                }
                isValidURL = Regex.IsMatch(passThrough.RedirectUrl, @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            }
            if (!isValidURL)
            {
                passThrough.RedirectUrl = "http://www.google.com"; //TODO: hardcode
            }
            if (bProcessFault)
            {
                if (!string.IsNullOrEmpty(passThrough.RedirectUrl))
                {
                    obj.RedirectUrl = passThrough.RedirectUrl;
                    obj.ReturnLoginPage = false;
                    return 0;
                }
                else
                {
                    //return RedirectToAction("Index", "Home");
                    obj.ReturnLoginPage = true;
                    obj.RedirectUrl = string.Empty;
                    return 2;
                }
            }


            return 1;
        }

        [NonAction]
        private void WriteCookiesPassThrough(string strReturnUrl, int userId)
        {
            var formsAuthCookie = new HttpCookie("UserPassThrough")
            {
                HttpOnly = true,
                Path = FormsAuthentication.FormsCookiePath,
                Secure = FormsAuthentication.RequireSSL
            };
            if (FormsAuthentication.CookieDomain != null)
            {
                formsAuthCookie.Domain = FormsAuthentication.CookieDomain;
            }
            formsAuthCookie["PassThroughUserID"] = userId.ToString();
            formsAuthCookie["PassThroughReturnURL"] = strReturnUrl;
            formsAuthCookie.Expires = DateTime.Now.AddDays(1d);
            formsAuthCookie["GUIDSession"] = Guid.NewGuid().ToString();
            Response.Cookies.Add(formsAuthCookie);
            var userLogon = new UserLogon()
            {
                UserID = userId,
                GUIDSession = formsAuthCookie["GUIDSession"]
            };
            parameters.UserLogonService.Save(userLogon);
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

        [NonAction]
        private PassThroughViewModel parsePassThrought(string strData, string strPrivateKey)
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
                    var obj = new JavaScriptSerializer().Deserialize<PassThroughViewModel>(tmp);
                    return obj;
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    //TODO: can't desirialize to object
                }
            }
            return null;
        }

        [NonAction]
        private PassThroughTeacherReviewerViewModel ParseTeacherReviewer(string strData, string strPrivateKey)
        {
            string tmp = string.Empty;
            try
            {
                tmp = DecryptString(strData, strPrivateKey);
            }
            catch (Exception exception)
            {
                PortalAuditManager.LogException(exception);
                //TODO: handle exception
            }

            if (!string.IsNullOrEmpty(tmp))
            {
                try
                {
                    var obj = new JavaScriptSerializer().Deserialize<PassThroughTeacherReviewerViewModel>(tmp);
                    return obj;
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                    //TODO: can't desirialize to object
                }
            }

            return null;
        }

        [NonAction]
        private APIAccount GetPrivateKey(string accessKey)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(accessKey))
                {
                    string pathAndQuery = Request.Url.ToString().ToLower();
                    APIAccount apiAccount = parameters.APIAccountServices.GetAPIAccountByClientAccessKey(accessKey);
                    if (apiAccount != null)
                    {
                        //return apiAccount;
                        //TODO: valid permission
                        List<int> listFunctionId = parameters.APIPermissionServices.GetAPIPermissionByTaget(apiAccount.TargetID);
                        if (parameters.APIFunctionServices.CheckValidURL(listFunctionId, pathAndQuery))
                            return apiAccount;
                    }
                }
            }
            catch (Exception exception)
            {
                PortalAuditManager.LogException(exception);
                return null;
            }
            return null;
        }

        private bool ValidUserRole(APIAccount apiAccount, User u)
        {
            try
            {
                if (apiAccount != null && u != null)
                {
                    if (apiAccount.APIAccountTypeID == (int)APIAccountType.District)
                    {
                        return u.DistrictId == apiAccount.TargetID;
                    }
                    else if (apiAccount.APIAccountTypeID == (int)APIAccountType.User)
                    {
                        var user = parameters.UserServices.GetUserById(apiAccount.TargetID);
                        if (user != null)
                        {
                            switch (user.RoleId)
                            {
                                case (int)Permissions.DistrictAdmin:
                                    {
                                        return u.DistrictId == user.DistrictId;
                                    }
                                case (int)Permissions.SchoolAdmin:
                                    {
                                        if (u.RoleId == (int)Permissions.SchoolAdmin ||
                                            u.RoleId == (int)Permissions.Teacher)
                                        {
                                            var lstSchoolApi = parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(user.Id).Select(o => o.SchoolId.GetValueOrDefault()).ToList();
                                            var lstSchoolUserLogin = parameters.UserSchoolServices.GetSchoolsUserHasAccessTo(u.Id).Select(o => o.SchoolId.GetValueOrDefault()).ToList();
                                            if (lstSchoolApi.Any() && lstSchoolUserLogin.Any() && lstSchoolApi.Any(lstSchoolUserLogin.Contains))
                                                return true;
                                        }
                                    }
                                    break;
                                case (int)Permissions.Teacher:
                                    {
                                        return u.Id == user.Id;
                                    }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return false;
            }
            return false;
        }

        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult GenerateSurveyURL(SurveyGenerationInput model)
        {
            if (string.IsNullOrEmpty(model.ClientKey))
            {
                model.ClientKey = string.Empty;
            }
            if (string.IsNullOrEmpty(model.ReturnURL))
            {
                model.ReturnURL = string.Empty;
            }
            string strURL = string.Empty;
            if (model.SchoolId > 0 && model.VirtualTestId > 0)
            {
                // Valid school Id, virtualTestId
                int districtId = 0;
                //1 Check DistrictID By DistrictID             
                var objSchool = parameters.SchoolService.GetSchoolById(model.SchoolId);
                if (objSchool == null)
                {
                    return Json(new { success = false, data = strURL, message = "Invalid SchoolID." });
                }

                var objDistrict = parameters.DistrictServices.GetDistrictById(objSchool.DistrictId);
                if (objDistrict == null)
                {
                    return Json(new { success = false, data = strURL, message = "Invalid District." });
                }

                var objVirtualtest = parameters.VirtualTestService.GetVirtualTestById(model.VirtualTestId);
                if (objVirtualtest == null)
                {
                    return Json(new { success = false, data = strURL, message = "Invalid VirtualTestID." });
                }

                //Init District
                districtId = objSchool.DistrictId;

                // Valid client key, secret key
                APIAccount apiAccount = parameters.APIAccountServices.GetAPIAccountByDistrictId(districtId);
                if (apiAccount == null)
                {
                    return Json(new { success = false, data = strURL, message = "District not exists ApiAccount." });
                }
                if (apiAccount.ClientAccessKeyID.ToString().ToLower() != model.ClientKey.ToLower())
                {
                    return Json(new { success = false, data = strURL, message = "Invalid ClientAccesskey!" });
                }

                // Initial class, teacher, term... (do not generate student)
                //2 Check TeacherID By DistrictID                 
                string strSurveyUserNameFormat = "Survey Teacher";
                int surveyUserId = parameters.UserServices.GetUserSurveyByUserNameAndDistrictId(objSchool.DistrictId, strSurveyUserNameFormat);
                if (surveyUserId == 0)
                    surveyUserId = CreateSurveyUser(objSchool.DistrictId, objSchool.Id);

                //3 Check DistrictTerm By DistrictID
                string strSurveyDistrictTermNameFormat = RoleGetDistrictTerm(objDistrict.Id, objDistrict.Name);
                int surveyDistrictTermId = parameters.DistrictTermService.GetSurveyDistrictTermId(objSchool.DistrictId, strSurveyDistrictTermNameFormat);
                if (surveyDistrictTermId == 0)
                {
                    surveyDistrictTermId = CreateSurveyDistrictTerm(objSchool.DistrictId, strSurveyDistrictTermNameFormat);
                }

                //4 Check Class By SchoolID, DistrictTermID, DistrictID & Name (SurveyName + SchoolName)
                string strSurveyClassNameFormat = "Survey Class";
                var surveyClassId = parameters.ClassService.GetSurveyClassBySchoolDistrictTermAndName(objSchool.DistrictId, objSchool.Id, surveyDistrictTermId, strSurveyClassNameFormat);
                if (surveyClassId == 0)
                    surveyClassId = CreateSurveyClass(objSchool.DistrictId, objSchool.Id, surveyDistrictTermId, surveyUserId);

                var objClassAssignment = parameters.QTITestClassAssignmentService.GetSurveyRosterAssignment(objSchool.DistrictId, surveyClassId, model.VirtualTestId);
                if (objClassAssignment == null)
                    objClassAssignment = CreateSurveyRosterAssignment(objSchool.DistrictId, surveyClassId, model.VirtualTestId);

                string strSurveyUrl = ConfigurationManager.AppSettings["SurveyUrl"] ?? "http://survey.linkit.devblock.net";

                strURL = string.Format("{0}?schoolId={1}&surveyId={2}&code={3}&returnurl={4}", strSurveyUrl, model.SchoolId, model.VirtualTestId, objClassAssignment.Code, HttpUtility.UrlEncode(model.ReturnURL));

                strURL = parameters.ShortLinkService.GenerateShortLink(strURL);

                return Json(new { success = true, data = strURL, message = string.Empty });
            }
            string strMessage = string.Format("Invalid Input Data: SchoolID={0}, VirtualTestID={1}", model.SchoolId, model.VirtualTestId);
            return Json(new { success = false, data = strURL, message = strMessage });
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public ActionResult GenerateSurveyTestTakerURL(string key, string code, string returnUrl, int schoolId = 0, int surveyId = 0)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var url = parameters.ShortLinkService.GetFullLink(key);
                Uri uri = new Uri(url);
                int.TryParse(HttpUtility.ParseQueryString(uri.Query).Get("schoolId"), out schoolId);
                int.TryParse(HttpUtility.ParseQueryString(uri.Query).Get("surveyId"), out surveyId);
                code = HttpUtility.ParseQueryString(uri.Query).Get("code");
                returnUrl = HttpUtility.ParseQueryString(uri.Query).Get("returnUrl");
            }

            string strURL = string.Empty;
            if (schoolId > 0 && surveyId > 0 && !string.IsNullOrEmpty(code))
            {
                // Valid schoolId, surveyId, code
                var objVirtualtest = parameters.VirtualTestService.GetVirtualTestById(surveyId);
                if (objVirtualtest == null)
                {
                    return Json(new { success = false, data = strURL, message = "Invalid VirtualTestID." }, JsonRequestBehavior.AllowGet);
                }
                if (objVirtualtest.DatasetOriginID == (int)DataSetOriginEnum.Survey)
                {
                    return RedirectToAction("GenerateNewSurveyTestTakerURL", new { code = code, surveyId = surveyId, schoolId = schoolId, returnUrl = returnUrl });
                }

                var objSchool = parameters.SchoolService.GetSchoolById(schoolId);
                if (objSchool == null)
                {
                    return Json(new { success = false, data = strURL, message = "Invalid SchoolID." }, JsonRequestBehavior.AllowGet);
                }

                var objClassAssignment = parameters.QTITestClassAssignmentService.GetSurveyRosterByCode(code, surveyId, objSchool.DistrictId);
                if (objClassAssignment == null || objClassAssignment.VirtualTestId != surveyId)
                {
                    return Json(new { success = false, data = strURL, message = "Invalid Code." }, JsonRequestBehavior.AllowGet);
                }
                var objClass = parameters.ClassService.SurveyGetClassByClassId(objClassAssignment.ClassId);
                if (objClass == null || objClass.SchoolId != schoolId)
                {
                    return Json(new { success = false, data = strURL, message = "Input data not make." }, JsonRequestBehavior.AllowGet);
                }

                //Generate Fake Student
                var surveyStudentId = CreateSurveyStudent(objSchool.DistrictId);
                //Assign student to class
                parameters.ClassStudentService.SurveyInsertClassStudent(objClassAssignment.ClassId, surveyStudentId);

                PassThroughResponse passThrough = new PassThroughResponse();
                passThrough.TestCode = objClassAssignment.Code;
                passThrough.AssignmentGUID = objClassAssignment.AssignmentGuId;
                passThrough.StudentID = surveyStudentId.ToString();
                passThrough.RedirectUrl = returnUrl;
                passThrough.PassThroughType = "survey";

                // Build TestTaker Passthrough
                strURL = GenerateSurveyTestTakerPassThrough(passThrough, objSchool.DistrictId);
                return Json(new { success = true, data = strURL, message = string.Empty }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, data = strURL, message = "Invalid Input Data." }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowCrossSiteJson]
        public ActionResult GenerateNewSurveyTestTakerURL(string code, int surveyId, int schoolId, string returnUrl)
        {
            var url = string.Empty;
            var school = parameters.SchoolService.GetSchoolById(schoolId);
            var classAssignment = parameters.QTITestClassAssignmentService.GetSurveyAssignmentByCode(code, surveyId, school.DistrictId);
            if (classAssignment == null || classAssignment.VirtualTestId != surveyId)
            {
                return Json(new { success = false, data = url, message = "Invalid Code." }, JsonRequestBehavior.AllowGet);
            }
            var surveyClass = parameters.ClassService.SurveyGetClassByClassId(classAssignment.ClassId);
            if (surveyClass == null || surveyClass.SchoolId != schoolId)
            {
                return Json(new { success = false, data = url, message = "Class assignment is not belong to school." }, JsonRequestBehavior.AllowGet);
            }
            var studentId = 0;
            if (classAssignment.SurveyAssignmentType.HasValue && classAssignment.SurveyAssignmentType == 1)
            {
                studentId = CreateSurveyPublicAnonymousStudent(school.DistrictId);
                parameters.ClassStudentService.SurveyInsertClassStudent(classAssignment.ClassId, studentId);
            }
            else
            {
                var studentAssignment = parameters.QTITestStudentAssignmentService.GetByQTITestClassAssignmentId(classAssignment.QTITestClassAssignmentId);
                if (studentAssignment.Any())
                    studentId = studentAssignment.FirstOrDefault().StudentId;
            }

            var passThrough = new PassThroughResponse()
            {
                TestCode = classAssignment.Code,
                AssignmentGUID = classAssignment.AssignmentGuId,
                StudentID = studentId.ToString(),
                RedirectUrl = returnUrl,
                PassThroughType = "survey"
            };
            url = GenerateSurveyTestTakerPassThrough(passThrough, school.DistrictId);
            return Json(new { success = true, data = url, message = string.Empty }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PassThrought V-DET

        [HttpGet]
        public ActionResult GeneratePassThroughVDET()
        {
            var obj = new GeneratePassThroughVDETViewModel
            {
                AccessKey = "00000000-0000-0000-0000-000000000000",
                PrivateKey = string.Empty,
                RawData = string.Empty
            };
            return View(obj);
        }

        public ActionResult InsightPlatformLogOn(string SAMLResponse)
        {
            if (!string.IsNullOrEmpty(SAMLResponse))
            {
                var samlResponse = Encoding.UTF8.GetString(Convert.FromBase64String(SAMLResponse));
                var certString = ConfigurationManager.AppSettings["AzureVdetCertString"];
                var signingCert = new X509Certificate2(Encoding.UTF8.GetBytes(certString));

                if (SAML20Assertion.ParseAzureSAMLResponse(samlResponse, signingCert,
                    new PassThroughVDETMessageViewModel(), SAMLConfigurationBuilder.GetConfiguration(SSOClient.Vdet)) != null)
                {
                    Session["IsVdetLogin"] = true;
                }
            }

            return View();
        }

        public ActionResult InsightPlatformSignIn()
        {
            var configuration = SAMLConfigurationBuilder.GetConfiguration(SSOClient.Vdet);
            return BuildSignInRequest(configuration);
        }

        public ActionResult CecvSignIn()
        {
            var configuration = SAMLConfigurationBuilder.GetConfiguration(SSOClient.Cecv);
            return BuildSignInRequest(configuration);
        }

        private ActionResult BuildSignInRequest(SAMLConfiguration configuration)
        {
            string request = SAML20Assertion.CreateSsoAuthenticationRequest(configuration);
            string encodedRequest = Server.UrlEncode(request);
            var ssoTargetUrl = configuration.SsoTargetUrl;

            var urlRedirect = $"{ssoTargetUrl}?SAMLRequest={encodedRequest}";

            return Redirect(urlRedirect);
        }

        public ActionResult InsightPlatformSignOut()
        {
            Session["IsVdetLogin"] = false;

            return Redirect("https://login.microsoftonline.com/logout.srf");
        }

        public ActionResult VdetInitialEnpoint()
        {
            string request = SAML20Assertion.CreateAzureVdetRequest();
            string encodedRequest = Server.UrlEncode(request);

            var ssoTargetUrl = ConfigurationManager.AppSettings["AzureVdetSSOTargetURL"];

            return Redirect(ssoTargetUrl + "?SAMLRequest=" + encodedRequest);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GeneratePassThroughVDET(GeneratePassThroughVDETViewModel obj)
        {
            APIAccount apiAccount = parameters.APIAccountServices.GetAPIAccountByClientAccessKey(obj.AccessKey);
            //obj.Timestamp = DateTime.UtcNow.ToString();
            var samlRequest = new SAMLRequest
            {
                SectorId = obj.Sector,
                UserId = obj.UserID,
                ActionType = obj.ActionType,
                ReturnUrl = obj.RedirectUrl,
                ErrorUrl = "insight.com",
                Issuer = "http://insight.com",
                Audience = "http://linkitau.com",
                Recipient = "http://linkitau.com"
            };

            // get the certificate
            String certPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/CoverMyMeds.pfx");
            X509Certificate2 signingCert = new X509Certificate2(certPath, "4CoverMyMeds");
            var xmlResponse = SAML20Assertion.CreateSAML20Response(samlRequest, signingCert);
            obj.RawData = xmlResponse;
            obj.PrivateKey = apiAccount == null ? string.Empty : apiAccount.LinkitPrivateKey;
            return View(obj);
        }

        [HttpGet]
        [ValidateInput(false)]
        public JsonResult GenerateVDETURL(string rawData, string accessKey, string privateKey)
        {
            string strURL = string.Empty;
            string strK = string.Empty;
            string strReturnData = string.Empty;
            if (!string.IsNullOrEmpty(privateKey))
            {
                string strData = EncryptString(rawData, privateKey);
                var url = ConfigurationManager.AppSettings["LinkItUrl"];
                var vhttp = HelperExtensions.GetHTTPProtocal(Request);
                var redirectUrl = string.Format("{0}://demo.{1}", vhttp, url);
                //strURL = string.Format("{0}/passthrough/Vdet?q={1}&k={2}", redirectUrl, HttpUtility.UrlEncode(strData), HttpUtility.UrlEncode(accessKey));
                strURL = string.Format("{0}/passthrough/Vdet", redirectUrl);
                strK = accessKey;
                strReturnData = strData;
            }
            return Json(new { success = true, url = strURL, data = strReturnData, k = strK }, JsonRequestBehavior.AllowGet);
            //return Json(new { success = true, data = strURL }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Error(string message)
        {
            return View((object)message);
        }

        public ActionResult Cecv(string SAMLResponse)
        {
            return ProcessSamlPackage(SAMLResponse, SAMLConfigurationBuilder.GetConfiguration(SSOClient.Cecv));
        }

        public ActionResult Vdet(string SAMLResponse)
        {
            return ProcessSamlPackage(SAMLResponse, SAMLConfigurationBuilder.GetConfiguration(SSOClient.Vdet));
        }

        private ActionResult ProcessSamlPackage(string SAMLResponse, SAMLConfiguration samlConfiguration)
        {
            try
            {
                if (!string.IsNullOrEmpty(SAMLResponse))
                {
                    var samlResponse = DecodedSamlResponse(SAMLResponse, samlConfiguration);
                    LogToEventViewer(samlResponse);
                    //var certString = ConfigurationManager.AppSettings["AzureVdetCertString"];
                    var certString = samlConfiguration.CertString;
                    var signingCert = new X509Certificate2(Encoding.UTF8.GetBytes(certString));

                    var parsedResponse = SAML20Assertion.ParseAzureSAMLResponse(samlResponse, signingCert,
                        new PassThroughVDETMessageViewModel(), samlConfiguration);
                    LogToEventViewer(JsonConvert.SerializeObject(parsedResponse));
                    if (parsedResponse != null)
                    {
                        SAMLRequest passThrough = new SAMLRequest();
                        passThrough.UserId = parsedResponse.UserId;
                        passThrough.SectorId = parsedResponse.SectorId;
                        //passThrough.ReturnUrl = ConfigurationManager.AppSettings["InsightLoginRedirectUrl"];
                        passThrough.ReturnUrl = samlConfiguration.LoginRedirectUrl;
                        passThrough.LogoutRedirectUrl = samlConfiguration.LogoutRedirectUrl;
                        string code = string.Empty;
                        string message = string.Empty;
                        string returnUrlAfterLogin = string.Empty;
                        LogToEventViewer(JsonConvert.SerializeObject(passThrough));
                        GetResultVDETLogin(passThrough, out code, out message, out returnUrlAfterLogin);
                        LogToEventViewer(string.Format("code [{0}] message [{1}] returnUrl [{2}]", code, message, returnUrlAfterLogin));
                        if (code == Util.PassThroughVDETCode201)
                        {
                            return Redirect(returnUrlAfterLogin);
                        }
                        else
                        {
                            return RedirectToAction("Error", new { message = message });
                        }
                    }
                    else
                    {
                        return RedirectToAction("Error", new { message = Util.PassThroughVDETMessage104 });
                    }
                }
                else
                {
                    return RedirectToAction("Error", new { message = Util.PassThroughVDETMessage105 });
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                LogToEventViewer(ex.ToString());
                return RedirectToAction("Error", new { message = Util.PassThroughVDETCode102 });
            }
        }

        private static string DecodedSamlResponse(string SAMLResponse, SAMLConfiguration configuration)
        {
            if (configuration.Client == SSOClient.Cecv)
            {
                var samlResponse = Encoding.UTF8.GetString(Convert.FromBase64String(Uri.UnescapeDataString(SAMLResponse)));
                return samlResponse;
            }
            else
            {
                var samlResponse = Encoding.UTF8.GetString(Convert.FromBase64String(SAMLResponse));
                return samlResponse;
            }
        }

        public void LogToEventViewer(string content)
        {
            try
            {
                var source = "VDET-SAML";
                var log = "Application";
                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(source, log);

                EventLog.WriteEntry(source, content);
            }
            catch {; }
        }

        public class SAMLProcessor
        {

            public string UserId { get; set; }
            public string SectorId { get; set; }

            #region Properties
            public string DecodedSAML { get; set; }
            public string EncodedeSAML { get; set; }
            public string Audience { get; set; }
            public string SubjectNameID { get; set; }
            public string FirstName { get; set; }
            public string Mail { get; set; }
            public string LastName { get; set; }
            public bool AuthenticationStatus { get; set; }
            public string Issuer { get; set; }
            public string Destination { get; set; }
            public string ResponseID { get; set; }
            public bool VerifiedResponse { get; set; }
            public string SignatureValue { get; set; }
            public string SignatureReferenceDigestValue { get; set; }
            public DateTime AutheticationTime { get; set; }
            public string AuthenticationSession { get; set; }
            #endregion


            #region Ctror
            public SAMLProcessor(string rawSamlData)
            {
                EncodedeSAML = rawSamlData;
                // the sample data sent us may be already encoded, 
                // which results in double encoding
                if (rawSamlData.Contains('%'))
                {
                    rawSamlData = HttpUtility.UrlDecode(rawSamlData);
                }

                // read the base64 encoded bytes
                string samlAssertion = Decode64Bit(rawSamlData);
                DecodedSAML = samlAssertion;
                SamlParser(DecodedSAML);
            }
            #endregion

            private static string Decode64Bit(string rawSamlData)
            {
                byte[] samlData = Convert.FromBase64String(rawSamlData);

                // read back into a UTF string
                string samlAssertion = Encoding.UTF8.GetString(samlData);
                return samlAssertion;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="samldata"></param>
            /// <returns></returns>
            public string SamlParser(string samlXMLdata)
            {

                //samldata = Decode64Bit("PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4=") + samldata;
                string samldata = samlXMLdata;

                if (!samldata.StartsWith(@"<?xml version="))
                {
                    samldata = Decode64Bit("PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4=") + samldata;
                }

                string firstName = string.Empty;
                XmlDocument xDoc = new XmlDocument();
                samldata = samldata.Replace(@"\", "");
                xDoc.LoadXml(samldata);
                //xDoc.Load(new System.IO.TextReader());//Suppose the xml you have provided is stored in this xml file.

                XmlNamespaceManager xMan = new XmlNamespaceManager(xDoc.NameTable);
                xMan.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
                xMan.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
                xMan.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");


                XmlNode xNode = xDoc.SelectSingleNode("/samlp:Response/samlp:Status/samlp:StatusCode/@Value", xMan);
                if (xNode != null)
                {
                    this.AuthenticationStatus = false;
                    string statusCode = xNode.Value;
                    if (statusCode.EndsWith("status:Success"))
                    {
                        this.AuthenticationStatus = true;
                    }

                }

                xNode = xDoc.SelectSingleNode("/samlp:Response/@Destination", xMan);
                if (xNode != null)
                {
                    this.Destination = xNode.Value;
                }
                xNode = xDoc.SelectSingleNode("/samlp:Response/@IssueInstant", xMan);
                if (xNode != null)
                {
                    this.AutheticationTime = Convert.ToDateTime(xNode.Value);
                }
                xNode = xDoc.SelectSingleNode("/samlp:Response/@ID", xMan);
                if (xNode != null)
                {
                    this.ResponseID = xNode.Value;
                }
                xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Issuer", xMan);
                if (xNode != null)
                {
                    this.Issuer = xNode.InnerText;
                }

                xNode = xDoc.SelectSingleNode("/samlp:Response/ds:Signature/ds:SignedInfo/ds:Reference/ds:DigestValue", xMan);
                if (xNode != null)
                {
                    this.SignatureReferenceDigestValue = xNode.InnerText;
                }
                xNode = xDoc.SelectSingleNode("/samlp:Response/ds:Signature/ds:SignatureValue", xMan);
                if (xNode != null)
                {
                    this.SignatureValue = xNode.InnerText;
                }
                xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/@ID", xMan);
                if (xNode != null)
                {
                    this.AuthenticationSession = xNode.Value;
                }

                xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:Subject/saml:NameID", xMan);
                if (xNode != null)
                {
                    this.SubjectNameID = xNode.InnerText;
                }
                xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:Conditions/saml:AudienceRestriction/saml:Audience", xMan);
                if (xNode != null)
                {
                    this.Audience = xNode.InnerText;
                }

                //reverse order
                //</saml:AttributeValue></saml:Attribute></saml:AttributeStatement></saml:Assertion></samlp:Response>

                //string xQryStr = "//NewPatient[Name='" + name + "']";

                //XmlNode matchedNode = xDoc.SelectSingleNode(xQryStr);
                // samlp:Response  saml:Assertion saml:AttributeStatement saml:Attribute
                xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = 'FIRSTNAME']/saml:AttributeValue", xMan);
                if (xNode != null)
                {
                    this.FirstName = xNode.InnerText;
                }

                // samlp:Response  saml:Assertion saml:AttributeStatement saml:Attribute
                xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = 'MAIL']/saml:AttributeValue", xMan);
                if (xNode != null)
                {
                    this.Mail = xNode.InnerText;
                }

                // samlp:Response  saml:Assertion saml:AttributeStatement saml:Attribute
                xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = 'LASTNAME']/saml:AttributeValue", xMan);
                if (xNode != null)
                {
                    this.LastName = xNode.InnerText;
                }

                return this.FirstName;
            }
        }

        private ActionResult GetResultVDETLogin(SAMLRequest passThrough, out string code, out string message, out string returnUrlAfterLogin)
        {
            code = Util.PassThroughVDETCode102;
            message = Util.PassThroughVDETMessage102;
            returnUrlAfterLogin = passThrough.ReturnUrl;

            UserDistrictSector vUserDistrictSector = null;
            if (!string.IsNullOrEmpty(passThrough.UserId))
            {
                vUserDistrictSector = parameters.UserDistrictSectorService.GetByCodeAndSector(passThrough.UserId, passThrough.SectorId);
            }
            else if (!string.IsNullOrEmpty(passThrough.UserEmailAddress))
            {
                vUserDistrictSector = parameters.UserDistrictSectorService.GetByEmailAndSector(passThrough.UserEmailAddress, passThrough.SectorId);
            }

            int userId = vUserDistrictSector != null ? vUserDistrictSector.UserID : 0;

            var user = parameters.UserServices.GetUserById(userId);
            if (user == null || user.UserStatusId != (int)UserStatus.Active)
            {
                code = Util.PassThroughVDETCode113;
                message = Util.PassThroughVDETMessage113;
                return Json(new { status = Util.PassThroughVDETStatusFail, code = code, message = message, redirecturl = returnUrlAfterLogin });
            }

            LoadUserMetaData(user);

            user.IsVDETUser = true;
            parameters.AuthenticationServices.SignOut();
            parameters.AuthenticationServices.SignIn(user, false, true);
            parameters.UserServices.UpdateLastLogin(userId); //update LastLoginDate for SSO users

            var districtSubDomain = "demo";
            var district = parameters.DistrictServices.GetDistrictById(user.DistrictId.Value);
            districtSubDomain = district.IsNull() ? districtSubDomain : district.LICode.ToLower();

            var redirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), districtSubDomain,
                user);
            //var redirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser("https", districtSubDomain,
            //    user);
            WriteCookiesPassThroughVDET(passThrough.LogoutRedirectUrl, userId, user.DistrictId.Value);

            //return Redirect(redirectUrl);
            var obj = new PassThroughVDETMessageViewModel()
            {
                Status = Util.PassThroughVDETStatusSuccess,
                Code = Util.PassThroughVDETCode201,
                Message = Util.PassThroughVDETMessage201,
                RedirectUrl = redirectUrl
            };

            returnUrlAfterLogin = redirectUrl;
            code = obj.Code;
            message = obj.Message;
            return Json(new { status = obj.Status, code = obj.Code, message = obj.Message, redirecturl = obj.RedirectUrl });
        }

        [NonAction]
        private void WriteCookiesPassThroughVDET(string strReturnUrl, int userId, int districtID)
        {
            var formsAuthCookie = new HttpCookie("UserPassThrough")
            {
                HttpOnly = true,
                Path = FormsAuthentication.FormsCookiePath,
                Secure = FormsAuthentication.RequireSSL
            };
            if (FormsAuthentication.CookieDomain != null)
            {
                formsAuthCookie.Domain = FormsAuthentication.CookieDomain;
            }
            formsAuthCookie["PassThroughUserID"] = userId.ToString();
            formsAuthCookie["PassThroughDistrictID"] = districtID.ToString();
            formsAuthCookie["PassThroughReturnURL"] = strReturnUrl;
            formsAuthCookie["PassThroughVDET"] = "1";
            formsAuthCookie.Expires = DateTime.Now.AddDays(1d);
            formsAuthCookie["GUIDSession"] = Guid.NewGuid().ToString();
            Response.Cookies.Add(formsAuthCookie);
            var userLogon = new UserLogon()
            {
                UserID = userId,
                GUIDSession = formsAuthCookie["GUIDSession"]
            };
            parameters.UserLogonService.Save(userLogon);
        }

        public ActionResult VDetUser()
        {
            //\ check Pass Through
            string userId = string.Empty;
            string returnURL = string.Empty;

            HttpCookie passThroughCookie = Request.Cookies["UserPassThrough"];
            if (passThroughCookie != null)
            {
                userId = passThroughCookie["PassThroughUserID"];
                returnURL = passThroughCookie["PassThroughReturnURL"];
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LogOn", "Account");
            }
            parameters.AuthenticationServices.SignOut();
            if (Session != null && Session["MenuItem"] != null)
                Session["MenuItem"] = null;
            if (Session != null && Session["SubDomainDistrictID"] != null)
                Session["SubDomainDistrictID"] = null;

            if (!string.IsNullOrEmpty(returnURL))
            {
                return Redirect(returnURL);
            }
            return RedirectToAction("LogOn", "Account");
        }

        [HttpGet]
        public ActionResult CreateSAMLToken()
        {
            var samlRequest = new SAMLRequest
            {
                SectorId = "GVT",
                UserId = "123456",
                ActionType = "LogOn",
                ReturnUrl = "gvt.linkitau.com",
                ErrorUrl = "insight.com",
                Issuer = "http://insight.com",
                Audience = "http://linkitau.com",
                Recipient = "http://linkitau.com"
            };

            // get the certificate
            String certPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/CoverMyMeds.pfx");
            X509Certificate2 signingCert = new X509Certificate2(certPath, "4CoverMyMeds");

            var xmlResponse = SAML20Assertion.CreateSAML20Response(samlRequest, signingCert);

            var obj = new PassThroughMessage() { ReturnLoginPage = false, MessageError = xmlResponse, RedirectUrl = string.Empty };

            return View(obj);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ValidateSAMLToken(string xmlResponse)
        {
            String certPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/CoverMyMeds.cer");
            X509Certificate2 signingCert = new X509Certificate2(certPath);
            var obj = new PassThroughVDETMessageViewModel();
            var samlRequest = SAML20Assertion.ParseSAMLResponse(xmlResponse, signingCert, obj);

            bool result = samlRequest != null;

            return Json(new { Result = result.ToString() }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Init Survey Data
        private string GenerateSurveyTestTakerPassThrough(PassThroughResponse passThrough, int districtId)
        {
            try
            {
                string env = ConfigurationManager.AppSettings["ApiPrefix"] ?? "00000001";
                APIAccount apiAccount = parameters.APIAccountServices.GetAPIAccountByDistrictId(districtId);

                passThrough.Timestamp = $"{DateTime.UtcNow:G}";
                passThrough.RedirectUrl = string.IsNullOrEmpty(passThrough.RedirectUrl) ? string.Empty : passThrough.RedirectUrl;
                string strRawData = new JavaScriptSerializer().Serialize(passThrough);
                string strData = EncryptString(strRawData, apiAccount.LinkitPrivateKey);
                string testTakerUrl = ConfigurationManager.AppSettings["TestTakerURL"] ?? "https://test.linkit1.devblock.net/testtaker/testtaker.html";
                string strUrl = $"{testTakerUrl}?q={HttpUtility.UrlEncode(strData)}&k={HttpUtility.UrlEncode(apiAccount.ClientAccessKeyID.ToString())}&e={env}";

                if (parameters.DistrictDecodeService.DistrictSupportTestTakerNewSkin(districtId))
                {
                    strUrl = string.Format("{0}&{1}", strUrl, ContaintUtil.TESTTAKER_NEWSKIN);
                }

                return strUrl;
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                return string.Empty;
            }
        }

        /// Create a User with role=2 (teacher) belong districtId                
        private int CreateSurveyUser(int districtId, int schoolId)
        {
            var objConfiguration = parameters.ConfigurationService.GetConfigurationByKey(ContaintUtil.SurveyUserEmail); //SurveyUserEmail
            var surveyUser = new User();
            surveyUser.UserName = "Survey Teacher";
            surveyUser.LocalCode = "SurveyTeacher";
            surveyUser.FirstName = "Survey";
            surveyUser.LastName = "Teacher";
            surveyUser.DistrictId = districtId;
            surveyUser.Password = string.Empty;
            surveyUser.HashedPassword = System.Web.Helpers.Crypto.HashPassword("Linkit@Survey#2019");
            surveyUser.Hint = "Survey";
            surveyUser.RoleId = 2;
            surveyUser.SchoolId = schoolId;
            surveyUser.Privileges = string.Empty;
            surveyUser.DistrictGroupId = 0;
            surveyUser.Active = false;
            surveyUser.UserStatusId = 3; //1  Active, 2   Suspended,3   Inactive, 4   Archived
            surveyUser.AddedByUserId = 2;
            surveyUser.EmailAddress = objConfiguration != null ? objConfiguration.Value : "tungtt@devblock.net";
            surveyUser.PhoneNumber = "";
            surveyUser.PasswordQuestion = null;
            surveyUser.PasswordAnswer = null;
            surveyUser.DateConfirmedActive = DateTime.UtcNow;
            surveyUser.CreatedDate = DateTime.UtcNow;
            surveyUser.ModifiedDate = DateTime.UtcNow;
            surveyUser.ModifiedBy = "Survey";
            surveyUser.ModifiedUser = 2;

            parameters.UserServices.SaveUser(surveyUser);

            return surveyUser.Id;
        }

        private int CreateSurveyDistrictTerm(int districtId, string strDistrictTermName)
        {
            var objPreference = parameters.PreferencesService.GetPreferenceByLevelLabelAndId(ContaintUtil.TestPreferenceLevelDistrict, ContaintUtil.TestPreferenceLabelDefaultStartMonth, districtId);
            if (objPreference == null)
            {
                objPreference = parameters.PreferencesService.GetPreferenceByLevelLabelAndId(ContaintUtil.TestPreferenceLevelEnterprise, ContaintUtil.TestPreferenceLabelDefaultStartMonth, 0);
            }
            //Logic
            var vCurrentMonth = DateTime.UtcNow.Month;
            int iStartMonth = 0;
            int.TryParse(objPreference.Value, out iStartMonth);
            DateTime dtStartDate;
            DateTime dtEndDate;
            if (vCurrentMonth >= iStartMonth)
            {
                dtStartDate = new DateTime(DateTime.UtcNow.Year, iStartMonth, 1);
                dtEndDate = new DateTime(DateTime.UtcNow.Year + 1, iStartMonth, 1);
                dtEndDate = dtEndDate.AddSeconds(-1);
            }
            else
            {
                dtStartDate = new DateTime(DateTime.UtcNow.Year - 1, iStartMonth, 1);
                dtEndDate = new DateTime(DateTime.UtcNow.Year, iStartMonth, 1);
                dtEndDate = dtEndDate.AddSeconds(-1);
            }

            var tmpDistrictTerm = new DistrictTerm();
            tmpDistrictTerm.Name = strDistrictTermName;
            tmpDistrictTerm.DistrictID = districtId;
            tmpDistrictTerm.Code = "Survey";
            tmpDistrictTerm.CreatedByUserID = 2;
            tmpDistrictTerm.UpdatedByUserID = 2;
            tmpDistrictTerm.DateCreated = DateTime.UtcNow;
            tmpDistrictTerm.DateUpdated = DateTime.UtcNow;
            tmpDistrictTerm.ModifiedUser = 2;
            tmpDistrictTerm.ModifiedBy = "Survey";

            tmpDistrictTerm.DateStart = dtStartDate;
            tmpDistrictTerm.DateEnd = dtEndDate;

            parameters.DistrictTermService.Save(tmpDistrictTerm);
            return tmpDistrictTerm.DistrictTermID;
        }

        private int CreateSurveyClass(int distritId, int schoolId, int districtTermId, int userId)
        {
            var tmpClass = new Class();
            tmpClass.Course = string.Empty;
            tmpClass.CourseNumber = string.Empty;
            tmpClass.Section = string.Empty;
            tmpClass.Locked = false;
            tmpClass.Name = "Survey Class";
            tmpClass.DistrictTermId = districtTermId;
            tmpClass.SchoolId = schoolId;
            tmpClass.UserId = userId;
            tmpClass.DistrictId = distritId;
            tmpClass.ClassType = 1; //default value
            tmpClass.CreatedDate = DateTime.UtcNow;
            tmpClass.ModifiedDate = DateTime.UtcNow;
            tmpClass.ModifiedBy = "Survey";
            tmpClass.ModifiedUser = 2;

            parameters.ClassService.SurveyCreateClass(tmpClass);
            return tmpClass.Id;
        }

        private QTITestClassAssignmentData CreateSurveyRosterAssignment(int districtId, int classId, int virtualtestId)
        {
            //1 Create QTITestClassAssignment
            var qtiTest = new QTITestClassAssignmentData()
            {
                VirtualTestId = virtualtestId,
                ClassId = classId,
                AssignmentDate = DateTime.UtcNow,
                Code = GenerateClassTestCode(),
                CodeTimestamp = DateTime.UtcNow,
                AssignmentGuId = Guid.NewGuid().ToString(),
                TestSetting = string.Empty,
                Status = 1,
                ModifiedDate = DateTime.UtcNow,
                ModifiedUserId = 2,
                Type = (int)AssignmentType.Roster,
                ModifiedBy = "Survey",
                TutorialMode = 0,
                DistrictID = districtId
            };
            parameters.QTITestClassAssignmentService.AssignClass(qtiTest);
            //2 Create Preferences
            SaveSettingPreference(qtiTest.QTITestClassAssignmentId, districtId, virtualtestId);
            return qtiTest;
        }

        private int CreateSurveyStudent(int districtId)
        {
            Student objStudent = new Student();
            objStudent.Code = DateTime.UtcNow.Ticks.ToString();
            objStudent.DistrictId = districtId;
            objStudent.FirstName = "Participant " + string.Format("{0:yyMMddHHmmss}", DateTime.UtcNow);
            objStudent.MiddleName = string.Empty;
            objStudent.LastName = "Survey";
            objStudent.AltCode = string.Empty;
            objStudent.Status = 2;//1 - active, 2 - inactive

            objStudent.CreatedDate = DateTime.UtcNow;
            objStudent.ModifiedDate = DateTime.UtcNow;
            objStudent.ModifiedBy = "Survey";
            objStudent.ModifiedUser = 2;

            parameters.StudentService.SurveyInsertStudent(objStudent);
            return objStudent.Id;
        }
        private int CreateSurveyPublicAnonymousStudent(int districtId)
        {
            var student = new Student()
            {
                Code = DateTime.UtcNow.Ticks.ToString(),
                DistrictId = districtId,
                FirstName = "Survey",
                MiddleName = string.Empty,
                LastName = "Participant",
                AltCode = string.Empty,
                Status = 3,//1 - active, 2 - inactive

                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "Survey",
                ModifiedUser = 2
            };
            parameters.StudentService.SurveyInsertStudent(student);
            return student.Id;
        }
        private string GenerateClassTestCode()
        {
            var objCon = parameters.ConfigurationService.GetConfigurationByKey(Constanst.ClassTestCodeLength);
            int iTestCodeLength = 5;
            if (objCon != null)
            {
                iTestCodeLength = CommonUtils.ConverStringToInt(objCon.Value, 5);
            }

            string strTestCodePrefix = ConfigurationManager.AppSettings["SurveyTestCodePrefix"] ?? "001";
            var testCode = parameters.TestCodeGenerator.GenerateTestCode(iTestCodeLength, strTestCodePrefix);
            return testCode;
        }

        private void SaveSettingPreference(int id, int districtId, int testId)
        {
            string assignmentPreferenceString = string.Empty;
            var preferenceTest = parameters.PreferencesService.GetPreferenceByLevelAndID(testId, ContaintUtil.TestPreferenceLevelTest);
            if (preferenceTest != null)
            {
                assignmentPreferenceString = preferenceTest.Value;
            }
            else
            {
                var preferenceDistrict = parameters.PreferencesService.GetPreferenceByLevelAndId(2, districtId, ContaintUtil.TestPreferenceLevelDistrict);
                if (preferenceDistrict != null)
                {
                    assignmentPreferenceString = preferenceDistrict.Value;
                }
                else
                {
                    var preferenceEnterprise = parameters.PreferencesService.GetPreferenceByLevelAndId(2, districtId, ContaintUtil.TestPreferenceLevelEnterprise);
                    assignmentPreferenceString = preferenceEnterprise.Value;
                }
            }
            var preferences = new Preferences()
            {
                Id = id,
                Level = ContaintUtil.TestPreferenceLevelTestAssignment,
                Label = ContaintUtil.TestPreferenceLabelTest,
                Value = assignmentPreferenceString
            };
            parameters.PreferencesService.SaveAssignment(preferences);
        }

        private string RoleGetDistrictTerm(int districtId, string strDistrictName)
        {
            var objPreference = parameters.PreferencesService.GetPreferenceByLevelLabelAndId(ContaintUtil.TestPreferenceLevelDistrict, ContaintUtil.TestPreferenceLabelDefaultStartMonth, districtId);
            if (objPreference == null)
            {
                objPreference = parameters.PreferencesService.GetPreferenceByLevelLabelAndId(ContaintUtil.TestPreferenceLevelEnterprise, ContaintUtil.TestPreferenceLabelDefaultStartMonth, 0);
            }
            //Logic
            var vCurrentMonth = DateTime.UtcNow.Month;
            int iStartMonth = 0;
            int.TryParse(objPreference.Value, out iStartMonth);
            DateTime dtStartDate;
            DateTime dtEndDate;
            if (vCurrentMonth >= iStartMonth)
            {
                dtStartDate = new DateTime(DateTime.UtcNow.Year, iStartMonth, 1);
                dtEndDate = new DateTime(DateTime.UtcNow.Year + 1, iStartMonth, 1);
                dtEndDate = dtEndDate.AddSeconds(-1);
            }
            else
            {
                dtStartDate = new DateTime(DateTime.UtcNow.Year - 1, iStartMonth, 1);
                dtEndDate = new DateTime(DateTime.UtcNow.Year, iStartMonth, 1);
                dtEndDate = dtEndDate.AddSeconds(-1);
            }
            return string.Format("Survey District Term {0} {1} - {2}", strDistrictName, dtStartDate.Year, dtEndDate.Year);
        }

        private void LoadUserMetaData(User currentUser)
        {
            var userMeta = userMetaService.GetByUserId(currentUser.Id, UserMetaLabelConst.NOTIFICATION);
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
                userMetaService.Save(userMeta);
            }
            currentUser.UserMetaValue = userMeta.UserMetaValue;
        }
        #endregion

        public ActionResult NYCSignIn(string loginType = "")
        {
            var configuration = SAMLConfigurationBuilder.GetConfiguration(SSOClient.Nyc);

            var subdomain = HelperExtensions.GetSubdomain().ToLower();
            string request = SAML20Assertion.CreateSsoAuthenticationRequest(configuration);
            string encodedRequest = Server.UrlEncode(request);
            var ssoTargetUrl = configuration.SsoTargetUrl;

            var relayState = $"subdomain={subdomain}&loginType={loginType}";
            string encodedRelayState = Server.UrlEncode(relayState);

            var urlRedirect = $"{ssoTargetUrl}?SAMLRequest={encodedRequest}&RelayState={encodedRelayState}";

            return Redirect(urlRedirect);
        }

        [HttpPost]
        public ActionResult NYCCallback(string SAMLResponse, string RelayState)
        {
            var queryParams = HttpUtility.ParseQueryString(RelayState);
            string subdomain = queryParams["subdomain"];
            string logintype = queryParams["logintype"] ?? string.Empty;

            var callbackUrl = $"https://{subdomain}.{ConfigurationManager.AppSettings["LinkItUrl"]}/account/nyccallbackprocessing";
            string encodedSAMLResponse = Server.UrlEncode(SAMLResponse);
            var urlRedirect = $"{callbackUrl}?SAMLResponse={encodedSAMLResponse}&logintype={logintype}";

            return Redirect(urlRedirect);
        }
    }
}
