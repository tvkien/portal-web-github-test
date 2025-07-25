namespace LinkIt.BubbleSheetPortal.Web.Helpers.SAML
{
    public class SAMLConfiguration
    {
        public string CertString { get; set; }
        public string LoginRedirectUrl { get; set; }
        public string UserIdKey { get; set; }
        public string RoleKey { get; set; }
        public string SsoTargetUrl { get; set; }
        public string Issuer { get; set; }
        public SSOClient Client { get; set; }
        public string SectorID { get; set; }
        public string LogoutRedirectUrl { get; set; }
    }
}