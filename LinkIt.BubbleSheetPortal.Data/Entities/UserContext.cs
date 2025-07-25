using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    internal partial class UserDataContext
    {
        internal static UserDataContext Get(string connectionString)
        {
            var context = new UserDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new UserDataContext(conn);
        }
    }
}
