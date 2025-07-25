using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.CustomException;
using LinkIt.BubbleSheetPortal.Models.DTOs.EDM;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Services.EDM
{
    public class EDMHttpRequest : IEDMHttpRequest
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IUrlSignatureService _urlSignatureService;
        private readonly HttpContextBase _httpContext;

        public EDMHttpRequest(IUrlSignatureService urlSignatureService, HttpContextBase httpContext)
        {
            _urlSignatureService = urlSignatureService;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("LICode", HttpContext.Current.GetLICodeFromRequest());
            _httpContext = httpContext;
        }

        public T SendPostRequest<T>(string requestUri, object payload = null)
        {
            var request = CreateRequest(requestUri, HttpMethod.Post, payload);
            var response =  _httpClient.SendAsync(request).Result;
            return ReadEDMResponse<T>(response);
        }

        public T SendGetRequest<T>(string requestUri)
        {
            var request = CreateRequest(requestUri, HttpMethod.Get);
            var response = _httpClient.SendAsync(request).Result;
            return ReadEDMResponse<T>(response);
        }

        public T SendDeleteRequest<T>(string requestUri, object payload = null)
        {
            var request = CreateRequest(requestUri, HttpMethod.Delete, payload);
            var response = _httpClient.SendAsync(request).Result;
            return ReadEDMResponse<T>(response);
        }

        private T ReadEDMResponse<T>(HttpResponseMessage response)
        {
            var responseString = response.Content.ReadAsStringAsync().Result;

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<EDMResponseDto<T>>(responseString).Data;
                case HttpStatusCode.BadRequest:
                    throw new NotFoundException($"EDM Response: {responseString}");
                case HttpStatusCode.Forbidden:
                    throw new ForbiddenException($"EDM Response: {responseString}");
                default:
                    throw new IntegrationException($"Call EDM Api failed. Response: {responseString}");
            }
        }

        private HttpRequestMessage CreateRequest(string requestUri, HttpMethod httpMethod, object payload = null)
        {
            var urlSignature = _urlSignatureService.CreateUrlIncludeSignature(requestUri);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(urlSignature),
                Method = httpMethod,
            };

            request.Headers.Add("CurrentUser", JsonConvert.SerializeObject(new { UserId = _httpContext.User.GetPropValue<int>("Id"), DisplayName = _httpContext.User.GetPropValue<string>("Name") }));

            if (payload != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8,
                    Constanst.EDM.ContentType.JSON);
            }

            return request;
        }
    }
}
