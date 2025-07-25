using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.SAML
{
    public class SAMLRequest
    {
        public string SectorId { get; set; }
        public string UserId { get; set; }
        public string ActionType { get; set; }
        public string ReturnUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Recipient { get; set; }
        public string UserEmailAddress { get; set; }
        public string LogoutRedirectUrl { get; set; }
    }
}