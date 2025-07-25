using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Envoc.Core.Shared.Extensions;
using FluentValidation.Results;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    public class StudentPassThroughController : BaseController
    {
        private readonly PassThroughControllerParameters parameters;
        public StudentPassThroughController(PassThroughControllerParameters passThroughControllerParameters)
        {
            this.parameters = passThroughControllerParameters;
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

            var passThrough = parsePassThrought(HttpUtility.HtmlDecode(q), apiAccount.LinkitPrivateKey);
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



        private ActionResult PassThroughLogin(StudentPassThroughViewModel passThrough)
        {
            int userId = passThrough.UserID;
            var user = parameters.UserServices.GetUserById(userId);
            parameters.AuthenticationServices.SignOut();
            parameters.AuthenticationServices.SignIn(user, false, true);

            var districtSubDomain = "portal";
            var district = parameters.DistrictServices.GetDistrictById(user.DistrictId.Value);
            districtSubDomain = district.IsNull() ? districtSubDomain : district.LICode.ToLower();

            var redirectUrl = HelperExtensions.GetStartUrlForAuthenticatedUser(HelperExtensions.GetHTTPProtocal(Request), districtSubDomain,
                user);
            WriteCookiesPassThrough(passThrough.RedirectUrl, userId);

            return Redirect(redirectUrl);
        }

        #region NonAction
        [NonAction]
        private int ValidParameter(StudentPassThroughViewModel passThrough, PassThroughMessage obj, APIAccount apiAccount)
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
            if (string.IsNullOrEmpty(passThrough.StudentID))
            {
                bProcessFault = true;
                obj.MessageError = "Data not found!";
            }
            else
            {
                int studentId = 0;
                int.TryParse(passThrough.StudentID, out studentId);
                var student = parameters.StudentService.GetStudentById(studentId);

                if (student == null)
                {
                    bProcessFault = true;
                    obj.MessageError = "Student is not existed!";
                }
                else
                {                     
                    var userId = parameters.StudentService.GetUserIdViaStudentUser(student.Id);  
                    if (userId == 0)
                    {
                        bProcessFault = true;
                        obj.MessageError = "Student is not registered!";
                    }
                    else
                    {                        
                        user = parameters.UserServices.GetUserById(userId);
                        if (user.IsNull() || !user.DistrictId.HasValue || user.UserStatusId != (int)UserStatus.Active
                       || user.RoleId == (int)Permissions.Publisher || !ValidUserRole(apiAccount, user))
                        {
                            bProcessFault = true;
                            obj.MessageError = "User not authorized!";
                        }
                        else
                        {
                            passThrough.UserID = user.Id;
                        }
                    }
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
        private StudentPassThroughViewModel parsePassThrought(string strData, string strPrivateKey)
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
                    var obj = new JavaScriptSerializer().Deserialize<StudentPassThroughViewModel>(tmp);
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
        private APIAccount GetPrivateKey(string accessKey)
        {
            string pathAndQuery = Request.Url.ToString().ToLower();
            APIAccount apiAccount = parameters.APIAccountServices.GetAPIAccountByClientAccessKey(accessKey);
            if (apiAccount != null)
            {
                //TODO: valid permission
                List<int> listFunctionId = parameters.APIPermissionServices.GetAPIPermissionByTaget(apiAccount.TargetID);
                if (parameters.APIFunctionServices.CheckValidURL(listFunctionId, pathAndQuery))
                    return apiAccount;
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
                                case (int)Permissions.Student:
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

        #endregion
    }
}
