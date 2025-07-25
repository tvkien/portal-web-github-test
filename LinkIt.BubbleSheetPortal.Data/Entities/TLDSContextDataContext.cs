using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    public partial class TLDSContextDataContext
    {
        internal static TLDSContextDataContext Get(string connectionString)
        {
            var context = new TLDSContextDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new TLDSContextDataContext(conn);
        }
    }
}
