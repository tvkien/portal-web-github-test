using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker
{
    public class GetMostRecentItemVersionsDto
    {
        public int QTIItemHistoryID { get; set; }
        public int QTIItemID { get; set; }
        public string ChangedDate { get; set; }
        public string XmlContent { get; set; }
        public int? AuthorID { get; set; }
        public string AuthorFullName { get; set; }
        public string RevertedFromDate { get; set; }
    }
}
