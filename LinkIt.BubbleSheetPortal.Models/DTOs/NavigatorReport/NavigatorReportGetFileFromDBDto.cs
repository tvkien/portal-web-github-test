namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportGetFileFromDBDto
    {
        public int NavigatorReportID { get; set; }
        public string S3FileFullName { get; set; }
        public int PageNumber { get; set; }
        public string MasterFileName { get; set; }
    }
}
