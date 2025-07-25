namespace GenericLog
{
    public interface IPortalLogRepository
    {
        void Insert(LogViewModel obj);
        void InsertUserLogoutLog(UserLogOutModel dto);
    }
}
