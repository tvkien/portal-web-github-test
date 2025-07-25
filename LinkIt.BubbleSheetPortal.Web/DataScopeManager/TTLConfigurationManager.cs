using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Web.Resolver;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Web.Caching;
using System.Configuration;

namespace LinkIt.BubbleSheetPortal.Web.DataScopeManager
{
    public static class TTLConfigurationManager
    {
        private const string TTLCacheKey = "TTLCacheKey";

        private static List<TTLConfigs> _ttlConfigs
        {
            get
            {
                var ttlService = IoCContainer.GetService<TTLConfigService>(DependencyResolver.Current);
                var ttlConfigs = ttlService.GetAllTTLConfigs();

                return ttlConfigs;
            }
        }

        public static List<TTLConfigs> TTLConfigs
        {
            get
            {
                List<TTLConfigs> result = HttpContext.Current.Cache[TTLCacheKey] as List<TTLConfigs>;
                if (result == null)
                {
                    result = _ttlConfigs;
                    if (result != null)
                    {
                        int cacheTTLExpireInMinute = 60;
                        if(ConfigurationManager.AppSettings["CacheTTLExpireInMinute"] != null)
                        {
                            int.TryParse(ConfigurationManager.AppSettings["CacheTTLExpireInMinute"].ToString(), out cacheTTLExpireInMinute);
                        }

                        HttpContext.Current.Cache.Add(TTLCacheKey, result, null, DateTime.Now.AddMinutes(cacheTTLExpireInMinute), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                    }
                }

                return result;
            }
        }
    }
}