using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.MfaSettings;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestPreferences;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Models.Old.Configugration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Services.MfaServices
{
    public class MfaService : IMfaService
    {
        private readonly IAmazonCognitoIdentityProvider _amazonCognitoIdentityProvider;
        private readonly ConfigurationService _configurationService;
        private readonly SecurityPreferencesService _securityPreferencesService;
        private CognitoCredentialSetting _cognitoCredentialSetting;

        public MfaService(
            IAmazonCognitoIdentityProvider amazonCognitoIdentityProvider,
            ConfigurationService configurationService,
            SecurityPreferencesService securityPreferencesService
        )
        {
            _amazonCognitoIdentityProvider = amazonCognitoIdentityProvider;
            _configurationService = configurationService;
            _securityPreferencesService = securityPreferencesService;
        }

        public MfaSettingDto CheckFlowMfa(CognitoCredentialSetting cognitoCredentialSetting, User user)
        {
            _cognitoCredentialSetting = cognitoCredentialSetting;
            try
            {
                (bool isEnable, string preferredMfaSetting) = IsEnableMfa(user);
                if (!isEnable)
                    return new MfaSettingDto();

                if (string.IsNullOrEmpty(user.EmailAddress))
                {
                    return new MfaSettingDto
                    {
                        IsEnableMfa = true,
                        HasEmailAddress = false
                    };
                }

                var userName = GetCognitoUserName(user);
                var cognitoUser = GetAdminGetUser(userName);
                if (cognitoUser == null)
                {
                    AdminCreateUser(userName, user.EmailAddress, user.PhoneNumber, user.Name);
                    AdminSetUserPassword(userName, user.HashedPassword);
                    AdminAddUserToGroup(userName, GetUserGroup(user.RoleId));
                    AdminSetUserMFAPreference(userName, user.EmailAddress, user.PhoneNumber, preferredMfaSetting);
                }
                else
                {
                    if (!string.Equals(cognitoUser.PreferredMfaSetting, preferredMfaSetting, StringComparison.OrdinalIgnoreCase))
                    {
                        AdminSetUserMFAPreference(userName, user.EmailAddress, user.PhoneNumber, preferredMfaSetting);
                    }

                    if (HasChangeUserAttributes(cognitoUser, user.EmailAddress, user.PhoneNumber))
                    {
                        AdminUpdateUserAttributes(userName, user.EmailAddress, user.PhoneNumber, user.Name);
                    }
                }

                var authResponse = AdminInitiateAuth(userName, user.HashedPassword);
                var mfaSetting = new MfaSettingDto
                {
                    IsEnableMfa = true,
                    ChallengeName = authResponse.ChallengeName,
                    MfaSession = authResponse.Session,
                    CodeDeliveryDestination = authResponse.ChallengeParameters["CODE_DELIVERY_DESTINATION"],
                    UserName = userName,
                    HashedPassword = user.HashedPassword,
                    UserId = user.Id,
                    HasEmailAddress = true,
                    CreatedDate = DateTime.UtcNow,
                };

                return mfaSetting;
            }
            catch (Exception ex)
            {
                return new MfaSettingDto
                {
                    IsEnableMfa = false
                };
            }
        }

        public MfaSettingDto Resend(CognitoCredentialSetting cognitoCredentialSetting, MfaSettingDto mfaSetting)
        {
            _cognitoCredentialSetting = cognitoCredentialSetting;
            var authResponse = AdminInitiateAuth(mfaSetting.UserName, mfaSetting.HashedPassword);
            mfaSetting.ChallengeName = authResponse.ChallengeName;
            mfaSetting.MfaSession = authResponse.Session;
            mfaSetting.CodeDeliveryDestination = authResponse.ChallengeParameters["CODE_DELIVERY_DESTINATION"];
            mfaSetting.CreatedDate = DateTime.UtcNow;

            return mfaSetting;
        }

        public bool Verify(CognitoCredentialSetting cognitoCredentialSetting, MfaSettingDto mfaSetting, string code)
        {
            _cognitoCredentialSetting = cognitoCredentialSetting;
            return AdminRespondToAuthChallenge(mfaSetting.UserName, mfaSetting.MfaSession, code, mfaSetting.ChallengeName);
        }

        private (bool isEnable, string preferredMfaSetting) IsEnableMfa(User user)
        {
            var globalConfig = _configurationService.GetConfigurationByKeyWithDefaultValue(ConfigurationKey.IsEnableMfa, false);
            if (globalConfig == false) return (false, string.Empty);

            var myConfig = _securityPreferencesService.GetPreferenceInCurrentLevel(new GetPreferencesParams
            {
                CurrrentLevelId = (int)SecurityPreferenceLevel.User,
                DistrictId = user.DistrictId.GetValueOrDefault(),
                UserId = user.Id,
                UserRoleId = user.RoleId,
                StateId = user.StateId.GetValueOrDefault()
            });

            var isEnable = myConfig?.OptionTags?
                .Any(x => string.Equals(x.Key, "enableMFAEmail_user", StringComparison.OrdinalIgnoreCase) && x.Value == "1");
            return (isEnable ?? false, "EMAIL_OTP");
        }

        private string GetCognitoUserName(User user)
        {
            return $"{user.DistrictId}_{user.Id}";
        }

        private string GetUserGroup(int roleID)
        {
            switch (roleID)
            {
                case (int)RoleEnum.Student:
                    return "students";
                case (int)RoleEnum.Parent:
                    return "parents";
                default:
                    return "staffs";
            }
        }

        private bool HasChangeUserAttributes(AdminGetUserResponse cognitoUser, string emailAddress, string phoneNumber)
        {
            var email = cognitoUser.UserAttributes?.FirstOrDefault(x => string.Equals(x.Name, "email", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
            if (!string.Equals(email, emailAddress, StringComparison.OrdinalIgnoreCase))
                return true;

            var phone = cognitoUser.UserAttributes?.FirstOrDefault(x => string.Equals(x.Name, "phone_number", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
            if (!string.Equals(phone, phoneNumber, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        #region Cognito request

        private AdminGetUserResponse GetAdminGetUser(string userName)
        {
            try
            {
                var request = new AdminGetUserRequest
                {
                    UserPoolId = _cognitoCredentialSetting.UserPoolID,
                    Username = userName
                };
                var response = _amazonCognitoIdentityProvider.AdminGetUser(request);
                return response;
            }
            catch (UserNotFoundException)
            {
                return null;
            }
        }

        private UserType AdminCreateUser(string userName, string email, string phone, string displayName)
        {
            var request = new AdminCreateUserRequest
            {
                UserPoolId = _cognitoCredentialSetting.UserPoolID,
                Username = userName,
                UserAttributes = new List<AttributeType>(),
                MessageAction = "SUPPRESS"
            };

            if (!string.IsNullOrEmpty(email))
            {
                request.UserAttributes.Add(new AttributeType { Name = "email", Value = email });
                request.UserAttributes.Add(new AttributeType { Name = "email_verified", Value = "true" });
            }

            if (!string.IsNullOrEmpty(phone))
            {
                request.UserAttributes.Add(new AttributeType { Name = "phone_number", Value = phone });
                request.UserAttributes.Add(new AttributeType { Name = "phone_number_verified", Value = "true" });
            }

            if (!string.IsNullOrEmpty(displayName))
            {
                request.UserAttributes.Add(new AttributeType { Name = "name", Value = displayName });
            }

            var response = _amazonCognitoIdentityProvider.AdminCreateUser(request);
            return response.User;
        }

        private void AdminSetUserMFAPreference(string userName, string email, string phone, string preferredMfaSetting)
        {
            var request = new AdminSetUserMFAPreferenceRequest
            {
                UserPoolId = _cognitoCredentialSetting.UserPoolID,
                Username = userName,
                SoftwareTokenMfaSettings = new SoftwareTokenMfaSettingsType
                {
                    Enabled = false,
                    PreferredMfa = false,
                },
                EmailMfaSettings = new EmailMfaSettingsType
                {
                    Enabled = false,
                    PreferredMfa = false,
                },
                SMSMfaSettings = new SMSMfaSettingsType
                {
                    Enabled = false,
                    PreferredMfa = false,
                }
            };

            if (!string.IsNullOrEmpty(email) && string.Equals(preferredMfaSetting, "EMAIL_OTP"))
            {
                request.EmailMfaSettings.Enabled = true;
                request.EmailMfaSettings.PreferredMfa = true;
            }

            if (!string.IsNullOrEmpty(phone) && string.Equals(preferredMfaSetting, "SMS_MFA"))
            {
                request.SMSMfaSettings.Enabled = true;
                request.SMSMfaSettings.PreferredMfa = true;
            }

            var response = _amazonCognitoIdentityProvider.AdminSetUserMFAPreference(request);
        }

        private void AdminUpdateUserAttributes(string userName, string email, string phone, string displayName)
        {
            var request = new AdminUpdateUserAttributesRequest
            {
                UserPoolId = _cognitoCredentialSetting.UserPoolID,
                Username = userName,
                UserAttributes = new List<AttributeType>()
            };

            if (!string.IsNullOrEmpty(email))
            {
                request.UserAttributes.Add(new AttributeType { Name = "email", Value = email });
                request.UserAttributes.Add(new AttributeType { Name = "email_verified", Value = "true" });
            }

            if (!string.IsNullOrEmpty(phone))
            {
                request.UserAttributes.Add(new AttributeType { Name = "phone_number", Value = phone });
                request.UserAttributes.Add(new AttributeType { Name = "phone_number_verified", Value = "true" });
            }

            if (!string.IsNullOrEmpty(displayName))
            {
                request.UserAttributes.Add(new AttributeType { Name = "name", Value = displayName });
            }

            var response = _amazonCognitoIdentityProvider.AdminUpdateUserAttributes(request);
        }

        private void AdminSetUserPassword(string userName, string password)
        {
            var request = new AdminSetUserPasswordRequest
            {
                UserPoolId = _cognitoCredentialSetting.UserPoolID,
                Username = userName,
                Password = password,
                Permanent = true
            };
            var response = _amazonCognitoIdentityProvider.AdminSetUserPassword(request);
        }

        private void AdminAddUserToGroup(string userName, string groupName)
        {
            var request = new AdminAddUserToGroupRequest
            {
                UserPoolId = _cognitoCredentialSetting.UserPoolID,
                Username = userName,
                GroupName = groupName
            };
            var response = _amazonCognitoIdentityProvider.AdminAddUserToGroup(request);
        }

        private AdminInitiateAuthResponse AdminInitiateAuth(string userName, string password)
        {
            try
            {
                var request = new AdminInitiateAuthRequest
                {
                    AuthFlow = "ADMIN_USER_PASSWORD_AUTH",
                    ClientId = _cognitoCredentialSetting.ClientID,
                    UserPoolId = _cognitoCredentialSetting.UserPoolID,
                    AuthParameters = new Dictionary<string, string>
                    {
                        { "USERNAME", userName },
                        { "PASSWORD", password },
                        { "SECRET_HASH", GenerateSecretHash(userName,_cognitoCredentialSetting.ClientID, _cognitoCredentialSetting.ClientSecret) },
                    }
                };
                var response = _amazonCognitoIdentityProvider.AdminInitiateAuth(request);
                return response;
            }
            catch (NotAuthorizedException)
            {
                AdminSetUserPassword(userName, password);
                return AdminInitiateAuth(userName, password);
            }
        }

        private bool AdminRespondToAuthChallenge(string userName, string session, string code, string preferredMfaSetting)
        {
            try
            {
                var mfaChallengeMap = new Dictionary<string, (string ChallengeName, string CodeKey)>(StringComparer.OrdinalIgnoreCase)
                {
                    { "EMAIL_OTP", ("EMAIL_OTP", "EMAIL_OTP_CODE") },
                    { "SMS_MFA",   ("SMS_MFA", "SMS_MFA_CODE") }
                };

                var request = new AdminRespondToAuthChallengeRequest
                {
                    ChallengeName = mfaChallengeMap[preferredMfaSetting].ChallengeName,
                    ClientId = _cognitoCredentialSetting.ClientID,
                    UserPoolId = _cognitoCredentialSetting.UserPoolID,
                    Session = session,
                    ChallengeResponses = new Dictionary<string, string>
                    {
                        { "USERNAME", userName },
                        { mfaChallengeMap[preferredMfaSetting].CodeKey, code },
                        { "SECRET_HASH", GenerateSecretHash(userName,_cognitoCredentialSetting.ClientID, _cognitoCredentialSetting.ClientSecret) },
                    }
                };

                var response = _amazonCognitoIdentityProvider.AdminRespondToAuthChallenge(request);
                return true;
            }
            catch (CodeMismatchException ex)
            {
                return false;
            }
            catch (NotAuthorizedException ex)
            {
                return false;
            }
        }

        private string GenerateSecretHash(string username, string clientId, string clientSecret)
        {
            string message = username + clientId;
            byte[] keyBytes = Encoding.UTF8.GetBytes(clientSecret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        #endregion
    }
}
