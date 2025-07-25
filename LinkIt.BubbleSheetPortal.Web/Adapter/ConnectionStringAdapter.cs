using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using System.Configuration;

namespace LinkIt.BubbleSheetPortal.Web.Adapter
{
    public class ConnectionStringAdapter : IConnectionString
    {
        public string GetLinkItConnectionString()
        {
            var test = LinkitConfigurationManager.GetDatabaseSettings();
            var result = test.LinkItConnectionString;
            return result;
        }

        public string GetAdminReportingLogConnectionString()
        {
            var result = LinkitConfigurationManager.GetDatabaseSettings().AdminReportingLogConnectionString;
            return result;
        }
    }
}
