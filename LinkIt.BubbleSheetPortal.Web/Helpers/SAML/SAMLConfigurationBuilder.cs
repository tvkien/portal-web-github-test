using System;
using System.Configuration;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.SAML
{
    public class SAMLConfigurationBuilder
    {
        public static SAMLConfiguration GetConfiguration(SSOClient client)
        {
            switch (client)
            {
                case SSOClient.Vdet:
                    return new SAMLConfiguration
                    {
                        CertString = ConfigurationManager.AppSettings["AzureVdetCertString"],
                        LoginRedirectUrl = ConfigurationManager.AppSettings["InsightLoginRedirectUrl"],
                        UserIdKey = ConfigurationManager.AppSettings["InsightSamlKeyUserId"],
                        RoleKey = ConfigurationManager.AppSettings["InsightSamlKeyRole"],
                        SsoTargetUrl = ConfigurationManager.AppSettings["AzureVdetSSOTargetURL"],
                        Issuer = ConfigurationManager.AppSettings["AzureInsightIssuer"],
                        LogoutRedirectUrl = ConfigurationManager.AppSettings["InsightLogoutRedirectUrl"],
                        Client = client
                    };
                case SSOClient.Cecv:
                    return new SAMLConfiguration
                    {
                        CertString = ConfigurationManager.AppSettings["CecvCertString"],
                        LoginRedirectUrl = ConfigurationManager.AppSettings["CecvLoginRedirectUrl"],
                        UserIdKey = ConfigurationManager.AppSettings["CecvSamlKeyUserId"],
                        RoleKey = ConfigurationManager.AppSettings["CecvSamlKeyRole"],
                        SsoTargetUrl = ConfigurationManager.AppSettings["CecvSSOTargetURL"],
                        Issuer = ConfigurationManager.AppSettings["CecvIssuer"],
                        SectorID = ConfigurationManager.AppSettings["CecvSectorID"],
                        LogoutRedirectUrl = ConfigurationManager.AppSettings["CecvLogoutRedirectUrl"],
                        Client = client
                    };
                case SSOClient.Nyc:
                    return new SAMLConfiguration
                    {
                        CertString = ConfigurationManager.AppSettings["NYCSamlSSOCertString"],                        
                        SsoTargetUrl = ConfigurationManager.AppSettings["NYCSamlSSOUrlLogin"],
                        Issuer = ConfigurationManager.AppSettings["NYCSamlSSOIssuer"],                       
                        Client = client
                    };
                default:
                    throw new ArgumentOutOfRangeException("client", "Invalid SSO Client");
            }
        }
    }
}
