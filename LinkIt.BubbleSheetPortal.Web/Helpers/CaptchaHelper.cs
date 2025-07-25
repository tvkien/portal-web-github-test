using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class CaptchaHelper
    {
        public static CaptchaResponse Verify(string secretKey,string googleCaptchaUrl,string recaptChaResponse)
        {
            using (var client = new HttpClient())
            {
                var url = string.Format("{0}?secret={1}&response={2}", googleCaptchaUrl, secretKey, recaptChaResponse);
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var reply = response.Content.ReadAsStringAsync().Result;
                    var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);
                    return captchaResponse;
                }
                else
                {
                    return new CaptchaResponse(){Success = false};
                }
            }
        }
    }

    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}