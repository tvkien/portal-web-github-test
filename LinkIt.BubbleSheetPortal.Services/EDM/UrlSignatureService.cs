using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Services.EDM
{
    public class UrlSignatureService : IUrlSignatureService
    {
        private readonly IReadOnlyRepository<APIAccount> _apiAccountRepository;

        public UrlSignatureService(IReadOnlyRepository<APIAccount> apiAccountRepository)
        {
            _apiAccountRepository = apiAccountRepository;
        }

        public string CreateUrlIncludeSignature(string originalUrl)
        {
            var uri = new Uri(originalUrl);
            var apiClientKey = GetApiClientKey();
            var queryString = new NameValueCollection
            {
                HttpUtility.ParseQueryString(uri.Query),
                { Constanst.EDM.QueryString.CLIENT_ID, apiClientKey.ClientId },
                { Constanst.EDM.QueryString.TIMESTAMP, DateTimeOffset.UtcNow.ToString(Constanst.EDM.Signature.SIGNATURE_EXPIRED_TIME_FORMAT, CultureInfo.InvariantCulture) }
            };
            var uriPath = uri.GetLeftPart(UriPartial.Path);
            var urlWithoutSignature = string.Format("{0}{1}", uriPath, ConvertNameValueCollectionToQueryString(queryString));
            var signature = GenerateHMACString(urlWithoutSignature, apiClientKey.ClientSecret);
            queryString.Add(Constanst.EDM.QueryString.SIGNATURE, signature);

            return string.Format("{0}{1}", uriPath, ConvertNameValueCollectionToQueryString(queryString));
        }

        private ApiClientKeyDto GetApiClientKey()
        {
            var apiAccount = _apiAccountRepository.Select().FirstOrDefault(x => x.LinkitPublicKey == Constanst.EDM.API_ACCOUNT_PUBLIC_KEY);
            return JsonConvert.DeserializeObject<ApiClientKeyDto>(apiAccount.LinkitPrivateKey);
        }

        private string GenerateHMACString(string message, string key)
        {
            using (var hmac = new HMACSHA1(Encoding.ASCII.GetBytes(key)))
            {
                byte[] hashMessage = hmac.ComputeHash(Encoding.ASCII.GetBytes(message));
                return string.Concat(hashMessage.Select(b => b.ToString("x2")));
            }
        }

        private string ConvertNameValueCollectionToQueryString(NameValueCollection nvCollection)
        {
            if (nvCollection == null) return string.Empty;

            var sb = new StringBuilder();

            foreach (string key in nvCollection.Keys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;

                string[] values = nvCollection.GetValues(key);
                if (values == null) continue;

                foreach (string value in values)
                {
                    sb.Append(sb.Length == 0 ? "?" : "&");
                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), value);
                }
            }

            return sb.ToString();
        }
    }
}
