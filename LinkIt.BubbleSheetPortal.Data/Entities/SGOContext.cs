using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    internal partial class SGODataContext
    {
        internal static SGODataContext Get(string connectionString)
        {
            var context = new SGODataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new SGODataContext(conn);
        }
    }
}
