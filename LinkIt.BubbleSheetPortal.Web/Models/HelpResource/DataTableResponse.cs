using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Models.HelpResource
{
    public class DataTableResponse
    {
        public int sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public List<HelpResourceRow> aaData { get; set; }
        public string sColumns { get; set; }
    }
}