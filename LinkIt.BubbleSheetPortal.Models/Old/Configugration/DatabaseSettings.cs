namespace LinkIt.BubbleSheetPortal.Models.Configugration
{
    public class DatabaseSettings
    {
        public string LinkItConnectionString { get; set; }
        public string S3PortalLinkContext { get; set; }
        public string AdminReportingLogConnectionString { get; set; }
        public string SessionStateLinkit { get; set; }
        public string SessionStateSQLConnectionString { get; set; }
        public string SessionStateRedisConnectionString { get; set; }
    }
}
