using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    internal partial class MultiPartQuestionContext
    {
        internal static MultiPartQuestionContext Get(string connectionString)
        {
            var context = new AlgorithmicContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new MultiPartQuestionContext(conn);
        }
    }
}
