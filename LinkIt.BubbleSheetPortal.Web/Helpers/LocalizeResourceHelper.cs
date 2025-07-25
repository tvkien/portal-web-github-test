using LinkIt.BubbleSheetPortal.Web.Helpers;
using System.Web;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web
{
    public class LocalizeHelper
    {
        public static IHtmlString Localized(string key)
        {
            int districtID = HttpContext.Current.GetCurrentDistrictID();

            string cacheKey = string.Format("{0}_{1}", key, districtID);
            if (!CacheHelper.Exists(cacheKey))
            {
                var localizeService = DependencyResolver.Current.GetService<Services.LocalizeResourceService>();
                string label = localizeService.GetLabelByKey(districtID, key);

                CacheHelper.Add(label, cacheKey, ConfigHelper.GetDefaultCacheInMinute);
            }

            string result;
            CacheHelper.Get(cacheKey, out result);
            result = string.Format("<!--{0}-->{1}", key, result);

            return new HtmlString(result);
        }

        public static IHtmlString LocalizedWithoutComment(string key)
        {
            int districtID = HttpContext.Current.GetCurrentDistrictID();

            string cacheKey = string.Format("{0}_{1}", key, districtID);
            if (!CacheHelper.Exists(cacheKey))
            {
                var localizeService = DependencyResolver.Current.GetService<Services.LocalizeResourceService>();
                string label = localizeService.GetLabelByKey(districtID, key);

                CacheHelper.Add(label, cacheKey, ConfigHelper.GetDefaultCacheInMinute);
            }

            string result;
            CacheHelper.Get(cacheKey, out result);
            result = string.Format("{0}", result);

            return new HtmlString(result);
        }

        public static IHtmlString LocalizedWithoutComment(string key, int districtId = 0)
        {
            if (districtId == 0)
                districtId = HttpContext.Current.GetCurrentDistrictID();

            string cacheKey = string.Format("{0}_{1}", key, districtId);
            if (!CacheHelper.Exists(cacheKey))
            {
                var localizeService = DependencyResolver.Current.GetService<Services.LocalizeResourceService>();
                string label = localizeService.GetLabelByKey(districtId, key);

                CacheHelper.Add(label, cacheKey, ConfigHelper.GetDefaultCacheInMinute);
            }

            string result;
            CacheHelper.Get(cacheKey, out result);
            result = string.Format("{0}", result);

            return new HtmlString(result);
        }

        public static string LocalizedToString(string key)
        {
            int districtID = HttpContext.Current.GetCurrentDistrictID();

            string cacheKey = $"{key}_{districtID}";
            if (!CacheHelper.Exists(cacheKey))
            {
                var localizeService = DependencyResolver.Current.GetService<Services.LocalizeResourceService>();
                string label = localizeService.GetLabelByKey(districtID, key);

                CacheHelper.Add(label, cacheKey, ConfigHelper.GetDefaultCacheInMinute);
            }

            string result;
            CacheHelper.Get(cacheKey, out result);
            if (result == key)
                return string.Empty;

            return result;
        }
    }
}
