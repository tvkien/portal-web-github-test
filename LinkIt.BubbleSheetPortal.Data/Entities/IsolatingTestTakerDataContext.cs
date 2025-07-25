using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    internal partial class IsolatingTestTakerDataContext
    {
        internal static IsolatingTestTakerDataContext Get(string connectionString)
        {
            var context = new IsolatingTestTakerDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new IsolatingTestTakerDataContext(conn);
        }
    }
}
