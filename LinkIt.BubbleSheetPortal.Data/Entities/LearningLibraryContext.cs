using StackExchange.Profiling;
namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    partial class LearningLibraryDataContext
    {
        internal static LearningLibraryDataContext Get(string connectionString)
        {
            var context = new LearningLibraryDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new LearningLibraryDataContext(conn);
        }
    }
}
