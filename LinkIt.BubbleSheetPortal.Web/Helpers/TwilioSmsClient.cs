using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Web.Models;
using LinkIt.BubbleSheetPortal.Web.Security;
using Twilio;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class TwilioSmsClient
    {
        private const string AccountSid = "AC7e07c4696e615e9addc0b9b602ad405d";
        private const string AuthToken = "b8aebee59c0878ee6f83da2f5d097f76";
        public const string FromPhoneNumber = "+14322424918";

        public void Send(List<TwilioSms> messages)
        {
            var twilio = new TwilioRestClient(AccountSid, AuthToken);

            foreach (var message in messages)
            {
                try
                {
                    var result = twilio.SendSmsMessage(FromPhoneNumber, message.PhoneNumber, message.Message, "");
                    if (result.Status != null)
                        message.SendResult = 1;
                }catch(Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                }
            }
        }
        public void Send(List<TwilioSms> messages,string fromPhoneNumber)
        {
            var twilio = new TwilioRestClient(AccountSid, AuthToken);

            foreach (var message in messages)
            {
                try
                {
                    var result = twilio.SendSmsMessage(fromPhoneNumber, message.PhoneNumber, message.Message, "");
                    if (result.Status != null)
                        message.SendResult = 1;
                }
                catch (Exception ex)
                {
                }
            }
        }
        public string Send(TwilioSms message, string fromPhoneNumber)
        {
            var twilio = new TwilioRestClient(AccountSid, AuthToken);
            try
            {
                var result = twilio.SendSmsMessage(fromPhoneNumber, message.PhoneNumber, message.Message, "");
                if (result.Status != null)
                {
                    message.SendResult = 1;
                    return string.Empty;//success
                }
                return result.RestException.Message;//error happend

            }
            catch (Exception ex)
            {
                return ex.Message;//error information
            }
        }
    }
}
