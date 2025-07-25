using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.MfaSettings
{
    [Serializable]
    public class MfaSettingDto
    {
        public bool IsEnableMfa { get; set; }
        public string ChallengeName { get; set; }
        public string MfaSession { get; set; }
        public string CodeDeliveryDestination { get; set; }
        public string UserName { get; set; }
        public string HashedPassword { get; set; }
        public int UserId { get; set; }
        public bool HasEmailAddress { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
