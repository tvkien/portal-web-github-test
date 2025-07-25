using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    internal partial class BubbleSheetDataContext
    {
        internal static BubbleSheetDataContext Get(string connectionString)
        {
            var context = new BubbleSheetDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new BubbleSheetDataContext(conn);
        }
    }
}
