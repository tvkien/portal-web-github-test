using LinkIt.BubbleSheetPortal.Web.Models.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Security
{
    public class PortalAuditManager
    {
        private const string PortalLoggingKey = "PortalLoggingKey";
        //private const string MessageTemplate = "{@StartTime} {@Duration} {@UserID} {@UserName} {@RoleID} {@DistrictID} {@IPAddress} {@InstanceName} {@Url} {@Method} {@ActionName} {@ControllerName} {@QueryString} {@Payload} {@StatusCode} {@IsSuccess} {@Exceptions} {@ErrorLogs}";

        public static PortalLogEntry PortalLogEntry
        {
            get
            {
                if (HttpContext.Current == null) return null;

                var loggingData = HttpContext.Current.Items[PortalLoggingKey] as PortalLogEntry;

                if (loggingData == null)
                {
                    loggingData = new PortalLogEntry();
                    HttpContext.Current.Items[PortalLoggingKey] = loggingData;
                }

                return loggingData;
            }
        }

        public static void LogFull()
        {
            if (PortalLogEntry == null)
            {
                return;
            }

            var logMessage = JsonConvert.SerializeObject(PortalLogEntry);
            Serilog.Log.Information(logMessage);
        }

        public static void LogRequest(
            HttpRequestBase request,
            UserPrincipal currentUser,
            ActionExecutingContext filterContext)
        {
            var routeData = filterContext.RouteData;
            var actionName = routeData.Values["action"]?.ToString();
            var controllerName = routeData.Values["controller"]?.ToString();

            if (PortalLogEntry == null || request == null || string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName)) return;

            PortalLogEntry.StartTime = DateTime.UtcNow;
            PortalLogEntry.IPAddress = GetIpAddress(request);
            PortalLogEntry.InstanceName = Environment.MachineName;
            PortalLogEntry.Url = request.Url?.ToString();
            PortalLogEntry.Method = request.HttpMethod;
            PortalLogEntry.QueryString = request.QueryString?.ToString();
            PortalLogEntry.Payload = GetRequestBody(request, controllerName, actionName);

            if (currentUser != null)
            {
                PortalLogEntry.UserID = currentUser.Id;
                PortalLogEntry.UserName = currentUser.UserName;
                PortalLogEntry.RoleID = currentUser.RoleId;
                PortalLogEntry.DistrictID = currentUser.DistrictId ?? 0;
            }
            
            PortalLogEntry.ActionName = actionName;
            PortalLogEntry.ControllerName = controllerName;
            PortalLogEntry.IsSuccess = true;
        }

        public static void LogResponse(HttpResponseBase response)
        {
            if (PortalLogEntry == null || response == null) return;
            PortalLogEntry.StatusCode = response.StatusCode;
            PortalLogEntry.Duration = DateTime.UtcNow.Subtract(PortalLogEntry.StartTime).TotalSeconds;
        }

        public static void LogError(string errorMessage)
        {
            if (PortalLogEntry == null || string.IsNullOrWhiteSpace(errorMessage)) return;

            if (PortalLogEntry.ErrorLogs == null)
            {
                PortalLogEntry.ErrorLogs = new List<string>();
            }

            PortalLogEntry.ErrorLogs.Add(errorMessage);
        }

        public static void LogException(Exception exception)
        {
            if (PortalLogEntry == null || exception == null) return;

            if (PortalLogEntry.Exceptions == null)
            {
                PortalLogEntry.Exceptions = new List<string>();
            }

            PortalLogEntry.IsSuccess = false;
            PortalLogEntry.StatusCode = 500;
            PortalLogEntry.Exceptions.Add(exception.ToString());
        }

        private static string GetIpAddress(HttpRequestBase request)
        {
            // Get the IP address from the request
            string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        private static string GetRequestBody(HttpRequestBase request, string controllerName, string actionName)
        {
            try
            {
                // Check for large requests and file uploads
                const int maxContentLength = 1024 * 1024; // 1 MB (adjust as needed)

                if (request.ContentLength > maxContentLength)
                {
                    return string.Empty; // Skip large requests
                }

                // Check for file uploads based on ContentType
                if (request.ContentType != null && (request.ContentType.Contains("multipart/form-data") || request.ContentType.Contains("application/octet-stream")))
                {
                    return string.Empty; // Skip file uploads
                }

                if (request.HttpMethod == "POST" || request.HttpMethod == "PUT")
                {
                    request.InputStream.Position = 0;
                    using (StreamReader reader = new StreamReader(request.InputStream))
                    {
                        var requestBody = reader.ReadToEnd();
                        string sanitizedBody = SanitizeBody(requestBody, controllerName, actionName);
                        return sanitizedBody;
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        private static string SanitizeBody(string requestBody, string controllerName, string actionName)
        {
            try
            {
                if ((controllerName == "Account" && actionName == "LogOn") ||
                    (controllerName == "Student" && actionName == "LogOn"))
                {
                    string sanitizedBody = Regex.Replace(requestBody, @"(?<=\bPassword=)[^&]*", "***", RegexOptions.IgnoreCase);
                    return sanitizedBody;
                }
                return requestBody;
            }
            catch (Exception)
            {
                return requestBody;
            }
        }
    }
}
