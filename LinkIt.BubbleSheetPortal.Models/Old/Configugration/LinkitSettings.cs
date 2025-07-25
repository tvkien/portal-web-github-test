using LinkIt.BubbleSheetPortal.Models.Old.Configugration;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.Configugration
{
    public class LinkitSettings
    {
        public S3Settings S3Settings { get; set; }
        public DatabaseSettings DatabaseSettings { get; set; }
        public string DatabaseID { get; set; }
        public AppSettings AppSettings { get; set; }
        public TTLConfigsModel TTLConfigs { get; set; }
        public List<EmailCredentialSetting> EmailCredentialSettings { get; set; }
    }
}
