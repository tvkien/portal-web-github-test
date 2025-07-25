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
    public class SmsSender
    {
        private string _currentUserDistrictName = string.Empty;
        private string _senderFirstName = string.Empty;
        private string _senderLastName = string.Empty;
        private string _subject = string.Empty;
        private string _messageTemplate = string.Empty;
        private string _portalLink = string.Empty;
        TwilioSmsClient client = new TwilioSmsClient();
        private string _fromNumber = string.Empty;
        public SmsSender(DistrictConfigurationService districtConfigurationService,DistrictService districtService,UserService userService, int districtId, int currentUserId, string subject)
        {
            DistrictConfiguration messageConfig = districtConfigurationService.GetDistrictConfigurationByKey(districtId, "PC-SMS-Content");
            if (messageConfig != null)
            {
                _messageTemplate = messageConfig.Value.IsNull() ? string.Empty : messageConfig.Value;
            }
            DistrictConfiguration portalLinkConfig = districtConfigurationService.GetDistrictConfigurationByKey(districtId, "PC-PortalLink");
            if (portalLinkConfig != null)
            {

                _portalLink = portalLinkConfig.Value.IsNull() ? string.Empty : portalLinkConfig.Value;
            }

            District district = districtService.GetDistrictById(districtId);
            _currentUserDistrictName = district.Name;
            _subject = subject.IsNull() ? string.Empty : subject;
            User currentUser = userService.GetUserById(currentUserId);//CurrentUser.FirstName and LastName has no value so that we have to get from database
            this._senderFirstName = currentUser.FirstName.IsNull() ? string.Empty : currentUser.FirstName;
            this._senderLastName = currentUser.LastName.IsNull() ? string.Empty : currentUser.LastName;
            try
            {
                this._fromNumber = System.Configuration.ConfigurationManager.AppSettings["SMSFromPhoneNumber"];
            }
            catch (Exception)
            {
                this._fromNumber = string.Empty;
            }

        }
        public string FromNumber
        {
            get { return _fromNumber; }
        }
        public string Send(string toNumber)
        {
            if (string.IsNullOrEmpty(toNumber))
            {
                return "Receiving number is empty!";
            }
            var message =
                new TwilioSms()
                {
                    Message = GenereateMessage,
                    PhoneNumber = toNumber,
                };


            return client.Send(message, this.FromNumber);

        }

        private string _generatedMessage;
        public string GenereateMessage
        {
            get
            {
                if (_generatedMessage.IsNull())
                {
                    //[DISTRICT_NAME] - [SENDER_FIRST_NAME] [SENDER_LAST_NAME] - RE: [SUBJECT]. Please log into LinkIt!: [PORTAL_LINK] to view, acknowledge and reply to this message.
                    //With current template, need only one time to generate message

                    _generatedMessage = _messageTemplate.Replace("[DISTRICT_NAME]", _currentUserDistrictName);
                    _generatedMessage = _generatedMessage.Replace("[SENDER_FIRST_NAME]", _senderFirstName);
                    _generatedMessage = _generatedMessage.Replace("[SENDER_LAST_NAME]", _senderLastName);
                    _generatedMessage = _generatedMessage.Replace("[SUBJECT]", _subject);
                    _generatedMessage = _generatedMessage.Replace("[PORTAL_LINK]", _portalLink);//may use bitly service here
                    return _generatedMessage;
                }
                else
                {

                    return _generatedMessage;
                }
            }
        }
    }
}