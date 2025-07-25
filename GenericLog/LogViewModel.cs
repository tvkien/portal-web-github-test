using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GenericLog
{
    public class LogViewModel
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public string Action { get; set; }
        public long ExecutionTime { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string Date { get; set; }
        public string UserName { get; set; }
        public string District { get; set; }
        public string Exception { get; set; }
        public string KeyName_1 { get; set; }
        public string KeyValue_1 { get; set; }
        public string KeyName_2 { get; set; }
        public string KeyValue_2 { get; set; }
        public string KeyName_3 { get; set; }
        public string KeyValue_3 { get; set; }
        public string KeyName_4 { get; set; }
        public string KeyValue_4 { get; set; }
        public string OutName_1 { get; set; }
        public string OutValue_1 { get; set; }
        public string OutName_2 { get; set; }
        public string OutValue_2 { get; set; }
        public string OutName_3 { get; set; }
        public string OutValue_3 { get; set; }
        public string OutName_4 { get; set; }
        public string OutValue_4 { get; set; }
      
        public void InitData(ActionExecutedContext context, long executionTime, IDictionary<string, object> actionParameters, ActionLogDTO config)
        {
            try
            {
                if (config.IsLogInput)
                {
                    AddInput(context.HttpContext.Request, config, actionParameters);
                }

                if (config.IsLogOutput)
                {
                    AddOutput(context, config);
                }

                ExecutionTime = executionTime;
                Exception = context.Exception != null ? context.Exception.Message : string.Empty;
                Method = context.HttpContext.Request.HttpMethod;
                Action = context.HttpContext.Request.Path;
                District = context.HttpContext.Request.Headers["HOST"].Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    UserName = context.HttpContext.User.Identity.Name;
                }
            }
            catch (Exception)
            {
            }

        }
      
        private void AddInput(HttpRequestBase request, ActionLogDTO config, IDictionary<string, object> actionParameters)
        {
            var listInput = Helper.ParseQueryStringToDictionary(request).Concat(actionParameters).Distinct().ToDictionary(d => d.Key, d => d.Value);

            Input = Helper.ParseDictionaryToJson(listInput);

            MapKeyValue(config, listInput);
        }

        private void MapOutValue(ActionLogDTO logAttr, object result)
        {
            if (result != null)
            {
                if (!string.IsNullOrEmpty(logAttr.KeyLogs.OutName_1))
                {
                    var propValue = result.GetType().GetProperty(logAttr.KeyLogs.OutName_1).GetValue(result, null);
                    OutName_1 = logAttr.KeyLogs.OutName_1;
                    OutValue_1 = Helper.ParseObjectToJson(propValue);
                }

                if (!string.IsNullOrEmpty(logAttr.KeyLogs.OutName_2))
                {
                    var propValue = result.GetType().GetProperty(logAttr.KeyLogs.OutName_2).GetValue(result, null);
                    OutName_2 = logAttr.KeyLogs.OutName_2;
                    OutValue_2 = Helper.ParseObjectToJson(propValue);
                }

                if (!string.IsNullOrEmpty(logAttr.KeyLogs.OutName_3))
                {
                    var propValue = result.GetType().GetProperty(logAttr.KeyLogs.OutName_3).GetValue(result, null);
                    OutName_3 = logAttr.KeyLogs.OutName_3;
                    OutValue_3 = Helper.ParseObjectToJson(propValue);
                }

                if (!string.IsNullOrEmpty(logAttr.KeyLogs.OutName_4))
                {
                    var propValue = result.GetType().GetProperty(logAttr.KeyLogs.OutName_4).GetValue(result, null);
                    OutName_4 = logAttr.KeyLogs.OutName_4;
                    OutValue_4= Helper.ParseObjectToJson(propValue);
                }
            }
        }

        private void MapKeyValue(ActionLogDTO logAttr, Dictionary<string, object> listInput)
        {
            if (listInput != null)
            {
                if (!string.IsNullOrEmpty(logAttr.KeyLogs.KeyName_1) && listInput.ContainsKey(logAttr.KeyLogs.KeyName_1))
                {
                    KeyName_1 = logAttr.KeyLogs.KeyName_1;
                    KeyValue_1 = JsonConvert.SerializeObject(listInput[logAttr.KeyLogs.KeyName_1]);
                }

                if (!string.IsNullOrEmpty(logAttr.KeyLogs.KeyName_2) && listInput.ContainsKey(logAttr.KeyLogs.KeyName_2))
                {
                    KeyName_2 = logAttr.KeyLogs.KeyName_2;
                    KeyValue_2 = JsonConvert.SerializeObject(listInput[logAttr.KeyLogs.KeyName_2]);
                }

                if (!string.IsNullOrEmpty(logAttr.KeyLogs.KeyName_3) && listInput.ContainsKey(logAttr.KeyLogs.KeyName_3))
                {
                    KeyName_3 = logAttr.KeyLogs.KeyName_3;
                    KeyValue_3 = JsonConvert.SerializeObject(listInput[logAttr.KeyLogs.KeyName_3]);
                }

                if (!string.IsNullOrEmpty(logAttr.KeyLogs.KeyName_4) && listInput.ContainsKey(logAttr.KeyLogs.KeyName_4))
                {
                    KeyName_4 = logAttr.KeyLogs.KeyName_4;
                    KeyValue_4 = JsonConvert.SerializeObject(listInput[logAttr.KeyLogs.KeyName_4]);
                }
            }
        }

        private void AddOutput(ActionExecutedContext context, ActionLogDTO config)
        {
            var resultType = context.Result.GetType();
            object result = null;
            switch (resultType.Name)
            {
                case "JsonResult":
                    result = ((JsonResult)context.Result).Data;
                    break;
                case "ViewResult":
                    result = ((ViewResult)context.Result).Model;
                    break;
                case "PartialViewResult":
                    result = ((PartialViewResult)context.Result).Model;
                    break;
                default:
                    break;
            }

            Output = JsonConvert.SerializeObject(result);
            MapOutValue(config, result);
        }
    }
}
