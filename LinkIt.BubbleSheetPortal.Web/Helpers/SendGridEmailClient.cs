using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using LinkIt.BubbleSheetPortal.Web.Models;
using System.Net.Mail;
using SendGridMail;
using SendGridMail.Transport;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class SendGridEmailClient
    {
        private const string Username = "linkitJosh";
        private const string Password = "vanadiumPi11ar";

        public void Send(List<SendGridEmail> messages)
        {
            foreach (var message in messages)
            {
                try
                {
                    var myMessage = CreateSendGridMessage(message);

                    // Create credentials, specifying your user name and password.
                    var credentials = new NetworkCredential(Username, Password);

                    // Create an SMTP transport for sending email.
                    var transportSmtp = SMTP.GetInstance(credentials);

                    // Send the email.

                    transportSmtp.Deliver(myMessage);

                    message.SendResult = 1;
                }
                catch (Exception)
                {
                    message.SendResult = 0;
                }
            }
        }
        public string Send(SendGridEmail message)
        {
            try
            {
                var myMessage = CreateSendGridMessage(message);

                // Create credentials, specifying your user name and password.
                var credentials = new NetworkCredential(Username, Password);

                // Create an SMTP transport for sending email.
                var transportSmtp = SMTP.GetInstance(credentials);

                // Send the email.

                transportSmtp.Deliver(myMessage);

                message.SendResult = 1;
                return string.Empty;//success
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                message.SendResult = 0;
                return ex.Message;//error information
            }
        }
        private SendGrid CreateSendGridMessage(SendGridEmail message)
        {
            // Create the email object first, then add the properties.
            SendGrid myMessage = SendGridMail.SendGrid.GetInstance();

            myMessage.AddTo(message.To);
            myMessage.From = new MailAddress(message.From, message.Alias);
            myMessage.Subject = message.Subject;
            myMessage.Text = message.PlainTextBody;

            if (!string.IsNullOrEmpty(message.HtmlBody))
            {
                myMessage.Html = message.HtmlBody;
            }

            if (!string.IsNullOrEmpty(message.BCC))
            {
                var recipients = new List<string>(message.BCC.Split(';')) { message.To };
                myMessage.Header.AddTo(recipients);
            }

            if (!string.IsNullOrEmpty(message.PlainTextFooter) || !string.IsNullOrEmpty(message.HtmlFooter))
            {
                myMessage.EnableFooter(message.PlainTextFooter, message.HtmlFooter);
            }

            if (message.File != null)
            {
                myMessage.AddAttachment(message.File.InputStream, message.File.FileName);
            }

            return myMessage;
        }
    }
}
