using LinkIt.BubbleSheetPortal.Models.SGO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.DynamoIsolating.Model
{
    public class SGOManagerLog
    {
        public string SGOManagerLogID { get; set; }
        public int UserID { get; set; }
        public int? SGOID { get; set; }
        public DateTime DynamoCreatedDate { get; set; }
        public string ActionName { get; set; }
        public string RequestData { get; set; }
        public string ResponseData { get; set; }
        public List<SGOLoggingData> LoggingDataBefore { get; set; }
        public List<SGOLoggingData> LogginDataAfter { get; set; }
        public long TTL { get; set; }
    }
}
