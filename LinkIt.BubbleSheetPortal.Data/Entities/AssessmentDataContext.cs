using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    public partial class AssessmentDataContext
    {
        internal static AssessmentDataContext Get(string connectionString)
        {
            var context = new APILogContextDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new AssessmentDataContext(conn);
        }
    }
}