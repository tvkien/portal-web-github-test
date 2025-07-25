
using System.ComponentModel.DataAnnotations;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.SSO
{
    public class LtiAuthorizeDto
    {
        public string ClientId { get; set; }
        public string ResponseType { get; set; }
        public string ResponseMode { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
        [Required]
        public string LoginHint { get; set; }
        public string Nonce { get; set; }
        public string Prompt { get; set; }
        public string LtiMessageHint { get; set; }
        [Required]
        public string PlatformID { get; set; }
        [Required]
        public string TargetLinkUri { get; set; }
        public string DeploymentId { get; set; }
    }
}
