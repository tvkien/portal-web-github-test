namespace Linkit.AspNet.SessionState
{
    using System;
    using System.Threading.Tasks;
    using System.Web.SessionState;
    enum RepositoryType
    {
        SqlServer,
        InMemory,
        InMemoryDurable,
        FrameworkCompat
    }
    interface ISqlSessionStateRepository
    {
        void CreateSessionStateTable();
        void DeleteExpiredSessions();
        SessionItem GetSessionStateItem(string id, bool exclusive);
        void CreateOrUpdateSessionStateItem(bool newItem, string id, byte[] buf, int length, int timeout, int lockCookie, int orginalStreamLen);
        void ResetSessionItemTimeout(string id);
        void RemoveSessionItem(string id, object lockId);
        void ReleaseSessionItem(string id, object lockId);
        void CreateUninitializedSessionItem(string id, int length, byte[] buf, int timeout);
    }
    class SessionItem
    {
        public SessionItem(byte[] item, bool locked, TimeSpan lockAge, object lockId,
            SessionStateActions actions)
        {
            Item = item;
            Locked = locked;
            LockAge = lockAge;
            LockId = lockId;
            Actions = actions;
        }
        public byte[] Item { get; private set; }
        public bool Locked { get; private set; }
        public TimeSpan LockAge { get; private set; }
        public object LockId { get; private set; }
        public SessionStateActions Actions { get; private set; }
    }
}
