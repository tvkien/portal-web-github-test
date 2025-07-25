using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Utils;
using LinkIt.BubbleSheetPortal.Web.ViewModels.LTI;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.LTI
{
    public class LtiDeepLinkingRequest : LtiRequest
    {
        /// <inheritdoc />
        /// <summary>
        /// Create an empty request.
        /// </summary>
        public LtiDeepLinkingRequest()
        {
            MessageType = LtiConstants.Lti.LtiDeepLinkingRequestMessageType;
            Version = LtiConstants.Lti.Version;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a request with the claims.
        /// </summary>
        /// <param name="claims">A list of claims.</param>
        public LtiDeepLinkingRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a request with the claims in payload.
        /// </summary>
        /// <param name="payload"></param>
        public LtiDeepLinkingRequest(JwtPayload payload) : base(payload.Claims)
        {
        }

        /// <summary>
        /// Deep Linking settings.
        /// </summary>
        public DeepLinkingSettingsClaimValueType DeepLinkingSettings
        {
            get { return this.GetClaimValue<DeepLinkingSettingsClaimValueType>(LtiConstants.LtiClaims.DeepLinkingSettings); }
            set { this.SetClaimValue(LtiConstants.LtiClaims.DeepLinkingSettings, value); }
        }
    }
}
