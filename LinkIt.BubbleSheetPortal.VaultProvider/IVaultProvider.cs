using LinkIt.BubbleSheetPortal.VaultProvider.Model;

namespace LinkIt.BubbleSheetPortal.VaultProvider
{
    public interface IVaultProvider
    {
        Vault GetVaultByCode(string code = "");
        string GetLICodeByCleveDistrictId(string cleverDistrictId);
    }
}
