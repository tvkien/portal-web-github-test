using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class ConfigHelper
    {
        
        public static bool SaveUploadItemToLocal
        {
            get
            {
                var value = ConfigurationManager.AppSettings["SaveUploadItemToLocal"];
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                if (value.ToLower().Equals("true"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
        }
        public static string ThirdPartyItemMediaPath
        {
            get
            {
                var value = ConfigurationManager.AppSettings["ThirdPartyItemMediaPath"];
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }
                return value;
            }

        }
        public static string AUVirtualTestFolder
        {
            get
            {
                var value = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;
               
                return value;
            }

        }
        public static string DataFileUploadPath
        {
            get
            {
                var value = ConfigurationManager.AppSettings["DataFileUploadPath"];
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }
                return value;
            }

        }

        public static int GetDefaultCacheInMinute
        {
            get
            {
                int defaultCacheInMinute = 60;
                if (ConfigurationManager.AppSettings["DefaultCacheInMinute"] != null)
                {
                    int.TryParse(ConfigurationManager.AppSettings["DefaultCacheInMinute"].ToString(), out defaultCacheInMinute);
                }

                return defaultCacheInMinute;
            }
           
        }
    }
}