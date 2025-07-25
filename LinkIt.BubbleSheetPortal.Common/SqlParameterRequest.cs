using System.Collections.Generic;
using System.Data;

namespace LinkIt.BubbleSheetPortal.Common
{
    public class SqlParameterRequest
    {
        public string StoredName { get; set; }
        public IEnumerable<(string ParameterName,string TypeName, SqlDbType SqlDbType, object Value, ParameterDirection Direction)> Parameters { get; set; }
    }
}
