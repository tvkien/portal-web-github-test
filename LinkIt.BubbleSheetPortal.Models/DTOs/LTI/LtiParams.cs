using System.Collections.Generic;
using System.Collections.Specialized;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.LTI
{
    public class LtiParams
    {
        public Dictionary<string, string> ltiParams = new Dictionary<string, string>();

        public LtiParams(NameValueCollection vars)
        {
            if (vars != null && vars.Count > 0)
            {
                foreach (string key in vars.Keys)
                {
                    ltiParams.Add(key, vars[key]);
                }
            }
        }
    }
}
