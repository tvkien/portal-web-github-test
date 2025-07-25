using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.Models.Infrastructure
{
    public class CorsSettings
    {
        private IEnumerable<string> _allowedOrigins = Enumerable.Empty<string>();

        public bool AllowAllOrigins { get; set; }

        public IEnumerable<string> AllowedOrigins
        {
            get => _allowedOrigins;
            set => _allowedOrigins = value ?? Enumerable.Empty<string>();
        }
    }
}
