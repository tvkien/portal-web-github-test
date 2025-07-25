namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportFillTableResultDto
    {
        public int DataRow { get; set; }
        public int ReportTypeId { get; set; }
        public string SchoolName { get; set; }
        public string ReportSuffix { get; set; } = string.Empty;
    }
}
