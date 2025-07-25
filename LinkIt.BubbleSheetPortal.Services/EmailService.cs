using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using LinkIt.BubbleSheetPortal.Models;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Repositories.ManageParent;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.Old.Configugration;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class EmailService
    {
        private readonly UserService _userService;
        private readonly DistrictService _districtService;
        private readonly ConfigurationService _configurationService;
        private readonly DistrictDecodeService _districtDecodeService;
        public const string ForgotPasswordEmailTemplate = "TemplateForgotPassword";
        private readonly IManageParentRepository _manageParentRepository;
        private readonly StudentService _studentService;
        private readonly SchoolService _schoolService;

        public EmailService(UserService userService, DistrictService districtService, ConfigurationService configurationService,
            DistrictDecodeService districtDecodeService, IManageParentRepository manageParentRepository, StudentService studentService, SchoolService schoolService)
        {
            this._userService = userService;
            this._districtService = districtService;
            this._configurationService = configurationService;
            this._districtDecodeService = districtDecodeService;
            this._manageParentRepository = manageParentRepository;
            this._studentService = studentService;
            this._schoolService = schoolService;
        }

        public void SendTokenResetPassword(User user, string token, EmailCredentialSetting emailCredentialSetting)
        {
            var message = GenerateEmail(user, token);
            var mailClient = SetupMailClient(emailCredentialSetting);
            mailClient.Send(message);
        }

        public string GetForgotPasswordEmailTemplate(int districtId, string urlLogin, string token, string name)
        {
            var template = string.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                template = "<p>Hi " + name + ",</p>";
            }
            template += "<p>You recently requested to reset you password for your LinkIt account. Click the button bellow to reset it.</p>";
            template += "<p style=\"margin: 30px 0\"><a style=\"background: #ff494c;color: white;padding: 16px 40px;font-size: 12pt;text-decoration: none;border-radius: 2px;font-weight: bold;\" href=\"[TokenURL]\">Click here to reset your password</a></p>";
            template += "<p>Thanks,</p><p>The LinkIt Team</p>";

            var districtDecode = _districtDecodeService.GetDistrictDecodesByLabel(ForgotPasswordEmailTemplate)
                .FirstOrDefault(m => m.DistrictID == districtId);

            if (districtDecode != null && districtDecode.Value.Contains("[TokenURL]"))
            {
                template = districtDecode.Value;
            }
            else
            {
                var configuration = _configurationService.GetConfigurationByKey(ForgotPasswordEmailTemplate);

                if (configuration != null && configuration.Value.Contains("[TokenURL]"))
                {
                    template = configuration.Value;
                }
            }

            template = template.Replace("[TokenURL]", "{0}").Replace("[FirstName]", "{1}");

            string strBody = string.Format(template, token, name);
            return strBody;
        }


        public void SendPortalWarningEmail(string mailTo, string subject, string body, EmailCredentialSetting emailCredentialSetting)
        {
            var message = GeneratePortalWarningEmail(mailTo, subject, body);
            var mailClient = SetupMailClient(emailCredentialSetting);
            mailClient.Send(message);
        }

        private MailMessage GeneratePortalWarningEmail(string mailTo, string subject, string body, bool isBodyHtml = true)
        {
            return new MailMessage(EmailSetting.LinkItFromEmail, mailTo)
            {
                Subject = subject,
                IsBodyHtml = isBodyHtml,
                Body = body
            };
        }

        public IEnumerable<int> SendMailRegistrationCodeToStudent(int districtId, StudentRegistrationCodeEmailModel objStudentEmail, EmailCredentialSetting emailCredentialSetting)
        {
            if ((objStudentEmail?.StudentEmailModels?.Count() ?? 0) == 0)
                yield break;

            var mailClient = InitMailClientStudentParent(emailCredentialSetting);
            var vSubject = EmailSetting.SubjectRegistrationStudent ?? "LinkIt Student Portal Account Registration"; ;
            string strBody = string.Empty;

            var allowStudentUserGeneration = _districtDecodeService.GetDistrictDecodeOrConfigurationByLabel(districtId, Constanst.ALLOW_STUDENT_USER_GENERATION);

            foreach (var studentInfo in objStudentEmail.StudentEmailModels)
            {
                try
                {
                    var student = _studentService.GetStudentDataForRegistrationCodeEmail(studentInfo.StudentId);
                    int districtID = student == null ? 0 : student.DistrictId;
                    var districtInfo = _districtService.GetDistrictById(districtID);
                    var schoolName = student.AdminSchoolId.HasValue ? _schoolService.GetSchoolById(student.AdminSchoolId.Value)?.Name ?? "" : "";

                    if (student != null && districtInfo != null)
                    {
                        string studentPortalUrl = string.Format("{0}://{1}.{2}/Student", objStudentEmail.HTTPProtocal, districtInfo.LICode, ConfigurationManager.AppSettings["LinkItUrl"]);
                        var objTemplateEmail = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(districtID, Constanst.TemplateStudentRegistrationCode);
                        var userDesc = allowStudentUserGeneration && !string.IsNullOrEmpty(studentInfo.UserName)
                            ? $" and your username is <b>{studentInfo.UserName}</b>" : string.Empty;

                        var keyPairValues = new Dictionary<string, string>()
                        {
                            { "[StudentFirstName]",student.FirstName },
                            { "[DistrictName]",districtInfo.Name},
                            { "[StudentPortalURL]",studentPortalUrl },
                            { "[RegistrationCode]",studentInfo.RegistrationCode },
                            { "[StudentCurrentSchoolName]",schoolName },
                            { "[UserFirstName]",student.FirstName },
                            { "[UserLastName]",student.LastName },
                            { "[UserDesc]", userDesc}
                        };

                        if (objTemplateEmail != null)
                        {
                            strBody = objTemplateEmail.Value;
                            foreach (var keyPairValue in keyPairValues)
                            {
                                strBody = strBody.Replace(keyPairValue.Key, keyPairValue.Value);
                                vSubject = vSubject.Replace(keyPairValue.Key, keyPairValue.Value);
                            }
                        }
                        var message = new MailMessage(EmailSetting.FromRegistration, student.Email)
                        {
                            Subject = vSubject,
                            IsBodyHtml = true,
                            Body = strBody
                        };
                        mailClient.Send(message);
                    }
                }
                catch (Exception ex) { }
                yield return studentInfo.StudentId;

            }
        }

        private MailMessage GenerateEmail(User user)
        {
            var linkitUrl = ConfigurationManager.AppSettings["LinkItUrl"];
            var district = _districtService.GetDistrictById(user.DistrictId.GetValueOrDefault(-1));
            var licode = district == null ? "demo" : district.LICode.ToLower();
            var url = string.Format("{0}.{1}", licode, linkitUrl);
            return new MailMessage(EmailSetting.LinkItFromEmail, user.EmailAddress)
            {
                Subject = "Your LinkIt! Password has been reset",
                IsBodyHtml = true,
                Body = "Your LinkIt! password has been reset. Please navigate to " + url + " and use the following temporary password: " + _userService.SetTemporaryPassword(user)
            };
        }

        private MailMessage GenerateEmail(User user, string token)
        {
            var linkitUrl = ConfigurationManager.AppSettings["LinkItUrl"];
            var district = _districtService.GetDistrictById(user.DistrictId.GetValueOrDefault(-1));
            var licode = district == null ? "demo" : district.LICode.ToLower();
            var url = string.Format("{0}.{1}", licode, linkitUrl);

            if (string.IsNullOrEmpty(token))
            {
                token = _userService.SetTemporaryPassword(user);
            }
            var body = GetForgotPasswordEmailTemplate(user.DistrictId.GetValueOrDefault(), url, token, user.FirstName);
            return new MailMessage(EmailSetting.LinkItFromEmail, user.EmailAddress)
            {
                Subject = "Complete your Linkit! Password Reset",
                IsBodyHtml = true,
                Body = body
            };
        }

        private MailMessage GenerateStudentEmail(User user)
        {
            return new MailMessage(EmailSetting.LinkItFromEmail, user.EmailAddress)
            {
                Subject = "Your LinkIt! Password has been reset",
                IsBodyHtml = true,
                Body = "Your LinkIt! password has been reset. Please navigate to https://chyten.linkit.com/studentlogin and use the following temporary password: " + _userService.SetTemporaryPassword(user)
            };
        }

        private MailMessage GenerateStudentPortalEmail(User user, string url)
        {
            return new MailMessage(EmailSetting.LinkItFromEmail, user.EmailAddress)
            {
                Subject = "Your LinkIt! Password has been reset",
                IsBodyHtml = true,
                Body = "Your LinkIt! password has been reset. Please navigate to " + url + " and use the following temporary password: " + _userService.SetTemporaryPassword(user)
            };
        }

        public SmtpClient SetupMailClient(EmailCredentialSetting emailCredentialSetting)
        {
            var mailClient = new SmtpClient(emailCredentialSetting.Host, emailCredentialSetting.Port)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            if (EmailSetting.SendMailUsingCredential)
            {
                mailClient.Credentials = new NetworkCredential(emailCredentialSetting.UserName, emailCredentialSetting.Password);
            }

            return mailClient;
        }

        public IEnumerable<int> SendMailWhenRegistrationCode(ParentStudentEmailModel parentStudentEmailModel, EmailCredentialSetting emailCredentialSetting)
        {
            if ((parentStudentEmailModel?.ListUserRegistrationCode?.Count ?? 0) == 0)
                yield break;

            var mailClient = InitMailClientStudentParent(emailCredentialSetting);
            var vSubject = EmailSetting.SubjectRegistration ?? "LinkIt Parent Portal Account Registration";
            string strBody = string.Empty;

            foreach (var userRegistration in parentStudentEmailModel.ListUserRegistrationCode)
            {
                try
                {
                    var emailMacros = _manageParentRepository.GetParentsInformationForDistributeRegistrationCode(userRegistration.UserId);

                    if (emailMacros != null)
                    {
                        string strParentLoginUrl = string.Format("{0}://{1}.{2}/Parent", parentStudentEmailModel.HTTPProtocal, emailMacros.LICode, ConfigurationManager.AppSettings["LinkItUrl"]);
                        var objTemplateEmail = _districtDecodeService.GetDistrictDecodeOfDistrictOrConfigurationByLabel(emailMacros.DistrictId ?? 0, Constanst.TemplateStudentParentRegistrationCode);


                        var keyPairValues = new Dictionary<string, string>()
                        {
                            { "[StudentFirstName]",emailMacros.StudentFirstName },
                            { "[DistrictName]",emailMacros.DistrictName},
                            { "[ParentPortalURL]",strParentLoginUrl },
                            { "[RegistrationCode]",emailMacros.RegistrationCode },
                            { "[StudentCurrentSchoolName]",emailMacros.StudentCurrentSchoolName },
                            { "[UserFirstName]",emailMacros.UserFirstName },
                            { "[UserLastName]",emailMacros.UserLastName },
                        };
                        if (objTemplateEmail != null)
                        {
                            strBody = objTemplateEmail.Value;
                            foreach (var keyPairValue in keyPairValues)
                            {
                                strBody = strBody.Replace(keyPairValue.Key, keyPairValue.Value);
                                vSubject = vSubject.Replace(keyPairValue.Key, keyPairValue.Value);
                            }
                        }
                        var message = new MailMessage(EmailSetting.FromRegistration, emailMacros.EmailAddress)
                        {
                            Subject = vSubject,
                            IsBodyHtml = true,
                            Body = strBody
                        };
                        mailClient.Send(message);
                    }
                }
                catch (Exception ex) { }
                yield return userRegistration.UserId;

            }
        }

        public SmtpClient InitMailClientStudentParent(EmailCredentialSetting emailCredentialSetting)
        {
            var mailClient = new SmtpClient(emailCredentialSetting.Host, emailCredentialSetting.Port);
            mailClient.EnableSsl = true;

            if (EmailSetting.SendMailUsingCredentialRegistration)
            {
                mailClient.Credentials = new NetworkCredential(emailCredentialSetting.UserName, emailCredentialSetting.Password);
            }
            else
            {
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            }
            return mailClient;
        }

        private SmtpClient NavigatorMailClient(EmailCredentialSetting emailCredentialSetting)
        {
            var mailClient = new SmtpClient(emailCredentialSetting.Host, emailCredentialSetting.Port);
            mailClient.EnableSsl = true;

            if (EmailSetting.SendMailUsingCredentialNavigator)
            {
                mailClient.Credentials = new NetworkCredential(emailCredentialSetting.UserName, emailCredentialSetting.Password);
            }
            else
            {
                mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            }
            return mailClient;
        }

        public void SendEmailNavigator(NavigatorReportEmailModelDTO emailModel, EmailCredentialSetting emailCredentialSetting)
        {
            MailMessage message = new MailMessage
            {
                From = new MailAddress(EmailSetting.SmtpFromNavigator),
                IsBodyHtml = true,

                Subject = emailModel.Subject,
                Body = emailModel.Body
            };
            if (emailModel.ArrEmailTo != null && emailModel.ArrEmailTo.Count > 0)
            {
                foreach (string strEmail in emailModel.ArrEmailTo)
                {
                    if (CommonUtils.IsValidEmail(strEmail))
                    {
                        message.To.Add(strEmail);
                    }
                }
            }
            if (emailModel.ArrEmailCC != null && emailModel.ArrEmailCC.Count > 0)
            {
                foreach (string strEmailCC in emailModel.ArrEmailCC)
                {
                    if (CommonUtils.IsValidEmail(strEmailCC))
                    {
                        message.CC.Add(strEmailCC);
                    }
                }
            }
            if (emailModel.ArrEmailBCC != null && emailModel.ArrEmailBCC.Count > 0)
            {
                foreach (string strEmailBCC in emailModel.ArrEmailBCC)
                {
                    if (CommonUtils.IsValidEmail(strEmailBCC))
                    {
                        message.Bcc.Add(strEmailBCC);
                    }
                }
            }
            var smtpClientReport = NavigatorMailClient(emailCredentialSetting);
            smtpClientReport.Send(message);

        }

    }
}
