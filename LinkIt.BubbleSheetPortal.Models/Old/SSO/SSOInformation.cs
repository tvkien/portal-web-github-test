using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.SSO
{
    public class SSOInformation
    {
        public int SSOInformationID { get; set; }
        public string Auth0ClientId { get; set; }
        public string Auth0ClientSecret { get; set; }
        public string UrlLandingPage { get; set; }
        public string UrlLogoutPage { get; set; }
        public string DefaultConnection { get; set; }
        public virtual List<SSODistrictGroup> SSODistrictGroup { get; set; }
    }
}
