using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.VaultProvider.Model
{
    public class Vault
    {
        public List<DatabaseServer> DBServers { get; set; }
        public List<EmailCredential> EmailCredentials { get; set; }
        public string Code { get; set; }
        public string DatabaseID { get; set; }
        public VaultS3 VaultS3 { get; set; }
        public AppSettings AppSetting { get; set; }
        public TTLConfigs TTLConfigs { get; set; }
        public string SecretManager { get; set; }
        public ASPStateDto ASPState { get; set; }
    }
}
