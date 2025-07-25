using System.Configuration;

namespace LinkIt.BubbleSheetPortal.VaultProvider.Model
{
    public static class VaultInfo
    {
        public static string VaultTable { get; } = ConfigurationManager.AppSettings["VaultTable"];
        public static string VaultPasswordPassphrase { get; } = ConfigurationManager.AppSettings["VaultPasswordPassphrase"];
    }
}
