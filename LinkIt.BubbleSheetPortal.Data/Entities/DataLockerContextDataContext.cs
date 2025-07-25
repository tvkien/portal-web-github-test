using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    public partial class DataLockerContextDataContext
    {
        internal static DataLockerContextDataContext Get(string connectionString)
        {
            var context = new DataLockerContextDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new DataLockerContextDataContext(conn);
        }
    }
}