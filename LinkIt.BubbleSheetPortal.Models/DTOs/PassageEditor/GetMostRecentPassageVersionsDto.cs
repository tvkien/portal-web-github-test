using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.PassageEditor
{
    public class GetMostRecentPassageVersionsDto
    {
        public int QTIRefObjectHistoryId { get; set; }
        public int QTIRefObjectId { get; set; }
        public string ChangedDate { get; set; }
        public string XmlContent { get; set; }
        public int AuthorId { get; set; }
        public string AuthorFullName { get; set; }
        public string RevertedFromDate { get; set; }
    }
}
