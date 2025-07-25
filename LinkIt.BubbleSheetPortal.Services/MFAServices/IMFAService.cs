using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.MfaSettings;
using LinkIt.BubbleSheetPortal.Models.Old.Configugration;

namespace LinkIt.BubbleSheetPortal.Services.MfaServices
{
    public interface IMfaService
    {
        MfaSettingDto CheckFlowMfa(CognitoCredentialSetting cognitoCredentialSetting, User user);
        MfaSettingDto Resend(CognitoCredentialSetting cognitoCredentialSetting, MfaSettingDto mfaSetting);
        bool Verify(CognitoCredentialSetting cognitoCredentialSetting, MfaSettingDto mfaSetting, string code);
    }
}
