using Linkit.AspNet.SessionState;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace LinkIt.BubbleSheetPortal.Web.Resolver
{
    public class CustomSessionStateProvider : SessionStateStoreProviderBase
    {
        private readonly RedisSessionStateProvider _redisSessionStateProvider;
        private readonly SqlSessionStateProvider _sqlSessionStateProvider;
        private NameValueCollection _config;

        public CustomSessionStateProvider()
        {
            _redisSessionStateProvider = new RedisSessionStateProvider();
            _sqlSessionStateProvider = new SqlSessionStateProvider();
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            _config = config;
            _redisSessionStateProvider.Initialize(name, config);
            _sqlSessionStateProvider.Initialize(name, config);
        }

        private SessionStateStoreProviderBase GetCurrentProvider(HttpContext context)
        {
            try
            {
                var vault = LinkitConfigurationManager.GetVault(context);
                if (vault == null)
                {
                    return null;
                }

                if (vault?.ASPState?.Mode == "Redis" && vault.ASPState?.RedisConnection != null)
                {
                    var connectionString = LinkitConfigurationManager.BuildConnectionString(vault.ASPState?.RedisConnection);
                    _config["connectionString"] = connectionString;
                    _redisSessionStateProvider.RefreshConnection(_config);
                    return _redisSessionStateProvider;
                }
                else
                {
                    var connectionString = LinkitConfigurationManager.BuildConnectionString(vault.ASPState?.SQLConnection);
                    _config["connectionString"] = connectionString;
                    _sqlSessionStateProvider.RefreshConnection(_config);
                    return _sqlSessionStateProvider;
                }
            }
            catch (Exception) { }
            return _sqlSessionStateProvider;
        }

        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            var provider = GetCurrentProvider(context);
            if (provider == null)
            {
                return _sqlSessionStateProvider.CreateNewStoreData(context, timeout);
            }

            return provider.CreateNewStoreData(context, timeout);
        }

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            var provider = GetCurrentProvider(context);
            if (provider == null) return;

            provider.CreateUninitializedItem(context, id, timeout);
        }

        public override void Dispose()
        {
            _redisSessionStateProvider.Dispose();
            _sqlSessionStateProvider.Dispose();
        }

        public override void EndRequest(HttpContext context)
        {
            var provider = GetCurrentProvider(context);
            if (provider == null) return;

            provider.EndRequest(context);
        }

        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            var provider = GetCurrentProvider(context);
            if (provider == null)
            {
                locked = false;
                lockAge = TimeSpan.Zero;
                lockId = 0;
                actions = SessionStateActions.None;
                return null;
            }

            return provider.GetItem(context, id, out locked, out lockAge, out lockId, out actions);
        }

        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            var provider = GetCurrentProvider(context);
            if (provider == null)
            {
                locked = false;
                lockAge = TimeSpan.Zero;
                lockId = 0;
                actions = SessionStateActions.None;
                return null;
            }

            return provider.GetItemExclusive(context, id, out locked, out lockAge, out lockId, out actions);
        }

        public override void InitializeRequest(HttpContext context)
        {
            _redisSessionStateProvider.InitializeRequest(context);
            _sqlSessionStateProvider.InitializeRequest(context);
        }

        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            var provider = GetCurrentProvider(context);
            if (provider == null) return;

            provider.ReleaseItemExclusive(context, id, lockId);
        }

        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            var provider = GetCurrentProvider(context);
            if (provider == null) return;

            provider.RemoveItem(context, id, lockId, item);
        }

        public override void ResetItemTimeout(HttpContext context, string id)
        {
            if (!IsKeepRequest(context)) return;

            var provider = GetCurrentProvider(context);
            if (provider == null) return;

            provider.ResetItemTimeout(context, id);
        }

        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            var provider = GetCurrentProvider(context);
            if (provider == null) return;

            provider.SetAndReleaseItemExclusive(context, id, item, lockId, newItem);
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return false;
        }

        private bool IsKeepRequest(HttpContext context)
        {
            try
            {
                if (context.Request.HttpMethod != "GET") return true;
                if (string.IsNullOrEmpty(context.Request.CurrentExecutionFilePathExtension)) return true;
                if (!File.Exists(context.Server.MapPath(context.Request.Url.AbsolutePath))) return true;
                var isKeepRequestsToStaticFiles = CacheManager.GetOrAdd<bool?>(ConfigurationKey.IsKeepRequestsToStaticFiles, () =>
                {
                    var configurationService = IoCContainer.GetService<ConfigurationService>(DependencyResolver.Current);
                    var value = configurationService.GetConfigurationByKeyWithDefaultValue(ConfigurationKey.IsKeepRequestsToStaticFiles, false);
                    return value;
                }, 5);
                return isKeepRequestsToStaticFiles ?? false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
