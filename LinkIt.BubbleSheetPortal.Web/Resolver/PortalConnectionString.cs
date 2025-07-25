using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.VaultProvider;

namespace LinkIt.BubbleSheetPortal.Web.Resolver
{
    public class PortalConnectionString : IConnectionString
    {
        private readonly IVaultProvider vaultProvider;
        public PortalConnectionString(IVaultProvider vaultProvider)
        {
            this.vaultProvider = vaultProvider;
        }

        public string GetLinkItConnectionString()
        {
            return vaultProvider.GetConnectionString();
        }

        public string GetS3PortalLinkContext()
        {
            throw new System.NotImplementedException();
        }

        public string GetAdminReportingLogConnectionString()
        {
            return vaultProvider.GetLogConnectionString();
        }

        public string GetIsolatingTestTakerConnectionString()
        {
            return vaultProvider.GetIsolatingConnectionString();
        }

        public string GetAuroraPortalLogConnectionString()
        {
            return string.Empty;
        }
    }
}