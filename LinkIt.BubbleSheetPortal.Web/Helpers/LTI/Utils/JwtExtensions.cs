using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Utils
{
    internal static class JwtExtensions
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string GetClaimValue(this JwtPayload payload, string type)
        {
            return GetClaimValue<string>(payload, type);
        }

        public static T GetClaimValue<T>(this JwtPayload payload, string type)
        {
            if (typeof(T).IsArray)
            {
                return GetClaimValues<T>(payload, type);
            }

            if (payload.TryGetValue(type, out var value))
            {
                return typeof(T) == typeof(string)
                    ? JsonConvert.DeserializeObject<T>($"\"{value}\"")
                    : JsonConvert.DeserializeObject<T>(value.ToString());
            }

            return default(T);
        }

        private static T GetClaimValues<T>(this JwtPayload payload, string type)
        {
            var values = payload.Claims
                .Where(c => c.Type == type)
                .Select(c => c.Value).ToArray();

            if (0 == values.Length)
                return default(T);

            var elementType = typeof(T).GetElementType();
            if (elementType != null && elementType.IsClass && !elementType.IsEquivalentTo(typeof(string)))
            {
                return JsonConvert.DeserializeObject<T>("[" + string.Join(",", values) + "]");
            }
            return JsonConvert.DeserializeObject<T>("[\"" + string.Join("\",\"", values) + "\"]");
        }

        public static void SetClaimValue<T>(this JwtPayload payload, string type, T value)
        {
            if (payload.ContainsKey(type))
            {
                payload.Remove(type);
            }

            if (typeof(T) == typeof(string))
            {
                var stringValue = value?.ToString();
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    payload.AddClaim(new Claim(type, stringValue, ClaimValueTypes.String));
                }
            }
            else if (typeof(T) == typeof(int))
            {
                payload.AddClaim(new Claim(type, value.ToString(), ClaimValueTypes.Integer));
            }
            else if (typeof(T).IsArray)
            {
                payload.AddClaim(new Claim(type, JsonConvert.SerializeObject(value, Settings), JsonClaimValueTypes.JsonArray));
            }
            else
            {
                var json = JsonConvert.SerializeObject(value, Settings);
                payload.AddClaim(new Claim(type, json, JsonClaimValueTypes.Json));
            }
        }
    }
}
