using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace GenericLog
{
    public class GenericLogService : IGenericLogService
    {
        public List<ActionLogDTO> ActionLogs { get; set; }
        public IPortalLogRepository _portalLogRepository;

        public GenericLogService(IPortalLogRepository portalLogRepository)
        {
            _portalLogRepository = portalLogRepository;
        }

        public void SaveLog(ActionExecutedContext context, long executionTime, IDictionary<string, object> actionParameters)
        {
            try
            {
                var action = context.HttpContext.Request.Path.Split('/');

                var config = IsNeedSave(action.Length == 3 ? action[2] : "index", action[1], context.HttpContext.Request.HttpMethod);
                if (config != null)
                {
                    var logVM = new LogViewModel();
                    logVM.InitData(context, executionTime, actionParameters, config);
                    _portalLogRepository.Insert(logVM);
                }
            }
            catch (Exception)
            {
            }
        }
        public void SaveUserLogoutLog(int? districtId, int userId, string reason)
        {
            try
            {
                var logVM = new UserLogOutModel()
                {
                    DistrictId = districtId,
                    UserId = userId,
                    Reason = reason
                };          
                _portalLogRepository.InsertUserLogoutLog(logVM);
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// Save log for User Impersonate
        /// </summary>
        /// <param name="logViewModel"></param>
        public void SavePortalWarningLog(LogViewModel logViewModel)
        {
            try
            {
                _portalLogRepository.Insert(logViewModel);
            }
            catch (Exception)
            {
            }
        }

        private ActionLogDTO IsNeedSave(string action, string controller, string method)
        {
            var act = ActionLogs.FirstOrDefault(m => m.Action.ToLower() == action.ToLower() && m.Controller.ToLower() == controller.ToLower() && m.Method.ToLower() == method.ToLower());
            return act;
        }

        public void GetConfig(List<ActionLogDTO> config)
        {
            if(ActionLogs == null)
            {
                ActionLogs = config;
            }
        }
    }

}
