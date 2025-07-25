using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Services.Reporting
{
    public class ReportingHttpClient: IReportingHttpClient
    {
        private readonly HttpClient _httpClient;
        private string _serviceUrl;
        private readonly HttpContextBase _httpContext;

        private string ServiceUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_serviceUrl))
                {
                    var liCode = HttpContext.Current.GetLICodeFromRequest();
                    _serviceUrl = ConfigurationManager.AppSettings[Constanst.AdminReporting.Configuration.REPORTING_URL];
                    _serviceUrl = _serviceUrl.Replace("[LICode]", liCode);
                }

                return _serviceUrl;
            }
        }

        public ReportingHttpClient(HttpContextBase httpContext)
        {
            _httpClient = new HttpClient();
            _httpContext = httpContext;
        }

        public T Get<T>(string requestUri)
        {
            var request = CreateRequest(requestUri, HttpMethod.Get);
            var response = _httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var responseStream = response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<T>(responseStream.Result);
            return data;
        }

        public T Put<T>(string requestUri, object payload = null)
        {
            var request = CreateRequest(requestUri, HttpMethod.Put, payload);
            var response = _httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var responseStream = response.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<T>(responseStream.Result);
            return data;
        }

        private HttpRequestMessage CreateRequest(string requestUri, HttpMethod httpMethod, object payload = null)
        {
            var uri = new Uri(new Uri(ServiceUrl), requestUri);
            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = httpMethod,
            };

            var authorizationToken = _httpContext.Request.Cookies[Constanst.LKARCookie];
            request.Headers.Add("Authorization", authorizationToken?.Value);

            if (payload != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8,
                    "application/json");
            }

            return request;
        }
    }
}
