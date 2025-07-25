namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportByLevelFilterDTO
    {
        public int? DistrictId;

        public int? SchoolId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string AcceptedYears { get; set; }
    }
}
