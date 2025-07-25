using LinkIt.BubbleSheetPortal.DynamoIsolating.Model;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Services;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.SGO
{
    public class SGOManagerLogFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var isLog = true;
                //var service = DependencyResolver.Current.GetService<TTLConfigService>();
                //int iRetention = service.GetSGORetentionInDay();
                int iRetention = 0;
                if (TTLConfigurationManager.TTLConfigs != null )
                {
                    var objTTLSGO = TTLConfigurationManager.TTLConfigs.FirstOrDefault(o => o.DynamoTableName.Equals("SGOManagerLog"));
                    if(objTTLSGO != null)
                    {
                        iRetention = objTTLSGO.RetentionInDay;
                    }
                }
                var sgoManagerLog = new SGOManagerLog
                {
                    SGOManagerLogID = Guid.NewGuid().ToString(),
                    ActionName = DetectFunctionName(filterContext),
                    DynamoCreatedDate = DateTime.UtcNow                    
                };
                if(iRetention > 0)
                {
                    sgoManagerLog.TTL = Util.ToEpochTime(DateTime.UtcNow.AddDays(iRetention));
                }

                var form = filterContext.HttpContext.Request.Form;
                if(form != null && form.AllKeys.Any())
                {
                    var dictionary = form.AllKeys.ToDictionary(k => k, k => form[k]);
                    sgoManagerLog.RequestData = JsonConvert.SerializeObject(dictionary);
                    sgoManagerLog.SGOID = DetectSGOId(dictionary);
                }else if(filterContext.ActionParameters != null && filterContext.ActionParameters.Any())
                {
                    sgoManagerLog.RequestData = JsonConvert.SerializeObject(filterContext.ActionParameters);

                    var sgoidString = GetJsonValueByKey(sgoManagerLog.RequestData, "sgoid");
                    if (!string.IsNullOrEmpty(sgoidString))
                    {
                        try
                        {
                            sgoManagerLog.SGOID = Convert.ToInt32(sgoidString);
                        }
                        catch (Exception ex)
                        {
                            PortalAuditManager.LogException(ex);
                        }
                    }                    
                }
                
                var currentUser = (UserPrincipal)filterContext.HttpContext.User.Identity;
                if(currentUser != null)
                {
                    sgoManagerLog.UserID = currentUser.Id;
                }

                if(sgoManagerLog.ActionName == "SGOManage.ShowStudentGroupTable")
                {
                    var isAutoGroup = GetJsonValueByKey(sgoManagerLog.RequestData, "isautogroup");
                    if(isAutoGroup != "true")
                    {
                        isLog = false;
                    }
                }

                if (isLog)
                {
                    filterContext.HttpContext.Items.Add("SGOManagerLogID", sgoManagerLog.SGOManagerLogID);
                    if (sgoManagerLog.SGOID.HasValue)
                    {
                        var sgoObjectService = DependencyResolver.Current.GetService<SGOObjectService>();
                        sgoManagerLog.LoggingDataBefore = sgoObjectService.GetFullDataForLogging(sgoManagerLog.SGOID.Value);
                    }

                    var logService = DependencyResolver.Current.GetService<SGOManagerLogService>();
                    logService.PutItem(sgoManagerLog);
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
            }

            base.OnActionExecuting(filterContext);
        }

        private string GetJsonValueByKey(string jsonString, string keyName)
        {
            if (!string.IsNullOrEmpty(jsonString))
            {
                try
                {
                    var indexOf = jsonString.ToLower().IndexOf(string.Format("\"{0}\"", keyName));
                    if (indexOf > 0)
                    {
                        var remainString = jsonString.Substring(indexOf + keyName.Length + 3);
                        var value = "";
                        for (int i = 0; i < remainString.Length; i++)
                        {
                            if (remainString[i] == ',' || remainString[i] == '}')
                            {
                                break;
                            }
                            value += remainString[i].ToString();
                        }

                        return value;
                    }
                }
                catch (Exception ex)
                {
                    PortalAuditManager.LogException(ex);
                }
            }

            return null;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                var sgoManagerLogID = filterContext.HttpContext.Items["SGOManagerLogID"];
                if(sgoManagerLogID != null)
                {
                    var logService = DependencyResolver.Current.GetService<SGOManagerLogService>();
                    var sgoManagerLog = logService.GetByID(sgoManagerLogID.ToString());

                    if (sgoManagerLog != null)
                    {
                        sgoManagerLog.ResponseData = JsonConvert.SerializeObject(filterContext.Result);

                        var functionName = DetectFunctionName(filterContext);
                        if (functionName == "SGOManage.CreateSGOByName" && filterContext.Result is JsonResult)
                        {
                            var responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(((JsonResult)filterContext.Result).Data));
                            sgoManagerLog.SGOID = DetectSGOId(responseDictionary);
                        }

                        if (sgoManagerLog.SGOID.HasValue)
                        {
                            var sgoObjectService = DependencyResolver.Current.GetService<SGOObjectService>();
                            sgoManagerLog.LogginDataAfter = sgoObjectService.GetFullDataForLogging(sgoManagerLog.SGOID.Value);
                        }

                        logService.UpdateItem(sgoManagerLog);
                    }
                }
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
            }

            base.OnActionExecuted(filterContext);
        }
        
        private int DetectSGOId(Dictionary<string, string> dictionary)
        {
            var sgoId = 0;

            try
            {
                foreach (var item in dictionary)
                {
                    if (item.Key.ToUpper() == "SGOID")
                    {
                        sgoId = Convert.ToInt32(item.Value);
                        break;
                    }
                }
            }
            catch (Exception ex) { PortalAuditManager.LogException(ex); }

            return sgoId;
        }

        private string DetectFunctionName(ActionExecutingContext filterContext)
        {
            var routeValues = filterContext.HttpContext.Request.RequestContext.RouteData.Values;
            var controllerName = "[Unkown]";
            var actionName = "[Unkown]";
            if (routeValues.ContainsKey("controller"))
            {
                controllerName = (string)routeValues["controller"];
            }
            if (routeValues.ContainsKey("action"))
            {
                actionName = (string)routeValues["action"];
            }

            return string.Format("{0}.{1}", controllerName, actionName);
        }

        private string DetectFunctionName(ActionExecutedContext filterContext)
        {
            var routeValues = filterContext.HttpContext.Request.RequestContext.RouteData.Values;
            var controllerName = "[Unkown]";
            var actionName = "[Unkown]";
            if (routeValues.ContainsKey("controller"))
            {
                controllerName = (string)routeValues["controller"];
            }
            if (routeValues.ContainsKey("action"))
            {
                actionName = (string)routeValues["action"];
            }

            return string.Format("{0}.{1}", controllerName, actionName);
        }
    }
}
