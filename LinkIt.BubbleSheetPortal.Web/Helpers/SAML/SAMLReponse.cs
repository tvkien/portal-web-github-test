using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.SAML
{
    public class SAMLReponse
    {
        public DateTime NotBefore { get; set; }
        public DateTime NotOnOrAfter { get; set; }
        public string TenantId { get; set; }
        public string ObjectIdentifier { get; set; }
        public string StatusCode { get; set; }
        public string UserId { get; set; }
        public string SectorId { get; set; }
    }

    public class NYCSamlResponse
    {
        public string CertString { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
