namespace LinkIt.BubbleSheetPortal.VaultProvider
{
    public interface ISecretsManager
    {
        string GetSecretString(string secretId);
    }
}
