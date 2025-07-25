using System;
using System.Runtime.Caching;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class CacheManager
    {
        private static readonly MemoryCache cacher = MemoryCache.Default;

        public static T Get<T>(string key)
        {
            if (cacher == null)
            {
                return default;
            }

            return (T)cacher.Get(key);
        }

        public static void Add<T>(string key, T value, int expiredTimeInMinutes = 60)
        {
            if (cacher != null)
            {
                cacher.Set(key, value, DateTimeOffset.UtcNow.AddMinutes(expiredTimeInMinutes));
            }
        }

        public static T GetOrAdd<T>(string key, Func<T> function, int expiredTimeInMinutes = 60)
        {
            var reponse = Get<T>(key);

            if (reponse != null)
            {
                return reponse;
            }

            reponse = function();

            if (reponse == null)
            {
                return default;
            }

            Add(key, reponse, expiredTimeInMinutes);
            return reponse;
        }
    }
}
