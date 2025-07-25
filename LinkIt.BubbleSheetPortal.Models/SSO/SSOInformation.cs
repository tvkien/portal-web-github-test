using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.SSO
{
    public class SSOInformation
    {
        public int SSOInformationID { get; set; }
        public string ClientId { get; set; }
        public string SecretId { get; set; }
        public string UrlLandingPage { get; set; }
        public string UrlLogoutPage { get; set; }
        public string DefaultConnection { get; set; }
        public string Type { get; set; }
        public virtual List<SSODistrictGroup> SSODistrictGroup { get; set; }
    }
}
