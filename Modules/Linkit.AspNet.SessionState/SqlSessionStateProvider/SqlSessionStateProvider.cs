using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;

namespace Linkit.AspNet.SessionState
{
    public class SqlSessionStateProvider : SessionStateStoreProviderBase
    {
        private const string REPOSITORY_TYPE_CONFIGURATION_NAME = "repositoryType";
        private const string INMEMORY_TABLE_CONFIGURATION_NAME = "useInMemoryTable";
        private const string MAX_RETRY_NUMBER_CONFIGURATION_NAME = "maxRetryNumber";
        private const string RETRY_INTERVAL_CONFIGURATION_NAME = "retryInterval";
        private const string CONNECTIONSTRING_NAME_CONFIGURATION_NAME = "connectionStringName";
        private const string CONNECTIONSTRING = "connectionString";
        private const string SESSION_TABLE_CONFIGURATION_NAME = "sessionTableName";
        private const string SESSIONSTATE_SECTION_PATH = "system.web/sessionState";
        private const double SessionExpiresFrequencyCheckIntervalTicks = 30 * TimeSpan.TicksPerSecond;
        private static long s_lastSessionPurgeTicks;
        private static int s_inPurge;
        private static string s_appSuffix;
        private static bool s_compressionEnabled;
        private static bool s_oneTimeInited = false;
        private static object s_lock = new object();
        private ISqlSessionStateRepository s_sqlSessionStateRepository;
        private static RepositoryType s_repositoryType;
        private static Dictionary<string, ISqlSessionStateRepository> _sessionStateRepositoryDict = new Dictionary<string, ISqlSessionStateRepository>();
        private int _rqOrigStreamLen;
        /// <summary>
        /// Initialize the provider through the configuration
        /// </summary>
        /// <param name="name">Sessionstate provider name</param>
        /// <param name="config">Configuration values</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (String.IsNullOrEmpty(name))
            {
                name = "SqlSessionStateProvider";
            }
            var ssc = (SessionStateSection)ConfigurationManager.GetSection(SESSIONSTATE_SECTION_PATH);
            var connectionString = config[CONNECTIONSTRING];
            base.Initialize(name, config);
            Initialize(config, ssc, connectionString, true);
            var appId = AppId ?? HttpRuntime.AppDomainAppId;
            Debug.Assert(appId != null);
            s_appSuffix = appId.GetHashCode().ToString("X8", CultureInfo.InvariantCulture);
        }
        public void RefreshConnection(NameValueCollection config)
        {
            var ssc = (SessionStateSection)ConfigurationManager.GetSection(SESSIONSTATE_SECTION_PATH);
            var connectionString = config[CONNECTIONSTRING];
            Initialize(config, ssc, connectionString, true);
        }

        internal void Initialize(NameValueCollection config, SessionStateSection ssc, string connectionString,
                                    bool shouldCreateTable = false)
        {
            if (string.IsNullOrEmpty(connectionString)) return;
            if (_sessionStateRepositoryDict.TryGetValue(connectionString, out var repository))
            {
                s_sqlSessionStateRepository = repository;
            }
            else
            {
                lock (s_lock)
                {
                    if (_sessionStateRepositoryDict.TryGetValue(connectionString, out var repository2))
                    {
                        s_sqlSessionStateRepository = repository2;
                    }
                    else
                    {
                        string tableName = config[SESSION_TABLE_CONFIGURATION_NAME];   // Null/Not-found is ok
                        s_sqlSessionStateRepository = new SqlSessionStateRepository(connectionString, tableName,
                                (int)ssc.SqlCommandTimeout.TotalSeconds, GetRetryInterval(config), GetMaxRetryNum(config));
                        if (shouldCreateTable)
                        {
                            s_sqlSessionStateRepository.CreateSessionStateTable();
                        }
                        _sessionStateRepositoryDict[connectionString] = s_sqlSessionStateRepository;
                    }
                }
            }
        }
        #region properties/methods for unit tests
        internal ISqlSessionStateRepository SqlSessionStateRepository
        {
            get { return s_sqlSessionStateRepository; }
            set { s_sqlSessionStateRepository = value; }
        }
        internal RepositoryType RepositoryType
        {
            get { return s_repositoryType; }
            set { s_repositoryType = value; }
        }
        internal bool CompressionEnabled
        {
            get { return s_compressionEnabled; }
        }
        internal void ResetOneTimeInited()
        {
            s_oneTimeInited = false;
        }
        internal string AppId
        {
            get; set;
        }
        internal int OrigStreamLen
        {
            get { return _rqOrigStreamLen; }
        }
        internal static Func<HttpContext, HttpStaticObjectsCollection> GetSessionStaticObjects
        {
            get; set;
        } = SessionStateUtility.GetSessionStaticObjects;
        #endregion
        private int? GetMaxRetryNum(NameValueCollection config)
        {
            int maxRetryNum;
            var val = config[MAX_RETRY_NUMBER_CONFIGURATION_NAME];
            if (val != null && int.TryParse(val, out maxRetryNum))
            {
                return maxRetryNum;
            }
            return null;
        }
        private int? GetRetryInterval(NameValueCollection config)
        {
            int retryInterval;
            var val = config[RETRY_INTERVAL_CONFIGURATION_NAME];
            if (val != null && int.TryParse(val, out retryInterval))
            {
                return retryInterval;
            }
            return null;
        }
        /// <summary>
        /// Create a new SessionStateStoreData
        /// </summary>
        /// <param name="context">Httpcontext</param>
        /// <param name="timeout">Session timeout</param>
        /// <returns></returns>
        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            HttpStaticObjectsCollection staticObjects = null;
            if (context != null)
            {
                staticObjects = GetSessionStaticObjects(context.ApplicationInstance.Context);
            }
            return new SessionStateStoreData(new SessionStateItemCollection(), staticObjects, timeout);
        }

        /// <inheritdoc />
        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (id.Length > SessionIDManager.SessionIDMaxLength)
            {
                throw new ArgumentException(SqlResource.Session_id_too_long);
            }
            id = AppendAppIdHash(id);
            byte[] buf;
            int length;
            var item = new SessionStateStoreData(new SessionStateItemCollection(),
                        GetSessionStaticObjects(context.ApplicationInstance.Context),
                        timeout);
            SerializeStoreData(item, SqlSessionStateRepositoryUtil.DefaultItemLength, out buf, out length, s_compressionEnabled);
            s_sqlSessionStateRepository.CreateUninitializedSessionItem(id, length, buf, timeout);
        }
        /// <inheritdoc />
        public override void Dispose()
        {
        }

        /// <inheritdoc />
        public override void EndRequest(HttpContext context)
        {
            PurgeIfNeeded();
        }

        /// <inheritdoc />
        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            return GetItemFromSessionStore(false, context, id, out locked, out lockAge, out lockId, out actions);
        }

        /// <inheritdoc />
        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            var sessionData = GetItemFromSessionStore(true, context, id, out locked, out lockAge, out lockId, out actions);
            return sessionData;
        }

        /// <inheritdoc />
        public override void InitializeRequest(HttpContext context)
        {
            _rqOrigStreamLen = 0;
        }

        /// <inheritdoc />
        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (id.Length > SessionIDManager.SessionIDMaxLength)
            {
                throw new ArgumentException(SqlResource.Session_id_too_long);
            }
            id = AppendAppIdHash(id);
            s_sqlSessionStateRepository.ReleaseSessionItem(id, lockId);
        }
        /// <inheritdoc />
        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (id.Length > SessionIDManager.SessionIDMaxLength)
            {
                throw new ArgumentException(SqlResource.Session_id_too_long);
            }
            id = AppendAppIdHash(id);
            s_sqlSessionStateRepository.RemoveSessionItem(id, lockId);
        }
        /// <inheritdoc />
        public override void ResetItemTimeout(HttpContext context, string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (id.Length > SessionIDManager.SessionIDMaxLength)
            {
                throw new ArgumentException(SqlResource.Session_id_too_long);
            }
            id = AppendAppIdHash(id);
            s_sqlSessionStateRepository.ResetSessionItemTimeout(id);
        }
        /// <inheritdoc />
        public override void SetAndReleaseItemExclusive(
            HttpContext context,
            string id,
            SessionStateStoreData item,
            object lockId,
            bool newItem)
        {
            byte[] buf;
            int length;
            int lockCookie;
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (id.Length > SessionIDManager.SessionIDMaxLength)
            {
                throw new ArgumentException(SqlResource.Session_id_too_long);
            }
            id = AppendAppIdHash(id);
            try
            {
                SerializeStoreData(item, SqlSessionStateRepositoryUtil.DefaultItemLength, out buf, out length, s_compressionEnabled);
            }
            catch
            {
                if (!newItem)
                {
                    ReleaseItemExclusive(context, id, lockId);
                }
                throw;
            }
            lockCookie = lockId == null ? 0 : int.TryParse(lockId.ToString(), out var v) ? v : 0;
            s_sqlSessionStateRepository.CreateOrUpdateSessionStateItem(newItem, id, buf, length, item.Timeout, lockCookie, _rqOrigStreamLen);
        }
        /// <inheritdoc />
        public override bool SetItemExpireCallback(System.Web.SessionState.SessionStateItemExpireCallback expireCallback)
        {
            return false;
        }

        private SessionStateStoreData GetItemFromSessionStore(bool isWriteLockRequired, HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            SessionStateStoreData sessionStateStoreData = null;
            locked = false;
            lockAge = TimeSpan.Zero;
            lockId = 0;
            actions = SessionStateActions.None;
            if (id == null)
            {
                return null;
            }
            id = AppendAppIdHash(id);

            var sessionItem = s_sqlSessionStateRepository.GetSessionStateItem(id, isWriteLockRequired);
            if (sessionItem == null)
            {
                return null;
            }
            locked = sessionItem.Locked;
            lockAge = sessionItem.LockAge;
            lockId = sessionItem.LockId;
            actions = sessionItem.Actions;

            if (sessionItem.Item == null)
            {
                return null;
            }
            using (var stream = new MemoryStream(sessionItem.Item))
            {
                sessionStateStoreData = DeserializeStoreData(context, stream, s_compressionEnabled);
                _rqOrigStreamLen = (int)stream.Position;
            }
            return sessionStateStoreData;
        }


        // We just want to append an 8 char hash from the AppDomainAppId to prevent any session id collisions
        internal static string AppendAppIdHash(string id)
        {
            if (!id.EndsWith(s_appSuffix))
            {
                return id + s_appSuffix;
            }
            return id;
        }
        // Internal code copied from SessionStateUtility
        internal static void SerializeStoreData(
            SessionStateStoreData item,
            int initialStreamSize,
            out byte[] buf,
            out int length,
            bool compressionEnabled)
        {
            using (MemoryStream s = new MemoryStream(initialStreamSize))
            {
                Serialize(item, s);
                if (compressionEnabled)
                {
                    byte[] serializedBuffer = s.GetBuffer();
                    int serializedLength = (int)s.Length;
                    // truncate the MemoryStream so we can write the compressed data in it
                    s.SetLength(0);
                    // compress the serialized bytes
                    using (DeflateStream zipStream = new DeflateStream(s, CompressionMode.Compress, true))
                    {
                        zipStream.Write(serializedBuffer, 0, serializedLength);
                    }
                    // if the session state tables have ANSI_PADDING disabled, last )s are trimmed.
                    // This shouldn't happen, but to be sure, we are padding with an extra byte
                    s.WriteByte((byte)0xff);
                }
                buf = s.GetBuffer();
                length = (int)s.Length;
            }
        }
        private static void Serialize(SessionStateStoreData item, Stream stream)
        {
            bool hasItems = true;
            bool hasStaticObjects = true;
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(item.Timeout);
            if (item.Items == null || item.Items.Count == 0)
            {
                hasItems = false;
            }
            writer.Write(hasItems);
            if (item.StaticObjects == null || item.StaticObjects.NeverAccessed)
            {
                hasStaticObjects = false;
            }
            writer.Write(hasStaticObjects);
            if (hasItems)
            {
                ((SessionStateItemCollection)item.Items).Serialize(writer);
            }
            if (hasStaticObjects)
            {
                item.StaticObjects.Serialize(writer);
            }
            // Prevent truncation of the stream
            writer.Write(unchecked((byte)0xff));
        }
        internal static SessionStateStoreData DeserializeStoreData(HttpContext context, Stream stream, bool compressionEnabled)
        {
            if (compressionEnabled)
            {
                // apply the compression decorator on top of the stream
                // the data should not be bigger than 4GB - compression doesn't work for more than that
                using (DeflateStream zipStream = new DeflateStream(stream, CompressionMode.Decompress, true))
                {
                    return Deserialize(context, zipStream);
                }
            }
            return Deserialize(context, stream);
        }
        private static SessionStateStoreData Deserialize(HttpContext context, Stream stream)
        {
            int timeout;
            SessionStateItemCollection sessionItems;
            bool hasItems;
            bool hasStaticObjects;
            HttpStaticObjectsCollection staticObjects;
            byte eof;
            Debug.Assert(context != null);
            try
            {
                BinaryReader reader = new BinaryReader(stream);
                timeout = reader.ReadInt32();
                hasItems = reader.ReadBoolean();
                hasStaticObjects = reader.ReadBoolean();
                if (hasItems)
                {
                    sessionItems = SessionStateItemCollection.Deserialize(reader);
                }
                else
                {
                    sessionItems = new SessionStateItemCollection();
                }
                if (hasStaticObjects)
                {
                    staticObjects = HttpStaticObjectsCollection.Deserialize(reader);
                }
                else
                {
                    staticObjects = GetSessionStaticObjects(context.ApplicationInstance.Context);
                }
                eof = reader.ReadByte();
                if (eof != 0xff)
                {
                    throw new HttpException(String.Format(CultureInfo.CurrentCulture, SqlResource.Invalid_session_state));
                }
            }
            catch (EndOfStreamException)
            {
                throw new HttpException(String.Format(CultureInfo.CurrentCulture, SqlResource.Invalid_session_state));
            }
            return new SessionStateStoreData(sessionItems, staticObjects, timeout);
        }
        private bool CanPurge()
        {
            return (
                DateTime.UtcNow.Ticks - s_lastSessionPurgeTicks > SessionExpiresFrequencyCheckIntervalTicks
                && Interlocked.CompareExchange(ref s_inPurge, 1, 0) == 0
                );
        }
        private void PurgeIfNeeded()
        {
            // Only check for expired sessions every 30 seconds.
            if (CanPurge())
            {
                Task.Run(() => PurgeExpiredSessions());
            }
        }
        private void PurgeExpiredSessions()
        {
            try
            {
                s_sqlSessionStateRepository.DeleteExpiredSessions();
                s_lastSessionPurgeTicks = DateTime.UtcNow.Ticks;
            }
            catch
            {
                // Swallow all failures, this is called from an async Task and we don't want to crash
            }
            finally
            {
                Interlocked.CompareExchange(ref s_inPurge, 0, 1);
            }
        }
        private static ConnectionStringSettings GetConnectionString(string connectionstringName)
        {
            if (string.IsNullOrEmpty(connectionstringName))
            {
                throw new ProviderException(SqlResource.Connection_name_not_specified);
            }
            ConnectionStringSettings conn = ConfigurationManager.ConnectionStrings[connectionstringName];
            if (conn == null)
            {
                throw new ProviderException(
                    String.Format(CultureInfo.CurrentCulture, SqlResource.Connection_string_not_found, connectionstringName));
            }
            return conn;
        }
    }
}
