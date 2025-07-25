using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    internal partial class TestAuditDataContext
    {
        internal static TestAuditDataContext Get(string connectionString)
        {
            var context = new TestDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new TestAuditDataContext(conn);
        }
    }
}
