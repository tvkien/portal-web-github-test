using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.TLDS
{
    public class TLDSUserMetaValueModel
    {
        public TLDSUserConfigurations TLDSUserConfigurations { get; set; }

        public static TLDSUserMetaValueModel ParseFromJsonData(string jsonData)
        {
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                return null;
            }
            var deserializeValue = new JavaScriptSerializer().Deserialize<TLDSUserMetaValueModel>(jsonData);
            return deserializeValue;
        }
        
    }
}
