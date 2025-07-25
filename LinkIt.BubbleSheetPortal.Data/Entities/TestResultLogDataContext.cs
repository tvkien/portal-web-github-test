using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    public partial class TestResultLogDataContext
    {
        internal static TestResultLogDataContext Get(string connectionString)
        {
            var context = new TestResultLogDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new TestResultLogDataContext(conn);
        }
    }
}
