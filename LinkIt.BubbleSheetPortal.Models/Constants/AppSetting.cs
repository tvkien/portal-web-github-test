using System.Configuration;

namespace LinkIt.BubbleSheetPortal.Models.Constants
{
    public static class AppSetting
    {
        public static string LinkItUrl { get; } = ConfigurationManager.AppSettings["LinkItUrl"];
        public static string HealthCheckReportingAPIUrlLocal { get; } = ConfigurationManager.AppSettings["HealthCheckReportingAPIUrlLocal"];
        public static string HealthCheckIMAPIUrlLocal { get; } = ConfigurationManager.AppSettings["HealthCheckIMAPIUrlLocal"];
        public static string HealthCheckTestTakerAPIUrlLocal { get; } = ConfigurationManager.AppSettings["HealthCheckTestTakerAPIUrlLocal"];
        public static string HealthCheckRESTAPIUrlLocal { get; } = ConfigurationManager.AppSettings["HealthCheckRESTAPIUrlLocal"];
    }
}
