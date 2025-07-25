namespace LinkIt.BubbleSheetPortal.VaultProvider.Model
{
    public class EmailCredential
    {
        public string Name { get; set; }
        public CredentialSetting CredentialSetting { get; set; }
    }

    public class CredentialSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
