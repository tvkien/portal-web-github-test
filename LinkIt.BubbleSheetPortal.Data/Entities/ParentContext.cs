using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    internal partial class ParentDataContext
    {
        internal static ParentDataContext Get(string connectionString)
        {
            var context = new ParentDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new ParentDataContext(conn);
        }
    }
}
