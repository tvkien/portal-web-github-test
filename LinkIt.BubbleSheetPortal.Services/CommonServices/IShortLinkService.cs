using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Services.CommonServices
{
    public interface IShortLinkService
    {
        string GetFullLink(string key);
        string GenerateShortLink(string url);
    }
}
