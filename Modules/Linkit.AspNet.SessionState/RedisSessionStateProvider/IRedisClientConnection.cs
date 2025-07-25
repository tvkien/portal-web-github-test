using System;
using System.Web.SessionState;

namespace Linkit.AspNet.SessionState
{
    internal interface IRedisClientConnection
    {
        bool Expiry(string key, int timeInSeconds);
        object Eval(string script, string[] keyArgs, object[] valueArgs);
        string GetLockId(object rowDataFromRedis);
        int GetSessionTimeout(object rowDataFromRedis);
        bool IsLocked(object rowDataFromRedis);
        ISessionStateItemCollection GetSessionData(object rowDataFromRedis);
        void Set(string key, byte[] data, DateTime utcExpiry);
        byte[] Get(string key);
        void Remove(string key);
        byte[] GetOutputCacheDataFromResult(object rowDataFromRedis);
    }
}
