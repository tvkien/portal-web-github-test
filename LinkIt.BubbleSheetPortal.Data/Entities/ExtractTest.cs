using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    internal partial class ExtractTestDataContext
    {
        internal static ExtractTestDataContext Get(string connectionString)
        {
            var context = new TestDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new ExtractTestDataContext(conn);
        }
    }
}
