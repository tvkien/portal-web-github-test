using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.InteractiveRubric.DataContext
{
    partial class RubricDataContext
    {
        internal static RubricDataContext Get(string connectionString)
        {
            var context = new RubricDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new RubricDataContext(conn);
        }
    }
}
