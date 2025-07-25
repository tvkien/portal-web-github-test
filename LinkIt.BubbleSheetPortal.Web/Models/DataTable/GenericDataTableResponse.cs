using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Models.DataTable
{
    public class GenericDataTableResponse<T>
    {
        public int sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public List<T> aaData { get; set; }
        public string sColumns { get; set; }
    }
}