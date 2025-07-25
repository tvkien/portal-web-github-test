using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.SSO.Clever
{
    public class CleverPrimaryTokenInfoDto
    {
        public bool IsStudent { get; set; }
        public string  AccessToken { get; set; }
        public DateTime ExpireOn { get; set; }
    }
}
