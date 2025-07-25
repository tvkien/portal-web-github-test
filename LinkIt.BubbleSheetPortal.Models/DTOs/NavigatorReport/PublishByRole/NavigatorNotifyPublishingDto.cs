using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole
{
    public class NavigatorNotifyPublishingDto
    {
        public int PublisherUserId { get; set; }
        public int PublisherRoleId { get; set; }
        public string InitiatorEmail { get; set; }
        public string CustomNote { get; set; }
        public PublishByRoleEmailListDto PublishByRoleEmailList { get; set; }
        public int DistrictId { get; set; }
        public string InitiatorName { get; set; }
        public int[] ExcludeUserMailIds { get; set; }
        public string GeneralUrl { get; set; }
    }
}
