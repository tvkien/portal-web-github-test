using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportUnpublishRequestDto
    {
        public string NavigatorReportIds { get; set; }
        public string UserIds { get; set; }
        public bool? PublishDistrictAdmin { get; set; }
        public bool? PublishSchoolAdmin { get; set; }
        public bool? PublishTeacher { get; set; }
        public bool? PublishStudent { get; set; }
        public DateTime PublishTime { get; set; }
        public int PublisherId { get; set; }
        public string NodePath { get; set; }
        public int DistrictId { get; set; }
    }
}
