using LinkIt.BubbleSheetPortal.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace LinkIt.BubbleSheetPortal.Web.HealthCheck
{
    public static class InternalHealthChecker
    {
        private static readonly HttpClient _client = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        private static readonly Dictionary<string, string> HealthUrls = new Dictionary<string, string>
        {
            { "IM Api", AppSetting.HealthCheckIMAPIUrlLocal },
            { "Reporting Api", AppSetting.HealthCheckReportingAPIUrlLocal },
            { "TestTaker Api", AppSetting.HealthCheckTestTakerAPIUrlLocal },
            { "Rest Api", AppSetting.HealthCheckRESTAPIUrlLocal },

        };

        public static HealthCheckDto HealthyCheck(System.Web.HttpRequest request)
        {
            try
            {
                var path = request.Path.ToLowerInvariant();

                if (path == "/awshealthcheck.html")
                {
                    return null;
                }

                if (path == "/healthcheck")
                {
                    foreach (var healthUrl in HealthUrls)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(healthUrl.Value))
                            {
                                continue;
                            }

                            var res = _client.GetAsync(healthUrl.Value).Result;

                            if (!res.IsSuccessStatusCode)
                            {
                                return new HealthCheckDto
                                {
                                    StatusCode = 503,
                                    Message = $"{healthUrl.Key} Unhealthy"
                                };
                            }
                        }
                        catch
                        {
                            return new HealthCheckDto
                            {
                                StatusCode = 503,
                                Message = $"{healthUrl.Key} Unhealthy"
                            };
                        }
                    }

                    return new HealthCheckDto
                    {
                        StatusCode = 200,
                        Message = "Healthy"
                    };
                }

                if (HealthUrls.Any(x => x.Value.ToLower() == request.Url.AbsoluteUri?.ToLower()))
                {
                    var apiName = HealthUrls.FirstOrDefault(x => x.Value.ToLower() == request.Url.AbsoluteUri?.ToLower()).Key;

                    return new HealthCheckDto
                    {
                        StatusCode = 503,
                        Message = $"{apiName} Unhealthy"
                    };
                }

                if (!request.Url.Host.ToLower().Contains(AppSetting.LinkItUrl?.ToLower()))
                {
                    return new HealthCheckDto
                    {
                        StatusCode = 404,
                        Message = "Host name is invalid."
                    };
                }
            }
            catch {}

            return null;
        }
    }

    public class HealthCheckDto
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }
    }
}
