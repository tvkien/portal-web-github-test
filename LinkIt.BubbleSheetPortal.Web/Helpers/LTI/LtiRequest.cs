using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.DTOs.LTI;
using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Enums;
using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Utils;
using LinkIt.BubbleSheetPortal.Web.ViewModels.LTI;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.LTI
{
    public abstract class LtiRequest : JwtPayload
    {
        #region Constructors

        protected LtiRequest()
        {
        }

        protected LtiRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

        protected LtiRequest(JwtPayload payload) : base(payload.Claims)
        {
        }

        #endregion

        #region Required Message Claims

        public string[] Audiences
        {
            get { return this.GetClaimValue<string[]>(JwtRegisteredClaimNames.Aud); }
            set { this.SetClaimValue(JwtRegisteredClaimNames.Aud, value); }
        }

        public string DeploymentId
        {
            get { return this.GetClaimValue(LtiConstants.LtiClaims.DeploymentId); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.DeploymentId, value); }
        }

        public string Lti11LegacyUserId
        {
            get { return this.GetClaimValue(LtiConstants.LtiClaims.Lti11LegacyUserId); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.Lti11LegacyUserId, value); }
        }

        public string MessageType
        {
            get { return this.GetClaimValue(LtiConstants.LtiClaims.MessageType); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.MessageType, value); }
        }

        public new string Nonce
        {
            get { return base.Nonce; }
            set { this.SetClaimValue(JwtRegisteredClaimNames.Nonce, value); }
        }

        public RoleEnum[] Roles
        {
            get { return this.GetClaimValue<RoleEnum[]>(LtiConstants.LtiClaims.Roles); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.Roles, value); }
        }

        public string TargetLinkUri
        {
            get { return this.GetClaimValue(LtiConstants.LtiClaims.TargetLinkUri); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.TargetLinkUri, value); }
        }

        public string UserId
        {
            get { return this.GetClaimValue(JwtRegisteredClaimNames.Sub); }
            set { this.SetClaimValue(JwtRegisteredClaimNames.Sub, value); }
        }

        public string Version
        {
            get { return this.GetClaimValue(LtiConstants.LtiClaims.Version); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.Version, value); }
        }

        #endregion

        #region Optional Message Claims

        public ContextClaimValueType Context
        {
            get { return this.GetClaimValue<ContextClaimValueType>(LtiConstants.LtiClaims.Context); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.Context, value); }
        }
        public Dictionary<string, string> Custom
        {
            get { return this.GetClaimValue<Dictionary<string, string>>(LtiConstants.LtiClaims.Custom); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.Custom, value); }
        }

        public LaunchPresentationClaimValueType LaunchPresentation
        {
            get { return this.GetClaimValue<LaunchPresentationClaimValueType>(LtiConstants.LtiClaims.LaunchPresentation); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.LaunchPresentation, value); }
        }

        public LisClaimValueType Lis
        {
            get { return this.GetClaimValue<LisClaimValueType>(LtiConstants.LtiClaims.Lis); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.Lis, value); }
        }

        public PlatformClaimValueType Platform
        {
            get { return this.GetClaimValue<PlatformClaimValueType>(LtiConstants.LtiClaims.Platform); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.Platform, value); }
        }

        public string[] RoleScopeMentor
        {
            get { return this.GetClaimValue<string[]>(LtiConstants.LtiClaims.RoleScopeMentor); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.RoleScopeMentor, value); }
        }

        #endregion

        #region Optional OpenID Connect claims

        public string Email
        {
            get { return this.GetClaimValue(JwtRegisteredClaimNames.Email); }
            set { this.SetClaimValue(JwtRegisteredClaimNames.Email, value); }
        }

        public string FamilyName
        {
            get { return this.GetClaimValue(JwtRegisteredClaimNames.FamilyName); }
            set { this.SetClaimValue(JwtRegisteredClaimNames.FamilyName, value); }
        }

        public string GivenName
        {
            get { return this.GetClaimValue(JwtRegisteredClaimNames.GivenName); }
            set { this.SetClaimValue(JwtRegisteredClaimNames.GivenName, value); }
        }

        public string MiddleName
        {
            get { return this.GetClaimValue(LtiConstants.OidcClaims.MiddleName); }
            set { this.SetClaimValue(LtiConstants.OidcClaims.MiddleName, value); }
        }
        public string Name
        {
            get { return this.GetClaimValue(LtiConstants.OidcClaims.Name); }
            set { this.SetClaimValue(LtiConstants.OidcClaims.Name, value); }
        }

        public string Picture
        {
            get { return this.GetClaimValue(LtiConstants.OidcClaims.Picture); }
            set { this.SetClaimValue(LtiConstants.OidcClaims.Picture, value); }
        }

        #endregion
    }
}
