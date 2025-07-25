using System.Collections.Generic;
using System.Web.Mvc;

namespace GenericLog
{
    public interface IGenericLogService
    {
        void GetConfig(List<ActionLogDTO> config);
        void SaveLog(ActionExecutedContext context, long executionTime, IDictionary<string, object> actionParameters);
        void SaveUserLogoutLog(int? districtId, int userId, string reason);
        void SavePortalWarningLog(LogViewModel logViewModel);
    }
}
