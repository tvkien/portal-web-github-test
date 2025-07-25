using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Profiling;

namespace LinkIt.BubbleSheetPortal.Data.Entities
{
    partial class ClassStudentEntity
    {
    }

    internal partial class StudentDataContext
    {
        internal static StudentDataContext Get(string connectionString)
        {
            var context = new StudentDataContext(connectionString);
            var conn = new StackExchange.Profiling.Data.ProfiledDbConnection(context.Connection, MiniProfiler.Current);
            return new StudentDataContext(conn);
        }
    }
}
