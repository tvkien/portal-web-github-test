using StackExchange.Profiling;
namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    public partial class RestrictionDataContext
    {
        internal static RestrictionDataContext Get(string connectionString)
        {
            var context = new RestrictionDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new RestrictionDataContext(conn);
        }
    }
}
