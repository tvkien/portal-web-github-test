using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    public partial class SectionBranchingDataContext
    {
        internal static SectionBranchingDataContext Get(string connectionString)
        {
            var context = new SectionBranchingDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new SectionBranchingDataContext(conn);
        }
    }
}
