using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Models;
namespace LinkIt.BubbleSheetPortal.Web.Models.ParentConnect
{
    public class EmailSender
    {
        private string _emailSubjectTemplate = string.Empty;
        private string _emailBodyTemplate = string.Empty;
        private string _emailFooterTemplate = string.Empty;
        private string _currentUserDistrcitName = string.Empty;
        private string _fromSenderEmail = string.Empty;
        private string _senderFirstName = string.Empty;
        private string _senderLastName = string.Empty;
        private string _alias = string.Empty;
        private string _subject = string.Empty;
        private string _portalLink = string.Empty;

        SendGridEmailClient client = new SendGridEmailClient();

        public EmailSender(DistrictConfigurationService districtConfigurationService, DistrictService districtService,UserService userService,int districtId, int currentUserId, string alias, string subject)
        {

            DistrictConfiguration emailSubjectConfig = districtConfigurationService.GetDistrictConfigurationByKey(districtId, "PC-Email-Subject");
            DistrictConfiguration emailBodyConfig = districtConfigurationService.GetDistrictConfigurationByKey(districtId, "PC-Email-Body");
            DistrictConfiguration emailFooterConfig = districtConfigurationService.GetDistrictConfigurationByKey(districtId, "PC-Email-Footer");
            DistrictConfiguration portalLinkConfig = districtConfigurationService.GetDistrictConfigurationByKey(districtId, "PC-PortalLink");
            DistrictConfiguration emailFromConfig = districtConfigurationService.GetDistrictConfigurationByKey(districtId, "PC-Email-From");

            District district = districtService.GetDistrictById(districtId);
            if (emailSubjectConfig != null)
            {
                _emailSubjectTemplate = emailSubjectConfig.Value.IsNull() ? string.Empty : emailSubjectConfig.Value;
            }

            if (emailBodyConfig != null)
            {
                _emailBodyTemplate = emailBodyConfig.Value.IsNull() ? string.Empty : emailBodyConfig.Value;
            }
            if (emailFooterConfig != null)
            {
                _emailFooterTemplate = emailFooterConfig.Value.IsNull() ? string.Empty : emailFooterConfig.Value;
            }
            if (portalLinkConfig != null)
            {
                _portalLink = portalLinkConfig.Value.IsNull() ? string.Empty : portalLinkConfig.Value;
            }
            if (emailFromConfig != null)
            {
                _fromSenderEmail = emailFromConfig.Value.IsNull() ? string.Empty : emailFromConfig.Value;
            }
            User currentUser = userService.GetUserById(currentUserId);//CurrentUser.FirstName and LastName has no value so that we have to get from database
            this._senderFirstName = currentUser.FirstName.IsNull() ? string.Empty : currentUser.FirstName;
            this._senderLastName = currentUser.LastName.IsNull() ? string.Empty : currentUser.LastName;
            this._subject = subject.IsNull() ? string.Empty : subject;
            this._alias = alias.IsNull() ? string.Empty : alias;
            this._currentUserDistrcitName = district.Name;
        }


        public string SendGridEmail(string receiverFirstName, string receiverLastName, string emailReceiver)
        {
            if (string.IsNullOrEmpty(receiverFirstName))
            {
                receiverFirstName = string.Empty;
            }
            if (string.IsNullOrEmpty(receiverLastName))
            {
                receiverLastName = string.Empty;
            }
            if (string.IsNullOrEmpty(emailReceiver))
            {
                emailReceiver = string.Empty;
            }
            if (string.IsNullOrEmpty(_emailFooterTemplate))
            {
                _emailFooterTemplate = string.Empty;
            }

            string htmlBody = GenerateEmailBody(_senderFirstName, _senderLastName, receiverFirstName, receiverLastName);
            string generatedSubject = GenerateEmailSubject(_subject);

            var message = new SendGridEmail()
            {
                Alias = _alias,
                //BCC = model.BCC,
                //File = model.File,
                From = _fromSenderEmail,
                HtmlBody = htmlBody,
                HtmlFooter = _emailFooterTemplate,
                //PlainTextBody = model.PlainTextBody,
                //PlainTextFooter = model.PlainTextFooter,
                Subject = generatedSubject,
                To = emailReceiver
            };


            string result = string.Empty;
            result = client.Send(message);

            return result;

        }

        private string GenerateEmailSubject(string subject)
        {
            //[DISTRICT_NAME] - [SUBJECT]
            string generatedSubject = _emailSubjectTemplate.Replace("[DISTRICT_NAME]", _currentUserDistrcitName);
            generatedSubject = generatedSubject.Replace("[SUBJECT]", subject);
            return generatedSubject;
        }
        private string GenerateEmailBody(string senderFirstName, string senderLastName, string receiverFirstName, string receiverLastName)
        {
            //<p><em> [FIRST_NAME] [LAST_NAME]:</em></p><p>You&#39;ve received a new message. Please log into LinkIt!: [PORTAL_LINK] to view, acknowledge and reply to this message.</p><p>Thank you,</p><p><em>[SENDER_FIRST_NAME] [SENDER_LAST_NAME]</em></p>
            string generatedBody = _emailBodyTemplate.Replace("[FIRST_NAME]", receiverFirstName);
            generatedBody = generatedBody.Replace("[LAST_NAME]", receiverLastName);
            generatedBody = generatedBody.Replace("[PORTAL_LINK]", _portalLink);
            generatedBody = generatedBody.Replace("[SENDER_FIRST_NAME]", senderFirstName);
            generatedBody = generatedBody.Replace("[SENDER_LAST_NAME]", senderLastName);
            return generatedBody;
        }
    }
}