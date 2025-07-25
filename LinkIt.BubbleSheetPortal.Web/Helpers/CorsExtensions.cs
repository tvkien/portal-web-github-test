using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    internal static class CorsExtensions
    {
        private const string _wildcardSubdomain = "*.";

        public static bool IsOriginAnAllowedSubdomain(this string origin, IEnumerable<string> allowedOrigins)
        {
            if (allowedOrigins.Contains(origin))
            {
                return true;
            }

            if (Uri.TryCreate(origin, UriKind.Absolute, out var originUri))
            {
                return allowedOrigins
                    .Where(o => o.Contains($"://{_wildcardSubdomain}"))
                    .Select(CreateDomainUri)
                    .Any(domain => UriHelpers.IsSubdomainOf(originUri, domain));
            }

            return false;
        }

        private static Uri CreateDomainUri(string origin)
        {
            return new Uri(origin.Replace(_wildcardSubdomain, string.Empty), UriKind.Absolute);
        }
    }
}
