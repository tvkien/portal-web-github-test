using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.Helper
{
    public interface IBulkHelper
    {
        DataSet BulkCopy(string tempTableCreateScript, string tempTableName, IEnumerable<object> objectsToBeBulked, string finalizeProcedure, params object[] inputParams);
    }
}
