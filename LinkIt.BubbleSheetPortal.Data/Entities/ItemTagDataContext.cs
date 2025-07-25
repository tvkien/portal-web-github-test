using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    public partial class ItemTagDataContext
    {
        internal static ItemTagDataContext Get(string connectionString)
        {
            var context = new ItemTagDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new ItemTagDataContext(conn);
        }
    }
}