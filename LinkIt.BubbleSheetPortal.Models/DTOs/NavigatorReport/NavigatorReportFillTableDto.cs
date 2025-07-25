namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportFillTableDto
    {
        public int NavigatorCategory { get; set; }
        public int DataRow { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }
    public class NavigatorReportPatternDto
    {
        public string FileName { get; set; }
        public string PathName { get; set; }
    }
}
